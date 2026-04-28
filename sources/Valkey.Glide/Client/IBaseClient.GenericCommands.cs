// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands.Options;

namespace Valkey.Glide;

/// <summary>
/// Generic commands for Valkey GLIDE clients.
/// </summary>
/// <remarks>
/// These methods use Valkey GLIDE naming conventions. For StackExchange.Redis-compatible
/// methods with "Key" prefix, use <see cref="IDatabaseAsync"/>.
/// </remarks>
/// <seealso href="https://valkey.io/commands/#generic">Valkey – Generic Commands</seealso>
public partial interface IBaseClient
{
    /// <summary>
    /// Removes the specified key from the database. A key is ignored if it does not exist.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/del"/>
    /// <param name="key">The key to delete.</param>
    /// <returns><see langword="true"/> if the key was removed.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var deleted = await client.DeleteAsync("key");
    /// Console.WriteLine($"Key was deleted: {deleted}");
    /// </code>
    /// </example>
    /// </remarks>
    Task<bool> DeleteAsync(ValkeyKey key);

    /// <summary>
    /// Removes the specified keys from the database. A key is ignored if it does not exist.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/del"/>
    /// <note>When in cluster mode, if keys in keys map to different hash slots, the command
    /// will be split across these slots and executed separately for each. This means the command
    /// is atomic only at the slot level. If one or more slot-specific requests fail, the entire
    /// call will return the first encountered error, even though some requests may have succeeded
    /// while others did not. If this behavior impacts your application logic, consider splitting
    /// the request into sub-requests per slot to ensure atomicity.</note>
    /// <param name="keys">The keys to delete.</param>
    /// <returns>The number of keys that were removed.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var deleted = await client.DeleteAsync(["key1", "key2"]);
    /// Console.WriteLine($"Number of keys deleted: {deleted}");
    /// </code>
    /// </example>
    /// </remarks>
    Task<long> DeleteAsync(IEnumerable<ValkeyKey> keys);

    /// <summary>
    /// Unlinks (removes) the specified key from the database. A key is ignored if it does not exist.
    /// This command is similar to <seealso cref="DeleteAsync(ValkeyKey)"/>, however, this command does not block the server.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/unlink"/>
    /// <param name="key">The key to unlink.</param>
    /// <returns><see langword="true"/> if the key was unlinked.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var unlinked = await client.UnlinkAsync("key");
    /// Console.WriteLine($"Key was unlinked: {unlinked}");
    /// </code>
    /// </example>
    /// </remarks>
    Task<bool> UnlinkAsync(ValkeyKey key);

    /// <summary>
    /// Unlinks (removes) the specified key from the database. A key is ignored if it does not exist.
    /// This command is similar to <seealso cref="DeleteAsync(IEnumerable{ValkeyKey})"/>, however, this command does not block the server.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/unlink"/>
    /// <note>When in cluster mode, if keys in keys map to different hash slots, the command
    /// will be split across these slots and executed separately for each. This means the command
    /// is atomic only at the slot level. If one or more slot-specific requests fail, the entire
    /// call will return the first encountered error, even though some requests may have succeeded
    /// while others did not. If this behavior impacts your application logic, consider splitting
    /// the request into sub-requests per slot to ensure atomicity.</note>
    /// <param name="keys">The keys to unlink.</param>
    /// <returns>The number of keys that were unlinked.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var unlinked = await client.UnlinkAsync(["key1", "key2"]);
    /// Console.WriteLine($"Number of keys unlinked: {unlinked}");
    /// </code>
    /// </example>
    /// </remarks>
    Task<long> UnlinkAsync(IEnumerable<ValkeyKey> keys);

    /// <summary>
    /// Returns if key exists.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/exists"/>
    /// <param name="key">The key to check.</param>
    /// <returns><see langword="true"/> if the key exists. <see langword="false"/> if the key does not exist.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var exists = await client.ExistsAsync("key");
    /// Console.WriteLine($"Key exists: {exists}");
    /// </code>
    /// </example>
    /// </remarks>
    Task<bool> ExistsAsync(ValkeyKey key);

