// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using System.Globalization;
using System.Text;
using System.Text.Json;

using Valkey.Glide.ServerModules;

using static Valkey.Glide.Errors;
using static Valkey.Glide.Internals.FFI;
using static Valkey.Glide.Internals.TimeUtils;

namespace Valkey.Glide.Internals;

internal static partial class Request
{
    #region Command Builders

    public static Cmd<string, string> FtCreate(ValkeyKey indexName, IEnumerable<Ft.CreateField> schema, Ft.CreateOptions? options = null)
        => Simple<string>(RequestType.FtCreate, [indexName, .. ToArgs(options), .. ToArgs(schema)]);

    public static Cmd<string, string> FtDropIndex(ValkeyKey indexName)
        => Simple<string>(RequestType.FtDropIndex, [indexName]);

    public static Cmd<object[], ISet<ValkeyValue>> FtList()
        => new(RequestType.FtList, [], false, ToValkeyValueSet);

    public static Cmd<object[], Ft.SearchResult> FtSearch(ValkeyKey indexName, ValkeyValue query, Ft.SearchOptions? options = null)
        => new(RequestType.FtSearch, [indexName, query, .. ToArgs(options)], false, data => ParseFtSearchResponse(data, options));

    public static Cmd<object[], IDictionary<ValkeyValue, ValkeyValue>[]> FtAggregate(ValkeyKey indexName, ValkeyValue query, Ft.AggregateOptions? options = null)
    {
        if (options?.WithCursorOptions is not null)
        {
            throw new ArgumentException($"{nameof(Ft.AggregateOptions.WithCursorOptions)} is set; use {nameof(Ft.AggregateWithCursorAsync)} to page results.");
        }

        return new(RequestType.FtAggregate, [indexName, query, .. ToArgs(options)], false, ParseFtAggregateResponse);
    }

    public static Cmd<object?, Ft.CursorResult> FtAggregateWithCursor(ValkeyKey indexName, ValkeyValue query, Ft.AggregateOptions options)
        => new(RequestType.FtAggregate, [indexName, query, .. ToArgs(options), .. (options.WithCursorOptions ?? new Ft.WithCursor()).ToArgs()], true, ParseCursorWrappedAggregateResponse, true);

    public static Cmd<object?, Ft.CursorResult> FtCursorRead(ValkeyKey indexName, long cursorId, long? count)
    {
        List<GlideString> args = [ValkeyLiterals.CURSOR, ValkeyLiterals.READ, indexName, cursorId.ToGlideString()];
        if (count.HasValue)
        {
            args.Add(ValkeyLiterals.COUNT);
            args.Add(count.Value.ToGlideString());
        }

        return new(RequestType.CustomCommand, [.. args], true, ParseCursorWrappedAggregateResponse, true);
    }

    public static Cmd<string, string> FtCursorDel(ValkeyKey indexName, long cursorId)
        => Simple<string>(RequestType.CustomCommand, [ValkeyLiterals.CURSOR, ValkeyLiterals.DEL, indexName, cursorId.ToGlideString()]);

    public static Cmd<object?, IDictionary<ValkeyValue, ValkeyValue>[]> FtHybrid(ValkeyKey indexName, Ft.HybridSearchArm searchArm, Ft.HybridVsimArm vsimArm, Ft.HybridOptions? options = null)
    {
        if (options?.WithCursorOptions is not null)
        {
            throw new ArgumentException($"{nameof(Ft.HybridOptions.WithCursorOptions)} is set; use {nameof(Ft.HybridWithCursorAsync)} to page results.");
        }

        return new(RequestType.CustomCommand, BuildHybridArgs(indexName, searchArm, vsimArm, options, withCursor: false), true, data => data is null ? [] : ParseFtAggregateResponse((object[])data), true);
    }

    public static Cmd<object?, Ft.CursorResult> FtHybridWithCursor(ValkeyKey indexName, Ft.HybridSearchArm searchArm, Ft.HybridVsimArm vsimArm, Ft.HybridOptions options)
        => new(RequestType.CustomCommand, BuildHybridArgs(indexName, searchArm, vsimArm, options, withCursor: true), true, ParseCursorWrappedAggregateResponse, true);

    public static Cmd<object, Ft.InfoLocalResult> FtInfoLocal(ValkeyKey indexName, Ft.InfoOptions? options = null)
        => new(RequestType.FtInfo, [indexName, ValkeyLiterals.LOCAL, .. ToArgs(options)], false, ParseFtInfoLocalResponse);

