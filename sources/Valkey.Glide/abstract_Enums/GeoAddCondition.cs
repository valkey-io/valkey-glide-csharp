// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using static Valkey.Glide.Commands.Constants.Constants;

namespace Valkey.Glide;

/// <summary>
/// The condition for an operation to add or update geospatial items.
/// </summary>
/// <seealso href="https://valkey.io/commands/geoadd/"/>
public enum GeoAddCondition
{
    /// <summary>
    /// Always add or update the geospatial items.
    /// </summary>
    Always,

    /// <summary>
    /// Only add new geospatial items (NX).
    /// </summary>
    OnlyIfNotExists,

    /// <summary>
    /// Only update existing geospatial items (XX).
    /// </summary>
    OnlyIfExists,
}

internal static class GeoAddConditionExtensions
{
    /// <summary>
    /// Converts to command arguments.
    /// </summary>
    internal static GlideString[] ToArgs(this GeoAddCondition condition)
        => condition switch
        {
            GeoAddCondition.Always => [],
            GeoAddCondition.OnlyIfNotExists => [NxKeyword],
            GeoAddCondition.OnlyIfExists => [XxKeyword],
            _ => throw new ArgumentOutOfRangeException(nameof(condition)),
        };
}
