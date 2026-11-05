// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide;

/// <summary>
/// A rectangular search area for geospatial commands.
/// </summary>
/// <remarks>
/// Creates a <see cref="GeoSearchBox"/>.
/// </remarks>
/// <param name="height">The height of the box.</param>
/// <param name="width">The width of the box.</param>
/// <param name="unit">The distance unit.</param>
public class GeoSearchBox(double height, double width, GeoUnit unit = GeoUnit.Meters) : GeoSearchShape(unit)
{
    private readonly double _height = height;
    private readonly double _width = width;

    /// <inheritdoc/>
    internal override GlideString[] ToArgs() =>
    [
        ValkeyLiterals.BYBOX,
        _width.ToGlideString(),
        _height.ToGlideString(),
        Unit.ToLiteral()
    ];
}
