// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands;

namespace Valkey.Glide;

/// <summary>
/// Transaction commands with <see cref="CommandFlags"/> for StackExchange.Redis compatibility.
/// </summary>
/// <seealso cref="ITransactionBaseCommands" />
/// <seealso cref="ITransactionCommands" />
/// <seealso cref="ITransactionClusterCommands" />
public partial interface IDatabaseAsync
{
    /// <inheritdoc cref="ITransactionBaseCommands.WatchAsync(IEnumerable{ValkeyKey})"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    Task WatchAsync(IEnumerable<ValkeyKey> keys, CommandFlags flags);

    /// <inheritdoc cref="ITransactionCommands.UnwatchAsync()"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    Task UnwatchAsync(CommandFlags flags);

    /// <inheritdoc cref="ITransactionClusterCommands.UnwatchAsync(Route)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    Task UnwatchAsync(Route route, CommandFlags flags);
}
