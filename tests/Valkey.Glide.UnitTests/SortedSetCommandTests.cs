// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using System.Linq;

using Valkey.Glide.Commands.Options;
using Valkey.Glide.Internals;

using Xunit;

namespace Valkey.Glide.UnitTests;

public class SortedSetCommandTests
{
    [Fact]
    public void SortedSetCommands_ValidateArguments()
    {
        Assert.Multiple(
            // SortedSetAdd - Single Member
            () => Assert.Equal(["ZADD", "key", "10.5", "member"], Request.SortedSetAddAsync("key", "member", 10.5).GetArgs()),
            () => Assert.Equal(["ZADD", "key", "NX", "10.5", "member"], Request.SortedSetAddAsync("key", "member", 10.5, SortedSetWhen.NotExists).GetArgs()),
            () => Assert.Equal(["ZADD", "key", "XX", "10.5", "member"], Request.SortedSetAddAsync("key", "member", 10.5, SortedSetWhen.Exists).GetArgs()),
            () => Assert.Equal(["ZADD", "key", "GT", "10.5", "member"], Request.SortedSetAddAsync("key", "member", 10.5, SortedSetWhen.GreaterThan).GetArgs()),
            () => Assert.Equal(["ZADD", "key", "LT", "10.5", "member"], Request.SortedSetAddAsync("key", "member", 10.5, SortedSetWhen.LessThan).GetArgs()),

            // SortedSetAdd - Multiple Members
            () => Assert.Equal(["ZADD", "key", "10.5", "member1", "8.25", "member2"], Request.SortedSetAddAsync("key", [
                new SortedSetEntry("member1", 10.5),
                new SortedSetEntry("member2", 8.25)
            ]).GetArgs()),
            () => Assert.Equal(["ZADD", "key", "NX", "10.5", "member1", "8.25", "member2"], Request.SortedSetAddAsync("key", [
                new SortedSetEntry("member1", 10.5),
                new SortedSetEntry("member2", 8.25)
            ], SortedSetWhen.NotExists).GetArgs()),
            () => Assert.Equal(["ZADD", "key", "XX", "10.5", "member1", "8.25", "member2"], Request.SortedSetAddAsync("key", [
                new SortedSetEntry("member1", 10.5),
                new SortedSetEntry("member2", 8.25)
            ], SortedSetWhen.Exists).GetArgs()),
            () => Assert.Equal(["ZADD", "key", "GT", "10.5", "member1", "8.25", "member2"], Request.SortedSetAddAsync("key", [
                new SortedSetEntry("member1", 10.5),
                new SortedSetEntry("member2", 8.25)
            ], SortedSetWhen.GreaterThan).GetArgs()),
            () => Assert.Equal(["ZADD", "key", "LT", "10.5", "member1", "8.25", "member2"], Request.SortedSetAddAsync("key", [
                new SortedSetEntry("member1", 10.5),
                new SortedSetEntry("member2", 8.25)
            ], SortedSetWhen.LessThan).GetArgs()),

            // SortedSetRemove - Single Member
            () => Assert.Equal(["ZREM", "key", "member"], Request.SortedSetRemoveAsync("key", "member").GetArgs()),

            // SortedSetRemove - Multiple Members
            () => Assert.Equal(["ZREM", "key", "member1", "member2", "member3"], Request.SortedSetRemoveAsync("key", ["member1", "member2", "member3"]).GetArgs()),
            () => Assert.Equal(["ZREM", "key"], Request.SortedSetRemoveAsync("key", Array.Empty<ValkeyValue>()).GetArgs()),
            () => Assert.Equal(["ZREM", "key", "", " ", "null", "0", "-1"], Request.SortedSetRemoveAsync("key", ["", " ", "null", "0", "-1"]).GetArgs()),

            // SortedSetCard
            () => Assert.Equal(["ZCARD", "key"], Request.SortedSetCardAsync("key").GetArgs()),
            () => Assert.Equal(["ZCARD", "mykey"], Request.SortedSetCardAsync("mykey").GetArgs()),
            () => Assert.Equal(["ZCARD", "test:sorted:set"], Request.SortedSetCardAsync("test:sorted:set").GetArgs()),
            () => Assert.Equal(["ZCARD", ""], Request.SortedSetCardAsync("").GetArgs()),

            // SortedSetCount
            () => Assert.Equal(["ZCOUNT", "key", "-inf", "+inf"], Request.SortedSetCountAsync("key").GetArgs()),
            () => Assert.Equal(["ZCOUNT", "key", "1", "10"], Request.SortedSetCountAsync("key", 1.0, 10.0).GetArgs()),
            () => Assert.Equal(["ZCOUNT", "key", "0", "100"], Request.SortedSetCountAsync("key", 0.0, 100.0).GetArgs()),
            () => Assert.Equal(["ZCOUNT", "key", "-5", "5"], Request.SortedSetCountAsync("key", -5.0, 5.0).GetArgs()),
            () => Assert.Equal(["ZCOUNT", "key", "1.5", "9.75"], Request.SortedSetCountAsync("key", 1.5, 9.75).GetArgs()),
            () => Assert.Equal(["ZCOUNT", "key", "0.1", "0.9"], Request.SortedSetCountAsync("key", 0.1, 0.9).GetArgs()),
            () => Assert.Equal(["ZCOUNT", "key", "-inf", "10"], Request.SortedSetCountAsync("key", double.NegativeInfinity, 10.0).GetArgs()),
            () => Assert.Equal(["ZCOUNT", "key", "0", "+inf"], Request.SortedSetCountAsync("key", 0.0, double.PositiveInfinity).GetArgs()),
            () => Assert.Equal(["ZCOUNT", "key", "-inf", "+inf"], Request.SortedSetCountAsync("key", double.NegativeInfinity, double.PositiveInfinity).GetArgs()),
            () => Assert.Equal(["ZCOUNT", "key", "1", "10"], Request.SortedSetCountAsync("key", 1.0, 10.0, Exclude.None).GetArgs()),
            () => Assert.Equal(["ZCOUNT", "key", "(1", "10"], Request.SortedSetCountAsync("key", 1.0, 10.0, Exclude.Start).GetArgs()),
            () => Assert.Equal(["ZCOUNT", "key", "1", "(10"], Request.SortedSetCountAsync("key", 1.0, 10.0, Exclude.Stop).GetArgs()),
            () => Assert.Equal(["ZCOUNT", "key", "(1", "(10"], Request.SortedSetCountAsync("key", 1.0, 10.0, Exclude.Both).GetArgs()),
            () => Assert.Equal(["ZCOUNT", "key", "0", "0"], Request.SortedSetCountAsync("key", 0.0, 0.0).GetArgs()),
            () => Assert.Equal(["ZCOUNT", "key", "(0", "(0"], Request.SortedSetCountAsync("key", 0.0, 0.0, Exclude.Both).GetArgs()),
            () => Assert.Equal(["ZCOUNT", "mykey", "1", "10"], Request.SortedSetCountAsync("mykey", 1.0, 10.0).GetArgs()),
            () => Assert.Equal(["ZCOUNT", "test:sorted:set", "1", "10"], Request.SortedSetCountAsync("test:sorted:set", 1.0, 10.0).GetArgs()),

            // SortedSetRangeByRank
            () => Assert.Equal(["ZRANGE", "key", "0", "-1"], Request.SortedSetRangeByRankAsync("key").GetArgs()),
            () => Assert.Equal(["ZRANGE", "key", "1", "5"], Request.SortedSetRangeByRankAsync("key", 1, 5).GetArgs()),
            () => Assert.Equal(["ZRANGE", "key", "-5", "-1"], Request.SortedSetRangeByRankAsync("key", -5, -1).GetArgs()),
            () => Assert.Equal(["ZRANGE", "key", "0", "-1", "REV"], Request.SortedSetRangeByRankAsync("key", 0, -1, Order.Descending).GetArgs()),
            () => Assert.Equal(["ZRANGE", "key", "0", "-1", "WITHSCORES"], Request.SortedSetRangeByRankWithScoresAsync("key").GetArgs()),
            () => Assert.Equal(["ZRANGE", "key", "1", "5", "WITHSCORES"], Request.SortedSetRangeByRankWithScoresAsync("key", 1, 5).GetArgs()),
            () => Assert.Equal(["ZRANGE", "key", "0", "-1", "REV", "WITHSCORES"], Request.SortedSetRangeByRankWithScoresAsync("key", 0, -1, Order.Descending).GetArgs()),
            () => Assert.Equal(["ZRANGE", "key", "0", "0"], Request.SortedSetRangeByRankAsync("key", 0, 0).GetArgs()),
            () => Assert.Equal(["ZRANGE", "mykey", "0", "10"], Request.SortedSetRangeByRankAsync("mykey", 0, 10).GetArgs()),

            // SortedSetRangeByScore
            () => Assert.Equal(["ZRANGE", "key", "-inf", "+inf", "BYSCORE"], Request.SortedSetRangeByScoreAsync("key").GetArgs()),
            () => Assert.Equal(["ZRANGE", "key", "1", "10", "BYSCORE"], Request.SortedSetRangeByScoreAsync("key", 1.0, 10.0).GetArgs()),
            () => Assert.Equal(["ZRANGE", "key", "1.5", "9.75", "BYSCORE"], Request.SortedSetRangeByScoreAsync("key", 1.5, 9.75).GetArgs()),
            () => Assert.Equal(["ZRANGE", "key", "-inf", "10", "BYSCORE"], Request.SortedSetRangeByScoreAsync("key", double.NegativeInfinity, 10.0).GetArgs()),
            () => Assert.Equal(["ZRANGE", "key", "(1", "10", "BYSCORE"], Request.SortedSetRangeByScoreAsync("key", 1.0, 10.0, Exclude.Start).GetArgs()),
            () => Assert.Equal(["ZRANGE", "key", "1", "(10", "BYSCORE"], Request.SortedSetRangeByScoreAsync("key", 1.0, 10.0, Exclude.Stop).GetArgs()),
            () => Assert.Equal(["ZRANGE", "key", "(1", "(10", "BYSCORE"], Request.SortedSetRangeByScoreAsync("key", 1.0, 10.0, Exclude.Both).GetArgs()),
            () => Assert.Equal(["ZRANGE", "key", "10", "1", "BYSCORE", "REV"], Request.SortedSetRangeByScoreAsync("key", 1.0, 10.0, Exclude.None, Order.Descending).GetArgs()),
            () => Assert.Equal(["ZRANGE", "key", "1", "10", "BYSCORE", "LIMIT", "2", "3"], Request.SortedSetRangeByScoreAsync("key", 1.0, 10.0, Exclude.None, Order.Ascending, 2, 3).GetArgs()),
            () => Assert.Equal(["ZRANGE", "key", "(10", "(1", "BYSCORE", "REV", "LIMIT", "1", "5"], Request.SortedSetRangeByScoreAsync("key", 1.0, 10.0, Exclude.Both, Order.Descending, 1, 5).GetArgs()),
            () => Assert.Equal(["ZRANGE", "key", "-inf", "+inf", "BYSCORE", "WITHSCORES"], Request.SortedSetRangeByScoreWithScoresAsync("key").GetArgs()),
            () => Assert.Equal(["ZRANGE", "key", "1", "10", "BYSCORE", "WITHSCORES"], Request.SortedSetRangeByScoreWithScoresAsync("key", 1.0, 10.0).GetArgs()),
            () => Assert.Equal(["ZRANGE", "key", "10", "1", "BYSCORE", "REV", "WITHSCORES"], Request.SortedSetRangeByScoreWithScoresAsync("key", 1.0, 10.0, Exclude.None, Order.Descending).GetArgs()),
            () => Assert.Equal(["ZRANGE", "key", "1", "10", "BYSCORE", "LIMIT", "2", "3", "WITHSCORES"], Request.SortedSetRangeByScoreWithScoresAsync("key", 1.0, 10.0, Exclude.None, Order.Ascending, 2, 3).GetArgs()),
            () => Assert.Equal(["ZRANGE", "key", "0", "0", "BYSCORE"], Request.SortedSetRangeByScoreAsync("key", 0.0, 0.0).GetArgs()),
            () => Assert.Equal(["ZRANGE", "key", "(0", "(0", "BYSCORE"], Request.SortedSetRangeByScoreAsync("key", 0.0, 0.0, Exclude.Both).GetArgs()),

            // SortedSetRangeByValue
            () => Assert.Equal(["ZRANGE", "key", "[a", "[z", "BYLEX"], Request.SortedSetRangeByValueAsync("key", "a", "z", Exclude.None, 0, -1).GetArgs()),
            () => Assert.Equal(["ZRANGE", "key", "[apple", "[zebra", "BYLEX"], Request.SortedSetRangeByValueAsync("key", "apple", "zebra", Exclude.None, 0, -1).GetArgs()),
            () => Assert.Equal(["ZRANGE", "key", "(a", "[z", "BYLEX"], Request.SortedSetRangeByValueAsync("key", "a", "z", Exclude.Start, 0, -1).GetArgs()),
            () => Assert.Equal(["ZRANGE", "key", "[a", "(z", "BYLEX"], Request.SortedSetRangeByValueAsync("key", "a", "z", Exclude.Stop, 0, -1).GetArgs()),
            () => Assert.Equal(["ZRANGE", "key", "(a", "(z", "BYLEX"], Request.SortedSetRangeByValueAsync("key", "a", "z", Exclude.Both, 0, -1).GetArgs()),
            () => Assert.Equal(["ZRANGE", "key", "[a", "[z", "BYLEX", "LIMIT", "2", "3"], Request.SortedSetRangeByValueAsync("key", "a", "z", Exclude.None, 2, 3).GetArgs()),
            () => Assert.Equal(["ZRANGE", "key", "(a", "(z", "BYLEX", "LIMIT", "1", "5"], Request.SortedSetRangeByValueAsync("key", "a", "z", Exclude.Both, 1, 5).GetArgs()),
            () => Assert.Equal(["ZRANGE", "key", "-", "+", "BYLEX"], Request.SortedSetRangeByValueAsync("key").GetArgs()),
            () => Assert.Equal(["ZRANGE", "key", "-", "+", "BYLEX"], Request.SortedSetRangeByValueAsync("key", default, default, Exclude.None, Order.Ascending, 0, -1).GetArgs()),
            () => Assert.Equal(["ZRANGE", "key", "+", "-", "BYLEX", "REV"], Request.SortedSetRangeByValueAsync("key", default, default, Exclude.None, Order.Descending, 0, -1).GetArgs()),
            () => Assert.Equal(["ZRANGE", "key", "-", "+", "BYLEX", "LIMIT", "2", "3"], Request.SortedSetRangeByValueAsync("key", default, default, Exclude.None, Order.Ascending, 2, 3).GetArgs()),
            () => Assert.Equal(["ZRANGE", "key", "-", "+", "BYLEX"], Request.SortedSetRangeByValueAsync("key", double.NegativeInfinity, double.PositiveInfinity, Exclude.None, Order.Ascending, 0, -1).GetArgs()),
            () => Assert.Equal(["ZRANGE", "key", "[a", "[a", "BYLEX"], Request.SortedSetRangeByValueAsync("key", "a", "a", Exclude.None, 0, -1).GetArgs()),
            () => Assert.Equal(["ZRANGE", "key", "[", "[z", "BYLEX"], Request.SortedSetRangeByValueAsync("key", "", "z", Exclude.None, 0, -1).GetArgs()),

            // SortedSetCombine operations
            () => Assert.Equal(["ZUNION", "2", "key1", "key2"], Request.SortedSetCombineAsync(SetOperation.Union, ["key1", "key2"]).GetArgs()),
            () => Assert.Equal(["ZINTER", "2", "key1", "key2"], Request.SortedSetCombineAsync(SetOperation.Intersect, ["key1", "key2"]).GetArgs()),
            () => Assert.Equal(["ZDIFF", "2", "key1", "key2"], Request.SortedSetCombineAsync(SetOperation.Difference, ["key1", "key2"]).GetArgs()),
            () => Assert.Equal(["ZUNION", "3", "key1", "key2", "key3", "WEIGHTS", "1", "2", "3"], Request.SortedSetCombineAsync(SetOperation.Union, ["key1", "key2", "key3"], [1.0, 2.0, 3.0]).GetArgs()),
            () => Assert.Equal(["ZINTER", "2", "key1", "key2", "AGGREGATE", "MAX"], Request.SortedSetCombineAsync(SetOperation.Intersect, ["key1", "key2"], null, Aggregate.Max).GetArgs()),
            () => Assert.Equal(["ZUNION", "2", "key1", "key2", "WEIGHTS", "1.5", "2.5", "AGGREGATE", "MIN"], Request.SortedSetCombineAsync(SetOperation.Union, ["key1", "key2"], [1.5, 2.5], Aggregate.Min).GetArgs()),
            () => Assert.Equal(["ZUNION", "2", "key1", "key2", "WITHSCORES"], Request.SortedSetCombineWithScoresAsync(SetOperation.Union, ["key1", "key2"]).GetArgs()),
            () => Assert.Equal(["ZINTER", "2", "key1", "key2", "WITHSCORES"], Request.SortedSetCombineWithScoresAsync(SetOperation.Intersect, ["key1", "key2"]).GetArgs()),
            () => Assert.Equal(["ZDIFF", "2", "key1", "key2", "WITHSCORES"], Request.SortedSetCombineWithScoresAsync(SetOperation.Difference, ["key1", "key2"]).GetArgs()),
            () => Assert.Equal(["ZUNION", "3", "key1", "key2", "key3", "WEIGHTS", "1", "2", "3", "WITHSCORES"], Request.SortedSetCombineWithScoresAsync(SetOperation.Union, ["key1", "key2", "key3"], [1.0, 2.0, 3.0]).GetArgs()),
            () => Assert.Equal(["ZINTER", "2", "key1", "key2", "AGGREGATE", "MAX", "WITHSCORES"], Request.SortedSetCombineWithScoresAsync(SetOperation.Intersect, ["key1", "key2"], null, Aggregate.Max).GetArgs()),
            () => Assert.Equal(["ZUNIONSTORE", "dest", "2", "key1", "key2"], Request.SortedSetCombineAndStoreAsync(SetOperation.Union, "dest", "key1", "key2").GetArgs()),
            () => Assert.Equal(["ZINTERSTORE", "dest", "2", "key1", "key2"], Request.SortedSetCombineAndStoreAsync(SetOperation.Intersect, "dest", "key1", "key2").GetArgs()),
            () => Assert.Equal(["ZDIFFSTORE", "dest", "2", "key1", "key2"], Request.SortedSetCombineAndStoreAsync(SetOperation.Difference, "dest", "key1", "key2").GetArgs()),
            () => Assert.Equal(["ZUNIONSTORE", "dest", "2", "key1", "key2", "AGGREGATE", "MAX"], Request.SortedSetCombineAndStoreAsync(SetOperation.Union, "dest", "key1", "key2", Aggregate.Max).GetArgs()),
            () => Assert.Equal(["ZUNIONSTORE", "dest", "3", "key1", "key2", "key3"], Request.SortedSetCombineAndStoreAsync(SetOperation.Union, "dest", ["key1", "key2", "key3"]).GetArgs()),
            () => Assert.Equal(["ZUNIONSTORE", "dest", "3", "key1", "key2", "key3", "WEIGHTS", "1", "2", "3"], Request.SortedSetCombineAndStoreAsync(SetOperation.Union, "dest", ["key1", "key2", "key3"], [1.0, 2.0, 3.0]).GetArgs()),
            () => Assert.Equal(["ZINTERSTORE", "dest", "2", "key1", "key2", "AGGREGATE", "MIN"], Request.SortedSetCombineAndStoreAsync(SetOperation.Intersect, "dest", ["key1", "key2"], null, Aggregate.Min).GetArgs()),

            // SortedSetIncrement
            () => Assert.Equal(["ZINCRBY", "key", "2.5", "member"], Request.SortedSetIncrementAsync("key", "member", 2.5).GetArgs()),
            () => Assert.Equal(["ZINCRBY", "key", "-1.5", "member"], Request.SortedSetIncrementAsync("key", "member", -1.5).GetArgs()),
            () => Assert.Equal(["ZINCRBY", "key", "0", "member"], Request.SortedSetIncrementAsync("key", "member", 0.0).GetArgs()),

            // SortedSetIntersectionLength
            () => Assert.Equal(["ZINTERCARD", "2", "key1", "key2"], Request.SortedSetIntersectionLengthAsync(["key1", "key2"]).GetArgs()),
            () => Assert.Equal(["ZINTERCARD", "3", "key1", "key2", "key3"], Request.SortedSetIntersectionLengthAsync(["key1", "key2", "key3"]).GetArgs()),
            () => Assert.Equal(["ZINTERCARD", "2", "key1", "key2", "LIMIT", "10"], Request.SortedSetIntersectionLengthAsync(["key1", "key2"], 10).GetArgs()),

            // SortedSetLengthByValue
            () => Assert.Equal(["ZLEXCOUNT", "key", "[a", "[z"], Request.SortedSetLengthByValueAsync("key", "a", "z").GetArgs()),
            () => Assert.Equal(["ZLEXCOUNT", "key", "(a", "(z"], Request.SortedSetLengthByValueAsync("key", "a", "z", Exclude.Both).GetArgs()),
            () => Assert.Equal(["ZLEXCOUNT", "key", "(a", "[z"], Request.SortedSetLengthByValueAsync("key", "a", "z", Exclude.Start).GetArgs()),
            () => Assert.Equal(["ZLEXCOUNT", "key", "[a", "(z"], Request.SortedSetLengthByValueAsync("key", "a", "z", Exclude.Stop).GetArgs()),
            () => Assert.Equal(["ZLEXCOUNT", "key", "[", "["], Request.SortedSetLengthByValueAsync("key", ValkeyValue.Null, ValkeyValue.Null).GetArgs()),

            // SortedSetPop
            () => Assert.Equal(["ZMPOP", "2", "key1", "key2", "MIN", "COUNT", "1"], Request.SortedSetPopAsync(["key1", "key2"], 1).GetArgs()),
            () => Assert.Equal(["ZMPOP", "2", "key1", "key2", "MAX", "COUNT", "3"], Request.SortedSetPopAsync(["key1", "key2"], 3, Order.Descending).GetArgs()),
            () => Assert.Equal(["ZMPOP", "3", "key1", "key2", "key3", "MIN", "COUNT", "2"], Request.SortedSetPopAsync(["key1", "key2", "key3"], 2, Order.Ascending).GetArgs()),

            // SortedSetScores
            () => Assert.Equal(["ZMSCORE", "key", "member1"], Request.SortedSetScoresAsync("key", ["member1"]).GetArgs()),
            () => Assert.Equal(["ZMSCORE", "key", "member1", "member2", "member3"], Request.SortedSetScoresAsync("key", ["member1", "member2", "member3"]).GetArgs()),
            () => Assert.Equal(["ZMSCORE", "key"], Request.SortedSetScoresAsync("key", []).GetArgs()),

            // SortedSetBlockingPopAsync  - single key, single element (uses BZPOPMIN/BZPOPMAX)
            () => Assert.Equal(["BZPOPMIN", "key", "5"], Request.SortedSetBlockingPopAsync("key", Order.Ascending, 5.0).GetArgs()),
            () => Assert.Equal(["BZPOPMAX", "key", "0"], Request.SortedSetBlockingPopAsync("key", Order.Descending, 0.0).GetArgs()),
            () => Assert.Equal(["BZPOPMIN", "key", "10.5"], Request.SortedSetBlockingPopAsync("key", Order.Ascending, 10.5).GetArgs()),

            // SortedSetBlockingPopAsync - single key, multiple elements (always uses BZPOPMIN/BZPOPMAX like SER)
            () => Assert.Equal(["BZPOPMIN", "key", "5"], Request.SortedSetBlockingPopAsync("key", 3, Order.Ascending, 5.0).GetArgs()),
            () => Assert.Equal(["BZPOPMAX", "key", "0"], Request.SortedSetBlockingPopAsync("key", 1, Order.Descending, 0.0).GetArgs()),
            () => Assert.Equal(["BZPOPMIN", "key", "10.5"], Request.SortedSetBlockingPopAsync("key", 2, Order.Ascending, 10.5).GetArgs()),

            // SortedSetBlockingPopAsync (BZMPOP) - multi-key, multiple elements
            () => Assert.Equal(["BZMPOP", "5", "2", "key1", "key2", "MIN", "COUNT", "3"], Request.SortedSetBlockingPopAsync(["key1", "key2"], 3, Order.Ascending, 5.0).GetArgs()),
            () => Assert.Equal(["BZMPOP", "0", "1", "key", "MAX", "COUNT", "1"], Request.SortedSetBlockingPopAsync(["key"], 1, Order.Descending, 0.0).GetArgs()),
            () => Assert.Equal(["BZMPOP", "10.5", "3", "key1", "key2", "key3", "MIN", "COUNT", "2"], Request.SortedSetBlockingPopAsync(["key1", "key2", "key3"], 2, Order.Ascending, 10.5).GetArgs()),

            // Double formatting tests
            () => Assert.Equal("+inf", double.PositiveInfinity.ToGlideString().ToString()),
            () => Assert.Equal("-inf", double.NegativeInfinity.ToGlideString().ToString()),
            () => Assert.Equal("nan", double.NaN.ToGlideString().ToString()),
            () => Assert.Equal("0", 0.0.ToGlideString().ToString()),
            () => Assert.Equal("10.5", 10.5.ToGlideString().ToString()),

            () => Assert.Equal(["ZRANGE", "key", "-", "+", "BYLEX"], Request.SortedSetRangeByValueAsync("key", double.NegativeInfinity, double.PositiveInfinity, Exclude.None, Order.Ascending, 0, -1).GetArgs()),
            () => Assert.Equal(["ZRANGE", "key", "6", "2", "BYSCORE", "REV", "WITHSCORES"], Request.SortedSetRangeByScoreWithScoresAsync("key", 2.0, 6.0, order: Order.Descending).GetArgs())
        );
    }

