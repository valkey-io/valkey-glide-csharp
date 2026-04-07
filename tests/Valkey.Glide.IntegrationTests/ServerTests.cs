// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using System.Net;

namespace Valkey.Glide.IntegrationTests;

/// <summary>
/// Tests for <see cref="ValkeyServer" /> class.
/// </summary>
public class ServerTests(TestConfiguration config)
{
    public TestConfiguration Config { get; } = config;

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestConnections), MemberType = typeof(TestConfiguration))]
    public void CanGetServers(ConnectionMultiplexer conn, bool isCluster)
    {
        (string host, ushort port) = isCluster ? TestConfiguration.CLUSTER_ADDRESS : TestConfiguration.STANDALONE_ADDRESS;

        Assert.Equal($"{host}:{port}", Format.ToString(conn.GetServer(host, port).EndPoint));
        Assert.Equal($"{host}:{port}", Format.ToString(conn.GetServer($"{host}:{port}").EndPoint));
        Assert.Equal($"{host}:{port}", Format.ToString(conn.GetServer(IPAddress.Parse(host), port).EndPoint));
        Assert.Equal($"{host}:{port}", Format.ToString(conn.GetServer(new IPEndPoint(IPAddress.Parse(host), port)).EndPoint));

        // TODO currently this returns only primary node on standalone
        // https://github.com/valkey-io/valkey-glide/issues/4293
        var expectedServerCount = isCluster ? TestConfiguration.CLUSTER_ADDRESSES.Count : 1;
        Assert.Equal(expectedServerCount, conn.GetServers().Length);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestConnections), MemberType = typeof(TestConfiguration))]
    public async Task CanGetServerInfo(ConnectionMultiplexer conn, bool isCluster)
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

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestConnections), MemberType = typeof(TestConfiguration))]
    public async Task PingAsync_Succeeds(ConnectionMultiplexer conn, bool _)
    {
        foreach (IServer server in conn.GetServers())
        {
            TimeSpan ping = await server.PingAsync();
            Assert.True(ping > TimeSpan.Zero);
        }
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestConnections), MemberType = typeof(TestConfiguration))]
    public async Task PingAsync_WithMessage_Succeeds(ConnectionMultiplexer conn, bool _)
    {
        ValkeyValue message = "hello";
        foreach (IServer server in conn.GetServers())
        {
            TimeSpan ping = await server.PingAsync(message);
            Assert.True(ping > TimeSpan.Zero);
        }
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestConnections), MemberType = typeof(TestConfiguration))]
    public async Task EchoAsync_Succeeds(ConnectionMultiplexer conn, bool _)
    {
        ValkeyValue message = "hello";
        foreach (IServer server in conn.GetServers())
        {
            ValkeyValue echo = await server.EchoAsync(message);
            Assert.Equal(message, echo);
        }
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestConnections), MemberType = typeof(TestConfiguration))]
    public async Task CanGetClientId(ConnectionMultiplexer conn, bool isCluster)
    {
        foreach (IServer server in conn.GetServers())
        {
            Assert.Equal(isCluster ? ServerType.Cluster : ServerType.Standalone, server.ServerType);

            // Test CLIENT ID command directly
            long clientId = await server.ClientIdAsync();
            Assert.True(clientId > 0, "Client ID should be a positive number");

            // Test GetConnectionId from ConnectionMultiplexer (matching SER pattern)
            long? connectionId = await conn.GetConnectionIdAsync(server.EndPoint, ConnectionType.Interactive);
            _ = Assert.NotNull(connectionId);
            Assert.Equal(clientId, connectionId.Value);

            // Test synchronous version
            long? syncConnectionId = conn.GetConnectionId(server.EndPoint, ConnectionType.Interactive);
            _ = Assert.NotNull(syncConnectionId);
            Assert.Equal(clientId, syncConnectionId.Value);
        }
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestConnections), MemberType = typeof(TestConfiguration))]
    public async Task CanGetClientName(ConnectionMultiplexer conn, bool isCluster)
    {
        foreach (IServer server in conn.GetServers())
        {
            Assert.Equal(isCluster ? ServerType.Cluster : ServerType.Standalone, server.ServerType);

            // Test CLIENT GETNAME command - should return ValkeyValue.Null initially (no name set)
            ValkeyValue clientName = await server.ClientGetNameAsync();
            Assert.Equal(ValkeyValue.Null, clientName); // No name should be set initially
        }
    }
}
