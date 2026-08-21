// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using static Valkey.Glide.TestUtils.Builders;

namespace Valkey.Glide.UnitTests;

public class MonitorConfigTests
{
    #region Constants

    private static readonly string Host = "HOST";
    private const ushort Port = 1234;
    private static readonly string Username = "USERNAME";
    private static readonly string Password = "PASSWORD";
    private static readonly ushort Database = 5;

    #endregion
    #region Tests

    [Fact]
    public void Constructor()
    {
        using var config = BuildMonitorConfig(Host, Port);

        Assert.Equal(Host, config.Host);
        Assert.Equal(Port, config.Port);
        Assert.False(config.UseTls);
        Assert.Null(config.Username);
        Assert.Null(config.Password);
        Assert.Equal(0, config.Database);

        _ = Assert.Throws<ArgumentNullException>(() => new MonitorConfig(null!, Port));
    }

    [Fact]
    public void WithAuth()
    {
        // Password only
        using var passwordOnly = BuildMonitorConfig().WithAuth(Password);
        Assert.Null(passwordOnly.Username);
        Assert.Equal(Password.ToCharArray(), passwordOnly.Password);

        // Username and password
        using var withUsername = BuildMonitorConfig().WithAuth(Username, Password);
        Assert.Equal(Username, withUsername.Username);
        Assert.Equal(Password.ToCharArray(), withUsername.Password);

        // Null arguments
        _ = Assert.Throws<ArgumentNullException>(() => BuildMonitorConfig().WithAuth(null!));
        _ = Assert.Throws<ArgumentNullException>(() => BuildMonitorConfig().WithAuth(null!, Password));
        _ = Assert.Throws<ArgumentNullException>(() => BuildMonitorConfig().WithAuth(Username, null!));
    }

    [Fact]
    public void WithDatabase()
    {
        using var config = BuildMonitorConfig().WithDatabase(Database);
        Assert.Equal(Database, config.Database);
    }

    [Fact]
    public void ToString_OmitsSensitiveData()
    {
        using var config = BuildMonitorConfig().WithAuth(Username, Password);

        var result = config.ToString();
        Assert.DoesNotContain(Username, result);
        Assert.DoesNotContain(Password, result);
    }

    [Fact]
    public void Dispose_ThrowsOnSensitiveAccess()
    {
        var config = BuildMonitorConfig().WithAuth(Username, Password);
        config.Dispose();

        _ = Assert.Throws<ObjectDisposedException>(() => config.Password);
        _ = Assert.Throws<ObjectDisposedException>(() => config.Username);

        _ = Assert.Throws<ObjectDisposedException>(() => config.WithAuth(Password));
        _ = Assert.Throws<ObjectDisposedException>(() => config.WithAuth(Username, Password));
        _ = Assert.Throws<ObjectDisposedException>(() => config.WithTls());
    }

    [Fact]
    public void Dispose_IsIdempotent()
    {
        var config = BuildMonitorConfig().WithAuth(Password);

        config.Dispose();
        config.Dispose(); // Should not throw
    }

    [Fact]
    public void Dispose_ClearsPasswordArray()
    {
        var config = BuildMonitorConfig().WithAuth(Username, Password);
        char[] passwordRef = config.Password!;

        config.Dispose();

        Assert.All(passwordRef, c => Assert.Equal('\0', c));
    }

    [Fact]
    public void LibraryName_NotSet_DefaultsToGlideCSharp()
    {
        using var config = BuildMonitorConfig();
        Assert.Equal("GlideC#", config.LibraryName);
        Assert.Equal("GlideC#", config.ResolvedLibName);
    }

    [Fact]
    public void WithLibraryName_OverridesDefault()
    {
        using var config = BuildMonitorConfig().WithLibraryName("custom-mon");
        Assert.Equal("custom-mon", config.LibraryName);
        Assert.Equal("custom-mon", config.ResolvedLibName);
    }

    [Fact]
    public void WithClientInfoTag_AppendsToDefaultLibName()
    {
        using var config = BuildMonitorConfig().WithClientInfoTag("svc:1.0");
        Assert.Equal("GlideC#(svc:1.0)", config.ResolvedLibName);
    }

    [Fact]
    public void WithLibraryNameAndClientInfoTag_Composes()
    {
        using var config = BuildMonitorConfig()
            .WithLibraryName("custom-mon")
            .WithClientInfoTag("svc:1.0");
        Assert.Equal("custom-mon(svc:1.0)", config.ResolvedLibName);
    }

    [Theory]
    [InlineData(" ")]
    [InlineData("   ")]
    [InlineData("\t")]
    [InlineData("\n")]
    public void WithClientInfoTag_WhitespaceOnly_DoesNotAppendParens(string tag)
    {
        using var config = BuildMonitorConfig().WithClientInfoTag(tag);
        Assert.Equal("GlideC#", config.ResolvedLibName);
    }

    #endregion
}
