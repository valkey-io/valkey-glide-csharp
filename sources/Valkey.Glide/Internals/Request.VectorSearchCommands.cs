// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands.Options;

using static Valkey.Glide.Internals.FFI;

namespace Valkey.Glide.Internals;

internal partial class Request
{
    public static Cmd<string, string> FtCreate(ValkeyKey indexName, IEnumerable<IField> schema, FtCreateOptions? options)
    {
        List<GlideString> args = [indexName];
        if (options is not null)
        {
            args.AddRange(options.ToArgs());
        }
        args.Add(ValkeyLiterals.SCHEMA);
        foreach (var field in schema)
        {
            args.AddRange(field.ToArgs());
        }
        return Simple<string>(RequestType.FtCreate, [.. args]);
    }

    public static Cmd<string, string> FtDropIndex(ValkeyKey indexName)
        => Simple<string>(RequestType.FtDropIndex, [(GlideString)indexName]);

    public static Cmd<object[], ISet<ValkeyValue>> FtList()
        => new(RequestType.FtList, [], false, ToValkeyValueSet);

    private static HashSet<ValkeyValue> ToValkeyValueSet(object[] objects)
    {
        HashSet<ValkeyValue> result = new(objects.Length);
        foreach (var obj in objects)
        {
            _ = result.Add((ValkeyValue)(GlideString)obj);
        }
        return result;
    }

    public static Cmd<object[], FtSearchResult> FtSearch(ValkeyKey indexName, ValkeyValue query, FtSearchOptions? options)
    {
        List<GlideString> args = [indexName, query];
        // Server ignores WITHSORTKEYS when NOCONTENT is set, so treat as plain search.
        bool withSortKeys = options is { SortBy.WithSortKeys: true, NoContent: false };
        if (options is not null)
        {
            args.AddRange(options.ToArgs());
        }
        return new(RequestType.FtSearch, [.. args], false, data => ParseFtSearchResponse(data, withSortKeys));
    }

    public static Cmd<object[], FtAggregateRow[]> FtAggregate(ValkeyKey indexName, ValkeyValue query, FtAggregateOptions? options)
    {
        List<GlideString> args = [indexName, query];
        if (options is not null)
        {
            args.AddRange(options.ToArgs());
        }
        return new(RequestType.FtAggregate, [.. args], false, ParseFtAggregateResponse);
    }

    public static Cmd<object, Dictionary<ValkeyValue, object>> FtInfo(ValkeyKey indexName, FtInfoOptions? options)
    {
        List<GlideString> args = [indexName];
        if (options is not null)
        {
            args.AddRange(options.ToArgs());
        }
        return new(RequestType.FtInfo, [.. args], false, ParseFtInfoResponse);
    }

    // --- response parsers ---

    private static FtSearchResult ParseFtSearchResponse(object[] data, bool withSortKeys)
    {
        // Format: [count, {key1: fields1, key2: fields2, ...}]
        if (data.Length < 2)
        {
            return new FtSearchResult(0, []);
        }

        long count = Convert.ToInt64(data[0]);
        List<FtSearchDocument> docs = [];

        if (data[1] is Dictionary<GlideString, object> map)
        {
            foreach (var kvp in map)
            {
                ValkeyKey key = (ValkeyKey)kvp.Key.Bytes;
                ValkeyValue? sortKey = null;
                Dictionary<ValkeyValue, ValkeyValue> fields = [];

                if (withSortKeys && kvp.Value is object[] pair && pair.Length == 2)
                {
                    sortKey = pair[0] is GlideString gs ? (ValkeyValue)gs : ValkeyValue.Null;
                    if (pair[1] is Dictionary<GlideString, object> fieldMap)
                    {
                        fields = fieldMap.ToDictionary(
                            f => (ValkeyValue)f.Key,
                            f => (ValkeyValue)(GlideString)f.Value);
                    }
                }
                else if (kvp.Value is Dictionary<GlideString, object> directFieldMap)
                {
                    fields = directFieldMap.ToDictionary(
                        f => (ValkeyValue)f.Key,
                        f => (ValkeyValue)(GlideString)f.Value);
                }

                docs.Add(new FtSearchDocument(key, fields, sortKey));
            }
        }

        return new FtSearchResult(count, docs);
    }

    private static FtAggregateRow[] ParseFtAggregateResponse(object[] data)
    {
        // The Rust core normalizes the response: strips leading count, converts rows to maps.
        var results = new List<FtAggregateRow>();
        foreach (var row in data)
        {
            if (row is Dictionary<GlideString, object> map)
            {
                results.Add(new FtAggregateRow(map.ToDictionary(
                    kvp => (ValkeyValue)kvp.Key,
                    kvp => ToValkeyValue(kvp.Value))));
            }
        }
        return [.. results];
    }

    private static ValkeyValue ToValkeyValue(object? value) => value switch
    {
        null => ValkeyValue.Null,
        GlideString gs => (ValkeyValue)gs,
        long l => (ValkeyValue)l,
        double d => (ValkeyValue)d,
        bool b => (ValkeyValue)b,
        _ => (ValkeyValue)(value.ToString() ?? string.Empty),
    };

    private static Dictionary<ValkeyValue, object> ParseFtInfoResponse(object data) => data is Dictionary<GlideString, object> map
            ? map.ToDictionary(
                kvp => (ValkeyValue)kvp.Key,
                kvp => ConvertFtValue(kvp.Value) ?? ValkeyValue.Null)
            : [];

    private static object? ConvertFtValue(object? value) => value switch
    {
        null => null,
        GlideString gs => (ValkeyValue)gs,
        Dictionary<GlideString, object> nested => nested.ToDictionary(
            kvp => (ValkeyValue)kvp.Key,
            kvp => ConvertFtValue(kvp.Value)),
        object[] arr => arr.Select(ConvertFtValue).ToArray(),
        _ => value,
    };
}
