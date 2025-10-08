// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using static Valkey.Glide.Internals.FFI;

namespace Valkey.Glide.Internals;

internal static partial class Request
{
    /// <summary>
    /// Creates a request for GEOADD command.
    /// </summary>
    /// <param name="key">The key of the sorted set.</param>
    /// <param name="value">The geospatial item to add.</param>
    /// <returns>A <see cref="Cmd{T, R}"/> with the request.</returns>
    public static Cmd<long, bool> GeoAddAsync(ValkeyKey key, GeoEntry value)
    {
        GlideString[] args = [key.ToGlideString(), value.Longitude.ToGlideString(), value.Latitude.ToGlideString(), value.Member.ToGlideString()];
        return Boolean<long>(RequestType.GeoAdd, args);
    }

    /// <summary>
    /// Creates a request for GEOADD command with multiple values.
    /// </summary>
    /// <param name="key">The key of the sorted set.</param>
    /// <param name="values">The geospatial items to add.</param>
    /// <returns>A <see cref="Cmd{T, R}"/> with the request.</returns>
    public static Cmd<long, long> GeoAddAsync(ValkeyKey key, GeoEntry[] values)
    {
        List<GlideString> args = [key.ToGlideString()];
        
        foreach (var value in values)
        {
            args.Add(value.Longitude.ToGlideString());
            args.Add(value.Latitude.ToGlideString());
            args.Add(value.Member.ToGlideString());
        }

        return Simple<long>(RequestType.GeoAdd, [.. args]);
    }

    /// <summary>
    /// Creates a request for GEODIST command.
    /// </summary>
    /// <param name="key">The key of the sorted set.</param>
    /// <param name="member1">The first member.</param>
    /// <param name="member2">The second member.</param>
    /// <param name="unit">The unit of distance.</param>
    /// <returns>A <see cref="Cmd{T, R}"/> with the request.</returns>
    public static Cmd<double?, double?> GeoDistanceAsync(ValkeyKey key, ValkeyValue member1, ValkeyValue member2, GeoUnit unit)
    {
        GlideString[] args = [key.ToGlideString(), member1.ToGlideString(), member2.ToGlideString(), unit.ToLiteral()];
        return Simple<double?>(RequestType.GeoDist, args, true);
    }

    /// <summary>
    /// Creates a request for GEOHASH command for a single member.
    /// </summary>
    /// <param name="key">The key of the sorted set.</param>
    /// <param name="member">The member to get the geohash for.</param>
    /// <returns>A <see cref="Cmd{T, R}"/> with the request.</returns>
    public static Cmd<object[], string?> GeoHashAsync(ValkeyKey key, ValkeyValue member)
    {
        GlideString[] args = [key.ToGlideString(), member.ToGlideString()];
        return new(RequestType.GeoHash, args, false, response => response.Length > 0 ? response[0]?.ToString() : null);
    }

    /// <summary>
    /// Creates a request for GEOHASH command for multiple members.
    /// </summary>
    /// <param name="key">The key of the sorted set.</param>
    /// <param name="members">The members to get the geohashes for.</param>
    /// <returns>A <see cref="Cmd{T, R}"/> with the request.</returns>
    public static Cmd<object[], string?[]> GeoHashAsync(ValkeyKey key, ValkeyValue[] members)
    {
        GlideString[] args = [key.ToGlideString(), .. members.Select(m => m.ToGlideString())];
        return new(RequestType.GeoHash, args, false, response => response.Select(item => item?.ToString()).ToArray());
    }

    /// <summary>
    /// Creates a request for GEOPOS command for a single member.
    /// </summary>
    /// <param name="key">The key of the sorted set.</param>
    /// <param name="member">The member to get the position for.</param>
    /// <returns>A <see cref="Cmd{T, R}"/> with the request.</returns>
    public static Cmd<object[], GeoPosition?> GeoPositionAsync(ValkeyKey key, ValkeyValue member)
    {
        GlideString[] args = [key.ToGlideString(), member.ToGlideString()];
        return new(RequestType.GeoPos, args, false, response => 
        {
            if (response.Length == 0 || response[0] == null) return (GeoPosition?)null;
            var posArray = (object[])response[0];
            if (posArray.Length < 2 || posArray[0] == null || posArray[1] == null) return (GeoPosition?)null;
            return (GeoPosition?)new GeoPosition(double.Parse(posArray[0].ToString()!), double.Parse(posArray[1].ToString()!));
        });
    }

    /// <summary>
    /// Creates a request for GEOPOS command for multiple members.
    /// </summary>
    /// <param name="key">The key of the sorted set.</param>
    /// <param name="members">The members to get the positions for.</param>
    /// <returns>A <see cref="Cmd{T, R}"/> with the request.</returns>
    public static Cmd<object[], GeoPosition?[]> GeoPositionAsync(ValkeyKey key, ValkeyValue[] members)
    {
        GlideString[] args = [key.ToGlideString(), .. members.Select(m => m.ToGlideString())];
        return new(RequestType.GeoPos, args, false, response => 
        {
            return response.Select(item => 
            {
                if (item == null) return (GeoPosition?)null;
                var posArray = (object[])item;
                if (posArray.Length < 2 || posArray[0] == null || posArray[1] == null) return (GeoPosition?)null;
                return (GeoPosition?)new GeoPosition(double.Parse(posArray[0].ToString()!), double.Parse(posArray[1].ToString()!));
            }).ToArray();
        });
    }
}