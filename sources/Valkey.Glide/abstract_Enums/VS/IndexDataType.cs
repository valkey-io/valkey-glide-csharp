// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.Commands.Options;

/// <summary>
/// Represents the type of the index dataset.
/// </summary>
/// <seealso href="https://valkey.io/commands/ft.create/">valkey.io</seealso>
public enum IndexDataType
{
    /// <summary>Data stored in hashes; field identifiers are field names within the hashes.</summary>
    Hash,
    /// <summary>Data stored as JSON documents; field identifiers are JSON Path expressions.</summary>
    Json,
}

internal static class IndexDataTypeExtensions
{
    internal static GlideString ToLiteral(this IndexDataType dataType)
        => dataType switch
        {
            IndexDataType.Hash => "HASH",
            IndexDataType.Json => "JSON",
            _ => throw new ArgumentOutOfRangeException(nameof(dataType)),
        };
}
