// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using System.Diagnostics;

using Valkey.Glide.Commands.Options;
using Valkey.Glide.TestUtils;

using static Valkey.Glide.TestUtils.Builders;

namespace Valkey.Glide.IntegrationTests;

/// <summary>
/// Tests for connection management commands.
/// </summary>
[Collection(typeof(ConnectionManagementCommandTests))]
[CollectionDefinition(DisableParallelization = true)]
public class ConnectionManagementCommandTests(ServerFixture fixture) : IClassFixture<ServerFixture>
{
    #region Constants

    // TODO #414: Remove when ClientInfoAsync implemented.
    private static readonly GlideString[] InfoCommand = ["CLIENT", "INFO"];

    // Library version is set dynamically by the CD workflow,
    // and defaults to "unknown" for local and CI builds.
    private static readonly string LibVersion =
        Environment.GetEnvironmentVariable("GLIDE_VERSION") ?? "unknown";

    #endregion
    #region ClientInfoAsync

    // TODO #414: Update when ClientInfoAsync implemented.
    [Theory]
    [MemberData(nameof(Data.ClusterMode), MemberType = typeof(Data))]
    public async Task TestClientInfo_ReportsCorrectLibNameAndVersion(bool clusterMode)
    {
        await using var client = await fixture.GetServer(clusterMode).CreateClientAsync();

        var result = client is GlideClusterClient clusterClient
            ? (await clusterClient.CustomCommand(InfoCommand, Route.Random)).SingleValue
            : await ((GlideClient)client).CustomCommand(InfoCommand);
        var info = result!.ToString()!;

        Assert.Contains("lib-name=GlideC#", info);
        Assert.Contains($"lib-ver={LibVersion}", info);
        Assert.Contains("name= ", info);
    }

    // TODO #414: Update when ClientInfoAsync implemented.
    [Theory]
    [MemberData(nameof(Data.ClusterMode), MemberType = typeof(Data))]
    public async Task TestClientInfo_WithClientName_ReportsName(bool clusterMode)
    {
        const string clientName = "client";

        await using BaseClient client = clusterMode
            ? await GlideClusterClient.CreateClient(
                fixture.ClusterServer.CreateConfigBuilder()
                    .WithClientName(clientName)
                    .Build())
            : await GlideClient.CreateClient(
                fixture.StandaloneServer.CreateConfigBuilder()
                    .WithClientName(clientName)
                    .Build());

        var result = client is GlideClusterClient clusterClient
            ? (await clusterClient.CustomCommand(InfoCommand, Route.Random)).SingleValue
            : await ((GlideClient)client).CustomCommand(InfoCommand);

        Assert.Contains($"name={clientName} ", result!.ToString()!);
    }

    #endregion
    #region ClientTrackingInfoAsync

    [Theory]
    [MemberData(nameof(Data.ClusterMode), MemberType = typeof(Data))]
    public async Task ClientTrackingInfo_Off(bool clusterMode)
    {
        await using var client = await fixture.GetServer(clusterMode).CreateClientAsync();

        var info = await client.ClientTrackingInfoAsync();
        AssertTrackingInfoOff(info);
    }

    [Fact]
    public async Task ClientTrackingInfo_Off_WithRoute()
    {
        await using GlideClusterClient client = await fixture.ClusterServer.CreateClusterClientAsync();

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
        var cache = BuildClientSideCacheConfig().WithServerAssisted();

        await using BaseClient client = clusterMode
            ? await GlideClusterClient.CreateClient(
                fixture.ClusterServer.CreateConfigBuilder()
                    .WithClientSideCache(cache)
                    .Build())
            : await GlideClient.CreateClient(
                fixture.StandaloneServer.CreateConfigBuilder()
                    .WithClientSideCache(cache)
                    .Build());

        AssertTrackingInfoOn(await client.ClientTrackingInfoAsync());
    }

