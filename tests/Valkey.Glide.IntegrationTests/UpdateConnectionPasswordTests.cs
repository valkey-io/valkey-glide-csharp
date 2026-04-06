// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.TestUtils;

using static Valkey.Glide.Errors;
using static Valkey.Glide.TestUtils.Client;

namespace Valkey.Glide.IntegrationTests;

public class UpdateConnectionPasswordTests()
{
    private static readonly string Password = "PASSWORD";

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task UpdateConnectionPassword_DelayAuth(bool clusterMode)
    {
        using Server server = clusterMode ? new ClusterServer() : new StandaloneServer();
        await using BaseClient client = await server.CreateClientAsync();

        // Update password and verify connection.
        await client.UpdateConnectionPasswordAsync(Password, immediateAuth: false);
        await AssertConnected(client);

        // Update password, kill clients, and verify reconnection.
        await server.SetPasswordAsync(Password);
        await server.KillClientsAsync();
        await AssertReconnected(client);

        // Clear password and verify connection.
        await client.ClearConnectionPasswordAsync(immediateAuth: false);
        await AssertConnected(client);

        // Clear password, kill clients, and verify reconnection.
        await server.ClearPasswordAsync();
        await server.KillClientsAsync();
        await AssertReconnected(client);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task UpdateConnectionPassword_ImmediateAuth(bool clusterMode)
    {
        using Server server = clusterMode ? new ClusterServer() : new StandaloneServer();
        await using BaseClient client = await server.CreateClientAsync();

        // Update password and verify connection.
        await server.SetPasswordAsync(Password);
        await client.UpdateConnectionPasswordAsync(Password, immediateAuth: true);
        await AssertConnected(client);

        // Clear passwords, kill clients, and verify reconnection.
        await server.ClearPasswordAsync();
        await server.KillClientsAsync();
        await AssertConnected(client);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task UpdateConnectionPassword_InvalidPassword(bool clusterMode)
    {
        using Server server = clusterMode ? new ClusterServer() : new StandaloneServer();
        await using BaseClient client = await server.CreateClientAsync();

        _ = await Assert.ThrowsAsync<ArgumentException>(() => client.UpdateConnectionPasswordAsync(null!, immediateAuth: true));
        _ = await Assert.ThrowsAsync<ArgumentException>(() => client.UpdateConnectionPasswordAsync("", immediateAuth: true));
        _ = await Assert.ThrowsAsync<RequestException>(() => client.UpdateConnectionPasswordAsync("INVALID", immediateAuth: true));
    }
}
