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
    /// <param name="schema">Field definitions that describe the index schema.</param>
    /// <param name="options">Optional index creation parameters.</param>
    /// <returns>"OK" on success.</returns>
    Task<string> FtCreateAsync(string indexName, IField[] schema, FtCreateOptions? options = null);

    /// <summary>
    /// Drops an existing search index.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/ft.dropindex/">valkey.io</seealso>
    /// <param name="indexName">The name of the index to drop.</param>
    /// <returns>"OK" on success.</returns>
    Task<string> FtDropIndexAsync(string indexName);

    /// <summary>
    /// Returns a list of all existing index names.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/ft._list/">valkey.io</seealso>
    /// <returns>A string array of index names.</returns>
    Task<string[]> FtListAsync();

    /// <summary>
    /// Executes a search query against an index.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/ft.search/">valkey.io</seealso>
    /// <param name="indexName">The name of the index to search.</param>
    /// <param name="query">The search query string.</param>
    /// <param name="options">Optional search parameters.</param>
    /// <returns>The search result containing total count and document data.</returns>
    Task<FtSearchResult> FtSearchAsync(string indexName, string query, FtSearchOptions? options = null);

    /// <summary>
    /// Runs an aggregation pipeline against an index.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/ft.aggregate/">valkey.io</seealso>
    /// <param name="indexName">The name of the index to aggregate.</param>
    /// <param name="query">The filter query string.</param>
    /// <param name="options">Optional aggregation parameters.</param>
    /// <returns>An array of result row dictionaries.</returns>
    Task<Dictionary<string, object?>[]> FtAggregateAsync(string indexName, string query, FtAggregateOptions? options = null);

    /// <summary>
    /// Returns information and statistics about an index.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/ft.info/">valkey.io</seealso>
    /// <param name="indexName">The name of the index to inspect.</param>
    /// <param name="options">Optional info parameters.</param>
    /// <returns>A dictionary of field names to their values describing the index.</returns>
    Task<Dictionary<string, object?>> FtInfoAsync(string indexName, FtInfoOptions? options = null);

    /// <summary>
    /// Adds an alias to an existing index.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/ft.aliasadd/">valkey.io</seealso>
    /// <param name="alias">The alias name to add.</param>
    /// <param name="indexName">The index to associate the alias with.</param>
    /// <returns>"OK" on success.</returns>
    Task<string> FtAliasAddAsync(string alias, string indexName);

    /// <summary>
    /// Removes an alias from an index.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/ft.aliasdel/">valkey.io</seealso>
    /// <param name="alias">The alias name to remove.</param>
    /// <returns>"OK" on success.</returns>
    Task<string> FtAliasDelAsync(string alias);

    /// <summary>
    /// Updates an existing alias to point to a different index.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/ft.aliasupdate/">valkey.io</seealso>
    /// <param name="alias">The alias name to update.</param>
    /// <param name="indexName">The new index to associate the alias with.</param>
    /// <returns>"OK" on success.</returns>
    Task<string> FtAliasUpdateAsync(string alias, string indexName);

    /// <summary>
    /// Returns a map of all aliases to their associated index names.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/ft._aliaslist/">valkey.io</seealso>
    /// <returns>A dictionary where keys are alias names and values are index names.</returns>
    Task<Dictionary<string, string>> FtAliasListAsync();
}
