// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.UnitTests;

public class ScriptOptionsTests
{
    [Fact]
    public void Constructor_CreatesInstanceWithNullProperties()
    {
        // Act
        var options = new ScriptOptions();

        // Assert
        Assert.Null(options.Keys);
        Assert.Null(options.Args);
    }

    [Fact]
    public void WithKeys_SetsKeysProperty()
    {
        // Arrange
        var options = new ScriptOptions();
        string[] keys = ["key1", "key2", "key3"];

        // Act
        var result = options.WithKeys(keys);

        // Assert
        Assert.Same(options, result); // Fluent interface returns same instance
        Assert.Equal(keys, options.Keys);
    }

    [Fact]
    public void WithKeys_WithParamsArray_SetsKeysProperty()
    {
        // Arrange
        var options = new ScriptOptions();

        // Act
        var result = options.WithKeys("key1", "key2", "key3");

        // Assert
        Assert.Same(options, result);
        Assert.NotNull(options.Keys);
        Assert.Equal(["key1", "key2", "key3"], options.Keys);
    }

    [Fact]
    public void WithArgs_SetsArgsProperty()
    {
        // Arrange
        var options = new ScriptOptions();
        string[] args = ["arg1", "arg2", "arg3"];

        // Act
        var result = options.WithArgs(args);

        // Assert
        Assert.Same(options, result); // Fluent interface returns same instance
        Assert.Equal(args, options.Args);
    }

    [Fact]
    public void WithArgs_WithParamsArray_SetsArgsProperty()
    {
        // Arrange
        var options = new ScriptOptions();

        // Act
        var result = options.WithArgs("arg1", "arg2", "arg3");

        // Assert
        Assert.Same(options, result);
        Assert.NotNull(options.Args);
        Assert.Equal(["arg1", "arg2", "arg3"], options.Args);
    }

    [Fact]
    public void FluentBuilder_ChainsMultipleCalls()
    {
        // Arrange & Act
        var options = new ScriptOptions()
            .WithKeys("key1", "key2")
            .WithArgs("arg1", "arg2", "arg3");

        // Assert
        Assert.NotNull(options.Keys);
        Assert.NotNull(options.Args);
        Assert.Equal(["key1", "key2"], options.Keys);
        Assert.Equal(["arg1", "arg2", "arg3"], options.Args);
    }

    [Fact]
    public void WithKeys_WithEmptyArray_SetsEmptyArray()
    {
        // Arrange
        var options = new ScriptOptions();

        // Act
        options.WithKeys([]);

        // Assert
        Assert.NotNull(options.Keys);
        Assert.Empty(options.Keys);
    }

    [Fact]
    public void WithArgs_WithEmptyArray_SetsEmptyArray()
    {
        // Arrange
        var options = new ScriptOptions();

        // Act
        options.WithArgs([]);

        // Assert
        Assert.NotNull(options.Args);
        Assert.Empty(options.Args);
    }

    [Fact]
    public void WithKeys_OverwritesPreviousValue()
    {
        // Arrange
        var options = new ScriptOptions()
            .WithKeys("key1", "key2");

        // Act
        options.WithKeys("key3", "key4");

        // Assert
        Assert.NotNull(options.Keys);
        Assert.Equal(["key3", "key4"], options.Keys);
    }

    [Fact]
    public void WithArgs_OverwritesPreviousValue()
    {
        // Arrange
        var options = new ScriptOptions()
            .WithArgs("arg1", "arg2");

        // Act
        options.WithArgs("arg3", "arg4");

        // Assert
        Assert.NotNull(options.Args);
        Assert.Equal(["arg3", "arg4"], options.Args);
    }

    [Fact]
    public void PropertySetters_WorkDirectly()
    {
        // Arrange
        var options = new ScriptOptions();
        string[] keys = ["key1"];
        string[] args = ["arg1"];

        // Act
        options.Keys = keys;
        options.Args = args;

        // Assert
        Assert.Equal(keys, options.Keys);
        Assert.Equal(args, options.Args);
    }

    [Fact]
    public void PropertySetters_CanSetToNull()
    {
        // Arrange
        var options = new ScriptOptions()
            .WithKeys("key1")
            .WithArgs("arg1");

        // Act
        options.Keys = null;
        options.Args = null;

        // Assert
        Assert.Null(options.Keys);
        Assert.Null(options.Args);
    }
}