    /// <summary>
    /// Returns the number of keys that exist in the database.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/exists"/>
    /// <note>When in cluster mode, if keys in keys map to different hash slots, the command
    /// will be split across these slots and executed separately for each. This means the command
    /// is atomic only at the slot level. If one or more slot-specific requests fail, the entire
    /// call will return the first encountered error, even though some requests may have succeeded
    /// while others did not. If this behavior impacts your application logic, consider splitting
    /// the request into sub-requests per slot to ensure atomicity.</note>
    /// <param name="keys">The keys to check.</param>
    /// <returns>The number of existing keys.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var existing = await client.ExistsAsync(["key1", "key2"]);
    /// Console.WriteLine($"Number of existing keys: {existing}");
    /// </code>
    /// </example>
    /// </remarks>
    Task<long> ExistsAsync(IEnumerable<ValkeyKey> keys);

    /// <summary>
    /// Set a timeout on key. After the timeout has expired, the key will automatically be deleted.<br/>
    /// If key already has an existing expire set, the time to live is updated to the new value.
    /// If expiry is a non-positive value, the key will be deleted rather than expired.
    /// The timeout will only be cleared by commands that delete or overwrite the contents of key.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/expire"/>
    /// <param name="key">The key to expire.</param>
    /// <param name="expiry">Duration for the key to expire.</param>
    /// <param name="condition">The condition for setting the expiry.</param>
    /// <returns><see langword="true"/> if the timeout was set. <see langword="false"/> if key does not exist or the timeout could not be set.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var set = await client.ExpireAsync("key", TimeSpan.FromSeconds(10));
    /// Console.WriteLine($"Timeout was set: {set}");
    /// </code>
    /// </example>
    /// </remarks>
    Task<bool> ExpireAsync(ValkeyKey key, TimeSpan? expiry, ExpireCondition condition = ExpireCondition.Always);

    /// <summary>
    /// Sets a timeout on key. It takes an absolute Unix timestamp (seconds since January 1, 1970) instead of
    /// specifying the duration. A timestamp in the past will delete the key immediately. After the timeout has
    /// expired, the key will automatically be deleted.<br/>
    /// If key already has an existing expire set, the time to live is updated to the new value.
    /// The timeout will only be cleared by commands that delete or overwrite the contents of key.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/expireat"/>
    /// <param name="key">The key to expire.</param>
    /// <param name="expiry">The timestamp for expiry.</param>
    /// <param name="condition">The condition for setting the expiry.</param>
    /// <returns><see langword="true"/> if the timeout was set. <see langword="false"/> if key does not exist or the timeout could not be set.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var set = await client.ExpireAsync("key", DateTimeOffset.UtcNow.AddMinutes(5));
    /// Console.WriteLine($"Timeout was set: {set}");
    /// </code>
    /// </example>
    /// </remarks>
    Task<bool> ExpireAsync(ValkeyKey key, DateTimeOffset? expiry, ExpireCondition condition = ExpireCondition.Always);

    /// <summary>
    /// Returns the remaining time to live of a key that has a timeout.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/pttl"/>
    /// <param name="key">The key to return its timeout.</param>
    /// <returns>
    /// A <see cref="TimeToLiveResult"/> containing the TTL information.
    /// Use <see cref="TimeToLiveResult.Exists"/> to check if the key exists,
    /// <see cref="TimeToLiveResult.HasTimeToLive"/> to check if it has an expiry,
    /// and <see cref="TimeToLiveResult.TimeToLive"/> to get the remaining time.
    /// </returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var ttlResult = await client.TimeToLiveAsync("key");
    ///
    /// if (!ttlResult.Exists)
    /// {
    ///     Console.WriteLine("Key does not exist");
    /// }
    /// else if (!ttlResult.HasTimeToLive)
    /// {
    ///     Console.WriteLine("Key has no expiry");
    /// }
    /// else
    /// {
    ///     Console.WriteLine($"TTL: {ttlResult.TimeToLive}");
    /// }
    /// </code>
    /// </example>
    /// </remarks>
    Task<TimeToLiveResult> TimeToLiveAsync(ValkeyKey key);

