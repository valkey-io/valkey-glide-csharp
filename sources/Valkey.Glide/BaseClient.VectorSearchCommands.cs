// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands;
using Valkey.Glide.Commands.Options;
using Valkey.Glide.Internals;

namespace Valkey.Glide;

public abstract partial class BaseClient : IVectorSearchCommands
{
    /// <inheritdoc/>
    public async Task<string> FtCreateAsync(string indexName, IField[] schema, FtCreateOptions? options = null)
        => await Command(Request.FtCreate(indexName, schema, options));

    /// <inheritdoc/>
    public async Task<string> FtDropIndexAsync(string indexName)
        => await Command(Request.FtDropIndex(indexName));

    /// <inheritdoc/>
    public async Task<string[]> FtListAsync()
        => await Command(Request.FtList());

    /// <inheritdoc/>
    public async Task<FtSearchResult> FtSearchAsync(string indexName, string query, FtSearchOptions? options = null)
        => await Command(Request.FtSearch(indexName, query, options));

    /// <inheritdoc/>
    public async Task<Dictionary<string, object?>[]> FtAggregateAsync(string indexName, string query, FtAggregateOptions? options = null)
        => await Command(Request.FtAggregate(indexName, query, options));

    /// <inheritdoc/>
    public async Task<Dictionary<string, object?>> FtInfoAsync(string indexName, FtInfoOptions? options = null)
        => await Command(Request.FtInfo(indexName, options));

    /// <inheritdoc/>
    public async Task<string> FtAliasAddAsync(string alias, string indexName)
        => await Command(Request.FtAliasAdd(alias, indexName));

    /// <inheritdoc/>
    public async Task<string> FtAliasDelAsync(string alias)
        => await Command(Request.FtAliasDel(alias));

    /// <inheritdoc/>
    public async Task<string> FtAliasUpdateAsync(string alias, string indexName)
        => await Command(Request.FtAliasUpdate(alias, indexName));

    /// <inheritdoc/>
    public async Task<Dictionary<string, string>> FtAliasListAsync()
        => await Command(Request.FtAliasList());
}
