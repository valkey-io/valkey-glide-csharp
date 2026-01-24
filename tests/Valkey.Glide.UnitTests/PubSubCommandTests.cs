// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.UnitTests;

/// <summary>
/// Unit tests for PubSub command request building and parameter validation.
/// Tests command argument construction, parameter validation, and interface segregation.
/// </summary>
public class PubSubCommandTests
{
    // Test constants
    private static readonly string ChannelNews = "news";
    private static readonly string ChannelWeather = "weather";
    private static readonly string PatternNews = "news.*";
    private static readonly string PatternWeather = "weather.*";
    private static readonly string Message = "message";

    #region PublishCommands

    [Fact]
    public void Publish_BuildsCorrectCommand()
    {
        Assert.Equal(["PUBLISH", ChannelWeather, Message], Request.Publish(ChannelWeather, Message).GetArgs());
        Assert.Equal(["PUBLISH", ChannelWeather, ""], Request.Publish(ChannelWeather, "").GetArgs());
        Assert.Equal(["PUBLISH", "", Message], Request.Publish("", Message).GetArgs());
    }

    [Fact]
    public void SPublish_BuildsCorrectCommand()
    {
        Assert.Equal(["SPUBLISH", ChannelNews, Message], Request.SPublish(ChannelNews, Message).GetArgs());
        Assert.Equal(["SPUBLISH", ChannelNews, ""], Request.SPublish(ChannelNews, "").GetArgs());
        Assert.Equal(["SPUBLISH", "", Message], Request.SPublish("", Message).GetArgs());
    }

    #endregion
    #region SubscribeCommands

    // TODO #193: Add Subscribe, PSubscribe, SSubscribe tests

    #endregion
    #region UnsubscribeCommands

    // TODO #193: Add Unsubscribe, PUnsubscribe, SUnsubscribe tests

    #endregion
    #region PubSubInfoCommands

    [Fact]
    public void PubSubChannels_BuildsCorrectCommand()
    {
        Assert.Equal(["PUBSUB", "CHANNELS"], Request.PubSubChannels().GetArgs());
        Assert.Equal(["PUBSUB", "CHANNELS", PatternNews], Request.PubSubChannels(PatternNews).GetArgs());
        Assert.Equal(["PUBSUB", "CHANNELS", "*"], Request.PubSubChannels("*").GetArgs());
    }

    [Fact]
    public void PubSubChannels_ConvertsResponseCorrectly()
    {
        var cmd = Request.PubSubChannels();

        // Simulated response from server
        object[] response = [(GlideString)ChannelNews, (GlideString)ChannelWeather];

        var expected = new[] { ChannelNews, ChannelWeather };
        Assert.Equivalent(expected, cmd.Converter(response));
    }

    [Fact]
    public void PubSubNumSub_BuildsCorrectCommand()
    {
        Assert.Equal(["PUBSUB", "NUMSUB"], Request.PubSubNumSub([]).GetArgs());
        Assert.Equal(["PUBSUB", "NUMSUB", ChannelNews], Request.PubSubNumSub([ChannelNews]).GetArgs());
        Assert.Equal(["PUBSUB", "NUMSUB", ChannelNews, ChannelWeather], Request.PubSubNumSub([ChannelNews, ChannelWeather]).GetArgs());
    }

    [Fact]
    public void PubSubNumSub_ConvertsResponseCorrectly()
    {
        var cmd = Request.PubSubNumSub([ChannelNews, ChannelWeather]);

        // Simulated response from server
        Dictionary<GlideString, object> response = new() {
             { (GlideString)ChannelNews, 5L },
              { (GlideString)ChannelWeather, 10L } };

        var expected = new Dictionary<string, long>
        {
            { ChannelNews, 5L },
            { ChannelWeather, 10L }
        };

        Assert.Equivalent(expected, cmd.Converter(response));
    }

