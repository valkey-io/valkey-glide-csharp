// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.TestUtils;

namespace Valkey.Glide.IntegrationTests;

/// <summary>
/// Integration tests verifying CLIENT INFO reports the correct lib-name
/// when LibName and/or ClientInfoTag are configured.
/// Requires Valkey server >= 7.2.0 (CLIENT SETINFO support).
/// </summary>
[Collection("GlideTests")]
public class ClientInfoTagTests(TestConfiguration config)
{
    private static readonly Version Valkey7_2 = new("7.2.0");
    private static readonly GlideString[] InfoCommand = ["CLIENT", "INFO"];

    public TestConfiguration Config { get; } = config;

    private static void SkipIfClientSetInfoNotSupported()
        => Assert.SkipWhen(
            TestConfiguration.SERVER_VERSION < Valkey7_2,
            "CLIENT SETINFO requires Valkey 7.2+");

    /// <summary>
    /// LibName override only → CLIENT INFO contains lib-name=custom-client
    /// </summary>
    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Data.ClusterMode), MemberType = typeof(Data))]
    public async Task TestClientInfo_WithLibNameOnly_ReportsCustomLibName(bool useCluster)
    {
        SkipIfClientSetInfoNotSupported();

        const string customLibName = "custom-client";

        using BaseClient client = useCluster
            ? await GlideClusterClient.CreateClient(
                TestConfiguration.DefaultClusterClientConfig()
                    .WithLibName(customLibName)
                    .Build())
            : await GlideClient.CreateClient(
                TestConfiguration.DefaultClientConfig()
                    .WithLibName(customLibName)
                    .Build());

        string info = await GetClientInfo(client);

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
        SkipIfClientSetInfoNotSupported();

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

        string info = await GetClientInfo(client);

        Assert.Contains($"lib-name=GlideC#({tag})", info);
    }

    /// <summary>
    /// Both LibName and ClientInfoTag → CLIENT INFO contains lib-name=custom-client(tag)
    /// </summary>
    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Data.ClusterMode), MemberType = typeof(Data))]
    public async Task TestClientInfo_WithLibNameAndClientInfoTag_ReportsCombined(bool useCluster)
    {
        SkipIfClientSetInfoNotSupported();

        const string customLibName = "custom-client";
        const string tag = "my-framework:1.0";

        using BaseClient client = useCluster
            ? await GlideClusterClient.CreateClient(
                TestConfiguration.DefaultClusterClientConfig()
                    .WithLibName(customLibName)
                    .WithClientInfoTag(tag)
                    .Build())
            : await GlideClient.CreateClient(
                TestConfiguration.DefaultClientConfig()
                    .WithLibName(customLibName)
                    .WithClientInfoTag(tag)
                    .Build());

        string info = await GetClientInfo(client);

        Assert.Contains($"lib-name={customLibName}({tag})", info);
    }

    private static async Task<string> GetClientInfo(BaseClient client)
    {
        object? result = client is GlideClusterClient clusterClient
            ? (await clusterClient.CustomCommand(InfoCommand, Route.Random)).SingleValue
            : await ((GlideClient)client).CustomCommand(InfoCommand);

        return result!.ToString()!;
    }
}