    #endregion
    #region Argument Builders

    /// <summary>
    /// Converts the given <see cref="Ft.CreateOptions"/> to command arguments.
    /// </summary>
    /// <param name="options">The index creation options to convert.</param>
    /// <exception cref="ArgumentException"></exception>
    private static GlideString[] ToArgs(Ft.CreateOptions? options)
    {
        if (options is null)
        {
            return [];
        }

        if (options.WithOffsets && options.NoOffsets)
        {
            throw new ArgumentException($"{nameof(Ft.CreateOptions.WithOffsets)} and {nameof(Ft.CreateOptions.NoOffsets)} are mutually exclusive.");
        }

        if (!options.Filter.IsNull && options.Filter.IsNullOrEmpty)
        {
            throw new ArgumentException($"{nameof(Ft.CreateOptions.Filter)} cannot be empty.");
        }

        List<GlideString> args = [
            ValkeyLiterals.ON,
            .. options.DataType.ToArgs()];

        var prefixes = options.Prefixes;
        if (prefixes.Any())
        {
            args.AddRange(ToArgs(ValkeyLiterals.PREFIX, prefixes));
        }

        if (options.Score.HasValue)
        {
            args.Add(ValkeyLiterals.SCORE);
            args.Add(options.Score.Value.ToString(System.Globalization.CultureInfo.InvariantCulture));
        }

        if (!options.Language.IsNull)
        {
            args.Add(ValkeyLiterals.LANGUAGE);
            args.Add((GlideString)options.Language);
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

        if (options.WithOffsets)
        {
            args.Add(ValkeyLiterals.WITHOFFSETS);
        }
        else if (options.NoOffsets)
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

        if (!options.Filter.IsNull)
        {
            args.Add(ValkeyLiterals.FILTER);
            args.Add((GlideString)options.Filter);
        }

        if (!options.ScoreField.IsNull)
        {
            args.Add(ValkeyLiterals.SCORE_FIELD);
            args.Add((GlideString)options.ScoreField);
        }

        if (options.NoHighlight)
        {
            args.Add(ValkeyLiterals.NOHL);
        }

        return [.. args];
    }

    /// <summary>
    /// Converts the given <see cref="Ft.CreateField"/> array to command arguments.
    /// </summary>
    /// <param name="schema">The index schema fields to convert.</param>
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
    /// Converts the given <see cref="Ft.SearchOptions"/> to command arguments.
    /// </summary>
    /// <param name="options">The search options to convert.</param>
    /// <exception cref="ArgumentException"></exception>
    private static GlideString[] ToArgs(Ft.SearchOptions? options)
    {
        if (options is null)
        {
            return [];
        }

        if (options.Count && options.Limit is not null)
        {
            throw new ArgumentException(
                $"{nameof(Ft.SearchOptions.Count)} and {nameof(Ft.SearchOptions.Limit)} are mutually exclusive -- " +
                    $"{nameof(Ft.SearchOptions.Count)} maps to LIMIT 0 0 internally, so an explicit limit would conflict with it.");
        }

        List<GlideString> args = [];

        var returnFields = options.Return?.ToArray();
        bool isNoContent = returnFields is { Length: 0 };

        if (isNoContent)
        {
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

        if (options.InKeys is not null)
        {
            var inKeys = options.InKeys.ToArray();
            args.Add(ValkeyLiterals.INKEYS);
            args.Add(inKeys.Length.ToGlideString());
            args.AddRange(inKeys.Select(k => (GlideString)k));
        }

        if (options.Limit is not null)
        {
            args.Add(ValkeyLiterals.LIMIT);
            args.Add(options.Limit.Offset.ToGlideString());
            args.Add(options.Limit.Count.ToGlideString());
        }
        else if (options.Count)
        {
            args.Add(ValkeyLiterals.LIMIT);
            args.Add("0");
            args.Add("0");
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
            args.AddRange(ToParams(options.Params));
        }

        if (options.Timeout.HasValue)
        {
            args.Add(ValkeyLiterals.TIMEOUT);
            args.Add(ToULongMs(options.Timeout.Value, nameof(options.Timeout)).ToGlideString());
        }

        if (options.WithScores)
        {
            args.Add(ValkeyLiterals.WITHSCORES);
        }

        if (!options.Scorer.IsNull)
        {
            args.Add(ValkeyLiterals.SCORER);
            args.Add((GlideString)options.Scorer);
        }

        return [.. args];
    }

    /// <summary>
    /// Converts the given <see cref="Ft.AggregateOptions"/> to command arguments.
    /// </summary>
    /// <param name="options">The aggregation options to convert.</param>
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

        args.AddRange(ToLoadArgs(options.LoadFieldEntries, options.LoadFields));

        if (options.Params.Count > 0)
        {
            args.AddRange(ToParams(options.Params));
        }

        if (options.Timeout.HasValue)
        {
            args.Add(ValkeyLiterals.TIMEOUT);
            args.Add(ToULongMs(options.Timeout.Value, nameof(options.Timeout)).ToGlideString());
        }

        foreach (var clause in options.Clauses)
        {
            args.AddRange(clause.ToArgs());
        }

        if (options.Dialect.HasValue)
        {
            args.Add(ValkeyLiterals.DIALECT);
            args.Add(options.Dialect.Value.ToGlideString());
        }

        if (options.AddScores)
        {
            args.Add(ValkeyLiterals.ADDSCORES);
        }

        if (!options.Scorer.IsNull)
        {
            args.Add(ValkeyLiterals.SCORER);
            args.Add((GlideString)options.Scorer);
        }

        return [.. args];
    }

    private static GlideString[] ToLoadArgs(IEnumerable<Ft.LoadField>? entries, IEnumerable<ValkeyValue>? plainFields)
    {
        if (entries is not null)
        {
            var entryArray = entries.ToArray();
            if (entryArray.Length == 0)
            {
                return [];
            }

            List<GlideString> fieldArgs = [];
            foreach (var entry in entryArray)
            {
                fieldArgs.AddRange(entry.ToArgs());
            }

            return [ValkeyLiterals.LOAD, fieldArgs.Count.ToGlideString(), .. fieldArgs];
        }

        if (plainFields is null)
        {
            return [ValkeyLiterals.LOAD, ValkeyLiterals.STAR];
        }

        var fields = plainFields.ToArray();
        return fields.Length == 0 ? [] : ToArgs(ValkeyLiterals.LOAD, fields);
    }

    /// <summary>
    /// Converts the given <see cref="Ft.InfoOptions"/> to command arguments (without scope keyword).
    /// </summary>
    /// <param name="options">The info options to convert.</param>
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
    /// <param name="field">The schema field to convert.</param>
    /// <exception cref="ArgumentException">Thrown if the field type is not supported.</exception>
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

                if (text.Weight.HasValue)
                {
                    args.Add(ValkeyLiterals.WEIGHT);
                    args.Add(text.Weight.Value.ToString(System.Globalization.CultureInfo.InvariantCulture));
                }

                if (!text.WithSuffixTrie)
                {
                    args.Add(ValkeyLiterals.NOSUFFIXTRIE);
                }

                AddSortableUnf(args, text.Sortable, text.Unf);
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

                AddSortableUnf(args, tag.Sortable, tag.Unf);
                break;

            case Ft.CreateNumericField numeric:
                args.Add(ValkeyLiterals.NUMERIC);
                AddSortableUnf(args, numeric.Sortable, numeric.Unf);
                break;

            case Ft.CreateVectorFieldFlat flat:
                args.Add(ValkeyLiterals.VECTOR);
                args.Add(ValkeyLiterals.FLAT);
                List<GlideString> flatAttrs =
                [
                    ValkeyLiterals.DIM, flat.Dimensions.ToGlideString(),
                    ValkeyLiterals.DISTANCE_METRIC, .. flat.DistanceMetric.ToArgs(),
                    ValkeyLiterals.TYPE, flat.Type.ToArg(),
                ];
                if (flat.InitialCap.HasValue)
                {
                    flatAttrs.Add(ValkeyLiterals.INITIAL_CAP);
                    flatAttrs.Add(flat.InitialCap.Value.ToGlideString());
                }

                args.Add(flatAttrs.Count.ToGlideString());
                args.AddRange(flatAttrs);
                if (flat.Sortable)
                {
                    args.Add(ValkeyLiterals.SORTABLE);
                }

                break;

            case Ft.CreateVectorFieldHnsw hnsw:
                args.Add(ValkeyLiterals.VECTOR);
                args.Add(ValkeyLiterals.HNSW);
                List<GlideString> hnswAttrs =
                [
                    ValkeyLiterals.DIM, hnsw.Dimensions.ToGlideString(),
                    ValkeyLiterals.DISTANCE_METRIC, .. hnsw.DistanceMetric.ToArgs(),
                    ValkeyLiterals.TYPE, hnsw.Type.ToArg(),
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
                if (hnsw.Sortable)
                {
                    args.Add(ValkeyLiterals.SORTABLE);
                }

                break;

            default:
                throw new ArgumentException($"Unknown field type: {field.GetType()}");
        }

        return [.. args];
    }

