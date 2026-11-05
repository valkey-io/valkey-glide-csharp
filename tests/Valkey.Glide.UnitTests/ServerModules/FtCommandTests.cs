// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.ServerModules;

using static Valkey.Glide.Internals.FFI;

namespace Valkey.Glide.UnitTests.ServerModules;

public class FtCommandTests
{
    [Fact]
    public void FtCreate()
    {
        CmdAssert.Equal(
            RequestType.FtCreate,
            ["idx", "SCHEMA", "title", "TEXT"],
            Request.FtCreate("idx", [new Ft.CreateTextField("title")]));

        CmdAssert.Equal(
            RequestType.FtCreate,
            ["idx", "ON", "HASH", "SCHEMA", "title", "TEXT"],
            Request.FtCreate("idx", [new Ft.CreateTextField("title")], new Ft.CreateOptions()));

        CmdAssert.Equal(
            RequestType.FtCreate,
            ["idx", "ON", "JSON", "SCHEMA", "f", "TEXT"],
            Request.FtCreate("idx", [new Ft.CreateTextField("f")], new Ft.CreateOptions { DataType = Ft.DataType.Json }));

        CmdAssert.Equal(
            RequestType.FtCreate,
            ["idx", "ON", "HASH", "PREFIX", "2", "a:", "b:", "SCHEMA", "f", "TEXT"],
            Request.FtCreate("idx", [new Ft.CreateTextField("f")], new Ft.CreateOptions { Prefixes = ["a:", "b:"] }));

        CmdAssert.Equal(
            RequestType.FtCreate,
            ["idx", "ON", "HASH", "SKIPINITIALSCAN", "SCHEMA", "f", "TEXT"],
            Request.FtCreate("idx", [new Ft.CreateTextField("f")], new Ft.CreateOptions { SkipInitialScan = true }));

        CmdAssert.Equal(
            RequestType.FtCreate,
            ["idx", "ON", "HASH", "MINSTEMSIZE", "3", "SCHEMA", "f", "TEXT"],
            Request.FtCreate("idx", [new Ft.CreateTextField("f")], new Ft.CreateOptions { MinStemSize = 3 }));

        CmdAssert.Equal(
            RequestType.FtCreate,
            ["idx", "ON", "HASH", "NOOFFSETS", "SCHEMA", "f", "TEXT"],
            Request.FtCreate("idx", [new Ft.CreateTextField("f")], new Ft.CreateOptions { NoOffsets = true }));

        CmdAssert.Equal(
            RequestType.FtCreate,
            ["idx", "ON", "HASH", "STOPWORDS", "2", "the", "a", "SCHEMA", "f", "TEXT"],
            Request.FtCreate("idx", [new Ft.CreateTextField("f")], new Ft.CreateOptions { StopWords = ["the", "a"] }));

        CmdAssert.Equal(
            RequestType.FtCreate,
            ["idx", "ON", "HASH", "STOPWORDS", "0", "SCHEMA", "f", "TEXT"],
            Request.FtCreate("idx", [new Ft.CreateTextField("f")], new Ft.CreateOptions { StopWords = Ft.CreateOptions.NoStopWords }));

        CmdAssert.Equal(
            RequestType.FtCreate,
            ["idx", "ON", "HASH", "PUNCTUATION", ".,", "SCHEMA", "f", "TEXT"],
            Request.FtCreate("idx", [new Ft.CreateTextField("f")], new Ft.CreateOptions { Punctuation = ".," }));

        CmdAssert.Equal(
            RequestType.FtCreate,
            ["idx", "SCHEMA", "$.f", "AS", "f", "TEXT", "NOSTEM", "NOSUFFIXTRIE"],
            Request.FtCreate("idx", [new Ft.CreateTextField("$.f", "f") { NoStem = true, WithSuffixTrie = false }]));

        CmdAssert.Equal(
            RequestType.FtCreate,
            ["idx", "SCHEMA", "f", "TAG", "SEPARATOR", "|", "CASESENSITIVE"],
            Request.FtCreate("idx", [new Ft.CreateTagField("f") { Separator = '|', CaseSensitive = true }]));

        CmdAssert.Equal(
            RequestType.FtCreate,
            ["idx", "SCHEMA", "$.f", "AS", "f", "NUMERIC"],
            Request.FtCreate("idx", [new Ft.CreateNumericField("$.f", "f")]));

        CmdAssert.Equal(
            RequestType.FtCreate,
            ["idx", "SCHEMA", "v", "VECTOR", "FLAT", "6", "DIM", "128", "DISTANCE_METRIC", "COSINE", "TYPE", "FLOAT32"],
            Request.FtCreate("idx", [new Ft.CreateVectorFieldFlat { Identifier = "v", Dimensions = 128, DistanceMetric = Ft.DistanceMetric.Cosine }]));

        CmdAssert.Equal(
            RequestType.FtCreate,
            ["idx", "SCHEMA", "v", "VECTOR", "FLAT", "8", "DIM", "64", "DISTANCE_METRIC", "L2", "TYPE", "FLOAT32", "INITIAL_CAP", "1000"],
            Request.FtCreate("idx", [new Ft.CreateVectorFieldFlat { Identifier = "v", Dimensions = 64, DistanceMetric = Ft.DistanceMetric.Euclidean, InitialCap = 1000 }]));

        CmdAssert.Equal(
            RequestType.FtCreate,
            ["idx", "SCHEMA", "v", "VECTOR", "HNSW", "12", "DIM", "256", "DISTANCE_METRIC", "IP", "TYPE", "FLOAT32", "M", "16", "EF_CONSTRUCTION", "200", "EF_RUNTIME", "10"],
            Request.FtCreate("idx", [new Ft.CreateVectorFieldHnsw { Identifier = "v", Dimensions = 256, DistanceMetric = Ft.DistanceMetric.InnerProduct, NumberOfEdges = 16, VectorsExaminedOnConstruction = 200, VectorsExaminedOnRuntime = 10 }]));

        CmdAssert.Equal(
            RequestType.FtCreate,
            ["idx", "ON", "HASH", "PREFIX", "1", "p:", "SCHEMA", "t", "TEXT", "c", "TAG", "n", "NUMERIC", "v", "VECTOR", "FLAT", "6", "DIM", "3", "DISTANCE_METRIC", "COSINE", "TYPE", "FLOAT32"],
            Request.FtCreate("idx", [new Ft.CreateTextField("t"), new Ft.CreateTagField("c"), new Ft.CreateNumericField("n"), new Ft.CreateVectorFieldFlat { Identifier = "v", Dimensions = 3, DistanceMetric = Ft.DistanceMetric.Cosine }], new Ft.CreateOptions { Prefixes = ["p:"] }));
    }

