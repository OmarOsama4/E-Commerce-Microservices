using BusinessLogicLayer.DTO;
using DataAccessLayer.Entities;
using MongoDB.Driver;

namespace BusinessLogicLayer.Contract;
public interface IOrdersService
{
    Task<List<OrderResponseDTO?>> GetOrdersAsync();
    Task<List<OrderResponseDTO?>> GetOrdersByConditionAsync(FilterDefinition<Order> filter);
    Task<OrderResponseDTO?> GetOrderByConditionAsync(FilterDefinition<Order> filter);
    Task<OrderResponseDTO?> AddOrderAsync(OrderAddRequestDTO orderAddRequest);
    Task<OrderResponseDTO?> UpdateOrderAsync(OrderUpdateRequestDTO orderUpdateRequest);
    Task<bool> DeleteOrderAsync(Guid OrderID);
}
