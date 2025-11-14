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
        Assert.Throws<ArgumentException>(() => builder.WithFlushInterval(TimeSpan.FromSeconds(-1)));
        Assert.Throws<ArgumentException>(() => builder.WithFlushInterval(TimeSpan.Zero));
    }

    [Fact]
    public void Build_WithoutTracesOrMetrics_ThrowsInvalidOperationException()
    {
        var builder = OpenTelemetryConfig.CreateBuilder();
        Assert.Throws<InvalidOperationException>(() => builder.Build());
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

public class TracesConfigTests
{
    private static readonly uint SamplePercentage = 50;
    private static readonly string Endpoint = "http://localhost:4321";

    [Fact]
    public void WithEndpoint_WithInvalidEndpoint_ThrowsArgumentException()
    {
        var builder = TracesConfig.CreateBuilder();
        Assert.Throws<ArgumentException>(() => builder.WithEndpoint(null!));
        Assert.Throws<ArgumentException>(() => builder.WithEndpoint(""));
        Assert.Throws<ArgumentException>(() => builder.WithEndpoint("\t"));
    }

    [Fact]
    public void WithSamplePercentage_WithInvalidPercentage_ThrowsArgumentException()
    {
        var builder = TracesConfig.CreateBuilder();
        Assert.Throws<ArgumentException>(() => builder.WithSamplePercentage(101));
    }

    [Fact]
    public void Build_WithoutEndpoint_ThrowsInvalidOperationException()
    {
        var builder = TracesConfig.CreateBuilder();
        Assert.Throws<InvalidOperationException>(() => builder.Build());
    }

    [Fact]
    public void Build_WithSamplePercentage_Succeeds()
    {
        var config = TracesConfig.CreateBuilder()
            .WithEndpoint(Endpoint)
            .Build();

        Assert.Equal(Endpoint, config.Endpoint);
        Assert.Equal(TracesConfig.DefaultSamplePercentage, config.SamplePercentage);
    }

    [Fact]
    public void Build_WithoutSamplePercentage_Succeeds()
    {
        // Arrange & Act
        var config = TracesConfig.CreateBuilder()
            .WithEndpoint(Endpoint)
            .WithSamplePercentage(SamplePercentage)
            .Build();

        Assert.Equal(Endpoint, config.Endpoint);
        Assert.Equal(SamplePercentage, config.SamplePercentage);
    }
}

public class MetricsConfigTests
{
    private static readonly string Endpoint = "http://localhost:4321";

    [Fact]
    public void WithEndpoint_WithInvalidEndpoint_ThrowsArgumentException()
    {
        var builder = MetricsConfig.CreateBuilder();
        Assert.Throws<ArgumentException>(() => builder.WithEndpoint(null!));
        Assert.Throws<ArgumentException>(() => builder.WithEndpoint(""));
        Assert.Throws<ArgumentException>(() => builder.WithEndpoint("\t"));
    }

    [Fact]
    public void Build_WithoutEndpoint_ThrowsInvalidOperationException()
    {
        var builder = MetricsConfig.CreateBuilder();
        Assert.Throws<InvalidOperationException>(() => builder.Build());
    }

    [Fact]
    public void Build_WithEndpoint_Succeeds()
    {
        var config = MetricsConfig.CreateBuilder()
            .WithEndpoint(Endpoint)
            .Build();

        Assert.Equal(Endpoint, config.Endpoint);
    }
}
