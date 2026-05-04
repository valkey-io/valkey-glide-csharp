// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands;
using Valkey.Glide.Commands.Options;
using Valkey.Glide.Internals;

namespace Valkey.Glide;

public abstract partial class BaseClient
{
    /// <inheritdoc cref="ISetBaseCommands.SetAddAsync(ValkeyKey, ValkeyValue)"/>
    public async Task<bool> SetAddAsync(ValkeyKey key, ValkeyValue value)
        => await Command(Request.SetAddAsync(key, value));

    /// <inheritdoc cref="ISetBaseCommands.SetAddAsync(ValkeyKey, IEnumerable{ValkeyValue})"/>
    public async Task<long> SetAddAsync(ValkeyKey key, IEnumerable<ValkeyValue> values)
        => await Command(Request.SetAddAsync(key, values));

    /// <inheritdoc cref="ISetBaseCommands.SetRemoveAsync(ValkeyKey, ValkeyValue)"/>
    public async Task<bool> SetRemoveAsync(ValkeyKey key, ValkeyValue value)
        => await Command(Request.SetRemoveAsync(key, value));

    /// <inheritdoc cref="ISetBaseCommands.SetRemoveAsync(ValkeyKey, IEnumerable{ValkeyValue})"/>
    public async Task<long> SetRemoveAsync(ValkeyKey key, IEnumerable<ValkeyValue> values)
        => await Command(Request.SetRemoveAsync(key, values));

    /// <inheritdoc cref="IBaseClient.SetMembersAsync(ValkeyKey)"/>
    public async Task<ISet<ValkeyValue>> SetMembersAsync(ValkeyKey key)
        => await Command(Request.SetMembersAsync(key));

    /// <inheritdoc cref="IBaseClient.SetCardAsync(ValkeyKey)"/>
    public async Task<long> SetCardAsync(ValkeyKey key)
        => await Command(Request.SetCardAsync(key));

    /// <inheritdoc cref="IBaseClient.SetInterCardAsync(IEnumerable{ValkeyKey}, long)"/>
    public async Task<long> SetInterCardAsync(IEnumerable<ValkeyKey> keys, long limit = 0)
        => await Command(Request.SetInterCardAsync(keys, limit));

    /// <inheritdoc cref="ISetBaseCommands.SetPopAsync(ValkeyKey)"/>
    public async Task<ValkeyValue> SetPopAsync(ValkeyKey key)
        => await Command(Request.SetPopAsync(key));

    /// <inheritdoc cref="IBaseClient.SetPopAsync(ValkeyKey, long)"/>
    public async Task<ISet<ValkeyValue>> SetPopAsync(ValkeyKey key, long count)
        => await Command(Request.SetPopAsync(key, count));

    /// <inheritdoc cref="IBaseClient.SetUnionAsync(IEnumerable{ValkeyKey})"/>
    public async Task<ISet<ValkeyValue>> SetUnionAsync(IEnumerable<ValkeyKey> keys)
        => await Command(Request.SetUnionAsync(keys));

    /// <inheritdoc cref="IBaseClient.SetInterAsync(IEnumerable{ValkeyKey})"/>
    public async Task<ISet<ValkeyValue>> SetInterAsync(IEnumerable<ValkeyKey> keys)
        => await Command(Request.SetInterAsync(keys));

    /// <inheritdoc cref="IBaseClient.SetDiffAsync(IEnumerable{ValkeyKey})"/>
    public async Task<ISet<ValkeyValue>> SetDiffAsync(IEnumerable<ValkeyKey> keys)
        => await Command(Request.SetDiffAsync(keys));

    /// <inheritdoc cref="IBaseClient.SetUnionStoreAsync(ValkeyKey, IEnumerable{ValkeyKey})"/>
    public async Task<long> SetUnionStoreAsync(ValkeyKey destination, IEnumerable<ValkeyKey> keys)
        => await Command(Request.SetUnionStoreAsync(destination, keys));

    /// <inheritdoc cref="IBaseClient.SetInterStoreAsync(ValkeyKey, IEnumerable{ValkeyKey})"/>
    public async Task<long> SetInterStoreAsync(ValkeyKey destination, IEnumerable<ValkeyKey> keys)
        => await Command(Request.SetInterStoreAsync(destination, keys));

    /// <inheritdoc cref="IBaseClient.SetDiffStoreAsync(ValkeyKey, IEnumerable{ValkeyKey})"/>
    public async Task<long> SetDiffStoreAsync(ValkeyKey destination, IEnumerable<ValkeyKey> keys)
        => await Command(Request.SetDiffStoreAsync(destination, keys));

    /// <inheritdoc cref="IBaseClient.SetIsMemberAsync(ValkeyKey, ValkeyValue)"/>
    public async Task<bool> SetIsMemberAsync(ValkeyKey key, ValkeyValue value)
        => await Command(Request.SetIsMemberAsync(key, value));

    /// <inheritdoc cref="IBaseClient.SetIsMemberAsync(ValkeyKey, IEnumerable{ValkeyValue})"/>
    public async Task<bool[]> SetIsMemberAsync(ValkeyKey key, IEnumerable<ValkeyValue> values)
        => await Command(Request.SetIsMemberAsync(key, values));

    /// <inheritdoc cref="ISetBaseCommands.SetRandomMemberAsync(ValkeyKey)"/>
    public async Task<ValkeyValue> SetRandomMemberAsync(ValkeyKey key)
        => await Command(Request.SetRandomMemberAsync(key));

    /// <inheritdoc cref="ISetBaseCommands.SetRandomMembersAsync(ValkeyKey, long)"/>
    public async Task<ValkeyValue[]> SetRandomMembersAsync(ValkeyKey key, long count)
        => await Command(Request.SetRandomMembersAsync(key, count));

    /// <inheritdoc cref="ISetBaseCommands.SetMoveAsync(ValkeyKey, ValkeyKey, ValkeyValue)"/>
    public async Task<bool> SetMoveAsync(ValkeyKey source, ValkeyKey destination, ValkeyValue value)
        => await Command(Request.SetMoveAsync(source, destination, value));

    // TODO #287
    /// <inheritdoc cref="IBaseClient.SetScanAsync(ValkeyKey, ScanOptions?)"/>
    public async IAsyncEnumerable<ValkeyValue> SetScanAsync(ValkeyKey key, ScanOptions? options = null)
    {
        long currentCursor = 0;

        do
        {
            (long nextCursor, ValkeyValue[] elements) = await Command(Request.SetScanAsync(key, currentCursor, options));

            foreach (ValkeyValue element in elements)
            {
                yield return element;
            }

            currentCursor = nextCursor;
        } while (currentCursor != 0);
    }
}
