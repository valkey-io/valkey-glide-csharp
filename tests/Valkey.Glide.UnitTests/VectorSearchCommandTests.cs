// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands.Options;

namespace Valkey.Glide.UnitTests;

public class VectorSearchCommandTests
{
    [Fact]
    public void FtCreate_BasicWithTextField() =>
        Assert.Equal(
            ["FT.CREATE", "myindex", "SCHEMA", "title", "TEXT"],
            Request.FtCreate("myindex", [new TextField("title")], null).GetArgs());

    [Fact]
    public void FtCreate_WithOnHashAndPrefix() =>
        Assert.Equal(
            ["FT.CREATE", "myindex", "ON", "HASH", "PREFIX", "1", "doc:", "SCHEMA", "title", "TEXT"],
            Request.FtCreate("myindex", [new TextField("title")],
                new FtCreateOptions { DataType = IndexDataType.Hash, Prefixes = ["doc:"] }).GetArgs());

    [Fact]
    public void FtCreate_WithMultiplePrefixes() =>
        Assert.Equal(
            ["FT.CREATE", "myindex", "ON", "JSON", "PREFIX", "2", "a:", "b:", "SCHEMA", "score", "NUMERIC"],
            Request.FtCreate("myindex", [new NumericField("score")],
                new FtCreateOptions { DataType = IndexDataType.Json, Prefixes = ["a:", "b:"] }).GetArgs());

    [Fact]
    public void FtCreate_TextFieldWithAllOptions() =>
        Assert.Equal(
            ["FT.CREATE", "idx", "SCHEMA", "title", "AS", "t", "TEXT", "NOSTEM", "WEIGHT", "2", "WITHSUFFIXTRIE", "SORTABLE"],
            Request.FtCreate("idx", [new TextField("title") { Alias = "t", NoStem = true, Weight = 2.0, SuffixTrie = true, Sortable = true }], null).GetArgs());

    [Fact]
    public void FtCreate_TagFieldWithSeparatorAndCaseSensitive() =>
        Assert.Equal(
            ["FT.CREATE", "idx", "SCHEMA", "tags", "TAG", "SEPARATOR", ",", "CASESENSITIVE", "SORTABLE"],
            Request.FtCreate("idx", [new TagField("tags") { Separator = ",", CaseSensitive = true, Sortable = true }], null).GetArgs());

    [Fact]
    public void FtCreate_NumericFieldWithAliasAndSortable() =>
        Assert.Equal(
            ["FT.CREATE", "idx", "SCHEMA", "price", "AS", "p", "NUMERIC", "SORTABLE"],
            Request.FtCreate("idx", [new NumericField("price") { Alias = "p", Sortable = true }], null).GetArgs());

    [Fact]
    public void FtCreate_VectorFieldFlat() =>
        Assert.Equal(
            ["FT.CREATE", "idx", "SCHEMA", "vec", "VECTOR", "FLAT", "6", "DIM", "128", "DISTANCE_METRIC", "L2", "TYPE", "FLOAT32"],
            Request.FtCreate("idx", [new VectorFieldFlat("vec", DistanceMetric.Euclidean, 128)], null).GetArgs());

    [Fact]
    public void FtCreate_VectorFieldFlatWithInitialCap() =>
        Assert.Equal(
            ["FT.CREATE", "idx", "SCHEMA", "vec", "VECTOR", "FLAT", "8", "DIM", "4", "DISTANCE_METRIC", "COSINE", "TYPE", "FLOAT32", "INITIAL_CAP", "100"],
            Request.FtCreate("idx", [new VectorFieldFlat("vec", DistanceMetric.Cosine, 4) { InitialCap = 100 }], null).GetArgs());

    [Fact]
    public void FtCreate_VectorFieldHnswWithAllOptions() =>
        Assert.Equal(
            ["FT.CREATE", "idx", "SCHEMA", "vec", "VECTOR", "HNSW", "14", "DIM", "256", "DISTANCE_METRIC", "IP", "TYPE", "FLOAT32", "INITIAL_CAP", "1000", "M", "32", "EF_CONSTRUCTION", "200", "EF_RUNTIME", "50"],
            Request.FtCreate("idx", [new VectorFieldHnsw("vec", DistanceMetric.InnerProduct, 256)
            {
                InitialCap = 1000,
                NumberOfEdges = 32,
                VectorsExaminedOnConstruction = 200,
                VectorsExaminedOnRuntime = 50
            }], null).GetArgs());

