// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands.Options;

using static Valkey.Glide.ConnectionConfiguration;

namespace Valkey.Glide.IntegrationTests;

[Collection("GlideTests")]
public class CompressionTests(TestConfiguration config)
{
    private const int LargeValueSize = 1000;
    private const int SmallValueSize = 100;
    private const int MinSizeThreshold = 500;
    private const int CustomCompressionLevel = 10;
    private const int MultiOpCount = 10;
    private const int MultiOpBaseSize = 500;
    private const int MultiOpSizeIncrement = 100;
    private static readonly string LargeValue = new('y', LargeValueSize);

    public TestConfiguration Config { get; } = config;

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestStandaloneClients), MemberType = typeof(TestConfiguration))]
    public async Task Compression_Zstd_Standalone_CompressesAndDecompresses(GlideClient _)
    {
        var clientConfig = new StandaloneClientConfigurationBuilder()
            .WithAddress(TestConfiguration.STANDALONE_ADDRESS.Host, TestConfiguration.STANDALONE_ADDRESS.Port)
            .WithCompression(CompressionConfig.Zstd())
            .WithTls(TestConfiguration.TLS)
            .Build();

        await using var client = await GlideClient.CreateClient(clientConfig);

        var statsBefore = BaseClient.GetStatistics();

        string key = $"compression_test_{Guid.NewGuid()}";

        await client.SetAsync(key, LargeValue);
        var retrieved = await client.GetAsync(key);

        Assert.Equal(LargeValue, retrieved.ToString());

        var statsAfter = BaseClient.GetStatistics();
        Assert.True(statsAfter.TotalValuesCompressed > statsBefore.TotalValuesCompressed, $"Expected compression to occur. Before: {statsBefore.TotalValuesCompressed}, After: {statsAfter.TotalValuesCompressed}");
        Assert.True(statsAfter.TotalBytesCompressed > statsBefore.TotalBytesCompressed, $"Expected bytes compressed. Before: {statsBefore.TotalBytesCompressed}, After: {statsAfter.TotalBytesCompressed}");
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestStandaloneClients), MemberType = typeof(TestConfiguration))]
    public async Task Compression_Lz4_Standalone_CompressesAndDecompresses(GlideClient _)
    {
        var clientConfig = new StandaloneClientConfigurationBuilder()
            .WithAddress(TestConfiguration.STANDALONE_ADDRESS.Host, TestConfiguration.STANDALONE_ADDRESS.Port)
            .WithCompression(CompressionConfig.Lz4())
            .WithTls(TestConfiguration.TLS)
            .Build();

        await using var client = await GlideClient.CreateClient(clientConfig);

        var statsBefore = BaseClient.GetStatistics();

        string key = $"compression_lz4_test_{Guid.NewGuid()}";

        await client.SetAsync(key, LargeValue);
        var retrieved = await client.GetAsync(key);

        Assert.Equal(LargeValue, retrieved.ToString());

        var statsAfter = BaseClient.GetStatistics();
        Assert.True(statsAfter.TotalValuesCompressed > statsBefore.TotalValuesCompressed);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClusterClients), MemberType = typeof(TestConfiguration))]
    public async Task Compression_Zstd_Cluster_CompressesAndDecompresses(GlideClusterClient _)
    {
        var clientConfig = new ClusterClientConfigurationBuilder()
            .WithAddress(TestConfiguration.CLUSTER_ADDRESS.Host, TestConfiguration.CLUSTER_ADDRESS.Port)
            .WithCompression(CompressionConfig.Zstd())
            .WithTls(TestConfiguration.TLS)
            .Build();

        await using var client = await GlideClusterClient.CreateClient(clientConfig);

        var statsBefore = BaseClient.GetStatistics();

        string key = $"compression_cluster_test_{Guid.NewGuid()}";

        await client.SetAsync(key, LargeValue);
        var retrieved = await client.GetAsync(key);

        Assert.Equal(LargeValue, retrieved.ToString());

        var statsAfter = BaseClient.GetStatistics();
        Assert.True(statsAfter.TotalValuesCompressed > statsBefore.TotalValuesCompressed, $"Expected compression. Before: {statsBefore.TotalValuesCompressed}, After: {statsAfter.TotalValuesCompressed}");
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestStandaloneClients), MemberType = typeof(TestConfiguration))]
    public async Task Compression_MinSize_SkipsSmallValues(GlideClient _)
    {
        var clientConfig = new StandaloneClientConfigurationBuilder()
            .WithAddress(TestConfiguration.STANDALONE_ADDRESS.Host, TestConfiguration.STANDALONE_ADDRESS.Port)
            .WithCompression(CompressionConfig.Zstd(minCompressionSize: MinSizeThreshold))
            .WithTls(TestConfiguration.TLS)
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

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestStandaloneClients), MemberType = typeof(TestConfiguration))]
    public async Task Compression_CustomLevel_WorksCorrectly(GlideClient _)
    {
        var clientConfig = new StandaloneClientConfigurationBuilder()
            .WithAddress(TestConfiguration.STANDALONE_ADDRESS.Host, TestConfiguration.STANDALONE_ADDRESS.Port)
            .WithCompression(CompressionConfig.Zstd(compressionLevel: CustomCompressionLevel))
            .WithTls(TestConfiguration.TLS)
            .Build();

        await using var client = await GlideClient.CreateClient(clientConfig);

        string key = $"compression_level_test_{Guid.NewGuid()}";
        string value = new('z', LargeValueSize);

        await client.SetAsync(key, value);
        var retrieved = await client.GetAsync(key);

        Assert.Equal(value, retrieved.ToString());
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestStandaloneClients), MemberType = typeof(TestConfiguration))]
    public async Task Compression_BackwardCompatibility_ReadsUncompressedData(GlideClient _)
    {
        var configNoCompression = new StandaloneClientConfigurationBuilder()
            .WithAddress(TestConfiguration.STANDALONE_ADDRESS.Host, TestConfiguration.STANDALONE_ADDRESS.Port)
            .WithTls(TestConfiguration.TLS)
            .Build();

        await using var clientNoCompression = await GlideClient.CreateClient(configNoCompression);

        string key = $"backward_compat_test_{Guid.NewGuid()}";
        string value = "uncompressed_data";

        await clientNoCompression.SetAsync(key, value);

        var configWithCompression = new StandaloneClientConfigurationBuilder()
            .WithAddress(TestConfiguration.STANDALONE_ADDRESS.Host, TestConfiguration.STANDALONE_ADDRESS.Port)
            .WithCompression(CompressionConfig.Zstd())
            .WithTls(TestConfiguration.TLS)
            .Build();

        await using var client = await GlideClient.CreateClient(configWithCompression);

        var retrieved = await client.GetAsync(key);
        Assert.Equal(value, retrieved.ToString());
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestStandaloneClients), MemberType = typeof(TestConfiguration))]
    public async Task Compression_MultipleOperations_MaintainsDataIntegrity(GlideClient _)
    {
        var clientConfig = new StandaloneClientConfigurationBuilder()
            .WithAddress(TestConfiguration.STANDALONE_ADDRESS.Host, TestConfiguration.STANDALONE_ADDRESS.Port)
            .WithCompression(CompressionConfig.Lz4())
            .WithTls(TestConfiguration.TLS)
            .Build();

        await using var client = await GlideClient.CreateClient(clientConfig);

        var testData = new Dictionary<string, string>();
        for (int i = 0; i < MultiOpCount; i++)
        {
            string key = $"multi_op_test_{i}_{Guid.NewGuid()}";
            string value = new((char)('a' + i), MultiOpBaseSize + (i * MultiOpSizeIncrement));
            testData[key] = value;
            await client.SetAsync(key, value);
        }

        foreach (var kvp in testData)
        {
            var retrieved = await client.GetAsync(kvp.Key);
            Assert.Equal(kvp.Value, retrieved.ToString());
        }
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestStandaloneClients), MemberType = typeof(TestConfiguration))]
    public async Task Compression_BinaryData_HandlesCorrectly(GlideClient _)
    {
        var clientConfig = new StandaloneClientConfigurationBuilder()
            .WithAddress(TestConfiguration.STANDALONE_ADDRESS.Host, TestConfiguration.STANDALONE_ADDRESS.Port)
            .WithCompression(CompressionConfig.Zstd())
            .WithTls(TestConfiguration.TLS)
            .Build();

        await using var client = await GlideClient.CreateClient(clientConfig);

        string key = $"binary_test_{Guid.NewGuid()}";
        byte[] binaryData = new byte[LargeValueSize];
        new Random().NextBytes(binaryData);

        await client.SetAsync(key, binaryData);
        var retrieved = await client.GetAsync(key);

        Assert.Equal(binaryData, (byte[]?)retrieved);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestStandaloneClients), MemberType = typeof(TestConfiguration))]
    public async Task Compression_Statistics_ReflectOperations(GlideClient _)
    {
        var clientConfig = new StandaloneClientConfigurationBuilder()
            .WithAddress(TestConfiguration.STANDALONE_ADDRESS.Host, TestConfiguration.STANDALONE_ADDRESS.Port)
            .WithCompression(CompressionConfig.Zstd())
            .WithTls(TestConfiguration.TLS)
            .Build();

        await using var client = await GlideClient.CreateClient(clientConfig);

        var statsBefore = BaseClient.GetStatistics();

        string key = $"stats_test_{Guid.NewGuid()}";
        string value = new('m', LargeValueSize);

        await client.SetAsync(key, value);
        ValkeyValue retrieved = await client.GetAsync(key);

        var statsAfter = BaseClient.GetStatistics();

        Assert.True(statsAfter.TotalValuesCompressed > statsBefore.TotalValuesCompressed, $"Expected compression. Before: {statsBefore.TotalValuesCompressed}, After: {statsAfter.TotalValuesCompressed}");
        Assert.True(statsAfter.TotalOriginalBytes > statsBefore.TotalOriginalBytes, $"Expected original bytes. Before: {statsBefore.TotalOriginalBytes}, After: {statsAfter.TotalOriginalBytes}");
        Assert.True(statsAfter.TotalBytesCompressed > statsBefore.TotalBytesCompressed, $"Expected compressed bytes. Before: {statsBefore.TotalBytesCompressed}, After: {statsAfter.TotalBytesCompressed}");
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestStandaloneClients), MemberType = typeof(TestConfiguration))]
    public async Task Compression_MGet_DecompressesValues(GlideClient _)
    {
        var clientConfig = new StandaloneClientConfigurationBuilder()
            .WithAddress(TestConfiguration.STANDALONE_ADDRESS.Host, TestConfiguration.STANDALONE_ADDRESS.Port)
            .WithCompression(CompressionConfig.Zstd())
            .WithTls(TestConfiguration.TLS)
            .Build();

        await using var client = await GlideClient.CreateClient(clientConfig);

        var keysAndValues = new KeyValuePair<ValkeyKey, ValkeyValue>[5];
        for (int i = 0; i < 5; i++)
        {
            string key = $"mget_test_{i}_{Guid.NewGuid()}";
            string value = new((char)('a' + i), LargeValueSize);
            keysAndValues[i] = new(key, value);
            await client.SetAsync(key, value);
        }

        ValkeyKey[] keys = [.. keysAndValues.Select(kv => kv.Key)];
        ValkeyValue[] results = await client.GetAsync(keys);

        for (int i = 0; i < 5; i++)
        {
            Assert.Equal(keysAndValues[i].Value.ToString(), results[i].ToString());
        }
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestStandaloneClients), MemberType = typeof(TestConfiguration))]
    public async Task Compression_GetEx_DecompressesValue(GlideClient _)
    {
        var clientConfig = new StandaloneClientConfigurationBuilder()
            .WithAddress(TestConfiguration.STANDALONE_ADDRESS.Host, TestConfiguration.STANDALONE_ADDRESS.Port)
            .WithCompression(CompressionConfig.Zstd())
            .WithTls(TestConfiguration.TLS)
            .Build();

        await using var client = await GlideClient.CreateClient(clientConfig);

        string key = $"getex_test_{Guid.NewGuid()}";
        string value = new('g', LargeValueSize);

        await client.SetAsync(key, value);
        ValkeyValue retrieved = await client.GetExpiryAsync(key, GetExpiryOptions.ExpireIn(TimeSpan.FromSeconds(10)));

        Assert.Equal(value, retrieved.ToString());
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestStandaloneClients), MemberType = typeof(TestConfiguration))]
    public async Task Compression_GetDel_DecompressesValue(GlideClient _)
    {
        var clientConfig = new StandaloneClientConfigurationBuilder()
            .WithAddress(TestConfiguration.STANDALONE_ADDRESS.Host, TestConfiguration.STANDALONE_ADDRESS.Port)
            .WithCompression(CompressionConfig.Zstd())
            .WithTls(TestConfiguration.TLS)
            .Build();

        await using var client = await GlideClient.CreateClient(clientConfig);

        string key = $"getdel_test_{Guid.NewGuid()}";
        string value = new('d', LargeValueSize);

        await client.SetAsync(key, value);
        ValkeyValue retrieved = await client.GetDeleteAsync(key);

        Assert.Equal(value, retrieved.ToString());

        // Verify key was deleted
        ValkeyValue afterDelete = await client.GetAsync(key);
        Assert.True(afterDelete.IsNull);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestStandaloneClients), MemberType = typeof(TestConfiguration))]
    public async Task Compression_MSet_CompressesValues(GlideClient _)
    {
        var clientConfig = new StandaloneClientConfigurationBuilder()
            .WithAddress(TestConfiguration.STANDALONE_ADDRESS.Host, TestConfiguration.STANDALONE_ADDRESS.Port)
            .WithCompression(CompressionConfig.Zstd())
            .WithTls(TestConfiguration.TLS)
            .Build();

        await using var client = await GlideClient.CreateClient(clientConfig);

        var statsBefore = BaseClient.GetStatistics();

        var keysAndValues = new KeyValuePair<ValkeyKey, ValkeyValue>[3];
        for (int i = 0; i < 3; i++)
        {
            keysAndValues[i] = new($"mset_test_{i}_{Guid.NewGuid()}", new string((char)('a' + i), LargeValueSize));
        }

        await client.SetAsync(keysAndValues);

        var statsAfter = BaseClient.GetStatistics();
        Assert.True(statsAfter.TotalValuesCompressed > statsBefore.TotalValuesCompressed,
            $"MSET should compress values. Before: {statsBefore.TotalValuesCompressed}, After: {statsAfter.TotalValuesCompressed}");

        // Verify values can be retrieved and decompressed
        foreach (var kv in keysAndValues)
        {
            ValkeyValue retrieved = await client.GetAsync(kv.Key);
            Assert.Equal(kv.Value.ToString(), retrieved.ToString());
        }
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestStandaloneClients), MemberType = typeof(TestConfiguration))]
    public async Task Compression_MSetNX_CompressesValues(GlideClient _)
    {
        var clientConfig = new StandaloneClientConfigurationBuilder()
            .WithAddress(TestConfiguration.STANDALONE_ADDRESS.Host, TestConfiguration.STANDALONE_ADDRESS.Port)
            .WithCompression(CompressionConfig.Zstd())
            .WithTls(TestConfiguration.TLS)
            .Build();

        await using var client = await GlideClient.CreateClient(clientConfig);

        var statsBefore = BaseClient.GetStatistics();

        var keysAndValues = new KeyValuePair<ValkeyKey, ValkeyValue>[3];
        for (int i = 0; i < 3; i++)
        {
            keysAndValues[i] = new($"msetnx_test_{i}_{Guid.NewGuid()}", new string((char)('a' + i), LargeValueSize));
        }

        bool result = await client.SetIfNotExistsAsync(keysAndValues);
        Assert.True(result, "MSETNX should succeed for new keys");

        var statsAfter = BaseClient.GetStatistics();
        Assert.True(statsAfter.TotalValuesCompressed > statsBefore.TotalValuesCompressed,
            $"MSETNX should compress values. Before: {statsBefore.TotalValuesCompressed}, After: {statsAfter.TotalValuesCompressed}");

        // Verify values can be retrieved and decompressed
        foreach (var kv in keysAndValues)
        {
            ValkeyValue retrieved = await client.GetAsync(kv.Key);
            Assert.Equal(kv.Value.ToString(), retrieved.ToString());
        }
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestStandaloneClients), MemberType = typeof(TestConfiguration))]
    public async Task Compression_SetWithExpiry_CompressesValue(GlideClient _)
    {
        var clientConfig = new StandaloneClientConfigurationBuilder()
            .WithAddress(TestConfiguration.STANDALONE_ADDRESS.Host, TestConfiguration.STANDALONE_ADDRESS.Port)
            .WithCompression(CompressionConfig.Zstd())
            .WithTls(TestConfiguration.TLS)
            .Build();

        await using var client = await GlideClient.CreateClient(clientConfig);

        var statsBefore = BaseClient.GetStatistics();

        string key = $"set_expiry_test_{Guid.NewGuid()}";
        string value = new('s', LargeValueSize);

        await client.SetAsync(key, value);
        bool expired = await client.ExpireAsync(key, TimeSpan.FromSeconds(10));

        var statsAfter = BaseClient.GetStatistics();
        Assert.True(statsAfter.TotalValuesCompressed > statsBefore.TotalValuesCompressed,
            $"SET should compress values. Before: {statsBefore.TotalValuesCompressed}, After: {statsAfter.TotalValuesCompressed}");

        ValkeyValue retrieved = await client.GetAsync(key);
        Assert.Equal(value, retrieved.ToString());
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestStandaloneClients), MemberType = typeof(TestConfiguration))]
    public async Task Compression_Setnx_CompressesValue(GlideClient _)
    {
        var clientConfig = new StandaloneClientConfigurationBuilder()
            .WithAddress(TestConfiguration.STANDALONE_ADDRESS.Host, TestConfiguration.STANDALONE_ADDRESS.Port)
            .WithCompression(CompressionConfig.Zstd())
            .WithTls(TestConfiguration.TLS)
            .Build();

        await using var client = await GlideClient.CreateClient(clientConfig);

        var statsBefore = BaseClient.GetStatistics();

        string key = $"setnx_test_{Guid.NewGuid()}";
        string value = new('n', LargeValueSize);

        bool result = await client.SetAsync(key, value, SetCondition.OnlyIfDoesNotExist);
        Assert.True(result);

        var statsAfter = BaseClient.GetStatistics();
        Assert.True(statsAfter.TotalValuesCompressed > statsBefore.TotalValuesCompressed,
            $"SET NX should compress values. Before: {statsBefore.TotalValuesCompressed}, After: {statsAfter.TotalValuesCompressed}");

        ValkeyValue retrieved = await client.GetAsync(key);
        Assert.Equal(value, retrieved.ToString());
    }
}
