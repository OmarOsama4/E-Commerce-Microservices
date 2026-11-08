using AutoMapper;
using BusinessLogicLayer.Contract;
using BusinessLogicLayer.DTO;
using BusinessLogicLayer.HttpClients;
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
    IValidator<OrderUpdateRequestDTO> orderUpdateRequestValidator,
    UsersMicroserviceClient usersMicroserviceClient,
    ProductsMicroserviceClient productsMicroserviceClient) : IOrdersService
{
    public async Task<OrderResponseDTO?> AddOrderAsync(OrderAddRequestDTO orderAddRequest)
    {
        if (orderAddRequest == null)
            throw new ArgumentNullException(nameof(orderAddRequest));

        List<ProductDTO?> products = new List<ProductDTO?>();

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
            var product = await productsMicroserviceClient.GetProductByIdAsync(orderItem.ProductID);
            if (product == null)
            {
                throw new ArgumentException($"Product with ID {orderItem.ProductID} does not exist.");
            }
            products.Add(product);
        }
        var user = await usersMicroserviceClient.GetUserByUserId(orderAddRequest.UserID);
        if (user == null)
        {
            throw new ArgumentException($"User with ID {orderAddRequest.UserID} does not exist.");
        }
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

        var addedOrderResponse = mapper.Map<OrderResponseDTO>(addedOrder);
        if (addedOrderResponse != null)
        {
            mapper.Map<UserDTO, OrderResponseDTO>(user, addedOrderResponse);
            foreach (var orderItemResponse in addedOrderResponse.OrderItems)
            {
                ProductDTO? productDTO = products.Where(temp => temp.ProductID == orderItemResponse.ProductID).FirstOrDefault();

                if (productDTO == null)
                    continue;

                mapper.Map<ProductDTO, OrderItemResponseDTO>(productDTO, orderItemResponse);
            }
        }
        return addedOrderResponse;
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
        var responseOrder = mapper.Map<OrderResponseDTO?>(order);

        if (responseOrder != null)
        {
            var user = await usersMicroserviceClient.GetUserByUserId(responseOrder.UserID);
            if (user != null)
                mapper.Map<UserDTO, OrderResponseDTO>(user, responseOrder);

            foreach (var orderItem in responseOrder.OrderItems)
            {
                var product = await productsMicroserviceClient.GetProductByIdAsync(orderItem.ProductID);
                if (product == null)
                    continue;

                mapper.Map<ProductDTO, OrderItemResponseDTO>(product, orderItem);
            }
        }
        return responseOrder;
    }

    public async Task<List<OrderResponseDTO?>> GetOrdersAsync()
    {
        var orders = await ordersRepository.GetOrders();
        var responseOrders = mapper.Map<List<OrderResponseDTO?>>(orders);
        foreach (var responseOrder in responseOrders)
        {
            if (responseOrder == null)
                continue;
            var user = await usersMicroserviceClient.GetUserByUserId(responseOrder.UserID);
            if (user != null)
                mapper.Map<UserDTO, OrderResponseDTO>(user, responseOrder);

            foreach (var orderItem in responseOrder.OrderItems)
            {
                var product = await productsMicroserviceClient.GetProductByIdAsync(orderItem.ProductID);
                if (product == null)
                    continue;

                mapper.Map<ProductDTO, OrderItemResponseDTO>(product, orderItem);
            }
        }
        return responseOrders;
    }

    public async Task<List<OrderResponseDTO?>> GetOrdersByConditionAsync(FilterDefinition<Order> filter)
    {
        var orders = await ordersRepository.GetOrdersByCondition(filter);
        var responseOrders = mapper.Map<List<OrderResponseDTO?>>(orders);
        foreach (var responseOrder in responseOrders)
        {
            if (responseOrder == null)
                continue;
            var user = await usersMicroserviceClient.GetUserByUserId(responseOrder.UserID);
            if (user != null)
                mapper.Map<UserDTO, OrderResponseDTO>(user, responseOrder);
            foreach (var orderItem in responseOrder.OrderItems)
            {
                var product = await productsMicroserviceClient.GetProductByIdAsync(orderItem.ProductID);
                if (product == null)
                    continue;

                mapper.Map<ProductDTO, OrderItemResponseDTO>(product, orderItem);
            }
        }
        return responseOrders;
    }

    public async Task<OrderResponseDTO?> UpdateOrderAsync(OrderUpdateRequestDTO orderUpdateRequest)
    {
        if (orderUpdateRequest == null)
            throw new ArgumentNullException(nameof(orderUpdateRequest));
        List<ProductDTO> products = new List<ProductDTO>();

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
            var product = await productsMicroserviceClient.GetProductByIdAsync(orderItem.ProductID);
            if (product == null)
            {
                throw new ArgumentException($"Product with ID {orderItem.ProductID} does not exist.");
            }
        }
        var user = await usersMicroserviceClient.GetUserByUserId(orderUpdateRequest.UserID);
        if (user == null)
        {
            throw new ArgumentException($"User with ID {orderUpdateRequest.UserID} does not exist.");
        }
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

        var updatedOrderResponse = mapper.Map<OrderResponseDTO>(updatedOrder); //Map updatedOrder ('Order' type) into 'OrderResponse' type (it invokes OrderToOrderResponseMappingProfile).


        //TO DO: Load ProductName and Category in OrderItem
        if (updatedOrderResponse != null)
        {
            mapper.Map<UserDTO, OrderResponseDTO>(user, updatedOrderResponse);
            foreach (var orderItemResponse in updatedOrderResponse.OrderItems)
            {
                ProductDTO? productDTO = products.Where(temp => temp.ProductID == orderItemResponse.ProductID).FirstOrDefault();

                if (productDTO == null)
                    continue;

                mapper.Map<ProductDTO, OrderItemResponseDTO>(productDTO, orderItemResponse);
            }
        }

        return updatedOrderResponse;
    }
}
