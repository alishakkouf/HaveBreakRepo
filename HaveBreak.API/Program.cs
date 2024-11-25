using HaveBreak.API.Configuration;
using HaveBreak.Common.Middlewares;
using HealthChecks.UI.Client;
using HealthChecks.UI.Configuration;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Serilog;
using HaveBreak.Data;
using HealthChecks.UI.Client;
using HaveBreak.Manager;
using HaveBreak.API;

IConfiguration configuration = new ConfigurationBuilder()
                            .AddJsonFile("appsettings.json")
                            .Build();

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog based on environment
var environment = builder.Environment;

/*
 -->context: Provides information about the hosting environment and configuration (like appsettings.json).
 -->services: Allows access to the app’s dependency injection services, enabling us to retrieve required services, such as TelemetryConfiguration for Application Insights.
 -->configuration: Represents Serilog's configuration builder.
 * */
builder.Host.UseSerilog((context, services, configuration) =>
{
    Serilog.Debugging.SelfLog.Enable(Console.Out);

    //Reads Serilog settings from appsettings.json. This loads configurations like log levels, enrichers, and sinks.
    //Adds contextual data to logs.
    //For example, if extra information like a user ID is added to the logging context during a request,
    //it will appear in the logs, helping with traceability.
    configuration.ReadFrom.Configuration(context.Configuration)
                 .Enrich.FromLogContext();

    if (environment.IsDevelopment())
    {
        // Additional configuration for local development
        configuration.WriteTo.File("Logs/local-log-.txt", rollingInterval: RollingInterval.Day);
    }
    else
    {
        // Additional configuration for production => I'll not handle it cause it's just an assignment

        //configuration.WriteTo.ApplicationInsights(
        //    services.GetRequiredService<TelemetryConfiguration>(),
        //    TelemetryConverter.Traces
        //);
    }
});

Log.Information("starting server.");

// Add services to the container.
builder.Services.ConfigureDataModule(configuration);
builder.Services.ConfigureManagerModule(configuration);
builder.Services.ConfigureApiControllers(configuration, "CorsPolicy");
builder.Services.ConfigureApiIdentity(configuration);
builder.Services.AddControllers();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Congiguring Health Ckeck
builder.Services.ConfigureHealthChecks(builder.Configuration);

builder.Services.AddSwaggerDocumentation();

var app = builder.Build();

app.UseMiddleware<LoggingEnrichmentMiddleware>();

//HealthCheck Middleware
app.MapHealthChecks("/api/health", new HealthCheckOptions()
{
    Predicate = _ => true,
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});
app.UseHealthChecksUI(delegate (Options options)
{
    options.UIPath = "/healthcheck-ui";//https://localhost:7223/healthcheck-ui
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwaggerDocumentation(configuration);
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.UseCustomExceptionHandler();

app.UseMiddleware<ApiKeyMiddleware>();

app.UseCors("CorsPolicy");

app.UseStaticFiles();

app.UseRequestLocalization();

app.UseResultWrapper();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();
app.UseUnitOfWork();

try
{
    Log.Information("Trying to migrate db.");
    await app.MigrateAndSeedDatabaseAsync();
}
catch (Exception exception)
{
    Log.Error(exception, "Stopped program because of exception");
    throw;
}

app.Run();
