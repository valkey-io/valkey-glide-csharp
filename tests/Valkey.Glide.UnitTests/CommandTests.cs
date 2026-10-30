// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands.Options;

using static Valkey.Glide.Internals.FFI;

namespace Valkey.Glide.UnitTests;

public class CommandTests
{
    // Reponse values for testing converters.
    private const long ConvertLong = 100L;
    private const double ConvertDouble = 1.5;

    private static readonly TimeSpan OneSecond = TimeSpan.FromSeconds(1);

    [Fact]
    public void ValidateCommandArgs() => Assert.Multiple(
        () => CmdAssert.Equal(RequestType.CustomCommand, ["get", "a"], Request.CustomCommand(["get", "a"])),
        () => CmdAssert.Equal(RequestType.CustomCommand, ["ping", "pong", "pang"], Request.CustomCommand(["ping", "pong", "pang"])),
        () => CmdAssert.Equal(RequestType.CustomCommand, ["get"], Request.CustomCommand(["get"])),
        () => CmdAssert.Equal(RequestType.CustomCommand, [], Request.CustomCommand([])),

        // String Commands
        () => CmdAssert.Equal(RequestType.Set, ["key", "value"], Request.Set("key", "value", new SetOptions())),
        () => CmdAssert.Equal(RequestType.Set, ["key", "value", "NX"], Request.Set("key", "value", new SetOptions { Condition = SetCondition.OnlyIfDoesNotExist })),
        () => CmdAssert.Equal(RequestType.Set, ["key", "value", "XX"], Request.Set("key", "value", new SetOptions { Condition = SetCondition.OnlyIfExists })),
        () => CmdAssert.Equal(RequestType.Set, ["key", "value", "IFEQ", "other"], Request.Set("key", "value", new SetOptions { Condition = SetCondition.OnlyIfEqual("other") })),
        () => CmdAssert.Equal(RequestType.Set, ["key", "value", "IFNE", "other"], Request.Set("key", "value", new SetOptions { Condition = SetCondition.OnlyIfNotEqual("other") })),
        () => CmdAssert.Equal(RequestType.Set, ["key", "value", "NX", "PX", "5000"], Request.Set("key", "value", new SetOptions { Condition = SetCondition.OnlyIfDoesNotExist, Expiry = SetExpiryOptions.ExpireIn(TimeSpan.FromSeconds(5)) })),
        () => CmdAssert.Equal(RequestType.Set, ["key", "value", "PX", "10000"], Request.Set("key", "value", new SetOptions { Expiry = SetExpiryOptions.ExpireIn(TimeSpan.FromSeconds(10)) })),
        () => CmdAssert.Equal(RequestType.Set, ["key", "value", "KEEPTTL"], Request.Set("key", "value", new SetOptions { Expiry = SetExpiryOptions.KeepTimeToLive() })),
        () => CmdAssert.Equal(RequestType.Set, ["key", "value", "GET"], Request.GetSet("key", "value", new SetOptions())),
        () => CmdAssert.Equal(RequestType.Set, ["key", "value", "NX", "GET"], Request.GetSet("key", "value", new SetOptions { Condition = SetCondition.OnlyIfDoesNotExist })),
        () => CmdAssert.Equal(RequestType.Set, ["key", "value", "XX", "GET"], Request.GetSet("key", "value", new SetOptions { Condition = SetCondition.OnlyIfExists })),
        () => CmdAssert.Equal(RequestType.Set, ["key", "value", "IFEQ", "other", "GET"], Request.GetSet("key", "value", new SetOptions { Condition = SetCondition.OnlyIfEqual("other") })),
        () => CmdAssert.Equal(RequestType.Set, ["key", "value", "IFNE", "other", "GET"], Request.GetSet("key", "value", new SetOptions { Condition = SetCondition.OnlyIfNotEqual("other") })),
        () => CmdAssert.Equal(RequestType.Set, ["key", "value", "NX", "PX", "5000", "GET"], Request.GetSet("key", "value", new SetOptions { Condition = SetCondition.OnlyIfDoesNotExist, Expiry = SetExpiryOptions.ExpireIn(TimeSpan.FromSeconds(5)) })),
        () => CmdAssert.Equal(RequestType.Set, ["key", "value", "PX", "10000", "GET"], Request.GetSet("key", "value", new SetOptions { Expiry = SetExpiryOptions.ExpireIn(TimeSpan.FromSeconds(10)) })),
        () => CmdAssert.Equal(RequestType.Get, ["key"], Request.Get("key")),
        () => CmdAssert.Equal(RequestType.MGet, ["key1", "key2", "key3"], Request.Get(["key1", "key2", "key3"])),
        () => CmdAssert.Equal(RequestType.MSet, ["key1", "value1", "key2", "value2"], Request.Set([new KeyValuePair<ValkeyKey, ValkeyValue>("key1", "value1"), new KeyValuePair<ValkeyKey, ValkeyValue>("key2", "value2")])),
        () => CmdAssert.Equal(RequestType.Strlen, ["key"], Request.Length("key")),
        () => CmdAssert.Equal(RequestType.GetRange, ["key", "0", "5"], Request.GetRange("key", 0, 5)),
        () => CmdAssert.Equal(RequestType.SetRange, ["key", "10", "value"], Request.SetRange("key", 10, "value")),
        () => CmdAssert.Equal(RequestType.Append, ["key", "value"], Request.Append("key", "value")),
        () => Assert.Equal(11L, Request.Append("key", "value").Converter(11L)),
        () => CmdAssert.Equal(RequestType.Decr, ["key"], Request.Decrement("key")),
        () => CmdAssert.Equal(RequestType.DecrBy, ["key", "5"], Request.DecrementBy("key", 5)),
        () => CmdAssert.Equal(RequestType.DelIfEq, ["key", "value"], Request.DeleteIfEqual("key", "value")),
        () => CmdAssert.Equal(RequestType.Incr, ["key"], Request.Increment("key")),
        () => CmdAssert.Equal(RequestType.IncrBy, ["key", "5"], Request.IncrementBy("key", 5)),
        () => CmdAssert.Equal(RequestType.IncrByFloat, ["key", "0.5"], Request.IncrementByFloat("key", 0.5)),
        () => CmdAssert.Equal(RequestType.MSetNX, ["key1", "value1", "key2", "value2"], Request.SetIfNotExists([new KeyValuePair<ValkeyKey, ValkeyValue>("key1", "value1"), new KeyValuePair<ValkeyKey, ValkeyValue>("key2", "value2")])),
        () => CmdAssert.Equal(RequestType.MSetNX, [], Request.SetIfNotExists([])),
        () => CmdAssert.Equal(RequestType.GetDel, ["key"], Request.GetDelete("key")),
        () => CmdAssert.Equal(RequestType.GetDel, ["test_key"], Request.GetDelete("test_key")),
        () => CmdAssert.Equal(RequestType.GetEx, ["key", "PX", "60000"], Request.GetExpiry("key", GetExpiryOptions.ExpireIn(TimeSpan.FromSeconds(60)))),
        () => CmdAssert.Equal(RequestType.GetEx, ["test_key", "PX", "60000"], Request.GetExpiry("test_key", GetExpiryOptions.ExpireIn(TimeSpan.FromSeconds(60)))),
        () => CmdAssert.Equal(RequestType.GetEx, ["key", "PERSIST"], Request.GetExpiry("key", GetExpiryOptions.Persist())),
        () => CmdAssert.Equal(RequestType.GetEx, ["test_key", "PERSIST"], Request.GetExpiry("test_key", GetExpiryOptions.Persist())),
        () => CmdAssert.Equal(RequestType.GetEx, ["key", "PXAT", "1609459200000"], Request.GetExpiry("key", GetExpiryOptions.ExpireAt(new DateTimeOffset(2021, 1, 1, 0, 0, 0, TimeSpan.Zero)))),
        () => CmdAssert.Equal(RequestType.GetEx, ["test_key", "PXAT", "1609459200000"], Request.GetExpiry("test_key", GetExpiryOptions.ExpireAt(new DateTimeOffset(2021, 1, 1, 0, 0, 0, TimeSpan.Zero)))),
        () => CmdAssert.Equal(RequestType.LCS, ["key1", "key2"], Request.LongestCommonSubsequence("key1", "key2")),
        () => CmdAssert.Equal(RequestType.LCS, ["key1", "key2", "LEN"], Request.LongestCommonSubsequenceLength("key1", "key2")),
        () => CmdAssert.Equal(RequestType.LCS, ["key1", "key2", "IDX", "MINMATCHLEN", "0", "WITHMATCHLEN"], Request.LongestCommonSubsequenceWithMatches("key1", "key2")),
        () => CmdAssert.Equal(RequestType.LCS, ["key1", "key2", "IDX", "MINMATCHLEN", "5", "WITHMATCHLEN"], Request.LongestCommonSubsequenceWithMatches("key1", "key2", 5)),
        () => CmdAssert.Equal(RequestType.LCS, ["key1", "key2", "IDX", "MINMATCHLEN", "0", "WITHMATCHLEN"], Request.LongestCommonSubsequenceWithMatches("key1", "key2", 0)),

        // Info Command Args
        () => CmdAssert.Equal(RequestType.Info, [], Request.Info([])),
        () => CmdAssert.Equal(RequestType.Info, ["SERVER"], Request.Info([InfoOptions.Section.SERVER])),
        () => CmdAssert.Equal(RequestType.Info, ["CLIENTS"], Request.Info([InfoOptions.Section.CLIENTS])),
        () => CmdAssert.Equal(RequestType.Info, ["MEMORY"], Request.Info([InfoOptions.Section.MEMORY])),
        () => CmdAssert.Equal(RequestType.Info, ["PERSISTENCE"], Request.Info([InfoOptions.Section.PERSISTENCE])),
        () => CmdAssert.Equal(RequestType.Info, ["STATS"], Request.Info([InfoOptions.Section.STATS])),
        () => CmdAssert.Equal(RequestType.Info, ["REPLICATION"], Request.Info([InfoOptions.Section.REPLICATION])),
        () => CmdAssert.Equal(RequestType.Info, ["CPU"], Request.Info([InfoOptions.Section.CPU])),
        () => CmdAssert.Equal(RequestType.Info, ["COMMANDSTATS"], Request.Info([InfoOptions.Section.COMMANDSTATS])),
        () => CmdAssert.Equal(RequestType.Info, ["LATENCYSTATS"], Request.Info([InfoOptions.Section.LATENCYSTATS])),
        () => CmdAssert.Equal(RequestType.Info, ["SENTINEL"], Request.Info([InfoOptions.Section.SENTINEL])),
        () => CmdAssert.Equal(RequestType.Info, ["CLUSTER"], Request.Info([InfoOptions.Section.CLUSTER])),
        () => CmdAssert.Equal(RequestType.Info, ["CLUSTER_INFO"], Request.Info([InfoOptions.Section.CLUSTER_INFO])),
        () => CmdAssert.Equal(RequestType.Info, ["MODULES"], Request.Info([InfoOptions.Section.MODULES])),
        () => CmdAssert.Equal(RequestType.Info, ["KEYSPACE"], Request.Info([InfoOptions.Section.KEYSPACE])),
        () => CmdAssert.Equal(RequestType.Info, ["ERRORSTATS"], Request.Info([InfoOptions.Section.ERRORSTATS])),
        () => CmdAssert.Equal(RequestType.Info, ["ALL"], Request.Info([InfoOptions.Section.ALL])),
        () => CmdAssert.Equal(RequestType.Info, ["DEFAULT"], Request.Info([InfoOptions.Section.DEFAULT])),
        () => CmdAssert.Equal(RequestType.Info, ["EVERYTHING"], Request.Info([InfoOptions.Section.EVERYTHING])),
        () => CmdAssert.Equal(RequestType.Info, ["CLIENTS", "CPU"], Request.Info([InfoOptions.Section.CLIENTS, InfoOptions.Section.CPU])),
        () => CmdAssert.Equal(RequestType.Info, ["SERVER", "MEMORY", "STATS"], Request.Info([InfoOptions.Section.SERVER, InfoOptions.Section.MEMORY, InfoOptions.Section.STATS])),

        // Connection Management Commands
        () => CmdAssert.Equal(RequestType.ClientGetName, [], Request.ClientGetName()),
        () => CmdAssert.Equal(RequestType.ClientId, [], Request.ClientId()),
        () => CmdAssert.Equal(RequestType.ClientPause, ["1000", "WRITE"], Request.ClientPauseWrite(OneSecond)),
        () => CmdAssert.Equal(RequestType.ClientPause, ["1000"], Request.ClientPause(OneSecond)),
        () => CmdAssert.Equal(RequestType.ClientTrackingInfo, [], Request.ClientTrackingInfo()),
        () => CmdAssert.Equal(RequestType.ClientUnpause, [], Request.ClientUnpause()),
        () => CmdAssert.Equal(RequestType.Echo, ["message"], Request.Echo("message")),
        () => CmdAssert.Equal(RequestType.Ping, [""], Request.Ping("")),
        () => CmdAssert.Equal(RequestType.Ping, ["Hello"], Request.Ping("Hello")),
        () => CmdAssert.Equal(RequestType.Ping, ["PONG"], Request.Ping("PONG")),
        () => CmdAssert.Equal(RequestType.Ping, ["test message"], Request.Ping("test message")),
        () => CmdAssert.Equal(RequestType.Ping, [], Request.Ping()),
        () => CmdAssert.Equal(RequestType.Select, ["-1"], Request.Select(-1)),
        () => CmdAssert.Equal(RequestType.Select, ["0"], Request.Select(0)),
        () => CmdAssert.Equal(RequestType.Select, ["1"], Request.Select(1)),
        () => CmdAssert.Equal(RequestType.Select, ["15"], Request.Select(15)),

        // Server Management Commands
        () => CmdAssert.Equal(RequestType.BgRewriteAof, [], Request.BgRewriteAof()),
        () => CmdAssert.Equal(RequestType.BgSave, ["CANCEL"], Request.BackgroundSaveCancel()),
        () => CmdAssert.Equal(RequestType.BgSave, ["SCHEDULE"], Request.BackgroundSaveSchedule()),
        () => CmdAssert.Equal(RequestType.BgSave, [], Request.BackgroundSave()),
        () => CmdAssert.Equal(RequestType.ConfigGet, ["*"], Request.ConfigGet("*")),
        () => CmdAssert.Equal(RequestType.ConfigGet, ["*"], Request.ConfigGet()),
        () => CmdAssert.Equal(RequestType.ConfigGet, ["maxmemory", "lfu-decay-time"], Request.ConfigGet([(ValkeyValue)"maxmemory", (ValkeyValue)"lfu-decay-time"])),
        () => CmdAssert.Equal(RequestType.ConfigGet, ["maxmemory"], Request.ConfigGet("maxmemory")),
        () => CmdAssert.Equal(RequestType.ConfigResetStat, [], Request.ConfigResetStatistics()),
        () => CmdAssert.Equal(RequestType.ConfigRewrite, [], Request.ConfigRewrite()),
        () => CmdAssert.Equal(RequestType.ConfigSet, ["lfu-decay-time", "5", "lfu-log-factor", "20"], Request.ConfigSet(new Dictionary<ValkeyValue, ValkeyValue> { { "lfu-decay-time", "5" }, { "lfu-log-factor", "20" } })),
        () => CmdAssert.Equal(RequestType.ConfigSet, ["maxmemory", "100mb"], Request.ConfigSet("maxmemory", "100mb")),
        () => CmdAssert.Equal(RequestType.ConfigSet, ["timeout", "300"], Request.ConfigSet("timeout", "300")),
        () => CmdAssert.Equal(RequestType.DBSize, [], Request.DatabaseSize()),
        () => CmdAssert.Equal(RequestType.DBSize, [], Request.DatabaseSize()),
        () => CmdAssert.Equal(RequestType.FailOver, ["ABORT"], Request.Failover(FailoverOptions.Abort())),
        () => CmdAssert.Equal(RequestType.FailOver, ["TIMEOUT", "5000"], Request.Failover(FailoverOptions.Timeout(TimeSpan.FromSeconds(5)))),
        () => CmdAssert.Equal(RequestType.FailOver, ["TO", "localhost", "6380", "FORCE", "TIMEOUT", "5000"], Request.Failover(FailoverOptions.Forced("localhost", 6380, TimeSpan.FromSeconds(5)))),
        () => CmdAssert.Equal(RequestType.FailOver, ["TO", "localhost", "6380", "TIMEOUT", "5000"], Request.Failover(FailoverOptions.To("localhost", 6380, TimeSpan.FromSeconds(5)))),
        () => CmdAssert.Equal(RequestType.FailOver, ["TO", "localhost", "6380"], Request.Failover(FailoverOptions.To("localhost", 6380))),
        () => CmdAssert.Equal(RequestType.FailOver, [], Request.Failover()),
        () => CmdAssert.Equal(RequestType.FlushAll, ["ASYNC"], Request.FlushAllDatabases(FlushMode.Async)),
        () => CmdAssert.Equal(RequestType.FlushAll, ["SYNC"], Request.FlushAllDatabases(FlushMode.Sync)),
        () => CmdAssert.Equal(RequestType.FlushAll, [], Request.FlushAllDatabases()),
        () => CmdAssert.Equal(RequestType.FlushDB, ["ASYNC"], Request.FlushDatabase(FlushMode.Async)),
        () => CmdAssert.Equal(RequestType.FlushDB, ["SYNC"], Request.FlushDatabase(FlushMode.Sync)),
        () => CmdAssert.Equal(RequestType.FlushDB, [], Request.FlushDatabase()),
        () => CmdAssert.Equal(RequestType.FlushDB, [], Request.FlushDatabase()),
        () => CmdAssert.Equal(RequestType.LastSave, [], Request.LastSave()),
        () => CmdAssert.Equal(RequestType.LatencyHistory, ["command"], Request.LatencyHistory("command")),
        () => CmdAssert.Equal(RequestType.LatencyLatest, [], Request.LatencyLatest()),
        () => CmdAssert.Equal(RequestType.LatencyReset, ["command", "fast-command"], Request.LatencyReset(["command", "fast-command"])),
        () => CmdAssert.Equal(RequestType.LatencyReset, ["command"], Request.LatencyReset(["command"])),
        () => CmdAssert.Equal(RequestType.LatencyReset, [], Request.LatencyReset([])),
        () => CmdAssert.Equal(RequestType.Lolwut, ["40", "20"], Request.Lolwut(new LolwutOptions { Parameters = [40, 20] })),
        () => CmdAssert.Equal(RequestType.Lolwut, ["VERSION", "5"], Request.Lolwut(new LolwutOptions { Version = 5 })),
        () => CmdAssert.Equal(RequestType.Lolwut, ["VERSION", "6", "40", "20"], Request.Lolwut(new LolwutOptions { Version = 6, Parameters = [40, 20] })),
        () => CmdAssert.Equal(RequestType.Lolwut, [], Request.Lolwut()),
        () => CmdAssert.Equal(RequestType.Lolwut, [], Request.Lolwut(options: null)),
        () => CmdAssert.Equal(RequestType.MemoryDoctor, [], Request.MemoryDoctor()),
        () => CmdAssert.Equal(RequestType.MemoryMallocStats, [], Request.MemoryMallocStats()),
        () => CmdAssert.Equal(RequestType.MemoryPurge, [], Request.MemoryPurge()),
        () => CmdAssert.Equal(RequestType.MemoryStats, [], Request.MemoryStats()),
        () => CmdAssert.Equal(RequestType.ReplicaOf, ["localhost", "6379"], Request.ReplicaOf("localhost", 6379)),
        () => CmdAssert.Equal(RequestType.ReplicaOf, ["NO", "ONE"], Request.ReplicaOfNoOne()),
        () => CmdAssert.Equal(RequestType.Reset, [], Request.Reset()),
        () => CmdAssert.Equal(RequestType.Save, [], Request.Save()),
        () => CmdAssert.Equal(RequestType.Time, [], Request.Time()),

        // Set Commands
        () => CmdAssert.Equal(RequestType.SAdd, ["key", "member"], Request.SetAdd("key", "member")),
        () => CmdAssert.Equal(RequestType.SAdd, ["key", "member1", "member2"], Request.SetAdd("key", ["member1", "member2"])),
        () => CmdAssert.Equal(RequestType.SRem, ["key", "member"], Request.SetRemove("key", "member")),
        () => CmdAssert.Equal(RequestType.SRem, ["key", "member1", "member2"], Request.SetRemove("key", ["member1", "member2"])),
        () => CmdAssert.Equal(RequestType.SMembers, ["key"], Request.SetMembers("key")),
        () => CmdAssert.Equal(RequestType.SCard, ["key"], Request.SetCard("key")),
        () => CmdAssert.Equal(RequestType.SInterCard, ["2", "key1", "key2"], Request.SetInterCard(["key1", "key2"])),
        () => CmdAssert.Equal(RequestType.SInterCard, ["2", "key1", "key2", "LIMIT", "10"], Request.SetInterCard(["key1", "key2"], 10)),
        () => CmdAssert.Equal(RequestType.SPop, ["key"], Request.SetPop("key")),
        () => CmdAssert.Equal(RequestType.SPop, ["key", "3"], Request.SetPop("key", 3)),
        () => CmdAssert.Equal(RequestType.SUnion, ["key1", "key2"], Request.SetUnion(["key1", "key2"])),
        () => CmdAssert.Equal(RequestType.SInter, ["key1", "key2"], Request.SetInter(["key1", "key2"])),
        () => CmdAssert.Equal(RequestType.SDiff, ["key1", "key2"], Request.SetDiff(["key1", "key2"])),
        () => CmdAssert.Equal(RequestType.SUnionStore, ["dest", "key1", "key2"], Request.SetUnionStore("dest", ["key1", "key2"])),
        () => CmdAssert.Equal(RequestType.SInterStore, ["dest", "key1", "key2"], Request.SetInterStore("dest", ["key1", "key2"])),
        () => CmdAssert.Equal(RequestType.SDiffStore, ["dest", "key1", "key2"], Request.SetDiffStore("dest", ["key1", "key2"])),
        () => CmdAssert.Equal(RequestType.SIsMember, ["key", "member"], Request.SetIsMember("key", "member")),
        () => CmdAssert.Equal(RequestType.SIsMember, ["mykey", "value"], Request.SetIsMember("mykey", "value")),
        () => CmdAssert.Equal(RequestType.SIsMember, ["test:set", "test-member"], Request.SetIsMember("test:set", "test-member")),
        () => CmdAssert.Equal(RequestType.SMIsMember, ["key", "member1", "member2", "member3"], Request.SetIsMember("key", ["member1", "member2", "member3"])),
        () => CmdAssert.Equal(RequestType.SIsMember, ["key", "member", "XX"], Request.SetIsMemberIfExists("key", "member")),
        () => CmdAssert.Equal(RequestType.SMIsMember, ["key"], Request.SetIsMember("key", [])),
        () => CmdAssert.Equal(RequestType.SMIsMember, ["key", "", " ", "null", "0", "-1"], Request.SetIsMember("key", ["", " ", "null", "0", "-1"])),
        () => CmdAssert.Equal(RequestType.SRandMember, ["key"], Request.SetRandomMember("key")),
        () => CmdAssert.Equal(RequestType.SRandMember, ["mykey"], Request.SetRandomMember("mykey")),
        () => CmdAssert.Equal(RequestType.SRandMember, ["test:set"], Request.SetRandomMember("test:set")),
        () => CmdAssert.Equal(RequestType.SRandMember, ["key", "3"], Request.SetRandomMembers("key", 3)),
        () => CmdAssert.Equal(RequestType.SRandMember, ["key", "-5"], Request.SetRandomMembers("key", -5)),
        () => CmdAssert.Equal(RequestType.SRandMember, ["key", "0"], Request.SetRandomMembers("key", 0)),
        () => CmdAssert.Equal(RequestType.SRandMember, ["key", "1"], Request.SetRandomMembers("key", 1)),
        () => CmdAssert.Equal(RequestType.SMove, ["source", "dest", "member"], Request.SetMove("source", "dest", "member")),
        () => CmdAssert.Equal(RequestType.SMove, ["key1", "key2", "value"], Request.SetMove("key1", "key2", "value")),
        () => CmdAssert.Equal(RequestType.SMove, ["src:set", "dst:set", "test-member"], Request.SetMove("src:set", "dst:set", "test-member")),
        () => CmdAssert.Equal(RequestType.SScan, ["key", "0"], Request.SetScan("key", 0)),
        () => CmdAssert.Equal(RequestType.SScan, ["key", "10"], Request.SetScan("key", 10)),
        () => CmdAssert.Equal(RequestType.SScan, ["mykey", "0"], Request.SetScan("mykey", 0)),
        () => CmdAssert.Equal(RequestType.SScan, ["key", "0", "MATCH", "pattern*"], Request.SetScan("key", 0, new ScanOptions { MatchPattern = "pattern*" })),
        () => CmdAssert.Equal(RequestType.SScan, ["key", "5", "MATCH", "test*"], Request.SetScan("key", 5, new ScanOptions { MatchPattern = "test*" })),
        () => CmdAssert.Equal(RequestType.SScan, ["key", "0", "MATCH", "*suffix"], Request.SetScan("key", 0, new ScanOptions { MatchPattern = "*suffix" })),
        () => CmdAssert.Equal(RequestType.SScan, ["key", "0", "COUNT", "10"], Request.SetScan("key", 0, new ScanOptions { Count = 10 })),
        () => CmdAssert.Equal(RequestType.SScan, ["key", "5", "COUNT", "20"], Request.SetScan("key", 5, new ScanOptions { Count = 20 })),
        () => CmdAssert.Equal(RequestType.SScan, ["key", "0", "COUNT", "1"], Request.SetScan("key", 0, new ScanOptions { Count = 1 })),
        () => CmdAssert.Equal(RequestType.SScan, ["key", "0", "MATCH", "pattern*", "COUNT", "10"], Request.SetScan("key", 0, new ScanOptions { MatchPattern = "pattern*", Count = 10 })),
        () => CmdAssert.Equal(RequestType.SScan, ["key", "5", "MATCH", "test*", "COUNT", "20"], Request.SetScan("key", 5, new ScanOptions { MatchPattern = "test*", Count = 20 })),
        () => CmdAssert.Equal(RequestType.SScan, ["key", "10", "MATCH", "*suffix", "COUNT", "5"], Request.SetScan("key", 10, new ScanOptions { MatchPattern = "*suffix", Count = 5 })),
        () => CmdAssert.Equal(RequestType.SIsMember, ["", "member"], Request.SetIsMember("", "member")),
        () => CmdAssert.Equal(RequestType.SIsMember, ["key", ""], Request.SetIsMember("key", "")),
        () => CmdAssert.Equal(RequestType.SMove, ["", "", ""], Request.SetMove("", "", "")),
        () => CmdAssert.Equal(RequestType.SScan, ["", "0"], Request.SetScan("", 0)),

        // Generic Commands
        () => CmdAssert.Equal(RequestType.Copy, ["src", "dest", "DB", "1", "REPLACE"], Request.Copy("src", "dest", 1, true)),
        () => CmdAssert.Equal(RequestType.Copy, ["src", "dest"], Request.Copy("src", "dest")),
        () => CmdAssert.Equal(RequestType.Del, ["key"], Request.Delete("key")),
        () => CmdAssert.Equal(RequestType.Del, ["key1", "key2"], Request.Delete(["key1", "key2"])),
        () => CmdAssert.Equal(RequestType.Dump, ["key"], Request.Dump("key")),
        () => CmdAssert.Equal(RequestType.Exists, ["key"], Request.Exists("key")),
        () => CmdAssert.Equal(RequestType.Exists, ["key1", "key2"], Request.Exists(["key1", "key2"])),
        () => CmdAssert.Equal(RequestType.Migrate, ["host", "6379", "", "0", "5000", "KEYS", "k1", "k2"], Request.Migrate(["k1", "k2"], new MigrateOptions("host", 6379, 0, TimeSpan.FromSeconds(5)))),
        () => CmdAssert.Equal(RequestType.Migrate, ["host", "6379", "key", "0", "5000", "COPY", "REPLACE"], Request.Migrate(["key"], new MigrateOptions("host", 6379, 0, TimeSpan.FromSeconds(5)).WithCopy().WithReplace())),
        () => CmdAssert.Equal(RequestType.Move, ["key", "1"], Request.Move("key", 1)),
        () => CmdAssert.Equal(RequestType.ObjectEncoding, ["key"], Request.ObjectEncoding("key")),
        () => CmdAssert.Equal(RequestType.ObjectFreq, ["key"], Request.ObjectFrequency("key")),
        () => CmdAssert.Equal(RequestType.ObjectIdleTime, ["key"], Request.ObjectIdleTime("key")),
        () => CmdAssert.Equal(RequestType.ObjectRefCount, ["key"], Request.ObjectRefCount("key")),
        () => CmdAssert.Equal(RequestType.Persist, ["key"], Request.Persist("key")),
        () => CmdAssert.Equal(RequestType.PExpire, ["key", "60000", "NX"], Request.Expire("key", TimeSpan.FromSeconds(60), ExpireCondition.OnlyIfNotExists)),
        () => CmdAssert.Equal(RequestType.PExpire, ["key", "60000"], Request.Expire("key", TimeSpan.FromSeconds(60))),
        () => CmdAssert.Equal(RequestType.PExpireAt, ["key", "1609459200000"], Request.Expire("key", new DateTimeOffset(2021, 1, 1, 0, 0, 0, TimeSpan.Zero))),
        () => CmdAssert.Equal(RequestType.PExpireTime, ["key"], Request.ExpireTime("key")),
        () => CmdAssert.Equal(RequestType.PTTL, ["key"], Request.TimeToLive("key")),
        () => CmdAssert.Equal(RequestType.RandomKey, [], Request.RandomKey()),
        () => CmdAssert.Equal(RequestType.Rename, ["oldkey", "newkey"], Request.Rename("oldkey", "newkey")),
        () => CmdAssert.Equal(RequestType.RenameNX, ["oldkey", "newkey"], Request.RenameIfNotExists("oldkey", "newkey")),
        () => CmdAssert.Equal(RequestType.Restore, ["key", "0", "data", "FREQ", "5"], Request.Restore("key", "data"u8.ToArray(), new RestoreOptions { Frequency = 5 })),
        () => CmdAssert.Equal(RequestType.Restore, ["key", "0", "data", "IDLETIME", "1000"], Request.Restore("key", "data"u8.ToArray(), new RestoreOptions { IdleTime = 1000 })),
        () => CmdAssert.Equal(RequestType.Restore, ["key", "0", "data", "REPLACE", "FREQ", "10"], Request.Restore("key", "data"u8.ToArray(), new RestoreOptions { Replace = true, Frequency = 10 })),
        () => CmdAssert.Equal(RequestType.Restore, ["key", "0", "data", "REPLACE", "IDLETIME", "2000"], Request.Restore("key", "data"u8.ToArray(), new RestoreOptions { Replace = true, IdleTime = 2000 })),
        () => CmdAssert.Equal(RequestType.Restore, ["key", "0", "data", "REPLACE"], Request.Restore("key", "data"u8.ToArray(), new RestoreOptions { Replace = true })),
        () => CmdAssert.Equal(RequestType.Restore, ["key", "0", "data"], Request.Restore("key", "data"u8.ToArray())),
        () => CmdAssert.Equal(RequestType.Restore, ["key", "2303596800000", "data", "ABSTTL", "FREQ", "10"], Request.Restore("key", "data"u8.ToArray(), new RestoreOptions { ExpireAt = new DateTimeOffset(2042, 12, 31, 0, 0, 0, TimeSpan.Zero), Frequency = 10 })),
        () => CmdAssert.Equal(RequestType.Restore, ["key", "2303596800000", "data", "ABSTTL", "REPLACE"], Request.Restore("key", "data"u8.ToArray(), new RestoreOptions { ExpireAt = new DateTimeOffset(2042, 12, 31, 0, 0, 0, TimeSpan.Zero), Replace = true })),
        () => CmdAssert.Equal(RequestType.Restore, ["key", "2303596800000", "data", "ABSTTL"], Request.Restore("key", "data"u8.ToArray(), new RestoreOptions { ExpireAt = new DateTimeOffset(2042, 12, 31, 0, 0, 0, TimeSpan.Zero) })),
        () => CmdAssert.Equal(RequestType.Restore, ["key", "5000", "data", "IDLETIME", "1000"], Request.Restore("key", "data"u8.ToArray(), new RestoreOptions { Ttl = TimeSpan.FromSeconds(5), IdleTime = 1000 })),
        () => CmdAssert.Equal(RequestType.Restore, ["key", "5000", "data", "REPLACE"], Request.Restore("key", "data"u8.ToArray(), new RestoreOptions { Ttl = TimeSpan.FromSeconds(5), Replace = true })),
        () => CmdAssert.Equal(RequestType.Restore, ["key", "5000", "data"], Request.Restore("key", "data"u8.ToArray(), new RestoreOptions { Ttl = TimeSpan.FromSeconds(5) })),
        () => CmdAssert.Equal(RequestType.Touch, ["key"], Request.Touch("key")),
        () => CmdAssert.Equal(RequestType.Touch, ["key1", "key2"], Request.Touch(["key1", "key2"])),
        () => CmdAssert.Equal(RequestType.Type, ["key"], Request.Type("key")),
        () => CmdAssert.Equal(RequestType.Unlink, ["key"], Request.Unlink("key")),
        () => CmdAssert.Equal(RequestType.Unlink, ["key1", "key2"], Request.Unlink(["key1", "key2"])),

        () => Assert.Throws<ArgumentException>(() => Request.Restore("key", "data"u8.ToArray(), new RestoreOptions { IdleTime = 1000, Frequency = 5 })),
        () => Assert.Throws<ArgumentException>(() => Request.Restore("key", "data"u8.ToArray(), new RestoreOptions { Ttl = TimeSpan.FromSeconds(5), ExpireAt = DateTimeOffset.UtcNow })),

        // SCAN Commands
        () => CmdAssert.Equal(RequestType.Scan, ["0"], Request.Scan("0")),
        () => CmdAssert.Equal(RequestType.Scan, ["10"], Request.Scan("10")),
        () => CmdAssert.Equal(RequestType.Scan, ["0", "MATCH", "pattern*"], Request.Scan("0", new ScanOptions { MatchPattern = "pattern*" })),
        () => CmdAssert.Equal(RequestType.Scan, ["5", "MATCH", "test*"], Request.Scan("5", new ScanOptions { MatchPattern = "test*" })),
        () => CmdAssert.Equal(RequestType.Scan, ["0", "COUNT", "10"], Request.Scan("0", new ScanOptions { Count = 10 })),
        () => CmdAssert.Equal(RequestType.Scan, ["5", "COUNT", "20"], Request.Scan("5", new ScanOptions { Count = 20 })),
        () => CmdAssert.Equal(RequestType.Scan, ["0", "TYPE", "string"], Request.Scan("0", new ScanOptions { Type = ValkeyType.String })),
        () => CmdAssert.Equal(RequestType.Scan, ["5", "TYPE", "list"], Request.Scan("5", new ScanOptions { Type = ValkeyType.List })),
        () => CmdAssert.Equal(RequestType.Scan, ["0", "TYPE", "set"], Request.Scan("0", new ScanOptions { Type = ValkeyType.Set })),
        () => CmdAssert.Equal(RequestType.Scan, ["0", "TYPE", "zset"], Request.Scan("0", new ScanOptions { Type = ValkeyType.SortedSet })),
        () => CmdAssert.Equal(RequestType.Scan, ["0", "TYPE", "hash"], Request.Scan("0", new ScanOptions { Type = ValkeyType.Hash })),
        () => CmdAssert.Equal(RequestType.Scan, ["0", "TYPE", "stream"], Request.Scan("0", new ScanOptions { Type = ValkeyType.Stream })),
        () => CmdAssert.Equal(RequestType.Scan, ["10", "MATCH", "key*", "COUNT", "20", "TYPE", "string"], Request.Scan("10", new ScanOptions { MatchPattern = "key*", Count = 20, Type = ValkeyType.String })),

        // WAIT Commands
        () => CmdAssert.Equal(RequestType.Wait, ["1", "1000"], Request.Wait(1, OneSecond)),
        () => CmdAssert.Equal(RequestType.Wait, ["0", "0"], Request.Wait(0, TimeSpan.Zero)),
        () => CmdAssert.Equal(RequestType.Wait, ["3", "5000"], Request.Wait(3, TimeSpan.FromMilliseconds(5000))),
        () => CmdAssert.Equal(RequestType.WaitAof, ["1", "1", "1000"], Request.WaitAof(true, 1, OneSecond)),
        () => CmdAssert.Equal(RequestType.WaitAof, ["0", "0", "0"], Request.WaitAof(false, 0, TimeSpan.Zero)),
        () => CmdAssert.Equal(RequestType.WaitAof, ["1", "2", "5000"], Request.WaitAof(true, 2, TimeSpan.FromMilliseconds(5000))),

        // List Commands
        () => CmdAssert.Equal(RequestType.LPop, ["a"], Request.ListLeftPop("a")),
        () => CmdAssert.Equal(RequestType.LPop, ["a", "3"], Request.ListLeftPop("a", 3)),
        () => CmdAssert.Equal(RequestType.LPush, ["a", "value"], Request.ListLeftPush("a", "value")),
        () => CmdAssert.Equal(RequestType.LPush, ["a", "one", "two"], Request.ListLeftPush("a", ["one", "two"])),
        () => CmdAssert.Equal(RequestType.RPop, ["a"], Request.ListRightPop("a")),
        () => CmdAssert.Equal(RequestType.RPop, ["a", "2"], Request.ListRightPop("a", 2)),
        () => CmdAssert.Equal(RequestType.RPush, ["a", "value"], Request.ListRightPush("a", "value")),
        () => CmdAssert.Equal(RequestType.RPush, ["a", "one", "two"], Request.ListRightPush("a", ["one", "two"])),
        () => CmdAssert.Equal(RequestType.LLen, ["a"], Request.ListLength("a")),
        () => CmdAssert.Equal(RequestType.LRem, ["a", "0", "value"], Request.ListRemove("a", "value", 0)),
        () => CmdAssert.Equal(RequestType.LRem, ["a", "2", "value"], Request.ListRemove("a", "value", 2)),
        () => CmdAssert.Equal(RequestType.LRem, ["a", "-1", "value"], Request.ListRemove("a", "value", -1)),
        () => CmdAssert.Equal(RequestType.LTrim, ["a", "0", "10"], Request.ListTrim("a", 0, 10)),
        () => CmdAssert.Equal(RequestType.LTrim, ["a", "1", "-1"], Request.ListTrim("a", 1, -1)),
        () => CmdAssert.Equal(RequestType.LRange, ["a", "0", "-1"], Request.ListRange("a", 0, -1)),
        () => CmdAssert.Equal(RequestType.LRange, ["a", "1", "5"], Request.ListRange("a", 1, 5)),
        () => CmdAssert.Equal(RequestType.BLPop, ["key1", "key2", "5"], Request.ListBlockingLeftPop(["key1", "key2"], TimeSpan.FromSeconds(5))),
        () => CmdAssert.Equal(RequestType.BLPop, ["key", "0"], Request.ListBlockingLeftPop(["key"], TimeSpan.Zero)),
        () => CmdAssert.Equal(RequestType.BLPop, ["a", "b", "c", "10"], Request.ListBlockingLeftPop(["a", "b", "c"], TimeSpan.FromSeconds(10))),
        () => CmdAssert.Equal(RequestType.BLPop, ["single", "0.5"], Request.ListBlockingLeftPop(["single"], TimeSpan.FromMilliseconds(500))),
        () => CmdAssert.Equal(RequestType.BRPop, ["key1", "key2", "10"], Request.ListBlockingRightPop(["key1", "key2"], TimeSpan.FromSeconds(10))),
        () => CmdAssert.Equal(RequestType.BRPop, ["key", "0"], Request.ListBlockingRightPop(["key"], TimeSpan.Zero)),
        () => CmdAssert.Equal(RequestType.BRPop, ["x", "y", "z", "0.5"], Request.ListBlockingRightPop(["x", "y", "z"], TimeSpan.FromMilliseconds(500))),
        () => CmdAssert.Equal(RequestType.BRPop, ["test", "1.5"], Request.ListBlockingRightPop(["test"], TimeSpan.FromSeconds(1.5))),
        () => CmdAssert.Equal(RequestType.BLMove, ["source", "dest", "LEFT", "RIGHT", "3"], Request.ListBlockingMove("source", "dest", ListSide.Left, ListSide.Right, TimeSpan.FromSeconds(3))),
        () => CmdAssert.Equal(RequestType.BLMove, ["src", "dst", "RIGHT", "LEFT", "0"], Request.ListBlockingMove("src", "dst", ListSide.Right, ListSide.Left, TimeSpan.Zero)),
        () => CmdAssert.Equal(RequestType.BLMove, ["a", "b", "LEFT", "LEFT", "2.5"], Request.ListBlockingMove("a", "b", ListSide.Left, ListSide.Left, TimeSpan.FromSeconds(2.5))),
        () => CmdAssert.Equal(RequestType.BLMove, ["list1", "list2", "RIGHT", "RIGHT", "1"], Request.ListBlockingMove("list1", "list2", ListSide.Right, ListSide.Right, TimeSpan.FromSeconds(1))),
        () => CmdAssert.Equal(RequestType.BLMPop, ["2", "2", "key1", "key2", "LEFT"], Request.ListBlockingPop(["key1", "key2"], ListSide.Left, TimeSpan.FromSeconds(2))),
        () => CmdAssert.Equal(RequestType.BLMPop, ["0", "1", "key", "RIGHT"], Request.ListBlockingPop(["key"], ListSide.Right, TimeSpan.Zero)),
        () => CmdAssert.Equal(RequestType.BLMPop, ["1.5", "3", "a", "b", "c", "LEFT"], Request.ListBlockingPop(["a", "b", "c"], ListSide.Left, TimeSpan.FromSeconds(1.5))),
        () => CmdAssert.Equal(RequestType.BLMPop, ["0.25", "1", "single", "RIGHT"], Request.ListBlockingPop(["single"], ListSide.Right, TimeSpan.FromMilliseconds(250))),
        () => CmdAssert.Equal(RequestType.BLMPop, ["5", "2", "key1", "key2", "LEFT", "COUNT", "3"], Request.ListBlockingPop(["key1", "key2"], ListSide.Left, 3, TimeSpan.FromSeconds(5))),
        () => CmdAssert.Equal(RequestType.BLMPop, ["0", "1", "key", "RIGHT", "COUNT", "10"], Request.ListBlockingPop(["key"], ListSide.Right, 10, TimeSpan.Zero)),
        () => CmdAssert.Equal(RequestType.BLMPop, ["2.5", "4", "w", "x", "y", "z", "LEFT", "COUNT", "1"], Request.ListBlockingPop(["w", "x", "y", "z"], ListSide.Left, 1, TimeSpan.FromSeconds(2.5))),
        () => CmdAssert.Equal(RequestType.BLMPop, ["1", "1", "test", "RIGHT", "COUNT", "5"], Request.ListBlockingPop(["test"], ListSide.Right, 5, TimeSpan.FromSeconds(1))),
        () => CmdAssert.Equal(RequestType.LMPop, ["2", "key1", "key2", "LEFT", "COUNT", "3"], Request.ListLeftPop(["key1", "key2"], 3)),
        () => CmdAssert.Equal(RequestType.LMPop, ["2", "key1", "key2", "RIGHT", "COUNT", "3"], Request.ListRightPop(["key1", "key2"], 3)),
        () => CmdAssert.Equal(RequestType.LPushX, ["a", "value"], Request.ListLeftPush("a", "value", When.Exists)),
        () => CmdAssert.Equal(RequestType.LPushX, ["a", "one", "two"], Request.ListLeftPush("a", ["one", "two"], When.Exists)),
        () => CmdAssert.Equal(RequestType.RPushX, ["a", "value"], Request.ListRightPush("a", "value", When.Exists)),
        () => CmdAssert.Equal(RequestType.RPushX, ["a", "one", "two"], Request.ListRightPush("a", ["one", "two"], When.Exists)),
        () => CmdAssert.Equal(RequestType.LIndex, ["a", "0"], Request.ListGetByIndex("a", 0)),
        () => CmdAssert.Equal(RequestType.LIndex, ["a", "-1"], Request.ListGetByIndex("a", -1)),
        () => CmdAssert.Equal(RequestType.LInsert, ["a", "BEFORE", "pivot", "value"], Request.ListInsertBefore("a", "pivot", "value")),
        () => CmdAssert.Equal(RequestType.LInsert, ["a", "AFTER", "pivot", "value"], Request.ListInsertAfter("a", "pivot", "value")),
        () => CmdAssert.Equal(RequestType.LMove, ["src", "dest", "LEFT", "RIGHT"], Request.ListMove("src", "dest", ListSide.Left, ListSide.Right)),
        () => CmdAssert.Equal(RequestType.LMove, ["src", "dest", "RIGHT", "LEFT"], Request.ListMove("src", "dest", ListSide.Right, ListSide.Left)),
        () => CmdAssert.Equal(RequestType.LPos, ["a", "element"], Request.ListPosition("a", "element")),
        () => CmdAssert.Equal(RequestType.LPos, ["a", "element", "RANK", "2"], Request.ListPosition("a", "element", 2)),
        () => CmdAssert.Equal(RequestType.LPos, ["a", "element", "MAXLEN", "100"], Request.ListPosition("a", "element", 1, 100)),
        () => CmdAssert.Equal(RequestType.LPos, ["a", "element", "COUNT", "5"], Request.ListPositions("a", "element", 5)),
        () => CmdAssert.Equal(RequestType.LPos, ["a", "element", "COUNT", "5", "RANK", "2"], Request.ListPositions("a", "element", 5, 2)),
        () => CmdAssert.Equal(RequestType.LPos, ["a", "element", "COUNT", "5", "MAXLEN", "50"], Request.ListPositions("a", "element", 5, 1, 50)),
        () => CmdAssert.Equal(RequestType.LSet, ["a", "0", "value"], Request.ListSetByIndex("a", 0, "value")),
        () => CmdAssert.Equal(RequestType.LSet, ["a", "-1", "value"], Request.ListSetByIndex("a", -1, "value")),

        // Hash Commands
        () => CmdAssert.Equal(RequestType.HGet, ["key", "field"], Request.HashGet("key", "field")),
        () => CmdAssert.Equal(RequestType.HMGet, ["key", "field1", "field2"], Request.HashGet("key", ["field1", "field2"])),
        () => CmdAssert.Equal(RequestType.HGetAll, ["key"], Request.HashGet("key")),
        () => CmdAssert.Equal(RequestType.HSet, ["key", "field1", "value1", "field2", "value2"], Request.HashSet("key", [new KeyValuePair<ValkeyValue, ValkeyValue>("field1", "value1"), new KeyValuePair<ValkeyValue, ValkeyValue>("field2", "value2")])),
        () => CmdAssert.Equal(RequestType.HSet, ["key", "field", "value"], Request.HashSet("key", "field", "value")),
        () => CmdAssert.Equal(RequestType.HSetNX, ["key", "field", "value"], Request.HashSetNotExists("key", "field", "value")),
        () => CmdAssert.Equal(RequestType.HDel, ["key", "field"], Request.HashDelete("key", "field")),
        () => CmdAssert.Equal(RequestType.HDel, ["key", "field1", "field2"], Request.HashDelete("key", ["field1", "field2"])),
        () => CmdAssert.Equal(RequestType.HExists, ["key", "field"], Request.HashExists("key", "field")),
        () => CmdAssert.Equal(RequestType.HIncrBy, ["key", "field", "5"], Request.HashIncrementBy("key", "field", 5L)),
        () => CmdAssert.Equal(RequestType.HIncrBy, ["key", "field", "1"], Request.HashIncrementBy("key", "field", 1L)),
        () => CmdAssert.Equal(RequestType.HIncrByFloat, ["key", "field", "2.5"], Request.HashIncrementBy("key", "field", 2.5)),
        () => CmdAssert.Equal(RequestType.HKeys, ["key"], Request.HashKeys("key")),
        () => CmdAssert.Equal(RequestType.HLen, ["key"], Request.HashLength("key")),
        () => CmdAssert.Equal(RequestType.HStrlen, ["key", "field"], Request.HashStringLength("key", "field")),
        () => CmdAssert.Equal(RequestType.HVals, ["key"], Request.HashValues("key")),
        () => CmdAssert.Equal(RequestType.HRandField, ["key"], Request.HashRandomField("key")),
        () => CmdAssert.Equal(RequestType.HRandField, ["key", "3"], Request.HashRandomFields("key", 3)),
        () => CmdAssert.Equal(RequestType.HRandField, ["key", "3", "WITHVALUES"], Request.HashRandomFieldsWithValues("key", 3)),

        // Hash Field Expire Commands (Valkey 9.0+)
        () => CmdAssert.Equal(RequestType.HGetEx, ["key", "PX", "60000", "FIELDS", "2", "field1", "field2"], Request.HashGet("key", ["field1", "field2"], GetExpiryOptions.ExpireIn(TimeSpan.FromSeconds(60)))),
        () => CmdAssert.Equal(RequestType.HGetEx, ["key", "PX", "5000", "FIELDS", "1", "field1"], Request.HashGet("key", ["field1"], GetExpiryOptions.ExpireIn(TimeSpan.FromMilliseconds(5000)))),
        () => CmdAssert.Equal(RequestType.HGetEx, ["key", "PXAT", "1609459200000", "FIELDS", "1", "field1"], Request.HashGet("key", ["field1"], GetExpiryOptions.ExpireAt(DateTimeOffset.FromUnixTimeSeconds(1609459200)))),
        () => CmdAssert.Equal(RequestType.HGetEx, ["key", "PXAT", "1609459200000", "FIELDS", "1", "field1"], Request.HashGet("key", ["field1"], GetExpiryOptions.ExpireAt(DateTimeOffset.FromUnixTimeMilliseconds(1609459200000)))),
        () => CmdAssert.Equal(RequestType.HGetEx, ["key", "PERSIST", "FIELDS", "1", "field1"], Request.HashGet("key", ["field1"], GetExpiryOptions.Persist())),
        () => CmdAssert.Equal(RequestType.HSetEx, ["key", "PX", "60000", "FIELDS", "2", "field1", "value1", "field2", "value2"], Request.HashSet("key", [new KeyValuePair<ValkeyValue, ValkeyValue>("field1", "value1"), new KeyValuePair<ValkeyValue, ValkeyValue>("field2", "value2")], new HashSetOptions { Expiry = SetExpiryOptions.ExpireIn(TimeSpan.FromSeconds(60)) })),
        () => CmdAssert.Equal(RequestType.HSetEx, ["key", "PX", "5000", "FIELDS", "1", "field1", "value1"], Request.HashSet("key", [new KeyValuePair<ValkeyValue, ValkeyValue>("field1", "value1")], new HashSetOptions { Expiry = SetExpiryOptions.ExpireIn(TimeSpan.FromMilliseconds(5000)) })),
        () => CmdAssert.Equal(RequestType.HSetEx, ["key", "PXAT", "60000", "FIELDS", "2", "field1", "value1", "field2", "value2"], Request.HashSet("key", [new KeyValuePair<ValkeyValue, ValkeyValue>("field1", "value1"), new KeyValuePair<ValkeyValue, ValkeyValue>("field2", "value2")], new HashSetOptions { Expiry = SetExpiryOptions.ExpireAt(DateTimeOffset.FromUnixTimeMilliseconds(60000)) })),
        () => CmdAssert.Equal(RequestType.HSetEx, ["key", "PXAT", "5000", "FIELDS", "1", "field1", "value1"], Request.HashSet("key", [new KeyValuePair<ValkeyValue, ValkeyValue>("field1", "value1")], new HashSetOptions { Expiry = SetExpiryOptions.ExpireAt(DateTimeOffset.FromUnixTimeMilliseconds(5000)) })),
        () => CmdAssert.Equal(RequestType.HSetEx, ["key", "KEEPTTL", "FIELDS", "1", "field1", "value1"], Request.HashSet("key", [new KeyValuePair<ValkeyValue, ValkeyValue>("field1", "value1")], new HashSetOptions { Expiry = SetExpiryOptions.KeepTimeToLive() })),
        () => CmdAssert.Equal(RequestType.HSetEx, ["key", "FNX", "PX", "60000", "FIELDS", "1", "field1", "value1"], Request.HashSet("key", [new KeyValuePair<ValkeyValue, ValkeyValue>("field1", "value1")], new HashSetOptions { Condition = HashSetCondition.OnlyIfNoneExist, Expiry = SetExpiryOptions.ExpireIn(TimeSpan.FromSeconds(60)) })),
        () => CmdAssert.Equal(RequestType.HSetEx, ["key", "FXX", "PX", "60000", "FIELDS", "1", "field1", "value1"], Request.HashSet("key", [new KeyValuePair<ValkeyValue, ValkeyValue>("field1", "value1")], new HashSetOptions { Condition = HashSetCondition.OnlyIfAllExist, Expiry = SetExpiryOptions.ExpireIn(TimeSpan.FromSeconds(60)) })),
        () => CmdAssert.Equal(RequestType.HPersist, ["key", "FIELDS", "2", "field1", "field2"], Request.HashPersist("key", ["field1", "field2"])),
        () => CmdAssert.Equal(RequestType.HPExpire, ["key", "60000", "FIELDS", "2", "field1", "field2"], Request.HashExpire("key", TimeSpan.FromSeconds(60), ["field1", "field2"], ExpireCondition.Always)),
        () => CmdAssert.Equal(RequestType.HPExpire, ["key", "60000", "NX", "FIELDS", "2", "field1", "field2"], Request.HashExpire("key", TimeSpan.FromSeconds(60), ["field1", "field2"], ExpireCondition.OnlyIfNotExists)),
        () => CmdAssert.Equal(RequestType.HPExpire, ["key", "60000", "XX", "FIELDS", "2", "field1", "field2"], Request.HashExpire("key", TimeSpan.FromSeconds(60), ["field1", "field2"], ExpireCondition.OnlyIfExists)),
        () => CmdAssert.Equal(RequestType.HPExpire, ["key", "60000", "GT", "FIELDS", "2", "field1", "field2"], Request.HashExpire("key", TimeSpan.FromSeconds(60), ["field1", "field2"], ExpireCondition.OnlyIfGreaterThan)),
        () => CmdAssert.Equal(RequestType.HPExpire, ["key", "60000", "LT", "FIELDS", "2", "field1", "field2"], Request.HashExpire("key", TimeSpan.FromSeconds(60), ["field1", "field2"], ExpireCondition.OnlyIfLessThan)),
        () => CmdAssert.Equal(RequestType.HPExpire, ["key", "5500", "FIELDS", "2", "field1", "field2"], Request.HashExpire("key", TimeSpan.FromMilliseconds(5500), ["field1", "field2"], ExpireCondition.Always)),
        () => CmdAssert.Equal(RequestType.HPExpireAt, ["key", "1609459200000", "FIELDS", "2", "field1", "field2"], Request.HashExpireAt("key", DateTimeOffset.FromUnixTimeSeconds(1609459200), ["field1", "field2"], ExpireCondition.Always)),
        () => CmdAssert.Equal(RequestType.HPExpireAt, ["key", "1609459200000", "NX", "FIELDS", "2", "field1", "field2"], Request.HashExpireAt("key", DateTimeOffset.FromUnixTimeSeconds(1609459200), ["field1", "field2"], ExpireCondition.OnlyIfNotExists)),
        () => CmdAssert.Equal(RequestType.HPExpireAt, ["key", "1609459200500", "FIELDS", "2", "field1", "field2"], Request.HashExpireAt("key", DateTimeOffset.FromUnixTimeMilliseconds(1609459200500), ["field1", "field2"], ExpireCondition.Always)),
        () => CmdAssert.Equal(RequestType.HPExpireTime, ["key", "FIELDS", "2", "field1", "field2"], Request.HashExpireTime("key", ["field1", "field2"])),
        () => CmdAssert.Equal(RequestType.HPTtl, ["key", "FIELDS", "2", "field1", "field2"], Request.HashTimeToLive("key", ["field1", "field2"])),

        // Hash HGETDEL Command (Valkey 9.1+)
        () => CmdAssert.Equal(RequestType.HGetDel, ["key", "FIELDS", "2", "field1", "field2"], Request.HashGetDelete("key", ["field1", "field2"])),
        () => CmdAssert.Equal(RequestType.HGetDel, ["key", "FIELDS", "0"], Request.HashGetDelete("key", [])),

        // Hash HSETEX Command with Key Condition (Valkey 9.1+)
        () => CmdAssert.Equal(RequestType.HSetEx, ["key", "NX", "FIELDS", "1", "field1", "value1"], Request.HashSet("key", [new KeyValuePair<ValkeyValue, ValkeyValue>("field1", "value1")], new HashSetOptions { KeyCondition = HashSetKeyCondition.OnlyIfDoesNotExist })),
        () => CmdAssert.Equal(RequestType.HSetEx, ["key", "XX", "FIELDS", "1", "field1", "value1"], Request.HashSet("key", [new KeyValuePair<ValkeyValue, ValkeyValue>("field1", "value1")], new HashSetOptions { KeyCondition = HashSetKeyCondition.OnlyIfExists })),
        () => CmdAssert.Equal(RequestType.HSetEx, ["key", "XX", "FNX", "PX", "60000", "FIELDS", "1", "field1", "value1"], Request.HashSet("key", [new KeyValuePair<ValkeyValue, ValkeyValue>("field1", "value1")], new HashSetOptions { KeyCondition = HashSetKeyCondition.OnlyIfExists, Condition = HashSetCondition.OnlyIfNoneExist, Expiry = SetExpiryOptions.ExpireIn(TimeSpan.FromSeconds(60)) })),

        // Geospatial Commands
        () => CmdAssert.Equal(RequestType.GeoAdd, ["key", "13.361389000000001", "38.115555999999998", "Palermo"], Request.GeoAdd("key", "Palermo", new GeoPosition(13.361389, 38.115556))),
        () => CmdAssert.Equal(RequestType.GeoAdd, ["key", "15.087268999999999", "37.502668999999997", "Catania", "13.361389000000001", "38.115555999999998", "Palermo"], Request.GeoAdd("key", new SortedDictionary<ValkeyValue, GeoPosition> { ["Palermo"] = new(13.361389, 38.115556), ["Catania"] = new(15.087269, 37.502669) })),
        () => CmdAssert.Equal(RequestType.GeoAdd, ["key", "NX", "13.361389000000001", "38.115555999999998", "Palermo"], Request.GeoAdd("key", "Palermo", new GeoPosition(13.361389, 38.115556), new GeoAddOptions { Condition = GeoAddCondition.OnlyIfNotExists })),
        () => CmdAssert.Equal(RequestType.GeoAdd, ["key", "XX", "13.361389000000001", "38.115555999999998", "Palermo"], Request.GeoAdd("key", "Palermo", new GeoPosition(13.361389, 38.115556), new GeoAddOptions { Condition = GeoAddCondition.OnlyIfExists })),
        () => CmdAssert.Equal(RequestType.GeoAdd, ["key", "13.361389000000001", "38.115555999999998", "Palermo"], Request.GeoAdd("key", "Palermo", new GeoPosition(13.361389, 38.115556), new GeoAddOptions { Condition = GeoAddCondition.Always })),
        () => CmdAssert.Equal(RequestType.GeoAdd, ["key", "CH", "13.361389000000001", "38.115555999999998", "Palermo"], Request.GeoAdd("key", "Palermo", new GeoPosition(13.361389, 38.115556), new GeoAddOptions { Changed = true })),
        () => CmdAssert.Equal(RequestType.GeoAdd, ["key", "NX", "CH", "13.361389000000001", "38.115555999999998", "Palermo"], Request.GeoAdd("key", "Palermo", new GeoPosition(13.361389, 38.115556), new GeoAddOptions { Condition = GeoAddCondition.OnlyIfNotExists, Changed = true })),
        () => CmdAssert.Equal(RequestType.GeoDist, ["key", "Palermo", "Catania", "m"], Request.GeoDistance("key", "Palermo", "Catania", GeoUnit.Meters)),
        () => CmdAssert.Equal(RequestType.GeoDist, ["key", "Palermo", "Catania", "km"], Request.GeoDistance("key", "Palermo", "Catania", GeoUnit.Kilometers)),
        () => CmdAssert.Equal(RequestType.GeoDist, ["key", "Palermo", "Catania", "mi"], Request.GeoDistance("key", "Palermo", "Catania", GeoUnit.Miles)),
        () => CmdAssert.Equal(RequestType.GeoDist, ["key", "Palermo", "Catania", "ft"], Request.GeoDistance("key", "Palermo", "Catania", GeoUnit.Feet)),
        () => CmdAssert.Equal(RequestType.GeoHash, ["key", "Palermo", "Catania"], Request.GeoHash("key", ["Palermo", "Catania"])),
        () => CmdAssert.Equal(RequestType.GeoPos, ["key", "Palermo", "Catania"], Request.GeoPosition("key", ["Palermo", "Catania"])),
        () => CmdAssert.Equal(RequestType.GeoSearch, ["key", "FROMMEMBER", "Palermo", "BYRADIUS", "100", "km"], Request.GeoSearch("key", "Palermo", new GeoSearchCircle(100, GeoUnit.Kilometers))),
        () => CmdAssert.Equal(RequestType.GeoSearch, ["key", "FROMLONLAT", "13.361389000000001", "38.115555999999998", "BYRADIUS", "200", "m"], Request.GeoSearch("key", new GeoPosition(13.361389, 38.115556), new GeoSearchCircle(200, GeoUnit.Meters))),
        () => CmdAssert.Equal(RequestType.GeoSearch, ["key", "FROMMEMBER", "Palermo", "BYBOX", "300", "400", "km"], Request.GeoSearch("key", "Palermo", new GeoSearchBox(400, 300, GeoUnit.Kilometers))),
        () => CmdAssert.Equal(RequestType.GeoSearch, ["key", "FROMMEMBER", "Palermo", "BYRADIUS", "100", "km", "ASC", "COUNT", "10", "WITHCOORD", "WITHDIST", "WITHHASH"], Request.GeoSearch("key", "Palermo", new GeoSearchCircle(100, GeoUnit.Kilometers), new GeoSearchOptions { Order = Order.Ascending, Count = 10, WithPosition = true, WithDistance = true, WithHash = true })),
        () => CmdAssert.Equal(RequestType.GeoSearch, ["key", "FROMMEMBER", "Palermo", "BYRADIUS", "100", "km", "COUNT", "5", "ANY"], Request.GeoSearch("key", "Palermo", new GeoSearchCircle(100, GeoUnit.Kilometers), new GeoSearchOptions { Count = 5, Any = true })),
        () => CmdAssert.Equal(RequestType.GeoSearchStore, ["dest", "key", "FROMMEMBER", "Palermo", "BYRADIUS", "100", "km"], Request.GeoSearchAndStore("key", "dest", "Palermo", new GeoSearchCircle(100, GeoUnit.Kilometers))),
        () => CmdAssert.Equal(RequestType.GeoSearchStore, ["dest", "key", "FROMLONLAT", "13.361389000000001", "38.115555999999998", "BYRADIUS", "200", "m", "STOREDIST"], Request.GeoSearchAndStore("key", "dest", new GeoPosition(13.361389, 38.115556), new GeoSearchCircle(200, GeoUnit.Meters), new GeoSearchStoreOptions { StoreDistances = true })),
        () => CmdAssert.Equal(RequestType.GeoSearchStore, ["dest", "key", "FROMMEMBER", "Palermo", "BYRADIUS", "100", "km", "ASC", "COUNT", "5", "ANY"], Request.GeoSearchAndStore("key", "dest", "Palermo", new GeoSearchCircle(100, GeoUnit.Kilometers), new GeoSearchStoreOptions { Order = Order.Ascending, Count = 5, Any = true })),
        () => Assert.Throws<ArgumentException>(() => Request.GeoSearch("key", "Palermo", new GeoSearchCircle(100, GeoUnit.Kilometers), new GeoSearchOptions { Any = true })),
        () => Assert.Throws<ArgumentException>(() => Request.GeoSearchAndStore("key", "dest", "Palermo", new GeoSearchCircle(100, GeoUnit.Kilometers), new GeoSearchStoreOptions { Any = true })),

        // HyperLogLog Commands
        () => CmdAssert.Equal(RequestType.PfAdd, ["key", "element"], Request.HyperLogLogAdd("key", "element")),
        () => CmdAssert.Equal(RequestType.PfAdd, ["key", "element1", "element2", "element3"], Request.HyperLogLogAdd("key", ["element1", "element2", "element3"])),
        () => CmdAssert.Equal(RequestType.PfCount, ["key"], Request.HyperLogLogLength("key")),
        () => CmdAssert.Equal(RequestType.PfCount, ["key1", "key2", "key3"], Request.HyperLogLogLength(["key1", "key2", "key3"])),
        () => CmdAssert.Equal(RequestType.PfMerge, ["dest", "src1", "src2"], Request.HyperLogLogMerge("dest", "src1", "src2")),
        () => CmdAssert.Equal(RequestType.PfMerge, ["dest", "src1", "src2", "src3"], Request.HyperLogLogMerge("dest", ["src1", "src2", "src3"])),

        // Bitmap Commands
        () => CmdAssert.Equal(RequestType.GetBit, ["key", "0"], Request.GetBit("key", 0)),
        () => CmdAssert.Equal(RequestType.GetBit, ["key", "100"], Request.GetBit("key", 100)),
        () => CmdAssert.Equal(RequestType.SetBit, ["key", "0", "1"], Request.SetBit("key", 0, true)),
        () => CmdAssert.Equal(RequestType.SetBit, ["key", "5", "0"], Request.SetBit("key", 5, false)),
        () => CmdAssert.Equal(RequestType.BitCount, ["key", "0", "-1"], Request.BitCount("key", 0, -1, BitmapIndexType.Byte)),
        () => CmdAssert.Equal(RequestType.BitCount, ["key", "1", "5", "BIT"], Request.BitCount("key", 1, 5, BitmapIndexType.Bit)),
        () => CmdAssert.Equal(RequestType.BitPos, ["key", "1", "0", "-1"], Request.BitPos("key", true, 0, -1, BitmapIndexType.Byte)),
        () => CmdAssert.Equal(RequestType.BitPos, ["key", "0", "2", "10", "BIT"], Request.BitPos("key", false, 2, 10, BitmapIndexType.Bit)),
        () => CmdAssert.Equal(RequestType.BitOp, ["AND", "dest", "key1", "key2"], Request.BitOp(Bitwise.And, "dest", ["key1", "key2"])),
        () => CmdAssert.Equal(RequestType.BitOp, ["OR", "dest", "key1", "key2", "key3"], Request.BitOp(Bitwise.Or, "dest", ["key1", "key2", "key3"])),
        () => CmdAssert.Equal(RequestType.BitOp, ["XOR", "dest", "key1", "key2"], Request.BitOp(Bitwise.Xor, "dest", ["key1", "key2"])),
        () => CmdAssert.Equal(RequestType.BitOp, ["NOT", "dest", "key1"], Request.BitOp(Bitwise.Not, "dest", ["key1"])),
        () => CmdAssert.Equal(RequestType.BitField, ["key", "GET", "u8", "0"], Request.BitField("key", [new BitFieldOptions.BitFieldGet(BitFieldOptions.Encoding.Unsigned(8), new BitFieldOptions.BitOffset(0))])),
        () => CmdAssert.Equal(RequestType.BitField, ["key", "SET", "i16", "#1", "100"], Request.BitField("key", [new BitFieldOptions.BitFieldSet(BitFieldOptions.Encoding.Signed(16), new BitFieldOptions.BitOffsetMultiplier(1), 100)])),
        () => CmdAssert.Equal(RequestType.BitField, ["key", "INCRBY", "u32", "8", "5"], Request.BitField("key", [new BitFieldOptions.BitFieldIncrBy(BitFieldOptions.Encoding.Unsigned(32), new BitFieldOptions.BitOffset(8), 5)])),
        () => CmdAssert.Equal(RequestType.BitField, ["key", "OVERFLOW", "WRAP", "SET", "u8", "0", "255"], Request.BitField("key", [new BitFieldOptions.BitFieldOverflow(BitFieldOptions.OverflowType.Wrap), new BitFieldOptions.BitFieldSet(BitFieldOptions.Encoding.Unsigned(8), new BitFieldOptions.BitOffset(0), 255)])),
        () => CmdAssert.Equal(RequestType.BitFieldReadOnly, ["key", "GET", "u8", "0", "GET", "i4", "8"], Request.BitFieldReadOnly("key", [new BitFieldOptions.BitFieldGet(BitFieldOptions.Encoding.Unsigned(8), new BitFieldOptions.BitOffset(0)), new BitFieldOptions.BitFieldGet(BitFieldOptions.Encoding.Signed(4), new BitFieldOptions.BitOffset(8))]))
        );

