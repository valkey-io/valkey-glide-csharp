// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide;

/// <summary>
/// The options for a geospatial search operation.
/// </summary>
/// <seealso href="https://valkey.io/commands/geosearch/"/>
public readonly struct GeoSearchOptions
{
    /// <summary>
    /// The sort order for results, or <see langword="null"/> for server default (ASC/DESC).
    /// </summary>
    public Order? Order { get; init; }

    /// <summary>
    /// The maximum number of results to return, or <see langword="null"/> for no limit (COUNT).
    /// </summary>
    public long? Count { get; init; }

    /// <summary>
    /// Whether to allow non-closest results when <see cref="Count"/> is set (ANY).
    /// </summary>
    public bool Any { get; init; }

    /// <summary>
    /// Whether to include the position in each result (WITHCOORD).
    /// </summary>
    public bool WithPosition { get; init; }

    /// <summary>
    /// Whether to include the distance from the search origin in each result (WITHDIST).
    /// </summary>
    public bool WithDistance { get; init; }

    /// <summary>
    /// Whether to include the geohash integer in each result (WITHHASH).
    /// </summary>
    public bool WithHash { get; init; }

    /// <summary>
    /// Converts to command arguments.
    /// </summary>
    internal readonly GlideString[] ToArgs()
    {
        if (Any && !Count.HasValue)
        {
            throw new ArgumentException($"{nameof(Any)} requires {nameof(Count)} to be set.");
        }

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

        if (WithPosition)
        {
            args.Add(ValkeyLiterals.WITHCOORD);
        }

        if (WithDistance)
        {
            args.Add(ValkeyLiterals.WITHDIST);
        }

        if (WithHash)
        {
            args.Add(ValkeyLiterals.WITHHASH);
        }

        return [.. args];
    }
}
