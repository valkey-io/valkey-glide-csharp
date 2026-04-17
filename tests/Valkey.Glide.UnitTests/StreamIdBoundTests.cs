// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.UnitTests;

public class StreamIdBoundTests
{
    #region Constants

    private static readonly string TestId = "1234567890-0";

    #endregion
    #region Tests

    [Fact]
    public void Min_Value() => Assert.Equal("-", StreamIdBound.Min.Value.ToString());

    [Fact]
    public void Max_Value() => Assert.Equal("+", StreamIdBound.Max.Value.ToString());

    [Fact]
    public void Inclusive_ValkeyValue() => Assert.Multiple(
        () => Assert.Equal(TestId, StreamIdBound.Inclusive((ValkeyValue)TestId).Value.ToString()),
        () => Assert.Equal("0-0", StreamIdBound.Inclusive((ValkeyValue)"0-0").Value.ToString()));

    [Fact]
    public void Inclusive_String() => Assert.Multiple(
        () => Assert.Equal(TestId, StreamIdBound.Inclusive(TestId).Value.ToString()),
        () => Assert.Equal("0-0", StreamIdBound.Inclusive("0-0").Value.ToString()));

    [Fact]
    public void Exclusive_ValkeyValue() => Assert.Multiple(
        () => Assert.Equal($"({TestId}", StreamIdBound.Exclusive((ValkeyValue)TestId).Value.ToString()),
        () => Assert.Equal("(0-0", StreamIdBound.Exclusive((ValkeyValue)"0-0").Value.ToString()));

    [Fact]
    public void Exclusive_String() => Assert.Multiple(
        () => Assert.Equal($"({TestId}", StreamIdBound.Exclusive(TestId).Value.ToString()),
        () => Assert.Equal("(0-0", StreamIdBound.Exclusive("0-0").Value.ToString()));

    [Fact]
    public void ImplicitConversion_FromValkeyValue()
    {
        StreamIdBound bound = (ValkeyValue)TestId;
        Assert.Equal(TestId, bound.Value.ToString());
    }

    [Fact]
    public void ImplicitConversion_FromString()
    {
        StreamIdBound bound = TestId;
        Assert.Equal(TestId, bound.Value.ToString());
    }

    #endregion
}
