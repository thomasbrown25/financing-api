using financing_api.Data;
using financing_api.DbLogger;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace financing_api.Services.HealthService
{
    public class HealthService(ILogging logging) : IHealthCheck
    {
        private readonly ILogging _logging = logging;

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {

            _logging.LogTrace("Health Check Succeeded");

            return HealthCheckResult.Healthy("A healthy result.");
        }

    }
}