    [Fact]
    public void FtDropIndex()
        => CmdAssert.Equal(RequestType.FtDropIndex, ["idx"], Request.FtDropIndex("idx"));

    [Fact]
    public void FtList()
        => CmdAssert.Equal(RequestType.FtList, [], Request.FtList());

    [Fact]
    public void FtSearch()
    {
        CmdAssert.Equal(
            RequestType.FtSearch,
            ["idx", "*"],
            Request.FtSearch("idx", "*"));

        CmdAssert.Equal(
            RequestType.FtSearch,
            ["idx", "*"],
            Request.FtSearch("idx", "*", new Ft.SearchOptions()));

        CmdAssert.Equal(
            RequestType.FtSearch,
            ["idx", "*", "NOCONTENT"],
            Request.FtSearch("idx", "*", new Ft.SearchOptions { Return = Ft.SearchOptions.NoContent }));

        CmdAssert.Equal(
            RequestType.FtSearch,
            ["idx", "*", "WITHSORTKEYS", "SORTBY", "f", "ASC"],
            Request.FtSearch("idx", "*", new Ft.SearchOptions { SortBy = new Ft.SearchSortBy { Field = "f", Order = SortOrder.Ascending, WithSortKeys = true } }));

        _ = Assert.Throws<ArgumentException>(() =>
            Request.FtSearch("idx", "*", new Ft.SearchOptions { Return = Ft.SearchOptions.NoContent, SortBy = new Ft.SearchSortBy { Field = "f", WithSortKeys = true } }));

        CmdAssert.Equal(
            RequestType.FtSearch,
            ["idx", "*", "VERBATIM", "INCONSISTENT", "SOMESHARDS", "INORDER"],
            Request.FtSearch("idx", "*", new Ft.SearchOptions { Verbatim = true, Inconsistent = true, SomeShards = true, InOrder = true }));

        CmdAssert.Equal(
            RequestType.FtSearch,
            ["idx", "*", "SLOP", "2"],
            Request.FtSearch("idx", "*", new Ft.SearchOptions { Slop = 2 }));

        CmdAssert.Equal(
            RequestType.FtSearch,
            ["idx", "*", "LIMIT", "10", "20"],
            Request.FtSearch("idx", "*", new Ft.SearchOptions { Limit = new Ft.SearchLimit { Offset = 10, Count = 20 } }));

        CmdAssert.Equal(
            RequestType.FtSearch,
            ["idx", "*", "SORTBY", "f", "ASC"],
            Request.FtSearch("idx", "*", new Ft.SearchOptions { SortBy = new Ft.SearchSortBy { Field = "f", Order = SortOrder.Ascending } }));

        CmdAssert.Equal(
            RequestType.FtSearch,
            ["idx", "*", "SORTBY", "f", "DESC"],
            Request.FtSearch("idx", "*", new Ft.SearchOptions { SortBy = new Ft.SearchSortBy { Field = "f", Order = SortOrder.Descending } }));

        CmdAssert.Equal(
            RequestType.FtSearch,
            ["idx", "*", "SORTBY", "f"],
            Request.FtSearch("idx", "*", new Ft.SearchOptions { SortBy = new Ft.SearchSortBy { Field = "f" } }));

        CmdAssert.Equal(
            RequestType.FtSearch,
            ["idx", "*", "RETURN", "4", "a", "$.b", "AS", "b"],
            Request.FtSearch("idx", "*", new Ft.SearchOptions { Return = ["a", new Ft.SearchReturnField { Field = "$.b", Name = "b" }] }));

        CmdAssert.Equal(
            RequestType.FtSearch,
            ["idx", "@f:$t", "PARAMS", "2", "t", "v"],
            Request.FtSearch("idx", "@f:$t", new Ft.SearchOptions { Params = new Dictionary<ValkeyValue, ValkeyValue> { ["t"] = "v" } }));

        CmdAssert.Equal(
            RequestType.FtSearch,
            ["idx", "*", "TIMEOUT", "500"],
            Request.FtSearch("idx", "*", new Ft.SearchOptions { Timeout = TimeSpan.FromMilliseconds(500) }));
    }

