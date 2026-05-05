// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.ServerModules;

using static Valkey.Glide.Errors;
using static Valkey.Glide.Internals.FFI;

namespace Valkey.Glide.Internals;

internal partial class Request
{
    #region Public Methods

    public static Cmd<string, string> FtCreate(ValkeyKey indexName, IEnumerable<Ft.CreateField> schema, Ft.CreateOptions? options = null)
        => Simple<string>(RequestType.FtCreate, [indexName, .. ToArgs(options), .. ToArgs(schema)]);

    public static Cmd<string, string> FtDropIndex(ValkeyKey indexName)
        => Simple<string>(RequestType.FtDropIndex, [indexName]);

    public static Cmd<object[], ISet<ValkeyValue>> FtList()
        => new(RequestType.FtList, [], false, ToValkeyValueSet);

    public static Cmd<object[], Ft.SearchResult> FtSearch(ValkeyKey indexName, ValkeyValue query, Ft.SearchOptions? options = null)
        => new(RequestType.FtSearch, [indexName, query, .. ToArgs(options)], false, ParseFtSearchResponse);

    public static Cmd<object[], IDictionary<ValkeyValue, ValkeyValue>[]> FtAggregate(ValkeyKey indexName, ValkeyValue query, Ft.AggregateOptions? options = null)
        => new(RequestType.FtAggregate, [indexName, query, .. ToArgs(options)], false, ParseFtAggregateResponse);

    public static Cmd<object, Ft.InfoLocalResult> FtInfoLocal(ValkeyKey indexName, Ft.InfoOptions? options = null)
        => new(RequestType.FtInfo, [indexName, ValkeyLiterals.LOCAL, .. ToArgs(options)], false, ParseFtInfoLocalResponse);

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

