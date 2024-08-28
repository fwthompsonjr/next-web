using Microsoft.Extensions.Diagnostics.HealthChecks;
using next.processor.api.interfaces;
using System.Collections.ObjectModel;

namespace next.processor.api.services
{
    public class CheckContainerServices(IQueueExecutor queue) : IHealthCheck
    {
        private readonly IQueueExecutor executor = queue;

        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            var available = executor.IsReadyCount();
            var actual = executor.InstallerCount();
            var status = 0;
            if (available > 0) status = 1;
            if (available == actual) status = 2;
            var details = executor.GetDetails();
            var data = new ReadOnlyDictionary<string, object>(details);
            HealthCheckResult health = status switch
            {
                2 => HealthCheckResult.Healthy("Container services are available.", data),
                0 => HealthCheckResult.Unhealthy("Container services are not ready.", null, data),
                _ => HealthCheckResult.Degraded("1 or more Container services are inactive.", null, data)
            };
            return Task.FromResult(health);
        }
    }
}
