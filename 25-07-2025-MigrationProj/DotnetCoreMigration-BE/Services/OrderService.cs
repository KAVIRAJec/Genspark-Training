using DotnetCoreMigration.DTOs;
using DotnetCoreMigration.Interfaces;
using DotnetCoreMigration.Models;
using DotnetCoreMigration.Misc.Mappers;

namespace DotnetCoreMigration.Services;

public class OrderService : IOrderService
{
    private readonly IRepository<Order, int> _orderRepository;

    public OrderService(IRepository<Order, int> orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<IEnumerable<OrderDto>> GetAllOrdersAsync()
    {
        var orders = await _orderRepository.GetAll();
        return OrderMapper.ToDtoList(orders);
    }

    public async Task<PaginatedResponse<OrderDto>> GetAllOrdersPaginatedAsync(PaginationRequest request)
    {
        var paginatedOrders = await _orderRepository.GetAllPaginated(request);
        var orderDtos = OrderMapper.ToDtoList(paginatedOrders.Data);

        return new PaginatedResponse<OrderDto>(
            orderDtos.ToList(),
            paginatedOrders.TotalCount,
            paginatedOrders.PageNumber,
            paginatedOrders.PageSize
        );
    }

    public async Task<OrderDto?> GetOrderByIdAsync(int id)
    {
        var order = await _orderRepository.GetById(id);
        return order == null ? null : OrderMapper.ToDto(order);
    }

    public async Task<IEnumerable<OrderDto>> GetOrdersByUserIdAsync(int userId)
    {
        var orders = await _orderRepository.GetAll();
        var userOrders = orders.Where(o => o.UserId == userId);
        return OrderMapper.ToDtoList(userOrders);
    }

    public async Task<IEnumerable<OrderDto>> GetOrdersByStatusAsync(string status)
    {
        var orders = await _orderRepository.GetAll();
        var statusOrders = orders.Where(o => o.Status.ToLower() == status.ToLower());
        return OrderMapper.ToDtoList(statusOrders);
    }

    public async Task<OrderDto> CreateOrderAsync(CreateOrderDto createOrderDto)
    {
        var order = OrderMapper.ToEntity(createOrderDto);
        var createdOrder = await _orderRepository.Create(order);
        return OrderMapper.ToDto(createdOrder);
    }

    public async Task<OrderDto> UpdateOrderAsync(UpdateOrderDto updateOrderDto)
    {
        var existingOrder = await _orderRepository.GetById(updateOrderDto.OrderId);
        if (existingOrder == null)
        {
            throw new InvalidOperationException("Order not found.");
        }

        OrderMapper.UpdateEntity(existingOrder, updateOrderDto);
        var updatedOrder = await _orderRepository.Update(existingOrder);
        return OrderMapper.ToDto(updatedOrder);
    }

    public async Task<OrderDto> UpdateOrderStatusAsync(UpdateOrderStatusDto updateOrderStatusDto)
    {
        var existingOrder = await _orderRepository.GetById(updateOrderStatusDto.OrderId);
        if (existingOrder == null)
        {
            throw new InvalidOperationException("Order not found.");
        }

        OrderMapper.UpdateStatusEntity(existingOrder, updateOrderStatusDto);
        var updatedOrder = await _orderRepository.Update(existingOrder);
        return OrderMapper.ToDto(updatedOrder);
    }

    public async Task<bool> DeleteOrderAsync(int id)
    {
        return await _orderRepository.Delete(id);
    }

    public async Task<bool> OrderExistsAsync(int id)
    {
        var order = await _orderRepository.GetById(id);
        return order != null;
    }
}
