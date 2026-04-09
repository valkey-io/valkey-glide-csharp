// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide;

public partial interface IBaseClient
{
    /// <summary>
    /// Adds or changes a geospatial member.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/geoadd/"/>
    /// <param name="key">The sorted set key.</param>
    /// <param name="member">The member name to add or change.</param>
    /// <param name="position">The position of the member.</param>
    /// <param name="options">The options for adding or changing the member.</param>
    /// <returns><see langword="true"/> if the member was added (or changed, if
    /// <see cref="GeoAddOptions.Changed"/> is set), <see langword="false"/> otherwise.</returns>
    Task<bool> GeoAddAsync(
        ValkeyKey key,
        ValkeyValue member,
        GeoPosition position,
        GeoAddOptions options = default);

    /// <summary>
    /// Adds or changes geospatial members.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/geoadd/"/>
    /// <param name="key">The sorted set key.</param>
    /// <param name="members">A dictionary of member names and positions.</param>
    /// <param name="options">The options for adding or changing the members.</param>
    /// <returns>The number of members added (or changed, if
    /// <see cref="GeoAddOptions.Changed"/> is set).</returns>
    Task<long> GeoAddAsync(
        ValkeyKey key,
        IDictionary<ValkeyValue, GeoPosition> members,
        GeoAddOptions options = default);
}
