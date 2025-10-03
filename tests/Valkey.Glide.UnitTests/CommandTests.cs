// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands.Options;
using Valkey.Glide.Internals;

using Xunit;

namespace Valkey.Glide.UnitTests;

public class CommandTests
{
    [Fact]
    public void ValidateCommandArgs()
    {
        Assert.Multiple(
            () => Assert.Equal(["get", "a"], Request.CustomCommand(["get", "a"]).GetArgs()),
            () => Assert.Equal(["ping", "pong", "pang"], Request.CustomCommand(["ping", "pong", "pang"]).GetArgs()),
            () => Assert.Equal(["get"], Request.CustomCommand(["get"]).GetArgs()),
            () => Assert.Equal([], Request.CustomCommand([]).GetArgs()),

            // String Commands
            () => Assert.Equal(["SET", "key", "value"], Request.StringSet("key", "value").GetArgs()),
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
            () => Assert.Equal(["GETEX", "key", "EX", "60"], Request.StringGetSetExpiry("key", TimeSpan.FromSeconds(60)).GetArgs()),
            () => Assert.Equal(["GETEX", "test_key", "EX", "60"], Request.StringGetSetExpiry("test_key", TimeSpan.FromSeconds(60)).GetArgs()),
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
            () => Assert.Equal(["SMISMEMBER", "key"], Request.SetContainsAsync("key", Array.Empty<ValkeyValue>()).GetArgs()),
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
            () => Assert.Equal(["SSCAN", "key", "0", "MATCH", "pattern*"], Request.SetScanAsync("key", 0, "pattern*").GetArgs()),
            () => Assert.Equal(["SSCAN", "key", "5", "MATCH", "test*"], Request.SetScanAsync("key", 5, "test*").GetArgs()),
            () => Assert.Equal(["SSCAN", "key", "0", "MATCH", "*suffix"], Request.SetScanAsync("key", 0, "*suffix").GetArgs()),
            () => Assert.Equal(["SSCAN", "key", "0", "COUNT", "10"], Request.SetScanAsync("key", 0, default, 10).GetArgs()),
            () => Assert.Equal(["SSCAN", "key", "5", "COUNT", "20"], Request.SetScanAsync("key", 5, default, 20).GetArgs()),
            () => Assert.Equal(["SSCAN", "key", "0", "COUNT", "1"], Request.SetScanAsync("key", 0, default, 1).GetArgs()),
            () => Assert.Equal(["SSCAN", "key", "0", "MATCH", "pattern*", "COUNT", "10"], Request.SetScanAsync("key", 0, "pattern*", 10).GetArgs()),
            () => Assert.Equal(["SSCAN", "key", "5", "MATCH", "test*", "COUNT", "20"], Request.SetScanAsync("key", 5, "test*", 20).GetArgs()),
            () => Assert.Equal(["SSCAN", "key", "10", "MATCH", "*suffix", "COUNT", "5"], Request.SetScanAsync("key", 10, "*suffix", 5).GetArgs()),
            () => Assert.Equal(["SISMEMBER", "", "member"], Request.SetContainsAsync("", "member").GetArgs()),
            () => Assert.Equal(["SISMEMBER", "key", ""], Request.SetContainsAsync("key", "").GetArgs()),
            () => Assert.Equal(["SMOVE", "", "", ""], Request.SetMoveAsync("", "", "").GetArgs()),
            () => Assert.Equal(["SSCAN", "", "0"], Request.SetScanAsync("", 0).GetArgs()),

            // Generic Commands
            () => Assert.Equal(["DEL", "key"], Request.KeyDeleteAsync("key").GetArgs()),
            () => Assert.Equal(["DEL", "key1", "key2"], Request.KeyDeleteAsync(["key1", "key2"]).GetArgs()),
            () => Assert.Equal(["UNLINK", "key"], Request.KeyUnlinkAsync("key").GetArgs()),
            () => Assert.Equal(["UNLINK", "key1", "key2"], Request.KeyUnlinkAsync(["key1", "key2"]).GetArgs()),
            () => Assert.Equal(["EXISTS", "key"], Request.KeyExistsAsync("key").GetArgs()),
            () => Assert.Equal(["EXISTS", "key1", "key2"], Request.KeyExistsAsync(["key1", "key2"]).GetArgs()),
            () => Assert.Equal(["EXPIRE", "key", "60"], Request.KeyExpireAsync("key", TimeSpan.FromSeconds(60)).GetArgs()),
            () => Assert.Equal(["EXPIRE", "key", "60", "NX"], Request.KeyExpireAsync("key", TimeSpan.FromSeconds(60), ExpireWhen.HasNoExpiry).GetArgs()),
            () => Assert.Equal(["EXPIREAT", "key", "1609459200"], Request.KeyExpireAsync("key", new DateTime(2021, 1, 1, 0, 0, 0, DateTimeKind.Utc)).GetArgs()),
            () => Assert.Equal(["PTTL", "key"], Request.KeyTimeToLiveAsync("key").GetArgs()),
            () => Assert.Equal(["TYPE", "key"], Request.KeyTypeAsync("key").GetArgs()),
            () => Assert.Equal(["RENAME", "oldkey", "newkey"], Request.KeyRenameAsync("oldkey", "newkey").GetArgs()),
            () => Assert.Equal(["RENAMENX", "oldkey", "newkey"], Request.KeyRenameNXAsync("oldkey", "newkey").GetArgs()),
            () => Assert.Equal(["PERSIST", "key"], Request.KeyPersistAsync("key").GetArgs()),
            () => Assert.Equal(["DUMP", "key"], Request.KeyDumpAsync("key").GetArgs()),
            () => Assert.Equal(["RESTORE", "key", "0", "data"], Request.KeyRestoreAsync("key", "data"u8.ToArray()).GetArgs()),
            () => Assert.Equal(["RESTORE", "key", "0", "data", "ABSTTL"], Request.KeyRestoreDateTimeAsync("key", "data"u8.ToArray()).GetArgs()),
            () => Assert.Equal(["RESTORE", "key", "5000", "data"], Request.KeyRestoreAsync("key", "data"u8.ToArray(), TimeSpan.FromSeconds(5)).GetArgs()),
            () => Assert.Equal(["RESTORE", "key", "2303596800000", "data", "ABSTTL"], Request.KeyRestoreDateTimeAsync("key", "data"u8.ToArray(), new DateTime(2042, 12, 31, 0, 0, 0, DateTimeKind.Utc)).GetArgs()),
            () => Assert.Equal(["RESTORE", "key", "0", "data", "REPLACE"], Request.KeyRestoreAsync("key", "data"u8.ToArray(), restoreOptions: new RestoreOptions().Replace()).GetArgs()),
            () => Assert.Equal(["RESTORE", "key", "0", "data", "IDLETIME", "1000"], Request.KeyRestoreAsync("key", "data"u8.ToArray(), restoreOptions: new RestoreOptions().SetIdletime(1000)).GetArgs()),
            () => Assert.Equal(["RESTORE", "key", "0", "data", "FREQ", "5"], Request.KeyRestoreAsync("key", "data"u8.ToArray(), restoreOptions: new RestoreOptions().SetFrequency(5)).GetArgs()),
            () => Assert.Equal(["RESTORE", "key", "0", "data", "REPLACE", "IDLETIME", "2000"], Request.KeyRestoreAsync("key", "data"u8.ToArray(), restoreOptions: new RestoreOptions().Replace().SetIdletime(2000)).GetArgs()),
            () => Assert.Equal(["RESTORE", "key", "0", "data", "REPLACE", "FREQ", "10"], Request.KeyRestoreAsync("key", "data"u8.ToArray(), restoreOptions: new RestoreOptions().Replace().SetFrequency(10)).GetArgs()),
            () => Assert.Equal(["RESTORE", "key", "0", "data", "ABSTTL"], Request.KeyRestoreDateTimeAsync("key", "data"u8.ToArray(), restoreOptions: new RestoreOptions()).GetArgs()),
            () => Assert.Equal(["RESTORE", "key", "0", "data", "ABSTTL", "IDLETIME", "2000"], Request.KeyRestoreDateTimeAsync("key", "data"u8.ToArray(), restoreOptions: new RestoreOptions().SetIdletime(2000)).GetArgs()),
            () => Assert.Equal(["RESTORE", "key", "0", "data", "ABSTTL", "FREQ", "10"], Request.KeyRestoreDateTimeAsync("key", "data"u8.ToArray(), restoreOptions: new RestoreOptions().SetFrequency(10)).GetArgs()),
            () => Assert.Equal(["RESTORE", "key", "0", "data", "ABSTTL", "REPLACE", "IDLETIME", "3000"], Request.KeyRestoreDateTimeAsync("key", "data"u8.ToArray(), restoreOptions: new RestoreOptions().Replace().SetIdletime(3000)).GetArgs()),
            () => Assert.Equal(["RESTORE", "key", "0", "data", "ABSTTL", "REPLACE", "FREQ", "20"], Request.KeyRestoreDateTimeAsync("key", "data"u8.ToArray(), restoreOptions: new RestoreOptions().Replace().SetFrequency(20)).GetArgs()),
            () => Assert.Equal(["RESTORE", "key", "2303596800000", "data", "ABSTTL", "REPLACE"], Request.KeyRestoreDateTimeAsync("key", "data"u8.ToArray(), new DateTime(2042, 12, 31, 0, 0, 0, DateTimeKind.Utc), new RestoreOptions().Replace()).GetArgs()),
            () => Assert.Throws<ArgumentException>(() => Request.KeyRestoreAsync("key", "data"u8.ToArray(), restoreOptions: new RestoreOptions().SetIdletime(1000).SetFrequency(5)).GetArgs()),
            () => Assert.Equal(["TOUCH", "key"], Request.KeyTouchAsync("key").GetArgs()),
            () => Assert.Equal(["TOUCH", "key1", "key2"], Request.KeyTouchAsync(["key1", "key2"]).GetArgs()),
            () => Assert.Equal(["COPY", "src", "dest"], Request.KeyCopyAsync("src", "dest").GetArgs()),
            () => Assert.Equal(["COPY", "src", "dest", "DB", "1", "REPLACE"], Request.KeyCopyAsync("src", "dest", 1, true).GetArgs()),
            () => Assert.Equal(["PEXPIRETIME", "key"], Request.KeyExpireTimeAsync("key").GetArgs()),
            () => Assert.Equal(["OBJECT", "ENCODING", "key"], Request.KeyEncodingAsync("key").GetArgs()),
            () => Assert.Equal(["OBJECT", "FREQ", "key"], Request.KeyFrequencyAsync("key").GetArgs()),
            () => Assert.Equal(["OBJECT", "IDLETIME", "key"], Request.KeyIdleTimeAsync("key").GetArgs()),
            () => Assert.Equal(["OBJECT", "REFCOUNT", "key"], Request.KeyRefCountAsync("key").GetArgs()),
            () => Assert.Equal(["RANDOMKEY"], Request.KeyRandomAsync().GetArgs()),
            () => Assert.Equal(["MOVE", "key", "1"], Request.KeyMoveAsync("key", 1).GetArgs()),

            // SCAN Commands
            () => Assert.Equal(["SCAN", "0"], Request.ScanAsync(0).GetArgs()),
            () => Assert.Equal(["SCAN", "10"], Request.ScanAsync(10).GetArgs()),
            () => Assert.Equal(["SCAN", "0", "MATCH", "pattern*"], Request.ScanAsync(0, "pattern*").GetArgs()),
            () => Assert.Equal(["SCAN", "5", "MATCH", "test*"], Request.ScanAsync(5, "test*").GetArgs()),
            () => Assert.Equal(["SCAN", "0", "COUNT", "10"], Request.ScanAsync(0, pageSize: 10).GetArgs()),
            () => Assert.Equal(["SCAN", "5", "COUNT", "20"], Request.ScanAsync(5, pageSize: 20).GetArgs()),
            () => Assert.Equal(["SCAN", "0", "MATCH", "pattern*", "COUNT", "10"], Request.ScanAsync(0, "pattern*", 10).GetArgs()),
            () => Assert.Equal(["SCAN", "10", "MATCH", "*suffix", "COUNT", "5"], Request.ScanAsync(10, "*suffix", 5).GetArgs()),

            // WAIT Commands
            () => Assert.Equal(["WAIT", "1", "1000"], Request.WaitAsync(1, 1000).GetArgs()),
            () => Assert.Equal(["WAIT", "0", "0"], Request.WaitAsync(0, 0).GetArgs()),
            () => Assert.Equal(["WAIT", "3", "5000"], Request.WaitAsync(3, 5000).GetArgs()),

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
            () => Assert.Equal(new string[] { "HMGET", "key", "field1", "field2" }, Request.HashGetAsync("key", new ValkeyValue[] { "field1", "field2" }).GetArgs()),
            () => Assert.Equal(new string[] { "HGETALL", "key" }, Request.HashGetAllAsync("key").GetArgs()),
            () => Assert.Equal(new string[] { "HMSET", "key", "field1", "value1", "field2", "value2" }, Request.HashSetAsync("key", new HashEntry[] { new HashEntry("field1", "value1"), new HashEntry("field2", "value2") }).GetArgs()),
            () => Assert.Equal(new string[] { "HMSET", "key", "field", "value" }, Request.HashSetAsync("key", new HashEntry[] { new HashEntry("field", "value") }).GetArgs()),
            () => Assert.Equal(new string[] { "HDEL", "key", "field" }, Request.HashDeleteAsync("key", "field").GetArgs()),
            () => Assert.Equal(new string[] { "HDEL", "key", "field1", "field2" }, Request.HashDeleteAsync("key", new ValkeyValue[] { "field1", "field2" }).GetArgs()),
            () => Assert.Equal(new string[] { "HEXISTS", "key", "field" }, Request.HashExistsAsync("key", "field").GetArgs()),
            () => Assert.Equal(new string[] { "HINCRBY", "key", "field", "5" }, Request.HashIncrementAsync("key", "field", 5L).GetArgs()),
            () => Assert.Equal(new string[] { "HINCRBY", "key", "field", "1" }, Request.HashIncrementAsync("key", "field", 1L).GetArgs()),
            () => Assert.Equal(new string[] { "HINCRBYFLOAT", "key", "field", "2.5" }, Request.HashIncrementAsync("key", "field", 2.5).GetArgs()),
            () => Assert.Equal(new string[] { "HKEYS", "key" }, Request.HashKeysAsync("key").GetArgs()),
            () => Assert.Equal(new string[] { "HLEN", "key" }, Request.HashLengthAsync("key").GetArgs()),
            () => Assert.Equal(new string[] { "HSCAN", "key", "0" }, Request.HashScanAsync<HashEntry>("key", 0, ValkeyValue.Null, 0).GetArgs()),
            () => Assert.Equal(new string[] { "HSCAN", "key", "0", "MATCH", "pattern*", "COUNT", "10" }, Request.HashScanAsync<HashEntry>("key", 0, "pattern*", 10).GetArgs()),
            () => Assert.Equal(new string[] { "HSCAN", "key", "0" }, Request.HashScanAsync<ValkeyValue>("key", 0, ValkeyValue.Null, 0, false).GetArgs()),
            () => Assert.Equal(new string[] { "HSCAN", "key", "0", "MATCH", "pattern*", "COUNT", "10" }, Request.HashScanAsync<ValkeyValue>("key", 0, "pattern*", 10, false).GetArgs()),
            () => Assert.Equal(new string[] { "HSTRLEN", "key", "field" }, Request.HashStringLengthAsync("key", "field").GetArgs()),
            () => Assert.Equal(new string[] { "HVALS", "key" }, Request.HashValuesAsync("key").GetArgs()),
            () => Assert.Equal(new string[] { "HRANDFIELD", "key" }, Request.HashRandomFieldAsync("key").GetArgs()),
            () => Assert.Equal(new string[] { "HRANDFIELD", "key", "3" }, Request.HashRandomFieldsAsync("key", 3).GetArgs()),
            () => Assert.Equal(new string[] { "HRANDFIELD", "key", "3", "WITHVALUES" }, Request.HashRandomFieldsWithValuesAsync("key", 3).GetArgs())
        );
    }

