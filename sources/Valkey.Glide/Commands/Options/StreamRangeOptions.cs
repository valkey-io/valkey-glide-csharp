// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.Commands.Options;

/// <summary>
/// Optional arguments for the XRANGE / XREVRANGE commands.
/// </summary>
/// <seealso href="https://valkey.io/commands/xrange/"/>
/// <seealso href="https://valkey.io/commands/xrevrange/"/>
public class StreamRangeOptions
{
    /// <summary>
    /// The minimum ID of the range (inclusive). Use <see cref="StreamConstants.ReadMinValue"/> for the smallest ID.
    /// Defaults to <c>"-"</c> (smallest ID) if <c>null</c>.
    /// </summary>
    public ValkeyValue? MinId { get; init; }

    /// <summary>
    /// The maximum ID of the range (inclusive). Use <see cref="StreamConstants.ReadMaxValue"/> for the largest ID.
    /// Defaults to <c>"+"</c> (largest ID) if <c>null</c>.
    /// </summary>
    public ValkeyValue? MaxId { get; init; }

    /// <summary>
    /// The maximum number of entries to return. If <c>null</c>, all matching entries are returned.
    /// </summary>
    public int? Count { get; init; }

    /// <summary>
    /// The order to return entries. <see cref="Order.Ascending"/> uses XRANGE,
    /// <see cref="Order.Descending"/> uses XREVRANGE. Defaults to <see cref="Order.Ascending"/>.
    /// </summary>
    public Order MessageOrder { get; init; } = Order.Ascending;
}
