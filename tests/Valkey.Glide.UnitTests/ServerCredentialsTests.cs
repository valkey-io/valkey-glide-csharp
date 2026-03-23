// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.UnitTests;

public class ServerCredentialsTests
{
    #region Constants

    private static readonly string Username = "USERNAME";
    private static readonly string Password = "PASSWORD";

    #endregion
    #region Tests

    [Fact]
    public void ServerCredentials_UsernamePassword()
    {
        using var credentials = new ServerCredentials(Username, Password);

        Assert.Equal(Username, credentials.Username);
        Assert.Equal(Password, credentials.Password);
        Assert.Null(credentials.IamConfig);
        Assert.False(credentials.IsIamAuth());
    }

    [Fact]
    public void ServerCredentials_PasswordOnly()
    {
        using var credentials = new ServerCredentials(Password);

        Assert.Null(credentials.Username);
        Assert.Equal(Password, credentials.Password);
        Assert.Null(credentials.IamConfig);
        Assert.False(credentials.IsIamAuth());
    }

    [Fact]
    public void ServerCredentials_UsernameIamAuthConfig()
    {
        using var iamAuthConfig = BuildIamAuthConfig();
        using var credentials = new ServerCredentials(Username, iamAuthConfig);

        Assert.Equal(Username, credentials.Username);
        Assert.Null(credentials.Password);
        Assert.True(credentials.IsIamAuth());
        Assert.Equal(iamAuthConfig, credentials.IamConfig);
    }

    [Fact]
    public void ServerCredentials_ThrowsArgumentNullException()
    {
        // Password-based authentication.
        _ = Assert.Throws<ArgumentNullException>(() => new ServerCredentials(null!));
        _ = Assert.Throws<ArgumentNullException>(() => new ServerCredentials(Username, (string)null!));

        // IAM authentication.
        using var iamAuthConfig = BuildIamAuthConfig();
        _ = Assert.Throws<ArgumentNullException>(() => new ServerCredentials(null!, iamAuthConfig));
        _ = Assert.Throws<ArgumentNullException>(() => new ServerCredentials(Username, (IamAuthConfig)null!));
    }

    [Fact]
    public void ToString_PasswordAuth_OmitsSensitiveInfo()
    {
        using var credentials = new ServerCredentials(Username, Password);
        string result = credentials.ToString();

        // Verify that string representation contains the username
        // but omits sensitive information (password).
        Assert.Contains(Username, result);
        Assert.DoesNotContain(Password, result);
    }

    [Fact]
    public void ToString_UsernameIamAuthConfig_OmitsSensitiveInfo()
    {
        var refreshInterval = IamAuthConfig.MinRefreshIntervalSeconds;
        using var iamAuthConfig = BuildIamAuthConfig(refreshIntervalSeconds: refreshInterval);
        using var credentials = new ServerCredentials(Username, iamAuthConfig);
        string result = credentials.ToString();

        // Verify that string representation contains the username
        // but omits sensitive information (cluster name, region, and refresh interval).
        // See also <see cref="IamAuthConfigTests.ToString_OmitsSensitiveInfo"/>.
        Assert.Contains(Username, result);
        Assert.DoesNotContain(iamAuthConfig.ClusterName, result);
        Assert.DoesNotContain(iamAuthConfig.Region, result);
        Assert.DoesNotContain(iamAuthConfig.RefreshIntervalSeconds!.Value.ToString(), result);
    }

    [Fact]
    public void Dispose_PasswordAuth_AllPublicMembers_ThrowObjectDisposedException()
    {
        var credentials = new ServerCredentials(Username, Password);

        credentials.Dispose();

        _ = Assert.Throws<ObjectDisposedException>(() => credentials.Username);
        _ = Assert.Throws<ObjectDisposedException>(() => credentials.Password);
        _ = Assert.Throws<ObjectDisposedException>(() => credentials.IamConfig);
        _ = Assert.Throws<ObjectDisposedException>(() => credentials.IsIamAuth());
        _ = Assert.Throws<ObjectDisposedException>(() => _ = credentials.ToString());
    }

    [Fact]
    public void Dispose_IamAuth_AllPublicMembers_ThrowObjectDisposedException()
    {
        using var iamAuthConfig = BuildIamAuthConfig();
        var credentials = new ServerCredentials(Username, iamAuthConfig);

        credentials.Dispose();

        _ = Assert.Throws<ObjectDisposedException>(() => credentials.Username);
        _ = Assert.Throws<ObjectDisposedException>(() => credentials.Password);
        _ = Assert.Throws<ObjectDisposedException>(() => credentials.IamConfig);
        _ = Assert.Throws<ObjectDisposedException>(() => credentials.IsIamAuth());
        _ = Assert.Throws<ObjectDisposedException>(() => _ = credentials.ToString());
    }

    #endregion
    #region Helpers

    /// <summary>
    /// Builds and returns a new IAM authentication configuration for testing.
    /// If required parameters are not specified, default values are used.
    /// </summary>
    private static IamAuthConfig BuildIamAuthConfig(
        string clusterName = "CLUSTER_NAME",
        ServiceType serviceType = ServiceType.ElastiCache,
        string region = "REGION",
        uint? refreshIntervalSeconds = null
    )
        => new(clusterName, serviceType, region, refreshIntervalSeconds);

    #endregion
}
