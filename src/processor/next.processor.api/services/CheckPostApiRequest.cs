using Microsoft.Extensions.Diagnostics.HealthChecks;
using next.processor.api.interfaces;

namespace next.processor.api.services
{
    public class CheckPostApiRequest(IApiWrapper api) : IHealthCheck
    {
        private readonly IApiWrapper svc = api;

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            var data = await svc.FetchAsync();
            var status = data != null;
            var health = status switch
            {
                true => HealthCheckResult.Healthy("Data services are available."),
                _ => HealthCheckResult.Unhealthy("Data services are not responding.")
            };
            return health;
        }
    }
}