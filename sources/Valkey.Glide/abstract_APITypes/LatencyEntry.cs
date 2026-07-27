// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide;

/// <summary>
/// Represents a <see href="https://valkey.io/commands/latency-history/">LATENCY HISTORY</see> entry.
/// </summary>
/// <seealso href="https://valkey.io/commands/latency-history/"/>
public readonly record struct LatencyEntry
{
    /// <summary>
    /// The time of the latency spike.
    /// </summary>
    public DateTimeOffset Time { get; init; }

    /// <summary>
    /// The duration of the latency spike.
    /// </summary>
    public TimeSpan Duration { get; init; }
}
