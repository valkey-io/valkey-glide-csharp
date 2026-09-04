// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.TestUtils;

namespace Valkey.Glide.IntegrationTests;

/// <summary>
/// Integration tests verifying CLIENT INFO reports the correct lib-name
/// when LibraryName and/or ClientInfoTag are configured.
/// Requires Valkey server >= 7.2.0 (CLIENT SETINFO support).
/// </summary>
[Collection("GlideTests")]
public class ClientInfoTagTests(TestConfiguration config)
{
    public TestConfiguration Config { get; } = config;

    /// <summary>
    /// LibraryName override only → CLIENT INFO contains lib-name=custom-client
    /// </summary>
    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Data.ClusterMode), MemberType = typeof(Data))]
    public async Task TestClientInfo_WithLibNameOnly_ReportsCustomLibName(bool useCluster)
    {
        Skip.IfClientSetInfoNotSupported();

        const string customLibName = "custom-client";

        using BaseClient client = useCluster
            ? await GlideClusterClient.CreateClient(
                TestConfiguration.DefaultClusterClientConfig()
                    .WithLibraryName(customLibName)
                    .Build())
            : await GlideClient.CreateClient(
                TestConfiguration.DefaultClientConfig()
                    .WithLibraryName(customLibName)
                    .Build());

        string info = await Client.GetClientInfo(client);

        Assert.Contains($"lib-name={customLibName}", info);
        // Should NOT have parenthesized tag when no ClientInfoTag is set
        Assert.DoesNotContain($"lib-name={customLibName}(", info);
    }

    /// <summary>
    /// ClientInfoTag only → CLIENT INFO contains lib-name=GlideC#(tag)
    /// </summary>
    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Data.ClusterMode), MemberType = typeof(Data))]
    public async Task TestClientInfo_WithClientInfoTagOnly_ReportsDefaultLibNameWithTag(bool useCluster)
    {
        Skip.IfClientSetInfoNotSupported();

        const string tag = "my-framework:1.0";

        using BaseClient client = useCluster
            ? await GlideClusterClient.CreateClient(
                TestConfiguration.DefaultClusterClientConfig()
                    .WithClientInfoTag(tag)
                    .Build())
            : await GlideClient.CreateClient(
                TestConfiguration.DefaultClientConfig()
                    .WithClientInfoTag(tag)
                    .Build());

        string info = await Client.GetClientInfo(client);

        Assert.Contains($"lib-name=GlideC#({tag})", info);
    }

    /// <summary>
    /// Both LibraryName and ClientInfoTag → CLIENT INFO contains lib-name=custom-client(tag)
    /// </summary>
    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Data.ClusterMode), MemberType = typeof(Data))]
    public async Task TestClientInfo_WithLibNameAndClientInfoTag_ReportsCombined(bool useCluster)
    {
        Skip.IfClientSetInfoNotSupported();

        const string customLibName = "custom-client";
        const string tag = "my-framework:1.0";

        using BaseClient client = useCluster
            ? await GlideClusterClient.CreateClient(
                TestConfiguration.DefaultClusterClientConfig()
                    .WithLibraryName(customLibName)
                    .WithClientInfoTag(tag)
                    .Build())
            : await GlideClient.CreateClient(
                TestConfiguration.DefaultClientConfig()
                    .WithLibraryName(customLibName)
                    .WithClientInfoTag(tag)
                    .Build());

        string info = await Client.GetClientInfo(client);

        Assert.Contains($"lib-name={customLibName}({tag})", info);
    }

    /// <summary>
    /// A malformed library name is rejected by glide-core before any network activity, so client
    /// creation fails rather than silently dropping the lib-name.
    /// <para/>
    /// This pins the boundary the binding deliberately delegates: C# performs structural
    /// composition only and leaves character validation to core. If core's grammar changed, C#
    /// behaviour would change with it — this test fails loudly instead of drifting silently.
    /// <para/>
    /// Note a single matched trailing <c>(tag)</c> group is VALID (that is what makes the
    /// binding's own <c>base(tag)</c> composition legal), so the invalid paren cases below are
    /// unmatched, doubled and empty groups rather than parens as such.
    /// </summary>
    [Theory(DisableDiscoveryEnumeration = true)]
    [InlineData("Glide C#")]        // interior space
    [InlineData("GlideC#(")]        // unmatched open paren
    [InlineData("GlideC#()")]       // empty tag group
    [InlineData("GlideC#(a)(b)")]   // more than one group
    [InlineData("caf\u00e9")]       // non-ASCII
    public async Task TestClientCreation_WithMalformedLibraryName_IsRejectedByCore(string invalidLibName)
    {
        Skip.IfClientSetInfoNotSupported();

        _ = await Assert.ThrowsAnyAsync<Exception>(async () =>
        {
            await using BaseClient client = await GlideClient.CreateClient(
                TestConfiguration.DefaultClientConfig()
                    .WithLibraryName(invalidLibName)
                    .Build());
        });
    }

    /// <summary>
    /// A tag containing interior whitespace composes a value core rejects, so client creation fails.
    /// </summary>
    [Fact]
    public async Task TestClientCreation_WithInteriorWhitespaceTag_IsRejectedByCore()
    {
        Skip.IfClientSetInfoNotSupported();

        _ = await Assert.ThrowsAnyAsync<Exception>(async () =>
        {
            await using BaseClient client = await GlideClient.CreateClient(
                TestConfiguration.DefaultClientConfig()
                    .WithClientInfoTag("foo bar")
                    .Build());
        });
    }

    /// <summary>
    /// A whitespace-only tag is composed and passed through, not folded to absent, so core rejects
    /// it and client creation fails. The client defers all validation — whitespace, empty, or
    /// otherwise — to the server; a whitespace-only tag is a caller mistake and must surface loudly
    /// rather than being silently ignored.
    /// </summary>
    [Theory]
    [InlineData(" ")]
    [InlineData("   ")]
    [InlineData("\t")]
    [InlineData("")]
    public async Task TestClientCreation_WithWhitespaceOrEmptyTag_IsRejectedByCore(string tag)
    {
        Skip.IfClientSetInfoNotSupported();

        _ = await Assert.ThrowsAnyAsync<Exception>(async () =>
        {
            await using BaseClient client = await GlideClient.CreateClient(
                TestConfiguration.DefaultClientConfig()
                    .WithClientInfoTag(tag)
                    .Build());
        });
    }
}
