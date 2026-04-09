// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide;

/// <summary>
/// A shape that defines the search area for geospatial commands.
/// </summary>
public abstract class GeoSearchShape
{
    /// <summary>
    /// The unit to use for the shape dimensions.
    /// </summary>
    protected GeoUnit Unit { get; }

    /// <summary>
    /// Constructs a <see cref="GeoSearchShape"/>.
    /// </summary>
    /// <param name="unit">The geography unit to use.</param>
    protected GeoSearchShape(GeoUnit unit)
    {
        Unit = unit;
    }

    /// <summary>
    /// Converts to command arguments.
    /// </summary>
    internal abstract GlideString[] ToArgs();
}

/// <summary>
/// A circular search area for geospatial commands.
/// </summary>
public class GeoSearchCircle : GeoSearchShape
{
    private readonly double _radius;

    /// <summary>
    /// Creates a <see cref="GeoSearchCircle"/>.
    /// </summary>
    /// <param name="radius">The radius of the circle.</param>
    /// <param name="unit">The distance unit, defaults to Meters.</param>
    public GeoSearchCircle(double radius, GeoUnit unit = GeoUnit.Meters) : base(unit)
    {
        _radius = radius;
    }

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
public class GeoSearchBox : GeoSearchShape
{
    private readonly double _height;
    private readonly double _width;

    /// <summary>
    /// Creates a <see cref="GeoSearchBox"/>.
    /// </summary>
    /// <param name="height">The height of the box.</param>
    /// <param name="width">The width of the box.</param>
    /// <param name="unit">The distance unit, defaults to metres.</param>
    public GeoSearchBox(double height, double width, GeoUnit unit = GeoUnit.Meters) : base(unit)
    {
        _height = height;
        _width = width;
    }

    /// <inheritdoc/>
    internal override GlideString[] ToArgs() =>
    [
        ValkeyLiterals.BYBOX,
        _width.ToGlideString(),
        _height.ToGlideString(),
        Unit.ToLiteral()
    ];
}
