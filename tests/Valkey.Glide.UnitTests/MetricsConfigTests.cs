// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.TestUtils;

namespace Valkey.Glide.UnitTests;

public class MetricsConfigTests
{
    #region Tests

    [Theory]
    [MemberData(nameof(Data.InvalidEndpoints), MemberType = typeof(Data))]
    public void WithEndpoint_WithInvalidEndpoint_ThrowsArgumentException(string endpoint)
    {
        var builder = MetricsConfig.CreateBuilder();
        _ = Assert.Throws<ArgumentException>(() => builder.WithEndpoint(endpoint));
    }

    [Theory]
    [MemberData(nameof(Data.ValidEndpoints), MemberType = typeof(Data))]
    public void WithEndpoint_WithValidEndpoint_Succeeds(string endpoint)
    {
        var config = MetricsConfig.CreateBuilder()
            .WithEndpoint(endpoint)
            .Build();

        Assert.Equal(endpoint, config.Endpoint);
    }

    [Fact]
    public void Build_WithoutEndpoint_ThrowsInvalidOperationException()
    {
        var builder = MetricsConfig.CreateBuilder();
        _ = Assert.Throws<InvalidOperationException>(builder.Build);
    }

    #endregion
}
