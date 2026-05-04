// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.ServerModules;

namespace Valkey.Glide.IntegrationTests.SearchModules;

/// <summary>
/// Utility methods for search integration tests.
/// </summary>
public static class FtUtils
{
    /// <summary>
    /// Maximum duration to wait for index operations.
    /// </summary>
    public static readonly TimeSpan MaxDuration = TimeSpan.FromSeconds(10);

    /// <summary>
    /// Retry interval when polling index state.
    /// </summary>
    public static readonly TimeSpan RetryInterval = TimeSpan.FromMilliseconds(200);

    /// <summary>
    /// Polls <c>FT.INFO LOCAL</c> until the index is ready, backfill is complete,
    /// and the mutation queue is drained.
    /// </summary>
    /// <param name="client">The client to execute the command.</param>
    /// <param name="indexName">The name of the index to wait for.</param>
    /// <exception cref="TimeoutException">If the index does not finish indexing within <see cref="MaxDuration"/>.</exception>
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