    [Fact]
    public void FtCreate_MultipleFields() =>
        Assert.Equal(
            ["FT.CREATE", "idx", "ON", "HASH", "PREFIX", "1", "doc:", "SCHEMA", "title", "TEXT", "score", "NUMERIC", "embedding", "VECTOR", "FLAT", "6", "DIM", "4", "DISTANCE_METRIC", "L2", "TYPE", "FLOAT32"],
            Request.FtCreate("idx",
            [
                new TextField("title"),
                new NumericField("score"),
                new VectorFieldFlat("embedding", DistanceMetric.Euclidean, 4),
            ],
            new FtCreateOptions { DataType = IndexDataType.Hash, Prefixes = ["doc:"] }).GetArgs());

    [Fact]
    public void FtCreate_WithSkipInitialScanNoOffsetsNoStopWords() =>
        Assert.Equal(
            ["FT.CREATE", "idx", "SKIPINITIALSCAN", "NOOFFSETS", "NOSTOPWORDS", "SCHEMA", "f", "TEXT"],
            Request.FtCreate("idx", [new TextField("f")],
                new FtCreateOptions { SkipInitialScan = true, Offsets = false, NoStopWords = true }).GetArgs());

    [Fact]
    public void FtCreate_WithStopWords() =>
        Assert.Equal(
            ["FT.CREATE", "idx", "STOPWORDS", "2", "the", "a", "SCHEMA", "f", "TEXT"],
            Request.FtCreate("idx", [new TextField("f")],
                new FtCreateOptions { StopWords = ["the", "a"] }).GetArgs());

    [Fact]
    public void ValidateFtDropIndexArgs() => Assert.Equal(["FT.DROPINDEX", "myindex"], Request.FtDropIndex("myindex").GetArgs());

    [Fact]
    public void ValidateFtListArgs() => Assert.Equal(["FT._LIST"], Request.FtList().GetArgs());

    [Fact]
    public void ValidateFtSearchArgs_Basic() =>
        Assert.Equal(
            ["FT.SEARCH", "idx", "*"],
            Request.FtSearch("idx", "*", null).GetArgs());

    [Fact]
    public void ValidateFtSearchArgs_NoContent() =>
        Assert.Equal(
            ["FT.SEARCH", "idx", "@title:hello", "NOCONTENT"],
            Request.FtSearch("idx", "@title:hello", new FtSearchOptions { NoContent = true }).GetArgs());

    [Fact]
    public void ValidateFtSearchArgs_VerbatimAndLimit() =>
        Assert.Equal(
            ["FT.SEARCH", "idx", "*", "VERBATIM", "LIMIT", "0", "10"],
            Request.FtSearch("idx", "*", new FtSearchOptions { Verbatim = true, Limit = new FtLimit(0, 10) }).GetArgs());

    [Fact]
    public void ValidateFtSearchArgs_SortByDesc() =>
        Assert.Equal(
            ["FT.SEARCH", "idx", "*", "SORTBY", "score", "DESC"],
            Request.FtSearch("idx", "*", new FtSearchOptions { SortBy = new FtSearchSortBy("score", FtSearchSortOrder.Descending) }).GetArgs());

    [Fact]
    public void ValidateFtSearchArgs_ReturnFields() =>
        Assert.Equal(
            ["FT.SEARCH", "idx", "*", "RETURN", "3", "title", "AS", "t"],
            Request.FtSearch("idx", "*", new FtSearchOptions
            {
                ReturnFields = [new FtSearchReturnField("title", "t")]
            }).GetArgs());

    [Fact]
    public void ValidateFtSearchArgs_Params() =>
        Assert.Equal(
            ["FT.SEARCH", "idx", "*", "PARAMS", "2", "k1", "v1"],
            Request.FtSearch("idx", "*", new FtSearchOptions
            {
                Params = [new FtSearchParam("k1", "v1")]
            }).GetArgs());

