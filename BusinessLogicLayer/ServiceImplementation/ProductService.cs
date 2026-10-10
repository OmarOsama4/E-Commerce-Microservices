using AutoMapper;
using BusinessLogicLayer.DTO;
using BusinessLogicLayer.RabbitMQ;
using BusinessLogicLayer.ServiceContract;
using DataAccessLayer.Contracts;
using DataAccessLayer.Entities;
using FluentValidation;
using System.Linq.Expressions;

namespace BusinessLogicLayer.ServiceImplementation;
public class ProductService(IProductRepository productRepository,
    IMapper mapper,
    IValidator<ProductAddRequestDTO> productAddRequestValidator,
    IValidator<ProductUpdateRequestDTO> productUpdateRequestValidator,
    IRabbitMQPublisher publisher) : IProductService
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

    public async Task<bool> DeleteProductAsync(Guid productId)
    {
        var isDeleted = await productRepository.DeleteProduct(productId);
        if (isDeleted)
        {
            await publisher.Publish("product.delete", new { ProductID = productId});
        }
        return isDeleted;
    }

    public async Task<ProductResponseDTO?> GetProductByConditionAsync(Expression<Func<Product, bool>> condition)
    {
        var product = await productRepository.GetProductByCondition(condition);
        if (product == null)
            return null;

        return mapper.Map<ProductResponseDTO>(product);
    }

    public async Task<List<ProductResponseDTO?>> GetProductsAsync()
    {
        var products = await productRepository.GetProductsAsync();
        return mapper.Map<List<ProductResponseDTO?>>(products);
    }

    public async Task<List<ProductResponseDTO?>> GetProductsByConditionAsync(Expression<Func<Product, bool>> condition)
    {
        var products = await productRepository.GetProductsByCondition(condition);
        return mapper.Map<List<ProductResponseDTO?>>(products);
    }

    public async Task<ProductResponseDTO?> UpdateProductAsync(ProductUpdateRequestDTO productUpdateRequest)
    {
        var existingProduct = await productRepository.GetProductByCondition(temp => temp.ProductID == productUpdateRequest.ProductID);
        if (existingProduct == null)
            throw new ArgumentException("Invalid Product ID");

        var validationResult = await productUpdateRequestValidator.ValidateAsync(productUpdateRequest);
        if (!validationResult.IsValid)
        {
            var errors = string.Join("\n", validationResult.Errors.Select(e => e.ErrorMessage));
            throw new ArgumentException(errors);
        }

        var isProductNameChanged = productUpdateRequest.ProductName != existingProduct.ProductName;

        var product = await productRepository.UpdateProduct(mapper.Map<Product>(productUpdateRequest));

        if (isProductNameChanged)
        {
            var message = new {ProductID = product!.ProductID, NewName = product.ProductName};
            await publisher.Publish("product.update.name", message);
        }

        return mapper.Map<ProductResponseDTO?>(product);
    }
}