// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands.Options;
using Valkey.Glide.Internals;

namespace Valkey.Glide;

public abstract partial class BaseClient
{
    /// <inheritdoc/>
    public Task FtCreateAsync(ValkeyKey indexName, IEnumerable<IField> schema)
        => Command(Request.FtCreate(indexName, schema, null));

    /// <inheritdoc/>
    public Task FtCreateAsync(ValkeyKey indexName, IEnumerable<IField> schema, FtCreateOptions options)
        => Command(Request.FtCreate(indexName, schema, options));

    /// <inheritdoc/>
    public Task FtDropIndexAsync(ValkeyKey indexName)
        => Command(Request.FtDropIndex(indexName));

    /// <inheritdoc/>
    public Task<ISet<string>> FtListAsync()
        => Command(Request.FtList());

    /// <inheritdoc/>
    public Task<FtSearchResult> FtSearchAsync(ValkeyKey indexName, ValkeyValue query)
        => Command(Request.FtSearch(indexName, query, null));

    /// <inheritdoc/>
    public Task<FtSearchResult> FtSearchAsync(ValkeyKey indexName, ValkeyValue query, FtSearchOptions options)
        => Command(Request.FtSearch(indexName, query, options));

    /// <inheritdoc/>
    public Task<FtAggregateRow[]> FtAggregateAsync(ValkeyKey indexName, ValkeyValue query)
        => Command(Request.FtAggregate(indexName, query, null));

    /// <inheritdoc/>
    public Task<FtAggregateRow[]> FtAggregateAsync(ValkeyKey indexName, ValkeyValue query, FtAggregateOptions options)
        => Command(Request.FtAggregate(indexName, query, options));

    /// <inheritdoc/>
    public Task<Dictionary<string, object>> FtInfoAsync(ValkeyKey indexName)
        => Command(Request.FtInfo(indexName, null));

    /// <inheritdoc/>
    public Task<Dictionary<string, object>> FtInfoAsync(ValkeyKey indexName, FtInfoOptions options)
        => Command(Request.FtInfo(indexName, options));

}