    [Fact]
    public void ValidateCommandConverters() => Assert.Multiple(
        () => Assert.Equal(1, Request.CustomCommand([]).Converter(1)),
        () => Assert.Equal(.1, Request.CustomCommand([]).Converter(.1)),
        () => Assert.Null(Request.CustomCommand([]).Converter(null)),

        // String Commands
        () => Assert.True(Request.Set("key", "value", new SetOptions()).Converter("OK")),
        () => Assert.True(Request.Set("key", "value", new SetOptions { Condition = SetCondition.OnlyIfDoesNotExist }).Converter("OK")),
        () => Assert.False(Request.Set("key", "value", new SetOptions { Condition = SetCondition.OnlyIfDoesNotExist }).Converter(null)),
        () => Assert.Equal<GlideString>("value", Request.Get("key").Converter("value")),
        () => Assert.Equal(ValkeyValue.Null, Request.Get("key").Converter(null!)),
        () => Assert.Equal(5L, Request.Length("key").Converter(5L)),
        () => Assert.Equal(0L, Request.Length("key").Converter(0L)),
        () => Assert.Equal(new ValkeyValue("hello"), Request.GetRange("key", 0, 4).Converter("hello")),
        () => Assert.Equal(new ValkeyValue(""), Request.GetRange("key", 0, 4).Converter("")),
        () => Assert.Equal(ValkeyValue.Null, Request.GetRange("key", 0, 4).Converter(null!)),
        () => Assert.Equal((ValkeyValue)10L, Request.SetRange("key", 5, "world").Converter(10L)),
        () => Assert.Equal(11L, Request.Append("key", "value").Converter(11L)),
        () => Assert.Equal(9L, Request.Decrement("key").Converter(9L)),
        () => Assert.Equal(5L, Request.DecrementBy("key", 5).Converter(5L)),
        () => Assert.True(Request.DeleteIfEqual("key", "value").Converter(true)),
        () => Assert.False(Request.DeleteIfEqual("key", "value").Converter(false)),
        () => Assert.Equal(11L, Request.Increment("key").Converter(11L)),
        () => Assert.Equal(15L, Request.IncrementBy("key", 5).Converter(15L)),
        () => Assert.Equal(10.5, Request.IncrementByFloat("key", 0.5).Converter(10.5)),
        // TODO #454: Set should return ValkeyValue.Ok instead of bool.
        () => Assert.True(Request.Set([new KeyValuePair<ValkeyKey, ValkeyValue>("key1", "value1"), new KeyValuePair<ValkeyKey, ValkeyValue>("key2", "value2")]).Converter("OK")),
        () => Assert.True(Request.SetIfNotExists([new KeyValuePair<ValkeyKey, ValkeyValue>("key1", "value1")]).Converter(true)),
        () => Assert.False(Request.SetIfNotExists([new KeyValuePair<ValkeyKey, ValkeyValue>("key1", "value1")]).Converter(false)),
        () => Assert.Equal("test_value", Request.GetDelete("test_key").Converter(new GlideString("test_value")).ToString()),
        () => Assert.True(Request.GetDelete("test_key").Converter(null!).IsNull),
        () => Assert.Equal("test_value", Request.GetExpiry("test_key", GetExpiryOptions.ExpireIn(TimeSpan.FromSeconds(60))).Converter(new GlideString("test_value")).ToString()),
        () => Assert.True(Request.GetExpiry("test_key", GetExpiryOptions.ExpireIn(TimeSpan.FromSeconds(60))).Converter(null!).IsNull),
        () => Assert.Equal("test_value", Request.GetExpiry("test_key", GetExpiryOptions.ExpireAt(new DateTimeOffset(2021, 1, 1, 0, 0, 0, TimeSpan.Zero))).Converter(new GlideString("test_value")).ToString()),
        () => Assert.True(Request.GetExpiry("test_key", GetExpiryOptions.ExpireAt(new DateTimeOffset(2021, 1, 1, 0, 0, 0, TimeSpan.Zero))).Converter(null!).IsNull),

        // Server Management Command Converters
        () => Assert.Equal("Background append only file rewriting started", Request.BgRewriteAof().Converter("Background append only file rewriting started")),
        () => Assert.Equal([new("maxmemory", "100mb")], Request.ConfigGet("maxmemory").Converter(new object[] { new GlideString("maxmemory"), "100mb" })),
        () => Assert.Empty(Request.ConfigGet("nonexistent").Converter(Array.Empty<object>())),
        () => Assert.Equal(100L, Request.DatabaseSize().Converter(100L)),
        () => Assert.Equal(0L, Request.DatabaseSize().Converter(0L)),
        () => Assert.Equal(DateTimeOffset.FromUnixTimeSeconds(1609459200), Request.LastSave().Converter(1609459200L)),
        () => Assert.Empty(Request.LatencyHistory("command").Converter(Array.Empty<object>())),
        () => Assert.Equal(
            [new LatencyEntry {
                Time = DateTimeOffset.FromUnixTimeSeconds(1609459200),
                Duration = TimeSpan.FromMilliseconds(50) }],
            Request.LatencyHistory("command").Converter(new object[] { new object[] { 1609459200L, 50L } })),
        () => Assert.Empty(Request.LatencyLatest().Converter(Array.Empty<object>())),
        () => Assert.Equal(
            [new LatencyEventInfo {
                EventName = "command",
                LatestTime = DateTimeOffset.FromUnixTimeSeconds(1609459200),
                LatestDuration = TimeSpan.FromMilliseconds(50),
                MaxDuration = TimeSpan.FromMilliseconds(100) }],
            Request.LatencyLatest().Converter(new object[] { new object[] { new GlideString("command"), 1609459200L, 50L, 100L } })),
        () => Assert.Equal(
            [new LatencyEventInfo {
                EventName = "command",
                LatestTime = DateTimeOffset.FromUnixTimeSeconds(1609459200),
                LatestDuration = TimeSpan.FromMilliseconds(50),
                MaxDuration = TimeSpan.FromMilliseconds(100),
                Sum = TimeSpan.FromMilliseconds(200),
                Count = 5L, }],
            Request.LatencyLatest().Converter(new object[] { new object[] { new GlideString("command"), 1609459200L, 50L, 100L, 200L, 5L } })),
        () => Assert.Equal(0L, Request.LatencyReset([]).Converter(0L)),
        () => Assert.Equal(3L, Request.LatencyReset(["command"]).Converter(3L)),
        () => Assert.Equal("Valkey 7.0.0", Request.Lolwut().Converter("Valkey 7.0.0")),
        () => Assert.Equal("Sam, I have no memory problems", Request.MemoryDoctor().Converter("Sam, I have no memory problems")),
        () => Assert.Equal("jemalloc stats", Request.MemoryMallocStats().Converter("jemalloc stats")),
        () => Assert.Equal(ValkeyValue.Ok, Request.Save().Converter("OK")),
        () => Assert.Equal(DateTimeOffset.FromUnixTimeSeconds(1609459200).AddTicks(123456 * 10), Request.Time().Converter(["1609459200", "123456"])),

        () => Assert.Equal("common", Request.LongestCommonSubsequence("key1", "key2").Converter(new GlideString("common"))),
        () => Assert.Equal(5L, Request.LongestCommonSubsequenceLength("key1", "key2").Converter(5L)),

        // Info Command Converters
        () => Assert.Equal("info", Request.Info([]).Converter("info")),
        () => Assert.Equal("server info", Request.Info([InfoOptions.Section.SERVER]).Converter("server info")),
        () => Assert.Equal("clients info", Request.Info([InfoOptions.Section.CLIENTS]).Converter("clients info")),
        () => Assert.Equal("memory info", Request.Info([InfoOptions.Section.MEMORY]).Converter("memory info")),
        () => Assert.Equal("persistence info", Request.Info([InfoOptions.Section.PERSISTENCE]).Converter("persistence info")),
        () => Assert.Equal("stats info", Request.Info([InfoOptions.Section.STATS]).Converter("stats info")),
        () => Assert.Equal("replication info", Request.Info([InfoOptions.Section.REPLICATION]).Converter("replication info")),
        () => Assert.Equal("cpu info", Request.Info([InfoOptions.Section.CPU]).Converter("cpu info")),
        () => Assert.Equal("commandstats info", Request.Info([InfoOptions.Section.COMMANDSTATS]).Converter("commandstats info")),
        () => Assert.Equal("latencystats info", Request.Info([InfoOptions.Section.LATENCYSTATS]).Converter("latencystats info")),
        () => Assert.Equal("sentinel info", Request.Info([InfoOptions.Section.SENTINEL]).Converter("sentinel info")),
        () => Assert.Equal("cluster info", Request.Info([InfoOptions.Section.CLUSTER]).Converter("cluster info")),
        () => Assert.Equal("modules info", Request.Info([InfoOptions.Section.MODULES]).Converter("modules info")),
        () => Assert.Equal("keyspace info", Request.Info([InfoOptions.Section.KEYSPACE]).Converter("keyspace info")),
        () => Assert.Equal("errorstats info", Request.Info([InfoOptions.Section.ERRORSTATS]).Converter("errorstats info")),
        () => Assert.Equal("all info", Request.Info([InfoOptions.Section.ALL]).Converter("all info")),
        () => Assert.Equal("default info", Request.Info([InfoOptions.Section.DEFAULT]).Converter("default info")),
        () => Assert.Equal("everything info", Request.Info([InfoOptions.Section.EVERYTHING]).Converter("everything info")),

        // Ping Command Converters
        () => Assert.IsType<ValkeyValue>(Request.Ping().Converter(new GlideString("PONG"))),
        () => Assert.IsType<ValkeyValue>(Request.Ping("Hello").Converter(new GlideString("Hello"))),
        () => Assert.Equal<ValkeyValue>("PONG", Request.Ping().Converter(new GlideString("PONG"))),
        () => Assert.Equal<ValkeyValue>("test", Request.Ping("test").Converter(new GlideString("test"))),
        () => Assert.Equal<ValkeyValue>("message", Request.Echo("message").Converter("message")),

        () => Assert.Equal(ValkeyValue.Null, Request.ClientGetName().Converter(null!)),
        () => Assert.Equal("test-connection", Request.ClientGetName().Converter(new GlideString("test-connection"))),
        () => Assert.Equal(12345L, Request.ClientId().Converter(12345L)),
        () => Assert.Equal(ValkeyValue.Ok, Request.Select(0).Converter("OK")),
        () => Assert.Equal(ValkeyValue.Ok, Request.ClientPause(OneSecond).Converter("OK")),
        () => Assert.Equal(ValkeyValue.Ok, Request.ClientPauseWrite(OneSecond).Converter("OK")),
        () => Assert.Equal(ValkeyValue.Ok, Request.ClientUnpause().Converter("OK")),

        // Set command converters
        () => Assert.True(Request.SetAdd("key", "member").Converter(1L)),
        () => Assert.False(Request.SetAdd("key", "member").Converter(0L)),
        () => Assert.Equal(2L, Request.SetAdd("key", ["member1", "member2"]).Converter(2L)),
        () => Assert.Equal(5L, Request.SetCard("key").Converter(5L)),
        () => Assert.Equal(1L, Request.SetDiffStore("dest", ["key1", "key2"]).Converter(1L)),
        () => Assert.Equal(3L, Request.SetInterCard(["key1", "key2"]).Converter(3L)),
        () => Assert.Equal(2L, Request.SetInterStore("dest", ["key1", "key2"]).Converter(2L)),
        () => Assert.True(Request.SetIsMember("key", "member").Converter(true)),
        () => Assert.False(Request.SetIsMember("key", "member").Converter(false)),
        () => Assert.True(Request.SetIsMemberIfExists("key", "member").Converter(1L)),
        () => Assert.False(Request.SetIsMemberIfExists("key", "member").Converter(0L)),
        () => Assert.Null(Request.SetIsMemberIfExists("key", "member").Converter(-1L)),
        () => Assert.Equal<ValkeyValue>("member", Request.SetPop("key").Converter("member")),
        () => Assert.Null(Request.SetPop("key").Converter(null!)),
        () => Assert.True(Request.SetRemove("key", "member").Converter(1L)),
        () => Assert.False(Request.SetRemove("key", "member").Converter(0L)),
        () => Assert.Equal(1L, Request.SetRemove("key", ["member1", "member2"]).Converter(1L)),
        () => Assert.Equal(4L, Request.SetUnionStore("dest", ["key1", "key2"]).Converter(4L)),

        // Generic Commands Converters
        () => Assert.True(Request.Delete("key").Converter(1L)),
        () => Assert.False(Request.Delete("key").Converter(0L)),
        () => Assert.Equal(2L, Request.Delete(["key1", "key2"]).Converter(2L)),
        () => Assert.True(Request.Unlink("key").Converter(1L)),
        () => Assert.False(Request.Unlink("key").Converter(0L)),
        () => Assert.Equal(3L, Request.Unlink(["key1", "key2", "key3"]).Converter(3L)),
        () => Assert.True(Request.Exists("key").Converter(1L)),
        () => Assert.False(Request.Exists("key").Converter(0L)),
        () => Assert.Equal(2L, Request.Exists(["key1", "key2"]).Converter(2L)),
        () => Assert.True(Request.Expire("key", TimeSpan.FromSeconds(60)).Converter(true)),
        () => Assert.False(Request.Expire("key", TimeSpan.FromSeconds(60)).Converter(false)),
        () => Assert.Equal(TimeSpan.FromMilliseconds(30), Request.TimeToLive("key").Converter(30L).TimeToLive),
        () => Assert.False(Request.TimeToLive("key").Converter(-1L).HasTimeToLive),
        () => Assert.False(Request.TimeToLive("key").Converter(-2L).Exists),
        () => Assert.Equal(ValkeyType.String, Request.Type("key").Converter("string")),
        () => Assert.Equal(ValkeyType.List, Request.Type("key").Converter("list")),
        () => Assert.Equal(ValkeyType.Set, Request.Type("key").Converter("set")),
        () => Assert.Equal(ValkeyType.Hash, Request.Type("key").Converter("hash")),
        () => Assert.Equal(ValkeyType.Stream, Request.Type("key").Converter("stream")),
        () => Assert.Equal(ValkeyType.None, Request.Type("key").Converter("none")),
        // TODO #454: RenameAsync should return ValkeyValue.Ok instead of bool.
        () => Assert.True(Request.Rename("oldkey", "newkey").Converter("OK")),
        () => Assert.True(Request.RenameIfNotExists("oldkey", "newkey").Converter(true)),
        () => Assert.False(Request.RenameIfNotExists("oldkey", "newkey").Converter(false)),
        () => Assert.True(Request.Persist("key").Converter(true)),
        () => Assert.False(Request.Persist("key").Converter(false)),
        () => Assert.NotNull(Request.Dump("key").Converter("dumpdata")),
        () => Assert.Null(Request.Dump("key").Converter(null!)),
        () => Assert.Equal(ValkeyValue.Ok, Request.Restore("key", []).Converter("OK")),
        () => Assert.True(Request.Touch("key").Converter(1L)),
        () => Assert.False(Request.Touch("key").Converter(0L)),
        () => Assert.Equal(2L, Request.Touch(["key1", "key2"]).Converter(2L)),
        () => Assert.True(Request.Copy("src", "dest").Converter(true)),
        () => Assert.False(Request.Copy("src", "dest").Converter(false)),
        () => Assert.Equal(DateTimeOffset.FromUnixTimeMilliseconds(1609459200000L), Request.ExpireTime("key").Converter(1609459200000L)),
        () => Assert.Null(Request.ExpireTime("key").Converter(-1L)),
        () => Assert.Null(Request.ExpireTime("key").Converter(-2L)),
        () => Assert.Equal("embstr", Request.ObjectEncoding("key").Converter(new GlideString("embstr"))),
        () => Assert.Null(Request.ObjectEncoding("key").Converter(null!)),
        () => Assert.Equal(5L, Request.ObjectFrequency("key").Converter(5L)),
        () => Assert.Null(Request.ObjectFrequency("key").Converter(-1L)),
        () => Assert.Equal(TimeSpan.FromSeconds(10), Request.ObjectIdleTime("key").Converter(10L)),
        () => Assert.Null(Request.ObjectIdleTime("key").Converter(-1L)),
        () => Assert.Equal(3L, Request.ObjectRefCount("key").Converter(3L)),
        () => Assert.Null(Request.ObjectRefCount("key").Converter(-1L)),
        () => Assert.Equal(new ValkeyKey("randomkey"), Request.RandomKey().Converter(new GlideString("randomkey"))),
        () => Assert.Null(Request.RandomKey().Converter(null!)),
        () => Assert.True(Request.Move("key", 1).Converter(true)),
        () => Assert.False(Request.Move("key", 1).Converter(false)),

        // SCAN Commands Converters
        () =>
        {
            var result = Request.Scan("0").Converter(["0", new object[] { new GlideString("key1"), new GlideString("key2") }]);
            Assert.Equal("0", result.Item1);
            Assert.Equal([new ValkeyKey("key1"), new ValkeyKey("key2")], result.Item2);
        },
        () =>
        {
            var result = Request.Scan("10").Converter(["5", new object[] { new GlideString("test") }]);
            Assert.Equal("5", result.Item1);
            Assert.Equal([new ValkeyKey("test")], result.Item2);
        },
        () =>
        {
            var result = Request.Scan("0").Converter(["0", Array.Empty<object>()]);
            Assert.Equal("0", result.Item1);
            Assert.Empty(result.Item2);
        },

        // WAIT Commands Converters
        () => Assert.Equal(2L, Request.Wait(1, OneSecond).Converter(2L)),
        () => Assert.Equal(0L, Request.Wait(0, TimeSpan.Zero).Converter(0L)),
        () => Assert.Equal(1L, Request.Wait(3, TimeSpan.FromMilliseconds(5000)).Converter(1L)),
        () => Assert.Equal(new long[] { 1L, 0L }, Request.WaitAof(true, 1, OneSecond).Converter([1L, 0L])),
        () => Assert.Equal(new long[] { 0L, 0L }, Request.WaitAof(false, 0, TimeSpan.Zero).Converter([0L, 0L])),

        () => Assert.Equal("one", Request.ListLeftPop("a").Converter("one")),
        () => Assert.Equal(["one", "two"], Request.ListLeftPop("a", 2).Converter([new GlideString("one"), new GlideString("two")])!),
        () => Assert.Null(Request.ListLeftPop("a", 2).Converter(null!)),
        () => Assert.Equal(ValkeyValue.Null, Request.ListLeftPop("a").Converter(null!)),
        () => Assert.Equal(1L, Request.ListLeftPush("a", "value").Converter(1L)),
        () => Assert.Equal(2L, Request.ListLeftPush("a", ["one", "two"]).Converter(2L)),
        () => Assert.Equal("three", Request.ListRightPop("a").Converter("three")),
        () => Assert.Equal(ValkeyValue.Null, Request.ListRightPop("a").Converter(null!)),
        () => Assert.Equal(["three", "four"], Request.ListRightPop("a", 2).Converter([new GlideString("three"), new GlideString("four")])!),
        () => Assert.Null(Request.ListRightPop("a", 2).Converter(null!)),
        () => Assert.Equal(2L, Request.ListRightPush("a", "value").Converter(2L)),
        () => Assert.Equal(3L, Request.ListRightPush("a", ["three", "four"]).Converter(3L)),
        () => Assert.Equal(5L, Request.ListLength("a").Converter(5L)),
        () => Assert.Equal(0L, Request.ListLength("nonexistent").Converter(0L)),
        () => Assert.Equal(2L, Request.ListRemove("a", "value", 0).Converter(2L)),
        () => Assert.Equal(1L, Request.ListRemove("a", "value", 1).Converter(1L)),
        () => Assert.Equal(0L, Request.ListRemove("a", "nonexistent", 0).Converter(0L)),
        () => Assert.Equal(ValkeyValue.Ok, Request.ListTrim("a", 0, 10).Converter("OK")),
        () => Assert.Equal(["one", "two", "three"], Request.ListRange("a", 0, -1).Converter([new GlideString("one"), new GlideString("two"), new GlideString("three")])),
        () => Assert.IsType<ValkeyValue[]>(Request.ListRange("a", 0, -1).Converter([new GlideString("one"), new GlideString("two"), new GlideString("three")])),
        () => Assert.Equal([], Request.ListRange("nonexistent", 0, -1).Converter([])),

        // Hash Commands
        () => Assert.Equal<GlideString>("value", Request.HashGet("key", "field").Converter("value")),
        () => Assert.Equal(ValkeyValue.Null, Request.HashGet("key", "field").Converter(null!)),
        () => Assert.Equal(2L, Request.HashSet("key", [new KeyValuePair<ValkeyValue, ValkeyValue>("field", "value")]).Converter(2L)),
        () => Assert.True(Request.HashDelete("key", "field").Converter(1L)),
        () => Assert.False(Request.HashDelete("key", "field").Converter(0L)),
        () => Assert.Equal(2L, Request.HashDelete("key", ["field1", "field2"]).Converter(2L)),
        () => Assert.True(Request.HashExists("key", "field").Converter(true)),
        () => Assert.False(Request.HashExists("key", "field").Converter(false)),
        () => Assert.Equal(15L, Request.HashIncrementBy("key", "field", 5L).Converter(15L)),
        () => Assert.Equal(10L, Request.HashIncrementBy("key", "field", 1L).Converter(10L)),
        () => Assert.Equal(12.5, Request.HashIncrementBy("key", "field", 2.5).Converter(12.5)),
        () => Assert.Equal(new HashSet<ValkeyValue> { "field1", "field2" }, Request.HashKeys("key").Converter([new GlideString("field1"), new GlideString("field2")])),
        () => Assert.Empty(Request.HashKeys("nonexistent").Converter([])),
        () => Assert.Equal(5L, Request.HashLength("key").Converter(5L)),
        () => Assert.Equal(10L, Request.HashStringLength("key", "field").Converter(10L)),

        // Hash Field Expire Commands converters (Valkey 9.0+)
        () => Assert.Equal((ValkeyValue[])["value1", "value2"], Request.HashGet("key", ["field1", "field2"], GetExpiryOptions.ExpireIn(TimeSpan.FromSeconds(60))).Converter([new GlideString("value1"), new GlideString("value2")])),
        () => Assert.Equal((ValkeyValue[])[ValkeyValue.Null], Request.HashGet("key", ["field1"], GetExpiryOptions.Persist()).Converter([null!])),
        () => Assert.True(Request.HashSet("key", [new KeyValuePair<ValkeyValue, ValkeyValue>("field1", "value1")], new HashSetOptions { Expiry = SetExpiryOptions.ExpireIn(TimeSpan.FromSeconds(60)) }).Converter(1L)),
        () => Assert.False(Request.HashSet("key", [new KeyValuePair<ValkeyValue, ValkeyValue>("field1", "value1")], new HashSetOptions { Expiry = SetExpiryOptions.ExpireIn(TimeSpan.FromSeconds(60)) }).Converter(0L)),
        () => Assert.Equal([HashPersistResult.ExpiryRemoved, HashPersistResult.NoExpiry, HashPersistResult.NoField], Request.HashPersist("key", ["field1", "field2", "field3"]).Converter([1L, -1L, -2L])),
        () => Assert.Equal([HashExpireResult.ExpirySet, HashExpireResult.ConditionNotMet, HashExpireResult.NoField], Request.HashExpire("key", TimeSpan.FromSeconds(60), ["field1", "field2", "field3"], ExpireCondition.Always).Converter([1L, 0L, -2L])),
        () => Assert.Equal([HashExpireResult.ExpirySet, HashExpireResult.ConditionNotMet, HashExpireResult.NoField], Request.HashExpireAt("key", DateTimeOffset.FromUnixTimeSeconds(1609459200), ["field1", "field2", "field3"], ExpireCondition.Always).Converter([1L, 0L, -2L])),
        () => Assert.True(Request.HashExpireTime("key", ["field1"]).Converter([1609459200000L])[0].HasExpiry),
        () => Assert.False(Request.HashExpireTime("key", ["field1"]).Converter([-1L])[0].HasExpiry),
        () => Assert.False(Request.HashExpireTime("key", ["field1"]).Converter([-2L])[0].Exists),
        () => Assert.True(Request.HashTimeToLive("key", ["field1"]).Converter([60000L])[0].HasTimeToLive),
        () => Assert.False(Request.HashTimeToLive("key", ["field1"]).Converter([-1L])[0].HasTimeToLive),
        () => Assert.False(Request.HashTimeToLive("key", ["field1"]).Converter([-2L])[0].Exists),

        // Hash HGETDEL Commands converter (Valkey 9.1+)
        () => Assert.Equal<ValkeyValue>("value1", Request.HashGetDelete("key", ["field1"]).ToFirstValue().Converter([new GlideString("value1")])),
        () => Assert.Equal(ValkeyValue.Null, Request.HashGetDelete("key", ["field1"]).ToFirstValue().Converter([null!])),
        () => Assert.Equal((ValkeyValue[])["value1", ValkeyValue.Null], Request.HashGetDelete("key", ["field1", "field2"]).Converter([new GlideString("value1"), null!])),
        () => Assert.Equal(HashPersistResult.ExpiryRemoved, Request.HashPersist("key", ["field1"]).ToFirstValue().Converter([1L])),
        () => Assert.Equal(HashExpireResult.ExpirySet, Request.HashExpire("key", TimeSpan.FromSeconds(60), ["field1"], ExpireCondition.Always).ToFirstValue().Converter([1L])),

        // List Commands converters
        () => Assert.Equal(["key", "value"], Request.ListBlockingLeftPop(["key"], TimeSpan.FromSeconds(1)).Converter([new GlideString("key"), new GlideString("value")])!),
        () => Assert.Null(Request.ListBlockingLeftPop(["key"], TimeSpan.FromSeconds(1)).Converter(null!)),
        () => Assert.Equal(["list1", "element"], Request.ListBlockingRightPop(["list1", "list2"], TimeSpan.FromSeconds(5)).Converter([new GlideString("list1"), new GlideString("element")])!),
        () => Assert.Null(Request.ListBlockingRightPop(["key"], TimeSpan.Zero).Converter(null!)),
        () => Assert.Equal("moved_value", Request.ListBlockingMove("src", "dest", ListSide.Left, ListSide.Right, TimeSpan.FromSeconds(2)).Converter("moved_value")),
        () => Assert.Equal(ValkeyValue.Null, Request.ListBlockingMove("src", "dest", ListSide.Left, ListSide.Right, TimeSpan.FromSeconds(2)).Converter(null!)),
        () => Assert.True(Request.ListBlockingPop(["key"], ListSide.Left, TimeSpan.FromSeconds(1)).Converter(null!).IsNull),
        () => Assert.True(Request.ListBlockingPop(["key"], ListSide.Left, 2, TimeSpan.FromSeconds(1)).Converter(null!).IsNull),
        () => Assert.False(Request.ListBlockingPop(["mylist"], ListSide.Left, TimeSpan.FromSeconds(1)).Converter(new() { { new GlideString("mylist"), new object[] { new GlideString("value1") } } }).IsNull),
        () => Assert.False(Request.ListBlockingPop(["list2"], ListSide.Right, 3, TimeSpan.FromSeconds(2)).Converter(new() { { new GlideString("list2"), new object[] { new GlideString("elem1"), new GlideString("elem2") } } }).IsNull),
        () => Assert.True(Request.ListBlockingPop(["key"], ListSide.Left, TimeSpan.FromSeconds(1)).Converter([]).IsNull),
        () => Assert.True(Request.ListLeftPop(["key1", "key2"], 2).Converter(null!).IsNull),
        () => Assert.True(Request.ListRightPop(["key1", "key2"], 3).Converter(null!).IsNull),
        () => Assert.False(Request.ListLeftPop(["mylist"], 1).Converter(new() { { new GlideString("mylist"), new object[] { new GlideString("left_value") } } }).IsNull),
        () => Assert.False(Request.ListRightPop(["list2"], 2).Converter(new() { { new GlideString("list2"), new object[] { new GlideString("right1"), new GlideString("right2") } } }).IsNull),
        () => Assert.True(Request.ListLeftPop(["empty"], 1).Converter([]).IsNull),
        () => Assert.True(Request.ListRightPop(["empty"], 1).Converter([]).IsNull),

        // HyperLogLog Command Converters
        () => Assert.True(Request.HyperLogLogAdd("key", "element").Converter(true)),
        () => Assert.False(Request.HyperLogLogAdd("key", "element").Converter(false)),
        () => Assert.True(Request.HyperLogLogAdd("key", ["element1", "element2"]).Converter(true)),
        () => Assert.False(Request.HyperLogLogAdd("key", ["element1", "element2"]).Converter(false)),
        () => Assert.Equal(42L, Request.HyperLogLogLength("key").Converter(42L)),
        () => Assert.Equal(0L, Request.HyperLogLogLength("key").Converter(0L)),
        () => Assert.Equal(100L, Request.HyperLogLogLength(["key1", "key2"]).Converter(100L)),
        () => Assert.Equal(ValkeyValue.Ok, Request.HyperLogLogMerge("dest", "src1", "src2").Converter("OK")),
        () => Assert.Equal(ValkeyValue.Ok, Request.HyperLogLogMerge("dest", ["src1", "src2"]).Converter("OK")),

        // Transaction Commands
        () => CmdAssert.Equal(RequestType.Watch, ["key1"], Request.Watch(["key1"])),
        () => CmdAssert.Equal(RequestType.Watch, ["key1", "key2", "key3"], Request.Watch(["key1", "key2", "key3"])),
        () => CmdAssert.Equal(RequestType.UnWatch, [], Request.Unwatch()),
        () => Assert.Equal("OK", Request.Watch(["key1"]).Converter("OK")),
        () => Assert.Equal("ERROR", Request.Watch(["key1"]).Converter("ERROR")),
        () => Assert.Equal("OK", Request.Unwatch().Converter("OK")),
        () => Assert.Equal("ERROR", Request.Unwatch().Converter("ERROR")),

        // Bitmap Command Converters
        () => Assert.True(Request.GetBit("key", 0).Converter(1L)),
        () => Assert.False(Request.GetBit("key", 0).Converter(0L)),
        () => Assert.True(Request.SetBit("key", 0, true).Converter(1L)),
        () => Assert.False(Request.SetBit("key", 0, false).Converter(0L)),
        () => Assert.Equal(26L, Request.BitCount("key", 0, -1, BitmapIndexType.Byte).Converter(26L)),
        () => Assert.Equal(0L, Request.BitCount("key", 0, -1, BitmapIndexType.Byte).Converter(0L)),
        () => Assert.Equal(2L, Request.BitPos("key", true, 0, -1, BitmapIndexType.Byte).Converter(2L)),
        () => Assert.Equal(-1L, Request.BitPos("key", true, 0, -1, BitmapIndexType.Byte).Converter(-1L)),
        () => Assert.Equal(6L, Request.BitOp(Bitwise.And, "dest", ["key1", "key2"]).Converter(6L)),
        () => Assert.Equal(0L, Request.BitOp(Bitwise.Or, "dest", ["key1", "key2"]).Converter(0L)),
        () => Assert.Equal([65L, null, 100L], Request.BitField("key", [new BitFieldOptions.BitFieldGet(BitFieldOptions.Encoding.Unsigned(8), new BitFieldOptions.BitOffset(0)), new BitFieldOptions.BitFieldSet(BitFieldOptions.Encoding.Unsigned(8), new BitFieldOptions.BitOffset(0), 100)]).Converter([65L, null!, 100L])),
        () => Assert.Equal([65L, 4L], Request.BitFieldReadOnly("key", [new BitFieldOptions.BitFieldGet(BitFieldOptions.Encoding.Unsigned(8), new BitFieldOptions.BitOffset(0)), new BitFieldOptions.BitFieldGet(BitFieldOptions.Encoding.Unsigned(4), new BitFieldOptions.BitOffset(0))]).Converter([65L, 4L]))
        );

