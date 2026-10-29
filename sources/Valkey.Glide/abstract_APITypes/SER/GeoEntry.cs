// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide;

/// <summary>
/// Describes a GeoEntry element with the corresponding value.
/// GeoEntries are stored as SortedSetEntries.
/// </summary>
/// <remarks>
/// Initializes a GeoEntry value.
/// </remarks>
/// <param name="longitude">The longitude position to use.</param>
/// <param name="latitude">The latitude position to use.</param>
/// <param name="member">The value to store for this position.</param>
public readonly struct GeoEntry(double longitude, double latitude, ValkeyValue member) : IEquatable<GeoEntry>
{
    /// <summary>
    /// The name of the GeoEntry.
    /// </summary>
    public ValkeyValue Member { get; } = member;

    /// <summary>
    /// Describes the longitude and latitude of a GeoEntry.
    /// </summary>
    public GeoPosition Position { get; } = new GeoPosition(longitude, latitude);

    /// <summary>
    /// The longitude of the GeoEntry.
    /// </summary>
    public double Longitude => Position.Longitude;

    /// <summary>
    /// The latitude of the GeoEntry.
    /// </summary>
    public double Latitude => Position.Latitude;

    /// <summary>
    /// Compares two values for equality.
    /// </summary>
    /// <param name="x">The first entry to compare.</param>
    /// <param name="y">The second entry to compare.</param>
    public static bool operator ==(GeoEntry x, GeoEntry y) => x.Position == y.Position && x.Member == y.Member;

    /// <summary>
    /// Compares two values for non-equality.
    /// </summary>
    /// <param name="x">The first entry to compare.</param>
    /// <param name="y">The second entry to compare.</param>
    public static bool operator !=(GeoEntry x, GeoEntry y) => x.Position != y.Position || x.Member != y.Member;

    /// <summary>
    /// A "({Longitude},{Latitude})={Member}" string representation of this entry.
    /// </summary>
    public override string ToString() => $"({Longitude},{Latitude})={Member}";

    /// <inheritdoc/>
    public override int GetHashCode() => Position.GetHashCode() ^ Member.GetHashCode();

    /// <summary>
    /// Compares two values for equality.
    /// </summary>
    /// <param name="obj">The <see cref="GeoEntry"/> to compare to.</param>
    public override bool Equals(object? obj) => obj is GeoEntry geObj && Equals(geObj);

    /// <summary>
    /// Compares two values for equality.
    /// </summary>
    /// <param name="other">The <see cref="GeoEntry"/> to compare to.</param>
    public bool Equals(GeoEntry other) => this == other;
}
