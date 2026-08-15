// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.Commands.Options;

/// <summary>
/// Optional arguments for the <c>XRANGE</c> command.
/// </summary>
/// <seealso href="https://valkey.io/commands/xrange/"/>
/// <seealso href="https://valkey.io/commands/xrevrange/"/>
public sealed class StreamRangeOptions
{
    #region Public Properties

    /// <summary>
    /// The stream ID range to query.
    /// </summary>
    public StreamIdRange Range { get; init; } = StreamIdRange.All;

    /// <summary>
    /// The maximum number of matching entries to return.
    /// If not specified, all matching entries are returned.
    /// </summary>
    public int? Count { get; init; } = null;

    /// <summary>
    /// The order to return entries.
    /// </summary>
    public Order Order { get; init; } = Order.Ascending;

    #endregion
    #region Internal Methods

    /// <summary>
    /// Builds the command arguments for these options.
    /// </summary>
    internal GlideString[] ToArgs()
    {
        var start = Range.Start.Value;
        var end = Range.End.Value;

        // The start and end IDs are reversed for a descending (XREVRANGE) query.
        List<GlideString> args = Order == Order.Descending ? [end, start] : [start, end];

        if (Count.HasValue)
        {
            args.Add(ValkeyLiterals.COUNT);
            args.Add(Count.Value.ToGlideString());
        }

        return [.. args];
    }

    #endregion
}
