// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands.Options;

namespace Valkey.Glide.UnitTests;

public class ClientFilterOptionsTests
{
    [Fact]
    public void ToArgs_Success()
    {
        Assert.Empty(new ClientFilterOptions().ToArgs());
        Assert.Equal(["ID", "1"], new ClientFilterOptions().WithId(1).ToArgs());
        Assert.Equal(["ID", "1", "2", "3"], new ClientFilterOptions().WithIds([1, 2, 3]).ToArgs());
        Assert.Equal(["TYPE", "normal"], new ClientFilterOptions().WithType(ClientType.Normal).ToArgs());
        Assert.Equal(["TYPE", "master"], new ClientFilterOptions().WithType(ClientType.Primary).ToArgs());
        Assert.Equal(["TYPE", "replica"], new ClientFilterOptions().WithType(ClientType.Replica).ToArgs());
        Assert.Equal(["TYPE", "pubsub"], new ClientFilterOptions().WithType(ClientType.PubSub).ToArgs());
        Assert.Equal(["USER", "user"], new ClientFilterOptions().WithUser("user").ToArgs());
        Assert.Equal(["ADDR", "addr:1234"], new ClientFilterOptions().WithAddress("addr", 1234).ToArgs());
        Assert.Equal(["ADDR", "127.0.0.1:1234"], new ClientFilterOptions().WithAddress("127.0.0.1", 1234).ToArgs());
        Assert.Equal(["ADDR", "[::1]:1234"], new ClientFilterOptions().WithAddress("::1", 1234).ToArgs());
        Assert.Equal(["LADDR", "laddr:4321"], new ClientFilterOptions().WithLocalAddress("laddr", 4321).ToArgs());
        Assert.Equal(["LADDR", "127.0.0.1:4321"], new ClientFilterOptions().WithLocalAddress("127.0.0.1", 4321).ToArgs());
        Assert.Equal(["LADDR", "[::1]:4321"], new ClientFilterOptions().WithLocalAddress("::1", 4321).ToArgs());
        Assert.Equal(["SKIPME", "yes"], new ClientFilterOptions().WithSkipMe(true).ToArgs());
        Assert.Equal(["SKIPME", "no"], new ClientFilterOptions().WithSkipMe(false).ToArgs());
        Assert.Equal(["MAXAGE", "1"], new ClientFilterOptions().WithMaxAge(TimeSpan.FromSeconds(1)).ToArgs());
        Assert.Equal(["MAXAGE", "2"], new ClientFilterOptions().WithMaxAge(TimeSpan.FromSeconds(1.5)).ToArgs());
        Assert.Equal(["MAXAGE", "60"], new ClientFilterOptions().WithMaxAge(TimeSpan.FromMinutes(1)).ToArgs());

        // Positive filters (Since Valkey 9.0.0)
        Assert.Equal(["NAME", "myname"], new ClientFilterOptions().WithName("myname").ToArgs());
        Assert.Equal(["IDLE", "1"], new ClientFilterOptions().WithIdle(TimeSpan.FromSeconds(1)).ToArgs());
        Assert.Equal(["IDLE", "2"], new ClientFilterOptions().WithIdle(TimeSpan.FromSeconds(1.5)).ToArgs());
        Assert.Equal(["IDLE", "90"], new ClientFilterOptions().WithIdle(TimeSpan.FromMinutes(1.5)).ToArgs());
        Assert.Equal(["FLAGS", "P"], new ClientFilterOptions().WithFlag(ClientFlag.PubSub).ToArgs());
        Assert.Equal(["FLAGS", "MP"], new ClientFilterOptions().WithFlags([ClientFlag.Primary, ClientFlag.PubSub]).ToArgs());
        Assert.Equal(["FLAGS", "bx"], new ClientFilterOptions().WithFlags("bx").ToArgs());
        Assert.Equal(["FLAGS", "N"], new ClientFilterOptions().WithFlag('N').ToArgs());
        Assert.Equal(["LIB-NAME", "mylib"], new ClientFilterOptions().WithLibraryName("mylib").ToArgs());
        Assert.Equal(["LIB-VER", "1.0.0"], new ClientFilterOptions().WithLibraryVersion("1.0.0").ToArgs());
        Assert.Equal(["DB", "0"], new ClientFilterOptions().WithDatabaseId(0).ToArgs());
        Assert.Equal(["DB", "5"], new ClientFilterOptions().WithDatabaseId(5).ToArgs());
        Assert.Equal(["CAPA", "r"], new ClientFilterOptions().WithCapability(ClientCapability.Redirect).ToArgs());
        Assert.Equal(["IP", "192.168.1.1"], new ClientFilterOptions().WithIpAddress("192.168.1.1").ToArgs());

        // Negative filters (Since Valkey 9.0.0)
        Assert.Equal(["NOT-ID", "1"], new ClientFilterOptions().WithoutId(1).ToArgs());
        Assert.Equal(["NOT-ID", "1", "2", "3"], new ClientFilterOptions().WithoutIds([1, 2, 3]).ToArgs());
        Assert.Equal(["NOT-TYPE", "normal"], new ClientFilterOptions().WithoutType(ClientType.Normal).ToArgs());
        Assert.Equal(["NOT-TYPE", "master"], new ClientFilterOptions().WithoutType(ClientType.Primary).ToArgs());
        Assert.Equal(["NOT-TYPE", "replica"], new ClientFilterOptions().WithoutType(ClientType.Replica).ToArgs());
        Assert.Equal(["NOT-TYPE", "pubsub"], new ClientFilterOptions().WithoutType(ClientType.PubSub).ToArgs());
        Assert.Equal(["NOT-ADDR", "addr:1234"], new ClientFilterOptions().WithoutAddress("addr", 1234).ToArgs());
        Assert.Equal(["NOT-ADDR", "127.0.0.1:1234"], new ClientFilterOptions().WithoutAddress("127.0.0.1", 1234).ToArgs());
        Assert.Equal(["NOT-ADDR", "[::1]:1234"], new ClientFilterOptions().WithoutAddress("::1", 1234).ToArgs());
        Assert.Equal(["NOT-LADDR", "laddr:4321"], new ClientFilterOptions().WithoutLocalAddress("laddr", 4321).ToArgs());
        Assert.Equal(["NOT-LADDR", "127.0.0.1:4321"], new ClientFilterOptions().WithoutLocalAddress("127.0.0.1", 4321).ToArgs());
        Assert.Equal(["NOT-LADDR", "[::1]:4321"], new ClientFilterOptions().WithoutLocalAddress("::1", 4321).ToArgs());
        Assert.Equal(["NOT-USER", "user"], new ClientFilterOptions().WithoutUser("user").ToArgs());
        Assert.Equal(["NOT-FLAGS", "S"], new ClientFilterOptions().WithoutFlag(ClientFlag.Replica).ToArgs());
        Assert.Equal(["NOT-FLAGS", "MP"], new ClientFilterOptions().WithoutFlags("PM").ToArgs());
        Assert.Equal(["NOT-FLAGS", "N"], new ClientFilterOptions().WithoutFlags("N").ToArgs());
        Assert.Equal(["NOT-NAME", "myname"], new ClientFilterOptions().WithoutName("myname").ToArgs());
        Assert.Equal(["NOT-LIB-NAME", "mylib"], new ClientFilterOptions().WithoutLibraryName("mylib").ToArgs());
        Assert.Equal(["NOT-LIB-VER", "1.0.0"], new ClientFilterOptions().WithoutLibraryVersion("1.0.0").ToArgs());
        Assert.Equal(["NOT-DB", "0"], new ClientFilterOptions().WithoutDatabaseId(0).ToArgs());
        Assert.Equal(["NOT-DB", "5"], new ClientFilterOptions().WithoutDatabaseId(5).ToArgs());
        Assert.Equal(["NOT-CAPA", "r"], new ClientFilterOptions().WithoutCapability(ClientCapability.Redirect).ToArgs());
        Assert.Equal(["NOT-IP", "192.168.1.1"], new ClientFilterOptions().WithoutIpAddress("192.168.1.1").ToArgs());

        Assert.Equal(
            ["TYPE", "normal",
             "ID", "42",
             "USER", "user",
             "ADDR", "addr:1234",
             "LADDR", "laddr:4321",
             "SKIPME", "no",
             "MAXAGE", "1"],
            new ClientFilterOptions()
                .WithId(42)
                .WithType(ClientType.Normal)
                .WithUser("user")
                .WithAddress("addr", 1234)
                .WithLocalAddress("laddr", 4321)
                .WithSkipMe(false)
                .WithMaxAge(TimeSpan.FromSeconds(1))
                .ToArgs());

        // All positive filters (Since Valkey 9.0.0)
        Assert.Equal(
            ["TYPE", "normal",
             "ID", "1",
             "USER", "admin",
             "ADDR", "host:1234",
             "LADDR", "local:4321",
             "SKIPME", "yes",
             "NAME", "conn1",
             "IDLE", "5",
             "FLAGS", "N",
             "LIB-NAME", "mylib",
             "LIB-VER", "2.0",
             "DB", "3",
             "CAPA", "r",
             "IP", "10.0.0.1"],
            new ClientFilterOptions()
                .WithId(1)
                .WithType(ClientType.Normal)
                .WithUser("admin")
                .WithAddress("host", 1234)
                .WithLocalAddress("local", 4321)
                .WithSkipMe(true)
                .WithName("conn1")
                .WithIdle(TimeSpan.FromSeconds(5))
                .WithFlag(ClientFlag.None)
                .WithLibraryName("mylib")
                .WithLibraryVersion("2.0")
                .WithDatabaseId(3)
                .WithCapability(ClientCapability.Redirect)
                .WithIpAddress("10.0.0.1")
                .ToArgs());

        // All negative filters (Since Valkey 9.0.0)
        Assert.Equal(
            ["NOT-TYPE", "replica",
             "NOT-ID", "99",
             "NOT-USER", "guest",
             "NOT-ADDR", "badhost:6379",
             "NOT-LADDR", "badlocal:6380",
             "NOT-NAME", "old",
             "NOT-FLAGS", "S",
             "NOT-LIB-NAME", "oldlib",
             "NOT-LIB-VER", "1.0",
             "NOT-DB", "7",
             "NOT-CAPA", "r",
             "NOT-IP", "192.168.0.1"],
            new ClientFilterOptions()
                .WithoutId(99)
                .WithoutType(ClientType.Replica)
                .WithoutAddress("badhost", 6379)
                .WithoutLocalAddress("badlocal", 6380)
                .WithoutUser("guest")
                .WithoutFlag(ClientFlag.Replica)
                .WithoutName("old")
                .WithoutLibraryName("oldlib")
                .WithoutLibraryVersion("1.0")
                .WithoutDatabaseId(7)
                .WithoutCapability(ClientCapability.Redirect)
                .WithoutIpAddress("192.168.0.1")
                .ToArgs());
    }

