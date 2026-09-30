using BusinessLogicLayer.DTO;
using BusinessLogicLayer.ServiceContract;

namespace ProductsMicroService.API.APIEndPoints;
public static class ProductAPIEndPoints
{
    public static IEndpointRouteBuilder MapProductAPIEndPoints(this IEndpointRouteBuilder app)
    {
        //Get Products Endpoint
        app.MapGet("/api/products", async (IProductService productService) =>
        {
            var products = await productService.GetProductsAsync();
            return Results.Ok(products);
        });

        //Get Product By Condition Endpoint
        app.MapGet("/api/products/search/product-id/{ProductID:guid}", async (IProductService productService, Guid ProductID) =>
        {
            var product = await productService.GetProductByConditionAsync(p => p.ProductID == ProductID);
            return Results.Ok(product);
        });

        //Get Products By Condition Endpoint
        app.MapGet("/api/products/search/{searchstring}", async (IProductService productService, string searchstring) => 
        {
            var productsByName = await productService.GetProductsByConditionAsync(p => p.ProductName != null &&
                                                        p.ProductName.Contains(searchstring, StringComparison.OrdinalIgnoreCase));

            var productsByCategory = await productService.GetProductsByConditionAsync(p => p.Category != null &&
                                                        p.ProductName.Contains(searchstring, StringComparison.OrdinalIgnoreCase));

            var products = productsByName.Union(productsByCategory);
            return Results.Ok(products);
        });

        //Add Product Endpoint
        app.MapPost("/api/products", async (IProductService productService, ProductAddRequestDTO productAddRequest) =>
        {
            var product = await productService.AddProductAsync(productAddRequest);
            return Results.Ok(product);
        });

        //Update Product Endpoint
        app.MapPut("/api/products", async (IProductService productService, ProductUpdateRequestDTO productUpdateRequest) =>
        {
            var product = await productService.UpdateProductAsync(productUpdateRequest);
            return Results.Ok(product);
        });

        //Delete Product Endpoint
        app.MapDelete("/api/products/{ProductID:guid}", async (IProductService productService, Guid ProductID) =>
        {
            var result = await productService.DeleteProductAsync(ProductID);
            if (result)
            {
                return Results.Ok($"Product with ID {ProductID} deleted successfully.");
            }
            else
            {
                return Results.NotFound($"Product with ID {ProductID} not found.");
            }
        });

        return app;
    }
}
