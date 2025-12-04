// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using System.Text.Json;

using Valkey.Glide.Pipeline;
using Valkey.Glide.TestUtils;

namespace Valkey.Glide.IntegrationTests;

[Collection(typeof(OpenTelemetryTests))]
[CollectionDefinition(DisableParallelization = true)]
public class OpenTelemetryTests : IDisposable
{
    private static readonly uint SamplePercentageNone = 0u;
    private static readonly uint SamplePercentageAll = 100u;
    private static readonly TimeSpan FlushInterval = TimeSpan.FromMilliseconds(100);
    private static readonly TimeSpan WaitInterval = TimeSpan.FromMilliseconds(1000);

    private static readonly TempFile TracesFile = new TempFile();

    static OpenTelemetryTests()
    {
        // Before any tests, initialize OpenTelemetry.
        // Ensure it's not already initialized from other tests.
        Assert.False(OpenTelemetry.IsInitialized());

        var tracesConfig = TracesConfig.CreateBuilder()
            .WithEndpoint($"file://{TracesFile.Path}")
            .Build();

        var config = OpenTelemetryConfig.CreateBuilder()
            .WithTraces(tracesConfig)
            .WithFlushInterval(FlushInterval)
            .Build();

        OpenTelemetry.Init(config);
    }

    public void Dispose()
    {
        // After each test, turn off tracing and clear traces file.
        OpenTelemetry.SetSamplePercentage(SamplePercentageNone);
        TracesFile.Text = string.Empty;
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task Commands_WhenSamplingNone_NoSpans(BaseClient client)
    {
        OpenTelemetry.SetSamplePercentage(SamplePercentageNone);
        await ExecuteSetGetDelete(client);
        AssertSpanNames([]);
    }


    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task Commands_WhenSamplingAll_CreateSpans(BaseClient client)
    {
        OpenTelemetry.SetSamplePercentage(SamplePercentageAll);
        await ExecuteSetGetDelete(client);
        AssertSpanNames(["SET", "GET", "DEL"]);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task Batch_WhenSamplingNone_NoSpans(BaseClient client)
    {
        OpenTelemetry.SetSamplePercentage(SamplePercentageNone);
        await ExecuteBatchSetGetDelete(client);
        AssertSpanNames([]);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task Batch_WhenSamplingAll_CreateSpans(BaseClient client)
    {
        OpenTelemetry.SetSamplePercentage(SamplePercentageAll);
        await ExecuteBatchSetGetDelete(client);
        AssertSpanNames(["Batch"]);
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

    private void AssertSpanNames(List<string> expectedSpanNames)
    {
        var actualSpanNames = new List<string>();
        foreach (var line in TracesFile.Lines)
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
