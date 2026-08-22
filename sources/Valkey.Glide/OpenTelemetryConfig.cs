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
    public TimeSpan? FlushInterval => FlushIntervalMs.HasValue
        ? TimeSpan.FromMilliseconds(FlushIntervalMs.Value)
        : null;

    #endregion Public Properties
    #region Constructors & Builders

    private OpenTelemetryConfig(TracesConfig? traces, MetricsConfig? metrics, uint? flushIntervalMs)
    {
        Traces = traces;
        Metrics = metrics;
        FlushIntervalMs = flushIntervalMs;
    }

    #endregion Constructors & Builders
    #region Public Methods

    /// <summary>
    /// Creates a new OpenTelemetryConfig builder.
    /// </summary>
    public static Builder CreateBuilder() => new();

    #endregion Public Methods
    #region Internal Fields

    internal readonly uint? FlushIntervalMs;

    #endregion Internal Fields

    /// <summary>
    /// Builder for OpenTelemetryConfig.
    /// </summary>
    public sealed class Builder
    {
        private TracesConfig? _traces;
        private MetricsConfig? _metrics;
        private uint? _flushIntervalMs;

        /// <summary>
        /// Sets the traces configuration.
        /// </summary>
        /// <param name="traces">The traces configuration to use.</param>
        public Builder WithTraces(TracesConfig traces)
        {
            _traces = traces;
            return this;
        }

        /// <summary>
        /// Sets the metrics configuration.
        /// </summary>
        /// <param name="metrics">The metrics configuration to use.</param>
        public Builder WithMetrics(MetricsConfig metrics)
        {
            _metrics = metrics;
            return this;
        }

        /// <summary>
        /// Sets the flush interval.
        /// </summary>
        /// <param name="flushInterval">The interval for flushing telemetry data to the collector.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="flushInterval"/> is not positive or exceeds <see cref="uint.MaxValue"/> milliseconds.</exception>
        public Builder WithFlushInterval(TimeSpan flushInterval)
        {
            _flushIntervalMs = Internals.TimeUtils.ToPositiveUintMs(flushInterval, nameof(flushInterval));
            return this;
        }

        /// <summary>
        /// Builds the OpenTelemetryConfig.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if neither traces nor metrics are configured.</exception>
        public OpenTelemetryConfig Build()
        {
            if (_traces == null && _metrics == null)
            {
                throw new InvalidOperationException("At least one of traces or metrics must be configured");
            }

            return new OpenTelemetryConfig(_traces, _metrics, _flushIntervalMs);
        }
    }
}
