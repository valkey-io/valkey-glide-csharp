// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.UnitTests;

public class OpenTelemetryConfigTests
{
    private static readonly string Endpoint = "http://localhost:4321";
    private static readonly TimeSpan FlushInterval = TimeSpan.FromMinutes(1);

    [Fact]
    public void WithFlushInterval_WithInvalidInterval_ThrowsArgumentException()
    {
        var builder = OpenTelemetryConfig.CreateBuilder();
        _ = Assert.Throws<ArgumentException>(() => builder.WithFlushInterval(TimeSpan.FromSeconds(-1)));
        _ = Assert.Throws<ArgumentException>(() => builder.WithFlushInterval(TimeSpan.Zero));
    }

    [Fact]
    public void Build_WithoutTracesOrMetrics_ThrowsInvalidOperationException()
    {
        var builder = OpenTelemetryConfig.CreateBuilder();
        _ = Assert.Throws<InvalidOperationException>(builder.Build);
    }

    [Fact]
    public void Build_WithTracesOnly_Succeeds()
    {
        var traces = TracesConfig.CreateBuilder().WithEndpoint(Endpoint).Build();
        var builder = OpenTelemetryConfig.CreateBuilder().WithTraces(traces);
        var config = builder.Build();

        Assert.NotNull(config.Traces);
        Assert.Null(config.Metrics);
        Assert.Null(config.FlushInterval);
    }

    [Fact]
    public void Build_WithMetricsOnly_Succeeds()
    {
        var metrics = MetricsConfig.CreateBuilder().WithEndpoint(Endpoint).Build();
        var builder = OpenTelemetryConfig.CreateBuilder().WithMetrics(metrics);
        var config = builder.Build();

        Assert.Null(config.Traces);
        Assert.NotNull(config.Metrics);
        Assert.Null(config.FlushInterval);
    }

    [Fact]
    public void Build_WithAll_Succeeds()
    {
        var traces = TracesConfig.CreateBuilder().WithEndpoint(Endpoint).Build();
        var metrics = MetricsConfig.CreateBuilder().WithEndpoint(Endpoint).Build();
        var builder = OpenTelemetryConfig.CreateBuilder().WithMetrics(metrics).WithTraces(traces).WithFlushInterval(FlushInterval);
        var config = builder.Build();

        Assert.NotNull(config.Traces);
        Assert.NotNull(config.Metrics);
        Assert.Equal(FlushInterval, config.FlushInterval);
    }
}
