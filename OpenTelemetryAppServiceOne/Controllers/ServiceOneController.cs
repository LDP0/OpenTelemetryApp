using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace OpenTelemetryTracesApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ServiceOneController : ControllerBase
    {
        private readonly HttpClient _HttpClient;
        private static readonly ActivitySource ActivitySource = new ActivitySource("ServiceOneActivitySource");

        public ServiceOneController(IHttpClientFactory httpClientFactory)
        {
            _HttpClient = httpClientFactory.CreateClient("ServiceTwoClient");
        }

        // Endpoint to get ServiceOne response with a call to ServiceTwo
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            // Call ServiceTwo’s basic endpoint
            try
            {
                var response = await _HttpClient.GetStringAsync("http://localhost:5002/ServiceTwo");
                return Ok($"ServiceOne received response from ServiceTwo: {response}");
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(500, $"ServiceOne failed to get a response from ServiceTwo: {ex.Message}");
            }
        }

        // Endpoint to call ServiceTwo's calculate method, which will then call back to ServiceOne
        [HttpGet("chain-call")]
        public async Task<IActionResult> ChainCall()
        {
            try
            {
                // Call ServiceTwo’s calculate endpoint, which will call back to ServiceOne’s "callback" endpoint
                var response = await _HttpClient.GetStringAsync("http://localhost:5002/ServiceTwo/calculate");
                return Ok($"ServiceOne received calculation response from ServiceTwo: {response}");
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(500, $"ServiceOne failed to get a response from ServiceTwo: {ex.Message}");
            }
        }

        // Callback endpoint to be called by ServiceTwo
        [HttpGet("callback")]
        public IActionResult Callback()
        {
            return Ok("ServiceOne Callback reached by ServiceTwo");
        }
    }
}