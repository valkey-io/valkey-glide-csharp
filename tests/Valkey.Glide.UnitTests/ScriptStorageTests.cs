// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.UnitTests;

public class ScriptStorageTests
{
    [Fact]
    public void StoreScript_WithValidScript_ReturnsHash()
    {
        // Arrange
        string script = "return 'Hello, World!'";

        // Act
        string hash = FFI.StoreScript(script);

        // Assert
        Assert.NotNull(hash);
        Assert.NotEmpty(hash);
        // SHA1 hashes are 40 characters long (hex representation)
        Assert.Equal(40, hash.Length);

        // Clean up
        FFI.DropScript(hash);
    }

    [Fact]
    public void StoreScript_WithNullScript_ThrowsArgumentException()
        => Assert.Equal("script", Assert.Throws<ArgumentException>(() => _ = FFI.StoreScript(null!)).ParamName);

    [Fact]
    public void StoreScript_WithEmptyScript_ThrowsArgumentException()
        => Assert.Equal("script", Assert.Throws<ArgumentException>(() => _ = FFI.StoreScript(string.Empty)).ParamName);

    [Fact]
    public void DropScript_WithNullHash_ThrowsArgumentException()
        => Assert.Equal("hash", Assert.Throws<ArgumentException>(() => FFI.DropScript(null!)).ParamName);

    [Fact]
    public void DropScript_WithEmptyHash_ThrowsArgumentException()
        => Assert.Equal("hash", Assert.Throws<ArgumentException>(() => FFI.DropScript(string.Empty)).ParamName);
}