    [Fact]
    public async Task ClientTrackingInfo_On_WithRoute()
    {
        var cache = BuildClientSideCacheConfig().WithServerAssisted();

        await using var client = await GlideClusterClient.CreateClient(
            fixture.ClusterServer.CreateConfigBuilder()
                .WithClientSideCache(cache)
                .Build());

        var response = await client.ClientTrackingInfoAsync(Route.AllNodes);

        Assert.NotEmpty(response.MultiValue);
        foreach (var multiInfo in response.MultiValue.Values)
        {
            AssertTrackingInfoOn(multiInfo);
        }
    }

    private static void AssertTrackingInfoOff(ClientTrackingInfo info)
    {
        Assert.Equivalent(new HashSet<string> { "off" }, info.Flags);
        Assert.Equal(-1, info.Redirect);
        Assert.Empty(info.Prefixes);
    }

    private static void AssertTrackingInfoOn(ClientTrackingInfo info)
    {
        Assert.Equivalent(new HashSet<string> { "on", "bcast" }, info.Flags);
        Assert.Equal(0, info.Redirect);
        Assert.Equivalent(new HashSet<string> { "" }, info.Prefixes);
    }

    #endregion
    #region ClientPauseAsync / ClientUnpauseAsync

    [Theory]
    [MemberData(nameof(Data.ClusterMode), MemberType = typeof(Data))]
    public async Task TestClientPause_ReadsPausedUntilExpires(bool clusterMode)
    {
        // Request timeout must be longer than the pause duration.
        var pauseFor = TimeSpan.FromSeconds(1);
        var requestTimeout = pauseFor + TimeSpan.FromSeconds(1);

        await using BaseClient client = clusterMode
            ? await GlideClusterClient.CreateClient(
                fixture.ClusterServer.CreateConfigBuilder()
                    .WithRequestTimeout(requestTimeout)
                    .Build())
            : await GlideClient.CreateClient(
                fixture.StandaloneServer.CreateConfigBuilder()
                    .WithRequestTimeout(requestTimeout)
                    .Build());

        var key = Guid.NewGuid().ToString();
        await client.SetAsync(key, "value");

        var sw = Stopwatch.StartNew();
        await client.ClientPauseAsync(pauseFor);

        // Verify that read commands are blocked until the pause expires.
        _ = await client.GetAsync(key);
        Assert.True(sw.Elapsed >= pauseFor);

        await client.ClientUnpauseAsync();
    }

    [Theory]
    [MemberData(nameof(Data.ClusterMode), MemberType = typeof(Data))]
    public async Task TestClientPause_WritesPausedUntilExpires(bool clusterMode)
    {
        // Request timeout must be longer than the pause duration.
        var pauseFor = TimeSpan.FromSeconds(1);
        var requestTimeout = pauseFor + TimeSpan.FromSeconds(1);

        await using BaseClient client = clusterMode
            ? await GlideClusterClient.CreateClient(
                fixture.ClusterServer.CreateConfigBuilder()
                    .WithRequestTimeout(requestTimeout)
                    .Build())
            : await GlideClient.CreateClient(
                fixture.StandaloneServer.CreateConfigBuilder()
                    .WithRequestTimeout(requestTimeout)
                    .Build());

        var key = Guid.NewGuid().ToString();
        await client.SetAsync(key, "before");

        var sw = Stopwatch.StartNew();
        await client.ClientPauseAsync(pauseFor);

        // Verify that write commands are blocked until the pause expires.
        await client.SetAsync(key, "after");
        Assert.True(sw.Elapsed >= pauseFor);

        await client.ClientUnpauseAsync();
    }

    [Theory]
    [MemberData(nameof(Data.ClusterMode), MemberType = typeof(Data))]
    public async Task TestClientPauseWrite_ReadsNotPaused(bool clusterMode)
    {
        await using var client = await fixture.GetServer(clusterMode).CreateClientAsync();

        var key = Guid.NewGuid().ToString();
        await client.SetAsync(key, "before");

        var pauseFor = TimeSpan.FromMinutes(1);
        await client.ClientPauseWriteAsync(pauseFor);

        var sw = Stopwatch.StartNew();

        // Verify that read commands are not blocked.
        Assert.Equal("before", await client.GetAsync(key));
        Assert.True(sw.Elapsed < pauseFor);

        await client.ClientUnpauseAsync();
    }

