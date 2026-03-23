// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands.Options;

namespace Valkey.Glide.UnitTests;

public class VectorSearchCommandTests
{
    [Fact]
    public void ValidateFtCreateArgs()
    {
        Assert.Multiple(
            // Basic create with text field
            () => Assert.Equal(
                ["FT.CREATE", "myindex", "SCHEMA", "title", "TEXT"],
                Request.FtCreate("myindex", [new TextField("title")], null).GetArgs()),

            // Create with options: ON HASH, PREFIX
            () => Assert.Equal(
                ["FT.CREATE", "myindex", "ON", "HASH", "PREFIX", "1", "doc:", "SCHEMA", "title", "TEXT"],
                Request.FtCreate("myindex", [new TextField("title")],
                    new FtCreateOptions { DataType = IndexDataType.HASH, Prefixes = ["doc:"] }).GetArgs()),

            // Create with multiple prefixes
            () => Assert.Equal(
                ["FT.CREATE", "myindex", "ON", "JSON", "PREFIX", "2", "a:", "b:", "SCHEMA", "score", "NUMERIC"],
                Request.FtCreate("myindex", [new NumericField("score")],
                    new FtCreateOptions { DataType = IndexDataType.JSON, Prefixes = ["a:", "b:"] }).GetArgs()),

            // TextField with all options
            () => Assert.Equal(
                ["FT.CREATE", "idx", "SCHEMA", "title", "AS", "t", "TEXT", "NOSTEM", "WEIGHT", "2", "WITHSUFFIXTRIE", "SORTABLE"],
                Request.FtCreate("idx", [new TextField("title") { Alias = "t", NoStem = true, Weight = 2.0, WithSuffixTrie = true, Sortable = true }], null).GetArgs()),

            // TagField with separator and case sensitive
            () => Assert.Equal(
                ["FT.CREATE", "idx", "SCHEMA", "tags", "TAG", "SEPARATOR", ",", "CASESENSITIVE", "SORTABLE"],
                Request.FtCreate("idx", [new TagField("tags") { Separator = ",", CaseSensitive = true, Sortable = true }], null).GetArgs()),

            // NumericField with alias and sortable
            () => Assert.Equal(
                ["FT.CREATE", "idx", "SCHEMA", "price", "AS", "p", "NUMERIC", "SORTABLE"],
                Request.FtCreate("idx", [new NumericField("price") { Alias = "p", Sortable = true }], null).GetArgs()),

            // VectorFieldFlat
            () => Assert.Equal(
                ["FT.CREATE", "idx", "SCHEMA", "vec", "VECTOR", "FLAT", "6", "DIM", "128", "DISTANCE_METRIC", "L2", "TYPE", "FLOAT32"],
                Request.FtCreate("idx", [new VectorFieldFlat("vec", DistanceMetric.L2, 128)], null).GetArgs()),

            // VectorFieldFlat with InitialCap
            () => Assert.Equal(
                ["FT.CREATE", "idx", "SCHEMA", "vec", "VECTOR", "FLAT", "8", "DIM", "4", "DISTANCE_METRIC", "COSINE", "TYPE", "FLOAT32", "INITIAL_CAP", "100"],
                Request.FtCreate("idx", [new VectorFieldFlat("vec", DistanceMetric.COSINE, 4) { InitialCap = 100 }], null).GetArgs()),

            // VectorFieldHnsw with all options
            () => Assert.Equal(
                ["FT.CREATE", "idx", "SCHEMA", "vec", "VECTOR", "HNSW", "14", "DIM", "256", "DISTANCE_METRIC", "IP", "TYPE", "FLOAT32", "INITIAL_CAP", "1000", "M", "32", "EF_CONSTRUCTION", "200", "EF_RUNTIME", "50"],
                Request.FtCreate("idx", [new VectorFieldHnsw("vec", DistanceMetric.IP, 256)
                {
                    InitialCap = 1000, NumberOfEdges = 32,
                    VectorsExaminedOnConstruction = 200, VectorsExaminedOnRuntime = 50
                }], null).GetArgs()),

            // Multiple fields
            () => Assert.Equal(
                ["FT.CREATE", "idx", "ON", "HASH", "PREFIX", "1", "doc:", "SCHEMA", "title", "TEXT", "score", "NUMERIC", "embedding", "VECTOR", "FLAT", "6", "DIM", "4", "DISTANCE_METRIC", "L2", "TYPE", "FLOAT32"],
                Request.FtCreate("idx",
                [
                    new TextField("title"),
                    new NumericField("score"),
                    new VectorFieldFlat("embedding", DistanceMetric.L2, 4),
                ],
                new FtCreateOptions { DataType = IndexDataType.HASH, Prefixes = ["doc:"] }).GetArgs()),

            // Create with SkipInitialScan, NoOffsets, NoStopWords
            () => Assert.Equal(
                ["FT.CREATE", "idx", "SKIPINITIALSCAN", "NOOFFSETS", "NOSTOPWORDS", "SCHEMA", "f", "TEXT"],
                Request.FtCreate("idx", [new TextField("f")],
                    new FtCreateOptions { SkipInitialScan = true, NoOffsets = true, NoStopWords = true }).GetArgs()),

            // Create with StopWords
            () => Assert.Equal(
                ["FT.CREATE", "idx", "STOPWORDS", "2", "the", "a", "SCHEMA", "f", "TEXT"],
                Request.FtCreate("idx", [new TextField("f")],
                    new FtCreateOptions { StopWords = ["the", "a"] }).GetArgs())
        );
    }