    [Fact]
    public void PubSubNumPat_BuildsCorrectCommand()
    {
        Assert.Equal(["PUBSUB", "NUMPAT"], Request.PubSubNumPat().GetArgs());
    }

    [Fact]
    public void PubSubShardChannels_BuildsCorrectCommand()
    {
        Assert.Equal(["PUBSUB", "SHARDCHANNELS"], Request.PubSubShardChannels().GetArgs());
        Assert.Equal(["PUBSUB", "SHARDCHANNELS", PatternNews], Request.PubSubShardChannels(PatternNews).GetArgs());
    }

    [Fact]
    public void PubSubShardNumSub_BuildsCorrectCommand()
    {
        Assert.Equal(["PUBSUB", "SHARDNUMSUB"], Request.PubSubShardNumSub([]).GetArgs());
        Assert.Equal(["PUBSUB", "SHARDNUMSUB", ChannelNews], Request.PubSubShardNumSub([ChannelNews]).GetArgs());
        Assert.Equal(["PUBSUB", "SHARDNUMSUB", ChannelNews, ChannelWeather], Request.PubSubShardNumSub([ChannelNews, ChannelWeather]).GetArgs());
    }

    [Fact]
    public void PubSubShardNumSub_ConvertsResponseCorrectly()
    {
        var cmd = Request.PubSubShardNumSub([ChannelNews, ChannelWeather]);

        // Simulated response from server
        Dictionary<GlideString, object> response = new()
        {
            { (GlideString)ChannelNews, 3L },
            { (GlideString)ChannelWeather, 7L }
        };

        var expected = new Dictionary<string, long>
        {
            { ChannelNews, 3L },
            { ChannelWeather, 7L }
        };

        Assert.Equivalent(expected, cmd.Converter(response));
    }

    [Fact]
    public void PubSubShardChannels_ConvertsResponseCorrectly()
    {
        var cmd = Request.PubSubShardChannels();

        // Simulated response from server
        object[] response = [(GlideString)ChannelNews, (GlideString)ChannelWeather];

        var expected = new[] { ChannelNews, ChannelWeather };
        Assert.Equivalent(expected, cmd.Converter(response));
    }

    [Fact]
    public void PubSubCommands_HandleEmptyResponses()
    {
        Assert.Empty(Request.PubSubChannels().Converter([]));
        Assert.Empty(Request.PubSubShardChannels().Converter([]));
        Assert.Empty(Request.PubSubNumSub([]).Converter([]));
        Assert.Empty(Request.PubSubShardNumSub([]).Converter([]));
    }

    #endregion
    #region SpecialCharactersAndEdgeCases

    [Fact]
    public void PubSubCommands_HandleSpecialCharacters()
    {
        Assert.Equal(["PUBLISH", "channel:with:colons", Message], Request.Publish("channel:with:colons", Message).GetArgs());
        Assert.Equal(["PUBLISH", "channel-with-dashes", Message], Request.Publish("channel-with-dashes", Message).GetArgs());
        Assert.Equal(["PUBLISH", "channel_with_underscores", Message], Request.Publish("channel_with_underscores", Message).GetArgs());
        Assert.Equal(["PUBLISH", "channel.with.dots", Message], Request.Publish("channel.with.dots", Message).GetArgs());
        Assert.Equal(["PUBLISH", ChannelNews, "message:with:special:chars"], Request.Publish(ChannelNews, "message:with:special:chars").GetArgs());
    }

    [Fact]
    public void PubSubCommands_HandleUnicodeCharacters()
    {
        Assert.Equal(["PUBLISH", "频道", "消息"], Request.Publish("频道", "消息").GetArgs());
        Assert.Equal(["PUBLISH", "канал", "сообщение"], Request.Publish("канал", "сообщение").GetArgs());
        Assert.Equal(["PUBLISH", "チャンネル", "メッセージ"], Request.Publish("チャンネル", "メッセージ").GetArgs());
    }

    #endregion
}
