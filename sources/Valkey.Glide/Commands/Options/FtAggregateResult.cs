// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.Commands.Options;

/// <summary>
/// Represents a single result row returned by <c>IBaseClient.FtAggregateAsync</c>.
/// Each row is a flat set of field/value pairs produced by the aggregate pipeline.
/// <para>
/// Values are typed as <see cref="ValkeyValue"/> to preserve binary data. The glide-core
/// layer does not coerce FT.AGGREGATE values, so the actual runtime type depends on the
/// server and RESP protocol version.
/// </para>
/// </summary>
/// <seealso href="https://valkey.io/commands/ft.aggregate/">valkey.io</seealso>
public sealed class FtAggregateRow(IReadOnlyDictionary<ValkeyValue, ValkeyValue> fields)
{
    /// <summary>The field/value pairs for this result row.</summary>
    public IReadOnlyDictionary<ValkeyValue, ValkeyValue> Fields { get; } = fields;

    /// <summary>
    /// Gets the value of a field by name, or <see cref="ValkeyValue.Null"/> if the field is not present.
    /// </summary>
    public ValkeyValue this[ValkeyValue field] => Fields.TryGetValue(field, out ValkeyValue value) ? value : ValkeyValue.Null;
}
