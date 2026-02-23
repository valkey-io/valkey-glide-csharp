// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Xunit;

namespace Valkey.Glide.IntegrationTests;

[Collection("Sequential")]
public class CompressionTests : IAsyncLifetime
{
    private GlideClient? _client;
    private GlideClusterClient? _clusterClient;

    public ValueTask InitializeAsync() => ValueTask.CompletedTask;

    public async ValueTask DisposeAsync()
    {
        if (_client != null)
        {
            await _client.DisposeAsync();
        }
        if (_clusterClient != null)
        {
            await _clusterClient.DisposeAsync();
        }
    }

    [Fact]
    public async Task Compression_Zstd_Standalone_CompressesAndDecompresses()
    {
        Skip.IfNot(TestConfiguration.IsStandaloneAvailable, "Standalone server not available");

        var config = new StandaloneClientConfigurationBuilder()
            .WithAddress(TestConfiguration.StandaloneHost, TestConfiguration.StandalonePort)
            .WithCompression(CompressionConfig.Zstd())
            .Build();

        _client = await GlideClient.CreateClient(config);

        // Test with compressible data
        string key = $"compression_test_{Guid.NewGuid()}";
        string largeValue = new string('a', 1000); // Highly compressible

        await _client.StringSetAsync(key, largeValue);
        var retrieved = await _client.StringGetAsync(key);

        Assert.Equal(largeValue, retrieved.ToString());

        // Verify statistics
        var stats = BaseClient.GetCompressionStatistics();
        Assert.True(stats.TotalValuesCompressed > 0);
        Assert.True(stats.TotalBytesCompressed < stats.TotalOriginalBytes);
    }

    [Fact]
    public async Task Compression_Lz4_Standalone_CompressesAndDecompresses()
    {
        Skip.IfNot(TestConfiguration.IsStandaloneAvailable, "Standalone server not available");

        var config = new StandaloneClientConfigurationBuilder()
            .WithAddress(TestConfiguration.StandaloneHost, TestConfiguration.StandalonePort)
            .WithCompression(CompressionConfig.Lz4())
            .Build();

        _client = await GlideClient.CreateClient(config);

        string key = $"compression_lz4_test_{Guid.NewGuid()}";
        string largeValue = new string('b', 1000);

        await _client.StringSetAsync(key, largeValue);
        var retrieved = await _client.StringGetAsync(key);

        Assert.Equal(largeValue, retrieved.ToString());

        var stats = BaseClient.GetCompressionStatistics();
        Assert.True(stats.TotalValuesCompressed > 0);
    }

    [Fact]
    public async Task Compression_Zstd_Cluster_CompressesAndDecompresses()
    {
        Skip.IfNot(TestConfiguration.IsClusterAvailable, "Cluster not available");

        var config = new ClusterClientConfigurationBuilder()
            .WithAddress(TestConfiguration.ClusterHost, TestConfiguration.ClusterPort)
            .WithCompression(CompressionConfig.Zstd())
            .Build();

        _clusterClient = await GlideClusterClient.CreateClient(config);

        string key = $"compression_cluster_test_{Guid.NewGuid()}";
        string largeValue = new string('c', 1000);

        await _clusterClient.StringSetAsync(key, largeValue);
        var retrieved = await _clusterClient.StringGetAsync(key);

        Assert.Equal(largeValue, retrieved.ToString());

        var stats = BaseClient.GetCompressionStatistics();
        Assert.True(stats.TotalValuesCompressed > 0);
    }

    [Fact]
    public async Task Compression_MinSize_SkipsSmallValues()
    {
        Skip.IfNot(TestConfiguration.IsStandaloneAvailable, "Standalone server not available");

        var config = new StandaloneClientConfigurationBuilder()
            .WithAddress(TestConfiguration.StandaloneHost, TestConfiguration.StandalonePort)
            .WithCompression(CompressionConfig.Zstd(minCompressionSize: 500))
            .Build();

        _client = await GlideClient.CreateClient(config);

        var statsBefore = BaseClient.GetCompressionStatistics();

        // Small value - should not be compressed
        string smallKey = $"small_{Guid.NewGuid()}";
        string smallValue = new string('x', 100);
        await _client.StringSetAsync(smallKey, smallValue);

        var statsAfterSmall = BaseClient.GetCompressionStatistics();
        var skippedCountSmall = statsAfterSmall.CompressionSkippedCount - statsBefore.CompressionSkippedCount;

        // Large value - should be compressed
        string largeKey = $"large_{Guid.NewGuid()}";
        string largeValue = new string('y', 1000);
        await _client.StringSetAsync(largeKey, largeValue);

        var statsAfterLarge = BaseClient.GetCompressionStatistics();
        var compressedCountLarge = statsAfterLarge.TotalValuesCompressed - statsAfterSmall.TotalValuesCompressed;

        Assert.True(skippedCountSmall > 0, "Small value should have been skipped");
        Assert.True(compressedCountLarge > 0, "Large value should have been compressed");
    }

