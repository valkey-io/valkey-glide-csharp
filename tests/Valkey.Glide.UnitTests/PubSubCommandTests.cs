// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.UnitTests;

/// <summary>
/// Unit tests for pub/sub command request building and parameter validation.
/// </summary>
public class PubSubCommandTests
{
    // Test constants
    private static readonly string Channel1 = "news";
    private static readonly string Channel2 = "weather";
    private static readonly string Pattern1 = "news.*";
    private static readonly string Pattern2 = "weather.*";
    private static readonly string Message = "message";
    private static readonly double TimeoutMs = 123;
    private static readonly string TimeoutExpected = TimeoutMs.ToString();

    #region PublishCommands

    [Fact]
    public void Publish_BuildsCorrectCommand()
    {
        Assert.Equal(["PUBLISH", Channel2, Message], Request.Publish(Channel2, Message).GetArgs());
        Assert.Equal(["PUBLISH", Channel2, ""], Request.Publish(Channel2, "").GetArgs());
        Assert.Equal(["PUBLISH", "", Message], Request.Publish("", Message).GetArgs());
    }

    [Fact]
    public void SPublish_BuildsCorrectCommand()
    {
        Assert.Equal(["SPUBLISH", Channel1, Message], Request.SPublish(Channel1, Message).GetArgs());
        Assert.Equal(["SPUBLISH", Channel1, ""], Request.SPublish(Channel1, "").GetArgs());
        Assert.Equal(["SPUBLISH", "", Message], Request.SPublish("", Message).GetArgs());
    }

    #endregion
    #region SubscribeCommands

    [Fact]
    public void Subscribe_BuildsCorrectCommand()
    {
        Assert.Equal(["SUBSCRIBEBLOCKING", Channel1, TimeoutExpected], Request.SubscribeBlocking([Channel1], TimeoutMs).GetArgs());
        Assert.Equal(["SUBSCRIBEBLOCKING", Channel1, Channel2, TimeoutExpected], Request.SubscribeBlocking([Channel1, Channel2], TimeoutMs).GetArgs());
    }

    [Fact]
    public void SubscribeLazy_BuildsCorrectCommand()
    {
        Assert.Equal(["SUBSCRIBE", Channel1], Request.Subscribe([Channel1]).GetArgs());
        Assert.Equal(["SUBSCRIBE", Channel1, Channel2], Request.Subscribe([Channel1, Channel2]).GetArgs());
    }

    [Fact]
    public void PSubscribe_BuildsCorrectCommand()
    {
        Assert.Equal(["PSUBSCRIBEBLOCKING", Pattern1, TimeoutExpected], Request.PSubscribeBlocking([Pattern1], TimeoutMs).GetArgs());
        Assert.Equal(["PSUBSCRIBEBLOCKING", Pattern1, Pattern2, TimeoutExpected], Request.PSubscribeBlocking([Pattern1, Pattern2], TimeoutMs).GetArgs());
    }

    [Fact]
    public void PSubscribeLazy_BuildsCorrectCommand()
    {
        Assert.Equal(["PSUBSCRIBE", Pattern1], Request.PSubscribe([Pattern1]).GetArgs());
        Assert.Equal(["PSUBSCRIBE", Pattern1, Pattern2], Request.PSubscribe([Pattern1, Pattern2]).GetArgs());
    }

    [Fact]
    public void SSubscribe_BuildsCorrectCommand()
    {
        Assert.Equal(["SSUBSCRIBEBLOCKING", Channel1, TimeoutExpected], Request.SSubscribeBlocking([Channel1], TimeoutMs).GetArgs());
        Assert.Equal(["SSUBSCRIBEBLOCKING", Channel1, Channel2, TimeoutExpected], Request.SSubscribeBlocking([Channel1, Channel2], TimeoutMs).GetArgs());
    }

    [Fact]
    public void SSubscribeLazy_BuildsCorrectCommand()
    {
        Assert.Equal(["SSUBSCRIBE", Channel1], Request.SSubscribe([Channel1]).GetArgs());
        Assert.Equal(["SSUBSCRIBE", Channel1, Channel2], Request.SSubscribe([Channel1, Channel2]).GetArgs());
    }

    #endregion
    #region UnsubscribeCommands

    [Fact]
    public void UnSubscribe_BuildsCorrectCommand()
    {
        Assert.Equal(["UNSUBSCRIBEBLOCKING", TimeoutExpected], Request.UnsubscribeBlocking([], TimeoutMs).GetArgs());
        Assert.Equal(["UNSUBSCRIBEBLOCKING", Channel1, TimeoutExpected], Request.UnsubscribeBlocking([Channel1], TimeoutMs).GetArgs());
        Assert.Equal(["UNSUBSCRIBEBLOCKING", Channel1, Channel2, TimeoutExpected], Request.UnsubscribeBlocking([Channel1, Channel2], TimeoutMs).GetArgs());
    }

    [Fact]
    public void UnSubscribeLazy_BuildsCorrectCommand()
    {
        Assert.Equal(["UNSUBSCRIBE"], Request.Unsubscribe([]).GetArgs());
        Assert.Equal(["UNSUBSCRIBE", Channel1], Request.Unsubscribe([Channel1]).GetArgs());
        Assert.Equal(["UNSUBSCRIBE", Channel1, Channel2], Request.Unsubscribe([Channel1, Channel2]).GetArgs());
    }

