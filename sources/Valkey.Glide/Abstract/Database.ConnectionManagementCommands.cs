// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using System.Diagnostics;

using Valkey.Glide.Internals;

namespace Valkey.Glide;

internal partial class Database
{
    /// <inheritdoc cref="IRedisAsync.PingAsync(CommandFlags)"/>
    public async Task<TimeSpan> PingAsync(CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);

        Stopwatch stopwatch = Stopwatch.StartNew();
        _ = await PingAsync();
        stopwatch.Stop();

        return stopwatch.Elapsed;
    }
}