    [Fact]
    public async Task Compression_CustomLevel_WorksCorrectly()
    {
        Skip.IfNot(TestConfiguration.IsStandaloneAvailable, "Standalone server not available");

        var config = new StandaloneClientConfigurationBuilder()
            .WithAddress(TestConfiguration.StandaloneHost, TestConfiguration.StandalonePort)
            .WithCompression(CompressionConfig.Zstd(compressionLevel: 10))
            .Build();

        _client = await GlideClient.CreateClient(config);

        string key = $"compression_level_test_{Guid.NewGuid()}";
        string value = new string('z', 1000);

        await _client.StringSetAsync(key, value);
        var retrieved = await _client.StringGetAsync(key);

        Assert.Equal(value, retrieved.ToString());
    }

    [Fact]
    public async Task Compression_BackwardCompatibility_ReadsUncompressedData()
    {
        Skip.IfNot(TestConfiguration.IsStandaloneAvailable, "Standalone server not available");

        // First, write data without compression
        var configNoCompression = new StandaloneClientConfigurationBuilder()
            .WithAddress(TestConfiguration.StandaloneHost, TestConfiguration.StandalonePort)
            .Build();

        using var clientNoCompression = await GlideClient.CreateClient(configNoCompression);

        string key = $"backward_compat_test_{Guid.NewGuid()}";
        string value = "uncompressed_data";

        await clientNoCompression.StringSetAsync(key, value);

        // Now read with compression enabled
        var configWithCompression = new StandaloneClientConfigurationBuilder()
            .WithAddress(TestConfiguration.StandaloneHost, TestConfiguration.StandalonePort)
            .WithCompression(CompressionConfig.Zstd())
            .Build();

        _client = await GlideClient.CreateClient(configWithCompression);

        var retrieved = await _client.StringGetAsync(key);
        Assert.Equal(value, retrieved.ToString());
    }

    [Fact]
    public async Task Compression_MultipleOperations_MaintainsDataIntegrity()
    {
        Skip.IfNot(TestConfiguration.IsStandaloneAvailable, "Standalone server not available");

        var config = new StandaloneClientConfigurationBuilder()
            .WithAddress(TestConfiguration.StandaloneHost, TestConfiguration.StandalonePort)
            .WithCompression(CompressionConfig.Lz4())
            .Build();

        _client = await GlideClient.CreateClient(config);

        var testData = new Dictionary<string, string>();
        for (int i = 0; i < 10; i++)
        {
            string key = $"multi_op_test_{i}_{Guid.NewGuid()}";
            string value = new string((char)('a' + i), 500 + i * 100);
            testData[key] = value;
            await _client.StringSetAsync(key, value);
        }

        // Verify all data
        foreach (var kvp in testData)
        {
            var retrieved = await _client.StringGetAsync(kvp.Key);
            Assert.Equal(kvp.Value, retrieved.ToString());
        }
    }

    [Fact]
    public async Task Compression_BinaryData_HandlesCorrectly()
    {
        Skip.IfNot(TestConfiguration.IsStandaloneAvailable, "Standalone server not available");

        var config = new StandaloneClientConfigurationBuilder()
            .WithAddress(TestConfiguration.StandaloneHost, TestConfiguration.StandalonePort)
            .WithCompression(CompressionConfig.Zstd())
            .Build();

        _client = await GlideClient.CreateClient(config);

        string key = $"binary_test_{Guid.NewGuid()}";
        byte[] binaryData = new byte[1000];
        new Random().NextBytes(binaryData);

        await _client.StringSetAsync(key, binaryData);
        var retrieved = await _client.StringGetAsync(key);

        Assert.Equal(binaryData, retrieved.ToByteArray());
    }

    [Fact]
    public async Task Compression_Statistics_ReflectOperations()
    {
        Skip.IfNot(TestConfiguration.IsStandaloneAvailable, "Standalone server not available");

        var config = new StandaloneClientConfigurationBuilder()
            .WithAddress(TestConfiguration.StandaloneHost, TestConfiguration.StandalonePort)
            .WithCompression(CompressionConfig.Zstd())
            .Build();

        _client = await GlideClient.CreateClient(config);

        var statsBefore = BaseClient.GetCompressionStatistics();

        string key = $"stats_test_{Guid.NewGuid()}";
        string value = new string('m', 1000);

        await _client.StringSetAsync(key, value);
        await _client.StringGetAsync(key);

        var statsAfter = BaseClient.GetCompressionStatistics();

        Assert.True(statsAfter.TotalValuesCompressed > statsBefore.TotalValuesCompressed);
        Assert.True(statsAfter.TotalOriginalBytes > statsBefore.TotalOriginalBytes);
        Assert.True(statsAfter.TotalBytesCompressed > statsBefore.TotalBytesCompressed);
    }
}
