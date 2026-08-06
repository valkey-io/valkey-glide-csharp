// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.UnitTests;

public class UtilsTests
{
    [Fact]
    public void FormatAddress_Success()
    {
        Assert.Equal("127.0.0.1:6379", Utils.FormatAddress("127.0.0.1", 6379));
        Assert.Equal("localhost:6379", Utils.FormatAddress("localhost", 6379));
        Assert.Equal("[::1]:6379", Utils.FormatAddress("::1", 6379));
        Assert.Equal("[2001:db8::1]:8080", Utils.FormatAddress("2001:db8::1", 8080));
    }
}