    [Fact]
    public void FtAggregate()
    {
        CmdAssert.Equal(
            RequestType.FtAggregate,
            ["idx", "*"],
            Request.FtAggregate("idx", "*"));

        CmdAssert.Equal(
            RequestType.FtAggregate,
            ["idx", "*"],
            Request.FtAggregate("idx", "*", new Ft.AggregateOptions()));

        CmdAssert.Equal(
            RequestType.FtAggregate,
            ["idx", "*", "VERBATIM", "INORDER", "SLOP", "3"],
            Request.FtAggregate("idx", "*", new Ft.AggregateOptions { Verbatim = true, InOrder = true, Slop = 3 }));

        CmdAssert.Equal(
            RequestType.FtAggregate,
            ["idx", "*", "LOAD", "*"],
            Request.FtAggregate("idx", "*", new Ft.AggregateOptions { LoadFields = null }));

        CmdAssert.Equal(
            RequestType.FtAggregate,
            ["idx", "*", "LOAD", "2", "@a", "@b"],
            Request.FtAggregate("idx", "*", new Ft.AggregateOptions { LoadFields = ["@a", "@b"] }));

        CmdAssert.Equal(
            RequestType.FtAggregate,
            ["idx", "*"],
            Request.FtAggregate("idx", "*", new Ft.AggregateOptions { LoadFields = Ft.AggregateOptions.LoadNone }));

        CmdAssert.Equal(
            RequestType.FtAggregate,
            ["idx", "@f:$t", "PARAMS", "2", "t", "v"],
            Request.FtAggregate("idx", "@f:$t", new Ft.AggregateOptions { Params = new Dictionary<ValkeyValue, ValkeyValue> { ["t"] = "v" } }));

        CmdAssert.Equal(
            RequestType.FtAggregate,
            ["idx", "*", "TIMEOUT", "1000"],
            Request.FtAggregate("idx", "*", new Ft.AggregateOptions { Timeout = TimeSpan.FromSeconds(1) }));

        CmdAssert.Equal(
            RequestType.FtAggregate,
            ["idx", "*", "GROUPBY", "1", "@c", "REDUCE", "COUNT", "0", "AS", "n"],
            Request.FtAggregate("idx", "*", new Ft.AggregateOptions { Clauses = [new Ft.AggregateGroupBy { Fields = ["@c"], Reducers = [new Ft.AggregateReducer { Function = Ft.ReducerFunction.Count, Name = "n" }] }] }));

        CmdAssert.Equal(
            RequestType.FtAggregate,
            ["idx", "*", "SORTBY", "2", "@s", "ASC", "MAX", "10"],
            Request.FtAggregate("idx", "*", new Ft.AggregateOptions { Clauses = [new Ft.AggregateSortBy { Expressions = [new Ft.AggregateSortExpression { Expression = "@s", Order = SortOrder.Ascending }], Max = 10 }] }));

        CmdAssert.Equal(
            RequestType.FtAggregate,
            ["idx", "*", "FILTER", "@p > 0"],
            Request.FtAggregate("idx", "*", new Ft.AggregateOptions { Clauses = [new Ft.AggregateFilter("@p > 0")] }));

        CmdAssert.Equal(
            RequestType.FtAggregate,
            ["idx", "*", "APPLY", "upper(@n)", "AS", "u"],
            Request.FtAggregate("idx", "*", new Ft.AggregateOptions { Clauses = [new Ft.AggregateApply { Expression = "upper(@n)", Name = "u" }] }));

        CmdAssert.Equal(
            RequestType.FtAggregate,
            ["idx", "*", "LIMIT", "5", "10"],
            Request.FtAggregate("idx", "*", new Ft.AggregateOptions { Clauses = [new Ft.AggregateLimit { Offset = 5, Count = 10 }] }));

        CmdAssert.Equal(
            RequestType.FtAggregate,
            ["idx", "*", "GROUPBY", "1", "@c", "SORTBY", "1", "@n", "LIMIT", "0", "5"],
            Request.FtAggregate("idx", "*", new Ft.AggregateOptions { Clauses = [new Ft.AggregateGroupBy { Fields = ["@c"] }, new Ft.AggregateSortBy { Expressions = ["@n"] }, new Ft.AggregateLimit { Count = 5 }] }));
    }

