// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands.Options;

namespace Valkey.Glide.UnitTests;

public class SortedSetCommandTests
{
    [Fact]
    public void SortedSetCommands_ValidateArguments() => Assert.Multiple(
            // SortedSetAdd - Single member
            () => Assert.Equal(["ZADD", "key", "10.5", "member"], Request.SortedSetAdd("key", "member", 10.5).GetArgs()),
            () => Assert.Equal(["ZADD", "key", "NX", "10.5", "member"], Request.SortedSetAdd("key", "member", 10.5, new SortedSetAddOptions { Condition = SortedSetAddCondition.OnlyIfNotExists }).GetArgs()),
            () => Assert.Equal(["ZADD", "key", "XX", "10.5", "member"], Request.SortedSetAdd("key", "member", 10.5, new SortedSetAddOptions { Condition = SortedSetAddCondition.OnlyIfExists }).GetArgs()),
            () => Assert.Equal(["ZADD", "key", "GT", "10.5", "member"], Request.SortedSetAdd("key", "member", 10.5, new SortedSetAddOptions { Condition = SortedSetAddCondition.OnlyIfNotExistsOrGreaterThan }).GetArgs()),
            () => Assert.Equal(["ZADD", "key", "LT", "10.5", "member"], Request.SortedSetAdd("key", "member", 10.5, new SortedSetAddOptions { Condition = SortedSetAddCondition.OnlyIfNotExistsOrLessThan }).GetArgs()),
            () => Assert.Equal(["ZADD", "key", "XX", "GT", "10.5", "member"], Request.SortedSetAdd("key", "member", 10.5, new SortedSetAddOptions { Condition = SortedSetAddCondition.OnlyIfGreaterThan }).GetArgs()),
            () => Assert.Equal(["ZADD", "key", "XX", "LT", "10.5", "member"], Request.SortedSetAdd("key", "member", 10.5, new SortedSetAddOptions { Condition = SortedSetAddCondition.OnlyIfLessThan }).GetArgs()),
            () => Assert.Equal(["ZADD", "key", "CH", "10.5", "member"], Request.SortedSetAdd("key", "member", 10.5, new SortedSetAddOptions { Changed = true }).GetArgs()),
            () => Assert.Equal(["ZADD", "key", "NX", "CH", "10.5", "member"], Request.SortedSetAdd("key", "member", 10.5, new SortedSetAddOptions { Condition = SortedSetAddCondition.OnlyIfNotExists, Changed = true }).GetArgs()),

            // SortedSetAdd - Multiple members
            () => Assert.Equal(["ZADD", "key", "10.5", "member1", "8.25", "member2"], Request.SortedSetAdd("key", [new SortedSetEntry("member1", 10.5), new SortedSetEntry("member2", 8.25)]).GetArgs()),
            () => Assert.Equal(["ZADD", "key", "NX", "10.5", "member1", "8.25", "member2"], Request.SortedSetAdd("key", [new SortedSetEntry("member1", 10.5), new SortedSetEntry("member2", 8.25)], new SortedSetAddOptions { Condition = SortedSetAddCondition.OnlyIfNotExists }).GetArgs()),
            () => Assert.Equal(["ZADD", "key", "XX", "10.5", "member1", "8.25", "member2"], Request.SortedSetAdd("key", [new SortedSetEntry("member1", 10.5), new SortedSetEntry("member2", 8.25)], new SortedSetAddOptions { Condition = SortedSetAddCondition.OnlyIfExists }).GetArgs()),

            // SortedSetIncrementBy with options (ZADD INCR)
            () => Assert.Equal(["ZADD", "key", "NX", "INCR", "5", "member"], Request.SortedSetIncrementBy("key", "member", 5.0, new SortedSetAddOptions { Condition = SortedSetAddCondition.OnlyIfNotExists }).GetArgs()),
            () => Assert.Equal(["ZADD", "key", "XX", "GT", "INCR", "5", "member"], Request.SortedSetIncrementBy("key", "member", 5.0, new SortedSetAddOptions { Condition = SortedSetAddCondition.OnlyIfGreaterThan }).GetArgs()),

            // SortedSetRemove - Single Member
            () => Assert.Equal(["ZREM", "key", "member"], Request.SortedSetRemove("key", "member").GetArgs()),

            // SortedSetRemove - Multiple Members
            () => Assert.Equal(["ZREM", "key", "member1", "member2", "member3"], Request.SortedSetRemove("key", ["member1", "member2", "member3"]).GetArgs()),
            () => Assert.Equal(["ZREM", "key"], Request.SortedSetRemove("key", []).GetArgs()),
            () => Assert.Equal(["ZREM", "key", "", " ", "null", "0", "-1"], Request.SortedSetRemove("key", ["", " ", "null", "0", "-1"]).GetArgs()),

            // SortedSetCard
            () => Assert.Equal(["ZCARD", "key"], Request.SortedSetCard("key").GetArgs()),
            () => Assert.Equal(["ZCARD", "mykey"], Request.SortedSetCard("mykey").GetArgs()),
            () => Assert.Equal(["ZCARD", "test:sorted:set"], Request.SortedSetCard("test:sorted:set").GetArgs()),
            () => Assert.Equal(["ZCARD", ""], Request.SortedSetCard("").GetArgs()),

            // SortedSetCount
            () => Assert.Equal(["ZCOUNT", "key", "-inf", "+inf"], Request.SortedSetCount("key", ScoreRange.MinToMax).GetArgs()),
            () => Assert.Equal(["ZCOUNT", "key", "1", "10"], Request.SortedSetCount("key", ScoreRange.Between(1.0, 10.0)).GetArgs()),
            () => Assert.Equal(["ZCOUNT", "key", "0", "100"], Request.SortedSetCount("key", ScoreRange.Between(0.0, 100.0)).GetArgs()),
            () => Assert.Equal(["ZCOUNT", "key", "-5", "5"], Request.SortedSetCount("key", ScoreRange.Between(-5.0, 5.0)).GetArgs()),
            () => Assert.Equal(["ZCOUNT", "key", "1.5", "9.75"], Request.SortedSetCount("key", ScoreRange.Between(1.5, 9.75)).GetArgs()),
            () => Assert.Equal(["ZCOUNT", "key", "0.10000000000000001", "0.90000000000000002"], Request.SortedSetCount("key", ScoreRange.Between(0.1, 0.9)).GetArgs()),
            () => Assert.Equal(["ZCOUNT", "key", "-inf", "10"], Request.SortedSetCount("key", ScoreRange.Between(ScoreBound.Min, 10.0)).GetArgs()),
            () => Assert.Equal(["ZCOUNT", "key", "0", "+inf"], Request.SortedSetCount("key", ScoreRange.Between(0.0, ScoreBound.Max)).GetArgs()),
            () => Assert.Equal(["ZCOUNT", "key", "-inf", "+inf"], Request.SortedSetCount("key", ScoreRange.MinToMax).GetArgs()),
            () => Assert.Equal(["ZCOUNT", "key", "1", "10"], Request.SortedSetCount("key", ScoreRange.Between(1.0, 10.0)).GetArgs()),
            () => Assert.Equal(["ZCOUNT", "key", "(1", "10"], Request.SortedSetCount("key", ScoreRange.Between(ScoreBound.Exclusive(1.0), 10.0)).GetArgs()),
            () => Assert.Equal(["ZCOUNT", "key", "1", "(10"], Request.SortedSetCount("key", ScoreRange.Between(1.0, ScoreBound.Exclusive(10.0))).GetArgs()),
            () => Assert.Equal(["ZCOUNT", "key", "(1", "(10"], Request.SortedSetCount("key", ScoreRange.Between(ScoreBound.Exclusive(1.0), ScoreBound.Exclusive(10.0))).GetArgs()),
            () => Assert.Equal(["ZCOUNT", "key", "0", "0"], Request.SortedSetCount("key", ScoreRange.Between(0.0, 0.0)).GetArgs()),
            () => Assert.Equal(["ZCOUNT", "key", "(0", "(0"], Request.SortedSetCount("key", ScoreRange.Between(ScoreBound.Exclusive(0.0), ScoreBound.Exclusive(0.0))).GetArgs()),
            () => Assert.Equal(["ZCOUNT", "mykey", "1", "10"], Request.SortedSetCount("mykey", ScoreRange.Between(1.0, 10.0)).GetArgs()),
            () => Assert.Equal(["ZCOUNT", "test:sorted:set", "1", "10"], Request.SortedSetCount("test:sorted:set", ScoreRange.Between(1.0, 10.0)).GetArgs()),

            // SortedSetUnion/Inter/Diff
            () => Assert.Equal(["ZUNION", "2", "key1", "key2"], Request.SortedSetUnion(["key1", "key2"]).GetArgs()),
            () => Assert.Equal(["ZUNION", "3", "key1", "key2", "key3", "WEIGHTS", "1", "2", "3"], Request.SortedSetUnion(new Dictionary<ValkeyKey, double> { ["key1"] = 1.0, ["key2"] = 2.0, ["key3"] = 3.0 }).GetArgs()),
            () => Assert.Equal(["ZUNION", "2", "key1", "key2", "AGGREGATE", "MAX"], Request.SortedSetUnion(["key1", "key2"], Aggregate.Max).GetArgs()),
            () => Assert.Equal(["ZUNION", "2", "key1", "key2", "WEIGHTS", "1.5", "2.5", "AGGREGATE", "MIN"], Request.SortedSetUnion(new Dictionary<ValkeyKey, double> { ["key1"] = 1.5, ["key2"] = 2.5 }, Aggregate.Min).GetArgs()),
            () => Assert.Equal(["ZUNION", "2", "key1", "key2", "WITHSCORES"], Request.SortedSetUnionWithScore(["key1", "key2"]).GetArgs()),
            () => Assert.Equal(["ZUNION", "2", "key1", "key2", "WEIGHTS", "1", "2", "WITHSCORES"], Request.SortedSetUnionWithScore(new Dictionary<ValkeyKey, double> { ["key1"] = 1.0, ["key2"] = 2.0 }).GetArgs()),
            () => Assert.Equal(["ZINTER", "2", "key1", "key2"], Request.SortedSetInter(["key1", "key2"]).GetArgs()),
            () => Assert.Equal(["ZINTER", "2", "key1", "key2", "AGGREGATE", "MAX"], Request.SortedSetInter(["key1", "key2"], Aggregate.Max).GetArgs()),
            () => Assert.Equal(["ZINTER", "2", "key1", "key2", "WEIGHTS", "2", "3"], Request.SortedSetInter(new Dictionary<ValkeyKey, double> { ["key1"] = 2.0, ["key2"] = 3.0 }).GetArgs()),
            () => Assert.Equal(["ZINTER", "2", "key1", "key2", "WITHSCORES"], Request.SortedSetInterWithScore(["key1", "key2"]).GetArgs()),
            () => Assert.Equal(["ZINTER", "2", "key1", "key2", "WEIGHTS", "1", "2", "AGGREGATE", "MAX", "WITHSCORES"], Request.SortedSetInterWithScore(new Dictionary<ValkeyKey, double> { ["key1"] = 1.0, ["key2"] = 2.0 }, Aggregate.Max).GetArgs()),
            () => Assert.Equal(["ZDIFF", "2", "key1", "key2"], Request.SortedSetDiff(["key1", "key2"]).GetArgs()),
            () => Assert.Equal(["ZDIFF", "3", "key1", "key2", "key3"], Request.SortedSetDiff(["key1", "key2", "key3"]).GetArgs()),
            () => Assert.Equal(["ZDIFF", "2", "key1", "key2", "WITHSCORES"], Request.SortedSetDiffWithScore(["key1", "key2"]).GetArgs()),

            // SortedSetUnion/Inter/Diff AndStore
            () => Assert.Equal(["ZUNIONSTORE", "dest", "2", "key1", "key2"], Request.SortedSetUnionAndStore("dest", ["key1", "key2"]).GetArgs()),
            () => Assert.Equal(["ZUNIONSTORE", "dest", "3", "key1", "key2", "key3", "WEIGHTS", "1", "2", "3"], Request.SortedSetUnionAndStore("dest", new Dictionary<ValkeyKey, double> { ["key1"] = 1.0, ["key2"] = 2.0, ["key3"] = 3.0 }).GetArgs()),
            () => Assert.Equal(["ZUNIONSTORE", "dest", "2", "key1", "key2", "AGGREGATE", "MIN"], Request.SortedSetUnionAndStore("dest", ["key1", "key2"], Aggregate.Min).GetArgs()),
            () => Assert.Equal(["ZINTERSTORE", "dest", "2", "key1", "key2"], Request.SortedSetInterAndStore("dest", ["key1", "key2"]).GetArgs()),
            () => Assert.Equal(["ZINTERSTORE", "dest", "2", "key1", "key2", "AGGREGATE", "MIN"], Request.SortedSetInterAndStore("dest", ["key1", "key2"], Aggregate.Min).GetArgs()),
            () => Assert.Equal(["ZINTERSTORE", "dest", "2", "key1", "key2", "WEIGHTS", "2", "3"], Request.SortedSetInterAndStore("dest", new Dictionary<ValkeyKey, double> { ["key1"] = 2.0, ["key2"] = 3.0 }).GetArgs()),
            () => Assert.Equal(["ZDIFFSTORE", "dest", "2", "key1", "key2"], Request.SortedSetDiffAndStore("dest", ["key1", "key2"]).GetArgs()),

            // SortedSetIncrementBy (convenience, uses ZADD INCR under the hood)
            () => Assert.Equal(["ZADD", "key", "INCR", "2.5", "member"], Request.SortedSetIncrementBy("key", "member", 2.5, new SortedSetAddOptions()).GetArgs()),
            () => Assert.Equal(["ZADD", "key", "INCR", "-1.5", "member"], Request.SortedSetIncrementBy("key", "member", -1.5, new SortedSetAddOptions()).GetArgs()),
            () => Assert.Equal(["ZADD", "key", "INCR", "0", "member"], Request.SortedSetIncrementBy("key", "member", 0.0, new SortedSetAddOptions()).GetArgs()),

            // SortedSetInterCard
            () => Assert.Equal(["ZINTERCARD", "2", "key1", "key2"], Request.SortedSetInterCard(["key1", "key2"]).GetArgs()),
            () => Assert.Equal(["ZINTERCARD", "3", "key1", "key2", "key3"], Request.SortedSetInterCard(["key1", "key2", "key3"]).GetArgs()),
            () => Assert.Equal(["ZINTERCARD", "2", "key1", "key2", "LIMIT", "10"], Request.SortedSetInterCard(["key1", "key2"], 10).GetArgs()),

            // SortedSetLexCount
            () => Assert.Equal(["ZLEXCOUNT", "key", "[a", "[z"], Request.SortedSetLexCount("key", LexRange.Between("a", "z")).GetArgs()),
            () => Assert.Equal(["ZLEXCOUNT", "key", "(a", "(z"], Request.SortedSetLexCount("key", LexRange.Between(LexBound.Exclusive("a"), LexBound.Exclusive("z"))).GetArgs()),
            () => Assert.Equal(["ZLEXCOUNT", "key", "(a", "[z"], Request.SortedSetLexCount("key", LexRange.Between(LexBound.Exclusive("a"), "z")).GetArgs()),
            () => Assert.Equal(["ZLEXCOUNT", "key", "[a", "(z"], Request.SortedSetLexCount("key", LexRange.Between("a", LexBound.Exclusive("z"))).GetArgs()),
            () => Assert.Equal(["ZLEXCOUNT", "key", "-", "+"], Request.SortedSetLexCount("key", LexRange.MinToMax).GetArgs()),

            // SortedSetRangeAsync - Rank
            () => Assert.Equal(["ZRANGE", "key", "0", "-1"], Request.SortedSetRange("key", new() { Range = IndexRange.FirstToLast }).GetArgs()),
            () => Assert.Equal(["ZRANGE", "key", "1", "3"], Request.SortedSetRange("key", new() { Range = IndexRange.Between(1, 3) }).GetArgs()),
            () => Assert.Equal(["ZRANGE", "key", "0", "-1", "REV"], Request.SortedSetRange("key", new() { Range = IndexRange.FirstToLast, Order = Order.Descending }).GetArgs()),
            () => Assert.Throws<ArgumentException>(() => Request.SortedSetRange("key", new() { Range = IndexRange.FirstToLast, Offset = 2, Count = 3 }).GetArgs()),
            () => Assert.Throws<ArgumentException>(() => Request.SortedSetRange("key", new() { Range = IndexRange.FirstToLast, Order = Order.Descending, Offset = 1, Count = 5 }).GetArgs()),

            // SortedSetRangeAsync - Score range
            () => Assert.Equal(["ZRANGE", "key", "-inf", "+inf", "BYSCORE"], Request.SortedSetRange("key", new() { Range = ScoreRange.MinToMax }).GetArgs()),
            () => Assert.Equal(["ZRANGE", "key", "1", "10", "BYSCORE"], Request.SortedSetRange("key", new() { Range = ScoreRange.Between(1.0, 10.0) }).GetArgs()),
            () => Assert.Equal(["ZRANGE", "key", "1", "10", "BYSCORE", "LIMIT", "2", "3"], Request.SortedSetRange("key", new() { Range = ScoreRange.Between(1.0, 10.0), Offset = 2, Count = 3 }).GetArgs()),
            () => Assert.Equal(["ZRANGE", "key", "-inf", "+inf", "BYSCORE", "REV"], Request.SortedSetRange("key", new() { Range = ScoreRange.MinToMax, Order = Order.Descending }).GetArgs()),

            // SortedSetRangeAsync - Lex range
            () => Assert.Equal(["ZRANGE", "key", "-", "+", "BYLEX"], Request.SortedSetRange("key", new() { Range = LexRange.MinToMax }).GetArgs()),
            () => Assert.Equal(["ZRANGE", "key", "[a", "[z", "BYLEX"], Request.SortedSetRange("key", new() { Range = LexRange.Between("a", "z") }).GetArgs()),
            () => Assert.Equal(["ZRANGE", "key", "[a", "[z", "BYLEX", "LIMIT", "1", "5"], Request.SortedSetRange("key", new() { Range = LexRange.Between("a", "z"), Offset = 1, Count = 5 }).GetArgs()),
            () => Assert.Equal(["ZRANGE", "key", "-", "+", "BYLEX", "REV", "LIMIT", "2", "3"], Request.SortedSetRange("key", new() { Range = LexRange.MinToMax, Order = Order.Descending, Offset = 2, Count = 3 }).GetArgs()),
            () => Assert.Equal(["ZRANGE", "key", "-", "+", "BYLEX", "LIMIT", "5", "-1"], Request.SortedSetRange("key", new() { Range = LexRange.MinToMax, Offset = 5 }).GetArgs()),
            () => Assert.Equal(["ZRANGE", "key", "-", "+", "BYLEX", "LIMIT", "0", "10"], Request.SortedSetRange("key", new() { Range = LexRange.MinToMax, Count = 10 }).GetArgs()),

            // SortedSetScores
            () => Assert.Equal(["ZMSCORE", "key", "member1"], Request.SortedSetScores("key", ["member1"]).GetArgs()),
            () => Assert.Equal(["ZMSCORE", "key", "member1", "member2", "member3"], Request.SortedSetScores("key", ["member1", "member2", "member3"]).GetArgs()),
            () => Assert.Equal(["ZMSCORE", "key"], Request.SortedSetScores("key", []).GetArgs()),

            // SortedSetPopMin / SortedSetPopMax
            () => Assert.Equal(["ZPOPMIN", "key"], Request.SortedSetPopMin("key").GetArgs()),
            () => Assert.Equal(["ZPOPMAX", "key"], Request.SortedSetPopMax("key").GetArgs()),
            () => Assert.Equal(["ZPOPMIN", "key", "3"], Request.SortedSetPopMin("key", 3).GetArgs()),
            () => Assert.Equal(["ZPOPMAX", "key", "5"], Request.SortedSetPopMax("key", 5).GetArgs()),

            // SortedSetPopMin / SortedSetPopMax - Multi-key (ZMPOP)
            () => Assert.Equal(["ZMPOP", "2", "key1", "key2", "MIN", "COUNT", "1"], Request.SortedSetPopMin(["key1", "key2"]).GetArgs()),
            () => Assert.Equal(["ZMPOP", "2", "key1", "key2", "MAX", "COUNT", "1"], Request.SortedSetPopMax(["key1", "key2"]).GetArgs()),
            () => Assert.Equal(["ZMPOP", "2", "key1", "key2", "MIN", "COUNT", "3"], Request.SortedSetPopMin(["key1", "key2"], 3).GetArgs()),
            () => Assert.Equal(["ZMPOP", "2", "key1", "key2", "MAX", "COUNT", "5"], Request.SortedSetPopMax(["key1", "key2"], 5).GetArgs()),

            // SortedSetPopMin / SortedSetPopMax - Blocking multi-key (BZMPOP)
            () => Assert.Equal(["BZMPOP", "2", "2", "key1", "key2", "MIN", "COUNT", "1"], Request.SortedSetPopMin(["key1", "key2"], TimeSpan.FromSeconds(2)).GetArgs()),
            () => Assert.Equal(["BZMPOP", "2", "2", "key1", "key2", "MAX", "COUNT", "1"], Request.SortedSetPopMax(["key1", "key2"], TimeSpan.FromSeconds(2)).GetArgs()),
            () => Assert.Equal(["BZMPOP", "5", "1", "key1", "MIN", "COUNT", "3"], Request.SortedSetPopMin(["key1"], 3, TimeSpan.FromSeconds(5)).GetArgs()),
            () => Assert.Equal(["BZMPOP", "5", "1", "key1", "MAX", "COUNT", "3"], Request.SortedSetPopMax(["key1"], 3, TimeSpan.FromSeconds(5)).GetArgs()),

            // SortedSetRandomMember
            () => Assert.Equal(["ZRANDMEMBER", "key"], Request.SortedSetRandomMember("key").GetArgs()),
            () => Assert.Equal(["ZRANDMEMBER", "key", "3"], Request.SortedSetRandomMembers("key", 3).GetArgs()),
            () => Assert.Equal(["ZRANDMEMBER", "key", "5", "WITHSCORES"], Request.SortedSetRandomMembersWithScores("key", 5).GetArgs()),
            () => Assert.Equal(["ZRANDMEMBER", "key", "1", "WITHSCORES"], Request.SortedSetRandomMemberWithScore("key").GetArgs()),

            // SortedSetRemoveRange - by rank
            () => Assert.Equal(["ZREMRANGEBYRANK", "key", "0", "3"], Request.SortedSetRemoveRange("key", IndexRange.Between(0, 3)).GetArgs()),
            () => Assert.Equal(["ZREMRANGEBYRANK", "key", "0", "-1"], Request.SortedSetRemoveRange("key", IndexRange.FirstToLast).GetArgs()),
            // SortedSetRemoveRange - by score
            () => Assert.Equal(["ZREMRANGEBYSCORE", "key", "1", "10"], Request.SortedSetRemoveRange("key", ScoreRange.Between(1.0, 10.0)).GetArgs()),
            () => Assert.Equal(["ZREMRANGEBYSCORE", "key", "(1", "(10"], Request.SortedSetRemoveRange("key", ScoreRange.Between(ScoreBound.Exclusive(1.0), ScoreBound.Exclusive(10.0))).GetArgs()),
            () => Assert.Equal(["ZREMRANGEBYSCORE", "key", "-inf", "+inf"], Request.SortedSetRemoveRange("key", ScoreRange.MinToMax).GetArgs()),
            // SortedSetRemoveRange - by lex
            () => Assert.Equal(["ZREMRANGEBYLEX", "key", "[a", "[z"], Request.SortedSetRemoveRange("key", LexRange.Between("a", "z")).GetArgs()),
            () => Assert.Equal(["ZREMRANGEBYLEX", "key", "(a", "(z"], Request.SortedSetRemoveRange("key", LexRange.Between(LexBound.Exclusive("a"), LexBound.Exclusive("z"))).GetArgs()),

            // SortedSetRangeAndStore
            () => Assert.Equal(["ZRANGESTORE", "dest", "src", "0", "-1"], Request.SortedSetRangeAndStore("src", "dest", new() { Range = IndexRange.FirstToLast }).GetArgs()),
            () => Assert.Equal(["ZRANGESTORE", "dest", "src", "1", "3"], Request.SortedSetRangeAndStore("src", "dest", new() { Range = IndexRange.Between(1, 3) }).GetArgs()),
            () => Assert.Equal(["ZRANGESTORE", "dest", "src", "-inf", "+inf", "BYSCORE"], Request.SortedSetRangeAndStore("src", "dest", new() { Range = ScoreRange.MinToMax }).GetArgs()),
            () => Assert.Equal(["ZRANGESTORE", "dest", "src", "[a", "[z", "BYLEX"], Request.SortedSetRangeAndStore("src", "dest", new() { Range = LexRange.Between("a", "z") }).GetArgs()),

            // SortedSetRank
            () => Assert.Equal(["ZRANK", "key", "member"], Request.SortedSetRank("key", "member").GetArgs()),
            () => Assert.Equal(["ZRANK", "key", "member"], Request.SortedSetRank("key", "member", Order.Ascending).GetArgs()),
            () => Assert.Equal(["ZREVRANK", "key", "member"], Request.SortedSetRank("key", "member", Order.Descending).GetArgs()),

            // SortedSetRankWithScore
            () => Assert.Equal(["ZRANK", "key", "member", "WITHSCORE"], Request.SortedSetRankWithScore("key", "member").GetArgs()),
            () => Assert.Equal(["ZRANK", "key", "member", "WITHSCORE"], Request.SortedSetRankWithScore("key", "member", Order.Ascending).GetArgs()),
            () => Assert.Equal(["ZREVRANK", "key", "member", "WITHSCORE"], Request.SortedSetRankWithScore("key", "member", Order.Descending).GetArgs()),

            // SortedSetScan
            () => Assert.Equal(["ZSCAN", "key", "0"], Request.SortedSetScan("key", 0).GetArgs()),
            () => Assert.Equal(["ZSCAN", "key", "5"], Request.SortedSetScan("key", 5).GetArgs()),
            () => Assert.Equal(["ZSCAN", "key", "0", "MATCH", "pattern*"], Request.SortedSetScan("key", 0, new ScanOptions { MatchPattern = "pattern*" }).GetArgs()),
            () => Assert.Equal(["ZSCAN", "key", "0", "COUNT", "20"], Request.SortedSetScan("key", 0, new ScanOptions { Count = 20 }).GetArgs()),
            () => Assert.Equal(["ZSCAN", "key", "5", "MATCH", "pattern*", "COUNT", "20"], Request.SortedSetScan("key", 5, new ScanOptions { MatchPattern = "pattern*", Count = 20 }).GetArgs()),
            () => Assert.Equal(["ZSCAN", "key", "10", "MATCH", "user:*", "COUNT", "50"], Request.SortedSetScan("key", 10, new ScanOptions { MatchPattern = "user:*", Count = 50 }).GetArgs()),
            () => Assert.Equal(["ZSCAN", "key", "0", "MATCH", "*"], Request.SortedSetScan("key", 0, new ScanOptions { MatchPattern = "*" }).GetArgs()),
            () => Assert.Equal(["ZSCAN", "key", "100"], Request.SortedSetScan("key", 100).GetArgs()),

            // SortedSetScore
            () => Assert.Equal(["ZSCORE", "key", "member"], Request.SortedSetScore("key", "member").GetArgs())
        );

