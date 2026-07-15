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
        using var config = BuildIamAuthConfig(
            clusterName: ClusterName,
            serviceType: ServiceType.ElastiCache,
            region: Region);

        Assert.Equal(ClusterName, config.ClusterName);
        Assert.Equal(ServiceType.ElastiCache, config.ServiceType);
        Assert.Equal(Region, config.Region);
        Assert.Null(config.RefreshIntervalSeconds);
    }

    [Fact]
    public void Constructor_WithOptionalArgs()
    {
        using var config = BuildIamAuthConfig(refreshIntervalSeconds: RefreshInterval);

        Assert.Equal(RefreshInterval, config.RefreshIntervalSeconds);
    }

    [Fact]
    public void Constructor_ThrowsOnNull()
    {
        _ = Assert.Throws<ArgumentNullException>(
            () => new IamAuthConfig(null!, ServiceType.ElastiCache, Region));
        _ = Assert.Throws<ArgumentNullException>(
            () => new IamAuthConfig(ClusterName, ServiceType.ElastiCache, null!));
    }

    [Fact]
    public void ToString_OmitsSensitiveData()
    {
        using var config = BuildIamAuthConfig(
            clusterName: ClusterName,
            region: Region,
            refreshIntervalSeconds: RefreshInterval);

        var result = config.ToString();

        Assert.DoesNotContain(ClusterName, result);
        Assert.DoesNotContain(Region, result);
        Assert.DoesNotContain(RefreshInterval.ToString(), result);
    }

    [Fact]
    public void Dispose_ThrowsOnSensitiveAccess()
    {
        var config = BuildIamAuthConfig();
        config.Dispose();

        _ = Assert.Throws<ObjectDisposedException>(() => config.ClusterName);
        _ = Assert.Throws<ObjectDisposedException>(() => config.Region);
        _ = Assert.Throws<ObjectDisposedException>(() => config.RefreshIntervalSeconds);

    }

    [Fact]
    public void Dispose_IsIdempotent()
    {
        var config = BuildIamAuthConfig();

        config.Dispose();
        config.Dispose(); // Should not throw
    }

    #endregion
    #region Helpers

    // TODO #435: Move to TestUtils class.

    /// <summary>
    /// Builds and returns an IAM authentication configuration for testing.
    /// If required parameters are not specified, default values are used.
    /// </summary>
    private static IamAuthConfig BuildIamAuthConfig(
        string? clusterName = null,
        ServiceType serviceType = ServiceType.ElastiCache,
        string? region = null,
        uint? refreshIntervalSeconds = null)
        => new(clusterName ?? ClusterName, serviceType, region ?? Region, refreshIntervalSeconds);

    #endregion
}
