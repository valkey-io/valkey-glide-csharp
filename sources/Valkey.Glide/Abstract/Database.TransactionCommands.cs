// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands;
using Valkey.Glide.Internals;

namespace Valkey.Glide;

/// <summary>
/// Transaction commands with <see cref="CommandFlags"/> for StackExchange.Redis compatibility.
/// </summary>
/// <seealso cref="ITransactionBaseCommands" />
internal partial class Database
{
    /// <inheritdoc cref="ITransactionBaseCommands.WatchAsync(IEnumerable{ValkeyKey})"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task WatchAsync(IEnumerable<ValkeyKey> keys, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        await WatchAsync(keys);
    }

    /// <inheritdoc cref="ITransactionCommands.UnwatchAsync()"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task UnwatchAsync(CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        await UnwatchAsync();
    }

    /// <inheritdoc cref="ITransactionClusterCommands.UnwatchAsync(Route)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task UnwatchAsync(Route route, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        _ = await Command(Request.Unwatch(), route);
    }
}
