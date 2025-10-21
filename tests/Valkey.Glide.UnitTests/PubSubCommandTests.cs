// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Internals;

using Xunit;

namespace Valkey.Glide.UnitTests;

/// <summary>
/// Unit tests for PubSub command request building and parameter validation.
/// Tests command argument construction, parameter validation, and interface segregation.
/// </summary>
public class PubSubCommandTests
{
    [Fact]
    public void Publish_BuildsCorrectCommand()
    {
        // Test basic publish command
        Cmd<long, long> cmd = Request.Publish("channel", "message");
        Assert.Equal(["PUBLISH", "channel", "message"], cmd.GetArgs());
    }

    [Fact]
    public void Publish_WithDifferentChannelsAndMessages()
    {
        // Test with various channel and message combinations
        Assert.Multiple(
            () => Assert.Equal(["PUBLISH", "news", "Breaking news!"], Request.Publish("news", "Breaking news!").GetArgs()),
            () => Assert.Equal(["PUBLISH", "updates", "New update"], Request.Publish("updates", "New update").GetArgs()),
            () => Assert.Equal(["PUBLISH", "channel1", ""], Request.Publish("channel1", "").GetArgs()),
            () => Assert.Equal(["PUBLISH", "", "message"], Request.Publish("", "message").GetArgs())
        );
    }

    [Fact]
    public void SPublish_BuildsCorrectCommand()
    {
        // Test sharded publish command
        Cmd<long, long> cmd = Request.SPublish("shard-channel", "shard-message");
        Assert.Equal(["SPUBLISH", "shard-channel", "shard-message"], cmd.GetArgs());
    }

    [Fact]
    public void SPublish_WithDifferentChannelsAndMessages()
    {
        // Test with various sharded channel and message combinations
        Assert.Multiple(
            () => Assert.Equal(["SPUBLISH", "shard1", "message1"], Request.SPublish("shard1", "message1").GetArgs()),
            () => Assert.Equal(["SPUBLISH", "shard2", "message2"], Request.SPublish("shard2", "message2").GetArgs()),
            () => Assert.Equal(["SPUBLISH", "local-news", "Local update"], Request.SPublish("local-news", "Local update").GetArgs())
        );
    }

    [Fact]
    public void PubSubChannels_WithoutPattern_BuildsCorrectCommand()
    {
        // Test listing all channels
        Cmd<object[], string[]> cmd = Request.PubSubChannels();
        Assert.Equal(["PUBSUB", "CHANNELS"], cmd.GetArgs());
    }

    [Fact]
    public void PubSubChannels_WithPattern_BuildsCorrectCommand()
    {
        // Test listing channels with pattern
        Cmd<object[], string[]> cmd = Request.PubSubChannels("news.*");
        Assert.Equal(["PUBSUB", "CHANNELS", "news.*"], cmd.GetArgs());
    }

    [Fact]
    public void PubSubChannels_WithDifferentPatterns()
    {
        // Test with various patterns
        Assert.Multiple(
            () => Assert.Equal(["PUBSUB", "CHANNELS", "news.*"], Request.PubSubChannels("news.*").GetArgs()),
            () => Assert.Equal(["PUBSUB", "CHANNELS", "updates*"], Request.PubSubChannels("updates*").GetArgs()),
            () => Assert.Equal(["PUBSUB", "CHANNELS", "*"], Request.PubSubChannels("*").GetArgs()),
            () => Assert.Equal(["PUBSUB", "CHANNELS", "channel[123]"], Request.PubSubChannels("channel[123]").GetArgs())
        );
    }

    [Fact]
    public void PubSubNumSub_BuildsCorrectCommand()
    {
        // Test with single channel
        Cmd<Dictionary<GlideString, object>, Dictionary<string, long>> cmd = Request.PubSubNumSub(["channel1"]);
        Assert.Equal(["PUBSUB", "NUMSUB", "channel1"], cmd.GetArgs());
    }

    [Fact]
    public void PubSubNumSub_WithMultipleChannels()
    {
        // Test with multiple channels
        Cmd<Dictionary<GlideString, object>, Dictionary<string, long>> cmd = Request.PubSubNumSub(["channel1", "channel2", "channel3"]);
        Assert.Equal(["PUBSUB", "NUMSUB", "channel1", "channel2", "channel3"], cmd.GetArgs());
    }

