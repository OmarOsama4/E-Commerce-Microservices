using AutoMapper;
using BusinessLogicLayer.Contract;
using BusinessLogicLayer.DTO;
using DataAccessLayer.Contracts;
using DataAccessLayer.Entities;
using FluentValidation;
using MongoDB.Driver;

namespace BusinessLogicLayer.ServiceImplementation;
public class OrdersService(IOrdersRepository ordersRepository,
    IMapper mapper,
    IValidator<OrderAddRequestDTO> orderAddRequestValidator, 
    IValidator<OrderItemAddRequestDTO> orderItemAddRequestValidator,
    IValidator<OrderItemUpdateRequestDTO> orderItemUpdateRequestValidator,
    IValidator<OrderUpdateRequestDTO> orderUpdateRequestValidator) : IOrdersService
{
    public async Task<OrderResponseDTO?> AddOrderAsync(OrderAddRequestDTO orderAddRequest)
    {
        if (orderAddRequest == null)
            throw new ArgumentNullException(nameof(orderAddRequest));

        #region Validate orderAddRequest and its orderItems
        var validationResult = await orderAddRequestValidator.ValidateAsync(orderAddRequest);
        if (!validationResult.IsValid)
        {
            string errorMessages = string.Join("\n ", validationResult.Errors.Select(e => e.ErrorMessage));
            throw new ArgumentException(errorMessages);
        }
        foreach (var orderItem in orderAddRequest.OrderItems)
        {
            var itemValidationResult = await orderItemAddRequestValidator.ValidateAsync(orderItem);
            if (!itemValidationResult.IsValid)
            {
                string errorMessages = string.Join("\n ", itemValidationResult.Errors.Select(e => e.ErrorMessage));
                throw new ArgumentException(errorMessages);
            }
        }
        //TODO: Check if the user exists in the database before proceeding with the order creation.
        #endregion
        
        var order = mapper.Map<Order>(orderAddRequest);

        #region Calculate the total bill of the order
        foreach (var orderItem in order.OrderItems)
        {
            orderItem.TotalPrice = orderItem.Quantity * orderItem.UnitPrice;
        }
        order.TotalBill = order.OrderItems.Sum(item => item.TotalPrice); 
        #endregion

        var addedOrder = await ordersRepository.AddOrder(order);
        if (addedOrder == null)
            return null;
        return mapper.Map<OrderResponseDTO>(addedOrder);
    }

    public async Task<bool> DeleteOrderAsync(Guid OrderID)
    {
        return await ordersRepository.DeleteOrder(OrderID);
    }

    public async Task<OrderResponseDTO?> GetOrderByConditionAsync(FilterDefinition<Order> filter)
    {
        var order = await ordersRepository.GetOrderByCondition(filter);
        if (order == null)
            return null;
        return mapper.Map<OrderResponseDTO>(order);
    }

    public async Task<List<OrderResponseDTO?>> GetOrdersAsync()
    {
        var orders = await ordersRepository.GetOrders();
        return mapper.Map<List<OrderResponseDTO?>>(orders);
    }

    public async Task<List<OrderResponseDTO?>> GetOrdersByConditionAsync(FilterDefinition<Order> filter)
    {
        var orders = await ordersRepository.GetOrdersByCondition(filter);
        return mapper.Map<List<OrderResponseDTO?>>(orders);
    }

    public async Task<OrderResponseDTO?> UpdateOrderAsync(OrderUpdateRequestDTO orderUpdateRequest)
    {
        if (orderUpdateRequest == null)
            throw new ArgumentNullException(nameof(orderUpdateRequest));

        #region Validate orderUpdateRequest and its orderItems
        var validationResult = await orderUpdateRequestValidator.ValidateAsync(orderUpdateRequest);
        if (!validationResult.IsValid)
        {
            string errorMessages = string.Join("\n ", validationResult.Errors.Select(e => e.ErrorMessage));
            throw new ArgumentException(errorMessages);
        }
        foreach (var orderItem in orderUpdateRequest.OrderItems)
        {
            var itemValidationResult = await orderItemUpdateRequestValidator.ValidateAsync(orderItem);
            if (!itemValidationResult.IsValid)
            {
                string errorMessages = string.Join("\n ", itemValidationResult.Errors.Select(e => e.ErrorMessage));
                throw new ArgumentException(errorMessages);
            }
        }
        //TODO: Check if the user exists in the database before proceeding with the order creation.
        #endregion

        var order = mapper.Map<Order>(orderUpdateRequest);

        #region Calculate the total bill of the order
        foreach (var orderItem in order.OrderItems)
        {
            orderItem.TotalPrice = orderItem.Quantity * orderItem.UnitPrice;
        }
        order.TotalBill = order.OrderItems.Sum(item => item.TotalPrice);
        #endregion

        var updatedOrder = await ordersRepository.UpdateOrder(order);
        if (updatedOrder == null)
            return null;

        return mapper.Map<OrderResponseDTO>(updatedOrder);
    }
}
