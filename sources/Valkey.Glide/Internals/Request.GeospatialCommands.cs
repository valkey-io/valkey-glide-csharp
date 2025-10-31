// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using static Valkey.Glide.Internals.FFI;

namespace Valkey.Glide.Internals;

internal static partial class Request
{
    /// <summary>
    /// Adds GeoAddOptions to the argument list.
    /// </summary>
    /// <param name="args">The argument list to add options to.</param>
    /// <param name="options">The options to add.</param>
    private static void AddGeoAddOptions(List<GlideString> args, GeoAddOptions? options)
    {
        if (options?.ConditionalChange.HasValue == true)
        {
            args.Add(options.ConditionalChange.Value == ConditionalChange.ONLY_IF_DOES_NOT_EXIST
                ? ValkeyLiterals.NX.ToGlideString()
                : ValkeyLiterals.XX.ToGlideString());
        }

        if (options?.Changed == true)
        {
            args.Add(ValkeyLiterals.CH.ToGlideString());
        }
    }

    /// <summary>
    /// Creates a request for GEOADD command.
    /// </summary>
    /// <param name="key">The key of the sorted set.</param>
    /// <param name="value">The geospatial item to add.</param>
    /// <param name="options">The options for the GEOADD command.</param>
    /// <returns>A <see cref="Cmd{T, R}"/> with the request.</returns>
    public static Cmd<long, bool> GeoAddAsync(ValkeyKey key, GeoEntry value, GeoAddOptions? options = null)
    {
        List<GlideString> args = [key.ToGlideString()];
        AddGeoAddOptions(args, options);
        args.Add(value.Longitude.ToGlideString());
        args.Add(value.Latitude.ToGlideString());
        args.Add(value.Member.ToGlideString());
        return Boolean<long>(RequestType.GeoAdd, [.. args]);
    }

    /// <summary>
    /// Creates a request for GEOADD command with multiple values.
    /// </summary>
    /// <param name="key">The key of the sorted set.</param>
    /// <param name="values">The geospatial items to add.</param>
    /// <param name="options">The options for the GEOADD command.</param>
    /// <returns>A <see cref="Cmd{T, R}"/> with the request.</returns>
    public static Cmd<long, long> GeoAddAsync(ValkeyKey key, GeoEntry[] values, GeoAddOptions? options = null)
    {
        List<GlideString> args = [key.ToGlideString()];
        AddGeoAddOptions(args, options);
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
        var args = new List<GlideString> { key.ToGlideString() };
        args.AddRange(members.Select(m => m.ToGlideString()));
        return new(RequestType.GeoHash, [.. args], false, response => [.. response.Select(item => item?.ToString())]);
    }

