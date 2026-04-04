// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.TestUtils;

namespace Valkey.Glide.UnitTests;

public class TracesConfigTests
{
    #region Tests

    [Theory]
    [MemberData(nameof(Data.InvalidEndpoints), MemberType = typeof(Data))]
    public void WithEndpoint_WithInvalidEndpoint_ThrowsArgumentException(string endpoint)
    {
        var builder = TracesConfig.CreateBuilder();
        _ = Assert.Throws<ArgumentException>(() => builder.WithEndpoint(endpoint));
    }

    [Theory]
    [MemberData(nameof(Data.ValidEndpoints), MemberType = typeof(Data))]
    public void WithEndpoint_WithValidEndpoint_Succeeds(string endpoint)
    {
        var config = TracesConfig.CreateBuilder()
            .WithEndpoint(endpoint)
            .Build();

        Assert.Equal(endpoint, config.Endpoint);
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
    public void Build_WithoutSamplePercentage_Succeeds()
    {
        var endpoint = Data.ValidEndpoints.First();
        var config = TracesConfig.CreateBuilder()
            .WithEndpoint(endpoint)
            .Build();

        Assert.Equal(endpoint, config.Endpoint);
        Assert.Equal(TracesConfig.DefaultSamplePercentage, config.SamplePercentage);
    }

    [Fact]
    public void Build_WithSamplePercentage_Succeeds()
    {
        var endpoint = Data.ValidEndpoints.First();
        var samplePercentage = 50u;

        var config = TracesConfig.CreateBuilder()
            .WithEndpoint(endpoint)
            .WithSamplePercentage(samplePercentage)
            .Build();

        Assert.Equal(endpoint, config.Endpoint);
        Assert.Equal(samplePercentage, config.SamplePercentage);
    }

    #endregion
}
