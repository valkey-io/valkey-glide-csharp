// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.TestUtils;

using static Valkey.Glide.Errors;
using static Valkey.Glide.TestUtils.Client;

namespace Valkey.Glide.IntegrationTests;

public class UpdateConnectionPasswordTests
{
    private static readonly string Password = "PASSWORD";
    private static readonly string InvalidPassword = "INVALID";
    private static readonly GlideString[] KillClientCommandArgs = ["CLIENT", "KILL", "TYPE", "NORMAL"];

    [Fact]
    public async Task UpdateConnectionPassword_Standalone_DelayAuth()
    {
        // Start server and build clients.
        using var server = new StandaloneServer();
        var config = server.CreateConfigBuilder().Build();

        await using var client = await GlideClient.CreateClient(config);
        await AssertConnected(client);

        await using var adminClient = await GlideClient.CreateClient(config);
        await AssertConnected(adminClient);

        // Update client connection password.
        await client.UpdateConnectionPasswordAsync(Password, immediateAuth: false);

        await AssertConnected(client); // No reconnect

        // Update server password and kill all clients.
        await adminClient.ConfigSetAsync("requirepass", Password);
        await adminClient.CustomCommand(KillClientCommandArgs);
        Task.Delay(1000).Wait();

        await AssertConnected(client); // Reconnect

        // Clear client connection password.
        await client.ClearConnectionPasswordAsync(immediateAuth: false);

        await AssertConnected(client); // No reconnect

        // Clear server password and kill all clients.
        await adminClient.ConfigSetAsync("requirepass", "");
        await adminClient.CustomCommand(KillClientCommandArgs);
        Task.Delay(1000).Wait();

        await AssertConnected(client); // Reconnect
    }

    [Fact]
    public async Task UpdateConnectionPassword_Standalone_ImmediateAuth()
    {
        // Start server and build client.
        using var server = new StandaloneServer();
        var config = server.CreateConfigBuilder().Build();

        await using var client = await GlideClient.CreateClient(config);
        await AssertConnected(client);

        // Update server password.
        await client.ConfigSetAsync("requirepass", Password);
        Task.Delay(1000).Wait();

        // Update client connection password.
        await client.UpdateConnectionPasswordAsync(Password, immediateAuth: true);

        await AssertConnected(client);

        // Clear server password.
        await client.ConfigSetAsync("requirepass", "");
        Task.Delay(1000).Wait();

        // Clear client connection password.
        await client.ClearConnectionPasswordAsync(immediateAuth: false);

        await AssertConnected(client);
    }

    [Fact]
    public async Task UpdateConnectionPassword_Standalone_InvalidPassword()
    {
        await using var client = TestConfiguration.DefaultStandaloneClient();
        await Assert.ThrowsAsync<ArgumentException>(() => client.UpdateConnectionPasswordAsync(null!, immediateAuth: true));
        await Assert.ThrowsAsync<ArgumentException>(() => client.UpdateConnectionPasswordAsync("", immediateAuth: true));
        await Assert.ThrowsAsync<RequestException>(() => client.UpdateConnectionPasswordAsync(InvalidPassword, immediateAuth: true));
    }

    [Fact]
    public async Task UpdateConnectionPassword_Cluster_DelayAuth()
    {
        // Start cluster and build clients.
        using var server = new ClusterServer();
        var config = server.CreateConfigBuilder().Build();

        await using var client = await GlideClusterClient.CreateClient(config);
        await AssertConnected(client);

        await using var adminClient = await GlideClusterClient.CreateClient(config);
        await AssertConnected(adminClient);

        // Update client connection password.
        await client.UpdateConnectionPasswordAsync(Password, immediateAuth: false);

        await AssertConnected(client); // No reconnect

        // Update server password and kill all clients.
        await adminClient.ConfigSetAsync("requirepass", Password);
        await adminClient.CustomCommand(KillClientCommandArgs);
        Task.Delay(1000).Wait();

        await AssertConnected(client); // Reconnect

        // Clear client connection password.
        await client.ClearConnectionPasswordAsync(immediateAuth: false);

        await AssertConnected(client); // No reconnect

        // Clear server password and kill all clients.
        await adminClient.ConfigSetAsync("requirepass", "");
        await adminClient.CustomCommand(KillClientCommandArgs);
        Task.Delay(1000).Wait();

        await AssertConnected(client); // Reconnect
    }

    [Fact]
    public async Task UpdateConnectionPassword_Cluster_ImmediateAuth()
    {
        // Start cluster and build client.
        using var server = new ClusterServer();
        var config = server.CreateConfigBuilder().Build();

        await using var client = await GlideClusterClient.CreateClient(config);
        await AssertConnected(client);

        // Update server password.
        await client.ConfigSetAsync("requirepass", Password, Route.AllNodes);
        Task.Delay(1000).Wait();

        // Update client connection password.
        await client.UpdateConnectionPasswordAsync(Password, immediateAuth: true);

        await AssertConnected(client);

        // Clear server password.
        await client.ConfigSetAsync("requirepass", "", Route.AllNodes);
        Task.Delay(1000).Wait();

        // Clear client connection password.
        await client.ClearConnectionPasswordAsync(immediateAuth: false);

        await AssertConnected(client);
    }

    [Fact]
    public async Task UpdateConnectionPassword_Cluster_InvalidPassword()
    {
        await using var client = TestConfiguration.DefaultClusterClient();
        await Assert.ThrowsAsync<ArgumentException>(() => client.UpdateConnectionPasswordAsync(null!, immediateAuth: true));
        await Assert.ThrowsAsync<ArgumentException>(() => client.UpdateConnectionPasswordAsync("", immediateAuth: true));
        await Assert.ThrowsAsync<RequestException>(() => client.UpdateConnectionPasswordAsync(InvalidPassword, immediateAuth: true));
    }
}
