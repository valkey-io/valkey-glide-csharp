// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.ServerModules;

using static Valkey.Glide.Internals.FFI;

namespace Valkey.Glide.Internals;

internal partial class Request
{
    #region Public Methods

    public static Cmd<string, string> FtCreate(ValkeyKey indexName, IEnumerable<Ft.Field> schema, Ft.CreateOptions? options)
        => Simple<string>(RequestType.FtCreate, [indexName, .. ToArgs(options), .. ToArgs(schema)]);

    public static Cmd<string, string> FtDropIndex(ValkeyKey indexName)
        => Simple<string>(RequestType.FtDropIndex, [indexName]);

    public static Cmd<object[], ISet<ValkeyValue>> FtList()
        => new(RequestType.FtList, [], false, ToValkeyValueSet);

    public static Cmd<object[], Ft.SearchResult> FtSearch(ValkeyKey indexName, ValkeyValue query, Ft.SearchOptions? options)
        => new(RequestType.FtSearch, [indexName, query, .. ToArgs(options)], false, ParseFtSearchResponse);

    public static Cmd<object[], Ft.AggregateRow[]> FtAggregate(ValkeyKey indexName, ValkeyValue query, Ft.AggregateOptions? options)
        => new(RequestType.FtAggregate, [indexName, query, .. ToArgs(options)], false, ParseFtAggregateResponse);

    public static Cmd<object, Dictionary<ValkeyValue, object>> FtInfo(ValkeyKey indexName, Ft.InfoOptions? options)
        => new(RequestType.FtInfo, [indexName, .. ToArgs(options)], false, ParseFtInfoResponse);

    #endregion
    #region Private Methods

    /// <summary>
    /// Converts the given <see cref="Ft.CreateOptions"/> to command arguments.
    /// </summary>
    private static GlideString[] ToArgs(Ft.CreateOptions? options)
    {
        if (options is null)
        {
            return [];
        }

        List<GlideString> args = [
            ValkeyLiterals.ON,
            options.DataType switch
            {
                Ft.DataType.Hash => ValkeyLiterals.HASH,
                Ft.DataType.Json => ValkeyLiterals.JSON,
                _ => throw new ArgumentOutOfRangeException(nameof(options.DataType)),
            }];

        args.AddRange(ToArgs(ValkeyLiterals.PREFIX, options.Prefixes));

        if (options.SkipInitialScan)
        {
            args.Add(ValkeyLiterals.SKIPINITIALSCAN);
        }

        if (options.MinStemSize.HasValue)
        {
            args.Add(ValkeyLiterals.MINSTEMSIZE);
            args.Add(options.MinStemSize.Value.ToGlideString());
        }

        if (options.NoOffsets)
        {
            args.Add(ValkeyLiterals.NOOFFSETS);
        }

        if (options.StopWords is not null)
        {
            args.AddRange(ToArgs(ValkeyLiterals.STOPWORDS, options.StopWords));
        }

        if (!options.Punctuation.IsNull)
        {
            args.Add(ValkeyLiterals.PUNCTUATION);
            args.Add((GlideString)options.Punctuation);
        }

        return [.. args];
    }

    /// <summary>
    /// Converts the given <see cref="Ft.Field"/> array to command arguments.
    /// </summary>
    private static GlideString[] ToArgs(IEnumerable<Ft.Field> schema)
    {
        List<GlideString> args = [ValkeyLiterals.SCHEMA];
        foreach (var field in schema)
        {
            args.AddRange(ToArgs(field));
        }
        return [.. args];
    }

