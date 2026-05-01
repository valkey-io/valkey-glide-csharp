// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.ServerModules;

namespace Valkey.Glide.UnitTests.SearchModules;

public class FtCommandTests
{
    [Fact]
    public void FtCreate()
    {
        Assert.Equal(
            ["FT.CREATE", "idx", "SCHEMA", "title", "TEXT"],
            Request.FtCreate("idx", [new Ft.TextField("title")]).GetArgs());

        Assert.Equal(
            ["FT.CREATE", "idx", "ON", "HASH", "SCHEMA", "title", "TEXT"],
            Request.FtCreate("idx", [new Ft.TextField("title")], new Ft.CreateOptions()).GetArgs());

        Assert.Equal(
            ["FT.CREATE", "idx", "ON", "JSON", "SCHEMA", "f", "TEXT"],
            Request.FtCreate("idx", [new Ft.TextField("f")], new Ft.CreateOptions { DataType = Ft.DataType.Json }).GetArgs());

        Assert.Equal(
            ["FT.CREATE", "idx", "ON", "HASH", "PREFIX", "2", "a:", "b:", "SCHEMA", "f", "TEXT"],
            Request.FtCreate("idx", [new Ft.TextField("f")], new Ft.CreateOptions { Prefixes = ["a:", "b:"] }).GetArgs());

        Assert.Equal(
            ["FT.CREATE", "idx", "ON", "HASH", "SKIPINITIALSCAN", "SCHEMA", "f", "TEXT"],
            Request.FtCreate("idx", [new Ft.TextField("f")], new Ft.CreateOptions { SkipInitialScan = true }).GetArgs());

        Assert.Equal(
            ["FT.CREATE", "idx", "ON", "HASH", "MINSTEMSIZE", "3", "SCHEMA", "f", "TEXT"],
            Request.FtCreate("idx", [new Ft.TextField("f")], new Ft.CreateOptions { MinStemSize = 3 }).GetArgs());

        Assert.Equal(
            ["FT.CREATE", "idx", "ON", "HASH", "NOOFFSETS", "SCHEMA", "f", "TEXT"],
            Request.FtCreate("idx", [new Ft.TextField("f")], new Ft.CreateOptions { NoOffsets = true }).GetArgs());

        Assert.Equal(
            ["FT.CREATE", "idx", "ON", "HASH", "STOPWORDS", "2", "the", "a", "SCHEMA", "f", "TEXT"],
            Request.FtCreate("idx", [new Ft.TextField("f")], new Ft.CreateOptions { StopWords = ["the", "a"] }).GetArgs());

        Assert.Equal(
            ["FT.CREATE", "idx", "ON", "HASH", "STOPWORDS", "0", "SCHEMA", "f", "TEXT"],
            Request.FtCreate("idx", [new Ft.TextField("f")], new Ft.CreateOptions { StopWords = Ft.CreateOptions.NoStopWords }).GetArgs());

        Assert.Equal(
            ["FT.CREATE", "idx", "ON", "HASH", "PUNCTUATION", ".,", "SCHEMA", "f", "TEXT"],
            Request.FtCreate("idx", [new Ft.TextField("f")], new Ft.CreateOptions { Punctuation = ".," }).GetArgs());

        Assert.Equal(
            ["FT.CREATE", "idx", "SCHEMA", "$.f", "AS", "f", "TEXT", "NOSTEM", "NOSUFFIXTRIE"],
            Request.FtCreate("idx", [new Ft.TextField("$.f", "f") { NoStem = true, NoSuffixTrie = true }]).GetArgs());

        Assert.Equal(
            ["FT.CREATE", "idx", "SCHEMA", "f", "TAG", "SEPARATOR", "|", "CASESENSITIVE"],
            Request.FtCreate("idx", [new Ft.TagField("f") { Separator = '|', CaseSensitive = true }]).GetArgs());

        Assert.Equal(
            ["FT.CREATE", "idx", "SCHEMA", "$.f", "AS", "f", "NUMERIC"],
            Request.FtCreate("idx", [new Ft.NumericField("$.f", "f")]).GetArgs());

        Assert.Equal(
            ["FT.CREATE", "idx", "SCHEMA", "v", "VECTOR", "FLAT", "6", "DIM", "128", "DISTANCE_METRIC", "COSINE", "TYPE", "FLOAT32"],
            Request.FtCreate("idx", [new Ft.VectorFieldFlat { Identifier = "v", Dimensions = 128, DistanceMetric = Ft.DistanceMetric.Cosine }]).GetArgs());

        Assert.Equal(
            ["FT.CREATE", "idx", "SCHEMA", "v", "VECTOR", "FLAT", "8", "DIM", "64", "DISTANCE_METRIC", "L2", "TYPE", "FLOAT32", "INITIAL_CAP", "1000"],
            Request.FtCreate("idx", [new Ft.VectorFieldFlat { Identifier = "v", Dimensions = 64, DistanceMetric = Ft.DistanceMetric.Euclidean, InitialCap = 1000 }]).GetArgs());

        Assert.Equal(
            ["FT.CREATE", "idx", "SCHEMA", "v", "VECTOR", "HNSW", "12", "DIM", "256", "DISTANCE_METRIC", "IP", "TYPE", "FLOAT32", "M", "16", "EF_CONSTRUCTION", "200", "EF_RUNTIME", "10"],
            Request.FtCreate("idx", [new Ft.VectorFieldHnsw { Identifier = "v", Dimensions = 256, DistanceMetric = Ft.DistanceMetric.InnerProduct, NumberOfEdges = 16, VectorsExaminedOnConstruction = 200, VectorsExaminedOnRuntime = 10 }]).GetArgs());

        Assert.Equal(
            ["FT.CREATE", "idx", "ON", "HASH", "PREFIX", "1", "p:", "SCHEMA", "t", "TEXT", "c", "TAG", "n", "NUMERIC", "v", "VECTOR", "FLAT", "6", "DIM", "3", "DISTANCE_METRIC", "COSINE", "TYPE", "FLOAT32"],
            Request.FtCreate("idx", [new Ft.TextField("t"), new Ft.TagField("c"), new Ft.NumericField("n"), new Ft.VectorFieldFlat { Identifier = "v", Dimensions = 3, DistanceMetric = Ft.DistanceMetric.Cosine }], new Ft.CreateOptions { Prefixes = ["p:"] }).GetArgs());
    }

