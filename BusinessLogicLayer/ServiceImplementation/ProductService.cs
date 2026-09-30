using AutoMapper;
using BusinessLogicLayer.DTO;
using BusinessLogicLayer.ServiceContract;
using DataAccessLayer.Contracts;
using DataAccessLayer.Entities;
using FluentValidation;
using System.Linq.Expressions;

namespace BusinessLogicLayer.ServiceImplementation;
public class ProductService(IProductRepository productRepository,
    IMapper mapper,
    IValidator<ProductAddRequestDTO> productAddRequestValidator,
    IValidator<ProductUpdateRequestDTO> productUpdateRequestValidator) : IProductService
{
    public async Task<ProductResponseDTO?> AddProductAsync(ProductAddRequestDTO productAddRequest)
    {
        if (productAddRequest == null)
        {
            throw new ArgumentNullException(nameof(productAddRequest));
        }
        var validationResult = await productAddRequestValidator.ValidateAsync(productAddRequest);

        if (!validationResult.IsValid)
        {
            var errors = string.Join("\n",validationResult.Errors.Select(e => e.ErrorMessage));
            throw new ArgumentException(errors);
        }
        var product = await productRepository.AddProduct(mapper.Map<Product>(productAddRequest));

        if (product == null)
        {
            return null;
        }
        return mapper.Map<ProductResponseDTO>(product);
    }

    public Task<bool> DeleteProductAsync(Guid productId)
    {
        throw new NotImplementedException();
    }

    public Task<ProductResponseDTO?> GetProductByConditionAsync(Expression<Func<Product, bool>> condition)
    {
        throw new NotImplementedException();
    }

    public Task<List<ProductResponseDTO?>> GetProductsAsync()
    {
        throw new NotImplementedException();
    }

    public Task<List<ProductResponseDTO?>> GetProductsByConditionAsync(Expression<Func<Product, bool>> condition)
    {
        throw new NotImplementedException();
    }

    public Task<ProductResponseDTO?> UpdateProductAsync(ProductAddRequestDTO productAddRequest)
    {
        throw new NotImplementedException();
    }
}