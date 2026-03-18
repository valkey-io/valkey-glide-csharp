// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.UnitTests;

public class TracesConfigTests
{
    #region Constants

    private static readonly uint SamplePercentage = 50;
    private static readonly string Endpoint = "http://localhost:4321";

    #endregion
    #region Tests

    [Fact]
    public void WithEndpoint_WithInvalidEndpoint_ThrowsArgumentException()
    {
        var builder = TracesConfig.CreateBuilder();
        _ = Assert.Throws<ArgumentException>(() => builder.WithEndpoint(null!));
        _ = Assert.Throws<ArgumentException>(() => builder.WithEndpoint(""));
        _ = Assert.Throws<ArgumentException>(() => builder.WithEndpoint("\t"));
    }

    [Fact]
    public void WithSamplePercentage_WithInvalidPercentage_ThrowsArgumentException()
    {
        var builder = TracesConfig.CreateBuilder();
        _ = Assert.Throws<ArgumentException>(() => builder.WithSamplePercentage(101));
    }

    [Fact]
    public void Build_WithoutEndpoint_ThrowsInvalidOperationException()
    {
        var builder = TracesConfig.CreateBuilder();
        _ = Assert.Throws<InvalidOperationException>(builder.Build);
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

    [Fact]
    public void WithEndpoint_WithInvalidUri_ThrowsArgumentException()
    {
        var builder = TracesConfig.CreateBuilder();
        _ = Assert.Throws<ArgumentException>(() => builder.WithEndpoint("not-a-url"));
        _ = Assert.Throws<ArgumentException>(() => builder.WithEndpoint("://missing-scheme"));
        _ = Assert.Throws<ArgumentException>(() => builder.WithEndpoint("just some text"));
    }

    #endregion
}
