// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands.Options;
using Valkey.Glide.Pipeline;

namespace Valkey.Glide.IntegrationTests;

/// <summary>
/// Fixture that provides shared compression-enabled clients for tests.
/// </summary>
public class CompressionFixture : IAsyncLifetime
{
    public GlideClient? ZstdClient { get; private set; }
    public GlideClient? Lz4Client { get; private set; }
    public GlideClusterClient? ZstdClusterClient { get; private set; }
    public GlideClusterClient? Lz4ClusterClient { get; private set; }

    public async ValueTask InitializeAsync()
    {
        var zstdConfig = TestConfiguration.DefaultClientConfig()
            .WithCompression(CompressionConfig.Zstd())
            .Build();

        var lz4Config = TestConfiguration.DefaultClientConfig()
            .WithCompression(CompressionConfig.Lz4())
            .Build();

        var zstdClusterConfig = TestConfiguration.DefaultClusterClientConfig()
            .WithCompression(CompressionConfig.Zstd())
            .Build();

        var lz4ClusterConfig = TestConfiguration.DefaultClusterClientConfig()
            .WithCompression(CompressionConfig.Lz4())
            .Build();

        ZstdClient = await GlideClient.CreateClient(zstdConfig);
        Lz4Client = await GlideClient.CreateClient(lz4Config);
        ZstdClusterClient = await GlideClusterClient.CreateClient(zstdClusterConfig);
        Lz4ClusterClient = await GlideClusterClient.CreateClient(lz4ClusterConfig);
    }

    public ValueTask DisposeAsync()
    {
        ZstdClient?.Dispose();
        Lz4Client?.Dispose();
        ZstdClusterClient?.Dispose();
        Lz4ClusterClient?.Dispose();
        return ValueTask.CompletedTask;
    }
}

[CollectionDefinition("CompressionTests")]
public class CompressionTestsCollection : ICollectionFixture<CompressionFixture>;

[Collection("CompressionTests")]
public class CompressionTests(CompressionFixture fixture)
{
    private const int LargeValueSize = 1000;
    private const int SmallValueSize = 100;
    private const int MinSizeThreshold = 500;
    private const int CustomCompressionLevel = 10;
    private const int MultiOpCount = 10;
    private const int MultiOpBaseSize = 500;
    private const int MultiOpSizeIncrement = 100;

    private static readonly string LargeValue = new('y', LargeValueSize);

    private GlideClient ZstdClient => fixture.ZstdClient!;
    private GlideClient Lz4Client => fixture.Lz4Client!;
    private GlideClusterClient ZstdClusterClient => fixture.ZstdClusterClient!;
    private GlideClusterClient Lz4ClusterClient => fixture.Lz4ClusterClient!;

    [Fact]
    public async Task Compression_Zstd_Standalone_CompressesAndDecompresses()
    {
        var statsBefore = BaseClient.GetStatistics();

        string key = $"compression_test_{Guid.NewGuid()}";

        await ZstdClient.SetAsync(key, LargeValue);
        var retrieved = await ZstdClient.GetAsync(key);

        Assert.Equal(LargeValue, retrieved.ToString());

        var statsAfter = BaseClient.GetStatistics();
        Assert.True(statsAfter.TotalValuesCompressed > statsBefore.TotalValuesCompressed, $"Expected compression to occur. Before: {statsBefore.TotalValuesCompressed}, After: {statsAfter.TotalValuesCompressed}");
        Assert.True(statsAfter.TotalBytesCompressed > statsBefore.TotalBytesCompressed, $"Expected bytes compressed. Before: {statsBefore.TotalBytesCompressed}, After: {statsAfter.TotalBytesCompressed}");
    }

    [Fact]
    public async Task Compression_Lz4_Standalone_CompressesAndDecompresses()
    {
        var statsBefore = BaseClient.GetStatistics();

        string key = $"compression_lz4_test_{Guid.NewGuid()}";

        await Lz4Client.SetAsync(key, LargeValue);
        var retrieved = await Lz4Client.GetAsync(key);

        Assert.Equal(LargeValue, retrieved.ToString());

        var statsAfter = BaseClient.GetStatistics();
        Assert.True(statsAfter.TotalValuesCompressed > statsBefore.TotalValuesCompressed);
    }

