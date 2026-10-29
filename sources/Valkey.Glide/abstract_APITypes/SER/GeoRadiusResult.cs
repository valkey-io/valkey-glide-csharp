// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide;

/// <summary>
/// The result of a GeoRadius command.
/// </summary>
/// <remarks>
/// Returns a new GeoRadiusResult.
/// </remarks>
/// <param name="member">The value from the result.</param>
/// <param name="distance">The distance from the result.</param>
/// <param name="hash">The hash of the result.</param>
/// <param name="position">The GeoPosition of the result.</param>
public readonly struct GeoRadiusResult(in ValkeyValue member, double? distance, long? hash, GeoPosition? position)
{
    /// <summary>
    /// The matched member.
    /// </summary>
    public ValkeyValue Member { get; } = member;

    /// <summary>
    /// The distance of the matched member from the center of the geo radius command.
    /// </summary>
    public double? Distance { get; } = distance;

    /// <summary>
    /// The hash value of the matched member as an integer. (The key in the sorted set).
    /// </summary>
    /// <remarks>Note that this is not the same as the hash returned from GeoHash.</remarks>
    public long? Hash { get; } = hash;

    /// <summary>
    /// The coordinates of the matched member.
    /// </summary>
    public GeoPosition? Position { get; } = position;

    /// <summary>
    /// Indicate the member being represented.
    /// </summary>
    public override string ToString() => Member.ToString();
}
