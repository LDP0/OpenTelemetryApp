using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenTelemetryLibrary;

var builder = WebApplication.CreateBuilder(args);

// Use localhost port 5002 for ServiceTwo
builder.WebHost.UseUrls("http://localhost:5002");

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Pass the configuration to AddCustomOpenTelemetry
builder.Services.AddCustomOpenTelemetry(builder.Configuration);

// Register HttpClient for inter-service calls to ServiceOne
builder.Services.AddHttpClient("ServiceOneClient", client =>
{
    client.BaseAddress = new Uri("http://localhost:5001");
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    Console.WriteLine("Running ServiceTwo on http://localhost:5002");
}

// Enable Swagger
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "OpenTelemetryAppServiceTwo API V1");
    c.RoutePrefix = string.Empty;
});

app.UseRouting();
app.UseAuthorization();

app.MapControllers();

app.Run();