    [Fact]
    public void ValidateClientTrackingInfoConverter()
    {
        // Tracking off
        var offResponse = new Dictionary<GlideString, object>()
        {
            ["flags"] = new HashSet<object> { new GlideString("off") },
            ["redirect"] = -1L,
            ["prefixes"] = new HashSet<object>(),
        };

        var offInfo = Request.ClientTrackingInfo().Converter(offResponse);
        Assert.Equal(new HashSet<string> { "off" }, offInfo.Flags);
        Assert.Equal(-1L, offInfo.Redirect);
        Assert.Empty(offInfo.Prefixes);

        // Tracking on
        var onResponse = new Dictionary<GlideString, object>()
        {
            ["flags"] = new HashSet<object> {
                new GlideString("on"),
                new GlideString("bcast") },
            ["redirect"] = 0L,
            ["prefixes"] = new HashSet<object> {
                new GlideString("user:"),
                new GlideString("session:") },
        };

        var onInfo = Request.ClientTrackingInfo().Converter(onResponse);
        Assert.Equal(new HashSet<string> { "on", "bcast" }, onInfo.Flags);
        Assert.Equal(0L, onInfo.Redirect);
        Assert.Equal(new HashSet<string> { "user:", "session:" }, onInfo.Prefixes);
    }

    [Fact]
    public void BitField_AutoOptimization_UsesCorrectRequestType()
    {
        // Test that read-only subcommands use BitFieldReadOnlyAsync
        var readOnlySubCommands = new BitFieldOptions.IBitFieldSubCommand[]
        {
            new BitFieldOptions.BitFieldGet(BitFieldOptions.Encoding.Unsigned(8), new BitFieldOptions.BitOffset(0)),
            new BitFieldOptions.BitFieldGet(BitFieldOptions.Encoding.Unsigned(4), new BitFieldOptions.BitOffset(0))
        };

        // Verify that all subcommands are read-only
        var allReadOnly = readOnlySubCommands.All(cmd => cmd is BitFieldOptions.IBitFieldReadOnlySubCommand);
        Assert.True(allReadOnly);

        // Test that mixed subcommands don't qualify for read-only optimization
        var mixedSubCommands = new BitFieldOptions.IBitFieldSubCommand[]
        {
            new BitFieldOptions.BitFieldGet(BitFieldOptions.Encoding.Unsigned(8), new BitFieldOptions.BitOffset(0)),
            new BitFieldOptions.BitFieldSet(BitFieldOptions.Encoding.Unsigned(8), new BitFieldOptions.BitOffset(0), 100)
        };

        // Verify that mixed subcommands are not all read-only
        var mixedAllReadOnly = mixedSubCommands.All(cmd => cmd is BitFieldOptions.IBitFieldReadOnlySubCommand);
        Assert.False(mixedAllReadOnly);
    }