    [Fact]
    public void SortedSetCommands_ValidateConverters()
    {
        Assert.Multiple(
            // Basic converter tests
            () => Assert.True(Request.SortedSetAddAsync("key", "member", 10.5).Converter(1L)),
            () => Assert.False(Request.SortedSetAddAsync("key", "member", 10.5).Converter(0L)),
            () => Assert.Equal(2L, Request.SortedSetAddAsync("key", [new SortedSetEntry("member1", 10.5), new SortedSetEntry("member2", 8.25)]).Converter(2L)),
            () => Assert.Equal(1L, Request.SortedSetAddAsync("key", [new SortedSetEntry("member1", 10.5)]).Converter(1L)),
            () => Assert.True(Request.SortedSetRemoveAsync("key", "member").Converter(1L)),
            () => Assert.False(Request.SortedSetRemoveAsync("key", "member").Converter(0L)),
            () => Assert.Equal(2L, Request.SortedSetRemoveAsync("key", ["member1", "member2"]).Converter(2L)),
            () => Assert.Equal(5L, Request.SortedSetCardAsync("key").Converter(5L)),
            () => Assert.Equal(3L, Request.SortedSetCountAsync("key", 1.0, 10.0).Converter(3L)),
            () => Assert.Equal(0L, Request.SortedSetCountAsync("key").Converter(0L)),

            // Type converter test
            () => Assert.Equal(ValkeyType.SortedSet, Request.KeyTypeAsync("key").Converter("zset"))
        );
    }

