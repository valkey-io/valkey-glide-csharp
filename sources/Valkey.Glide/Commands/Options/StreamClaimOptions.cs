// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.Commands.Options;

/// <summary>
/// Optional arguments for the XCLAIM command.
/// </summary>
/// <seealso href="https://valkey.io/commands/xclaim/"/>
public class StreamClaimOptions
{
    /// <summary>
    /// Set the idle time (last delivery time) of the message. Equivalent to the IDLE option.
    /// </summary>
    public TimeSpan? Idle { get; init; }

    /// <summary>
    /// Set the idle time to a specific Unix timestamp. Equivalent to the TIME option.
    /// </summary>
    public DateTimeOffset? IdleUnix { get; init; }

    /// <summary>
    /// Set the retry counter to the specified value. Equivalent to the RETRYCOUNT option.
    /// </summary>
    public int? RetryCount { get; init; }

    /// <summary>
    /// Create a PEL entry even if the message is not already assigned to a consumer. Equivalent to the FORCE option.
    /// </summary>
    public bool Force { get; init; }

    internal GlideString[] ToArgs()
    {
        List<GlideString> args = [];

        if (Idle.HasValue)
        {
            args.Add(ValkeyLiterals.IDLE.ToGlideString());
            args.Add(ToMilliseconds(Idle.Value).ToGlideString());
        }

        if (IdleUnix.HasValue)
        {
            args.Add(ValkeyLiterals.TIME.ToGlideString());
            args.Add(IdleUnix.Value.ToUnixTimeMilliseconds().ToGlideString());
        }

        if (RetryCount.HasValue)
        {
            args.Add(ValkeyLiterals.RETRYCOUNT.ToGlideString());
            args.Add(RetryCount.Value.ToGlideString());
        }

        if (Force)
        {
            args.Add(ValkeyLiterals.FORCE.ToGlideString());
        }

        return [.. args];
    }

    /// <summary>
    /// Converts the given time span to milliseconds without truncation.
    /// </summary>
    private static long ToMilliseconds(TimeSpan timeSpan)
        => timeSpan.Ticks / TimeSpan.TicksPerMillisecond;
}