    /// <summary>
    /// Parses a position array into a GeoPosition.
    /// </summary>
    /// <param name="item">The position array from server response.</param>
    /// <returns>A GeoPosition or null if parsing fails.</returns>
    private static GeoPosition? ParseGeoPosition(object? item)
    {
        if (item == null) return null;
        var posArray = (object[])item;
        if (posArray.Length < 2 || posArray[0] == null || posArray[1] == null) return null;
        return new GeoPosition(double.Parse(posArray[0].ToString()!), double.Parse(posArray[1].ToString()!));
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
            response.Length > 0 ? ParseGeoPosition(response[0]) : null);
    }

    /// <summary>
    /// Creates a request for GEOPOS command for multiple members.
    /// </summary>
    /// <param name="key">The key of the sorted set.</param>
    /// <param name="members">The members to get the positions for.</param>
    /// <returns>A <see cref="Cmd{T, R}"/> with the request.</returns>
    public static Cmd<object[], GeoPosition?[]> GeoPositionAsync(ValkeyKey key, ValkeyValue[] members)
    {
        var args = new List<GlideString> { key.ToGlideString() };
        args.AddRange(members.Select(m => m.ToGlideString()));
        return new(RequestType.GeoPos, [.. args], false, response =>
            [.. response.Select(ParseGeoPosition)]);
    }

    /// <summary>
    /// Creates a request for GEOSEARCH command with member origin.
    /// </summary>
    /// <param name="key">The key of the sorted set.</param>
    /// <param name="fromMember">The member to search from.</param>
    /// <param name="shape">The search area shape.</param>
    /// <param name="count">The maximum number of results to return.</param>
    /// <param name="demandClosest">When true, returns the closest results. When false, allows any results.</param>
    /// <param name="order">The order in which to return results.</param>
    /// <param name="options">The options for the search result format.</param>
    /// <returns>A <see cref="Cmd{T, R}"/> with the request.</returns>
    public static Cmd<object[], GeoRadiusResult[]> GeoSearchAsync(ValkeyKey key, ValkeyValue fromMember, GeoSearchShape shape, long count = -1, bool demandClosest = true, Order? order = null, GeoRadiusOptions options = GeoRadiusOptions.None)
    {
        List<GlideString> args = [key.ToGlideString(), ValkeyLiterals.FROMMEMBER.ToGlideString(), fromMember.ToGlideString()];
        var shapeArgs = new List<ValkeyValue>();
        shape.AddArgs(shapeArgs);
        args.AddRange(shapeArgs.Select(a => a.ToGlideString()));
        if (count > 0)
        {
            args.Add(ValkeyLiterals.COUNT.ToGlideString());
            args.Add(count.ToGlideString());
            if (!demandClosest)
            {
                args.Add(ValkeyLiterals.ANY.ToGlideString());
            }
        }
        if (order.HasValue)
        {
            args.Add(order.Value.ToLiteral().ToGlideString());
        }
        List<ValkeyValue> optionArgs = [];
        options.AddArgs(optionArgs);
        args.AddRange(optionArgs.Select(a => a.ToGlideString()));
        return new(RequestType.GeoSearch, [.. args], false, response => ProcessGeoSearchResponse(response, options));
    }

    /// <summary>
    /// Creates a request for GEOSEARCH command with position origin.
    /// </summary>
    /// <param name="key">The key of the sorted set.</param>
    /// <param name="fromPosition">The position to search from.</param>
    /// <param name="shape">The search area shape.</param>
    /// <param name="count">The maximum number of results to return.</param>
    /// <param name="demandClosest">When true, returns the closest results. When false, allows any results.</param>
    /// <param name="order">The order in which to return results.</param>
    /// <param name="options">The options for the search result format.</param>
    /// <returns>A <see cref="Cmd{T, R}"/> with the request.</returns>
    public static Cmd<object[], GeoRadiusResult[]> GeoSearchAsync(ValkeyKey key, GeoPosition fromPosition, GeoSearchShape shape, long count = -1, bool demandClosest = true, Order? order = null, GeoRadiusOptions options = GeoRadiusOptions.None)
    {
        List<GlideString> args = [key.ToGlideString(), ValkeyLiterals.FROMLONLAT.ToGlideString(), fromPosition.Longitude.ToGlideString(), fromPosition.Latitude.ToGlideString()];
        List<ValkeyValue> shapeArgs = [];
        shape.AddArgs(shapeArgs);
        args.AddRange(shapeArgs.Select(a => a.ToGlideString()));
        if (count > 0)
        {
            args.Add(ValkeyLiterals.COUNT.ToGlideString());
            args.Add(count.ToGlideString());
            if (!demandClosest)
            {
                args.Add(ValkeyLiterals.ANY.ToGlideString());
            }
        }
        if (order.HasValue)
        {
            args.Add(order.Value.ToLiteral().ToGlideString());
        }
        List<ValkeyValue> optionArgs = [];
        options.AddArgs(optionArgs);
        args.AddRange(optionArgs.Select(a => a.ToGlideString()));
        return new(RequestType.GeoSearch, [.. args], false, response => ProcessGeoSearchResponse(response, options));
    }



    /// <summary>
    /// Processes the response from GEOSEARCH command based on the options.
    /// </summary>
    /// <param name="response">The raw response from the server.</param>
    /// <param name="options">The options used in the request.</param>
    /// <returns>An array of GeoRadiusResult objects.</returns>
    private static GeoRadiusResult[] ProcessGeoSearchResponse(object[] response, GeoRadiusOptions options)
    {


        return [.. response.Select(item =>
        {
            // If no options are specified, Redis returns simple strings (member names)
            if (options == GeoRadiusOptions.None)
            {
                return new GeoRadiusResult(new ValkeyValue(item?.ToString() ?? ""), null, null, null);
            }

            // With options, Redis returns arrays: [member, ...additional data based on options]
            if (item is not object[] itemArray || itemArray.Length == 0)
            {
                // Fallback for unexpected format
                return new GeoRadiusResult(new ValkeyValue(item?.ToString() ?? ""), null, null, null);
            }

            var member = new ValkeyValue(itemArray[0]?.ToString() ?? "");
            double? distance = null;
            long? hash = null;
            GeoPosition? position = null;

            int index = 1;

            // Redis returns additional data in this specific order:
            // 1. Distance (if WITHDIST)
            // 2. Hash (if WITHHASH)
            // 3. Coordinates (if WITHCOORD)

            if ((options & GeoRadiusOptions.WithDistance) != 0 && index < itemArray.Length)
            {
                // Distance comes as a nested array: [distance_value]
                if (itemArray[index] is object[] distArray && distArray.Length > 0)
                {
                    if (double.TryParse(distArray[0]?.ToString(), out var dist))
                        distance = dist;
                }
                index++;
            }

            if ((options & GeoRadiusOptions.WithGeoHash) != 0 && index < itemArray.Length)
            {
                // Hash comes as a nested array: [hash_value]
                if (itemArray[index] is object[] hashArray && hashArray.Length > 0)
                {
                    if (long.TryParse(hashArray[0]?.ToString(), out var h))
                        hash = h;
                }
                index++;
            }

            if ((options & GeoRadiusOptions.WithCoordinates) != 0 && index < itemArray.Length)
            {
                // Coordinates come as a triple-nested array: [[[longitude, latitude]]]
                if (itemArray[index] is object[] coordOuterArray && coordOuterArray.Length > 0 &&
                    coordOuterArray[0] is object[] coordMiddleArray && coordMiddleArray.Length >= 2)
                {
                    if (double.TryParse(coordMiddleArray[0]?.ToString(), out var lon) &&
                        double.TryParse(coordMiddleArray[1]?.ToString(), out var lat))
                    {
                        position = new GeoPosition(lon, lat);
                    }
                }
            }

            return new GeoRadiusResult(member, distance, hash, position);
        })];
    }

    /// <summary>
    /// Adds common GeoSearchAndStore arguments to the argument list.
    /// </summary>
    /// <param name="args">The argument list to add to.</param>
    /// <param name="shape">The search area shape.</param>
    /// <param name="count">The maximum number of results to return.</param>
    /// <param name="demandClosest">When true, returns the closest results.</param>
    /// <param name="order">The order in which to return results.</param>
    /// <param name="storeDistances">When true, stores distances instead of just member names.</param>
    private static void AddGeoSearchAndStoreArgs(List<GlideString> args, GeoSearchShape shape, long count, bool demandClosest, Order? order, bool storeDistances)
    {
        List<ValkeyValue> shapeArgs = [];
        shape.AddArgs(shapeArgs);
        args.AddRange(shapeArgs.Select(a => a.ToGlideString()));
        if (count > 0)
        {
            args.Add(ValkeyLiterals.COUNT.ToGlideString());
            args.Add(count.ToGlideString());
            if (!demandClosest)
            {
                args.Add(ValkeyLiterals.ANY.ToGlideString());
            }
        }
        if (order.HasValue)
        {
            args.Add(order.Value.ToLiteral().ToGlideString());
        }
        if (storeDistances)
        {
            args.Add(ValkeyLiterals.STOREDIST.ToGlideString());
        }
    }

    /// <summary>
    /// Creates a request for GEOSEARCHSTORE command with member origin.
    /// </summary>
    /// <param name="sourceKey">The key of the source sorted set.</param>
    /// <param name="destinationKey">The key where results will be stored.</param>
    /// <param name="fromMember">The member to search from.</param>
    /// <param name="shape">The search area shape.</param>
    /// <param name="count">The maximum number of results to return.</param>
    /// <param name="demandClosest">When true, returns the closest results. When false, allows any results.</param>
    /// <param name="order">The order in which to return results.</param>
    /// <param name="storeDistances">When true, stores distances instead of just member names.</param>
    /// <returns>A <see cref="Cmd{T, R}"/> with the request.</returns>
    public static Cmd<long, long> GeoSearchAndStoreAsync(ValkeyKey sourceKey, ValkeyKey destinationKey, ValkeyValue fromMember, GeoSearchShape shape, long count, bool demandClosest, Order? order, bool storeDistances)
    {
        List<GlideString> args = [destinationKey.ToGlideString(), sourceKey.ToGlideString(), ValkeyLiterals.FROMMEMBER.ToGlideString(), fromMember.ToGlideString()];
        AddGeoSearchAndStoreArgs(args, shape, count, demandClosest, order, storeDistances);
        return Simple<long>(RequestType.GeoSearchStore, [.. args]);
    }

    /// <summary>
    /// Creates a request for GEOSEARCHSTORE command with position origin.
    /// </summary>
    /// <param name="sourceKey">The key of the source sorted set.</param>
    /// <param name="destinationKey">The key where results will be stored.</param>
    /// <param name="fromPosition">The position to search from.</param>
    /// <param name="shape">The search area shape.</param>
    /// <param name="count">The maximum number of results to return.</param>
    /// <param name="demandClosest">When true, returns the closest results. When false, allows any results.</param>
    /// <param name="order">The order in which to return results.</param>
    /// <param name="storeDistances">When true, stores distances instead of just member names.</param>
    /// <returns>A <see cref="Cmd{T, R}"/> with the request.</returns>
    public static Cmd<long, long> GeoSearchAndStoreAsync(ValkeyKey sourceKey, ValkeyKey destinationKey, GeoPosition fromPosition, GeoSearchShape shape, long count, bool demandClosest, Order? order, bool storeDistances)
    {
        List<GlideString> args = [destinationKey.ToGlideString(), sourceKey.ToGlideString(), ValkeyLiterals.FROMLONLAT.ToGlideString(), fromPosition.Longitude.ToGlideString(), fromPosition.Latitude.ToGlideString()];
        AddGeoSearchAndStoreArgs(args, shape, count, demandClosest, order, storeDistances);
        return Simple<long>(RequestType.GeoSearchStore, [.. args]);
    }
}
