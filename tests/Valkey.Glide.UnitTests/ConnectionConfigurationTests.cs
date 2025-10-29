// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide;

using Xunit;

using static Valkey.Glide.ConnectionConfiguration;

namespace Valkey.Glide.UnitTests;

public class ConnectionConfigurationTests
{
    // Test constants
    private const string Username = "testUsername";
    private const string Password = "testPassword";
    private const string ClusterName = "testClusterName";
    private const string Region = "testRegion";
    private const int refreshIntervalSeconds = 600;

    [Fact]
    public void WithAuthentication_UsernamePassword()
    {
        var builder = new StandaloneClientConfigurationBuilder();
        builder.WithAuthentication(Username, Password);

        var config = builder.Build();
        var authenticationInfo = config.Request.AuthenticationInfo.Value;

        Assert.Equal(Username, authenticationInfo.Username);
        Assert.Equal(Password, authenticationInfo.Password);
        Assert.Null(authenticationInfo.IamCredentials);

        // Password cannot be null.
        Assert.Throws<ArgumentNullException>(() => builder.WithAuthentication(Username, (string)null));
    }

    [Fact]
    public void WithAuthentication_PasswordOnly()
    {
        var builder = new StandaloneClientConfigurationBuilder();
        builder.WithAuthentication(Password);

        var config = builder.Build();
        var authenticationInfo = config.Request.AuthenticationInfo.Value;

        Assert.Null(authenticationInfo.Username);
        Assert.Equal(Password, authenticationInfo.Password);
        Assert.Null(authenticationInfo.IamCredentials);

        // Password cannot be null.
        Assert.Throws<ArgumentNullException>(() => builder.WithAuthentication((string)null));
    }

    [Fact]
    public void WithAuthentication_UsernameIamAuthConfig_ConfiguresCorrectly()
    {
        var iamConfig = new IamAuthConfig(ClusterName, ServiceType.ElastiCache, Region, refreshIntervalSeconds);
        var builder = new StandaloneClientConfigurationBuilder();
        builder.WithAuthentication(Username, iamConfig);

        var config = builder.Build();
        var authenticationInfo = config.Request.AuthenticationInfo.Value;
        var iamCredentials = authenticationInfo.IamCredentials.Value;

        Assert.Equal(Username, authenticationInfo.Username);
        Assert.Null(authenticationInfo.Password);
        Assert.Equal(ClusterName, iamCredentials.ClusterName);
        Assert.Equal(Region, iamCredentials.Region);
        Assert.Equal((uint)ServiceType.ElastiCache, iamCredentials.ServiceType);
        Assert.Equal(600u, iamCredentials.RefreshIntervalSeconds);

        // Username and IamAuthConfig cannot be null.
        Assert.Throws<ArgumentNullException>(() => builder.WithAuthentication((string)null, iamConfig));
        Assert.Throws<ArgumentNullException>(() => builder.WithAuthentication(Username, (IamAuthConfig)null));
    }

    [Fact]
    public void WithAuthentication_UsernameClusterServiceRegionRefreshInterval()
    {
        var builder = new StandaloneClientConfigurationBuilder();
        builder.WithAuthentication(Username, ClusterName, ServiceType.MemoryDB, Region, refreshIntervalSeconds);

        var config = builder.Build();
        var authenticationInfo = config.Request.AuthenticationInfo.Value;
        var iamCredentials = authenticationInfo.IamCredentials.Value;

        Assert.Equal(Username, authenticationInfo.Username);
        Assert.Null(authenticationInfo.Password);
        Assert.Equal(ClusterName, iamCredentials.ClusterName);
        Assert.Equal(Region, iamCredentials.Region);
        Assert.Equal((uint)ServiceType.MemoryDB, iamCredentials.ServiceType);
        Assert.Equal((uint)refreshIntervalSeconds, iamCredentials.RefreshIntervalSeconds);

        // Username, cluster name, and region cannot be null.
        Assert.Throws<ArgumentNullException>(() => builder.WithAuthentication((string)null, ClusterName, ServiceType.ElastiCache, Region, refreshIntervalSeconds));
        Assert.Throws<ArgumentNullException>(() => builder.WithAuthentication(Username, (string)null, ServiceType.ElastiCache, Region, refreshIntervalSeconds));
        Assert.Throws<ArgumentNullException>(() => builder.WithAuthentication(Username, ClusterName, ServiceType.ElastiCache, (string)null, refreshIntervalSeconds));
    }

