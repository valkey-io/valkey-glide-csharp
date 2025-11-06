// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using static Valkey.Glide.ConnectionConfiguration;
using static Valkey.Glide.Errors;

namespace Valkey.Glide.IntegrationTests;

public class UpdateConnectionPasswordTests(TestConfiguration config)
{
    public TestConfiguration Config { get; } = config;

    private static readonly string Password = "PASSWORD";
    private static readonly string InvalidPassword = "INVALID";
    private static readonly GlideString[] KillClientCommandArgs = ["CLIENT", "KILL", "TYPE", "NORMAL"];

    [Fact]
    public async Task UpdateConnectionPassword_Standalone_DelayAuth()
    {
        string serverName = $"test_{Guid.NewGuid():N}";

        try
        {
            // Start server and build clients.
            var addresses = ServerManager.StartStandaloneServer(serverName);
            var config = new StandaloneClientConfigurationBuilder()
                .WithAddress(addresses[0].host, addresses[0].port).Build();

            using var client = await GlideClient.CreateClient(config);
            using var adminClient = await GlideClient.CreateClient(config);

            VerifyConnection(client);
            VerifyConnection(adminClient);

            // Update client connection password.
            await client.UpdateConnectionPasswordAsync(Password, immediateAuth: false);

            VerifyConnection(client); // No reconnect

            // Update server password and kill all clients.
            await adminClient.ConfigSetAsync("requirepass", Password);
            await adminClient.CustomCommand(KillClientCommandArgs);
            Task.Delay(1000).Wait();

            VerifyConnection(client); // Reconnect

            // Clear client connection password.
            await client.ClearConnectionPasswordAsync(immediateAuth: false);

            VerifyConnection(client); // No reconnect

            // Clear server password and kill all clients.
            await adminClient.ConfigSetAsync("requirepass", "");
            await adminClient.CustomCommand(KillClientCommandArgs);
            Task.Delay(1000).Wait();

            VerifyConnection(client); // Reconnect
        }
        finally
        {
            ServerManager.StopServer(serverName);
        }
    }

    [Fact]
    public async Task UpdateConnectionPassword_Standalone_ImmediateAuth()
    {
        string serverName = $"test_{Guid.NewGuid():N}";

        try
        {
            // Start server and build client.
            var addresses = ServerManager.StartStandaloneServer(serverName);
            var config = new StandaloneClientConfigurationBuilder()
                .WithAddress(addresses[0].host, addresses[0].port).Build();

            using var client = await GlideClient.CreateClient(config);

            VerifyConnection(client);

            // Update server password.
            await client.ConfigSetAsync("requirepass", Password);
            Task.Delay(1000).Wait();

            // Update client connection password.
            await client.UpdateConnectionPasswordAsync(Password, immediateAuth: true);

            VerifyConnection(client);

            // Clear server password.
            await client.ConfigSetAsync("requirepass", "");
            Task.Delay(1000).Wait();

            // Clear client connection password.
            await client.ClearConnectionPasswordAsync(immediateAuth: false);

            VerifyConnection(client);
        }
        finally
        {
            ServerManager.StopServer(serverName);
        }
    }

    [Fact]
    public async Task UpdateConnectionPassword_Standalone_InvalidPassword()
    {
        using var client = TestConfiguration.DefaultStandaloneClient();
        await Assert.ThrowsAsync<ArgumentException>(() => client.UpdateConnectionPasswordAsync(null!, immediateAuth: true));
        await Assert.ThrowsAsync<ArgumentException>(() => client.UpdateConnectionPasswordAsync("", immediateAuth: true));
        await Assert.ThrowsAsync<RequestException>(() => client.UpdateConnectionPasswordAsync(InvalidPassword, immediateAuth: true));
    }

    [Fact]
    public async Task UpdateConnectionPassword_Cluster_DelayAuth()
    {
        string serverName = $"test_{Guid.NewGuid():N}";

        try
        {
            // Start cluster and build clients.
            var addresses = ServerManager.StartClusterServer(serverName);
            var config = new ClusterClientConfigurationBuilder()
                .WithAddress(addresses[0].host, addresses[0].port).Build();

            using var client = await GlideClusterClient.CreateClient(config);
            using var adminClient = await GlideClusterClient.CreateClient(config);

            VerifyConnection(client);
            VerifyConnection(adminClient);

            // Update client connection password.
            await client.UpdateConnectionPasswordAsync(Password, immediateAuth: false);

            VerifyConnection(client); // No reconnect

            // Update server password and kill all clients.
            await adminClient.ConfigSetAsync("requirepass", Password);
            await adminClient.CustomCommand(KillClientCommandArgs);
            Task.Delay(1000).Wait();

            VerifyConnection(client); // Reconnect

            // Clear client connection password.
            await client.ClearConnectionPasswordAsync(immediateAuth: false);

            VerifyConnection(client); // No reconnect

            // Clear server password and kill all clients.
            await adminClient.ConfigSetAsync("requirepass", "");
            await adminClient.CustomCommand(KillClientCommandArgs);
            Task.Delay(1000).Wait();

            VerifyConnection(client); // Reconnect
        }
        finally
        {
            ServerManager.StopServer(serverName);
        }
    }

    [Fact]
    public async Task UpdateConnectionPassword_Cluster_ImmediateAuth()
    {
        string serverName = $"test_{Guid.NewGuid():N}";

        try
        {
            // Start cluster and build client.
            var addresses = ServerManager.StartClusterServer(serverName);
            var config = new ClusterClientConfigurationBuilder()
                .WithAddress(addresses[0].host, addresses[0].port).Build();

            using var client = await GlideClusterClient.CreateClient(config);

            VerifyConnection(client);

            // Update server password.
            await client.ConfigSetAsync("requirepass", Password, Route.AllNodes);
            Task.Delay(1000).Wait();

            // Update client connection password.
            await client.UpdateConnectionPasswordAsync(Password, immediateAuth: true);

            VerifyConnection(client);

            // Clear server password.
            await client.ConfigSetAsync("requirepass", "", Route.AllNodes);
            Task.Delay(1000).Wait();

            // Clear client connection password.
            await client.ClearConnectionPasswordAsync(immediateAuth: false);

            VerifyConnection(client);
        }
        finally
        {
            ServerManager.StopServer(serverName);
        }
    }

    [Fact]
    public async Task UpdateConnectionPassword_Cluster_InvalidPassword()
    {
        using var client = TestConfiguration.DefaultClusterClient();
        await Assert.ThrowsAsync<ArgumentException>(() => client.UpdateConnectionPasswordAsync(null!, immediateAuth: true));
        await Assert.ThrowsAsync<ArgumentException>(() => client.UpdateConnectionPasswordAsync("", immediateAuth: true));
        await Assert.ThrowsAsync<RequestException>(() => client.UpdateConnectionPasswordAsync(InvalidPassword, immediateAuth: true));
    }

    private static async void VerifyConnection(GlideClient client)
    {
        Assert.True(await client.PingAsync() > TimeSpan.Zero);
    }

    private static async void VerifyConnection(GlideClusterClient client)
    {
        Assert.True(await client.PingAsync() > TimeSpan.Zero);
    }
}
