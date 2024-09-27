using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace OpenTelemetryLibrary;

public static class OpenTelemetryExtensions
{
    public static IServiceCollection AddCustomOpenTelemetry(this IServiceCollection services, 
        string serviceName, 
        string jaegerHost = "localhost",
        int jaegerPort = 6831,
        string prometheusEndpoint = "/metrics",
        int scrapeCacheDurationMilliseconds = 5000,
        bool enableConsoleExporter = false)
    {
        // Add OpenTelemetry services for tracing and metrics
        services.AddOpenTelemetry()
            .WithTracing(tracerProviderBuilder =>
            {
                tracerProviderBuilder
                    .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(serviceName))
                    .AddAspNetCoreInstrumentation()
                    .AddJaegerExporter(opt =>
                    {
                        opt.AgentHost = jaegerHost;   // Use passed configuration (default: localhost)
                        opt.AgentPort = jaegerPort;  // Use passed configuration (default: 6831)
                    });
                
                if (enableConsoleExporter)  // Conditionally add console exporter
                {
                    tracerProviderBuilder.AddConsoleExporter();
                }
            })
            .WithMetrics(metricsProviderBuilder =>
            {
                metricsProviderBuilder
                    .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(serviceName))
                    .AddAspNetCoreInstrumentation()
                    .AddPrometheusExporter(opt =>
                    {
                        opt.ScrapeResponseCacheDurationMilliseconds = scrapeCacheDurationMilliseconds;  // Optional cache configuration
                        opt.ScrapeEndpointPath = prometheusEndpoint;  // Configurable scrape endpoint (default: /metrics)
                    });

                if (enableConsoleExporter)  // Conditionally add console exporter
                {
                    metricsProviderBuilder.AddConsoleExporter();
                }
            });

        return services;
    }
}