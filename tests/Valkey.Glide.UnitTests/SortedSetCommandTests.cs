// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands.Options;

using static Valkey.Glide.Internals.FFI;

namespace Valkey.Glide.UnitTests;

public class SortedSetCommandTests
{
    [Fact]
    public void SortedSetCommands_ValidateArguments() => Assert.Multiple(
        // SortedSetAdd - Single member
        () => CmdAssert.Equal(RequestType.ZAdd, ["key", "10.5", "member"], Request.SortedSetAdd("key", "member", 10.5)),
        () => CmdAssert.Equal(RequestType.ZAdd, ["key", "NX", "10.5", "member"], Request.SortedSetAdd("key", "member", 10.5, new SortedSetAddOptions { Condition = SortedSetAddCondition.OnlyIfNotExists })),
        () => CmdAssert.Equal(RequestType.ZAdd, ["key", "XX", "10.5", "member"], Request.SortedSetAdd("key", "member", 10.5, new SortedSetAddOptions { Condition = SortedSetAddCondition.OnlyIfExists })),
        () => CmdAssert.Equal(RequestType.ZAdd, ["key", "GT", "10.5", "member"], Request.SortedSetAdd("key", "member", 10.5, new SortedSetAddOptions { Condition = SortedSetAddCondition.OnlyIfNotExistsOrGreaterThan })),
        () => CmdAssert.Equal(RequestType.ZAdd, ["key", "LT", "10.5", "member"], Request.SortedSetAdd("key", "member", 10.5, new SortedSetAddOptions { Condition = SortedSetAddCondition.OnlyIfNotExistsOrLessThan })),
        () => CmdAssert.Equal(RequestType.ZAdd, ["key", "XX", "GT", "10.5", "member"], Request.SortedSetAdd("key", "member", 10.5, new SortedSetAddOptions { Condition = SortedSetAddCondition.OnlyIfGreaterThan })),
        () => CmdAssert.Equal(RequestType.ZAdd, ["key", "XX", "LT", "10.5", "member"], Request.SortedSetAdd("key", "member", 10.5, new SortedSetAddOptions { Condition = SortedSetAddCondition.OnlyIfLessThan })),
        () => CmdAssert.Equal(RequestType.ZAdd, ["key", "CH", "10.5", "member"], Request.SortedSetAdd("key", "member", 10.5, new SortedSetAddOptions { Changed = true })),
        () => CmdAssert.Equal(RequestType.ZAdd, ["key", "NX", "CH", "10.5", "member"], Request.SortedSetAdd("key", "member", 10.5, new SortedSetAddOptions { Condition = SortedSetAddCondition.OnlyIfNotExists, Changed = true })),

        // SortedSetAdd - Multiple members
        () => CmdAssert.Equal(RequestType.ZAdd, ["key", "10.5", "member1", "8.25", "member2"], Request.SortedSetAdd("key", [new SortedSetEntry("member1", 10.5), new SortedSetEntry("member2", 8.25)])),
        () => CmdAssert.Equal(RequestType.ZAdd, ["key", "NX", "10.5", "member1", "8.25", "member2"], Request.SortedSetAdd("key", [new SortedSetEntry("member1", 10.5), new SortedSetEntry("member2", 8.25)], new SortedSetAddOptions { Condition = SortedSetAddCondition.OnlyIfNotExists })),
        () => CmdAssert.Equal(RequestType.ZAdd, ["key", "XX", "10.5", "member1", "8.25", "member2"], Request.SortedSetAdd("key", [new SortedSetEntry("member1", 10.5), new SortedSetEntry("member2", 8.25)], new SortedSetAddOptions { Condition = SortedSetAddCondition.OnlyIfExists })),

        // SortedSetIncrementBy with options (ZADD INCR)
        () => CmdAssert.Equal(RequestType.ZAdd, ["key", "NX", "INCR", "5", "member"], Request.SortedSetIncrementBy("key", "member", 5.0, new SortedSetAddOptions { Condition = SortedSetAddCondition.OnlyIfNotExists })),
        () => CmdAssert.Equal(RequestType.ZAdd, ["key", "XX", "GT", "INCR", "5", "member"], Request.SortedSetIncrementBy("key", "member", 5.0, new SortedSetAddOptions { Condition = SortedSetAddCondition.OnlyIfGreaterThan })),

        // SortedSetRemove - Single Member
        () => CmdAssert.Equal(RequestType.ZRem, ["key", "member"], Request.SortedSetRemove("key", "member")),

        // SortedSetRemove - Multiple Members
        () => CmdAssert.Equal(RequestType.ZRem, ["key", "member1", "member2", "member3"], Request.SortedSetRemove("key", ["member1", "member2", "member3"])),
        () => CmdAssert.Equal(RequestType.ZRem, ["key"], Request.SortedSetRemove("key", [])),
        () => CmdAssert.Equal(RequestType.ZRem, ["key", "", " ", "null", "0", "-1"], Request.SortedSetRemove("key", ["", " ", "null", "0", "-1"])),

        // SortedSetCard
        () => CmdAssert.Equal(RequestType.ZCard, ["key"], Request.SortedSetCard("key")),
        () => CmdAssert.Equal(RequestType.ZCard, ["mykey"], Request.SortedSetCard("mykey")),
        () => CmdAssert.Equal(RequestType.ZCard, ["test:sorted:set"], Request.SortedSetCard("test:sorted:set")),
        () => CmdAssert.Equal(RequestType.ZCard, [""], Request.SortedSetCard("")),

        // SortedSetCount
        () => CmdAssert.Equal(RequestType.ZCount, ["key", "-inf", "+inf"], Request.SortedSetCount("key", ScoreRange.MinToMax)),
        () => CmdAssert.Equal(RequestType.ZCount, ["key", "1", "10"], Request.SortedSetCount("key", ScoreRange.Between(1.0, 10.0))),
        () => CmdAssert.Equal(RequestType.ZCount, ["key", "0", "100"], Request.SortedSetCount("key", ScoreRange.Between(0.0, 100.0))),
        () => CmdAssert.Equal(RequestType.ZCount, ["key", "-5", "5"], Request.SortedSetCount("key", ScoreRange.Between(-5.0, 5.0))),
        () => CmdAssert.Equal(RequestType.ZCount, ["key", "1.5", "9.75"], Request.SortedSetCount("key", ScoreRange.Between(1.5, 9.75))),
        () => CmdAssert.Equal(RequestType.ZCount, ["key", "0.10000000000000001", "0.90000000000000002"], Request.SortedSetCount("key", ScoreRange.Between(0.1, 0.9))),
        () => CmdAssert.Equal(RequestType.ZCount, ["key", "-inf", "10"], Request.SortedSetCount("key", ScoreRange.Between(ScoreBound.Min, 10.0))),
        () => CmdAssert.Equal(RequestType.ZCount, ["key", "0", "+inf"], Request.SortedSetCount("key", ScoreRange.Between(0.0, ScoreBound.Max))),
        () => CmdAssert.Equal(RequestType.ZCount, ["key", "-inf", "+inf"], Request.SortedSetCount("key", ScoreRange.MinToMax)),
        () => CmdAssert.Equal(RequestType.ZCount, ["key", "1", "10"], Request.SortedSetCount("key", ScoreRange.Between(1.0, 10.0))),
        () => CmdAssert.Equal(RequestType.ZCount, ["key", "(1", "10"], Request.SortedSetCount("key", ScoreRange.Between(ScoreBound.Exclusive(1.0), 10.0))),
        () => CmdAssert.Equal(RequestType.ZCount, ["key", "1", "(10"], Request.SortedSetCount("key", ScoreRange.Between(1.0, ScoreBound.Exclusive(10.0)))),
        () => CmdAssert.Equal(RequestType.ZCount, ["key", "(1", "(10"], Request.SortedSetCount("key", ScoreRange.Between(ScoreBound.Exclusive(1.0), ScoreBound.Exclusive(10.0)))),
        () => CmdAssert.Equal(RequestType.ZCount, ["key", "0", "0"], Request.SortedSetCount("key", ScoreRange.Between(0.0, 0.0))),
        () => CmdAssert.Equal(RequestType.ZCount, ["key", "(0", "(0"], Request.SortedSetCount("key", ScoreRange.Between(ScoreBound.Exclusive(0.0), ScoreBound.Exclusive(0.0)))),
        () => CmdAssert.Equal(RequestType.ZCount, ["mykey", "1", "10"], Request.SortedSetCount("mykey", ScoreRange.Between(1.0, 10.0))),
        () => CmdAssert.Equal(RequestType.ZCount, ["test:sorted:set", "1", "10"], Request.SortedSetCount("test:sorted:set", ScoreRange.Between(1.0, 10.0))),

        // SortedSetUnion/Inter/Diff
        () => CmdAssert.Equal(RequestType.ZUnion, ["2", "key1", "key2"], Request.SortedSetUnion(["key1", "key2"])),
        () => CmdAssert.Equal(RequestType.ZUnion, ["3", "key1", "key2", "key3", "WEIGHTS", "1", "2", "3"], Request.SortedSetUnion(new Dictionary<ValkeyKey, double> { ["key1"] = 1.0, ["key2"] = 2.0, ["key3"] = 3.0 })),
        () => CmdAssert.Equal(RequestType.ZUnion, ["2", "key1", "key2", "AGGREGATE", "MAX"], Request.SortedSetUnion(["key1", "key2"], Aggregate.Max)),
        () => CmdAssert.Equal(RequestType.ZUnion, ["2", "key1", "key2", "WEIGHTS", "1.5", "2.5", "AGGREGATE", "MIN"], Request.SortedSetUnion(new Dictionary<ValkeyKey, double> { ["key1"] = 1.5, ["key2"] = 2.5 }, Aggregate.Min)),
        () => CmdAssert.Equal(RequestType.ZUnion, ["2", "key1", "key2", "WITHSCORES"], Request.SortedSetUnionWithScore(["key1", "key2"])),
        () => CmdAssert.Equal(RequestType.ZUnion, ["2", "key1", "key2", "WEIGHTS", "1", "2", "WITHSCORES"], Request.SortedSetUnionWithScore(new Dictionary<ValkeyKey, double> { ["key1"] = 1.0, ["key2"] = 2.0 })),
        () => CmdAssert.Equal(RequestType.ZInter, ["2", "key1", "key2"], Request.SortedSetInter(["key1", "key2"])),
        () => CmdAssert.Equal(RequestType.ZInter, ["2", "key1", "key2", "AGGREGATE", "MAX"], Request.SortedSetInter(["key1", "key2"], Aggregate.Max)),
        () => CmdAssert.Equal(RequestType.ZInter, ["2", "key1", "key2", "WEIGHTS", "2", "3"], Request.SortedSetInter(new Dictionary<ValkeyKey, double> { ["key1"] = 2.0, ["key2"] = 3.0 })),
        () => CmdAssert.Equal(RequestType.ZInter, ["2", "key1", "key2", "WITHSCORES"], Request.SortedSetInterWithScore(["key1", "key2"])),
        () => CmdAssert.Equal(RequestType.ZInter, ["2", "key1", "key2", "WEIGHTS", "1", "2", "AGGREGATE", "MAX", "WITHSCORES"], Request.SortedSetInterWithScore(new Dictionary<ValkeyKey, double> { ["key1"] = 1.0, ["key2"] = 2.0 }, Aggregate.Max)),
        () => CmdAssert.Equal(RequestType.ZDiff, ["2", "key1", "key2"], Request.SortedSetDiff(["key1", "key2"])),
        () => CmdAssert.Equal(RequestType.ZDiff, ["3", "key1", "key2", "key3"], Request.SortedSetDiff(["key1", "key2", "key3"])),
        () => CmdAssert.Equal(RequestType.ZDiff, ["2", "key1", "key2", "WITHSCORES"], Request.SortedSetDiffWithScore(["key1", "key2"])),

        // SortedSetUnion/Inter/Diff AndStore
        () => CmdAssert.Equal(RequestType.ZUnionStore, ["dest", "2", "key1", "key2"], Request.SortedSetUnionAndStore("dest", ["key1", "key2"])),
        () => CmdAssert.Equal(RequestType.ZUnionStore, ["dest", "3", "key1", "key2", "key3", "WEIGHTS", "1", "2", "3"], Request.SortedSetUnionAndStore("dest", new Dictionary<ValkeyKey, double> { ["key1"] = 1.0, ["key2"] = 2.0, ["key3"] = 3.0 })),
        () => CmdAssert.Equal(RequestType.ZUnionStore, ["dest", "2", "key1", "key2", "AGGREGATE", "MIN"], Request.SortedSetUnionAndStore("dest", ["key1", "key2"], Aggregate.Min)),
        () => CmdAssert.Equal(RequestType.ZInterStore, ["dest", "2", "key1", "key2"], Request.SortedSetInterAndStore("dest", ["key1", "key2"])),
        () => CmdAssert.Equal(RequestType.ZInterStore, ["dest", "2", "key1", "key2", "AGGREGATE", "MIN"], Request.SortedSetInterAndStore("dest", ["key1", "key2"], Aggregate.Min)),
        () => CmdAssert.Equal(RequestType.ZInterStore, ["dest", "2", "key1", "key2", "WEIGHTS", "2", "3"], Request.SortedSetInterAndStore("dest", new Dictionary<ValkeyKey, double> { ["key1"] = 2.0, ["key2"] = 3.0 })),
        () => CmdAssert.Equal(RequestType.ZDiffStore, ["dest", "2", "key1", "key2"], Request.SortedSetDiffAndStore("dest", ["key1", "key2"])),

        // SortedSetIncrementBy (convenience, uses ZADD INCR under the hood)
        () => CmdAssert.Equal(RequestType.ZAdd, ["key", "INCR", "2.5", "member"], Request.SortedSetIncrementBy("key", "member", 2.5, new SortedSetAddOptions())),
        () => CmdAssert.Equal(RequestType.ZAdd, ["key", "INCR", "-1.5", "member"], Request.SortedSetIncrementBy("key", "member", -1.5, new SortedSetAddOptions())),
        () => CmdAssert.Equal(RequestType.ZAdd, ["key", "INCR", "0", "member"], Request.SortedSetIncrementBy("key", "member", 0.0, new SortedSetAddOptions())),

        // SortedSetInterCard
        () => CmdAssert.Equal(RequestType.ZInterCard, ["2", "key1", "key2"], Request.SortedSetInterCard(["key1", "key2"])),
        () => CmdAssert.Equal(RequestType.ZInterCard, ["3", "key1", "key2", "key3"], Request.SortedSetInterCard(["key1", "key2", "key3"])),
        () => CmdAssert.Equal(RequestType.ZInterCard, ["2", "key1", "key2", "LIMIT", "10"], Request.SortedSetInterCard(["key1", "key2"], 10)),

        // SortedSetLexCount
        () => CmdAssert.Equal(RequestType.ZLexCount, ["key", "[a", "[z"], Request.SortedSetLexCount("key", LexRange.Between("a", "z"))),
        () => CmdAssert.Equal(RequestType.ZLexCount, ["key", "(a", "(z"], Request.SortedSetLexCount("key", LexRange.Between(LexBound.Exclusive("a"), LexBound.Exclusive("z")))),
        () => CmdAssert.Equal(RequestType.ZLexCount, ["key", "(a", "[z"], Request.SortedSetLexCount("key", LexRange.Between(LexBound.Exclusive("a"), "z"))),
        () => CmdAssert.Equal(RequestType.ZLexCount, ["key", "[a", "(z"], Request.SortedSetLexCount("key", LexRange.Between("a", LexBound.Exclusive("z")))),
        () => CmdAssert.Equal(RequestType.ZLexCount, ["key", "-", "+"], Request.SortedSetLexCount("key", LexRange.MinToMax)),

        // SortedSetRangeAsync - Rank
        () => CmdAssert.Equal(RequestType.ZRange, ["key", "0", "-1"], Request.SortedSetRange("key", new() { Range = IndexRange.FirstToLast })),
        () => CmdAssert.Equal(RequestType.ZRange, ["key", "1", "3"], Request.SortedSetRange("key", new() { Range = IndexRange.Between(1, 3) })),
        () => CmdAssert.Equal(RequestType.ZRange, ["key", "0", "-1", "REV"], Request.SortedSetRange("key", new() { Range = IndexRange.FirstToLast, Order = Order.Descending })),
        () => Assert.Throws<ArgumentException>(() => Request.SortedSetRange("key", new() { Range = IndexRange.FirstToLast, Offset = 2, Count = 3 })),
        () => Assert.Throws<ArgumentException>(() => Request.SortedSetRange("key", new() { Range = IndexRange.FirstToLast, Order = Order.Descending, Offset = 1, Count = 5 })),

        // SortedSetRangeAsync - Score range
        () => CmdAssert.Equal(RequestType.ZRange, ["key", "-inf", "+inf", "BYSCORE"], Request.SortedSetRange("key", new() { Range = ScoreRange.MinToMax })),
        () => CmdAssert.Equal(RequestType.ZRange, ["key", "1", "10", "BYSCORE"], Request.SortedSetRange("key", new() { Range = ScoreRange.Between(1.0, 10.0) })),
        () => CmdAssert.Equal(RequestType.ZRange, ["key", "1", "10", "BYSCORE", "LIMIT", "2", "3"], Request.SortedSetRange("key", new() { Range = ScoreRange.Between(1.0, 10.0), Offset = 2, Count = 3 })),
        () => CmdAssert.Equal(RequestType.ZRange, ["key", "-inf", "+inf", "BYSCORE", "REV"], Request.SortedSetRange("key", new() { Range = ScoreRange.MinToMax, Order = Order.Descending })),

        // SortedSetRangeAsync - Lex range
        () => CmdAssert.Equal(RequestType.ZRange, ["key", "-", "+", "BYLEX"], Request.SortedSetRange("key", new() { Range = LexRange.MinToMax })),
        () => CmdAssert.Equal(RequestType.ZRange, ["key", "[a", "[z", "BYLEX"], Request.SortedSetRange("key", new() { Range = LexRange.Between("a", "z") })),
        () => CmdAssert.Equal(RequestType.ZRange, ["key", "[a", "[z", "BYLEX", "LIMIT", "1", "5"], Request.SortedSetRange("key", new() { Range = LexRange.Between("a", "z"), Offset = 1, Count = 5 })),
        () => CmdAssert.Equal(RequestType.ZRange, ["key", "-", "+", "BYLEX", "REV", "LIMIT", "2", "3"], Request.SortedSetRange("key", new() { Range = LexRange.MinToMax, Order = Order.Descending, Offset = 2, Count = 3 })),
        () => CmdAssert.Equal(RequestType.ZRange, ["key", "-", "+", "BYLEX", "LIMIT", "5", "-1"], Request.SortedSetRange("key", new() { Range = LexRange.MinToMax, Offset = 5 })),
        () => CmdAssert.Equal(RequestType.ZRange, ["key", "-", "+", "BYLEX", "LIMIT", "0", "10"], Request.SortedSetRange("key", new() { Range = LexRange.MinToMax, Count = 10 })),

        // SortedSetScores
        () => CmdAssert.Equal(RequestType.ZMScore, ["key", "member1"], Request.SortedSetScores("key", ["member1"])),
        () => CmdAssert.Equal(RequestType.ZMScore, ["key", "member1", "member2", "member3"], Request.SortedSetScores("key", ["member1", "member2", "member3"])),
        () => CmdAssert.Equal(RequestType.ZMScore, ["key"], Request.SortedSetScores("key", [])),

        // SortedSetPopMin / SortedSetPopMax
        () => CmdAssert.Equal(RequestType.ZPopMin, ["key"], Request.SortedSetPopMin("key")),
        () => CmdAssert.Equal(RequestType.ZPopMax, ["key"], Request.SortedSetPopMax("key")),
        () => CmdAssert.Equal(RequestType.ZPopMin, ["key", "3"], Request.SortedSetPopMin("key", 3)),
        () => CmdAssert.Equal(RequestType.ZPopMax, ["key", "5"], Request.SortedSetPopMax("key", 5)),

        // SortedSetPopMin / SortedSetPopMax - Multi-key (ZMPOP)
        () => CmdAssert.Equal(RequestType.ZMPop, ["2", "key1", "key2", "MIN", "COUNT", "1"], Request.SortedSetPopMin(["key1", "key2"])),
        () => CmdAssert.Equal(RequestType.ZMPop, ["2", "key1", "key2", "MAX", "COUNT", "1"], Request.SortedSetPopMax(["key1", "key2"])),
        () => CmdAssert.Equal(RequestType.ZMPop, ["2", "key1", "key2", "MIN", "COUNT", "3"], Request.SortedSetPopMin(["key1", "key2"], 3)),
        () => CmdAssert.Equal(RequestType.ZMPop, ["2", "key1", "key2", "MAX", "COUNT", "5"], Request.SortedSetPopMax(["key1", "key2"], 5)),

        // SortedSetPopMin / SortedSetPopMax - Blocking multi-key (BZMPOP)
        () => CmdAssert.Equal(RequestType.BZMPop, ["2", "2", "key1", "key2", "MIN", "COUNT", "1"], Request.SortedSetPopMin(["key1", "key2"], TimeSpan.FromSeconds(2))),
        () => CmdAssert.Equal(RequestType.BZMPop, ["2", "2", "key1", "key2", "MAX", "COUNT", "1"], Request.SortedSetPopMax(["key1", "key2"], TimeSpan.FromSeconds(2))),
        () => CmdAssert.Equal(RequestType.BZMPop, ["5", "1", "key1", "MIN", "COUNT", "3"], Request.SortedSetPopMin(["key1"], 3, TimeSpan.FromSeconds(5))),
        () => CmdAssert.Equal(RequestType.BZMPop, ["5", "1", "key1", "MAX", "COUNT", "3"], Request.SortedSetPopMax(["key1"], 3, TimeSpan.FromSeconds(5))),

        // SortedSetRandomMember
        () => CmdAssert.Equal(RequestType.ZRandMember, ["key"], Request.SortedSetRandomMember("key")),
        () => CmdAssert.Equal(RequestType.ZRandMember, ["key", "3"], Request.SortedSetRandomMembers("key", 3)),
        () => CmdAssert.Equal(RequestType.ZRandMember, ["key", "5", "WITHSCORES"], Request.SortedSetRandomMembersWithScores("key", 5)),
        () => CmdAssert.Equal(RequestType.ZRandMember, ["key", "1", "WITHSCORES"], Request.SortedSetRandomMemberWithScore("key")),

        // SortedSetRemoveRange - by rank
        () => CmdAssert.Equal(RequestType.ZRemRangeByRank, ["key", "0", "3"], Request.SortedSetRemoveRange("key", IndexRange.Between(0, 3))),
        () => CmdAssert.Equal(RequestType.ZRemRangeByRank, ["key", "0", "-1"], Request.SortedSetRemoveRange("key", IndexRange.FirstToLast)),
        // SortedSetRemoveRange - by score
        () => CmdAssert.Equal(RequestType.ZRemRangeByScore, ["key", "1", "10"], Request.SortedSetRemoveRange("key", ScoreRange.Between(1.0, 10.0))),
        () => CmdAssert.Equal(RequestType.ZRemRangeByScore, ["key", "(1", "(10"], Request.SortedSetRemoveRange("key", ScoreRange.Between(ScoreBound.Exclusive(1.0), ScoreBound.Exclusive(10.0)))),
        () => CmdAssert.Equal(RequestType.ZRemRangeByScore, ["key", "-inf", "+inf"], Request.SortedSetRemoveRange("key", ScoreRange.MinToMax)),
        // SortedSetRemoveRange - by lex
        () => CmdAssert.Equal(RequestType.ZRemRangeByLex, ["key", "[a", "[z"], Request.SortedSetRemoveRange("key", LexRange.Between("a", "z"))),
        () => CmdAssert.Equal(RequestType.ZRemRangeByLex, ["key", "(a", "(z"], Request.SortedSetRemoveRange("key", LexRange.Between(LexBound.Exclusive("a"), LexBound.Exclusive("z")))),

        // SortedSetRangeAndStore
        () => CmdAssert.Equal(RequestType.ZRangeStore, ["dest", "src", "0", "-1"], Request.SortedSetRangeAndStore("src", "dest", new() { Range = IndexRange.FirstToLast })),
        () => CmdAssert.Equal(RequestType.ZRangeStore, ["dest", "src", "1", "3"], Request.SortedSetRangeAndStore("src", "dest", new() { Range = IndexRange.Between(1, 3) })),
        () => CmdAssert.Equal(RequestType.ZRangeStore, ["dest", "src", "-inf", "+inf", "BYSCORE"], Request.SortedSetRangeAndStore("src", "dest", new() { Range = ScoreRange.MinToMax })),
        () => CmdAssert.Equal(RequestType.ZRangeStore, ["dest", "src", "[a", "[z", "BYLEX"], Request.SortedSetRangeAndStore("src", "dest", new() { Range = LexRange.Between("a", "z") })),

        // SortedSetRank
        () => CmdAssert.Equal(RequestType.ZRank, ["key", "member"], Request.SortedSetRank("key", "member")),
        () => CmdAssert.Equal(RequestType.ZRank, ["key", "member"], Request.SortedSetRank("key", "member", Order.Ascending)),
        () => CmdAssert.Equal(RequestType.ZRevRank, ["key", "member"], Request.SortedSetRank("key", "member", Order.Descending)),

        // SortedSetRankWithScore
        () => CmdAssert.Equal(RequestType.ZRank, ["key", "member", "WITHSCORE"], Request.SortedSetRankWithScore("key", "member")),
        () => CmdAssert.Equal(RequestType.ZRank, ["key", "member", "WITHSCORE"], Request.SortedSetRankWithScore("key", "member", Order.Ascending)),
        () => CmdAssert.Equal(RequestType.ZRevRank, ["key", "member", "WITHSCORE"], Request.SortedSetRankWithScore("key", "member", Order.Descending)),

        // SortedSetScan
        () => CmdAssert.Equal(RequestType.ZScan, ["key", "0"], Request.SortedSetScan("key", 0)),
        () => CmdAssert.Equal(RequestType.ZScan, ["key", "5"], Request.SortedSetScan("key", 5)),
        () => CmdAssert.Equal(RequestType.ZScan, ["key", "0", "MATCH", "pattern*"], Request.SortedSetScan("key", 0, new ScanOptions { MatchPattern = "pattern*" })),
        () => CmdAssert.Equal(RequestType.ZScan, ["key", "0", "COUNT", "20"], Request.SortedSetScan("key", 0, new ScanOptions { Count = 20 })),
        () => CmdAssert.Equal(RequestType.ZScan, ["key", "5", "MATCH", "pattern*", "COUNT", "20"], Request.SortedSetScan("key", 5, new ScanOptions { MatchPattern = "pattern*", Count = 20 })),
        () => CmdAssert.Equal(RequestType.ZScan, ["key", "10", "MATCH", "user:*", "COUNT", "50"], Request.SortedSetScan("key", 10, new ScanOptions { MatchPattern = "user:*", Count = 50 })),
        () => CmdAssert.Equal(RequestType.ZScan, ["key", "0", "MATCH", "*"], Request.SortedSetScan("key", 0, new ScanOptions { MatchPattern = "*" })),
        () => CmdAssert.Equal(RequestType.ZScan, ["key", "100"], Request.SortedSetScan("key", 100)),

        // SortedSetScore
        () => CmdAssert.Equal(RequestType.ZScore, ["key", "member"], Request.SortedSetScore("key", "member"))
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
        var testScoreDict = new Dictionary<GlideString, object>() {
            { "member1", 10.5 },
            { "member2", 8.25 },
            { "member3", 15.0 }
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
                var response = new Dictionary<GlideString, object>() { { "member1", 8.25 } };
                var result = Assert.NotNull(Request.SortedSetPopMin("key").Converter(response));

                Assert.Equal("member1", result.Element);
                Assert.Equal(8.25, result.Score);
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
                var testDict = new Dictionary<GlideString, object>() { { "member1", 10.5 } };
                SortedSetEntry? result = Request.SortedSetPopMax("key").Converter(testDict);
                _ = Assert.NotNull(result);
                Assert.Equal("member1", result.Value.Element);
                Assert.Equal(10.5, result.Value.Score);
            },

            // Test SortedSetPopMinAsync converter - multiple elements
            () =>
            {
                Dictionary<GlideString, object> testDict = new()
                {
                    { "member1", 5.0 },
                    { "member2", 8.25 }
                };
                SortedSetEntry[] result = Request.SortedSetPopMin("key", 2).Converter(testDict);
                Assert.Equal(2, result.Length);
                SortedSetEntry member1Entry = result.First(e => e.Element.ToString() == "member1");
                Assert.Equal(5.0, member1Entry.Score);
            },

            // Test SortedSetPopMaxAsync converter - multiple elements
            () =>
            {
                Dictionary<GlideString, object> testDict = new()
                {
                    { "member1", 10.5 },
                    { "member2", 8.25 }
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
                object[] testRandomResponse = [new GlideString("member1"), new GlideString("member2")];
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
                    new object[] { new GlideString("member1"), new GlideString("10.5"), new GlideString("member2"), new GlideString("8.25") }
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
                    new object[] { new GlideString("single"), new GlideString("42.0") }
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
                    new GlideString("15"),
                    new object[] { new GlideString("test"), new GlideString("1.5") }
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
