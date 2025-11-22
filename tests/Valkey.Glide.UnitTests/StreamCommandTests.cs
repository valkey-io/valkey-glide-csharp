// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Internals;

namespace Valkey.Glide.UnitTests;

public class StreamCommandTests
{
    [Fact]
    public void ValidateStreamCommandArgs()
    {
        Assert.Multiple(
            // StreamAdd
            () => Assert.Equal(["XADD", "key", "*", "field", "value"], Request.StreamAddAsync("key", default, null, default, false, [new NameValueEntry("field", "value")], null, false).GetArgs()),
            () => Assert.Equal(["XADD", "key", "1-0", "field1", "value1", "field2", "value2"], Request.StreamAddAsync("key", "1-0", null, default, false, [new NameValueEntry("field1", "value1"), new NameValueEntry("field2", "value2")], null, false).GetArgs()),
            () => Assert.Equal(["XADD", "key", "MAXLEN", "~", "1000", "*", "field", "value"], Request.StreamAddAsync("key", default, 1000, default, true, [new NameValueEntry("field", "value")], null, false).GetArgs()),
            () => Assert.Equal(["XADD", "key", "MINID", "~", "0-1", "*", "field", "value"], Request.StreamAddAsync("key", default, null, "0-1", true, [new NameValueEntry("field", "value")], null, false).GetArgs()),
            () => Assert.Equal(["XADD", "key", "NOMKSTREAM", "*", "field", "value"], Request.StreamAddAsync("key", default, null, default, false, [new NameValueEntry("field", "value")], null, true).GetArgs()),

            // StreamRead
            () => Assert.Equal(["XREAD", "STREAMS", "key", "0-0"], Request.StreamReadAsync("key", "0-0", null, null).GetArgs()),
            () => Assert.Equal(["XREAD", "COUNT", "10", "STREAMS", "key", "0-0"], Request.StreamReadAsync("key", "0-0", 10, null).GetArgs()),
            () => Assert.Equal(["XREAD", "BLOCK", "1000", "STREAMS", "key", "0-0"], Request.StreamReadAsync("key", "0-0", null, 1000).GetArgs()),
            () => Assert.Equal(["XREAD", "STREAMS", "key1", "key2", "0-0", "1-0"], Request.StreamReadAsync([new StreamPosition("key1", "0-0"), new StreamPosition("key2", "1-0")], null, null).GetArgs()),

            // StreamRange
            () => Assert.Equal(["XRANGE", "key", "-", "+"], Request.StreamRangeAsync("key", "-", "+", null, Order.Ascending).GetArgs()),
            () => Assert.Equal(["XRANGE", "key", "1-0", "2-0", "COUNT", "10"], Request.StreamRangeAsync("key", "1-0", "2-0", 10, Order.Ascending).GetArgs()),
            () => Assert.Equal(["XREVRANGE", "key", "-", "+"], Request.StreamRangeAsync("key", "-", "+", null, Order.Descending).GetArgs()),

            // StreamLength
            () => Assert.Equal(["XLEN", "key"], Request.StreamLengthAsync("key").GetArgs()),

            // StreamDelete
            () => Assert.Equal(["XDEL", "key", "1-0", "2-0"], Request.StreamDeleteAsync("key", ["1-0", "2-0"]).GetArgs()),

            // StreamTrim
            () => Assert.Equal(["XTRIM", "key", "MAXLEN", "1000"], Request.StreamTrimAsync("key", 1000, default, false, null).GetArgs()),
            () => Assert.Equal(["XTRIM", "key", "MAXLEN", "~", "1000"], Request.StreamTrimAsync("key", 1000, default, true, null).GetArgs()),
            () => Assert.Equal(["XTRIM", "key", "MINID", "0-1"], Request.StreamTrimAsync("key", null, "0-1", false, null).GetArgs()),

            // StreamCreateConsumerGroup
            () => Assert.Equal(["XGROUPCREATE", "key", "group", "$", "MKSTREAM"], Request.StreamCreateConsumerGroupAsync("key", "group", default, true, null).GetArgs()),
            () => Assert.Equal(["XGROUPCREATE", "key", "group", "0"], Request.StreamCreateConsumerGroupAsync("key", "group", "0", false, null).GetArgs()),
            () => Assert.Equal(["XGROUPCREATE", "key", "group", "0", "ENTRIESREAD", "10"], Request.StreamCreateConsumerGroupAsync("key", "group", "0", false, 10).GetArgs()),

            // StreamDeleteConsumerGroup
            () => Assert.Equal(["XGROUPDESTROY", "key", "group"], Request.StreamDeleteConsumerGroupAsync("key", "group").GetArgs()),

            // StreamDeleteConsumer
            () => Assert.Equal(["XGROUPDELCONSUMER", "key", "group", "consumer"], Request.StreamDeleteConsumerAsync("key", "group", "consumer").GetArgs()),

            // StreamConsumerGroupSetPosition
            () => Assert.Equal(["XGROUPSETID", "key", "group", "0-0"], Request.StreamConsumerGroupSetPositionAsync("key", "group", "0-0", null).GetArgs()),
            () => Assert.Equal(["XGROUPSETID", "key", "group", "0-0", "ENTRIESREAD", "5"], Request.StreamConsumerGroupSetPositionAsync("key", "group", "0-0", 5).GetArgs()),

            // StreamReadGroup
            () => Assert.Equal(["XREADGROUP", "GROUP", "group", "consumer", "STREAMS", "key", ">"], Request.StreamReadGroupAsync("key", "group", "consumer", default, null, false).GetArgs()),
            () => Assert.Equal(["XREADGROUP", "GROUP", "group", "consumer", "COUNT", "10", "STREAMS", "key", ">"], Request.StreamReadGroupAsync("key", "group", "consumer", default, 10, false).GetArgs()),
            () => Assert.Equal(["XREADGROUP", "GROUP", "group", "consumer", "NOACK", "STREAMS", "key", ">"], Request.StreamReadGroupAsync("key", "group", "consumer", default, null, true).GetArgs()),

            // StreamAcknowledge
            () => Assert.Equal(["XACK", "key", "group", "1-0", "2-0"], Request.StreamAcknowledgeAsync("key", "group", ["1-0", "2-0"]).GetArgs()),

            // StreamPending
            () => Assert.Equal(["XPENDING", "key", "group"], Request.StreamPendingAsync("key", "group").GetArgs()),

            // StreamPendingMessages
            () => Assert.Equal(["XPENDING", "key", "group", "-", "+", "10", "consumer"], Request.StreamPendingMessagesAsync("key", "group", "-", "+", 10, "consumer", null).GetArgs()),
            () => Assert.Equal(["XPENDING", "key", "group", "IDLE", "1000", "-", "+", "10", "consumer"], Request.StreamPendingMessagesAsync("key", "group", "-", "+", 10, "consumer", 1000).GetArgs()),

            // StreamClaim
            () => Assert.Equal(["XCLAIM", "key", "group", "consumer", "1000", "1-0"], Request.StreamClaimAsync("key", "group", "consumer", 1000, ["1-0"], null, null, null, false).GetArgs()),
            () => Assert.Equal(["XCLAIM", "key", "group", "consumer", "1000", "1-0", "IDLE", "500"], Request.StreamClaimAsync("key", "group", "consumer", 1000, ["1-0"], 500, null, null, false).GetArgs()),
            () => Assert.Equal(["XCLAIM", "key", "group", "consumer", "1000", "1-0", "FORCE"], Request.StreamClaimAsync("key", "group", "consumer", 1000, ["1-0"], null, null, null, true).GetArgs()),

            // StreamClaimIdsOnly
            () => Assert.Equal(["XCLAIM", "key", "group", "consumer", "1000", "1-0", "JUSTID"], Request.StreamClaimIdsOnlyAsync("key", "group", "consumer", 1000, ["1-0"], null, null, null, false).GetArgs()),

            // StreamAutoClaim
            () => Assert.Equal(["XAUTOCLAIM", "key", "group", "consumer", "1000", "0-0"], Request.StreamAutoClaimAsync("key", "group", "consumer", 1000, "0-0", null).GetArgs()),
            () => Assert.Equal(["XAUTOCLAIM", "key", "group", "consumer", "1000", "0-0", "COUNT", "10"], Request.StreamAutoClaimAsync("key", "group", "consumer", 1000, "0-0", 10).GetArgs()),

            // StreamAutoClaimIdsOnly
            () => Assert.Equal(["XAUTOCLAIM", "key", "group", "consumer", "1000", "0-0", "JUSTID"], Request.StreamAutoClaimIdsOnlyAsync("key", "group", "consumer", 1000, "0-0", null).GetArgs()),

            // StreamInfo
            () => Assert.Equal(["XINFOSTREAM", "key"], Request.StreamInfoAsync("key").GetArgs()),

            // StreamGroupInfo
            () => Assert.Equal(["XINFOGROUPS", "key"], Request.StreamGroupInfoAsync("key").GetArgs()),

            // StreamConsumerInfo
            () => Assert.Equal(["XINFOCONSUMERS", "key", "group"], Request.StreamConsumerInfoAsync("key", "group").GetArgs())
        );
    }