    [Fact]
    public void ValidateCommandConverters()
    {
        Assert.Multiple(
            () => Assert.Equal(1, Request.CustomCommand([]).Converter(1)),
            () => Assert.Equal(.1, Request.CustomCommand([]).Converter(.1)),
            () => Assert.Null(Request.CustomCommand([]).Converter(null)),

            // String Commands
            () => Assert.True(Request.StringSet("key", "value").Converter("OK")),
            () => Assert.Equal<GlideString>("value", Request.StringGet("key").Converter("value")),
            () => Assert.Equal(ValkeyValue.Null, Request.StringGet("key").Converter(null)),
            () => Assert.Equal(5L, Request.StringLength("key").Converter(5L)),
            () => Assert.Equal(0L, Request.StringLength("key").Converter(0L)),
            () => Assert.Equal(new ValkeyValue("hello"), Request.StringGetRange("key", 0, 4).Converter("hello")),
            () => Assert.Equal(new ValkeyValue(""), Request.StringGetRange("key", 0, 4).Converter("")),
            () => Assert.Equal(ValkeyValue.Null, Request.StringGetRange("key", 0, 4).Converter(null)),
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
            () => Assert.True(Request.StringGetDelete("test_key").Converter(null).IsNull),
            () => Assert.Equal("test_value", Request.StringGetSetExpiry("test_key", TimeSpan.FromSeconds(60)).Converter(new GlideString("test_value")).ToString()),
            () => Assert.True(Request.StringGetSetExpiry("test_key", TimeSpan.FromSeconds(60)).Converter(null).IsNull),

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
            () => Assert.Equal(new KeyValuePair<string, string>[] { new("maxmemory", "100mb") }, Request.ConfigGetAsync("maxmemory").Converter(new object[] { (gs)"maxmemory", "100mb" })),
            () => Assert.Equal(Array.Empty<KeyValuePair<string, string>>(), Request.ConfigGetAsync("nonexistent").Converter(Array.Empty<object>())),
            () => Assert.Equal(100L, Request.DatabaseSizeAsync().Converter(100L)),
            () => Assert.Equal(0L, Request.DatabaseSizeAsync().Converter(0L)),
            () => Assert.Equal(DateTime.UnixEpoch.AddSeconds(1609459200), Request.LastSaveAsync().Converter(1609459200L)),
            () => Assert.Equal(DateTime.UnixEpoch.AddSeconds(1609459200).AddTicks(123456 * 10), Request.TimeAsync().Converter(new object[] { "1609459200", "123456" })),
            () => Assert.Equal("Valkey 7.0.0", Request.LolwutAsync().Converter("Valkey 7.0.0")),
            () => Assert.Equal("test_value", Request.StringGetSetExpiry("test_key", new DateTime(2021, 1, 1, 0, 0, 0, DateTimeKind.Utc)).Converter(new GlideString("test_value")).ToString()),
            () => Assert.Equal("common", Request.StringLongestCommonSubsequence("key1", "key2").Converter(new GlideString("common")).ToString()),
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
            () => Assert.IsType<TimeSpan>(Request.Ping().Converter("PONG")),
            () => Assert.IsType<TimeSpan>(Request.Ping("Hello").Converter("Hello")),
            () => Assert.True(Request.Ping().Converter("PONG") > TimeSpan.Zero),
            () => Assert.True(Request.Ping("test").Converter("test") >= TimeSpan.Zero),
            () => Assert.Equal<ValkeyValue>("message", Request.Echo("message").Converter("message")),

            () => Assert.Equal(ValkeyValue.Null, Request.ClientGetName().Converter(null)),
            () => Assert.Equal("test-connection", Request.ClientGetName().Converter(new GlideString("test-connection"))),
            () => Assert.Equal(12345L, Request.ClientId().Converter(12345L)),
            () => Assert.Equal("OK", Request.Select(0).Converter("OK")),

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
            () => Assert.Null(Request.SetPopAsync("key").Converter(null)),

            // Generic Commands Converters
            () => Assert.True(Request.KeyDeleteAsync("key").Converter(1L)),
            () => Assert.False(Request.KeyDeleteAsync("key").Converter(0L)),
            () => Assert.Equal(2L, Request.KeyDeleteAsync(["key1", "key2"]).Converter(2L)),
            () => Assert.True(Request.KeyUnlinkAsync("key").Converter(1L)),
            () => Assert.False(Request.KeyUnlinkAsync("key").Converter(0L)),
            () => Assert.Equal(3L, Request.KeyUnlinkAsync(["key1", "key2", "key3"]).Converter(3L)),
            () => Assert.True(Request.KeyExistsAsync("key").Converter(1L)),
            () => Assert.False(Request.KeyExistsAsync("key").Converter(0L)),
            () => Assert.Equal(2L, Request.KeyExistsAsync(["key1", "key2"]).Converter(2L)),
            () => Assert.True(Request.KeyExpireAsync("key", TimeSpan.FromSeconds(60)).Converter(true)),
            () => Assert.False(Request.KeyExpireAsync("key", TimeSpan.FromSeconds(60)).Converter(false)),
            () => Assert.Equal(TimeSpan.FromMilliseconds(30), Request.KeyTimeToLiveAsync("key").Converter(30L)),
            () => Assert.Null(Request.KeyTimeToLiveAsync("key").Converter(-1L)),
            () => Assert.Null(Request.KeyTimeToLiveAsync("key").Converter(-2L)),
            () => Assert.Equal(ValkeyType.String, Request.KeyTypeAsync("key").Converter("string")),
            () => Assert.Equal(ValkeyType.List, Request.KeyTypeAsync("key").Converter("list")),
            () => Assert.Equal(ValkeyType.Set, Request.KeyTypeAsync("key").Converter("set")),
            () => Assert.Equal(ValkeyType.Hash, Request.KeyTypeAsync("key").Converter("hash")),
            () => Assert.Equal(ValkeyType.Stream, Request.KeyTypeAsync("key").Converter("stream")),
            () => Assert.Equal(ValkeyType.None, Request.KeyTypeAsync("key").Converter("none")),
            () => Assert.True(Request.KeyRenameAsync("oldkey", "newkey").Converter("OK")),
            () => Assert.True(Request.KeyRenameNXAsync("oldkey", "newkey").Converter(true)),
            () => Assert.False(Request.KeyRenameNXAsync("oldkey", "newkey").Converter(false)),
            () => Assert.True(Request.KeyPersistAsync("key").Converter(true)),
            () => Assert.False(Request.KeyPersistAsync("key").Converter(false)),
            () => Assert.NotNull(Request.KeyDumpAsync("key").Converter("dumpdata")),
            () => Assert.Null(Request.KeyDumpAsync("key").Converter(null)),
            () => Assert.Equal("OK", Request.KeyRestoreAsync("key", new byte[0]).Converter("OK")),
            () => Assert.Equal("OK", Request.KeyRestoreDateTimeAsync("key", new byte[0]).Converter("OK")),
            () => Assert.True(Request.KeyTouchAsync("key").Converter(1L)),
            () => Assert.False(Request.KeyTouchAsync("key").Converter(0L)),
            () => Assert.Equal(2L, Request.KeyTouchAsync(["key1", "key2"]).Converter(2L)),
            () => Assert.True(Request.KeyCopyAsync("src", "dest").Converter(true)),
            () => Assert.False(Request.KeyCopyAsync("src", "dest").Converter(false)),
            () => Assert.Equal(new DateTime(2021, 1, 1, 0, 0, 0, DateTimeKind.Utc), Request.KeyExpireTimeAsync("key").Converter(1609459200000L)),
            () => Assert.Null(Request.KeyExpireTimeAsync("key").Converter(-1L)),
            () => Assert.Null(Request.KeyExpireTimeAsync("key").Converter(-2L)),
            () => Assert.Equal("embstr", Request.KeyEncodingAsync("key").Converter(new GlideString("embstr"))),
            () => Assert.Null(Request.KeyEncodingAsync("key").Converter(null)),
            () => Assert.Equal(5L, Request.KeyFrequencyAsync("key").Converter(5L)),
            () => Assert.Null(Request.KeyFrequencyAsync("key").Converter(-1L)),
            () => Assert.Equal(10L, Request.KeyIdleTimeAsync("key").Converter(10L)),
            () => Assert.Null(Request.KeyIdleTimeAsync("key").Converter(-1L)),
            () => Assert.Equal(3L, Request.KeyRefCountAsync("key").Converter(3L)),
            () => Assert.Null(Request.KeyRefCountAsync("key").Converter(-1L)),
            () => Assert.Equal("randomkey", Request.KeyRandomAsync().Converter(new GlideString("randomkey"))),
            () => Assert.Null(Request.KeyRandomAsync().Converter(null)),
            () => Assert.True(Request.KeyMoveAsync("key", 1).Converter(true)),
            () => Assert.False(Request.KeyMoveAsync("key", 1).Converter(false)),

            // SCAN Commands Converters
            () =>
            {
                var result = Request.ScanAsync(0).Converter(new object[] { 0L, new object[] { (gs)"key1", (gs)"key2" } });
                Assert.Equal(0L, result.Item1);
                Assert.Equal(["key1", "key2"], result.Item2.Select(k => k.ToString()).ToArray());
            },
            () =>
            {
                var result = Request.ScanAsync(10).Converter(new object[] { 5L, new object[] { (gs)"test" } });
                Assert.Equal(5L, result.Item1);
                Assert.Equal(["test"], result.Item2.Select(k => k.ToString()).ToArray());
            },
            () =>
            {
                var result = Request.ScanAsync(0).Converter(new object[] { 0L, Array.Empty<object>() });
                Assert.Equal(0L, result.Item1);
                Assert.Empty(result.Item2);
            },

            // WAIT Commands Converters
            () => Assert.Equal(2L, Request.WaitAsync(1, 1000).Converter(2L)),
            () => Assert.Equal(0L, Request.WaitAsync(0, 0).Converter(0L)),
            () => Assert.Equal(1L, Request.WaitAsync(3, 5000).Converter(1L)),

            () => Assert.Equal("one", Request.ListLeftPopAsync("a").Converter("one")),
            () => Assert.Equal(["one", "two"], Request.ListLeftPopAsync("a", 2).Converter([(gs)"one", (gs)"two"])),
            () => Assert.Null(Request.ListLeftPopAsync("a", 2).Converter(null)),
            () => Assert.Equal(ValkeyValue.Null, Request.ListLeftPopAsync("a").Converter(null)),
            () => Assert.Equal(1L, Request.ListLeftPushAsync("a", "value").Converter(1L)),
            () => Assert.Equal(2L, Request.ListLeftPushAsync("a", ["one", "two"]).Converter(2L)),
            () => Assert.Equal("three", Request.ListRightPopAsync("a").Converter("three")),
            () => Assert.Equal(ValkeyValue.Null, Request.ListRightPopAsync("a").Converter(null)),
            () => Assert.Equal(["three", "four"], Request.ListRightPopAsync("a", 2).Converter([(gs)"three", (gs)"four"])),
            () => Assert.Null(Request.ListRightPopAsync("a", 2).Converter(null)),
            () => Assert.Equal(2L, Request.ListRightPushAsync("a", "value").Converter(2L)),
            () => Assert.Equal(3L, Request.ListRightPushAsync("a", ["three", "four"]).Converter(3L)),
            () => Assert.Equal(5L, Request.ListLengthAsync("a").Converter(5L)),
            () => Assert.Equal(0L, Request.ListLengthAsync("nonexistent").Converter(0L)),
            () => Assert.Equal(2L, Request.ListRemoveAsync("a", "value", 0).Converter(2L)),
            () => Assert.Equal(1L, Request.ListRemoveAsync("a", "value", 1).Converter(1L)),
            () => Assert.Equal(0L, Request.ListRemoveAsync("a", "nonexistent", 0).Converter(0L)),
            () => Assert.Equal("OK", Request.ListTrimAsync("a", 0, 10).Converter("OK")),
            () => Assert.Equal(["one", "two", "three"], Request.ListRangeAsync("a", 0, -1).Converter([(gs)"one", (gs)"two", (gs)"three"])),
            () => Assert.IsType<ValkeyValue[]>(Request.ListRangeAsync("a", 0, -1).Converter([(gs)"one", (gs)"two", (gs)"three"])),
            () => Assert.Equal([], Request.ListRangeAsync("nonexistent", 0, -1).Converter([])),

            // Hash Commands
            () => Assert.Equal<GlideString>("value", Request.HashGetAsync("key", "field").Converter("value")),
            () => Assert.Equal(ValkeyValue.Null, Request.HashGetAsync("key", "field").Converter(null)),
            () => Assert.Equal("OK", Request.HashSetAsync("key", new HashEntry[] { new HashEntry("field", "value") }).Converter("OK")),
            () => Assert.True(Request.HashDeleteAsync("key", "field").Converter(1L)),
            () => Assert.False(Request.HashDeleteAsync("key", "field").Converter(0L)),
            () => Assert.Equal(2L, Request.HashDeleteAsync("key", ["field1", "field2"]).Converter(2L)),
            () => Assert.True(Request.HashExistsAsync("key", "field").Converter(true)),
            () => Assert.False(Request.HashExistsAsync("key", "field").Converter(false)),
            () => Assert.Equal(15L, Request.HashIncrementAsync("key", "field", 5L).Converter(15L)),
            () => Assert.Equal(10L, Request.HashIncrementAsync("key", "field", 1L).Converter(10L)),
            () => Assert.Equal(12.5, Request.HashIncrementAsync("key", "field", 2.5).Converter(12.5)),
            () => Assert.Equal(["field1", "field2"], Request.HashKeysAsync("key").Converter([(gs)"field1", (gs)"field2"])),
            () => Assert.Equal([], Request.HashKeysAsync("nonexistent").Converter([])),
            () => Assert.Equal(5L, Request.HashLengthAsync("key").Converter(5L)),
            () => Assert.Equal(10L, Request.HashStringLengthAsync("key", "field").Converter(10L)),

            // List Commands converters
            () => Assert.Equal(["key", "value"], Request.ListBlockingLeftPopAsync(["key"], TimeSpan.FromSeconds(1)).Converter([(gs)"key", (gs)"value"])),
            () => Assert.Null(Request.ListBlockingLeftPopAsync(["key"], TimeSpan.FromSeconds(1)).Converter(null)),
            () => Assert.Equal(["list1", "element"], Request.ListBlockingRightPopAsync(["list1", "list2"], TimeSpan.FromSeconds(5)).Converter([(gs)"list1", (gs)"element"])),
            () => Assert.Null(Request.ListBlockingRightPopAsync(["key"], TimeSpan.Zero).Converter(null)),
            () => Assert.Equal("moved_value", Request.ListBlockingMoveAsync("src", "dest", ListSide.Left, ListSide.Right, TimeSpan.FromSeconds(2)).Converter("moved_value")),
            () => Assert.Equal(ValkeyValue.Null, Request.ListBlockingMoveAsync("src", "dest", ListSide.Left, ListSide.Right, TimeSpan.FromSeconds(2)).Converter(null)),
            () => Assert.True(Request.ListBlockingPopAsync(["key"], ListSide.Left, TimeSpan.FromSeconds(1)).Converter(null).IsNull),
            () => Assert.True(Request.ListBlockingPopAsync(["key"], ListSide.Left, 2, TimeSpan.FromSeconds(1)).Converter(null).IsNull),
            () => Assert.False(Request.ListBlockingPopAsync(["mylist"], ListSide.Left, TimeSpan.FromSeconds(1)).Converter(new Dictionary<GlideString, object> { { (GlideString)"mylist", new object[] { (GlideString)"value1" } } }).IsNull),
            () => Assert.False(Request.ListBlockingPopAsync(["list2"], ListSide.Right, 3, TimeSpan.FromSeconds(2)).Converter(new Dictionary<GlideString, object> { { (GlideString)"list2", new object[] { (GlideString)"elem1", (GlideString)"elem2" } } }).IsNull),
            () => Assert.True(Request.ListBlockingPopAsync(["key"], ListSide.Left, TimeSpan.FromSeconds(1)).Converter(new Dictionary<GlideString, object>()).IsNull),
            () => Assert.True(Request.ListLeftPopAsync(["key1", "key2"], 2).Converter(null).IsNull),
            () => Assert.True(Request.ListRightPopAsync(["key1", "key2"], 3).Converter(null).IsNull),
            () => Assert.False(Request.ListLeftPopAsync(["mylist"], 1).Converter(new Dictionary<GlideString, object> { { (GlideString)"mylist", new object[] { (GlideString)"left_value" } } }).IsNull),
            () => Assert.False(Request.ListRightPopAsync(["list2"], 2).Converter(new Dictionary<GlideString, object> { { (GlideString)"list2", new object[] { (GlideString)"right1", (GlideString)"right2" } } }).IsNull),
            () => Assert.True(Request.ListLeftPopAsync(["empty"], 1).Converter(new Dictionary<GlideString, object>()).IsNull),
            () => Assert.True(Request.ListRightPopAsync(["empty"], 1).Converter(new Dictionary<GlideString, object>()).IsNull)
        );
    }