    [Fact]
    public async Task Compression_Zstd_Cluster_CompressesAndDecompresses()
    {
        var statsBefore = BaseClient.GetStatistics();

        string key = $"compression_cluster_test_{Guid.NewGuid()}";

        await ZstdClusterClient.SetAsync(key, LargeValue);
        var retrieved = await ZstdClusterClient.GetAsync(key);

        Assert.Equal(LargeValue, retrieved.ToString());

        var statsAfter = BaseClient.GetStatistics();
        Assert.True(statsAfter.TotalValuesCompressed > statsBefore.TotalValuesCompressed, $"Expected compression. Before: {statsBefore.TotalValuesCompressed}, After: {statsAfter.TotalValuesCompressed}");
    }

    [Fact]
    public async Task Compression_Lz4_Cluster_CompressesAndDecompresses()
    {
        var statsBefore = BaseClient.GetStatistics();

        string key = $"compression_lz4_cluster_test_{Guid.NewGuid()}";

        await Lz4ClusterClient.SetAsync(key, LargeValue);
        var retrieved = await Lz4ClusterClient.GetAsync(key);

        Assert.Equal(LargeValue, retrieved.ToString());

        var statsAfter = BaseClient.GetStatistics();
        Assert.True(statsAfter.TotalValuesCompressed > statsBefore.TotalValuesCompressed, $"Expected compression. Before: {statsBefore.TotalValuesCompressed}, After: {statsAfter.TotalValuesCompressed}");
    }

    [Fact]
    public async Task Compression_MinSize_SkipsSmallValues()
    {
        // This test needs a custom client with minSize threshold
        var clientConfig = TestConfiguration.DefaultClientConfig()
            .WithCompression(CompressionConfig.Zstd(minCompressionSize: MinSizeThreshold))
            .Build();

        await using var client = await GlideClient.CreateClient(clientConfig);

        var statsBefore = BaseClient.GetStatistics();

        string smallKey = $"small_{Guid.NewGuid()}";
        string smallValue = new('x', SmallValueSize);
        await client.SetAsync(smallKey, smallValue);

        var statsAfterSmall = BaseClient.GetStatistics();
        var skippedCountSmall = statsAfterSmall.CompressionSkippedCount - statsBefore.CompressionSkippedCount;

        string largeKey = $"large_{Guid.NewGuid()}";
        await client.SetAsync(largeKey, LargeValue);

        var statsAfterLarge = BaseClient.GetStatistics();
        var compressedCountLarge = statsAfterLarge.TotalValuesCompressed - statsAfterSmall.TotalValuesCompressed;

        Assert.True(skippedCountSmall > 0, $"Small value should have been skipped. Skipped delta: {skippedCountSmall}");
        Assert.True(compressedCountLarge > 0, $"Large value should have been compressed. Compressed delta: {compressedCountLarge}");
    }

    [Fact]
    public async Task Compression_CustomLevel_WorksCorrectly()
    {
        // This test needs a custom client with custom compression level
        var clientConfig = TestConfiguration.DefaultClientConfig()
            .WithCompression(CompressionConfig.Zstd(compressionLevel: CustomCompressionLevel))
            .Build();

        await using var client = await GlideClient.CreateClient(clientConfig);

        string key = $"compression_level_test_{Guid.NewGuid()}";

        await client.SetAsync(key, LargeValue);
        var retrieved = await client.GetAsync(key);

        Assert.Equal(LargeValue, retrieved.ToString());
    }

    [Fact]
    public async Task Compression_MaxDecompressedSize_WorksCorrectly()
    {
        // Test with custom max decompressed size (1GB)
        var clientConfig = TestConfiguration.DefaultClientConfig()
            .WithCompression(CompressionConfig.Zstd(maxDecompressedSize: 1024 * 1024 * 1024))
            .Build();

        await using var client = await GlideClient.CreateClient(clientConfig);

        string key = $"max_decompressed_size_test_{Guid.NewGuid()}";

        await client.SetAsync(key, LargeValue);
        var retrieved = await client.GetAsync(key);

        Assert.Equal(LargeValue, retrieved.ToString());
    }

    [Fact]
    public async Task Compression_Lz4_MaxDecompressedSize_WorksCorrectly()
    {
        // Test LZ4 with custom max decompressed size
        var clientConfig = TestConfiguration.DefaultClientConfig()
            .WithCompression(CompressionConfig.Lz4(maxDecompressedSize: 512 * 1024 * 1024))
            .Build();

        await using var client = await GlideClient.CreateClient(clientConfig);

        string key = $"lz4_max_decompressed_size_test_{Guid.NewGuid()}";

        await client.SetAsync(key, LargeValue);
        var retrieved = await client.GetAsync(key);

        Assert.Equal(LargeValue, retrieved.ToString());
    }

