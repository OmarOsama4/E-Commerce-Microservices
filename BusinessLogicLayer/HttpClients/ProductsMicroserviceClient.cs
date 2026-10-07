using BusinessLogicLayer.DTO;
using Microsoft.Extensions.Caching.Distributed;
using System.Net.Http.Json;
using System.Text.Json;

namespace BusinessLogicLayer.HttpClients;
public class ProductsMicroserviceClient(HttpClient httpClient, IDistributedCache distributedCache)
{
    public async Task<ProductDTO?> GetProductByIdAsync(Guid productID)
    {
        string cacheKey = $"Product_{productID}";
        string? cacheProduct = await distributedCache.GetStringAsync(cacheKey);
        if (cacheProduct != null)
        {
            return JsonSerializer.Deserialize<ProductDTO>(cacheProduct);
        }

        var response = await httpClient.GetAsync($"/api/products/search/product-id/{productID}");
        if (!response.IsSuccessStatusCode)
        {
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null; 
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                throw new HttpRequestException("Bad request", null, System.Net.HttpStatusCode.BadRequest);
            }
            else
            {
                throw new HttpRequestException($"Http request failed with status code {response.StatusCode}");
            }
        }
        var product = await response.Content.ReadFromJsonAsync<ProductDTO>();

        #region Caching the product in redis
        var productJson = JsonSerializer.Serialize(product);
        var cacheKeyToWrite = $"product_{productID}";

        DistributedCacheEntryOptions options = new DistributedCacheEntryOptions()
            .SetAbsoluteExpiration(TimeSpan.FromSeconds(30))
            .SetSlidingExpiration(TimeSpan.FromSeconds(10));

        await distributedCache.SetStringAsync(cacheKeyToWrite, productJson, options); 
        #endregion

        return product;
    }
}
