// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.IntegrationTests;

public class DatabaseTests(TestConfiguration config)
{
    public TestConfiguration Config { get; } = config;

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestConnections), MemberType = typeof(TestConfiguration))]
    public async Task Basic(ConnectionMultiplexer conn, bool isCluster)
    {
        var db = conn.GetDatabase();
        string key = Guid.NewGuid().ToString();

        ValkeyValue result = await db.StringGetAsync(key);
        Assert.True(result.IsNull);
        await db.StringSetAsync(key, "val");
        ValkeyValue retrievedValue = await db.StringGetAsync(key);
        Assert.Equal("val", retrievedValue.ToString());

        // InfoAsync is a server management command on the GLIDE client layer, not on IDatabase.
        // Use IServer.InfoRawAsync to get server info through the SER-compatible path.
        IServer server = conn.GetServer(conn.GetEndPoints(true).First());
        string? info = await server.InfoRawAsync("cluster");

        Assert.True(isCluster
            ? info!.Contains("cluster_enabled:1")
            : info!.Contains("cluster_enabled:0"));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestConnections), MemberType = typeof(TestConfiguration))]
    public async Task PingAsync_Succeeds(ConnectionMultiplexer conn, bool _)
    {
        IDatabase db = conn.GetDatabase();

        TimeSpan ping = await db.PingAsync();
        Assert.True(ping > TimeSpan.Zero);
    }
}