    private static void AddSortableUnf(List<GlideString> args, bool sortable, bool unf)
    {
        if (unf && !sortable)
        {
            throw new ArgumentException("Unf requires Sortable to be set.");
        }

        if (sortable)
        {
            args.Add(ValkeyLiterals.SORTABLE);
            if (unf)
            {
                args.Add(ValkeyLiterals.UNF);
            }
        }
    }

    /// <summary>
    /// Converts parameter name-value pairs into a counted array: <c>PARAMS nargs name1 value1 name2 value2 ...</c>.
    /// </summary>
    /// <param name="pairs">The parameter name-value pairs to convert.</param>
    private static GlideString[] ToParams(IEnumerable<KeyValuePair<ValkeyValue, ValkeyValue>> pairs)
    {
        ValkeyValue[] values = ToValkeyValues(pairs);
        return [ValkeyLiterals.PARAMS, values.Length.ToGlideString(), .. values];
    }

    #endregion
    #region Response Converters

    private static Ft.SearchResult ParseFtSearchResponse(object[] data, Ft.SearchOptions? options)
    {
        if (data.Length == 0)
        {
            return new Ft.SearchResult(0, []);
        }

        long count = Convert.ToInt64(data[0], CultureInfo.InvariantCulture);

        // Format: [count, {key1: fields1, key2: fields2, ...}]
        // Without WITHSORTKEYS: fields is Dictionary<GlideString, object>
        // With WITHSORTKEYS: fields is object[] { sortKey, Dictionary<GlideString, object> }
        if (data.Length < 2)
        {
            return new Ft.SearchResult(count, []);
        }

        bool withScores = options?.WithScores ?? false;
        bool withSortKeys = options?.SortBy is { WithSortKeys: true };

        List<Ft.SearchDocument> docs = [];
        if (data[1] is Dictionary<GlideString, object> map)
        {
            foreach (var kvp in map)
            {
                ValkeyKey key = (ValkeyKey)kvp.Key.Bytes;
                docs.Add(ParseFtSearchDocument(key, kvp.Value, withScores, withSortKeys));
            }
        }

        return new Ft.SearchResult(count, [.. docs]);
    }

