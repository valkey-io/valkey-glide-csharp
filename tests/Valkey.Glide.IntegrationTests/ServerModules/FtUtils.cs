// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.ServerModules;
using Valkey.Glide.TestUtils;

namespace Valkey.Glide.IntegrationTests.ServerModules;

/// <summary>
/// Utility methods for search integration tests.
/// </summary>
public static class FtUtils
{
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

        await Ft.CreateAsync(
            client,
            index,
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

        _ = await client.HashSetAsync(
            keys[0],
            [
                    new("title", "Alpha Widget"),
                    new("price", "10"),
                    new("category", "electronics"),
                ]);

        _ = await client.HashSetAsync(
            keys[1],
            [
                    new("title", "Beta Gadget"),
                    new("price", "25"),
                    new("category", "electronics"),
                ]);

        _ = await client.HashSetAsync(
            keys[2],
            [
                    new("title", "Gamma Tool"),
                    new("price", "50"),
                    new("category", "hardware"),
                ]);

        await SearchIndexing.WaitForIndexingAsync(client, index);

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

        await Ft.CreateAsync(
            client,
            index,
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

        _ = await client.HashSetAsync(
            keys[0],
            [
                    new("title", "Alpha Widget"),
                    new("price", "10"),
                    new("category", "electronics"),
                ]);

        _ = await client.HashSetAsync(
            keys[1],
            [
                    new("title", "Beta Gadget"),
                    new("price", "25"),
                    new("category", "electronics"),
                ]);

        _ = await client.HashSetAsync(
            keys[2],
            [
                    new("title", "Gamma Tool"),
                    new("price", "50"),
                    new("category", "hardware"),
                ]);

        _ = await client.HashSetAsync(
            keys[3],
            [
                    new("title", "Delta Device"),
                    new("price", "30"),
                    new("category", "electronics"),
                ]);

        _ = await client.HashSetAsync(
            keys[4],
            [
                    new("title", "Epsilon Wrench"),
                    new("price", "15"),
                    new("category", "hardware"),
                ]);

        await SearchIndexing.WaitForIndexingAsync(client, index);

        return (index, prefix, keys);
    }
}
