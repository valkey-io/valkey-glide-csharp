// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide;

/// <summary>
/// The options for an operation to add or change a geospatial item.
/// </summary>
/// <seealso href="https://valkey.io/commands/geoadd/"/>
public readonly struct GeoAddOptions
{
    /// <summary>
    /// The condition under which to add or change members.
    /// </summary>
    public GeoAddCondition Condition { get; init; }

    /// <summary>
    /// Whether to return the number of changed elements (added and
    /// updated) instead of the number of added elements only (CH).
    /// </summary>
    public bool Changed { get; init; }

    /// <summary>
    /// Converts to command arguments.
    /// </summary>
    internal GlideString[] ToArgs()
    {
        List<GlideString> args = [];
        args.AddRange(Condition.ToArgs());

        if (Changed)
        {
            args.Add(ValkeyLiterals.CH);
        }

        return [.. args];
    }
}