    /// <summary>
    /// Returns the ValkeyType of the value stored at key.
    /// The different types that can be returned are: String, List, Set, SortedSet, Hash and Stream.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/type"/>
    /// <param name="key">The key to check its data type.</param>
    /// <returns>Type of key, or none when key does not exist.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var type = await client.TypeAsync("key");
    /// Console.WriteLine($"Key type: {type}");
    /// </code>
    /// </example>
    /// </remarks>
    Task<ValkeyType> TypeAsync(ValkeyKey key);

    /// <summary>
    /// Renames key to newKey.
    /// If newKey already exists it is overwritten.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/rename"/>
    /// <note>When in cluster mode, both key and newKey must map to the same hash slot.</note>
    /// <param name="key">The key to rename.</param>
    /// <param name="newKey">The new name of the key.</param>
    /// <returns>A task that completes when the rename succeeds. If key does not exist, an error is thrown.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.RenameAsync("oldkey", "newkey");
    /// </code>
    /// </example>
    /// </remarks>
    Task RenameAsync(ValkeyKey key, ValkeyKey newKey);

    /// <summary>
    /// Renames key to newKey if newKey does not yet exist.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/renamenx"/>
    /// <note>When in cluster mode, both key and newKey must map to the same hash slot.</note>
    /// <param name="key">The key to rename.</param>
    /// <param name="newKey">The new name of the key.</param>
    /// <returns><see langword="true"/> if the key was renamed, <see langword="false"/> if newKey already exists.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var renamed = await client.RenameIfNotExistsAsync("oldkey", "newkey");
    /// Console.WriteLine($"Key was renamed: {renamed}");
    /// </code>
    /// </example>
    /// </remarks>
    Task<bool> RenameIfNotExistsAsync(ValkeyKey key, ValkeyKey newKey);

    /// <summary>
    /// Removes the existing timeout on key, turning the key from volatile
    /// (a key with an expire set) to persistent (a key that will never expire as no timeout is associated).
    /// </summary>
    /// <seealso href="https://valkey.io/commands/persist"/>
    /// <param name="key">The key to remove the existing timeout on.</param>
    /// <returns><see langword="true"/> if the timeout was removed. <see langword="false"/> if key does not exist or does not have an associated timeout.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var removed = await client.PersistAsync("key");
    /// Console.WriteLine($"Timeout was removed: {removed}");
    /// </code>
    /// </example>
    /// </remarks>
    Task<bool> PersistAsync(ValkeyKey key);

    /// <summary>
    /// Serializes the value stored at key in a Valkey-specific format.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/dump"/>
    /// <param name="key">The key to serialize.</param>
    /// <returns>
    /// The serialized value of the data stored at key.
    /// If key does not exist, <see langword="null"/> will be returned.
    /// </returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var serialized = await client.DumpAsync("key");
    /// Console.WriteLine($"Serialized value: {serialized}");
    /// </code>
    /// </example>
    /// </remarks>
    Task<byte[]?> DumpAsync(ValkeyKey key);

