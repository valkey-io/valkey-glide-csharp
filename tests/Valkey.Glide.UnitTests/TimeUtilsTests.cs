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
    public void ToNonNegativeDoubleSecs()
    {
        Assert.Equal(0.0, TimeUtils.ToNonNegativeDoubleSecs(Zero, "p"));
        Assert.Equal(0.5, TimeUtils.ToNonNegativeDoubleSecs(HalfSecond, "p"));
        Assert.Equal(1.5, TimeUtils.ToNonNegativeDoubleSecs(RoundUpToTwoSeconds, "p"));

        _ = Assert.Throws<ArgumentOutOfRangeException>(() => TimeUtils.ToNonNegativeDoubleSecs(Negative, "p"));
    }

    [Fact]
    public void ToPositiveUintMs()
    {
        Assert.Equal(500u, TimeUtils.ToPositiveUintMs(HalfSecond, "p"));
        Assert.Equal(1500u, TimeUtils.ToPositiveUintMs(RoundUpToTwoSeconds, "p"));
        Assert.Equal(uint.MaxValue, TimeUtils.ToPositiveUintMs(MaxUintMilliseconds, "p"));

        _ = Assert.Throws<ArgumentOutOfRangeException>(() => TimeUtils.ToPositiveUintMs(Zero, "p"));
        _ = Assert.Throws<ArgumentOutOfRangeException>(() => TimeUtils.ToPositiveUintMs(Negative, "p"));
        _ = Assert.Throws<ArgumentOutOfRangeException>(() => TimeUtils.ToPositiveUintMs(ExceedsUintMilliseconds, "p"));
    }

    [Fact]
    public void ToPositiveUintSecs()
    {
        Assert.Equal(1u, TimeUtils.ToPositiveUintSecs(RoundDownToOneSecond, "p"));
        Assert.Equal(2u, TimeUtils.ToPositiveUintSecs(RoundUpToTwoSeconds, "p"));
        Assert.Equal(uint.MaxValue, TimeUtils.ToPositiveUintSecs(MaxUintSeconds, "p"));

        _ = Assert.Throws<ArgumentOutOfRangeException>(() => TimeUtils.ToPositiveUintSecs(Zero, "p"));
        _ = Assert.Throws<ArgumentOutOfRangeException>(() => TimeUtils.ToPositiveUintSecs(Negative, "p"));
        _ = Assert.Throws<ArgumentOutOfRangeException>(() => TimeUtils.ToPositiveUintSecs(ExceedsUintSeconds, "p"));
    }

    [Fact]
    public void ToPositiveULongMs()
    {
        Assert.Equal(500UL, TimeUtils.ToPositiveULongMs(HalfSecond, "p"));
        Assert.Equal(60_000UL, TimeUtils.ToPositiveULongMs(OneMinute, "p"));

        _ = Assert.Throws<ArgumentOutOfRangeException>(() => TimeUtils.ToPositiveULongMs(Zero, "p"));
        _ = Assert.Throws<ArgumentOutOfRangeException>(() => TimeUtils.ToPositiveULongMs(Negative, "p"));
    }

    [Fact]
    public void ToULongMs()
    {
        Assert.Equal(0UL, TimeUtils.ToULongMs(Zero, "p"));
        Assert.Equal(500UL, TimeUtils.ToULongMs(HalfSecond, "p"));
        Assert.Equal(60_000UL, TimeUtils.ToULongMs(OneMinute, "p"));

        _ = Assert.Throws<ArgumentOutOfRangeException>(() => TimeUtils.ToULongMs(Negative, "p"));
    }

    [Fact]
    public void ToPositiveULongSecs()
    {
        Assert.Equal(1UL, TimeUtils.ToPositiveULongSecs(RoundDownToOneSecond, "p"));
        Assert.Equal(2UL, TimeUtils.ToPositiveULongSecs(RoundUpToTwoSeconds, "p"));
        Assert.Equal(60UL, TimeUtils.ToPositiveULongSecs(OneMinute, "p"));

        _ = Assert.Throws<ArgumentOutOfRangeException>(() => TimeUtils.ToPositiveULongSecs(Zero, "p"));
        _ = Assert.Throws<ArgumentOutOfRangeException>(() => TimeUtils.ToPositiveULongSecs(Negative, "p"));
    }

    [Fact]
    public void ToULongSecs()
    {
        Assert.Equal(0UL, TimeUtils.ToULongSecs(Zero, "p"));
        Assert.Equal(1UL, TimeUtils.ToULongSecs(RoundDownToOneSecond, "p"));
        Assert.Equal(2UL, TimeUtils.ToULongSecs(RoundUpToTwoSeconds, "p"));
        Assert.Equal(60UL, TimeUtils.ToULongSecs(OneMinute, "p"));

        _ = Assert.Throws<ArgumentOutOfRangeException>(() => TimeUtils.ToULongSecs(Negative, "p"));
    }
}
