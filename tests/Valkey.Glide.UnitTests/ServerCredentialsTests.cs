// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.UnitTests;

public class ServerCredentialsTests
{
    #region Constants

    private static readonly string Username = "USERNAME";
    private static readonly string Password = "PASSWORD";

    private static readonly IamAuthConfig IamAuthConfig = new(
        clusterName: "CLUSTER_NAME",
        serviceType: ServiceType.ElastiCache,
        region: "REGION",
        refreshIntervalSeconds: IamAuthConfig.MinRefreshIntervalSeconds + 1);

    #endregion
    #region Tests

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
        var credentials = new ServerCredentials(Username, IamAuthConfig);

        Assert.Equal(Username, credentials.Username);
        Assert.Null(credentials.Password);
        Assert.True(credentials.IsIamAuth());
        Assert.Equal(IamAuthConfig, credentials.IamConfig);
    }

    [Fact]
    public void ServerCredentials_ThrowsArgumentNullException()
    {
        // Password-based authentication.
        _ = Assert.Throws<ArgumentNullException>(() => new ServerCredentials(null!));
        _ = Assert.Throws<ArgumentNullException>(() => new ServerCredentials(Username, (string)null!));

        // IAM authentication.
        _ = Assert.Throws<ArgumentNullException>(() => new ServerCredentials(null!, IamAuthConfig));
        _ = Assert.Throws<ArgumentNullException>(() => new ServerCredentials(Username, (IamAuthConfig)null!));
    }

    [Fact]
    public void ToString_PasswordAuth_OmitsSensitiveInfo()
    {
        var credentials = new ServerCredentials(Username, Password);
        string result = credentials.ToString();

        // Verify that string representation contains the username
        // but omits sensitive information (password).
        Assert.Contains(Username, result);
        Assert.DoesNotContain(Password, result);
    }

    [Fact]
    public void ToString_UsernameIamAuthConfig_OmitsSensitiveInfo()
    {
        var credentials = new ServerCredentials(Username, IamAuthConfig);
        string result = credentials.ToString();

        // Verify that string representation contains the username
        // but omits sensitive information (cluster name, region, and refresh interval).
        // See also <see cref="IamAuthConfigTests.ToString_OmitsSensitiveInfo"/>.
        Assert.Contains(Username, result);
        Assert.DoesNotContain(IamAuthConfig.ClusterName, result);
        Assert.DoesNotContain(IamAuthConfig.Region, result);
        Assert.DoesNotContain(IamAuthConfig.RefreshIntervalSeconds!.Value.ToString(), result);
    }

    #endregion
}