    [Fact]
    public void FtInfoLocal()
    {
        CmdAssert.Equal(
            RequestType.FtInfo,
            ["idx", "LOCAL"],
            Request.FtInfoLocal("idx"));

        CmdAssert.Equal(
            RequestType.FtInfo,
            ["idx", "LOCAL", "SOMESHARDS"],
            Request.FtInfoLocal("idx", new Ft.InfoOptions { SomeShards = true }));

        CmdAssert.Equal(
            RequestType.FtInfo,
            ["idx", "LOCAL", "INCONSISTENT"],
            Request.FtInfoLocal("idx", new Ft.InfoOptions { Inconsistent = true }));

        CmdAssert.Equal(
            RequestType.FtInfo,
            ["idx", "LOCAL", "SOMESHARDS", "INCONSISTENT"],
            Request.FtInfoLocal("idx", new Ft.InfoOptions { SomeShards = true, Inconsistent = true }));
    }

    [Fact]
    public void FtCreateParity13()
    {
        // FLOAT16 / BFLOAT16 vector types
        CmdAssert.Equal(
            RequestType.FtCreate,
            ["idx", "SCHEMA", "v", "VECTOR", "FLAT", "6", "DIM", "4", "DISTANCE_METRIC", "L2", "TYPE", "FLOAT16"],
            Request.FtCreate("idx", [new Ft.CreateVectorFieldFlat { Identifier = "v", Dimensions = 4, DistanceMetric = Ft.DistanceMetric.Euclidean, Type = Ft.VectorType.Float16 }]));

        CmdAssert.Equal(
            RequestType.FtCreate,
            ["idx", "SCHEMA", "v", "VECTOR", "HNSW", "6", "DIM", "4", "DISTANCE_METRIC", "L2", "TYPE", "BFLOAT16"],
            Request.FtCreate("idx", [new Ft.CreateVectorFieldHnsw { Identifier = "v", Dimensions = 4, DistanceMetric = Ft.DistanceMetric.Euclidean, Type = Ft.VectorType.Bfloat16 }]));

        // SORTABLE / UNF, only directly after SORTABLE
        CmdAssert.Equal(
            RequestType.FtCreate,
            ["idx", "SCHEMA", "f", "TEXT", "SORTABLE", "UNF"],
            Request.FtCreate("idx", [new Ft.CreateTextField("f") { Sortable = true, Unf = true }]));

        CmdAssert.Equal(
            RequestType.FtCreate,
            ["idx", "SCHEMA", "f", "TAG", "SORTABLE"],
            Request.FtCreate("idx", [new Ft.CreateTagField("f") { Sortable = true }]));

        CmdAssert.Equal(
            RequestType.FtCreate,
            ["idx", "SCHEMA", "f", "NUMERIC", "SORTABLE", "UNF"],
            Request.FtCreate("idx", [new Ft.CreateNumericField("f") { Sortable = true, Unf = true }]));

        // L1: Unf without Sortable raises, matching Python's ValueError("unf requires
        // sortable=True.") -- previously silently dropped UNF (and SORTABLE) instead.
        _ = Assert.Throws<ArgumentException>(() =>
            Request.FtCreate("idx", [new Ft.CreateTextField("f") { Unf = true }]));

        // WEIGHT on TEXT
        CmdAssert.Equal(
            RequestType.FtCreate,
            ["idx", "SCHEMA", "f", "TEXT", "WEIGHT", "2.5"],
            Request.FtCreate("idx", [new Ft.CreateTextField("f") { Weight = 2.5 }]));

        // FT.CREATE-level: SCORE, LANGUAGE, WITHOFFSETS, FILTER, SCORE_FIELD, NOHL
        CmdAssert.Equal(
            RequestType.FtCreate,
            ["idx", "ON", "HASH", "SCORE", "0.5", "LANGUAGE", "english", "WITHOFFSETS", "FILTER", "@active:{true}", "SCORE_FIELD", "popularity", "NOHL", "SCHEMA", "f", "TEXT"],
            Request.FtCreate(
                "idx",
                [new Ft.CreateTextField("f")],
                new Ft.CreateOptions
                {
                    Score = 0.5,
                    Language = "english",
                    WithOffsets = true,
                    Filter = "@active:{true}",
                    ScoreField = "popularity",
                    NoHighlight = true,
                }));

        _ = Assert.Throws<ArgumentException>(() =>
            Request.FtCreate("idx", [new Ft.CreateTextField("f")], new Ft.CreateOptions { WithOffsets = true, NoOffsets = true }));

        _ = Assert.Throws<ArgumentException>(() =>
            Request.FtCreate("idx", [new Ft.CreateTextField("f")], new Ft.CreateOptions { Filter = string.Empty }));
    }

