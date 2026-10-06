using BusinessLogicLayer.DTO;
using Microsoft.Extensions.Logging;
using Polly.CircuitBreaker;
using System.Net.Http.Json;

namespace BusinessLogicLayer.HttpClients;
public class UsersMicroserviceClient(HttpClient httpClient,
    ILogger<UsersMicroserviceClient> logger)
{
    public async Task<UserDTO?> GetUserByUserId(Guid userId)
    {
        try
        {
            var response = await httpClient.GetAsync($"/api/users/{userId}");
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
            return await response.Content.ReadFromJsonAsync<UserDTO>();
        }
        catch (BrokenCircuitException ex)
        {
            logger.LogError(ex, "Circuit breaker is open. Users microservice is unavailable.");
            return new UserDTO(Guid.Empty, "Unavailable right now", "Unavailable right now", "Unavailable right now");
        }
    }
}
