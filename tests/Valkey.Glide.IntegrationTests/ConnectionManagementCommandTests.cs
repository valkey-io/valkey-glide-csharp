// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.TestUtils;

namespace Valkey.Glide.IntegrationTests;

[Collection("GlideTests")]
public class ConnectionManagementCommandTests(TestConfiguration config)
{
    #region Constants

    //TODO #414: Remove when ClientInfoAsync implemented.
    private readonly GlideString[] InfoCommand = ["CLIENT", "INFO"];

    #endregion
    #region Public Properties

    public TestConfiguration Config { get; } = config;

    #endregion
    #region Tests

    // TODO #414: Update when ClientInfoAsync implemented.
    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestClientInfo_ReportsCorrectLibNameAndVersion(BaseClient client)
    {
        var result = client is GlideClusterClient clusterClient
            ? (await clusterClient.CustomCommand(InfoCommand, Route.Random)).SingleValue
            : await ((GlideClient)client).CustomCommand(InfoCommand);
        var info = result!.ToString()!;

        Assert.Contains("lib-name=GlideC#", info);
        Assert.Contains("lib-ver=1.2.0", info);
        Assert.Contains("name= ", info);
    }

    // TODO #414: Update when ClientInfoAsync implemented.
    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Data.ClusterMode), MemberType = typeof(Data))]
    public async Task TestClientInfo_WithClientName_ReportsName(bool useCluster)
    {
        const string clientName = "client";
        using BaseClient client = useCluster
            ? await GlideClusterClient.CreateClient(
                TestConfiguration.DefaultClusterClientConfig()
                    .WithClientName(clientName)
                    .Build())
            : await GlideClient.CreateClient(
                TestConfiguration.DefaultClientConfig()
                    .WithClientName(clientName)
                    .Build());

        var result = client is GlideClusterClient clusterClient
            ? (await clusterClient.CustomCommand(InfoCommand, Route.Random)).SingleValue
            : await ((GlideClient)client).CustomCommand(InfoCommand);

        Assert.Contains($"name={clientName} ", result!.ToString()!);
    }

    #endregion
}