    [Fact]
    public void FtDropIndex()
        => Assert.Equal(["FT.DROPINDEX", "idx"], Request.FtDropIndex("idx").GetArgs());

    [Fact]
    public void FtList()
        => Assert.Equal(["FT._LIST"], Request.FtList().GetArgs());

    [Fact]
    public void FtSearch()
    {
        Assert.Equal(
            ["FT.SEARCH", "idx", "*"],
            Request.FtSearch("idx", "*").GetArgs());

        Assert.Equal(
            ["FT.SEARCH", "idx", "*"],
            Request.FtSearch("idx", "*", new Ft.SearchOptions()).GetArgs());

        Assert.Equal(
            ["FT.SEARCH", "idx", "*", "NOCONTENT"],
            Request.FtSearch("idx", "*", new Ft.SearchOptions { Return = Ft.SearchOptions.NoContent }).GetArgs());

        Assert.Equal(
            ["FT.SEARCH", "idx", "*", "WITHSORTKEYS", "SORTBY", "f", "ASC"],
            Request.FtSearch("idx", "*", new Ft.SearchOptions { SortBy = new Ft.SearchSortBy { Field = "f", Order = SortOrder.Ascending, WithSortKeys = true } }).GetArgs());

        _ = Assert.Throws<ArgumentException>(() =>
            Request.FtSearch("idx", "*", new Ft.SearchOptions { Return = Ft.SearchOptions.NoContent, SortBy = new Ft.SearchSortBy { Field = "f", WithSortKeys = true } }));

        Assert.Equal(
            ["FT.SEARCH", "idx", "*", "VERBATIM", "INCONSISTENT", "SOMESHARDS", "INORDER"],
            Request.FtSearch("idx", "*", new Ft.SearchOptions { Verbatim = true, Inconsistent = true, SomeShards = true, InOrder = true }).GetArgs());

        Assert.Equal(
            ["FT.SEARCH", "idx", "*", "SLOP", "2"],
            Request.FtSearch("idx", "*", new Ft.SearchOptions { Slop = 2 }).GetArgs());

        Assert.Equal(
            ["FT.SEARCH", "idx", "*", "LIMIT", "10", "20"],
            Request.FtSearch("idx", "*", new Ft.SearchOptions { Limit = new Ft.SearchLimit { Offset = 10, Count = 20 } }).GetArgs());

        Assert.Equal(
            ["FT.SEARCH", "idx", "*", "SORTBY", "f", "ASC"],
            Request.FtSearch("idx", "*", new Ft.SearchOptions { SortBy = new Ft.SearchSortBy { Field = "f", Order = SortOrder.Ascending } }).GetArgs());

        Assert.Equal(
            ["FT.SEARCH", "idx", "*", "SORTBY", "f", "DESC"],
            Request.FtSearch("idx", "*", new Ft.SearchOptions { SortBy = new Ft.SearchSortBy { Field = "f", Order = SortOrder.Descending } }).GetArgs());

        Assert.Equal(
            ["FT.SEARCH", "idx", "*", "SORTBY", "f"],
            Request.FtSearch("idx", "*", new Ft.SearchOptions { SortBy = new Ft.SearchSortBy { Field = "f" } }).GetArgs());

        Assert.Equal(
            ["FT.SEARCH", "idx", "*", "RETURN", "4", "a", "$.b", "AS", "b"],
            Request.FtSearch("idx", "*", new Ft.SearchOptions { Return = ["a", new Ft.SearchReturnField { Field = "$.b", Name = "b" }] }).GetArgs());

        Assert.Equal(
            ["FT.SEARCH", "idx", "@f:$t", "PARAMS", "2", "t", "v"],
            Request.FtSearch("idx", "@f:$t", new Ft.SearchOptions { Params = new Dictionary<ValkeyValue, ValkeyValue> { ["t"] = "v" } }).GetArgs());

        Assert.Equal(
            ["FT.SEARCH", "idx", "*", "TIMEOUT", "500"],
            Request.FtSearch("idx", "*", new Ft.SearchOptions { Timeout = TimeSpan.FromMilliseconds(500) }).GetArgs());
    }

