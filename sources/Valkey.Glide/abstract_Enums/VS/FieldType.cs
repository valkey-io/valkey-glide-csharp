// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.Commands.Options;

/// <summary>
/// Represents the data type of a field in a vector search index schema.
/// </summary>
/// <seealso href="https://valkey.io/commands/ft.create/">valkey.io</seealso>
public enum FieldType
{
    /// <summary>Full-text search field.</summary>
    Text,
    /// <summary>Tag field (delimited list of tags).</summary>
    Tag,
    /// <summary>Numeric field.</summary>
    Numeric,
    /// <summary>Vector field for similarity search.</summary>
    Vector,
}

internal static class FieldTypeExtensions
{
    internal static GlideString ToLiteral(this FieldType fieldType)
        => fieldType switch
        {
            FieldType.Text => "TEXT",
            FieldType.Tag => "TAG",
            FieldType.Numeric => "NUMERIC",
            FieldType.Vector => "VECTOR",
            _ => throw new ArgumentOutOfRangeException(nameof(fieldType)),
        };
}
