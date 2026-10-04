using BusinessLogicLayer.DTO;
using System.Net.Http.Json;

namespace BusinessLogicLayer.HttpClients;
public class UsersMicroserviceClient(HttpClient httpClient)
{
    public async Task<UserDTO?> GetUserByUserId(Guid userId)
    {
        var response = await httpClient.GetAsync($"api/users/{userId}");
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
        return await response.Content.ReadFromJsonAsync<UserDTO>();
    }
}
