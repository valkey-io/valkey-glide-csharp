// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using System.Text.Json;

using Valkey.Glide.Pipeline;

namespace Valkey.Glide.IntegrationTests;

// By default, XUnit tests run sequentially within a collection (by default, each class is a
// collection), but in parallel across collections. To prevent other tests from polluting the
// OpenTelemetry traces, we define a new collection with parallelization disabled.
[CollectionDefinition(nameof(OpenTelemetryCollection), DisableParallelization = true)]
public class OpenTelemetryCollection { }

[Collection(nameof(OpenTelemetryCollection))]
public class OpenTelemetryTests : IDisposable
{
    private static readonly uint SamplePercentageNone = 0;
    private static readonly uint SamplePercentageAll = 100;
    private static readonly TimeSpan FlushInterval = TimeSpan.FromMilliseconds(100);
    private static readonly TimeSpan WaitInterval = TimeSpan.FromMilliseconds(1000);

    private TracesFile Traces { get; }
    private static readonly string TracesFilePath = System.IO.Path.GetTempFileName();

    // When class loads, initialize OpenTelemetry.
    static OpenTelemetryTests()
    {

        var tracesConfig = TracesConfig.CreateBuilder()
            .WithEndpoint($"file://{TracesFilePath}")
            .WithSamplePercentage(SamplePercentageNone)
            .Build();

        var config = OpenTelemetryConfig.CreateBuilder()
            .WithTraces(tracesConfig)
            .WithFlushInterval(FlushInterval)
            .Build();

        OpenTelemetry.Init(config);
    }

    // Before each test, initialize traces file.
    public OpenTelemetryTests()
    {
        Traces = new TracesFile(TracesFilePath);
    }

    // After each test. disbaled traces and dispose of traces file.
    public void Dispose()
    {
        OpenTelemetry.SetSamplePercentage(SamplePercentageNone);
        Traces.Dispose();
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task Commands_WhenSamplingNull_NoSpans(BaseClient client)
    {
        OpenTelemetry.SetSamplePercentage(null);
        await ExecuteSetGetDelete(client);
        Traces!.AssertSpanNames([]);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task Commands_WhenSamplingNone_NoSpans(BaseClient client)
    {
        OpenTelemetry.SetSamplePercentage(SamplePercentageNone);
        await ExecuteSetGetDelete(client);
        Traces!.AssertSpanNames([]);
    }


    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task Commands_WhenSamplingAll_CreateSpans(BaseClient client)
    {
        OpenTelemetry.SetSamplePercentage(SamplePercentageAll);
        await ExecuteSetGetDelete(client);
        Traces!.AssertSpanNames(["SET", "GET", "DEL"]);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task Batch_WhenSamplingNull_NoSpans(BaseClient client)
    {
        OpenTelemetry.SetSamplePercentage(null);
        await ExecuteBatchSetGetDelete(client);
        Traces!.AssertSpanNames([]);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task Batch_WhenSamplingNone_NoSpans(BaseClient client)
    {
        OpenTelemetry.SetSamplePercentage(SamplePercentageNone);
        await ExecuteBatchSetGetDelete(client);
        Traces!.AssertSpanNames([]);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task Batch_WhenSamplingAll_CreateSpans(BaseClient client)
    {
        OpenTelemetry.SetSamplePercentage(SamplePercentageAll);
        await ExecuteBatchSetGetDelete(client);
        Traces!.AssertSpanNames(["Batch"]);
    }

    // Executes SET, GET, and DEL commands on the given client.
    private async Task ExecuteSetGetDelete(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();

        await client.StringSetAsync(key, "value");
        await client.StringGetAsync(key);
        await client.KeyDeleteAsync(key);

        await Task.Delay(WaitInterval);
    }

    // Executes SET, GET, and DEL commands in a batch on the given client.
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

    // Temporary file for storing traces.
    private readonly struct TracesFile : IDisposable
    {
        private readonly string _path;

        public TracesFile(string path)
        {
            _path = path;

            // Ensure file exists and is initially empty.
            File.WriteAllText(path, string.Empty);
        }

        public readonly void Dispose()
        {
            File.Delete(_path);
        }

        public readonly void AssertSpanNames(List<string> expectedSpanNames)
        {
            var lines = File.ReadAllLines(_path);

            var actualSpanNames = new List<string>();
            foreach (var line in lines)
            {
                using var doc = JsonDocument.Parse(line);
                if (doc.RootElement.TryGetProperty("name", out var nameElement))
                {
                    actualSpanNames.Add(nameElement.GetString()!);
                }
            }

            Assert.Equal(expectedSpanNames, actualSpanNames);
        }
    }
}
