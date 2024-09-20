using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

// Use the default URL for local development
var urls = "http://localhost:5001";

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add OpenTelemetry services
builder.Services.AddOpenTelemetry()
    .WithTracing(tracerProviderBuilder =>
    {
        tracerProviderBuilder
            .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("OpenTelemetryApp"))
            .AddAspNetCoreInstrumentation()
            .AddJaegerExporter(opt =>
            {
                opt.AgentHost = "localhost";
                opt.AgentPort = 6831;
            })
            .AddConsoleExporter();
    })
    .WithMetrics(metricsProviderBuilder =>
    {
        metricsProviderBuilder
            .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("OpenTelemetryApp"))
            .AddAspNetCoreInstrumentation()
            .AddPrometheusExporter()
            .AddConsoleExporter();
    });

builder.Services.AddHttpClient();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    urls = "http://localhost:5000";
    Console.WriteLine($"Running Swagger on: {urls}");
}

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "OpenTelemetryApp API V1");
    c.RoutePrefix = string.Empty;
});

app.UseRouting();
app.UseAuthorization();

app.MapControllers();
app.MapPrometheusScrapingEndpoint();

app.Run(urls);