    [Fact]
    public void ValidateFtSearchArgs_TimeoutAndDialect() =>
        Assert.Equal(
            ["FT.SEARCH", "idx", "*", "TIMEOUT", "5000", "DIALECT", "2"],
            Request.FtSearch("idx", "*", new FtSearchOptions { Timeout = TimeSpan.FromMilliseconds(5000), Dialect = 2 }).GetArgs());

    [Fact]
    public void ValidateFtSearchArgs_WithSortKeys() =>
        Assert.Equal(
            ["FT.SEARCH", "idx", "*", "SORTBY", "score", "WITHSORTKEYS"],
            Request.FtSearch("idx", "*", new FtSearchOptions { SortBy = new FtSearchSortBy("score", withSortKeys: true) }).GetArgs());

    [Fact]
    public void ValidateFtSearchArgs_WithSortKeysAndNoContent_SortKeysStripped() =>
        Assert.Equal(
            ["FT.SEARCH", "idx", "*", "NOCONTENT", "SORTBY", "score"],
            Request.FtSearch("idx", "*", new FtSearchOptions { SortBy = new FtSearchSortBy("score", withSortKeys: true), NoContent = true }).GetArgs());

    [Fact]
    public void ValidateFtSearchArgs_InOrderAndSlop() =>
        Assert.Equal(
            ["FT.SEARCH", "idx", "*", "INORDER", "SLOP", "2"],
            Request.FtSearch("idx", "*", new FtSearchOptions { InOrder = true, Slop = 2 }).GetArgs());

    [Fact]
    public void ValidateFtSearchArgs_ShardScopeAndConsistency() =>
        Assert.Equal(
            ["FT.SEARCH", "idx", "@tag:{test}", "SOMESHARDS", "INCONSISTENT"],
            Request.FtSearch("idx", "@tag:{test}", new FtSearchOptions
            {
                ShardScope = FtSearchShardScope.SomeShards,
                Consistency = FtSearchConsistencyMode.Inconsistent
            }).GetArgs());

    [Fact]
    public void ValidateFtSearchArgs_AllShardsAndConsistent() =>
        Assert.Equal(
            ["FT.SEARCH", "idx", "*", "ALLSHARDS", "CONSISTENT"],
            Request.FtSearch("idx", "*", new FtSearchOptions
            {
                ShardScope = FtSearchShardScope.AllShards,
                Consistency = FtSearchConsistencyMode.Consistent
            }).GetArgs());

    [Fact]
    public void ValidateFtAggregateArgs_Basic() =>
        Assert.Equal(
            ["FT.AGGREGATE", "idx", "*"],
            Request.FtAggregate("idx", "*", null).GetArgs());

    [Fact]
    public void ValidateFtAggregateArgs_LoadAll() =>
        Assert.Equal(
            ["FT.AGGREGATE", "idx", "*", "LOAD", "*"],
            Request.FtAggregate("idx", "*", new FtAggregateOptions { LoadAll = true }).GetArgs());

    [Fact]
    public void ValidateFtAggregateArgs_LoadFields() =>
        Assert.Equal(
            ["FT.AGGREGATE", "idx", "*", "LOAD", "2", "@f1", "@f2"],
            Request.FtAggregate("idx", "*", new FtAggregateOptions { LoadFields = ["@f1", "@f2"] }).GetArgs());

    [Fact]
    public void ValidateFtAggregateArgs_GroupByAndReduce() =>
        Assert.Equal(
            ["FT.AGGREGATE", "idx", "*", "GROUPBY", "1", "@color", "REDUCE", "COUNT", "0", "AS", "count"],
            Request.FtAggregate("idx", "*", new FtAggregateOptions
            {
                Clauses =
                [
                    new FtAggregateGroupBy("@color")
                    {
                        Reducers = [new FtAggregateReducer(FtReducerFunction.Count) { Name = "count" }]
                    }
                ]
            }).GetArgs());

    [Fact]
    public void ValidateFtAggregateArgs_SortBy() =>
        Assert.Equal(
            ["FT.AGGREGATE", "idx", "*", "SORTBY", "2", "@score", "DESC", "MAX", "10"],
            Request.FtAggregate("idx", "*", new FtAggregateOptions
            {
                Clauses =
                [
                    new FtAggregateSortBy([new FtAggregateSortProperty("@score", SortOrder.Descending)], 10)
                ]
            }).GetArgs());

