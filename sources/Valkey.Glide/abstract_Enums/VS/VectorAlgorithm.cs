// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.Commands.Options;

/// <summary>
/// Represents the algorithm used for vector similarity search.
/// </summary>
/// <seealso href="https://valkey.io/commands/ft.create/">valkey.io</seealso>
public enum VectorAlgorithm
{
    /// <summary>Brute force (flat) algorithm.</summary>
    Flat,
    /// <summary>Hierarchical Navigable Small World algorithm.</summary>
    Hnsw,
}

internal static class VectorAlgorithmExtensions
{
    internal static GlideString ToLiteral(this VectorAlgorithm algorithm)
        => algorithm switch
        {
            VectorAlgorithm.Flat => "FLAT",
            VectorAlgorithm.Hnsw => "HNSW",
            _ => throw new ArgumentOutOfRangeException(nameof(algorithm)),
        };
}