    /// <summary>
    /// Converts the given <see cref="Ft.CreateOptions"/> to command arguments.
    /// </summary>
    private static GlideString[] ToArgs(Ft.SearchOptions? options)
    {
        if (options is null)
        {
            return [];
        }

        List<GlideString> args = [];

        var returnFields = options.Return?.ToArray();
        bool isNoContent = returnFields is { Length: 0 };

        if (isNoContent)
        {
            if (options.SortBy is { WithSortKeys: true })
            {
                throw new ArgumentException(
                    $"{nameof(Ft.SearchSortBy.WithSortKeys)} cannot be used with {nameof(Ft.SearchOptions.NoContent)}.");
            }

            args.Add(ValkeyLiterals.NOCONTENT);
        }

        if (options.SortBy is { WithSortKeys: true })
        {
            args.Add(ValkeyLiterals.WITHSORTKEYS);
        }

        if (options.Verbatim)
        {
            args.Add(ValkeyLiterals.VERBATIM);
        }

        if (options.Inconsistent)
        {
            args.Add(ValkeyLiterals.INCONSISTENT);
        }

        if (options.SomeShards)
        {
            args.Add(ValkeyLiterals.SOMESHARDS);
        }

        if (options.InOrder)
        {
            args.Add(ValkeyLiterals.INORDER);
        }

        if (options.Slop.HasValue)
        {
            args.Add(ValkeyLiterals.SLOP);
            args.Add(options.Slop.Value.ToGlideString());
        }

        if (options.Limit is not null)
        {
            args.Add(ValkeyLiterals.LIMIT);
            args.Add(options.Limit.Offset.ToGlideString());
            args.Add(options.Limit.Count.ToGlideString());
        }

        if (options.SortBy is not null)
        {
            args.Add(ValkeyLiterals.SORTBY);
            args.Add(options.SortBy.Field);
            switch (options.SortBy.Order)
            {
                case SortOrder.Ascending:
                    args.Add(ValkeyLiterals.ASC);
                    break;
                case SortOrder.Descending:
                    args.Add(ValkeyLiterals.DESC);
                    break;
                case SortOrder.Default:
                default:
                    break;
            }
        }

        if (returnFields is { Length: > 0 })
        {
            List<GlideString> fieldArgs = [];
            foreach (var rf in returnFields)
            {
                fieldArgs.Add(rf.Field);
                if (!rf.Name.IsNull)
                {
                    fieldArgs.Add(ValkeyLiterals.AS);
                    fieldArgs.Add(rf.Name);
                }
            }

            args.Add(ValkeyLiterals.RETURN);
            args.Add(fieldArgs.Count.ToGlideString());
            args.AddRange(fieldArgs);
        }

        if (options.Params.Count > 0)
        {
            args.Add(ValkeyLiterals.PARAMS);
            args.Add((options.Params.Count * 2).ToGlideString());
            foreach (var p in options.Params)
            {
                args.Add(p.Key);
                args.Add(p.Value);
            }
        }

        if (options.Timeout.HasValue)
        {
            args.Add(ValkeyLiterals.TIMEOUT);
            args.Add(ToMilliseconds(options.Timeout.Value));
        }

        return [.. args];
    }

    /// <summary>
    /// Converts the given <see cref="Ft.AggregateOptions"/> to command arguments.
    /// </summary>
    private static GlideString[] ToArgs(Ft.AggregateOptions? options)
    {
        if (options is null)
        {
            return [];
        }

        List<GlideString> args = [];

        if (options.Verbatim)
        {
            args.Add(ValkeyLiterals.VERBATIM);
        }

        if (options.InOrder)
        {
            args.Add(ValkeyLiterals.INORDER);
        }

        if (options.Slop.HasValue)
        {
            args.Add(ValkeyLiterals.SLOP);
            args.Add(options.Slop.Value.ToGlideString());
        }

        if (options.LoadFields is null)
        {
            args.Add(ValkeyLiterals.LOAD);
            args.Add(ValkeyLiterals.STAR);
        }
        else
        {
            args.AddRange(ToArgs(ValkeyLiterals.LOAD, options.LoadFields));
        }

        if (options.Params.Count > 0)
        {
            args.Add(ValkeyLiterals.PARAMS);
            args.Add((options.Params.Count * 2).ToGlideString());
            foreach (var p in options.Params)
            {
                args.Add(p.Key);
                args.Add(p.Value);
            }
        }

        if (options.Timeout.HasValue)
        {
            args.Add(ValkeyLiterals.TIMEOUT);
            args.Add(ToMilliseconds(options.Timeout.Value));
        }

        foreach (var clause in options.Clauses)
        {
            args.AddRange(clause.ToArgs());
        }

        return [.. args];
    }

    /// <summary>
    /// Converts the given <see cref="Ft.InfoOptions"/> to command arguments.
    /// </summary>
    private static GlideString[] ToArgs(Ft.InfoOptions? options)
    {
        if (options is null)
        {
            return [];
        }

        List<GlideString> args =
        [
            options.Scope switch
            {
                Ft.InfoScope.Local => ValkeyLiterals.LOCAL,
                Ft.InfoScope.Cluster => ValkeyLiterals.CLUSTER,
                Ft.InfoScope.Primary => ValkeyLiterals.PRIMARY,
                _ => throw new ArgumentOutOfRangeException(nameof(options.Scope)),
            },
        ];

        if (options.SomeShards)
        {
            args.Add(ValkeyLiterals.SOMESHARDS);
        }

        if (options.Inconsistent)
        {
            args.Add(ValkeyLiterals.INCONSISTENT);
        }

        return [.. args];
    }

