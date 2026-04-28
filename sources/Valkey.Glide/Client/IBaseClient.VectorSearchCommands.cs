// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands.Options;

namespace Valkey.Glide;

/// <summary>
/// Vector Search (FT.*) commands for Valkey GLIDE clients.
/// </summary>
/// <seealso href="https://valkey.io/docs/topics/search/">valkey.io</seealso>
public partial interface IBaseClient
{
    /// <summary>
    /// Creates a new search index.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/ft.create/">valkey.io</seealso>
    /// <param name="indexName">The name of the index to create.</param>
    /// <param name="schema">Field definitions that describe the index schema. Must contain at least one field.</param>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.FtCreateAsync("my-index",
    ///     [new TextField("title"), new NumericField("published_at"), new TagField("category")]);
    /// </code>
    /// </example>
    /// </remarks>
    Task FtCreateAsync(ValkeyKey indexName, IEnumerable<IField> schema);

    /// <summary>
    /// Creates a new search index with additional options.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/ft.create/">valkey.io</seealso>
    /// <param name="indexName">The name of the index to create.</param>
    /// <param name="schema">Field definitions that describe the index schema. Must contain at least one field.</param>
    /// <param name="options">Index creation parameters.</param>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.FtCreateAsync("my-index",
    ///     [new TextField("title"), new NumericField("published_at"), new TagField("category")],
    ///     new FtCreateOptions { DataType = IndexDataType.Hash, Prefixes = ["blog:post:"] });
    /// </code>
    /// </example>
    /// </remarks>
    Task FtCreateAsync(ValkeyKey indexName, IEnumerable<IField> schema, FtCreateOptions options);

    /// <summary>
    /// Drops an existing search index.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/ft.dropindex/">valkey.io</seealso>
    /// <param name="indexName">The name of the index to drop.</param>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.FtDropIndexAsync("my-index");
    /// </code>
    /// </example>
    /// </remarks>
    Task FtDropIndexAsync(ValkeyKey indexName);

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
    /// <returns>The search result containing total count and document data.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// FtSearchResult result = await client.FtSearchAsync("my-index", "*");
    /// Console.WriteLine(result.TotalResults);
    /// </code>
    /// </example>
    /// </remarks>
    Task<FtSearchResult> FtSearchAsync(ValkeyKey indexName, ValkeyValue query);

    /// <summary>
    /// Executes a search query against an index with additional options.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/ft.search/">valkey.io</seealso>
    /// <param name="indexName">The name of the index to search.</param>
    /// <param name="query">The search query string.</param>
    /// <param name="options">Search parameters.</param>
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
    Task<FtSearchResult> FtSearchAsync(ValkeyKey indexName, ValkeyValue query, FtSearchOptions options);

    /// <summary>
    /// Runs an aggregation pipeline against an index.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/ft.aggregate/">valkey.io</seealso>
    /// <param name="indexName">The name of the index to aggregate.</param>
    /// <param name="query">The filter query string.</param>
    /// <returns>
    /// An array of result rows. Each row's field values are typed as <see cref="object"/>
    /// because the glide-core layer does not coerce FT.AGGREGATE values, so the actual
    /// runtime type depends on the server and RESP protocol version.
    /// </returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// FtAggregateRow[] rows = await client.FtAggregateAsync("my-index", "*");
    /// </code>
    /// </example>
    /// </remarks>
    Task<FtAggregateRow[]> FtAggregateAsync(ValkeyKey indexName, ValkeyValue query);

    /// <summary>
    /// Runs an aggregation pipeline against an index with additional options.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/ft.aggregate/">valkey.io</seealso>
    /// <param name="indexName">The name of the index to aggregate.</param>
    /// <param name="query">The filter query string.</param>
    /// <param name="options">Aggregation parameters.</param>
    /// <returns>
    /// An array of result rows. Each row's field values are typed as <see cref="object"/>
    /// because the glide-core layer does not coerce FT.AGGREGATE values, so the actual
    /// runtime type depends on the server and RESP protocol version.
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
    ///                 Reducers = [new FtAggregateReducer(FtReducerFunction.Count) { Name = "count" }]
    ///             }
    ///         ]
    ///     });
    /// foreach (var row in rows)
    ///     Console.WriteLine($"{row["condition"]}: {row["count"]}");
    /// </code>
    /// </example>
    /// </remarks>
    Task<FtAggregateRow[]> FtAggregateAsync(ValkeyKey indexName, ValkeyValue query, FtAggregateOptions options);

    /// <summary>
    /// Returns information and statistics about an index.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/ft.info/">valkey.io</seealso>
    /// <param name="indexName">The name of the index to inspect.</param>
    /// <returns>A dictionary of field names to their values describing the index.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// Dictionary&lt;string, object&gt; info = await client.FtInfoAsync("my-index");
    /// Console.WriteLine(info["index_name"]); // Output: my-index
    /// </code>
    /// </example>
    /// </remarks>
    Task<Dictionary<string, object>> FtInfoAsync(ValkeyKey indexName);

    /// <summary>
    /// Returns information and statistics about an index with additional options.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/ft.info/">valkey.io</seealso>
    /// <param name="indexName">The name of the index to inspect.</param>
    /// <param name="options">Info parameters.</param>
    /// <returns>A dictionary of field names to their values describing the index.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// Dictionary&lt;string, object&gt; info = await client.FtInfoAsync("my-index",
    ///     new FtInfoOptions { Scope = FtInfoScope.Local });
    /// Console.WriteLine(info["index_name"]); // Output: my-index
    /// </code>
    /// </example>
    /// </remarks>
    Task<Dictionary<string, object>> FtInfoAsync(ValkeyKey indexName, FtInfoOptions options);

    /// <summary>
    /// Adds an alias to an existing index.
    /// TODO: Once valkey-search 1.3 commands come out, verify there's support for byte arrays as aliases (ValkeyKey)
    /// </summary>
    /// <seealso href="https://valkey.io/commands/ft.aliasadd/">valkey.io</seealso>
    /// <param name="alias">The alias name to add.</param>
    /// <param name="indexName">The index to associate the alias with.</param>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.FtAliasAddAsync("my-alias", "my-index");
    /// </code>
    /// </example>
    /// </remarks>
    Task FtAliasAddAsync(string alias, ValkeyKey indexName);

    /// <summary>
    /// Removes an alias from an index.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/ft.aliasdel/">valkey.io</seealso>
    /// <param name="alias">The alias name to remove.</param>
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
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.FtAliasUpdateAsync("my-alias", "my-index-v2");
    /// </code>
    /// </example>
    /// </remarks>
    Task FtAliasUpdateAsync(string alias, ValkeyKey indexName);

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
