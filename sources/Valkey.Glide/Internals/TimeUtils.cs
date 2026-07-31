// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.Internals;

/// <summary>
/// Time utility methods.
/// </summary>
internal static class TimeUtils
{
    /// <summary>
    /// Converts a <see cref="TimeSpan"/> to <see cref="ulong"/> milliseconds.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="timeSpan"/> is negative.</exception>
    public static ulong ToMilliseconds(TimeSpan timeSpan)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(timeSpan, TimeSpan.Zero, nameof(timeSpan));

        // Use tick-based arithmetic to avoid floating-point precision loss.
        return (ulong)(timeSpan.Ticks / TimeSpan.TicksPerMillisecond);
    }

    /// <summary>
    /// Converts a <see cref="TimeSpan"/> to <see cref="double"/> seconds.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="timeSpan"/> is negative.</exception>
    public static double ToSeconds(TimeSpan timeSpan)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(timeSpan, TimeSpan.Zero, nameof(timeSpan));
        return timeSpan.TotalSeconds;
    }
}
