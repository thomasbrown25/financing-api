using financing_api.Data;
using financing_api.DbLogger;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace financing_api.Services.HealthService
{
    public class HealthService(ILogging logging, IConfiguration configuration) : IHealthCheck
    {
        private readonly ILogging _logging = logging;
        private readonly IConfiguration _configuration = configuration;

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            if (Convert.ToBoolean(_configuration["Logging.TraceLogs.Enabled"]))
                _logging.LogTrace("Health Check Succeeded");

            return HealthCheckResult.Healthy("A healthy result.");
        }

    }
}