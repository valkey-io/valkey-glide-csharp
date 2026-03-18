// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.UnitTests;

public class IamAuthConfigTests
{
    private const string ClusterName = "testClusterName";
    private const string Region = "testRegion";

    [Fact]
    public void NullRefreshInterval_Succeeds()
    {
        var config = new IamAuthConfig(ClusterName, ServiceType.ElastiCache, Region, null);
        Assert.Null(config.RefreshIntervalSeconds);
    }

    [Fact]
    public void BoundaryRefreshInterval_Succeeds()
    {
        var configMin = new IamAuthConfig(ClusterName, ServiceType.ElastiCache, Region, 10);
        Assert.Equal(10u, configMin.RefreshIntervalSeconds);

        var configMax = new IamAuthConfig(ClusterName, ServiceType.ElastiCache, Region, 3600);
        Assert.Equal(3600u, configMax.RefreshIntervalSeconds);
    }

    [Fact]
    public void NullClusterName_Throws()
    {
        _ = Assert.Throws<ArgumentNullException>(
            () => new IamAuthConfig(null!, ServiceType.ElastiCache, Region));
    }

    [Fact]
    public void NullRegion_Throws()
    {
        _ = Assert.Throws<ArgumentNullException>(
            () => new IamAuthConfig(ClusterName, ServiceType.ElastiCache, null!));
    }

    [Fact]
    public void ToString_RedactsSensitiveFields()
    {
        var config = new IamAuthConfig("my-cluster", ServiceType.ElastiCache, "us-east-1");
        string result = config.ToString();

        Assert.Contains("ServiceType", result);
        Assert.DoesNotContain("my-cluster", result);
        Assert.DoesNotContain("us-east-1", result);
    }

    [Fact]
    public void RefreshInterval_BelowMinimum_Throws()
        => _ = Assert.Throws<ArgumentOutOfRangeException>(
            () => new IamAuthConfig(ClusterName, ServiceType.ElastiCache, Region, 1));

    [Fact]
    public void RefreshInterval_AboveMaximum_Throws()
        => _ = Assert.Throws<ArgumentOutOfRangeException>(
            () => new IamAuthConfig(ClusterName, ServiceType.ElastiCache, Region, 86400));
}
