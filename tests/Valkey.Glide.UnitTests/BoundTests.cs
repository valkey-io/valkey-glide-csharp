// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.UnitTests;

public class BoundTests
{
    private static readonly byte[] Bytes = [0x74, 0x65, 0x73, 0x74, 0xC0, 0xAF];
    private static readonly byte[] MinBytes = System.Text.Encoding.UTF8.GetBytes("-");
    private static readonly byte[] MaxBytes = System.Text.Encoding.UTF8.GetBytes("+");

    #region ScoreBound Tests

    [Fact]
    public void ScoreBound_ToArgs() => Assert.Multiple(
        // Inclusive
        () => Assert.Equal(["5"], ScoreBound.Inclusive(5.0).ToArgs()),
        () => Assert.Equal(["0"], ScoreBound.Inclusive(0.0).ToArgs()),
        () => Assert.Equal(["-5"], ScoreBound.Inclusive(-5.0).ToArgs()),
        () => Assert.Equal(["-inf"], ScoreBound.Inclusive(double.NegativeInfinity).ToArgs()),
        () => Assert.Equal(["+inf"], ScoreBound.Inclusive(double.PositiveInfinity).ToArgs()),

        // Exclusive
        () => Assert.Equal(["(5"], ScoreBound.Exclusive(5.0).ToArgs()),
        () => Assert.Equal(["(0"], ScoreBound.Exclusive(0.0).ToArgs()),
        () => Assert.Equal(["(-5"], ScoreBound.Exclusive(-5.0).ToArgs()),
        () => Assert.Equal(["-inf"], ScoreBound.Exclusive(double.NegativeInfinity).ToArgs()),
        () => Assert.Equal(["+inf"], ScoreBound.Exclusive(double.PositiveInfinity).ToArgs()),

        // Min/Max
        () => Assert.Equal(["-inf"], ScoreBound.Min.ToArgs()),
        () => Assert.Equal(["+inf"], ScoreBound.Max.ToArgs()),

        // From double
        () => Assert.Equal(["5"], ((ScoreBound)5.0).ToArgs()),
        () => Assert.Equal(["0"], ((ScoreBound)0.0).ToArgs()),
        () => Assert.Equal(["-5"], ((ScoreBound)(-5.0)).ToArgs()),
        () => Assert.Equal(["-inf"], ((ScoreBound)double.NegativeInfinity).ToArgs()),
        () => Assert.Equal(["+inf"], ((ScoreBound)double.PositiveInfinity).ToArgs()));

    [Fact]
    public void ScoreBound_Equality() => Assert.Multiple(
        // Same value, same inclusivity
        () => Assert.Equal(ScoreBound.Inclusive(5.0), ScoreBound.Inclusive(5.0)),
        () => Assert.Equal(ScoreBound.Exclusive(3.0), ScoreBound.Exclusive(3.0)),
        () => Assert.Equal(ScoreBound.Min, ScoreBound.Min),
        () => Assert.Equal(ScoreBound.Max, ScoreBound.Max),

        // Different value or different inclusivity
        () => Assert.NotEqual(ScoreBound.Inclusive(5.0), ScoreBound.Exclusive(5.0)),
        () => Assert.NotEqual(ScoreBound.Inclusive(1.0), ScoreBound.Inclusive(2.0)),
        () => Assert.NotEqual(ScoreBound.Min, ScoreBound.Max),

        // Null
        () => Assert.False(ScoreBound.Inclusive(5.0).Equals(null)),
        () => Assert.False(ScoreBound.Min.Equals(null)),

        // Infinity via Inclusive/Exclusive both equal Min/Max
        () => Assert.Equal(ScoreBound.Min, ScoreBound.Inclusive(double.NegativeInfinity)),
        () => Assert.Equal(ScoreBound.Max, ScoreBound.Inclusive(double.PositiveInfinity)),
        () => Assert.Equal(ScoreBound.Min, ScoreBound.Exclusive(double.NegativeInfinity)),
        () => Assert.Equal(ScoreBound.Max, ScoreBound.Exclusive(double.PositiveInfinity)),

        // Implicit double cast equals Min/Max
        () => Assert.Equal(ScoreBound.Min, (ScoreBound)double.NegativeInfinity),
        () => Assert.Equal(ScoreBound.Max, (ScoreBound)double.PositiveInfinity),

        // Cross-infinity not equal
        () => Assert.NotEqual(ScoreBound.Min, ScoreBound.Max),
        () => Assert.NotEqual(ScoreBound.Inclusive(double.NegativeInfinity), ScoreBound.Inclusive(double.PositiveInfinity)));

