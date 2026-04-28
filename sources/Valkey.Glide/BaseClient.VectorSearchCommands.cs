// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands;
using Valkey.Glide.Commands.Options;
using Valkey.Glide.Internals;

namespace Valkey.Glide;

public abstract partial class BaseClient : IVectorSearchCommands
{
    /// <inheritdoc/>
    public async Task FtCreateAsync(ValkeyKey indexName, IEnumerable<IField> schema)
        => await Command(Request.FtCreate(indexName, schema, null));

    /// <inheritdoc/>
    public async Task FtCreateAsync(ValkeyKey indexName, IEnumerable<IField> schema, FtCreateOptions options)
        => await Command(Request.FtCreate(indexName, schema, options));

    /// <inheritdoc/>
    public async Task FtDropIndexAsync(ValkeyKey indexName)
        => await Command(Request.FtDropIndex(indexName));

    /// <inheritdoc/>
    public async Task<ISet<string>> FtListAsync()
        => await Command(Request.FtList());

    /// <inheritdoc/>
    public async Task<FtSearchResult> FtSearchAsync(ValkeyKey indexName, ValkeyValue query)
        => await Command(Request.FtSearch(indexName, query, null));

    /// <inheritdoc/>
    public async Task<FtSearchResult> FtSearchAsync(ValkeyKey indexName, ValkeyValue query, FtSearchOptions options)
        => await Command(Request.FtSearch(indexName, query, options));

    /// <inheritdoc/>
    public async Task<FtAggregateRow[]> FtAggregateAsync(ValkeyKey indexName, ValkeyValue query)
        => await Command(Request.FtAggregate(indexName, query, null));

    /// <inheritdoc/>
    public async Task<FtAggregateRow[]> FtAggregateAsync(ValkeyKey indexName, ValkeyValue query, FtAggregateOptions options)
        => await Command(Request.FtAggregate(indexName, query, options));

    /// <inheritdoc/>
    public async Task<Dictionary<string, object>> FtInfoAsync(ValkeyKey indexName)
        => await Command(Request.FtInfo(indexName, null));

    /// <inheritdoc/>
    public async Task<Dictionary<string, object>> FtInfoAsync(ValkeyKey indexName, FtInfoOptions options)
        => await Command(Request.FtInfo(indexName, options));

    /// <inheritdoc/>
    public async Task FtAliasAddAsync(string alias, ValkeyKey indexName)
        => await Command(Request.FtAliasAdd(alias, indexName));

    /// <inheritdoc/>
    public async Task FtAliasDelAsync(string alias)
        => await Command(Request.FtAliasDel(alias));

    /// <inheritdoc/>
    public async Task FtAliasUpdateAsync(string alias, ValkeyKey indexName)
        => await Command(Request.FtAliasUpdate(alias, indexName));

    /// <inheritdoc/>
    public async Task<Dictionary<string, string>> FtAliasListAsync()
        => await Command(Request.FtAliasList());
}