    [Fact]
    public void FtSearchScoring13()
    {
        CmdAssert.Equal(
            RequestType.FtSearch,
            ["idx", "*", "WITHSCORES"],
            Request.FtSearch("idx", "*", new Ft.SearchOptions { WithScores = true }));

        CmdAssert.Equal(
            RequestType.FtSearch,
            ["idx", "*", "SCORER", "BM25STD"],
            Request.FtSearch("idx", "*", new Ft.SearchOptions { Scorer = "BM25STD" }));

        CmdAssert.Equal(
            RequestType.FtSearch,
            ["idx", "*", "INKEYS", "2", "k1", "k2"],
            Request.FtSearch("idx", "*", new Ft.SearchOptions { InKeys = ["k1", "k2"] }));

        CmdAssert.Equal(
            RequestType.FtSearch,
            ["idx", "*", "INKEYS", "0"],
            Request.FtSearch("idx", "*", new Ft.SearchOptions { InKeys = [] }));

        CmdAssert.Equal(
            RequestType.FtSearch,
            ["idx", "*", "LIMIT", "0", "0"],
            Request.FtSearch("idx", "*", new Ft.SearchOptions { Count = true }));

        _ = Assert.Throws<ArgumentException>(() =>
            Request.FtSearch("idx", "*", new Ft.SearchOptions { Count = true, Limit = new Ft.SearchLimit { Count = 1 } }));
    }

    [Fact]
    public void FtAggregateScoringAndCursor13()
    {
        CmdAssert.Equal(
            RequestType.FtAggregate,
            ["idx", "*", "ADDSCORES"],
            Request.FtAggregate("idx", "*", new Ft.AggregateOptions { AddScores = true }));

        CmdAssert.Equal(
            RequestType.FtAggregate,
            ["idx", "*", "SCORER", "BM25STD"],
            Request.FtAggregate("idx", "*", new Ft.AggregateOptions { Scorer = "BM25STD" }));

        CmdAssert.Equal(
            RequestType.FtAggregate,
            ["idx", "*", "DIALECT", "2"],
            Request.FtAggregate("idx", "*", new Ft.AggregateOptions { Dialect = 2 }));

        CmdAssert.Equal(
            RequestType.FtAggregate,
            ["idx", "*", "LOAD", "3", "f", "AS", "alias"],
            Request.FtAggregate("idx", "*", new Ft.AggregateOptions { LoadFieldEntries = [new Ft.LoadField("f", "alias")] }));

        CmdAssert.Equal(
            RequestType.FtAggregate,
            ["idx", "*", "WITHCURSOR", "COUNT", "10", "MAXIDLE", "1000"],
            Request.FtAggregateWithCursor("idx", "*", new Ft.AggregateOptions { WithCursorOptions = new Ft.WithCursor { Count = 10, MaxIdle = 1000 } }));

        // TOLIST / FIRST_VALUE / RANDOM_SAMPLE reducers
        CmdAssert.Equal(
            RequestType.FtAggregate,
            ["idx", "*", "GROUPBY", "1", "@c", "REDUCE", "TOLIST", "1", "@color", "AS", "colors"],
            Request.FtAggregate("idx", "*", new Ft.AggregateOptions { Clauses = [new Ft.AggregateGroupBy { Fields = ["@c"], Reducers = [Ft.AggregateReducer.ToList("@color", "colors")] }] }));

        CmdAssert.Equal(
            RequestType.FtAggregate,
            ["idx", "*", "GROUPBY", "1", "@c", "REDUCE", "FIRST_VALUE", "4", "@title", "BY", "@price", "DESC", "AS", "most_expensive"],
            Request.FtAggregate("idx", "*", new Ft.AggregateOptions { Clauses = [new Ft.AggregateGroupBy { Fields = ["@c"], Reducers = [Ft.AggregateReducer.FirstValue("@title", by: "@price", order: SortOrder.Descending, name: "most_expensive")] }] }));

        CmdAssert.Equal(
            RequestType.FtAggregate,
            ["idx", "*", "GROUPBY", "1", "@c", "REDUCE", "RANDOM_SAMPLE", "2", "@score", "5", "AS", "sample"],
            Request.FtAggregate("idx", "*", new Ft.AggregateOptions { Clauses = [new Ft.AggregateGroupBy { Fields = ["@c"], Reducers = [Ft.AggregateReducer.RandomSample("@score", 5, "sample")] }] }));

        _ = Assert.Throws<ArgumentException>(() => Ft.AggregateReducer.FirstValue("@title", order: SortOrder.Descending));
    }