    [Fact]
    public void WithAuthentication_MultipleCalls_LastWins()
    {
        // Password-based authentication last.
        var builder = new StandaloneClientConfigurationBuilder();
        builder.WithAuthentication(Username, ClusterName, ServiceType.MemoryDB, Region);
        builder.WithAuthentication(Username, Password);

        var config = builder.Build();
        var authenticationInfo = config.Request.AuthenticationInfo.Value;

        Assert.Equal(Username, authenticationInfo.Username);
        Assert.Equal(Password, authenticationInfo.Password);
        Assert.Null(authenticationInfo.IamCredentials);

        // IAM authentication last.
        builder = new StandaloneClientConfigurationBuilder();
        builder.WithAuthentication(Username, Password);
        builder.WithAuthentication(Username, ClusterName, ServiceType.MemoryDB, Region, refreshIntervalSeconds);

        config = builder.Build();
        authenticationInfo = config.Request.AuthenticationInfo.Value;
        var iamCredentials = authenticationInfo.IamCredentials.Value;

        Assert.Equal(Username, authenticationInfo.Username);
        Assert.Null(authenticationInfo.Password);
        Assert.Equal(ClusterName, iamCredentials.ClusterName);
        Assert.Equal(Region, iamCredentials.Region);
        Assert.Equal((uint)ServiceType.MemoryDB, iamCredentials.ServiceType);
        Assert.Null(iamCredentials.RefreshIntervalSeconds);
    }

    [Fact]
    public void WithCredentials()
    {
        var iamConfig = new IamAuthConfig(ClusterName, ServiceType.MemoryDB, Region);
        var credentials = new ServerCredentials(Username, iamConfig);
        var builder = new StandaloneClientConfigurationBuilder();
        builder.WithCredentials(credentials);

        var config = builder.Build();
        var authenticationInfo = config.Request.AuthenticationInfo.Value;
        var iamCredentials = authenticationInfo.IamCredentials.Value;

        Assert.Equal(Username, authenticationInfo.Username);
        Assert.Null(authenticationInfo.Password);
        Assert.Equal(ClusterName, iamCredentials.ClusterName);
        Assert.Equal(Region, iamCredentials.Region);
        Assert.Equal((uint)ServiceType.MemoryDB, iamCredentials.ServiceType);
        Assert.Null(iamCredentials.RefreshIntervalSeconds);

        // Credentials cannot be null.
        Assert.Throws<ArgumentNullException>(() => builder.WithCredentials((ServerCredentials)null));
    }

    [Fact]
    public void WithCredentials_MultipleCalls_LastWins()
    {
        var iamConfig = new IamAuthConfig(ClusterName, ServiceType.MemoryDB, Region);
        var iamServerCredentials = new ServerCredentials(Username, iamConfig);
        var passwordServerCredentials = new ServerCredentials(Username, Password);

        // Password-based authentication last.
        var builder = new StandaloneClientConfigurationBuilder();
        builder.WithCredentials(iamServerCredentials);
        builder.WithCredentials(passwordServerCredentials);

        var config = builder.Build();
        var authenticationInfo = config.Request.AuthenticationInfo.Value;

        Assert.Equal(Username, authenticationInfo.Username);
        Assert.Equal(Password, authenticationInfo.Password);
        Assert.Null(authenticationInfo.IamCredentials);

        // IAM authentication last.
        builder = new StandaloneClientConfigurationBuilder();
        builder.WithCredentials(passwordServerCredentials);
        builder.WithCredentials(iamServerCredentials);

        config = builder.Build();
        authenticationInfo = config.Request.AuthenticationInfo.Value;
        var iamCredentials = authenticationInfo.IamCredentials.Value;

        Assert.Equal(Username, authenticationInfo.Username);
        Assert.Null(authenticationInfo.Password);
        Assert.Equal(ClusterName, iamCredentials.ClusterName);
        Assert.Equal(Region, iamCredentials.Region);
        Assert.Equal((uint)ServiceType.MemoryDB, iamCredentials.ServiceType);
        Assert.Null(iamCredentials.RefreshIntervalSeconds);
    }
}
