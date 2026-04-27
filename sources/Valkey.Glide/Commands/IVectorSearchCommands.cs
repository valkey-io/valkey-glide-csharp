// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands.Options;

namespace Valkey.Glide.Commands;

/// <summary>
/// Supports commands for the "Vector Search" (FT.*) command group for standalone and cluster clients.
/// <br />
/// See more on <see href="https://valkey.io/docs/topics/search/">valkey.io</see>.
/// </summary>
public interface IVectorSearchCommands
{
    /// <summary>
    /// Creates a new search index.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/ft.create/">valkey.io</seealso>
    /// <param name="indexName">The name of the index to create.</param>
    /// <param name="schema">Field definitions that describe the index schema. Must contain at least one field.</param>
    /// <param name="options">Optional index creation parameters.</param>
    /// <returns>A task that completes when the index is created.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.FtCreateAsync("my-index",
    ///     [new TextField("title"), new NumericField("published_at"), new TagField("category")],
    ///     new FtCreateOptions { DataType = IndexDataType.HASH, Prefixes = ["blog:post:"] });
    /// </code>
    /// </example>
    /// </remarks>
    Task FtCreateAsync(string indexName, IEnumerable<IField> schema, FtCreateOptions? options = null);

    /// <summary>
    /// Drops an existing search index.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/ft.dropindex/">valkey.io</seealso>
    /// <param name="indexName">The name of the index to drop.</param>
    /// <returns>A task that completes when the index is dropped.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.FtDropIndexAsync("my-index");
    /// </code>
    /// </example>
    /// </remarks>
    Task FtDropIndexAsync(string indexName);

    /// <summary>
    /// Returns a list of all existing index names.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/ft._list/">valkey.io</seealso>
    /// <returns>An unordered set of index names.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// ISet&lt;string&gt; indexes = await client.FtListAsync();
    /// foreach (string idx in indexes)
    ///     Console.WriteLine(idx);
    /// </code>
    /// </example>
    /// </remarks>
    Task<ISet<string>> FtListAsync();

    /// <summary>
    /// Executes a search query against an index.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/ft.search/">valkey.io</seealso>
    /// <param name="indexName">The name of the index to search.</param>
    /// <param name="query">The search query string.</param>
    /// <param name="options">Optional search parameters.</param>
    /// <returns>The search result containing total count and document data.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// byte[] queryVec = new byte[8];
    /// FtSearchResult knn = await client.FtSearchAsync("vec-index", "*=>[KNN 2 @VEC $query_vec]",
    ///     new FtSearchOptions
    ///     {
    ///         Params = [new FtSearchParam("query_vec", System.Text.Encoding.Latin1.GetString(queryVec))],
    ///         ReturnFields = [new FtSearchReturnField("vec")],
    ///     });
    /// Console.WriteLine(knn.TotalResults); // Output: 2
    /// </code>
    /// </example>
    /// </remarks>
    Task<FtSearchResult> FtSearchAsync(string indexName, string query, FtSearchOptions? options = null);

    /// <summary>
    /// Runs an aggregation pipeline against an index.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/ft.aggregate/">valkey.io</seealso>
    /// <param name="indexName">The name of the index to aggregate.</param>
    /// <param name="query">The filter query string.</param>
    /// <param name="options">Optional aggregation parameters.</param>
    /// <returns>
    /// An array of result rows. Each row's field values are typed as <see cref="object"/>:
    /// fields loaded from documents are <see cref="string"/>, while reducer outputs
    /// (e.g. COUNT, AVG, SUM) are <see cref="double"/>.
    /// </returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// // Group by a field and count results per group
    /// FtAggregateRow[] rows = await client.FtAggregateAsync("my-index", "*",
    ///     new FtAggregateOptions
    ///     {
    ///         LoadFields = ["__key"],
    ///         Clauses =
    ///         [
    ///             new FtAggregateGroupBy("@condition")
    ///             {
    ///                 Reducers = [new FtAggregateReducer(FtReducerFunction.COUNT) { Name = "count" }]
    ///             }
    ///         ]
    ///     });
    /// foreach (var row in rows)
    ///     Console.WriteLine($"{row["condition"]}: {row["count"]}");
    /// </code>
    /// </example>
    /// </remarks>
    Task<FtAggregateRow[]> FtAggregateAsync(string indexName, string query, FtAggregateOptions? options = null);

    /// <summary>
    /// Returns information and statistics about an index.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/ft.info/">valkey.io</seealso>
    /// <param name="indexName">The name of the index to inspect.</param>
    /// <param name="options">Optional info parameters.</param>
    /// <returns>A dictionary of field names to their values describing the index.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// Dictionary&lt;string, object&gt; localInfo = await client.FtInfoAsync("my-index",
    ///     new FtInfoOptions
    ///     {
    ///         Scope = FtInfoScope.LOCAL,
    ///         ShardScope = FtInfoShardScope.ALLSHARDS,
    ///         Consistency = FtInfoConsistencyMode.CONSISTENT,
    ///     });
    /// Console.WriteLine(localInfo["index_name"]); // Output: my-index
    /// </code>
    /// </example>
    /// </remarks>
    Task<Dictionary<string, object>> FtInfoAsync(string indexName, FtInfoOptions? options = null);

    /// <summary>
    /// Adds an alias to an existing index.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/ft.aliasadd/">valkey.io</seealso>
    /// <param name="alias">The alias name to add.</param>
    /// <param name="indexName">The index to associate the alias with.</param>
    /// <returns>A task that completes when the alias is added.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.FtAliasAddAsync("my-alias", "my-index");
    /// </code>
    /// </example>
    /// </remarks>
    Task FtAliasAddAsync(string alias, string indexName);

    /// <summary>
    /// Removes an alias from an index.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/ft.aliasdel/">valkey.io</seealso>
    /// <param name="alias">The alias name to remove.</param>
    /// <returns>A task that completes when the alias is removed.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.FtAliasDelAsync("my-alias");
    /// </code>
    /// </example>
    /// </remarks>
    Task FtAliasDelAsync(string alias);

    /// <summary>
    /// Updates an existing alias to point to a different index.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/ft.aliasupdate/">valkey.io</seealso>
    /// <param name="alias">The alias name to update.</param>
    /// <param name="indexName">The new index to associate the alias with.</param>
    /// <returns>A task that completes when the alias is updated.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.FtAliasUpdateAsync("my-alias", "my-index-v2");
    /// </code>
    /// </example>
    /// </remarks>
    Task FtAliasUpdateAsync(string alias, string indexName);

    /// <summary>
    /// Returns a map of all aliases to their associated index names.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/ft._aliaslist/">valkey.io</seealso>
    /// <returns>A dictionary where keys are alias names and values are index names.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// Dictionary&lt;string, string&gt; aliases = await client.FtAliasListAsync();
    /// foreach (var (alias, index) in aliases)
    ///     Console.WriteLine($"{alias} -> {index}");
    /// </code>
    /// </example>
    /// </remarks>
    Task<Dictionary<string, string>> FtAliasListAsync();
}
