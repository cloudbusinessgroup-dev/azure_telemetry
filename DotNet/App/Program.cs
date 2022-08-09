using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.ApplicationInsights;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

IConfiguration configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables()
    .AddCommandLine(args)
    .Build();

Console.WriteLine("Telemetry solution within .NET 6 / C#");

using var channel = new InMemoryChannel();

IServiceCollection services = new ServiceCollection();
services.Configure<TelemetryConfiguration>(config => config.TelemetryChannel = channel);
services.AddLogging(builder =>
{
    builder.AddApplicationInsights(
        configureTelemetryConfiguration: (config) => config.ConnectionString = configuration.GetValue<string>("ApplicationInsights:ConnectionString"),
        configureApplicationInsightsLoggerOptions: (options) => {  }
    );
});

IServiceProvider serviceProvider = services.BuildServiceProvider();
ILogger<Program> logger = serviceProvider.GetRequiredService<ILogger<Program>>();

try
{
    logger.LogInformation("Logger is working...");
    throw new Exception("Exception to log");
}
catch (System.Exception ex)
{
    Console.WriteLine($"Log exception: {ex.Message}");
    logger.Log(LogLevel.Error, ex.Message);
}