// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.UnitTests;

public class StreamIdRangeTests
{
    #region Constants

    private static readonly string StartId = "1000-0";
    private static readonly string EndId = "2000-0";

    #endregion
    #region Tests

    [Fact]
    public void All_MinToMax() => Assert.Multiple(
        () => Assert.Equal("-", StreamIdRange.All.Start.Value.ToString()),
        () => Assert.Equal("+", StreamIdRange.All.End.Value.ToString()));

    [Fact]
    public void From_StreamIdBound()
    {
        var range = StreamIdRange.From(StreamIdBound.Inclusive(StartId));
        Assert.Equal(StartId, range.Start.Value.ToString());
        Assert.Equal("+", range.End.Value.ToString());
    }

    [Fact]
    public void From_ValkeyValue()
    {
        var range = StreamIdRange.From((ValkeyValue)StartId);
        Assert.Equal(StartId, range.Start.Value.ToString());
        Assert.Equal("+", range.End.Value.ToString());
    }

    [Fact]
    public void From_String()
    {
        var range = StreamIdRange.From(StartId);
        Assert.Equal(StartId, range.Start.Value.ToString());
        Assert.Equal("+", range.End.Value.ToString());
    }

    [Fact]
    public void To_StreamIdBound()
    {
        var range = StreamIdRange.To(StreamIdBound.Inclusive(EndId));
        Assert.Equal("-", range.Start.Value.ToString());
        Assert.Equal(EndId, range.End.Value.ToString());
    }

    [Fact]
    public void To_ValkeyValue()
    {
        var range = StreamIdRange.To((ValkeyValue)EndId);
        Assert.Equal("-", range.Start.Value.ToString());
        Assert.Equal(EndId, range.End.Value.ToString());
    }

    [Fact]
    public void To_String()
    {
        var range = StreamIdRange.To(EndId);
        Assert.Equal("-", range.Start.Value.ToString());
        Assert.Equal(EndId, range.End.Value.ToString());
    }

    [Fact]
    public void Between_StreamIdBound()
    {
        var range = StreamIdRange.Between(StreamIdBound.Inclusive(StartId), StreamIdBound.Inclusive(EndId));
        Assert.Equal(StartId, range.Start.Value.ToString());
        Assert.Equal(EndId, range.End.Value.ToString());
    }

    [Fact]
    public void Between_ValkeyValue()
    {
        var range = StreamIdRange.Between((ValkeyValue)StartId, (ValkeyValue)EndId);
        Assert.Equal(StartId, range.Start.Value.ToString());
        Assert.Equal(EndId, range.End.Value.ToString());
    }

    [Fact]
    public void Between_String()
    {
        var range = StreamIdRange.Between(StartId, EndId);
        Assert.Equal(StartId, range.Start.Value.ToString());
        Assert.Equal(EndId, range.End.Value.ToString());
    }

    [Fact]
    public void Between_ExclusiveBounds()
    {
        var range = StreamIdRange.Between(StreamIdBound.Exclusive(StartId), StreamIdBound.Exclusive(EndId));
        Assert.Equal($"({StartId}", range.Start.Value.ToString());
        Assert.Equal($"({EndId}", range.End.Value.ToString());
    }

    [Fact]
    public void Between_MixedBounds()
    {
        var range = StreamIdRange.Between(StreamIdBound.Exclusive(StartId), StreamIdBound.Inclusive(EndId));
        Assert.Equal($"({StartId}", range.Start.Value.ToString());
        Assert.Equal(EndId, range.End.Value.ToString());
    }

    #endregion
}
