// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands.Options;

namespace Valkey.Glide.UnitTests;

public class SortedSetCommandTests
{
    [Fact]
    public void SortedSetCommands_ValidateArguments() => Assert.Multiple(
            // SortedSetAdd - Single Member
            () => Assert.Equal(["ZADD", "key", "10.5", "member"], Request.SortedSetAddAsync("key", "member", 10.5).GetArgs()),

            // SortedSetAdd - Single Member with conditions
            () => Assert.Equal(["ZADD", "key", "NX", "10.5", "member"], Request.SortedSetAddAsync("key", "member", 10.5, new SortedSetAddOptions { Condition = SortedSetAddCondition.OnlyIfNotExists }).GetArgs()),
            () => Assert.Equal(["ZADD", "key", "XX", "10.5", "member"], Request.SortedSetAddAsync("key", "member", 10.5, new SortedSetAddOptions { Condition = SortedSetAddCondition.OnlyIfExists }).GetArgs()),
            () => Assert.Equal(["ZADD", "key", "GT", "10.5", "member"], Request.SortedSetAddAsync("key", "member", 10.5, new SortedSetAddOptions { Condition = SortedSetAddCondition.OnlyIfNotExistsOrGreaterThan }).GetArgs()),
            () => Assert.Equal(["ZADD", "key", "LT", "10.5", "member"], Request.SortedSetAddAsync("key", "member", 10.5, new SortedSetAddOptions { Condition = SortedSetAddCondition.OnlyIfNotExistsOrLessThan }).GetArgs()),
            () => Assert.Equal(["ZADD", "key", "XX", "GT", "10.5", "member"], Request.SortedSetAddAsync("key", "member", 10.5, new SortedSetAddOptions { Condition = SortedSetAddCondition.OnlyIfGreaterThan }).GetArgs()),
            () => Assert.Equal(["ZADD", "key", "XX", "LT", "10.5", "member"], Request.SortedSetAddAsync("key", "member", 10.5, new SortedSetAddOptions { Condition = SortedSetAddCondition.OnlyIfLessThan }).GetArgs()),
            () => Assert.Equal(["ZADD", "key", "CH", "10.5", "member"], Request.SortedSetAddAsync("key", "member", 10.5, new SortedSetAddOptions { Changed = true }).GetArgs()),
            () => Assert.Equal(["ZADD", "key", "NX", "CH", "10.5", "member"], Request.SortedSetAddAsync("key", "member", 10.5, new SortedSetAddOptions { Condition = SortedSetAddCondition.OnlyIfNotExists, Changed = true }).GetArgs()),

            // SortedSetAdd - Multiple Members
            () => Assert.Equal(["ZADD", "key", "10.5", "member1", "8.25", "member2"], Request.SortedSetAddAsync("key", new Dictionary<ValkeyValue, double> { ["member1"] = 10.5, ["member2"] = 8.25 }).GetArgs()),
            () => Assert.Equal(["ZADD", "key", "NX", "10.5", "member1", "8.25", "member2"], Request.SortedSetAddAsync("key", new Dictionary<ValkeyValue, double> { ["member1"] = 10.5, ["member2"] = 8.25 }, new SortedSetAddOptions { Condition = SortedSetAddCondition.OnlyIfNotExists }).GetArgs()),
            () => Assert.Equal(["ZADD", "key", "XX", "10.5", "member1", "8.25", "member2"], Request.SortedSetAddAsync("key", new Dictionary<ValkeyValue, double> { ["member1"] = 10.5, ["member2"] = 8.25 }, new SortedSetAddOptions { Condition = SortedSetAddCondition.OnlyIfExists }).GetArgs()),

            // SortedSetIncrementBy with options (ZADD INCR)
            () => Assert.Equal(["ZADD", "key", "NX", "INCR", "5", "member"], Request.SortedSetIncrementByAsync("key", "member", 5.0, new SortedSetAddOptions { Condition = SortedSetAddCondition.OnlyIfNotExists }).GetArgs()),
            () => Assert.Equal(["ZADD", "key", "XX", "GT", "INCR", "5", "member"], Request.SortedSetIncrementByAsync("key", "member", 5.0, new SortedSetAddOptions { Condition = SortedSetAddCondition.OnlyIfGreaterThan }).GetArgs()),

            // SortedSetRemove - Single Member
            () => Assert.Equal(["ZREM", "key", "member"], Request.SortedSetRemoveAsync("key", "member").GetArgs()),

            // SortedSetRemove - Multiple Members
            () => Assert.Equal(["ZREM", "key", "member1", "member2", "member3"], Request.SortedSetRemoveAsync("key", ["member1", "member2", "member3"]).GetArgs()),
            () => Assert.Equal(["ZREM", "key"], Request.SortedSetRemoveAsync("key", []).GetArgs()),
            () => Assert.Equal(["ZREM", "key", "", " ", "null", "0", "-1"], Request.SortedSetRemoveAsync("key", ["", " ", "null", "0", "-1"]).GetArgs()),

            // SortedSetCard
            () => Assert.Equal(["ZCARD", "key"], Request.SortedSetCardAsync("key").GetArgs()),
            () => Assert.Equal(["ZCARD", "mykey"], Request.SortedSetCardAsync("mykey").GetArgs()),
            () => Assert.Equal(["ZCARD", "test:sorted:set"], Request.SortedSetCardAsync("test:sorted:set").GetArgs()),
            () => Assert.Equal(["ZCARD", ""], Request.SortedSetCardAsync("").GetArgs()),

            // SortedSetCount
            () => Assert.Equal(["ZCARD", "key"], Request.SortedSetCountAsync("key", ScoreRange.All).GetArgs()),
            () => Assert.Equal(["ZCOUNT", "key", "1", "10"], Request.SortedSetCountAsync("key", ScoreRange.Between(ScoreBound.Inclusive(1.0), ScoreBound.Inclusive(10.0))).GetArgs()),
            () => Assert.Equal(["ZCOUNT", "key", "0", "100"], Request.SortedSetCountAsync("key", ScoreRange.Between(ScoreBound.Inclusive(0.0), ScoreBound.Inclusive(100.0))).GetArgs()),
            () => Assert.Equal(["ZCOUNT", "key", "-5", "5"], Request.SortedSetCountAsync("key", ScoreRange.Between(ScoreBound.Inclusive(-5.0), ScoreBound.Inclusive(5.0))).GetArgs()),
            () => Assert.Equal(["ZCOUNT", "key", "1.5", "9.75"], Request.SortedSetCountAsync("key", ScoreRange.Between(ScoreBound.Inclusive(1.5), ScoreBound.Inclusive(9.75))).GetArgs()),
            () => Assert.Equal(["ZCOUNT", "key", "0.10000000000000001", "0.90000000000000002"], Request.SortedSetCountAsync("key", ScoreRange.Between(ScoreBound.Inclusive(0.1), ScoreBound.Inclusive(0.9))).GetArgs()),
            () => Assert.Equal(["ZCOUNT", "key", "-inf", "10"], Request.SortedSetCountAsync("key", ScoreRange.To(ScoreBound.Inclusive(10.0))).GetArgs()),
            () => Assert.Equal(["ZCOUNT", "key", "0", "+inf"], Request.SortedSetCountAsync("key", ScoreRange.From(ScoreBound.Inclusive(0.0))).GetArgs()),
            () => Assert.Equal(["ZCARD", "key"], Request.SortedSetCountAsync("key", ScoreRange.All).GetArgs()),
            () => Assert.Equal(["ZCOUNT", "key", "1", "10"], Request.SortedSetCountAsync("key", ScoreRange.Between(ScoreBound.Inclusive(1.0), ScoreBound.Inclusive(10.0))).GetArgs()),
            () => Assert.Equal(["ZCOUNT", "key", "(1", "10"], Request.SortedSetCountAsync("key", ScoreRange.Between(ScoreBound.Exclusive(1.0), ScoreBound.Inclusive(10.0))).GetArgs()),
            () => Assert.Equal(["ZCOUNT", "key", "1", "(10"], Request.SortedSetCountAsync("key", ScoreRange.Between(ScoreBound.Inclusive(1.0), ScoreBound.Exclusive(10.0))).GetArgs()),
            () => Assert.Equal(["ZCOUNT", "key", "(1", "(10"], Request.SortedSetCountAsync("key", ScoreRange.Between(ScoreBound.Exclusive(1.0), ScoreBound.Exclusive(10.0))).GetArgs()),
            () => Assert.Equal(["ZCOUNT", "key", "0", "0"], Request.SortedSetCountAsync("key", ScoreRange.Between(ScoreBound.Inclusive(0.0), ScoreBound.Inclusive(0.0))).GetArgs()),
            () => Assert.Equal(["ZCOUNT", "key", "(0", "(0"], Request.SortedSetCountAsync("key", ScoreRange.Between(ScoreBound.Exclusive(0.0), ScoreBound.Exclusive(0.0))).GetArgs()),
            () => Assert.Equal(["ZCOUNT", "mykey", "1", "10"], Request.SortedSetCountAsync("mykey", ScoreRange.Between(ScoreBound.Inclusive(1.0), ScoreBound.Inclusive(10.0))).GetArgs()),
            () => Assert.Equal(["ZCOUNT", "test:sorted:set", "1", "10"], Request.SortedSetCountAsync("test:sorted:set", ScoreRange.Between(ScoreBound.Inclusive(1.0), ScoreBound.Inclusive(10.0))).GetArgs()),

            // GLIDE-native SortedSetUnion/Inter/Diff
            () => Assert.Equal(["ZUNION", "2", "key1", "key2"], Request.SortedSetUnionAsync(["key1", "key2"]).GetArgs()),
            () => Assert.Equal(["ZUNION", "3", "key1", "key2", "key3", "WEIGHTS", "1", "2", "3"], Request.SortedSetUnionAsync(new Dictionary<ValkeyKey, double> { ["key1"] = 1.0, ["key2"] = 2.0, ["key3"] = 3.0 }).GetArgs()),
            () => Assert.Equal(["ZUNION", "2", "key1", "key2", "AGGREGATE", "MAX"], Request.SortedSetUnionAsync(["key1", "key2"], Aggregate.Max).GetArgs()),
            () => Assert.Equal(["ZUNION", "2", "key1", "key2", "WEIGHTS", "1.5", "2.5", "AGGREGATE", "MIN"], Request.SortedSetUnionAsync(new Dictionary<ValkeyKey, double> { ["key1"] = 1.5, ["key2"] = 2.5 }, Aggregate.Min).GetArgs()),
            () => Assert.Equal(["ZUNION", "2", "key1", "key2", "WITHSCORES"], Request.SortedSetUnionWithScoreAsync(["key1", "key2"]).GetArgs()),
            () => Assert.Equal(["ZUNION", "2", "key1", "key2", "WEIGHTS", "1", "2", "WITHSCORES"], Request.SortedSetUnionWithScoreAsync(new Dictionary<ValkeyKey, double> { ["key1"] = 1.0, ["key2"] = 2.0 }).GetArgs()),
            () => Assert.Equal(["ZINTER", "2", "key1", "key2"], Request.SortedSetInterAsync(["key1", "key2"]).GetArgs()),
            () => Assert.Equal(["ZINTER", "2", "key1", "key2", "AGGREGATE", "MAX"], Request.SortedSetInterAsync(["key1", "key2"], Aggregate.Max).GetArgs()),
            () => Assert.Equal(["ZINTER", "2", "key1", "key2", "WEIGHTS", "2", "3"], Request.SortedSetInterAsync(new Dictionary<ValkeyKey, double> { ["key1"] = 2.0, ["key2"] = 3.0 }).GetArgs()),
            () => Assert.Equal(["ZINTER", "2", "key1", "key2", "WITHSCORES"], Request.SortedSetInterWithScoreAsync(["key1", "key2"]).GetArgs()),
            () => Assert.Equal(["ZINTER", "2", "key1", "key2", "WEIGHTS", "1", "2", "AGGREGATE", "MAX", "WITHSCORES"], Request.SortedSetInterWithScoreAsync(new Dictionary<ValkeyKey, double> { ["key1"] = 1.0, ["key2"] = 2.0 }, Aggregate.Max).GetArgs()),
            () => Assert.Equal(["ZDIFF", "2", "key1", "key2"], Request.SortedSetDiffAsync(["key1", "key2"]).GetArgs()),
            () => Assert.Equal(["ZDIFF", "3", "key1", "key2", "key3"], Request.SortedSetDiffAsync(["key1", "key2", "key3"]).GetArgs()),
            () => Assert.Equal(["ZDIFF", "2", "key1", "key2", "WITHSCORES"], Request.SortedSetDiffWithScoreAsync(["key1", "key2"]).GetArgs()),

            // GLIDE-native SortedSetUnion/Inter/Diff AndStore
            () => Assert.Equal(["ZUNIONSTORE", "dest", "2", "key1", "key2"], Request.SortedSetUnionAndStoreAsync("dest", ["key1", "key2"]).GetArgs()),
            () => Assert.Equal(["ZUNIONSTORE", "dest", "3", "key1", "key2", "key3", "WEIGHTS", "1", "2", "3"], Request.SortedSetUnionAndStoreAsync("dest", new Dictionary<ValkeyKey, double> { ["key1"] = 1.0, ["key2"] = 2.0, ["key3"] = 3.0 }).GetArgs()),
            () => Assert.Equal(["ZUNIONSTORE", "dest", "2", "key1", "key2", "AGGREGATE", "MIN"], Request.SortedSetUnionAndStoreAsync("dest", ["key1", "key2"], Aggregate.Min).GetArgs()),
            () => Assert.Equal(["ZINTERSTORE", "dest", "2", "key1", "key2"], Request.SortedSetInterAndStoreAsync("dest", ["key1", "key2"]).GetArgs()),
            () => Assert.Equal(["ZINTERSTORE", "dest", "2", "key1", "key2", "AGGREGATE", "MIN"], Request.SortedSetInterAndStoreAsync("dest", ["key1", "key2"], Aggregate.Min).GetArgs()),
            () => Assert.Equal(["ZINTERSTORE", "dest", "2", "key1", "key2", "WEIGHTS", "2", "3"], Request.SortedSetInterAndStoreAsync("dest", new Dictionary<ValkeyKey, double> { ["key1"] = 2.0, ["key2"] = 3.0 }).GetArgs()),
            () => Assert.Equal(["ZDIFFSTORE", "dest", "2", "key1", "key2"], Request.SortedSetDiffAndStoreAsync("dest", ["key1", "key2"]).GetArgs()),

            // SortedSetIncrementBy (convenience, uses ZADD INCR under the hood)
            () => Assert.Equal(["ZADD", "key", "INCR", "2.5", "member"], Request.SortedSetIncrementByAsync("key", "member", 2.5, new SortedSetAddOptions()).GetArgs()),
            () => Assert.Equal(["ZADD", "key", "INCR", "-1.5", "member"], Request.SortedSetIncrementByAsync("key", "member", -1.5, new SortedSetAddOptions()).GetArgs()),
            () => Assert.Equal(["ZADD", "key", "INCR", "0", "member"], Request.SortedSetIncrementByAsync("key", "member", 0.0, new SortedSetAddOptions()).GetArgs()),

            // SortedSetInterCard
            () => Assert.Equal(["ZINTERCARD", "2", "key1", "key2"], Request.SortedSetInterCardAsync(["key1", "key2"]).GetArgs()),
            () => Assert.Equal(["ZINTERCARD", "3", "key1", "key2", "key3"], Request.SortedSetInterCardAsync(["key1", "key2", "key3"]).GetArgs()),
            () => Assert.Equal(["ZINTERCARD", "2", "key1", "key2", "LIMIT", "10"], Request.SortedSetInterCardAsync(["key1", "key2"], 10).GetArgs()),

            // SortedSetLexCount
            () => Assert.Equal(["ZLEXCOUNT", "key", "[a", "[z"], Request.SortedSetLexCountAsync("key", LexRange.Between(LexBound.Inclusive("a"), LexBound.Inclusive("z"))).GetArgs()),
            () => Assert.Equal(["ZLEXCOUNT", "key", "(a", "(z"], Request.SortedSetLexCountAsync("key", LexRange.Between(LexBound.Exclusive("a"), LexBound.Exclusive("z"))).GetArgs()),
            () => Assert.Equal(["ZLEXCOUNT", "key", "(a", "[z"], Request.SortedSetLexCountAsync("key", LexRange.Between(LexBound.Exclusive("a"), LexBound.Inclusive("z"))).GetArgs()),
            () => Assert.Equal(["ZLEXCOUNT", "key", "[a", "(z"], Request.SortedSetLexCountAsync("key", LexRange.Between(LexBound.Inclusive("a"), LexBound.Exclusive("z"))).GetArgs()),
            () => Assert.Equal(["ZLEXCOUNT", "key", "-", "+"], Request.SortedSetLexCountAsync("key", LexRange.All).GetArgs()),

            // GLIDE-native SortedSetRange (ZRANGE with SortedSetRangeOptions)
            // Rank range
            () => Assert.Equal(["ZRANGE", "key", "0", "-1"], Request.SortedSetRangeAsync("key", new() { Range = RankRange.All }).GetArgs()),
            () => Assert.Equal(["ZRANGE", "key", "1", "3"], Request.SortedSetRangeAsync("key", new() { Range = RankRange.Between(1, 3) }).GetArgs()),
            () => Assert.Equal(["ZRANGE", "key", "0", "-1", "REV"], Request.SortedSetRangeAsync("key", new() { Range = RankRange.All, Order = Order.Descending }).GetArgs()),
            () => Assert.Equal(["ZRANGE", "key", "0", "-1", "LIMIT", "2", "3"], Request.SortedSetRangeAsync("key", new() { Range = RankRange.All, Offset = 2, Count = 3 }).GetArgs()),
            () => Assert.Equal(["ZRANGE", "key", "0", "-1", "REV", "LIMIT", "1", "5"], Request.SortedSetRangeAsync("key", new() { Range = RankRange.All, Order = Order.Descending, Offset = 1, Count = 5 }).GetArgs()),
            // Score range
            () => Assert.Equal(["ZRANGE", "key", "-inf", "+inf", "BYSCORE"], Request.SortedSetRangeAsync("key", new() { Range = ScoreRange.All }).GetArgs()),
            () => Assert.Equal(["ZRANGE", "key", "1", "10", "BYSCORE"], Request.SortedSetRangeAsync("key", new() { Range = ScoreRange.Between(ScoreBound.Inclusive(1.0), ScoreBound.Inclusive(10.0)) }).GetArgs()),
            () => Assert.Equal(["ZRANGE", "key", "1", "10", "BYSCORE", "LIMIT", "2", "3"], Request.SortedSetRangeAsync("key", new() { Range = ScoreRange.Between(ScoreBound.Inclusive(1.0), ScoreBound.Inclusive(10.0)), Offset = 2, Count = 3 }).GetArgs()),
            () => Assert.Equal(["ZRANGE", "key", "-inf", "+inf", "BYSCORE", "REV"], Request.SortedSetRangeAsync("key", new() { Range = ScoreRange.All, Order = Order.Descending }).GetArgs()),
            // Lex range
            () => Assert.Equal(["ZRANGE", "key", "-", "+", "BYLEX"], Request.SortedSetRangeAsync("key", new() { Range = LexRange.All }).GetArgs()),
            () => Assert.Equal(["ZRANGE", "key", "[a", "[z", "BYLEX"], Request.SortedSetRangeAsync("key", new() { Range = LexRange.Between(LexBound.Inclusive("a"), LexBound.Inclusive("z")) }).GetArgs()),
            () => Assert.Equal(["ZRANGE", "key", "[a", "[z", "BYLEX", "LIMIT", "1", "5"], Request.SortedSetRangeAsync("key", new() { Range = LexRange.Between(LexBound.Inclusive("a"), LexBound.Inclusive("z")), Offset = 1, Count = 5 }).GetArgs()),
            () => Assert.Equal(["ZRANGE", "key", "-", "+", "BYLEX", "REV", "LIMIT", "2", "3"], Request.SortedSetRangeAsync("key", new() { Range = LexRange.All, Order = Order.Descending, Offset = 2, Count = 3 }).GetArgs()),
            // LIMIT defaults: Offset only → Count defaults to -1
            () => Assert.Equal(["ZRANGE", "key", "0", "-1", "LIMIT", "5", "-1"], Request.SortedSetRangeAsync("key", new() { Range = RankRange.All, Offset = 5 }).GetArgs()),
            // LIMIT defaults: Count only → Offset defaults to 0
            () => Assert.Equal(["ZRANGE", "key", "0", "-1", "LIMIT", "0", "10"], Request.SortedSetRangeAsync("key", new() { Range = RankRange.All, Count = 10 }).GetArgs()),

            // SortedSetScores
            () => Assert.Equal(["ZMSCORE", "key", "member1"], Request.SortedSetScoresAsync("key", ["member1"]).GetArgs()),
            () => Assert.Equal(["ZMSCORE", "key", "member1", "member2", "member3"], Request.SortedSetScoresAsync("key", ["member1", "member2", "member3"]).GetArgs()),
            () => Assert.Equal(["ZMSCORE", "key"], Request.SortedSetScoresAsync("key", []).GetArgs()),

            // Double formatting tests
            () => Assert.Equal("+inf", double.PositiveInfinity.ToGlideString().ToString()),
            () => Assert.Equal("-inf", double.NegativeInfinity.ToGlideString().ToString()),
            () => Assert.Equal("nan", double.NaN.ToGlideString().ToString()),
            () => Assert.Equal("0", 0.0.ToGlideString().ToString()),
            () => Assert.Equal("10.5", 10.5.ToGlideString().ToString()),

            // SortedSetPopMin / SortedSetPopMax (GLIDE-native)
            () => Assert.Equal(["ZPOPMIN", "key"], Request.SortedSetPopMinAsync("key").GetArgs()),
            () => Assert.Equal(["ZPOPMAX", "key"], Request.SortedSetPopMaxAsync("key").GetArgs()),
            () => Assert.Equal(["ZPOPMIN", "key", "3"], Request.SortedSetPopMinAsync("key", 3).GetArgs()),
            () => Assert.Equal(["ZPOPMAX", "key", "5"], Request.SortedSetPopMaxAsync("key", 5).GetArgs()),

            // SortedSetPopMin / SortedSetPopMax - Multi-key (ZMPOP)
            () => Assert.Equal(["ZMPOP", "2", "key1", "key2", "MIN", "COUNT", "1"], Request.SortedSetPopMinMultiKeyAsync(["key1", "key2"]).GetArgs()),
            () => Assert.Equal(["ZMPOP", "2", "key1", "key2", "MAX", "COUNT", "1"], Request.SortedSetPopMaxMultiKeyAsync(["key1", "key2"]).GetArgs()),
            () => Assert.Equal(["ZMPOP", "2", "key1", "key2", "MIN", "COUNT", "3"], Request.SortedSetPopMinMultiKeyAsync(["key1", "key2"], 3).GetArgs()),
            () => Assert.Equal(["ZMPOP", "2", "key1", "key2", "MAX", "COUNT", "5"], Request.SortedSetPopMaxMultiKeyAsync(["key1", "key2"], 5).GetArgs()),

            // SortedSetPopMin / SortedSetPopMax - Blocking multi-key (BZMPOP)
            () => Assert.Equal(["BZMPOP", "2", "2", "key1", "key2", "MIN", "COUNT", "1"], Request.SortedSetBlockingPopMinMultiKeyAsync(["key1", "key2"], TimeSpan.FromSeconds(2)).GetArgs()),
            () => Assert.Equal(["BZMPOP", "2", "2", "key1", "key2", "MAX", "COUNT", "1"], Request.SortedSetBlockingPopMaxMultiKeyAsync(["key1", "key2"], TimeSpan.FromSeconds(2)).GetArgs()),
            () => Assert.Equal(["BZMPOP", "5", "1", "key1", "MIN", "COUNT", "3"], Request.SortedSetBlockingPopMinMultiKeyAsync(["key1"], 3, TimeSpan.FromSeconds(5)).GetArgs()),
            () => Assert.Equal(["BZMPOP", "5", "1", "key1", "MAX", "COUNT", "3"], Request.SortedSetBlockingPopMaxMultiKeyAsync(["key1"], 3, TimeSpan.FromSeconds(5)).GetArgs()),

            // SortedSetRandomMember
            () => Assert.Equal(["ZRANDMEMBER", "key"], Request.SortedSetRandomMemberAsync("key").GetArgs()),
            () => Assert.Equal(["ZRANDMEMBER", "key", "3"], Request.SortedSetRandomMembersAsync("key", 3).GetArgs()),
            () => Assert.Equal(["ZRANDMEMBER", "key", "5", "WITHSCORES"], Request.SortedSetRandomMembersWithScoresAsync("key", 5).GetArgs()),
            () => Assert.Equal(["ZRANDMEMBER", "key", "1", "WITHSCORES"], Request.SortedSetRandomMemberWithScoreAsync("key").GetArgs()),

            // SortedSetRemoveRange - by rank
            () => Assert.Equal(["ZREMRANGEBYRANK", "key", "0", "3"], Request.SortedSetRemoveRangeAsync("key", RankRange.Between(0, 3)).GetArgs()),
            () => Assert.Equal(["ZREMRANGEBYRANK", "key", "0", "-1"], Request.SortedSetRemoveRangeAsync("key", RankRange.All).GetArgs()),
            // SortedSetRemoveRange - by score
            () => Assert.Equal(["ZREMRANGEBYSCORE", "key", "1", "10"], Request.SortedSetRemoveRangeAsync("key", ScoreRange.Between(ScoreBound.Inclusive(1.0), ScoreBound.Inclusive(10.0))).GetArgs()),
            () => Assert.Equal(["ZREMRANGEBYSCORE", "key", "(1", "(10"], Request.SortedSetRemoveRangeAsync("key", ScoreRange.Between(ScoreBound.Exclusive(1.0), ScoreBound.Exclusive(10.0))).GetArgs()),
            () => Assert.Equal(["ZREMRANGEBYSCORE", "key", "-inf", "+inf"], Request.SortedSetRemoveRangeAsync("key", ScoreRange.All).GetArgs()),
            // SortedSetRemoveRange - by lex
            () => Assert.Equal(["ZREMRANGEBYLEX", "key", "[a", "[z"], Request.SortedSetRemoveRangeAsync("key", LexRange.Between(LexBound.Inclusive("a"), LexBound.Inclusive("z"))).GetArgs()),
            () => Assert.Equal(["ZREMRANGEBYLEX", "key", "(a", "(z"], Request.SortedSetRemoveRangeAsync("key", LexRange.Between(LexBound.Exclusive("a"), LexBound.Exclusive("z"))).GetArgs()),

            // SortedSetRangeAndStore
            () => Assert.Equal(["ZRANGESTORE", "dest", "src", "0", "-1"], Request.SortedSetRangeAndStoreAsync("src", "dest", new() { Range = RankRange.All }).GetArgs()),
            () => Assert.Equal(["ZRANGESTORE", "dest", "src", "1", "3"], Request.SortedSetRangeAndStoreAsync("src", "dest", new() { Range = RankRange.Between(1, 3) }).GetArgs()),
            () => Assert.Equal(["ZRANGESTORE", "dest", "src", "-inf", "+inf", "BYSCORE"], Request.SortedSetRangeAndStoreAsync("src", "dest", new() { Range = ScoreRange.All }).GetArgs()),
            () => Assert.Equal(["ZRANGESTORE", "dest", "src", "[a", "[z", "BYLEX"], Request.SortedSetRangeAndStoreAsync("src", "dest", new() { Range = LexRange.Between(LexBound.Inclusive("a"), LexBound.Inclusive("z")) }).GetArgs()),

            // SortedSetRank
            () => Assert.Equal(["ZRANK", "key", "member"], Request.SortedSetRankAsync("key", "member").GetArgs()),
            () => Assert.Equal(["ZRANK", "key", "member"], Request.SortedSetRankAsync("key", "member", Order.Ascending).GetArgs()),
            () => Assert.Equal(["ZREVRANK", "key", "member"], Request.SortedSetRankAsync("key", "member", Order.Descending).GetArgs()),

            // SortedSetScan
            () => Assert.Equal(["ZSCAN", "key", "0"], Request.SortedSetScanAsync("key").GetArgs()),
            () => Assert.Equal(["ZSCAN", "key", "5"], Request.SortedSetScanAsync("key", cursor: 5).GetArgs()),
            () => Assert.Equal(["ZSCAN", "key", "0", "MATCH", "pattern*"], Request.SortedSetScanAsync("key", "pattern*").GetArgs()),
            () => Assert.Equal(["ZSCAN", "key", "0", "COUNT", "20"], Request.SortedSetScanAsync("key", pageSize: 20).GetArgs()),
            () => Assert.Equal(["ZSCAN", "key", "5", "MATCH", "pattern*", "COUNT", "20"], Request.SortedSetScanAsync("key", "pattern*", 20, 5).GetArgs()),
            () => Assert.Equal(["ZSCAN", "key", "10", "MATCH", "user:*", "COUNT", "50"], Request.SortedSetScanAsync("key", "user:*", 50, 10).GetArgs()),
            () => Assert.Equal(["ZSCAN", "key", "0", "MATCH", "*"], Request.SortedSetScanAsync("key", "*").GetArgs()),
            () => Assert.Equal(["ZSCAN", "key", "100"], Request.SortedSetScanAsync("key", cursor: 100).GetArgs()),

            // SortedSetScore
            () => Assert.Equal(["ZSCORE", "key", "member"], Request.SortedSetScoreAsync("key", "member").GetArgs())
        );

