namespace OpenTelemetryLibrary
{
    public class OpenTelemetryOptions
    {
        public string ServiceName { get; set; }
        public string OtlpTracesEndpoint { get; set; }
        public string OtlpMetricsEndpoint { get; set; }
        public bool EnableConsoleExporter { get; set; }
        public string Environment { get; set; }
        public string Version { get; set; }
        public string Region { get; set; }
        public Dictionary<string, string> CustomAttributes { get; set; }
        public string Sampler { get; set; }
        public double SampleRate { get; set; }
        public bool EnableTraces { get; set; }
        public bool EnableMetrics { get; set; }
    }
}