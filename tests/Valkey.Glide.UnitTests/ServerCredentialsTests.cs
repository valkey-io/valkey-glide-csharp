// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.UnitTests;

public class ServerCredentialsTests
{
    // Test constants
    private const string Username = "testUsername";
    private const string Password = "testPassword";
    private const string ClusterName = "testClusterName";
    private const string Region = "testRegion";

    [Fact]
    public void ServerCredentials_UsernamePassword()
    {
        var credentials = new ServerCredentials(Username, Password);

        Assert.Equal(Username, credentials.Username);
        Assert.Equal(Password, credentials.Password);
        Assert.Null(credentials.IamConfig);
        Assert.False(credentials.IsIamAuth());
    }

    [Fact]
    public void ServerCredentials_PasswordOnly()
    {
        var credentials = new ServerCredentials(Password);

        Assert.Null(credentials.Username);
        Assert.Equal(Password, credentials.Password);
        Assert.Null(credentials.IamConfig);
        Assert.False(credentials.IsIamAuth());
    }

    [Fact]
    public void ServerCredentials_UsernameIamAuthConfig()
    {
        var iamConfig = new IamAuthConfig(ClusterName, ServiceType.ElastiCache, Region);
        var credentials = new ServerCredentials(Username, iamConfig);

        Assert.Equal(Username, credentials.Username);
        Assert.Null(credentials.Password);
        Assert.Equal(ClusterName, credentials.IamConfig!.ClusterName);
        Assert.Equal(ServiceType.ElastiCache, credentials.IamConfig!.ServiceType);
        Assert.Equal(Region, credentials.IamConfig!.Region);
        Assert.Null(credentials.IamConfig!.RefreshIntervalSeconds);
        Assert.True(credentials.IsIamAuth());
    }

    [Fact]
    public void ServerCredentials_UsernameIamAuthConfigWithCustomRefresh()
    {
        var iamConfig = new IamAuthConfig(ClusterName, ServiceType.MemoryDB, Region, 600);
        var credentials = new ServerCredentials("iamUser", iamConfig);

        Assert.Equal(ServiceType.MemoryDB, credentials.IamConfig!.ServiceType);
        Assert.Equal(600u, credentials.IamConfig!.RefreshIntervalSeconds);
        Assert.True(credentials.IsIamAuth());
    }

    [Fact]
    public void ServerCredentials_ThrowsArgumentNullException()
    {
        // Password-based authentication.
        Assert.Throws<ArgumentNullException>(() => new ServerCredentials(null!));
        Assert.Throws<ArgumentNullException>(() => new ServerCredentials(Username, (string)null!));

        // IAM authentication.
        var iamConfig = new IamAuthConfig(ClusterName, ServiceType.ElastiCache, Region);
        Assert.Throws<ArgumentNullException>(() => new ServerCredentials(null!, iamConfig));
        Assert.Throws<ArgumentNullException>(() => new ServerCredentials(Username, (IamAuthConfig)null!));
    }
}