    [Fact]
    public void SortedSetCommands_ValidateArrayConverters()
    {
        // Test data for SortedSetRangeByRankAsync
        object[] testRankArray = [
            (GlideString)"member1",
            (GlideString)"member2",
            (GlideString)"member3"
        ];

        // Test data for SortedSetRangeByRankWithScoresAsync and SortedSetRangeByScoreWithScoresAsync
        Dictionary<GlideString, object> testScoreDict = new Dictionary<GlideString, object> {
            {"member1", 10.5},
            {"member2", 8.25},
            {"member3", 15.0}
        };

        Assert.Multiple(
            // Test SortedSetRangeByRankAsync converter
            () =>
            {
                ValkeyValue[] result = Request.SortedSetRangeByRankAsync("key", 0, -1).Converter(testRankArray);
                Assert.Equal(3, result.Length);
                Assert.All(result, item => Assert.IsType<ValkeyValue>(item));
                Assert.Equal("member1", result[0]);
                Assert.Equal("member2", result[1]);
                Assert.Equal("member3", result[2]);
            },

            // Test SortedSetRangeByRankWithScoresAsync converter
            () =>
            {
                SortedSetEntry[] result = Request.SortedSetRangeByRankWithScoresAsync("key", 0, -1).Converter(testScoreDict);
                Assert.Equal(3, result.Length);
                Assert.All(result, entry => Assert.IsType<SortedSetEntry>(entry));
                Assert.Equal("member2", result[0].Element);
                Assert.Equal(8.25, result[0].Score);
                Assert.Equal("member1", result[1].Element);
                Assert.Equal(10.5, result[1].Score);
                Assert.Equal("member3", result[2].Element);
                Assert.Equal(15.0, result[2].Score);
            },

            // Test SortedSetRangeByScoreAsync converter
            () =>
            {
                ValkeyValue[] result = Request.SortedSetRangeByScoreAsync("key", 1.0, 20.0).Converter(testRankArray);
                Assert.Equal(3, result.Length);
                Assert.All(result, item => Assert.IsType<ValkeyValue>(item));
                Assert.Equal("member1", result[0]);
                Assert.Equal("member2", result[1]);
                Assert.Equal("member3", result[2]);
            },

            // Test SortedSetRangeByScoreWithScoresAsync converter
            () =>
            {
                SortedSetEntry[] result = Request.SortedSetRangeByScoreWithScoresAsync("key", 1.0, 20.0).Converter(testScoreDict);
                Assert.Equal(3, result.Length);
                Assert.All(result, entry => Assert.IsType<SortedSetEntry>(entry));
                // Check that entries have proper element and score values
                foreach (SortedSetEntry entry in result)
                {
                    Assert.IsType<ValkeyValue>(entry.Element);
                    Assert.IsType<double>(entry.Score);
                }
                // Validate specific values (sorted by score)
                var sortedResults = result.OrderBy(e => e.Score).ToArray();
                Assert.Equal("member2", result[0].Element);
                Assert.Equal(8.25, result[0].Score);
                Assert.Equal("member1", result[1].Element);
                Assert.Equal(10.5, result[1].Score);
                Assert.Equal("member3", result[2].Element);
                Assert.Equal(15.0, result[2].Score);
            },

            // Test SortedSetRangeByValueAsync converter
            () =>
            {
                ValkeyValue[] result = Request.SortedSetRangeByValueAsync("key", "a", "z", Exclude.None, 0, -1).Converter(testRankArray);
                Assert.Equal(3, result.Length);
                Assert.All(result, item => Assert.IsType<ValkeyValue>(item));
                Assert.Equal("member1", result[0]);
                Assert.Equal("member2", result[1]);
                Assert.Equal("member3", result[2]);
            },

            // Test SortedSetRangeByValueAsync with order converter
            // Note: This test validates the converter function only, not the ordering logic.
            () =>
            {
                ValkeyValue[] result = Request.SortedSetRangeByValueAsync("key", default, default, Exclude.None, Order.Descending, 0, -1).Converter(testRankArray);
                Assert.Equal(3, result.Length);
                Assert.All(result, item => Assert.IsType<ValkeyValue>(item));
                Assert.Equal("member1", result[0]);
                Assert.Equal("member2", result[1]);
                Assert.Equal("member3", result[2]);
            },

            // Test empty arrays
            () =>
            {
                ValkeyValue[] emptyResult = Request.SortedSetRangeByRankAsync("key").Converter([]);
                Assert.Empty(emptyResult);
            },

            () =>
            {
                SortedSetEntry[] emptyScoreResult = Request.SortedSetRangeByRankWithScoresAsync("key").Converter(new Dictionary<GlideString, object>());
                Assert.Empty(emptyScoreResult);
            },

            // Test SortedSetCombineWithScoresAsync converter
            () =>
            {
                SortedSetEntry[] result = Request.SortedSetCombineWithScoresAsync(SetOperation.Union, ["key1", "key2"]).Converter(testScoreDict);
                Assert.Equal(3, result.Length);
                Assert.All(result, entry => Assert.IsType<SortedSetEntry>(entry));
                // Check that entries are sorted by score
                var sortedResults = result.OrderBy(e => e.Score).ToArray();
                Assert.Equal("member2", sortedResults[0].Element);
                Assert.Equal(8.25, sortedResults[0].Score);
                Assert.Equal("member1", sortedResults[1].Element);
                Assert.Equal(10.5, sortedResults[1].Score);
                Assert.Equal("member3", sortedResults[2].Element);
                Assert.Equal(15.0, sortedResults[2].Score);
            },

            // Test SortedSetIncrementAsync converter
            () =>
            {
                double result = Request.SortedSetIncrementAsync("key", "member", 2.5).Converter(12.5);
                Assert.Equal(12.5, result);
            },

            // Test SortedSetPopAsync converter - with result
            () =>
            {
                object[] testPopResponse = [
                    (GlideString)"key1",
                    new Dictionary<GlideString, object>
                    {
                        { (GlideString)"member1", 10.5 },
                        { (GlideString)"member2", 8.25 }
                    }
                ];
                SortedSetPopResult result = Request.SortedSetPopAsync(["key1", "key2"], 2).Converter(testPopResponse);
                Assert.False(result.IsNull);
                Assert.Equal("key1", result.Key);
                Assert.Equal(2, result.Entries.Length);
                Assert.Equal("member1", result.Entries[0].Element);
                Assert.Equal(10.5, result.Entries[0].Score);
                Assert.Equal("member2", result.Entries[1].Element);
                Assert.Equal(8.25, result.Entries[1].Score);
            },

            // Test SortedSetPopAsync converter - null result
            () =>
            {
                SortedSetPopResult result = Request.SortedSetPopAsync(["key1", "key2"], 2).Converter(null);
                Assert.True(result.IsNull);
                Assert.Equal(ValkeyKey.Null, result.Key);
                Assert.Empty(result.Entries);
            },

            // Test SortedSetScoresAsync converter
            () =>
            {
                object[] testScoresResponse = [10.5, null, 8.25];
                double?[] result = Request.SortedSetScoresAsync("key", ["member1", "member2", "member3"]).Converter(testScoresResponse);
                Assert.Equal(3, result.Length);
                Assert.Equal(10.5, result[0]);
                Assert.Null(result[1]);
                Assert.Equal(8.25, result[2]);
            },

            // Test SortedSetBlockingPopAsync (single key, single element) converter
            () =>
            {
                // BZPOPMIN/BZPOPMAX returns [key, member, score]
                object[] testBlockingPopResponse = [
                    (GlideString)"key1",
                    (GlideString)"member1",
                    10.5
                ];
                SortedSetEntry? result = Request.SortedSetBlockingPopAsync("key1", Order.Ascending, 5.0).Converter(testBlockingPopResponse);
                Assert.NotNull(result);
                Assert.Equal("member1", result.Value.Element);
                Assert.Equal(10.5, result.Value.Score);
            },

            // Test SortedSetBlockingPopAsync (single key, single element) converter with null response
            () =>
            {
                SortedSetEntry? result = Request.SortedSetBlockingPopAsync("key1", Order.Ascending, 5.0).Converter(null);
                Assert.Null(result);
            },

            // Test SortedSetBlockingPopAsync (single key, multiple elements) converter
            () =>
            {
                // BZPOPMIN/BZPOPMAX returns [key, member, score] - only one element
                object[] testBlockingPopResponse = [
                    (GlideString)"key1",
                    (GlideString)"member1",
                    10.5
                ];
                SortedSetEntry[] result = Request.SortedSetBlockingPopAsync("key1", 2, Order.Ascending, 5.0).Converter(testBlockingPopResponse);
                Assert.Single(result);
                Assert.Equal("member1", result[0].Element);
                Assert.Equal(10.5, result[0].Score);
            },

            // Test SortedSetBlockingPopAsync (single key, multiple elements) converter with null response
            () =>
            {
                SortedSetEntry[] result = Request.SortedSetBlockingPopAsync("key1", 2, Order.Ascending, 5.0).Converter(null);
                Assert.Empty(result);
            },

            // Test SortedSetBlockingPopAsync (multi-key, multiple elements) converter
            () =>
            {
                object[] testBlockingPopResponse = [
                    (GlideString)"key1",
                    new Dictionary<GlideString, object>
                    {
                        { (GlideString)"member1", 10.5 },
                        { (GlideString)"member2", 8.25 }
                    }
                ];
                SortedSetPopResult result = Request.SortedSetBlockingPopAsync(["key1", "key2"], 2, Order.Ascending, 5.0).Converter(testBlockingPopResponse);
                Assert.False(result.IsNull);
                Assert.Equal("key1", result.Key);
                Assert.Equal(2, result.Entries.Length);
                Assert.Equal("member1", result.Entries[0].Element);
                Assert.Equal(10.5, result.Entries[0].Score);
                Assert.Equal("member2", result.Entries[1].Element);
                Assert.Equal(8.25, result.Entries[1].Score);
            },

            // Test SortedSetBlockingPopAsync (multi-key, multiple elements) converter with null response
            () =>
            {
                SortedSetPopResult result = Request.SortedSetBlockingPopAsync(["key1", "key2"], 2, Order.Ascending, 5.0).Converter(null);
                Assert.True(result.IsNull);
                Assert.Equal(ValkeyKey.Null, result.Key);
                Assert.Empty(result.Entries);
            },

            // Test empty arrays
            () =>
            {
                SortedSetEntry[] emptyResult = Request.SortedSetCombineWithScoresAsync(SetOperation.Union, ["key1"]).Converter(new Dictionary<GlideString, object>());
                Assert.Empty(emptyResult);
            }
        );
    }

