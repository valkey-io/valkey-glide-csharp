// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using static Valkey.Glide.Internals.FFI;

namespace Valkey.Glide.UnitTests;

/// <summary>
/// Unit tests for pub/sub command request building and parameter validation.
/// </summary>
public class PubSubCommandTests
{
    // Test constants
    private const string Channel1 = "news";
    private const string Channel2 = "weather";
    private const string Pattern1 = "news.*";
    private const string Pattern2 = "weather.*";
    private const string Message = "message";

    private const string TimeoutExpected = "123";

    private static readonly TimeSpan Timeout = TimeSpan.FromMilliseconds(123);

    #region PublishCommands

    [Fact]
    public void Publish_BuildsCorrectCommand()
    {
        CmdAssert.Equal(RequestType.Publish, [Channel2, Message], Request.Publish(Channel2, Message));
        CmdAssert.Equal(RequestType.Publish, [Channel2, ""], Request.Publish(Channel2, ""));
        CmdAssert.Equal(RequestType.Publish, ["", Message], Request.Publish("", Message));
    }

    [Fact]
    public void SPublish_BuildsCorrectCommand()
    {
        CmdAssert.Equal(RequestType.SPublish, [Channel1, Message], Request.SPublish(Channel1, Message));
        CmdAssert.Equal(RequestType.SPublish, [Channel1, ""], Request.SPublish(Channel1, ""));
        CmdAssert.Equal(RequestType.SPublish, ["", Message], Request.SPublish("", Message));
    }

    #endregion
    #region SubscribeCommands

    [Fact]
    public void Subscribe_BuildsCorrectCommand()
    {
        CmdAssert.Equal(RequestType.SubscribeBlocking, [Channel1, TimeoutExpected], Request.SubscribeBlocking([Channel1], Timeout));
        CmdAssert.Equal(RequestType.SubscribeBlocking, [Channel1, Channel2, TimeoutExpected], Request.SubscribeBlocking([Channel1, Channel2], Timeout));
    }

    [Fact]
    public void SubscribeLazy_BuildsCorrectCommand()
    {
        CmdAssert.Equal(RequestType.Subscribe, [Channel1], Request.Subscribe([Channel1]));
        CmdAssert.Equal(RequestType.Subscribe, [Channel1, Channel2], Request.Subscribe([Channel1, Channel2]));
    }

    [Fact]
    public void PSubscribe_BuildsCorrectCommand()
    {
        CmdAssert.Equal(RequestType.PSubscribeBlocking, [Pattern1, TimeoutExpected], Request.PSubscribeBlocking([Pattern1], Timeout));
        CmdAssert.Equal(RequestType.PSubscribeBlocking, [Pattern1, Pattern2, TimeoutExpected], Request.PSubscribeBlocking([Pattern1, Pattern2], Timeout));
    }

    [Fact]
    public void PSubscribeLazy_BuildsCorrectCommand()
    {
        CmdAssert.Equal(RequestType.PSubscribe, [Pattern1], Request.PSubscribe([Pattern1]));
        CmdAssert.Equal(RequestType.PSubscribe, [Pattern1, Pattern2], Request.PSubscribe([Pattern1, Pattern2]));
    }

    [Fact]
    public void SSubscribe_BuildsCorrectCommand()
    {
        CmdAssert.Equal(RequestType.SSubscribeBlocking, [Channel1, TimeoutExpected], Request.SSubscribeBlocking([Channel1], Timeout));
        CmdAssert.Equal(RequestType.SSubscribeBlocking, [Channel1, Channel2, TimeoutExpected], Request.SSubscribeBlocking([Channel1, Channel2], Timeout));
    }

    [Fact]
    public void SSubscribeLazy_BuildsCorrectCommand()
    {
        CmdAssert.Equal(RequestType.SSubscribe, [Channel1], Request.SSubscribe([Channel1]));
        CmdAssert.Equal(RequestType.SSubscribe, [Channel1, Channel2], Request.SSubscribe([Channel1, Channel2]));
    }

    #endregion
    #region UnsubscribeCommands