    [Fact]
    public void ScoreBound_GetHashCode() => Assert.Multiple(
        () => Assert.Equal(ScoreBound.Inclusive(5.0).GetHashCode(), ScoreBound.Inclusive(5.0).GetHashCode()),
        () => Assert.Equal(ScoreBound.Min.GetHashCode(), ScoreBound.Inclusive(double.NegativeInfinity).GetHashCode()),
        () => Assert.Equal(ScoreBound.Max.GetHashCode(), ScoreBound.Exclusive(double.PositiveInfinity).GetHashCode()),
        () => Assert.NotEqual(ScoreBound.Inclusive(5.0).GetHashCode(), ScoreBound.Exclusive(5.0).GetHashCode()),
        () => Assert.NotEqual(ScoreBound.Min.GetHashCode(), ScoreBound.Max.GetHashCode()));

    #endregion
    #region LexBound Tests

    [Fact]
    public void LexBound_ToArgs() => Assert.Multiple(
        // Inclusive
        () => Assert.Equal(["[abc"], LexBound.Inclusive("abc").ToArgs()),
        () => Assert.Equal(["-"], LexBound.Inclusive("-").ToArgs()),
        () => Assert.Equal(["+"], LexBound.Inclusive("+").ToArgs()),

        // Exclusive
        () => Assert.Equal(["(abc"], LexBound.Exclusive("abc").ToArgs()),
        () => Assert.Equal(["-"], LexBound.Exclusive("-").ToArgs()),
        () => Assert.Equal(["+"], LexBound.Exclusive("+").ToArgs()),

        // Min/Max
        () => Assert.Equal(["-"], LexBound.Min.ToArgs()),
        () => Assert.Equal(["+"], LexBound.Max.ToArgs()),

        // From string
        () => Assert.Equal(["[abc"], ((LexBound)"abc").ToArgs()),
        () => Assert.Equal(["-"], ((LexBound)"-").ToArgs()),
        () => Assert.Equal(["+"], ((LexBound)"+").ToArgs()),

        // From bytes
        () => Assert.Equal(["["u8.ToArray().ToGlideString() + Bytes.ToGlideString()], ((LexBound)Bytes).ToArgs()),
        () => Assert.Equal(["-"], ((LexBound)MinBytes).ToArgs()),
        () => Assert.Equal(["+"], ((LexBound)MaxBytes).ToArgs()),

        // From ValkeyValue
        () => Assert.Equal(["[test"], ((LexBound)(ValkeyValue)"test").ToArgs()),
        () => Assert.Equal(["[hello"], ((LexBound)(ValkeyValue)"hello").ToArgs()),
        () => Assert.Equal(["-"], ((LexBound)(ValkeyValue)"-").ToArgs()),
        () => Assert.Equal(["+"], ((LexBound)(ValkeyValue)"+").ToArgs()));