    private static Ft.SearchDocument ParseFtSearchDocument(ValkeyKey key, object value, bool withScores, bool withSortKeys)
    {
        if (!withScores && !withSortKeys)
        {
            return new Ft.SearchDocument(key, ToFieldMap(value));
        }

        if (value is not object[] tuple)
        {
            throw new RequestException(
                $"Unexpected FT.SEARCH document shape for key '{key}' (WithScores={withScores}, WithSortKeys={withSortKeys}): expected a tuple, got {value?.GetType()}");
        }

        switch (tuple.Length)
        {
            case 2:
                if (withScores)
                {
                    return new Ft.SearchDocument(key, ToFieldMap(tuple[1]), score: ParseFtScore(key, tuple[0]));
                }

                ValkeyValue sortKey = tuple[0] is GlideString gs ? (ValkeyValue)gs : ValkeyValue.Null;
                return new Ft.SearchDocument(key, ToFieldMap(tuple[1]), sortKey: sortKey);

            case 3:
                double score = ParseFtScore(key, tuple[0]);
                ValkeyValue sortKey3 = tuple[1] is GlideString gs3 ? (ValkeyValue)gs3 : ValkeyValue.Null;
                return new Ft.SearchDocument(key, ToFieldMap(tuple[2]), sortKey: sortKey3, score: score);

            default:
                throw new RequestException(
                    $"Unexpected FT.SEARCH tuple length {tuple.Length} for key '{key}' (WithScores={withScores}, WithSortKeys={withSortKeys})");
        }
    }

