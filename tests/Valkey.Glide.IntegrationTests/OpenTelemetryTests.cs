// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using System.Text.Json;

using Valkey.Glide.Pipeline;

namespace Valkey.Glide.IntegrationTests;

[CollectionDefinition(nameof(OpenTelemetrySequentialCollection), DisableParallelization = true)]
public class OpenTelemetrySequentialCollection { }

[Collection(nameof(OpenTelemetrySequentialCollection))]
public class OpenTelemetryTests : IDisposable
{
    private static readonly uint SamplePercentageNone = 0;
    private static readonly uint SamplePercentageAll = 100;
    private static readonly TimeSpan FlushInterval = TimeSpan.FromMilliseconds(100);
    private static readonly TimeSpan WaitInterval = TimeSpan.FromMilliseconds(1000);

    private static TracesFile? s_traces;

    public OpenTelemetryTests()
    {
        if (s_traces != null)
        {
            return;
        }

        // Initialize OpenTelemetry and traces file.
        s_traces = new TracesFile();

        var tracesConfig = TracesConfig.CreateBuilder()
            .WithEndpoint(s_traces.EndPoint)
            .WithSamplePercentage(SamplePercentageNone)
            .Build();

        var config = OpenTelemetryConfig.CreateBuilder()
            .WithTraces(tracesConfig)
            .WithFlushInterval(FlushInterval)
            .Build();

        OpenTelemetry.Init(config);
    }

    public void Dispose()
    {
        // Disable tracing and clear traces file.
        OpenTelemetry.SetSamplePercentage(SamplePercentageNone);
        s_traces!.Clear();
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task Commands_WhenSamplingNull_NoSpans(BaseClient client)
    {
        OpenTelemetry.SetSamplePercentage(null);
        await ExecuteSetGetDelete(client);
        s_traces!.AssertSpanNames([]);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task Commands_WhenSamplingNone_NoSpans(BaseClient client)
    {
        OpenTelemetry.SetSamplePercentage(SamplePercentageNone);
        await ExecuteSetGetDelete(client);
        s_traces!.AssertSpanNames([]);
    }


    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task Commands_WhenSamplingAll_CreateSpans(BaseClient client)
    {
        OpenTelemetry.SetSamplePercentage(SamplePercentageAll);
        await ExecuteSetGetDelete(client);
        s_traces!.AssertSpanNames(["SET", "GET", "DEL"]);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task Batch_WhenSamplingNull_NoSpans(BaseClient client)
    {
        OpenTelemetry.SetSamplePercentage(null);
        await ExecuteBatchSetGetDelete(client);
        s_traces!.AssertSpanNames([]);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task Batch_WhenSamplingNone_NoSpans(BaseClient client)
    {
        OpenTelemetry.SetSamplePercentage(SamplePercentageNone);
        await ExecuteBatchSetGetDelete(client);
        s_traces!.AssertSpanNames([]);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task Batch_WhenSamplingAll_CreateSpans(BaseClient client)
    {
        OpenTelemetry.SetSamplePercentage(SamplePercentageAll);
        await ExecuteBatchSetGetDelete(client);
        s_traces!.AssertSpanNames(["Batch"]);
    }

    private async Task ExecuteSetGetDelete(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();

        await client.StringSetAsync(key, "value");
        await client.StringGetAsync(key);
        await client.KeyDeleteAsync(key);

        await Task.Delay(WaitInterval);
    }

    private async Task ExecuteBatchSetGetDelete(BaseClient client)
    {
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
            var actualSpanNames = new List<string>();
            foreach (var line in File.ReadAllLines(Path))
            {
                using var doc = JsonDocument.Parse(line);
                if (doc.RootElement.TryGetProperty("name", out var nameElement))
                {
                    actualSpanNames.Add(nameElement.GetString()!);
                }
            }

            Assert.Equal(expectedSpanNames, actualSpanNames);
        }

        public void Clear()
        {
            File.WriteAllText(Path, string.Empty);
        }
    }
}
