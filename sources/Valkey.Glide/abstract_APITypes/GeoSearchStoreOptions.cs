// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide;

/// <summary>
/// The options for a geospatial search and store operation.
/// </summary>
/// <seealso href="https://valkey.io/commands/geosearchstore/"/>
public readonly struct GeoSearchStoreOptions
{
    /// <summary>
    /// The sort order for results, or <see langword="null"/> for server default (ASC/DESC).
    /// </summary>
    public Order? Order { get; init; }

    /// <summary>
    /// The maximum number of results to store, or <see langword="null"/> for no limit (COUNT).
    /// </summary>
    public long? Count { get; init; }

    /// <summary>
    /// Whether to allow non-closest results when <see cref="Count"/> is set (ANY).
    /// </summary>
    public bool Any { get; init; }

    /// <summary>
    /// Whether to store distances as scores instead of geohash values (STOREDIST).
    /// </summary>
    public bool StoreDistances { get; init; }

    /// <summary>
    /// Converts to command arguments.
    /// </summary>
    internal readonly GlideString[] ToArgs()
    {
        List<GlideString> args = [];

        if (Order.HasValue)
        {
            args.Add(Order.Value.ToLiteral());
        }

        if (Count.HasValue)
        {
            args.Add(ValkeyLiterals.COUNT);
            args.Add(Count.Value.ToGlideString());

            if (Any)
            {
                args.Add(ValkeyLiterals.ANY);
            }
        }

        if (StoreDistances)
        {
            args.Add(ValkeyLiterals.STOREDIST);
        }

        return [.. args];
    }
}
