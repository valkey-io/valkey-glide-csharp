// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands.Options;

namespace Valkey.Glide.UnitTests;

public class ClientFilterOptionsTests
{
    [Fact]
    public void ToClientKillArgs_NoFilters_ReturnsEmpty()
        => Assert.Empty(new ClientFilterOptions().ToClientKillArgs());

    [Fact]
    public void ToClientKillArgs_WithSingleId()
        => Assert.Equal(["ID", "42"], Str(new ClientFilterOptions().WithId(42).ToClientKillArgs()));

    [Fact]
    public void ToClientKillArgs_WithIds()
        => Assert.Equal(["ID", "1", "ID", "2", "ID", "3"], Str(new ClientFilterOptions().WithIds([1, 2, 3]).ToClientKillArgs()));

    [Fact]
    public void ToClientKillArgs_WithId_CalledMultipleTimes_Accumulates()
        => Assert.Equal(["ID", "10", "ID", "20"], Str(new ClientFilterOptions().WithId(10).WithId(20).ToClientKillArgs()));

    [Fact]
    public void ToClientKillArgs_WithIds_Deduplicates()
        => Assert.Equal(["ID", "1", "ID", "2"], Str(new ClientFilterOptions().WithIds([1, 2, 1]).ToClientKillArgs()));

    [Theory]
    [InlineData(ClientType.Normal, "normal")]
    [InlineData(ClientType.Primary, "master")]
    [InlineData(ClientType.Replica, "replica")]
    [InlineData(ClientType.PubSub, "pubsub")]
    public void ToClientKillArgs_WithType(ClientType type, string expected)
        => Assert.Equal(["TYPE", expected], Str(new ClientFilterOptions().WithType(type).ToClientKillArgs()));

    [Fact]
    public void ToClientKillArgs_WithUser()
        => Assert.Equal(["USER", "admin"], Str(new ClientFilterOptions().WithUser("admin").ToClientKillArgs()));

    [Fact]
    public void WithUser_Empty_Throws()
        => _ = Assert.Throws<ArgumentException>(() => new ClientFilterOptions().WithUser(""));

    [Fact]
    public void ToClientKillArgs_WithAddress()
        => Assert.Equal(["ADDR", "10.0.0.1:6379"], Str(new ClientFilterOptions().WithAddress("10.0.0.1", 6379).ToClientKillArgs()));

    [Fact]
    public void WithAddress_EmptyHost_Throws()
        => _ = Assert.Throws<ArgumentException>(() => new ClientFilterOptions().WithAddress("", 6379));

    [Fact]
    public void ToClientKillArgs_WithLocalAddress()
        => Assert.Equal(["LADDR", "192.168.1.1:6380"], Str(new ClientFilterOptions().WithLocalAddress("192.168.1.1", 6380).ToClientKillArgs()));

    [Theory]
    [InlineData(true, "yes")]
    [InlineData(false, "no")]
    public void ToClientKillArgs_WithSkipMe(bool skipMe, string expected)
        => Assert.Equal(["SKIPME", expected], Str(new ClientFilterOptions().WithSkipMe(skipMe).ToClientKillArgs()));

    [Fact]
    public void ToClientKillArgs_WithMaxAge()
        => Assert.Equal(["MAXAGE", "60"], Str(new ClientFilterOptions().WithMaxAge(TimeSpan.FromSeconds(60)).ToClientKillArgs()));

    [Fact]
    public void ToClientKillArgs_WithMaxAge_FractionalSeconds_Rounds()
        => Assert.Equal(["MAXAGE", "91"], Str(new ClientFilterOptions().WithMaxAge(TimeSpan.FromSeconds(90.7)).ToClientKillArgs()));

    [Fact]
    public void WithMaxAge_Zero_Throws()
        => _ = Assert.Throws<ArgumentOutOfRangeException>(() => new ClientFilterOptions().WithMaxAge(TimeSpan.Zero));

    [Fact]
    public void WithMaxAge_Negative_Throws()
        => _ = Assert.Throws<ArgumentOutOfRangeException>(() => new ClientFilterOptions().WithMaxAge(TimeSpan.FromSeconds(-1)));

    [Fact]
    public void ToClientKillArgs_CombinedFilters()
    {
        var options = new ClientFilterOptions()
            .WithId(42)
            .WithType(ClientType.Normal)
            .WithUser("default")
            .WithAddress("127.0.0.1", 6379)
            .WithLocalAddress("0.0.0.0", 6379)
            .WithSkipMe(false)
            .WithMaxAge(TimeSpan.FromMinutes(5));

        Assert.Equal(
            ["ID", "42", "TYPE", "normal", "USER", "default", "ADDR", "127.0.0.1:6379", "LADDR", "0.0.0.0:6379", "SKIPME", "no", "MAXAGE", "300"],
            Str(options.ToClientKillArgs()));
    }

    private static string[] Str(GlideString[] args) => args.Select(a => a.ToString()).ToArray();
}