    [Fact]
    public void SortedSetCommands_ValidateConverters() => Assert.Multiple(
            // Basic converter tests
            () => Assert.True(Request.SortedSetAdd("key", "member", 10.5).Converter(1L)),
            () => Assert.False(Request.SortedSetAdd("key", "member", 10.5).Converter(0L)),
            () => Assert.True(Request.SortedSetRemove("key", "member").Converter(1L)),
            () => Assert.False(Request.SortedSetRemove("key", "member").Converter(0L)),
            () => Assert.Equal(2L, Request.SortedSetRemove("key", ["member1", "member2"]).Converter(2L)),
            () => Assert.Equal(5L, Request.SortedSetCard("key").Converter(5L)),
            () => Assert.Equal(3L, Request.SortedSetCount("key", ScoreRange.Between(1.0, 10.0)).Converter(3L)),
            () => Assert.Equal(0L, Request.SortedSetCount("key", ScoreRange.MinToMax).Converter(0L)),

            // Type converter test
            () => Assert.Equal(ValkeyType.SortedSet, Request.Type("key").Converter("zset"))
        );

    [Fact]
    public void SortedSetCommands_ValidateArrayConverters()
    {
        // Test data for score-based converters
        Dictionary<GlideString, object> testScoreDict = new()
        {
            {"member1", 10.5},
            {"member2", 8.25},
            {"member3", 15.0}
        };

        Assert.Multiple(
            // Test SortedSetIncrementByAsync converter
            () =>
            {
                double? result = Request.SortedSetIncrementBy("key", "member", 2.5, new SortedSetAddOptions()).Converter(12.5);
                Assert.Equal(12.5, result);
            },

            // Test SortedSetUnionWithScoreAsync converter
            () =>
            {
                SortedSetEntry[] result = Request.SortedSetUnionWithScore(["key1", "key2"]).Converter(testScoreDict);
                Assert.Equal(3, result.Length);
                Assert.Contains(result, r => r.Element == "member1" && r.Score == 10.5);
                Assert.Contains(result, r => r.Element == "member2" && r.Score == 8.25);
                Assert.Contains(result, r => r.Element == "member3" && r.Score == 15.0);
            },

            // Test SortedSetScoresAsync converter
            () =>
            {
                object[] testScoresResponse = [10.5, null!, 8.25];
                double?[] result = Request.SortedSetScores("key", ["member1", "member2", "member3"]).Converter(testScoresResponse);
                Assert.Equal(3, result.Length);
                Assert.Equal(10.5, result[0]);
                Assert.Null(result[1]);
                Assert.Equal(8.25, result[2]);
            },

            // Test SortedSetPopMinAsync converter - single element
            () =>
            {
                Dictionary<gs, object> testDict = new()
                {
                    { (gs)"member1", 8.25 }
                };
                SortedSetEntry? result = Request.SortedSetPopMin("key").Converter(testDict);
                _ = Assert.NotNull(result);
                Assert.Equal("member1", result.Value.Element);
                Assert.Equal(8.25, result.Value.Score);
            },

            // Test SortedSetPopMinAsync converter - null result
            () =>
            {
                SortedSetEntry? result = Request.SortedSetPopMin("key").Converter(null);
                Assert.Null(result);
            },

            // Test SortedSetPopMaxAsync converter - single element
            () =>
            {
                Dictionary<gs, object> testDict = new()
                {
                    { (gs)"member1", 10.5 }
                };
                SortedSetEntry? result = Request.SortedSetPopMax("key").Converter(testDict);
                _ = Assert.NotNull(result);
                Assert.Equal("member1", result.Value.Element);
                Assert.Equal(10.5, result.Value.Score);
            },

            // Test SortedSetPopMinAsync converter - multiple elements
            () =>
            {
                Dictionary<gs, object> testDict = new()
                {
                    { (gs)"member1", 5.0 },
                    { (gs)"member2", 8.25 }
                };
                SortedSetEntry[] result = Request.SortedSetPopMin("key", 2).Converter(testDict);
                Assert.Equal(2, result.Length);
                SortedSetEntry member1Entry = result.First(e => e.Element.ToString() == "member1");
                Assert.Equal(5.0, member1Entry.Score);
            },

            // Test SortedSetPopMaxAsync converter - multiple elements
            () =>
            {
                Dictionary<gs, object> testDict = new()
                {
                    { (gs)"member1", 10.5 },
                    { (gs)"member2", 8.25 }
                };
                SortedSetEntry[] result = Request.SortedSetPopMax("key", 2).Converter(testDict);
                Assert.Equal(2, result.Length);
                SortedSetEntry member1Entry = result.First(e => e.Element.ToString() == "member1");
                Assert.Equal(10.5, member1Entry.Score);
            },

            // Test SortedSetRandomMemberAsync converter
            () =>
            {
                ValkeyValue result = Request.SortedSetRandomMember("key").Converter(null);
                Assert.Equal(ValkeyValue.Null, result);
            },

            // Test SortedSetRandomMembersAsync converter
            () =>
            {
                object[] testRandomResponse = [(gs)"member1", (gs)"member2"];
                ValkeyValue[] result = Request.SortedSetRandomMembers("key", 2).Converter(testRandomResponse);
                Assert.Equal(2, result.Length);
                Assert.Equal("member1", result[0]);
                Assert.Equal("member2", result[1]);
            },

            // Test SortedSetRankAsync converter
            () =>
            {
                long? result = Request.SortedSetRank("key", "member").Converter(3L);
                Assert.Equal(3L, result);
            },

            // Test SortedSetRankAsync converter - null result
            () =>
            {
                long? result = Request.SortedSetRank("key", "member").Converter(null);
                Assert.Null(result);
            },

            // Test SortedSetRankWithScoreAsync converter
            () =>
            {
                object[] response = [2L, 10.5];
                (long Rank, double Score)? result = Request.SortedSetRankWithScore("key", "member").Converter(response);
                _ = Assert.NotNull(result);
                Assert.Equal(2L, result.Value.Rank);
                Assert.Equal(10.5, result.Value.Score);
            },

            // Test SortedSetRankWithScoreAsync converter - null result
            () =>
            {
                (long Rank, double Score)? result = Request.SortedSetRankWithScore("key", "member").Converter([]);
                Assert.Null(result);
            },

            // Test SortedSetScanAsync converter - basic case
            () =>
            {
                object[] testScanResponse = [
                    5L,
                    new object[] { (gs)"member1", (gs)"10.5", (gs)"member2", (gs)"8.25" }
                ];
                (long cursor, SortedSetEntry[] items) = Request.SortedSetScan("key", 0).Converter(testScanResponse);
                Assert.Equal(5L, cursor);
                Assert.Equal(2, items.Length);
                Assert.Equal("member1", items[0].Element);
                Assert.Equal(10.5, items[0].Score);
                Assert.Equal("member2", items[1].Element);
                Assert.Equal(8.25, items[1].Score);
            },

            // Test SortedSetScanAsync converter - empty result
            () =>
            {
                object[] testScanResponse = [
                    0L,
                    new object[] { }
                ];
                (long cursor, SortedSetEntry[] items) = Request.SortedSetScan("key", 0).Converter(testScanResponse);
                Assert.Equal(0L, cursor);
                Assert.Empty(items);
            },

            // Test SortedSetScanAsync converter - single entry
            () =>
            {
                object[] testScanResponse = [
                    10L,
                    new object[] { (gs)"single", (gs)"42.0" }
                ];
                (long cursor, SortedSetEntry[] items) = Request.SortedSetScan("key", 0).Converter(testScanResponse);
                Assert.Equal(10L, cursor);
                _ = Assert.Single(items);
                Assert.Equal("single", items[0].Element);
                Assert.Equal(42.0, items[0].Score);
            },

            // Test SortedSetScanAsync converter - cursor as GlideString
            () =>
            {
                object[] testScanResponse = [
                    (gs)"15",
                    new object[] { (gs)"test", (gs)"1.5" }
                ];
                (long cursor, SortedSetEntry[] items) = Request.SortedSetScan("key", 0).Converter(testScanResponse);
                Assert.Equal(15L, cursor);
                _ = Assert.Single(items);
                Assert.Equal("test", items[0].Element);
                Assert.Equal(1.5, items[0].Score);
            },

            // Test SortedSetScoreAsync converter
            () =>
            {
                double? result = Request.SortedSetScore("key", "member").Converter(10.5);
                Assert.Equal(10.5, result);
            },

            // Test SortedSetScoreAsync converter - null result
            () =>
            {
                double? result = Request.SortedSetScore("key", "member").Converter(null);
                Assert.Null(result);
            }
        );
    }
}