    /// <summary>
    /// Creates a key associated with a value that is obtained by
    /// deserializing the provided serialized value (obtained via <seealso cref="DumpAsync"/>).
    /// </summary>
    /// <seealso href="https://valkey.io/commands/restore"/>
    /// <note>When in cluster mode, both source and destination must map to the same hash slot.</note>
    /// <param name="key">The key to create.</param>
    /// <param name="value">The serialized value to deserialize and assign to key.</param>
    /// <param name="options">Optional restore options including TTL, replace, idle time, and frequency.</param>
    /// <remarks>
    /// <example>
    /// <code>
    /// var bytes = new byte[] { 0x00, 0x01, 0x02 };
    /// await client.RestoreAsync("key", bytes);
    /// </code>
    /// </example>
    /// <example>
    /// <code>
    /// var bytes = new byte[] { 0x00, 0x01, 0x02 };
    /// var options = new RestoreOptions { Ttl = TimeSpan.FromMinutes(5) };
    /// await client.RestoreAsync("key", bytes, options);
    /// </code>
    /// </example>
    /// <example>
    /// <code>
    /// var bytes = new byte[] { 0x00, 0x01, 0x02 };
    /// var options = new RestoreOptions { ExpireAt = DateTimeOffset.UtcNow.AddHours(1) };
    /// await client.RestoreAsync("key", bytes, options);
    /// </code>
    /// </example>
    /// <example>
    /// <code>
    /// var bytes = new byte[] { 0x00, 0x01, 0x02 };
    /// var options = new RestoreOptions { Replace = true, Ttl = TimeSpan.FromMinutes(5) };
    /// await client.RestoreAsync("key", bytes, options);
    /// </code>
    /// </example>
    /// </remarks>
    Task RestoreAsync(ValkeyKey key, byte[] value, RestoreOptions? options = null);

    /// <summary>
    /// Alters the last access time of a key(s). A key is ignored if it does not exist.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/touch"/>
    /// <param name="key">The keys to update last access time.</param>
    /// <returns><see langword="true"/> if the key was touched, <see langword="false"/> otherwise.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var touched = await client.TouchAsync("key");
    /// Console.WriteLine($"Key was touched: {touched}");
    /// </code>
    /// </example>
    /// </remarks>
    Task<bool> TouchAsync(ValkeyKey key);

    /// <summary>
    /// Alters the last access time of a key(s). A key is ignored if it does not exist.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/touch"/>
    /// <note>When in cluster mode, if keys in keys map to different hash slots, the command
    /// will be split across these slots and executed separately for each. This means the command
    /// is atomic only at the slot level. If one or more slot-specific requests fail, the entire
    /// call will return the first encountered error, even though some requests may have succeeded
    /// while others did not. If this behavior impacts your application logic, consider splitting
    /// the request into sub-requests per slot to ensure atomicity.</note>
    /// <param name="keys">The keys to update last access time.</param>
    /// <returns>The number of keys that were updated.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var touched = await client.TouchAsync(["key1", "key2"]);
    /// Console.WriteLine($"Number of keys touched: {touched}");
    /// </code>
    /// </example>
    /// </remarks>
    Task<long> TouchAsync(IEnumerable<ValkeyKey> keys);

    /// <summary>
    /// Returns the absolute time at which the given key will expire.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/pexpiretime"/>
    /// <param name="key">The key to determine the expiration value of.</param>
    /// <returns>
    /// The expiration time as a <see cref="DateTimeOffset"/>, or <see langword="null"/> if the key
    /// does not exist or has no associated expiration.
    /// </returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var expiration = await client.ExpireTimeAsync("key");
    ///
    /// if (expiration is null)
    /// {
    ///     Console.WriteLine("Key does not exist or has no expiry");
    /// }
    /// else
    /// {
    ///     Console.WriteLine($"Expires at: {expiration}");
    /// }
    /// </code>
    /// </example>
    /// </remarks>
    Task<DateTimeOffset?> ExpireTimeAsync(ValkeyKey key);

    /// <summary>
    /// Returns the internal encoding for the object stored at key.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/object-encoding"/>
    /// <param name="key">The key to determine the encoding of.</param>
    /// <returns>The encoding of the object, or <see langword="null"/> when key does not exist.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var encoding = await client.ObjectEncodingAsync("key");
    /// Console.WriteLine($"Encoding: {encoding}");
    /// </code>
    /// </example>
    /// </remarks>
    Task<string?> ObjectEncodingAsync(ValkeyKey key);

