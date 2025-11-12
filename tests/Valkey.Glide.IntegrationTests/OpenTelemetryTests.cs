// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using System.Text.Json;

using Valkey.Glide.Pipeline;

namespace Valkey.Glide.IntegrationTests;

public class OpenTelemetryTests
{
    private static readonly uint SamplePercentageNone = 0;
    private static readonly uint SamplePercentageAll = 100;
    private static readonly TimeSpan FlushInterval = TimeSpan.FromMilliseconds(100);
    private static readonly TimeSpan WaitInterval = TimeSpan.FromMilliseconds(1000);

    private static readonly TracesFile Traces = new();

    public OpenTelemetryTests()
    {
        // Initialize OpenTelemetry.
        var tracesConfig = TracesConfig.CreateBuilder()
            .WithEndpoint(Traces.EndPoint)
            .Build();

        var config = OpenTelemetryConfig.CreateBuilder()
            .WithTraces(tracesConfig)
            .WithFlushInterval(FlushInterval)
            .Build();

        OpenTelemetry.Init(config);

        // Clear traces file.
        Traces.Clear();
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task Commands_WhenSamplingNull_NoSpans(BaseClient client)
    {
        OpenTelemetry.SetSamplePercentage(null);
        await ExecuteSetGetDelete(client);
        Traces.AssertSpanNames([]);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task Commands_WhenSamplingNone_NoSpans(BaseClient client)
    {
        OpenTelemetry.SetSamplePercentage(SamplePercentageNone);
        await ExecuteSetGetDelete(client);
        Traces.AssertSpanNames([]);
    }


    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task Commands_WhenSamplingAll_CreateSpans(BaseClient client)
    {
        OpenTelemetry.SetSamplePercentage(SamplePercentageAll);
        await ExecuteSetGetDelete(client);
        Traces.AssertSpanNames(["SET", "GET", "DEL"]);
    }

    private async Task ExecuteSetGetDelete(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();

        await client.StringSetAsync(key, "value");
        await client.StringGetAsync(key);
        await client.KeyDeleteAsync(key);

        await Task.Delay(WaitInterval);
    }


    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task Batch_WhenSamplingNull_NoSpans(BaseClient client)
    {
        OpenTelemetry.SetSamplePercentage(null);

        string key = Guid.NewGuid().ToString();
        if (client is GlideClient standaloneClient)
        {
            var batch = new Batch(isAtomic: false);
            batch.StringSetAsync(key, "value");
            batch.StringGetAsync(key);
            batch.KeyDelete(key);

            await standaloneClient.Exec(batch, raiseOnError: true);
        }
        else if (client is GlideClusterClient clusterClient)
        {
            var batch = new ClusterBatch(isAtomic: false);
            batch.StringSetAsync(key, "value");
            batch.StringGetAsync(key);
            batch.KeyDelete(key);

            await clusterClient.Exec(batch, raiseOnError: true);
        }

        await Task.Delay(WaitInterval);

        Traces.AssertSpanNames([]);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task Batch_WhenSamplingNone_NoSpans(BaseClient client)
    {
        OpenTelemetry.SetSamplePercentage(SamplePercentageNone);

        string key = Guid.NewGuid().ToString();
        if (client is GlideClient standaloneClient)
        {
            var batch = new Batch(isAtomic: false);
            batch.StringSetAsync(key, "value");
            batch.StringGetAsync(key);
            batch.KeyDelete(key);

            await standaloneClient.Exec(batch, raiseOnError: true);
        }
        else if (client is GlideClusterClient clusterClient)
        {
            var batch = new ClusterBatch(isAtomic: false);
            batch.StringSetAsync(key, "value");
            batch.StringGetAsync(key);
            batch.KeyDelete(key);

            await clusterClient.Exec(batch, raiseOnError: true);
        }

        await Task.Delay(WaitInterval);

        Traces.AssertSpanNames([]);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task Batch_WhenSamplingAll_CreateSpans(BaseClient client)
    {
        OpenTelemetry.SetSamplePercentage(SamplePercentageAll);

        string key = Guid.NewGuid().ToString();
        if (client is GlideClient standaloneClient)
        {
            var batch = new Batch(isAtomic: false);
            batch.StringSetAsync(key, "value");
            batch.StringGetAsync(key);
            batch.KeyDelete(key);

            await standaloneClient.Exec(batch, raiseOnError: true);
        }
        else if (client is GlideClusterClient clusterClient)
        {
            var batch = new ClusterBatch(isAtomic: false);
            batch.StringSetAsync(key, "value");
            batch.StringGetAsync(key);
            batch.KeyDelete(key);

            await clusterClient.Exec(batch, raiseOnError: true);
        }

        await Task.Delay(WaitInterval);

        Traces.AssertSpanNames(["Batch"]);
    }

    /// <summary>
    /// Temporary file for storing traces.
    /// </summary>
    private sealed class TracesFile
    {
        public string Path { get; }
        public string EndPoint { get; }

        public TracesFile()
        {
            Path = System.IO.Path.GetTempFileName();
            EndPoint = $"file://{Path}";
        }

        public void Dispose()
        {
            File.Delete(Path);
        }

        public void AssertSpanNames(List<string> expectedSpanNames)
        {
            // Get spans from file.
            var actualSpanNames = new List<string>();
            foreach (var line in File.ReadAllLines(Path))
            {
                using var doc = JsonDocument.Parse(line);
                if (doc.RootElement.TryGetProperty("name", out var nameElement))
                {
                    actualSpanNames.Add(nameElement.GetString()!);
                }
            }

            // Verify span names.
            Assert.Equal(expectedSpanNames, actualSpanNames);
        }

        public void Clear()
        {
            File.WriteAllText(Path, string.Empty);
        }
    }
}
