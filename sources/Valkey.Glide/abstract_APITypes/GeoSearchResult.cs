// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide;

/// <summary>
/// The result of a geospatial search operation corresponding to a matched geospatial member.
/// </summary>
/// <seealso href="https://valkey.io/commands/geosearch/"/>
public readonly struct GeoSearchResult : IEquatable<GeoSearchResult>
{
    /// <summary>
    /// The member name.
    /// </summary>
    public ValkeyValue Member { get; }

    /// <summary>
    /// The coordinates of the member, or <see langword="null"/> if not requested.
    /// </summary>
    public GeoPosition? Position { get; }

    /// <summary>
    /// The distance from the member to the search origin, or <see langword="null"/> if not requested.
    /// </summary>
    public double? Distance { get; }

    /// <summary>
    /// The geohash integer of the member, or <see langword="null"/> if not requested.
    /// </summary>
    public long? Hash { get; }

    /// <summary>
    /// Initializes a new <see cref="GeoSearchResult"/>.
    /// </summary>
    /// <param name="member">The matched member name.</param>
    /// <param name="position">The coordinates of the member, or <see langword="null"/> if not requested.</param>
    /// <param name="distance">The distance from the search origin, or <see langword="null"/> if not requested.</param>
    /// <param name="hash">The geohash integer of the member, or <see langword="null"/> if not requested.</param>
    internal GeoSearchResult(
        ValkeyValue member,
        GeoPosition? position = null,
        double? distance = null,
        long? hash = null)
    {
        Member = member;
        Position = position;
        Distance = distance;
        Hash = hash;
    }

    /// <inheritdoc/>
    public override string ToString()
        => Member.ToString();

    /// <inheritdoc/>
    public bool Equals(GeoSearchResult other)
        => Member == other.Member
        && Position.Equals(other.Position)
        && Distance.Equals(other.Distance)
        && Hash.Equals(other.Hash);

    /// <inheritdoc/>
    public override bool Equals(object? obj)
        => obj is GeoSearchResult other && Equals(other);

    /// <inheritdoc/>
    public override int GetHashCode()
        => HashCode.Combine(Member, Position, Distance, Hash);

    /// <summary>
    /// Equality operator.
    /// </summary>
    /// <param name="left">The first result to compare.</param>
    /// <param name="right">The second result to compare.</param>
    public static bool operator ==(GeoSearchResult left, GeoSearchResult right)
        => left.Equals(right);

    /// <summary>
    /// Inequality operator.
    /// </summary>
    /// <param name="left">The first result to compare.</param>
    /// <param name="right">The second result to compare.</param>
    public static bool operator !=(GeoSearchResult left, GeoSearchResult right)
        => !left.Equals(right);
}
