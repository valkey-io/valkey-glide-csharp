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
    /// bool result = await client.DeleteAsync(key);
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
    /// long result = await client.DeleteAsync([key1, key2]);
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
    /// bool result = await client.UnlinkAsync(key);
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
    /// long result = await client.UnlinkAsync([key1, key2]);
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
    /// bool result = await client.ExistsAsync(key);
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
    /// long result = await client.ExistsAsync([key1, key2]);
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
    /// <returns><see langword="true"/> if the timeout was set. <see langword="false"/> if key does not exist or the timeout could not be set.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// bool result = await client.ExpireAsync(key, TimeSpan.FromSeconds(10));
    /// </code>
    /// </example>
    /// </remarks>
    Task<bool> ExpireAsync(ValkeyKey key, TimeSpan? expiry);

    /// <summary>
    /// Set a timeout on key. After the timeout has expired, the key will automatically be deleted.<br/>
    /// If key already has an existing expire set, the time to live is updated to the new value.
    /// If expiry is a non-positive value, the key will be deleted rather than expired.
    /// The timeout will only be cleared by commands that delete or overwrite the contents of key.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/expire"/>
    /// <param name="key">The key to expire.</param>
    /// <param name="expiry">Duration for the key to expire.</param>
    /// <param name="when">The option to set expiry.</param>
    /// <returns><see langword="true"/> if the timeout was set. <see langword="false"/> if key does not exist or the timeout could not be set.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// bool result = await client.ExpireAsync(key, TimeSpan.FromSeconds(10), ExpireWhen.HasNoExpiry);
    /// </code>
    /// </example>
    /// </remarks>
    Task<bool> ExpireAsync(ValkeyKey key, TimeSpan? expiry, ExpireWhen when);

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
    /// <returns><see langword="true"/> if the timeout was set. <see langword="false"/> if key does not exist or the timeout could not be set.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// bool result = await client.ExpireAsync(key, DateTime.UtcNow.AddMinutes(5));
    /// </code>
    /// </example>
    /// </remarks>
    // TODO #269: Replace DateTime with DateTimeOffset.
    Task<bool> ExpireAsync(ValkeyKey key, DateTime? expiry);

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
    /// <param name="when">In Valkey 7+, the option to set expiry.</param>
    /// <returns><see langword="true"/> if the timeout was set. <see langword="false"/> if key does not exist or the timeout could not be set.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// bool result = await client.ExpireAsync(key, DateTime.UtcNow.AddMinutes(5), ExpireWhen.HasNoExpiry);
    /// </code>
    /// </example>
    /// </remarks>
    // TODO #269: Replace DateTime with DateTimeOffset.
    Task<bool> ExpireAsync(ValkeyKey key, DateTime? expiry, ExpireWhen when);

    /// <summary>
    /// Returns the remaining time to live of a key that has a timeout.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/ttl"/>
    /// <param name="key">The key to return its timeout.</param>
    /// <returns>TTL, or <see langword="null"/> when key does not exist or key exists but has no associated expiration.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// TimeSpan? result = await client.TimeToLiveAsync(key);
    /// </code>
    /// </example>
    /// </remarks>
    Task<TimeSpan?> TimeToLiveAsync(ValkeyKey key);

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
    /// ValkeyType result = await client.TypeAsync(key);
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
    /// await client.RenameAsync(oldKey, newKey);
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
    /// bool result = await client.RenameNXAsync(oldKey, newKey);
    /// </code>
    /// </example>
    /// </remarks>
    Task<bool> RenameNXAsync(ValkeyKey key, ValkeyKey newKey);

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
    /// bool result = await client.PersistAsync(key);
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
    /// byte[]? result = await client.DumpAsync(key);
    /// </code>
    /// </example>
    /// </remarks>
    Task<byte[]?> DumpAsync(ValkeyKey key);

    /// <summary>
    /// Creates a key associated with a value that is obtained by
    /// deserializing the provided serialized value (obtained via <seealso cref="DumpAsync"/>).
    /// This method takes a duration for the expiry.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/restore"/>
    /// <note>When in cluster mode, both source and destination must map to the same hash slot.</note>
    /// <param name="key">The key to create.</param>
    /// <param name="value">The serialized value to deserialize and assign to key.</param>
    /// <param name="expiry">The expiry to set as a duration. Default value is set to persist.</param>
    /// <param name="restoreOptions">Set restore options with replace and absolute TTL modifiers, object idletime and frequency.</param>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.RestoreAsync(key, serializedValue, TimeSpan.FromMinutes(5));
    /// </code>
    /// </example>
    /// </remarks>
    Task RestoreAsync(ValkeyKey key, byte[] value, TimeSpan? expiry = null, RestoreOptions? restoreOptions = null);

    /// <summary>
    /// Creates a key associated with a value that is obtained by
    /// deserializing the provided serialized value (obtained via <seealso cref="DumpAsync"/>).
    /// This method takes an exact date and time for the expiry.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/restore"/>
    /// <note>When in cluster mode, both source and destination must map to the same hash slot.</note>
    /// <param name="key">The key to create.</param>
    /// <param name="value">The serialized value to deserialize and assign to key.</param>
    /// <param name="expiry">The expiry to set as a date and time. Default value is set to persist.</param>
    /// <param name="restoreOptions">Set restore options with replace and absolute TTL modifiers, object idletime and frequency.</param>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.RestoreDateTimeAsync(key, serializedValue, DateTime.UtcNow.AddMinutes(5));
    /// </code>
    /// </example>
    /// </remarks>
    // TODO #269: Replace DateTime with DateTimeOffset.
    Task RestoreDateTimeAsync(ValkeyKey key, byte[] value, DateTime? expiry = null, RestoreOptions? restoreOptions = null);

    /// <summary>
    /// Alters the last access time of a key(s). A key is ignored if it does not exist.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/touch"/>
    /// <param name="key">The keys to update last access time.</param>
    /// <returns><see langword="true"/> if the key was touched, <see langword="false"/> otherwise.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// bool result = await client.TouchAsync(key);
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
    /// long result = await client.TouchAsync([key1, key2]);
    /// </code>
    /// </example>
    /// </remarks>
    Task<long> TouchAsync(IEnumerable<ValkeyKey> keys);

    /// <summary>
    /// Returns the absolute time at which the given key will expire.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/pexpiretime"/>
    /// <param name="key">The key to determine the expiration value of.</param>
    /// <returns>The expiration time, or <see langword="null"/> when key does not exist or key exists but has no associated expiration.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// DateTime? expiration = await client.ExpireTimeAsync(key);
    /// </code>
    /// </example>
    /// </remarks>
    // TODO #269: Replace DateTime with DateTimeOffset.
    Task<DateTime?> ExpireTimeAsync(ValkeyKey key);

    /// <summary>
    /// Returns the internal encoding for the object stored at key.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/object-encoding"/>
    /// <param name="key">The key to determine the encoding of.</param>
    /// <returns>The encoding of the object, or <see langword="null"/> when key does not exist.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// string? encoding = await client.ObjectEncodingAsync(key);
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
    /// long? frequency = await client.ObjectFrequencyAsync(key);
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
    /// TimeSpan? idleTime = await client.ObjectIdleTimeAsync(key);
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
    /// long? refCount = await client.ObjectRefCountAsync(key);
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
    /// <param name="sourceKey">The key to the source value.</param>
    /// <param name="destinationKey">The key where the value should be copied to.</param>
    /// <param name="replace">Whether to overwrite an existing values at destinationKey.</param>
    /// <returns><see langword="true"/> if sourceKey was copied. <see langword="false"/> if sourceKey was not copied.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// bool result = await client.CopyAsync(sourceKey, destKey, replace: true);
    /// </code>
    /// </example>
    /// </remarks>
    Task<bool> CopyAsync(ValkeyKey sourceKey, ValkeyKey destinationKey, bool replace = false);

    /// <summary>
    /// Moves key from the currently selected database to the specified destination database.
    /// When key already exists in the destination database, or it does not exist in the source database, it does nothing.
    /// It is possible to use MOVE as a locking primitive because of this.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/move"/>
    /// <param name="key">The key to move.</param>
    /// <param name="database">The database to move the key to.</param>
    /// <returns><see langword="true"/> if key was moved. <see langword="false"/> if key was not moved.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// bool result = await client.MoveAsync(key, 2);
    /// </code>
    /// </example>
    /// </remarks>
    Task<bool> MoveAsync(ValkeyKey key, int database);

    /// <summary>
    /// Copies the value stored at the source to the destination key in the specified database. When
    /// replace is true, removes the destination key first if it already
    /// exists, otherwise performs no action.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/copy"/>
    /// <note>Since Valkey 6.2.0 and above.</note>
    /// <param name="sourceKey">The key to the source value.</param>
    /// <param name="destinationKey">The key where the value should be copied to.</param>
    /// <param name="destinationDatabase">The database ID to store destinationKey in.</param>
    /// <param name="replace">Whether to overwrite an existing values at destinationKey.</param>
    /// <returns><see langword="true"/> if sourceKey was copied. <see langword="false"/> if sourceKey was not copied.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// bool result = await client.CopyAsync(sourceKey, destKey, 1, replace: true);
    /// </code>
    /// </example>
    /// </remarks>
    Task<bool> CopyAsync(ValkeyKey sourceKey, ValkeyKey destinationKey, int destinationDatabase, bool replace = false);

    /// <summary>
    /// Returns a random key from the database.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/randomkey"/>
    /// <returns>A random key, or <see langword="null"/> when the database is empty.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// ValkeyKey? randomKey = await client.RandomKeyAsync();
    /// </code>
    /// </example>
    /// </remarks>
    Task<ValkeyKey?> RandomKeyAsync();
}
