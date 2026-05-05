// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.ServerModules;

namespace Valkey.Glide.IntegrationTests.ServerModules;

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

    /// <summary>
    /// Creates a index with text, numeric, and tag fiels and a unique prefix,
    /// populates 3 hash documents, waits for indexing, and returns the index name,
    /// prefix, and document keys.
    /// </summary>
    /// <param name="client">The client to execute the commands.</param>
    /// <returns>A tuple containing the index name, prefix, and document keys.</returns>
    public static async Task<(string IndexName, string Prefix, string[] DocKeys)> CreateSearchIndexAsync(
        BaseClient client)
    {
        var index = Guid.NewGuid().ToString();
        var prefix = $"{index}:";
        var tag = $"{{{index}}}";

        await Ft.CreateAsync(client, index,
        [
            new Ft.CreateTextField("title"),
            new Ft.CreateNumericField("price"),
            new Ft.CreateTagField("category"),
        ],
        new Ft.CreateOptions
        {
            DataType = Ft.DataType.Hash,
            Prefixes = [prefix],
        });

        string[] keys =
        [
            $"{prefix}{tag}:1",
            $"{prefix}{tag}:2",
            $"{prefix}{tag}:3",
        ];

        _ = await client.HashSetAsync(keys[0],
        [
            new("title", "Alpha Widget"),
            new("price", "10"),
            new("category", "electronics"),
        ]);

        _ = await client.HashSetAsync(keys[1],
        [
            new("title", "Beta Gadget"),
            new("price", "25"),
            new("category", "electronics"),
        ]);

        _ = await client.HashSetAsync(keys[2],
        [
            new("title", "Gamma Tool"),
            new("price", "50"),
            new("category", "hardware"),
        ]);

        await WaitForIndexingAsync(client, index);

        return (index, prefix, keys);
    }

    /// <summary>
    /// Creates a text+numeric+tag index with a unique prefix, populates 5 hash documents,
    /// waits for indexing, and returns the index name, prefix, and document keys.
    /// </summary>
    /// <param name="client">The client to execute the commands.</param>
    /// <returns>A tuple containing the index name, prefix, and document keys.</returns>
    public static async Task<(string IndexName, string Prefix, string[] DocKeys)> CreateAggregateIndexAsync(
        BaseClient client)
    {
        var index = Guid.NewGuid().ToString();
        var prefix = $"{index}:";
        var tag = $"{{{index}}}";

        await Ft.CreateAsync(client, index,
        [
            new Ft.CreateTextField("title"),
            new Ft.CreateNumericField("price"),
            new Ft.CreateTagField("category"),
        ],
        new Ft.CreateOptions
        {
            DataType = Ft.DataType.Hash,
            Prefixes = [prefix],
        });

        string[] keys =
        [
            $"{prefix}{tag}:1",
            $"{prefix}{tag}:2",
            $"{prefix}{tag}:3",
            $"{prefix}{tag}:4",
            $"{prefix}{tag}:5",
        ];

        _ = await client.HashSetAsync(keys[0],
        [
            new("title", "Alpha Widget"),
            new("price", "10"),
            new("category", "electronics"),
        ]);

        _ = await client.HashSetAsync(keys[1],
        [
            new("title", "Beta Gadget"),
            new("price", "25"),
            new("category", "electronics"),
        ]);

        _ = await client.HashSetAsync(keys[2],
        [
            new("title", "Gamma Tool"),
            new("price", "50"),
            new("category", "hardware"),
        ]);

        _ = await client.HashSetAsync(keys[3],
        [
            new("title", "Delta Device"),
            new("price", "30"),
            new("category", "electronics"),
        ]);

        _ = await client.HashSetAsync(keys[4],
        [
            new("title", "Epsilon Wrench"),
            new("price", "15"),
            new("category", "hardware"),
        ]);

        await WaitForIndexingAsync(client, index);

        return (index, prefix, keys);
    }
}
