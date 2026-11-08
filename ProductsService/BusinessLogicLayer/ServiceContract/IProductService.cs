using BusinessLogicLayer.DTO;
using DataAccessLayer.Entities;
using System.Linq.Expressions;

namespace BusinessLogicLayer.ServiceContract;
public interface IProductService
{
    Task<List<ProductResponseDTO?>> GetProductsAsync();
    Task<List<ProductResponseDTO?>> GetProductsByConditionAsync(Expression<Func<Product, bool>> condition);
    Task<ProductResponseDTO?> GetProductByConditionAsync(Expression<Func<Product, bool>> condition);
    Task<ProductResponseDTO?> AddProductAsync(ProductAddRequestDTO productAddRequest);
    Task<ProductResponseDTO?> UpdateProductAsync(ProductUpdateRequestDTO productUpdateRequest);
    Task<bool> DeleteProductAsync(Guid productId);
}
