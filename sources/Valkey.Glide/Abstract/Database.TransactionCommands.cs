// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Internals;

namespace Valkey.Glide;

internal partial class Database
{
    #region Transaction Commands with CommandFlags (SER Compatibility)

    public async Task WatchAsync(IEnumerable<ValkeyKey> keys, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        await WatchAsync(keys);
    }

    public async Task UnwatchAsync(CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        await UnwatchAsync();
    }

    public async Task UnwatchAsync(Route route, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        _ = await Command(Request.Unwatch(), route);
    }

    #endregion
}