    [Fact]
    public void SortedSetCommands_ValidateConverters() => Assert.Multiple(
            // Basic converter tests
            () => Assert.True(Request.SortedSetAddAsync("key", "member", 10.5).Converter(1L)),
            () => Assert.False(Request.SortedSetAddAsync("key", "member", 10.5).Converter(0L)),
            () => Assert.True(Request.SortedSetRemoveAsync("key", "member").Converter(1L)),
            () => Assert.False(Request.SortedSetRemoveAsync("key", "member").Converter(0L)),
            () => Assert.Equal(2L, Request.SortedSetRemoveAsync("key", ["member1", "member2"]).Converter(2L)),
            () => Assert.Equal(5L, Request.SortedSetCardAsync("key").Converter(5L)),
            () => Assert.Equal(3L, Request.SortedSetCountAsync("key", ScoreRange.Between(ScoreBound.Inclusive(1.0), ScoreBound.Inclusive(10.0))).Converter(3L)),
            () => Assert.Equal(0L, Request.SortedSetCountAsync("key", ScoreRange.All).Converter(0L)),

            // Type converter test
            () => Assert.Equal(ValkeyType.SortedSet, Request.KeyTypeAsync("key").Converter("zset"))
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
                double? result = Request.SortedSetIncrementByAsync("key", "member", 2.5, new SortedSetAddOptions()).Converter(12.5);
                Assert.Equal(12.5, result);
            },

