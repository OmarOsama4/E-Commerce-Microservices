using Microsoft.Extensions.Logging;
using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;
using Polly.Timeout;

namespace BusinessLogicLayer.PollyPolicies;
public class UsersMicroservicePolicies(ILogger<UsersMicroservicePolicies> logger,
    IPollyPolicies pollyPolicies) : IUsersMicroservicePolicies
{
    public IAsyncPolicy<HttpResponseMessage> GetCombinedPolicy()
    {
        var retryPolicy = pollyPolicies.GetRetryPolicy(3);
        var circuitBreakerPolicy = pollyPolicies.GetCircuitBreakerPolicy(2, TimeSpan.FromSeconds(30));
        var timeoutPolicy = pollyPolicies.GetTimeoutPolicy(TimeSpan.FromSeconds(10));
        var combinedPolicy = Policy.WrapAsync(retryPolicy, circuitBreakerPolicy, timeoutPolicy);
        return combinedPolicy;
    }
}
