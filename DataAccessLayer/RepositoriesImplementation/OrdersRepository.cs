using DataAccessLayer.Contracts;
using DataAccessLayer.Entities;
using MongoDB.Driver;

namespace DataAccessLayer.RepositoriesImplementation;
public class OrdersRepository : IOrdersRepository
{
    private readonly IMongoCollection<Order> _ordersCollection;
    private readonly string _collectionName = "orders";
    public OrdersRepository(IMongoDatabase database)
    {
        _ordersCollection = database.GetCollection<Order>(_collectionName);
    }

    public async Task<Order?> AddOrder(Order order)
    {
        order.OrderID = Guid.NewGuid();
        order._id = order.OrderID;

        foreach (var item in order.OrderItems)
        {
            item._id = Guid.NewGuid();
        }

        await _ordersCollection.InsertOneAsync(order);
        return order;
    }

    public async Task<bool> DeleteOrder(Guid orderID)
    {
        FilterDefinition<Order> filter = Builders<Order>.Filter.Eq(o => o.OrderID, orderID);
        var deletedResult = await _ordersCollection.DeleteOneAsync(filter);
        return deletedResult.DeletedCount>0 ;
    }

    public async Task<Order?> GetOrderByCondition(FilterDefinition<Order> filter)
    {
        return (await _ordersCollection.FindAsync(filter)).FirstOrDefault();
    }

    public async Task<IEnumerable<Order>> GetOrders()
    {
        FilterDefinition<Order> filter = Builders<Order>.Filter.Empty;
        return (await _ordersCollection.FindAsync(filter)).ToList();
    }

    public async Task<IEnumerable<Order?>> GetOrdersByCondition(FilterDefinition<Order> filter)
    {
        return (await _ordersCollection.FindAsync(filter)).ToList();
    }

    public async Task<Order?> UpdateOrder(Order order)
    {
        FilterDefinition<Order> filter = Builders<Order>.Filter.Eq(o => o.OrderID, order.OrderID);
        return await _ordersCollection.FindOneAndReplaceAsync(filter, order, 
            new FindOneAndReplaceOptions<Order>
            {
                ReturnDocument = ReturnDocument.After
            });
    }
}