    [Fact]
    public void ValidateFtAggregateArgs_Filter() =>
        Assert.Equal(
            ["FT.AGGREGATE", "idx", "*", "FILTER", "@score > 5"],
            Request.FtAggregate("idx", "*", new FtAggregateOptions
            {
                Clauses = [new FtAggregateFilter("@score > 5")]
            }).GetArgs());

    [Fact]
    public void ValidateFtAggregateArgs_Apply() =>
        Assert.Equal(
            ["FT.AGGREGATE", "idx", "*", "APPLY", "@score * 2", "AS", "doubled"],
            Request.FtAggregate("idx", "*", new FtAggregateOptions
            {
                Clauses = [new FtAggregateApply("@score * 2", "doubled")]
            }).GetArgs());

    [Fact]
    public void ValidateFtAggregateArgs_Limit() =>
        Assert.Equal(
            ["FT.AGGREGATE", "idx", "*", "LIMIT", "0", "10"],
            Request.FtAggregate("idx", "*", new FtAggregateOptions
            {
                Clauses = [new FtLimit(0, 10)]
            }).GetArgs());

    [Fact]
    public void ValidateFtAggregateArgs_VerbatimTimeoutParamsDialect() =>
        Assert.Equal(
            ["FT.AGGREGATE", "idx", "*", "VERBATIM", "TIMEOUT", "3000", "PARAMS", "2", "k", "v", "DIALECT", "2"],
            Request.FtAggregate("idx", "*", new FtAggregateOptions
            {
                Verbatim = true,
                Timeout = TimeSpan.FromMilliseconds(3000),
                Params = [new FtAggregateParam("k", "v")],
                Dialect = 2
            }).GetArgs());

    [Fact]
    public void ValidateFtInfoArgs_Basic() =>
        Assert.Equal(["FT.INFO", "idx"], Request.FtInfo("idx", null).GetArgs());

    [Fact]
    public void ValidateFtInfoArgs_LocalScope() =>
        Assert.Equal(
            ["FT.INFO", "idx", "LOCAL"],
            Request.FtInfo("idx", new FtInfoOptions { Scope = FtInfoScope.Local }).GetArgs());

    [Fact]
    public void ValidateFtInfoArgs_ClusterWithFlags() =>
        Assert.Equal(
            ["FT.INFO", "idx", "CLUSTER", "ALLSHARDS", "CONSISTENT"],
            Request.FtInfo("idx", new FtInfoOptions
            {
                Scope = FtInfoScope.Cluster,
                ShardScope = FtInfoShardScope.AllShards,
                Consistency = FtInfoConsistencyMode.Consistent
            }).GetArgs());

    [Fact]
    public void ValidateFtAliasArgs_Add() =>
        Assert.Equal(["FT.ALIASADD", "myalias", "myindex"], Request.FtAliasAdd("myalias", "myindex").GetArgs());

    [Fact]
    public void ValidateFtAliasArgs_Del() =>
        Assert.Equal(["FT.ALIASDEL", "myalias"], Request.FtAliasDel("myalias").GetArgs());

    [Fact]
    public void ValidateFtAliasArgs_Update() =>
        Assert.Equal(["FT.ALIASUPDATE", "myalias", "newindex"], Request.FtAliasUpdate("myalias", "newindex").GetArgs());

    [Fact]
    public void ValidateFtAliasArgs_List() =>
        Assert.Equal(["FT._ALIASLIST"], Request.FtAliasList().GetArgs());

    [Fact]
    public void ValidateFtCreateOptionsValidation() =>
        // NoStopWords and StopWords are mutually exclusive
        _ = Assert.Throws<ArgumentException>(() =>
            new FtCreateOptions { NoStopWords = true, StopWords = ["the"] }.ToArgs());

