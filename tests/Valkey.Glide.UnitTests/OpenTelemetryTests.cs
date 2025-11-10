// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.UnitTests;

public class OpenTelemetryTests
{
    private static readonly string Endpoint = "http://localhost:4321";

    public OpenTelemetryTests()
    {
        OpenTelemetry.Clear();
    }

    [Fact]
    public void Init()
    {
        Assert.False(OpenTelemetry.IsInitialized());

        // Throws when null.
        Assert.Throws<ArgumentNullException>(() => OpenTelemetry.Init(null!));
        Assert.False(OpenTelemetry.IsInitialized());

        var tracesConfig = TracesConfig.CreateBuilder()
            .WithEndpoint(Endpoint)
            .Build();
        var config = OpenTelemetryConfig.CreateBuilder()
            .WithTraces(tracesConfig)
            .Build();

        OpenTelemetry.Init(config);

        Assert.True(OpenTelemetry.IsInitialized());
    }

    [Fact]
    public void SetSamplePercentage()
    {
        Assert.Null(OpenTelemetry.GetSamplePercentage());

        // Throws when not initialized.
        Assert.Throws<InvalidOperationException>(() => OpenTelemetry.SetSamplePercentage(50));
        Assert.Null(OpenTelemetry.GetSamplePercentage());

        var tracesConfig = TracesConfig.CreateBuilder()
            .WithEndpoint(Endpoint)
            .Build();
        var config = OpenTelemetryConfig.CreateBuilder()
            .WithTraces(tracesConfig)
            .Build();

        OpenTelemetry.Init(config);

        Assert.Null(OpenTelemetry.GetSamplePercentage());

        // Throws when out of range.
        Assert.Throws<ArgumentException>(() => OpenTelemetry.SetSamplePercentage(101));

        uint percentage = 11;
        OpenTelemetry.SetSamplePercentage(percentage);
        Assert.Equal(percentage, OpenTelemetry.GetSamplePercentage());
    }

    [Fact]
    public void ShouldSample()
    {
        // False when not initialized.
        Assert.False(OpenTelemetry.ShouldSample());

        var tracesConfig = TracesConfig.CreateBuilder()
            .WithEndpoint(Endpoint)
            .Build();
        var config = OpenTelemetryConfig.CreateBuilder()
            .WithTraces(tracesConfig)
            .Build();

        OpenTelemetry.Init(config);

        // False when sample percentage not specified.
        Assert.False(OpenTelemetry.ShouldSample());

        // False when sample percentage is 0.
        OpenTelemetry.SetSamplePercentage(0u);
        Assert.False(OpenTelemetry.ShouldSample());

        // True when sample percentage is 100.
        OpenTelemetry.SetSamplePercentage(100u);
        Assert.True(OpenTelemetry.ShouldSample());
    }
}