    [Fact]
    public void FtAggregate()
    {
        Assert.Equal(
            ["FT.AGGREGATE", "idx", "*"],
            Request.FtAggregate("idx", "*").GetArgs());

        Assert.Equal(
            ["FT.AGGREGATE", "idx", "*"],
            Request.FtAggregate("idx", "*", new Ft.AggregateOptions()).GetArgs());

        Assert.Equal(
            ["FT.AGGREGATE", "idx", "*", "VERBATIM", "INORDER", "SLOP", "3"],
            Request.FtAggregate("idx", "*", new Ft.AggregateOptions { Verbatim = true, InOrder = true, Slop = 3 }).GetArgs());

        Assert.Equal(
            ["FT.AGGREGATE", "idx", "*", "LOAD", "*"],
            Request.FtAggregate("idx", "*", new Ft.AggregateOptions { LoadFields = null }).GetArgs());

        Assert.Equal(
            ["FT.AGGREGATE", "idx", "*", "LOAD", "2", "@a", "@b"],
            Request.FtAggregate("idx", "*", new Ft.AggregateOptions { LoadFields = ["@a", "@b"] }).GetArgs());

        Assert.Equal(
            ["FT.AGGREGATE", "idx", "*"],
            Request.FtAggregate("idx", "*", new Ft.AggregateOptions { LoadFields = Ft.AggregateOptions.LoadNone }).GetArgs());

        Assert.Equal(
            ["FT.AGGREGATE", "idx", "@f:$t", "PARAMS", "2", "t", "v"],
            Request.FtAggregate("idx", "@f:$t", new Ft.AggregateOptions { Params = new Dictionary<ValkeyValue, ValkeyValue> { ["t"] = "v" } }).GetArgs());

        Assert.Equal(
            ["FT.AGGREGATE", "idx", "*", "TIMEOUT", "1000"],
            Request.FtAggregate("idx", "*", new Ft.AggregateOptions { Timeout = TimeSpan.FromSeconds(1) }).GetArgs());

        Assert.Equal(
            ["FT.AGGREGATE", "idx", "*", "GROUPBY", "1", "@c", "REDUCE", "COUNT", "0", "AS", "n"],
            Request.FtAggregate("idx", "*", new Ft.AggregateOptions { Clauses = [new Ft.AggregateGroupBy { Fields = ["@c"], Reducers = [new Ft.AggregateReducer { Function = Ft.ReducerFunction.Count, Name = "n" }] }] }).GetArgs());

        Assert.Equal(
            ["FT.AGGREGATE", "idx", "*", "SORTBY", "2", "@s", "ASC", "MAX", "10"],
            Request.FtAggregate("idx", "*", new Ft.AggregateOptions { Clauses = [new Ft.AggregateSortBy { Expressions = [new Ft.AggregateSortExpression { Expression = "@s", Order = SortOrder.Ascending }], Max = 10 }] }).GetArgs());

        Assert.Equal(
            ["FT.AGGREGATE", "idx", "*", "FILTER", "@p > 0"],
            Request.FtAggregate("idx", "*", new Ft.AggregateOptions { Clauses = [new Ft.AggregateFilter { Expression = "@p > 0" }] }).GetArgs());

        Assert.Equal(
            ["FT.AGGREGATE", "idx", "*", "APPLY", "upper(@n)", "AS", "u"],
            Request.FtAggregate("idx", "*", new Ft.AggregateOptions { Clauses = [new Ft.AggregateApply { Expression = "upper(@n)", Name = "u" }] }).GetArgs());

        Assert.Equal(
            ["FT.AGGREGATE", "idx", "*", "LIMIT", "5", "10"],
            Request.FtAggregate("idx", "*", new Ft.AggregateOptions { Clauses = [new Ft.AggregateLimit { Offset = 5, Count = 10 }] }).GetArgs());

        Assert.Equal(
            ["FT.AGGREGATE", "idx", "*", "GROUPBY", "1", "@c", "SORTBY", "1", "@n", "LIMIT", "0", "5"],
            Request.FtAggregate("idx", "*", new Ft.AggregateOptions { Clauses = [new Ft.AggregateGroupBy { Fields = ["@c"] }, new Ft.AggregateSortBy { Expressions = ["@n"] }, new Ft.AggregateLimit { Count = 5 }] }).GetArgs());
    }

