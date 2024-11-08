using Microsoft.AspNetCore.Mvc;

namespace OpenTelemetryAppServiceTwo.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ServiceTwoController : ControllerBase
    {
        private readonly HttpClient _HttpClient;

        public ServiceTwoController(IHttpClientFactory httpClientFactory)
        {
            _HttpClient = httpClientFactory.CreateClient("ServiceOneClient");
        }

        // Basic endpoint to get a response
        [HttpGet]
        public IActionResult Get()
        {
            return Ok("Hello from ServiceTwo");
        }

        // Calculate endpoint, which also calls back to ServiceOne
        [HttpGet("calculate")]
        public async Task<IActionResult> Calculate()
        {
            var result = 42;

            try
            {
                // Call back to ServiceOne's callback endpoint
                var callbackResponse = await _HttpClient.GetStringAsync("http://localhost:5001/ServiceOne/callback");
                return Ok(new { Result = result, CallbackResponse = callbackResponse });
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(500, $"ServiceTwo failed to call ServiceOne's callback: {ex.Message}");
            }
        }
    }
}