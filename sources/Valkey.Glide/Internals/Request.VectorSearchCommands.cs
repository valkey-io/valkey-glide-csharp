// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands.Options;

using static Valkey.Glide.Internals.FFI;

namespace Valkey.Glide.Internals;

internal partial class Request
{
    public static Cmd<string, string> FtCreate(string indexName, IEnumerable<IField> schema, FtCreateOptions? options)
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

    public static Cmd<string, string> FtDropIndex(string indexName)
        => Simple<string>(RequestType.FtDropIndex, [(GlideString)indexName]);

    public static Cmd<object[], ISet<string>> FtList()
        => new(RequestType.FtList, [], false, arr => arr.Cast<GlideString>().Select(gs => gs.ToString()).ToHashSet());

    public static Cmd<object[], FtSearchResult> FtSearch(string indexName, string query, FtSearchOptions? options)
    {
        List<GlideString> args = [indexName, query];
        // Server ignores WITHSORTKEYS when NOCONTENT is set, so treat as plain search.
        bool withSortKeys = options is { WithSortKeys: true, NoContent: false };
        if (options is not null)
        {
            args.AddRange(options.ToArgs());
        }
        return new(RequestType.FtSearch, [.. args], false, data => ParseFtSearchResponse(data, withSortKeys));
    }

    public static Cmd<object[], FtAggregateRow[]> FtAggregate(string indexName, string query, FtAggregateOptions? options)
    {
        List<GlideString> args = [indexName, query];
        if (options is not null)
        {
            args.AddRange(options.ToArgs());
        }
        return new(RequestType.FtAggregate, [.. args], false, ParseFtAggregateResponse);
    }

    public static Cmd<object, Dictionary<string, object>> FtInfo(string indexName, FtInfoOptions? options)
    {
        List<GlideString> args = [indexName];
        if (options is not null)
        {
            args.AddRange(options.ToArgs());
        }
        return new(RequestType.FtInfo, [.. args], false, ParseFtInfoResponse);
    }

    public static Cmd<string, string> FtAliasAdd(string alias, string indexName)
        => Simple<string>(RequestType.FtAliasAdd, [(GlideString)alias, (GlideString)indexName]);

    public static Cmd<string, string> FtAliasDel(string alias)
        => Simple<string>(RequestType.FtAliasDel, [(GlideString)alias]);

    public static Cmd<string, string> FtAliasUpdate(string alias, string indexName)
        => Simple<string>(RequestType.FtAliasUpdate, [(GlideString)alias, (GlideString)indexName]);

    public static Cmd<Dictionary<GlideString, object>, Dictionary<string, string>> FtAliasList()
        => new(RequestType.FtAliasList, [], false, dict =>
            dict.ToDictionary(kvp => kvp.Key.ToString(), kvp => ((GlideString)kvp.Value).ToString()));

    // --- response parsers ---

    private static FtSearchResult ParseFtSearchResponse(object[] data, bool withSortKeys)
    {
        // Format: [count, {key1: fields1, key2: fields2, ...}]
        if (data.Length < 2)
            return new FtSearchResult(0, []);

        long count = Convert.ToInt64(data[0]);
        List<FtSearchDocument> docs = [];

        if (data[1] is Dictionary<GlideString, object> map)
        {
            foreach (var kvp in map)
            {
                string key = kvp.Key.ToString();
                string sortKey = "";
                Dictionary<string, object?> fields = [];

                if (withSortKeys && kvp.Value is object[] pair && pair.Length == 2)
                {
                    sortKey = pair[0] is GlideString gs ? gs.ToString() : pair[0]?.ToString() ?? "";
                    if (pair[1] is Dictionary<GlideString, object> fieldMap)
                    {
                        fields = fieldMap.ToDictionary(
                            f => f.Key.ToString(),
                            f => ConvertFtValue(f.Value));
                    }
                }
                else
                {
                    var converted = ConvertFtValue(kvp.Value);
                    if (converted is Dictionary<string, object?> fieldDict)
                        fields = fieldDict;
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
                    kvp => kvp.Key.ToString(),
                    kvp => ConvertFtValue(kvp.Value)!)));
            }
        }
        return [.. results];
    }

    private static Dictionary<string, object> ParseFtInfoResponse(object data)
    {
        // May arrive as a Map or as a flat key/value array.
        if (data is Dictionary<GlideString, object> map)
        {
            return map.ToDictionary(
                kvp => kvp.Key.ToString(),
                kvp => ConvertFtValue(kvp.Value)!);
        }

        if (data is object[] arr)
        {
            Dictionary<string, object> result = [];
            for (int i = 0; i + 1 < arr.Length; i += 2)
            {
                string key = arr[i] is GlideString gs ? gs.ToString() : i.ToString();
                result[key] = ConvertFtValue(arr[i + 1])!;
            }
            return result;
        }

        return [];
    }

    private static object? ConvertFtValue(object? value) => value switch
    {
        null => null,
        GlideString gs => gs.ToString(),
        Dictionary<GlideString, object> nested => nested.ToDictionary(
            kvp => kvp.Key.ToString(),
            kvp => ConvertFtValue(kvp.Value)),
        object[] arr => arr.Select(ConvertFtValue).ToArray(),
        _ => value,
    };
}
