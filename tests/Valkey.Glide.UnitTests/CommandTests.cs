// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands.Options;

namespace Valkey.Glide.UnitTests;

public class CommandTests
{
    [Fact]
    public void ValidateCommandArgs() => Assert.Multiple(
            () => Assert.Equal(["get", "a"], Request.CustomCommand(["get", "a"]).GetArgs()),
            () => Assert.Equal(["ping", "pong", "pang"], Request.CustomCommand(["ping", "pong", "pang"]).GetArgs()),
            () => Assert.Equal(["get"], Request.CustomCommand(["get"]).GetArgs()),
            () => Assert.Equal([], Request.CustomCommand([]).GetArgs()),

            // String Commands
            () => Assert.Equal(["SET", "key", "value"], Request.StringSet("key", "value").GetArgs()),
            () => Assert.Equal(["SET", "key", "value", "NX"], Request.StringSetNX("key", "value").GetArgs()),
            () => Assert.Equal(["SET", "key", "value", "XX"], Request.StringSetXX("key", "value").GetArgs()),
            () => Assert.Equal(["GET", "key"], Request.StringGet("key").GetArgs()),
            () => Assert.Equal(["MGET", "key1", "key2", "key3"], Request.StringGetMultiple(["key1", "key2", "key3"]).GetArgs()),
            () => Assert.Equal(["MSET", "key1", "value1", "key2", "value2"], Request.StringSetMultiple([
                new KeyValuePair<ValkeyKey, ValkeyValue>("key1", "value1"),
                new KeyValuePair<ValkeyKey, ValkeyValue>("key2", "value2")
            ]).GetArgs()),
            () => Assert.Equal(["STRLEN", "key"], Request.StringLength("key").GetArgs()),
            () => Assert.Equal(["GETRANGE", "key", "0", "5"], Request.StringGetRange("key", 0, 5).GetArgs()),
            () => Assert.Equal(["SETRANGE", "key", "10", "value"], Request.StringSetRange("key", 10, "value").GetArgs()),
            () => Assert.Equal(["APPEND", "key", "value"], Request.StringAppend("key", "value").GetArgs()),
            () => Assert.Equal(11L, Request.StringAppend("key", "value").Converter(11L)),
            () => Assert.Equal(["DECR", "key"], Request.StringDecr("key").GetArgs()),
            () => Assert.Equal(["DECRBY", "key", "5"], Request.StringDecrBy("key", 5).GetArgs()),
            () => Assert.Equal(["INCR", "key"], Request.StringIncr("key").GetArgs()),
            () => Assert.Equal(["INCRBY", "key", "5"], Request.StringIncrBy("key", 5).GetArgs()),
            () => Assert.Equal(["INCRBYFLOAT", "key", "0.5"], Request.StringIncrByFloat("key", 0.5).GetArgs()),
            () => Assert.Equal(["MSETNX", "key1", "value1", "key2", "value2"], Request.StringSetMultipleNX([
                new KeyValuePair<ValkeyKey, ValkeyValue>("key1", "value1"),
                new KeyValuePair<ValkeyKey, ValkeyValue>("key2", "value2")
            ]).GetArgs()),
            () => Assert.Equal(["MSETNX"], Request.StringSetMultipleNX([]).GetArgs()),
            () => Assert.Equal(["GETDEL", "key"], Request.StringGetDelete("key").GetArgs()),
            () => Assert.Equal(["GETDEL", "test_key"], Request.StringGetDelete("test_key").GetArgs()),
            () => Assert.Equal(["GETEX", "key", "PX", "60000"], Request.StringGetSetExpiry("key", TimeSpan.FromSeconds(60)).GetArgs()),
            () => Assert.Equal(["GETEX", "test_key", "PX", "60000"], Request.StringGetSetExpiry("test_key", TimeSpan.FromSeconds(60)).GetArgs()),
            () => Assert.Equal(["GETEX", "key", "PERSIST"], Request.StringGetSetExpiry("key", null).GetArgs()),
            () => Assert.Equal(["GETEX", "test_key", "PERSIST"], Request.StringGetSetExpiry("test_key", null).GetArgs()),
            () => Assert.Equal(["GETEX", "key", "EXAT", "1609459200"], Request.StringGetSetExpiry("key", new DateTime(2021, 1, 1, 0, 0, 0, DateTimeKind.Utc)).GetArgs()),
            () => Assert.Equal(["GETEX", "test_key", "EXAT", "1609459200"], Request.StringGetSetExpiry("test_key", new DateTime(2021, 1, 1, 0, 0, 0, DateTimeKind.Utc)).GetArgs()),
            () => Assert.Equal(["LCS", "key1", "key2"], Request.StringLongestCommonSubsequence("key1", "key2").GetArgs()),
            () => Assert.Equal(["LCS", "key1", "key2", "LEN"], Request.StringLongestCommonSubsequenceLength("key1", "key2").GetArgs()),
            () => Assert.Equal(["LCS", "key1", "key2", "IDX", "MINMATCHLEN", "0", "WITHMATCHLEN"], Request.StringLongestCommonSubsequenceWithMatches("key1", "key2").GetArgs()),
            () => Assert.Equal(["LCS", "key1", "key2", "IDX", "MINMATCHLEN", "5", "WITHMATCHLEN"], Request.StringLongestCommonSubsequenceWithMatches("key1", "key2", 5).GetArgs()),
            () => Assert.Equal(["LCS", "key1", "key2", "IDX", "MINMATCHLEN", "0", "WITHMATCHLEN"], Request.StringLongestCommonSubsequenceWithMatches("key1", "key2", 0).GetArgs()),

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

            // Connection Management Commands - Ping
            () => Assert.Equal(["PING"], Request.Ping().GetArgs()),
            () => Assert.Equal(["PING", "Hello"], Request.Ping("Hello").GetArgs()),
            () => Assert.Equal(["PING", "test message"], Request.Ping("test message").GetArgs()),
            () => Assert.Equal(["PING", ""], Request.Ping("").GetArgs()),
            () => Assert.Equal(["PING", "PONG"], Request.Ping("PONG").GetArgs()),
            () => Assert.Equal(["ECHO", "message"], Request.Echo("message").GetArgs()),
            () => Assert.Equal(["SELECT", "0"], Request.Select(0).GetArgs()),
            () => Assert.Equal(["SELECT", "1"], Request.Select(1).GetArgs()),
            () => Assert.Equal(["SELECT", "15"], Request.Select(15).GetArgs()),
            () => Assert.Equal(["SELECT", "-1"], Request.Select(-1).GetArgs()),

            // Server Management Commands
            () => Assert.Equal(["CLIENTGETNAME"], Request.ClientGetName().GetArgs()),
            () => Assert.Equal(["CLIENTID"], Request.ClientId().GetArgs()),

            // Set Commands
            () => Assert.Equal(["SADD", "key", "member"], Request.SetAddAsync("key", "member").GetArgs()),
            () => Assert.Equal(["SADD", "key", "member1", "member2"], Request.SetAddAsync("key", ["member1", "member2"]).GetArgs()),
            () => Assert.Equal(["SREM", "key", "member"], Request.SetRemoveAsync("key", "member").GetArgs()),
            () => Assert.Equal(["SREM", "key", "member1", "member2"], Request.SetRemoveAsync("key", ["member1", "member2"]).GetArgs()),
            () => Assert.Equal(["SMEMBERS", "key"], Request.SetMembersAsync("key").GetArgs()),
            () => Assert.Equal(["SCARD", "key"], Request.SetLengthAsync("key").GetArgs()),
            () => Assert.Equal(["SINTERCARD", "2", "key1", "key2"], Request.SetIntersectionLengthAsync(["key1", "key2"]).GetArgs()),
            () => Assert.Equal(["SINTERCARD", "2", "key1", "key2", "LIMIT", "10"], Request.SetIntersectionLengthAsync(["key1", "key2"], 10).GetArgs()),
            () => Assert.Equal(["SPOP", "key"], Request.SetPopAsync("key").GetArgs()),
            () => Assert.Equal(["SPOP", "key", "3"], Request.SetPopAsync("key", 3).GetArgs()),
            () => Assert.Equal(["SUNION", "key1", "key2"], Request.SetUnionAsync(["key1", "key2"]).GetArgs()),
            () => Assert.Equal(["SINTER", "key1", "key2"], Request.SetIntersectAsync(["key1", "key2"]).GetArgs()),
            () => Assert.Equal(["SDIFF", "key1", "key2"], Request.SetDifferenceAsync(["key1", "key2"]).GetArgs()),
            () => Assert.Equal(["SUNIONSTORE", "dest", "key1", "key2"], Request.SetUnionStoreAsync("dest", ["key1", "key2"]).GetArgs()),
            () => Assert.Equal(["SINTERSTORE", "dest", "key1", "key2"], Request.SetIntersectStoreAsync("dest", ["key1", "key2"]).GetArgs()),
            () => Assert.Equal(["SDIFFSTORE", "dest", "key1", "key2"], Request.SetDifferenceStoreAsync("dest", ["key1", "key2"]).GetArgs()),
            () => Assert.Equal(["SISMEMBER", "key", "member"], Request.SetContainsAsync("key", "member").GetArgs()),
            () => Assert.Equal(["SISMEMBER", "mykey", "value"], Request.SetContainsAsync("mykey", "value").GetArgs()),
            () => Assert.Equal(["SISMEMBER", "test:set", "test-member"], Request.SetContainsAsync("test:set", "test-member").GetArgs()),
            () => Assert.Equal(["SMISMEMBER", "key", "member1", "member2", "member3"], Request.SetContainsAsync("key", ["member1", "member2", "member3"]).GetArgs()),
            () => Assert.Equal(["SMISMEMBER", "key"], Request.SetContainsAsync("key", []).GetArgs()),
            () => Assert.Equal(["SMISMEMBER", "key", "", " ", "null", "0", "-1"], Request.SetContainsAsync("key", ["", " ", "null", "0", "-1"]).GetArgs()),
            () => Assert.Equal(["SRANDMEMBER", "key"], Request.SetRandomMemberAsync("key").GetArgs()),
            () => Assert.Equal(["SRANDMEMBER", "mykey"], Request.SetRandomMemberAsync("mykey").GetArgs()),
            () => Assert.Equal(["SRANDMEMBER", "test:set"], Request.SetRandomMemberAsync("test:set").GetArgs()),
            () => Assert.Equal(["SRANDMEMBER", "key", "3"], Request.SetRandomMembersAsync("key", 3).GetArgs()),
            () => Assert.Equal(["SRANDMEMBER", "key", "-5"], Request.SetRandomMembersAsync("key", -5).GetArgs()),
            () => Assert.Equal(["SRANDMEMBER", "key", "0"], Request.SetRandomMembersAsync("key", 0).GetArgs()),
            () => Assert.Equal(["SRANDMEMBER", "key", "1"], Request.SetRandomMembersAsync("key", 1).GetArgs()),
            () => Assert.Equal(["SMOVE", "source", "dest", "member"], Request.SetMoveAsync("source", "dest", "member").GetArgs()),
            () => Assert.Equal(["SMOVE", "key1", "key2", "value"], Request.SetMoveAsync("key1", "key2", "value").GetArgs()),
            () => Assert.Equal(["SMOVE", "src:set", "dst:set", "test-member"], Request.SetMoveAsync("src:set", "dst:set", "test-member").GetArgs()),
            () => Assert.Equal(["SSCAN", "key", "0"], Request.SetScanAsync("key", 0).GetArgs()),
            () => Assert.Equal(["SSCAN", "key", "10"], Request.SetScanAsync("key", 10).GetArgs()),
            () => Assert.Equal(["SSCAN", "mykey", "0"], Request.SetScanAsync("mykey", 0).GetArgs()),
            () => Assert.Equal(["SSCAN", "key", "0", "MATCH", "pattern*"], Request.SetScanAsync("key", 0, new ScanOptions { MatchPattern = "pattern*" }).GetArgs()),
            () => Assert.Equal(["SSCAN", "key", "5", "MATCH", "test*"], Request.SetScanAsync("key", 5, new ScanOptions { MatchPattern = "test*" }).GetArgs()),
            () => Assert.Equal(["SSCAN", "key", "0", "MATCH", "*suffix"], Request.SetScanAsync("key", 0, new ScanOptions { MatchPattern = "*suffix" }).GetArgs()),
            () => Assert.Equal(["SSCAN", "key", "0", "COUNT", "10"], Request.SetScanAsync("key", 0, new ScanOptions { Count = 10 }).GetArgs()),
            () => Assert.Equal(["SSCAN", "key", "5", "COUNT", "20"], Request.SetScanAsync("key", 5, new ScanOptions { Count = 20 }).GetArgs()),
            () => Assert.Equal(["SSCAN", "key", "0", "COUNT", "1"], Request.SetScanAsync("key", 0, new ScanOptions { Count = 1 }).GetArgs()),
            () => Assert.Equal(["SSCAN", "key", "0", "MATCH", "pattern*", "COUNT", "10"], Request.SetScanAsync("key", 0, new ScanOptions { MatchPattern = "pattern*", Count = 10 }).GetArgs()),
            () => Assert.Equal(["SSCAN", "key", "5", "MATCH", "test*", "COUNT", "20"], Request.SetScanAsync("key", 5, new ScanOptions { MatchPattern = "test*", Count = 20 }).GetArgs()),
            () => Assert.Equal(["SSCAN", "key", "10", "MATCH", "*suffix", "COUNT", "5"], Request.SetScanAsync("key", 10, new ScanOptions { MatchPattern = "*suffix", Count = 5 }).GetArgs()),
            () => Assert.Equal(["SISMEMBER", "", "member"], Request.SetContainsAsync("", "member").GetArgs()),
            () => Assert.Equal(["SISMEMBER", "key", ""], Request.SetContainsAsync("key", "").GetArgs()),
            () => Assert.Equal(["SMOVE", "", "", ""], Request.SetMoveAsync("", "", "").GetArgs()),
            () => Assert.Equal(["SSCAN", "", "0"], Request.SetScanAsync("", 0).GetArgs()),

            // Generic Commands
            () => Assert.Equal(["DEL", "key"], Request.DeleteAsync("key").GetArgs()),
            () => Assert.Equal(["DEL", "key1", "key2"], Request.DeleteAsync(["key1", "key2"]).GetArgs()),
            () => Assert.Equal(["UNLINK", "key"], Request.UnlinkAsync("key").GetArgs()),
            () => Assert.Equal(["UNLINK", "key1", "key2"], Request.UnlinkAsync(["key1", "key2"]).GetArgs()),
            () => Assert.Equal(["EXISTS", "key"], Request.ExistsAsync("key").GetArgs()),
            () => Assert.Equal(["EXISTS", "key1", "key2"], Request.ExistsAsync(["key1", "key2"]).GetArgs()),
            () => Assert.Equal(["PEXPIRE", "key", "60000"], Request.ExpireAsync("key", TimeSpan.FromSeconds(60)).GetArgs()),
            () => Assert.Equal(["PEXPIRE", "key", "60000", "NX"], Request.ExpireAsync("key", TimeSpan.FromSeconds(60), ExpireCondition.OnlyIfNotExists).GetArgs()),
            () => Assert.Equal(["PEXPIREAT", "key", "1609459200000"], Request.ExpireAsync("key", new DateTimeOffset(2021, 1, 1, 0, 0, 0, TimeSpan.Zero)).GetArgs()),
            () => Assert.Equal(["PTTL", "key"], Request.TimeToLiveAsync("key").GetArgs()),
            () => Assert.Equal(["TYPE", "key"], Request.TypeAsync("key").GetArgs()),
            () => Assert.Equal(["RENAME", "oldkey", "newkey"], Request.RenameAsync("oldkey", "newkey").GetArgs()),
            () => Assert.Equal(["RENAMENX", "oldkey", "newkey"], Request.RenameIfNotExistsAsync("oldkey", "newkey").GetArgs()),
            () => Assert.Equal(["PERSIST", "key"], Request.PersistAsync("key").GetArgs()),
            () => Assert.Equal(["DUMP", "key"], Request.DumpAsync("key").GetArgs()),
            () => Assert.Equal(["RESTORE", "key", "0", "data"], Request.RestoreAsync("key", "data"u8.ToArray()).GetArgs()),
            () => Assert.Equal(["RESTORE", "key", "5000", "data"], Request.RestoreAsync("key", "data"u8.ToArray(), new RestoreOptions { Ttl = TimeSpan.FromSeconds(5) }).GetArgs()),
            () => Assert.Equal(["RESTORE", "key", "2303596800000", "data", "ABSTTL"], Request.RestoreAsync("key", "data"u8.ToArray(), new RestoreOptions { ExpireAt = new DateTimeOffset(2042, 12, 31, 0, 0, 0, TimeSpan.Zero) }).GetArgs()),
            () => Assert.Equal(["RESTORE", "key", "0", "data", "REPLACE"], Request.RestoreAsync("key", "data"u8.ToArray(), new RestoreOptions { Replace = true }).GetArgs()),
            () => Assert.Equal(["RESTORE", "key", "0", "data", "IDLETIME", "1000"], Request.RestoreAsync("key", "data"u8.ToArray(), new RestoreOptions { IdleTime = 1000 }).GetArgs()),
            () => Assert.Equal(["RESTORE", "key", "0", "data", "FREQ", "5"], Request.RestoreAsync("key", "data"u8.ToArray(), new RestoreOptions { Frequency = 5 }).GetArgs()),
            () => Assert.Equal(["RESTORE", "key", "0", "data", "REPLACE", "IDLETIME", "2000"], Request.RestoreAsync("key", "data"u8.ToArray(), new RestoreOptions { Replace = true, IdleTime = 2000 }).GetArgs()),
            () => Assert.Equal(["RESTORE", "key", "0", "data", "REPLACE", "FREQ", "10"], Request.RestoreAsync("key", "data"u8.ToArray(), new RestoreOptions { Replace = true, Frequency = 10 }).GetArgs()),
            () => Assert.Equal(["RESTORE", "key", "5000", "data", "REPLACE"], Request.RestoreAsync("key", "data"u8.ToArray(), new RestoreOptions { Ttl = TimeSpan.FromSeconds(5), Replace = true }).GetArgs()),
            () => Assert.Equal(["RESTORE", "key", "2303596800000", "data", "ABSTTL", "REPLACE"], Request.RestoreAsync("key", "data"u8.ToArray(), new RestoreOptions { ExpireAt = new DateTimeOffset(2042, 12, 31, 0, 0, 0, TimeSpan.Zero), Replace = true }).GetArgs()),
            () => Assert.Equal(["RESTORE", "key", "5000", "data", "IDLETIME", "1000"], Request.RestoreAsync("key", "data"u8.ToArray(), new RestoreOptions { Ttl = TimeSpan.FromSeconds(5), IdleTime = 1000 }).GetArgs()),
            () => Assert.Equal(["RESTORE", "key", "2303596800000", "data", "ABSTTL", "FREQ", "10"], Request.RestoreAsync("key", "data"u8.ToArray(), new RestoreOptions { ExpireAt = new DateTimeOffset(2042, 12, 31, 0, 0, 0, TimeSpan.Zero), Frequency = 10 }).GetArgs()),
            () => Assert.Throws<ArgumentException>(() => Request.RestoreAsync("key", "data"u8.ToArray(), new RestoreOptions { IdleTime = 1000, Frequency = 5 }).GetArgs()),
            () => Assert.Throws<ArgumentException>(() => Request.RestoreAsync("key", "data"u8.ToArray(), new RestoreOptions { Ttl = TimeSpan.FromSeconds(5), ExpireAt = DateTimeOffset.UtcNow }).GetArgs()),
            () => Assert.Equal(["TOUCH", "key"], Request.TouchAsync("key").GetArgs()),
            () => Assert.Equal(["TOUCH", "key1", "key2"], Request.TouchAsync(["key1", "key2"]).GetArgs()),
            () => Assert.Equal(["COPY", "src", "dest"], Request.CopyAsync("src", "dest").GetArgs()),
            () => Assert.Equal(["COPY", "src", "dest", "DB", "1", "REPLACE"], Request.CopyAsync("src", "dest", 1, true).GetArgs()),
            () => Assert.Equal(["PEXPIRETIME", "key"], Request.ExpireTimeAsync("key").GetArgs()),
            () => Assert.Equal(["OBJECT", "ENCODING", "key"], Request.ObjectEncodingAsync("key").GetArgs()),
            () => Assert.Equal(["OBJECT", "FREQ", "key"], Request.ObjectFrequencyAsync("key").GetArgs()),
            () => Assert.Equal(["OBJECT", "IDLETIME", "key"], Request.ObjectIdleTimeAsync("key").GetArgs()),
            () => Assert.Equal(["OBJECT", "REFCOUNT", "key"], Request.ObjectRefCountAsync("key").GetArgs()),
            () => Assert.Equal(["RANDOMKEY"], Request.RandomKeyAsync().GetArgs()),
            () => Assert.Equal(["MOVE", "key", "1"], Request.MoveAsync("key", 1).GetArgs()),

            // SCAN Commands
            () => Assert.Equal(["SCAN", "0"], Request.ScanAsync("0").GetArgs()),
            () => Assert.Equal(["SCAN", "10"], Request.ScanAsync("10").GetArgs()),
            () => Assert.Equal(["SCAN", "0", "MATCH", "pattern*"], Request.ScanAsync("0", new ScanOptions { MatchPattern = "pattern*" }).GetArgs()),
            () => Assert.Equal(["SCAN", "5", "MATCH", "test*"], Request.ScanAsync("5", new ScanOptions { MatchPattern = "test*" }).GetArgs()),
            () => Assert.Equal(["SCAN", "0", "COUNT", "10"], Request.ScanAsync("0", new ScanOptions { Count = 10 }).GetArgs()),
            () => Assert.Equal(["SCAN", "5", "COUNT", "20"], Request.ScanAsync("5", new ScanOptions { Count = 20 }).GetArgs()),
            () => Assert.Equal(["SCAN", "0", "TYPE", "string"], Request.ScanAsync("0", new ScanOptions { Type = ValkeyType.String }).GetArgs()),
            () => Assert.Equal(["SCAN", "5", "TYPE", "list"], Request.ScanAsync("5", new ScanOptions { Type = ValkeyType.List }).GetArgs()),
            () => Assert.Equal(["SCAN", "0", "TYPE", "set"], Request.ScanAsync("0", new ScanOptions { Type = ValkeyType.Set }).GetArgs()),
            () => Assert.Equal(["SCAN", "0", "TYPE", "zset"], Request.ScanAsync("0", new ScanOptions { Type = ValkeyType.SortedSet }).GetArgs()),
            () => Assert.Equal(["SCAN", "0", "TYPE", "hash"], Request.ScanAsync("0", new ScanOptions { Type = ValkeyType.Hash }).GetArgs()),
            () => Assert.Equal(["SCAN", "0", "TYPE", "stream"], Request.ScanAsync("0", new ScanOptions { Type = ValkeyType.Stream }).GetArgs()),
            () => Assert.Equal(["SCAN", "10", "MATCH", "key*", "COUNT", "20", "TYPE", "string"], Request.ScanAsync("10", new ScanOptions { MatchPattern = "key*", Count = 20, Type = ValkeyType.String }).GetArgs()),

            // WAIT Commands
            () => Assert.Equal(["WAIT", "1", "1000"], Request.WaitAsync(1, TimeSpan.FromMilliseconds(1000)).GetArgs()),
            () => Assert.Equal(["WAIT", "0", "0"], Request.WaitAsync(0, TimeSpan.Zero).GetArgs()),
            () => Assert.Equal(["WAIT", "3", "5000"], Request.WaitAsync(3, TimeSpan.FromMilliseconds(5000)).GetArgs()),

            // List Commands
            () => Assert.Equal(["LPOP", "a"], Request.ListLeftPopAsync("a").GetArgs()),
            () => Assert.Equal(["LPOP", "a", "3"], Request.ListLeftPopAsync("a", 3).GetArgs()),
            () => Assert.Equal(["LPUSH", "a", "value"], Request.ListLeftPushAsync("a", "value").GetArgs()),
            () => Assert.Equal(["LPUSH", "a", "one", "two"], Request.ListLeftPushAsync("a", ["one", "two"]).GetArgs()),
            () => Assert.Equal(["RPOP", "a"], Request.ListRightPopAsync("a").GetArgs()),
            () => Assert.Equal(["RPOP", "a", "2"], Request.ListRightPopAsync("a", 2).GetArgs()),
            () => Assert.Equal(["RPUSH", "a", "value"], Request.ListRightPushAsync("a", "value").GetArgs()),
            () => Assert.Equal(["RPUSH", "a", "one", "two"], Request.ListRightPushAsync("a", ["one", "two"]).GetArgs()),
            () => Assert.Equal(["LLEN", "a"], Request.ListLengthAsync("a").GetArgs()),
            () => Assert.Equal(["LREM", "a", "0", "value"], Request.ListRemoveAsync("a", "value", 0).GetArgs()),
            () => Assert.Equal(["LREM", "a", "2", "value"], Request.ListRemoveAsync("a", "value", 2).GetArgs()),
            () => Assert.Equal(["LREM", "a", "-1", "value"], Request.ListRemoveAsync("a", "value", -1).GetArgs()),
            () => Assert.Equal(["LTRIM", "a", "0", "10"], Request.ListTrimAsync("a", 0, 10).GetArgs()),
            () => Assert.Equal(["LTRIM", "a", "1", "-1"], Request.ListTrimAsync("a", 1, -1).GetArgs()),
            () => Assert.Equal(["LRANGE", "a", "0", "-1"], Request.ListRangeAsync("a", 0, -1).GetArgs()),
            () => Assert.Equal(["LRANGE", "a", "1", "5"], Request.ListRangeAsync("a", 1, 5).GetArgs()),
            () => Assert.Equal(["BLPOP", "key1", "key2", "5"], Request.ListBlockingLeftPopAsync(["key1", "key2"], TimeSpan.FromSeconds(5)).GetArgs()),
            () => Assert.Equal(["BLPOP", "key", "0"], Request.ListBlockingLeftPopAsync(["key"], TimeSpan.Zero).GetArgs()),
            () => Assert.Equal(["BLPOP", "a", "b", "c", "10"], Request.ListBlockingLeftPopAsync(["a", "b", "c"], TimeSpan.FromSeconds(10)).GetArgs()),
            () => Assert.Equal(["BLPOP", "single", "0.5"], Request.ListBlockingLeftPopAsync(["single"], TimeSpan.FromMilliseconds(500)).GetArgs()),
            () => Assert.Equal(["BRPOP", "key1", "key2", "10"], Request.ListBlockingRightPopAsync(["key1", "key2"], TimeSpan.FromSeconds(10)).GetArgs()),
            () => Assert.Equal(["BRPOP", "key", "0"], Request.ListBlockingRightPopAsync(["key"], TimeSpan.Zero).GetArgs()),
            () => Assert.Equal(["BRPOP", "x", "y", "z", "0.5"], Request.ListBlockingRightPopAsync(["x", "y", "z"], TimeSpan.FromMilliseconds(500)).GetArgs()),
            () => Assert.Equal(["BRPOP", "test", "1.5"], Request.ListBlockingRightPopAsync(["test"], TimeSpan.FromSeconds(1.5)).GetArgs()),
            () => Assert.Equal(["BLMOVE", "source", "dest", "LEFT", "RIGHT", "3"], Request.ListBlockingMoveAsync("source", "dest", ListSide.Left, ListSide.Right, TimeSpan.FromSeconds(3)).GetArgs()),
            () => Assert.Equal(["BLMOVE", "src", "dst", "RIGHT", "LEFT", "0"], Request.ListBlockingMoveAsync("src", "dst", ListSide.Right, ListSide.Left, TimeSpan.Zero).GetArgs()),
            () => Assert.Equal(["BLMOVE", "a", "b", "LEFT", "LEFT", "2.5"], Request.ListBlockingMoveAsync("a", "b", ListSide.Left, ListSide.Left, TimeSpan.FromSeconds(2.5)).GetArgs()),
            () => Assert.Equal(["BLMOVE", "list1", "list2", "RIGHT", "RIGHT", "1"], Request.ListBlockingMoveAsync("list1", "list2", ListSide.Right, ListSide.Right, TimeSpan.FromSeconds(1)).GetArgs()),
            () => Assert.Equal(["BLMPOP", "2", "2", "key1", "key2", "LEFT"], Request.ListBlockingPopAsync(["key1", "key2"], ListSide.Left, TimeSpan.FromSeconds(2)).GetArgs()),
            () => Assert.Equal(["BLMPOP", "0", "1", "key", "RIGHT"], Request.ListBlockingPopAsync(["key"], ListSide.Right, TimeSpan.Zero).GetArgs()),
            () => Assert.Equal(["BLMPOP", "1.5", "3", "a", "b", "c", "LEFT"], Request.ListBlockingPopAsync(["a", "b", "c"], ListSide.Left, TimeSpan.FromSeconds(1.5)).GetArgs()),
            () => Assert.Equal(["BLMPOP", "0.25", "1", "single", "RIGHT"], Request.ListBlockingPopAsync(["single"], ListSide.Right, TimeSpan.FromMilliseconds(250)).GetArgs()),
            () => Assert.Equal(["BLMPOP", "5", "2", "key1", "key2", "LEFT", "COUNT", "3"], Request.ListBlockingPopAsync(["key1", "key2"], ListSide.Left, 3, TimeSpan.FromSeconds(5)).GetArgs()),
            () => Assert.Equal(["BLMPOP", "0", "1", "key", "RIGHT", "COUNT", "10"], Request.ListBlockingPopAsync(["key"], ListSide.Right, 10, TimeSpan.Zero).GetArgs()),
            () => Assert.Equal(["BLMPOP", "2.5", "4", "w", "x", "y", "z", "LEFT", "COUNT", "1"], Request.ListBlockingPopAsync(["w", "x", "y", "z"], ListSide.Left, 1, TimeSpan.FromSeconds(2.5)).GetArgs()),
            () => Assert.Equal(["BLMPOP", "1", "1", "test", "RIGHT", "COUNT", "5"], Request.ListBlockingPopAsync(["test"], ListSide.Right, 5, TimeSpan.FromSeconds(1)).GetArgs()),
            () => Assert.Equal(["LMPOP", "2", "key1", "key2", "LEFT", "COUNT", "3"], Request.ListLeftPopAsync(["key1", "key2"], 3).GetArgs()),
            () => Assert.Equal(["LMPOP", "2", "key1", "key2", "RIGHT", "COUNT", "3"], Request.ListRightPopAsync(["key1", "key2"], 3).GetArgs()),
            () => Assert.Equal(["LPUSHX", "a", "value"], Request.ListLeftPushAsync("a", "value", When.Exists).GetArgs()),
            () => Assert.Equal(["LPUSHX", "a", "one", "two"], Request.ListLeftPushAsync("a", ["one", "two"], When.Exists).GetArgs()),
            () => Assert.Equal(["RPUSHX", "a", "value"], Request.ListRightPushAsync("a", "value", When.Exists).GetArgs()),
            () => Assert.Equal(["RPUSHX", "a", "one", "two"], Request.ListRightPushAsync("a", ["one", "two"], When.Exists).GetArgs()),
            () => Assert.Equal(["LINDEX", "a", "0"], Request.ListGetByIndexAsync("a", 0).GetArgs()),
            () => Assert.Equal(["LINDEX", "a", "-1"], Request.ListGetByIndexAsync("a", -1).GetArgs()),
            () => Assert.Equal(["LINSERT", "a", "BEFORE", "pivot", "value"], Request.ListInsertBeforeAsync("a", "pivot", "value").GetArgs()),
            () => Assert.Equal(["LINSERT", "a", "AFTER", "pivot", "value"], Request.ListInsertAfterAsync("a", "pivot", "value").GetArgs()),
            () => Assert.Equal(["LMOVE", "src", "dest", "LEFT", "RIGHT"], Request.ListMoveAsync("src", "dest", ListSide.Left, ListSide.Right).GetArgs()),
            () => Assert.Equal(["LMOVE", "src", "dest", "RIGHT", "LEFT"], Request.ListMoveAsync("src", "dest", ListSide.Right, ListSide.Left).GetArgs()),
            () => Assert.Equal(["LPOS", "a", "element"], Request.ListPositionAsync("a", "element").GetArgs()),
            () => Assert.Equal(["LPOS", "a", "element", "RANK", "2"], Request.ListPositionAsync("a", "element", 2).GetArgs()),
            () => Assert.Equal(["LPOS", "a", "element", "MAXLEN", "100"], Request.ListPositionAsync("a", "element", 1, 100).GetArgs()),
            () => Assert.Equal(["LPOS", "a", "element", "COUNT", "5"], Request.ListPositionsAsync("a", "element", 5).GetArgs()),
            () => Assert.Equal(["LPOS", "a", "element", "COUNT", "5", "RANK", "2"], Request.ListPositionsAsync("a", "element", 5, 2).GetArgs()),
            () => Assert.Equal(["LPOS", "a", "element", "COUNT", "5", "MAXLEN", "50"], Request.ListPositionsAsync("a", "element", 5, 1, 50).GetArgs()),
            () => Assert.Equal(["LSET", "a", "0", "value"], Request.ListSetByIndexAsync("a", 0, "value").GetArgs()),
            () => Assert.Equal(["LSET", "a", "-1", "value"], Request.ListSetByIndexAsync("a", -1, "value").GetArgs()),

            // Hash Commands
            () => Assert.Equal(new string[] { "HGET", "key", "field" }, Request.HashGetAsync("key", "field").GetArgs()),
            () => Assert.Equal(["HMGET", "key", "field1", "field2"], Request.HashGetAsync("key", ["field1", "field2"]).GetArgs()),
            () => Assert.Equal(new string[] { "HGETALL", "key" }, Request.HashGetAllAsync("key").GetArgs()),
            () => Assert.Equal(["HSET", "key", "field1", "value1", "field2", "value2"], Request.HashSetAsync("key", [new KeyValuePair<ValkeyValue, ValkeyValue>("field1", "value1"), new KeyValuePair<ValkeyValue, ValkeyValue>("field2", "value2")]).GetArgs()),
            () => Assert.Equal(["HSET", "key", "field", "value"], Request.HashSetAsync("key", "field", "value").GetArgs()),
            () => Assert.Equal(["HSETNX", "key", "field", "value"], Request.HashSetNotExistsAsync("key", "field", "value").GetArgs()),
            () => Assert.Equal(new string[] { "HDEL", "key", "field" }, Request.HashDeleteAsync("key", "field").GetArgs()),
            () => Assert.Equal(["HDEL", "key", "field1", "field2"], Request.HashDeleteAsync("key", ["field1", "field2"]).GetArgs()),
            () => Assert.Equal(new string[] { "HEXISTS", "key", "field" }, Request.HashExistsAsync("key", "field").GetArgs()),
            () => Assert.Equal(new string[] { "HINCRBY", "key", "field", "5" }, Request.HashIncrementByAsync("key", "field", 5L).GetArgs()),
            () => Assert.Equal(new string[] { "HINCRBY", "key", "field", "1" }, Request.HashIncrementByAsync("key", "field", 1L).GetArgs()),
            () => Assert.Equal(new string[] { "HINCRBYFLOAT", "key", "field", "2.5" }, Request.HashIncrementByAsync("key", "field", 2.5).GetArgs()),
            () => Assert.Equal(new string[] { "HKEYS", "key" }, Request.HashKeysAsync("key").GetArgs()),
            () => Assert.Equal(new string[] { "HLEN", "key" }, Request.HashLengthAsync("key").GetArgs()),
            () => Assert.Equal(new string[] { "HSTRLEN", "key", "field" }, Request.HashStringLengthAsync("key", "field").GetArgs()),
            () => Assert.Equal(new string[] { "HVALS", "key" }, Request.HashValuesAsync("key").GetArgs()),
            () => Assert.Equal(new string[] { "HRANDFIELD", "key" }, Request.HashRandomFieldAsync("key").GetArgs()),
            () => Assert.Equal(new string[] { "HRANDFIELD", "key", "3" }, Request.HashRandomFieldsAsync("key", 3).GetArgs()),
            () => Assert.Equal(new string[] { "HRANDFIELD", "key", "3", "WITHVALUES" }, Request.HashRandomFieldsWithValuesAsync("key", 3).GetArgs()),

            // Geospatial Commands
            () => Assert.Equal(["GEOADD", "key", "13.361389000000001", "38.115555999999998", "Palermo"], Request.GeoAddAsync("key", "Palermo", new GeoPosition(13.361389, 38.115556)).GetArgs()),
            () => Assert.Equal(["GEOADD", "key", "15.087268999999999", "37.502668999999997", "Catania", "13.361389000000001", "38.115555999999998", "Palermo"], Request.GeoAddAsync("key", new SortedDictionary<ValkeyValue, GeoPosition> { ["Palermo"] = new(13.361389, 38.115556), ["Catania"] = new(15.087269, 37.502669) }).GetArgs()),
            () => Assert.Equal(["GEOADD", "key", "NX", "13.361389000000001", "38.115555999999998", "Palermo"], Request.GeoAddAsync("key", "Palermo", new GeoPosition(13.361389, 38.115556), new GeoAddOptions { Condition = GeoAddCondition.OnlyIfNotExists }).GetArgs()),
            () => Assert.Equal(["GEOADD", "key", "XX", "13.361389000000001", "38.115555999999998", "Palermo"], Request.GeoAddAsync("key", "Palermo", new GeoPosition(13.361389, 38.115556), new GeoAddOptions { Condition = GeoAddCondition.OnlyIfExists }).GetArgs()),
            () => Assert.Equal(["GEOADD", "key", "13.361389000000001", "38.115555999999998", "Palermo"], Request.GeoAddAsync("key", "Palermo", new GeoPosition(13.361389, 38.115556), new GeoAddOptions { Condition = GeoAddCondition.Always }).GetArgs()),
            () => Assert.Equal(["GEOADD", "key", "CH", "13.361389000000001", "38.115555999999998", "Palermo"], Request.GeoAddAsync("key", "Palermo", new GeoPosition(13.361389, 38.115556), new GeoAddOptions { Changed = true }).GetArgs()),
            () => Assert.Equal(["GEOADD", "key", "NX", "CH", "13.361389000000001", "38.115555999999998", "Palermo"], Request.GeoAddAsync("key", "Palermo", new GeoPosition(13.361389, 38.115556), new GeoAddOptions { Condition = GeoAddCondition.OnlyIfNotExists, Changed = true }).GetArgs()),
            () => Assert.Equal(["GEODIST", "key", "Palermo", "Catania", "m"], Request.GeoDistanceAsync("key", "Palermo", "Catania", GeoUnit.Meters).GetArgs()),
            () => Assert.Equal(["GEODIST", "key", "Palermo", "Catania", "km"], Request.GeoDistanceAsync("key", "Palermo", "Catania", GeoUnit.Kilometers).GetArgs()),
            () => Assert.Equal(["GEODIST", "key", "Palermo", "Catania", "mi"], Request.GeoDistanceAsync("key", "Palermo", "Catania", GeoUnit.Miles).GetArgs()),
            () => Assert.Equal(["GEODIST", "key", "Palermo", "Catania", "ft"], Request.GeoDistanceAsync("key", "Palermo", "Catania", GeoUnit.Feet).GetArgs()),
            () => Assert.Equal(["GEOHASH", "key", "Palermo"], Request.GeoHashAsync("key", "Palermo").GetArgs()),
            () => Assert.Equal(["GEOHASH", "key", "Palermo", "Catania"], Request.GeoHashAsync("key", ["Palermo", "Catania"]).GetArgs()),
            () => Assert.Equal(["GEOPOS", "key", "Palermo"], Request.GeoPositionAsync("key", "Palermo").GetArgs()),
            () => Assert.Equal(["GEOPOS", "key", "Palermo", "Catania"], Request.GeoPositionAsync("key", ["Palermo", "Catania"]).GetArgs()),
            () => Assert.Equal(["GEOSEARCH", "key", "FROMMEMBER", "Palermo", "BYRADIUS", "100", "km"], Request.GeoSearchAsync("key", "Palermo", new GeoSearchCircle(100, GeoUnit.Kilometers)).GetArgs()),
            () => Assert.Equal(["GEOSEARCH", "key", "FROMLONLAT", "13.361389000000001", "38.115555999999998", "BYRADIUS", "200", "m"], Request.GeoSearchAsync("key", new GeoPosition(13.361389, 38.115556), new GeoSearchCircle(200, GeoUnit.Meters)).GetArgs()),
            () => Assert.Equal(["GEOSEARCH", "key", "FROMMEMBER", "Palermo", "BYBOX", "300", "400", "km"], Request.GeoSearchAsync("key", "Palermo", new GeoSearchBox(400, 300, GeoUnit.Kilometers)).GetArgs()),
            () => Assert.Equal(["GEOSEARCH", "key", "FROMMEMBER", "Palermo", "BYRADIUS", "100", "km", "ASC", "COUNT", "10", "WITHCOORD", "WITHDIST", "WITHHASH"], Request.GeoSearchAsync("key", "Palermo", new GeoSearchCircle(100, GeoUnit.Kilometers), new GeoSearchOptions { Order = Order.Ascending, Count = 10, WithPosition = true, WithDistance = true, WithHash = true }).GetArgs()),
            () => Assert.Equal(["GEOSEARCH", "key", "FROMMEMBER", "Palermo", "BYRADIUS", "100", "km", "COUNT", "5", "ANY"], Request.GeoSearchAsync("key", "Palermo", new GeoSearchCircle(100, GeoUnit.Kilometers), new GeoSearchOptions { Count = 5, Any = true }).GetArgs()),
            () => Assert.Equal(["GEOSEARCHSTORE", "dest", "key", "FROMMEMBER", "Palermo", "BYRADIUS", "100", "km"], Request.GeoSearchAndStoreAsync("key", "dest", "Palermo", new GeoSearchCircle(100, GeoUnit.Kilometers)).GetArgs()),
            () => Assert.Equal(["GEOSEARCHSTORE", "dest", "key", "FROMLONLAT", "13.361389000000001", "38.115555999999998", "BYRADIUS", "200", "m", "STOREDIST"], Request.GeoSearchAndStoreAsync("key", "dest", new GeoPosition(13.361389, 38.115556), new GeoSearchCircle(200, GeoUnit.Meters), new GeoSearchStoreOptions { StoreDistances = true }).GetArgs()),
            () => Assert.Equal(["GEOSEARCHSTORE", "dest", "key", "FROMMEMBER", "Palermo", "BYRADIUS", "100", "km", "ASC", "COUNT", "5", "ANY"], Request.GeoSearchAndStoreAsync("key", "dest", "Palermo", new GeoSearchCircle(100, GeoUnit.Kilometers), new GeoSearchStoreOptions { Order = Order.Ascending, Count = 5, Any = true }).GetArgs()),
            () => Assert.Throws<ArgumentException>(() => Request.GeoSearchAsync("key", "Palermo", new GeoSearchCircle(100, GeoUnit.Kilometers), new GeoSearchOptions { Any = true })),
            () => Assert.Throws<ArgumentException>(() => Request.GeoSearchAndStoreAsync("key", "dest", "Palermo", new GeoSearchCircle(100, GeoUnit.Kilometers), new GeoSearchStoreOptions { Any = true })),

            // HyperLogLog Commands
            () => Assert.Equal(["PFADD", "key", "element"], Request.HyperLogLogAddAsync("key", "element").GetArgs()),
            () => Assert.Equal(["PFADD", "key", "element1", "element2", "element3"], Request.HyperLogLogAddAsync("key", ["element1", "element2", "element3"]).GetArgs()),
            () => Assert.Equal(["PFCOUNT", "key"], Request.HyperLogLogLengthAsync("key").GetArgs()),
            () => Assert.Equal(["PFCOUNT", "key1", "key2", "key3"], Request.HyperLogLogLengthAsync(["key1", "key2", "key3"]).GetArgs()),
            () => Assert.Equal(["PFMERGE", "dest", "src1", "src2"], Request.HyperLogLogMergeAsync("dest", "src1", "src2").GetArgs()),
            () => Assert.Equal(["PFMERGE", "dest", "src1", "src2", "src3"], Request.HyperLogLogMergeAsync("dest", ["src1", "src2", "src3"]).GetArgs()),

            // Bitmap Commands
            () => Assert.Equal(["GETBIT", "key", "0"], Request.GetBitAsync("key", 0).GetArgs()),
            () => Assert.Equal(["GETBIT", "key", "100"], Request.GetBitAsync("key", 100).GetArgs()),
            () => Assert.Equal(["SETBIT", "key", "0", "1"], Request.SetBitAsync("key", 0, true).GetArgs()),
            () => Assert.Equal(["SETBIT", "key", "5", "0"], Request.SetBitAsync("key", 5, false).GetArgs()),
            () => Assert.Equal(["BITCOUNT", "key", "0", "-1"], Request.BitCountAsync("key", 0, -1, BitmapIndexType.Byte).GetArgs()),
            () => Assert.Equal(["BITCOUNT", "key", "1", "5", "BIT"], Request.BitCountAsync("key", 1, 5, BitmapIndexType.Bit).GetArgs()),
            () => Assert.Equal(["BITPOS", "key", "1", "0", "-1"], Request.BitPosAsync("key", true, 0, -1, BitmapIndexType.Byte).GetArgs()),
            () => Assert.Equal(["BITPOS", "key", "0", "2", "10", "BIT"], Request.BitPosAsync("key", false, 2, 10, BitmapIndexType.Bit).GetArgs()),
            () => Assert.Equal(["BITOP", "AND", "dest", "key1", "key2"], Request.BitOpAsync(Bitwise.And, "dest", ["key1", "key2"]).GetArgs()),
            () => Assert.Equal(["BITOP", "OR", "dest", "key1", "key2", "key3"], Request.BitOpAsync(Bitwise.Or, "dest", ["key1", "key2", "key3"]).GetArgs()),
            () => Assert.Equal(["BITOP", "XOR", "dest", "key1", "key2"], Request.BitOpAsync(Bitwise.Xor, "dest", ["key1", "key2"]).GetArgs()),
            () => Assert.Equal(["BITOP", "NOT", "dest", "key1"], Request.BitOpAsync(Bitwise.Not, "dest", ["key1"]).GetArgs()),
            () => Assert.Equal(["BITFIELD", "key", "GET", "u8", "0"], Request.BitFieldAsync("key", [new BitFieldOptions.BitFieldGet(BitFieldOptions.Encoding.Unsigned(8), new BitFieldOptions.BitOffset(0))]).GetArgs()),
            () => Assert.Equal(["BITFIELD", "key", "SET", "i16", "#1", "100"], Request.BitFieldAsync("key", [new BitFieldOptions.BitFieldSet(BitFieldOptions.Encoding.Signed(16), new BitFieldOptions.BitOffsetMultiplier(1), 100)]).GetArgs()),
            () => Assert.Equal(["BITFIELD", "key", "INCRBY", "u32", "8", "5"], Request.BitFieldAsync("key", [new BitFieldOptions.BitFieldIncrBy(BitFieldOptions.Encoding.Unsigned(32), new BitFieldOptions.BitOffset(8), 5)]).GetArgs()),
            () => Assert.Equal(["BITFIELD", "key", "OVERFLOW", "WRAP", "SET", "u8", "0", "255"], Request.BitFieldAsync("key", [new BitFieldOptions.BitFieldOverflow(BitFieldOptions.OverflowType.Wrap), new BitFieldOptions.BitFieldSet(BitFieldOptions.Encoding.Unsigned(8), new BitFieldOptions.BitOffset(0), 255)]).GetArgs()),
            () => Assert.Equal(["BITFIELDREADONLY", "key", "GET", "u8", "0", "GET", "i4", "8"], Request.BitFieldReadOnlyAsync("key", [new BitFieldOptions.BitFieldGet(BitFieldOptions.Encoding.Unsigned(8), new BitFieldOptions.BitOffset(0)), new BitFieldOptions.BitFieldGet(BitFieldOptions.Encoding.Signed(4), new BitFieldOptions.BitOffset(8))]).GetArgs()),

            // Hash Field Expire Commands (Valkey 9.0+)
            () => Assert.Equal(["HGETEX", "key", "PX", "60000", "FIELDS", "2", "field1", "field2"], Request.HashGetExpiryAsync("key", ["field1", "field2"], GetExpiryOption.ExpireIn(TimeSpan.FromSeconds(60))).GetArgs()),
            () => Assert.Equal(["HGETEX", "key", "PX", "5000", "FIELDS", "1", "field1"], Request.HashGetExpiryAsync("key", ["field1"], GetExpiryOption.ExpireIn(TimeSpan.FromMilliseconds(5000))).GetArgs()),
            () => Assert.Equal(["HGETEX", "key", "PXAT", "1609459200000", "FIELDS", "1", "field1"], Request.HashGetExpiryAsync("key", ["field1"], GetExpiryOption.ExpireAt(DateTimeOffset.FromUnixTimeSeconds(1609459200))).GetArgs()),
            () => Assert.Equal(["HGETEX", "key", "PXAT", "1609459200000", "FIELDS", "1", "field1"], Request.HashGetExpiryAsync("key", ["field1"], GetExpiryOption.ExpireAt(DateTimeOffset.FromUnixTimeMilliseconds(1609459200000))).GetArgs()),
            () => Assert.Equal(["HGETEX", "key", "PERSIST", "FIELDS", "1", "field1"], Request.HashGetExpiryAsync("key", ["field1"], GetExpiryOption.Persist()).GetArgs()),
            () => Assert.Equal(["HSETEX", "key", "PX", "60000", "FIELDS", "2", "field1", "value1", "field2", "value2"], Request.HashSetExpiryAsync("key", [new KeyValuePair<ValkeyValue, ValkeyValue>("field1", "value1"), new KeyValuePair<ValkeyValue, ValkeyValue>("field2", "value2")], SetExpiryOption.ExpireIn(TimeSpan.FromSeconds(60)), HashSetCondition.Always).GetArgs()),
            () => Assert.Equal(["HSETEX", "key", "PX", "5000", "FIELDS", "1", "field1", "value1"], Request.HashSetExpiryAsync("key", [new KeyValuePair<ValkeyValue, ValkeyValue>("field1", "value1")], SetExpiryOption.ExpireIn(TimeSpan.FromMilliseconds(5000)), HashSetCondition.Always).GetArgs()),
            () => Assert.Equal(["HSETEX", "key", "PXAT", "60000", "FIELDS", "2", "field1", "value1", "field2", "value2"], Request.HashSetExpiryAsync("key", [new KeyValuePair<ValkeyValue, ValkeyValue>("field1", "value1"), new KeyValuePair<ValkeyValue, ValkeyValue>("field2", "value2")], SetExpiryOption.ExpireAt(DateTimeOffset.FromUnixTimeMilliseconds(60000)), HashSetCondition.Always).GetArgs()),
            () => Assert.Equal(["HSETEX", "key", "PXAT", "5000", "FIELDS", "1", "field1", "value1"], Request.HashSetExpiryAsync("key", [new KeyValuePair<ValkeyValue, ValkeyValue>("field1", "value1")], SetExpiryOption.ExpireAt(DateTimeOffset.FromUnixTimeMilliseconds(5000)), HashSetCondition.Always).GetArgs()),
            () => Assert.Equal(["HSETEX", "key", "KEEPTTL", "FIELDS", "1", "field1", "value1"], Request.HashSetExpiryAsync("key", [new KeyValuePair<ValkeyValue, ValkeyValue>("field1", "value1")], SetExpiryOption.KeepTimeToLive(), HashSetCondition.Always).GetArgs()),
            () => Assert.Equal(["HSETEX", "key", "FNX", "PX", "60000", "FIELDS", "1", "field1", "value1"], Request.HashSetExpiryAsync("key", [new KeyValuePair<ValkeyValue, ValkeyValue>("field1", "value1")], SetExpiryOption.ExpireIn(TimeSpan.FromSeconds(60)), HashSetCondition.OnlyIfNoneExist).GetArgs()),
            () => Assert.Equal(["HSETEX", "key", "FXX", "PX", "60000", "FIELDS", "1", "field1", "value1"], Request.HashSetExpiryAsync("key", [new KeyValuePair<ValkeyValue, ValkeyValue>("field1", "value1")], SetExpiryOption.ExpireIn(TimeSpan.FromSeconds(60)), HashSetCondition.OnlyIfAllExist).GetArgs()),
            () => Assert.Equal(["HPERSIST", "key", "FIELDS", "2", "field1", "field2"], Request.HashPersistAsync("key", ["field1", "field2"]).GetArgs()),
            () => Assert.Equal(["HPEXPIRE", "key", "60000", "FIELDS", "2", "field1", "field2"], Request.HashExpireAsync("key", TimeSpan.FromSeconds(60), ["field1", "field2"], ExpireCondition.Always).GetArgs()),
            () => Assert.Equal(["HPEXPIRE", "key", "60000", "NX", "FIELDS", "2", "field1", "field2"], Request.HashExpireAsync("key", TimeSpan.FromSeconds(60), ["field1", "field2"], ExpireCondition.OnlyIfNotExists).GetArgs()),
            () => Assert.Equal(["HPEXPIRE", "key", "60000", "XX", "FIELDS", "2", "field1", "field2"], Request.HashExpireAsync("key", TimeSpan.FromSeconds(60), ["field1", "field2"], ExpireCondition.OnlyIfExists).GetArgs()),
            () => Assert.Equal(["HPEXPIRE", "key", "60000", "GT", "FIELDS", "2", "field1", "field2"], Request.HashExpireAsync("key", TimeSpan.FromSeconds(60), ["field1", "field2"], ExpireCondition.OnlyIfGreaterThan).GetArgs()),
            () => Assert.Equal(["HPEXPIRE", "key", "60000", "LT", "FIELDS", "2", "field1", "field2"], Request.HashExpireAsync("key", TimeSpan.FromSeconds(60), ["field1", "field2"], ExpireCondition.OnlyIfLessThan).GetArgs()),
            () => Assert.Equal(["HPEXPIRE", "key", "5500", "FIELDS", "2", "field1", "field2"], Request.HashExpireAsync("key", TimeSpan.FromMilliseconds(5500), ["field1", "field2"], ExpireCondition.Always).GetArgs()),
            () => Assert.Equal(["HPEXPIREAT", "key", "1609459200000", "FIELDS", "2", "field1", "field2"], Request.HashExpireAtAsync("key", DateTimeOffset.FromUnixTimeSeconds(1609459200), ["field1", "field2"], ExpireCondition.Always).GetArgs()),
            () => Assert.Equal(["HPEXPIREAT", "key", "1609459200000", "NX", "FIELDS", "2", "field1", "field2"], Request.HashExpireAtAsync("key", DateTimeOffset.FromUnixTimeSeconds(1609459200), ["field1", "field2"], ExpireCondition.OnlyIfNotExists).GetArgs()),
            () => Assert.Equal(["HPEXPIREAT", "key", "1609459200500", "FIELDS", "2", "field1", "field2"], Request.HashExpireAtAsync("key", DateTimeOffset.FromUnixTimeMilliseconds(1609459200500), ["field1", "field2"], ExpireCondition.Always).GetArgs()),
            () => Assert.Equal(["HPEXPIRETIME", "key", "FIELDS", "2", "field1", "field2"], Request.HashExpireTimeAsync("key", ["field1", "field2"]).GetArgs()),
            () => Assert.Equal(["HPTTL", "key", "FIELDS", "2", "field1", "field2"], Request.HashTimeToLiveAsync("key", ["field1", "field2"]).GetArgs())
        );

