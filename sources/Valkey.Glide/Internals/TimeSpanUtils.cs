// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.Internals;

/// <summary>
/// Utility methods for converting <see cref="TimeSpan"/> values to Valkey command arguments.
/// </summary>
internal static class TimeSpanUtils
{
    /// <summary>
    /// Converts a <see cref="TimeSpan"/> to <see cref="ulong"/> milliseconds.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown if <paramref name="timeSpan"/> is negative.</exception>
    public static ulong ToUlongMilliseconds(TimeSpan timeSpan)
        => timeSpan < TimeSpan.Zero
            ? throw new ArgumentException("Time span cannot be negative.")
            : (ulong)(timeSpan.Ticks / TimeSpan.TicksPerMillisecond);

    /// <summary>
    /// Converts a <see cref="TimeSpan"/> to <see cref="GlideString"/> milliseconds.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown if <paramref name="timeSpan"/> is negative.</exception>
    public static GlideString ToGlideStringMilliseconds(TimeSpan timeSpan)
        => ToUlongMilliseconds(timeSpan).ToGlideString();

    /// <summary>
    /// Converts a <see cref="TimeSpan"/> to <see cref="GlideString"/> seconds.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown if <paramref name="timeSpan"/> is negative.</exception>
    public static GlideString ToGlideStringSeconds(TimeSpan timeSpan)
        => timeSpan < TimeSpan.Zero
            ? throw new ArgumentException("Time span cannot be negative.")
            : timeSpan.TotalSeconds.ToGlideString();
}
