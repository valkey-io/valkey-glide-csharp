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
    /// <seealso href="https://valkey.io/commands/del/">Valkey commands – DEL</seealso>
    /// <param name="key">The key to delete.</param>
    /// <returns><see langword="true"/> if the key was removed.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var deleted = await client.DeleteAsync("key");
    /// </code>
    /// </example>
    /// </remarks>
    Task<bool> DeleteAsync(ValkeyKey key);

    /// <summary>
    /// Removes the specified keys from the database. A key is ignored if it does not exist.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/del/">Valkey commands – DEL</seealso>
    /// <note>In cluster mode, if keys in <paramref name="keys"/> map to different hash slots, the command
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
    /// </code>
    /// </example>
    /// </remarks>
    Task<long> DeleteAsync(IEnumerable<ValkeyKey> keys);

    /// <summary>
    /// Unlinks (removes) the specified key from the database without blocking the server.
    /// A key is ignored if it does not exist.
    /// Similar to <see cref="DeleteAsync(ValkeyKey)"/>, but non-blocking.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/unlink/">Valkey commands – UNLINK</seealso>
    /// <param name="key">The key to unlink.</param>
    /// <returns><see langword="true"/> if the key was unlinked.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var unlinked = await client.UnlinkAsync("key");
    /// </code>
    /// </example>
    /// </remarks>
    Task<bool> UnlinkAsync(ValkeyKey key);

    /// <summary>
    /// Unlinks (removes) the specified keys from the database without blocking the server.
    /// A key is ignored if it does not exist.
    /// Similar to <see cref="DeleteAsync(IEnumerable{ValkeyKey})"/>, but non-blocking.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/unlink/">Valkey commands – UNLINK</seealso>
    /// <note>In cluster mode, if keys in <paramref name="keys"/> map to different hash slots, the command
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
    /// </code>
    /// </example>
    /// </remarks>
    Task<long> UnlinkAsync(IEnumerable<ValkeyKey> keys);

    /// <summary>
    /// Checks if a key exists.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/exists/">Valkey commands – EXISTS</seealso>
    /// <param name="key">The key to check.</param>
    /// <returns><see langword="true"/> if <paramref name="key"/> exists, <see langword="false"/> otherwise.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var exists = await client.ExistsAsync("key");
    /// </code>
    /// </example>
    /// </remarks>
    Task<bool> ExistsAsync(ValkeyKey key);

    /// <summary>
    /// Returns the number of keys that exist in the database.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/exists/">Valkey commands – EXISTS</seealso>
    /// <note>In cluster mode, if keys in <paramref name="keys"/> map to different hash slots, the command
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
    /// </code>
    /// </example>
    /// </remarks>
    Task<long> ExistsAsync(IEnumerable<ValkeyKey> keys);

    /// <summary>
    /// Sets a timeout on a key. After the timeout has expired, the key will automatically be deleted.<br/>
    /// If the key already has an existing expire set, the time to live is updated to the new value.
    /// If <paramref name="expiry"/> is a non-positive value, the key will be deleted rather than expired.
    /// The timeout will only be cleared by commands that delete or overwrite the contents of the key.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/expire/">Valkey commands – EXPIRE</seealso>
    /// <param name="key">The key to expire.</param>
    /// <param name="expiry">Duration for the key to expire.</param>
    /// <param name="condition">The condition for setting the expiry.</param>
    /// <returns><see langword="true"/> if the timeout was set. <see langword="false"/> if <paramref name="key"/> does not exist or the timeout could not be set.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var set = await client.ExpireAsync("key", TimeSpan.FromSeconds(10));
    /// </code>
    /// </example>
    /// </remarks>
    Task<bool> ExpireAsync(ValkeyKey key, TimeSpan? expiry, ExpireCondition condition = ExpireCondition.Always);

    /// <summary>
    /// Sets a timeout on a key using an absolute timestamp.
    /// A timestamp in the past will delete the key immediately. After the timeout has
    /// expired, the key will automatically be deleted.<br/>
    /// If the key already has an existing expire set, the time to live is updated to the new value.
    /// The timeout will only be cleared by commands that delete or overwrite the contents of the key.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/expireat/">Valkey commands – EXPIREAT</seealso>
    /// <param name="key">The key to expire.</param>
    /// <param name="expiry">The timestamp for expiry.</param>
    /// <param name="condition">The condition for setting the expiry.</param>
    /// <returns><see langword="true"/> if the timeout was set. <see langword="false"/> if <paramref name="key"/> does not exist or the timeout could not be set.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var set = await client.ExpireAsync("key", DateTimeOffset.UtcNow.AddMinutes(5));
    /// </code>
    /// </example>
    /// </remarks>
    Task<bool> ExpireAsync(ValkeyKey key, DateTimeOffset? expiry, ExpireCondition condition = ExpireCondition.Always);

    /// <summary>
    /// Returns the remaining time to live of a key that has a timeout.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/pttl/">Valkey commands – PTTL</seealso>
    /// <param name="key">The key to check the timeout of.</param>
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
    /// Returns the type of the value stored at a key.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/type/">Valkey commands – TYPE</seealso>
    /// <param name="key">The key to check the data type of.</param>
    /// <returns>The <see cref="ValkeyType"/> of <paramref name="key"/>, or none when <paramref name="key"/> does not exist.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var type = await client.TypeAsync("key");
    /// </code>
    /// </example>
    /// </remarks>
    Task<ValkeyType> TypeAsync(ValkeyKey key);

    /// <summary>
    /// Renames a key.
    /// If <paramref name="newKey"/> already exists it is overwritten.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/rename/">Valkey commands – RENAME</seealso>
    /// <note>When in cluster mode, both <paramref name="key"/> and <paramref name="newKey"/> must map to the same hash slot.</note>
    /// <param name="key">The key to rename.</param>
    /// <param name="newKey">The new name of the key.</param>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.RenameAsync("oldkey", "newkey");
    /// </code>
    /// </example>
    /// </remarks>
    Task RenameAsync(ValkeyKey key, ValkeyKey newKey);

    /// <summary>
    /// Renames a key only if the new key does not yet exist.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/renamenx/">Valkey commands – RENAMENX</seealso>
    /// <note>When in cluster mode, both <paramref name="key"/> and <paramref name="newKey"/> must map to the same hash slot.</note>
    /// <param name="key">The key to rename.</param>
    /// <param name="newKey">The new name of the key.</param>
    /// <returns><see langword="true"/> if the key was renamed, <see langword="false"/> if <paramref name="newKey"/> already exists.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var renamed = await client.RenameIfNotExistsAsync("oldkey", "newkey");
    /// </code>
    /// </example>
    /// </remarks>
    Task<bool> RenameIfNotExistsAsync(ValkeyKey key, ValkeyKey newKey);

    /// <summary>
    /// Removes the existing timeout on a key, turning it from volatile
    /// (a key with an expire set) to persistent (a key that will never expire as no timeout is associated).
    /// </summary>
    /// <seealso href="https://valkey.io/commands/persist/">Valkey commands – PERSIST</seealso>
    /// <param name="key">The key to remove the existing timeout on.</param>
    /// <returns><see langword="true"/> if the timeout was removed. <see langword="false"/> if <paramref name="key"/> does not exist or does not have an associated timeout.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var removed = await client.PersistAsync("key");
    /// </code>
    /// </example>
    /// </remarks>
    Task<bool> PersistAsync(ValkeyKey key);

    /// <summary>
    /// Serializes the value stored at a key in a Valkey-specific format.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/dump/">Valkey commands – DUMP</seealso>
    /// <param name="key">The key to serialize.</param>
    /// <returns>
    /// The serialized value of the data stored at <paramref name="key"/>,
    /// or <see langword="null"/> if <paramref name="key"/> does not exist.
    /// </returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var serialized = await client.DumpAsync("key");
    /// </code>
    /// </example>
    /// </remarks>
    Task<byte[]?> DumpAsync(ValkeyKey key);

    /// <summary>
    /// Creates a key associated with a value obtained by deserializing the provided serialized
    /// <paramref name="value"/> (obtained via <see cref="DumpAsync"/>).
    /// </summary>
    /// <seealso href="https://valkey.io/commands/restore/">Valkey commands – RESTORE</seealso>
    /// <note>When in cluster mode, both source and destination must map to the same hash slot.</note>
    /// <param name="key">The key to create.</param>
    /// <param name="value">The serialized value to deserialize and assign to <paramref name="key"/>.</param>
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
    /// Alters the last access time of a key. A key is ignored if it does not exist.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/touch/">Valkey commands – TOUCH</seealso>
    /// <param name="key">The key to update the last access time of.</param>
    /// <returns><see langword="true"/> if the key was touched, <see langword="false"/> otherwise.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var touched = await client.TouchAsync("key");
    /// </code>
    /// </example>
    /// </remarks>
    Task<bool> TouchAsync(ValkeyKey key);

    /// <summary>
    /// Alters the last access time of the specified keys. A key is ignored if it does not exist.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/touch/">Valkey commands – TOUCH</seealso>
    /// <note>In cluster mode, if keys in <paramref name="keys"/> map to different hash slots, the command
    /// will be split across these slots and executed separately for each. This means the command
    /// is atomic only at the slot level. If one or more slot-specific requests fail, the entire
    /// call will return the first encountered error, even though some requests may have succeeded
    /// while others did not. If this behavior impacts your application logic, consider splitting
    /// the request into sub-requests per slot to ensure atomicity.</note>
    /// <param name="keys">The keys to update the last access time of.</param>
    /// <returns>The number of keys that were updated.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var touched = await client.TouchAsync(["key1", "key2"]);
    /// </code>
    /// </example>
    /// </remarks>
    Task<long> TouchAsync(IEnumerable<ValkeyKey> keys);

    /// <summary>
    /// Returns the absolute time at which a key will expire.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/pexpiretime/">Valkey commands – PEXPIRETIME</seealso>
    /// <note>Since Valkey 7.0.0.</note>
    /// <param name="key">The key to determine the expiration value of.</param>
    /// <returns>
    /// The expiration time as a <see cref="DateTimeOffset"/>, or <see langword="null"/> if <paramref name="key"/>
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
    /// Returns the internal encoding for the object stored at a key.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/object-encoding/">Valkey commands – OBJECT ENCODING</seealso>
    /// <param name="key">The key to determine the encoding of.</param>
    /// <returns>The encoding of the object, or <see langword="null"/> when <paramref name="key"/> does not exist.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var encoding = await client.ObjectEncodingAsync("key");
    /// </code>
    /// </example>
    /// </remarks>
    Task<string?> ObjectEncodingAsync(ValkeyKey key);

    /// <summary>
    /// Returns the logarithmic access frequency counter for the object stored at a key.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/object-freq/">Valkey commands – OBJECT FREQ</seealso>
    /// <param name="key">The key to determine the frequency of.</param>
    /// <returns>The frequency counter, or <see langword="null"/> when <paramref name="key"/> does not exist.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var frequency = await client.ObjectFrequencyAsync("key");
    /// </code>
    /// </example>
    /// </remarks>
    Task<long?> ObjectFrequencyAsync(ValkeyKey key);

    /// <summary>
    /// Returns the time since the object stored at a key was last accessed.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/object-idletime/">Valkey commands – OBJECT IDLETIME</seealso>
    /// <param name="key">The key to determine the idle time of.</param>
    /// <returns>The idle time, or <see langword="null"/> when <paramref name="key"/> does not exist.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var idleTime = await client.ObjectIdleTimeAsync("key");
    /// </code>
    /// </example>
    /// </remarks>
    Task<TimeSpan?> ObjectIdleTimeAsync(ValkeyKey key);

    /// <summary>
    /// Returns the reference count of the object stored at a key.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/object-refcount/">Valkey commands – OBJECT REFCOUNT</seealso>
    /// <param name="key">The key to determine the reference count of.</param>
    /// <returns>The reference count, or <see langword="null"/> when <paramref name="key"/> does not exist.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var refCount = await client.ObjectRefCountAsync("key");
    /// </code>
    /// </example>
    /// </remarks>
    Task<long?> ObjectRefCountAsync(ValkeyKey key);

    /// <summary>
    /// Copies the value stored at <paramref name="source"/> to <paramref name="destination"/>.
    /// When <paramref name="replace"/> is <see langword="true"/>, removes the <paramref name="destination"/> key first
    /// if it already exists, otherwise performs no action.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/copy/">Valkey commands – COPY</seealso>
    /// <note>When in cluster mode, both <paramref name="source"/> and <paramref name="destination"/> must map to the same hash slot.</note>
    /// <param name="source">The key to the source value.</param>
    /// <param name="destination">The key where the value should be copied to.</param>
    /// <param name="replace">Whether to overwrite an existing value at <paramref name="destination"/>.</param>
    /// <returns><see langword="true"/> if <paramref name="source"/> was copied, <see langword="false"/> otherwise.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var copied = await client.CopyAsync("source", "destination", replace: true);
    /// </code>
    /// </example>
    /// </remarks>
    Task<bool> CopyAsync(ValkeyKey source, ValkeyKey destination, bool replace = false);

    /// <summary>
    /// Returns a random key from the database.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/randomkey/">Valkey commands – RANDOMKEY</seealso>
    /// <returns>A random key, or <see langword="null"/> when the database is empty.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var randomKey = await client.RandomKeyAsync();
    /// </code>
    /// </example>
    /// </remarks>
    Task<ValkeyKey?> RandomKeyAsync();

    /// <summary>
    /// Blocks the current client until all previous write commands are successfully transferred
    /// and acknowledged by at least <paramref name="numreplicas"/> replicas.
    /// If the timeout is reached, the command returns even if the specified number of replicas were not yet reached.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/wait/">Valkey commands – WAIT</seealso>
    /// <param name="numreplicas">The number of replicas to wait for.</param>
    /// <param name="timeout">The timeout to wait.</param>
    /// <returns>The number of replicas that acknowledged the write commands.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var acknowledged = await client.WaitAsync(1, TimeSpan.FromSeconds(1));
    /// </code>
    /// </example>
    /// </remarks>
    Task<long> WaitAsync(long numreplicas, TimeSpan timeout);

    /// <summary>
    /// Blocks the current client until all previous write commands are successfully transferred and acknowledged
    /// by at least the specified number of local and replica AOF-synced nodes.
    /// If the timeout is reached, the command returns even if the specified number of acknowledgments were not yet reached.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/waitaof/">Valkey commands – WAITAOF</seealso>
    /// <note>Since Valkey 7.2.0.</note>
    /// <param name="localAof">Whether to wait for the local node to acknowledge AOF sync.</param>
    /// <param name="numreplicas">The number of replica nodes to wait for AOF sync.</param>
    /// <param name="timeout">The timeout to wait.</param>
    /// <returns>An array of two longs: the number of local and replica nodes that acknowledged the write commands.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var acknowledged = await client.WaitAofAsync(true, 1, TimeSpan.FromSeconds(1));
    /// </code>
    /// </example>
    /// </remarks>
    Task<long[]> WaitAofAsync(bool localAof, long numreplicas, TimeSpan timeout);

    /// <summary>
    /// Sorts the elements in a list, set, or sorted set and returns the result.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/sort/">Valkey commands – SORT</seealso>
    /// <param name="key">The key of the list, set, or sorted set to be sorted.</param>
    /// <param name="options">The options for the SORT command.</param>
    /// <returns>An array of sorted elements.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.ListLeftPushAsync("mylist", ["3", "1", "2"]);
    /// var options = new SortOptions { Order = SortOrder.Descending };
    /// var sorted = await client.SortAsync("mylist", options);  // ["3", "2", "1"]
    /// </code>
    /// </example>
    /// </remarks>
    Task<ValkeyValue[]> SortAsync(ValkeyKey key, SortOptions? options);

    /// <summary>
    /// Sorts the elements in a list, set, or sorted set and stores the result
    /// in <paramref name="destination"/>.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/sort/">Valkey commands – SORT</seealso>
    /// <param name="destination">The key to store the sorted result.</param>
    /// <param name="key">The key of the list, set, or sorted set to be sorted.</param>
    /// <param name="options">The options for the SORT command.</param>
    /// <returns>The number of elements stored in <paramref name="destination"/>.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.ListLeftPushAsync("mylist", ["3", "1", "2"]);
    /// var options = new SortOptions { Order = SortOrder.Descending };
    /// var stored = await client.SortAndStoreAsync("sorted", "mylist", options);
    /// </code>
    /// </example>
    /// </remarks>
    Task<long> SortAndStoreAsync(ValkeyKey destination, ValkeyKey key, SortOptions? options);

    /// <summary>
    /// Sorts the elements in a list, set, or sorted set and returns the result.
    /// Reads-only variant of SORT that is guaranteed not to modify the database.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/sort_ro/">Valkey commands – SORT_RO</seealso>
    /// <note>Since Valkey 7.0.0.</note>
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
    /// var sorted = await client.SortReadOnlyAsync("mylist");  // ["1", "2", "3"]
    /// </code>
    /// </example>
    /// </remarks>
    Task<ValkeyValue[]> SortReadOnlyAsync(ValkeyKey key, long skip = 0, long take = -1, Order order = Order.Ascending, SortType sortType = SortType.Numeric, ValkeyValue by = default, IEnumerable<ValkeyValue>? get = null);

    /// <summary>
    /// Sorts the elements in a list, set, or sorted set and returns the result.
    /// Reads-only variant of SORT that is guaranteed not to modify the database.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/sort_ro/">Valkey commands – SORT_RO</seealso>
    /// <note>Since Valkey 7.0.0.</note>
    /// <param name="key">The key of the list, set, or sorted set to be sorted.</param>
    /// <param name="options">The options for the SORT_RO command.</param>
    /// <returns>An array of sorted elements.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.ListLeftPushAsync("mylist", ["3", "1", "2"]);
    /// var options = new SortOptions { Order = SortOrder.Descending };
    /// var sorted = await client.SortReadOnlyAsync("mylist", options);  // ["3", "2", "1"]
    /// </code>
    /// </example>
    /// </remarks>
    Task<ValkeyValue[]> SortReadOnlyAsync(ValkeyKey key, SortOptions? options);
}
