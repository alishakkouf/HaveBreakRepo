using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace HaveBreak.API
{
    /// <summary>
    ///  This health check ensures that the database is not only reachable,
    ///  but also responsive to queries.
    /// </summary>
    public static class HealthCheck
    {
        public static void ConfigureHealthChecks(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHealthChecks()
                .AddSqlServer(configuration["ConnectionStrings:DefaultConnection"],
                              healthQuery: "select 1",
                              name: "SQL Server",
                              /*
                               * This indicates that if the health check fails,
                               * the overall health status should be marked as unhealthy.
                               */
                              failureStatus: HealthStatus.Unhealthy,
                              tags: new[] { "Feedback", "Database" })

                .AddCheck<MemoryHealthCheck>($"Feedback Service Memory Check",
                failureStatus: HealthStatus.Unhealthy,
                tags: new[] { "Feedback Service" });

            services.AddHealthChecksUI(opt =>
            {
                opt.SetEvaluationTimeInSeconds(10); //time in seconds between check    
                opt.MaximumHistoryEntriesPerEndpoint(60); //maximum history of checks    
                opt.SetApiMaxActiveRequests(1); //api requests concurrency    
                opt.AddHealthCheckEndpoint("feedback api", "/api/health"); //map health check api    

            })
                .AddInMemoryStorage();
        }
    }
}
