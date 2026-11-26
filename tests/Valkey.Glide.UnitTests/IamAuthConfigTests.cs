// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using System.Globalization;

using static Valkey.Glide.ConnectionConfiguration;
using static Valkey.Glide.TestUtils.Builders;

namespace Valkey.Glide.UnitTests;

public class IamAuthConfigTests
{
    #region Constants

    private const string ClusterName = "CLUSTER_NAME";
    private const string Region = "REGION";
    private const uint RefreshInterval = 300;
    private const string Username = "USERNAME";
    private const string Password = "PASSWORD";

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
        Assert.DoesNotContain(RefreshInterval.ToString(CultureInfo.InvariantCulture), result);
    }

    [Fact]
    public void Dispose_ThrowsOnSensitiveAccess()
    {
        var config = BuildIamAuthConfig();
        config.Dispose();

        _ = Assert.Throws<ObjectDisposedException>(() => config.ClusterName);
        _ = Assert.Throws<ObjectDisposedException>(() => config.Region);
        _ = Assert.Throws<ObjectDisposedException>(() => config.RefreshIntervalSeconds);
        _ = Assert.Throws<ObjectDisposedException>(() => config.CredentialsProvider);
    }

    [Fact]
    public void Dispose_IsIdempotent()
    {
        var config = BuildIamAuthConfig();

        config.Dispose();
        config.Dispose(); // Should not throw
    }

    [Fact]
    public void CredentialsProviderDefaultsToNull()
    {
        using var config = BuildIamAuthConfig();

        Assert.Null(config.CredentialsProvider);
    }

    [Fact]
    public void BuilderCapturesAndReplacesCredentialsProvider()
    {
        GlideCredentialProvider provider = _ => Task.FromResult(new AwsCredentials("access-key", "secret-key"));
        var iamConfig = new IamAuthConfig(ClusterName, ServiceType.ElastiCache, Region) { CredentialsProvider = provider };
        var builder = new StandaloneClientConfigurationBuilder().WithAuthentication(Username, iamConfig);

        // Captures the provider, and keeps it after the IAM configuration is disposed.
        Assert.Same(provider, builder.Build().Request.CredentialsProvider);

        iamConfig.Dispose();
        Assert.Same(provider, builder.Build().Request.CredentialsProvider);

        // Replacing with password authentication clears the provider.
        _ = builder.WithAuthentication(Username, Password);
        Assert.Null(builder.Build().Request.CredentialsProvider);

        // Replacing with IAM authentication without a provider clears the provider.
        using var iamConfigWithProvider = new IamAuthConfig(ClusterName, ServiceType.ElastiCache, Region) { CredentialsProvider = provider };
        _ = builder.WithAuthentication(Username, iamConfigWithProvider);
        Assert.Same(provider, builder.Build().Request.CredentialsProvider);

        using var iamConfigWithoutProvider = BuildIamAuthConfig();
        _ = builder.WithAuthentication(Username, iamConfigWithoutProvider);
        Assert.Null(builder.Build().Request.CredentialsProvider);
    }

    #endregion
}
