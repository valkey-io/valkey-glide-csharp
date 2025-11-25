// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using System.Text.Json;

namespace Valkey.Glide.UnitTests;

public class PubSubMessageTests
{
    [Fact]
    public void PubSubMessage_ExactChannelConstructor_SetsPropertiesCorrectly()
    {
        // Arrange
        const string message = "test message";
        const string channel = "test-channel";

        // Act
        var pubSubMessage = new PubSubMessage(message, channel);

        // Assert
        Assert.Equal(message, pubSubMessage.Message);
        Assert.Equal(channel, pubSubMessage.Channel);
        Assert.Null(pubSubMessage.Pattern);
    }

    [Fact]
    public void PubSubMessage_PatternConstructor_SetsPropertiesCorrectly()
    {
        // Arrange
        const string message = "test message";
        const string channel = "test-channel";
        const string pattern = "test-*";

        // Act
        var pubSubMessage = new PubSubMessage(message, channel, pattern);

        // Assert
        Assert.Equal(message, pubSubMessage.Message);
        Assert.Equal(channel, pubSubMessage.Channel);
        Assert.Equal(pattern, pubSubMessage.Pattern);
    }

    [Theory]
    [InlineData(null, "channel")]
    [InlineData("", "channel")]
    [InlineData("message", null)]
    [InlineData("message", "")]
    public void PubSubMessage_ExactChannelConstructor_ThrowsOnInvalidInput(string? message, string? channel)
    {
        // Act & Assert
        if (message == null || channel == null)
        {
            Assert.Throws<ArgumentNullException>(() => new PubSubMessage(message!, channel!));
        }
        else
        {
            Assert.Throws<ArgumentException>(() => new PubSubMessage(message, channel));
        }
    }

    [Theory]
    [InlineData(null, "channel", "pattern")]
    [InlineData("", "channel", "pattern")]
    [InlineData("message", null, "pattern")]
    [InlineData("message", "", "pattern")]
    [InlineData("message", "channel", null)]
    [InlineData("message", "channel", "")]
    public void PubSubMessage_PatternConstructor_ThrowsOnInvalidInput(string? message, string? channel, string? pattern)
    {
        // Act & Assert
        if (message == null || channel == null || pattern == null)
        {
            Assert.Throws<ArgumentNullException>(() => new PubSubMessage(message!, channel!, pattern!));
        }
        else
        {
            Assert.Throws<ArgumentException>(() => new PubSubMessage(message, channel, pattern));
        }
    }

    [Fact]
    public void PubSubMessage_ToString_ReturnsValidJson()
    {
        // Arrange
        const string message = "test message";
        const string channel = "test-channel";
        const string pattern = "test-*";
        var pubSubMessage = new PubSubMessage(message, channel, pattern);

        // Act
        var jsonString = pubSubMessage.ToString();

        // Assert
        Assert.NotNull(jsonString);
        Assert.NotEmpty(jsonString);

        // Verify it's valid JSON by deserializing
        JsonElement deserializedObject = JsonSerializer.Deserialize<JsonElement>(jsonString);
        Assert.Equal(message, deserializedObject.GetProperty("Message").GetString());
        Assert.Equal(channel, deserializedObject.GetProperty("Channel").GetString());
        Assert.Equal(pattern, deserializedObject.GetProperty("Pattern").GetString());
    }

    [Fact]
    public void PubSubMessage_ToString_ExactChannel_ReturnsValidJsonWithNullPattern()
    {
        // Arrange
        const string message = "test message";
        const string channel = "test-channel";
        var pubSubMessage = new PubSubMessage(message, channel);

        // Act
        var jsonString = pubSubMessage.ToString();

        // Assert
        Assert.NotNull(jsonString);
        Assert.NotEmpty(jsonString);

        // Verify it's valid JSON by deserializing
        JsonElement deserializedObject = JsonSerializer.Deserialize<JsonElement>(jsonString);
        Assert.Equal(message, deserializedObject.GetProperty("Message").GetString());
        Assert.Equal(channel, deserializedObject.GetProperty("Channel").GetString());
        Assert.Equal(JsonValueKind.Null, deserializedObject.GetProperty("Pattern").ValueKind);
    }

    [Fact]
    public void PubSubMessage_Equals_ReturnsTrueForEqualMessages()
    {
        // Arrange
        const string message = "test message";
        const string channel = "test-channel";
        const string pattern = "test-*";
        var message1 = new PubSubMessage(message, channel, pattern);
        var message2 = new PubSubMessage(message, channel, pattern);

        // Act & Assert
        Assert.Equal(message1, message2);
        Assert.True(message1.Equals(message2));
        Assert.True(message2.Equals(message1));
    }

    [Fact]
    public void PubSubMessage_Equals_ReturnsFalseForDifferentMessages()
    {
        // Arrange
        var message1 = new PubSubMessage("message1", "channel1", "pattern1");
        var message2 = new PubSubMessage("message2", "channel2", "pattern2");

        // Act & Assert
        Assert.NotEqual(message1, message2);
        Assert.False(message1.Equals(message2));
        Assert.False(message2.Equals(message1));
    }

    [Fact]
    public void PubSubMessage_Equals_ReturnsFalseForNull()
    {
        // Arrange
        var message = new PubSubMessage("test", "channel");

        // Act & Assert
        Assert.False(message.Equals(null));
    }

    [Fact]
    public void PubSubMessage_GetHashCode_SameForEqualMessages()
    {
        // Arrange
        const string message = "test message";
        const string channel = "test-channel";
        const string pattern = "test-*";
        var message1 = new PubSubMessage(message, channel, pattern);
        var message2 = new PubSubMessage(message, channel, pattern);

        // Act & Assert
        Assert.Equal(message1.GetHashCode(), message2.GetHashCode());
    }

    [Fact]
    public void PubSubMessage_GetHashCode_DifferentForDifferentMessages()
    {
        // Arrange
        var message1 = new PubSubMessage("message1", "channel1", "pattern1");
        var message2 = new PubSubMessage("message2", "channel2", "pattern2");

        // Act & Assert
        Assert.NotEqual(message1.GetHashCode(), message2.GetHashCode());
    }
}