    [Fact]
    public void PubSubNumSub_WithEmptyArray()
    {
        // Test with empty channel array
        Cmd<Dictionary<GlideString, object>, Dictionary<string, long>> cmd = Request.PubSubNumSub([]);
        Assert.Equal(["PUBSUB", "NUMSUB"], cmd.GetArgs());
    }

    [Fact]
    public void PubSubNumSub_ConvertsResponseCorrectly()
    {
        // Test response conversion from dictionary to dictionary
        Cmd<Dictionary<GlideString, object>, Dictionary<string, long>> cmd = Request.PubSubNumSub(["channel1", "channel2"]);

        // Simulate server response: Dictionary with channel names as keys and counts as values
        Dictionary<GlideString, object> response = new()
        {
            { (GlideString)"channel1", 5L },
            { (GlideString)"channel2", 10L }
        };

        Dictionary<string, long> result = cmd.Converter(response);

        Assert.Equal(2, result.Count);
        Assert.Equal(5L, result["channel1"]);
        Assert.Equal(10L, result["channel2"]);
    }

    [Fact]
    public void PubSubNumPat_BuildsCorrectCommand()
    {
        // Test pattern count command
        Cmd<long, long> cmd = Request.PubSubNumPat();
        Assert.Equal(["PUBSUB", "NUMPAT"], cmd.GetArgs());
    }

    [Fact]
    public void PubSubShardChannels_WithoutPattern_BuildsCorrectCommand()
    {
        // Test listing all sharded channels
        Cmd<object[], string[]> cmd = Request.PubSubShardChannels();
        Assert.Equal(["PUBSUB", "SHARDCHANNELS"], cmd.GetArgs());
    }

    [Fact]
    public void PubSubShardChannels_WithPattern_BuildsCorrectCommand()
    {
        // Test listing sharded channels with pattern
        Cmd<object[], string[]> cmd = Request.PubSubShardChannels("shard.*");
        Assert.Equal(["PUBSUB", "SHARDCHANNELS", "shard.*"], cmd.GetArgs());
    }

    [Fact]
    public void PubSubShardChannels_WithDifferentPatterns()
    {
        // Test with various patterns
        Assert.Multiple(
            () => Assert.Equal(["PUBSUB", "SHARDCHANNELS", "shard.*"], Request.PubSubShardChannels("shard.*").GetArgs()),
            () => Assert.Equal(["PUBSUB", "SHARDCHANNELS", "local*"], Request.PubSubShardChannels("local*").GetArgs()),
            () => Assert.Equal(["PUBSUB", "SHARDCHANNELS", "*"], Request.PubSubShardChannels("*").GetArgs())
        );
    }

    [Fact]
    public void PubSubShardNumSub_BuildsCorrectCommand()
    {
        // Test with single sharded channel
        Cmd<Dictionary<GlideString, object>, Dictionary<string, long>> cmd = Request.PubSubShardNumSub(["shard1"]);
        Assert.Equal(["PUBSUB", "SHARDNUMSUB", "shard1"], cmd.GetArgs());
    }

    [Fact]
    public void PubSubShardNumSub_WithMultipleChannels()
    {
        // Test with multiple sharded channels
        Cmd<Dictionary<GlideString, object>, Dictionary<string, long>> cmd = Request.PubSubShardNumSub(["shard1", "shard2", "shard3"]);
        Assert.Equal(["PUBSUB", "SHARDNUMSUB", "shard1", "shard2", "shard3"], cmd.GetArgs());
    }

    [Fact]
    public void PubSubShardNumSub_WithEmptyArray()
    {
        // Test with empty channel array
        Cmd<Dictionary<GlideString, object>, Dictionary<string, long>> cmd = Request.PubSubShardNumSub([]);
        Assert.Equal(["PUBSUB", "SHARDNUMSUB"], cmd.GetArgs());
    }

    [Fact]
    public void PubSubShardNumSub_ConvertsResponseCorrectly()
    {
        // Test response conversion from dictionary to dictionary
        Cmd<Dictionary<GlideString, object>, Dictionary<string, long>> cmd = Request.PubSubShardNumSub(["shard1", "shard2"]);

        // Simulate server response: Dictionary with shard names as keys and counts as values
        Dictionary<GlideString, object> response = new()
        {
            { (GlideString)"shard1", 3L },
            { (GlideString)"shard2", 7L }
        };

        Dictionary<string, long> result = cmd.Converter(response);

        Assert.Equal(2, result.Count);
        Assert.Equal(3L, result["shard1"]);
        Assert.Equal(7L, result["shard2"]);
    }

