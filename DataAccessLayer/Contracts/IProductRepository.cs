using DataAccessLayer.Entities;
using System.Linq.Expressions;

namespace DataAccessLayer.Contracts;
public interface IProductRepository
{
    Task<IEnumerable<Product>> GetProductsAsync();
    Task<IEnumerable<Product?>> GetProductsByCondition(Expression<Func<Product, bool>> condition);
    Task<Product?> GetProductByCondition(Expression<Func<Product, bool>> condition);
    Task<Product?> AddProduct(Product product);
    Task<Product?> UpdateProduct(Product product);
    Task<bool> DeleteProduct(Guid productID);
}
