// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide;

/// <summary>
/// The direction in which to sort elements.
/// </summary>
/// <remarks>
/// Unlike <see cref="Order"/>, this enum includes a <see cref="Default"/> option
/// that omits the order argument, letting the server use its default (ascending).
/// </remarks>
public enum SortOrder
{
    /// <summary>
    /// Use the server default (ascending).
    /// </summary>
    Default,

    /// <summary>
    /// Ordered from low values to high values (ASC).
    /// </summary>
    Ascending,

    /// <summary>
    /// Ordered from high values to low values (DESC).
    /// </summary>
    Descending,
}

// TODO #360 - default is incorrect!
internal static class SortOrderExtensions
{
    /// <summary>
    /// Converts SortOrder to Order for use with methods that require Order.
    /// Default maps to Ascending since that's the server default.
    /// </summary>
    internal static Order ToOrder(this SortOrder sortOrder) => sortOrder switch
    {
        SortOrder.Default => Order.Ascending,
        SortOrder.Ascending => Order.Ascending,
        SortOrder.Descending => Order.Descending,
        _ => throw new ArgumentOutOfRangeException(nameof(sortOrder)),
    };
}