        var prefixes = options.Prefixes;
        if (prefixes.Count() > 0)
        {
            args.AddRange(ToArgs(ValkeyLiterals.PREFIX, prefixes));
        }

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
    /// Converts the given <see cref="Ft.CreateField"/> array to command arguments.
    /// </summary>
    private static GlideString[] ToArgs(IEnumerable<Ft.CreateField> schema)
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
        else if (options.LoadFields.Any())
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
    /// Converts the given <see cref="Ft.InfoOptions"/> to command arguments (without scope keyword).
    /// </summary>
    private static GlideString[] ToArgs(Ft.InfoOptions? options)
    {
        if (options is null)
        {
            return [];
        }

        List<GlideString> args = [];

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
    /// Converts the given <see cref="Ft.CreateField"/> to command arguments.
    /// </summary>
    private static GlideString[] ToArgs(Ft.CreateField field)
    {
        List<GlideString> args = [field.Identifier];

        if (!field.Alias.IsNull)
        {
            args.Add(ValkeyLiterals.AS);
            args.Add(field.Alias);
        }

        switch (field)
        {
            case Ft.CreateTextField text:
                args.Add(ValkeyLiterals.TEXT);
                if (text.NoStem)
                {
                    args.Add(ValkeyLiterals.NOSTEM);
                }
                if (!text.WithSuffixTrie)
                {
                    args.Add(ValkeyLiterals.NOSUFFIXTRIE);
                }
                break;

            case Ft.CreateTagField tag:
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

            case Ft.CreateNumericField:
                args.Add(ValkeyLiterals.NUMERIC);
                break;

            case Ft.CreateVectorFieldFlat flat:
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

            case Ft.CreateVectorFieldHnsw hnsw:
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

    private static IDictionary<ValkeyValue, ValkeyValue>[] ParseFtAggregateResponse(object[] data)
    {
        var results = new List<IDictionary<ValkeyValue, ValkeyValue>>();
        foreach (var row in data)
        {
            if (row is Dictionary<GlideString, object> map)
            {
                results.Add(map.ToDictionary(
                    kvp => (ValkeyValue)kvp.Key,
                    kvp => kvp.Value switch
                    {
                        null => ValkeyValue.Null,
                        GlideString gs => (ValkeyValue)gs,
                        long l => (ValkeyValue)l,
                        double d => (ValkeyValue)d,
                        bool b => (ValkeyValue)b,
                        _ => throw new RequestException($"Unexpected FT.AGGREGATE value type: {kvp.Value.GetType()}"),
                    }));
            }
        }
        return [.. results];
    }

    private static Ft.InfoLocalResult ParseFtInfoLocalResponse(object data)
    {
        var map = ToStringMap(data);
        var indexDefinition = map.TryGetValue("index_definition", out var definitionObj)
            ? ToStringMap(definitionObj)
            : throw new RequestException("FT.INFO LOCAL response missing 'index_definition'");

        return new Ft.InfoLocalResult
        {
            IndexName = GetString(map, "index_name"),
            KeyType = GetString(indexDefinition, "key_type") switch
            {
                "HASH" => Ft.DataType.Hash,
                "JSON" => Ft.DataType.Json,
                var unknown => throw new RequestException($"Unknown FT.INFO key_type: '{unknown}'"),
            },
            Prefixes = GetValkeyValues(indexDefinition, "prefixes"),
            Attributes = ParseInfoFields(map),
            NumDocs = GetLong(map, "num_docs"),
            NumRecords = GetLong(map, "num_records"),
            TotalTermOccurrences = GetLong(map, "total_term_occurrences"),
            NumTerms = GetLong(map, "num_terms"),
            HashIndexingFailures = GetLong(map, "hash_indexing_failures"),
            BackfillInProgress = GetBool(map, "backfill_in_progress"),
            BackfillCompletePercent = GetDouble(map, "backfill_complete_percent"),
            MutationQueueSize = GetLong(map, "mutation_queue_size"),
            RecentMutationsQueueDelay = GetTimeSpan(map, "recent_mutations_queue_delay"),
            State = ParseInfoState(GetString(map, "state")),
            Punctuation = TryGetValkeyValue(map, "punctuation"),
            StopWords = TryGetValkeyValues(map, "stop_words"),
            WithOffsets = TryGetBool(map, "with_offsets"),
            MinStemSize = TryGetLong(map, "min_stem_size"),
        };
    }

    private static Ft.InfoState ParseInfoState(string state) => state switch
    {
        "ready" => Ft.InfoState.Ready,
        "backfill_in_progress" => Ft.InfoState.BackfillInProgress,
        "backfill_paused_by_oom" => Ft.InfoState.BackfillPausedByOom,
        _ => throw new RequestException($"Unknown FT.INFO state: '{state}'"),
    };

    private static Ft.InfoField[] ParseInfoFields(Dictionary<string, object> map)
    {
        if (!map.TryGetValue("attributes", out var attrsObj) || attrsObj is not object[] attrs)
        {
            return [];
        }

        var fields = new List<Ft.InfoField>();
        foreach (var attr in attrs)
        {
            var fieldMap = ToStringMap(attr);
            var type = GetString(fieldMap, "type");
            var identifier = GetValkeyValue(fieldMap, "identifier");
            var attribute = GetValkeyValue(fieldMap, "attribute");
            var userIndexedMemory = GetLong(fieldMap, "user_indexed_memory");

            Ft.InfoField field = type switch
            {
                "TEXT" => new Ft.InfoTextField
                {
                    Identifier = identifier,
                    Attribute = attribute,
                    UserIndexedMemory = userIndexedMemory,
                    WithSuffixTrie = GetBool(fieldMap, "WITH_SUFFIX_TRIE"),
                    NoStem = GetBool(fieldMap, "NO_STEM"),
                },
                "TAG" => new Ft.InfoTagField
                {
                    Identifier = identifier,
                    Attribute = attribute,
                    UserIndexedMemory = userIndexedMemory,
                    Separator = GetChar(fieldMap, "SEPARATOR"),
                    CaseSensitive = GetBool(fieldMap, "CASESENSITIVE"),
                    Size = GetLong(fieldMap, "size"),
                },
                "NUMERIC" => new Ft.InfoNumericField
                {
                    Identifier = identifier,
                    Attribute = attribute,
                    UserIndexedMemory = userIndexedMemory,
                },
                "VECTOR" => ParseInfoVectorField(identifier, attribute, userIndexedMemory, fieldMap),
                _ => throw new RequestException($"Unknown FT.INFO field type: '{type}'"),
            };

            fields.Add(field);
        }

        return [.. fields];
    }

    private static Ft.InfoVectorField ParseInfoVectorField(
        ValkeyValue identifier, ValkeyValue attribute, long userIndexedMemory,
        Dictionary<string, object> fieldMap)
    {
        var indexMap = fieldMap.TryGetValue("index", out var indexObj)
            ? ToStringMap(indexObj)
            : [];

        var algoMap = indexMap.TryGetValue("algorithm", out var algoObj)
            ? ToStringMap(algoObj)
            : [];

        var algoName = GetString(algoMap, "name");

        if (algoName is "HNSW")
        {
            return new Ft.InfoVectorFieldHnsw
            {
                Identifier = identifier,
                Attribute = attribute,
                UserIndexedMemory = userIndexedMemory,
                Capacity = GetLong(indexMap, "capacity"),
                Dimensions = GetLong(indexMap, "dimensions"),
                DistanceMetric = ParseDistanceMetric(GetString(indexMap, "distance_metric")),
                Size = GetLong(indexMap, "size"),
                M = GetLong(algoMap, "m"),
                EfConstruction = GetLong(algoMap, "ef_construction"),
                EfRuntime = GetLong(algoMap, "ef_runtime"),
            };
        }

        if (algoName is "FLAT")
        {
            return new Ft.InfoVectorFieldFlat
            {
                Identifier = identifier,
                Attribute = attribute,
                UserIndexedMemory = userIndexedMemory,
                Capacity = GetLong(indexMap, "capacity"),
                Dimensions = GetLong(indexMap, "dimensions"),
                DistanceMetric = ParseDistanceMetric(GetString(indexMap, "distance_metric")),
                Size = GetLong(indexMap, "size"),
                BlockSize = GetLong(algoMap, "block_size"),
            };
        }

        throw new RequestException($"Unknown FT.INFO vector algorithm: '{algoName}'");
    }

    private static Ft.DistanceMetric ParseDistanceMetric(string metric) => metric switch
    {
        "COSINE" => Ft.DistanceMetric.Cosine,
        "L2" => Ft.DistanceMetric.Euclidean,
        "IP" => Ft.DistanceMetric.InnerProduct,
        _ => throw new RequestException($"Unknown FT.INFO distance metric: '{metric}'"),
    };

    /// <summary>
    /// Converts a raw server response object to a string-keyed dictionary.
    /// Handles both <see cref="Dictionary{GlideString, Object}"/> (RESP3 maps) and
    /// <see cref="T:object[]"/> (RESP2 flat key-value pair arrays).
    /// </summary>
    private static Dictionary<string, object> ToStringMap(object data)
    {
        if (data is Dictionary<GlideString, object> glideMap)
        {
            return glideMap.ToDictionary(kvp => kvp.Key.ToString(), kvp => kvp.Value);
        }

        if (data is object[] arr && arr.Length >= 2)
        {
            var dict = new Dictionary<string, object>();
            for (int i = 0; i < arr.Length - 1; i += 2)
            {
                var key = arr[i] is GlideString gs ? gs.ToString() : arr[i]?.ToString() ?? string.Empty; // TODO
                dict[key] = arr[i + 1];
            }
            return dict;
        }

        return [];
    }

    /// <summary>
    /// Returns a <see langword="string"/> for the given key.
    /// </summary>
    /// <exception cref="RequestException">If the key does not exist.</exception>
    private static string GetString(Dictionary<string, object> map, string key)
        => TryGetString(map, key) ?? throw new RequestException($"FT.INFO response missing required field '{key}'");

    /// <summary>
    /// Returns a <see langword="string"/> for the given key, or <see langword="null"/> if absent.
    /// </summary>
    private static string? TryGetString(Dictionary<string, object> map, string key)
        => map.TryGetValue(key, out var value) ? ((GlideString)value).ToString() : null;

    /// <summary>
    /// Returns a <see cref="TimeSpan"/> parsed from a server duration string (e.g. <c>"0.123 sec"</c>).
    /// </summary>
    /// <exception cref="RequestException">If the key does not exist.</exception>
    private static TimeSpan GetTimeSpan(Dictionary<string, object> map, string key)
        => TimeSpan.FromSeconds(double.Parse(GetString(map, key).Replace(" sec", "")));

    /// <summary>
    /// Returns a single <see langword="char"/> for the given key.
    /// </summary>
    /// <exception cref="RequestException">If the key does not exist or the value is not exactly one character.</exception>
    private static char GetChar(Dictionary<string, object> map, string key)
    {
        var s = GetString(map, key);
        return s.Length == 1 ? s[0] : throw new RequestException($"FT.INFO field '{key}' expected single character, got '{s}'");
    }

    /// <summary>
    /// Returns a <see langword="long"/> for the given key.
    /// </summary>
    /// <exception cref="RequestException">If the key does not exist.</exception>
    private static long GetLong(Dictionary<string, object> map, string key)
        => TryGetLong(map, key) ?? throw new RequestException($"FT.INFO response missing required field '{key}'");

    /// <summary>
    /// Returns a <see langword="long"/> for the given key, or <see langword="null"/> if absent.
    /// </summary>
    private static long? TryGetLong(Dictionary<string, object> map, string key)
    {
        if (!map.TryGetValue(key, out var value))
        {
            return null;
        }

        return value switch
        {
            long l => l,
            GlideString gs => long.Parse(gs.ToString()),
            _ => throw new RequestException($"FT.INFO field '{key}' expected long or string, got {value.GetType()}"),
        };
    }

    /// <summary>
    /// Returns a <see langword="double"/> for the given key.
    /// </summary>
    /// <exception cref="RequestException">If the key does not exist.</exception>
    private static double GetDouble(Dictionary<string, object> map, string key)
        => TryGetDouble(map, key) ?? throw new RequestException($"FT.INFO response missing required field '{key}'");

    /// <summary>
    /// Returns a <see langword="double"/> for the given key, or <see langword="null"/> if absent.
    /// </summary>
    private static double? TryGetDouble(Dictionary<string, object> map, string key)
    {
        if (!map.TryGetValue(key, out var value))
        {
            return null;
        }

        return value switch
        {
            double d => d,
            long l => l,
            GlideString gs => double.Parse(gs.ToString()),
            _ => throw new RequestException($"FT.INFO field '{key}' expected double or string, got {value.GetType()}"),
        };
    }

    /// <summary>
    /// Returns a <see langword="bool"/> for the given key.
    /// Values are string-encoded as <c>"0"</c> or <c>"1"</c> by the server.
    /// </summary>
    /// <exception cref="RequestException">If the key does not exist.</exception>
    private static bool GetBool(Dictionary<string, object> map, string key)
        => TryGetBool(map, key) ?? throw new RequestException($"FT.INFO response missing required field '{key}'");

    /// <summary>
    /// Returns a <see langword="bool"/> for the given key, or <see langword="null"/> if absent.
    /// </summary>
    private static bool? TryGetBool(Dictionary<string, object> map, string key)
    {
        if (!map.TryGetValue(key, out var value))
        {
            return null;
        }

        var s = value is GlideString gs ? gs.ToString() : value?.ToString();
        return s is "1" or "true";
    }

    /// <summary>
    /// Returns a <see cref="ValkeyValue"/> for the given key.
    /// </summary>
    /// <exception cref="RequestException">If the key does not exist.</exception>
    private static ValkeyValue GetValkeyValue(Dictionary<string, object> map, string key)
    {
        var result = TryGetValkeyValue(map, key);
        return result != ValkeyValue.Null ? result : throw new RequestException($"FT.INFO response missing required field '{key}'");
    }

    /// <summary>
    /// Returns a <see cref="ValkeyValue"/> for the given key, or <see cref="ValkeyValue.Null"/> if absent.
    /// </summary>
    private static ValkeyValue TryGetValkeyValue(Dictionary<string, object> map, string key)
        => map.TryGetValue(key, out var value) ? (GlideString)value : ValkeyValue.Null;

    /// <summary>
    /// Returns a <see cref="ValkeyValue"/> array for the given key.
    /// </summary>
    /// <exception cref="RequestException">If the key does not exist.</exception>
    private static ValkeyValue[] GetValkeyValues(Dictionary<string, object> map, string key)
        => TryGetValkeyValues(map, key) ?? throw new RequestException($"FT.INFO response missing required field '{key}'");

    /// <summary>
    /// Returns a <see cref="ValkeyValue"/> array for the given key, or <see langword="null"/> if absent.
    /// </summary>
    private static ValkeyValue[]? TryGetValkeyValues(Dictionary<string, object> map, string key)
    {
        if (!map.TryGetValue(key, out var value))
        {
            return null;
        }

        IEnumerable<object> items = value switch
        {
            object[] arr => arr,
            HashSet<object> set => set,
            _ => throw new Errors.RequestException($"FT.INFO field '{key}' expected array, got {value.GetType()}"),
        };

        return [.. items.Cast<GlideString>().Select(gs => (ValkeyValue)gs)];
    }

    #endregion
}
