// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.UnitTests;

public class IamAuthConfigTests
{
    #region Constants

    private static readonly string ClusterName = "CLUSTER_NAME";
    private static readonly string Region = "REGION";
    private static readonly uint RefreshInterval = IamAuthConfig.MinRefreshIntervalSeconds + 1;

    #endregion
    #region Data

    public static TheoryData<uint> ValidRefreshIntervals =>
        [IamAuthConfig.MinRefreshIntervalSeconds,
         RefreshInterval,
        IamAuthConfig.MaxRefreshIntervalSeconds];

    public static TheoryData<uint> InvalidRefreshIntervals =>
        [uint.MinValue,
        IamAuthConfig.MinRefreshIntervalSeconds - 1,
        IamAuthConfig.MaxRefreshIntervalSeconds + 1,
        uint.MaxValue];

    #endregion
    #region Tests

    [Fact]
    public void Constructor_WithRequiredArgs()
    {
        var config = new IamAuthConfig(ClusterName, ServiceType.ElastiCache, Region);

        Assert.Equal(ClusterName, config.ClusterName);
        Assert.Equal(ServiceType.ElastiCache, config.ServiceType);
        Assert.Equal(Region, config.Region);
        Assert.Null(config.RefreshIntervalSeconds);
    }

    [Fact]
    public void Constructor_WithAllArgs()
    {
        var config = new IamAuthConfig(ClusterName, ServiceType.MemoryDB, Region, RefreshInterval);

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

    [Theory]
    [MemberData(nameof(ValidRefreshIntervals))]
    public void Constructor_ValidRefreshInterval_Succeeds(uint interval)
    {
        var config = new IamAuthConfig(ClusterName, ServiceType.ElastiCache, Region, interval);
        Assert.Equal(interval, config.RefreshIntervalSeconds);
    }

    [Theory]
    [MemberData(nameof(InvalidRefreshIntervals))]
    public void Constructor_InvalidRefreshInterval_Throws(uint interval)
        => _ = Assert.Throws<ArgumentOutOfRangeException>(
            () => new IamAuthConfig(ClusterName, ServiceType.ElastiCache, Region, interval));

    [Fact]
    public void ToString_ContainsServiceType()
    {
        var config = new IamAuthConfig(ClusterName, ServiceType.ElastiCache, Region);
        string result = config.ToString();

        // Verify that string representation contains the service type
        // but not the cluster name, region, or refresh interval.
        Assert.Contains(ServiceType.ElastiCache.ToString(), result);
        Assert.DoesNotContain(ClusterName, result);
        Assert.DoesNotContain(Region, result);
        Assert.DoesNotContain(RefreshInterval.ToString(), result);
    }

    #endregion
}