    [Fact]
    public async Task Compression_AllOptions_WorksCorrectly()
    {
        // Test with all compression options set
        var clientConfig = TestConfiguration.DefaultClientConfig()
            .WithCompression(CompressionConfig.Zstd(
                compressionLevel: 5,
                minCompressionSize: 100,
                maxDecompressedSize: 256 * 1024 * 1024))
            .Build();

        await using var client = await GlideClient.CreateClient(clientConfig);

        string key = $"all_options_test_{Guid.NewGuid()}";

        await client.SetAsync(key, LargeValue);
        var retrieved = await client.GetAsync(key);

        Assert.Equal(LargeValue, retrieved.ToString());
    }

    [Fact]
    public async Task Compression_BackwardCompatibility_ReadsUncompressedData()
    {
        // This test needs a client without compression
        var configNoCompression = TestConfiguration.DefaultClientConfig().Build();

        await using var clientNoCompression = await GlideClient.CreateClient(configNoCompression);

        string key = $"backward_compat_test_{Guid.NewGuid()}";
        string value = "uncompressed_data";

        await clientNoCompression.SetAsync(key, value);

        // Read with compression-enabled client
        var retrieved = await ZstdClient.GetAsync(key);
        Assert.Equal(value, retrieved.ToString());
    }

    [Fact]
    public async Task Compression_MultipleOperations_MaintainsDataIntegrity()
    {
        var testData = new Dictionary<string, string>();
        for (int i = 0; i < MultiOpCount; i++)
        {
            string key = $"multi_op_test_{i}_{Guid.NewGuid()}";
            string value = new((char)('a' + i), MultiOpBaseSize + (i * MultiOpSizeIncrement));
            testData[key] = value;
            await Lz4Client.SetAsync(key, value);
        }

        foreach (var kvp in testData)
        {
            var retrieved = await Lz4Client.GetAsync(kvp.Key);
            Assert.Equal(kvp.Value, retrieved.ToString());
        }
    }

    [Fact]
    public async Task Compression_BinaryData_HandlesCorrectly()
    {
        string key = $"binary_test_{Guid.NewGuid()}";
        byte[] binaryData = new byte[LargeValueSize];
        new Random().NextBytes(binaryData);

        await ZstdClient.SetAsync(key, binaryData);
        var retrieved = await ZstdClient.GetAsync(key);

        Assert.Equal(binaryData, (byte[]?)retrieved);
    }

    [Fact]
    public async Task Compression_Statistics_ReflectOperations()
    {
        var statsBefore = BaseClient.GetStatistics();

        string key = $"stats_test_{Guid.NewGuid()}";

        await ZstdClient.SetAsync(key, LargeValue);
        _ = await ZstdClient.GetAsync(key);

        var statsAfter = BaseClient.GetStatistics();

        Assert.True(statsAfter.TotalValuesCompressed > statsBefore.TotalValuesCompressed, $"Expected compression. Before: {statsBefore.TotalValuesCompressed}, After: {statsAfter.TotalValuesCompressed}");
        Assert.True(statsAfter.TotalOriginalBytes > statsBefore.TotalOriginalBytes, $"Expected original bytes. Before: {statsBefore.TotalOriginalBytes}, After: {statsAfter.TotalOriginalBytes}");
        Assert.True(statsAfter.TotalBytesCompressed > statsBefore.TotalBytesCompressed, $"Expected compressed bytes. Before: {statsBefore.TotalBytesCompressed}, After: {statsAfter.TotalBytesCompressed}");
    }

    [Fact]
    public async Task Compression_MGet_DecompressesValues()
    {
        var keysAndValues = new KeyValuePair<ValkeyKey, ValkeyValue>[5];
        for (int i = 0; i < 5; i++)
        {
            string key = $"mget_test_{i}_{Guid.NewGuid()}";
            string value = new((char)('a' + i), LargeValueSize);
            keysAndValues[i] = new(key, value);
            await ZstdClient.SetAsync(key, value);
        }

        ValkeyKey[] keys = [.. keysAndValues.Select(kv => kv.Key)];
        ValkeyValue[] results = await ZstdClient.GetAsync(keys);

        for (int i = 0; i < 5; i++)
        {
            Assert.Equal(keysAndValues[i].Value.ToString(), results[i].ToString());
        }
    }

