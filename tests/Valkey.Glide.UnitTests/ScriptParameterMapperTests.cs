// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.UnitTests;

public class ScriptParameterMapperTests
{
    [Fact]
    public void PrepareScript_WithValidScript_ExtractsParameters()
    {
        // Arrange
        string script = "return redis.call('GET', @key) + @value";

        // Act
        var (originalScript, executableScript, parameters) = ScriptParameterMapper.PrepareScript(script);

        // Assert
        Assert.Equal(script, originalScript);
        Assert.Equal("return redis.call('GET', {PARAM_0}) + {PARAM_1}", executableScript);
        Assert.Equal(2, parameters.Length);
        Assert.Equal("key", parameters[0]);
        Assert.Equal("value", parameters[1]);
    }

    [Fact]
    public void PrepareScript_WithDuplicateParameters_ExtractsUniqueParameters()
    {
        // Arrange
        string script = "return @key + @value + @key";

        // Act
        var (_, executableScript, parameters) = ScriptParameterMapper.PrepareScript(script);

        // Assert
        Assert.Equal("return {PARAM_0} + {PARAM_1} + {PARAM_0}", executableScript);
        Assert.Equal(2, parameters.Length);
        Assert.Equal("key", parameters[0]);
        Assert.Equal("value", parameters[1]);
    }

    [Fact]
    public void PrepareScript_WithNoParameters_ReturnsEmptyArray()
    {
        // Arrange
        string script = "return redis.call('GET', 'mykey')";

        // Act
        var (_, executableScript, parameters) = ScriptParameterMapper.PrepareScript(script);

        // Assert
        Assert.Equal(script, executableScript);
        Assert.Empty(parameters);
    }

