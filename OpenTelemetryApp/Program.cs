using OpenTelemetryLibrary; // Import your library

var builder = WebApplication.CreateBuilder(args);

// Use the default URL for local development
var urls = "http://localhost:5001";

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Use the custom OpenTelemetry setup from OpenTelemetryLibrary with console logging enabled
builder.Services.AddCustomOpenTelemetry(
    serviceName: "OpenTelemetryApp",        // Service name to identify in traces
    jaegerHost: "localhost",                // Jaeger host (default: localhost)
    jaegerPort: 6831,                       // Jaeger port (default: 6831)
    prometheusEndpoint: "/metrics",         // Prometheus scrape endpoint (default: /metrics)
    scrapeCacheDurationMilliseconds: 5000,  // Cache duration for Prometheus scrape (default: 5000ms)
    enableConsoleExporter: false            // Enable console logging for tracing and metrics
);

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