using Microsoft.EntityFrameworkCore;
using DotnetCoreMigration.Data;
using DotnetCoreMigration.DTOs;
using DotnetCoreMigration.Interfaces;
using DotnetCoreMigration.Models;

namespace DotnetCoreMigration.Repositories;

public class OrderRepository : IRepository<Order, int>
{
    private readonly ApplicationDbContext _context;

    public OrderRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Order>> GetAll()
    {
        return await _context.Orders
            .Where(o => o.IsActive)
            .Include(o => o.User)
            .Include(o => o.OrderDetails.Where(od => od.IsActive))
                .ThenInclude(od => od.Product)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();
    }

    public async Task<PaginatedResponse<Order>> GetAllPaginated(PaginationRequest request)
    {
        var query = _context.Orders
            .Where(o => o.IsActive)
            .Include(o => o.User)
            .Include(o => o.OrderDetails.Where(od => od.IsActive))
                .ThenInclude(od => od.Product)
            .AsQueryable();

        // Apply search filter
        if (!string.IsNullOrEmpty(request.SearchTerm))
        {
            query = query.Where(o => 
                o.OrderId.ToString().Contains(request.SearchTerm) ||
                (o.User != null && o.User.UserName.ToLower().Contains(request.SearchTerm.ToLower())) ||
                (o.User != null && o.User.Email.ToLower().Contains(request.SearchTerm.ToLower())));
        }

        // Apply sorting
        if (!string.IsNullOrEmpty(request.SortBy))
        {
            switch (request.SortBy.ToLower())
            {
                case "orderdate":
                    query = request.SortDescending ? query.OrderByDescending(o => o.OrderDate) : query.OrderBy(o => o.OrderDate);
                    break;
                case "totalprice":
                case "totalamount":
                    query = request.SortDescending ? query.OrderByDescending(o => o.TotalAmount) : query.OrderBy(o => o.TotalAmount);
                    break;
                case "orderid":
                    query = request.SortDescending ? query.OrderByDescending(o => o.OrderId) : query.OrderBy(o => o.OrderId);
                    break;
                default:
                    query = query.OrderByDescending(o => o.OrderDate);
                    break;
            }
        }
        else
        {
            query = query.OrderByDescending(o => o.OrderDate);
        }

        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync();

        return new PaginatedResponse<Order>(items, totalCount, request.PageNumber, request.PageSize);
    }

    public async Task<Order?> GetById(int id)
    {
        return await _context.Orders
            .Where(o => o.IsActive)
            .Include(o => o.User)
            .Include(o => o.OrderDetails.Where(od => od.IsActive))
                .ThenInclude(od => od.Product)
            .FirstOrDefaultAsync(o => o.OrderId == id);
    }

    public async Task<Order> Create(Order order)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            // Store order details temporarily and clear from order
            var orderDetails = order.OrderDetails.ToList();
            order.OrderDetails.Clear();

            // Calculate total price for each order detail
            foreach (var detail in orderDetails)
            {
                detail.TotalPrice = detail.Quantity * detail.UnitPrice;
            }

            // Calculate and validate order total
            var calculatedTotal = orderDetails.Sum(od => od.TotalPrice);
            order.TotalAmount = calculatedTotal;

            // Add order first to get OrderId
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            // Now add order details with the correct OrderId
            foreach (var detail in orderDetails)
            {
                detail.OrderId = order.OrderId;
                _context.OrderDetails.Add(detail);
            }
            await _context.SaveChangesAsync();

            await transaction.CommitAsync();
            return await GetById(order.OrderId) ?? order;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<Order> Update(Order order)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var existingOrder = await _context.Orders
                .Include(o => o.OrderDetails)
                .FirstOrDefaultAsync(o => o.OrderId == order.OrderId);

            if (existingOrder == null)
                throw new InvalidOperationException("Order not found.");

            // Update order properties
            existingOrder.Status = order.Status;
            existingOrder.ShippingAddress = order.ShippingAddress;
            existingOrder.Notes = order.Notes;

            // Handle order details updates
            if (order.OrderDetails != null && order.OrderDetails.Any())
            {
                var existingDetails = existingOrder.OrderDetails.Where(od => od.IsActive).ToList();
                var newDetails = order.OrderDetails.ToList();

                // Update existing order details or mark for deletion
                foreach (var existingDetail in existingDetails)
                {
                    var matchingNew = newDetails.FirstOrDefault(nd => nd.ProductId == existingDetail.ProductId);
                    if (matchingNew != null)
                    {
                        // Update existing detail
                        existingDetail.Quantity = matchingNew.Quantity;
                        existingDetail.UnitPrice = matchingNew.UnitPrice;
                        existingDetail.TotalPrice = matchingNew.Quantity * matchingNew.UnitPrice;
                        _context.OrderDetails.Update(existingDetail);
                        
                        // Remove from new list as it's been handled
                        newDetails.Remove(matchingNew);
                    }
                    else
                    {
                        // Mark for soft deletion
                        existingDetail.IsActive = false;
                        _context.OrderDetails.Update(existingDetail);
                    }
                }

                // Add completely new order details
                foreach (var newDetail in newDetails)
                {
                    newDetail.OrderId = order.OrderId;
                    newDetail.TotalPrice = newDetail.Quantity * newDetail.UnitPrice;
                    _context.OrderDetails.Add(newDetail);
                }

                // Recalculate order total from all active details
                var allActiveDetails = await _context.OrderDetails
                    .Where(od => od.OrderId == order.OrderId && od.IsActive)
                    .ToListAsync();
                
                // Include the new details being added
                var totalFromNewDetails = newDetails.Sum(nd => nd.TotalPrice);
                var totalFromUpdatedDetails = existingOrder.OrderDetails
                    .Where(od => od.IsActive)
                    .Sum(od => od.TotalPrice);
                
                existingOrder.TotalAmount = totalFromUpdatedDetails + totalFromNewDetails;
            }
            else
            {
                // If no new order details provided, recalculate from existing active details
                var activeDetails = existingOrder.OrderDetails.Where(od => od.IsActive);
                existingOrder.TotalAmount = activeDetails.Sum(od => od.TotalPrice);
            }

            _context.Orders.Update(existingOrder);
            await _context.SaveChangesAsync();

            // Validate total amount
            var finalOrder = await _context.Orders
                .Include(o => o.OrderDetails.Where(od => od.IsActive))
                .FirstOrDefaultAsync(o => o.OrderId == order.OrderId);

            if (finalOrder != null)
            {
                var calculatedTotal = finalOrder.OrderDetails.Sum(od => od.TotalPrice);
                if (Math.Abs(finalOrder.TotalAmount - calculatedTotal) > 0.01m)
                {
                    throw new InvalidOperationException("Order total does not match sum of order details.");
                }
            }

            await transaction.CommitAsync();
            return await GetById(order.OrderId) ?? order;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<bool> Delete(int id)
    {
        var order = await _context.Orders
            .Include(o => o.OrderDetails)
            .FirstOrDefaultAsync(o => o.OrderId == id);
        
        if (order == null) return false;

        // Soft delete - set IsActive to false for order and all order details
        order.IsActive = false;
        foreach (var detail in order.OrderDetails)
        {
            detail.IsActive = false;
            _context.OrderDetails.Update(detail);
        }

        _context.Orders.Update(order);
        await _context.SaveChangesAsync();
        return true;
    }
}