    [Fact]
    public void RangeByLex_ToArgs_GeneratesCorrectArguments()
    {
        Assert.Multiple(
            // Basic range
            () => Assert.Equal(["[a", "[z", "BYLEX"], new RangeByLex(LexBoundary.Inclusive("a"), LexBoundary.Inclusive("z")).ToArgs()),

            // Exclusive boundaries
            () => Assert.Equal(["(a", "(z", "BYLEX"], new RangeByLex(LexBoundary.Exclusive("a"), LexBoundary.Exclusive("z")).ToArgs()),

            // Mixed boundaries
            () => Assert.Equal(["[a", "(z", "BYLEX"], new RangeByLex(LexBoundary.Inclusive("a"), LexBoundary.Exclusive("z")).ToArgs()),

            // Infinity boundaries
            () => Assert.Equal(["-", "+", "BYLEX"], new RangeByLex(LexBoundary.NegativeInfinity(), LexBoundary.PositiveInfinity()).ToArgs()),

            // With reverse
            () => Assert.Equal(["[z", "[a", "BYLEX", "REV"], new RangeByLex(LexBoundary.Inclusive("a"), LexBoundary.Inclusive("z")).SetReverse().ToArgs()),

            // With limit
            () => Assert.Equal(["[a", "[z", "BYLEX", "LIMIT", "10", "20"], new RangeByLex(LexBoundary.Inclusive("a"), LexBoundary.Inclusive("z")).SetLimit(10, 20).ToArgs()),

            // With reverse and limit
            () => Assert.Equal(["[z", "[a", "BYLEX", "REV", "LIMIT", "5", "15"], new RangeByLex(LexBoundary.Inclusive("a"), LexBoundary.Inclusive("z")).SetReverse().SetLimit(5, 15).ToArgs())
        );
    }

