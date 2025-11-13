// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.UnitTests;

public class ScriptTests
{
    [Fact]
    public void Script_WithValidCode_CreatesSuccessfully()
    {
        // Arrange
        string code = "return 'Hello, World!'";

        // Act
        using var script = new Script(code);

        // Assert
        Assert.NotNull(script);
        Assert.NotNull(script.Hash);
        Assert.NotEmpty(script.Hash);
        // SHA1 hashes are 40 characters long (hex representation)
        Assert.Equal(40, script.Hash.Length);
    }

    [Fact]
    public void Script_WithNullCode_ThrowsArgumentNullException()
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() =>
        {
            var script = new Script(null!);
        });

        Assert.Equal("code", exception.ParamName);
        Assert.Contains("Script code cannot be null", exception.Message);
    }

    [Fact]
    public void Script_WithEmptyCode_ThrowsArgumentException()
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
        {
            var script = new Script(string.Empty);
        });

        Assert.Equal("code", exception.ParamName);
        Assert.Contains("Script code cannot be empty or whitespace", exception.Message);
    }

    [Fact]
    public void Script_WithWhitespaceCode_ThrowsArgumentException()
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
        {
            var script = new Script("   \t\n  ");
        });

        Assert.Equal("code", exception.ParamName);
        Assert.Contains("Script code cannot be empty or whitespace", exception.Message);
    }

    [Fact]
    public void Script_Hash_CalculatedCorrectly()
    {
        // Arrange
        string code = "return 42";

        // Act
        using var script = new Script(code);

        // Assert
        // The SHA1 hash of "return 42" should be consistent
        Assert.NotNull(script.Hash);
        Assert.Equal(40, script.Hash.Length);
        // Verify it's a valid hex string
        Assert.Matches("^[0-9a-f]{40}$", script.Hash);
    }

    [Fact]
    public void Script_Dispose_ReleasesResources()
    {
        // Arrange
        string code = "return 'test'";
        var script = new Script(code);
        string hash = script.Hash;

        // Act
        script.Dispose();

        // Assert
        // After disposal, accessing Hash should throw ObjectDisposedException
        Assert.Throws<ObjectDisposedException>(() => script.Hash);
    }

    [Fact]
    public void Script_MultipleDispose_IsSafe()
    {
        // Arrange
        string code = "return 'test'";
        var script = new Script(code);

        // Act & Assert
        // Multiple calls to Dispose should not throw
        script.Dispose();
        script.Dispose();
        script.Dispose();
    }

    [Fact]
    public void Script_AccessAfterDispose_ThrowsObjectDisposedException()
    {
        // Arrange
        string code = "return 'test'";
        var script = new Script(code);
        script.Dispose();

        // Act & Assert
        var exception = Assert.Throws<ObjectDisposedException>(() => script.Hash);
        Assert.Equal("Script", exception.ObjectName);
        Assert.Contains("Cannot access a disposed Script", exception.Message);
    }

    [Fact]
    public void Script_UsingStatement_DisposesAutomatically()
    {
        // Arrange
        string code = "return 'test'";
        Script? script = null;

        // Act
        using (script = new Script(code))
        {
            // Verify it works inside the using block
            Assert.NotNull(script.Hash);
        }

        // Assert
        // After the using block, accessing Hash should throw
        Assert.Throws<ObjectDisposedException>(() => script.Hash);
    }

    [Fact]
    public void Script_ConcurrentAccess_IsThreadSafe()
    {
        // Arrange
        string code = "return 'concurrent test'";
        using Script script = new(code);
        System.Collections.Concurrent.ConcurrentBag<Exception> exceptions = [];
        List<Task> tasks = [];

        // Act
        // Create multiple tasks that access the script concurrently
        for (int i = 0; i < 10; i++)
        {
            tasks.Add(Task.Run(() =>
            {
                try
                {
                    for (int j = 0; j < 100; j++)
                    {
                        var hash = script.Hash;
                        Assert.NotNull(hash);
                    }
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                }
            }));
        }

        Task.WaitAll([.. tasks]);

        // Assert
        Assert.Empty(exceptions);
    }

    [Fact]
    public void Script_ConcurrentDispose_IsThreadSafe()
    {
        // Arrange
        string code = "return 'concurrent dispose test'";
        Script script = new(code);
        System.Collections.Concurrent.ConcurrentBag<Exception> exceptions = [];
        List<Task> tasks = [];

        // Act
        // Create multiple tasks that try to dispose the script concurrently
        for (int i = 0; i < 10; i++)
        {
            tasks.Add(Task.Run(() =>
            {
                try
                {
                    script.Dispose();
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                }
            }));
        }

        Task.WaitAll([.. tasks]);

        // Assert
        // No exceptions should be thrown during concurrent disposal
        Assert.Empty(exceptions);
    }

    [Fact]
    public void Script_DifferentScripts_HaveDifferentHashes()
    {
        // Arrange
        string code1 = "return 1";
        string code2 = "return 2";

        // Act
        using var script1 = new Script(code1);
        using var script2 = new Script(code2);

        // Assert
        Assert.NotEqual(script1.Hash, script2.Hash);
    }

    [Fact]
    public void Script_SameCode_HasSameHash()
    {
        // Arrange
        string code = "return 'same code'";

        // Act
        using var script1 = new Script(code);
        using var script2 = new Script(code);

        // Assert
        // Same code should produce the same hash
        Assert.Equal(script1.Hash, script2.Hash);
    }

    [Fact]
    public void Script_ComplexLuaCode_CreatesSuccessfully()
    {
        // Arrange
        string code = @"
            local key = KEYS[1]
            local value = ARGV[1]
            redis.call('SET', key, value)
            return redis.call('GET', key)
        ";

        // Act
        using var script = new Script(code);

        // Assert
        Assert.NotNull(script.Hash);
        Assert.Equal(40, script.Hash.Length);
    }

    [Fact]
    public void Script_WithUnicodeCharacters_CreatesSuccessfully()
    {
        // Arrange
        string code = "return '你好世界 🌍'";

        // Act
        using var script = new Script(code);

        // Assert
        Assert.NotNull(script.Hash);
        Assert.Equal(40, script.Hash.Length);
    }
}