    [Fact]
    public void LexBound_Equality() => Assert.Multiple(
        // Same value, same inclusivity
        () => Assert.Equal(LexBound.Inclusive("abc"), LexBound.Inclusive("abc")),
        () => Assert.Equal(LexBound.Exclusive("abc"), LexBound.Exclusive("abc")),
        () => Assert.Equal(LexBound.Min, LexBound.Min),
        () => Assert.Equal(LexBound.Max, LexBound.Max),

        // Different value or different inclusivity
        () => Assert.NotEqual(LexBound.Inclusive("abc"), LexBound.Exclusive("abc")),
        () => Assert.NotEqual(LexBound.Inclusive("a"), LexBound.Inclusive("b")),
        () => Assert.NotEqual(LexBound.Min, LexBound.Max),

        // Null
        () => Assert.False(LexBound.Inclusive("x").Equals(null)),
        () => Assert.False(LexBound.Min.Equals(null)),

        // Sentinel values via Inclusive/Exclusive both equal Min/Max
        () => Assert.Equal(LexBound.Min, LexBound.Inclusive("-")),
        () => Assert.Equal(LexBound.Max, LexBound.Inclusive("+")),
        () => Assert.Equal(LexBound.Min, LexBound.Exclusive("-")),
        () => Assert.Equal(LexBound.Max, LexBound.Exclusive("+")),

        // Implicit string cast equals Min/Max
        () => Assert.Equal(LexBound.Min, (LexBound)"-"),
        () => Assert.Equal(LexBound.Max, (LexBound)"+"),

        // Implicit ValkeyValue cast equals Min/Max
        () => Assert.Equal(LexBound.Min, (LexBound)(ValkeyValue)"-"),
        () => Assert.Equal(LexBound.Max, (LexBound)(ValkeyValue)"+"),

        // Cross-sentinel not equal
        () => Assert.NotEqual(LexBound.Min, LexBound.Max),
        () => Assert.NotEqual(LexBound.Inclusive("-"), LexBound.Inclusive("+")));

    [Fact]
    public void LexBound_GetHashCode() => Assert.Multiple(
        () => Assert.Equal(LexBound.Inclusive("abc").GetHashCode(), LexBound.Inclusive("abc").GetHashCode()),
        () => Assert.Equal(LexBound.Min.GetHashCode(), LexBound.Inclusive("-").GetHashCode()),
        () => Assert.Equal(LexBound.Max.GetHashCode(), LexBound.Exclusive("+").GetHashCode()),
        () => Assert.NotEqual(LexBound.Inclusive("abc").GetHashCode(), LexBound.Exclusive("abc").GetHashCode()),
        () => Assert.NotEqual(LexBound.Min.GetHashCode(), LexBound.Max.GetHashCode()));

    [Fact]
    public void LexBound_Comparison() => Assert.Multiple(
        // Min is less than everything
        () => Assert.True(LexBound.Min < LexBound.Inclusive("a")),
        () => Assert.True(LexBound.Min < LexBound.Max),
        () => Assert.True(LexBound.Min <= LexBound.Inclusive("-")),

        // Max is greater than everything
        () => Assert.True(LexBound.Max > LexBound.Inclusive("z")),
        () => Assert.True(LexBound.Max > LexBound.Min),
        () => Assert.True(LexBound.Max >= LexBound.Inclusive("+")),

        // Regular values compare lexicographically
        () => Assert.True(LexBound.Inclusive("a") < LexBound.Inclusive("b")),
        () => Assert.True(LexBound.Inclusive("b") > LexBound.Inclusive("a")),
        () => Assert.True(LexBound.Inclusive("abc") < LexBound.Inclusive("abd")),
        () => Assert.True(LexBound.Inclusive("a") <= LexBound.Inclusive("a")),
        () => Assert.True(LexBound.Inclusive("a") >= LexBound.Inclusive("a")),

        // Inclusivity does not affect ordering
        () => Assert.Equal(0, LexBound.Inclusive("a").CompareTo(LexBound.Exclusive("a"))),
        () => Assert.True(LexBound.Exclusive("a") < LexBound.Inclusive("b")),

        // Sentinel equality
        () => Assert.Equal(0, LexBound.Min.CompareTo(LexBound.Min)),
        () => Assert.Equal(0, LexBound.Max.CompareTo(LexBound.Max)));

    #endregion
    #region Cross-Type Tests

    [Fact]
    public void Bound_CrossType_NotEqual() => Assert.Multiple(
        // LexBound and ScoreBound are never equal, even with identical ToArgs output
        () => Assert.False(ScoreBound.Min.Equals(LexBound.Min)),
        () => Assert.False(LexBound.Max.Equals(ScoreBound.Max)),
        () => Assert.False(ScoreBound.Min.Equals(LexBound.Exclusive("inf"))),
        () => Assert.False(LexBound.Min.Equals(ScoreBound.Inclusive(0.0))),
        () => Assert.False(ScoreBound.Inclusive(0.0).Equals(LexBound.Exclusive("0.0"))),
        () => Assert.False(ScoreBound.Inclusive(0).Equals(LexBound.Exclusive("0"))));

    #endregion
}