            // Test SortedSetUnionWithScoreAsync converter (GLIDE-native, returns SortedSetEntry[])
            () =>
            {
                SortedSetEntry[] result = Request.SortedSetUnionWithScoreAsync(["key1", "key2"]).Converter(testScoreDict);
                Assert.Equal(3, result.Length);
                Assert.Contains(result, r => r.Element == "member1" && r.Score == 10.5);
                Assert.Contains(result, r => r.Element == "member2" && r.Score == 8.25);
                Assert.Contains(result, r => r.Element == "member3" && r.Score == 15.0);
            },

            // Test SortedSetScoresAsync converter
            () =>
            {
                object[] testScoresResponse = [10.5, null!, 8.25];
                double?[] result = Request.SortedSetScoresAsync("key", ["member1", "member2", "member3"]).Converter(testScoresResponse);
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
                SortedSetEntry? result = Request.SortedSetPopMinAsync("key").Converter(testDict);
                _ = Assert.NotNull(result);
                Assert.Equal("member1", result.Value.Element);
                Assert.Equal(8.25, result.Value.Score);
            },

            // Test SortedSetPopMinAsync converter - null result
            () =>
            {
                SortedSetEntry? result = Request.SortedSetPopMinAsync("key").Converter(null);
                Assert.Null(result);
            },