    [Theory]
    [MemberData(nameof(Data.ClusterMode), MemberType = typeof(Data))]
    public async Task TestClientPauseWrite_ThenUnpause(bool clusterMode)
    {
        await using var client = await fixture.GetServer(clusterMode).CreateClientAsync();

        var key = Guid.NewGuid().ToString();
        await client.SetAsync(key, "before");

        var pausedFor = TimeSpan.FromMinutes(1);
        await client.ClientPauseWriteAsync(pausedFor);

        var sw = Stopwatch.StartNew();

        // Verify that write commands are unblocked once unpaused.
        await client.ClientUnpauseAsync();
        await client.SetAsync(key, "after");
        Assert.True(sw.Elapsed < pausedFor);
    }

    #endregion
    #region ResetAsync

    [Theory]
    [MemberData(nameof(Data.ClusterMode), MemberType = typeof(Data))]
    public async Task TestReset_ResetsConnectionState(bool clusterMode)
    {
        var cache = BuildClientSideCacheConfig().WithServerAssisted();

        await using BaseClient client = clusterMode
            ? await GlideClusterClient.CreateClient(
                fixture.ClusterServer.CreateConfigBuilder()
                    .WithClientSideCache(cache)
                    .Build())
            : await GlideClient.CreateClient(
                fixture.StandaloneServer.CreateConfigBuilder()
                    .WithClientSideCache(cache)
                    .Build());

        // Verify tracking is enabled.
        var infoBefore = await client.ClientTrackingInfoAsync();
        Assert.Contains("on", infoBefore.Flags);

        await client.ResetAsync();

        // Verify tracking is disabled after reset.
        var infoAfter = await client.ClientTrackingInfoAsync();
        Assert.Contains("off", infoAfter.Flags);
    }

    #endregion
    #region ClientKillAsync

    [Theory]
    [MemberData(nameof(Data.ClusterMode), MemberType = typeof(Data))]
    public async Task ClientKillAsync_ByFilter_KillsClientById(bool clusterMode)
    {
        await using var client = await fixture.GetServer(clusterMode).CreateClientAsync();
        await using var target = await fixture.GetServer(clusterMode).CreateClientAsync();

        var targetId = await target.ClientIdAsync();
        var killed = await client.ClientKillAsync(new ClientFilterOptions().WithId(targetId));

        Assert.Equal(1, killed);
    }

    [Theory]
    [MemberData(nameof(Data.ClusterMode), MemberType = typeof(Data))]
    public async Task ClientKillAsync_ByFilter_NonExistentId_ReturnsZero(bool clusterMode)
    {
        await using var client = await fixture.GetServer(clusterMode).CreateClientAsync();

        var killed = await client.ClientKillAsync(new ClientFilterOptions().WithId(999999999));

        Assert.Equal(0, killed);
    }

    [Theory]
    [MemberData(nameof(Data.ClusterMode), MemberType = typeof(Data))]
    public async Task ClientKillAsync_ByAddress_KillsClient(bool clusterMode)
    {
        await using var client = await fixture.GetServer(clusterMode).CreateClientAsync();
        await using var target = await fixture.GetServer(clusterMode).CreateClientAsync();

        var info = client is GlideClusterClient clusterClient
            ? (await clusterClient.CustomCommand(InfoCommand, Route.Random)).SingleValue!.ToString()!
            : (await ((GlideClient)client).CustomCommand(InfoCommand)).ToString()!;

        var addrField = info.Split(' ').First(f => f.StartsWith("addr="));
        var addr = addrField.Split('=')[1];
        var parts = addr.Split(':');
        var host = parts[0];
        var port = ushort.Parse(parts[1]);

        await client.ClientKillAsync(host, port);
    }

    [Fact]
    public async Task ClientKillAsync_WithRoute_KillsClientInCluster()
    {
        await using var client = await fixture.ClusterServer.CreateClusterClientAsync();
        await using var target = await fixture.ClusterServer.CreateClusterClientAsync();

        var targetId = await target.ClientIdAsync();
        var killed = await client.ClientKillAsync(
            new ClientFilterOptions().WithId(targetId), Route.AllPrimaries);

        Assert.Equal(1, killed);
    }

    #endregion
}
