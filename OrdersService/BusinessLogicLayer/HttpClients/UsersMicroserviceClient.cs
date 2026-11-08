using BusinessLogicLayer.DTO;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Polly.CircuitBreaker;
using Polly.Timeout;
using System.Net.Http.Json;
using System.Text.Json;

namespace BusinessLogicLayer.HttpClients;
public class UsersMicroserviceClient(HttpClient httpClient,
    ILogger<UsersMicroserviceClient> logger,
    IDistributedCache distributedCache)
{
    public async Task<UserDTO?> GetUserByUserId(Guid userId)
    {
        try
        {
            var cacheKey = $"User_{userId}";
            string? cacheValue = await distributedCache.GetStringAsync(cacheKey);

            if (cacheValue is not null)
                return JsonSerializer.Deserialize<UserDTO>(cacheValue);

            var response = await httpClient.GetAsync($"/gateway/users/{userId}");
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
                    return new UserDTO(Guid.Empty, "Unavailable right now", "Unavailable right now", "Unavailable right now");
                }
            }
            var user = await response.Content.ReadFromJsonAsync<UserDTO>();

            #region Caching the product in redis
            var cacheKeyToWrite = $"User_{userId}";
            var cacheUser = JsonSerializer.Serialize(user);
            var options = new DistributedCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(5))
                .SetSlidingExpiration(TimeSpan.FromMinutes(3));
            await distributedCache.SetStringAsync(cacheKeyToWrite, cacheUser, options); 
            #endregion

            return user;
        }

        catch (BrokenCircuitException ex)
        {
            logger.LogError(ex, "Circuit breaker is open. Users microservice is unavailable.");
            return new UserDTO(Guid.Empty, "Unavailable right now", "Unavailable right now", "Unavailable right now");
        }

        catch (TimeoutRejectedException ex)
        {
            logger.LogError(ex, "Request to users microservice timed out.");
            return new UserDTO(Guid.Empty, "Unavailable right now", "Unavailable right now", "Unavailable right now");
        }
    }
}
