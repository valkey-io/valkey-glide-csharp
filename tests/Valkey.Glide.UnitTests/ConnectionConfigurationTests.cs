// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Xunit;
using Valkey.Glide;
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
    public void ClientConfigurationBuilder_WithAuthentication_UsernamePassword_ConfiguresCorrectly()
    {
        var builder = new StandaloneClientConfigurationBuilder();
        builder.WithAuthentication(Username, Password);

        var config = builder.Build();

        Assert.NotNull(config.Request.AuthenticationInfo);
        Assert.Equal(Username, config.Request.AuthenticationInfo.Value.Username);
        Assert.Equal(Password, config.Request.AuthenticationInfo.Value.Password);
        Assert.Null(config.Request.AuthenticationInfo.Value.IamCredentials);
    }

    [Fact]
    public void ClientConfigurationBuilder_WithAuthentication_PasswordOnly_ConfiguresCorrectly()
    {
        var builder = new StandaloneClientConfigurationBuilder();
        builder.WithAuthentication(Password);

        var config = builder.Build();

        Assert.NotNull(config.Request.AuthenticationInfo);
        Assert.Null(config.Request.AuthenticationInfo.Value.Username);
        Assert.Equal(Password, config.Request.AuthenticationInfo.Value.Password);
        Assert.Null(config.Request.AuthenticationInfo.Value.IamCredentials);
    }

    [Fact]
    public void ClientConfigurationBuilder_WithAuthentication_IamAuthConfig_ConfiguresCorrectly()
    {
        var iamConfig = new IamAuthConfig(ClusterName, ServiceType.ElastiCache, Region, 600);
        var builder = new StandaloneClientConfigurationBuilder();
        builder.WithAuthentication(Username, iamConfig);

        var config = builder.Build();

        Assert.NotNull(config.Request.AuthenticationInfo);
        Assert.Equal(Username, config.Request.AuthenticationInfo.Value.Username);
        Assert.Null(config.Request.AuthenticationInfo.Value.Password);
        Assert.NotNull(config.Request.AuthenticationInfo.Value.IamCredentials);
        Assert.Equal(ClusterName, config.Request.AuthenticationInfo.Value.IamCredentials.Value.ClusterName);
        Assert.Equal(Region, config.Request.AuthenticationInfo.Value.IamCredentials.Value.Region);
        Assert.Equal(0u, config.Request.AuthenticationInfo.Value.IamCredentials.Value.ServiceType);
        Assert.Equal(600u, config.Request.AuthenticationInfo.Value.IamCredentials.Value.RefreshIntervalSeconds);
    }

    [Fact]
    public void ClientConfigurationBuilder_WithCredentials_ServerCredentials_ConfiguresCorrectly()
    {
        var iamConfig = new IamAuthConfig(ClusterName, ServiceType.MemoryDB, Region);
        var credentials = new ServerCredentials(Username, iamConfig);
        var builder = new StandaloneClientConfigurationBuilder();
        builder.WithCredentials(credentials);
        var config = builder.Build();

        Assert.NotNull(config.Request.AuthenticationInfo);
        Assert.Equal(Username, config.Request.AuthenticationInfo.Value.Username);
        Assert.Null(config.Request.AuthenticationInfo.Value.Password);
        Assert.NotNull(config.Request.AuthenticationInfo.Value.IamCredentials);
        Assert.Equal(1u, config.Request.AuthenticationInfo.Value.IamCredentials.Value.ServiceType); // MemoryDB = 1
    }
}
