// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands;
using Valkey.Glide.Commands.Options;
using Valkey.Glide.Internals;

namespace Valkey.Glide;

public partial class GlideClient
{
    /// <inheritdoc cref="IGlideClient.MigrateAsync(IEnumerable{ValkeyKey}, MigrateOptions)"/>
    public async Task<bool> MigrateAsync(IEnumerable<ValkeyKey> keys, MigrateOptions options)
        => await Command(Request.MigrateAsync(keys, options));

    /// <inheritdoc/>
    public async Task<(string cursor, ValkeyKey[] keys)> ScanAsync(string cursor, ScanOptions? options = null)
        => await Command(Request.ScanAsync(cursor, options));

    /// <inheritdoc cref="IGlideClient.ScanAsync(ScanOptions?)"/>
    public async IAsyncEnumerable<ValkeyKey> ScanAsync(ScanOptions? options = null)
    {
        string currentCursor = "0";

        do
        {
            (string nextCursor, ValkeyKey[] keys) = await Command(Request.ScanAsync(currentCursor, options));

            foreach (ValkeyKey key in keys)
            {
                yield return key;
            }

            currentCursor = nextCursor;
        } while (currentCursor != "0");
    }

    /// <inheritdoc cref="ITransactionCommands.UnwatchAsync()"/>
    public async Task UnwatchAsync()
        => _ = await Command(Request.Unwatch());

    /// <inheritdoc cref="ITransactionBaseCommands.WatchAsync(IEnumerable{ValkeyKey})"/>
    public async Task WatchAsync(IEnumerable<ValkeyKey> keys)
        => _ = await Command(Request.Watch(keys));
}