    [Fact]
    public void ValidateStringCommandArrayConverters()
    {
        // Test MGET with GlideString objects (what the server actually returns)
        var mgetResponse = new object[] { new GlideString("value1"), null!, new GlideString("value3") };
        var mgetResult = Request.Get(["key1", "key2", "key3"]).Converter(mgetResponse);
        Assert.Equal(3, mgetResult.Length);
        Assert.Equal(new ValkeyValue("value1"), mgetResult[0]);
        Assert.Equal(ValkeyValue.Null, mgetResult[1]);
        Assert.Equal(new ValkeyValue("value3"), mgetResult[2]);

        // Test empty MGET response
        var emptyResult = Request.Get([]).Converter([]);
        Assert.Empty(emptyResult);

        // Test MGET with all null values
        var allNullResponse = new object[] { null!, null! };
        var allNullResult = Request.Get(["key1", "key2"]).Converter(allNullResponse);
        Assert.Equal(2, allNullResult.Length);
        Assert.Equal(ValkeyValue.Null, allNullResult[0]);
        Assert.Equal(ValkeyValue.Null, allNullResult[1]);
    }

    [Fact]
    public void ValidateSetCommandHashSetConverters()
    {
        HashSet<object> testHashSet =
        [
            new GlideString("member1"),
            new GlideString("member2"),
            new GlideString("member3")
        ];

        Assert.Multiple([
            () => {
                var result = Request.SetMembers("key").Converter(testHashSet);
                Assert.Equal(3, result.Count);
                Assert.All(result, item => Assert.IsType<ValkeyValue>(item));
            },

            () => {
                var result = Request.SetPop("key", 2).Converter(testHashSet);
                Assert.Equal(3, result.Count);
                Assert.All(result, item => Assert.IsType<ValkeyValue>(item));
            },

            () => {
                var result = Request.SetUnion(["key1", "key2"]).Converter(testHashSet);
                Assert.Equal(3, result.Count);
                Assert.All(result, item => Assert.IsType<ValkeyValue>(item));
            },

            () => {
                var result = Request.SetInter(["key1", "key2"]).Converter(testHashSet);
                Assert.Equal(3, result.Count);
                Assert.All(result, item => Assert.IsType<ValkeyValue>(item));
            },

            () => {
                var result = Request.SetDiff(["key1", "key2"]).Converter(testHashSet);
                Assert.Equal(3, result.Count);
                Assert.All(result, item => Assert.IsType<ValkeyValue>(item));
            },
        ]);
    }

