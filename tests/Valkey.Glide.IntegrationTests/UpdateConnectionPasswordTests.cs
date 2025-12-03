// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using static Valkey.Glide.Errors;

namespace Valkey.Glide.IntegrationTests;

public class UpdateConnectionPasswordTests() : IDisposable
{
    private static readonly string ServerName = $"test_{Guid.NewGuid():N}";
    private static readonly string Password = "PASSWORD";
    private static readonly string InvalidPassword = "INVALID";
    private static readonly GlideString[] KillClientCommandArgs = ["CLIENT", "KILL", "TYPE", "NORMAL"];

    // Stop server after each test.
    public void Dispose()
    {
        ServerManager.StopServer(ServerName);
    }

    [Fact]
    public async Task UpdateConnectionPassword_Standalone_DelayAuth()
    {
        // Start server and build clients.
        var config = ServerManager.StartStandaloneServer(ServerName).Build();
        using var client = await GlideClient.CreateClient(config);
        using var adminClient = await GlideClient.CreateClient(config);

        await ServerManager.AssertConnected(client);
        await ServerManager.AssertConnected(adminClient);

        // Update client connection password.
        await client.UpdateConnectionPasswordAsync(Password, immediateAuth: false);

        await ServerManager.AssertConnected(client); // No reconnect

        // Update server password and kill all clients.
        await adminClient.ConfigSetAsync("requirepass", Password);
        await adminClient.CustomCommand(KillClientCommandArgs);
        Task.Delay(1000).Wait();

        await ServerManager.AssertConnected(client); // Reconnect

        // Clear client connection password.
        await client.ClearConnectionPasswordAsync(immediateAuth: false);

        await ServerManager.AssertConnected(client); // No reconnect

        // Clear server password and kill all clients.
        await adminClient.ConfigSetAsync("requirepass", "");
        await adminClient.CustomCommand(KillClientCommandArgs);
        Task.Delay(1000).Wait();

        await ServerManager.AssertConnected(client); // Reconnect
    }

    [Fact]
    public async Task UpdateConnectionPassword_Standalone_ImmediateAuth()
    {
        // Start server and build client.
        var config = ServerManager.StartStandaloneServer(ServerName).Build();
        using var client = await GlideClient.CreateClient(config);

        await ServerManager.AssertConnected(client);

        // Update server password.
        await client.ConfigSetAsync("requirepass", Password);
        Task.Delay(1000).Wait();

        // Update client connection password.
        await client.UpdateConnectionPasswordAsync(Password, immediateAuth: true);

        await ServerManager.AssertConnected(client);

        // Clear server password.
        await client.ConfigSetAsync("requirepass", "");
        Task.Delay(1000).Wait();

        // Clear client connection password.
        await client.ClearConnectionPasswordAsync(immediateAuth: false);

        await ServerManager.AssertConnected(client);
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
        // Start cluster and build clients.
        var config = ServerManager.StartClusterServer(ServerName).Build();
        using var client = await GlideClusterClient.CreateClient(config);
        using var adminClient = await GlideClusterClient.CreateClient(config);

        await ServerManager.AssertConnected(client);
        await ServerManager.AssertConnected(adminClient);

        // Update client connection password.
        await client.UpdateConnectionPasswordAsync(Password, immediateAuth: false);

        await ServerManager.AssertConnected(client); // No reconnect

        // Update server password and kill all clients.
        await adminClient.ConfigSetAsync("requirepass", Password);
        await adminClient.CustomCommand(KillClientCommandArgs);
        Task.Delay(1000).Wait();

        await ServerManager.AssertConnected(client); // Reconnect

        // Clear client connection password.
        await client.ClearConnectionPasswordAsync(immediateAuth: false);

        await ServerManager.AssertConnected(client); // No reconnect

        // Clear server password and kill all clients.
        await adminClient.ConfigSetAsync("requirepass", "");
        await adminClient.CustomCommand(KillClientCommandArgs);
        Task.Delay(1000).Wait();

        await ServerManager.AssertConnected(client); // Reconnect
    }

    [Fact]
    public async Task UpdateConnectionPassword_Cluster_ImmediateAuth()
    {
        // Start cluster and build client.
        var config = ServerManager.StartClusterServer(ServerName).Build();
        using var client = await GlideClusterClient.CreateClient(config);

        await ServerManager.AssertConnected(client);

        // Update server password.
        await client.ConfigSetAsync("requirepass", Password, Route.AllNodes);
        Task.Delay(1000).Wait();

        // Update client connection password.
        await client.UpdateConnectionPasswordAsync(Password, immediateAuth: true);

        await ServerManager.AssertConnected(client);

        // Clear server password.
        await client.ConfigSetAsync("requirepass", "", Route.AllNodes);
        Task.Delay(1000).Wait();

        // Clear client connection password.
        await client.ClearConnectionPasswordAsync(immediateAuth: false);

        await ServerManager.AssertConnected(client);
    }

    [Fact]
    public async Task UpdateConnectionPassword_Cluster_InvalidPassword()
    {
        using var client = TestConfiguration.DefaultClusterClient();
        await Assert.ThrowsAsync<ArgumentException>(() => client.UpdateConnectionPasswordAsync(null!, immediateAuth: true));
        await Assert.ThrowsAsync<ArgumentException>(() => client.UpdateConnectionPasswordAsync("", immediateAuth: true));
        await Assert.ThrowsAsync<RequestException>(() => client.UpdateConnectionPasswordAsync(InvalidPassword, immediateAuth: true));
    }
}