            // Test SortedSetPopMaxAsync converter - single element
            () =>
            {
                Dictionary<gs, object> testDict = new()
                {
                    { (gs)"member1", 10.5 }
                };
                SortedSetEntry? result = Request.SortedSetPopMaxAsync("key").Converter(testDict);
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
                SortedSetEntry[] result = Request.SortedSetPopMinAsync("key", 2).Converter(testDict);
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
                SortedSetEntry[] result = Request.SortedSetPopMaxAsync("key", 2).Converter(testDict);
                Assert.Equal(2, result.Length);
                SortedSetEntry member1Entry = result.First(e => e.Element.ToString() == "member1");
                Assert.Equal(10.5, member1Entry.Score);
            },

            // Test SortedSetRandomMemberAsync converter
            () =>
            {
                ValkeyValue result = Request.SortedSetRandomMemberAsync("key").Converter(null);
                Assert.Equal(ValkeyValue.Null, result);
            },

            // Test SortedSetRandomMembersAsync converter
            () =>
            {
                object[] testRandomResponse = [(gs)"member1", (gs)"member2"];
                ValkeyValue[] result = Request.SortedSetRandomMembersAsync("key", 2).Converter(testRandomResponse);
                Assert.Equal(2, result.Length);
                Assert.Equal("member1", result[0]);
                Assert.Equal("member2", result[1]);
            },

