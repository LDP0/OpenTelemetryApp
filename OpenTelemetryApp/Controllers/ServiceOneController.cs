using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace OpenTelemetryTracesApp
{
    [ApiController]
    [Route("[controller]")]
    public class ServiceOneController : ControllerBase
    {
        private readonly IHttpClientFactory _HttpClientFactory;
        private static readonly ActivitySource ActivitySource = new("OpenTelemetryProject");

        public ServiceOneController(IHttpClientFactory httpClientFactory)
        {
            _HttpClientFactory = httpClientFactory;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            using var activity = ActivitySource.StartActivity("ServiceOneOperation");
            activity?.SetTag("http.method", "GET");
            activity?.SetTag("operation.name", "Fetch data from ServiceTwo");

            var client = _HttpClientFactory.CreateClient();

            // Get the current request's scheme and host
            var request = HttpContext.Request;
            var scheme = request.Scheme;
            var host = request.Host.Value;

            // Build the URL dynamically
            var url = $"{scheme}://{host}/ServiceTwo";

            var response = await client.GetStringAsync(url);

            activity?.SetTag("response.length", response.Length.ToString());

            return Ok($"ServiceOne received response: {response}");
        }

        [HttpGet("processdata")]
        public IActionResult ProcessData()
        {
            using var activity = ActivitySource.StartActivity("ServiceOneProcessData");
            activity?.SetTag("operation.name", "Data processing");

            // Simulate some processing logic
            var processedData = new { Message = "Data processed successfully", Timestamp = DateTime.UtcNow };

            activity?.SetTag("processing.result", "Success");

            return Ok(processedData);
        }

        [HttpGet("external/failure")]
        public async Task<IActionResult> CallExternalService()
        {
            using var activity = ActivitySource.StartActivity("ServiceOneCallExternalService");
            activity?.SetTag("operation.name", "Call External Service");

            var client = _HttpClientFactory.CreateClient();
            
            // Example of an external service (Failure)
            var externalServiceUrl = "https://api.github.com/"; 

            try
            {
                var response = await client.GetStringAsync(externalServiceUrl);
                activity?.SetTag("response.status_code", "200");
                activity?.SetTag("response.length", response.Length.ToString());

                return Ok($"ServiceOne received external response: {response}");
            }
            catch (HttpRequestException ex)
            {
                activity?.SetTag("response.status_code", "500");
                activity?.SetTag("error", true);
                activity?.SetTag("exception.message", ex.Message);

                return StatusCode(500, "Failed to retrieve data from external service");
            }
        }

        [HttpGet("external/success")]
        public async Task<IActionResult> CallSuccessfulExternalService()
        {
            using var activity = ActivitySource.StartActivity("ServiceOneCallSuccessfulExternalService");
            activity?.SetTag("operation.name", "Call Successful External Service");

            var client = _HttpClientFactory.CreateClient();
            
            // Mock external service (Succeessful)
            var externalServiceUrl = "https://jsonplaceholder.typicode.com/posts/1";

            try
            {
                var response = await client.GetStringAsync(externalServiceUrl);
                activity?.SetTag("response.status_code", "200");
                activity?.SetTag("response.length", response.Length.ToString());

                return Ok($"ServiceOne received external response: {response}");
            }
            catch (HttpRequestException ex)
            {
                activity?.SetTag("response.status_code", "500");
                activity?.SetTag("error", true);
                activity?.SetTag("exception.message", ex.Message);

                return StatusCode(500, "Failed to retrieve data from external service");
            }
        }
    }
}