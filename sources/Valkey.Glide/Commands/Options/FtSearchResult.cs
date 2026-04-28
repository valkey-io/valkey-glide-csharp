// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.Commands.Options;

/// <summary>
/// Represents a single document returned by FT.SEARCH.
/// The order of documents in <see cref="FtSearchResult.Documents"/> matches the order
/// returned by the server, which is significant when SORTBY is used.
/// </summary>
/// <seealso href="https://valkey.io/commands/ft.search/">valkey.io</seealso>
public sealed class FtSearchDocument
{
    internal FtSearchDocument(ValkeyKey key, Dictionary<string, ValkeyValue> fields, string? sortKey = null)
    {
        Key = key;
        Fields = fields;
        SortKey = sortKey;
    }

    /// <summary>
    /// The document key (e.g. the Valkey key name).
    /// </summary>
    public ValkeyKey Key { get; }

    /// <summary>
    /// The document's field data as key-value pairs.
    /// Field names are always strings (schema-defined). Field values are <see cref="ValkeyValue"/>
    /// to preserve binary data (e.g. raw vector bytes returned via RETURN).
    /// Empty when NOCONTENT is used.
    /// </summary>
    public Dictionary<string, ValkeyValue> Fields { get; }

    /// <summary>
    /// The sort key value returned when WITHSORTKEYS is used.
    /// <see langword="null"/> when WITHSORTKEYS is not requested.
    /// </summary>
    public string? SortKey { get; }
}

/// <summary>
/// Holds the parsed response from an FT.SEARCH command.
/// Documents are returned as an ordered list that preserves the server's
/// iteration order, which matters when SORTBY is specified.
/// </summary>
/// <seealso href="https://valkey.io/commands/ft.search/">valkey.io</seealso>
public sealed class FtSearchResult
{
    internal FtSearchResult(long totalResults, List<FtSearchDocument> documents)
    {
        TotalResults = totalResults;
        Documents = documents;
    }

    /// <summary>
    /// The total number of documents matching the query across the entire index,
    /// regardless of any LIMIT applied to this response.
    /// </summary>
    public long TotalResults { get; }

    /// <summary>
    /// An ordered list of search result documents.
    /// The list preserves the order returned by the server.
    /// </summary>
    public List<FtSearchDocument> Documents { get; }
}
