// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide;

/// <summary>
/// Configuration for OpenTelemetry integration.
/// </summary>
public sealed class OpenTelemetryConfig
{
    /// <summary>
    /// Configuration for traces export.
    /// </summary>
    public TracesConfig? Traces { get; }

    /// <summary>
    /// Configuration for metrics export.
    /// </summary>
    public MetricsConfig? Metrics { get; }

    /// <summary>
    /// Interval for flushing telemetry data to the collector.
    /// </summary>
    public TimeSpan? FlushInterval { get; }

    private OpenTelemetryConfig(TracesConfig? traces, MetricsConfig? metrics, TimeSpan? flushInterval)
    {
        Traces = traces;
        Metrics = metrics;
        FlushInterval = flushInterval;
    }

    /// <summary>
    /// Creates a new OpenTelemetryConfig builder.
    /// </summary>
    public static Builder CreateBuilder() => new();

    /// <summary>
    /// Builder for OpenTelemetryConfig.
    /// </summary>
    public sealed class Builder
    {
        private TracesConfig? _traces;
        private MetricsConfig? _metrics;
        private TimeSpan? _flushInterval;

        /// <summary>
        /// Sets the traces configuration.
        /// </summary>
        public Builder WithTraces(TracesConfig traces)
        {
            _traces = traces;
            return this;
        }

        /// <summary>
        /// Sets the metrics configuration.
        /// </summary>
        public Builder WithMetrics(MetricsConfig metrics)
        {
            _metrics = metrics;
            return this;
        }

        /// <summary>
        /// Sets the flush interval.
        /// </summary>
        /// <exception cref="ArgumentException">Thrown if flushInterval is not positive.</exception>
        public Builder WithFlushInterval(TimeSpan flushInterval)
        {
            if (flushInterval <= TimeSpan.Zero)
                throw new ArgumentException("Flush interval must be positive", nameof(flushInterval));

            _flushInterval = flushInterval;
            return this;
        }

        /// <summary>
        /// Builds the OpenTelemetryConfig.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if neither traces nor metrics are configured.</exception>
        public OpenTelemetryConfig Build()
        {
            if (_traces == null && _metrics == null)
                throw new InvalidOperationException("At least one of traces or metrics must be configured");

            return new OpenTelemetryConfig(_traces, _metrics, _flushInterval);
        }
    }
}

/// <summary>
/// Configuration for OpenTelemetry traces.
/// </summary>
public sealed class TracesConfig
{
    internal const uint DefaultSamplePercentage = 1u; // Exposed internally for testing.
    private const uint MaxSamplePercentage = 100u;

    /// <summary>
    /// The endpoint for traces export.
    /// </summary>
    public string Endpoint { get; }

    /// <summary>
    /// The percentage of requests to sample (0-100).
    /// If not specified, defaults to 1 percent.
    /// </summary>
    public uint SamplePercentage { get; private set; }

    private TracesConfig(string endpoint, uint samplePercentage)
    {
        Endpoint = endpoint;
        SamplePercentage = samplePercentage;
    }

    /// <summary>
    /// Sets the sample percentage.
    /// </summary>
    /// <param name="percentage">The sample percentage (0-100).</param>
    /// <exception cref="ArgumentException">Thrown if percentage is greater than 100.</exception>
    internal void SetSamplePercentage(uint percentage)
    {
        ValidateSamplePercentage(percentage);
        SamplePercentage = percentage;
    }

    private static void ValidateSamplePercentage(uint percentage)
    {
        if (percentage > MaxSamplePercentage)
        {
            throw new ArgumentException($"Sample percentage cannot be greater than {MaxSamplePercentage}", nameof(percentage));
        }
    }

    /// <summary>
    /// Creates a new TracesConfig builder.
    /// </summary>
    public static Builder CreateBuilder() => new();

    /// <summary>
    /// Builder for TracesConfig.
    /// </summary>
    public sealed class Builder
    {
        private string? _endpoint;
        private uint _samplePercentage = DefaultSamplePercentage;

        /// <summary>
        /// Sets the endpoint for traces export.
        /// </summary>
        /// <exception cref="ArgumentException">Thrown if endpoint is null or empty.</exception>
        public Builder WithEndpoint(string endpoint)
        {
            if (string.IsNullOrWhiteSpace(endpoint))
            {
                throw new ArgumentException("Endpoint cannot be null or empty", nameof(endpoint));
            }

            _endpoint = endpoint;
            return this;
        }

        /// <summary>
        /// Sets the sample percentage.
        /// </summary>
        /// <exception cref="ArgumentException">Thrown if samplePercentage is greater than 100.</exception>
        public Builder WithSamplePercentage(uint samplePercentage)
        {
            ValidateSamplePercentage(samplePercentage);

            _samplePercentage = samplePercentage;
            return this;
        }

        /// <summary>
        /// Builds the TracesConfig.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if endpoint is not specified.</exception>
        public TracesConfig Build()
        {
            if (_endpoint == null)
            {
                throw new InvalidOperationException("Endpoint must be specified");
            }

            return new TracesConfig(_endpoint, _samplePercentage);
        }
    }
}

/// <summary>
/// Configuration for OpenTelemetry metrics.
/// </summary>
public sealed class MetricsConfig
{
    /// <summary>
    /// The endpoint for metrics export.
    /// </summary>
    public string Endpoint { get; }

    private MetricsConfig(string endpoint)
    {
        Endpoint = endpoint;
    }

    /// <summary>
    /// Creates a new MetricsConfig builder.
    /// </summary>
    public static Builder CreateBuilder() => new();

    /// <summary>
    /// Builder for MetricsConfig.
    /// </summary>
    public sealed class Builder
    {
        private string? _endpoint;

        /// <summary>
        /// Sets the endpoint for metrics export.
        /// </summary>
        /// <exception cref="ArgumentException">Thrown if endpoint is null or empty.</exception>
        public Builder WithEndpoint(string endpoint)
        {
            if (string.IsNullOrWhiteSpace(endpoint))
            {
                throw new ArgumentException("Endpoint cannot be null or empty", nameof(endpoint));
            }

            _endpoint = endpoint;
            return this;
        }

        /// <summary>
        /// Builds the MetricsConfig.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if endpoint is not specified.</exception>
        public MetricsConfig Build()
        {
            if (_endpoint == null)
            {
                throw new InvalidOperationException("Endpoint must be specified");
            }

            return new MetricsConfig(_endpoint);
        }
    }
}