    /// <summary>
    /// Returns the logarithmic access frequency counter for the object stored at key.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/object-freq"/>
    /// <param name="key">The key to determine the frequency of.</param>
    /// <returns>The frequency counter, or <see langword="null"/> when key does not exist.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var frequency = await client.ObjectFrequencyAsync("key");
    /// Console.WriteLine($"Frequency: {frequency}");
    /// </code>
    /// </example>
    /// </remarks>
    Task<long?> ObjectFrequencyAsync(ValkeyKey key);

    /// <summary>
    /// Returns the time since the object stored at key was last accessed.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/object-idletime"/>
    /// <param name="key">The key to determine the idle time of.</param>
    /// <returns>The idle time, or <see langword="null"/> when key does not exist.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var idleTime = await client.ObjectIdleTimeAsync("key");
    /// Console.WriteLine($"Idle time: {idleTime}");
    /// </code>
    /// </example>
    /// </remarks>
    Task<TimeSpan?> ObjectIdleTimeAsync(ValkeyKey key);

    /// <summary>
    /// Returns the reference count of the object stored at key.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/object-refcount"/>
    /// <param name="key">The key to determine the reference count of.</param>
    /// <returns>The reference count, or <see langword="null"/> when key does not exist.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var refCount = await client.ObjectRefCountAsync("key");
    /// Console.WriteLine($"Reference count: {refCount}");
    /// </code>
    /// </example>
    /// </remarks>
    Task<long?> ObjectRefCountAsync(ValkeyKey key);

    /// <summary>
    /// Copies the value stored at the source to the destination key. When
    /// replace is true, removes the destination key first if it already
    /// exists, otherwise performs no action.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/copy"/>
    /// <note>Since Valkey 6.2.0 and above.</note>
    /// <note>When in cluster mode, both sourceKey and destinationKey must map to the same hash slot.</note>
    /// <param name="source">The key to the source value.</param>
    /// <param name="destination">The key where the value should be copied to.</param>
    /// <param name="replace">Whether to overwrite an existing values at destination.</param>
    /// <returns><see langword="true"/> if source was copied. <see langword="false"/> if source was not copied.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var copied = await client.CopyAsync("source", "destination", replace: true);
    /// Console.WriteLine($"Key was copied: {copied}");
    /// </code>
    /// </example>
    /// </remarks>
    Task<bool> CopyAsync(ValkeyKey source, ValkeyKey destination, bool replace = false);

    /// <summary>
    /// Returns a random key from the database.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/randomkey"/>
    /// <returns>A random key, or <see langword="null"/> when the database is empty.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var randomKey = await client.RandomKeyAsync();
    /// Console.WriteLine($"Random key: {randomKey}");
    /// </code>
    /// </example>
    /// </remarks>
    Task<ValkeyKey?> RandomKeyAsync();

    /// <summary>
    /// Blocks the current client until all the previous write commands are successfully transferred and acknowledged by at least numreplicas replicas.
    /// If the timeout is reached, the command returns even if the specified number of replicas were not yet reached.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/wait"/>
    /// <param name="numreplicas">The number of replicas to wait for.</param>
    /// <param name="timeout">The timeout to wait.</param>
    /// <returns>The number of replicas that acknowledged the write commands.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var acknowledged = await client.WaitAsync(1, TimeSpan.FromSeconds(1));
    /// Console.WriteLine($"Number of replicas that acknowledged: {acknowledged}");
    /// </code>
    /// </example>
    /// </remarks>
    Task<long> WaitAsync(long numreplicas, TimeSpan timeout);

