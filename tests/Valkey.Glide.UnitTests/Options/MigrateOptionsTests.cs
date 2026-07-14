// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands.Options;

using static Valkey.Glide.TestUtils.Options;

namespace Valkey.Glide.UnitTests;

public class MigrateOptionsTests
{
    [Fact]
    public void ToArgs_EmptyArray_Throws()
        => _ = Assert.Throws<ArgumentException>(() => BuildMigrateOptions().ToArgs([]));

    [Fact]
    public void ToArgs_WithKey_Succeeds()
    {
        Assert.Equal(["host", "6379", "key", "0", "5000"], new MigrateOptions("host", 6379, 0, TimeSpan.FromSeconds(5)).ToArgs(["key"]));
        Assert.Equal(["host", "6379", "key", "0", "0", "COPY"], new MigrateOptions("host", 6379, 0, TimeSpan.Zero).WithCopy().ToArgs(["key"]));
        Assert.Equal(["host", "6379", "key", "0", "0", "REPLACE"], new MigrateOptions("host", 6379, 0, TimeSpan.Zero).WithReplace().ToArgs(["key"]));
        Assert.Equal(["host", "6379", "key", "0", "0", "COPY", "REPLACE"], new MigrateOptions("host", 6379, 0, TimeSpan.Zero).WithCopy().WithReplace().ToArgs(["key"]));
        Assert.Equal(["host", "6379", "key", "0", "0", "AUTH", "secret"], new MigrateOptions("host", 6379, 0, TimeSpan.Zero).WithAuth("secret").ToArgs(["key"]));
        Assert.Equal(["host", "6379", "key", "0", "0", "AUTH2", "user", "pass"], new MigrateOptions("host", 6379, 0, TimeSpan.Zero).WithAuth("user", "pass").ToArgs(["key"]));
        Assert.Equal(["host", "6379", "key", "2", "10000", "COPY", "REPLACE", "AUTH2", "admin", "pass"], new MigrateOptions("host", 6379, 2, TimeSpan.FromSeconds(10)).WithCopy().WithReplace().WithAuth("admin", "pass").ToArgs(["key"]));
    }

    [Fact]
    public void ToArgs_WithKeys_Succeeds()
    {
        Assert.Equal(["host", "6379", "", "0", "5000", "KEYS", "k1", "k2"], new MigrateOptions("host", 6379, 0, TimeSpan.FromSeconds(5)).ToArgs(["k1", "k2"]));
        Assert.Equal(["host", "6379", "", "0", "0", "COPY", "REPLACE", "KEYS", "k1", "k2"], new MigrateOptions("host", 6379, 0, TimeSpan.Zero).WithCopy().WithReplace().ToArgs(["k1", "k2"]));
        Assert.Equal(["host", "6379", "", "1", "3000", "AUTH", "secret", "KEYS", "k1", "k2"], new MigrateOptions("host", 6379, 1, TimeSpan.FromSeconds(3)).WithAuth("secret").ToArgs(["k1", "k2"]));
        Assert.Equal(["host", "6379", "", "0", "0", "AUTH2", "user", "pass", "KEYS", "k1", "k2", "k3"], new MigrateOptions("host", 6379, 0, TimeSpan.Zero).WithAuth("user", "pass").ToArgs(["k1", "k2", "k3"]));
    }

    [Fact]
    public void Dispose_ThrowsAfter()
    {
        var options = BuildMigrateOptions();

        options.Dispose();

        _ = Assert.Throws<ObjectDisposedException>(() => options.WithAuth("pass"));
        _ = Assert.Throws<ObjectDisposedException>(() => options.WithAuth("user", "pass"));
        _ = Assert.Throws<ObjectDisposedException>(options.WithCopy);
        _ = Assert.Throws<ObjectDisposedException>(options.WithReplace);
        _ = Assert.Throws<ObjectDisposedException>(() => options.ToArgs(["key"]));
    }

    [Fact]
    public void Dispose_ClearsPassword()
    {
        var options = BuildMigrateOptions().WithAuth("user", "secret");
        char[] passwordRef = options.Password!;

        options.Dispose();

        Assert.Null(options.Password);
        Assert.Null(options.Username);
        Assert.All(passwordRef, c => Assert.Equal('\0', c));
    }
}
