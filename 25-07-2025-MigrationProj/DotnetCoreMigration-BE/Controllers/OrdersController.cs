using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using DotnetCoreMigration.DTOs;
using DotnetCoreMigration.Interfaces;
using DotnetCoreMigration.Extensions;

namespace DotnetCoreMigration.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    /// <summary>
    /// Get all orders (Admin only)
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<IEnumerable<OrderDto>>>> GetAllOrders()
    {
        try
        {
            var orders = await _orderService.GetAllOrdersAsync();
            return Ok(ApiResponse<IEnumerable<OrderDto>>.SuccessResponse(orders, "Orders retrieved successfully"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<IEnumerable<OrderDto>>.ErrorResponse("An error occurred while retrieving orders", ex.Message));
        }
    }

    /// <summary>
    /// Get all orders with pagination (Admin only)
    /// </summary>
    [HttpGet("paginated")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<PaginatedResponse<OrderDto>>>> GetAllOrdersPaginated([FromQuery] PaginationRequest request)
    {
        try
        {
            var paginatedOrders = await _orderService.GetAllOrdersPaginatedAsync(request);
            return Ok(ApiResponse<PaginatedResponse<OrderDto>>.SuccessResponse(paginatedOrders, "Paginated orders retrieved successfully"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<PaginatedResponse<OrderDto>>.ErrorResponse("An error occurred while retrieving orders", ex.Message));
        }
    }

    /// <summary>
    /// Get order by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<OrderDto>>> GetOrderById(int id)
    {
        try
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null)
            {
                return NotFound(ApiResponse<OrderDto>.ErrorResponse("Order not found", $"No order found with ID {id}"));
            }

            var userRole = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
            var userIdClaim = User.FindFirst("UserId")?.Value;
            int.TryParse(userIdClaim, out int userId);

            if (userRole != "Admin" && order.UserId != userId)
            {
                return Forbid();
            }

            return Ok(ApiResponse<OrderDto>.SuccessResponse(order, "Order retrieved successfully"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<OrderDto>.ErrorResponse("An error occurred while retrieving the order", ex.Message));
        }
    }

    /// <summary>
    /// Get orders by user ID
    /// </summary>
    [HttpGet("user/{userId}")]
    public async Task<ActionResult<ApiResponse<IEnumerable<OrderDto>>>> GetOrdersByUserId(int userId)
    {
        try
        {
            var userRole = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
            var userIdClaim = User.FindFirst("UserId")?.Value;
            int.TryParse(userIdClaim, out int currentUserId);

            if (userRole != "Admin" && userId != currentUserId)
            {
                return Forbid();
            }

            var orders = await _orderService.GetOrdersByUserIdAsync(userId);
            return Ok(ApiResponse<IEnumerable<OrderDto>>.SuccessResponse(orders, "User orders retrieved successfully"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<IEnumerable<OrderDto>>.ErrorResponse("An error occurred while retrieving user orders", ex.Message));
        }
    }

    /// <summary>
    /// Get orders by status
    /// </summary>
    [HttpGet("status/{status}")]
    public async Task<ActionResult<ApiResponse<IEnumerable<OrderDto>>>> GetOrdersByStatus(string status)
    {
        try
        {
            var orders = await _orderService.GetOrdersByStatusAsync(status);
            return Ok(ApiResponse<IEnumerable<OrderDto>>.SuccessResponse(orders, "Orders by status retrieved successfully"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<IEnumerable<OrderDto>>.ErrorResponse("An error occurred while retrieving orders by status", ex.Message));
        }
    }

    /// <summary>
    /// Create a new order
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<OrderDto>>> CreateOrder([FromBody] CreateOrderDto createOrderDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<OrderDto>.ErrorResponse("Validation failed", ModelState.GetErrorMessage()));
            }

            var order = await _orderService.CreateOrderAsync(createOrderDto);
            return CreatedAtAction(nameof(GetOrderById), new { id = order.OrderId }, ApiResponse<OrderDto>.SuccessResponse(order, "Order created successfully"));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<OrderDto>.ErrorResponse("Order creation failed", ex.Message));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<OrderDto>.ErrorResponse("An error occurred while creating the order", ex.Message));
        }
    }

    /// <summary>
    /// Update an existing order
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<OrderDto>>> UpdateOrder(int id, [FromBody] UpdateOrderDto updateOrderDto)
    {
        try
        {
            if (id != updateOrderDto.OrderId)
            {
                return BadRequest(ApiResponse<OrderDto>.ErrorResponse("Validation failed", "ID in URL does not match ID in request body"));
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<OrderDto>.ErrorResponse("Validation failed", ModelState.GetErrorMessage()));
            }

            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null)
            {
                return NotFound(ApiResponse<OrderDto>.ErrorResponse("Order not found", $"No order found with ID {id}"));
            }

            var userRole = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
            var userIdClaim = User.FindFirst("UserId")?.Value;
            int.TryParse(userIdClaim, out int userId);

            if (userRole != "Admin" && order.UserId != userId)
            {
                return Forbid();
            }

            var updatedOrder = await _orderService.UpdateOrderAsync(updateOrderDto);
            return Ok(ApiResponse<OrderDto>.SuccessResponse(updatedOrder, "Order updated successfully"));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<OrderDto>.ErrorResponse("Order update failed", ex.Message));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<OrderDto>.ErrorResponse("An error occurred while updating the order", ex.Message));
        }
    }

    /// <summary>
    /// Update order status
    /// </summary>
    [HttpPatch("{id}/status")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<OrderDto>>> UpdateOrderStatus(int id, [FromBody] UpdateOrderStatusDto updateOrderStatusDto)
    {
        try
        {
            if (id != updateOrderStatusDto.OrderId)
            {
                return BadRequest(ApiResponse<OrderDto>.ErrorResponse("Validation failed", "ID in URL does not match ID in request body"));
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<OrderDto>.ErrorResponse("Validation failed", ModelState.GetErrorMessage()));
            }

            var exists = await _orderService.OrderExistsAsync(id);
            if (!exists)
            {
                return NotFound(ApiResponse<OrderDto>.ErrorResponse("Order not found", $"No order found with ID {id}"));
            }

            var order = await _orderService.UpdateOrderStatusAsync(updateOrderStatusDto);
            return Ok(ApiResponse<OrderDto>.SuccessResponse(order, "Order status updated successfully"));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<OrderDto>.ErrorResponse("Order status update failed", ex.Message));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<OrderDto>.ErrorResponse("An error occurred while updating the order status", ex.Message));
        }
    }

    /// <summary>
    /// Delete an order (soft delete)
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteOrder(int id)
    {
        try
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null)
            {
                return NotFound(ApiResponse<bool>.ErrorResponse("Order not found", $"No order found with ID {id}"));
            }

            var userRole = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
            var userIdClaim = User.FindFirst("UserId")?.Value;
            int.TryParse(userIdClaim, out int userId);

            if (userRole != "Admin" && order.UserId != userId)
            {
                return Forbid();
            }

            var result = await _orderService.DeleteOrderAsync(id);
            return Ok(ApiResponse<bool>.SuccessResponse(result, "Order deleted successfully"));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<bool>.ErrorResponse("Order deletion failed", ex.Message));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<bool>.ErrorResponse("An error occurred while deleting the order", ex.Message));
        }
    }
}