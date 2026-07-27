// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide;

/// <summary>
/// Represents a <see href="https://valkey.io/commands/latency-latest/">LATENCY LATEST</see> entry.
/// </summary>
/// <seealso href="https://valkey.io/commands/latency-latest/"/>
public readonly record struct LatencyEventInfo
{
    /// <summary>
    /// The name of the event.
    /// </summary>
    public string EventName { get; init; }

    /// <summary>
    /// The time of the latest latency spike.
    /// </summary>
    public DateTimeOffset LatestTime { get; init; }

    /// <summary>
    /// The duration of the latest latency spike, in milliseconds.
    /// </summary>
    public TimeSpan LatestDuration { get; init; }

    /// <summary>
    /// The all-time maximum duration of a latency spike, in milliseconds.
    /// </summary>
    public TimeSpan MaxDuration { get; init; }

    /// <summary>
    /// The sum of all latency spike durations in the event's time series, in milliseconds.
    /// </summary>
    /// <note>Since Valkey 8.1.0.</note>
    public TimeSpan? Sum { get; init; }

    /// <summary>
    /// The number of latency spikes recorded in the event's time series.
    /// </summary>
    /// <note>Since Valkey 8.1.0.</note>
    public long? Count { get; init; }
}
