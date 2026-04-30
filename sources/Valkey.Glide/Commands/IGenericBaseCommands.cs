// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.Commands;

// ATTENTION: Methods should only be added to this interface if they are implemented
// by both Valkey GLIDE clients and StackExchange.Redis databases with the same signature.
// Methods with different naming conventions between GLIDE and SER should NOT be here:
// - GLIDE-style methods (DeleteAsync, ExistsAsync, etc.) go in IBaseClient.GenericCommands.cs
// - SER-style methods (KeyDeleteAsync, KeyExistsAsync, etc.) go in IDatabaseAsync.GenericCommands.cs

/// <summary>
/// Generic commands shared between Valkey GLIDE clients and StackExchange.Redis-compatible databases.
/// </summary>
/// <remarks>
/// This interface contains only methods where the naming and signature match between both APIs.
/// For GLIDE-style methods without "Key" prefix, see <see cref="IBaseClient"/>.
/// For SER-style methods with "Key" prefix, see <see cref="IDatabaseAsync"/>.
/// </remarks>
/// <seealso href="https://valkey.io/commands/#generic">Valkey – Generic Commands</seealso>
public interface IGenericBaseCommands
{
    /// <summary>
    /// Sorts the elements in a list, set, or sorted set and returns the result.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/sort/">Valkey commands – SORT</seealso>
    /// <param name="key">The key of the list, set, or sorted set to be sorted.</param>
    /// <param name="skip">The number of elements to skip.</param>
    /// <param name="take">The number of elements to take. -1 means take all.</param>
    /// <param name="order">The sort order.</param>
    /// <param name="sortType">The sort type.</param>
    /// <param name="by">The pattern to sort by external keys.</param>
    /// <param name="get">The patterns to retrieve external keys' values.</param>
    /// <returns>An array of sorted elements.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.ListLeftPushAsync("mylist", ["3", "1", "2"]);
    /// var sorted = await client.SortAsync("mylist");  // ["1", "2", "3"]
    /// </code>
    /// </example>
    /// </remarks>
    Task<ValkeyValue[]> SortAsync(ValkeyKey key, long skip = 0, long take = -1, Order order = Order.Ascending, SortType sortType = SortType.Numeric, ValkeyValue by = default, IEnumerable<ValkeyValue>? get = null);

    /// <summary>
    /// Sorts the elements in a list, set, or sorted set and stores the result.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/sort/">Valkey commands – SORT</seealso>
    /// <param name="destination">The key to store the sorted result.</param>
    /// <param name="key">The list, set, or sorted set key.</param>
    /// <param name="skip">The number of elements to skip.</param>
    /// <param name="take">The number of elements to take. -1 means take all.</param>
    /// <param name="order">The sort order.</param>
    /// <param name="sortType">The sort type.</param>
    /// <param name="by">The pattern to sort by external keys.</param>
    /// <param name="get">The patterns to retrieve external keys' values.</param>
    /// <returns>The number of elements stored in <paramref name="destination"/>.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.ListLeftPushAsync("mylist", ["3", "1", "2"]);
    /// var stored = await client.SortAndStoreAsync("sorted", "mylist");  // 3
    /// </code>
    /// </example>
    /// </remarks>
    Task<long> SortAndStoreAsync(ValkeyKey destination, ValkeyKey key, long skip = 0, long take = -1, Order order = Order.Ascending, SortType sortType = SortType.Numeric, ValkeyValue by = default, IEnumerable<ValkeyValue>? get = null);

}