    [Fact]
    public void PubSubChannels_ConvertsResponseCorrectly()
    {
        // Test response conversion from object array to string array
        Cmd<object[], string[]> cmd = Request.PubSubChannels();

        // Simulate server response: ["channel1", "channel2", "channel3"]
        object[] response = [
            (GlideString)"channel1",
            (GlideString)"channel2",
            (GlideString)"channel3"
        ];

        string[] result = cmd.Converter(response);

        Assert.Equal(3, result.Length);
        Assert.Equal("channel1", result[0]);
        Assert.Equal("channel2", result[1]);
        Assert.Equal("channel3", result[2]);
    }

    [Fact]
    public void PubSubShardChannels_ConvertsResponseCorrectly()
    {
        // Test response conversion from object array to string array
        Cmd<object[], string[]> cmd = Request.PubSubShardChannels();

        // Simulate server response: ["shard1", "shard2"]
        object[] response = [
            (GlideString)"shard1",
            (GlideString)"shard2"
        ];

        string[] result = cmd.Converter(response);

        Assert.Equal(2, result.Length);
        Assert.Equal("shard1", result[0]);
        Assert.Equal("shard2", result[1]);
    }

    [Fact]
    public void PubSubCommands_HandleEmptyResponses()
    {
        // Test handling of empty responses
        Assert.Multiple(
            () =>
            {
                Cmd<object[], string[]> channelsCmd = Request.PubSubChannels();
                string[] channelsResult = channelsCmd.Converter([]);
                Assert.Empty(channelsResult);
            },
            () =>
            {
                Cmd<object[], string[]> shardChannelsCmd = Request.PubSubShardChannels();
                string[] shardChannelsResult = shardChannelsCmd.Converter([]);
                Assert.Empty(shardChannelsResult);
            },
            () =>
            {
                Cmd<Dictionary<GlideString, object>, Dictionary<string, long>> numSubCmd = Request.PubSubNumSub([]);
                Dictionary<string, long> numSubResult = numSubCmd.Converter(new Dictionary<GlideString, object>());
                Assert.Empty(numSubResult);
            },
            () =>
            {
                Cmd<Dictionary<GlideString, object>, Dictionary<string, long>> shardNumSubCmd = Request.PubSubShardNumSub([]);
                Dictionary<string, long> shardNumSubResult = shardNumSubCmd.Converter(new Dictionary<GlideString, object>());
                Assert.Empty(shardNumSubResult);
            }
        );
    }

    [Fact]
    public void PubSubCommands_HandleSpecialCharacters()
    {
        // Test commands with special characters in channel names and messages
        Assert.Multiple(
            () => Assert.Equal(["PUBLISH", "channel:with:colons", "message"], Request.Publish("channel:with:colons", "message").GetArgs()),
            () => Assert.Equal(["PUBLISH", "channel-with-dashes", "message"], Request.Publish("channel-with-dashes", "message").GetArgs()),
            () => Assert.Equal(["PUBLISH", "channel_with_underscores", "message"], Request.Publish("channel_with_underscores", "message").GetArgs()),
            () => Assert.Equal(["PUBLISH", "channel.with.dots", "message"], Request.Publish("channel.with.dots", "message").GetArgs()),
            () => Assert.Equal(["PUBLISH", "channel", "message:with:special:chars"], Request.Publish("channel", "message:with:special:chars").GetArgs())
        );
    }

    [Fact]
    public void PubSubCommands_HandleUnicodeCharacters()
    {
        // Test commands with Unicode characters
        Assert.Multiple(
            () => Assert.Equal(["PUBLISH", "频道", "消息"], Request.Publish("频道", "消息").GetArgs()),
            () => Assert.Equal(["PUBLISH", "канал", "сообщение"], Request.Publish("канал", "сообщение").GetArgs()),
            () => Assert.Equal(["PUBLISH", "チャンネル", "メッセージ"], Request.Publish("チャンネル", "メッセージ").GetArgs())
        );
    }
}
