// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide;

/// <summary>
/// The result of a geospatial search operation corresponding to a matched geospatial member.
/// </summary>
/// <seealso href="https://valkey.io/commands/geosearch/"/>
public readonly struct GeoSearchResult
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
    public override string ToString() => Member.ToString();
}
