// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide;

/// <summary>
/// Specifies how elements should be aggregated when combining sorted sets.
/// </summary>
public enum Aggregate
{
    /// <summary>
    /// The values of the combined elements are added.
    /// </summary>
    Sum,

    /// <summary>
    /// The least value of the combined elements is used.
    /// </summary>
    Min,

    /// <summary>
    /// The greatest value of the combined elements is used.
    /// </summary>
    Max,
}

internal static class AggregateExtensions
{
    /// <summary>
    /// Converts to command arguments.
    /// </summary>
    internal static GlideString[] ToArgs(this Aggregate aggregate)
        => aggregate switch
        {
            Aggregate.Sum => [],
            Aggregate.Min => [ValkeyLiterals.AGGREGATE, ValkeyLiterals.MIN],
            Aggregate.Max => [ValkeyLiterals.AGGREGATE, ValkeyLiterals.MAX],
            _ => throw new ArgumentOutOfRangeException(nameof(aggregate)),
        };
}
