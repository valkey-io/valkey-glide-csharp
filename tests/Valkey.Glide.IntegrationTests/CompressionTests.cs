// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands.Options;

namespace Valkey.Glide.IntegrationTests;

/// <summary>
/// Fixture that provides shared compression-enabled clients for tests.
/// </summary>
public class CompressionFixture : IAsyncLifetime
{
    public GlideClient? ZstdClient { get; private set; }
    public GlideClient? Lz4Client { get; private set; }
    public GlideClusterClient? ZstdClusterClient { get; private set; }

    public async ValueTask InitializeAsync()
    {
        var zstdConfig = TestConfiguration.DefaultClientConfig()
            .WithCompression(CompressionConfig.Zstd())
            .Build();

        var lz4Config = TestConfiguration.DefaultClientConfig()
            .WithCompression(CompressionConfig.Lz4())
            .Build();

        var clusterConfig = TestConfiguration.DefaultClusterClientConfig()
            .WithCompression(CompressionConfig.Zstd())
            .Build();

        ZstdClient = await GlideClient.CreateClient(zstdConfig);
        Lz4Client = await GlideClient.CreateClient(lz4Config);
        ZstdClusterClient = await GlideClusterClient.CreateClient(clusterConfig);
    }

    public ValueTask DisposeAsync()
    {
        ZstdClient?.Dispose();
        Lz4Client?.Dispose();
        ZstdClusterClient?.Dispose();
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
        Assert.True(statsAfter.TotalValuesCompressed > statsBefore.TotalValuesCompressed,
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
        Assert.True(statsAfter.TotalValuesCompressed > statsBefore.TotalValuesCompressed,
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
        Assert.True(statsAfter.TotalValuesCompressed > statsBefore.TotalValuesCompressed,
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
        Assert.True(statsAfter.TotalValuesCompressed > statsBefore.TotalValuesCompressed,
            $"SET NX should compress values. Before: {statsBefore.TotalValuesCompressed}, After: {statsAfter.TotalValuesCompressed}");

        ValkeyValue retrieved = await ZstdClient.GetAsync(key);
        Assert.Equal(LargeValue, retrieved.ToString());
    }

    // ============================================================================
    // CustomCommand Tests for Compression
    // ============================================================================

    [Fact]
    public async Task Compression_SetExViaCustomCommand_CompressesValue()
    {
        var statsBefore = BaseClient.GetStatistics();

        string key = $"setex_custom_test_{Guid.NewGuid()}";

        // SETEX via custom command should compress value
        var result = await ZstdClient.CustomCommand(["SETEX", key, "10", LargeValue]);
        Assert.Equal("OK", result?.ToString());

        var statsAfter = BaseClient.GetStatistics();
        Assert.True(statsAfter.TotalValuesCompressed > statsBefore.TotalValuesCompressed,
            $"SETEX via CustomCommand should compress value. Before: {statsBefore.TotalValuesCompressed}, After: {statsAfter.TotalValuesCompressed}");

        // Verify value can be retrieved and decompressed
        ValkeyValue retrieved = await ZstdClient.GetAsync(key);
        Assert.Equal(LargeValue, retrieved.ToString());

        // Verify TTL was set
        var ttl = await ZstdClient.TimeToLiveAsync(key);
        Assert.True(ttl.HasTimeToLive);
        Assert.True(ttl.TimeToLive!.Value.TotalSeconds > 0 && ttl.TimeToLive!.Value.TotalSeconds <= 10);
    }

    [Fact]
    public async Task Compression_PSetExViaCustomCommand_CompressesValue()
    {
        var statsBefore = BaseClient.GetStatistics();

        string key = $"psetex_custom_test_{Guid.NewGuid()}";

        // PSETEX via custom command should compress value (10000ms = 10s)
        var result = await ZstdClient.CustomCommand(["PSETEX", key, "10000", LargeValue]);
        Assert.Equal("OK", result?.ToString());

        var statsAfter = BaseClient.GetStatistics();
        Assert.True(statsAfter.TotalValuesCompressed > statsBefore.TotalValuesCompressed,
            $"PSETEX via CustomCommand should compress value. Before: {statsBefore.TotalValuesCompressed}, After: {statsAfter.TotalValuesCompressed}");

        // Verify value can be retrieved and decompressed
        ValkeyValue retrieved = await ZstdClient.GetAsync(key);
        Assert.Equal(LargeValue, retrieved.ToString());

        // Verify TTL was set
        var ttl = await ZstdClient.TimeToLiveAsync(key);
        Assert.True(ttl.HasTimeToLive);
        Assert.True(ttl.TimeToLive!.Value.TotalSeconds > 0 && ttl.TimeToLive!.Value.TotalSeconds <= 10);
    }

    [Fact]
    public async Task Compression_SetNXViaCustomCommand_CompressesValue()
    {
        // Ensure key doesn't exist
        string key = $"setnx_custom_test_{Guid.NewGuid()}";
#pragma warning disable IDE0058 // Expression value is never used
        await ZstdClient.DeleteAsync(key);
#pragma warning restore IDE0058

        var statsBefore = BaseClient.GetStatistics();

        // SETNX via custom command should compress value
        var result = await ZstdClient.CustomCommand(["SETNX", key, LargeValue]);
        Assert.Equal(1L, result);

        var statsAfter = BaseClient.GetStatistics();
        Assert.True(statsAfter.TotalValuesCompressed > statsBefore.TotalValuesCompressed,
            $"SETNX via CustomCommand should compress value. Before: {statsBefore.TotalValuesCompressed}, After: {statsAfter.TotalValuesCompressed}");

        // Verify value can be retrieved and decompressed
        ValkeyValue retrieved = await ZstdClient.GetAsync(key);
        Assert.Equal(LargeValue, retrieved.ToString());
    }

    // ============================================================================
    // Incompatible Command Tests - These should error when compression is enabled
    // ============================================================================

    [Fact]
    public async Task Compression_Append_ThrowsIncompatibleError()
    {
        string key = $"append_test_{Guid.NewGuid()}";
        await ZstdClient.SetAsync(key, "initial");

        var exception = await Assert.ThrowsAsync<Errors.RequestException>(async () =>
            await ZstdClient.AppendAsync(key, "_suffix"));

        Assert.Contains("compression", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Compression_GetRange_ThrowsIncompatibleError()
    {
        string key = $"getrange_test_{Guid.NewGuid()}";
        await ZstdClient.SetAsync(key, "Hello World");

        var exception = await Assert.ThrowsAsync<Errors.RequestException>(async () =>
            await ZstdClient.GetRangeAsync(key, 0, 4));

        Assert.Contains("compression", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Compression_SetRange_ThrowsIncompatibleError()
    {
        string key = $"setrange_test_{Guid.NewGuid()}";
        await ZstdClient.SetAsync(key, "Hello World");

        var exception = await Assert.ThrowsAsync<Errors.RequestException>(async () =>
            await ZstdClient.SetRangeAsync(key, 6, "Valkey"));

        Assert.Contains("compression", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Compression_StrLen_ThrowsIncompatibleError()
    {
        string key = $"strlen_test_{Guid.NewGuid()}";
        await ZstdClient.SetAsync(key, "Hello");

        var exception = await Assert.ThrowsAsync<Errors.RequestException>(async () =>
            await ZstdClient.LengthAsync(key));

        Assert.Contains("compression", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Compression_Incr_ThrowsIncompatibleError()
    {
        string key = $"incr_test_{Guid.NewGuid()}";
        await ZstdClient.SetAsync(key, "100");

        var exception = await Assert.ThrowsAsync<Errors.RequestException>(async () =>
            await ZstdClient.IncrementAsync(key));

        Assert.Contains("compression", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Compression_IncrBy_ThrowsIncompatibleError()
    {
        string key = $"incrby_test_{Guid.NewGuid()}";
        await ZstdClient.SetAsync(key, "100");

        var exception = await Assert.ThrowsAsync<Errors.RequestException>(async () =>
            await ZstdClient.IncrementAsync(key, 10));

        Assert.Contains("compression", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Compression_IncrByFloat_ThrowsIncompatibleError()
    {
        string key = $"incrbyfloat_test_{Guid.NewGuid()}";
        await ZstdClient.SetAsync(key, "100.5");

        var exception = await Assert.ThrowsAsync<Errors.RequestException>(async () =>
            await ZstdClient.IncrementAsync(key, 1.5));

        Assert.Contains("compression", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Compression_Decr_ThrowsIncompatibleError()
    {
        string key = $"decr_test_{Guid.NewGuid()}";
        await ZstdClient.SetAsync(key, "100");

        var exception = await Assert.ThrowsAsync<Errors.RequestException>(async () =>
            await ZstdClient.DecrementAsync(key));

        Assert.Contains("compression", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Compression_DecrBy_ThrowsIncompatibleError()
    {
        string key = $"decrby_test_{Guid.NewGuid()}";
        await ZstdClient.SetAsync(key, "100");

        var exception = await Assert.ThrowsAsync<Errors.RequestException>(async () =>
            await ZstdClient.DecrementAsync(key, 10));

        Assert.Contains("compression", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Compression_GetBit_ThrowsIncompatibleError()
    {
        string key = $"getbit_test_{Guid.NewGuid()}";
        await ZstdClient.SetAsync(key, "Hello");

        var exception = await Assert.ThrowsAsync<Errors.RequestException>(async () =>
            await ZstdClient.GetBitAsync(key, 0));

        Assert.Contains("compression", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Compression_SetBit_ThrowsIncompatibleError()
    {
        string key = $"setbit_test_{Guid.NewGuid()}";

        var exception = await Assert.ThrowsAsync<Errors.RequestException>(async () =>
            await ZstdClient.SetBitAsync(key, 7, true));

        Assert.Contains("compression", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Compression_BitCount_ThrowsIncompatibleError()
    {
        string key = $"bitcount_test_{Guid.NewGuid()}";
        await ZstdClient.SetAsync(key, "Hello");

        var exception = await Assert.ThrowsAsync<Errors.RequestException>(async () =>
            await ZstdClient.BitCountAsync(key));

        Assert.Contains("compression", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    // ============================================================================
    // CustomCommand Incompatible Tests
    // ============================================================================

    [Fact]
    public async Task Compression_IncrViaCustomCommand_ThrowsIncompatibleError()
    {
        string key = $"incr_custom_test_{Guid.NewGuid()}";
        await ZstdClient.SetAsync(key, "100");

        var exception = await Assert.ThrowsAsync<Errors.RequestException>(async () =>
            await ZstdClient.CustomCommand(["INCR", key]));

        Assert.Contains("compression", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Compression_AppendViaCustomCommand_ThrowsIncompatibleError()
    {
        string key = $"append_custom_test_{Guid.NewGuid()}";
        await ZstdClient.SetAsync(key, "Hello");

        var exception = await Assert.ThrowsAsync<Errors.RequestException>(async () =>
            await ZstdClient.CustomCommand(["APPEND", key, " World"]));

        Assert.Contains("compression", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Compression_StrLenViaCustomCommand_ThrowsIncompatibleError()
    {
        string key = $"strlen_custom_test_{Guid.NewGuid()}";
        await ZstdClient.SetAsync(key, "Hello");

        var exception = await Assert.ThrowsAsync<Errors.RequestException>(async () =>
            await ZstdClient.CustomCommand(["STRLEN", key]));

        Assert.Contains("compression", exception.Message, StringComparison.OrdinalIgnoreCase);
    }
}
