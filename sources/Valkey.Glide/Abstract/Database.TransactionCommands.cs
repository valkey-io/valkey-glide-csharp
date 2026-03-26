// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Internals;

namespace Valkey.Glide;

internal partial class Database
{
    /// <inheritdoc cref="IDatabaseAsync.WatchAsync(IEnumerable{ValkeyKey}, CommandFlags)"/>
    public async Task WatchAsync(IEnumerable<ValkeyKey> keys, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        await WatchAsync(keys);
    }

    /// <inheritdoc cref="IDatabaseAsync.UnwatchAsync(CommandFlags)"/>
    public async Task UnwatchAsync(CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        await UnwatchAsync();
    }

    /// <inheritdoc cref="IDatabaseAsync.UnwatchAsync(Route, CommandFlags)"/>
    public async Task UnwatchAsync(Route route, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        _ = await Command(Request.Unwatch(), route);
    }
}
