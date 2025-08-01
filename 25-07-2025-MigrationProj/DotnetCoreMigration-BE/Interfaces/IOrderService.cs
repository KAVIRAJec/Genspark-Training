using DotnetCoreMigration.DTOs;

namespace DotnetCoreMigration.Interfaces;

public interface IOrderService
{
    Task<IEnumerable<OrderDto>> GetAllOrdersAsync();
    Task<PaginatedResponse<OrderDto>> GetAllOrdersPaginatedAsync(PaginationRequest request);
    Task<OrderDto?> GetOrderByIdAsync(int id);
    Task<IEnumerable<OrderDto>> GetOrdersByUserIdAsync(int userId);
    Task<IEnumerable<OrderDto>> GetOrdersByStatusAsync(string status);
    Task<OrderDto> CreateOrderAsync(CreateOrderDto createOrderDto);
    Task<OrderDto> UpdateOrderAsync(UpdateOrderDto updateOrderDto);
    Task<OrderDto> UpdateOrderStatusAsync(UpdateOrderStatusDto updateOrderStatusDto);
    Task<bool> DeleteOrderAsync(int id);
    Task<bool> OrderExistsAsync(int id);
}