    private static double ParseFtScore(ValkeyKey key, object value)
    {
        string? text = value switch
        {
            GlideString gs => gs.ToString(),
            string s => s,
            _ => null,
        };

        return text is not null && double.TryParse(text, System.Globalization.CultureInfo.InvariantCulture, out double score)
            ? score
            : throw new RequestException($"Unparseable FT.SEARCH score '{value}' for key '{key}'");
    }

    private static IDictionary<ValkeyValue, ValkeyValue> ToFieldMap(object value)
        => value is Dictionary<GlideString, object> map
            ? map.ToDictionary(f => (ValkeyValue)f.Key, f => (ValkeyValue)(GlideString)f.Value)
            : [];

    private static IDictionary<ValkeyValue, ValkeyValue>[] ParseFtAggregateResponse(object[] data)
    {
        var results = new List<IDictionary<ValkeyValue, ValkeyValue>>();
        foreach (var row in data)
        {
            if (row is Dictionary<GlideString, object> map)
            {
                results.Add(map.ToDictionary(
                    kvp => (ValkeyValue)kvp.Key,
                    kvp => ParseFtAggregateValue(kvp.Value)));
            }
        }

        return [.. results];
    }

    private static ValkeyValue ParseFtAggregateValue(object? value)
        => value switch
        {
            null => ValkeyValue.Null,
            GlideString gs => (ValkeyValue)gs,
            long l => (ValkeyValue)l,
            double d => (ValkeyValue)d,
            bool b => (ValkeyValue)b,
            object[] arr => (ValkeyValue)ToJsonArrayLiteral(arr),
            _ => throw new RequestException($"Unexpected FT.AGGREGATE value type: {value.GetType()}"),
        };

    private static string ToJsonArrayLiteral(object[] arr)
    {
        using var stream = new MemoryStream();
        using (var writer = new Utf8JsonWriter(stream))
        {
            WriteJsonValue(writer, arr);
        }

        return Encoding.UTF8.GetString(stream.ToArray());
    }

    private static void WriteJsonValue(Utf8JsonWriter writer, object? value)
    {
        switch (value)
        {
            case null:
                writer.WriteNullValue();
                break;
            case object[] array:
                writer.WriteStartArray();
                foreach (var item in array)
                {
                    WriteJsonValue(writer, item);
                }

                writer.WriteEndArray();
                break;
            case long l:
                writer.WriteNumberValue(l);
                break;
            case double d when double.IsFinite(d):
                writer.WriteNumberValue(d);
                break;
            case bool b:
                writer.WriteBooleanValue(b);
                break;
            case GlideString gs:
                writer.WriteStringValue(gs.ToString());
                break;
            default:
                writer.WriteStringValue(value.ToString());
                break;
        }
    }

    private static Ft.CursorResult ParseCursorWrappedAggregateResponse(object? data)
    {
        if (data is not object[] arr)
        {
            throw new RequestException($"Unexpected WITHCURSOR response type: {data?.GetType()}, expected object[]");
        }

        if (arr.Length != 2)
        {
            throw new RequestException($"Unexpected WITHCURSOR response: expected a 2-element [rows, cursorId] array, got {arr.Length} elements");
        }

        long cursorId = Convert.ToInt64(arr[1]);
        if (arr[0] is not object[] rows)
        {
            throw new RequestException($"Unexpected WITHCURSOR rows type: {arr[0]?.GetType()}, expected object[]");
        }

        return new Ft.CursorResult { Rows = ParseFtAggregateResponse(rows), CursorId = cursorId };
    }

