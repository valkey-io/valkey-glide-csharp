// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using static Valkey.Glide.TestUtils.Builders;

namespace Valkey.Glide.UnitTests;

public class ServerCredentialsTests
{
    #region Constants

    private static readonly string Username = "USERNAME";
    private static readonly string Password = "PASSWORD";

    #endregion
    #region Tests

    [Fact]
    public void Constructor_UsernamePassword()
    {
        using var credentials = BuildServerCredentials(
            username: Username,
            password: Password);

        Assert.Equal(Username, credentials.Username);
        Assert.Equal(Password.ToCharArray(), credentials.Password);
        Assert.Null(credentials.IamAuthConfig);
        Assert.False(credentials.IsIamAuth());
    }

    [Fact]
    public void Constructor_PasswordOnly()
    {
        using var credentials = BuildServerCredentials(password: Password);

        Assert.Null(credentials.Username);
        Assert.Equal(Password.ToCharArray(), credentials.Password);
        Assert.Null(credentials.IamAuthConfig);
        Assert.False(credentials.IsIamAuth());
    }

    [Fact]
    public void Constructor_IamAuth()
    {
        using var iam = BuildIamAuthConfig();
        using var credentials = new ServerCredentials(Username, iam);

        Assert.Equal(Username, credentials.Username);
        Assert.Null(credentials.Password);
        Assert.True(credentials.IsIamAuth());
        Assert.Equal(iam, credentials.IamAuthConfig);
    }

    [Fact]
    public void Constructor_ThrowsOnNull()
    {
        // Password-based
        _ = Assert.Throws<ArgumentNullException>(() => new ServerCredentials(null!));
        _ = Assert.Throws<ArgumentNullException>(() => new ServerCredentials(Username, (string)null!));

        // IAM
        _ = Assert.Throws<ArgumentNullException>(() => new ServerCredentials(null!, BuildIamAuthConfig()));
        _ = Assert.Throws<ArgumentNullException>(() => new ServerCredentials(Username, (IamAuthConfig)null!));
    }

    [Fact]
    public void ToString_OmitsSensitiveData()
    {
        using var credentials = BuildServerCredentials(password: Password);

        var result = credentials.ToString();
        Assert.DoesNotContain(Password, result);
    }

    [Fact]
    public void Dispose_ClearsPasswordArray()
    {
        var credentials = BuildServerCredentials();
        char[] passwordRef = credentials.Password!;

        credentials.Dispose();

        Assert.All(passwordRef, c => Assert.Equal('\0', c));
    }

    [Fact]
    public void Dispose_ThrowsOnSensitiveAccess()
    {
        var credentials = BuildServerCredentials();
        credentials.Dispose();

        _ = Assert.Throws<ObjectDisposedException>(() => credentials.Password);
        _ = Assert.Throws<ObjectDisposedException>(() => credentials.IamAuthConfig);
        _ = Assert.Throws<ObjectDisposedException>(() => credentials.IsIamAuth());
        _ = Assert.Throws<ObjectDisposedException>(credentials.ToString);
    }

    [Fact]
    public void Dispose_IsIdempotent()
    {
        var credentials = BuildServerCredentials();

        credentials.Dispose();
        credentials.Dispose(); // Should not throw
    }

    #endregion
}
