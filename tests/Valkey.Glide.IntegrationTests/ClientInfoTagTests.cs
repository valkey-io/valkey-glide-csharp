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
}
