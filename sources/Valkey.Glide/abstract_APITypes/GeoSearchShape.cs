// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide;

/// <summary>
/// A shape that defines the search area for geospatial commands.
/// </summary>
/// <remarks>
/// Constructs a <see cref="GeoSearchShape"/>.
/// </remarks>
/// <param name="unit">The geography unit to use.</param>
public abstract class GeoSearchShape(GeoUnit unit)
{
    /// <summary>
    /// The unit to use for the shape dimensions.
    /// </summary>
    protected GeoUnit Unit { get; } = unit;

    /// <summary>
    /// Converts to command arguments.
    /// </summary>
    internal abstract GlideString[] ToArgs();
}

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