    [Fact]
    public void RangeByScore_ToArgs_GeneratesCorrectArguments()
    {
        Assert.Multiple(
            // Basic range
            () => Assert.Equal(["10", "20", "BYSCORE"], new RangeByScore(ScoreBoundary.Inclusive(10), ScoreBoundary.Inclusive(20)).ToArgs()),

            // Exclusive boundaries
            () => Assert.Equal(["(10", "(20", "BYSCORE"], new RangeByScore(ScoreBoundary.Exclusive(10), ScoreBoundary.Exclusive(20)).ToArgs()),

            // Mixed boundaries
            () => Assert.Equal(["10", "(20", "BYSCORE"], new RangeByScore(ScoreBoundary.Inclusive(10), ScoreBoundary.Exclusive(20)).ToArgs()),

            // Infinity boundaries
            () => Assert.Equal(["-inf", "+inf", "BYSCORE"], new RangeByScore(ScoreBoundary.NegativeInfinity(), ScoreBoundary.PositiveInfinity()).ToArgs()),

            // With reverse
            () => Assert.Equal(["20", "10", "BYSCORE", "REV"], new RangeByScore(ScoreBoundary.Inclusive(10), ScoreBoundary.Inclusive(20)).SetReverse().ToArgs()),

            // With limit
            () => Assert.Equal(["10", "20", "BYSCORE", "LIMIT", "10", "20"], new RangeByScore(ScoreBoundary.Inclusive(10), ScoreBoundary.Inclusive(20)).SetLimit(10, 20).ToArgs()),

            // With reverse and limit
            () => Assert.Equal(["20", "10", "BYSCORE", "REV", "LIMIT", "5", "15"], new RangeByScore(ScoreBoundary.Inclusive(10), ScoreBoundary.Inclusive(20)).SetReverse().SetLimit(5, 15).ToArgs())
        );
    }
}