    [Fact]
    public async Task Compression_GetEx_DecompressesValue()
    {
        string key = $"getex_test_{Guid.NewGuid()}";

        await ZstdClient.SetAsync(key, LargeValue);
        ValkeyValue retrieved = await ZstdClient.GetExpiryAsync(key, GetExpiryOptions.ExpireIn(TimeSpan.FromSeconds(10)));

        Assert.Equal(LargeValue, retrieved.ToString());
    }

    [Fact]
    public async Task Compression_GetDel_DecompressesValue()
    {
        string key = $"getdel_test_{Guid.NewGuid()}";

        await ZstdClient.SetAsync(key, LargeValue);
        ValkeyValue retrieved = await ZstdClient.GetDeleteAsync(key);

        Assert.Equal(LargeValue, retrieved.ToString());

        // Verify key was deleted
        ValkeyValue afterDelete = await ZstdClient.GetAsync(key);
        Assert.True(afterDelete.IsNull);
    }

    [Fact]
    public async Task Compression_MSet_CompressesValues()
    {
        var statsBefore = BaseClient.GetStatistics();

        var keysAndValues = new KeyValuePair<ValkeyKey, ValkeyValue>[3];
        for (int i = 0; i < 3; i++)
        {
            keysAndValues[i] = new($"mset_test_{i}_{Guid.NewGuid()}", new string((char)('a' + i), LargeValueSize));
        }

        await ZstdClient.SetAsync(keysAndValues);

        var statsAfter = BaseClient.GetStatistics();
        Assert.True(
            statsAfter.TotalValuesCompressed > statsBefore.TotalValuesCompressed,
            $"MSET should compress values. Before: {statsBefore.TotalValuesCompressed}, After: {statsAfter.TotalValuesCompressed}");

        // Verify values can be retrieved and decompressed
        foreach (var kv in keysAndValues)
        {
            ValkeyValue retrieved = await ZstdClient.GetAsync(kv.Key);
            Assert.Equal(kv.Value.ToString(), retrieved.ToString());
        }
    }

    [Fact]
    public async Task Compression_MSetNX_CompressesValues()
    {
        var statsBefore = BaseClient.GetStatistics();

        var keysAndValues = new KeyValuePair<ValkeyKey, ValkeyValue>[3];
        for (int i = 0; i < 3; i++)
        {
            keysAndValues[i] = new($"msetnx_test_{i}_{Guid.NewGuid()}", new string((char)('a' + i), LargeValueSize));
        }

        bool result = await ZstdClient.SetIfNotExistsAsync(keysAndValues);
        Assert.True(result, "MSETNX should succeed for new keys");

        var statsAfter = BaseClient.GetStatistics();
        Assert.True(
            statsAfter.TotalValuesCompressed > statsBefore.TotalValuesCompressed,
            $"MSETNX should compress values. Before: {statsBefore.TotalValuesCompressed}, After: {statsAfter.TotalValuesCompressed}");

        // Verify values can be retrieved and decompressed
        foreach (var kv in keysAndValues)
        {
            ValkeyValue retrieved = await ZstdClient.GetAsync(kv.Key);
            Assert.Equal(kv.Value.ToString(), retrieved.ToString());
        }
    }

    [Fact]
    public async Task Compression_SetWithExpiry_CompressesValue()
    {
        var statsBefore = BaseClient.GetStatistics();

        string key = $"set_expiry_test_{Guid.NewGuid()}";

        await ZstdClient.SetAsync(key, LargeValue);
        _ = await ZstdClient.ExpireAsync(key, TimeSpan.FromSeconds(10));

        var statsAfter = BaseClient.GetStatistics();
        Assert.True(
            statsAfter.TotalValuesCompressed > statsBefore.TotalValuesCompressed,
            $"SET should compress values. Before: {statsBefore.TotalValuesCompressed}, After: {statsAfter.TotalValuesCompressed}");

        ValkeyValue retrieved = await ZstdClient.GetAsync(key);
        Assert.Equal(LargeValue, retrieved.ToString());
    }

    [Fact]
    public async Task Compression_Setnx_CompressesValue()
    {
        var statsBefore = BaseClient.GetStatistics();

        string key = $"setnx_test_{Guid.NewGuid()}";

        bool result = await ZstdClient.SetAsync(key, LargeValue, SetCondition.OnlyIfDoesNotExist);
        Assert.True(result);

        var statsAfter = BaseClient.GetStatistics();
        Assert.True(
            statsAfter.TotalValuesCompressed > statsBefore.TotalValuesCompressed,
            $"SET NX should compress values. Before: {statsBefore.TotalValuesCompressed}, After: {statsAfter.TotalValuesCompressed}");

        ValkeyValue retrieved = await ZstdClient.GetAsync(key);
        Assert.Equal(LargeValue, retrieved.ToString());
    }