    [Fact]
    public void ValidateHashCommandConverters()
    {
        // Test for HashGetAsync with multiple fields
        List<object?> testList =
        [
            new GlideString("value1"),
            new GlideString("value2"),
            null!,
        ];

        // Test for HashGetAsync (all) and HashRandomFieldsWithValuesAsync
        var testKvpList = new Dictionary<GlideString, object>() {
            { "field1", new GlideString("value1") },
            { "field2", new GlideString("value2") },
            { "field3", new GlideString("value3") }
        };

        object[] testObjectNestedArray =
         [
            new object[] { new GlideString("field1"), new GlideString("value1") },
            new object[] { new GlideString("field2"), new GlideString("value2") },
            new object[] { new GlideString("field3"), new GlideString("value3") },
         ];

        // Test for HashValuesAsync and HashRandomFieldsAsync
        object[] testObjectArray =
        [
            new GlideString("value1"),
            new GlideString("value2"),
            new GlideString("value3")
        ];

        Assert.Multiple(
            // Test HashGetAsync with multiple fields
            () =>
            {
                var result = Request.HashGet("key", ["field1", "field2", "field3"]).Converter((object[])testList.ToArray()!);
                Assert.Equal(3, result.Length);
                Assert.Equal("value1", result[0]);
                Assert.Equal("value2", result[1]);
                Assert.Equal(ValkeyValue.Null, result[2]);
            },

            // Test HashGetAsync (all)
            () =>
            {
                var result = Request.HashGet("key").Converter(testKvpList);
                Assert.Equal(3, result.Count);
                Assert.Equal("value1", result["field1"]);
            },

            // Test HashValuesAsync
            () =>
            {
                var result = Request.HashValues("key").Converter(testObjectArray);
                Assert.Equal(3, result.Count);
                foreach (var item in result)
                {
                    _ = Assert.IsType<ValkeyValue>(item);
                }
            },

            // Test HashRandomFieldAsync
            () =>
            {
                var result = Request.HashRandomField("key").Converter("field1");
                Assert.Equal("field1", result);
            },

            // Test HashRandomFieldsAsync
            () =>
            {
                var result = Request.HashRandomFields("key", 3).Converter(testObjectArray);
                Assert.Equal(3, result.Length);
                foreach (var item in result)
                {
                    _ = Assert.IsType<ValkeyValue>(item);
                }
            },

            // Test HashRandomFieldsWithValuesAsync
            () =>
            {
                var result = Request.HashRandomFieldsWithValues("key", 3).Converter(testObjectNestedArray);
                Assert.Equal(3, result.Length);
                foreach (var entry in result)
                {
                    _ = Assert.IsType<HashEntry>(entry);
                }
            }
        );
    }

