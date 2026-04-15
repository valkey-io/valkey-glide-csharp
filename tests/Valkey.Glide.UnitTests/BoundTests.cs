// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.UnitTests;

public class BoundTests
{
    #region ScoreBound Tests

    [Fact]
    public void ScoreBound_Inclusive() => Assert.Multiple(
        () => Assert.Equal(["5"], ScoreBound.Inclusive(5.0).ToArgs()),
        () => Assert.Equal(["0"], ScoreBound.Inclusive(0.0).ToArgs()),
        () => Assert.Equal(["-5"], ScoreBound.Inclusive(-5.0).ToArgs()),
        () => Assert.Equal(["10.5"], ScoreBound.Inclusive(10.5).ToArgs()));

    [Fact]
    public void ScoreBound_Exclusive() => Assert.Multiple(
        () => Assert.Equal(["(5"], ScoreBound.Exclusive(5.0).ToArgs()),
        () => Assert.Equal(["(0"], ScoreBound.Exclusive(0.0).ToArgs()),
        () => Assert.Equal(["(-5"], ScoreBound.Exclusive(-5.0).ToArgs()),
        () => Assert.Equal(["(10.5"], ScoreBound.Exclusive(10.5).ToArgs()));

    [Fact]
    public void ScoreBound_MinMax() => Assert.Multiple(
        () => Assert.Equal(["-inf"], ScoreBound.Min.ToArgs()),
        () => Assert.Equal(["+inf"], ScoreBound.Max.ToArgs()),
        () => Assert.True(ScoreBound.Min.IsMin),
        () => Assert.False(ScoreBound.Min.IsMax),
        () => Assert.True(ScoreBound.Max.IsMax),
        () => Assert.False(ScoreBound.Max.IsMin),
        () => Assert.False(ScoreBound.Inclusive(5.0).IsMin),
        () => Assert.False(ScoreBound.Inclusive(5.0).IsMax));

    [Fact]
    public void ScoreBound_ImplicitFromDouble() => Assert.Multiple(
        () => Assert.Equal(["3.1400000000000001"], ((ScoreBound)3.14).ToArgs()),
        () => Assert.Same(ScoreBound.Min, (ScoreBound)double.NegativeInfinity),
        () => Assert.Same(ScoreBound.Max, (ScoreBound)double.PositiveInfinity));

    [Fact]
    public void ScoreBound_ImplicitFromValkeyValue() => Assert.Multiple(
        () => Assert.Equal(["7.5"], ((ScoreBound)(ValkeyValue)"7.5").ToArgs()),
        () => Assert.Same(ScoreBound.Min, (ScoreBound)(ValkeyValue)"-"),
        () => Assert.Same(ScoreBound.Max, (ScoreBound)(ValkeyValue)"+"),
        () => Assert.Throws<ArgumentNullException>(() => (ScoreBound)ValkeyValue.Null));

    #endregion
    #region LexBound Tests

    [Fact]
    public void LexBound_Inclusive() => Assert.Multiple(
        () => Assert.Equal(["[abc"], LexBound.Inclusive("abc").ToArgs()),
        () => Assert.Equal(["[hello world"], LexBound.Inclusive("hello world").ToArgs()),
        () => Assert.Same(LexBound.Min, LexBound.Inclusive("-")),
        () => Assert.Same(LexBound.Min, LexBound.Inclusive("+")));

    [Fact]
    public void LexBound_Exclusive() => Assert.Multiple(
        () => Assert.Equal(["(abc"], LexBound.Exclusive("abc").ToArgs()),
        () => Assert.Equal(["(hello world"], LexBound.Exclusive("hello world").ToArgs()));

    [Fact]
    public void LexBound_MinMax() => Assert.Multiple(
        () => Assert.Equal(["-"], LexBound.Min.ToArgs()),
        () => Assert.Equal(["+"], LexBound.Max.ToArgs()));

    [Fact]
    public void LexBound_ImplicitFromString() => Assert.Multiple(
        () => Assert.Equal(["[hello"], ((LexBound)"hello").ToArgs()),
        () => Assert.Equal(["[world"], ((LexBound)"world").ToArgs()));

    [Fact]
    public void LexBound_ImplicitFromValkeyValue() => Assert.Multiple(
        () => Assert.Equal(["[test"], ((LexBound)(ValkeyValue)"test").ToArgs()),
        () => Assert.Throws<ArgumentNullException>(() => (LexBound)ValkeyValue.Null));

    #endregion
}
