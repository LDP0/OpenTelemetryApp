using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace OpenTelemetryTracesApp
{
    [ApiController]
    [Route("[controller]")]
    public class ServiceTwoController : ControllerBase
    {
        private static readonly ActivitySource ActivitySource = new("OpenTelemetryProject");

        [HttpGet]
        public IActionResult Get()
        {
            using var activity = ActivitySource.StartActivity("ServiceTwoOperation");
            activity?.SetTag("operation.name", "ServiceTwo Default Get");

            return Ok("Hello from ServiceTwo");
        }

        [HttpGet("calculate")]
        public IActionResult Calculate()
        {
            using var activity = ActivitySource.StartActivity("ServiceTwoCalculateOperation");
            activity?.SetTag("operation.name", "ServiceTwo Calculation");

            // Simulate a simple calculation
            var result = 42;

            activity?.SetTag("calculation.result", result.ToString());

            return Ok(new { Result = result });
        }
    }
}