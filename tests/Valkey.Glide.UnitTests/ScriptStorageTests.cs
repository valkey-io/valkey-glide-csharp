// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Internals;

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
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
        {
            FFI.StoreScript(null!);
        });

        Assert.Equal("script", exception.ParamName);
    }

    [Fact]
    public void StoreScript_WithEmptyScript_ThrowsArgumentException()
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
        {
            FFI.StoreScript(string.Empty);
        });

        Assert.Equal("script", exception.ParamName);
    }

    [Fact]
    public void DropScript_WithNullHash_ThrowsArgumentException()
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
        {
            FFI.DropScript(null!);
        });

        Assert.Equal("hash", exception.ParamName);
    }

    [Fact]
    public void DropScript_WithEmptyHash_ThrowsArgumentException()
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
        {
            FFI.DropScript(string.Empty);
        });

        Assert.Equal("hash", exception.ParamName);
    }
}
