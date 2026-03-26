// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands;
using Valkey.Glide.Commands.Options;

namespace Valkey.Glide;

/// <summary>
/// Generic commands with <see cref="CommandFlags"/> for StackExchange.Redis compatibility.
/// </summary>
/// <seealso cref="IGenericBaseCommands" />
public partial interface IDatabaseAsync
{
    /// <inheritdoc cref="IGenericBaseCommands.KeyDeleteAsync(ValkeyKey)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    Task<bool> KeyDeleteAsync(ValkeyKey key, CommandFlags flags);

    /// <inheritdoc cref="IGenericBaseCommands.KeyDeleteAsync(IEnumerable{ValkeyKey})"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    Task<long> KeyDeleteAsync(IEnumerable<ValkeyKey> keys, CommandFlags flags);

    /// <inheritdoc cref="IGenericBaseCommands.KeyUnlinkAsync(ValkeyKey)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    Task<bool> KeyUnlinkAsync(ValkeyKey key, CommandFlags flags);

    /// <inheritdoc cref="IGenericBaseCommands.KeyUnlinkAsync(IEnumerable{ValkeyKey})"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    Task<long> KeyUnlinkAsync(IEnumerable<ValkeyKey> keys, CommandFlags flags);

    /// <inheritdoc cref="IGenericBaseCommands.KeyExistsAsync(ValkeyKey)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    Task<bool> KeyExistsAsync(ValkeyKey key, CommandFlags flags);

    /// <inheritdoc cref="IGenericBaseCommands.KeyExistsAsync(IEnumerable{ValkeyKey})"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    Task<long> KeyExistsAsync(IEnumerable<ValkeyKey> keys, CommandFlags flags);

    /// <inheritdoc cref="IGenericBaseCommands.KeyExpireAsync(ValkeyKey, TimeSpan?)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    Task<bool> KeyExpireAsync(ValkeyKey key, TimeSpan? expiry, CommandFlags flags);

    /// <inheritdoc cref="IGenericBaseCommands.KeyExpireAsync(ValkeyKey, TimeSpan?, ExpireWhen)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    Task<bool> KeyExpireAsync(ValkeyKey key, TimeSpan? expiry, ExpireWhen when, CommandFlags flags);

    /// <inheritdoc cref="IGenericBaseCommands.KeyExpireAsync(ValkeyKey, DateTime?)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    Task<bool> KeyExpireAsync(ValkeyKey key, DateTime? expiry, CommandFlags flags);

    /// <inheritdoc cref="IGenericBaseCommands.KeyExpireAsync(ValkeyKey, DateTime?, ExpireWhen)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    Task<bool> KeyExpireAsync(ValkeyKey key, DateTime? expiry, ExpireWhen when, CommandFlags flags);

    /// <inheritdoc cref="IGenericBaseCommands.KeyTimeToLiveAsync(ValkeyKey)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    Task<TimeSpan?> KeyTimeToLiveAsync(ValkeyKey key, CommandFlags flags);

    /// <inheritdoc cref="IGenericBaseCommands.KeyTypeAsync(ValkeyKey)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    Task<ValkeyType> KeyTypeAsync(ValkeyKey key, CommandFlags flags);

    /// <inheritdoc cref="IGenericBaseCommands.KeyRenameAsync(ValkeyKey, ValkeyKey)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    Task<bool> KeyRenameAsync(ValkeyKey key, ValkeyKey newKey, CommandFlags flags);

    /// <inheritdoc cref="IGenericBaseCommands.KeyRenameNXAsync(ValkeyKey, ValkeyKey)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    Task<bool> KeyRenameNXAsync(ValkeyKey key, ValkeyKey newKey, CommandFlags flags);

    /// <inheritdoc cref="IGenericBaseCommands.KeyPersistAsync(ValkeyKey)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    Task<bool> KeyPersistAsync(ValkeyKey key, CommandFlags flags);

