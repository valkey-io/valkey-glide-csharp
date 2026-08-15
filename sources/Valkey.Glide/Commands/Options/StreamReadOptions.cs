// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using static Valkey.Glide.Internals.TimeUtils;

namespace Valkey.Glide.Commands.Options;

/// <summary>
/// Optional arguments for the <c>XREAD</c> command.
/// </summary>
/// <seealso href="https://valkey.io/commands/xread/"/>
public class StreamReadOptions
{
    #region Public Properties

    /// <summary>
    /// If specified, the maximum number of entries to return per stream (COUNT).
    /// </summary>
    public int? Count { get; init; } = null;

    /// <summary>
    /// If set, the request will block for the specified duration or until new entries are available.
    /// A value of <see cref="TimeSpan.Zero"/> blocks indefinitely (BLOCK).
    /// </summary>
    public TimeSpan? Block { get; init; } = null;

    #endregion
    #region Internal Methods

    /// <summary>
    /// Builds the command arguments for these options.
    /// </summary>
    internal virtual GlideString[] ToArgs()
    {
        List<GlideString> args = [];

        if (Count.HasValue)
        {
            args.Add(ValkeyLiterals.COUNT);
            args.Add(Count.Value.ToGlideString());
        }

        if (Block.HasValue)
        {
            args.Add(ValkeyLiterals.BLOCK);
            args.Add(ToULongMs(Block.Value, nameof(Block)).ToGlideString());
        }

        return [.. args];
    }

    #endregion
}
