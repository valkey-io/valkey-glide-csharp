// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.UnitTests;

[Collection(typeof(OpenTelemetryTests))]
[CollectionDefinition(DisableParallelization = true)]
public class OpenTelemetryTests : IDisposable
{
    private const uint SamplePercentageMin = 0u;
    private const uint SamplePercentageLow = 10u;
    private const uint SamplePercentageHigh = 90u;
    private const uint SamplePercentageMax = 100u;
    private const string TestEndpoint = "http://localhost:4317";

    // After each test, clear OpenTelemetry.
    public void Dispose()
    {
        OpenTelemetry.Clear();
    }

    [Fact]
    public void Init_WithNullConfig_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => OpenTelemetry.Init(null!));
    }

    [Fact]
    public void Init_WithValidConfig_Succeeds()
    {
        var traces = TracesConfig.CreateBuilder()
            .WithEndpoint(TestEndpoint)
            .WithSamplePercentage(SamplePercentageLow)
            .Build();
        var config = OpenTelemetryConfig.CreateBuilder()
            .WithTraces(traces)
            .Build();

        OpenTelemetry.Init(config);

        Assert.True(OpenTelemetry.IsInitialized());
        Assert.Equal(SamplePercentageLow, OpenTelemetry.GetSamplePercentage());
    }

    [Fact]
    public void Init_CalledTwice_IgnoresSecondCall()
    {
        var traces1 = TracesConfig.CreateBuilder()
            .WithEndpoint(TestEndpoint)
            .WithSamplePercentage(SamplePercentageLow)
            .Build();
        var config1 = OpenTelemetryConfig.CreateBuilder()
            .WithTraces(traces1)
            .Build();

        var traces2 = TracesConfig.CreateBuilder()
            .WithEndpoint(TestEndpoint)
            .WithSamplePercentage(SamplePercentageHigh)
            .Build();
        var config2 = OpenTelemetryConfig.CreateBuilder()
            .WithTraces(traces2)
            .Build();

        OpenTelemetry.Init(config1);
        OpenTelemetry.Init(config2);

        Assert.True(OpenTelemetry.IsInitialized());
        Assert.Equal(SamplePercentageLow, OpenTelemetry.GetSamplePercentage());
    }

    [Fact]
    public void IsInitialized_WhenNotInitialized_ReturnsFalse()
    {
        Assert.False(OpenTelemetry.IsInitialized());
    }

    [Fact]
    public void GetSamplePercentage_WhenNotInitialized_ReturnsNull()
    {
        Assert.Null(OpenTelemetry.GetSamplePercentage());
    }

    [Fact]
    public void GetSamplePercentage_WithTracesConfigured_ReturnsDefault()
    {
        var traces = TracesConfig.CreateBuilder()
            .WithEndpoint(TestEndpoint)
            .Build();
        var config = OpenTelemetryConfig.CreateBuilder()
            .WithTraces(traces)
            .Build();

        OpenTelemetry.Init(config);

        Assert.Equal(TracesConfig.DefaultSamplePercentage, OpenTelemetry.GetSamplePercentage());
    }

    [Fact]
    public void GetSamplePercentage_WithSamplePercentageConfigured_ReturnsPercentage()
    {
        var traces = TracesConfig.CreateBuilder()
            .WithEndpoint(TestEndpoint)
            .WithSamplePercentage(SamplePercentageLow)
            .Build();
        var config = OpenTelemetryConfig.CreateBuilder()
            .WithTraces(traces)
            .Build();

        OpenTelemetry.Init(config);

        Assert.Equal(SamplePercentageLow, OpenTelemetry.GetSamplePercentage());
    }

    [Fact]
    public void GetSamplePercentage_WithoutTracesConfigured_ReturnsNull()
    {
        var metrics = MetricsConfig.CreateBuilder()
            .WithEndpoint(TestEndpoint)
            .Build();
        var config = OpenTelemetryConfig.CreateBuilder()
            .WithMetrics(metrics)
            .Build();

        OpenTelemetry.Init(config);

        Assert.Null(OpenTelemetry.GetSamplePercentage());
    }

    [Fact]
    public void SetSamplePercentage_WhenNotInitialized_ThrowsInvalidOperationException()
    {
        Assert.Throws<InvalidOperationException>(() => OpenTelemetry.SetSamplePercentage(SamplePercentageLow));
    }

    [Fact]
    public void SetSamplePercentage_WithoutTracesConfigured_ThrowsInvalidOperationException()
    {
        var metrics = MetricsConfig.CreateBuilder()
            .WithEndpoint(TestEndpoint)
            .Build();
        var config = OpenTelemetryConfig.CreateBuilder()
            .WithMetrics(metrics)
            .Build();

        OpenTelemetry.Init(config);

        Assert.Throws<InvalidOperationException>(() => OpenTelemetry.SetSamplePercentage(SamplePercentageLow));
    }

    [Fact]
    public void SetSamplePercentage_WithValidPercentage_UpdatesValue()
    {
        var traces = TracesConfig.CreateBuilder()
            .WithEndpoint(TestEndpoint)
            .WithSamplePercentage(SamplePercentageLow)
            .Build();
        var config = OpenTelemetryConfig.CreateBuilder()
            .WithTraces(traces)
            .Build();

        OpenTelemetry.Init(config);
        OpenTelemetry.SetSamplePercentage(SamplePercentageHigh);

        Assert.Equal(SamplePercentageHigh, OpenTelemetry.GetSamplePercentage());
    }

    [Fact]
    public void SetSamplePercentage_WithInvalidPercentage_ThrowsArgumentException()
    {
        var traces = TracesConfig.CreateBuilder()
            .WithEndpoint(TestEndpoint)
            .Build();
        var config = OpenTelemetryConfig.CreateBuilder()
            .WithTraces(traces)
            .Build();

        OpenTelemetry.Init(config);

        Assert.Throws<ArgumentException>(() => OpenTelemetry.SetSamplePercentage(101));
    }

    [Fact]
    public void SetSamplePercentage_WithMinPercentage_Succeeds()
    {
        var traces = TracesConfig.CreateBuilder()
            .WithEndpoint(TestEndpoint)
            .Build();
        var config = OpenTelemetryConfig.CreateBuilder()
            .WithTraces(traces)
            .Build();

        OpenTelemetry.Init(config);
        OpenTelemetry.SetSamplePercentage(SamplePercentageMin);

        Assert.Equal(SamplePercentageMin, OpenTelemetry.GetSamplePercentage());
    }

    [Fact]
    public void SetSamplePercentage_WithMaxPercentage_Succeeds()
    {
        var traces = TracesConfig.CreateBuilder()
            .WithEndpoint(TestEndpoint)
            .Build();
        var config = OpenTelemetryConfig.CreateBuilder()
            .WithTraces(traces)
            .Build();

        OpenTelemetry.Init(config);
        OpenTelemetry.SetSamplePercentage(SamplePercentageMax);

        Assert.Equal(SamplePercentageMax, OpenTelemetry.GetSamplePercentage());
    }

    [Fact]
    public void Clear_ResetsInitializationState()
    {
        var traces = TracesConfig.CreateBuilder()
            .WithEndpoint(TestEndpoint)
            .Build();
        var config = OpenTelemetryConfig.CreateBuilder()
            .WithTraces(traces)
            .Build();

        OpenTelemetry.Init(config);
        Assert.True(OpenTelemetry.IsInitialized());

        OpenTelemetry.Clear();
        Assert.False(OpenTelemetry.IsInitialized());
        Assert.Null(OpenTelemetry.GetSamplePercentage());
    }
}
