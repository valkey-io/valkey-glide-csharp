// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.ServerModules;

namespace Valkey.Glide.ModuleTests;

/// <summary>
/// Shared helpers for module tests. A standalone copy of
/// <c>Valkey.Glide.IntegrationTests.ServerModules.FtUtils.WaitForIndexingAsync</c> (that type is
/// internal to the main integration-test project), kept here rather than referencing it, since
/// this harness deliberately doesn't share a client fixture with the main suite (see
/// <see cref="ModuleTestConfiguration"/>'s remarks).
/// </summary>
internal static class FtUtils
{
    public static readonly TimeSpan MaxDuration = TimeSpan.FromSeconds(10);
    public static readonly TimeSpan RetryInterval = TimeSpan.FromMilliseconds(200);

    /// <summary>
    /// Polls <c>FT.INFO LOCAL</c> until the index is ready, backfill is complete, and the
    /// mutation queue is drained.
    /// </summary>
    public static async Task WaitForIndexingAsync(BaseClient client, string indexName)
    {
        using CancellationTokenSource cts = new(MaxDuration);

        while (!cts.Token.IsCancellationRequested)
        {
            var info = await Ft.InfoLocalAsync(client, indexName);
            if (info.State == Ft.InfoState.Ready && !info.BackfillInProgress && info.MutationQueueSize == 0)
            {
                return;
            }

            await Task.Delay(RetryInterval, cts.Token);
        }

        throw new TimeoutException($"Index '{indexName}' did not finish indexing within {MaxDuration}");
    }
}
