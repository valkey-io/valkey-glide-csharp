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
}
