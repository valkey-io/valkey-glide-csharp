// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using static Valkey.Glide.Commands.Constants.Constants;

namespace Valkey.Glide;

/// <summary>
/// The condition for an operation to add or change a geospatial item.
/// </summary>
/// <seealso href="https://valkey.io/commands/geoadd/"/>
public enum GeoAddCondition
{
    /// <summary>
    /// Always add or change the geospatial item.
    /// </summary>
    Always,

    /// <summary>
    /// Only add the geospatial item if it does not exist (NX).
    /// </summary>
    OnlyIfNotExists,

    /// <summary>
    /// Only change the geospatial item if it exists (XX).
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
