// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.UnitTests;

public class RangeTests
{
    #region ScoreRange Tests

    [Fact]
    public void ScoreRange_MinToMax()
        => Assert.Equal(["-inf", "+inf"], ScoreRange.MinToMax.ToArgs());

    [Fact]
    public void ScoreRange_MaxToMin()
        => Assert.Equal(["+inf", "-inf"], ScoreRange.MaxToMin.ToArgs());

    [Fact]
    public void ScoreRange_Between() => Assert.Multiple(
        () => Assert.Equal(["1", "10"], ScoreRange.Between(ScoreBound.Inclusive(1.0), ScoreBound.Inclusive(10.0)).ToArgs()),
        () => Assert.Equal(["(1", "(10"], ScoreRange.Between(ScoreBound.Exclusive(1.0), ScoreBound.Exclusive(10.0)).ToArgs()),
        () => Assert.Equal(["(1", "10"], ScoreRange.Between(ScoreBound.Exclusive(1.0), ScoreBound.Inclusive(10.0)).ToArgs()),
        () => Assert.Equal(["1", "10"], ScoreRange.Between(1.0, 10.0).ToArgs()));

    #endregion
    #region LexRange Tests

    [Fact]
    public void LexRange_MinToMax()
        => Assert.Equal(["-", "+"], LexRange.MinToMax.ToArgs());

    [Fact]
    public void LexRange_MaxToMin()
        => Assert.Equal(["+", "-"], LexRange.MaxToMin.ToArgs());

    [Fact]
    public void LexRange_Between() => Assert.Multiple(
        () => Assert.Equal(["[a", "[z"], LexRange.Between(LexBound.Inclusive("a"), LexBound.Inclusive("z")).ToArgs()),
        () => Assert.Equal(["(a", "(z"], LexRange.Between(LexBound.Exclusive("a"), LexBound.Exclusive("z")).ToArgs()),
        () => Assert.Equal(["(a", "[z"], LexRange.Between(LexBound.Exclusive("a"), LexBound.Inclusive("z")).ToArgs()),
        () => Assert.Equal(["[a", "[z"], LexRange.Between("a", "z").ToArgs()));

    #endregion
    #region IndexRange Tests

    [Fact]
    public void IndexRange_FirstToLast()
        => Assert.Equal(["0", "-1"], IndexRange.FirstToLast.ToArgs());

    [Fact]
    public void IndexRange_LastToFirst()
        => Assert.Equal(["-1", "0"], IndexRange.LastToFirst.ToArgs());

    [Fact]
    public void IndexRange_Between() => Assert.Multiple(
        () => Assert.Equal(["1", "5"], IndexRange.Between(1, 5).ToArgs()),
        () => Assert.Equal(["-3", "-1"], IndexRange.Between(-3, -1).ToArgs()),
        () => Assert.Equal(["0", "0"], IndexRange.Between(0, 0).ToArgs()));

    #endregion
}
