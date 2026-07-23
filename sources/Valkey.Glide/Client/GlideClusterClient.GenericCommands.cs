// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands;
using Valkey.Glide.Commands.Options;
using Valkey.Glide.Internals;

using static Valkey.Glide.Route;

namespace Valkey.Glide;

public partial class GlideClusterClient
{
    /// <inheritdoc cref="IGlideClusterClient.ScanAsync(ScanOptions?)"/>
    public async IAsyncEnumerable<ValkeyKey> ScanAsync(ScanOptions? options = null)
    {
        string[] args = [.. Request.ToScanArgs(options).Select(a => a.ToString())];
        ClusterScanCursor cursor = ClusterScanCursor.InitialCursor();

        while (!cursor.IsFinished)
        {
            (cursor, var keys) = await ClusterScanCommand(cursor, args);

            foreach (ValkeyKey key in keys)
            {
                yield return key;
            }
        }
    }

    /// <inheritdoc cref="ITransactionClusterCommands.UnwatchAsync()"/>
    public async Task UnwatchAsync()
        => _ = await Command(Request.Unwatch(), AllPrimaries);

    /// <inheritdoc cref="ITransactionClusterCommands.UnwatchAsync(Route)"/>
    public async Task UnwatchAsync(Route route)
        => _ = await Command(Request.Unwatch(), route);

    /// <inheritdoc cref="IGlideClusterClient.WaitAofAsync(bool, long, TimeSpan, Route)"/>
    public async Task<long[]> WaitAofAsync(bool localAof, long numreplicas, TimeSpan timeout, Route route)
        => await Command(Request.WaitAofAsync(localAof, numreplicas, timeout), route);

    /// <inheritdoc cref="ITransactionBaseCommands.WatchAsync(IEnumerable{ValkeyKey})"/>
    public async Task WatchAsync(IEnumerable<ValkeyKey> keys)
        => _ = await Command(Request.Watch(keys));
}
