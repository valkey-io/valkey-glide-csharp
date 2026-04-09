// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands;
using Valkey.Glide.Commands.Options;

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
    /// <inheritdoc cref="IBaseClient.DeleteAsync(ValkeyKey)"/>
    Task<bool> KeyDeleteAsync(ValkeyKey key);

    /// <inheritdoc cref="IBaseClient.DeleteAsync(ValkeyKey)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<bool> KeyDeleteAsync(ValkeyKey key, CommandFlags flags);

    /// <inheritdoc cref="IBaseClient.DeleteAsync(IEnumerable{ValkeyKey})"/>
    Task<long> KeyDeleteAsync(IEnumerable<ValkeyKey> keys);

    /// <inheritdoc cref="IBaseClient.DeleteAsync(IEnumerable{ValkeyKey})"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<long> KeyDeleteAsync(IEnumerable<ValkeyKey> keys, CommandFlags flags);

    /// <inheritdoc cref="IBaseClient.UnlinkAsync(ValkeyKey)"/>
    Task<bool> KeyUnlinkAsync(ValkeyKey key);

    /// <inheritdoc cref="IBaseClient.UnlinkAsync(ValkeyKey)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<bool> KeyUnlinkAsync(ValkeyKey key, CommandFlags flags);

    /// <inheritdoc cref="IBaseClient.UnlinkAsync(IEnumerable{ValkeyKey})"/>
    Task<long> KeyUnlinkAsync(IEnumerable<ValkeyKey> keys);

    /// <inheritdoc cref="IBaseClient.UnlinkAsync(IEnumerable{ValkeyKey})"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<long> KeyUnlinkAsync(IEnumerable<ValkeyKey> keys, CommandFlags flags);

    /// <inheritdoc cref="IBaseClient.ExistsAsync(ValkeyKey)"/>
    Task<bool> KeyExistsAsync(ValkeyKey key);

    /// <inheritdoc cref="IBaseClient.ExistsAsync(ValkeyKey)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<bool> KeyExistsAsync(ValkeyKey key, CommandFlags flags);

    /// <inheritdoc cref="IBaseClient.ExistsAsync(IEnumerable{ValkeyKey})"/>
    Task<long> KeyExistsAsync(IEnumerable<ValkeyKey> keys);

    /// <inheritdoc cref="IBaseClient.ExistsAsync(IEnumerable{ValkeyKey})"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<long> KeyExistsAsync(IEnumerable<ValkeyKey> keys, CommandFlags flags);

    /// <inheritdoc cref="IBaseClient.ExpireAsync(ValkeyKey, TimeSpan?)"/>
    Task<bool> KeyExpireAsync(ValkeyKey key, TimeSpan? expiry);

    /// <inheritdoc cref="IBaseClient.ExpireAsync(ValkeyKey, TimeSpan?)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<bool> KeyExpireAsync(ValkeyKey key, TimeSpan? expiry, CommandFlags flags);

    /// <inheritdoc cref="IBaseClient.ExpireAsync(ValkeyKey, TimeSpan?, ExpireWhen)"/>
    Task<bool> KeyExpireAsync(ValkeyKey key, TimeSpan? expiry, ExpireWhen when);

    /// <inheritdoc cref="IBaseClient.ExpireAsync(ValkeyKey, TimeSpan?, ExpireWhen)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<bool> KeyExpireAsync(ValkeyKey key, TimeSpan? expiry, ExpireWhen when, CommandFlags flags);

    /// <inheritdoc cref="IBaseClient.ExpireAsync(ValkeyKey, DateTime?)"/>
    Task<bool> KeyExpireAsync(ValkeyKey key, DateTime? expiry);

    /// <inheritdoc cref="IBaseClient.ExpireAsync(ValkeyKey, DateTime?)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<bool> KeyExpireAsync(ValkeyKey key, DateTime? expiry, CommandFlags flags);

    /// <inheritdoc cref="IBaseClient.ExpireAsync(ValkeyKey, DateTime?, ExpireWhen)"/>
    Task<bool> KeyExpireAsync(ValkeyKey key, DateTime? expiry, ExpireWhen when);

    /// <inheritdoc cref="IBaseClient.ExpireAsync(ValkeyKey, DateTime?, ExpireWhen)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<bool> KeyExpireAsync(ValkeyKey key, DateTime? expiry, ExpireWhen when, CommandFlags flags);

    /// <inheritdoc cref="IBaseClient.TimeToLiveAsync(ValkeyKey)"/>
    Task<TimeSpan?> KeyTimeToLiveAsync(ValkeyKey key);

    /// <inheritdoc cref="IBaseClient.TimeToLiveAsync(ValkeyKey)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<TimeSpan?> KeyTimeToLiveAsync(ValkeyKey key, CommandFlags flags);

    /// <inheritdoc cref="IBaseClient.TypeAsync(ValkeyKey)"/>
    Task<ValkeyType> KeyTypeAsync(ValkeyKey key);

    /// <inheritdoc cref="IBaseClient.TypeAsync(ValkeyKey)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<ValkeyType> KeyTypeAsync(ValkeyKey key, CommandFlags flags);

    /// <inheritdoc cref="IBaseClient.RenameAsync(ValkeyKey, ValkeyKey)"/>
    Task KeyRenameAsync(ValkeyKey key, ValkeyKey newKey);

    /// <inheritdoc cref="IBaseClient.RenameAsync(ValkeyKey, ValkeyKey)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <returns><see langword="true"/> always (for SER compatibility).</returns>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<bool> KeyRenameAsync(ValkeyKey key, ValkeyKey newKey, CommandFlags flags);

    /// <inheritdoc cref="IBaseClient.RenameNXAsync(ValkeyKey, ValkeyKey)"/>
    Task<bool> KeyRenameNXAsync(ValkeyKey key, ValkeyKey newKey);

    /// <inheritdoc cref="IBaseClient.RenameNXAsync(ValkeyKey, ValkeyKey)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<bool> KeyRenameNXAsync(ValkeyKey key, ValkeyKey newKey, CommandFlags flags);

    /// <inheritdoc cref="IBaseClient.PersistAsync(ValkeyKey)"/>
    Task<bool> KeyPersistAsync(ValkeyKey key);

    /// <inheritdoc cref="IBaseClient.PersistAsync(ValkeyKey)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<bool> KeyPersistAsync(ValkeyKey key, CommandFlags flags);

    /// <inheritdoc cref="IBaseClient.DumpAsync(ValkeyKey)"/>
    Task<byte[]?> KeyDumpAsync(ValkeyKey key);

    /// <inheritdoc cref="IBaseClient.DumpAsync(ValkeyKey)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<byte[]?> KeyDumpAsync(ValkeyKey key, CommandFlags flags);

    /// <inheritdoc cref="IBaseClient.RestoreAsync(ValkeyKey, byte[], TimeSpan?, RestoreOptions?)"/>
    Task KeyRestoreAsync(ValkeyKey key, byte[] value, TimeSpan? expiry = null, RestoreOptions? restoreOptions = null);

    /// <inheritdoc cref="IBaseClient.RestoreAsync(ValkeyKey, byte[], TimeSpan?, RestoreOptions?)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task KeyRestoreAsync(ValkeyKey key, byte[] value, TimeSpan? expiry, RestoreOptions? restoreOptions, CommandFlags flags);

    /// <inheritdoc cref="IBaseClient.RestoreDateTimeAsync(ValkeyKey, byte[], DateTime?, RestoreOptions?)"/>
    Task KeyRestoreDateTimeAsync(ValkeyKey key, byte[] value, DateTime? expiry = null, RestoreOptions? restoreOptions = null);

    /// <inheritdoc cref="IBaseClient.RestoreDateTimeAsync(ValkeyKey, byte[], DateTime?, RestoreOptions?)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task KeyRestoreDateTimeAsync(ValkeyKey key, byte[] value, DateTime? expiry, RestoreOptions? restoreOptions, CommandFlags flags);

    /// <inheritdoc cref="IBaseClient.TouchAsync(ValkeyKey)"/>
    Task<bool> KeyTouchAsync(ValkeyKey key);

    /// <inheritdoc cref="IBaseClient.TouchAsync(ValkeyKey)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<bool> KeyTouchAsync(ValkeyKey key, CommandFlags flags);

    /// <inheritdoc cref="IBaseClient.TouchAsync(IEnumerable{ValkeyKey})"/>
    Task<long> KeyTouchAsync(IEnumerable<ValkeyKey> keys);

    /// <inheritdoc cref="IBaseClient.TouchAsync(IEnumerable{ValkeyKey})"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<long> KeyTouchAsync(IEnumerable<ValkeyKey> keys, CommandFlags flags);

    /// <inheritdoc cref="IBaseClient.ExpireTimeAsync(ValkeyKey)"/>
    Task<DateTime?> KeyExpireTimeAsync(ValkeyKey key);

    /// <inheritdoc cref="IBaseClient.ExpireTimeAsync(ValkeyKey)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<DateTime?> KeyExpireTimeAsync(ValkeyKey key, CommandFlags flags);

    /// <inheritdoc cref="IBaseClient.ObjectEncodingAsync(ValkeyKey)"/>
    Task<string?> KeyEncodingAsync(ValkeyKey key);

    /// <inheritdoc cref="IBaseClient.ObjectEncodingAsync(ValkeyKey)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<string?> KeyEncodingAsync(ValkeyKey key, CommandFlags flags);

    /// <inheritdoc cref="IBaseClient.ObjectFrequencyAsync(ValkeyKey)"/>
    Task<long?> KeyFrequencyAsync(ValkeyKey key);

    /// <inheritdoc cref="IBaseClient.ObjectFrequencyAsync(ValkeyKey)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<long?> KeyFrequencyAsync(ValkeyKey key, CommandFlags flags);

    /// <inheritdoc cref="IBaseClient.ObjectIdleTimeAsync(ValkeyKey)"/>
    Task<long?> KeyIdleTimeAsync(ValkeyKey key);

    /// <inheritdoc cref="IBaseClient.ObjectIdleTimeAsync(ValkeyKey)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<long?> KeyIdleTimeAsync(ValkeyKey key, CommandFlags flags);

    /// <inheritdoc cref="IBaseClient.ObjectRefCountAsync(ValkeyKey)"/>
    Task<long?> KeyRefCountAsync(ValkeyKey key);

    /// <inheritdoc cref="IBaseClient.ObjectRefCountAsync(ValkeyKey)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<long?> KeyRefCountAsync(ValkeyKey key, CommandFlags flags);

    /// <inheritdoc cref="IBaseClient.CopyAsync(ValkeyKey, ValkeyKey, bool)"/>
    Task<bool> KeyCopyAsync(ValkeyKey sourceKey, ValkeyKey destinationKey, bool replace = false);

    /// <inheritdoc cref="IBaseClient.CopyAsync(ValkeyKey, ValkeyKey, bool)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<bool> KeyCopyAsync(ValkeyKey sourceKey, ValkeyKey destinationKey, bool replace, CommandFlags flags);

    /// <inheritdoc cref="IBaseClient.MoveAsync(ValkeyKey, int)"/>
    Task<bool> KeyMoveAsync(ValkeyKey key, int database);

    /// <inheritdoc cref="IBaseClient.MoveAsync(ValkeyKey, int)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<bool> KeyMoveAsync(ValkeyKey key, int database, CommandFlags flags);

    /// <inheritdoc cref="IBaseClient.CopyAsync(ValkeyKey, ValkeyKey, int, bool)"/>
    Task<bool> KeyCopyAsync(ValkeyKey sourceKey, ValkeyKey destinationKey, int destinationDatabase, bool replace = false);

    /// <inheritdoc cref="IBaseClient.CopyAsync(ValkeyKey, ValkeyKey, int, bool)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<bool> KeyCopyAsync(ValkeyKey sourceKey, ValkeyKey destinationKey, int destinationDatabase, bool replace, CommandFlags flags);

    /// <inheritdoc cref="IBaseClient.RandomKeyAsync()"/>
    Task<string?> KeyRandomAsync();

    /// <inheritdoc cref="IBaseClient.RandomKeyAsync()"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<string?> KeyRandomAsync(CommandFlags flags);

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
}
