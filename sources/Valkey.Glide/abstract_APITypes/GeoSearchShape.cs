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
