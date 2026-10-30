// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide;

/// <summary>
/// A circular search area for geospatial commands.
/// </summary>
/// <remarks>
/// Creates a <see cref="GeoSearchCircle"/>.
/// </remarks>
/// <param name="radius">The radius of the circle.</param>
/// <param name="unit">The distance unit.</param>
public class GeoSearchCircle(double radius, GeoUnit unit = GeoUnit.Meters) : GeoSearchShape(unit)
{
    private readonly double _radius = radius;

    /// <inheritdoc/>
    internal override GlideString[] ToArgs() =>
    [
        ValkeyLiterals.BYRADIUS,
        _radius.ToGlideString(),
        Unit.ToLiteral()
    ];
}
