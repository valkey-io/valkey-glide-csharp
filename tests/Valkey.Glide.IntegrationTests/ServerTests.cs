// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using System.Net;

using Valkey.Glide.TestUtils;

using static Valkey.Glide.TestUtils.Assertions;

namespace Valkey.Glide.IntegrationTests;

/// <summary>
/// Tests for <see cref="ValkeyServer"/> class.
/// </summary>
public class ServerTests(TestConfiguration config)
{
    public TestConfiguration Config { get; } = config;

    #region ClientGetNameAsync

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestConnections), MemberType = typeof(TestConfiguration))]
    public async Task ClientGetNameAsync_ReturnsNull_WhenNoNameSet(ConnectionMultiplexer conn, bool isCluster)
    {
        foreach (IServer server in conn.GetServers())
        {
            Assert.Equal(isCluster ? ServerType.Cluster : ServerType.Standalone, server.ServerType);
            Assert.Equal(ValkeyValue.Null, await server.ClientGetNameAsync());
        }
    }

    #endregion
    #region ClientIdAsync

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestConnections), MemberType = typeof(TestConfiguration))]
    public async Task ClientIdAsync_ReturnsPositiveId(ConnectionMultiplexer conn, bool isCluster)
    {
        foreach (IServer server in conn.GetServers())
        {
            Assert.Equal(isCluster ? ServerType.Cluster : ServerType.Standalone, server.ServerType);

            long clientId = await server.ClientIdAsync();
            Assert.True(clientId > 0);

            long? connectionId = await conn.GetConnectionIdAsync(server.EndPoint, ConnectionType.Interactive);
            _ = Assert.NotNull(connectionId);
            Assert.Equal(clientId, connectionId.Value);

            long? syncConnectionId = conn.GetConnectionId(server.EndPoint, ConnectionType.Interactive);
            _ = Assert.NotNull(syncConnectionId);
            Assert.Equal(clientId, syncConnectionId.Value);
        }
    }

    #endregion
    #region ClientKillAsync

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestConnections), MemberType = typeof(TestConfiguration))]
    public async Task ClientKillAsync_ByAddress_NonExistentEndpoint_DoesNotThrow(ConnectionMultiplexer conn, bool _)
    {
        var server = conn.GetServers().First();
        await server.ClientKillAsync(new IPEndPoint(IPAddress.Parse("192.0.2.1"), 9999));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestStandaloneConnections), MemberType = typeof(TestConfiguration))]
    public async Task ClientKillAsync_ByAddress_KillsClient(ConnectionMultiplexer conn)
    {
        var target = ConnectionMultiplexer.Connect(conn.RawConfig).GetServers().First();
        var targetId = await target.ClientIdAsync();

        // TODO #414: Update to use ClientInfoAsync() once available on IServer.
        var info = (await target.ExecuteAsync("CLIENT", ["INFO"])).AsString()!;
        var addr = info.Split(' ').First(f => f.StartsWith("addr=")).Split('=')[1];
        var endpoint = IPEndPoint.Parse(addr);

        var server = conn.GetServers().First();
        await server.ClientKillAsync(endpoint);

        await AssertReconnected(target);
        Assert.NotEqual(targetId, await target.ClientIdAsync());
    }

    // In Valkey, client IDs are only unique per-server. As a result, we only test killing
    // clients by ID for standalone clients, since calls to ClientIdAsync() on cluster clients
    // are routed to all nodes and so could unexpectedly kill other clients.

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestStandaloneConnections), MemberType = typeof(TestConfiguration))]
    public async Task ClientKillAsync_ById_NonExistent_ReturnsZero(ConnectionMultiplexer conn)
    {
        var server = conn.GetServers().First();
        Assert.Equal(0, await server.ClientKillAsync(id: 999999999));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestStandaloneConnections), MemberType = typeof(TestConfiguration))]
    public async Task ClientKillAsync_ById_KillsClient(ConnectionMultiplexer conn)
    {
        var server = conn.GetServers().First();
        var id = await server.ClientIdAsync();

        Assert.Equal(1, await server.ClientKillAsync(id: id, skipMe: false));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestStandaloneConnections), MemberType = typeof(TestConfiguration))]
    public async Task ClientKillAsync_WithFilterId_KillsClient(ConnectionMultiplexer conn)
    {
        var server = conn.GetServers().First();
        var id = await server.ClientIdAsync();

        var filter = new ClientKillFilter().WithId(id).WithSkipMe(false);
        Assert.Equal(1, await server.ClientKillAsync(filter));
    }

    #endregion
    #region EchoAsync

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestConnections), MemberType = typeof(TestConfiguration))]
    public async Task EchoAsync_ReturnsMessage(ConnectionMultiplexer conn, bool _)
    {
        ValkeyValue message = "hello";
        foreach (IServer server in conn.GetServers())
        {
            Assert.Equal(message, await server.EchoAsync(message));
        }
    }

    #endregion
    #region GetServers

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestConnections), MemberType = typeof(TestConfiguration))]
    public async Task GetServers_ReturnsCorrectEndpoints(ConnectionMultiplexer conn, bool isCluster)
    {
        (string host, ushort port) = isCluster ? TestConfiguration.CLUSTER_ADDRESS : TestConfiguration.STANDALONE_ADDRESS;

        Assert.Equal($"{host}:{port}", Format.ToString(conn.GetServer(host, port).EndPoint));
        Assert.Equal($"{host}:{port}", Format.ToString(conn.GetServer($"{host}:{port}").EndPoint));
        Assert.Equal($"{host}:{port}", Format.ToString(conn.GetServer(IPAddress.Parse(host), port).EndPoint));
        Assert.Equal($"{host}:{port}", Format.ToString(conn.GetServer(new IPEndPoint(IPAddress.Parse(host), port)).EndPoint));

        var count = isCluster ? TestConfiguration.CLUSTER_ADDRESSES.Count : 1;
        await Polling.WaitForAsync(
            () => Task.FromResult(conn.GetServers().Length == count),
            $"Expected {count} servers, got {conn.GetServers().Length}");
    }

    #endregion
    #region InfoAsync

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestConnections), MemberType = typeof(TestConfiguration))]
    public async Task InfoAsync_ReturnsServerInfo(ConnectionMultiplexer conn, bool isCluster)
    {
        foreach (IServer server in conn.GetServers())
        {
            Assert.Equal(conn.RawConfig.Protocol, server.Protocol);
            Assert.Equal(TestConfiguration.SERVER_VERSION, server.Version);
            Assert.Equal(isCluster ? ServerType.Cluster : ServerType.Standalone, server.ServerType);

            string info = (await server.InfoRawAsync("server"))!;
            foreach (string line in info.Split("\r\n"))
            {
                if (line.Contains("tcp_port:"))
                {
                    Assert.Contains(Format.ToString(server.EndPoint).Split(':')[1], line);
                    break;
                }
            }

            ValkeyResult res = await server.ExecuteAsync("info", ["server"]);
            foreach (string line in res.AsString()!.Split("\r\n"))
            {
                if (line.Contains("tcp_port:"))
                {
                    Assert.Contains(server.EndPoint.ToString()!.Split(':')[1], line);
                    break;
                }
            }

            IGrouping<string, KeyValuePair<string, string>>[] infoParsed = await server.InfoAsync();
            foreach (IGrouping<string, KeyValuePair<string, string>> data in infoParsed)
            {
                if (data.Key == "Server")
                {
                    bool portFound = false;
                    foreach (KeyValuePair<string, string> pair in data)
                    {
                        if (pair.Key == "tcp_port")
                        {
                            Assert.Equal(pair.Value, Format.ToString(server.EndPoint).Split(':')[1]);
                            portFound = true;
                            break;
                        }
                    }
                    Assert.True(portFound);
                    break;
                }
            }
        }
    }

    #endregion
    #region PingAsync

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestConnections), MemberType = typeof(TestConfiguration))]
    public async Task PingAsync_ReturnsPositiveLatency(ConnectionMultiplexer conn, bool _)
    {
        foreach (IServer server in conn.GetServers())
        {
            Assert.True(await server.PingAsync() > TimeSpan.Zero);
        }
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestConnections), MemberType = typeof(TestConfiguration))]
    public async Task PingAsync_WithMessage_ReturnsPositiveLatency(ConnectionMultiplexer conn, bool _)
    {
        ValkeyValue message = "hello";
        foreach (IServer server in conn.GetServers())
        {
            Assert.True(await server.PingAsync(message) > TimeSpan.Zero);
        }
    }

    #endregion
}
