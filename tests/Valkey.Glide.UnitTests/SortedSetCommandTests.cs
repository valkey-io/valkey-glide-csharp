// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands.Options;

namespace Valkey.Glide.UnitTests;

public class SortedSetCommandTests
{
    [Fact]
    public void SortedSetCommands_ValidateArguments() => Assert.Multiple(
            // SortedSetAdd - Single member
            () => Assert.Equal(["ZADD", "key", "10.5", "member"], Request.SortedSetAddAsync("key", "member", 10.5).GetArgs()),
            () => Assert.Equal(["ZADD", "key", "NX", "10.5", "member"], Request.SortedSetAddAsync("key", "member", 10.5, new SortedSetAddOptions { Condition = SortedSetAddCondition.OnlyIfNotExists }).GetArgs()),
            () => Assert.Equal(["ZADD", "key", "XX", "10.5", "member"], Request.SortedSetAddAsync("key", "member", 10.5, new SortedSetAddOptions { Condition = SortedSetAddCondition.OnlyIfExists }).GetArgs()),
            () => Assert.Equal(["ZADD", "key", "GT", "10.5", "member"], Request.SortedSetAddAsync("key", "member", 10.5, new SortedSetAddOptions { Condition = SortedSetAddCondition.OnlyIfNotExistsOrGreaterThan }).GetArgs()),
            () => Assert.Equal(["ZADD", "key", "LT", "10.5", "member"], Request.SortedSetAddAsync("key", "member", 10.5, new SortedSetAddOptions { Condition = SortedSetAddCondition.OnlyIfNotExistsOrLessThan }).GetArgs()),
            () => Assert.Equal(["ZADD", "key", "XX", "GT", "10.5", "member"], Request.SortedSetAddAsync("key", "member", 10.5, new SortedSetAddOptions { Condition = SortedSetAddCondition.OnlyIfGreaterThan }).GetArgs()),
            () => Assert.Equal(["ZADD", "key", "XX", "LT", "10.5", "member"], Request.SortedSetAddAsync("key", "member", 10.5, new SortedSetAddOptions { Condition = SortedSetAddCondition.OnlyIfLessThan }).GetArgs()),
            () => Assert.Equal(["ZADD", "key", "CH", "10.5", "member"], Request.SortedSetAddAsync("key", "member", 10.5, new SortedSetAddOptions { Changed = true }).GetArgs()),
            () => Assert.Equal(["ZADD", "key", "NX", "CH", "10.5", "member"], Request.SortedSetAddAsync("key", "member", 10.5, new SortedSetAddOptions { Condition = SortedSetAddCondition.OnlyIfNotExists, Changed = true }).GetArgs()),

            // SortedSetAdd - Multiple members
            () => Assert.Equal(["ZADD", "key", "10.5", "member1", "8.25", "member2"], Request.SortedSetAddAsync("key", [new SortedSetEntry("member1", 10.5), new SortedSetEntry("member2", 8.25)]).GetArgs()),
            () => Assert.Equal(["ZADD", "key", "NX", "10.5", "member1", "8.25", "member2"], Request.SortedSetAddAsync("key", [new SortedSetEntry("member1", 10.5), new SortedSetEntry("member2", 8.25)], new SortedSetAddOptions { Condition = SortedSetAddCondition.OnlyIfNotExists }).GetArgs()),
            () => Assert.Equal(["ZADD", "key", "XX", "10.5", "member1", "8.25", "member2"], Request.SortedSetAddAsync("key", [new SortedSetEntry("member1", 10.5), new SortedSetEntry("member2", 8.25)], new SortedSetAddOptions { Condition = SortedSetAddCondition.OnlyIfExists }).GetArgs()),

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
            () => Assert.Equal(["ZCOUNT", "key", "-inf", "+inf"], Request.SortedSetCountAsync("key", ScoreRange.MinToMax).GetArgs()),
            () => Assert.Equal(["ZCOUNT", "key", "1", "10"], Request.SortedSetCountAsync("key", ScoreRange.Between(1.0, 10.0)).GetArgs()),
            () => Assert.Equal(["ZCOUNT", "key", "0", "100"], Request.SortedSetCountAsync("key", ScoreRange.Between(0.0, 100.0)).GetArgs()),
            () => Assert.Equal(["ZCOUNT", "key", "-5", "5"], Request.SortedSetCountAsync("key", ScoreRange.Between(-5.0, 5.0)).GetArgs()),
            () => Assert.Equal(["ZCOUNT", "key", "1.5", "9.75"], Request.SortedSetCountAsync("key", ScoreRange.Between(1.5, 9.75)).GetArgs()),
            () => Assert.Equal(["ZCOUNT", "key", "0.10000000000000001", "0.90000000000000002"], Request.SortedSetCountAsync("key", ScoreRange.Between(0.1, 0.9)).GetArgs()),
            () => Assert.Equal(["ZCOUNT", "key", "-inf", "10"], Request.SortedSetCountAsync("key", ScoreRange.Between(ScoreBound.Min, 10.0)).GetArgs()),
            () => Assert.Equal(["ZCOUNT", "key", "0", "+inf"], Request.SortedSetCountAsync("key", ScoreRange.Between(0.0, ScoreBound.Max)).GetArgs()),
            () => Assert.Equal(["ZCOUNT", "key", "-inf", "+inf"], Request.SortedSetCountAsync("key", ScoreRange.MinToMax).GetArgs()),
            () => Assert.Equal(["ZCOUNT", "key", "1", "10"], Request.SortedSetCountAsync("key", ScoreRange.Between(1.0, 10.0)).GetArgs()),
            () => Assert.Equal(["ZCOUNT", "key", "(1", "10"], Request.SortedSetCountAsync("key", ScoreRange.Between(ScoreBound.Exclusive(1.0), 10.0)).GetArgs()),
            () => Assert.Equal(["ZCOUNT", "key", "1", "(10"], Request.SortedSetCountAsync("key", ScoreRange.Between(1.0, ScoreBound.Exclusive(10.0))).GetArgs()),
            () => Assert.Equal(["ZCOUNT", "key", "(1", "(10"], Request.SortedSetCountAsync("key", ScoreRange.Between(ScoreBound.Exclusive(1.0), ScoreBound.Exclusive(10.0))).GetArgs()),
            () => Assert.Equal(["ZCOUNT", "key", "0", "0"], Request.SortedSetCountAsync("key", ScoreRange.Between(0.0, 0.0)).GetArgs()),
            () => Assert.Equal(["ZCOUNT", "key", "(0", "(0"], Request.SortedSetCountAsync("key", ScoreRange.Between(ScoreBound.Exclusive(0.0), ScoreBound.Exclusive(0.0))).GetArgs()),
            () => Assert.Equal(["ZCOUNT", "mykey", "1", "10"], Request.SortedSetCountAsync("mykey", ScoreRange.Between(1.0, 10.0)).GetArgs()),
            () => Assert.Equal(["ZCOUNT", "test:sorted:set", "1", "10"], Request.SortedSetCountAsync("test:sorted:set", ScoreRange.Between(1.0, 10.0)).GetArgs()),

            // SortedSetUnion/Inter/Diff
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

            // SortedSetUnion/Inter/Diff AndStore
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
            () => Assert.Equal(["ZLEXCOUNT", "key", "[a", "[z"], Request.SortedSetLexCountAsync("key", LexRange.Between("a", "z")).GetArgs()),
            () => Assert.Equal(["ZLEXCOUNT", "key", "(a", "(z"], Request.SortedSetLexCountAsync("key", LexRange.Between(LexBound.Exclusive("a"), LexBound.Exclusive("z"))).GetArgs()),
            () => Assert.Equal(["ZLEXCOUNT", "key", "(a", "[z"], Request.SortedSetLexCountAsync("key", LexRange.Between(LexBound.Exclusive("a"), "z")).GetArgs()),
            () => Assert.Equal(["ZLEXCOUNT", "key", "[a", "(z"], Request.SortedSetLexCountAsync("key", LexRange.Between("a", LexBound.Exclusive("z"))).GetArgs()),
            () => Assert.Equal(["ZLEXCOUNT", "key", "-", "+"], Request.SortedSetLexCountAsync("key", LexRange.MinToMax).GetArgs()),

            // SortedSetRangeAsync - Rank
            () => Assert.Equal(["ZRANGE", "key", "0", "-1"], Request.SortedSetRangeAsync("key", new() { Range = IndexRange.FirstToLast }).GetArgs()),
            () => Assert.Equal(["ZRANGE", "key", "1", "3"], Request.SortedSetRangeAsync("key", new() { Range = IndexRange.Between(1, 3) }).GetArgs()),
            () => Assert.Equal(["ZRANGE", "key", "0", "-1", "REV"], Request.SortedSetRangeAsync("key", new() { Range = IndexRange.FirstToLast, Order = Order.Descending }).GetArgs()),
            () => Assert.Throws<ArgumentException>(() => Request.SortedSetRangeAsync("key", new() { Range = IndexRange.FirstToLast, Offset = 2, Count = 3 }).GetArgs()),
            () => Assert.Throws<ArgumentException>(() => Request.SortedSetRangeAsync("key", new() { Range = IndexRange.FirstToLast, Order = Order.Descending, Offset = 1, Count = 5 }).GetArgs()),

            // SortedSetRangeAsync - Score range
            () => Assert.Equal(["ZRANGE", "key", "-inf", "+inf", "BYSCORE"], Request.SortedSetRangeAsync("key", new() { Range = ScoreRange.MinToMax }).GetArgs()),
            () => Assert.Equal(["ZRANGE", "key", "1", "10", "BYSCORE"], Request.SortedSetRangeAsync("key", new() { Range = ScoreRange.Between(1.0, 10.0) }).GetArgs()),
            () => Assert.Equal(["ZRANGE", "key", "1", "10", "BYSCORE", "LIMIT", "2", "3"], Request.SortedSetRangeAsync("key", new() { Range = ScoreRange.Between(1.0, 10.0), Offset = 2, Count = 3 }).GetArgs()),
            () => Assert.Equal(["ZRANGE", "key", "-inf", "+inf", "BYSCORE", "REV"], Request.SortedSetRangeAsync("key", new() { Range = ScoreRange.MinToMax, Order = Order.Descending }).GetArgs()),

            // SortedSetRangeAsync - Lex range
            () => Assert.Equal(["ZRANGE", "key", "-", "+", "BYLEX"], Request.SortedSetRangeAsync("key", new() { Range = LexRange.MinToMax }).GetArgs()),
            () => Assert.Equal(["ZRANGE", "key", "[a", "[z", "BYLEX"], Request.SortedSetRangeAsync("key", new() { Range = LexRange.Between("a", "z") }).GetArgs()),
            () => Assert.Equal(["ZRANGE", "key", "[a", "[z", "BYLEX", "LIMIT", "1", "5"], Request.SortedSetRangeAsync("key", new() { Range = LexRange.Between("a", "z"), Offset = 1, Count = 5 }).GetArgs()),
            () => Assert.Equal(["ZRANGE", "key", "-", "+", "BYLEX", "REV", "LIMIT", "2", "3"], Request.SortedSetRangeAsync("key", new() { Range = LexRange.MinToMax, Order = Order.Descending, Offset = 2, Count = 3 }).GetArgs()),
            () => Assert.Equal(["ZRANGE", "key", "-", "+", "BYLEX", "LIMIT", "5", "-1"], Request.SortedSetRangeAsync("key", new() { Range = LexRange.MinToMax, Offset = 5 }).GetArgs()),
            () => Assert.Equal(["ZRANGE", "key", "-", "+", "BYLEX", "LIMIT", "0", "10"], Request.SortedSetRangeAsync("key", new() { Range = LexRange.MinToMax, Count = 10 }).GetArgs()),

            // SortedSetScores
            () => Assert.Equal(["ZMSCORE", "key", "member1"], Request.SortedSetScoresAsync("key", ["member1"]).GetArgs()),
            () => Assert.Equal(["ZMSCORE", "key", "member1", "member2", "member3"], Request.SortedSetScoresAsync("key", ["member1", "member2", "member3"]).GetArgs()),
            () => Assert.Equal(["ZMSCORE", "key"], Request.SortedSetScoresAsync("key", []).GetArgs()),

            // SortedSetPopMin / SortedSetPopMax
            () => Assert.Equal(["ZPOPMIN", "key"], Request.SortedSetPopMinAsync("key").GetArgs()),
            () => Assert.Equal(["ZPOPMAX", "key"], Request.SortedSetPopMaxAsync("key").GetArgs()),
            () => Assert.Equal(["ZPOPMIN", "key", "3"], Request.SortedSetPopMinAsync("key", 3).GetArgs()),
            () => Assert.Equal(["ZPOPMAX", "key", "5"], Request.SortedSetPopMaxAsync("key", 5).GetArgs()),

            // SortedSetPopMin / SortedSetPopMax - Multi-key (ZMPOP)
            () => Assert.Equal(["ZMPOP", "2", "key1", "key2", "MIN", "COUNT", "1"], Request.SortedSetPopMinAsync(["key1", "key2"]).GetArgs()),
            () => Assert.Equal(["ZMPOP", "2", "key1", "key2", "MAX", "COUNT", "1"], Request.SortedSetPopMaxAsync(["key1", "key2"]).GetArgs()),
            () => Assert.Equal(["ZMPOP", "2", "key1", "key2", "MIN", "COUNT", "3"], Request.SortedSetPopMinAsync(["key1", "key2"], 3).GetArgs()),
            () => Assert.Equal(["ZMPOP", "2", "key1", "key2", "MAX", "COUNT", "5"], Request.SortedSetPopMaxAsync(["key1", "key2"], 5).GetArgs()),

            // SortedSetPopMin / SortedSetPopMax - Blocking multi-key (BZMPOP)
            () => Assert.Equal(["BZMPOP", "2", "2", "key1", "key2", "MIN", "COUNT", "1"], Request.SortedSetPopMinAsync(["key1", "key2"], TimeSpan.FromSeconds(2)).GetArgs()),
            () => Assert.Equal(["BZMPOP", "2", "2", "key1", "key2", "MAX", "COUNT", "1"], Request.SortedSetPopMaxAsync(["key1", "key2"], TimeSpan.FromSeconds(2)).GetArgs()),
            () => Assert.Equal(["BZMPOP", "5", "1", "key1", "MIN", "COUNT", "3"], Request.SortedSetPopMinAsync(["key1"], 3, TimeSpan.FromSeconds(5)).GetArgs()),
            () => Assert.Equal(["BZMPOP", "5", "1", "key1", "MAX", "COUNT", "3"], Request.SortedSetPopMaxAsync(["key1"], 3, TimeSpan.FromSeconds(5)).GetArgs()),

            // SortedSetRandomMember
            () => Assert.Equal(["ZRANDMEMBER", "key"], Request.SortedSetRandomMemberAsync("key").GetArgs()),
            () => Assert.Equal(["ZRANDMEMBER", "key", "3"], Request.SortedSetRandomMembersAsync("key", 3).GetArgs()),
            () => Assert.Equal(["ZRANDMEMBER", "key", "5", "WITHSCORES"], Request.SortedSetRandomMembersWithScoresAsync("key", 5).GetArgs()),
            () => Assert.Equal(["ZRANDMEMBER", "key", "1", "WITHSCORES"], Request.SortedSetRandomMemberWithScoreAsync("key").GetArgs()),

            // SortedSetRemoveRange - by rank
            () => Assert.Equal(["ZREMRANGEBYRANK", "key", "0", "3"], Request.SortedSetRemoveRangeAsync("key", IndexRange.Between(0, 3)).GetArgs()),
            () => Assert.Equal(["ZREMRANGEBYRANK", "key", "0", "-1"], Request.SortedSetRemoveRangeAsync("key", IndexRange.FirstToLast).GetArgs()),
            // SortedSetRemoveRange - by score
            () => Assert.Equal(["ZREMRANGEBYSCORE", "key", "1", "10"], Request.SortedSetRemoveRangeAsync("key", ScoreRange.Between(1.0, 10.0)).GetArgs()),
            () => Assert.Equal(["ZREMRANGEBYSCORE", "key", "(1", "(10"], Request.SortedSetRemoveRangeAsync("key", ScoreRange.Between(ScoreBound.Exclusive(1.0), ScoreBound.Exclusive(10.0))).GetArgs()),
            () => Assert.Equal(["ZREMRANGEBYSCORE", "key", "-inf", "+inf"], Request.SortedSetRemoveRangeAsync("key", ScoreRange.MinToMax).GetArgs()),
            // SortedSetRemoveRange - by lex
            () => Assert.Equal(["ZREMRANGEBYLEX", "key", "[a", "[z"], Request.SortedSetRemoveRangeAsync("key", LexRange.Between("a", "z")).GetArgs()),
            () => Assert.Equal(["ZREMRANGEBYLEX", "key", "(a", "(z"], Request.SortedSetRemoveRangeAsync("key", LexRange.Between(LexBound.Exclusive("a"), LexBound.Exclusive("z"))).GetArgs()),

            // SortedSetRangeAndStore
            () => Assert.Equal(["ZRANGESTORE", "dest", "src", "0", "-1"], Request.SortedSetRangeAndStoreAsync("src", "dest", new() { Range = IndexRange.FirstToLast }).GetArgs()),
            () => Assert.Equal(["ZRANGESTORE", "dest", "src", "1", "3"], Request.SortedSetRangeAndStoreAsync("src", "dest", new() { Range = IndexRange.Between(1, 3) }).GetArgs()),
            () => Assert.Equal(["ZRANGESTORE", "dest", "src", "-inf", "+inf", "BYSCORE"], Request.SortedSetRangeAndStoreAsync("src", "dest", new() { Range = ScoreRange.MinToMax }).GetArgs()),
            () => Assert.Equal(["ZRANGESTORE", "dest", "src", "[a", "[z", "BYLEX"], Request.SortedSetRangeAndStoreAsync("src", "dest", new() { Range = LexRange.Between("a", "z") }).GetArgs()),

            // SortedSetRank
            () => Assert.Equal(["ZRANK", "key", "member"], Request.SortedSetRankAsync("key", "member").GetArgs()),
            () => Assert.Equal(["ZRANK", "key", "member"], Request.SortedSetRankAsync("key", "member", Order.Ascending).GetArgs()),
            () => Assert.Equal(["ZREVRANK", "key", "member"], Request.SortedSetRankAsync("key", "member", Order.Descending).GetArgs()),

            // SortedSetRankWithScore
            () => Assert.Equal(["ZRANK", "key", "member", "WITHSCORE"], Request.SortedSetRankWithScoreAsync("key", "member").GetArgs()),
            () => Assert.Equal(["ZRANK", "key", "member", "WITHSCORE"], Request.SortedSetRankWithScoreAsync("key", "member", Order.Ascending).GetArgs()),
            () => Assert.Equal(["ZREVRANK", "key", "member", "WITHSCORE"], Request.SortedSetRankWithScoreAsync("key", "member", Order.Descending).GetArgs()),

            // SortedSetScan
            () => Assert.Equal(["ZSCAN", "key", "0"], Request.SortedSetScanAsync("key", 0).GetArgs()),
            () => Assert.Equal(["ZSCAN", "key", "5"], Request.SortedSetScanAsync("key", 5).GetArgs()),
            () => Assert.Equal(["ZSCAN", "key", "0", "MATCH", "pattern*"], Request.SortedSetScanAsync("key", 0, new ScanOptions { MatchPattern = "pattern*" }).GetArgs()),
            () => Assert.Equal(["ZSCAN", "key", "0", "COUNT", "20"], Request.SortedSetScanAsync("key", 0, new ScanOptions { Count = 20 }).GetArgs()),
            () => Assert.Equal(["ZSCAN", "key", "5", "MATCH", "pattern*", "COUNT", "20"], Request.SortedSetScanAsync("key", 5, new ScanOptions { MatchPattern = "pattern*", Count = 20 }).GetArgs()),
            () => Assert.Equal(["ZSCAN", "key", "10", "MATCH", "user:*", "COUNT", "50"], Request.SortedSetScanAsync("key", 10, new ScanOptions { MatchPattern = "user:*", Count = 50 }).GetArgs()),
            () => Assert.Equal(["ZSCAN", "key", "0", "MATCH", "*"], Request.SortedSetScanAsync("key", 0, new ScanOptions { MatchPattern = "*" }).GetArgs()),
            () => Assert.Equal(["ZSCAN", "key", "100"], Request.SortedSetScanAsync("key", 100).GetArgs()),

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
            () => Assert.Equal(3L, Request.SortedSetCountAsync("key", ScoreRange.Between(1.0, 10.0)).Converter(3L)),
            () => Assert.Equal(0L, Request.SortedSetCountAsync("key", ScoreRange.MinToMax).Converter(0L)),

            // Type converter test
            () => Assert.Equal(ValkeyType.SortedSet, Request.TypeAsync("key").Converter("zset"))
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

            // Test SortedSetUnionWithScoreAsync converter
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

            // Test SortedSetRankWithScoreAsync converter
            () =>
            {
                object[] response = [2L, 10.5];
                (long Rank, double Score)? result = Request.SortedSetRankWithScoreAsync("key", "member").Converter(response);
                _ = Assert.NotNull(result);
                Assert.Equal(2L, result.Value.Rank);
                Assert.Equal(10.5, result.Value.Score);
            },

            // Test SortedSetRankWithScoreAsync converter - null result
            () =>
            {
                (long Rank, double Score)? result = Request.SortedSetRankWithScoreAsync("key", "member").Converter([]);
                Assert.Null(result);
            },

            // Test SortedSetScanAsync converter - basic case
            () =>
            {
                object[] testScanResponse = [
                    5L,
                    new object[] { (gs)"member1", (gs)"10.5", (gs)"member2", (gs)"8.25" }
                ];
                (long cursor, SortedSetEntry[] items) = Request.SortedSetScanAsync("key", 0).Converter(testScanResponse);
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
                (long cursor, SortedSetEntry[] items) = Request.SortedSetScanAsync("key", 0).Converter(testScanResponse);
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
                (long cursor, SortedSetEntry[] items) = Request.SortedSetScanAsync("key", 0).Converter(testScanResponse);
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
                (long cursor, SortedSetEntry[] items) = Request.SortedSetScanAsync("key", 0).Converter(testScanResponse);
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
}
