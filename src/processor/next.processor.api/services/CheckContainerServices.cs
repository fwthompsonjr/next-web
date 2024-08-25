using Microsoft.Extensions.Diagnostics.HealthChecks;
using next.processor.api.interfaces;

namespace next.processor.api.services
{
    public class CheckContainerServices(IQueueExecutor queue) : IHealthCheck
    {
        private readonly IQueueExecutor executor = queue;

        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            var status = executor.IsReady();
            var health = status switch
            {
                true => HealthCheckResult.Healthy("Container services are available."),
                false => HealthCheckResult.Unhealthy("1 or more Container services are inactive."),
                _ => HealthCheckResult.Degraded("Container services are not ready.")
            };
            return Task.FromResult(health);
        }
    }
}
