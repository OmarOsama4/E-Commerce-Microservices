using Polly;

namespace BusinessLogicLayer.PollyPolicies;
public interface IUsersMicroservicePolicies
{
    IAsyncPolicy<HttpResponseMessage> GetCombinedPolicy();
}
