// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands;

namespace Valkey.Glide;

/// ATTENTION: Methods should only be added to this interface if they are implemented
/// by StackExchange.Redis databases but NOT by Valkey GLIDE clients. Methods implemented
/// by both should be added to <see cref="IGenericBaseCommands"/> instead.

public partial interface IDatabaseAsync
{
    /// <summary>
    /// Removes the specified key from the database using the DEL command.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/del/">Valkey commands – DEL</seealso>
    /// <param name="key">The key to delete.</param>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <returns><see langword="true"/> if the key was removed.</returns>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    /// <remarks>
    /// This is the blocking form of the delete operation; reclaiming memory occurs synchronously with the call.
    /// For a non-blocking variant that reclaims memory asynchronously, see <see cref="KeyUnlinkAsync(ValkeyKey, CommandFlags)"/>.
    /// StackExchange.Redis's <see cref="CommandFlags.FireAndForget"/> is not currently supported by GLIDE.
    /// <example>
    /// <code>
    /// var removed = await db.KeyDeleteAsync("key");  // true
    /// </code>
    /// </example>
    /// </remarks>
    Task<bool> KeyDeleteAsync(ValkeyKey key, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Removes the specified keys from the database using the DEL command.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/del/">Valkey commands – DEL</seealso>
    /// <param name="keys">The keys to delete.</param>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <returns>The number of keys that were removed.</returns>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    /// <remarks>
    /// This is the blocking form of the delete operation; reclaiming memory occurs synchronously with the call.
    /// For a non-blocking variant that reclaims memory asynchronously, see <see cref="KeyUnlinkAsync(IEnumerable{ValkeyKey}, CommandFlags)"/>.
    /// StackExchange.Redis's <see cref="CommandFlags.FireAndForget"/> is not currently supported by GLIDE.
    /// <example>
    /// <code>
    /// var removed = await db.KeyDeleteAsync(["key1", "key2"]);  // 2
    /// </code>
    /// </example>
    /// </remarks>
    Task<long> KeyDeleteAsync(IEnumerable<ValkeyKey> keys, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Removes the specified key from the database using the UNLINK command (non-blocking).
    /// </summary>
    /// <seealso href="https://valkey.io/commands/unlink/">Valkey commands – UNLINK</seealso>
    /// <param name="key">The key to unlink.</param>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <returns><see langword="true"/> if the key was unlinked.</returns>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    /// <remarks>
    /// This is a non-blocking variant of the delete operation; reclaiming memory asynchronously on a background thread.
    /// Use <see cref="KeyDeleteAsync(ValkeyKey, CommandFlags)"/> for the blocking form.
    /// StackExchange.Redis's <see cref="CommandFlags.FireAndForget"/> is not currently supported by GLIDE.
    /// <example>
    /// <code>
    /// var unlinked = await db.KeyUnlinkAsync("key");  // true
    /// </code>
    /// </example>
    /// </remarks>
    Task<bool> KeyUnlinkAsync(ValkeyKey key, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Removes the specified keys from the database using the UNLINK command (non-blocking).
    /// </summary>
    /// <seealso href="https://valkey.io/commands/unlink/">Valkey commands – UNLINK</seealso>
    /// <param name="keys">The keys to unlink.</param>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <returns>The number of keys that were unlinked.</returns>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    /// <remarks>
    /// This is a non-blocking variant of the delete operation; reclaiming memory asynchronously on a background thread.
    /// Use <see cref="KeyDeleteAsync(IEnumerable{ValkeyKey}, CommandFlags)"/> for the blocking form.
    /// StackExchange.Redis's <see cref="CommandFlags.FireAndForget"/> is not currently supported by GLIDE.
    /// <example>
    /// <code>
    /// var unlinked = await db.KeyUnlinkAsync(["key1", "key2"]);  // 2
    /// </code>
    /// </example>
    /// </remarks>
    Task<long> KeyUnlinkAsync(IEnumerable<ValkeyKey> keys, CommandFlags flags = CommandFlags.None);

    /// <inheritdoc cref="IBaseClient.ExistsAsync(ValkeyKey)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<bool> KeyExistsAsync(ValkeyKey key, CommandFlags flags = CommandFlags.None);

    /// <inheritdoc cref="IBaseClient.ExistsAsync(IEnumerable{ValkeyKey})"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<long> KeyExistsAsync(IEnumerable<ValkeyKey> keys, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Sets a timeout on key. After the timeout has expired, the key will automatically be deleted.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/expire/">Valkey commands – EXPIRE</seealso>
    /// <param name="key">The key to expire.</param>
    /// <param name="expiry">Duration for the key to expire.</param>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <returns><see langword="true"/> if the timeout was set.</returns>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<bool> KeyExpireAsync(ValkeyKey key, TimeSpan? expiry, CommandFlags flags = CommandFlags.None);

    /// <inheritdoc cref="KeyExpireAsync(ValkeyKey, TimeSpan?, CommandFlags)"/>
    /// <param name="when">The condition for setting the expiry.</param>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<bool> KeyExpireAsync(ValkeyKey key, TimeSpan? expiry, ExpireWhen when, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Sets a timeout on key using an absolute timestamp.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/expireat/">Valkey commands – EXPIREAT</seealso>
    /// <param name="key">The key to expire.</param>
    /// <param name="expiry">The timestamp for expiry.</param>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <returns><see langword="true"/> if the timeout was set.</returns>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<bool> KeyExpireAsync(ValkeyKey key, DateTime? expiry, CommandFlags flags = CommandFlags.None);

    /// <inheritdoc cref="KeyExpireAsync(ValkeyKey, DateTime?, CommandFlags)"/>
    /// <param name="when">The condition for setting the expiry.</param>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<bool> KeyExpireAsync(ValkeyKey key, DateTime? expiry, ExpireWhen when, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Returns the remaining time to live of a key that has a timeout.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/pttl/">Valkey commands – PTTL</seealso>
    /// <param name="key">The key to return its timeout.</param>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <returns>TTL, or <see langword="null"/> when key does not exist or key exists but has no associated expiration.</returns>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<TimeSpan?> KeyTimeToLiveAsync(ValkeyKey key, CommandFlags flags = CommandFlags.None);

    /// <inheritdoc cref="IBaseClient.TypeAsync(ValkeyKey)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<ValkeyType> KeyTypeAsync(ValkeyKey key, CommandFlags flags = CommandFlags.None);

    /// <inheritdoc cref="IBaseClient.RenameAsync(ValkeyKey, ValkeyKey)" path="/summary"/>
    /// <inheritdoc cref="IBaseClient.RenameAsync(ValkeyKey, ValkeyKey)" path="/seealso"/>
    /// <param name="key">The key to rename.</param>
    /// <param name="newKey">The new name of the key.</param>
    /// <returns><see langword="true"/> if the key was renamed (always true on success, throws on failure).</returns>
    Task<bool> KeyRenameAsync(ValkeyKey key, ValkeyKey newKey);

    /// <inheritdoc cref="IBaseClient.RenameAsync(ValkeyKey, ValkeyKey)" path="/summary"/>
    /// <inheritdoc cref="IBaseClient.RenameAsync(ValkeyKey, ValkeyKey)" path="/seealso"/>
    /// <param name="key">The key to rename.</param>
    /// <param name="newKey">The new name of the key.</param>
    /// <param name="when">Under what condition the key should be renamed. Only <see cref="When.Always"/> and <see cref="When.NotExists"/> are supported.</param>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <returns><see langword="true"/> if the key was renamed, <see langword="false"/> if newKey already exists (when <paramref name="when"/> is <see cref="When.NotExists"/>).</returns>
    /// <exception cref="ArgumentException">Thrown if <paramref name="when"/> is <see cref="When.Exists"/>.</exception>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<bool> KeyRenameAsync(ValkeyKey key, ValkeyKey newKey, When when = When.Always, CommandFlags flags = CommandFlags.None);

    /// <inheritdoc cref="IBaseClient.RenameIfNotExistsAsync(ValkeyKey, ValkeyKey)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<bool> KeyRenameNXAsync(ValkeyKey key, ValkeyKey newKey, CommandFlags flags = CommandFlags.None);

    /// <inheritdoc cref="IBaseClient.PersistAsync(ValkeyKey)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<bool> KeyPersistAsync(ValkeyKey key, CommandFlags flags = CommandFlags.None);

    /// <inheritdoc cref="IBaseClient.DumpAsync(ValkeyKey)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<byte[]?> KeyDumpAsync(ValkeyKey key, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Creates a key associated with a value obtained by deserializing the provided serialized value.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/restore/">Valkey commands – RESTORE</seealso>
    /// <param name="key">The key to create.</param>
    /// <param name="value">The serialized value to deserialize and assign to key.</param>
    /// <param name="expiry">The expiry to set as a duration.</param>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task KeyRestoreAsync(ValkeyKey key, byte[] value, TimeSpan? expiry = null, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Creates a key associated with a value obtained by deserializing the provided serialized value.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/restore/">Valkey commands – RESTORE</seealso>
    /// <param name="key">The key to create.</param>
    /// <param name="value">The serialized value to deserialize and assign to key.</param>
    /// <param name="expiry">The expiry to set as an absolute timestamp.</param>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task KeyRestoreAsync(ValkeyKey key, byte[] value, DateTime? expiry, CommandFlags flags = CommandFlags.None);

    /// <inheritdoc cref="IBaseClient.TouchAsync(ValkeyKey)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<bool> KeyTouchAsync(ValkeyKey key, CommandFlags flags = CommandFlags.None);

    /// <inheritdoc cref="IBaseClient.TouchAsync(IEnumerable{ValkeyKey})"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<long> KeyTouchAsync(IEnumerable<ValkeyKey> keys, CommandFlags flags = CommandFlags.None);

    /// <inheritdoc cref="IBaseClient.ExpireTimeAsync(ValkeyKey)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<DateTime?> KeyExpireTimeAsync(ValkeyKey key, CommandFlags flags = CommandFlags.None);

    /// <inheritdoc cref="IBaseClient.ObjectEncodingAsync(ValkeyKey)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<string?> KeyEncodingAsync(ValkeyKey key, CommandFlags flags = CommandFlags.None);

    /// <inheritdoc cref="IBaseClient.ObjectFrequencyAsync(ValkeyKey)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<long?> KeyFrequencyAsync(ValkeyKey key, CommandFlags flags = CommandFlags.None);

    /// <inheritdoc cref="IBaseClient.ObjectIdleTimeAsync(ValkeyKey)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<TimeSpan?> KeyIdleTimeAsync(ValkeyKey key, CommandFlags flags = CommandFlags.None);

    /// <inheritdoc cref="IBaseClient.ObjectRefCountAsync(ValkeyKey)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<long?> KeyRefCountAsync(ValkeyKey key, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Copies the value stored at the source to the destination key.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/copy/">Valkey commands – COPY</seealso>
    /// <param name="sourceKey">The key to the source value.</param>
    /// <param name="destinationKey">The key where the value should be copied to.</param>
    /// <param name="destinationDatabase">The database ID to store destinationKey in. A value of -1 means the current database.</param>
    /// <param name="replace">Whether to overwrite an existing value at destinationKey.</param>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <returns><see langword="true"/> if sourceKey was copied. <see langword="false"/> if sourceKey was not copied.</returns>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<bool> KeyCopyAsync(ValkeyKey sourceKey, ValkeyKey destinationKey, int destinationDatabase = -1, bool replace = false, CommandFlags flags = CommandFlags.None);

    /// <inheritdoc cref="IGlideClient.MoveAsync(ValkeyKey, int)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<bool> KeyMoveAsync(ValkeyKey key, int database, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Returns a random key from the database.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/randomkey/">Valkey commands – RANDOMKEY</seealso>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <returns>A random key, or default when the database is empty.</returns>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<ValkeyKey> KeyRandomAsync(CommandFlags flags = CommandFlags.None);

    /// <inheritdoc cref="IGenericBaseCommands.SortAsync(ValkeyKey, long, long, Order, SortType, ValkeyValue, IEnumerable{ValkeyValue})"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<ValkeyValue[]> SortAsync(
        ValkeyKey key,
        long skip,
        long take,
        Order order,
        SortType sortType,
        ValkeyValue by,
        IEnumerable<ValkeyValue>? get,
        CommandFlags flags);

    /// <inheritdoc cref="IGenericBaseCommands.SortAndStoreAsync(ValkeyKey, ValkeyKey, long, long, Order, SortType, ValkeyValue, IEnumerable{ValkeyValue})"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<long> SortAndStoreAsync(
        ValkeyKey destination,
        ValkeyKey key,
        long skip,
        long take,
        Order order,
        SortType sortType,
        ValkeyValue by,
        IEnumerable<ValkeyValue>? get,
        CommandFlags flags);
}
