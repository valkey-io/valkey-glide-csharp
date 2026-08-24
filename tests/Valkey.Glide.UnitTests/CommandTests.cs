// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands.Options;

namespace Valkey.Glide.UnitTests;

public class CommandTests
{
    private static readonly TimeSpan OneSecond = TimeSpan.FromSeconds(1);

    [Fact]
    public void ValidateCommandArgs() => Assert.Multiple(
            () => Assert.Equal(["get", "a"], Request.CustomCommand(["get", "a"]).GetArgs()),
            () => Assert.Equal(["ping", "pong", "pang"], Request.CustomCommand(["ping", "pong", "pang"]).GetArgs()),
            () => Assert.Equal(["get"], Request.CustomCommand(["get"]).GetArgs()),
            () => Assert.Equal([], Request.CustomCommand([]).GetArgs()),

            // String Commands
            () => Assert.Equal(["SET", "key", "value"], Request.Set("key", "value", new SetOptions()).GetArgs()),
            () => Assert.Equal(["SET", "key", "value", "NX"], Request.Set("key", "value", new SetOptions { Condition = SetCondition.OnlyIfDoesNotExist }).GetArgs()),
            () => Assert.Equal(["SET", "key", "value", "XX"], Request.Set("key", "value", new SetOptions { Condition = SetCondition.OnlyIfExists }).GetArgs()),
            () => Assert.Equal(["SET", "key", "value", "NX", "PX", "5000"], Request.Set("key", "value", new SetOptions { Condition = SetCondition.OnlyIfDoesNotExist, Expiry = SetExpiryOptions.ExpireIn(TimeSpan.FromSeconds(5)) }).GetArgs()),
            () => Assert.Equal(["SET", "key", "value", "PX", "10000"], Request.Set("key", "value", new SetOptions { Expiry = SetExpiryOptions.ExpireIn(TimeSpan.FromSeconds(10)) }).GetArgs()),
            () => Assert.Equal(["SET", "key", "value", "KEEPTTL"], Request.Set("key", "value", new SetOptions { Expiry = SetExpiryOptions.KeepTimeToLive() }).GetArgs()),
            () => Assert.Equal(["SET", "key", "value", "GET"], Request.GetSet("key", "value", new SetOptions()).GetArgs()),
            () => Assert.Equal(["SET", "key", "value", "NX", "GET"], Request.GetSet("key", "value", new SetOptions { Condition = SetCondition.OnlyIfDoesNotExist }).GetArgs()),
            () => Assert.Equal(["SET", "key", "value", "XX", "GET"], Request.GetSet("key", "value", new SetOptions { Condition = SetCondition.OnlyIfExists }).GetArgs()),
            () => Assert.Equal(["SET", "key", "value", "NX", "PX", "5000", "GET"], Request.GetSet("key", "value", new SetOptions { Condition = SetCondition.OnlyIfDoesNotExist, Expiry = SetExpiryOptions.ExpireIn(TimeSpan.FromSeconds(5)) }).GetArgs()),
            () => Assert.Equal(["SET", "key", "value", "PX", "10000", "GET"], Request.GetSet("key", "value", new SetOptions { Expiry = SetExpiryOptions.ExpireIn(TimeSpan.FromSeconds(10)) }).GetArgs()),
            () => Assert.Equal(["GET", "key"], Request.Get("key").GetArgs()),
            () => Assert.Equal(["MGET", "key1", "key2", "key3"], Request.Get(["key1", "key2", "key3"]).GetArgs()),
            () => Assert.Equal(["MSET", "key1", "value1", "key2", "value2"], Request.Set([
                new KeyValuePair<ValkeyKey, ValkeyValue>("key1", "value1"),
                new KeyValuePair<ValkeyKey, ValkeyValue>("key2", "value2")
            ]).GetArgs()),
            () => Assert.Equal(["STRLEN", "key"], Request.Length("key").GetArgs()),
            () => Assert.Equal(["GETRANGE", "key", "0", "5"], Request.GetRange("key", 0, 5).GetArgs()),
            () => Assert.Equal(["SETRANGE", "key", "10", "value"], Request.SetRange("key", 10, "value").GetArgs()),
            () => Assert.Equal(["APPEND", "key", "value"], Request.Append("key", "value").GetArgs()),
            () => Assert.Equal(11L, Request.Append("key", "value").Converter(11L)),
            () => Assert.Equal(["DECR", "key"], Request.Decrement("key").GetArgs()),
            () => Assert.Equal(["DECRBY", "key", "5"], Request.DecrementBy("key", 5).GetArgs()),
            () => Assert.Equal(["INCR", "key"], Request.Increment("key").GetArgs()),
            () => Assert.Equal(["INCRBY", "key", "5"], Request.IncrementBy("key", 5).GetArgs()),
            () => Assert.Equal(["INCRBYFLOAT", "key", "0.5"], Request.IncrementByFloat("key", 0.5).GetArgs()),
            () => Assert.Equal(["MSETNX", "key1", "value1", "key2", "value2"], Request.SetIfNotExists([
                new KeyValuePair<ValkeyKey, ValkeyValue>("key1", "value1"),
                new KeyValuePair<ValkeyKey, ValkeyValue>("key2", "value2")
            ]).GetArgs()),
            () => Assert.Equal(["MSETNX"], Request.SetIfNotExists([]).GetArgs()),
            () => Assert.Equal(["GETDEL", "key"], Request.GetDelete("key").GetArgs()),
            () => Assert.Equal(["GETDEL", "test_key"], Request.GetDelete("test_key").GetArgs()),
            () => Assert.Equal(["GETEX", "key", "PX", "60000"], Request.GetExpiry("key", GetExpiryOptions.ExpireIn(TimeSpan.FromSeconds(60))).GetArgs()),
            () => Assert.Equal(["GETEX", "test_key", "PX", "60000"], Request.GetExpiry("test_key", GetExpiryOptions.ExpireIn(TimeSpan.FromSeconds(60))).GetArgs()),
            () => Assert.Equal(["GETEX", "key", "PERSIST"], Request.GetExpiry("key", GetExpiryOptions.Persist()).GetArgs()),
            () => Assert.Equal(["GETEX", "test_key", "PERSIST"], Request.GetExpiry("test_key", GetExpiryOptions.Persist()).GetArgs()),
            () => Assert.Equal(["GETEX", "key", "PXAT", "1609459200000"], Request.GetExpiry("key", GetExpiryOptions.ExpireAt(new DateTimeOffset(2021, 1, 1, 0, 0, 0, TimeSpan.Zero))).GetArgs()),
            () => Assert.Equal(["GETEX", "test_key", "PXAT", "1609459200000"], Request.GetExpiry("test_key", GetExpiryOptions.ExpireAt(new DateTimeOffset(2021, 1, 1, 0, 0, 0, TimeSpan.Zero))).GetArgs()),
            () => Assert.Equal(["LCS", "key1", "key2"], Request.LongestCommonSubsequence("key1", "key2").GetArgs()),
            () => Assert.Equal(["LCS", "key1", "key2", "LEN"], Request.LongestCommonSubsequenceLength("key1", "key2").GetArgs()),
            () => Assert.Equal(["LCS", "key1", "key2", "IDX", "MINMATCHLEN", "0", "WITHMATCHLEN"], Request.LongestCommonSubsequenceWithMatches("key1", "key2").GetArgs()),
            () => Assert.Equal(["LCS", "key1", "key2", "IDX", "MINMATCHLEN", "5", "WITHMATCHLEN"], Request.LongestCommonSubsequenceWithMatches("key1", "key2", 5).GetArgs()),
            () => Assert.Equal(["LCS", "key1", "key2", "IDX", "MINMATCHLEN", "0", "WITHMATCHLEN"], Request.LongestCommonSubsequenceWithMatches("key1", "key2", 0).GetArgs()),

            // Info Command Args
            () => Assert.Equal(["INFO"], Request.Info([]).GetArgs()),
            () => Assert.Equal(["INFO", "SERVER"], Request.Info([InfoOptions.Section.SERVER]).GetArgs()),
            () => Assert.Equal(["INFO", "CLIENTS"], Request.Info([InfoOptions.Section.CLIENTS]).GetArgs()),
            () => Assert.Equal(["INFO", "MEMORY"], Request.Info([InfoOptions.Section.MEMORY]).GetArgs()),
            () => Assert.Equal(["INFO", "PERSISTENCE"], Request.Info([InfoOptions.Section.PERSISTENCE]).GetArgs()),
            () => Assert.Equal(["INFO", "STATS"], Request.Info([InfoOptions.Section.STATS]).GetArgs()),
            () => Assert.Equal(["INFO", "REPLICATION"], Request.Info([InfoOptions.Section.REPLICATION]).GetArgs()),
            () => Assert.Equal(["INFO", "CPU"], Request.Info([InfoOptions.Section.CPU]).GetArgs()),
            () => Assert.Equal(["INFO", "COMMANDSTATS"], Request.Info([InfoOptions.Section.COMMANDSTATS]).GetArgs()),
            () => Assert.Equal(["INFO", "LATENCYSTATS"], Request.Info([InfoOptions.Section.LATENCYSTATS]).GetArgs()),
            () => Assert.Equal(["INFO", "SENTINEL"], Request.Info([InfoOptions.Section.SENTINEL]).GetArgs()),
            () => Assert.Equal(["INFO", "CLUSTER"], Request.Info([InfoOptions.Section.CLUSTER]).GetArgs()),
            () => Assert.Equal(["INFO", "MODULES"], Request.Info([InfoOptions.Section.MODULES]).GetArgs()),
            () => Assert.Equal(["INFO", "KEYSPACE"], Request.Info([InfoOptions.Section.KEYSPACE]).GetArgs()),
            () => Assert.Equal(["INFO", "ERRORSTATS"], Request.Info([InfoOptions.Section.ERRORSTATS]).GetArgs()),
            () => Assert.Equal(["INFO", "ALL"], Request.Info([InfoOptions.Section.ALL]).GetArgs()),
            () => Assert.Equal(["INFO", "DEFAULT"], Request.Info([InfoOptions.Section.DEFAULT]).GetArgs()),
            () => Assert.Equal(["INFO", "EVERYTHING"], Request.Info([InfoOptions.Section.EVERYTHING]).GetArgs()),
            () => Assert.Equal(["INFO", "CLIENTS", "CPU"], Request.Info([InfoOptions.Section.CLIENTS, InfoOptions.Section.CPU]).GetArgs()),
            () => Assert.Equal(["INFO", "SERVER", "MEMORY", "STATS"], Request.Info([InfoOptions.Section.SERVER, InfoOptions.Section.MEMORY, InfoOptions.Section.STATS]).GetArgs()),

            // Connection Management Commands
            () => Assert.Equal(["CLIENTGETNAME"], Request.ClientGetName().GetArgs()),
            () => Assert.Equal(["CLIENTID"], Request.ClientId().GetArgs()),
            () => Assert.Equal(["CLIENTPAUSE", "1000", "WRITE"], Request.ClientPauseWrite(OneSecond).GetArgs()),
            () => Assert.Equal(["CLIENTPAUSE", "1000"], Request.ClientPause(OneSecond).GetArgs()),
            () => Assert.Equal(["CLIENTTRACKINGINFO"], Request.ClientTrackingInfo().GetArgs()),
            () => Assert.Equal(["CLIENTUNPAUSE"], Request.ClientUnpause().GetArgs()),
            () => Assert.Equal(["ECHO", "message"], Request.Echo("message").GetArgs()),
            () => Assert.Equal(["PING", ""], Request.Ping("").GetArgs()),
            () => Assert.Equal(["PING", "Hello"], Request.Ping("Hello").GetArgs()),
            () => Assert.Equal(["PING", "PONG"], Request.Ping("PONG").GetArgs()),
            () => Assert.Equal(["PING", "test message"], Request.Ping("test message").GetArgs()),
            () => Assert.Equal(["PING"], Request.Ping().GetArgs()),
            () => Assert.Equal(["SELECT", "-1"], Request.Select(-1).GetArgs()),
            () => Assert.Equal(["SELECT", "0"], Request.Select(0).GetArgs()),
            () => Assert.Equal(["SELECT", "1"], Request.Select(1).GetArgs()),
            () => Assert.Equal(["SELECT", "15"], Request.Select(15).GetArgs()),

            // Server Management Commands
            () => Assert.Equal(["BGREWRITEAOF"], Request.BgRewriteAof().GetArgs()),
            () => Assert.Equal(["BGSAVE", "CANCEL"], Request.BackgroundSaveCancel().GetArgs()),
            () => Assert.Equal(["BGSAVE", "SCHEDULE"], Request.BackgroundSaveSchedule().GetArgs()),
            () => Assert.Equal(["BGSAVE"], Request.BackgroundSave().GetArgs()),
            () => Assert.Equal(["CONFIGGET", "*"], Request.ConfigGet("*").GetArgs()),
            () => Assert.Equal(["CONFIGGET", "*"], Request.ConfigGet().GetArgs()),
            () => Assert.Equal(["CONFIGGET", "maxmemory", "lfu-decay-time"], Request.ConfigGet([(ValkeyValue)"maxmemory", (ValkeyValue)"lfu-decay-time"]).GetArgs()),
            () => Assert.Equal(["CONFIGGET", "maxmemory"], Request.ConfigGet("maxmemory").GetArgs()),
            () => Assert.Equal(["CONFIGRESETSTAT"], Request.ConfigResetStatistics().GetArgs()),
            () => Assert.Equal(["CONFIGREWRITE"], Request.ConfigRewrite().GetArgs()),
            () => Assert.Equal(["CONFIGSET", "lfu-decay-time", "5", "lfu-log-factor", "20"], Request.ConfigSet(new Dictionary<ValkeyValue, ValkeyValue> { { "lfu-decay-time", "5" }, { "lfu-log-factor", "20" } }).GetArgs()),
            () => Assert.Equal(["CONFIGSET", "maxmemory", "100mb"], Request.ConfigSet("maxmemory", "100mb").GetArgs()),
            () => Assert.Equal(["CONFIGSET", "timeout", "300"], Request.ConfigSet("timeout", "300").GetArgs()),
            () => Assert.Equal(["DBSIZE"], Request.DatabaseSize().GetArgs()),
            () => Assert.Equal(["DBSIZE"], Request.DatabaseSize().GetArgs()),
            () => Assert.Equal(["FAILOVER", "ABORT"], Request.Failover(FailoverOptions.Abort()).GetArgs()),
            () => Assert.Equal(["FAILOVER", "TIMEOUT", "5000"], Request.Failover(FailoverOptions.Timeout(TimeSpan.FromSeconds(5))).GetArgs()),
            () => Assert.Equal(["FAILOVER", "TO", "localhost", "6380", "FORCE", "TIMEOUT", "5000"], Request.Failover(FailoverOptions.Forced("localhost", 6380, TimeSpan.FromSeconds(5))).GetArgs()),
            () => Assert.Equal(["FAILOVER", "TO", "localhost", "6380", "TIMEOUT", "5000"], Request.Failover(FailoverOptions.To("localhost", 6380, TimeSpan.FromSeconds(5))).GetArgs()),
            () => Assert.Equal(["FAILOVER", "TO", "localhost", "6380"], Request.Failover(FailoverOptions.To("localhost", 6380)).GetArgs()),
            () => Assert.Equal(["FAILOVER"], Request.Failover().GetArgs()),
            () => Assert.Equal(["FLUSHALL", "ASYNC"], Request.FlushAllDatabases(FlushMode.Async).GetArgs()),
            () => Assert.Equal(["FLUSHALL", "SYNC"], Request.FlushAllDatabases(FlushMode.Sync).GetArgs()),
            () => Assert.Equal(["FLUSHALL"], Request.FlushAllDatabases().GetArgs()),
            () => Assert.Equal(["FLUSHDB", "ASYNC"], Request.FlushDatabase(FlushMode.Async).GetArgs()),
            () => Assert.Equal(["FLUSHDB", "SYNC"], Request.FlushDatabase(FlushMode.Sync).GetArgs()),
            () => Assert.Equal(["FLUSHDB"], Request.FlushDatabase().GetArgs()),
            () => Assert.Equal(["FLUSHDB"], Request.FlushDatabase().GetArgs()),
            () => Assert.Equal(["LASTSAVE"], Request.LastSave().GetArgs()),
            () => Assert.Equal(["LATENCY", "HISTORY", "command"], Request.LatencyHistory("command").GetArgs()),
            () => Assert.Equal(["LATENCY", "LATEST"], Request.LatencyLatest().GetArgs()),
            () => Assert.Equal(["LATENCY", "RESET", "command", "fast-command"], Request.LatencyReset(["command", "fast-command"]).GetArgs()),
            () => Assert.Equal(["LATENCY", "RESET", "command"], Request.LatencyReset(["command"]).GetArgs()),
            () => Assert.Equal(["LATENCY", "RESET"], Request.LatencyReset([]).GetArgs()),
            () => Assert.Equal(["LOLWUT", "40", "20"], Request.Lolwut(new LolwutOptions { Parameters = [40, 20] }).GetArgs()),
            () => Assert.Equal(["LOLWUT", "VERSION", "5"], Request.Lolwut(new LolwutOptions { Version = 5 }).GetArgs()),
            () => Assert.Equal(["LOLWUT", "VERSION", "6", "40", "20"], Request.Lolwut(new LolwutOptions { Version = 6, Parameters = [40, 20] }).GetArgs()),
            () => Assert.Equal(["LOLWUT"], Request.Lolwut().GetArgs()),
            () => Assert.Equal(["LOLWUT"], Request.Lolwut(options: null).GetArgs()),
            () => Assert.Equal(["MEMORY", "DOCTOR"], Request.MemoryDoctor().GetArgs()),
            () => Assert.Equal(["MEMORY", "MALLOC-STATS"], Request.MemoryMallocStats().GetArgs()),
            () => Assert.Equal(["MEMORY", "PURGE"], Request.MemoryPurge().GetArgs()),
            () => Assert.Equal(["MEMORY", "STATS"], Request.MemoryStats().GetArgs()),
            () => Assert.Equal(["REPLICAOF", "localhost", "6379"], Request.ReplicaOf("localhost", 6379).GetArgs()),
            () => Assert.Equal(["REPLICAOF", "NO", "ONE"], Request.ReplicaOfNoOne().GetArgs()),
            () => Assert.Equal(["RESET"], Request.Reset().GetArgs()),
            () => Assert.Equal(["SAVE"], Request.Save().GetArgs()),
            () => Assert.Equal(["TIME"], Request.Time().GetArgs()),

            // Set Commands
            () => Assert.Equal(["SADD", "key", "member"], Request.SetAdd("key", "member").GetArgs()),
            () => Assert.Equal(["SADD", "key", "member1", "member2"], Request.SetAdd("key", ["member1", "member2"]).GetArgs()),
            () => Assert.Equal(["SREM", "key", "member"], Request.SetRemove("key", "member").GetArgs()),
            () => Assert.Equal(["SREM", "key", "member1", "member2"], Request.SetRemove("key", ["member1", "member2"]).GetArgs()),
            () => Assert.Equal(["SMEMBERS", "key"], Request.SetMembers("key").GetArgs()),
            () => Assert.Equal(["SCARD", "key"], Request.SetCard("key").GetArgs()),
            () => Assert.Equal(["SINTERCARD", "2", "key1", "key2"], Request.SetInterCard(["key1", "key2"]).GetArgs()),
            () => Assert.Equal(["SINTERCARD", "2", "key1", "key2", "LIMIT", "10"], Request.SetInterCard(["key1", "key2"], 10).GetArgs()),
            () => Assert.Equal(["SPOP", "key"], Request.SetPop("key").GetArgs()),
            () => Assert.Equal(["SPOP", "key", "3"], Request.SetPop("key", 3).GetArgs()),
            () => Assert.Equal(["SUNION", "key1", "key2"], Request.SetUnion(["key1", "key2"]).GetArgs()),
            () => Assert.Equal(["SINTER", "key1", "key2"], Request.SetInter(["key1", "key2"]).GetArgs()),
            () => Assert.Equal(["SDIFF", "key1", "key2"], Request.SetDiff(["key1", "key2"]).GetArgs()),
            () => Assert.Equal(["SUNIONSTORE", "dest", "key1", "key2"], Request.SetUnionStore("dest", ["key1", "key2"]).GetArgs()),
            () => Assert.Equal(["SINTERSTORE", "dest", "key1", "key2"], Request.SetInterStore("dest", ["key1", "key2"]).GetArgs()),
            () => Assert.Equal(["SDIFFSTORE", "dest", "key1", "key2"], Request.SetDiffStore("dest", ["key1", "key2"]).GetArgs()),
            () => Assert.Equal(["SISMEMBER", "key", "member"], Request.SetIsMember("key", "member").GetArgs()),
            () => Assert.Equal(["SISMEMBER", "mykey", "value"], Request.SetIsMember("mykey", "value").GetArgs()),
            () => Assert.Equal(["SISMEMBER", "test:set", "test-member"], Request.SetIsMember("test:set", "test-member").GetArgs()),
            () => Assert.Equal(["SMISMEMBER", "key", "member1", "member2", "member3"], Request.SetIsMember("key", ["member1", "member2", "member3"]).GetArgs()),
            () => Assert.Equal(["SMISMEMBER", "key"], Request.SetIsMember("key", []).GetArgs()),
            () => Assert.Equal(["SMISMEMBER", "key", "", " ", "null", "0", "-1"], Request.SetIsMember("key", ["", " ", "null", "0", "-1"]).GetArgs()),
            () => Assert.Equal(["SRANDMEMBER", "key"], Request.SetRandomMember("key").GetArgs()),
            () => Assert.Equal(["SRANDMEMBER", "mykey"], Request.SetRandomMember("mykey").GetArgs()),
            () => Assert.Equal(["SRANDMEMBER", "test:set"], Request.SetRandomMember("test:set").GetArgs()),
            () => Assert.Equal(["SRANDMEMBER", "key", "3"], Request.SetRandomMembers("key", 3).GetArgs()),
            () => Assert.Equal(["SRANDMEMBER", "key", "-5"], Request.SetRandomMembers("key", -5).GetArgs()),
            () => Assert.Equal(["SRANDMEMBER", "key", "0"], Request.SetRandomMembers("key", 0).GetArgs()),
            () => Assert.Equal(["SRANDMEMBER", "key", "1"], Request.SetRandomMembers("key", 1).GetArgs()),
            () => Assert.Equal(["SMOVE", "source", "dest", "member"], Request.SetMove("source", "dest", "member").GetArgs()),
            () => Assert.Equal(["SMOVE", "key1", "key2", "value"], Request.SetMove("key1", "key2", "value").GetArgs()),
            () => Assert.Equal(["SMOVE", "src:set", "dst:set", "test-member"], Request.SetMove("src:set", "dst:set", "test-member").GetArgs()),
            () => Assert.Equal(["SSCAN", "key", "0"], Request.SetScan("key", 0).GetArgs()),
            () => Assert.Equal(["SSCAN", "key", "10"], Request.SetScan("key", 10).GetArgs()),
            () => Assert.Equal(["SSCAN", "mykey", "0"], Request.SetScan("mykey", 0).GetArgs()),
            () => Assert.Equal(["SSCAN", "key", "0", "MATCH", "pattern*"], Request.SetScan("key", 0, new ScanOptions { MatchPattern = "pattern*" }).GetArgs()),
            () => Assert.Equal(["SSCAN", "key", "5", "MATCH", "test*"], Request.SetScan("key", 5, new ScanOptions { MatchPattern = "test*" }).GetArgs()),
            () => Assert.Equal(["SSCAN", "key", "0", "MATCH", "*suffix"], Request.SetScan("key", 0, new ScanOptions { MatchPattern = "*suffix" }).GetArgs()),
            () => Assert.Equal(["SSCAN", "key", "0", "COUNT", "10"], Request.SetScan("key", 0, new ScanOptions { Count = 10 }).GetArgs()),
            () => Assert.Equal(["SSCAN", "key", "5", "COUNT", "20"], Request.SetScan("key", 5, new ScanOptions { Count = 20 }).GetArgs()),
            () => Assert.Equal(["SSCAN", "key", "0", "COUNT", "1"], Request.SetScan("key", 0, new ScanOptions { Count = 1 }).GetArgs()),
            () => Assert.Equal(["SSCAN", "key", "0", "MATCH", "pattern*", "COUNT", "10"], Request.SetScan("key", 0, new ScanOptions { MatchPattern = "pattern*", Count = 10 }).GetArgs()),
            () => Assert.Equal(["SSCAN", "key", "5", "MATCH", "test*", "COUNT", "20"], Request.SetScan("key", 5, new ScanOptions { MatchPattern = "test*", Count = 20 }).GetArgs()),
            () => Assert.Equal(["SSCAN", "key", "10", "MATCH", "*suffix", "COUNT", "5"], Request.SetScan("key", 10, new ScanOptions { MatchPattern = "*suffix", Count = 5 }).GetArgs()),
            () => Assert.Equal(["SISMEMBER", "", "member"], Request.SetIsMember("", "member").GetArgs()),
            () => Assert.Equal(["SISMEMBER", "key", ""], Request.SetIsMember("key", "").GetArgs()),
            () => Assert.Equal(["SMOVE", "", "", ""], Request.SetMove("", "", "").GetArgs()),
            () => Assert.Equal(["SSCAN", "", "0"], Request.SetScan("", 0).GetArgs()),

            // Generic Commands
            () => Assert.Equal(["COPY", "src", "dest", "DB", "1", "REPLACE"], Request.Copy("src", "dest", 1, true).GetArgs()),
            () => Assert.Equal(["COPY", "src", "dest"], Request.Copy("src", "dest").GetArgs()),
            () => Assert.Equal(["DEL", "key"], Request.Delete("key").GetArgs()),
            () => Assert.Equal(["DEL", "key1", "key2"], Request.Delete(["key1", "key2"]).GetArgs()),
            () => Assert.Equal(["DUMP", "key"], Request.Dump("key").GetArgs()),
            () => Assert.Equal(["EXISTS", "key"], Request.Exists("key").GetArgs()),
            () => Assert.Equal(["EXISTS", "key1", "key2"], Request.Exists(["key1", "key2"]).GetArgs()),
            () => Assert.Equal(["MIGRATE", "host", "6379", "", "0", "5000", "KEYS", "k1", "k2"], Request.Migrate(["k1", "k2"], new MigrateOptions("host", 6379, 0, TimeSpan.FromSeconds(5))).GetArgs()),
            () => Assert.Equal(["MIGRATE", "host", "6379", "key", "0", "5000", "COPY", "REPLACE"], Request.Migrate(["key"], new MigrateOptions("host", 6379, 0, TimeSpan.FromSeconds(5)).WithCopy().WithReplace()).GetArgs()),
            () => Assert.Equal(["MOVE", "key", "1"], Request.Move("key", 1).GetArgs()),
            () => Assert.Equal(["OBJECT", "ENCODING", "key"], Request.ObjectEncoding("key").GetArgs()),
            () => Assert.Equal(["OBJECT", "FREQ", "key"], Request.ObjectFrequency("key").GetArgs()),
            () => Assert.Equal(["OBJECT", "IDLETIME", "key"], Request.ObjectIdleTime("key").GetArgs()),
            () => Assert.Equal(["OBJECT", "REFCOUNT", "key"], Request.ObjectRefCount("key").GetArgs()),
            () => Assert.Equal(["PERSIST", "key"], Request.Persist("key").GetArgs()),
            () => Assert.Equal(["PEXPIRE", "key", "60000", "NX"], Request.Expire("key", TimeSpan.FromSeconds(60), ExpireCondition.OnlyIfNotExists).GetArgs()),
            () => Assert.Equal(["PEXPIRE", "key", "60000"], Request.Expire("key", TimeSpan.FromSeconds(60)).GetArgs()),
            () => Assert.Equal(["PEXPIREAT", "key", "1609459200000"], Request.Expire("key", new DateTimeOffset(2021, 1, 1, 0, 0, 0, TimeSpan.Zero)).GetArgs()),
            () => Assert.Equal(["PEXPIRETIME", "key"], Request.ExpireTime("key").GetArgs()),
            () => Assert.Equal(["PTTL", "key"], Request.TimeToLive("key").GetArgs()),
            () => Assert.Equal(["RANDOMKEY"], Request.RandomKey().GetArgs()),
            () => Assert.Equal(["RENAME", "oldkey", "newkey"], Request.Rename("oldkey", "newkey").GetArgs()),
            () => Assert.Equal(["RENAMENX", "oldkey", "newkey"], Request.RenameIfNotExists("oldkey", "newkey").GetArgs()),
            () => Assert.Equal(["RESTORE", "key", "0", "data", "FREQ", "5"], Request.Restore("key", "data"u8.ToArray(), new RestoreOptions { Frequency = 5 }).GetArgs()),
            () => Assert.Equal(["RESTORE", "key", "0", "data", "IDLETIME", "1000"], Request.Restore("key", "data"u8.ToArray(), new RestoreOptions { IdleTime = 1000 }).GetArgs()),
            () => Assert.Equal(["RESTORE", "key", "0", "data", "REPLACE", "FREQ", "10"], Request.Restore("key", "data"u8.ToArray(), new RestoreOptions { Replace = true, Frequency = 10 }).GetArgs()),
            () => Assert.Equal(["RESTORE", "key", "0", "data", "REPLACE", "IDLETIME", "2000"], Request.Restore("key", "data"u8.ToArray(), new RestoreOptions { Replace = true, IdleTime = 2000 }).GetArgs()),
            () => Assert.Equal(["RESTORE", "key", "0", "data", "REPLACE"], Request.Restore("key", "data"u8.ToArray(), new RestoreOptions { Replace = true }).GetArgs()),
            () => Assert.Equal(["RESTORE", "key", "0", "data"], Request.Restore("key", "data"u8.ToArray()).GetArgs()),
            () => Assert.Equal(["RESTORE", "key", "2303596800000", "data", "ABSTTL", "FREQ", "10"], Request.Restore("key", "data"u8.ToArray(), new RestoreOptions { ExpireAt = new DateTimeOffset(2042, 12, 31, 0, 0, 0, TimeSpan.Zero), Frequency = 10 }).GetArgs()),
            () => Assert.Equal(["RESTORE", "key", "2303596800000", "data", "ABSTTL", "REPLACE"], Request.Restore("key", "data"u8.ToArray(), new RestoreOptions { ExpireAt = new DateTimeOffset(2042, 12, 31, 0, 0, 0, TimeSpan.Zero), Replace = true }).GetArgs()),
            () => Assert.Equal(["RESTORE", "key", "2303596800000", "data", "ABSTTL"], Request.Restore("key", "data"u8.ToArray(), new RestoreOptions { ExpireAt = new DateTimeOffset(2042, 12, 31, 0, 0, 0, TimeSpan.Zero) }).GetArgs()),
            () => Assert.Equal(["RESTORE", "key", "5000", "data", "IDLETIME", "1000"], Request.Restore("key", "data"u8.ToArray(), new RestoreOptions { Ttl = TimeSpan.FromSeconds(5), IdleTime = 1000 }).GetArgs()),
            () => Assert.Equal(["RESTORE", "key", "5000", "data", "REPLACE"], Request.Restore("key", "data"u8.ToArray(), new RestoreOptions { Ttl = TimeSpan.FromSeconds(5), Replace = true }).GetArgs()),
            () => Assert.Equal(["RESTORE", "key", "5000", "data"], Request.Restore("key", "data"u8.ToArray(), new RestoreOptions { Ttl = TimeSpan.FromSeconds(5) }).GetArgs()),
            () => Assert.Equal(["TOUCH", "key"], Request.Touch("key").GetArgs()),
            () => Assert.Equal(["TOUCH", "key1", "key2"], Request.Touch(["key1", "key2"]).GetArgs()),
            () => Assert.Equal(["TYPE", "key"], Request.Type("key").GetArgs()),
            () => Assert.Equal(["UNLINK", "key"], Request.Unlink("key").GetArgs()),
            () => Assert.Equal(["UNLINK", "key1", "key2"], Request.Unlink(["key1", "key2"]).GetArgs()),

            () => Assert.Throws<ArgumentException>(() => Request.Restore("key", "data"u8.ToArray(), new RestoreOptions { IdleTime = 1000, Frequency = 5 }).GetArgs()),
            () => Assert.Throws<ArgumentException>(() => Request.Restore("key", "data"u8.ToArray(), new RestoreOptions { Ttl = TimeSpan.FromSeconds(5), ExpireAt = DateTimeOffset.UtcNow }).GetArgs()),

            // SCAN Commands
            () => Assert.Equal(["SCAN", "0"], Request.Scan("0").GetArgs()),
            () => Assert.Equal(["SCAN", "10"], Request.Scan("10").GetArgs()),
            () => Assert.Equal(["SCAN", "0", "MATCH", "pattern*"], Request.Scan("0", new ScanOptions { MatchPattern = "pattern*" }).GetArgs()),
            () => Assert.Equal(["SCAN", "5", "MATCH", "test*"], Request.Scan("5", new ScanOptions { MatchPattern = "test*" }).GetArgs()),
            () => Assert.Equal(["SCAN", "0", "COUNT", "10"], Request.Scan("0", new ScanOptions { Count = 10 }).GetArgs()),
            () => Assert.Equal(["SCAN", "5", "COUNT", "20"], Request.Scan("5", new ScanOptions { Count = 20 }).GetArgs()),
            () => Assert.Equal(["SCAN", "0", "TYPE", "string"], Request.Scan("0", new ScanOptions { Type = ValkeyType.String }).GetArgs()),
            () => Assert.Equal(["SCAN", "5", "TYPE", "list"], Request.Scan("5", new ScanOptions { Type = ValkeyType.List }).GetArgs()),
            () => Assert.Equal(["SCAN", "0", "TYPE", "set"], Request.Scan("0", new ScanOptions { Type = ValkeyType.Set }).GetArgs()),
            () => Assert.Equal(["SCAN", "0", "TYPE", "zset"], Request.Scan("0", new ScanOptions { Type = ValkeyType.SortedSet }).GetArgs()),
            () => Assert.Equal(["SCAN", "0", "TYPE", "hash"], Request.Scan("0", new ScanOptions { Type = ValkeyType.Hash }).GetArgs()),
            () => Assert.Equal(["SCAN", "0", "TYPE", "stream"], Request.Scan("0", new ScanOptions { Type = ValkeyType.Stream }).GetArgs()),
            () => Assert.Equal(["SCAN", "10", "MATCH", "key*", "COUNT", "20", "TYPE", "string"], Request.Scan("10", new ScanOptions { MatchPattern = "key*", Count = 20, Type = ValkeyType.String }).GetArgs()),

            // WAIT Commands
            () => Assert.Equal(["WAIT", "1", "1000"], Request.Wait(1, OneSecond).GetArgs()),
            () => Assert.Equal(["WAIT", "0", "0"], Request.Wait(0, TimeSpan.Zero).GetArgs()),
            () => Assert.Equal(["WAIT", "3", "5000"], Request.Wait(3, TimeSpan.FromMilliseconds(5000)).GetArgs()),
            () => Assert.Equal(["WAITAOF", "1", "1", "1000"], Request.WaitAof(true, 1, OneSecond).GetArgs()),
            () => Assert.Equal(["WAITAOF", "0", "0", "0"], Request.WaitAof(false, 0, TimeSpan.Zero).GetArgs()),
            () => Assert.Equal(["WAITAOF", "1", "2", "5000"], Request.WaitAof(true, 2, TimeSpan.FromMilliseconds(5000)).GetArgs()),

            // List Commands
            () => Assert.Equal(["LPOP", "a"], Request.ListLeftPop("a").GetArgs()),
            () => Assert.Equal(["LPOP", "a", "3"], Request.ListLeftPop("a", 3).GetArgs()),
            () => Assert.Equal(["LPUSH", "a", "value"], Request.ListLeftPush("a", "value").GetArgs()),
            () => Assert.Equal(["LPUSH", "a", "one", "two"], Request.ListLeftPush("a", ["one", "two"]).GetArgs()),
            () => Assert.Equal(["RPOP", "a"], Request.ListRightPop("a").GetArgs()),
            () => Assert.Equal(["RPOP", "a", "2"], Request.ListRightPop("a", 2).GetArgs()),
            () => Assert.Equal(["RPUSH", "a", "value"], Request.ListRightPush("a", "value").GetArgs()),
            () => Assert.Equal(["RPUSH", "a", "one", "two"], Request.ListRightPush("a", ["one", "two"]).GetArgs()),
            () => Assert.Equal(["LLEN", "a"], Request.ListLength("a").GetArgs()),
            () => Assert.Equal(["LREM", "a", "0", "value"], Request.ListRemove("a", "value", 0).GetArgs()),
            () => Assert.Equal(["LREM", "a", "2", "value"], Request.ListRemove("a", "value", 2).GetArgs()),
            () => Assert.Equal(["LREM", "a", "-1", "value"], Request.ListRemove("a", "value", -1).GetArgs()),
            () => Assert.Equal(["LTRIM", "a", "0", "10"], Request.ListTrim("a", 0, 10).GetArgs()),
            () => Assert.Equal(["LTRIM", "a", "1", "-1"], Request.ListTrim("a", 1, -1).GetArgs()),
            () => Assert.Equal(["LRANGE", "a", "0", "-1"], Request.ListRange("a", 0, -1).GetArgs()),
            () => Assert.Equal(["LRANGE", "a", "1", "5"], Request.ListRange("a", 1, 5).GetArgs()),
            () => Assert.Equal(["BLPOP", "key1", "key2", "5"], Request.ListBlockingLeftPop(["key1", "key2"], TimeSpan.FromSeconds(5)).GetArgs()),
            () => Assert.Equal(["BLPOP", "key", "0"], Request.ListBlockingLeftPop(["key"], TimeSpan.Zero).GetArgs()),
            () => Assert.Equal(["BLPOP", "a", "b", "c", "10"], Request.ListBlockingLeftPop(["a", "b", "c"], TimeSpan.FromSeconds(10)).GetArgs()),
            () => Assert.Equal(["BLPOP", "single", "0.5"], Request.ListBlockingLeftPop(["single"], TimeSpan.FromMilliseconds(500)).GetArgs()),
            () => Assert.Equal(["BRPOP", "key1", "key2", "10"], Request.ListBlockingRightPop(["key1", "key2"], TimeSpan.FromSeconds(10)).GetArgs()),
            () => Assert.Equal(["BRPOP", "key", "0"], Request.ListBlockingRightPop(["key"], TimeSpan.Zero).GetArgs()),
            () => Assert.Equal(["BRPOP", "x", "y", "z", "0.5"], Request.ListBlockingRightPop(["x", "y", "z"], TimeSpan.FromMilliseconds(500)).GetArgs()),
            () => Assert.Equal(["BRPOP", "test", "1.5"], Request.ListBlockingRightPop(["test"], TimeSpan.FromSeconds(1.5)).GetArgs()),
            () => Assert.Equal(["BLMOVE", "source", "dest", "LEFT", "RIGHT", "3"], Request.ListBlockingMove("source", "dest", ListSide.Left, ListSide.Right, TimeSpan.FromSeconds(3)).GetArgs()),
            () => Assert.Equal(["BLMOVE", "src", "dst", "RIGHT", "LEFT", "0"], Request.ListBlockingMove("src", "dst", ListSide.Right, ListSide.Left, TimeSpan.Zero).GetArgs()),
            () => Assert.Equal(["BLMOVE", "a", "b", "LEFT", "LEFT", "2.5"], Request.ListBlockingMove("a", "b", ListSide.Left, ListSide.Left, TimeSpan.FromSeconds(2.5)).GetArgs()),
            () => Assert.Equal(["BLMOVE", "list1", "list2", "RIGHT", "RIGHT", "1"], Request.ListBlockingMove("list1", "list2", ListSide.Right, ListSide.Right, TimeSpan.FromSeconds(1)).GetArgs()),
            () => Assert.Equal(["BLMPOP", "2", "2", "key1", "key2", "LEFT"], Request.ListBlockingPop(["key1", "key2"], ListSide.Left, TimeSpan.FromSeconds(2)).GetArgs()),
            () => Assert.Equal(["BLMPOP", "0", "1", "key", "RIGHT"], Request.ListBlockingPop(["key"], ListSide.Right, TimeSpan.Zero).GetArgs()),
            () => Assert.Equal(["BLMPOP", "1.5", "3", "a", "b", "c", "LEFT"], Request.ListBlockingPop(["a", "b", "c"], ListSide.Left, TimeSpan.FromSeconds(1.5)).GetArgs()),
            () => Assert.Equal(["BLMPOP", "0.25", "1", "single", "RIGHT"], Request.ListBlockingPop(["single"], ListSide.Right, TimeSpan.FromMilliseconds(250)).GetArgs()),
            () => Assert.Equal(["BLMPOP", "5", "2", "key1", "key2", "LEFT", "COUNT", "3"], Request.ListBlockingPop(["key1", "key2"], ListSide.Left, 3, TimeSpan.FromSeconds(5)).GetArgs()),
            () => Assert.Equal(["BLMPOP", "0", "1", "key", "RIGHT", "COUNT", "10"], Request.ListBlockingPop(["key"], ListSide.Right, 10, TimeSpan.Zero).GetArgs()),
            () => Assert.Equal(["BLMPOP", "2.5", "4", "w", "x", "y", "z", "LEFT", "COUNT", "1"], Request.ListBlockingPop(["w", "x", "y", "z"], ListSide.Left, 1, TimeSpan.FromSeconds(2.5)).GetArgs()),
            () => Assert.Equal(["BLMPOP", "1", "1", "test", "RIGHT", "COUNT", "5"], Request.ListBlockingPop(["test"], ListSide.Right, 5, TimeSpan.FromSeconds(1)).GetArgs()),
            () => Assert.Equal(["LMPOP", "2", "key1", "key2", "LEFT", "COUNT", "3"], Request.ListLeftPop(["key1", "key2"], 3).GetArgs()),
            () => Assert.Equal(["LMPOP", "2", "key1", "key2", "RIGHT", "COUNT", "3"], Request.ListRightPop(["key1", "key2"], 3).GetArgs()),
            () => Assert.Equal(["LPUSHX", "a", "value"], Request.ListLeftPush("a", "value", When.Exists).GetArgs()),
            () => Assert.Equal(["LPUSHX", "a", "one", "two"], Request.ListLeftPush("a", ["one", "two"], When.Exists).GetArgs()),
            () => Assert.Equal(["RPUSHX", "a", "value"], Request.ListRightPush("a", "value", When.Exists).GetArgs()),
            () => Assert.Equal(["RPUSHX", "a", "one", "two"], Request.ListRightPush("a", ["one", "two"], When.Exists).GetArgs()),
            () => Assert.Equal(["LINDEX", "a", "0"], Request.ListGetByIndex("a", 0).GetArgs()),
            () => Assert.Equal(["LINDEX", "a", "-1"], Request.ListGetByIndex("a", -1).GetArgs()),
            () => Assert.Equal(["LINSERT", "a", "BEFORE", "pivot", "value"], Request.ListInsertBefore("a", "pivot", "value").GetArgs()),
            () => Assert.Equal(["LINSERT", "a", "AFTER", "pivot", "value"], Request.ListInsertAfter("a", "pivot", "value").GetArgs()),
            () => Assert.Equal(["LMOVE", "src", "dest", "LEFT", "RIGHT"], Request.ListMove("src", "dest", ListSide.Left, ListSide.Right).GetArgs()),
            () => Assert.Equal(["LMOVE", "src", "dest", "RIGHT", "LEFT"], Request.ListMove("src", "dest", ListSide.Right, ListSide.Left).GetArgs()),
            () => Assert.Equal(["LPOS", "a", "element"], Request.ListPosition("a", "element").GetArgs()),
            () => Assert.Equal(["LPOS", "a", "element", "RANK", "2"], Request.ListPosition("a", "element", 2).GetArgs()),
            () => Assert.Equal(["LPOS", "a", "element", "MAXLEN", "100"], Request.ListPosition("a", "element", 1, 100).GetArgs()),
            () => Assert.Equal(["LPOS", "a", "element", "COUNT", "5"], Request.ListPositions("a", "element", 5).GetArgs()),
            () => Assert.Equal(["LPOS", "a", "element", "COUNT", "5", "RANK", "2"], Request.ListPositions("a", "element", 5, 2).GetArgs()),
            () => Assert.Equal(["LPOS", "a", "element", "COUNT", "5", "MAXLEN", "50"], Request.ListPositions("a", "element", 5, 1, 50).GetArgs()),
            () => Assert.Equal(["LSET", "a", "0", "value"], Request.ListSetByIndex("a", 0, "value").GetArgs()),
            () => Assert.Equal(["LSET", "a", "-1", "value"], Request.ListSetByIndex("a", -1, "value").GetArgs()),

            // Hash Commands
            () => Assert.Equal(new string[] { "HGET", "key", "field" }, Request.HashGet("key", "field").GetArgs()),
            () => Assert.Equal(["HMGET", "key", "field1", "field2"], Request.HashGet("key", ["field1", "field2"]).GetArgs()),
            () => Assert.Equal(new string[] { "HGETALL", "key" }, Request.HashGet("key").GetArgs()),
            () => Assert.Equal(["HSET", "key", "field1", "value1", "field2", "value2"], Request.HashSet("key", [new KeyValuePair<ValkeyValue, ValkeyValue>("field1", "value1"), new KeyValuePair<ValkeyValue, ValkeyValue>("field2", "value2")]).GetArgs()),
            () => Assert.Equal(["HSET", "key", "field", "value"], Request.HashSet("key", "field", "value").GetArgs()),
            () => Assert.Equal(["HSETNX", "key", "field", "value"], Request.HashSetNotExists("key", "field", "value").GetArgs()),
            () => Assert.Equal(new string[] { "HDEL", "key", "field" }, Request.HashDelete("key", "field").GetArgs()),
            () => Assert.Equal(["HDEL", "key", "field1", "field2"], Request.HashDelete("key", ["field1", "field2"]).GetArgs()),
            () => Assert.Equal(new string[] { "HEXISTS", "key", "field" }, Request.HashExists("key", "field").GetArgs()),
            () => Assert.Equal(new string[] { "HINCRBY", "key", "field", "5" }, Request.HashIncrementBy("key", "field", 5L).GetArgs()),
            () => Assert.Equal(new string[] { "HINCRBY", "key", "field", "1" }, Request.HashIncrementBy("key", "field", 1L).GetArgs()),
            () => Assert.Equal(new string[] { "HINCRBYFLOAT", "key", "field", "2.5" }, Request.HashIncrementBy("key", "field", 2.5).GetArgs()),
            () => Assert.Equal(new string[] { "HKEYS", "key" }, Request.HashKeys("key").GetArgs()),
            () => Assert.Equal(new string[] { "HLEN", "key" }, Request.HashLength("key").GetArgs()),
            () => Assert.Equal(new string[] { "HSTRLEN", "key", "field" }, Request.HashStringLength("key", "field").GetArgs()),
            () => Assert.Equal(new string[] { "HVALS", "key" }, Request.HashValues("key").GetArgs()),
            () => Assert.Equal(new string[] { "HRANDFIELD", "key" }, Request.HashRandomField("key").GetArgs()),
            () => Assert.Equal(new string[] { "HRANDFIELD", "key", "3" }, Request.HashRandomFields("key", 3).GetArgs()),
            () => Assert.Equal(new string[] { "HRANDFIELD", "key", "3", "WITHVALUES" }, Request.HashRandomFieldsWithValues("key", 3).GetArgs()),

            // Geospatial Commands
            () => Assert.Equal(["GEOADD", "key", "13.361389000000001", "38.115555999999998", "Palermo"], Request.GeoAdd("key", "Palermo", new GeoPosition(13.361389, 38.115556)).GetArgs()),
            () => Assert.Equal(["GEOADD", "key", "15.087268999999999", "37.502668999999997", "Catania", "13.361389000000001", "38.115555999999998", "Palermo"], Request.GeoAdd("key", new SortedDictionary<ValkeyValue, GeoPosition> { ["Palermo"] = new(13.361389, 38.115556), ["Catania"] = new(15.087269, 37.502669) }).GetArgs()),
            () => Assert.Equal(["GEOADD", "key", "NX", "13.361389000000001", "38.115555999999998", "Palermo"], Request.GeoAdd("key", "Palermo", new GeoPosition(13.361389, 38.115556), new GeoAddOptions { Condition = GeoAddCondition.OnlyIfNotExists }).GetArgs()),
            () => Assert.Equal(["GEOADD", "key", "XX", "13.361389000000001", "38.115555999999998", "Palermo"], Request.GeoAdd("key", "Palermo", new GeoPosition(13.361389, 38.115556), new GeoAddOptions { Condition = GeoAddCondition.OnlyIfExists }).GetArgs()),
            () => Assert.Equal(["GEOADD", "key", "13.361389000000001", "38.115555999999998", "Palermo"], Request.GeoAdd("key", "Palermo", new GeoPosition(13.361389, 38.115556), new GeoAddOptions { Condition = GeoAddCondition.Always }).GetArgs()),
            () => Assert.Equal(["GEOADD", "key", "CH", "13.361389000000001", "38.115555999999998", "Palermo"], Request.GeoAdd("key", "Palermo", new GeoPosition(13.361389, 38.115556), new GeoAddOptions { Changed = true }).GetArgs()),
            () => Assert.Equal(["GEOADD", "key", "NX", "CH", "13.361389000000001", "38.115555999999998", "Palermo"], Request.GeoAdd("key", "Palermo", new GeoPosition(13.361389, 38.115556), new GeoAddOptions { Condition = GeoAddCondition.OnlyIfNotExists, Changed = true }).GetArgs()),
            () => Assert.Equal(["GEODIST", "key", "Palermo", "Catania", "m"], Request.GeoDistance("key", "Palermo", "Catania", GeoUnit.Meters).GetArgs()),
            () => Assert.Equal(["GEODIST", "key", "Palermo", "Catania", "km"], Request.GeoDistance("key", "Palermo", "Catania", GeoUnit.Kilometers).GetArgs()),
            () => Assert.Equal(["GEODIST", "key", "Palermo", "Catania", "mi"], Request.GeoDistance("key", "Palermo", "Catania", GeoUnit.Miles).GetArgs()),
            () => Assert.Equal(["GEODIST", "key", "Palermo", "Catania", "ft"], Request.GeoDistance("key", "Palermo", "Catania", GeoUnit.Feet).GetArgs()),
            () => Assert.Equal(["GEOHASH", "key", "Palermo"], Request.GeoHash("key", "Palermo").GetArgs()),
            () => Assert.Equal(["GEOHASH", "key", "Palermo", "Catania"], Request.GeoHash("key", ["Palermo", "Catania"]).GetArgs()),
            () => Assert.Equal(["GEOPOS", "key", "Palermo"], Request.GeoPosition("key", "Palermo").GetArgs()),
            () => Assert.Equal(["GEOPOS", "key", "Palermo", "Catania"], Request.GeoPosition("key", ["Palermo", "Catania"]).GetArgs()),
            () => Assert.Equal(["GEOSEARCH", "key", "FROMMEMBER", "Palermo", "BYRADIUS", "100", "km"], Request.GeoSearch("key", "Palermo", new GeoSearchCircle(100, GeoUnit.Kilometers)).GetArgs()),
            () => Assert.Equal(["GEOSEARCH", "key", "FROMLONLAT", "13.361389000000001", "38.115555999999998", "BYRADIUS", "200", "m"], Request.GeoSearch("key", new GeoPosition(13.361389, 38.115556), new GeoSearchCircle(200, GeoUnit.Meters)).GetArgs()),
            () => Assert.Equal(["GEOSEARCH", "key", "FROMMEMBER", "Palermo", "BYBOX", "300", "400", "km"], Request.GeoSearch("key", "Palermo", new GeoSearchBox(400, 300, GeoUnit.Kilometers)).GetArgs()),
            () => Assert.Equal(["GEOSEARCH", "key", "FROMMEMBER", "Palermo", "BYRADIUS", "100", "km", "ASC", "COUNT", "10", "WITHCOORD", "WITHDIST", "WITHHASH"], Request.GeoSearch("key", "Palermo", new GeoSearchCircle(100, GeoUnit.Kilometers), new GeoSearchOptions { Order = Order.Ascending, Count = 10, WithPosition = true, WithDistance = true, WithHash = true }).GetArgs()),
            () => Assert.Equal(["GEOSEARCH", "key", "FROMMEMBER", "Palermo", "BYRADIUS", "100", "km", "COUNT", "5", "ANY"], Request.GeoSearch("key", "Palermo", new GeoSearchCircle(100, GeoUnit.Kilometers), new GeoSearchOptions { Count = 5, Any = true }).GetArgs()),
            () => Assert.Equal(["GEOSEARCHSTORE", "dest", "key", "FROMMEMBER", "Palermo", "BYRADIUS", "100", "km"], Request.GeoSearchAndStore("key", "dest", "Palermo", new GeoSearchCircle(100, GeoUnit.Kilometers)).GetArgs()),
            () => Assert.Equal(["GEOSEARCHSTORE", "dest", "key", "FROMLONLAT", "13.361389000000001", "38.115555999999998", "BYRADIUS", "200", "m", "STOREDIST"], Request.GeoSearchAndStore("key", "dest", new GeoPosition(13.361389, 38.115556), new GeoSearchCircle(200, GeoUnit.Meters), new GeoSearchStoreOptions { StoreDistances = true }).GetArgs()),
            () => Assert.Equal(["GEOSEARCHSTORE", "dest", "key", "FROMMEMBER", "Palermo", "BYRADIUS", "100", "km", "ASC", "COUNT", "5", "ANY"], Request.GeoSearchAndStore("key", "dest", "Palermo", new GeoSearchCircle(100, GeoUnit.Kilometers), new GeoSearchStoreOptions { Order = Order.Ascending, Count = 5, Any = true }).GetArgs()),
            () => Assert.Throws<ArgumentException>(() => Request.GeoSearch("key", "Palermo", new GeoSearchCircle(100, GeoUnit.Kilometers), new GeoSearchOptions { Any = true })),
            () => Assert.Throws<ArgumentException>(() => Request.GeoSearchAndStore("key", "dest", "Palermo", new GeoSearchCircle(100, GeoUnit.Kilometers), new GeoSearchStoreOptions { Any = true })),

            // HyperLogLog Commands
            () => Assert.Equal(["PFADD", "key", "element"], Request.HyperLogLogAdd("key", "element").GetArgs()),
            () => Assert.Equal(["PFADD", "key", "element1", "element2", "element3"], Request.HyperLogLogAdd("key", ["element1", "element2", "element3"]).GetArgs()),
            () => Assert.Equal(["PFCOUNT", "key"], Request.HyperLogLogLength("key").GetArgs()),
            () => Assert.Equal(["PFCOUNT", "key1", "key2", "key3"], Request.HyperLogLogLength(["key1", "key2", "key3"]).GetArgs()),
            () => Assert.Equal(["PFMERGE", "dest", "src1", "src2"], Request.HyperLogLogMerge("dest", "src1", "src2").GetArgs()),
            () => Assert.Equal(["PFMERGE", "dest", "src1", "src2", "src3"], Request.HyperLogLogMerge("dest", ["src1", "src2", "src3"]).GetArgs()),

            // Bitmap Commands
            () => Assert.Equal(["GETBIT", "key", "0"], Request.GetBit("key", 0).GetArgs()),
            () => Assert.Equal(["GETBIT", "key", "100"], Request.GetBit("key", 100).GetArgs()),
            () => Assert.Equal(["SETBIT", "key", "0", "1"], Request.SetBit("key", 0, true).GetArgs()),
            () => Assert.Equal(["SETBIT", "key", "5", "0"], Request.SetBit("key", 5, false).GetArgs()),
            () => Assert.Equal(["BITCOUNT", "key", "0", "-1"], Request.BitCount("key", 0, -1, BitmapIndexType.Byte).GetArgs()),
            () => Assert.Equal(["BITCOUNT", "key", "1", "5", "BIT"], Request.BitCount("key", 1, 5, BitmapIndexType.Bit).GetArgs()),
            () => Assert.Equal(["BITPOS", "key", "1", "0", "-1"], Request.BitPos("key", true, 0, -1, BitmapIndexType.Byte).GetArgs()),
            () => Assert.Equal(["BITPOS", "key", "0", "2", "10", "BIT"], Request.BitPos("key", false, 2, 10, BitmapIndexType.Bit).GetArgs()),
            () => Assert.Equal(["BITOP", "AND", "dest", "key1", "key2"], Request.BitOp(Bitwise.And, "dest", ["key1", "key2"]).GetArgs()),
            () => Assert.Equal(["BITOP", "OR", "dest", "key1", "key2", "key3"], Request.BitOp(Bitwise.Or, "dest", ["key1", "key2", "key3"]).GetArgs()),
            () => Assert.Equal(["BITOP", "XOR", "dest", "key1", "key2"], Request.BitOp(Bitwise.Xor, "dest", ["key1", "key2"]).GetArgs()),
            () => Assert.Equal(["BITOP", "NOT", "dest", "key1"], Request.BitOp(Bitwise.Not, "dest", ["key1"]).GetArgs()),
            () => Assert.Equal(["BITFIELD", "key", "GET", "u8", "0"], Request.BitField("key", [new BitFieldOptions.BitFieldGet(BitFieldOptions.Encoding.Unsigned(8), new BitFieldOptions.BitOffset(0))]).GetArgs()),
            () => Assert.Equal(["BITFIELD", "key", "SET", "i16", "#1", "100"], Request.BitField("key", [new BitFieldOptions.BitFieldSet(BitFieldOptions.Encoding.Signed(16), new BitFieldOptions.BitOffsetMultiplier(1), 100)]).GetArgs()),
            () => Assert.Equal(["BITFIELD", "key", "INCRBY", "u32", "8", "5"], Request.BitField("key", [new BitFieldOptions.BitFieldIncrBy(BitFieldOptions.Encoding.Unsigned(32), new BitFieldOptions.BitOffset(8), 5)]).GetArgs()),
            () => Assert.Equal(["BITFIELD", "key", "OVERFLOW", "WRAP", "SET", "u8", "0", "255"], Request.BitField("key", [new BitFieldOptions.BitFieldOverflow(BitFieldOptions.OverflowType.Wrap), new BitFieldOptions.BitFieldSet(BitFieldOptions.Encoding.Unsigned(8), new BitFieldOptions.BitOffset(0), 255)]).GetArgs()),
            () => Assert.Equal(["BITFIELDREADONLY", "key", "GET", "u8", "0", "GET", "i4", "8"], Request.BitFieldReadOnly("key", [new BitFieldOptions.BitFieldGet(BitFieldOptions.Encoding.Unsigned(8), new BitFieldOptions.BitOffset(0)), new BitFieldOptions.BitFieldGet(BitFieldOptions.Encoding.Signed(4), new BitFieldOptions.BitOffset(8))]).GetArgs()),

            // Hash Field Expire Commands (Valkey 9.0+)
            () => Assert.Equal(["HGETEX", "key", "PX", "60000", "FIELDS", "2", "field1", "field2"], Request.HashGet("key", ["field1", "field2"], GetExpiryOptions.ExpireIn(TimeSpan.FromSeconds(60))).GetArgs()),
            () => Assert.Equal(["HGETEX", "key", "PX", "5000", "FIELDS", "1", "field1"], Request.HashGet("key", ["field1"], GetExpiryOptions.ExpireIn(TimeSpan.FromMilliseconds(5000))).GetArgs()),
            () => Assert.Equal(["HGETEX", "key", "PXAT", "1609459200000", "FIELDS", "1", "field1"], Request.HashGet("key", ["field1"], GetExpiryOptions.ExpireAt(DateTimeOffset.FromUnixTimeSeconds(1609459200))).GetArgs()),
            () => Assert.Equal(["HGETEX", "key", "PXAT", "1609459200000", "FIELDS", "1", "field1"], Request.HashGet("key", ["field1"], GetExpiryOptions.ExpireAt(DateTimeOffset.FromUnixTimeMilliseconds(1609459200000))).GetArgs()),
            () => Assert.Equal(["HGETEX", "key", "PERSIST", "FIELDS", "1", "field1"], Request.HashGet("key", ["field1"], GetExpiryOptions.Persist()).GetArgs()),
            () => Assert.Equal(["HSETEX", "key", "PX", "60000", "FIELDS", "2", "field1", "value1", "field2", "value2"], Request.HashSet("key", [new KeyValuePair<ValkeyValue, ValkeyValue>("field1", "value1"), new KeyValuePair<ValkeyValue, ValkeyValue>("field2", "value2")], new HashSetOptions { Expiry = SetExpiryOptions.ExpireIn(TimeSpan.FromSeconds(60)) }).GetArgs()),
            () => Assert.Equal(["HSETEX", "key", "PX", "5000", "FIELDS", "1", "field1", "value1"], Request.HashSet("key", [new KeyValuePair<ValkeyValue, ValkeyValue>("field1", "value1")], new HashSetOptions { Expiry = SetExpiryOptions.ExpireIn(TimeSpan.FromMilliseconds(5000)) }).GetArgs()),
            () => Assert.Equal(["HSETEX", "key", "PXAT", "60000", "FIELDS", "2", "field1", "value1", "field2", "value2"], Request.HashSet("key", [new KeyValuePair<ValkeyValue, ValkeyValue>("field1", "value1"), new KeyValuePair<ValkeyValue, ValkeyValue>("field2", "value2")], new HashSetOptions { Expiry = SetExpiryOptions.ExpireAt(DateTimeOffset.FromUnixTimeMilliseconds(60000)) }).GetArgs()),
            () => Assert.Equal(["HSETEX", "key", "PXAT", "5000", "FIELDS", "1", "field1", "value1"], Request.HashSet("key", [new KeyValuePair<ValkeyValue, ValkeyValue>("field1", "value1")], new HashSetOptions { Expiry = SetExpiryOptions.ExpireAt(DateTimeOffset.FromUnixTimeMilliseconds(5000)) }).GetArgs()),
            () => Assert.Equal(["HSETEX", "key", "KEEPTTL", "FIELDS", "1", "field1", "value1"], Request.HashSet("key", [new KeyValuePair<ValkeyValue, ValkeyValue>("field1", "value1")], new HashSetOptions { Expiry = SetExpiryOptions.KeepTimeToLive() }).GetArgs()),
            () => Assert.Equal(["HSETEX", "key", "FNX", "PX", "60000", "FIELDS", "1", "field1", "value1"], Request.HashSet("key", [new KeyValuePair<ValkeyValue, ValkeyValue>("field1", "value1")], new HashSetOptions { Condition = HashSetCondition.OnlyIfNoneExist, Expiry = SetExpiryOptions.ExpireIn(TimeSpan.FromSeconds(60)) }).GetArgs()),
            () => Assert.Equal(["HSETEX", "key", "FXX", "PX", "60000", "FIELDS", "1", "field1", "value1"], Request.HashSet("key", [new KeyValuePair<ValkeyValue, ValkeyValue>("field1", "value1")], new HashSetOptions { Condition = HashSetCondition.OnlyIfAllExist, Expiry = SetExpiryOptions.ExpireIn(TimeSpan.FromSeconds(60)) }).GetArgs()),
            () => Assert.Equal(["HPERSIST", "key", "FIELDS", "2", "field1", "field2"], Request.HashPersist("key", ["field1", "field2"]).GetArgs()),
            () => Assert.Equal(["HPEXPIRE", "key", "60000", "FIELDS", "2", "field1", "field2"], Request.HashExpire("key", TimeSpan.FromSeconds(60), ["field1", "field2"], ExpireCondition.Always).GetArgs()),
            () => Assert.Equal(["HPEXPIRE", "key", "60000", "NX", "FIELDS", "2", "field1", "field2"], Request.HashExpire("key", TimeSpan.FromSeconds(60), ["field1", "field2"], ExpireCondition.OnlyIfNotExists).GetArgs()),
            () => Assert.Equal(["HPEXPIRE", "key", "60000", "XX", "FIELDS", "2", "field1", "field2"], Request.HashExpire("key", TimeSpan.FromSeconds(60), ["field1", "field2"], ExpireCondition.OnlyIfExists).GetArgs()),
            () => Assert.Equal(["HPEXPIRE", "key", "60000", "GT", "FIELDS", "2", "field1", "field2"], Request.HashExpire("key", TimeSpan.FromSeconds(60), ["field1", "field2"], ExpireCondition.OnlyIfGreaterThan).GetArgs()),
            () => Assert.Equal(["HPEXPIRE", "key", "60000", "LT", "FIELDS", "2", "field1", "field2"], Request.HashExpire("key", TimeSpan.FromSeconds(60), ["field1", "field2"], ExpireCondition.OnlyIfLessThan).GetArgs()),
            () => Assert.Equal(["HPEXPIRE", "key", "5500", "FIELDS", "2", "field1", "field2"], Request.HashExpire("key", TimeSpan.FromMilliseconds(5500), ["field1", "field2"], ExpireCondition.Always).GetArgs()),
            () => Assert.Equal(["HPEXPIREAT", "key", "1609459200000", "FIELDS", "2", "field1", "field2"], Request.HashExpireAt("key", DateTimeOffset.FromUnixTimeSeconds(1609459200), ["field1", "field2"], ExpireCondition.Always).GetArgs()),
            () => Assert.Equal(["HPEXPIREAT", "key", "1609459200000", "NX", "FIELDS", "2", "field1", "field2"], Request.HashExpireAt("key", DateTimeOffset.FromUnixTimeSeconds(1609459200), ["field1", "field2"], ExpireCondition.OnlyIfNotExists).GetArgs()),
            () => Assert.Equal(["HPEXPIREAT", "key", "1609459200500", "FIELDS", "2", "field1", "field2"], Request.HashExpireAt("key", DateTimeOffset.FromUnixTimeMilliseconds(1609459200500), ["field1", "field2"], ExpireCondition.Always).GetArgs()),
            () => Assert.Equal(["HPEXPIRETIME", "key", "FIELDS", "2", "field1", "field2"], Request.HashExpireTime("key", ["field1", "field2"]).GetArgs()),
            () => Assert.Equal(["HPTTL", "key", "FIELDS", "2", "field1", "field2"], Request.HashTimeToLive("key", ["field1", "field2"]).GetArgs())
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
            () => Assert.Equal(11L, Request.Increment("key").Converter(11L)),
            () => Assert.Equal(15L, Request.IncrementBy("key", 5).Converter(15L)),
            () => Assert.Equal(10.5, Request.IncrementByFloat("key", 0.5).Converter(10.5)),
            // TODO #454: Set should return ValkeyValue.Ok instead of bool.
            () => Assert.True(Request.Set([
                new KeyValuePair<ValkeyKey, ValkeyValue>("key1", "value1"),
                new KeyValuePair<ValkeyKey, ValkeyValue>("key2", "value2")
            ]).Converter("OK")),
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
            () => Assert.Equal([new("maxmemory", "100mb")], Request.ConfigGet("maxmemory").Converter(new object[] { (gs)"maxmemory", "100mb" })),
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

            () => Assert.Equal("common", Request.LongestCommonSubsequence("key1", "key2").Converter(new GlideString("common"))!.ToString()),
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

            () => Assert.True(Request.SetAdd("key", "member").Converter(1L)),
            () => Assert.False(Request.SetAdd("key", "member").Converter(0L)),
            () => Assert.True(Request.SetRemove("key", "member").Converter(1L)),
            () => Assert.False(Request.SetRemove("key", "member").Converter(0L)),

            () => Assert.Equal(2L, Request.SetAdd("key", ["member1", "member2"]).Converter(2L)),
            () => Assert.Equal(1L, Request.SetRemove("key", ["member1", "member2"]).Converter(1L)),
            () => Assert.Equal(5L, Request.SetCard("key").Converter(5L)),
            () => Assert.Equal(3L, Request.SetInterCard(["key1", "key2"]).Converter(3L)),
            () => Assert.Equal(4L, Request.SetUnionStore("dest", ["key1", "key2"]).Converter(4L)),
            () => Assert.Equal(2L, Request.SetInterStore("dest", ["key1", "key2"]).Converter(2L)),
            () => Assert.Equal(1L, Request.SetDiffStore("dest", ["key1", "key2"]).Converter(1L)),

            () => Assert.Equal<ValkeyValue>("member", Request.SetPop("key").Converter("member")),
            () => Assert.Null(Request.SetPop("key").Converter(null!)),

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
                var result = Request.Scan("0").Converter(["0", new object[] { (gs)"key1", (gs)"key2" }]);
                Assert.Equal("0", result.Item1);
                Assert.Equal([new ValkeyKey("key1"), new ValkeyKey("key2")], result.Item2);
            },
            () =>
            {
                var result = Request.Scan("10").Converter(["5", new object[] { (gs)"test" }]);
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
            () => Assert.Equal(["one", "two"], Request.ListLeftPop("a", 2).Converter([(gs)"one", (gs)"two"])!),
            () => Assert.Null(Request.ListLeftPop("a", 2).Converter(null!)),
            () => Assert.Equal(ValkeyValue.Null, Request.ListLeftPop("a").Converter(null!)),
            () => Assert.Equal(1L, Request.ListLeftPush("a", "value").Converter(1L)),
            () => Assert.Equal(2L, Request.ListLeftPush("a", ["one", "two"]).Converter(2L)),
            () => Assert.Equal("three", Request.ListRightPop("a").Converter("three")),
            () => Assert.Equal(ValkeyValue.Null, Request.ListRightPop("a").Converter(null!)),
            () => Assert.Equal(["three", "four"], Request.ListRightPop("a", 2).Converter([(gs)"three", (gs)"four"])!),
            () => Assert.Null(Request.ListRightPop("a", 2).Converter(null!)),
            () => Assert.Equal(2L, Request.ListRightPush("a", "value").Converter(2L)),
            () => Assert.Equal(3L, Request.ListRightPush("a", ["three", "four"]).Converter(3L)),
            () => Assert.Equal(5L, Request.ListLength("a").Converter(5L)),
            () => Assert.Equal(0L, Request.ListLength("nonexistent").Converter(0L)),
            () => Assert.Equal(2L, Request.ListRemove("a", "value", 0).Converter(2L)),
            () => Assert.Equal(1L, Request.ListRemove("a", "value", 1).Converter(1L)),
            () => Assert.Equal(0L, Request.ListRemove("a", "nonexistent", 0).Converter(0L)),
            () => Assert.Equal(ValkeyValue.Ok, Request.ListTrim("a", 0, 10).Converter("OK")),
            () => Assert.Equal(["one", "two", "three"], Request.ListRange("a", 0, -1).Converter([(gs)"one", (gs)"two", (gs)"three"])),
            () => Assert.IsType<ValkeyValue[]>(Request.ListRange("a", 0, -1).Converter([(gs)"one", (gs)"two", (gs)"three"])),
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
            () => Assert.Equal<ISet<ValkeyValue>>(new HashSet<ValkeyValue> { "field1", "field2" }, Request.HashKeys("key").Converter([(gs)"field1", (gs)"field2"])),
            () => Assert.Empty(Request.HashKeys("nonexistent").Converter([])),
            () => Assert.Equal(5L, Request.HashLength("key").Converter(5L)),
            () => Assert.Equal(10L, Request.HashStringLength("key", "field").Converter(10L)),

            // Hash Field Expire Commands converters (Valkey 9.0+)
            () => Assert.Equal((ValkeyValue[])["value1", "value2"], Request.HashGet("key", ["field1", "field2"], GetExpiryOptions.ExpireIn(TimeSpan.FromSeconds(60))).Converter([(gs)"value1", (gs)"value2"])),
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

            // List Commands converters
            () => Assert.Equal(["key", "value"], Request.ListBlockingLeftPop(["key"], TimeSpan.FromSeconds(1)).Converter([(gs)"key", (gs)"value"])!),
            () => Assert.Null(Request.ListBlockingLeftPop(["key"], TimeSpan.FromSeconds(1)).Converter(null!)),
            () => Assert.Equal(["list1", "element"], Request.ListBlockingRightPop(["list1", "list2"], TimeSpan.FromSeconds(5)).Converter([(gs)"list1", (gs)"element"])!),
            () => Assert.Null(Request.ListBlockingRightPop(["key"], TimeSpan.Zero).Converter(null!)),
            () => Assert.Equal("moved_value", Request.ListBlockingMove("src", "dest", ListSide.Left, ListSide.Right, TimeSpan.FromSeconds(2)).Converter("moved_value")),
            () => Assert.Equal(ValkeyValue.Null, Request.ListBlockingMove("src", "dest", ListSide.Left, ListSide.Right, TimeSpan.FromSeconds(2)).Converter(null!)),
            () => Assert.True(Request.ListBlockingPop(["key"], ListSide.Left, TimeSpan.FromSeconds(1)).Converter(null!).IsNull),
            () => Assert.True(Request.ListBlockingPop(["key"], ListSide.Left, 2, TimeSpan.FromSeconds(1)).Converter(null!).IsNull),
            () => Assert.False(Request.ListBlockingPop(["mylist"], ListSide.Left, TimeSpan.FromSeconds(1)).Converter(new() { { (GlideString)"mylist", new object[] { (GlideString)"value1" } } }).IsNull),
            () => Assert.False(Request.ListBlockingPop(["list2"], ListSide.Right, 3, TimeSpan.FromSeconds(2)).Converter(new() { { (GlideString)"list2", new object[] { (GlideString)"elem1", (GlideString)"elem2" } } }).IsNull),
            () => Assert.True(Request.ListBlockingPop(["key"], ListSide.Left, TimeSpan.FromSeconds(1)).Converter([]).IsNull),
            () => Assert.True(Request.ListLeftPop(["key1", "key2"], 2).Converter(null!).IsNull),
            () => Assert.True(Request.ListRightPop(["key1", "key2"], 3).Converter(null!).IsNull),
            () => Assert.False(Request.ListLeftPop(["mylist"], 1).Converter(new() { { (GlideString)"mylist", new object[] { (GlideString)"left_value" } } }).IsNull),
            () => Assert.False(Request.ListRightPop(["list2"], 2).Converter(new() { { (GlideString)"list2", new object[] { (GlideString)"right1", (GlideString)"right2" } } }).IsNull),
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
            () => Assert.Equal(["WATCH", "key1"], Request.Watch(["key1"]).GetArgs()),
            () => Assert.Equal(["WATCH", "key1", "key2", "key3"], Request.Watch(["key1", "key2", "key3"]).GetArgs()),
            () => Assert.Equal(["UNWATCH"], Request.Unwatch().GetArgs()),
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
        Assert.Equivalent(new HashSet<string> { "off" }, offInfo.Flags);
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
        Assert.Equivalent(new HashSet<string> { "on", "bcast" }, onInfo.Flags);
        Assert.Equal(0L, onInfo.Redirect);
        Assert.Equivalent(new HashSet<string> { "user:", "session:" }, onInfo.Prefixes);
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
        Assert.Multiple(
            () =>
            {
                // Test MGET with GlideString objects (what the server actually returns)
                var mgetResponse = new object[] { new GlideString("value1"), null!, new GlideString("value3") };
                var result = Request.Get(["key1", "key2", "key3"]).Converter(mgetResponse);
                Assert.Equal(3, result.Length);
                Assert.Equal(new ValkeyValue("value1"), result[0]);
                Assert.Equal(ValkeyValue.Null, result[1]);
                Assert.Equal(new ValkeyValue("value3"), result[2]);
            },

            () =>
            {
                // Test empty MGET response
                var emptyResult = Request.Get([]).Converter([]);
                Assert.Empty(emptyResult);
            },

            () =>
            {
                // Test MGET with all null values
                var allNullResponse = new object[] { null!, null! };
                var result = Request.Get(["key1", "key2"]).Converter(allNullResponse);
                Assert.Equal(2, result.Length);
                Assert.Equal(ValkeyValue.Null, result[0]);
                Assert.Equal(ValkeyValue.Null, result[1]);
            }
        );
    }