    [Fact]
    public void ValidateStringCommandArrayConverters()
    {
        Assert.Multiple(
            () =>
            {
                // Test MGET with GlideString objects (what the server actually returns)
                object[] mgetResponse = [new GlideString("value1"), null, new GlideString("value3")];
                ValkeyValue[] result = Request.StringGetMultiple(["key1", "key2", "key3"]).Converter(mgetResponse);
                Assert.Equal(3, result.Length);
                Assert.Equal(new ValkeyValue("value1"), result[0]);
                Assert.Equal(ValkeyValue.Null, result[1]);
                Assert.Equal(new ValkeyValue("value3"), result[2]);
            },

            () =>
            {
                // Test empty MGET response
                ValkeyValue[] emptyResult = Request.StringGetMultiple([]).Converter([]);
                Assert.Empty(emptyResult);
            },

            () =>
            {
                // Test MGET with all null values
                object[] allNullResponse = [null, null];
                ValkeyValue[] result = Request.StringGetMultiple(["key1", "key2"]).Converter(allNullResponse);
                Assert.Equal(2, result.Length);
                Assert.Equal(ValkeyValue.Null, result[0]);
                Assert.Equal(ValkeyValue.Null, result[1]);
            }
        );
    }

    [Fact]
    public void ValidateSetCommandHashSetConverters()
    {
        HashSet<object> testHashSet = new HashSet<object> {
            (gs)"member1",
            (gs)"member2",
            (gs)"member3"
        };

        Assert.Multiple([
            () => {
                ValkeyValue[] result = Request.SetMembersAsync("key").Converter(testHashSet);
                Assert.Equal(3, result.Length);
                Assert.All(result, item => Assert.IsType<ValkeyValue>(item));
            },

            () => {
                ValkeyValue[] result = Request.SetPopAsync("key", 2).Converter(testHashSet);
                Assert.Equal(3, result.Length);
                Assert.All(result, item => Assert.IsType<ValkeyValue>(item));
            },

            () => {
                ValkeyValue[] result = Request.SetUnionAsync(["key1", "key2"]).Converter(testHashSet);
                Assert.Equal(3, result.Length);
                Assert.All(result, item => Assert.IsType<ValkeyValue>(item));
            },

            () => {
                ValkeyValue[] result = Request.SetIntersectAsync(["key1", "key2"]).Converter(testHashSet);
                Assert.Equal(3, result.Length);
                Assert.All(result, item => Assert.IsType<ValkeyValue>(item));
            },

            () => {
                ValkeyValue[] result = Request.SetDifferenceAsync(["key1", "key2"]).Converter(testHashSet);
                Assert.Equal(3, result.Length);
                Assert.All(result, item => Assert.IsType<ValkeyValue>(item));
            },
        ]);
    }

