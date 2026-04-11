// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands;

namespace Valkey.Glide;

/// <summary>
/// Generic commands with StackExchange.Redis-compatible naming (Key* prefix) and <see cref="CommandFlags"/> support.
/// </summary>
/// <remarks>
/// These methods use StackExchange.Redis naming conventions. For Valkey GLIDE-style
/// methods without "Key" prefix, use <see cref="IBaseClient"/>.
/// </remarks>
/// <seealso href="https://valkey.io/commands/#generic">Valkey – Generic Commands</seealso>
public partial interface IDatabaseAsync
{
    /// <summary>
    /// Removes the specified key from the database using the DEL command.
    /// </summary>
    /// <remarks>
    /// In StackExchange.Redis, <c>KeyDeleteAsync</c> with <c>CommandFlags.FireAndForget</c> uses UNLINK instead of DEL.
    /// Since GLIDE does not support command flags, use <see cref="KeyUnlinkAsync(ValkeyKey, CommandFlags)"/> directly if you want
    /// non-blocking deletion (UNLINK).
    /// </remarks>
    /// <seealso href="https://valkey.io/commands/del"/>
    /// <seealso cref="KeyUnlinkAsync(ValkeyKey, CommandFlags)"/>
    /// <param name="key">The key to delete.</param>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <returns><see langword="true"/> if the key was removed.</returns>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<bool> KeyDeleteAsync(ValkeyKey key, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Removes the specified keys from the database using the DEL command.
    /// </summary>
    /// <remarks>
    /// In StackExchange.Redis, <c>KeyDeleteAsync</c> with <c>CommandFlags.FireAndForget</c> uses UNLINK instead of DEL.
    /// Since GLIDE does not support command flags, use <see cref="KeyUnlinkAsync(IEnumerable{ValkeyKey}, CommandFlags)"/> directly if you want
    /// non-blocking deletion (UNLINK).
    /// </remarks>
    /// <seealso href="https://valkey.io/commands/del"/>
    /// <seealso cref="KeyUnlinkAsync(IEnumerable{ValkeyKey}, CommandFlags)"/>
    /// <param name="keys">The keys to delete.</param>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <returns>The number of keys that were removed.</returns>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<long> KeyDeleteAsync(IEnumerable<ValkeyKey> keys, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Removes the specified key from the database using the UNLINK command (non-blocking).
    /// </summary>
    /// <remarks>
    /// UNLINK is similar to DEL but performs the actual memory reclaiming in a background thread,
    /// making it non-blocking. This is the command used by StackExchange.Redis when
    /// <c>KeyDeleteAsync</c> is called with <c>CommandFlags.FireAndForget</c>.
    /// </remarks>
    /// <seealso href="https://valkey.io/commands/unlink"/>
    /// <seealso cref="KeyDeleteAsync(ValkeyKey, CommandFlags)"/>
    /// <param name="key">The key to unlink.</param>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <returns><see langword="true"/> if the key was unlinked.</returns>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<bool> KeyUnlinkAsync(ValkeyKey key, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Removes the specified keys from the database using the UNLINK command (non-blocking).
    /// </summary>
    /// <remarks>
    /// UNLINK is similar to DEL but performs the actual memory reclaiming in a background thread,
    /// making it non-blocking. This is the command used by StackExchange.Redis when
    /// <c>KeyDeleteAsync</c> is called with <c>CommandFlags.FireAndForget</c>.
    /// </remarks>
    /// <seealso href="https://valkey.io/commands/unlink"/>
    /// <seealso cref="KeyDeleteAsync(IEnumerable{ValkeyKey}, CommandFlags)"/>
    /// <param name="keys">The keys to unlink.</param>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <returns>The number of keys that were unlinked.</returns>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<long> KeyUnlinkAsync(IEnumerable<ValkeyKey> keys, CommandFlags flags = CommandFlags.None);

    /// <inheritdoc cref="IBaseClient.ExistsAsync(ValkeyKey)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<bool> KeyExistsAsync(ValkeyKey key, CommandFlags flags = CommandFlags.None);

    /// <inheritdoc cref="IBaseClient.ExistsAsync(IEnumerable{ValkeyKey})"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<long> KeyExistsAsync(IEnumerable<ValkeyKey> keys, CommandFlags flags = CommandFlags.None);

    /// <inheritdoc cref="IBaseClient.ExpireAsync(ValkeyKey, TimeSpan?)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<bool> KeyExpireAsync(ValkeyKey key, TimeSpan? expiry, CommandFlags flags = CommandFlags.None);

    /// <inheritdoc cref="IBaseClient.ExpireAsync(ValkeyKey, TimeSpan?, ExpireWhen)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<bool> KeyExpireAsync(ValkeyKey key, TimeSpan? expiry, ExpireWhen when, CommandFlags flags = CommandFlags.None);

