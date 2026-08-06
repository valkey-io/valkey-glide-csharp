// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands.Options;

namespace Valkey.Glide.UnitTests;

public class ClientFilterOptionsTests
{
    [Fact]
    public void ToClientKillArgs_Success()
    {
        Assert.Empty(new ClientFilterOptions().ToClientKillArgs());
        Assert.Equal(["ID", "1"], new ClientFilterOptions().WithId(1).ToClientKillArgs());
        Assert.Equal(["ID", "1", "ID", "2", "ID", "3"], new ClientFilterOptions().WithIds([1, 2, 3]).ToClientKillArgs());
        Assert.Equal(["TYPE", "normal"], new ClientFilterOptions().WithType(ClientType.Normal).ToClientKillArgs());
        Assert.Equal(["TYPE", "master"], new ClientFilterOptions().WithType(ClientType.Primary).ToClientKillArgs());
        Assert.Equal(["TYPE", "replica"], new ClientFilterOptions().WithType(ClientType.Replica).ToClientKillArgs());
        Assert.Equal(["TYPE", "pubsub"], new ClientFilterOptions().WithType(ClientType.PubSub).ToClientKillArgs());
        Assert.Equal(["USER", "user"], new ClientFilterOptions().WithUser("user").ToClientKillArgs());
        Assert.Equal(["ADDR", "addr:1234"], new ClientFilterOptions().WithAddress("addr", 1234).ToClientKillArgs());
        Assert.Equal(["ADDR", "127.0.0.1:1234"], new ClientFilterOptions().WithAddress("127.0.0.1", 1234).ToClientKillArgs());
        Assert.Equal(["ADDR", "[::1]:1234"], new ClientFilterOptions().WithAddress("::1", 1234).ToClientKillArgs());
        Assert.Equal(["LADDR", "laddr:4321"], new ClientFilterOptions().WithLocalAddress("laddr", 4321).ToClientKillArgs());
        Assert.Equal(["LADDR", "127.0.0.1:4321"], new ClientFilterOptions().WithLocalAddress("127.0.0.1", 4321).ToClientKillArgs());
        Assert.Equal(["LADDR", "[::1]:4321"], new ClientFilterOptions().WithLocalAddress("::1", 4321).ToClientKillArgs());
        Assert.Equal(["SKIPME", "yes"], new ClientFilterOptions().WithSkipMe(true).ToClientKillArgs());
        Assert.Equal(["SKIPME", "no"], new ClientFilterOptions().WithSkipMe(false).ToClientKillArgs());
        Assert.Equal(["MAXAGE", "0"], new ClientFilterOptions().WithMaxAge(TimeSpan.Zero).ToClientKillArgs());
        Assert.Equal(["MAXAGE", "1"], new ClientFilterOptions().WithMaxAge(TimeSpan.FromSeconds(1)).ToClientKillArgs());
        Assert.Equal(["MAXAGE", "2"], new ClientFilterOptions().WithMaxAge(TimeSpan.FromSeconds(2.1)).ToClientKillArgs());
        Assert.Equal(["MAXAGE", "3"], new ClientFilterOptions().WithMaxAge(TimeSpan.FromSeconds(2.9)).ToClientKillArgs());

        Assert.Equal(
            ["ID", "42", "TYPE", "normal", "USER", "user", "ADDR", "addr:1234", "LADDR", "laddr:4321", "SKIPME", "no", "MAXAGE", "1"],
            new ClientFilterOptions()
                .WithId(42)
                .WithType(ClientType.Normal)
                .WithUser("user")
                .WithAddress("addr", 1234)
                .WithLocalAddress("laddr", 4321)
                .WithSkipMe(false)
                .WithMaxAge(TimeSpan.FromSeconds(1))
                .ToClientKillArgs());
    }

    [Fact]
    public void ToClientKillArgs_Failure()
    {
        _ = Assert.Throws<ArgumentException>(() => new ClientFilterOptions().WithUser(""));
        _ = Assert.Throws<ArgumentException>(() => new ClientFilterOptions().WithAddress("", 6379));
        _ = Assert.Throws<ArgumentException>(() => new ClientFilterOptions().WithLocalAddress("", 6379));
        _ = Assert.Throws<ArgumentOutOfRangeException>(() => new ClientFilterOptions().WithMaxAge(TimeSpan.FromSeconds(-1)));
    }
}