    [Fact]
    public void ValidateHashCommandConverters()
    {
        // Test for HashGetAsync with multiple fields
        List<object> testList = new List<object> {
            (gs)"value1",
            (gs)"value2",
            null
        };

        // Test for HashGetAllAsync and HashRandomFieldsWithValuesAsync
        Dictionary<GlideString, object> testKvpList = new Dictionary<GlideString, object> {
            {"field1", (gs)"value1" },
            {"field2", (gs)"value2" },
            {"field3", (gs)"value3" },
        };

        object[] testObjectNestedArray = new object[]
         {
            new object[] {(gs)"field1", (gs)"value1" },
            new object[] {(gs)"field2", (gs)"value2" },
            new object[] {(gs)"field3", (gs)"value3" },
         };

        // Test for HashValuesAsync and HashRandomFieldsAsync
        object[] testObjectArray = new object[]
        {
            (gs)"value1",
            (gs)"value2",
            (gs)"value3"
        };

        Assert.Multiple(
            // Test HashGetAsync with multiple fields
            () =>
            {
                ValkeyValue[] result = Request.HashGetAsync("key", new ValkeyValue[] { "field1", "field2", "field3" }).Converter(testList.ToArray());
                Assert.Equal(3, result.Length);
                Assert.Equal("value1", result[0]);
                Assert.Equal("value2", result[1]);
                Assert.Equal(ValkeyValue.Null, result[2]);
            },

            // Test HashGetAllAsync
            () =>
            {
                HashEntry[] result = Request.HashGetAllAsync("key").Converter(testKvpList);
                Assert.Equal(3, result.Length);
                foreach (HashEntry entry in result)
                {
                    Assert.IsType<HashEntry>(entry);
                    Assert.IsType<ValkeyValue>(entry.Name);
                    Assert.IsType<ValkeyValue>(entry.Value);
                }
                Assert.Equal("field1", result[0].Name);
                Assert.Equal("value1", result[0].Value);
            },

            // Test HashValuesAsync
            () =>
            {
                ValkeyValue[] result = Request.HashValuesAsync("key").Converter(testObjectArray);
                Assert.Equal(3, result.Length);
                foreach (ValkeyValue item in result) Assert.IsType<ValkeyValue>(item);
            },

            // Test HashRandomFieldAsync
            () =>
            {
                ValkeyValue result = Request.HashRandomFieldAsync("key").Converter("field1");
                Assert.Equal("field1", result);
            },

            // Test HashRandomFieldsAsync
            () =>
            {
                ValkeyValue[] result = Request.HashRandomFieldsAsync("key", 3).Converter(testObjectArray);
                Assert.Equal(3, result.Length);
                foreach (ValkeyValue item in result) Assert.IsType<ValkeyValue>(item);
            },

            // Test HashRandomFieldsWithValuesAsync
            () =>
            {
                HashEntry[] result = Request.HashRandomFieldsWithValuesAsync("key", 3).Converter(testObjectNestedArray);
                Assert.Equal(3, result.Length);
                foreach (HashEntry entry in result)
                {
                    Assert.IsType<HashEntry>(entry);
                    Assert.IsType<ValkeyValue>(entry.Name);
                    Assert.IsType<ValkeyValue>(entry.Value);
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
}