    [Fact]
    public void ValidateFtCreateSuffixTrieEncoding()
    {
        // SuffixTrie = true  → WITHSUFFIXTRIE
        Assert.Equal(
            ["title", "TEXT", "WITHSUFFIXTRIE"],
            new TextField("title") { SuffixTrie = true }.ToArgs());

        // SuffixTrie = false → NOSUFFIXTRIE
        Assert.Equal(
            ["title", "TEXT", "NOSUFFIXTRIE"],
            new TextField("title") { SuffixTrie = false }.ToArgs());

        // SuffixTrie = null  → omitted
        Assert.Equal(
            ["title", "TEXT"],
            new TextField("title") { SuffixTrie = null }.ToArgs());
    }

    [Fact]
    public void ValidateFtCreateOffsetsEncoding()
    {
        // Offsets = true  → WITHOFFSETS
        Assert.Equal(
            ["WITHOFFSETS"],
            new FtCreateOptions { Offsets = true }.ToArgs());

        // Offsets = false → NOOFFSETS
        Assert.Equal(
            ["NOOFFSETS"],
            new FtCreateOptions { Offsets = false }.ToArgs());

        // Offsets = null  → omitted
        Assert.Equal(
            [],
            new FtCreateOptions { Offsets = null }.ToArgs());
    }

    [Fact]
    public void ValidateFtSearchSortByFieldNameOnly() =>
        // SortBy with field name only — no order, no sort keys
        Assert.Equal(
            ["FT.SEARCH", "idx", "*", "SORTBY", "price"],
            Request.FtSearch("idx", "*", new FtSearchOptions { SortBy = new FtSearchSortBy("price") }).GetArgs());

    [Fact]
    public void ValidateFtAggregateOptionsValidation() =>
        // LoadAll and LoadFields are mutually exclusive
        _ = Assert.Throws<ArgumentException>(() =>
            new FtAggregateOptions { LoadAll = true, LoadFields = ["@f1"] }.ToArgs());

    [Fact]
    public void ValidateFtAggregateOptionsBuilder_WithAllFields() =>
        Assert.Equal(
            ["FT.AGGREGATE", "idx", "*", "LOAD", "*"],
            Request.FtAggregate("idx", "*",
                new FtAggregateOptionsBuilder().WithAllFields().Build()).GetArgs());

    [Fact]
    public void ValidateFtAggregateOptionsBuilder_WithField() =>
        Assert.Equal(
            ["FT.AGGREGATE", "idx", "*", "LOAD", "1", "@title"],
            Request.FtAggregate("idx", "*",
                new FtAggregateOptionsBuilder().WithField("@title").Build()).GetArgs());

    [Fact]
    public void ValidateFtAggregateOptionsBuilder_WithFieldAccumulates() =>
        Assert.Equal(
            ["FT.AGGREGATE", "idx", "*", "LOAD", "2", "@f1", "@f2"],
            Request.FtAggregate("idx", "*",
                new FtAggregateOptionsBuilder().WithField("@f1").WithField("@f2").Build()).GetArgs());

    [Fact]
    public void ValidateFtAggregateOptionsBuilder_WithFieldsReplaces() =>
        Assert.Equal(
            ["FT.AGGREGATE", "idx", "*", "LOAD", "2", "@a", "@b"],
            Request.FtAggregate("idx", "*",
                new FtAggregateOptionsBuilder()
                    .WithField("@old")
                    .WithFields(["@a", "@b"])
                    .Build()).GetArgs());

    [Fact]
    public void ValidateFtAggregateOptionsBuilder_WithAllFieldsAfterWithField() =>
        Assert.Equal(
            ["FT.AGGREGATE", "idx", "*", "LOAD", "*"],
            Request.FtAggregate("idx", "*",
                new FtAggregateOptionsBuilder().WithField("@f1").WithAllFields().Build()).GetArgs());

    [Fact]
    public void ValidateFtAggregateOptionsBuilder_WithFieldAfterWithAllFields() =>
        Assert.Equal(
            ["FT.AGGREGATE", "idx", "*", "LOAD", "1", "@f1"],
            Request.FtAggregate("idx", "*",
                new FtAggregateOptionsBuilder().WithAllFields().WithField("@f1").Build()).GetArgs());

