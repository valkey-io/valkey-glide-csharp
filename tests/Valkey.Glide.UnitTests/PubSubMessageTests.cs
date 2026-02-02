// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using System.Text.Json;

namespace Valkey.Glide.UnitTests;

public class PubSubMessageTests
{
    private static readonly string message = "test message";
    private static readonly string channel = "test-channel";
    private static readonly string pattern = "test-*";

    [Fact]
    public void PubSubMessage_Exact_SetsPropertiesCorrectly()
    {
        // Act
        var pubSubMessage = PubSubMessage.ExactMessage(message, channel);

        // Assert
        Assert.Equal(PubSubChannelMode.Exact, pubSubMessage.ChannelMode);
        Assert.Equal(message, pubSubMessage.Message);
        Assert.Equal(channel, pubSubMessage.Channel);
        Assert.Null(pubSubMessage.Pattern);
    }

    [Fact]
    public void PubSubMessage_Pattern_SetsPropertiesCorrectly()
    {
        // Act
        var pubSubMessage = PubSubMessage.PatternMessage(message, channel, pattern);

        // Assert
        Assert.Equal(PubSubChannelMode.Pattern, pubSubMessage.ChannelMode);
        Assert.Equal(message, pubSubMessage.Message);
        Assert.Equal(channel, pubSubMessage.Channel);
        Assert.Equal(pattern, pubSubMessage.Pattern);
    }

    [Fact]
    public void PubSubMessage_Sharded_SetsPropertiesCorrectly()
    {
        // Act
        var pubSubMessage = PubSubMessage.ShardedMessage(message, channel);

        // Assert
        Assert.Equal(PubSubChannelMode.Sharded, pubSubMessage.ChannelMode);
        Assert.Equal(message, pubSubMessage.Message);
        Assert.Equal(channel, pubSubMessage.Channel);
        Assert.Null(pubSubMessage.Pattern);
    }

    [Fact]
    public void PubSubMessage_ToString_Exact_ReturnsValidJsonWithNullPattern()
    {
        // Arrange
        var pubSubMessage = PubSubMessage.ExactMessage(message, channel);

        // Act
        var jsonString = pubSubMessage.ToString();

        // Assert
        Assert.NotNull(jsonString);
        Assert.NotEmpty(jsonString);

        // Verify it's valid JSON by deserializing
        JsonElement deserializedObject = JsonSerializer.Deserialize<JsonElement>(jsonString);
        Assert.Equal(PubSubChannelMode.Exact.ToString(), deserializedObject.GetProperty("ChannelMode").ToString());
        Assert.Equal(message, deserializedObject.GetProperty("Message").GetString());
        Assert.Equal(channel, deserializedObject.GetProperty("Channel").GetString());
        Assert.Equal(JsonValueKind.Null, deserializedObject.GetProperty("Pattern").ValueKind);
    }

    [Fact]
    public void PubSubMessage_ToString_Pattern_ReturnsValidJson()
    {
        // Arrange
        var pubSubMessage = PubSubMessage.PatternMessage(message, channel, pattern);

        // Act
        var jsonString = pubSubMessage.ToString();

        // Assert
        Assert.NotNull(jsonString);
        Assert.NotEmpty(jsonString);

        // Verify it's valid JSON by deserializing
        JsonElement deserializedObject = JsonSerializer.Deserialize<JsonElement>(jsonString);
        Assert.Equal(PubSubChannelMode.Pattern.ToString(), deserializedObject.GetProperty("ChannelMode").ToString());
        Assert.Equal(message, deserializedObject.GetProperty("Message").GetString());
        Assert.Equal(channel, deserializedObject.GetProperty("Channel").GetString());
        Assert.Equal(pattern, deserializedObject.GetProperty("Pattern").GetString());
    }

    [Fact]
    public void PubSubMessage_ToString_Sharded_ReturnsValidJsonWithNullPattern()
    {
        // Arrange
        var pubSubMessage = PubSubMessage.ShardedMessage(message, channel);

        // Act
        var jsonString = pubSubMessage.ToString();

        // Assert
        Assert.NotNull(jsonString);
        Assert.NotEmpty(jsonString);

        // Verify it's valid JSON by deserializing
        JsonElement deserializedObject = JsonSerializer.Deserialize<JsonElement>(jsonString);
        Assert.Equal(PubSubChannelMode.Sharded.ToString(), deserializedObject.GetProperty("ChannelMode").ToString());
        Assert.Equal(message, deserializedObject.GetProperty("Message").GetString());
        Assert.Equal(channel, deserializedObject.GetProperty("Channel").GetString());
        Assert.Equal(JsonValueKind.Null, deserializedObject.GetProperty("Pattern").ValueKind);
    }

    [Fact]
    public void PubSubMessage_Equals_ReturnsTrueForEqualMessages()
    {
        // Arrange
        var pubSubMessage1 = PubSubMessage.PatternMessage(message, channel, pattern);
        var pubSubMessage2 = PubSubMessage.PatternMessage(message, channel, pattern);

        // Act & Assert
        Assert.Equal(pubSubMessage1, pubSubMessage2);
        Assert.True(pubSubMessage1.Equals(pubSubMessage2));
        Assert.True(pubSubMessage2.Equals(pubSubMessage1));
    }

    [Fact]
    public void PubSubMessage_Equals_ReturnsFalseForDifferentMessages()
    {
        // Arrange
        var pubSubMessage1 = PubSubMessage.PatternMessage("message1", channel, pattern);
        var pubSubMessage2 = PubSubMessage.PatternMessage("message2", channel, pattern);

        // Act & Assert
        Assert.NotEqual(pubSubMessage1, pubSubMessage2);
        Assert.False(pubSubMessage1.Equals(pubSubMessage2));
        Assert.False(pubSubMessage2.Equals(pubSubMessage1));
    }

    [Fact]
    public void PubSubMessage_Equals_ReturnsFalseForNull()
    {
        // Arrange
        var pubSubMessage = PubSubMessage.ExactMessage(message, channel);

        // Act & Assert
        Assert.False(pubSubMessage.Equals(null));
    }

    [Fact]
    public void PubSubMessage_GetHashCode_SameForEqualMessages()
    {
        // Arrange
        var pubSubMessage1 = PubSubMessage.PatternMessage(message, channel, pattern);
        var pubSubMessage2 = PubSubMessage.PatternMessage(message, channel, pattern);

        // Act & Assert
        Assert.Equal(pubSubMessage1.GetHashCode(), pubSubMessage2.GetHashCode());
    }

    [Fact]
    public void PubSubMessage_GetHashCode_DifferentForDifferentMessages()
    {
        // Arrange
        var pubSubMessage1 = PubSubMessage.PatternMessage("message1", channel, pattern);
        var pubSubMessage2 = PubSubMessage.PatternMessage("message2", channel, pattern);

        // Act & Assert
        Assert.NotEqual(pubSubMessage1.GetHashCode(), pubSubMessage2.GetHashCode());
    }

    [Fact]
    public void PubSubChannelMode_HasCorrectValues()
    {
        // Assert
        Assert.Equal(0, (int)PubSubChannelMode.Exact);
        Assert.Equal(1, (int)PubSubChannelMode.Pattern);
        Assert.Equal(2, (int)PubSubChannelMode.Sharded);
    }
}
