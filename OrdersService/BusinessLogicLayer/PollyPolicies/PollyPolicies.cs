using Microsoft.Extensions.Logging;
using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;
using Polly.Timeout;

namespace BusinessLogicLayer.PollyPolicies;
public class PollyPolicies(ILogger<PollyPolicies> logger) : IPollyPolicies
{
    public IAsyncPolicy<HttpResponseMessage> GetRetryPolicy(int retryCount)
    {
        AsyncRetryPolicy<HttpResponseMessage> policy = Policy.HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
        .WaitAndRetryAsync(
            retryCount: retryCount,
            sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
            onRetry: (outcome, timespan, retryAttempts, context) =>
            {
                //TO DO: Log the retry attempt and the reason for the failure
                logger.LogWarning($"Retry attempt {retryAttempts} after {timespan.TotalSeconds} seconds due to: {outcome.Result?.StatusCode}");
            });
        return policy;
    }

    public IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy(int handledEventsAllowedBeforeBreaking, TimeSpan durationOfBreak)
    {
        AsyncCircuitBreakerPolicy<HttpResponseMessage> policy = Policy.HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
            .CircuitBreakerAsync(
            handledEventsAllowedBeforeBreaking: handledEventsAllowedBeforeBreaking,
            durationOfBreak: durationOfBreak,
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


    public IAsyncPolicy<HttpResponseMessage> GetTimeoutPolicy(TimeSpan timeout)
    {
        AsyncTimeoutPolicy<HttpResponseMessage> policy = Policy.TimeoutAsync<HttpResponseMessage>(timeout);
        return policy;
    }
}