    [Fact]
    public void ValidateSetCommandHashSetConverters()
    {
        HashSet<object> testHashSet =
        [
            (gs)"member1",
            (gs)"member2",
            (gs)"member3"
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
            (gs)"value1",
            (gs)"value2",
            null!,
        ];

        // Test for HashGetAsync (all) and HashRandomFieldsWithValuesAsync
        Dictionary<GlideString, object> testKvpList = new() {
            {"field1", (gs)"value1" },
            {"field2", (gs)"value2" },
            {"field3", (gs)"value3" },
        };

        object[] testObjectNestedArray =
         [
            new object[] { (gs)"field1", (gs)"value1" },
            new object[] { (gs)"field2", (gs)"value2" },
            new object[] { (gs)"field3", (gs)"value3" },
         ];

        // Test for HashValuesAsync and HashRandomFieldsAsync
        object[] testObjectArray =
        [
            (gs)"value1",
            (gs)"value2",
            (gs)"value3"
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
        Assert.Equal(["XACK", "key", "group", "1-0"], Request.StreamAcknowledge("key", "group", (ValkeyValue)"1-0").GetArgs());
        Assert.Equal(["XACK", "key", "group", "1-0", "2-0"], Request.StreamAcknowledge("key", "group", ["1-0", "2-0"]).GetArgs());
        Assert.Equal(["XADD", "key", "*", "field", "value"], Request.StreamAdd("key", [new NameValueEntry("field", "value")], new StreamAddOptions()).GetArgs());
        Assert.Equal(["XADD", "key", "1-0", "field1", "value1", "field2", "value2"], Request.StreamAdd("key", [new NameValueEntry("field1", "value1"), new NameValueEntry("field2", "value2")], new StreamAddOptions { Id = "1-0" }).GetArgs());
        Assert.Equal(["XADD", "key", "MAXLEN", "~", "1000", "*", "field", "value"], Request.StreamAdd("key", [new NameValueEntry("field", "value")], new StreamAddOptions { Trim = new StreamTrimOptions.MaxLen { MaxLength = 1000, Exact = false } }).GetArgs());
        Assert.Equal(["XADD", "key", "MINID", "~", "0-1", "*", "field", "value"], Request.StreamAdd("key", [new NameValueEntry("field", "value")], new StreamAddOptions { Trim = new StreamTrimOptions.MinId { MinEntryId = "0-1", Exact = false } }).GetArgs());
        Assert.Equal(["XADD", "key", "NOMKSTREAM", "*", "field", "value"], Request.StreamAdd("key", [new NameValueEntry("field", "value")], new StreamAddOptions { MakeStream = false }).GetArgs());
        Assert.Equal(["XAUTOCLAIM", "key", "group", "consumer", "1000", "0-0"], Request.StreamAutoClaim("key", "group", "consumer", StreamAutoClaimOptions.FromId(OneSecond, "0-0")).GetArgs());
        Assert.Equal(["XAUTOCLAIM", "key", "group", "consumer", "1000", "0-0", "COUNT", "10"], Request.StreamAutoClaim("key", "group", "consumer", StreamAutoClaimOptions.FromId(OneSecond, "0-0").WithCount(10)).GetArgs());
        Assert.Equal(["XAUTOCLAIM", "key", "group", "consumer", "1000", "0-0", "JUSTID"], Request.StreamAutoClaimJustId("key", "group", "consumer", StreamAutoClaimOptions.FromId(OneSecond, "0-0")).GetArgs());
        Assert.Equal(["XCLAIM", "key", "group", "consumer", "1000", "1-0"], Request.StreamClaim("key", "group", "consumer", ["1-0"], StreamClaimOptions.From(OneSecond)).GetArgs());
        Assert.Equal(["XCLAIM", "key", "group", "consumer", "1000", "1-0", "IDLE", "500"], Request.StreamClaim("key", "group", "consumer", ["1-0"], StreamClaimOptions.From(OneSecond).WithIdle(TimeSpan.FromMilliseconds(500))).GetArgs());
        Assert.Equal(["XCLAIM", "key", "group", "consumer", "1000", "1-0", "TIME", "1500"], Request.StreamClaim("key", "group", "consumer", ["1-0"], StreamClaimOptions.From(OneSecond).WithIdleUnix(DateTimeOffset.FromUnixTimeMilliseconds(1500))).GetArgs());
        Assert.Equal(["XCLAIM", "key", "group", "consumer", "1000", "1-0", "FORCE"], Request.StreamClaim("key", "group", "consumer", ["1-0"], StreamClaimOptions.From(OneSecond).WithForce()).GetArgs());
        Assert.Equal(["XCLAIM", "key", "group", "consumer", "1000", "1-0", "JUSTID"], Request.StreamClaimJustIds("key", "group", "consumer", ["1-0"], StreamClaimOptions.From(OneSecond)).GetArgs());
        Assert.Equal(["XGROUPSETID", "key", "group", "0-0"], Request.StreamGroupSetId("key", "group", "0-0", null).GetArgs());
        Assert.Equal(["XGROUPSETID", "key", "group", "0-0", "ENTRIESREAD", "5"], Request.StreamGroupSetId("key", "group", "0-0", 5).GetArgs());
        Assert.Equal(["XINFOCONSUMERS", "key", "group"], Request.StreamInfoConsumers("key", "group").GetArgs());
        Assert.Equal(["XGROUPCREATECONSUMER", "key", "group", "consumer"], Request.StreamGroupCreateConsumer("key", "group", "consumer").GetArgs());
        Assert.Equal(["XGROUPCREATE", "key", "group", "0"], Request.StreamGroupCreate("key", "group", "0").GetArgs());
        Assert.Equal(["XGROUPCREATE", "key", "group", "0"], Request.StreamGroupCreate("key", "group", "0", new StreamGroupCreateOptions()).GetArgs());
        Assert.Equal(["XGROUPCREATE", "key", "group", "$", "MKSTREAM"], Request.StreamGroupCreate("key", "group", StreamPosition.NewMessages, new StreamGroupCreateOptions { MakeStream = true }).GetArgs());
        Assert.Equal(["XGROUPCREATE", "key", "group", "0"], Request.StreamGroupCreate("key", "group", "0", new StreamGroupCreateOptions { MakeStream = false }).GetArgs());
        Assert.Equal(["XGROUPCREATE", "key", "group", "0", "ENTRIESREAD", "10"], Request.StreamGroupCreate("key", "group", "0", new StreamGroupCreateOptions { MakeStream = false, EntriesRead = 10 }).GetArgs());
        Assert.Equal(["XDEL", "key", "1-0"], Request.StreamDelete("key", (ValkeyValue)"1-0").GetArgs());
        Assert.Equal(["XDEL", "key", "1-0", "2-0"], Request.StreamDelete("key", ["1-0", "2-0"]).GetArgs());
        Assert.Equal(["XGROUPDELCONSUMER", "key", "group", "consumer"], Request.StreamGroupDeleteConsumer("key", "group", "consumer").GetArgs());
        Assert.Equal(["XGROUPDESTROY", "key", "group"], Request.StreamGroupDestroy("key", "group").GetArgs());
        Assert.Equal(["XINFOGROUPS", "key"], Request.StreamInfoGroups("key").GetArgs());
        Assert.Equal(["XINFOSTREAM", "key"], Request.StreamInfo("key").GetArgs());
        Assert.Equal(["XINFOSTREAM", "key", "FULL"], Request.StreamInfoFull("key", null).GetArgs());
        Assert.Equal(["XINFOSTREAM", "key", "FULL", "COUNT", "5"], Request.StreamInfoFull("key", 5).GetArgs());
        Assert.Equal(["XLEN", "key"], Request.StreamLength("key").GetArgs());
        Assert.Equal(["XPENDING", "key", "group"], Request.StreamPending("key", "group").GetArgs());
        Assert.Equal(["XPENDING", "key", "group", "-", "+", "10", "consumer"], Request.StreamPending("key", "group", new StreamPendingOptions { Count = 10, ConsumerName = "consumer" }).GetArgs());
        Assert.Equal(["XPENDING", "key", "group", "IDLE", "1000", "-", "+", "10", "consumer"], Request.StreamPending("key", "group", new StreamPendingOptions { Count = 10, ConsumerName = "consumer", MinIdleTime = OneSecond }).GetArgs());
        Assert.Equal(["XRANGE", "key", "-", "+"], Request.StreamRange("key", new StreamRangeOptions()).GetArgs());
        Assert.Equal(["XRANGE", "key", "1-0", "2-0", "COUNT", "10"], Request.StreamRange("key", new StreamRangeOptions { Range = StreamIdRange.Between("1-0", "2-0"), Count = 10 }).GetArgs());
        Assert.Equal(["XREVRANGE", "key", "+", "-"], Request.StreamRange("key", new StreamRangeOptions { Order = Order.Descending }).GetArgs());
        Assert.Equal(["XREAD", "STREAMS", "key", "0-0"], Request.StreamRead(new StreamPosition("key", "0-0"), new StreamReadOptions()).GetArgs());
        Assert.Equal(["XREAD", "COUNT", "10", "STREAMS", "key", "0-0"], Request.StreamRead(new StreamPosition("key", "0-0"), new StreamReadOptions { Count = 10 }).GetArgs());
        Assert.Equal(["XREAD", "STREAMS", "key1", "key2", "0-0", "1-0"], Request.StreamRead([new StreamPosition("key1", "0-0"), new StreamPosition("key2", "1-0")], new StreamReadOptions()).GetArgs());
        Assert.Equal(["XREADGROUP", "GROUP", "group", "consumer", "STREAMS", "key", ">"], Request.StreamReadGroup(new StreamPosition("key", ">"), "group", "consumer", new StreamReadGroupOptions()).GetArgs());
        Assert.Equal(["XREADGROUP", "GROUP", "group", "consumer", "COUNT", "10", "STREAMS", "key", ">"], Request.StreamReadGroup(new StreamPosition("key", ">"), "group", "consumer", new StreamReadGroupOptions { Count = 10 }).GetArgs());
        Assert.Equal(["XREADGROUP", "GROUP", "group", "consumer", "NOACK", "STREAMS", "key", ">"], Request.StreamReadGroup(new StreamPosition("key", ">"), "group", "consumer", new StreamReadGroupOptions { NoAck = true }).GetArgs());
        Assert.Equal(["XREADGROUP", "GROUP", "group", "consumer", "STREAMS", "key1", "key2", ">", ">"], Request.StreamReadGroup([new StreamPosition("key1", ">"), new StreamPosition("key2", ">")], "group", "consumer", new StreamReadGroupOptions()).GetArgs());
        Assert.Equal(["XREADGROUP", "GROUP", "group", "consumer", "COUNT", "5", "STREAMS", "key1", "key2", ">", "0-0"], Request.StreamReadGroup([new StreamPosition("key1", ">"), new StreamPosition("key2", "0-0")], "group", "consumer", new StreamReadGroupOptions { Count = 5 }).GetArgs());
        Assert.Equal(["XREADGROUP", "GROUP", "group", "consumer", "NOACK", "STREAMS", "key1", "key2", ">", ">"], Request.StreamReadGroup([new StreamPosition("key1", ">"), new StreamPosition("key2", ">")], "group", "consumer", new StreamReadGroupOptions { NoAck = true }).GetArgs());
        Assert.Equal(["XTRIM", "key", "MAXLEN", "1000"], Request.StreamTrim("key", new StreamTrimOptions.MaxLen { MaxLength = 1000 }).GetArgs());
        Assert.Equal(["XTRIM", "key", "MAXLEN", "~", "1000"], Request.StreamTrim("key", new StreamTrimOptions.MaxLen { MaxLength = 1000, Exact = false }).GetArgs());
        Assert.Equal(["XTRIM", "key", "MINID", "0-1"], Request.StreamTrim("key", new StreamTrimOptions.MinId { MinEntryId = "0-1" }).GetArgs());
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
            ["groups"] = 1L,
            ["last-generated-id"] = (GlideString)"5-0",
            ["max-deleted-entry-id"] = (GlideString)"2-0",
            ["entries-added"] = 4L,
            ["recorded-first-entry-id"] = (GlideString)"3-0",
            ["first-entry"] = new object[] { (GlideString)"3-0", new object[] { (GlideString)"f1", (GlideString)"v1" } },
            ["last-entry"] = new object[] { (GlideString)"5-0", new object[] { (GlideString)"f2", (GlideString)"v2" } },
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
            ["last-generated-id"] = (GlideString)"1-0",
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
                ["name"] = (GlideString)"consumer1",
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
                ["name"] = (GlideString)"consumer1",
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
                ["1-0"] = new object[] { new object[] { (GlideString)"f1", (GlideString)"v1" } },
                ["2-0"] = new object[]
                {
                    new object[] { (GlideString)"f1", (GlideString)"v1" },
                    new object[] { (GlideString)"f2", (GlideString)"v2" },
                    new object[] { (GlideString)"f3", (GlideString)"v3" },
                },
            },
        };