    /// <inheritdoc cref="IBaseClient.ExpireAsync(ValkeyKey, DateTime?)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<bool> KeyExpireAsync(ValkeyKey key, DateTime? expiry, CommandFlags flags = CommandFlags.None);

    /// <inheritdoc cref="IBaseClient.ExpireAsync(ValkeyKey, DateTime?, ExpireWhen)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<bool> KeyExpireAsync(ValkeyKey key, DateTime? expiry, ExpireWhen when, CommandFlags flags = CommandFlags.None);


    /// <summary>
    /// Returns the remaining time to live of a key that has a timeout.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/pttl"/>
    /// <param name="key">The key to return its timeout.</param>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <returns>TTL, or <see langword="null"/> when key does not exist or key exists but has no associated expiration.</returns>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<TimeSpan?> KeyTimeToLiveAsync(ValkeyKey key, CommandFlags flags = CommandFlags.None);

    /// <inheritdoc cref="IBaseClient.TypeAsync(ValkeyKey)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<ValkeyType> KeyTypeAsync(ValkeyKey key, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Renames key to newKey.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/rename"/>
    /// <param name="key">The key to rename.</param>
    /// <param name="newKey">The new name of the key.</param>
    /// <returns><see langword="true"/> if the key was renamed (always true on success, throws on failure).</returns>
    Task<bool> KeyRenameAsync(ValkeyKey key, ValkeyKey newKey);

    /// <summary>
    /// Renames key to newKey.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/rename"/>
    /// <param name="key">The key to rename.</param>
    /// <param name="newKey">The new name of the key.</param>
    /// <param name="when">Under what condition the key should be renamed. Only <see cref="When.Always"/> and <see cref="When.NotExists"/> are supported.</param>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <returns><see langword="true"/> if the key was renamed, <see langword="false"/> if newKey already exists (when <paramref name="when"/> is <see cref="When.NotExists"/>).</returns>
    /// <exception cref="ArgumentException">Thrown if <paramref name="when"/> is <see cref="When.Exists"/>.</exception>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<bool> KeyRenameAsync(ValkeyKey key, ValkeyKey newKey, When when = When.Always, CommandFlags flags = CommandFlags.None);

    /// <inheritdoc cref="IBaseClient.RenameNXAsync(ValkeyKey, ValkeyKey)"/>
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
    /// Creates a key associated with a value that is obtained by deserializing the provided serialized value.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/restore"/>
    /// <param name="key">The key to create.</param>
    /// <param name="value">The serialized value to deserialize and assign to key.</param>
    /// <param name="expiry">The expiry to set as a duration.</param>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task KeyRestoreAsync(ValkeyKey key, byte[] value, TimeSpan? expiry = null, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Creates a key associated with a value that is obtained by deserializing the provided serialized value.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/restore"/>
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
    /// <seealso href="https://valkey.io/commands/copy"/>
    /// <param name="sourceKey">The key to the source value.</param>
    /// <param name="destinationKey">The key where the value should be copied to.</param>
    /// <param name="destinationDatabase">The database ID to store destinationKey in. A value of -1 means the current database.</param>
    /// <param name="replace">Whether to overwrite an existing value at destinationKey.</param>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <returns><see langword="true"/> if sourceKey was copied. <see langword="false"/> if sourceKey was not copied.</returns>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<bool> KeyCopyAsync(ValkeyKey sourceKey, ValkeyKey destinationKey, int destinationDatabase = -1, bool replace = false, CommandFlags flags = CommandFlags.None);

    /// <inheritdoc cref="IBaseClient.MoveAsync(ValkeyKey, int)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<bool> KeyMoveAsync(ValkeyKey key, int database, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Returns a random key from the database.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/randomkey"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <returns>A random key, or default when the database is empty.</returns>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<ValkeyKey> KeyRandomAsync(CommandFlags flags = CommandFlags.None);

    /// <inheritdoc cref="IGenericBaseCommands.SortAsync(ValkeyKey, long, long, Order, SortType, ValkeyValue, IEnumerable{ValkeyValue})"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<ValkeyValue[]> SortAsync(
        ValkeyKey key,
        long skip = 0,
        long take = -1,
        Order order = Order.Ascending,
        SortType sortType = SortType.Numeric,
        ValkeyValue by = default,
        IEnumerable<ValkeyValue>? get = null,
        CommandFlags flags = CommandFlags.None);

    /// <inheritdoc cref="IGenericBaseCommands.SortAndStoreAsync(ValkeyKey, ValkeyKey, long, long, Order, SortType, ValkeyValue, IEnumerable{ValkeyValue})"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<long> SortAndStoreAsync(
        ValkeyKey destination,
        ValkeyKey key,
        long skip = 0,
        long take = -1,
        Order order = Order.Ascending,
        SortType sortType = SortType.Numeric,
        ValkeyValue by = default,
        IEnumerable<ValkeyValue>? get = null,
        CommandFlags flags = CommandFlags.None);
}
