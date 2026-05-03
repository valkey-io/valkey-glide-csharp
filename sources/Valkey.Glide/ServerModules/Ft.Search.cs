// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Internals;

namespace Valkey.Glide.ServerModules;

public static partial class Ft
{
    #region Public Methods

    /// <summary>
    /// Searches an index for documents matching a query.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/ft.search/">Valkey commands – FT.SEARCH</seealso>
    /// <param name="client">The client to execute the command.</param>
    /// <param name="index">The name of the index to search.</param>
    /// <param name="query">The search query expression.</param>
    /// <returns>A <see cref="SearchResult"/> for the query.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await Ft.CreateAsync(client, "index",
    ///     new Ft.CreateTextField("title"),
    ///     new Ft.CreateOptions { Prefixes = ["doc:"] });
    /// await client.HashSetAsync("doc:1", [new("title", "hello world")]);
    /// await client.HashSetAsync("doc:2", [new("title", "goodbye")]);
    ///
    /// var result = await Ft.SearchAsync(client, "index", "*");
    /// Console.WriteLine($"Total: {result.TotalResults}");  // 2
    /// </code>
    /// </example>
    /// </remarks>
    public static Task<SearchResult> SearchAsync(BaseClient client, ValkeyKey index, ValkeyValue query)
        => client.Command(Request.FtSearch(index, query));

    /// <summary>
    /// Searches an index for documents matching a query.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/ft.search/">Valkey commands – FT.SEARCH</seealso>
    /// <param name="client">The client to execute the command.</param>
    /// <param name="index">The name of the index to search.</param>
    /// <param name="query">The search query expression.</param>
    /// <param name="options">Additional options for the search command.</param>
    /// <returns>A <see cref="SearchResult"/> for the query.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await Ft.CreateAsync(client, "index",
    ///     new Ft.CreateField[] { new Ft.CreateTextField("title"), new Ft.CreateNumericField("price") },
    ///     new Ft.CreateOptions { Prefixes = ["item:"] });
    /// await client.HashSetAsync("item:1", [new("title", "hello"), new("price", "10")]);
    /// await client.HashSetAsync("item:2", [new("title", "world"), new("price", "20")]);
    ///
    /// var result = await Ft.SearchAsync(client, "index", "*",
    ///     new Ft.SearchOptions
    ///     {
    ///         Limit = new Ft.SearchLimit { Count = 10 },
    ///         SortBy = new Ft.SearchSortBy { Field = "price", Order = SortOrder.Ascending },
    ///         Return = new Ft.SearchReturnField[] { "title", "price" },
    ///     });
    /// Console.WriteLine($"Total: {result.TotalResults}");  // 2
    /// </code>
    /// </example>
    /// </remarks>
    public static Task<SearchResult> SearchAsync(BaseClient client, ValkeyKey index, ValkeyValue query, SearchOptions options)
        => client.Command(Request.FtSearch(index, query, options));

    #endregion

    #region Nested Types

    /// <summary>
    /// The options for a search operation.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/ft.search/">Valkey commands – FT.SEARCH</seealso>
    public sealed class SearchOptions
    {
        #region Constants
        
        /// <summary>
        /// Requests document IDs without field content (<c>NOCONTENT</c>).
        /// </summary>
        public static readonly IEnumerable<SearchReturnField> NoContent = [];

        #endregion
        #region Public Properties

        /// <summary>
        /// Whether to allow partial results from available shards (<c>SOMESHARDS</c>).
        /// </summary>
        public bool SomeShards { get; init; }

        /// <summary>
        /// Whether to allow inconsistent results across shards (<c>INCONSISTENT</c>).
        /// </summary>
        public bool Inconsistent { get; init; }

        /// <summary>
        /// Whether proximity matching of text terms must be in order (<c>INORDER</c>).
        /// </summary>
        public bool InOrder { get; init; }

        /// <summary>
        /// Pagination offset and count, or <see langword="null"/> for server default (<c>LIMIT</c>).
        /// </summary>
        public SearchLimit? Limit { get; init; }

        /// <summary>
        /// Fields to return, <see cref="NoContent"/> for IDs only,
        /// or <see langword="null"/> for all fields (<c>RETURN</c>/<c>NOCONTENT</c>).
        /// </summary>
        public IEnumerable<SearchReturnField>? Return { get; init; }

        /// <summary>
        /// Query parameter key/value pairs (<c>PARAMS</c>).
        /// </summary>
        public IDictionary<ValkeyValue, ValkeyValue> Params { get; init; } = new Dictionary<ValkeyValue, ValkeyValue>();

        /// <summary>
        /// The slop value for proximity matching,
        /// or <see langword="null"/> for server default (<c>SLOP</c>).
        /// </summary>
        public long? Slop { get; init; }