    /// <summary>
    /// Converts the given <see cref="Ft.Field"/> to command arguments.
    /// </summary>
    private static GlideString[] ToArgs(Ft.Field field)
    {
        List<GlideString> args = [field.Identifier];

        if (!field.Alias.IsNull)
        {
            args.Add(ValkeyLiterals.AS);
            args.Add(field.Alias);
        }

        switch (field)
        {
            case Ft.TextField text:
                args.Add(ValkeyLiterals.TEXT);
                if (text.NoStem)
                {
                    args.Add(ValkeyLiterals.NOSTEM);
                }
                if (text.NoSuffixTrie)
                {
                    args.Add(ValkeyLiterals.NOSUFFIXTRIE);
                }
                break;

            case Ft.TagField tag:
                args.Add(ValkeyLiterals.TAG);
                if (tag.Separator.HasValue)
                {
                    args.Add(ValkeyLiterals.SEPARATOR);
                    args.Add(tag.Separator.Value.ToString());
                }
                if (tag.CaseSensitive)
                {
                    args.Add(ValkeyLiterals.CASESENSITIVE);
                }

                break;

            case Ft.NumericField:
                args.Add(ValkeyLiterals.NUMERIC);
                break;

            case Ft.VectorFieldFlat flat:
                args.Add(ValkeyLiterals.VECTOR);
                args.Add(ValkeyLiterals.FLAT);
                List<GlideString> flatAttrs =
                [
                    ValkeyLiterals.DIM, flat.Dimensions.ToGlideString(),
                    ValkeyLiterals.DISTANCE_METRIC, ToArgs(flat.DistanceMetric),
                    ValkeyLiterals.TYPE, ValkeyLiterals.FLOAT32,
                ];
                if (flat.InitialCap.HasValue)
                {
                    flatAttrs.Add(ValkeyLiterals.INITIAL_CAP);
                    flatAttrs.Add(flat.InitialCap.Value.ToGlideString());
                }
                args.Add(flatAttrs.Count.ToGlideString());
                args.AddRange(flatAttrs);
                break;

            case Ft.VectorFieldHnsw hnsw:
                args.Add(ValkeyLiterals.VECTOR);
                args.Add(ValkeyLiterals.HNSW);
                List<GlideString> hnswAttrs =
                [
                    ValkeyLiterals.DIM, hnsw.Dimensions.ToGlideString(),
                    ValkeyLiterals.DISTANCE_METRIC, ToArgs(hnsw.DistanceMetric),
                    ValkeyLiterals.TYPE, ValkeyLiterals.FLOAT32,
                ];
                if (hnsw.InitialCap.HasValue)
                {
                    hnswAttrs.Add(ValkeyLiterals.INITIAL_CAP);
                    hnswAttrs.Add(hnsw.InitialCap.Value.ToGlideString());
                }
                if (hnsw.NumberOfEdges.HasValue)
                {
                    hnswAttrs.Add(ValkeyLiterals.M);
                    hnswAttrs.Add(hnsw.NumberOfEdges.Value.ToGlideString());
                }
                if (hnsw.VectorsExaminedOnConstruction.HasValue)
                {
                    hnswAttrs.Add(ValkeyLiterals.EF_CONSTRUCTION);
                    hnswAttrs.Add(hnsw.VectorsExaminedOnConstruction.Value.ToGlideString());
                }
                if (hnsw.VectorsExaminedOnRuntime.HasValue)
                {
                    hnswAttrs.Add(ValkeyLiterals.EF_RUNTIME);
                    hnswAttrs.Add(hnsw.VectorsExaminedOnRuntime.Value.ToGlideString());
                }
                args.Add(hnswAttrs.Count.ToGlideString());
                args.AddRange(hnswAttrs);
                break;

            default:
                throw new ArgumentException($"Unknown field type: {field.GetType()}");
        }

        return [.. args];
    }

    /// <summary>
    /// Converts the given <see cref="Ft.DistanceMetric"/> to command arguments.
    /// </summary>
    private static GlideString ToArgs(Ft.DistanceMetric metric) => metric switch
    {
        Ft.DistanceMetric.Cosine => ValkeyLiterals.COSINE,
        Ft.DistanceMetric.Euclidean => ValkeyLiterals.L2,
        Ft.DistanceMetric.InnerProduct => ValkeyLiterals.IP,
        _ => throw new ArgumentOutOfRangeException(nameof(metric)),
    };

    // TODO #360: refactor later
    // -------------------------

    private static Ft.SearchResult ParseFtSearchResponse(object[] data)
    {
        // Format: [count, {key1: fields1, key2: fields2, ...}]
        // Without WITHSORTKEYS: fields is Dictionary<GlideString, object>
        // With WITHSORTKEYS: fields is object[] { sortKey, Dictionary<GlideString, object> }
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

                if (kvp.Value is object[] pair && pair.Length == 2 && pair[1] is Dictionary<GlideString, object> sortedFieldMap)
                {
                    sortKey = pair[0] is GlideString gs ? (ValkeyValue)gs : ValkeyValue.Null;
                    fields = sortedFieldMap.ToDictionary(
                        f => (ValkeyValue)f.Key,
                        f => (ValkeyValue)(GlideString)f.Value);
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
