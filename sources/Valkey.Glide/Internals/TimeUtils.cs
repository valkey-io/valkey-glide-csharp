// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.Internals;

/// <summary>
/// Time utility methods.
/// </summary>
internal static class TimeUtils
{
    #region To Uint

    private static readonly TimeSpan MaxUintMilliseconds = TimeSpan.FromMilliseconds(uint.MaxValue);

    /// <summary>
    /// Converts a <see cref="TimeSpan"/> to <see cref="uint"/> milliseconds.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown if <paramref name="timeSpan"/> is not positive or exceeds <see cref="uint.MaxValue"/> milliseconds.
    /// </exception>
    public static uint ToPositiveUintMs(TimeSpan timeSpan, string paramName)
    {
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(timeSpan, TimeSpan.Zero, paramName);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(timeSpan, MaxUintMilliseconds, paramName);

        // Use tick-based arithmetic to avoid floating-point precision loss.
        return (uint)(timeSpan.Ticks / TimeSpan.TicksPerMillisecond);
    }

    /// <summary>
    /// Converts a <see cref="TimeSpan"/> to <see cref="uint"/> seconds, rounded to the nearest second.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="timeSpan"/> is not positive or exceeds <see cref="uint.MaxValue"/> seconds.</exception>
    public static uint ToPositiveUintSecs(TimeSpan timeSpan, string paramName)
    {
        var secs = Math.Round(timeSpan.TotalSeconds);

        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(timeSpan, TimeSpan.Zero, paramName);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(secs, uint.MaxValue, paramName);

        return (uint)secs;
    }

    #endregion To Uint
    #region To ULong

    /// <summary>
    /// Converts a <see cref="TimeSpan"/> to <see cref="ulong"/> milliseconds.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="timeSpan"/> is not positive.</exception>
    public static ulong ToPositiveULongMs(TimeSpan timeSpan, string paramName)
    {
        ArgumentOutOfRangeException.ThrowIfEqual(timeSpan, TimeSpan.Zero, paramName);
        return ToULongMs(timeSpan, paramName);
    }

    /// <summary>
    /// Converts a <see cref="TimeSpan"/> to <see cref="ulong"/> milliseconds.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="timeSpan"/> is negative.</exception>
    public static ulong ToULongMs(TimeSpan timeSpan, string paramName)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(timeSpan, TimeSpan.Zero, paramName);

        // Use tick-based arithmetic to avoid floating-point precision loss.
        return (ulong)(timeSpan.Ticks / TimeSpan.TicksPerMillisecond);
    }

    /// <summary>
    /// Converts a <see cref="TimeSpan"/> to <see cref="ulong"/> seconds, rounded to the nearest second.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="timeSpan"/> is not positive.</exception>
    public static ulong ToPositiveULongSecs(TimeSpan timeSpan, string paramName)
    {
        ArgumentOutOfRangeException.ThrowIfEqual(timeSpan, TimeSpan.Zero, paramName);
        return ToULongSecs(timeSpan, paramName);
    }

    /// <summary>
    /// Converts a <see cref="TimeSpan"/> to <see cref="ulong"/> seconds, rounded to the nearest second.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="timeSpan"/> is negative.</exception>
    public static ulong ToULongSecs(TimeSpan timeSpan, string paramName)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(timeSpan, TimeSpan.Zero, paramName);
        return (ulong)Math.Round(timeSpan.TotalSeconds);
    }

    #endregion To ULong
    #region To Double

    /// <summary>
    /// Converts a <see cref="TimeSpan"/> to <see cref="double"/> seconds.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="timeSpan"/> is negative.</exception>
    public static double ToNonNegativeDoubleSecs(TimeSpan timeSpan, string paramName)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(timeSpan, TimeSpan.Zero, paramName);
        return timeSpan.TotalSeconds;
    }

    #endregion To Double
}
