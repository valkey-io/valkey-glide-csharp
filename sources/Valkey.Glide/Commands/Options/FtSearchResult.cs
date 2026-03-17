// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.Commands.Options;

/// <summary>
/// Represents a single document returned by FT.SEARCH.
/// The order of documents in <see cref="FtSearchResult.Documents"/> matches the order
/// returned by the server, which is significant when SORTBY is used.
/// </summary>
/// <seealso href="https://valkey.io/commands/ft.search/">valkey.io</seealso>
public class FtSearchDocument(string key, Dictionary<string, object?> fields, string sortKey = "")
{
    /// <summary>The document key (e.g. the Valkey key name).</summary>
    public string Key { get; } = key;

    /// <summary>
    /// The document's field data as key-value pairs.
    /// Empty when NOCONTENT is used.
    /// </summary>
    public Dictionary<string, object?> Fields { get; } = fields;

    /// <summary>
    /// The sort key value returned when WITHSORTKEYS is used.
    /// Empty string when WITHSORTKEYS is not requested.
    /// </summary>
    public string SortKey { get; } = sortKey;
}

/// <summary>
/// Holds the parsed response from an FT.SEARCH command.
/// Documents are returned as an ordered list that preserves the server's
/// iteration order, which matters when SORTBY is specified.
/// </summary>
/// <seealso href="https://valkey.io/commands/ft.search/">valkey.io</seealso>
public class FtSearchResult(long totalResults, List<FtSearchDocument> documents)
{
    /// <summary>The total number of documents matching the query.</summary>
    public long TotalResults { get; } = totalResults;

    /// <summary>
    /// An ordered list of search result documents.
    /// The list preserves the order returned by the server.
    /// </summary>
    public List<FtSearchDocument> Documents { get; } = documents;
}
