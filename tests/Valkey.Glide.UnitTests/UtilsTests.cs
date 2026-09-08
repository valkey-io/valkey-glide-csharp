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

    [Fact]
    public void ResolveLibraryName_NoOverrides_ReturnsDefault()
        => Assert.Equal("GlideC#", Utils.ResolveLibraryName(null, null));

    [Fact]
    public void ResolveLibraryName_LibraryNameOverride_ReplacesDefault()
        => Assert.Equal("custom-lib", Utils.ResolveLibraryName("custom-lib", null));

    [Fact]
    public void ResolveLibraryName_Tag_AppendsToDefault()
        => Assert.Equal("GlideC#(my-framework:1.0)", Utils.ResolveLibraryName(null, "my-framework:1.0"));

    [Fact]
    public void ResolveLibraryName_LibraryNameAndTag_AppendsToOverride()
        => Assert.Equal("custom(lmcache:1.2)", Utils.ResolveLibraryName("custom", "lmcache:1.2"));

    [Fact]
    public void ResolveLibraryName_NullTag_LeavesDefaultUnchanged()
        => Assert.Equal("GlideC#", Utils.ResolveLibraryName(null, null));

    /// <summary>
    /// An empty tag is composed and passed through, not folded to absent — the client defers all
    /// validation, whitespace or otherwise, to the server. Contrast with <see langword="null"/>,
    /// which is the only value treated as "no tag" (<see cref="ResolveLibraryName_NullTag_LeavesDefaultUnchanged"/>).
    /// <c>GlideC#()</c> fails GLIDE core's grammar (a matched trailing group must be non-empty), so
    /// this composes a value the server will reject, by design.
    /// </summary>
    [Fact]
    public void ResolveLibraryName_EmptyTag_IsComposedAndLeftToCore()
        => Assert.Equal("GlideC#()", Utils.ResolveLibraryName(null, ""));

    [Theory]
    [InlineData(" ")]
    [InlineData("   ")]
    [InlineData("\t")]
    [InlineData("\n")]
    [InlineData(" \t \n ")]
    public void ResolveLibraryName_WhitespaceOnlyTag_IsComposedAndLeftToCore(string tag) =>
        // Whitespace-only tags are NOT folded away. Per the maintainer directive that bindings
        // compose and core validates, the tag is composed as-is and passed through; glide-core's
        // library-name grammar then rejects it (space is 0x20, below the \x21 floor), failing
        // client creation with a configuration error. That rejection is the intended outcome —
        // a whitespace-only tag is a caller mistake and the caller is told so.
        // Only null/empty are treated as absent, which is what keeps "GlideC#()" unreachable.
        Assert.Equal($"GlideC#({tag})", Utils.ResolveLibraryName(null, tag));

    [Fact]
    public void ResolveLibraryName_WhitespaceOnlyTag_WithLibraryName_IsComposed()
        => Assert.Equal("custom(   )", Utils.ResolveLibraryName("custom", "   "));

    [Fact]
    public void ResolveLibraryName_LibraryNameOnly_Cluster_OverridesDefault()
        => Assert.Equal("cluster-lib", Utils.ResolveLibraryName("cluster-lib", null));

    [Fact]
    public void ResolveLibraryName_TagOnly_Cluster_AppendsToDefault()
        => Assert.Equal("GlideC#(my-svc:2.0)", Utils.ResolveLibraryName(null, "my-svc:2.0"));
}
