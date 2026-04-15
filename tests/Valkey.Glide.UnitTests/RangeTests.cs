// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.UnitTests;

public class RangeTests
{
    #region ScoreRange Tests

    [Fact]
    public void ScoreRange_All() => Assert.Multiple(
        () => Assert.Equal(["-inf", "+inf"], ScoreRange.All.ToArgs()),
        () => Assert.True(ScoreRange.All.IsUnbounded()));

    [Fact]
    public void ScoreRange_Between() => Assert.Multiple(
        () => Assert.Equal(["1", "10"], ScoreRange.Between(ScoreBound.Inclusive(1.0), ScoreBound.Inclusive(10.0)).ToArgs()),
        () => Assert.Equal(["(1", "(10"], ScoreRange.Between(ScoreBound.Exclusive(1.0), ScoreBound.Exclusive(10.0)).ToArgs()),
        () => Assert.Equal(["(1", "10"], ScoreRange.Between(ScoreBound.Exclusive(1.0), ScoreBound.Inclusive(10.0)).ToArgs()),
        () => Assert.Equal(["1", "10"], ScoreRange.Between(1.0, 10.0).ToArgs()),
        () => Assert.False(ScoreRange.Between(1.0, 10.0).IsUnbounded()));

    [Fact]
    public void ScoreRange_From() => Assert.Multiple(
        () => Assert.Equal(["5", "+inf"], ScoreRange.From(ScoreBound.Inclusive(5.0)).ToArgs()),
        () => Assert.Equal(["(5", "+inf"], ScoreRange.From(ScoreBound.Exclusive(5.0)).ToArgs()),
        () => Assert.False(ScoreRange.From(ScoreBound.Inclusive(5.0)).IsUnbounded()));

    [Fact]
    public void ScoreRange_To() => Assert.Multiple(
        () => Assert.Equal(["-inf", "5"], ScoreRange.To(ScoreBound.Inclusive(5.0)).ToArgs()),
        () => Assert.Equal(["-inf", "(5"], ScoreRange.To(ScoreBound.Exclusive(5.0)).ToArgs()),
        () => Assert.False(ScoreRange.To(ScoreBound.Inclusive(5.0)).IsUnbounded()));

    #endregion
    #region LexRange Tests

    [Fact]
    public void LexRange_All() => Assert.Multiple(
        () => Assert.Equal(["-", "+"], LexRange.All.ToArgs()),
        () => Assert.True(LexRange.All.IsUnbounded()));

    [Fact]
    public void LexRange_Between() => Assert.Multiple(
        () => Assert.Equal(["[a", "[z"], LexRange.Between(LexBound.Inclusive("a"), LexBound.Inclusive("z")).ToArgs()),
        () => Assert.Equal(["(a", "(z"], LexRange.Between(LexBound.Exclusive("a"), LexBound.Exclusive("z")).ToArgs()),
        () => Assert.Equal(["(a", "[z"], LexRange.Between(LexBound.Exclusive("a"), LexBound.Inclusive("z")).ToArgs()),
        () => Assert.Equal(["[a", "[z"], LexRange.Between("a", "z").ToArgs()),
        () => Assert.False(LexRange.Between("a", "z").IsUnbounded()));

    [Fact]
    public void LexRange_From() => Assert.Multiple(
        () => Assert.Equal(["[m", "+"], LexRange.From(LexBound.Inclusive("m")).ToArgs()),
        () => Assert.Equal(["(m", "+"], LexRange.From(LexBound.Exclusive("m")).ToArgs()));

    [Fact]
    public void LexRange_To() => Assert.Multiple(
        () => Assert.Equal(["-", "[m"], LexRange.To(LexBound.Inclusive("m")).ToArgs()),
        () => Assert.Equal(["-", "(m"], LexRange.To(LexBound.Exclusive("m")).ToArgs()));

    #endregion
    #region RankRange Tests

    [Fact]
    public void RankRange_All() => Assert.Multiple(
        () => Assert.Equal(["0", "-1"], RankRange.All.ToArgs()),
        () => Assert.True(RankRange.All.IsUnbounded()));

    [Fact]
    public void RankRange_Between() => Assert.Multiple(
        () => Assert.Equal(["1", "5"], RankRange.Between(1, 5).ToArgs()),
        () => Assert.Equal(["-3", "-1"], RankRange.Between(-3, -1).ToArgs()),
        () => Assert.Equal(["0", "0"], RankRange.Between(0, 0).ToArgs()),
        () => Assert.False(RankRange.Between(1, 5).IsUnbounded()));

    [Fact]
    public void RankRange_From() => Assert.Multiple(
        () => Assert.Equal(["3", "-1"], RankRange.From(3).ToArgs()),
        () => Assert.Equal(["-2", "-1"], RankRange.From(-2).ToArgs()));

    [Fact]
    public void RankRange_To() => Assert.Multiple(
        () => Assert.Equal(["0", "5"], RankRange.To(5).ToArgs()),
        () => Assert.Equal(["0", "-2"], RankRange.To(-2).ToArgs()));

    #endregion
}