    [Fact]
    public void FtInfo()
    {
        Assert.Equal(
            ["FT.INFO", "idx"],
            Request.FtInfo("idx").GetArgs());

        Assert.Equal(
            ["FT.INFO", "idx", "LOCAL"],
            Request.FtInfo("idx", new Ft.InfoOptions()).GetArgs());

        Assert.Equal(
            ["FT.INFO", "idx", "CLUSTER"],
            Request.FtInfo("idx", new Ft.InfoOptions { Scope = Ft.InfoScope.Cluster }).GetArgs());

        Assert.Equal(
            ["FT.INFO", "idx", "PRIMARY"],
            Request.FtInfo("idx", new Ft.InfoOptions { Scope = Ft.InfoScope.Primary }).GetArgs());

        Assert.Equal(
            ["FT.INFO", "idx", "LOCAL", "SOMESHARDS"],
            Request.FtInfo("idx", new Ft.InfoOptions { SomeShards = true }).GetArgs());

        Assert.Equal(
            ["FT.INFO", "idx", "LOCAL", "INCONSISTENT"],
            Request.FtInfo("idx", new Ft.InfoOptions { Inconsistent = true }).GetArgs());

        Assert.Equal(
            ["FT.INFO", "idx", "CLUSTER", "SOMESHARDS", "INCONSISTENT"],
            Request.FtInfo("idx", new Ft.InfoOptions { Scope = Ft.InfoScope.Cluster, SomeShards = true, Inconsistent = true }).GetArgs());
    }
}
