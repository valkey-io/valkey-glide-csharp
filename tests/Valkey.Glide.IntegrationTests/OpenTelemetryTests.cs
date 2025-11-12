// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using System.Text.Json;

namespace Valkey.Glide.IntegrationTests;

public class OpenTelemetryTests
{
    private static readonly uint SamplePercentageNone = 0;
    private static readonly uint SamplePercentageAll = 100;
    private static readonly TimeSpan FlushInterval = TimeSpan.FromMilliseconds(100);
    private static readonly TimeSpan WaitInterval = TimeSpan.FromMilliseconds(1000);

    private static readonly TempTracesFile TracesFile = new();

    public OpenTelemetryTests()
    {
        // Initialize OpenTelemetry.
        var tracesConfig = TracesConfig.CreateBuilder()
            .WithEndpoint(TracesFile.EndPoint)
            .Build();

        var config = OpenTelemetryConfig.CreateBuilder()
            .WithTraces(tracesConfig)
            .WithFlushInterval(FlushInterval)
            .Build();

        OpenTelemetry.Init(config);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task Commands_WhenSamplingNull_NoSpans(BaseClient client)
    {
        OpenTelemetry.SetSamplePercentage(null);
        await ExecuteSetGetDelete(client);
        TracesFile.AssertSpanNames([]);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task Commands_WhenSamplingNone_NoSpans(BaseClient client)
    {
        OpenTelemetry.SetSamplePercentage(SamplePercentageNone);
        await ExecuteSetGetDelete(client);
        TracesFile.AssertSpanNames([]);
    }


    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task Commands_WhenSamplingAll_CreateSpans(BaseClient client)
    {
        OpenTelemetry.SetSamplePercentage(SamplePercentageAll);
        await ExecuteSetGetDelete(client);
        TracesFile.AssertSpanNames(["SET", "GET", "DEL"]);
    }

    private async Task ExecuteSetGetDelete(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();

        await client.StringSetAsync(key, "value");
        await client.StringGetAsync(key);
        await client.KeyDeleteAsync(key);

        await Task.Delay(WaitInterval);
    }

    /// <summary>
    /// Temporary file for storing traces.
    /// </summary>
    private sealed class TempTracesFile
    {
        public string Path { get; }
        public string EndPoint { get; }

        public TempTracesFile()
        {
            Path = System.IO.Path.GetTempFileName();
            EndPoint = $"file://{Path}";
        }

        public void Dispose()
        {
            File.Delete(Path);
        }

        /// <summary>
        /// Asserts that the span names in the traces file match the expected names.
        /// </summary>
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

            // Clear file for next use.
            File.WriteAllText(Path, string.Empty);
        }
    }
}