    [Fact]
    public void ValidateStreamCommandConverters()
    {
        Assert.Multiple(
            // StreamAdd
            () => Assert.Equal(new ValkeyValue("1-0"), Request.StreamAddAsync("key", default, null, default, false, [new NameValueEntry("f", "v")], null, false).Converter("1-0")),
            () => Assert.Equal(ValkeyValue.Null, Request.StreamAddAsync("key", default, null, default, false, [new NameValueEntry("f", "v")], null, false).Converter(null!)),

            // StreamLength
            () => Assert.Equal(5L, Request.StreamLengthAsync("key").Converter(5L)),
            () => Assert.Equal(0L, Request.StreamLengthAsync("key").Converter(0L)),

            // StreamDelete
            () => Assert.Equal(2L, Request.StreamDeleteAsync("key", ["1-0", "2-0"]).Converter(2L)),
            () => Assert.Equal(0L, Request.StreamDeleteAsync("key", ["1-0"]).Converter(0L)),

            // StreamTrim
            () => Assert.Equal(10L, Request.StreamTrimAsync("key", 100, default, false, null).Converter(10L)),

            // StreamCreateConsumerGroup
            () => Assert.True(Request.StreamCreateConsumerGroupAsync("key", "group", default, true, null).Converter("OK")),

            // StreamDeleteConsumerGroup
            () => Assert.True(Request.StreamDeleteConsumerGroupAsync("key", "group").Converter(true)),
            () => Assert.False(Request.StreamDeleteConsumerGroupAsync("key", "group").Converter(false)),

            // StreamDeleteConsumer
            () => Assert.Equal(5L, Request.StreamDeleteConsumerAsync("key", "group", "consumer").Converter(5L)),

            // StreamConsumerGroupSetPosition
            () => Assert.True(Request.StreamConsumerGroupSetPositionAsync("key", "group", "0-0", null).Converter("OK")),

            // StreamAcknowledge
            () => Assert.Equal(2L, Request.StreamAcknowledgeAsync("key", "group", ["1-0", "2-0"]).Converter(2L))
        );
    }
}