    private static GlideString[] BuildHybridArgs(ValkeyKey indexName, Ft.HybridSearchArm searchArm, Ft.HybridVsimArm vsimArm, Ft.HybridOptions? options, bool withCursor)
    {
        List<GlideString> args = [ValkeyLiterals.HYBRID, indexName, .. searchArm.ToArgs()];

        GlideString? generatedParamName = null;
        ValkeyValue generatedParamValue = default;
        if (vsimArm.IsParamReference)
        {
            args.AddRange(vsimArm.ToArgs((GlideString)vsimArm.Vector));
        }
        else
        {
            generatedParamName = "$" + Guid.NewGuid().ToString("N");
            generatedParamValue = vsimArm.Vector;
            args.AddRange(vsimArm.ToArgs(generatedParamName));
        }

        if (options?.Combine is not null)
        {
            args.AddRange(options.Combine.ToArgs());
        }

        Ft.HybridOptions tailOptions = options ?? new();

        if (tailOptions.LocalOnly)
        {
            args.Add(ValkeyLiterals.LOCALONLY);
        }

        args.AddRange(ToHybridLoadArgs(tailOptions.LoadAll, tailOptions.LoadFields));

        if (tailOptions.Timeout.HasValue)
        {
            args.Add(ValkeyLiterals.TIMEOUT);
            args.Add(ToULongMs(tailOptions.Timeout.Value, nameof(tailOptions.Timeout)).ToGlideString());
        }

        Dictionary<ValkeyValue, ValkeyValue> mergedParams = [with(tailOptions.Params)];
        if (generatedParamName is not null)
        {
            ValkeyValue key = (ValkeyValue)(GlideString)generatedParamName.ToString()[1..];
            if (mergedParams.ContainsKey(key))
            {
                throw new ArgumentException($"Generated VSIM parameter name '{generatedParamName}' collides with an entry already present in {nameof(Ft.HybridOptions.Params)}; rename the caller-supplied entry.");
            }

            mergedParams[key] = generatedParamValue;
        }

        if (mergedParams.Count > 0)
        {
            args.AddRange(ToParams(mergedParams));
        }

        foreach (var clause in tailOptions.Clauses)
        {
            args.AddRange(clause.ToArgs());
        }

        if (withCursor)
        {
            args.AddRange((tailOptions.WithCursorOptions ?? new Ft.WithCursor()).ToArgs());
        }

        if (tailOptions.Dialect.HasValue)
        {
            args.Add(ValkeyLiterals.DIALECT);
            args.Add(tailOptions.Dialect.Value.ToGlideString());
        }

        return [.. args];
    }

    private static GlideString[] ToHybridLoadArgs(bool loadAll, IEnumerable<Ft.LoadField>? loadFields)
    {
        if (loadAll)
        {
            return [ValkeyLiterals.LOAD, ValkeyLiterals.STAR];
        }

        return loadFields is null ? [] : ToLoadArgs(loadFields, null);
    }

    private static Ft.InfoLocalResult ParseFtInfoLocalResponse(object data)
    {
        var map = (Dictionary<GlideString, object>)data;
        var indexDefinition = map.TryGetValue("index_definition", out var definitionObj)
            ? ToFlatMap(definitionObj)
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
            ScoreField = TryGetValkeyValue(indexDefinition, "score_field"),
            DefaultScore = TryGetDouble(indexDefinition, "default_score"),
        };
    }

    private static Dictionary<GlideString, object> ToFlatMap(object value)
    {
        if (value is Dictionary<GlideString, object> alreadyMap)
        {
            return alreadyMap;
        }

        if (value is not object[] flat || flat.Length % 2 != 0)
        {
            throw new RequestException(
                $"Unexpected FT.INFO nested structure: expected a map or an even-length flat array, got {value?.GetType()}" +
                    (value is object[] odd ? $" (length {odd.Length})" : string.Empty));
        }

        Dictionary<GlideString, object> result = [];
        for (int i = 0; i < flat.Length; i += 2)
        {
            result[(GlideString)flat[i]] = flat[i + 1];
        }

        return result;
    }

    private static Ft.InfoState ParseInfoState(string state) => state switch
    {
        "ready" => Ft.InfoState.Ready,
        "backfill_in_progress" => Ft.InfoState.BackfillInProgress,
        "backfill_paused_by_oom" => Ft.InfoState.BackfillPausedByOom,
        _ => throw new RequestException($"Unknown FT.INFO state: '{state}'"),
    };

    private static Ft.InfoField[] ParseInfoFields(Dictionary<GlideString, object> map)
    {
        if (!map.TryGetValue("attributes", out var attrsObj) || attrsObj is not object[] attrs)
        {
            return [];
        }

        var fields = new List<Ft.InfoField>();
        foreach (var attr in attrs)
        {
            var fieldMap = ToFlatMap(attr);
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
        ValkeyValue identifier,
        ValkeyValue attribute,
        long userIndexedMemory,
        Dictionary<GlideString, object> fieldMap)
    {
        var indexMap = fieldMap.TryGetValue("index", out var indexObj)
            ? ToFlatMap(indexObj)
            : [];

        var algoMap = indexMap.TryGetValue("algorithm", out var algoObj)
            ? ToFlatMap(algoObj)
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

    #endregion
}
