// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.UnitTests;

public class IamAuthConfigTests
{
    #region Constants

    private static readonly string ClusterName = "CLUSTER_NAME";
    private static readonly string Region = "REGION";
    private const uint RefreshInterval = 300;

    #endregion
    #region Tests

    [Fact]
    public void Constructor_WithRequiredArgs()
    {
        using var config = new IamAuthConfig(ClusterName, ServiceType.ElastiCache, Region);

        Assert.Equal(ClusterName, config.ClusterName);
        Assert.Equal(ServiceType.ElastiCache, config.ServiceType);
        Assert.Equal(Region, config.Region);
        Assert.Null(config.RefreshIntervalSeconds);
    }

    [Fact]
    public void Constructor_WithAllArgs()
    {
        using var config = new IamAuthConfig(ClusterName, ServiceType.MemoryDB, Region, RefreshInterval);

        Assert.Equal(ClusterName, config.ClusterName);
        Assert.Equal(ServiceType.MemoryDB, config.ServiceType);
        Assert.Equal(Region, config.Region);
        Assert.Equal(RefreshInterval, config.RefreshIntervalSeconds);
    }

    [Fact]
    public void Constructor_NullClusterName_Throws()
        => _ = Assert.Throws<ArgumentNullException>(
            () => new IamAuthConfig(null!, ServiceType.ElastiCache, Region));

    [Fact]
    public void Constructor_NullRegion_Throws()
        => _ = Assert.Throws<ArgumentNullException>(
            () => new IamAuthConfig(ClusterName, ServiceType.ElastiCache, null!));

    [Fact]
    public void ToString_ContainsServiceType()
    {
        using var config = new IamAuthConfig(ClusterName, ServiceType.ElastiCache, Region);
        string result = config.ToString();

        // Verify that string representation contains the service type
        // but not the cluster name, region, or refresh interval.
        Assert.Contains(ServiceType.ElastiCache.ToString(), result);
        Assert.DoesNotContain(ClusterName, result);
        Assert.DoesNotContain(Region, result);
        Assert.DoesNotContain(RefreshInterval.ToString(), result);
    }

    [Fact]
    public void Dispose_AllPublicMembers_ThrowObjectDisposedException()
    {
        var config = new IamAuthConfig(ClusterName, ServiceType.MemoryDB, Region, RefreshInterval);

        config.Dispose();

        _ = Assert.Throws<ObjectDisposedException>(() => config.ClusterName);
        _ = Assert.Throws<ObjectDisposedException>(() => config.ServiceType);
        _ = Assert.Throws<ObjectDisposedException>(() => config.Region);
        _ = Assert.Throws<ObjectDisposedException>(() => config.RefreshIntervalSeconds);
        _ = Assert.Throws<ObjectDisposedException>(() => _ = config.ToString());
    }

    #endregion
}