    [Fact]
    public void ValidateFtDropIndexArgs()
    {
        Assert.Equal(["FT.DROPINDEX", "myindex"], Request.FtDropIndex("myindex").GetArgs());
    }

    [Fact]
    public void ValidateFtListArgs()
    {
        Assert.Equal(["FT._LIST"], Request.FtList().GetArgs());
    }

    [Fact]
    public void ValidateFtSearchArgs()
    {
        Assert.Multiple(
            // Basic search
            () => Assert.Equal(
                ["FT.SEARCH", "idx", "*"],
                Request.FtSearch("idx", "*", null).GetArgs()),

            // Search with NOCONTENT
            () => Assert.Equal(
                ["FT.SEARCH", "idx", "@title:hello", "NOCONTENT"],
                Request.FtSearch("idx", "@title:hello", new FtSearchOptions { NoContent = true }).GetArgs()),

            // Search with VERBATIM and LIMIT
            () => Assert.Equal(
                ["FT.SEARCH", "idx", "*", "VERBATIM", "LIMIT", "0", "10"],
                Request.FtSearch("idx", "*", new FtSearchOptions { Verbatim = true, Limit = new FtSearchLimit(0, 10) }).GetArgs()),

            // Search with SORTBY and order
            () => Assert.Equal(
                ["FT.SEARCH", "idx", "*", "SORTBY", "score", "DESC"],
                Request.FtSearch("idx", "*", new FtSearchOptions { SortBy = "score", SortByOrder = FtSearchSortOrder.DESC }).GetArgs()),

            // Search with RETURN fields
            () => Assert.Equal(
                ["FT.SEARCH", "idx", "*", "RETURN", "3", "title", "AS", "t"],
                Request.FtSearch("idx", "*", new FtSearchOptions
                {
                    ReturnFields = [new FtSearchReturnField("title") { Alias = "t" }]
                }).GetArgs()),

            // Search with PARAMS
            () => Assert.Equal(
                ["FT.SEARCH", "idx", "*", "PARAMS", "2", "k1", "v1"],
                Request.FtSearch("idx", "*", new FtSearchOptions
                {
                    Params = [new FtSearchParam("k1", "v1")]
                }).GetArgs()),

            // Search with TIMEOUT and DIALECT
            () => Assert.Equal(
                ["FT.SEARCH", "idx", "*", "TIMEOUT", "5000", "DIALECT", "2"],
                Request.FtSearch("idx", "*", new FtSearchOptions { Timeout = 5000, Dialect = 2 }).GetArgs()),

            // Search with WITHSORTKEYS
            () => Assert.Equal(
                ["FT.SEARCH", "idx", "*", "SORTBY", "score", "WITHSORTKEYS"],
                Request.FtSearch("idx", "*", new FtSearchOptions { SortBy = "score", WithSortKeys = true }).GetArgs()),

            // Search with WITHSORTKEYS + NOCONTENT — WITHSORTKEYS is silently stripped
            () => Assert.Equal(
                ["FT.SEARCH", "idx", "*", "NOCONTENT", "SORTBY", "score"],
                Request.FtSearch("idx", "*", new FtSearchOptions { SortBy = "score", WithSortKeys = true, NoContent = true }).GetArgs()),

            // Search with INORDER and SLOP
            () => Assert.Equal(
                ["FT.SEARCH", "idx", "*", "INORDER", "SLOP", "2"],
                Request.FtSearch("idx", "*", new FtSearchOptions { InOrder = true, Slop = 2 }).GetArgs()),

            // Search with SHARDSCOPE and CONSISTENCY
            () => Assert.Equal(
                ["FT.SEARCH", "idx", "@tag:{test}", "SOMESHARDS", "INCONSISTENT"],
                Request.FtSearch("idx", "@tag:{test}", new FtSearchOptions
                {
                    ShardScope = FtSearchShardScope.SOMESHARDS,
                    Consistency = FtSearchConsistencyMode.INCONSISTENT
                }).GetArgs()),

            // Search with ALLSHARDS and CONSISTENT
            () => Assert.Equal(
                ["FT.SEARCH", "idx", "*", "ALLSHARDS", "CONSISTENT"],
                Request.FtSearch("idx", "*", new FtSearchOptions
                {
                    ShardScope = FtSearchShardScope.ALLSHARDS,
                    Consistency = FtSearchConsistencyMode.CONSISTENT
                }).GetArgs())
        );
    }