    [Fact]
    public void ValidateCommandConverters() => Assert.Multiple(
            () => Assert.Equal(1, Request.CustomCommand([]).Converter(1)),
            () => Assert.Equal(.1, Request.CustomCommand([]).Converter(.1)),
            () => Assert.Null(Request.CustomCommand([]).Converter(null)),

            // String Commands
            () => Assert.True(Request.StringSet("key", "value").Converter("OK")),
            () => Assert.True(Request.StringSetNX("key", "value").Converter("OK")),
            () => Assert.False(Request.StringSetNX("key", "value").Converter(null)),
            () => Assert.True(Request.StringSetXX("key", "value").Converter("OK")),
            () => Assert.False(Request.StringSetXX("key", "value").Converter(null)),
            () => Assert.Equal<GlideString>("value", Request.StringGet("key").Converter("value")),
            () => Assert.Equal(ValkeyValue.Null, Request.StringGet("key").Converter(null!)),
            () => Assert.Equal(5L, Request.StringLength("key").Converter(5L)),
            () => Assert.Equal(0L, Request.StringLength("key").Converter(0L)),
            () => Assert.Equal(new ValkeyValue("hello"), Request.StringGetRange("key", 0, 4).Converter("hello")),
            () => Assert.Equal(new ValkeyValue(""), Request.StringGetRange("key", 0, 4).Converter("")),
            () => Assert.Equal(ValkeyValue.Null, Request.StringGetRange("key", 0, 4).Converter(null!)),
            () => Assert.Equal((ValkeyValue)10L, Request.StringSetRange("key", 5, "world").Converter(10L)),
            () => Assert.Equal(11L, Request.StringAppend("key", "value").Converter(11L)),
            () => Assert.Equal(9L, Request.StringDecr("key").Converter(9L)),
            () => Assert.Equal(5L, Request.StringDecrBy("key", 5).Converter(5L)),
            () => Assert.Equal(11L, Request.StringIncr("key").Converter(11L)),
            () => Assert.Equal(15L, Request.StringIncrBy("key", 5).Converter(15L)),
            () => Assert.Equal(10.5, Request.StringIncrByFloat("key", 0.5).Converter(10.5)),
            () => Assert.True(Request.StringSetMultiple([
                new KeyValuePair<ValkeyKey, ValkeyValue>("key1", "value1"),
                new KeyValuePair<ValkeyKey, ValkeyValue>("key2", "value2")
            ]).Converter("OK")),
            () => Assert.False(Request.StringSetMultiple([
                new KeyValuePair<ValkeyKey, ValkeyValue>("key1", "value1"),
                new KeyValuePair<ValkeyKey, ValkeyValue>("key2", "value2")
            ]).Converter("ERROR")),
            () => Assert.True(Request.StringSetMultipleNX([new KeyValuePair<ValkeyKey, ValkeyValue>("key1", "value1")]).Converter(true)),
            () => Assert.False(Request.StringSetMultipleNX([new KeyValuePair<ValkeyKey, ValkeyValue>("key1", "value1")]).Converter(false)),
            () => Assert.Equal("test_value", Request.StringGetDelete("test_key").Converter(new GlideString("test_value")).ToString()),
            () => Assert.True(Request.StringGetDelete("test_key").Converter(null!).IsNull),
            () => Assert.Equal("test_value", Request.StringGetSetExpiry("test_key", TimeSpan.FromSeconds(60)).Converter(new GlideString("test_value")).ToString()),
            () => Assert.True(Request.StringGetSetExpiry("test_key", TimeSpan.FromSeconds(60)).Converter(null!).IsNull),

            // Server Management Commands
            () => Assert.Equal(["CONFIGGET", "*"], Request.ConfigGetAsync("*").GetArgs()),
            () => Assert.Equal(["CONFIGGET", "maxmemory"], Request.ConfigGetAsync("maxmemory").GetArgs()),
            () => Assert.Equal(["CONFIGGET", "*"], Request.ConfigGetAsync().GetArgs()),
            () => Assert.Equal(["CONFIGRESETSTAT"], Request.ConfigResetStatisticsAsync().GetArgs()),
            () => Assert.Equal(["CONFIGREWRITE"], Request.ConfigRewriteAsync().GetArgs()),
            () => Assert.Equal(["CONFIGSET", "maxmemory", "100mb"], Request.ConfigSetAsync("maxmemory", "100mb").GetArgs()),
            () => Assert.Equal(["CONFIGSET", "timeout", "300"], Request.ConfigSetAsync("timeout", "300").GetArgs()),
            () => Assert.Equal(["DBSIZE"], Request.DatabaseSizeAsync().GetArgs()),
            () => Assert.Equal(["DBSIZE"], Request.DatabaseSizeAsync().GetArgs()),
            () => Assert.Equal(["FLUSHALL"], Request.FlushAllDatabasesAsync().GetArgs()),
            () => Assert.Equal(["FLUSHDB"], Request.FlushDatabaseAsync().GetArgs()),
            () => Assert.Equal(["FLUSHDB"], Request.FlushDatabaseAsync().GetArgs()),
            () => Assert.Equal(["LASTSAVE"], Request.LastSaveAsync().GetArgs()),
            () => Assert.Equal(["TIME"], Request.TimeAsync().GetArgs()),
            () => Assert.Equal(["LOLWUT"], Request.LolwutAsync().GetArgs()),

            // Server Management Command Converters
            () => Assert.Equal([new("maxmemory", "100mb")], Request.ConfigGetAsync("maxmemory").Converter(new object[] { (gs)"maxmemory", "100mb" })),
            () => Assert.Equal([], Request.ConfigGetAsync("nonexistent").Converter(Array.Empty<object>())),
            () => Assert.Equal(100L, Request.DatabaseSizeAsync().Converter(100L)),
            () => Assert.Equal(0L, Request.DatabaseSizeAsync().Converter(0L)),
            () => Assert.Equal(DateTime.UnixEpoch.AddSeconds(1609459200), Request.LastSaveAsync().Converter(1609459200L)),
            () => Assert.Equal(DateTime.UnixEpoch.AddSeconds(1609459200).AddTicks(123456 * 10), Request.TimeAsync().Converter(["1609459200", "123456"])),
            () => Assert.Equal("Valkey 7.0.0", Request.LolwutAsync().Converter("Valkey 7.0.0")),
            () => Assert.Equal("test_value", Request.StringGetSetExpiry("test_key", new DateTime(2021, 1, 1, 0, 0, 0, DateTimeKind.Utc)).Converter(new GlideString("test_value")).ToString()),
            () => Assert.Equal("common", Request.StringLongestCommonSubsequence("key1", "key2").Converter(new GlideString("common"))!.ToString()),
            () => Assert.Equal(5L, Request.StringLongestCommonSubsequenceLength("key1", "key2").Converter(5L)),

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

            () => Assert.True(Request.SetAddAsync("key", "member").Converter(1L)),
            () => Assert.False(Request.SetAddAsync("key", "member").Converter(0L)),
            () => Assert.True(Request.SetRemoveAsync("key", "member").Converter(1L)),
            () => Assert.False(Request.SetRemoveAsync("key", "member").Converter(0L)),

            () => Assert.Equal(2L, Request.SetAddAsync("key", ["member1", "member2"]).Converter(2L)),
            () => Assert.Equal(1L, Request.SetRemoveAsync("key", ["member1", "member2"]).Converter(1L)),
            () => Assert.Equal(5L, Request.SetLengthAsync("key").Converter(5L)),
            () => Assert.Equal(3L, Request.SetIntersectionLengthAsync(["key1", "key2"]).Converter(3L)),
            () => Assert.Equal(4L, Request.SetUnionStoreAsync("dest", ["key1", "key2"]).Converter(4L)),
            () => Assert.Equal(2L, Request.SetIntersectStoreAsync("dest", ["key1", "key2"]).Converter(2L)),
            () => Assert.Equal(1L, Request.SetDifferenceStoreAsync("dest", ["key1", "key2"]).Converter(1L)),

            () => Assert.Equal<ValkeyValue>("member", Request.SetPopAsync("key").Converter("member")),
            () => Assert.Null(Request.SetPopAsync("key").Converter(null!)),

            // Generic Commands Converters
            () => Assert.True(Request.DeleteAsync("key").Converter(1L)),
            () => Assert.False(Request.DeleteAsync("key").Converter(0L)),
            () => Assert.Equal(2L, Request.DeleteAsync(["key1", "key2"]).Converter(2L)),
            () => Assert.True(Request.UnlinkAsync("key").Converter(1L)),
            () => Assert.False(Request.UnlinkAsync("key").Converter(0L)),
            () => Assert.Equal(3L, Request.UnlinkAsync(["key1", "key2", "key3"]).Converter(3L)),
            () => Assert.True(Request.ExistsAsync("key").Converter(1L)),
            () => Assert.False(Request.ExistsAsync("key").Converter(0L)),
            () => Assert.Equal(2L, Request.ExistsAsync(["key1", "key2"]).Converter(2L)),
            () => Assert.True(Request.ExpireAsync("key", TimeSpan.FromSeconds(60)).Converter(true)),
            () => Assert.False(Request.ExpireAsync("key", TimeSpan.FromSeconds(60)).Converter(false)),
            () => Assert.Equal(TimeSpan.FromMilliseconds(30), Request.TimeToLiveAsync("key").Converter(30L).TimeToLive),
            () => Assert.False(Request.TimeToLiveAsync("key").Converter(-1L).HasTimeToLive),
            () => Assert.False(Request.TimeToLiveAsync("key").Converter(-2L).Exists),
            () => Assert.Equal(ValkeyType.String, Request.TypeAsync("key").Converter("string")),
            () => Assert.Equal(ValkeyType.List, Request.TypeAsync("key").Converter("list")),
            () => Assert.Equal(ValkeyType.Set, Request.TypeAsync("key").Converter("set")),
            () => Assert.Equal(ValkeyType.Hash, Request.TypeAsync("key").Converter("hash")),
            () => Assert.Equal(ValkeyType.Stream, Request.TypeAsync("key").Converter("stream")),
            () => Assert.Equal(ValkeyType.None, Request.TypeAsync("key").Converter("none")),
            () => Assert.True(Request.RenameAsync("oldkey", "newkey").Converter("OK")),
            () => Assert.True(Request.RenameIfNotExistsAsync("oldkey", "newkey").Converter(true)),
            () => Assert.False(Request.RenameIfNotExistsAsync("oldkey", "newkey").Converter(false)),
            () => Assert.True(Request.PersistAsync("key").Converter(true)),
            () => Assert.False(Request.PersistAsync("key").Converter(false)),
            () => Assert.NotNull(Request.DumpAsync("key").Converter("dumpdata")),
            () => Assert.Null(Request.DumpAsync("key").Converter(null!)),
            () => Assert.Equal(ValkeyValue.Ok, Request.RestoreAsync("key", []).Converter("OK")),
            () => Assert.True(Request.TouchAsync("key").Converter(1L)),
            () => Assert.False(Request.TouchAsync("key").Converter(0L)),
            () => Assert.Equal(2L, Request.TouchAsync(["key1", "key2"]).Converter(2L)),
            () => Assert.True(Request.CopyAsync("src", "dest").Converter(true)),
            () => Assert.False(Request.CopyAsync("src", "dest").Converter(false)),
            () => Assert.Equal(DateTimeOffset.FromUnixTimeMilliseconds(1609459200000L), Request.ExpireTimeAsync("key").Converter(1609459200000L)),
            () => Assert.Null(Request.ExpireTimeAsync("key").Converter(-1L)),
            () => Assert.Null(Request.ExpireTimeAsync("key").Converter(-2L)),
            () => Assert.Equal("embstr", Request.ObjectEncodingAsync("key").Converter(new GlideString("embstr"))),
            () => Assert.Null(Request.ObjectEncodingAsync("key").Converter(null!)),
            () => Assert.Equal(5L, Request.ObjectFrequencyAsync("key").Converter(5L)),
            () => Assert.Null(Request.ObjectFrequencyAsync("key").Converter(-1L)),
            () => Assert.Equal(TimeSpan.FromSeconds(10), Request.ObjectIdleTimeAsync("key").Converter(10L)),
            () => Assert.Null(Request.ObjectIdleTimeAsync("key").Converter(-1L)),
            () => Assert.Equal(3L, Request.ObjectRefCountAsync("key").Converter(3L)),
            () => Assert.Null(Request.ObjectRefCountAsync("key").Converter(-1L)),
            () => Assert.Equal(new ValkeyKey("randomkey"), Request.RandomKeyAsync().Converter(new GlideString("randomkey"))),
            () => Assert.Null(Request.RandomKeyAsync().Converter(null!)),
            () => Assert.True(Request.MoveAsync("key", 1).Converter(true)),
            () => Assert.False(Request.MoveAsync("key", 1).Converter(false)),

            // SCAN Commands Converters
            () =>
            {
                var result = Request.ScanAsync("0").Converter(["0", new object[] { (gs)"key1", (gs)"key2" }]);
                Assert.Equal("0", result.Item1);
                Assert.Equal([new ValkeyKey("key1"), new ValkeyKey("key2")], result.Item2);
            },
            () =>
            {
                var result = Request.ScanAsync("10").Converter(["5", new object[] { (gs)"test" }]);
                Assert.Equal("5", result.Item1);
                Assert.Equal([new ValkeyKey("test")], result.Item2);
            },
            () =>
            {
                var result = Request.ScanAsync("0").Converter(["0", Array.Empty<object>()]);
                Assert.Equal("0", result.Item1);
                Assert.Empty(result.Item2);
            },

            // WAIT Commands Converters
            () => Assert.Equal(2L, Request.WaitAsync(1, TimeSpan.FromMilliseconds(1000)).Converter(2L)),
            () => Assert.Equal(0L, Request.WaitAsync(0, TimeSpan.Zero).Converter(0L)),
            () => Assert.Equal(1L, Request.WaitAsync(3, TimeSpan.FromMilliseconds(5000)).Converter(1L)),

            () => Assert.Equal("one", Request.ListLeftPopAsync("a").Converter("one")),
            () => Assert.Equal(["one", "two"], Request.ListLeftPopAsync("a", 2).Converter([(gs)"one", (gs)"two"])!),
            () => Assert.Null(Request.ListLeftPopAsync("a", 2).Converter(null!)),
            () => Assert.Equal(ValkeyValue.Null, Request.ListLeftPopAsync("a").Converter(null!)),
            () => Assert.Equal(1L, Request.ListLeftPushAsync("a", "value").Converter(1L)),
            () => Assert.Equal(2L, Request.ListLeftPushAsync("a", ["one", "two"]).Converter(2L)),
            () => Assert.Equal("three", Request.ListRightPopAsync("a").Converter("three")),
            () => Assert.Equal(ValkeyValue.Null, Request.ListRightPopAsync("a").Converter(null!)),
            () => Assert.Equal(["three", "four"], Request.ListRightPopAsync("a", 2).Converter([(gs)"three", (gs)"four"])!),
            () => Assert.Null(Request.ListRightPopAsync("a", 2).Converter(null!)),
            () => Assert.Equal(2L, Request.ListRightPushAsync("a", "value").Converter(2L)),
            () => Assert.Equal(3L, Request.ListRightPushAsync("a", ["three", "four"]).Converter(3L)),
            () => Assert.Equal(5L, Request.ListLengthAsync("a").Converter(5L)),
            () => Assert.Equal(0L, Request.ListLengthAsync("nonexistent").Converter(0L)),
            () => Assert.Equal(2L, Request.ListRemoveAsync("a", "value", 0).Converter(2L)),
            () => Assert.Equal(1L, Request.ListRemoveAsync("a", "value", 1).Converter(1L)),
            () => Assert.Equal(0L, Request.ListRemoveAsync("a", "nonexistent", 0).Converter(0L)),
            () => Assert.Equal(ValkeyValue.Ok, Request.ListTrimAsync("a", 0, 10).Converter("OK")),
            () => Assert.Equal(["one", "two", "three"], Request.ListRangeAsync("a", 0, -1).Converter([(gs)"one", (gs)"two", (gs)"three"])),
            () => Assert.IsType<ValkeyValue[]>(Request.ListRangeAsync("a", 0, -1).Converter([(gs)"one", (gs)"two", (gs)"three"])),
            () => Assert.Equal([], Request.ListRangeAsync("nonexistent", 0, -1).Converter([])),

            // Hash Commands
            () => Assert.Equal<GlideString>("value", Request.HashGetAsync("key", "field").Converter("value")),
            () => Assert.Equal(ValkeyValue.Null, Request.HashGetAsync("key", "field").Converter(null!)),
            () => Assert.Equal(2L, Request.HashSetAsync("key", [new KeyValuePair<ValkeyValue, ValkeyValue>("field", "value")]).Converter(2L)),
            () => Assert.True(Request.HashDeleteAsync("key", "field").Converter(1L)),
            () => Assert.False(Request.HashDeleteAsync("key", "field").Converter(0L)),
            () => Assert.Equal(2L, Request.HashDeleteAsync("key", ["field1", "field2"]).Converter(2L)),
            () => Assert.True(Request.HashExistsAsync("key", "field").Converter(true)),
            () => Assert.False(Request.HashExistsAsync("key", "field").Converter(false)),
            () => Assert.Equal(15L, Request.HashIncrementByAsync("key", "field", 5L).Converter(15L)),
            () => Assert.Equal(10L, Request.HashIncrementByAsync("key", "field", 1L).Converter(10L)),
            () => Assert.Equal(12.5, Request.HashIncrementByAsync("key", "field", 2.5).Converter(12.5)),
            () => Assert.Equal<ISet<ValkeyValue>>(new HashSet<ValkeyValue> { "field1", "field2" }, Request.HashKeysAsync("key").Converter([(gs)"field1", (gs)"field2"])),
            () => Assert.Empty(Request.HashKeysAsync("nonexistent").Converter([])),
            () => Assert.Equal(5L, Request.HashLengthAsync("key").Converter(5L)),
            () => Assert.Equal(10L, Request.HashStringLengthAsync("key", "field").Converter(10L)),

            // Hash Field Expire Commands converters (Valkey 9.0+)
            () => Assert.Equal((ValkeyValue[])["value1", "value2"], Request.HashGetExpiryAsync("key", ["field1", "field2"], GetExpiryOption.ExpireIn(TimeSpan.FromSeconds(60))).Converter([(gs)"value1", (gs)"value2"])),
            () => Assert.Equal((ValkeyValue[])[ValkeyValue.Null], Request.HashGetExpiryAsync("key", ["field1"], GetExpiryOption.Persist()).Converter([null!])),
            () => Assert.True(Request.HashSetExpiryAsync("key", [new KeyValuePair<ValkeyValue, ValkeyValue>("field1", "value1")], SetExpiryOption.ExpireIn(TimeSpan.FromSeconds(60)), HashSetCondition.Always).Converter(1L)),
            () => Assert.False(Request.HashSetExpiryAsync("key", [new KeyValuePair<ValkeyValue, ValkeyValue>("field1", "value1")], SetExpiryOption.ExpireIn(TimeSpan.FromSeconds(60)), HashSetCondition.Always).Converter(0L)),
            () => Assert.Equal([HashPersistResult.ExpiryRemoved, HashPersistResult.NoExpiry, HashPersistResult.NoField], Request.HashPersistAsync("key", ["field1", "field2", "field3"]).Converter([1L, -1L, -2L])),
            () => Assert.Equal([HashExpireResult.ExpirySet, HashExpireResult.ConditionNotMet, HashExpireResult.NoField], Request.HashExpireAsync("key", TimeSpan.FromSeconds(60), ["field1", "field2", "field3"], ExpireCondition.Always).Converter([1L, 0L, -2L])),
            () => Assert.Equal([HashExpireResult.ExpirySet, HashExpireResult.ConditionNotMet, HashExpireResult.NoField], Request.HashExpireAtAsync("key", DateTimeOffset.FromUnixTimeSeconds(1609459200), ["field1", "field2", "field3"], ExpireCondition.Always).Converter([1L, 0L, -2L])),
            () => Assert.True(Request.HashExpireTimeAsync("key", ["field1"]).Converter([1609459200000L])[0].HasExpiry),
            () => Assert.False(Request.HashExpireTimeAsync("key", ["field1"]).Converter([-1L])[0].HasExpiry),
            () => Assert.False(Request.HashExpireTimeAsync("key", ["field1"]).Converter([-2L])[0].Exists),
            () => Assert.True(Request.HashTimeToLiveAsync("key", ["field1"]).Converter([60000L])[0].HasTimeToLive),
            () => Assert.False(Request.HashTimeToLiveAsync("key", ["field1"]).Converter([-1L])[0].HasTimeToLive),
            () => Assert.False(Request.HashTimeToLiveAsync("key", ["field1"]).Converter([-2L])[0].Exists),

            // List Commands converters
            () => Assert.Equal(["key", "value"], Request.ListBlockingLeftPopAsync(["key"], TimeSpan.FromSeconds(1)).Converter([(gs)"key", (gs)"value"])!),
            () => Assert.Null(Request.ListBlockingLeftPopAsync(["key"], TimeSpan.FromSeconds(1)).Converter(null!)),
            () => Assert.Equal(["list1", "element"], Request.ListBlockingRightPopAsync(["list1", "list2"], TimeSpan.FromSeconds(5)).Converter([(gs)"list1", (gs)"element"])!),
            () => Assert.Null(Request.ListBlockingRightPopAsync(["key"], TimeSpan.Zero).Converter(null!)),
            () => Assert.Equal("moved_value", Request.ListBlockingMoveAsync("src", "dest", ListSide.Left, ListSide.Right, TimeSpan.FromSeconds(2)).Converter("moved_value")),
            () => Assert.Equal(ValkeyValue.Null, Request.ListBlockingMoveAsync("src", "dest", ListSide.Left, ListSide.Right, TimeSpan.FromSeconds(2)).Converter(null!)),
            () => Assert.True(Request.ListBlockingPopAsync(["key"], ListSide.Left, TimeSpan.FromSeconds(1)).Converter(null!).IsNull),
            () => Assert.True(Request.ListBlockingPopAsync(["key"], ListSide.Left, 2, TimeSpan.FromSeconds(1)).Converter(null!).IsNull),
            () => Assert.False(Request.ListBlockingPopAsync(["mylist"], ListSide.Left, TimeSpan.FromSeconds(1)).Converter(new() { { (GlideString)"mylist", new object[] { (GlideString)"value1" } } }).IsNull),
            () => Assert.False(Request.ListBlockingPopAsync(["list2"], ListSide.Right, 3, TimeSpan.FromSeconds(2)).Converter(new() { { (GlideString)"list2", new object[] { (GlideString)"elem1", (GlideString)"elem2" } } }).IsNull),
            () => Assert.True(Request.ListBlockingPopAsync(["key"], ListSide.Left, TimeSpan.FromSeconds(1)).Converter([]).IsNull),
            () => Assert.True(Request.ListLeftPopAsync(["key1", "key2"], 2).Converter(null!).IsNull),
            () => Assert.True(Request.ListRightPopAsync(["key1", "key2"], 3).Converter(null!).IsNull),
            () => Assert.False(Request.ListLeftPopAsync(["mylist"], 1).Converter(new() { { (GlideString)"mylist", new object[] { (GlideString)"left_value" } } }).IsNull),
            () => Assert.False(Request.ListRightPopAsync(["list2"], 2).Converter(new() { { (GlideString)"list2", new object[] { (GlideString)"right1", (GlideString)"right2" } } }).IsNull),
            () => Assert.True(Request.ListLeftPopAsync(["empty"], 1).Converter([]).IsNull),
            () => Assert.True(Request.ListRightPopAsync(["empty"], 1).Converter([]).IsNull),

            // HyperLogLog Command Converters
            () => Assert.True(Request.HyperLogLogAddAsync("key", "element").Converter(true)),
            () => Assert.False(Request.HyperLogLogAddAsync("key", "element").Converter(false)),
            () => Assert.True(Request.HyperLogLogAddAsync("key", ["element1", "element2"]).Converter(true)),
            () => Assert.False(Request.HyperLogLogAddAsync("key", ["element1", "element2"]).Converter(false)),
            () => Assert.Equal(42L, Request.HyperLogLogLengthAsync("key").Converter(42L)),
            () => Assert.Equal(0L, Request.HyperLogLogLengthAsync("key").Converter(0L)),
            () => Assert.Equal(100L, Request.HyperLogLogLengthAsync(["key1", "key2"]).Converter(100L)),
            () => Assert.Equal(ValkeyValue.Ok, Request.HyperLogLogMergeAsync("dest", "src1", "src2").Converter("OK")),
            () => Assert.Equal(ValkeyValue.Ok, Request.HyperLogLogMergeAsync("dest", ["src1", "src2"]).Converter("OK")),

            // Transaction Commands
            () => Assert.Equal(["WATCH", "key1"], Request.Watch(["key1"]).GetArgs()),
            () => Assert.Equal(["WATCH", "key1", "key2", "key3"], Request.Watch(["key1", "key2", "key3"]).GetArgs()),
            () => Assert.Equal(["UNWATCH"], Request.Unwatch().GetArgs()),
            () => Assert.Equal("OK", Request.Watch(["key1"]).Converter("OK")),
            () => Assert.Equal("ERROR", Request.Watch(["key1"]).Converter("ERROR")),
            () => Assert.Equal("OK", Request.Unwatch().Converter("OK")),
            () => Assert.Equal("ERROR", Request.Unwatch().Converter("ERROR")),

            // Bitmap Command Converters
            () => Assert.True(Request.GetBitAsync("key", 0).Converter(1L)),
            () => Assert.False(Request.GetBitAsync("key", 0).Converter(0L)),
            () => Assert.True(Request.SetBitAsync("key", 0, true).Converter(1L)),
            () => Assert.False(Request.SetBitAsync("key", 0, false).Converter(0L)),
            () => Assert.Equal(26L, Request.BitCountAsync("key", 0, -1, BitmapIndexType.Byte).Converter(26L)),
            () => Assert.Equal(0L, Request.BitCountAsync("key", 0, -1, BitmapIndexType.Byte).Converter(0L)),
            () => Assert.Equal(2L, Request.BitPosAsync("key", true, 0, -1, BitmapIndexType.Byte).Converter(2L)),
            () => Assert.Equal(-1L, Request.BitPosAsync("key", true, 0, -1, BitmapIndexType.Byte).Converter(-1L)),
            () => Assert.Equal(6L, Request.BitOpAsync(Bitwise.And, "dest", ["key1", "key2"]).Converter(6L)),
            () => Assert.Equal(0L, Request.BitOpAsync(Bitwise.Or, "dest", ["key1", "key2"]).Converter(0L)),
            () => Assert.Equal([65L, null, 100L], Request.BitFieldAsync("key", [new BitFieldOptions.BitFieldGet(BitFieldOptions.Encoding.Unsigned(8), new BitFieldOptions.BitOffset(0)), new BitFieldOptions.BitFieldSet(BitFieldOptions.Encoding.Unsigned(8), new BitFieldOptions.BitOffset(0), 100)]).Converter([65L, null!, 100L])),
            () => Assert.Equal([65L, 4L], Request.BitFieldReadOnlyAsync("key", [new BitFieldOptions.BitFieldGet(BitFieldOptions.Encoding.Unsigned(8), new BitFieldOptions.BitOffset(0)), new BitFieldOptions.BitFieldGet(BitFieldOptions.Encoding.Unsigned(4), new BitFieldOptions.BitOffset(0))]).Converter([65L, 4L]))
        );

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
                var result = Request.StringGetMultiple(["key1", "key2", "key3"]).Converter(mgetResponse);
                Assert.Equal(3, result.Length);
                Assert.Equal(new ValkeyValue("value1"), result[0]);
                Assert.Equal(ValkeyValue.Null, result[1]);
                Assert.Equal(new ValkeyValue("value3"), result[2]);
            },

            () =>
            {
                // Test empty MGET response
                var emptyResult = Request.StringGetMultiple([]).Converter([]);
                Assert.Empty(emptyResult);
            },

            () =>
            {
                // Test MGET with all null values
                var allNullResponse = new object[] { null!, null! };
                var result = Request.StringGetMultiple(["key1", "key2"]).Converter(allNullResponse);
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
                var result = Request.SetMembersAsync("key").Converter(testHashSet);
                Assert.Equal(3, result.Length);
                Assert.All(result, item => Assert.IsType<ValkeyValue>(item));
            },

            () => {
                var result = Request.SetPopAsync("key", 2).Converter(testHashSet);
                Assert.Equal(3, result.Length);
                Assert.All(result, item => Assert.IsType<ValkeyValue>(item));
            },

            () => {
                var result = Request.SetUnionAsync(["key1", "key2"]).Converter(testHashSet);
                Assert.Equal(3, result.Length);
                Assert.All(result, item => Assert.IsType<ValkeyValue>(item));
            },

            () => {
                var result = Request.SetIntersectAsync(["key1", "key2"]).Converter(testHashSet);
                Assert.Equal(3, result.Length);
                Assert.All(result, item => Assert.IsType<ValkeyValue>(item));
            },

            () => {
                var result = Request.SetDifferenceAsync(["key1", "key2"]).Converter(testHashSet);
                Assert.Equal(3, result.Length);
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

        // Test for HashGetAllAsync and HashRandomFieldsWithValuesAsync
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
                var result = Request.HashGetAsync("key", ["field1", "field2", "field3"]).Converter((object[])testList.ToArray()!);
                Assert.Equal(3, result.Length);
                Assert.Equal("value1", result[0]);
                Assert.Equal("value2", result[1]);
                Assert.Equal(ValkeyValue.Null, result[2]);
            },

            // Test HashGetAllAsync
            () =>
            {
                var result = Request.HashGetAllAsync("key").Converter(testKvpList);
                Assert.Equal(3, result.Count);
                Assert.Equal("value1", result["field1"]);
            },

            // Test HashValuesAsync
            () =>
            {
                var result = Request.HashValuesAsync("key").Converter(testObjectArray);
                Assert.Equal(3, result.Count);
                foreach (var item in result) _ = Assert.IsType<ValkeyValue>(item);
            },

            // Test HashRandomFieldAsync
            () =>
            {
                var result = Request.HashRandomFieldAsync("key").Converter("field1");
                Assert.Equal("field1", result);
            },

            // Test HashRandomFieldsAsync
            () =>
            {
                var result = Request.HashRandomFieldsAsync("key", 3).Converter(testObjectArray);
                Assert.Equal(3, result.Length);
                foreach (var item in result) _ = Assert.IsType<ValkeyValue>(item);
            },

            // Test HashRandomFieldsWithValuesAsync
            () =>
            {
                var result = Request.HashRandomFieldsWithValuesAsync("key", 3).Converter(testObjectNestedArray);
                Assert.Equal(3, result.Count);
                foreach (var entry in result)
                {
                    _ = Assert.IsType<ValkeyValue>(entry.Key);
                    _ = Assert.IsType<ValkeyValue>(entry.Value);
                }
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
            () => Assert.Equal(["XREAD", "STREAMS", "key", "0-0"], Request.StreamReadAsync("key", "0-0", null).GetArgs()),
            () => Assert.Equal(["XREAD", "COUNT", "10", "STREAMS", "key", "0-0"], Request.StreamReadAsync("key", "0-0", 10).GetArgs()),
            () => Assert.Equal(["XREAD", "STREAMS", "key1", "key2", "0-0", "1-0"], Request.StreamReadAsync([new StreamPosition("key1", "0-0"), new StreamPosition("key2", "1-0")], null).GetArgs()),

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

            // StreamCreateConsumer
            () => Assert.Equal(["XGROUPCREATECONSUMER", "key", "group", "consumer"], Request.StreamCreateConsumerAsync("key", "group", "consumer").GetArgs()),

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
            () => Assert.Equal(["XPENDING", "key", "group", "IDLE", "1000", "-", "+", "10", "consumer"], Request.StreamPendingMessagesAsync("key", "group", "-", "+", 10, "consumer", TimeSpan.FromMilliseconds(1000)).GetArgs()),

            // StreamClaim
            () => Assert.Equal(["XCLAIM", "key", "group", "consumer", "1000", "1-0"], Request.StreamClaimAsync("key", "group", "consumer", TimeSpan.FromMilliseconds(1000), ["1-0"], null, null, null, false).GetArgs()),
            () => Assert.Equal(["XCLAIM", "key", "group", "consumer", "1000", "1-0", "IDLE", "500"], Request.StreamClaimAsync("key", "group", "consumer", TimeSpan.FromMilliseconds(1000), ["1-0"], TimeSpan.FromMilliseconds(500), null, null, false).GetArgs()),
            () => Assert.Equal(["XCLAIM", "key", "group", "consumer", "1000", "1-0", "FORCE"], Request.StreamClaimAsync("key", "group", "consumer", TimeSpan.FromMilliseconds(1000), ["1-0"], null, null, null, true).GetArgs()),

            // StreamClaimIdsOnly
            () => Assert.Equal(["XCLAIM", "key", "group", "consumer", "1000", "1-0", "JUSTID"], Request.StreamClaimIdsOnlyAsync("key", "group", "consumer", TimeSpan.FromMilliseconds(1000), ["1-0"], null, null, null, false).GetArgs()),

            // StreamAutoClaim
            () => Assert.Equal(["XAUTOCLAIM", "key", "group", "consumer", "1000", "0-0"], Request.StreamAutoClaimAsync("key", "group", "consumer", TimeSpan.FromMilliseconds(1000), "0-0", null).GetArgs()),
            () => Assert.Equal(["XAUTOCLAIM", "key", "group", "consumer", "1000", "0-0", "COUNT", "10"], Request.StreamAutoClaimAsync("key", "group", "consumer", TimeSpan.FromMilliseconds(1000), "0-0", 10).GetArgs()),

            // StreamAutoClaimIdsOnly
            () => Assert.Equal(["XAUTOCLAIM", "key", "group", "consumer", "1000", "0-0", "JUSTID"], Request.StreamAutoClaimIdsOnlyAsync("key", "group", "consumer", TimeSpan.FromMilliseconds(1000), "0-0", null).GetArgs()),

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

            // StreamCreateConsumer
            () => Assert.True(Request.StreamCreateConsumerAsync("key", "group", "consumer").Converter(1L)),
            () => Assert.False(Request.StreamCreateConsumerAsync("key", "group", "consumer").Converter(0L)),
            () => Assert.True(Request.StreamCreateConsumerAsync("key", "group", "consumer").Converter(true)),
            () => Assert.False(Request.StreamCreateConsumerAsync("key", "group", "consumer").Converter(false)),

            // StreamDeleteConsumer
            () => Assert.Equal(5L, Request.StreamDeleteConsumerAsync("key", "group", "consumer").Converter(5L)),

            // StreamConsumerGroupSetPosition
            () => Assert.True(Request.StreamConsumerGroupSetPositionAsync("key", "group", "0-0", null).Converter("OK")),

            // StreamAcknowledge
            () => Assert.Equal(2L, Request.StreamAcknowledgeAsync("key", "group", ["1-0", "2-0"]).Converter(2L))
        );
    }
}