    [Fact]
    public void ValidateStreamCommandArgs()
    {
        CmdAssert.Equal(RequestType.XAck, ["key", "group", "1-0"], Request.StreamAcknowledge("key", "group", (ValkeyValue)"1-0"));
        CmdAssert.Equal(RequestType.XAck, ["key", "group", "1-0", "2-0"], Request.StreamAcknowledge("key", "group", ["1-0", "2-0"]));
        CmdAssert.Equal(RequestType.XAdd, ["key", "*", "field", "value"], Request.StreamAdd("key", [new NameValueEntry("field", "value")], new StreamAddOptions()));
        CmdAssert.Equal(RequestType.XAdd, ["key", "1-0", "field1", "value1", "field2", "value2"], Request.StreamAdd("key", [new NameValueEntry("field1", "value1"), new NameValueEntry("field2", "value2")], new StreamAddOptions { Id = "1-0" }));
        CmdAssert.Equal(RequestType.XAdd, ["key", "MAXLEN", "~", "1000", "*", "field", "value"], Request.StreamAdd("key", [new NameValueEntry("field", "value")], new StreamAddOptions { Trim = new StreamTrimOptions.MaxLen { MaxLength = 1000, Exact = false } }));
        CmdAssert.Equal(RequestType.XAdd, ["key", "MINID", "~", "0-1", "*", "field", "value"], Request.StreamAdd("key", [new NameValueEntry("field", "value")], new StreamAddOptions { Trim = new StreamTrimOptions.MinId { MinEntryId = "0-1", Exact = false } }));
        CmdAssert.Equal(RequestType.XAdd, ["key", "NOMKSTREAM", "*", "field", "value"], Request.StreamAdd("key", [new NameValueEntry("field", "value")], new StreamAddOptions { MakeStream = false }));
        CmdAssert.Equal(RequestType.XAutoClaim, ["key", "group", "consumer", "1000", "0-0"], Request.StreamAutoClaim("key", "group", "consumer", StreamAutoClaimOptions.FromId(OneSecond, "0-0")));
        CmdAssert.Equal(RequestType.XAutoClaim, ["key", "group", "consumer", "1000", "0-0", "COUNT", "10"], Request.StreamAutoClaim("key", "group", "consumer", StreamAutoClaimOptions.FromId(OneSecond, "0-0").WithCount(10)));
        CmdAssert.Equal(RequestType.XAutoClaim, ["key", "group", "consumer", "1000", "0-0", "JUSTID"], Request.StreamAutoClaimJustId("key", "group", "consumer", StreamAutoClaimOptions.FromId(OneSecond, "0-0")));
        CmdAssert.Equal(RequestType.XClaim, ["key", "group", "consumer", "1000", "1-0"], Request.StreamClaim("key", "group", "consumer", ["1-0"], StreamClaimOptions.From(OneSecond)));
        CmdAssert.Equal(RequestType.XClaim, ["key", "group", "consumer", "1000", "1-0", "IDLE", "500"], Request.StreamClaim("key", "group", "consumer", ["1-0"], StreamClaimOptions.From(OneSecond).WithIdle(TimeSpan.FromMilliseconds(500))));
        CmdAssert.Equal(RequestType.XClaim, ["key", "group", "consumer", "1000", "1-0", "TIME", "1500"], Request.StreamClaim("key", "group", "consumer", ["1-0"], StreamClaimOptions.From(OneSecond).WithIdleUnix(DateTimeOffset.FromUnixTimeMilliseconds(1500))));
        CmdAssert.Equal(RequestType.XClaim, ["key", "group", "consumer", "1000", "1-0", "FORCE"], Request.StreamClaim("key", "group", "consumer", ["1-0"], StreamClaimOptions.From(OneSecond).WithForce()));
        CmdAssert.Equal(RequestType.XClaim, ["key", "group", "consumer", "1000", "1-0", "JUSTID"], Request.StreamClaimJustIds("key", "group", "consumer", ["1-0"], StreamClaimOptions.From(OneSecond)));
        CmdAssert.Equal(RequestType.XGroupSetId, ["key", "group", "0-0"], Request.StreamGroupSetId("key", "group", "0-0", null));
        CmdAssert.Equal(RequestType.XGroupSetId, ["key", "group", "0-0", "ENTRIESREAD", "5"], Request.StreamGroupSetId("key", "group", "0-0", 5));
        CmdAssert.Equal(RequestType.XInfoConsumers, ["key", "group"], Request.StreamInfoConsumers("key", "group"));
        CmdAssert.Equal(RequestType.XGroupCreateConsumer, ["key", "group", "consumer"], Request.StreamGroupCreateConsumer("key", "group", "consumer"));
        CmdAssert.Equal(RequestType.XGroupCreate, ["key", "group", "0"], Request.StreamGroupCreate("key", "group", "0"));
        CmdAssert.Equal(RequestType.XGroupCreate, ["key", "group", "0"], Request.StreamGroupCreate("key", "group", "0", new StreamGroupCreateOptions()));
        CmdAssert.Equal(RequestType.XGroupCreate, ["key", "group", "$", "MKSTREAM"], Request.StreamGroupCreate("key", "group", StreamPosition.NewMessages, new StreamGroupCreateOptions { MakeStream = true }));
        CmdAssert.Equal(RequestType.XGroupCreate, ["key", "group", "0"], Request.StreamGroupCreate("key", "group", "0", new StreamGroupCreateOptions { MakeStream = false }));
        CmdAssert.Equal(RequestType.XGroupCreate, ["key", "group", "0", "ENTRIESREAD", "10"], Request.StreamGroupCreate("key", "group", "0", new StreamGroupCreateOptions { MakeStream = false, EntriesRead = 10 }));
        CmdAssert.Equal(RequestType.XDel, ["key", "1-0"], Request.StreamDelete("key", (ValkeyValue)"1-0"));
        CmdAssert.Equal(RequestType.XDel, ["key", "1-0", "2-0"], Request.StreamDelete("key", ["1-0", "2-0"]));
        CmdAssert.Equal(RequestType.XGroupDelConsumer, ["key", "group", "consumer"], Request.StreamGroupDeleteConsumer("key", "group", "consumer"));
        CmdAssert.Equal(RequestType.XGroupDestroy, ["key", "group"], Request.StreamGroupDestroy("key", "group"));
        CmdAssert.Equal(RequestType.XInfoGroups, ["key"], Request.StreamInfoGroups("key"));
        CmdAssert.Equal(RequestType.XInfoStream, ["key"], Request.StreamInfo("key"));
        CmdAssert.Equal(RequestType.XInfoStream, ["key", "FULL"], Request.StreamInfoFull("key", null));
        CmdAssert.Equal(RequestType.XInfoStream, ["key", "FULL", "COUNT", "5"], Request.StreamInfoFull("key", 5));
        CmdAssert.Equal(RequestType.XLen, ["key"], Request.StreamLength("key"));
        CmdAssert.Equal(RequestType.XPending, ["key", "group"], Request.StreamPending("key", "group"));
        CmdAssert.Equal(RequestType.XPending, ["key", "group", "-", "+", "10", "consumer"], Request.StreamPending("key", "group", new StreamPendingOptions { Count = 10, ConsumerName = "consumer" }));
        CmdAssert.Equal(RequestType.XPending, ["key", "group", "IDLE", "1000", "-", "+", "10", "consumer"], Request.StreamPending("key", "group", new StreamPendingOptions { Count = 10, ConsumerName = "consumer", MinIdleTime = OneSecond }));
        CmdAssert.Equal(RequestType.XRange, ["key", "-", "+"], Request.StreamRange("key", new StreamRangeOptions()));
        CmdAssert.Equal(RequestType.XRange, ["key", "1-0", "2-0", "COUNT", "10"], Request.StreamRange("key", new StreamRangeOptions { Range = StreamIdRange.Between("1-0", "2-0"), Count = 10 }));
        CmdAssert.Equal(RequestType.XRevRange, ["key", "+", "-"], Request.StreamRange("key", new StreamRangeOptions { Order = Order.Descending }));
        CmdAssert.Equal(RequestType.XRead, ["STREAMS", "key", "0-0"], Request.StreamRead(new StreamPosition("key", "0-0"), new StreamReadOptions()));
        CmdAssert.Equal(RequestType.XRead, ["COUNT", "10", "STREAMS", "key", "0-0"], Request.StreamRead(new StreamPosition("key", "0-0"), new StreamReadOptions { Count = 10 }));
        CmdAssert.Equal(RequestType.XRead, ["STREAMS", "key1", "key2", "0-0", "1-0"], Request.StreamRead([new StreamPosition("key1", "0-0"), new StreamPosition("key2", "1-0")], new StreamReadOptions()));
        CmdAssert.Equal(RequestType.XReadGroup, ["GROUP", "group", "consumer", "STREAMS", "key", ">"], Request.StreamReadGroup(new StreamPosition("key", ">"), "group", "consumer", new StreamReadGroupOptions()));
        CmdAssert.Equal(RequestType.XReadGroup, ["GROUP", "group", "consumer", "COUNT", "10", "STREAMS", "key", ">"], Request.StreamReadGroup(new StreamPosition("key", ">"), "group", "consumer", new StreamReadGroupOptions { Count = 10 }));
        CmdAssert.Equal(RequestType.XReadGroup, ["GROUP", "group", "consumer", "NOACK", "STREAMS", "key", ">"], Request.StreamReadGroup(new StreamPosition("key", ">"), "group", "consumer", new StreamReadGroupOptions { NoAck = true }));
        CmdAssert.Equal(RequestType.XReadGroup, ["GROUP", "group", "consumer", "STREAMS", "key1", "key2", ">", ">"], Request.StreamReadGroup([new StreamPosition("key1", ">"), new StreamPosition("key2", ">")], "group", "consumer", new StreamReadGroupOptions()));
        CmdAssert.Equal(RequestType.XReadGroup, ["GROUP", "group", "consumer", "COUNT", "5", "STREAMS", "key1", "key2", ">", "0-0"], Request.StreamReadGroup([new StreamPosition("key1", ">"), new StreamPosition("key2", "0-0")], "group", "consumer", new StreamReadGroupOptions { Count = 5 }));
        CmdAssert.Equal(RequestType.XReadGroup, ["GROUP", "group", "consumer", "NOACK", "STREAMS", "key1", "key2", ">", ">"], Request.StreamReadGroup([new StreamPosition("key1", ">"), new StreamPosition("key2", ">")], "group", "consumer", new StreamReadGroupOptions { NoAck = true }));
        CmdAssert.Equal(RequestType.XTrim, ["key", "MAXLEN", "1000"], Request.StreamTrim("key", new StreamTrimOptions.MaxLen { MaxLength = 1000 }));
        CmdAssert.Equal(RequestType.XTrim, ["key", "MAXLEN", "~", "1000"], Request.StreamTrim("key", new StreamTrimOptions.MaxLen { MaxLength = 1000, Exact = false }));
        CmdAssert.Equal(RequestType.XTrim, ["key", "MINID", "0-1"], Request.StreamTrim("key", new StreamTrimOptions.MinId { MinEntryId = "0-1" }));
    }

