// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.Internals;

/// <summary>
/// Time utility methods.
/// </summary>
internal static class TimeUtils
{
    private static readonly TimeSpan MaxUintMilliseconds = TimeSpan.FromMilliseconds(uint.MaxValue);

    /// <summary>
    /// Converts a <see cref="TimeSpan"/> to <see cref="double"/> seconds.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="timeSpan"/> is not positive.</exception>
    public static double ToDoubleSeconds(TimeSpan timeSpan, string paramName)
    {
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(timeSpan, TimeSpan.Zero, paramName);
        return timeSpan.TotalSeconds;
    }

    /// <summary>
    /// Converts a <see cref="TimeSpan"/> to <see cref="uint"/> milliseconds, rounded to the nearest millisecond.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="timeSpan"/> is not positive or exceeds <see cref="uint.MaxValue"/> milliseconds.</exception>
    public static uint ToUintMilliseconds(TimeSpan timeSpan, string paramName)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThan(timeSpan, MaxUintMilliseconds, paramName);
        return (uint)ToULongMilliseconds(timeSpan, paramName);
    }

    /// <summary>
    /// Converts a <see cref="TimeSpan"/> to <see cref="uint"/> seconds, rounded to the nearest second.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="timeSpan"/> is not positive or exceeds <see cref="uint.MaxValue"/> seconds.</exception>
    public static uint ToUintSeconds(TimeSpan timeSpan, string paramName)
    {
        var secs = Math.Round(timeSpan.TotalSeconds);

        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(timeSpan, TimeSpan.Zero, paramName);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(secs, uint.MaxValue, paramName);

        return (uint)secs;
    }

    /// <summary>
    /// Converts a <see cref="TimeSpan"/> to <see cref="ulong"/> milliseconds.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="timeSpan"/> is not positive.</exception>
    public static ulong ToULongMilliseconds(TimeSpan timeSpan, string paramName)
    {
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(timeSpan, TimeSpan.Zero, paramName);

        // Use tick-based arithmetic to avoid floating-point precision loss.
        return (ulong)(timeSpan.Ticks / TimeSpan.TicksPerMillisecond);
    }
}
