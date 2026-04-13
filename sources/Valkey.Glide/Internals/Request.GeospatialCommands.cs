// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using static Valkey.Glide.Internals.FFI;

namespace Valkey.Glide.Internals;

internal static partial class Request
{
    public static Cmd<long, bool> GeoAddAsync(ValkeyKey key, ValkeyValue member, GeoPosition position, GeoAddOptions options = default)
    {
        List<GlideString> args = [key.ToGlideString()];
        args.AddRange(options.ToArgs());
        args.Add(position.Longitude.ToGlideString());
        args.Add(position.Latitude.ToGlideString());
        args.Add(member.ToGlideString());
        return Boolean<long>(RequestType.GeoAdd, [.. args]);
    }

    public static Cmd<long, long> GeoAddAsync(ValkeyKey key, IDictionary<ValkeyValue, GeoPosition> members, GeoAddOptions options = default)
    {
        List<GlideString> args = [key.ToGlideString()];
        args.AddRange(options.ToArgs());
        foreach (var (member, position) in members)
        {
            args.Add(position.Longitude.ToGlideString());
            args.Add(position.Latitude.ToGlideString());
            args.Add(member.ToGlideString());
        }

        return Simple<long>(RequestType.GeoAdd, [.. args]);
    }

    public static Cmd<double?, double?> GeoDistanceAsync(ValkeyKey key, ValkeyValue member1, ValkeyValue member2, GeoUnit unit)
    {
        GlideString[] args = [key.ToGlideString(), member1.ToGlideString(), member2.ToGlideString(), unit.ToLiteral()];
        return Simple<double?>(RequestType.GeoDist, args, true);
    }

    public static Cmd<object[], string?> GeoHashAsync(ValkeyKey key, ValkeyValue member)
    {
        GlideString[] args = [key.ToGlideString(), member.ToGlideString()];
        return new(RequestType.GeoHash, args, false, response => response.Length > 0 ? response[0]?.ToString() : null);
    }

    public static Cmd<object[], string?[]> GeoHashAsync(ValkeyKey key, ValkeyValue[] members)
    {
        var args = new List<GlideString> { key.ToGlideString() };
        args.AddRange(members.Select(m => m.ToGlideString()));
        return new(RequestType.GeoHash, [.. args], false, response => [.. response.Select(item => item?.ToString())]);
    }

    public static Cmd<object[], GeoPosition?> GeoPositionAsync(ValkeyKey key, ValkeyValue member)
    {
        GlideString[] args = [key.ToGlideString(), member.ToGlideString()];
        return new(RequestType.GeoPos, args, false, response =>
            response.Length > 0 ? ParseGeoPosition(response[0]) : null);
    }

    public static Cmd<object[], GeoPosition?[]> GeoPositionAsync(ValkeyKey key, ValkeyValue[] members)
    {
        var args = new List<GlideString> { key.ToGlideString() };
        args.AddRange(members.Select(m => m.ToGlideString()));
        return new(RequestType.GeoPos, [.. args], false, response =>
            [.. response.Select(ParseGeoPosition)]);
    }

    public static Cmd<object[], GeoSearchResult[]> GeoSearchAsync(ValkeyKey key, ValkeyValue from, GeoSearchShape shape, GeoSearchOptions options = default)
    {
        List<GlideString> args = [key.ToGlideString(), ValkeyLiterals.FROMMEMBER.ToGlideString(), from.ToGlideString()];
        args.AddRange(shape.ToArgs());
        args.AddRange(options.ToArgs());
        return new(RequestType.GeoSearch, [.. args], false, response => ProcessGeoSearchResponse(response, options));
    }