    [Fact]
    public void Unsubscribe_BuildsCorrectCommand()
    {
        CmdAssert.Equal(RequestType.UnsubscribeBlocking, [TimeoutExpected], Request.UnsubscribeBlocking([], Timeout));
        CmdAssert.Equal(RequestType.UnsubscribeBlocking, [Channel1, TimeoutExpected], Request.UnsubscribeBlocking([Channel1], Timeout));
        CmdAssert.Equal(RequestType.UnsubscribeBlocking, [Channel1, Channel2, TimeoutExpected], Request.UnsubscribeBlocking([Channel1, Channel2], Timeout));
    }

    [Fact]
    public void UnsubscribeLazy_BuildsCorrectCommand()
    {
        CmdAssert.Equal(RequestType.Unsubscribe, [], Request.Unsubscribe([]));
        CmdAssert.Equal(RequestType.Unsubscribe, [Channel1], Request.Unsubscribe([Channel1]));
        CmdAssert.Equal(RequestType.Unsubscribe, [Channel1, Channel2], Request.Unsubscribe([Channel1, Channel2]));
    }

    [Fact]
    public void PUnsubscribe_BuildsCorrectCommand()
    {
        CmdAssert.Equal(RequestType.PUnsubscribeBlocking, [TimeoutExpected], Request.PUnsubscribeBlocking([], Timeout));
        CmdAssert.Equal(RequestType.PUnsubscribeBlocking, [Pattern1, TimeoutExpected], Request.PUnsubscribeBlocking([Pattern1], Timeout));
        CmdAssert.Equal(RequestType.PUnsubscribeBlocking, [Pattern1, Pattern2, TimeoutExpected], Request.PUnsubscribeBlocking([Pattern1, Pattern2], Timeout));
    }

    [Fact]
    public void PUnsubscribeLazy_BuildsCorrectCommand()
    {
        CmdAssert.Equal(RequestType.PUnsubscribe, [], Request.PUnsubscribe([]));
        CmdAssert.Equal(RequestType.PUnsubscribe, [Pattern1], Request.PUnsubscribe([Pattern1]));
        CmdAssert.Equal(RequestType.PUnsubscribe, [Pattern1, Pattern2], Request.PUnsubscribe([Pattern1, Pattern2]));
    }

    [Fact]
    public void SUnsubscribe_BuildsCorrectCommand()
    {
        CmdAssert.Equal(RequestType.SUnsubscribeBlocking, [TimeoutExpected], Request.SUnsubscribeBlocking([], Timeout));
        CmdAssert.Equal(RequestType.SUnsubscribeBlocking, [Channel1, TimeoutExpected], Request.SUnsubscribeBlocking([Channel1], Timeout));
        CmdAssert.Equal(RequestType.SUnsubscribeBlocking, [Channel1, Channel2, TimeoutExpected], Request.SUnsubscribeBlocking([Channel1, Channel2], Timeout));
    }

    [Fact]
    public void SUnsubscribeLazy_BuildsCorrectCommand()
    {
        CmdAssert.Equal(RequestType.SUnsubscribe, [], Request.SUnsubscribe([]));
        CmdAssert.Equal(RequestType.SUnsubscribe, [Channel1], Request.SUnsubscribe([Channel1]));
        CmdAssert.Equal(RequestType.SUnsubscribe, [Channel1, Channel2], Request.SUnsubscribe([Channel1, Channel2]));
    }

    #endregion
    #region IntrospectionCommands

    [Fact]
    public void PubSubChannels_BuildsCorrectCommand()
    {
        CmdAssert.Equal(RequestType.PubSubChannels, [], Request.PubSubChannels());
        CmdAssert.Equal(RequestType.PubSubChannels, [Pattern1], Request.PubSubChannels(Pattern1));
        CmdAssert.Equal(RequestType.PubSubChannels, ["*"], Request.PubSubChannels("*"));
    }

    [Fact]
    public void PubSubChannels_ConvertsResponseCorrectly()
    {
        var cmd = Request.PubSubChannels();

        // Simulated response from server
        GlideString[] response = [Channel1, Channel2];

        var expected = new HashSet<ValkeyKey> { Channel1, Channel2 };
        Assert.Equal(expected, cmd.Converter(response));
    }

