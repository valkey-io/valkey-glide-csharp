// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using System;
using System.Threading.Channels;

namespace Valkey.Glide;

/// <summary>
/// Configuration options for PubSub performance tuning.
/// </summary>
public sealed class PubSubPerformanceConfig
{
    /// <summary>
    /// Default channel capacity for message queuing.
    /// </summary>
    public const int DefaultChannelCapacity = 1000;

    /// <summary>
    /// Default shutdown timeout in seconds.
    /// </summary>
    public const int DefaultShutdownTimeoutSeconds = 5;

    /// <summary>
    /// Maximum number of messages to queue before applying backpressure.
    /// Default: 1000
    /// </summary>
    public int ChannelCapacity { get; set; } = DefaultChannelCapacity;

    /// <summary>
    /// Strategy to use when the message channel is full.
    /// Default: Wait (apply backpressure)
    /// </summary>
    public BoundedChannelFullMode FullMode { get; set; } = BoundedChannelFullMode.Wait;

    /// <summary>
    /// Timeout for graceful shutdown of PubSub processing.
    /// Default: 5 seconds
    /// </summary>
    public TimeSpan ShutdownTimeout { get; set; } = TimeSpan.FromSeconds(DefaultShutdownTimeoutSeconds);

    /// <summary>
    /// Enable performance metrics logging.
    /// Default: false
    /// </summary>
    public bool EnableMetrics { get; set; } = false;

    /// <summary>
    /// Interval for logging performance metrics.
    /// Default: 30 seconds
    /// </summary>
    public TimeSpan MetricsInterval { get; set; } = TimeSpan.FromSeconds(30);

    /// <summary>
    /// Validates the configuration.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when configuration values are invalid.</exception>
    internal void Validate()
    {
        if (ChannelCapacity <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(ChannelCapacity), "Channel capacity must be greater than zero");
        }

        if (ShutdownTimeout <= TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(ShutdownTimeout), "Shutdown timeout must be greater than zero");
        }

        if (EnableMetrics && MetricsInterval <= TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(MetricsInterval), "Metrics interval must be greater than zero when metrics are enabled");
        }

        if (!Enum.IsDefined(typeof(BoundedChannelFullMode), FullMode))
        {
            throw new ArgumentOutOfRangeException(nameof(FullMode), "Invalid BoundedChannelFullMode value");
        }
    }
}

/// <summary>
/// Extension methods for configuring PubSub performance options.
/// </summary>
public static class PubSubConfigurationExtensions
{
    /// <summary>
    /// Configure performance options for PubSub message processing.
    /// </summary>
    /// <typeparam name="T">The configuration type.</typeparam>
    /// <param name="config">The PubSub subscription configuration.</param>
    /// <param name="performanceConfig">The performance configuration to apply.</param>
    /// <returns>The configuration instance for method chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when performanceConfig is null.</exception>
    public static T WithPerformanceConfig<T>(this T config, PubSubPerformanceConfig performanceConfig)
        where T : BasePubSubSubscriptionConfig
    {
        ArgumentNullException.ThrowIfNull(performanceConfig);
        performanceConfig.Validate();

        config.PerformanceConfig = performanceConfig;
        return config;
    }

    /// <summary>
    /// Configure channel capacity for PubSub message queuing.
    /// </summary>
    /// <typeparam name="T">The configuration type.</typeparam>
    /// <param name="config">The PubSub subscription configuration.</param>
    /// <param name="capacity">The maximum number of messages to queue.</param>
    /// <returns>The configuration instance for method chaining.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when capacity is less than or equal to zero.</exception>
    public static T WithChannelCapacity<T>(this T config, int capacity)
        where T : BasePubSubSubscriptionConfig
    {
        if (capacity <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(capacity), "Channel capacity must be greater than zero");
        }

        config.PerformanceConfig ??= new PubSubPerformanceConfig();
        config.PerformanceConfig.ChannelCapacity = capacity;
        return config;
    }

    /// <summary>
    /// Configure the backpressure strategy when the message channel is full.
    /// </summary>
    /// <typeparam name="T">The configuration type.</typeparam>
    /// <param name="config">The PubSub subscription configuration.</param>
    /// <param name="fullMode">The strategy to use when the channel is full.</param>
    /// <returns>The configuration instance for method chaining.</returns>
    public static T WithFullMode<T>(this T config, BoundedChannelFullMode fullMode)
        where T : BasePubSubSubscriptionConfig
    {
        if (!Enum.IsDefined(typeof(BoundedChannelFullMode), fullMode))
        {
            throw new ArgumentOutOfRangeException(nameof(fullMode), "Invalid BoundedChannelFullMode value");
        }

        config.PerformanceConfig ??= new PubSubPerformanceConfig();
        config.PerformanceConfig.FullMode = fullMode;
        return config;
    }

    /// <summary>
    /// Configure the shutdown timeout for graceful PubSub processing termination.
    /// </summary>
    /// <typeparam name="T">The configuration type.</typeparam>
    /// <param name="config">The PubSub subscription configuration.</param>
    /// <param name="timeout">The timeout duration.</param>
    /// <returns>The configuration instance for method chaining.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when timeout is less than or equal to zero.</exception>
    public static T WithShutdownTimeout<T>(this T config, TimeSpan timeout)
        where T : BasePubSubSubscriptionConfig
    {
        if (timeout <= TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(timeout), "Shutdown timeout must be greater than zero");
        }

        config.PerformanceConfig ??= new PubSubPerformanceConfig();
        config.PerformanceConfig.ShutdownTimeout = timeout;
        return config;
    }

    /// <summary>
    /// Enable performance metrics logging with optional custom interval.
    /// </summary>
    /// <typeparam name="T">The configuration type.</typeparam>
    /// <param name="config">The PubSub subscription configuration.</param>
    /// <param name="interval">The interval for logging metrics. If null, uses default of 30 seconds.</param>
    /// <returns>The configuration instance for method chaining.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when interval is less than or equal to zero.</exception>
    public static T WithMetrics<T>(this T config, TimeSpan? interval = null)
        where T : BasePubSubSubscriptionConfig
    {
        if (interval.HasValue && interval.Value <= TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(interval), "Metrics interval must be greater than zero");
        }

        config.PerformanceConfig ??= new PubSubPerformanceConfig();
        config.PerformanceConfig.EnableMetrics = true;

        if (interval.HasValue)
        {
            config.PerformanceConfig.MetricsInterval = interval.Value;
        }

        return config;
    }
}