    public static Cmd<object[], GeoSearchResult[]> GeoSearchAsync(ValkeyKey key, GeoPosition from, GeoSearchShape shape, GeoSearchOptions options = default)
    {
        List<GlideString> args = [key.ToGlideString(), ValkeyLiterals.FROMLONLAT.ToGlideString(), from.Longitude.ToGlideString(), from.Latitude.ToGlideString()];
        args.AddRange(shape.ToArgs());
        args.AddRange(options.ToArgs());
        return new(RequestType.GeoSearch, [.. args], false, response => ProcessGeoSearchResponse(response, options));
    }

    public static Cmd<long, long> GeoSearchAndStoreAsync(ValkeyKey source, ValkeyKey destination, ValkeyValue from, GeoSearchShape shape, GeoSearchStoreOptions options = default)
    {
        List<GlideString> args = [destination.ToGlideString(), source.ToGlideString(), ValkeyLiterals.FROMMEMBER.ToGlideString(), from.ToGlideString()];
        args.AddRange(shape.ToArgs());
        args.AddRange(options.ToArgs());
        return Simple<long>(RequestType.GeoSearchStore, [.. args]);
    }

    public static Cmd<long, long> GeoSearchAndStoreAsync(ValkeyKey source, ValkeyKey destination, GeoPosition from, GeoSearchShape shape, GeoSearchStoreOptions options = default)
    {
        List<GlideString> args = [destination.ToGlideString(), source.ToGlideString(), ValkeyLiterals.FROMLONLAT.ToGlideString(), from.Longitude.ToGlideString(), from.Latitude.ToGlideString()];
        args.AddRange(shape.ToArgs());
        args.AddRange(options.ToArgs());
        return Simple<long>(RequestType.GeoSearchStore, [.. args]);
    }

    private static GeoPosition? ParseGeoPosition(object? item)
    {
        if (item == null)
        {
            return null;
        }

        var posArray = (object[])item;
        if (posArray.Length < 2 || posArray[0] == null || posArray[1] == null)
        {
            return null;
        }

        return new GeoPosition(double.Parse(posArray[0].ToString()!), double.Parse(posArray[1].ToString()!));
    }

    private static GeoSearchResult[] ProcessGeoSearchResponse(object[] response, GeoSearchOptions options)
    {
        bool hasExtras = options.WithDistance || options.WithHash || options.WithPosition;

        return [.. response.Select(item =>
        {
            if (!hasExtras)
            {
                return new GeoSearchResult(new ValkeyValue(item?.ToString() ?? ""));
            }

            if (item is not object[] itemArray || itemArray.Length == 0)
            {
                return new GeoSearchResult(new ValkeyValue(item?.ToString() ?? ""));
            }

            var member = new ValkeyValue(itemArray[0]?.ToString() ?? "");
            double? distance = null;
            long? hash = null;
            GeoPosition? position = null;

            int index = 1;

            // Valkey returns additional data in this specific order:
            // 1. Distance (if WITHDIST)
            // 2. Hash (if WITHHASH)
            // 3. Coordinates (if WITHCOORD)

            if (options.WithDistance && index < itemArray.Length)
            {
                if (itemArray[index] is object[] distArray && distArray.Length > 0
                    && double.TryParse(distArray[0]?.ToString(), out var dist))
                {
                    distance = dist;
                }

                index++;
            }

            if (options.WithHash && index < itemArray.Length)
            {
                if (itemArray[index] is object[] hashArray && hashArray.Length > 0
                    && long.TryParse(hashArray[0]?.ToString(), out var h))
                {
                    hash = h;
                }

                index++;
            }

            if (options.WithPosition && index < itemArray.Length)
            {
                if (itemArray[index] is object[] coordOuterArray && coordOuterArray.Length > 0
                    && coordOuterArray[0] is object[] coordMiddleArray && coordMiddleArray.Length >= 2
                    && double.TryParse(coordMiddleArray[0]?.ToString(), out var lon)
                    && double.TryParse(coordMiddleArray[1]?.ToString(), out var lat))
                {
                    position = new GeoPosition(lon, lat);
                }
            }

            return new GeoSearchResult(member, position, distance, hash);
        })];
    }
}
