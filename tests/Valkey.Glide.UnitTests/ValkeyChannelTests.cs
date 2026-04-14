// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.UnitTests;

/// <summary>
/// Tests for <see cref="ValkeyChannel"/>.
/// </summary>
public class ValkeyChannelTests
{
    [Fact]
    public void KeySpaceSingleKey_BuildsCorrectChannel()
    {
        ValkeyChannel channel = ValkeyChannel.KeySpaceSingleKey("mykey", 0);

        Assert.Equal("__keyspace@0__:mykey", channel.ToString());
        Assert.False(channel.IsPattern);
    }

    [Fact]
    public void KeySpaceSingleKey_DifferentDatabase()
    {
        ValkeyChannel channel = ValkeyChannel.KeySpaceSingleKey("mykey", 5);

        Assert.Equal("__keyspace@5__:mykey", channel.ToString());
        Assert.False(channel.IsPattern);
    }

    [Fact]
    public void KeySpacePattern_WithDatabase_BuildsCorrectChannel()
    {
        ValkeyChannel channel = ValkeyChannel.KeySpacePattern("user:*", 0);

        Assert.Equal("__keyspace@0__:user:*", channel.ToString());
        Assert.True(channel.IsPattern);
    }

    [Fact]
    public void KeySpacePattern_WithoutDatabase_UsesWildcard()
    {
        ValkeyChannel channel = ValkeyChannel.KeySpacePattern("pattern*");

        Assert.Equal("__keyspace@*__:pattern*", channel.ToString());
        Assert.True(channel.IsPattern);
    }

    [Fact]
    public void KeySpacePattern_NullPattern_AppendsWildcard()
    {
        ValkeyChannel channel = ValkeyChannel.KeySpacePattern(ValkeyKey.Null);

        Assert.Equal("__keyspace@*__:*", channel.ToString());
        Assert.True(channel.IsPattern);
    }

    [Fact]
    public void KeySpacePrefix_BuildsCorrectChannel()
    {
        ValkeyChannel channel = ValkeyChannel.KeySpacePrefix("prefix", 0);

        Assert.Equal("__keyspace@0__:prefix*", channel.ToString());
        Assert.True(channel.IsPattern);
    }

    [Fact]
    public void KeySpacePrefix_EmptyPrefix_Throws()
        => _ = Assert.Throws<ArgumentNullException>(() => ValkeyChannel.KeySpacePrefix(ValkeyKey.Null));

    [Fact]
    public void KeySpacePrefix_Span_BuildsCorrectChannel()
    {
        ReadOnlySpan<byte> prefix = "prefix"u8;
        ValkeyChannel channel = ValkeyChannel.KeySpacePrefix(prefix, 0);

        Assert.Equal("__keyspace@0__:prefix*", channel.ToString());
        Assert.True(channel.IsPattern);
    }

    [Fact]
    public void KeySpacePrefix_Span_EmptyPrefix_Throws()
        => Assert.Throws<ArgumentNullException>(() => ValkeyChannel.KeySpacePrefix([]));

    [Fact]
    public void KeyEvent_WithType_BuildsCorrectChannel()
    {
        ValkeyChannel channel = ValkeyChannel.KeyEvent(KeyNotificationType.Del, 0);

        Assert.Equal("__keyevent@0__:del", channel.ToString());
        Assert.False(channel.IsPattern);
    }

    [Fact]
    public void KeyEvent_WithoutDatabase_UsesWildcard()
    {
        ValkeyChannel channel = ValkeyChannel.KeyEvent(KeyNotificationType.Del);

        Assert.Equal("__keyevent@*__:del", channel.ToString());
        Assert.True(channel.IsPattern);
    }

    [Fact]
    public void KeyEvent_Span_BuildsCorrectChannel()
    {
        ReadOnlySpan<byte> type = "del"u8;
        ValkeyChannel channel = ValkeyChannel.KeyEvent(type, 0);

        Assert.Equal("__keyevent@0__:del", channel.ToString());
        Assert.False(channel.IsPattern);
    }

    [Fact]
    public void KeyEvent_Span_EmptyType_Throws()
        => Assert.Throws<ArgumentNullException>(() => ValkeyChannel.KeyEvent([], 0));
}