            // Test SortedSetRankAsync converter
            () =>
            {
                long? result = Request.SortedSetRankAsync("key", "member").Converter(3L);
                Assert.Equal(3L, result);
            },

            // Test SortedSetRankAsync converter - null result
            () =>
            {
                long? result = Request.SortedSetRankAsync("key", "member").Converter(null);
                Assert.Null(result);
            },

            // Test SortedSetScanAsync converter - basic case
            () =>
            {
                object[] testScanResponse = [
                    5L,
                    new object[] { (gs)"member1", (gs)"10.5", (gs)"member2", (gs)"8.25" }
                ];
                var (cursor, items) = Request.SortedSetScanAsync("key").Converter(testScanResponse);
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
                var (cursor, items) = Request.SortedSetScanAsync("key").Converter(testScanResponse);
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
                var (cursor, items) = Request.SortedSetScanAsync("key").Converter(testScanResponse);
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
                var (cursor, items) = Request.SortedSetScanAsync("key").Converter(testScanResponse);
                Assert.Equal(15L, cursor);
                _ = Assert.Single(items);
                Assert.Equal("test", items[0].Element);
                Assert.Equal(1.5, items[0].Score);
            },

            // Test SortedSetScoreAsync converter
            () =>
            {
                double? result = Request.SortedSetScoreAsync("key", "member").Converter(10.5);
                Assert.Equal(10.5, result);
            },

            // Test SortedSetScoreAsync converter - null result
            () =>
            {
                double? result = Request.SortedSetScoreAsync("key", "member").Converter(null);
                Assert.Null(result);
            }
        );
    }

    [Fact]
    public void RangeByLex_ToArgs_GeneratesCorrectArguments()
        => Assert.Multiple(
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

    [Fact]
    public void RangeByScore_ToArgs_GeneratesCorrectArguments()
        => Assert.Multiple(
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