    [Fact]
    public void PrepareScript_WithNullScript_ThrowsArgumentException()
    {
        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() => ScriptParameterMapper.PrepareScript(null!));
        Assert.Contains("Script cannot be null or empty", ex.Message);
    }

    [Fact]
    public void PrepareScript_WithEmptyScript_ThrowsArgumentException()
    {
        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() => ScriptParameterMapper.PrepareScript(""));
        Assert.Contains("Script cannot be null or empty", ex.Message);
    }

    [Fact]
    public void PrepareScript_WithComplexParameterNames_ExtractsCorrectly()
    {
        // Arrange
        string script = "return @user_id + @item_count + @is_active";

        // Act
        var (_, _, parameters) = ScriptParameterMapper.PrepareScript(script);

        // Assert
        Assert.Equal(3, parameters.Length);
        Assert.Equal("user_id", parameters[0]);
        Assert.Equal("item_count", parameters[1]);
        Assert.Equal("is_active", parameters[2]);
    }

    [Fact]
    public void IsValidParameterHash_WithAllValidProperties_ReturnsTrue()
    {
        // Arrange
        var type = typeof(ValidParameterObject);
        string[] parameterNames = ["Key", "Value"];

        // Act
        bool isValid = ScriptParameterMapper.IsValidParameterHash(type, parameterNames,
            out string? missingMember, out string? badTypeMember);

        // Assert
        Assert.True(isValid);
        Assert.Null(missingMember);
        Assert.Null(badTypeMember);
    }

    [Fact]
    public void IsValidParameterHash_WithMissingProperty_ReturnsFalse()
    {
        // Arrange
        var type = typeof(ValidParameterObject);
        string[] parameterNames = ["Key", "NonExistent"];

        // Act
        bool isValid = ScriptParameterMapper.IsValidParameterHash(type, parameterNames,
            out string? missingMember, out string? badTypeMember);

        // Assert
        Assert.False(isValid);
        Assert.Equal("NonExistent", missingMember);
        Assert.Null(badTypeMember);
    }

    [Fact]
    public void IsValidParameterHash_WithInvalidType_ReturnsFalse()
    {
        // Arrange
        var type = typeof(InvalidParameterObject);
        string[] parameterNames = ["InvalidProperty"];

        // Act
        bool isValid = ScriptParameterMapper.IsValidParameterHash(type, parameterNames,
            out string? missingMember, out string? badTypeMember);

        // Assert
        Assert.False(isValid);
        Assert.Null(missingMember);
        Assert.Equal("InvalidProperty", badTypeMember);
    }

    [Fact]
    public void IsValidParameterHash_CaseInsensitive_ReturnsTrue()
    {
        // Arrange
        var type = typeof(ValidParameterObject);
        string[] parameterNames = ["key", "VALUE"]; // Different case

        // Act
        bool isValid = ScriptParameterMapper.IsValidParameterHash(type, parameterNames,
            out string? missingMember, out string? badTypeMember);

        // Assert
        Assert.True(isValid);
        Assert.Null(missingMember);
        Assert.Null(badTypeMember);
    }

    [Fact]
    public void GetParameterExtractor_WithValidObject_ExtractsParameters()
    {
        // Arrange
        var type = typeof(ValidParameterObject);
        string[] parameterNames = ["Key", "Value"];
        var extractor = ScriptParameterMapper.GetParameterExtractor(type, parameterNames);

        var paramObj = new ValidParameterObject
        {
            Key = "mykey",
            Value = 42
        };

        // Act
        var (keys, args) = extractor(paramObj, null);

        // Assert
        Assert.Single(keys);
        Assert.Equal("mykey", (string?)keys[0]);
        Assert.Single(args);
        Assert.Equal(42L, (long)args[0]);
    }

    [Fact]
    public void GetParameterExtractor_WithKeyPrefix_AppliesPrefix()
    {
        // Arrange
        var type = typeof(ValidParameterObject);
        string[] parameterNames = ["Key"];
        var extractor = ScriptParameterMapper.GetParameterExtractor(type, parameterNames);

        var paramObj = new ValidParameterObject
        {
            Key = "mykey"
        };

        ValkeyKey prefix = "prefix:";

        // Act
        var (keys, _) = extractor(paramObj, prefix);

        // Assert
        Assert.Single(keys);
        Assert.Equal("prefix:mykey", (string?)keys[0]);
    }

    [Fact]
    public void GetParameterExtractor_WithMultipleParameters_ExtractsAll()
    {
        // Arrange
        var type = typeof(MultiParameterObject);
        string[] parameterNames = ["Key1", "Key2", "Value1", "Value2"];
        var extractor = ScriptParameterMapper.GetParameterExtractor(type, parameterNames);

        var paramObj = new MultiParameterObject
        {
            Key1 = "key1",
            Key2 = "key2",
            Value1 = "value1",
            Value2 = 100
        };

        // Act
        var (keys, args) = extractor(paramObj, null);

        // Assert
        Assert.Equal(2, keys.Length);
        Assert.Equal("key1", (string?)keys[0]);
        Assert.Equal("key2", (string?)keys[1]);
        Assert.Equal(2, args.Length);
        Assert.Equal("value1", (string?)args[0]);
        Assert.Equal(100L, (long)args[1]);
    }

    [Fact]
    public void IsValidParameterType_WithValidTypes_ReturnsTrue()
    {
        // Assert
        Assert.True(ScriptParameterMapper.IsValidParameterType(typeof(string)));
        Assert.True(ScriptParameterMapper.IsValidParameterType(typeof(int)));
        Assert.True(ScriptParameterMapper.IsValidParameterType(typeof(long)));
        Assert.True(ScriptParameterMapper.IsValidParameterType(typeof(double)));
        Assert.True(ScriptParameterMapper.IsValidParameterType(typeof(bool)));
        Assert.True(ScriptParameterMapper.IsValidParameterType(typeof(byte[])));
        Assert.True(ScriptParameterMapper.IsValidParameterType(typeof(ValkeyKey)));
        Assert.True(ScriptParameterMapper.IsValidParameterType(typeof(ValkeyValue)));
        Assert.True(ScriptParameterMapper.IsValidParameterType(typeof(GlideString)));
    }

    [Fact]
    public void IsValidParameterType_WithNullableTypes_ReturnsTrue()
    {
        // Assert
        Assert.True(ScriptParameterMapper.IsValidParameterType(typeof(int?)));
        Assert.True(ScriptParameterMapper.IsValidParameterType(typeof(long?)));
        Assert.True(ScriptParameterMapper.IsValidParameterType(typeof(double?)));
        Assert.True(ScriptParameterMapper.IsValidParameterType(typeof(bool?)));
        Assert.True(ScriptParameterMapper.IsValidParameterType(typeof(ValkeyKey?)));
    }

    [Fact]
    public void IsValidParameterType_WithInvalidTypes_ReturnsFalse()
    {
        // Assert
        Assert.False(ScriptParameterMapper.IsValidParameterType(typeof(object)));
        Assert.False(ScriptParameterMapper.IsValidParameterType(typeof(DateTime)));
        Assert.False(ScriptParameterMapper.IsValidParameterType(typeof(List<string>)));
    }

    // Test helper classes
    private class ValidParameterObject
    {
        public ValkeyKey Key { get; set; }
        public int Value { get; set; }
    }

    private class InvalidParameterObject
    {
        public DateTime InvalidProperty { get; set; }
    }

    private class MultiParameterObject
    {
        public ValkeyKey Key1 { get; set; }
        public ValkeyKey Key2 { get; set; }
        public string Value1 { get; set; } = string.Empty;
        public int Value2 { get; set; }
    }
}
