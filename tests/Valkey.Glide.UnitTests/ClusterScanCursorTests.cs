// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Xunit;

namespace Valkey.Glide.UnitTests;

public class ClusterScanCursorTests
{
    [Fact]
    public void InitialCursor()
    {
        var cursor = ClusterScanCursor.InitialCursor();

        Assert.Equal("0", cursor.CursorId);
        Assert.False(cursor.IsFinished);
    }

    [Fact]
    public void IsFinished()
    {
        var cursorId = "finished";
        var cursor = new ClusterScanCursor(cursorId);

        Assert.Equal(cursorId, cursor.CursorId);
        Assert.True(cursor.IsFinished);

        cursorId = "some-cursor-id";
        cursor = new ClusterScanCursor(cursorId);

        Assert.Equal(cursorId, cursor.CursorId);
        Assert.False(cursor.IsFinished);
    }
}
