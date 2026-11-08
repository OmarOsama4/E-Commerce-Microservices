using DataAccessLayer.Context;
using DataAccessLayer.Contracts;
using DataAccessLayer.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DataAccessLayer.Repositories;
public class ProductRepository(ApplicationDbContext dbContext) : IProductRepository
{
    public async Task<Product?> AddProduct(Product product)
    {
        dbContext.Products.Add(product);
        await dbContext.SaveChangesAsync();
        return product;
    }

    public async Task<bool> DeleteProduct(Guid productID)
    {
        var product = await dbContext.Products.FirstOrDefaultAsync(p => p.ProductID == productID);
        if (product is null)
            return false;
        else
        {         
            dbContext.Products.Remove(product);
            await dbContext.SaveChangesAsync();
            return true;
        }
    }

    public async Task<Product?> GetProductByCondition(Expression<Func<Product, bool>> condition)
        => await dbContext.Products.FirstOrDefaultAsync(condition);

    public async Task<IEnumerable<Product>> GetProductsAsync()
        => await dbContext.Products.ToListAsync();

    public async Task<IEnumerable<Product?>> GetProductsByCondition(Expression<Func<Product, bool>> condition)
        => await dbContext.Products.Where(condition).ToListAsync();

    public async Task<Product?> UpdateProduct(Product product)
    {
        var existingproduct = await dbContext.Products.FirstOrDefaultAsync(p => p.ProductID == product.ProductID);
        if (existingproduct is null)
            return null;
        else
        {
            existingproduct.ProductName = product.ProductName;
            existingproduct.Category = product.Category;
            existingproduct.UnitPrice = product.UnitPrice;
            existingproduct.QuantityInStock = product.QuantityInStock;
            await dbContext.SaveChangesAsync();
            return existingproduct;
        }
    }
}