    [Fact]
    public void ToArgs_Failure()
    {
        _ = Assert.Throws<ArgumentException>(() => new ClientFilterOptions().WithUser(""));
        _ = Assert.Throws<ArgumentException>(() => new ClientFilterOptions().WithAddress("", 6379));
        _ = Assert.Throws<ArgumentException>(() => new ClientFilterOptions().WithLocalAddress("", 6379));
        _ = Assert.Throws<ArgumentOutOfRangeException>(() => new ClientFilterOptions().WithMaxAge(TimeSpan.Zero));
        _ = Assert.Throws<ArgumentOutOfRangeException>(() => new ClientFilterOptions().WithMaxAge(TimeSpan.FromSeconds(-1)));

        // Positive filters (Since Valkey 9.0.0)
        _ = Assert.Throws<ArgumentException>(() => new ClientFilterOptions().WithName(""));
        _ = Assert.Throws<ArgumentOutOfRangeException>(() => new ClientFilterOptions().WithIdle(TimeSpan.Zero));
        _ = Assert.Throws<ArgumentOutOfRangeException>(() => new ClientFilterOptions().WithIdle(TimeSpan.FromMilliseconds(-1)));
        _ = Assert.Throws<ArgumentException>(() => new ClientFilterOptions().WithLibraryName(""));
        _ = Assert.Throws<ArgumentException>(() => new ClientFilterOptions().WithLibraryVersion(""));
        _ = Assert.Throws<ArgumentException>(() => new ClientFilterOptions().WithIpAddress(""));

        // Negative filters (Since Valkey 9.0.0)
        _ = Assert.Throws<ArgumentException>(() => new ClientFilterOptions().WithoutAddress("", 6379));
        _ = Assert.Throws<ArgumentException>(() => new ClientFilterOptions().WithoutLocalAddress("", 6379));
        _ = Assert.Throws<ArgumentException>(() => new ClientFilterOptions().WithoutUser(""));
        _ = Assert.Throws<ArgumentException>(() => new ClientFilterOptions().WithoutName(""));
        _ = Assert.Throws<ArgumentException>(() => new ClientFilterOptions().WithoutLibraryName(""));
        _ = Assert.Throws<ArgumentException>(() => new ClientFilterOptions().WithoutLibraryVersion(""));
        _ = Assert.Throws<ArgumentException>(() => new ClientFilterOptions().WithoutIpAddress(""));
    }
}
