// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.TestUtils;

namespace Valkey.Glide.IntegrationTests;

[Collection("GlideTests")]
public class ConnectionManagementCommandTests(TestConfiguration config)
{
    #region Constants

    //TODO #414: Remove when ClientInfoAsync implemented.
    private static readonly GlideString[] InfoCommand = ["CLIENT", "INFO"];

    // Library version is set dynamically by the CD workflow,
    // and defaults to "unknown" for local and CI builds.
    private static readonly string LibVersion =
        Environment.GetEnvironmentVariable("GLIDE_VERSION") ?? "unknown";

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
        Assert.Contains($"lib-ver={LibVersion}", info);
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
    #region ClientTrackingInfo

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task ClientTrackingInfo_Off(BaseClient client)
        => AssertTrackingInfoOff(await client.ClientTrackingInfoAsync());

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClusterClients), MemberType = typeof(TestConfiguration))]
    public async Task ClientTrackingInfo_Off_WithRoute(GlideClusterClient client)
    {
        var response = await client.ClientTrackingInfoAsync(Route.AllNodes);

        Assert.NotEmpty(response.MultiValue);
        foreach (var info in response.MultiValue.Values)
        {
            AssertTrackingInfoOff(info);
        }
    }

    [Theory]
    [MemberData(nameof(Data.ClusterMode), MemberType = typeof(Data))]
    public async Task ClientTrackingInfo_On(bool clusterMode)
    {
        // TODO #435: Add builder to Options once #447 is merged.
        var cache = new ClientSideCacheConfig(1024, TimeSpan.FromMinutes(1))
            .WithServerAssisted();

        await using BaseClient client = clusterMode
            ? await GlideClusterClient.CreateClient(
                TestConfiguration.DefaultClusterClientConfig()
                    .WithClientSideCache(cache)
                    .Build())
            : await GlideClient.CreateClient(
                TestConfiguration.DefaultClientConfig()
                    .WithClientSideCache(cache)
                    .Build());

        AssertTrackingInfoOn(await client.ClientTrackingInfoAsync());
    }

    [Fact]
    public async Task ClientTrackingInfo_On_WithRoute()
    {
        // TODO #435: Add builder to Options once #447 is merged.
        var cache = new ClientSideCacheConfig(1024, TimeSpan.FromMinutes(1))
            .WithServerAssisted();

        await using var client = await GlideClusterClient.CreateClient(
            TestConfiguration.DefaultClusterClientConfig()
                .WithClientSideCache(cache)
                .Build());

        var response = await client.ClientTrackingInfoAsync(Route.AllNodes);

        Assert.NotEmpty(response.MultiValue);
        foreach (var multiInfo in response.MultiValue.Values)
        {
            AssertTrackingInfoOn(multiInfo);
        }
    }

    /// <summary>
    /// Asserts that the given <see cref="ClientTrackingInfo"/>
    /// contains the expected values when tracking is turned off.
    /// </summary>
    private static void AssertTrackingInfoOff(ClientTrackingInfo info)
    {
        Assert.Equivalent(new HashSet<string> { "off" }, info.Flags);
        Assert.Equal(-1, info.Redirect);
        Assert.Empty(info.Prefixes);
    }

    /// <summary>
    /// Asserts that the given <see cref="ClientTrackingInfo"/>
    /// contains the expected values when tracking is turned on.
    /// </summary>
    private static void AssertTrackingInfoOn(ClientTrackingInfo info)
    {
        Assert.Equivalent(new HashSet<string> { "on", "bcast" }, info.Flags);
        Assert.Equal(0, info.Redirect);
        Assert.Equivalent(new HashSet<string> { "" }, info.Prefixes);
    }

    #endregion
}
