// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.UnitTests;

public class TimeUtilsTests
{
    // Valid values
    private static readonly TimeSpan HalfSecond = TimeSpan.FromMilliseconds(500);
    private static readonly TimeSpan RoundDownToOneSecond = TimeSpan.FromMilliseconds(1400);
    private static readonly TimeSpan RoundUpToTwoSeconds = TimeSpan.FromMilliseconds(1500);
    private static readonly TimeSpan OneMinute = TimeSpan.FromMinutes(1);
    private static readonly TimeSpan MaxUintMilliseconds = TimeSpan.FromMilliseconds(uint.MaxValue);
    private static readonly TimeSpan MaxUintSeconds = TimeSpan.FromSeconds(uint.MaxValue);

    // Invalid values
    private static readonly TimeSpan Zero = TimeSpan.Zero;
    private static readonly TimeSpan Negative = TimeSpan.FromMilliseconds(-1);
    private static readonly TimeSpan ExceedsUintMilliseconds = TimeSpan.FromMilliseconds((double)uint.MaxValue + 1);
    private static readonly TimeSpan ExceedsUintSeconds = TimeSpan.FromSeconds((double)uint.MaxValue + 1);

    [Fact]
    public void ToUintMilliseconds()
    {
        Assert.Equal(500u, TimeUtils.ToUintMilliseconds(HalfSecond, "p"));
        Assert.Equal(1500u, TimeUtils.ToUintMilliseconds(RoundUpToTwoSeconds, "p"));
        Assert.Equal(uint.MaxValue, TimeUtils.ToUintMilliseconds(MaxUintMilliseconds, "p"));

        _ = Assert.Throws<ArgumentOutOfRangeException>(() => TimeUtils.ToUintMilliseconds(Zero, "p"));
        _ = Assert.Throws<ArgumentOutOfRangeException>(() => TimeUtils.ToUintMilliseconds(Negative, "p"));
        _ = Assert.Throws<ArgumentOutOfRangeException>(() => TimeUtils.ToUintMilliseconds(ExceedsUintMilliseconds, "p"));
    }

    [Fact]
    public void ToULongMilliseconds()
    {
        Assert.Equal(500UL, TimeUtils.ToULongMilliseconds(HalfSecond, "p"));
        Assert.Equal(60_000UL, TimeUtils.ToULongMilliseconds(OneMinute, "p"));

        _ = Assert.Throws<ArgumentOutOfRangeException>(() => TimeUtils.ToULongMilliseconds(Zero, "p"));
        _ = Assert.Throws<ArgumentOutOfRangeException>(() => TimeUtils.ToULongMilliseconds(Negative, "p"));
    }

    [Fact]
    public void ToDoubleSeconds()
    {
        Assert.Equal(0.5, TimeUtils.ToDoubleSeconds(HalfSecond, "p"));
        Assert.Equal(1.5, TimeUtils.ToDoubleSeconds(RoundUpToTwoSeconds, "p"));

        _ = Assert.Throws<ArgumentOutOfRangeException>(() => TimeUtils.ToDoubleSeconds(Zero, "p"));
        _ = Assert.Throws<ArgumentOutOfRangeException>(() => TimeUtils.ToDoubleSeconds(Negative, "p"));
    }

    [Fact]
    public void ToUintSeconds()
    {
        Assert.Equal(2u, TimeUtils.ToUintSeconds(RoundUpToTwoSeconds, "p"));
        Assert.Equal(1u, TimeUtils.ToUintSeconds(RoundDownToOneSecond, "p"));
        Assert.Equal(uint.MaxValue, TimeUtils.ToUintSeconds(MaxUintSeconds, "p"));

        _ = Assert.Throws<ArgumentOutOfRangeException>(() => TimeUtils.ToUintSeconds(Zero, "p"));
        _ = Assert.Throws<ArgumentOutOfRangeException>(() => TimeUtils.ToUintSeconds(Negative, "p"));
        _ = Assert.Throws<ArgumentOutOfRangeException>(() => TimeUtils.ToUintSeconds(ExceedsUintSeconds, "p"));
    }
}
