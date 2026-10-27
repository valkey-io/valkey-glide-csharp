// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide;

/// <summary>
/// Describes the longitude and latitude of a GeoEntry.
/// </summary>
/// <remarks>
/// Creates a new GeoPosition.
/// </remarks>
/// <param name="longitude">The longitude of the position.</param>
/// <param name="latitude">The latitude of the position.</param>
public readonly struct GeoPosition(double longitude, double latitude) : IEquatable<GeoPosition>
{
    internal static string GetValkeyUnit(GeoUnit unit) => unit switch
    {
        GeoUnit.Meters => "m",
        GeoUnit.Kilometers => "km",
        GeoUnit.Feet => "ft",
        GeoUnit.Miles => "mi",
        _ => throw new ArgumentOutOfRangeException(nameof(unit)),
    };

    /// <summary>
    /// The Latitude of the GeoPosition.
    /// </summary>
    public double Latitude { get; } = latitude;

    /// <summary>
    /// The Longitude of the GeoPosition.
    /// </summary>
    public double Longitude { get; } = longitude;

    /// <summary>
    /// A "{long} {lat}" string representation of this position.
    /// </summary>
    public override string ToString() => string.Format("{0} {1}", Longitude, Latitude);

    /// <summary>
    /// See <see cref="object.GetHashCode"/>.
    /// Diagonals not an issue in the case of lat/long.
    /// </summary>
    /// <remarks>
    /// Diagonals are not an issue in the case of lat/long.
    /// </remarks>
    public override int GetHashCode() => Longitude.GetHashCode() ^ Latitude.GetHashCode();

    /// <summary>
    /// Compares two values for equality.
    /// </summary>
    /// <param name="obj">The <see cref="GeoPosition"/> to compare to.</param>
    public override bool Equals(object? obj) => obj is GeoPosition gpObj && Equals(gpObj);

    /// <summary>
    /// Compares two values for equality.
    /// </summary>
    /// <param name="other">The <see cref="GeoPosition"/> to compare to.</param>
    public bool Equals(GeoPosition other) => this == other;

    /// <summary>
    /// Compares two values for equality.
    /// </summary>
    /// <param name="x">The first position to compare.</param>
    /// <param name="y">The second position to compare.</param>
    public static bool operator ==(GeoPosition x, GeoPosition y) => x.Longitude == y.Longitude && x.Latitude == y.Latitude;

    /// <summary>
    /// Compares two values for non-equality.
    /// </summary>
    /// <param name="x">The first position to compare.</param>
    /// <param name="y">The second position to compare.</param>
    public static bool operator !=(GeoPosition x, GeoPosition y) => x.Longitude != y.Longitude || x.Latitude != y.Latitude;
}