        var entries = Request.StreamRead(new StreamPosition("mystream", "0")).Converter(raw);

        // Verify that nil entry is skipped.
        Assert.Equal(2, entries.Length);

        // Verify entry with single field.
        Assert.Equal("1-0", entries[0].Id);
        Assert.Equivalent(new NameValueEntry[] { new("f1", "v1") }, entries[0].Values);

        // Verify entry with multiple fields.
        Assert.Equal("2-0", entries[1].Id);
        Assert.Equivalent(new NameValueEntry[] { new("f1", "v1"), new("f2", "v2"), new("f3", "v3") }, entries[1].Values);
    }

    [Fact]
    public void StreamRead_MultiConverter()
    {
        var raw = new Dictionary<GlideString, object>
        {
            ["stream0"] = new Dictionary<GlideString, object>(),
            ["stream1"] = new Dictionary<GlideString, object>
            {
                ["1-0"] = new object[] { new object[] { (GlideString)"v1", (GlideString)"f1" } },
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
        Assert.Equivalent(new NameValueEntry[] { new("v1", "f1") }, entry.Values);
    }

    #endregion
    #region MemoryStats Converter Tests

    // Reponse values for testing converters.
    private static readonly long ConvertLong = 100L;
    private static readonly double ConvertDouble = 1.5;

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
