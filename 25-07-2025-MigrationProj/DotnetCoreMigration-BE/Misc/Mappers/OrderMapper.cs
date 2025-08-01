using DotnetCoreMigration.DTOs;
using DotnetCoreMigration.Models;

namespace DotnetCoreMigration.Misc.Mappers;

public static class OrderMapper
{
    public static OrderDto ToDto(Order order)
    {
        return new OrderDto
        {
            OrderId = order.OrderId,
            UserId = order.UserId,
            UserName = order.User?.UserName ?? "Unknown",
            OrderDate = order.OrderDate,
            TotalAmount = order.TotalAmount,
            Status = order.Status,
            ShippingAddress = order.ShippingAddress,
            Notes = order.Notes,
            IsActive = order.IsActive,
            PaypalPaymentId = order.PaypalPaymentId,
            OrderDetails = order.OrderDetails?.Select(ToDetailDto).ToList() ?? new List<OrderDetailDto>()
        };
    }

    public static OrderDetailDto ToDetailDto(OrderDetail orderDetail)
    {
        return new OrderDetailDto
        {
            OrderDetailId = orderDetail.OrderDetailId,
            OrderId = orderDetail.OrderId,
            ProductId = orderDetail.ProductId,
            ProductName = orderDetail.Product?.ProductName ?? "Unknown",
            Quantity = orderDetail.Quantity,
            UnitPrice = orderDetail.UnitPrice,
            TotalPrice = orderDetail.TotalPrice,
            IsActive = orderDetail.IsActive
        };
    }

    public static Order ToEntity(CreateOrderDto createDto)
    {
        var order = new Order
        {
            UserId = createDto.UserId,
            OrderDate = DateTime.UtcNow,
            Status = "Pending",
            ShippingAddress = createDto.ShippingAddress,
            Notes = createDto.Notes,
            IsActive = true,
            PaypalPaymentId = createDto.PaypalPaymentId,
            OrderDetails = createDto.OrderDetails.Select(ToDetailEntity).ToList()
        };

        // Calculate total amount
        order.TotalAmount = order.OrderDetails.Sum(od => od.Quantity * od.UnitPrice);

        return order;
    }

    public static OrderDetail ToDetailEntity(CreateOrderDetailDto createDetailDto)
    {
        return new OrderDetail
        {
            ProductId = createDetailDto.ProductId,
            Quantity = createDetailDto.Quantity,
            UnitPrice = createDetailDto.UnitPrice,
            TotalPrice = createDetailDto.Quantity * createDetailDto.UnitPrice,
            IsActive = true
        };
    }

    public static void UpdateEntity(Order entity, UpdateOrderDto updateDto)
    {
        entity.Status = updateDto.Status;
        entity.ShippingAddress = updateDto.ShippingAddress;
        entity.Notes = updateDto.Notes;
    }

    public static void UpdateStatusEntity(Order entity, UpdateOrderStatusDto updateDto)
    {
        entity.Status = updateDto.Status;
    }

    public static IEnumerable<OrderDto> ToDtoList(IEnumerable<Order> orders)
    {
        return orders.Select(ToDto);
    }
}
