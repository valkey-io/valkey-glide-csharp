// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.Commands.Options;

/// <summary>
/// Optional arguments for the XRANGE / XREVRANGE commands.
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

    /// <inheritdoc/>
    internal GlideString[] ToArgs()
        => Count.HasValue ? [ValkeyLiterals.COUNT, Count.Value.ToGlideString()] : [];

    #endregion
}
