// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.Commands.Options;

/// <summary>
/// Represents a single result row returned by <see cref="IVectorSearchCommands.FtAggregateAsync"/>.
/// Each row is a flat set of field/value pairs produced by the aggregate pipeline.
/// <para>
/// Values are typed as <see cref="object"/> because the type depends on the pipeline stage:
/// fields loaded from documents are <see cref="string"/>, while reducer outputs
/// (e.g. COUNT, AVG, SUM) are <see cref="double"/>.
/// </para>
/// </summary>
/// <seealso href="https://valkey.io/commands/ft.aggregate/">valkey.io</seealso>
public class FtAggregateRow(IReadOnlyDictionary<string, object> fields)
{
    /// <summary>The field/value pairs for this result row.</summary>
    public IReadOnlyDictionary<string, object> Fields { get; } = fields;

    /// <summary>
    /// Gets the value of a field by name, or <see langword="null"/> if the field is not present.
    /// </summary>
    public object? this[string field] => Fields.TryGetValue(field, out object? value) ? value : null;
}
