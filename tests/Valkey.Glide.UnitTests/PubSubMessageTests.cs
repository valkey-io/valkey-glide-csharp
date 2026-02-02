// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using System.Text.Json;

namespace Valkey.Glide.UnitTests;

public class PubSubMessageTests
{
    private static readonly string Message = "test message";
    private static readonly string Channel = "test-channel";
    private static readonly string Pattern = "test-*";

    [Fact]
    public void PubSubMessage_Exact_SetsPropertiesCorrectly()
    {
        // Act
        var pubSubMessage = PubSubMessage.FromChannel(Message, Channel);

        // Assert
        Assert.Equal(PubSubChannelMode.Exact, pubSubMessage.ChannelMode);
        Assert.Equal(Message, pubSubMessage.Message);
        Assert.Equal(Channel, pubSubMessage.Channel);
        Assert.Null(pubSubMessage.Pattern);
    }

    [Fact]
    public void PubSubMessage_Pattern_SetsPropertiesCorrectly()
    {
        // Act
        var pubSubMessage = PubSubMessage.FromPattern(Message, Channel, Pattern);

        // Assert
        Assert.Equal(PubSubChannelMode.Pattern, pubSubMessage.ChannelMode);
        Assert.Equal(Message, pubSubMessage.Message);
        Assert.Equal(Channel, pubSubMessage.Channel);
        Assert.Equal(Pattern, pubSubMessage.Pattern);
    }

    [Fact]
    public void PubSubMessage_Sharded_SetsPropertiesCorrectly()
    {
        // Act
        var pubSubMessage = PubSubMessage.FromShardChannel(Message, Channel);

        // Assert
        Assert.Equal(PubSubChannelMode.Sharded, pubSubMessage.ChannelMode);
        Assert.Equal(Message, pubSubMessage.Message);
        Assert.Equal(Channel, pubSubMessage.Channel);
        Assert.Null(pubSubMessage.Pattern);
    }

    [Fact]
    public void PubSubMessage_ToString_Exact_ReturnsValidJsonWithNullPattern()
    {
        // Arrange
        var pubSubMessage = PubSubMessage.FromChannel(Message, Channel);

        // Act
        var jsonString = pubSubMessage.ToString();

        // Assert
        Assert.NotNull(jsonString);
        Assert.NotEmpty(jsonString);

        // Verify it's valid JSON by deserializing
        JsonElement deserializedObject = JsonSerializer.Deserialize<JsonElement>(jsonString);
        Assert.Equal(PubSubChannelMode.Exact.ToString(), deserializedObject.GetProperty("ChannelMode").ToString());
        Assert.Equal(Message, deserializedObject.GetProperty("Message").GetString());
        Assert.Equal(Channel, deserializedObject.GetProperty("Channel").GetString());
        Assert.Equal(JsonValueKind.Null, deserializedObject.GetProperty("Pattern").ValueKind);
    }

    [Fact]
    public void PubSubMessage_ToString_Pattern_ReturnsValidJson()
    {
        // Arrange
        var pubSubMessage = PubSubMessage.FromPattern(Message, Channel, Pattern);

        // Act
        var jsonString = pubSubMessage.ToString();

        // Assert
        Assert.NotNull(jsonString);
        Assert.NotEmpty(jsonString);

        // Verify it's valid JSON by deserializing
        JsonElement deserializedObject = JsonSerializer.Deserialize<JsonElement>(jsonString);
        Assert.Equal(PubSubChannelMode.Pattern.ToString(), deserializedObject.GetProperty("ChannelMode").ToString());
        Assert.Equal(Message, deserializedObject.GetProperty("Message").GetString());
        Assert.Equal(Channel, deserializedObject.GetProperty("Channel").GetString());
        Assert.Equal(Pattern, deserializedObject.GetProperty("Pattern").GetString());
    }

    [Fact]
    public void PubSubMessage_ToString_Sharded_ReturnsValidJsonWithNullPattern()
    {
        // Arrange
        var pubSubMessage = PubSubMessage.FromShardChannel(Message, Channel);

        // Act
        var jsonString = pubSubMessage.ToString();

        // Assert
        Assert.NotNull(jsonString);
        Assert.NotEmpty(jsonString);

        // Verify it's valid JSON by deserializing
        JsonElement deserializedObject = JsonSerializer.Deserialize<JsonElement>(jsonString);
        Assert.Equal(PubSubChannelMode.Sharded.ToString(), deserializedObject.GetProperty("ChannelMode").ToString());
        Assert.Equal(Message, deserializedObject.GetProperty("Message").GetString());
        Assert.Equal(Channel, deserializedObject.GetProperty("Channel").GetString());
        Assert.Equal(JsonValueKind.Null, deserializedObject.GetProperty("Pattern").ValueKind);
    }

    [Fact]
    public void PubSubMessage_Equals_ReturnsTrueForEqualMessages()
    {
        // Arrange
        var pubSubMessage1 = PubSubMessage.FromPattern(Message, Channel, Pattern);
        var pubSubMessage2 = PubSubMessage.FromPattern(Message, Channel, Pattern);

        // Act & Assert
        Assert.Equal(pubSubMessage1, pubSubMessage2);
        Assert.True(pubSubMessage1.Equals(pubSubMessage2));
        Assert.True(pubSubMessage2.Equals(pubSubMessage1));
    }

    [Fact]
    public void PubSubMessage_Equals_ReturnsFalseForDifferentMessages()
    {
        // Arrange
        var pubSubMessage1 = PubSubMessage.FromPattern("message1", Channel, Pattern);
        var pubSubMessage2 = PubSubMessage.FromPattern("message2", Channel, Pattern);

        // Act & Assert
        Assert.NotEqual(pubSubMessage1, pubSubMessage2);
        Assert.False(pubSubMessage1.Equals(pubSubMessage2));
        Assert.False(pubSubMessage2.Equals(pubSubMessage1));
    }

    [Fact]
    public void PubSubMessage_Equals_ReturnsFalseForNull()
    {
        // Arrange
        var pubSubMessage = PubSubMessage.FromChannel(Message, Channel);

        // Act & Assert
        Assert.False(pubSubMessage.Equals(null));
    }

    [Fact]
    public void PubSubMessage_GetHashCode_SameForEqualMessages()
    {
        // Arrange
        var pubSubMessage1 = PubSubMessage.FromPattern(Message, Channel, Pattern);
        var pubSubMessage2 = PubSubMessage.FromPattern(Message, Channel, Pattern);

        // Act & Assert
        Assert.Equal(pubSubMessage1.GetHashCode(), pubSubMessage2.GetHashCode());
    }

    [Fact]
    public void PubSubMessage_GetHashCode_DifferentForDifferentMessages()
    {
        // Arrange
        var pubSubMessage1 = PubSubMessage.FromPattern("message1", Channel, Pattern);
        var pubSubMessage2 = PubSubMessage.FromPattern("message2", Channel, Pattern);

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
