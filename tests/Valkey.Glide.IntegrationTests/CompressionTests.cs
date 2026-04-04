// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

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
        string largeValue = new string('a', LargeValueSize);

        await client.StringSetAsync(key, largeValue);
        var retrieved = await client.StringGetAsync(key);

        Assert.Equal(largeValue, retrieved.ToString());

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
        string largeValue = new string('b', LargeValueSize);

        await client.StringSetAsync(key, largeValue);
        var retrieved = await client.StringGetAsync(key);

        Assert.Equal(largeValue, retrieved.ToString());

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
        string largeValue = new string('c', LargeValueSize);

        await client.StringSetAsync(key, largeValue);
        var retrieved = await client.StringGetAsync(key);

        Assert.Equal(largeValue, retrieved.ToString());

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
        string smallValue = new string('x', SmallValueSize);
        await client.StringSetAsync(smallKey, smallValue);

        var statsAfterSmall = BaseClient.GetStatistics();
        var skippedCountSmall = statsAfterSmall.CompressionSkippedCount - statsBefore.CompressionSkippedCount;

        string largeKey = $"large_{Guid.NewGuid()}";
        string largeValue = new string('y', LargeValueSize);
        await client.StringSetAsync(largeKey, largeValue);

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
        string value = new string('z', LargeValueSize);

        await client.StringSetAsync(key, value);
        var retrieved = await client.StringGetAsync(key);

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

        await clientNoCompression.StringSetAsync(key, value);

        var configWithCompression = new StandaloneClientConfigurationBuilder()
            .WithAddress(TestConfiguration.STANDALONE_ADDRESS.Host, TestConfiguration.STANDALONE_ADDRESS.Port)
            .WithCompression(CompressionConfig.Zstd())
            .WithTls(TestConfiguration.TLS)
            .Build();

        await using var client = await GlideClient.CreateClient(configWithCompression);

        var retrieved = await client.StringGetAsync(key);
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
            string value = new string((char)('a' + i), MultiOpBaseSize + i * MultiOpSizeIncrement);
            testData[key] = value;
            await client.StringSetAsync(key, value);
        }

        foreach (var kvp in testData)
        {
            var retrieved = await client.StringGetAsync(kvp.Key);
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

        await client.StringSetAsync(key, binaryData);
        var retrieved = await client.StringGetAsync(key);

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
        string value = new string('m', LargeValueSize);

        await client.StringSetAsync(key, value);
        await client.StringGetAsync(key);

        var statsAfter = BaseClient.GetStatistics();

        Assert.True(statsAfter.TotalValuesCompressed > statsBefore.TotalValuesCompressed, $"Expected compression. Before: {statsBefore.TotalValuesCompressed}, After: {statsAfter.TotalValuesCompressed}");
        Assert.True(statsAfter.TotalOriginalBytes > statsBefore.TotalOriginalBytes, $"Expected original bytes. Before: {statsBefore.TotalOriginalBytes}, After: {statsAfter.TotalOriginalBytes}");
        Assert.True(statsAfter.TotalBytesCompressed > statsBefore.TotalBytesCompressed, $"Expected compressed bytes. Before: {statsBefore.TotalBytesCompressed}, After: {statsAfter.TotalBytesCompressed}");
    }
}
