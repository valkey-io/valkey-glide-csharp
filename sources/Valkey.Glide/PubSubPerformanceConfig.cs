// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

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

        if (!Enum.IsDefined(typeof(BoundedChannelFullMode), FullMode))
        {
            throw new ArgumentOutOfRangeException(nameof(FullMode), "Invalid BoundedChannelFullMode value");
        }
    }
}
