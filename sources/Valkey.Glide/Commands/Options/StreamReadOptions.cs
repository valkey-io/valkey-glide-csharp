// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.Commands.Options;

/// <summary>
/// Optional arguments for the XREAD command.
/// </summary>
/// <seealso href="https://valkey.io/commands/xread/"/>
public class StreamReadOptions
{
    /// <summary>
    /// The maximum number of entries to return per stream.
    /// Equivalent to the COUNT option.
    /// </summary>
    public int? Count { get; init; }

    /// <summary>
    /// If set, the request will block for the specified duration or until the server has the required
    /// number of entries. A value of <see cref="TimeSpan.Zero"/> blocks indefinitely.
    /// Equivalent to the BLOCK option.
    /// </summary>
    public TimeSpan? Block { get; init; }

    internal virtual GlideString[] ToArgs()
    {
        List<GlideString> args = [];

        if (Count.HasValue)
        {
            args.Add(ValkeyLiterals.COUNT.ToGlideString());
            args.Add(Count.Value.ToGlideString());
        }

        if (Block.HasValue)
        {
            args.Add(ValkeyLiterals.BLOCK.ToGlideString());
            args.Add(ToMilliseconds(Block.Value).ToGlideString());
        }

        return [.. args];
    }

    /// <summary>
    /// Converts the given time span to milliseconds without truncation.
    /// </summary>
    private protected static long ToMilliseconds(TimeSpan timeSpan)
        => timeSpan.Ticks / TimeSpan.TicksPerMillisecond;
}

/// <summary>
/// Optional arguments for the XREADGROUP command.
/// </summary>
/// <seealso href="https://valkey.io/commands/xreadgroup/"/>
public class StreamReadGroupOptions : StreamReadOptions
{
    /// <summary>
    /// If <c>true</c>, messages are not added to the Pending Entries List (PEL).
    /// This is equivalent to acknowledging the message when it is read.
    /// Equivalent to the NOACK option.
    /// </summary>
    public bool NoAck { get; init; }

    internal override GlideString[] ToArgs()
    {
        List<GlideString> args = [.. base.ToArgs()];

        if (NoAck)
        {
            args.Add(ValkeyLiterals.NOACK.ToGlideString());
        }

        return [.. args];
    }
}