    /// <summary>
    /// Blocks the current client until all the previous write commands are successfully transferred and acknowledged
    /// by at least the specified number of local and replica AOF-synced nodes.
    /// If the timeout is reached, the command returns even if the specified number of acknowledgments were not yet reached.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/waitaof"/>
    /// <param name="localAof">Whether to wait for the local node to acknowledge AOF sync.</param>
    /// <param name="numreplicas">The number of replica nodes to wait for AOF sync.</param>
    /// <param name="timeout">The timeout to wait.</param>
    /// <returns>An array of two longs: the number of local and replica nodes that acknowledged the write commands.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var acknowledged = await client.WaitAofAsync(true, 1, TimeSpan.FromSeconds(1));
    /// Console.WriteLine($"Local: {acknowledged[0]}, Replicas: {acknowledged[1]}");
    /// </code>
    /// </example>
    /// </remarks>
    Task<long[]> WaitAofAsync(bool localAof, long numreplicas, TimeSpan timeout);

    /// <summary>
    /// Sorts the elements in the list, set, or sorted set at key and returns the result.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/sort"/>
    /// <param name="key">The key of the list, set, or sorted set to be sorted.</param>
    /// <param name="options">The options for the SORT command.</param>
    /// <returns>An array of sorted elements.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.ListLeftPushAsync("mylist", ["3", "1", "2"]);
    /// var options = new SortOptions { Order = SortOrder.Descending };
    /// var sorted = await client.SortAsync("mylist", options);
    /// Console.WriteLine($"Sorted: {string.Join(", ", sorted)}");
    /// </code>
    /// </example>
    /// </remarks>
    Task<ValkeyValue[]> SortAsync(ValkeyKey key, SortOptions? options);

    /// <summary>
    /// Sorts the elements in the list, set, or sorted set at key and stores the result in destination.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/sort"/>
    /// <param name="destination">The key to store the sorted result.</param>
    /// <param name="key">The key of the list, set, or sorted set to be sorted.</param>
    /// <param name="options">The options for the SORT command.</param>
    /// <returns>The number of elements stored in destination.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.ListLeftPushAsync("mylist", ["3", "1", "2"]);
    /// var options = new SortOptions { Order = SortOrder.Descending };
    /// var stored = await client.SortAndStoreAsync("sorted", "mylist", options);
    /// Console.WriteLine($"Number of elements stored: {stored}");
    /// </code>
    /// </example>
    /// </remarks>
    Task<long> SortAndStoreAsync(ValkeyKey destination, ValkeyKey key, SortOptions? options);

    /// <summary>
    /// Sorts the elements in the list, set, or sorted set at key and returns the result.
    /// This is a read-only variant of SORT that is guaranteed not to modify the database.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/sort_ro"/>
    /// <note>Since Valkey 7.0.0 and above.</note>
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
    /// var sorted = await client.SortReadOnlyAsync("mylist");
    /// Console.WriteLine($"Sorted: {string.Join(", ", sorted)}");
    /// </code>
    /// </example>
    /// </remarks>
    Task<ValkeyValue[]> SortReadOnlyAsync(ValkeyKey key, long skip = 0, long take = -1, Order order = Order.Ascending, SortType sortType = SortType.Numeric, ValkeyValue by = default, IEnumerable<ValkeyValue>? get = null);

    /// <summary>
    /// Sorts the elements in the list, set, or sorted set at key and returns the result.
    /// This is a read-only variant of SORT that is guaranteed not to modify the database.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/sort_ro"/>
    /// <note>Since Valkey 7.0.0 and above.</note>
    /// <param name="key">The key of the list, set, or sorted set to be sorted.</param>
    /// <param name="options">The options for the SORT_RO command.</param>
    /// <returns>An array of sorted elements.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.ListLeftPushAsync("mylist", ["3", "1", "2"]);
    /// var options = new SortOptions { Order = SortOrder.Descending };
    /// var sorted = await client.SortReadOnlyAsync("mylist", options);
    /// Console.WriteLine($"Sorted: {string.Join(", ", sorted)}");
    /// </code>
    /// </example>
    /// </remarks>
    Task<ValkeyValue[]> SortReadOnlyAsync(ValkeyKey key, SortOptions? options);
}