    [Fact]
    public void FtCursor()
    {
        CmdAssert.Equal(
            RequestType.CustomCommand,
            ["FT.CURSOR", "READ", "idx", "42"],
            Request.FtCursorRead("idx", 42, null));

        CmdAssert.Equal(
            RequestType.CustomCommand,
            ["FT.CURSOR", "READ", "idx", "42", "COUNT", "5"],
            Request.FtCursorRead("idx", 42, 5));

        CmdAssert.Equal(
            RequestType.CustomCommand,
            ["FT.CURSOR", "DEL", "idx", "42"],
            Request.FtCursorDel("idx", 42));
    }

    [Fact]
    public void FtHybridArgvParity()
    {
        // Case 1: minimal SEARCH + VSIM (raw vector bytes), no options -- mirrors Python's first
        // doc example. The vector is given as a string here (not starting with '$') purely so the
        // generated-param-name path is exercised deterministically in a unit test; the generated
        // name itself is a random UUID, so this case asserts everything except that one token.
        var args1 = Request.FtHybrid(
            "idx",
            new Ft.HybridSearchArm { Query = "hello world" },
            new Ft.HybridVsimArm { Field = "vec", Vector = "rawvectorbytes", K = 10 })
            .Args
            .ToStrings();

        Assert.Equal("FT.HYBRID", args1[0]);
        Assert.Equal("idx", args1[1]);
        Assert.Equal("SEARCH", args1[2]);
        Assert.Equal("hello world", args1[3]);
        Assert.Equal("VSIM", args1[4]);
        Assert.Equal("@vec", args1[5]);
        Assert.StartsWith("$", args1[6]);
        Assert.DoesNotContain("-", args1[6]);
        Assert.Equal("KNN", args1[7]);
        Assert.Equal("2", args1[8]);
        Assert.Equal("K", args1[9]);
        Assert.Equal("10", args1[10]);
        Assert.Equal("PARAMS", args1[11]);
        Assert.Equal("2", args1[12]);
        Assert.Equal(args1[6][1..], args1[13]);
        Assert.Equal("rawvectorbytes", args1[14]);
        Assert.Equal(15, args1.Length);

        // Case 2: explicit $name VSIM reference, RRF combine, LOAD fields, WITHCURSOR -- mirrors
        // Python's second doc example exactly (same scorer/yield_score_as/combine/load/cursor shape).
        var args2 = Request.FtHybridWithCursor(
            "idx",
            new Ft.HybridSearchArm { Query = "hello world", YieldScoreAs = "ss" },
            new Ft.HybridVsimArm { Field = "vec", Vector = "$qv", K = 10, YieldScoreAs = "vs" },
            new Ft.HybridOptions
            {
                Combine = new Ft.HybridRrf { Constant = 60 },
                LoadFields = [new Ft.LoadField("title")],
                WithCursorOptions = new Ft.WithCursor { Count = 10 },
                Params = new Dictionary<ValkeyValue, ValkeyValue> { ["qv"] = "rawbytes" },
            })
            .Args
            .ToStrings();

        Assert.Equal(
            [
                "FT.HYBRID", "idx",
                "SEARCH", "hello world", "YIELD_SCORE_AS", "ss",
                "VSIM", "@vec", "$qv", "KNN", "2", "K", "10", "YIELD_SCORE_AS", "vs",
                "COMBINE", "RRF", "2", "CONSTANT", "60",
            ],
            args2[..20]);

        // Order-sensitive suffix after COMBINE: LOAD, PARAMS, WITHCURSOR (COMBINE's own WINDOW arg
        // omitted above deliberately isn't part of this call -- recheck full array instead).
        Assert.Equal("FT.HYBRID", args2[0]);
        Assert.Contains("LOAD", args2);
        Assert.Contains("PARAMS", args2);
        Assert.Contains("WITHCURSOR", args2);
        int loadIdx = Array.IndexOf(args2, "LOAD");
        int paramsIdx = Array.IndexOf(args2, "PARAMS");
        int cursorIdx = Array.IndexOf(args2, "WITHCURSOR");
        Assert.True(loadIdx < paramsIdx);
        Assert.True(paramsIdx < cursorIdx);

        // Case 3: LOCALONLY + a FILTER clause placed after the VSIM arm with no COMBINE and no
        // LOAD in between -- the regression test for the VSIM-parser FILTER-claiming bug.
        // PARAMS must still appear before the clause (LOCALONLY before
        // PARAMS, the aggregate FILTER clause after PARAMS), and the FILTER clause args here are
        // the literal AggregateFilter tokens, not folded into the VSIM arm's own filter.
        var args3 = Request.FtHybrid(
            "idx",
            new Ft.HybridSearchArm { Query = "hello" },
            new Ft.HybridVsimArm { Field = "vec", Vector = "$qv" },
            new Ft.HybridOptions
            {
                LocalOnly = true,
                Dialect = 2,
                Params = new Dictionary<ValkeyValue, ValkeyValue> { ["qv"] = "unused" },
                Clauses = [new Ft.AggregateFilter("@price > 10")],
            })
            .Args
            .ToStrings();

        Assert.Equal(
            [
                "FT.HYBRID", "idx",
                "SEARCH", "hello",
                "VSIM", "@vec", "$qv",
                "LOCALONLY",
                "PARAMS", "2", "qv", "unused",
                "FILTER", "@price > 10",
                "DIALECT", "2",
            ],
            args3);
    }

