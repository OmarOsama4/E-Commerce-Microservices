using Microsoft.Extensions.Logging;
using Polly;
using Polly.CircuitBreaker;
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

    public IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
    {
        AsyncCircuitBreakerPolicy<HttpResponseMessage> policy = Policy.HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
            .CircuitBreakerAsync(handledEventsAllowedBeforeBreaking: 3, durationOfBreak: TimeSpan.FromMinutes(1),
            onBreak: (outcome, timespan) =>
            {
                //TO DO: Log the circuit breaker state change
                logger.LogWarning($"Circuit breaker opened for {timespan.TotalMinutes} minutes due to: {outcome.Result?.StatusCode}");
            },
            onReset: () =>
            {
                //TO DO: Log the circuit breaker state change
                logger.LogInformation("Circuit breaker reset.");
            });
        return policy;
    }
}
