using BusinessLogicLayer.Contract;
using BusinessLogicLayer.DTO;
using DataAccessLayer.Entities;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace OrdersMicroService.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ordersController(IOrdersService ordersService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetOrders()
    {
        var orders = await ordersService.GetOrdersAsync();
        return Ok(orders);
    }

    [HttpGet("search/orderid/{orderID}")]
    public async Task<IActionResult> GetOrderById(Guid orderID)
    {
        var order = await ordersService.GetOrderByConditionAsync(Builders<Order>.Filter.Eq(o => o.OrderID, orderID));
        return Ok(order);
    }

    [HttpGet("search/productid/{productID}")]
    public async Task<IActionResult> GetOrdersByProductId(Guid productID)
    {
        FilterDefinition<Order> filter = Builders<Order>.Filter.ElemMatch(temp => temp.OrderItems,
                                         Builders<OrderItem>.Filter.Eq(item => item.ProductID, productID));

        var orders = await ordersService.GetOrdersByConditionAsync(filter);
        return Ok(orders);
    }

    [HttpGet("search/orderdate/{orderDate}")]
    public async Task<IActionResult> GetOrdersByOrderDate(DateTime orderDate)
    {
        FilterDefinition<Order> filter = Builders<Order>.Filter.Eq(o => o.OrderDate.ToString("yyyy-MM-dd"),
                                                                          orderDate.ToString("yyyy-MM-dd"));
        var orders = await ordersService.GetOrdersByConditionAsync(filter);
        return Ok(orders);
    }

    [HttpPost]
    public async Task<IActionResult> AddOrder(OrderAddRequestDTO orderAddRequest)
    {
        var order = await ordersService.AddOrderAsync(orderAddRequest);
        return Ok(order);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateOrder(OrderUpdateRequestDTO orderUpdateRequest)
    {
        var order = await ordersService.UpdateOrderAsync(orderUpdateRequest);
        return Ok(order);
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteOrder(Guid orderID)
    {
        var result = await ordersService.DeleteOrderAsync(orderID);
        if (result)
        {
            return Ok(new { message = "Order deleted successfully." });
        }
        else
        {
            return NotFound(new { message = "Order not found." });
        }
    }
}