    [Fact]
    public void ValidateStreamCommandConverters()
    {
        Assert.True(Request.StreamAcknowledge("key", "group", (ValkeyValue)"1-0").Converter(1L));
        Assert.False(Request.StreamAcknowledge("key", "group", (ValkeyValue)"1-0").Converter(0L));
        Assert.Equal(2L, Request.StreamAcknowledge("key", "group", ["1-0", "2-0"]).Converter(2L));
        Assert.Equal(new ValkeyValue("1-0"), Request.StreamAdd("key", [new NameValueEntry("f", "v")], new StreamAddOptions()).Converter("1-0"));
        Assert.Equal(ValkeyValue.Null, Request.StreamAdd("key", [new NameValueEntry("f", "v")], new StreamAddOptions()).Converter(null!));
        Assert.Equal(ValkeyValue.Ok, Request.StreamGroupSetId("key", "group", "0-0", null).Converter("OK"));
        Assert.True(Request.StreamGroupCreateConsumer("key", "group", "consumer").Converter(true));
        Assert.False(Request.StreamGroupCreateConsumer("key", "group", "consumer").Converter(false));
        Assert.Equal(ValkeyValue.Ok, Request.StreamGroupCreate("key", "group", default, new StreamGroupCreateOptions { MakeStream = true }).Converter("OK"));
        Assert.True(Request.StreamDelete("key", (ValkeyValue)"1-0").Converter(1L));
        Assert.False(Request.StreamDelete("key", (ValkeyValue)"1-0").Converter(0L));
        Assert.Equal(2L, Request.StreamDelete("key", ["1-0", "2-0"]).Converter(2L));
        Assert.Equal(0L, Request.StreamDelete("key", ["1-0"]).Converter(0L));
        Assert.Equal(5L, Request.StreamGroupDeleteConsumer("key", "group", "consumer").Converter(5L));
        Assert.True(Request.StreamGroupDestroy("key", "group").Converter(true));
        Assert.False(Request.StreamGroupDestroy("key", "group").Converter(false));
        Assert.Equal(5L, Request.StreamLength("key").Converter(5L));
        Assert.Equal(0L, Request.StreamLength("key").Converter(0L));
        Assert.Equal(10L, Request.StreamTrim("key", new StreamTrimOptions.MaxLen { MaxLength = 100 }).Converter(10L));
    }