    [Fact]
    public void ValidateFtAggregateOptionsBuilder_WithParamAccumulates() =>
        Assert.Equal(
            ["FT.AGGREGATE", "idx", "*", "PARAMS", "4", "k1", "v1", "k2", "v2", "DIALECT", "2"],
            Request.FtAggregate("idx", "*",
                new FtAggregateOptionsBuilder()
                    .WithParam(new FtAggregateParam("k1", "v1"))
                    .WithParam(new FtAggregateParam("k2", "v2"))
                    .Dialect(2)
                    .Build()).GetArgs());

    [Fact]
    public void ValidateFtAggregateOptionsBuilder_WithParamsReplaces() =>
        Assert.Equal(
            ["FT.AGGREGATE", "idx", "*", "PARAMS", "2", "k2", "v2"],
            Request.FtAggregate("idx", "*",
                new FtAggregateOptionsBuilder()
                    .WithParam(new FtAggregateParam("k1", "v1"))
                    .WithParams([new FtAggregateParam("k2", "v2")])
                    .Build()).GetArgs());

    [Fact]
    public void ValidateFtAggregateOptionsBuilder_WithClauseAccumulates() =>
        Assert.Equal(
            ["FT.AGGREGATE", "idx", "*", "FILTER", "@score > 5", "LIMIT", "0", "10"],
            Request.FtAggregate("idx", "*",
                new FtAggregateOptionsBuilder()
                    .WithClause(new FtAggregateFilter("@score > 5"))
                    .WithClause(new FtLimit(0, 10))
                    .Build()).GetArgs());

    [Fact]
    public void ValidateFtAggregateOptionsBuilder_WithClausesReplaces() =>
        Assert.Equal(
            ["FT.AGGREGATE", "idx", "*", "LIMIT", "0", "5"],
            Request.FtAggregate("idx", "*",
                new FtAggregateOptionsBuilder()
                    .WithClause(new FtAggregateFilter("@score > 5"))
                    .WithClauses([new FtLimit(0, 5)])
                    .Build()).GetArgs());

    [Fact]
    public void ValidateFtAggregateOptionsBuilder_BuilderMatchesInitializer()
    {
        var fromBuilder = Request.FtAggregate("idx", "*",
            new FtAggregateOptionsBuilder()
                .WithField("@f1").WithField("@f2")
                .Verbatim()
                .Timeout(TimeSpan.FromSeconds(3))
                .WithParam(new FtAggregateParam("k", "v"))
                .Dialect(2)
                .GroupBy(["@condition"], new FtAggregateReducer(FtReducerFunction.Count) { Name = "count" })
                .SortBy([new FtAggregateSortProperty("@score", SortOrder.Descending)], max: 10)
                .Filter("@score > 5")
                .Limit(0, 10)
                .Build()).GetArgs();

        var fromInitializer = Request.FtAggregate("idx", "*",
            new FtAggregateOptions
            {
                LoadFields = ["@f1", "@f2"],
                Verbatim = true,
                Timeout = TimeSpan.FromSeconds(3),
                Params = [new FtAggregateParam("k", "v")],
                Dialect = 2,
                Clauses =
                [
                    new FtAggregateGroupBy("@condition") { Reducers = [new FtAggregateReducer(FtReducerFunction.Count) { Name = "count" }] },
                    new FtAggregateSortBy([new FtAggregateSortProperty("@score", SortOrder.Descending)], 10),
                    new FtAggregateFilter("@score > 5"),
                    new FtLimit(0, 10),
                ]
            }).GetArgs();

        Assert.Equal(fromInitializer, fromBuilder);
    }

    [Fact]
    public void ValidateFtAggregateOptionsBuilder_WhenTrue() =>
        Assert.Equal(
            ["FT.AGGREGATE", "idx", "*", "LOAD", "*"],
            Request.FtAggregate("idx", "*",
                new FtAggregateOptionsBuilder()
                    .When(true, b => b.WithAllFields())
                    .Build()).GetArgs());

    [Fact]
    public void ValidateFtAggregateOptionsBuilder_WhenFalse() =>
        Assert.Equal(
            ["FT.AGGREGATE", "idx", "*"],
            Request.FtAggregate("idx", "*",
                new FtAggregateOptionsBuilder()
                    .When(false, b => b.WithAllFields())
                    .Build()).GetArgs());
}