    [Fact]
    public void PubSubNumSub_BuildsCorrectCommand()
    {
        CmdAssert.Equal(RequestType.PubSubNumSub, [], Request.PubSubNumSub([]));
        CmdAssert.Equal(RequestType.PubSubNumSub, [Channel1], Request.PubSubNumSub([Channel1]));
        CmdAssert.Equal(RequestType.PubSubNumSub, [Channel1, Channel2], Request.PubSubNumSub([Channel1, Channel2]));
    }

    [Fact]
    public void PubSubNumSub_ConvertsResponseCorrectly()
    {
        var cmd = Request.PubSubNumSub([Channel1, Channel2]);

        // Simulated response from server
        Dictionary<GlideString, object> response = new()
        {
            { Channel1, 5L },
            { Channel2, 10L }
        };

        var expected = new Dictionary<ValkeyKey, long>
        {
            { Channel1, 5L },
            { Channel2, 10L }
        };

        Assert.Equal(expected, cmd.Converter(response));
    }

    [Fact]
    public void PubSubNumPat_BuildsCorrectCommand()
        => CmdAssert.Equal(RequestType.PubSubNumPat, [], Request.PubSubNumPat());

    [Fact]
    public void PubSubShardChannels_BuildsCorrectCommand()
    {
        CmdAssert.Equal(RequestType.PubSubShardChannels, [], Request.PubSubShardChannels());
        CmdAssert.Equal(RequestType.PubSubShardChannels, [Pattern1], Request.PubSubShardChannels(Pattern1));
    }

    [Fact]
    public void PubSubShardNumSub_BuildsCorrectCommand()
    {
        CmdAssert.Equal(RequestType.PubSubShardNumSub, [], Request.PubSubShardNumSub([]));
        CmdAssert.Equal(RequestType.PubSubShardNumSub, [Channel1], Request.PubSubShardNumSub([Channel1]));
        CmdAssert.Equal(RequestType.PubSubShardNumSub, [Channel1, Channel2], Request.PubSubShardNumSub([Channel1, Channel2]));
    }

    [Fact]
    public void PubSubShardNumSub_ConvertsResponseCorrectly()
    {
        var cmd = Request.PubSubShardNumSub([Channel1, Channel2]);

        // Simulated response from server
        Dictionary<GlideString, object> response = new()
        {
            { Channel1, 3L },
            { Channel2, 7L }
        };

        var expected = new Dictionary<ValkeyKey, long>
        {
            { Channel1, 3L },
            { Channel2, 7L }
        };

        Assert.Equal(expected, cmd.Converter(response));
    }

    [Fact]
    public void PubSubShardChannels_ConvertsResponseCorrectly()
    {
        var cmd = Request.PubSubShardChannels();

        // Simulated response from server
        GlideString[] response = [Channel1, Channel2];

        var expected = new HashSet<ValkeyKey> { Channel1, Channel2 };
        Assert.Equal(expected, cmd.Converter(response));
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
        CmdAssert.Equal(RequestType.Publish, ["channel:with:colons", Message], Request.Publish("channel:with:colons", Message));
        CmdAssert.Equal(RequestType.Publish, ["channel-with-dashes", Message], Request.Publish("channel-with-dashes", Message));
        CmdAssert.Equal(RequestType.Publish, ["channel_with_underscores", Message], Request.Publish("channel_with_underscores", Message));
        CmdAssert.Equal(RequestType.Publish, ["channel.with.dots", Message], Request.Publish("channel.with.dots", Message));
        CmdAssert.Equal(RequestType.Publish, [Channel1, "message:with:special:chars"], Request.Publish(Channel1, "message:with:special:chars"));
    }

    [Fact]
    public void PubSubCommands_HandleUnicodeCharacters()
    {
        CmdAssert.Equal(RequestType.Publish, ["频道", "消息"], Request.Publish("频道", "消息"));
        CmdAssert.Equal(RequestType.Publish, ["канал", "сообщение"], Request.Publish("канал", "сообщение"));
        CmdAssert.Equal(RequestType.Publish, ["チャンネル", "メッセージ"], Request.Publish("チャンネル", "メッセージ"));
    }

    #endregion
}