    [Fact]
    public void FtHybridVsimFilterAndCombineVariants()
    {
        Assert.Equal(
            ["FILTER", "@tag:{a}"],
            new Ft.HybridVsimFilter("@tag:{a}").ToArgs());

        Assert.Equal(
            ["FILTER", "3", "@tag:{a}", "POLICY", "ADHOC"],
            new Ft.HybridVsimFilter("@tag:{a}") { Policy = Ft.HybridFilterPolicy.Adhoc }.ToArgs());

        Assert.Equal(
            ["FILTER", "5", "@tag:{a}", "POLICY", "BATCHES", "BATCH_SIZE", "100"],
            new Ft.HybridVsimFilter("@tag:{a}") { Policy = Ft.HybridFilterPolicy.Batches, BatchSize = 100 }.ToArgs());

        _ = Assert.Throws<ArgumentException>(() =>
            new Ft.HybridVsimFilter("@tag:{a}") { BatchSize = 100 }.ToArgs());

        Assert.Equal(
            ["COMBINE", "LINEAR", "4", "ALPHA", "0.3", "BETA", "0.7"],
            new Ft.HybridLinear { Alpha = 0.3, Beta = 0.7 }.ToArgs());

        _ = Assert.Throws<ArgumentException>(() => new Ft.HybridLinear { Alpha = 0.3 }.ToArgs());

        Assert.Equal(
            ["COMBINE", "FUNCTION", "2", "EXPR", "@ss + @vs"],
            new Ft.HybridFunction { Expr = "@ss + @vs" }.ToArgs());

        _ = Assert.Throws<ArgumentException>(() => new Ft.HybridRrf { Constant = -1 }.ToArgs());
    }