    [Fact]
    public void ValidateFtAggregateArgs()
    {
        Assert.Multiple(
            // Basic aggregate
            () => Assert.Equal(
                ["FT.AGGREGATE", "idx", "*"],
                Request.FtAggregate("idx", "*", null).GetArgs()),

            // Aggregate with LOAD *
            () => Assert.Equal(
                ["FT.AGGREGATE", "idx", "*", "LOAD", "*"],
                Request.FtAggregate("idx", "*", new FtAggregateOptions { LoadAll = true }).GetArgs()),

            // Aggregate with LOAD fields
            () => Assert.Equal(
                ["FT.AGGREGATE", "idx", "*", "LOAD", "2", "@f1", "@f2"],
                Request.FtAggregate("idx", "*", new FtAggregateOptions { LoadFields = ["@f1", "@f2"] }).GetArgs()),

            // Aggregate with GROUPBY and REDUCE
            () => Assert.Equal(
                ["FT.AGGREGATE", "idx", "*", "GROUPBY", "1", "@color", "REDUCE", "COUNT", "0", "AS", "count"],
                Request.FtAggregate("idx", "*", new FtAggregateOptions
                {
                    Clauses =
                    [
                        new FtAggregateGroupBy("@color")
                        {
                            Reducers = [new FtAggregateReducer("COUNT") { Name = "count" }]
                        }
                    ]
                }).GetArgs()),

            // Aggregate with SORTBY
            () => Assert.Equal(
                ["FT.AGGREGATE", "idx", "*", "SORTBY", "2", "@score", "DESC", "MAX", "10"],
                Request.FtAggregate("idx", "*", new FtAggregateOptions
                {
                    Clauses =
                    [
                        new FtAggregateSortBy(new FtAggregateSortProperty("@score", FtAggregateOrderBy.DESC)) { Max = 10 }
                    ]
                }).GetArgs()),

            // Aggregate with FILTER
            () => Assert.Equal(
                ["FT.AGGREGATE", "idx", "*", "FILTER", "@score > 5"],
                Request.FtAggregate("idx", "*", new FtAggregateOptions
                {
                    Clauses = [new FtAggregateFilter("@score > 5")]
                }).GetArgs()),

            // Aggregate with APPLY
            () => Assert.Equal(
                ["FT.AGGREGATE", "idx", "*", "APPLY", "@score * 2", "AS", "doubled"],
                Request.FtAggregate("idx", "*", new FtAggregateOptions
                {
                    Clauses = [new FtAggregateApply("@score * 2", "doubled")]
                }).GetArgs()),

            // Aggregate with LIMIT
            () => Assert.Equal(
                ["FT.AGGREGATE", "idx", "*", "LIMIT", "0", "10"],
                Request.FtAggregate("idx", "*", new FtAggregateOptions
                {
                    Clauses = [new FtAggregateLimit(0, 10)]
                }).GetArgs()),

            // Aggregate with VERBATIM, TIMEOUT, PARAMS, DIALECT
            () => Assert.Equal(
                ["FT.AGGREGATE", "idx", "*", "VERBATIM", "TIMEOUT", "3000", "PARAMS", "2", "k", "v", "DIALECT", "2"],
                Request.FtAggregate("idx", "*", new FtAggregateOptions
                {
                    Verbatim = true,
                    Timeout = 3000,
                    Params = [new FtAggregateParam("k", "v")],
                    Dialect = 2
                }).GetArgs())
        );
    }

