// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.UnitTests;

public class LoadedLuaScriptTests
{
    [Fact]
    public void Constructor_WithValidParameters_CreatesInstance()
    {
        // Arrange
        string scriptText = "return redis.call('GET', @key)";
        LuaScript script = LuaScript.Prepare(scriptText);
        byte[] hash = [0x12, 0x34, 0x56, 0x78];

        // Act
        LoadedLuaScript loaded = new(script, hash);

        // Assert
        Assert.NotNull(loaded);
        Assert.Equal(scriptText, loaded.OriginalScript);
        Assert.NotNull(loaded.ExecutableScript);
        Assert.Equal(hash, loaded.Hash);
    }

    [Fact]
    public void Constructor_WithNullScript_ThrowsArgumentNullException()
    {
        // Arrange
        byte[] hash = [0x12, 0x34, 0x56, 0x78];

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new LoadedLuaScript(null!, hash));
    }

    [Fact]
    public void Constructor_WithNullHash_ThrowsArgumentNullException()
    {
        // Arrange
        string scriptText = "return redis.call('GET', @key)";
        LuaScript script = LuaScript.Prepare(scriptText);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new LoadedLuaScript(script, null!));
    }

    [Fact]
    public void OriginalScript_ReturnsScriptOriginalScript()
    {
        // Arrange
        string scriptText = "return redis.call('GET', @key)";
        LuaScript script = LuaScript.Prepare(scriptText);
        byte[] hash = [0x12, 0x34, 0x56, 0x78];
        LoadedLuaScript loaded = new(script, hash);

        // Act
        string originalScript = loaded.OriginalScript;

        // Assert
        Assert.Equal(scriptText, originalScript);
    }

    [Fact]
    public void ExecutableScript_ReturnsScriptExecutableScript()
    {
        // Arrange
        string scriptText = "return redis.call('GET', @key)";
        LuaScript script = LuaScript.Prepare(scriptText);
        byte[] hash = [0x12, 0x34, 0x56, 0x78];
        LoadedLuaScript loaded = new(script, hash);

        // Act
        string executableScript = loaded.ExecutableScript;

        // Assert
        Assert.NotNull(executableScript);
        Assert.NotEqual(scriptText, executableScript); // Should be transformed
    }

    [Fact]
    public void Hash_ReturnsProvidedHash()
    {
        // Arrange
        string scriptText = "return redis.call('GET', @key)";
        LuaScript script = LuaScript.Prepare(scriptText);
        byte[] hash = [0x12, 0x34, 0x56, 0x78, 0x9A, 0xBC, 0xDE, 0xF0];
        LoadedLuaScript loaded = new(script, hash);

        // Act
        byte[] returnedHash = loaded.Hash;

        // Assert
        Assert.Equal(hash, returnedHash);
    }

    [Fact]
    public void Evaluate_WithNullDatabase_ThrowsArgumentNullException()
    {
        // Arrange
        string scriptText = "return redis.call('GET', @key)";
        LuaScript script = LuaScript.Prepare(scriptText);
        byte[] hash = [0x12, 0x34, 0x56, 0x78];
        LoadedLuaScript loaded = new(script, hash);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => loaded.Evaluate(null!));
    }

    [Fact]
    public async Task EvaluateAsync_WithNullDatabase_ThrowsArgumentNullException()
    {
        // Arrange
        string scriptText = "return redis.call('GET', @key)";
        LuaScript script = LuaScript.Prepare(scriptText);
        byte[] hash = [0x12, 0x34, 0x56, 0x78];
        LoadedLuaScript loaded = new(script, hash);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => loaded.EvaluateAsync(null!));
    }

    [Fact]
    public void Hash_IsNotSameReferenceAsInput()
    {
        // Arrange
        string scriptText = "return redis.call('GET', @key)";
        LuaScript script = LuaScript.Prepare(scriptText);
        byte[] hash = [0x12, 0x34, 0x56, 0x78];
        LoadedLuaScript loaded = new(script, hash);

        // Act
        byte[] returnedHash = loaded.Hash;

        // Assert
        // The hash should be the same reference (not a copy) for efficiency
        Assert.Same(hash, returnedHash);
    }

    [Fact]
    public void Constructor_WithDifferentScripts_CreatesDifferentInstances()
    {
        // Arrange
        string scriptText1 = "return redis.call('GET', @key)";
        string scriptText2 = "return redis.call('SET', @key, @value)";
        LuaScript script1 = LuaScript.Prepare(scriptText1);
        LuaScript script2 = LuaScript.Prepare(scriptText2);
        byte[] hash1 = [0x12, 0x34, 0x56, 0x78];
        byte[] hash2 = [0x9A, 0xBC, 0xDE, 0xF0];

        // Act
        LoadedLuaScript loaded1 = new(script1, hash1);
        LoadedLuaScript loaded2 = new(script2, hash2);

        // Assert
        Assert.NotEqual(loaded1.OriginalScript, loaded2.OriginalScript);
        Assert.NotEqual(loaded1.Hash, loaded2.Hash);
    }

    [Fact]
    public void OriginalScript_WithComplexScript_ReturnsOriginal()
    {
        // Arrange
        string scriptText = @"
            local key1 = @key1
            local key2 = @key2
            local value = @value
            redis.call('SET', key1, value)
            return redis.call('GET', key2)
        ";
        LuaScript script = LuaScript.Prepare(scriptText);
        byte[] hash = [0x12, 0x34, 0x56, 0x78];
        LoadedLuaScript loaded = new(script, hash);

        // Act
        string originalScript = loaded.OriginalScript;

        // Assert
        Assert.Equal(scriptText, originalScript);
    }

    [Fact]
    public void ExecutableScript_WithNoParameters_ReturnsSameAsOriginal()
    {
        // Arrange
        string scriptText = "return 'hello world'";
        LuaScript script = LuaScript.Prepare(scriptText);
        byte[] hash = [0x12, 0x34, 0x56, 0x78];
        LoadedLuaScript loaded = new(script, hash);

        // Act
        string executableScript = loaded.ExecutableScript;

        // Assert
        Assert.Equal(scriptText, executableScript);
    }

    [Fact]
    public void Hash_WithEmptyHash_StoresCorrectly()
    {
        // Arrange
        string scriptText = "return redis.call('GET', @key)";
        LuaScript script = LuaScript.Prepare(scriptText);
        byte[] hash = [];
        LoadedLuaScript loaded = new(script, hash);

        // Act
        byte[] returnedHash = loaded.Hash;

        // Assert
        Assert.Empty(returnedHash);
    }

    [Fact]
    public void Hash_WithLongHash_StoresCorrectly()
    {
        // Arrange
        string scriptText = "return redis.call('GET', @key)";
        LuaScript script = LuaScript.Prepare(scriptText);
        byte[] hash = [0x01, 0x23, 0x45, 0x67, 0x89, 0xAB, 0xCD, 0xEF,
                       0xFE, 0xDC, 0xBA, 0x98, 0x76, 0x54, 0x32, 0x10,
                       0x11, 0x22, 0x33, 0x44];
        LoadedLuaScript loaded = new(script, hash);

        // Act
        byte[] returnedHash = loaded.Hash;

        // Assert
        Assert.Equal(20, returnedHash.Length);
        Assert.Equal(hash, returnedHash);
    }

    [Fact]
    public void Properties_AreConsistentAcrossMultipleCalls()
    {
        // Arrange
        string scriptText = "return redis.call('GET', @key)";
        LuaScript script = LuaScript.Prepare(scriptText);
        byte[] hash = [0x12, 0x34, 0x56, 0x78];
        LoadedLuaScript loaded = new(script, hash);

        // Act
        string originalScript1 = loaded.OriginalScript;
        string originalScript2 = loaded.OriginalScript;
        string executableScript1 = loaded.ExecutableScript;
        string executableScript2 = loaded.ExecutableScript;
        byte[] hash1 = loaded.Hash;
        byte[] hash2 = loaded.Hash;

        // Assert
        Assert.Same(originalScript1, originalScript2);
        Assert.Same(executableScript1, executableScript2);
        Assert.Same(hash1, hash2);
    }
}
