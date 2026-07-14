// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands.Options;

namespace Valkey.Glide.UnitTests;

public class FailoverOptionsTests
{
    [Fact]
    public void ToArgs_Success() => Assert.Multiple(
        () => Assert.Equal(["ABORT"], FailoverOptions.Abort().ToArgs()),
        () => Assert.Equal(["TIMEOUT", "5000"], FailoverOptions.Timeout(TimeSpan.FromSeconds(5)).ToArgs()),
        () => Assert.Equal(["TIMEOUT", "0"], FailoverOptions.Timeout(TimeSpan.Zero).ToArgs()),
        () => Assert.Equal(["TO", "localhost", "6380"], FailoverOptions.To("localhost", 6380).ToArgs()),
        () => Assert.Equal(["TO", "127.0.0.1", "0"], FailoverOptions.To("127.0.0.1", 0).ToArgs()),
        () => Assert.Equal(["TO", "localhost", "6380", "TIMEOUT", "10000"], FailoverOptions.To("localhost", 6380, TimeSpan.FromSeconds(10)).ToArgs()),
        () => Assert.Equal(["TO", "localhost", "6380", "FORCE", "TIMEOUT", "5000"], FailoverOptions.Forced("localhost", 6380, TimeSpan.FromSeconds(5)).ToArgs()));

    [Fact]
    public void ToArgs_Failure() => Assert.Multiple(
        () => Assert.Throws<ArgumentException>(() => FailoverOptions.Timeout(TimeSpan.FromSeconds(-1)).ToArgs()),
        () => Assert.Throws<ArgumentException>(() => FailoverOptions.To("localhost", 6380, TimeSpan.FromSeconds(-1)).ToArgs()),
        () => Assert.Throws<ArgumentException>(() => FailoverOptions.Forced("localhost", 6380, TimeSpan.FromSeconds(-1)).ToArgs()));
}
