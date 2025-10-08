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
}