    [Fact]
    public void M3_ClientSideGuards()
    {
        // HybridSearchArm.Query must not be a vector query.
        _ = Assert.Throws<ArgumentException>(() =>
            new Ft.HybridSearchArm { Query = "*=>[KNN 10 @vec $p]" }.ToArgs());

        // HybridSearchArm/SearchOptions/AggregateOptions.Scorer: only BM25STD is registered.
        _ = Assert.Throws<ArgumentException>(() =>
            new Ft.HybridSearchArm { Query = "*", Scorer = "TFIDF" }.ToArgs());
        _ = Assert.Throws<ArgumentException>(() =>
            Request.FtSearch("idx", "*", new Ft.SearchOptions { Scorer = "TFIDF" }));
        _ = Assert.Throws<ArgumentException>(() =>
            Request.FtAggregate("idx", "*", new Ft.AggregateOptions { Scorer = "TFIDF" }));

        // Valid scorer passes through unchanged.
        Assert.Equal(
            ["SEARCH", "*", "SCORER", "BM25STD"],
            new Ft.HybridSearchArm { Query = "*", Scorer = "BM25STD" }.ToArgs());

        // HybridOptions.Dialect: FT.HYBRID is fixed at dialect 2.
        _ = Assert.Throws<ArgumentException>(() => Request.FtHybrid(
            "idx",
            new Ft.HybridSearchArm { Query = "*" },
            new Ft.HybridVsimArm { Field = "vec", Vector = "rawbytes", K = 10 },
            new Ft.HybridOptions { Dialect = 3 }).Args);

        // HybridOptions.LoadFields must not name either arm's YieldScoreAs alias.
        _ = Assert.Throws<ArgumentException>(() => Request.FtHybrid(
            "idx",
            new Ft.HybridSearchArm { Query = "*", YieldScoreAs = "ss" },
            new Ft.HybridVsimArm { Field = "vec", Vector = "rawbytes", K = 10 },
            new Ft.HybridOptions { LoadFields = ["ss"] }).Args);

        // AggregateReducer.RandomSample size must be 0..1000.
        _ = Assert.Throws<ArgumentOutOfRangeException>(() => Ft.AggregateReducer.RandomSample("@f", -1));
        _ = Assert.Throws<ArgumentOutOfRangeException>(() => Ft.AggregateReducer.RandomSample("@f", 1001));
        Assert.NotNull(Ft.AggregateReducer.RandomSample("@f", 1000));
        Assert.NotNull(Ft.AggregateReducer.RandomSample("@f", 0));

        // CreateOptions.Language: only the server's 12 registered stemmer languages.
        _ = Assert.Throws<ArgumentException>(() =>
            Request.FtCreate("idx", [new Ft.CreateTextField("f")], new Ft.CreateOptions { Language = "klingon" }));
        CmdAssert.Equal(
            RequestType.FtCreate,
            ["idx", "ON", "HASH", "LANGUAGE", "english", "SCHEMA", "f", "TEXT"],
            Request.FtCreate("idx", [new Ft.CreateTextField("f")], new Ft.CreateOptions { Language = "english" }));
    }

    [Fact]
    public void L3_RawVectorBytesStartingWithDollarSignAreNeverTreatedAsAParamReference()
    {
        // L3: a raw vector's packed binary representation can legitimately start with the byte
        // 0x24 ('$') by coincidence -- IsParamReference must check the ValkeyValue's storage type
        // (String vs Raw/byte[]), never the first byte, or such a vector would be misinterpreted
        // as a "$name" parameter reference and sent completely wrong on the wire.
        byte[] vectorStartingWithDollarByte = [0x24, 0x00, 0x00, 0x00];
        var vsimArm = new Ft.HybridVsimArm { Field = "vec", Vector = vectorStartingWithDollarByte, K = 10 };
        Assert.False(vsimArm.IsParamReference);

        var args = Request.FtHybrid(
            "idx",
            new Ft.HybridSearchArm { Query = "*" },
            vsimArm).Args;

        // The generated-$name-PARAMS path was taken (raw bytes, not a reference): PARAMS carries
        // the exact raw bytes value, byte-for-byte, not a literal "$..." string mistaken for one.
        GlideString paramsLiteral = "PARAMS";
        int paramsIdx = Array.IndexOf(args, paramsLiteral);
        Assert.True(paramsIdx >= 0);
        GlideString paramValue = args[paramsIdx + 3];
        Assert.Equal((GlideString)vectorStartingWithDollarByte, paramValue);
    }

    [Fact]
    public void L2_ReducerFunction_ReportsFactoryProducedValue()
    {
        // L2: ToList/FirstValue/RandomSample must report their own ReducerFunction value, not
        // the enum's default (Count) -- previously these set Function = default and emitted the
        // wire literal via a separate, non-public RawFunction field, so a caller inspecting
        // reducer.Function on a TOLIST reducer incorrectly saw Count.
        Assert.Equal(Ft.ReducerFunction.ToList, Ft.AggregateReducer.ToList("@f").Function);
        Assert.Equal(Ft.ReducerFunction.FirstValue, Ft.AggregateReducer.FirstValue("@f").Function);
        Assert.Equal(Ft.ReducerFunction.RandomSample, Ft.AggregateReducer.RandomSample("@f", 10).Function);
    }
}