        /// <summary>
        /// Sort options for the search results, or <see langword="null"/> for unsorted (<c>SORTBY</c>).
        /// </summary>
        public SearchSortBy? SortBy { get; init; }

        /// <summary>
        /// Module timeout override, or <see langword="null"/> for server default (<c>TIMEOUT</c>).
        /// </summary>
        public TimeSpan? Timeout { get; init; }

        /// <summary>
        /// Whether to disable stemming on query terms (<c>VERBATIM</c>).
        /// </summary>
        public bool Verbatim { get; init; }

        #endregion
    }

    /// <summary>
    /// Limit options for a search operation.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/ft.search/">Valkey commands – FT.SEARCH</seealso>
    public sealed class SearchLimit
    {
        /// <summary>
        /// Number of results to return.
        /// </summary>
        public required long Count { get; init; }

        /// <summary>
        /// Number of results to skip.
        /// </summary>
        public long Offset { get; init; } = 0;
    }

    /// <summary>
    /// A field to include in the results from a search operation.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/ft.search/">Valkey commands – FT.SEARCH</seealso>
    public sealed class SearchReturnField
    {
        /// <summary>
        /// The field name to return.
        /// </summary>
        public required ValkeyValue Field { get; init; }

        /// <summary>
        /// An optional alias for the field in results (<c>AS</c>).
        /// </summary>
        public ValkeyValue Name { get; init; }

        /// <summary>
        /// Converts a <see cref="string"/> to a <see cref="SearchReturnField"/>.
        /// </summary>
        /// <param name="field">The field name.</param>
        public static implicit operator SearchReturnField(string field)
            => new() { Field = field };

        /// <summary>
        /// Converts a <see cref="ValkeyValue"/> to a <see cref="SearchReturnField"/>.
        /// </summary>
        /// <param name="field">The field value.</param>
        public static implicit operator SearchReturnField(ValkeyValue field)
            => new() { Field = field };
    }

    /// <summary>
    /// Sort options for results from a search operation.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/ft.search/">Valkey commands – FT.SEARCH</seealso>
    public sealed class SearchSortBy
    {
        /// <summary>
        /// The field name to sort by (<c>SORTBY</c>).
        /// </summary>
        public required ValkeyValue Field { get; init; }

        /// <summary>
        /// The sort direction (<c>ASC</c>/<c>DESC</c>).
        /// </summary>
        public SortOrder Order { get; init; } = SortOrder.Default;

        /// <summary>
        /// Whether to include the sort key value in each result (<c>WITHSORTKEYS</c>).
        /// </summary>
        public bool WithSortKeys { get; init; }

        /// <summary>
        /// Converts a <see cref="string"/> to a <see cref="SearchSortBy"/>.
        /// </summary>
        /// <param name="field">The field name.</param>
        public static implicit operator SearchSortBy(string field)
            => new() { Field = field };

        /// <summary>
        /// Converts a <see cref="ValkeyValue"/> to a <see cref="SearchSortBy"/>.
        /// </summary>
        /// <param name="field">The field value.</param>
        public static implicit operator SearchSortBy(ValkeyValue field)
            => new() { Field = field };
    }

    /// <summary>
    /// Results from a search operation.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/ft.search/">Valkey commands – FT.SEARCH</seealso>
    public sealed class SearchResult
    {
        /// <summary>
        /// The total number of documents matching the query across the entire index.
        /// </summary>
        public long TotalResults { get; }

        /// <summary>
        /// Search result documents.
        /// </summary>
        public SearchDocument[] Documents { get; }

        internal SearchResult(long totalResults, SearchDocument[] documents)
        {
            TotalResults = totalResults;
            Documents = documents;
        }
    }

    /// <summary>
    /// A document returned from a search operation.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/ft.search/">Valkey commands – FT.SEARCH</seealso>
    public sealed class SearchDocument
    {
        /// <summary>
        /// The document key (e.g. the Valkey key name).
        /// </summary>
        public ValkeyKey Key { get; }

        /// <summary>
        /// The document's field data as key-value pairs.
        /// </summary>
        public IDictionary<ValkeyValue, ValkeyValue> Fields { get; }

        /// <summary>
        /// The sort key value returned when <c>WITHSORTKEYS</c> is specifed,
        /// or <see cref="ValkeyValue.Null"/>.
        /// </summary>
        public ValkeyValue SortKey { get; }

        internal SearchDocument(
            ValkeyKey key,
            IDictionary<ValkeyValue, ValkeyValue> fields,
            ValkeyValue sortKey = default)
        {
            Key = key;
            Fields = fields;
            SortKey = sortKey;
        }
    }

    #endregion
}
