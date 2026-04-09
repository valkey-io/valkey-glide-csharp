// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.Commands.Options;

/// <summary>
/// Options for the SORT and SORT_RO commands.
/// </summary>
/// <remarks>
/// See <a href="https://valkey.io/commands/sort/">valkey.io</a>
/// </remarks>
public class SortOptions
{
    /// <summary>
    /// The number of elements to skip (LIMIT offset).
    /// </summary>
    public long Skip { get; set; }

    /// <summary>
    /// The number of elements to take (LIMIT count). -1 means take all.
    /// </summary>
    public long Take { get; set; } = -1;

    /// <summary>
    /// The sort order (ASC or DESC).
    /// </summary>
    public Order Order { get; set; } = Order.Ascending;

    /// <summary>
    /// The sort type (Numeric or Alphabetic).
    /// </summary>
    public SortType SortType { get; set; } = SortType.Numeric;

    /// <summary>
    /// A pattern to sort by external keys instead of by the elements stored at the key themselves.
    /// </summary>
    /// <remarks>
    /// Supported in cluster mode since Valkey version 8.0 and above.
    /// </remarks>
    public ValkeyValue By { get; set; }

    /// <summary>
    /// Patterns to retrieve external keys' values.
    /// </summary>
    /// <remarks>
    /// Supported in cluster mode since Valkey version 8.0 and above.
    /// </remarks>
    public IEnumerable<ValkeyValue>? Get { get; set; }
}