    // ============================================================================
    // Incompatible Command Tests - These should error when compression is enabled
    // ============================================================================

    /// <summary>
    /// Test data for client methods that use commands incompatible with compression.
    /// Each entry contains the expected command name and a call to the client method.
    /// </summary>
    public static TheoryData<string, Func<BaseClient, Task>> IncompatibleMethods => new()
    {
        // String manipulation commands
        { "APPEND", c => c.AppendAsync("key", "suffix") },
        { "DELIFEQ", c => c.DeleteIfEqualAsync("key", "value") },
        { "GETRANGE", c => c.GetRangeAsync("key", 0, 4) },
        { "SET", c => c.GetSetAsync("key", "value", SetCondition.OnlyIfEqual("other")) },
        { "SET", c => c.GetSetAsync("key", "value", SetCondition.OnlyIfNotEqual("other")) },
        // TODO #318: Add LongestCommonSubsequenceAsync once it is available.
        { "STRLEN", c => c.LengthAsync("key") },
        { "SET", c => c.SetAsync("key", "value", SetCondition.OnlyIfEqual("other")) },
        { "SET", c => c.SetAsync("key", "value", SetCondition.OnlyIfNotEqual("other")) },
        { "SET", c => c.SetAsync("key", "value", new SetOptions { Condition = SetCondition.OnlyIfEqual("other"), Expiry = SetExpiryOptions.ExpireIn(TimeSpan.FromSeconds(10)) }) },
        { "SETRANGE", c => c.SetRangeAsync("key", 0, "test") },

        // Numeric operations
        { "DECR", c => c.DecrementAsync("key") },
        { "DECRBY", c => c.DecrementAsync("key", 10) },
        { "INCR", c => c.IncrementAsync("key") },
        { "INCRBY", c => c.IncrementAsync("key", 10) },
        { "INCRBYFLOAT", c => c.IncrementAsync("key", 1.5) },

        // Bit operations
        { "BITCOUNT", c => c.BitCountAsync("key") },
        { "BITFIELD", c => c.BitFieldAsync("key", new BitFieldOptions.BitFieldSet("u8", new BitFieldOptions.BitOffset(0), 1)) },
        { "BITFIELD_RO", c => c.BitFieldReadOnlyAsync("key", new BitFieldOptions.BitFieldGet("u8", new BitFieldOptions.BitOffset(0))) },
        { "BITOP", c => c.BitOpAsync(Bitwise.And, "key", ["key"]) },
        { "BITPOS", c => c.BitPosAsync("key", true) },
        { "GETBIT", c => c.GetBitAsync("key", 0) },
        { "SETBIT", c => c.SetBitAsync("key", 7, true) },
    };

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(IncompatibleMethods))]
    public async Task Compression_IncompatibleMethod_ThrowsError(string command, Func<BaseClient, Task> call)
    {
        var exception = await Assert.ThrowsAsync<Errors.RequestException>(() => call(ZstdClient));
        Assert.Contains($"Command '{command}' is incompatible with compression", exception.Message);
    }

    // ============================================================================
    // Batch/Pipeline Compression Tests
    // ============================================================================

    [Fact]
    public async Task Compression_Batch_CompressesAndDecompresses()
    {
        var statsBefore = BaseClient.GetStatistics();

        string key1 = $"batch_test_1_{Guid.NewGuid()}";
        string key2 = $"batch_test_2_{Guid.NewGuid()}";
        string key3 = $"batch_test_3_{Guid.NewGuid()}";

        var batch = new Batch(false)
            .SetAsync(key1, LargeValue)
            .SetAsync(key2, LargeValue)
            .SetAsync(key3, LargeValue)
            .GetAsync(key1)
            .GetAsync(key2)
            .GetAsync(key3);

        var results = await ZstdClient.Exec(batch, true);

        var statsAfter = BaseClient.GetStatistics();

        // Verify compression occurred (3 SET commands should compress)
        Assert.True(
            statsAfter.TotalValuesCompressed > statsBefore.TotalValuesCompressed,
            $"Batch should compress values. Before: {statsBefore.TotalValuesCompressed}, After: {statsAfter.TotalValuesCompressed}");

        // Verify results - first 3 are SET results (OK), last 3 are GET results
        Assert.NotNull(results);
        Assert.Equal(6, results.Length);
        Assert.Equal(LargeValue, results[3]?.ToString());
        Assert.Equal(LargeValue, results[4]?.ToString());
        Assert.Equal(LargeValue, results[5]?.ToString());
    }

    [Fact]
    public async Task Compression_Transaction_CompressesAndDecompresses()
    {
        var statsBefore = BaseClient.GetStatistics();

        string key1 = $"transaction_test_1_{Guid.NewGuid()}";
        string key2 = $"transaction_test_2_{Guid.NewGuid()}";

        // isAtomic = true makes it a transaction
        var transaction = new Batch(true)
            .SetAsync(key1, LargeValue)
            .SetAsync(key2, LargeValue)
            .GetAsync(key1)
            .GetAsync(key2);

        var results = await ZstdClient.Exec(transaction, true);

        var statsAfter = BaseClient.GetStatistics();

        // Verify compression occurred
        Assert.True(
            statsAfter.TotalValuesCompressed > statsBefore.TotalValuesCompressed,
            $"Transaction should compress values. Before: {statsBefore.TotalValuesCompressed}, After: {statsAfter.TotalValuesCompressed}");

        // Verify results
        Assert.NotNull(results);
        Assert.Equal(4, results.Length);
        Assert.Equal(LargeValue, results[2]?.ToString());
        Assert.Equal(LargeValue, results[3]?.ToString());
    }

    [Fact]
    public async Task Compression_ClusterBatch_CompressesAndDecompresses()
    {
        var statsBefore = BaseClient.GetStatistics();

        // Use same hash slot by using {tag} pattern
        string key1 = $"{{cluster_batch}}_test_1_{Guid.NewGuid()}";
        string key2 = $"{{cluster_batch}}_test_2_{Guid.NewGuid()}";

        var batch = new ClusterBatch(false)
            .SetAsync(key1, LargeValue)
            .SetAsync(key2, LargeValue)
            .GetAsync(key1)
            .GetAsync(key2);

        var results = await ZstdClusterClient.Exec(batch, true);

        var statsAfter = BaseClient.GetStatistics();

        // Verify compression occurred
        Assert.True(
            statsAfter.TotalValuesCompressed > statsBefore.TotalValuesCompressed,
            $"Cluster batch should compress values. Before: {statsBefore.TotalValuesCompressed}, After: {statsAfter.TotalValuesCompressed}");

        // Verify results
        Assert.NotNull(results);
        Assert.Equal(4, results.Length);
        Assert.Equal(LargeValue, results[2]?.ToString());
        Assert.Equal(LargeValue, results[3]?.ToString());
    }

    [Fact]
    public async Task Compression_BatchWithMGet_DecompressesMultipleValues()
    {
        var statsBefore = BaseClient.GetStatistics();

        string key1 = $"batch_mget_1_{Guid.NewGuid()}";
        string key2 = $"batch_mget_2_{Guid.NewGuid()}";
        string key3 = $"batch_mget_3_{Guid.NewGuid()}";
        string value1 = new('a', LargeValueSize);
        string value2 = new('b', LargeValueSize);
        string value3 = new('c', LargeValueSize);

        // Batch with SET commands followed by MGET (via GetAsync with multiple keys)
        var batch = new Batch(false)
            .SetAsync(key1, value1)
            .SetAsync(key2, value2)
            .SetAsync(key3, value3)
            .GetAsync([key1, key2, key3]);

        var results = await ZstdClient.Exec(batch, true);

        var statsAfter = BaseClient.GetStatistics();

        // Verify compression occurred (3 SET commands should compress)
        Assert.True(
            statsAfter.TotalValuesCompressed > statsBefore.TotalValuesCompressed,
            $"Batch should compress values. Before: {statsBefore.TotalValuesCompressed}, After: {statsAfter.TotalValuesCompressed}");

        // Verify results - first 3 are SET results (OK), last is MGET result (array)
        Assert.NotNull(results);
        Assert.Equal(4, results.Length);

        // MGET returns an array of values
        var mgetResult = results[3] as ValkeyValue[];
        Assert.NotNull(mgetResult);
        Assert.Equal(3, mgetResult.Length);
        Assert.Equal(value1, mgetResult[0].ToString());
        Assert.Equal(value2, mgetResult[1].ToString());
        Assert.Equal(value3, mgetResult[2].ToString());
    }
}
