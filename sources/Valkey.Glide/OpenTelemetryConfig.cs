// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide;

/// <summary>
/// Configuration for OpenTelemetry integration.
/// </summary>
public sealed class OpenTelemetryConfig
{
    #region Public Properties

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

    #endregion
    #region Constructors

    private OpenTelemetryConfig(TracesConfig? traces, MetricsConfig? metrics, TimeSpan? flushInterval)
    {
        Traces = traces;
        Metrics = metrics;
        FlushInterval = flushInterval;
    }

    #endregion
    #region Public Methods

    /// <summary>
    /// Creates a new OpenTelemetryConfig builder.
    /// </summary>
    public static Builder CreateBuilder() => new();

    #endregion

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
