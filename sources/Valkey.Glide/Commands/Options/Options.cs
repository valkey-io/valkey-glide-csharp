// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.Commands.Options;

/// <summary>
/// Abstract base class for options.
/// </summary>
public abstract class Options
{
    /// <summary>
    /// Converts the options to command arguments.
    /// </summary>
    internal abstract GlideString[] ToArgs();

    /// <summary>
    /// Converts the given time span to milliseconds.
    /// <param name="timeSpan">The time span to convert.</param>
    /// <returns>The milliseconds in the time span.</returns>
    /// </summary>
    protected static GlideString ToMilliseconds(TimeSpan timeSpan)
        => (timeSpan.Ticks / TimeSpan.TicksPerMillisecond).ToGlideString();

    /// <summary>
    /// Converts the given date/time offset to UNIX milliseconds.
    /// <param name="timestamp">The timestamp to convert.</param>
    /// <returns>The UNIX milliseconds for the timestamp.</returns>
    /// </summary>
    protected static GlideString ToUnixMilliseconds(DateTimeOffset timestamp)
        => timestamp.ToUnixTimeMilliseconds().ToGlideString();
}
