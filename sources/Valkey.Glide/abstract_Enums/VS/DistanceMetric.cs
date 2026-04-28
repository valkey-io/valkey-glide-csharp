// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.Commands.Options;

/// <summary>
/// Represents the distance metric used to measure similarity between vectors.
/// </summary>
/// <seealso href="https://valkey.io/commands/ft.create/">valkey.io</seealso>
public enum DistanceMetric
{
    /// <summary>Euclidean distance.</summary>
    Euclidean,
    /// <summary>Inner product.</summary>
    InnerProduct,
    /// <summary>Cosine distance.</summary>
    Cosine,
}

internal static class DistanceMetricExtensions
{
    internal static GlideString ToLiteral(this DistanceMetric metric)
        => metric switch
        {
            DistanceMetric.Euclidean => "L2",
            DistanceMetric.InnerProduct => "IP",
            DistanceMetric.Cosine => "COSINE",
            _ => throw new ArgumentOutOfRangeException(nameof(metric)),
        };
}