    [Fact]
    public void ValidateFtInfoArgs()
    {
        Assert.Multiple(
            () => Assert.Equal(["FT.INFO", "idx"], Request.FtInfo("idx", null).GetArgs()),
            () => Assert.Equal(
                ["FT.INFO", "idx", "LOCAL"],
                Request.FtInfo("idx", new FtInfoOptions { Scope = FtInfoScope.LOCAL }).GetArgs()),
            () => Assert.Equal(
                ["FT.INFO", "idx", "CLUSTER", "ALLSHARDS", "CONSISTENT"],
                Request.FtInfo("idx", new FtInfoOptions
                {
                    Scope = FtInfoScope.CLUSTER,
                    ShardScope = FtInfoShardScope.ALLSHARDS,
                    Consistency = FtInfoConsistencyMode.CONSISTENT
                }).GetArgs())
        );
    }

    [Fact]
    public void ValidateFtAliasArgs()
    {
        Assert.Multiple(
            () => Assert.Equal(["FT.ALIASADD", "myalias", "myindex"], Request.FtAliasAdd("myalias", "myindex").GetArgs()),
            () => Assert.Equal(["FT.ALIASDEL", "myalias"], Request.FtAliasDel("myalias").GetArgs()),
            () => Assert.Equal(["FT.ALIASUPDATE", "myalias", "newindex"], Request.FtAliasUpdate("myalias", "newindex").GetArgs()),
            () => Assert.Equal(["FT._ALIASLIST"], Request.FtAliasList().GetArgs())
        );
    }

    [Fact]
    public void ValidateFtCreateOptionsValidation()
    {
        // WithOffsets and NoOffsets are mutually exclusive
        _ = Assert.Throws<ArgumentException>(() =>
            new FtCreateOptions { WithOffsets = true, NoOffsets = true }.ToArgs());

        // NoStopWords and StopWords are mutually exclusive
        _ = Assert.Throws<ArgumentException>(() =>
            new FtCreateOptions { NoStopWords = true, StopWords = ["the"] }.ToArgs());

        // WithSuffixTrie and NoSuffixTrie are mutually exclusive
        _ = Assert.Throws<ArgumentException>(() =>
            new TextField("f") { WithSuffixTrie = true, NoSuffixTrie = true }.ToArgs());
    }

    [Fact]
    public void ValidateFtSearchOptionsValidation()
    {
        // WithSortKeys requires SortBy
        _ = Assert.Throws<ArgumentException>(() =>
            new FtSearchOptions { WithSortKeys = true }.ToArgs());

        // SortByOrder requires SortBy
        _ = Assert.Throws<ArgumentException>(() =>
            new FtSearchOptions { SortByOrder = FtSearchSortOrder.ASC }.ToArgs());
    }

    [Fact]
    public void ValidateFtAggregateOptionsValidation()
    {
        // LoadAll and LoadFields are mutually exclusive
        _ = Assert.Throws<ArgumentException>(() =>
            new FtAggregateOptions { LoadAll = true, LoadFields = ["@f1"] }.ToArgs());
    }
}