    [Fact]
    public void PUnSubscribe_BuildsCorrectCommand()
    {
        Assert.Equal(["PUNSUBSCRIBEBLOCKING", TimeoutExpected], Request.PUnsubscribeBlocking([], TimeoutMs).GetArgs());
        Assert.Equal(["PUNSUBSCRIBEBLOCKING", Pattern1, TimeoutExpected], Request.PUnsubscribeBlocking([Pattern1], TimeoutMs).GetArgs());
        Assert.Equal(["PUNSUBSCRIBEBLOCKING", Pattern1, Pattern2, TimeoutExpected], Request.PUnsubscribeBlocking([Pattern1, Pattern2], TimeoutMs).GetArgs());
    }

    [Fact]
    public void PUnSubscribeLazy_BuildsCorrectCommand()
    {
        Assert.Equal(["PUNSUBSCRIBE"], Request.PUnsubscribe([]).GetArgs());
        Assert.Equal(["PUNSUBSCRIBE", Pattern1], Request.PUnsubscribe([Pattern1]).GetArgs());
        Assert.Equal(["PUNSUBSCRIBE", Pattern1, Pattern2], Request.PUnsubscribe([Pattern1, Pattern2]).GetArgs());
    }

    [Fact]
    public void SUnSubscribe_BuildsCorrectCommand()
    {
        Assert.Equal(["SUNSUBSCRIBEBLOCKING", TimeoutExpected], Request.SUnsubscribeBlocking([], TimeoutMs).GetArgs());
        Assert.Equal(["SUNSUBSCRIBEBLOCKING", Channel1, TimeoutExpected], Request.SUnsubscribeBlocking([Channel1], TimeoutMs).GetArgs());
        Assert.Equal(["SUNSUBSCRIBEBLOCKING", Channel1, Channel2, TimeoutExpected], Request.SUnsubscribeBlocking([Channel1, Channel2], TimeoutMs).GetArgs());
    }

    [Fact]
    public void SUnSubscribeLazy_BuildsCorrectCommand()
    {
        Assert.Equal(["SUNSUBSCRIBE"], Request.SUnsubscribe([]).GetArgs());
        Assert.Equal(["SUNSUBSCRIBE", Channel1], Request.SUnsubscribe([Channel1]).GetArgs());
        Assert.Equal(["SUNSUBSCRIBE", Channel1, Channel2], Request.SUnsubscribe([Channel1, Channel2]).GetArgs());
    }

    #endregion
    #region IntrospectionCommands

    [Fact]
    public void PubSubChannels_BuildsCorrectCommand()
    {
        Assert.Equal(["PUBSUB", "CHANNELS"], Request.PubSubChannels().GetArgs());
        Assert.Equal(["PUBSUB", "CHANNELS", Pattern1], Request.PubSubChannels(Pattern1).GetArgs());
        Assert.Equal(["PUBSUB", "CHANNELS", "*"], Request.PubSubChannels("*").GetArgs());
    }

    [Fact]
    public void PubSubChannels_ConvertsResponseCorrectly()
    {
        var cmd = Request.PubSubChannels();

        // Simulated response from server
        object[] response = [(GlideString)Channel1, (GlideString)Channel2];

        var expected = new[] { Channel1, Channel2 };
        Assert.Equivalent(expected, cmd.Converter(response));
    }

    [Fact]
    public void PubSubNumSub_BuildsCorrectCommand()
    {
        Assert.Equal(["PUBSUB", "NUMSUB"], Request.PubSubNumSub([]).GetArgs());
        Assert.Equal(["PUBSUB", "NUMSUB", Channel1], Request.PubSubNumSub([Channel1]).GetArgs());
        Assert.Equal(["PUBSUB", "NUMSUB", Channel1, Channel2], Request.PubSubNumSub([Channel1, Channel2]).GetArgs());
    }

    [Fact]
    public void PubSubNumSub_ConvertsResponseCorrectly()
    {
        var cmd = Request.PubSubNumSub([Channel1, Channel2]);

        // Simulated response from server
        Dictionary<GlideString, object> response = new()
        {
            { (GlideString)Channel1, 5L },
            { (GlideString)Channel2, 10L }
        };

        var expected = new Dictionary<string, long>
        {
            { Channel1, 5L },
            { Channel2, 10L }
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
        Assert.Equal(["PUBSUB", "SHARDCHANNELS", Pattern1], Request.PubSubShardChannels(Pattern1).GetArgs());
    }

    [Fact]
    public void PubSubShardNumSub_BuildsCorrectCommand()
    {
        Assert.Equal(["PUBSUB", "SHARDNUMSUB"], Request.PubSubShardNumSub([]).GetArgs());
        Assert.Equal(["PUBSUB", "SHARDNUMSUB", Channel1], Request.PubSubShardNumSub([Channel1]).GetArgs());
        Assert.Equal(["PUBSUB", "SHARDNUMSUB", Channel1, Channel2], Request.PubSubShardNumSub([Channel1, Channel2]).GetArgs());
    }

    [Fact]
    public void PubSubShardNumSub_ConvertsResponseCorrectly()
    {
        var cmd = Request.PubSubShardNumSub([Channel1, Channel2]);

        // Simulated response from server
        Dictionary<GlideString, object> response = new()
        {
            { (GlideString)Channel1, 3L },
            { (GlideString)Channel2, 7L }
        };

        var expected = new Dictionary<string, long>
        {
            { Channel1, 3L },
            { Channel2, 7L }
        };

        Assert.Equivalent(expected, cmd.Converter(response));
    }

    [Fact]
    public void PubSubShardChannels_ConvertsResponseCorrectly()
    {
        var cmd = Request.PubSubShardChannels();

        // Simulated response from server
        object[] response = [(GlideString)Channel1, (GlideString)Channel2];

        var expected = new[] { Channel1, Channel2 };
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
        Assert.Equal(["PUBLISH", Channel1, "message:with:special:chars"], Request.Publish(Channel1, "message:with:special:chars").GetArgs());
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
