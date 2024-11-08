using OpenTelemetryLibrary;

var builder = WebApplication.CreateBuilder(args);

// Use localhost port 5001 for ServiceOne
builder.WebHost.UseUrls("http://localhost:5001");

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Pass the configuration to AddCustomOpenTelemetry
builder.Services.AddCustomOpenTelemetry(builder.Configuration);

// Register HttpClient for inter-service calls to ServiceTwo
builder.Services.AddHttpClient("ServiceTwoClient", client =>
{
    client.BaseAddress = new Uri("http://localhost:5002");
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    Console.WriteLine("Running ServiceOne on http://localhost:5001");
}

// Enable Swagger
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "OpenTelemetryAppServiceOne API V1");
    c.RoutePrefix = string.Empty;
});

app.UseRouting();
app.UseAuthorization();

app.MapControllers();

app.Run();