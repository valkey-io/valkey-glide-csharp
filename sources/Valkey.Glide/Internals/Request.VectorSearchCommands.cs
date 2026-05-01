// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.ServerModules;

using static Valkey.Glide.Internals.FFI;

namespace Valkey.Glide.Internals;

internal partial class Request
{
    #region Public Methods

    public static Cmd<string, string> FtCreate(ValkeyKey indexName, IEnumerable<Ft.ICreateField> schema, Ft.CreateOptions? options)
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

    public static Cmd<object[], Ft.SearchResult> FtSearch(ValkeyKey indexName, ValkeyValue query, Ft.SearchOptions? options)
    {
        List<GlideString> args = [indexName, query];
        bool isNoContent = options?.Return is not null
            && !(options.Return as ICollection<Ft.SearchReturnField> ?? [.. options.Return]).Any();
        bool withSortKeys = options is { SortBy.WithSortKeys: true } && !isNoContent;
        if (options is not null)
        {
            args.AddRange(options.ToArgs());
        }
        return new(RequestType.FtSearch, [.. args], false, data => ParseFtSearchResponse(data, withSortKeys));
    }

    public static Cmd<object[], Ft.AggregateRow[]> FtAggregate(ValkeyKey indexName, ValkeyValue query, Ft.AggregateOptions? options)
    {
        List<GlideString> args = [indexName, query];

        if (options is not null)
        {
            args.AddRange(options.ToArgs());
        }

        return new(RequestType.FtAggregate, [.. args], false, ParseFtAggregateResponse);
    }

    public static Cmd<object, Dictionary<ValkeyValue, object>> FtInfo(ValkeyKey indexName, Ft.InfoOptions? options)
    {
        List<GlideString> args = [indexName];
        if (options is not null)
        {
            args.AddRange(options.ToArgs());
        }
        return new(RequestType.FtInfo, [.. args], false, ParseFtInfoResponse);
    }

    #endregion
    #region Private Methods

    private static Ft.SearchResult ParseFtSearchResponse(object[] data, bool withSortKeys)
    {
        // Format: [count, {key1: fields1, key2: fields2, ...}]
        if (data.Length < 2)
        {
            return new Ft.SearchResult(0, []);
        }

        long count = Convert.ToInt64(data[0]);
        List<Ft.SearchDocument> docs = [];

        if (data[1] is Dictionary<GlideString, object> map)
        {
            foreach (var kvp in map)
            {
                ValkeyKey key = (ValkeyKey)kvp.Key.Bytes;
                ValkeyValue sortKey = default;
                IDictionary<ValkeyValue, ValkeyValue> fields = new Dictionary<ValkeyValue, ValkeyValue>();

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

                docs.Add(new Ft.SearchDocument(key, fields, sortKey));
            }
        }

        return new Ft.SearchResult(count, [.. docs]);
    }

    private static Ft.AggregateRow[] ParseFtAggregateResponse(object[] data)
    {
        // The Rust core normalizes the response: strips leading count, converts rows to maps.
        var results = new List<Ft.AggregateRow>();
        foreach (var row in data)
        {
            if (row is Dictionary<GlideString, object> map)
            {
                results.Add(new Ft.AggregateRow(map.ToDictionary(
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

    #endregion
}