    [Fact]
    public void StreamInfo_Converter()
    {
        var raw = new Dictionary<GlideString, object>
        {
            ["length"] = 3L,
            ["radix-tree-keys"] = 1L,
            ["radix-tree-nodes"] = 2L,
            ["last-generated-id"] = new GlideString("5-0"),
            ["max-deleted-entry-id"] = new GlideString("2-0"),
            ["entries-added"] = 4L,
            ["recorded-first-entry-id"] = new GlideString("3-0"),
            ["groups"] = 1L,
            ["first-entry"] = new object[] { new GlideString("3-0"), new object[] { new GlideString("f1"), new GlideString("v1") } },
            ["last-entry"] = new object[] { new GlideString("5-0"), new object[] { new GlideString("f2"), new GlideString("v2") } },
        };

        var info = Request.StreamInfo("key").Converter(raw);

        Assert.Equal(3, info.Length);
        Assert.Equal(1, info.RadixTreeKeys);
        Assert.Equal(2, info.RadixTreeNodes);
        Assert.Equal(1, info.ConsumerGroupCount);
        Assert.Equal(new ValkeyValue("5-0"), info.LastGeneratedId);

        // Fields added since server 7.0.
        Assert.Equal(new ValkeyValue("2-0"), info.MaxDeletedEntryId);
        Assert.Equal(4L, info.EntriesAdded);
        Assert.Equal(new ValkeyValue("3-0"), info.RecordedFirstEntryId);

        Assert.Equal(new ValkeyValue("3-0"), info.FirstEntry.Id);
        Assert.Equal(new ValkeyValue("5-0"), info.LastEntry.Id);
    }

    [Fact]
    public void StreamInfo_Converter_OmitsPre7Fields()
    {
        // Older servers (< 7.0) do not return the newer fields (max-deleted-entry-id, entries-added,
        // recorded-first-entry-id); they should default cleanly. The remaining fields are always present.
        var raw = new Dictionary<GlideString, object>
        {
            ["length"] = 1L,
            ["radix-tree-keys"] = 1L,
            ["radix-tree-nodes"] = 2L,
            ["last-generated-id"] = new GlideString("1-0"),
            ["groups"] = 0L,
            ["first-entry"] = null!,
            ["last-entry"] = null!,
        };

        var info = Request.StreamInfo("key").Converter(raw);

        Assert.Equal(1, info.Length);
        Assert.Equal(ValkeyValue.Null, info.MaxDeletedEntryId);
        Assert.Null(info.EntriesAdded);
        Assert.Equal(ValkeyValue.Null, info.RecordedFirstEntryId);

        Assert.True(info.FirstEntry.IsNull);
        Assert.True(info.LastEntry.IsNull);
    }

    [Fact]
    public void StreamConsumerInfo_Converter()
    {
        var raw = new object[]
        {
            new Dictionary<GlideString, object>
            {
                ["name"] = new GlideString("consumer1"),
                ["pending"] = 3L,
                ["idle"] = 5000L,
                ["inactive"] = 8000L,
            },
        };

        StreamConsumerInfo consumer = Assert.Single(Request.StreamInfoConsumers("key", "group").Converter(raw));

        Assert.Equal("consumer1", consumer.Name);
        Assert.Equal(3, consumer.PendingMessageCount);
        Assert.Equal(5000L, consumer.IdleTimeInMilliseconds);
        Assert.Equal(TimeSpan.FromMilliseconds(5000), consumer.Idle);
        Assert.Equal(TimeSpan.FromMilliseconds(8000), consumer.Inactive);
    }

    [Fact]
    public void StreamConsumerInfo_Converter_OmitsInactive()
    {
        // Servers < 7.2 do not return the `inactive` field; InactiveTime should be null.
        var raw = new object[]
        {
            new Dictionary<GlideString, object>
            {
                ["name"] = new GlideString("consumer1"),
                ["pending"] = 1L,
                ["idle"] = 100L,
            },
        };

        StreamConsumerInfo consumer = Assert.Single(Request.StreamInfoConsumers("key", "group").Converter(raw));

        Assert.Equal(TimeSpan.FromMilliseconds(100), consumer.Idle);
        Assert.Null(consumer.Inactive);
    }

    #region StreamRead Converter Tests

    [Fact]
    public void StreamRead_SingleConverter()
    {
        var raw = new Dictionary<GlideString, object>
        {
            ["mystream"] = new Dictionary<GlideString, object>
            {
                ["0-0"] = null!,
                ["1-0"] = new object[] { new object[] { new GlideString("f1"), new GlideString("v1") } },
                ["2-0"] = new object[]
                {
                    new object[] { new GlideString("f1"), new GlideString("v1") },
                    new object[] { new GlideString("f2"), new GlideString("v2") },
                    new object[] { new GlideString("f3"), new GlideString("v3") },
                },
            },
        };

        var entries = Request.StreamRead(new StreamPosition("mystream", "0")).Converter(raw);

        // Verify that nil entry is skipped.
        Assert.Equal(2, entries.Length);

        // Verify entry with single field.
        Assert.Equal("1-0", entries[0].Id);
        Assert.Equal(new NameValueEntry[] { new("f1", "v1") }, entries[0].Values);

        // Verify entry with multiple fields.
        Assert.Equal("2-0", entries[1].Id);
        Assert.Equal(new NameValueEntry[] { new("f1", "v1"), new("f2", "v2"), new("f3", "v3") }, entries[1].Values);
    }

    [Fact]
    public void StreamRead_MultiConverter()
    {
        var raw = new Dictionary<GlideString, object>
        {
            ["stream0"] = new Dictionary<GlideString, object>(),
            ["stream1"] = new Dictionary<GlideString, object>
            {
                ["1-0"] = new object[] { new object[] { new GlideString("v1"), new GlideString("f1") } },
            },
        };

        var streams = Request.ConvertValkeyStreamResponse(raw);
        Assert.Equal(2, streams.Length);

        // Verify empty stream.
        Assert.Equal("stream0", streams[0].Key);
        Assert.Empty(streams[0].Entries);

        // Verify non-empty stream.
        Assert.Equal("stream1", streams[1].Key);
        var entry = Assert.Single(streams[1].Entries);
        Assert.Equal("1-0", entry.Id);
        Assert.Equal(new NameValueEntry[] { new("v1", "f1") }, entry.Values);
    }

    #endregion
    #region MemoryStats Converter Tests

    [Fact]
    public void MemoryStatsConverter_WithAllFields()
    {
        var db0 = new Dictionary<GlideString, object>()
        {
            ["overhead.hashtable.main"] = ConvertLong,
            ["overhead.hashtable.expires"] = ConvertLong,
        };

        var db1 = new Dictionary<GlideString, object>()
        {
            ["overhead.hashtable.main"] = ConvertLong,
            ["overhead.hashtable.expires"] = ConvertLong,
        };

        var raw = new Dictionary<GlideString, object>()
        {
            ["db.0"] = db0,
            ["db.1"] = db1,
            ["allocator.active"] = ConvertLong,
            ["allocator.allocated"] = ConvertLong,
            ["allocator-fragmentation.bytes"] = ConvertLong,
            ["allocator.resident"] = ConvertLong,
            ["allocator-rss.bytes"] = ConvertLong,
            ["aof.buffer"] = ConvertLong,
            ["clients.normal"] = ConvertLong,
            ["clients.slaves"] = ConvertLong,
            ["dataset.bytes"] = ConvertLong,
            ["fragmentation.bytes"] = ConvertLong,
            ["keys.bytes-per-key"] = ConvertLong,
            ["keys.count"] = ConvertLong,
            ["lua.caches"] = ConvertLong,
            ["overhead.total"] = ConvertLong,
            ["peak.allocated"] = ConvertLong,
            ["replication.backlog"] = ConvertLong,
            ["rss-overhead.bytes"] = ConvertLong,
            ["startup.allocated"] = ConvertLong,
            ["total.allocated"] = ConvertLong,
            ["allocator-fragmentation.ratio"] = ConvertDouble,
            ["allocator-rss.ratio"] = ConvertDouble,
            ["dataset.percentage"] = ConvertDouble,
            ["fragmentation"] = ConvertDouble,
            ["peak.percentage"] = ConvertDouble,
            ["rss-overhead.ratio"] = ConvertDouble,

            // Optional 7.0+ fields
            ["cluster.links"] = ConvertLong,
            ["functions.caches"] = ConvertLong,

            // Optional 8.0+ fields
            ["allocator.muzzy"] = ConvertLong,
            ["db.dict.rehashing.count"] = ConvertLong,
            ["overhead.db.hashtable.lut"] = ConvertLong,
            ["overhead.db.hashtable.rehashing"] = ConvertLong,
        };

        var stats = Request.ParseMemoryStats(raw);

        Assert.Equal(2, stats.Db.Count);
        Assert.Equal(ConvertLong, stats.Db[0].OverheadHashtableMain);
        Assert.Equal(ConvertLong, stats.Db[0].OverheadHashtableExpires);
        Assert.Equal(ConvertLong, stats.Db[1].OverheadHashtableMain);
        Assert.Equal(ConvertLong, stats.Db[1].OverheadHashtableExpires);
        Assert.Equal(ConvertLong, stats.AllocatorActive);
        Assert.Equal(ConvertLong, stats.AllocatorAllocated);
        Assert.Equal(ConvertLong, stats.AllocatorFragmentationBytes);
        Assert.Equal(ConvertLong, stats.AllocatorResident);
        Assert.Equal(ConvertLong, stats.AllocatorRssBytes);
        Assert.Equal(ConvertLong, stats.AofBuffer);
        Assert.Equal(ConvertLong, stats.ClientsNormal);
        Assert.Equal(ConvertLong, stats.ClientsSlaves);
        Assert.Equal(ConvertLong, stats.DatasetBytes);
        Assert.Equal(ConvertLong, stats.FragmentationBytes);
        Assert.Equal(ConvertLong, stats.KeysBytesPerKey);
        Assert.Equal(ConvertLong, stats.KeysCount);
        Assert.Equal(ConvertLong, stats.LuaCaches);
        Assert.Equal(ConvertLong, stats.OverheadTotal);
        Assert.Equal(ConvertLong, stats.PeakAllocated);
        Assert.Equal(ConvertLong, stats.ReplicationBacklog);
        Assert.Equal(ConvertLong, stats.RssOverheadBytes);
        Assert.Equal(ConvertLong, stats.StartupAllocated);
        Assert.Equal(ConvertLong, stats.TotalAllocated);
        Assert.Equal(ConvertDouble, stats.AllocatorFragmentationRatio);
        Assert.Equal(ConvertDouble, stats.AllocatorRssRatio);
        Assert.Equal(ConvertDouble, stats.DatasetPercentage);
        Assert.Equal(ConvertDouble, stats.Fragmentation);
        Assert.Equal(ConvertDouble, stats.PeakPercentage);
        Assert.Equal(ConvertDouble, stats.RssOverheadRatio);
        Assert.Equal(ConvertLong, stats.ClusterLinks);
        Assert.Equal(ConvertLong, stats.FunctionsCaches);
        Assert.Equal(ConvertLong, stats.AllocatorMuzzy);
        Assert.Equal(ConvertLong, stats.DbDictRehashingCount);
        Assert.Equal(ConvertLong, stats.OverheadDbHashtableLut);
        Assert.Equal(ConvertLong, stats.OverheadDbHashtableRehashing);
    }

    [Fact]
    public void MemoryStatsConverter_WithoutOptionalFields()
    {
        var raw = new Dictionary<GlideString, object>()
        {
            ["allocator.active"] = ConvertLong,
            ["allocator.allocated"] = ConvertLong,
            ["allocator-fragmentation.bytes"] = ConvertLong,
            ["allocator.resident"] = ConvertLong,
            ["allocator-rss.bytes"] = ConvertLong,
            ["aof.buffer"] = ConvertLong,
            ["clients.normal"] = ConvertLong,
            ["clients.slaves"] = ConvertLong,
            ["dataset.bytes"] = ConvertLong,
            ["fragmentation.bytes"] = ConvertLong,
            ["keys.bytes-per-key"] = ConvertLong,
            ["keys.count"] = ConvertLong,
            ["lua.caches"] = ConvertLong,
            ["overhead.total"] = ConvertLong,
            ["peak.allocated"] = ConvertLong,
            ["replication.backlog"] = ConvertLong,
            ["rss-overhead.bytes"] = ConvertLong,
            ["startup.allocated"] = ConvertLong,
            ["total.allocated"] = ConvertLong,
            ["allocator-fragmentation.ratio"] = ConvertDouble,
            ["allocator-rss.ratio"] = ConvertDouble,
            ["dataset.percentage"] = ConvertDouble,
            ["fragmentation"] = ConvertDouble,
            ["peak.percentage"] = ConvertDouble,
            ["rss-overhead.ratio"] = ConvertDouble,
        };

        var stats = Request.ParseMemoryStats(raw);

        Assert.Empty(stats.Db);
        Assert.Equal(ConvertLong, stats.AllocatorActive);
        Assert.Equal(ConvertLong, stats.TotalAllocated);
        Assert.Equal(ConvertDouble, stats.AllocatorFragmentationRatio);
        Assert.Null(stats.ClusterLinks);
        Assert.Null(stats.FunctionsCaches);
        Assert.Null(stats.AllocatorMuzzy);
        Assert.Null(stats.DbDictRehashingCount);
        Assert.Null(stats.OverheadDbHashtableLut);
        Assert.Null(stats.OverheadDbHashtableRehashing);
    }

    #endregion
}
