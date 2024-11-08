using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using OpenTelemetry.Exporter;
using OpenTelemetry.Metrics;

namespace OpenTelemetryLibrary
{
    public static class OpenTelemetryExtensions
    {
        public static IServiceCollection AddCustomOpenTelemetry(this IServiceCollection services, IConfiguration configuration)
        {
            // Bind the OpenTelemetry settings from appsettings.json
            var otelOptions = new OpenTelemetryOptions();
            configuration.GetSection("OpenTelemetry").Bind(otelOptions);

            // Create resource attributes dictionary
            var resourceAttributes = new Dictionary<string, object>
            {
                { "service.name", otelOptions.ServiceName },
                { "environment", otelOptions.Environment },
                { "version", otelOptions.Version },
                { "region", otelOptions.Region }
            };

            // Add custom attributes from configuration
            if (otelOptions.CustomAttributes != null)
            {
                foreach (var attr in otelOptions.CustomAttributes)
                {
                    resourceAttributes.Add(attr.Key, attr.Value);
                }
            }

            // Build the resource with all attributes
            var resourceBuilder = ResourceBuilder.CreateDefault()
                .AddAttributes(resourceAttributes);

            // Configure the sampler based on settings
            Sampler sampler = otelOptions.Sampler switch
            {
                "AlwaysOn" => new AlwaysOnSampler(),
                "AlwaysOff" => new AlwaysOffSampler(),
                "TraceIdRatioBased" => new TraceIdRatioBasedSampler(otelOptions.SampleRate),
                _ => new AlwaysOnSampler()
            };

            // Add OpenTelemetry with tracing and metrics conditionally
            var openTelemetryBuilder = services.AddOpenTelemetry();

            if (otelOptions.EnableTraces)
            {
                openTelemetryBuilder.WithTracing(tracerProviderBuilder =>
                {
                    tracerProviderBuilder
                        .SetResourceBuilder(resourceBuilder)
                        .AddAspNetCoreInstrumentation()
                        .AddHttpClientInstrumentation()
                        .SetSampler(sampler)
                        .AddOtlpExporter(options =>
                        {
                            options.Endpoint = new Uri(otelOptions.OtlpTracesEndpoint);
                            options.Protocol = OtlpExportProtocol.Grpc;
                        });

                    if (otelOptions.EnableConsoleExporter)
                    {
                        tracerProviderBuilder.AddConsoleExporter();
                    }
                });
            }

            if (otelOptions.EnableMetrics)
            {
                openTelemetryBuilder.WithMetrics(metricProviderBuilder =>
                {
                    metricProviderBuilder
                        .SetResourceBuilder(resourceBuilder)
                        .AddAspNetCoreInstrumentation()
                        .AddHttpClientInstrumentation()
                        .AddOtlpExporter(options =>
                        {
                            options.Endpoint = new Uri(otelOptions.OtlpMetricsEndpoint);
                            options.Protocol = OtlpExportProtocol.HttpProtobuf;
                        });

                    if (otelOptions.EnableConsoleExporter)
                    {
                        metricProviderBuilder.AddConsoleExporter();
                    }
                });
            }

            return services;
        }
    }
}