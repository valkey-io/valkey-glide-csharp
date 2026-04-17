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

    #endregion
}
