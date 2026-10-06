using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;

namespace BusinessLogicLayer.PollyPolicies;
public class UsersMicroservicePolicies(ILogger<UsersMicroservicePolicies> logger) : IUsersMicroservicePolicies
{
    public IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
    {
        AsyncRetryPolicy<HttpResponseMessage> policy =  Policy.HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
        .WaitAndRetryAsync(retryCount: 5, sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2,retryAttempt)),
        onRetry: (outcome, timespan, retryAttempts, context) =>
        {
            //TO DO: Log the retry attempt and the reason for the failure
            logger.LogWarning($"Retry attempt {retryAttempts} after {timespan.TotalSeconds} seconds due to: {outcome.Result?.StatusCode}");
        });

        return policy;
    }
}