    /// <inheritdoc cref="IGenericBaseCommands.KeyDumpAsync(ValkeyKey)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    Task<byte[]?> KeyDumpAsync(ValkeyKey key, CommandFlags flags);

    /// <inheritdoc cref="IGenericBaseCommands.KeyRestoreAsync(ValkeyKey, byte[], TimeSpan?, RestoreOptions?)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    Task KeyRestoreAsync(ValkeyKey key, byte[] value, TimeSpan? expiry, RestoreOptions? restoreOptions, CommandFlags flags);

    /// <inheritdoc cref="IGenericBaseCommands.KeyRestoreDateTimeAsync(ValkeyKey, byte[], DateTime?, RestoreOptions?)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    Task KeyRestoreDateTimeAsync(ValkeyKey key, byte[] value, DateTime? expiry, RestoreOptions? restoreOptions, CommandFlags flags);

    /// <inheritdoc cref="IGenericBaseCommands.KeyTouchAsync(ValkeyKey)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    Task<bool> KeyTouchAsync(ValkeyKey key, CommandFlags flags);

    /// <inheritdoc cref="IGenericBaseCommands.KeyTouchAsync(IEnumerable{ValkeyKey})"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    Task<long> KeyTouchAsync(IEnumerable<ValkeyKey> keys, CommandFlags flags);

    /// <inheritdoc cref="IGenericBaseCommands.KeyExpireTimeAsync(ValkeyKey)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    Task<DateTime?> KeyExpireTimeAsync(ValkeyKey key, CommandFlags flags);

    /// <inheritdoc cref="IGenericBaseCommands.KeyEncodingAsync(ValkeyKey)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    Task<string?> KeyEncodingAsync(ValkeyKey key, CommandFlags flags);

    /// <inheritdoc cref="IGenericBaseCommands.KeyFrequencyAsync(ValkeyKey)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    Task<long?> KeyFrequencyAsync(ValkeyKey key, CommandFlags flags);

    /// <inheritdoc cref="IGenericBaseCommands.KeyIdleTimeAsync(ValkeyKey)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    Task<long?> KeyIdleTimeAsync(ValkeyKey key, CommandFlags flags);

    /// <inheritdoc cref="IGenericBaseCommands.KeyRefCountAsync(ValkeyKey)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    Task<long?> KeyRefCountAsync(ValkeyKey key, CommandFlags flags);

    /// <inheritdoc cref="IGenericBaseCommands.KeyCopyAsync(ValkeyKey, ValkeyKey, bool)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    Task<bool> KeyCopyAsync(ValkeyKey sourceKey, ValkeyKey destinationKey, bool replace, CommandFlags flags);

    /// <inheritdoc cref="IGenericBaseCommands.KeyMoveAsync(ValkeyKey, int)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    Task<bool> KeyMoveAsync(ValkeyKey key, int database, CommandFlags flags);

    /// <inheritdoc cref="IGenericBaseCommands.KeyCopyAsync(ValkeyKey, ValkeyKey, int, bool)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    Task<bool> KeyCopyAsync(ValkeyKey sourceKey, ValkeyKey destinationKey, int destinationDatabase, bool replace, CommandFlags flags);

    /// <inheritdoc cref="IGenericBaseCommands.KeyRandomAsync()"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    Task<string?> KeyRandomAsync(CommandFlags flags);

    /// <inheritdoc cref="IGenericBaseCommands.SortAsync(ValkeyKey, long, long, Order, SortType, ValkeyValue, IEnumerable{ValkeyValue}?)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    Task<ValkeyValue[]> SortAsync(ValkeyKey key, long skip, long take, Order order, SortType sortType, ValkeyValue by, IEnumerable<ValkeyValue>? get, CommandFlags flags);

    /// <inheritdoc cref="IGenericBaseCommands.WaitAsync(long, long)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    Task<long> WaitAsync(long numreplicas, long timeout, CommandFlags flags);
}
