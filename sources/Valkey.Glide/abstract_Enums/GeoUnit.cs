// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide;

/// <summary>
/// Units associated with Geo Commands.
/// </summary>
public enum GeoUnit
{
    /// <summary>
    /// Meters.
    /// </summary>
    Meters,

    /// <summary>
    /// Kilometers.
    /// </summary>
    Kilometers,

    /// <summary>
    /// Miles.
    /// </summary>
    Miles,

    /// <summary>
    /// Feet.
    /// </summary>
    Feet,
}

internal static class GeoUnitExtensions
{
    internal static ValkeyValue ToLiteral(this GeoUnit unit) => unit switch
    {
        GeoUnit.Meters => ValkeyLiterals.Meters,
        GeoUnit.Kilometers => ValkeyLiterals.Kilometers,
        GeoUnit.Feet => ValkeyLiterals.Feet,
        GeoUnit.Miles => ValkeyLiterals.Miles,
        _ => throw new ArgumentOutOfRangeException(nameof(unit)),
    };
}
