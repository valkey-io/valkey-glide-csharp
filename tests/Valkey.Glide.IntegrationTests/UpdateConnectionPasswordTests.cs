// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.TestUtils;

using static Valkey.Glide.Errors;
using static Valkey.Glide.TestUtils.Client;

namespace Valkey.Glide.IntegrationTests;

/// <summary>
/// Fixture class to manage server lifecycle for update connection password tests.
/// </summary>
public class UpdateConnectionPasswordFixture : IDisposable
{
    public StandaloneServer StandaloneServer = new();
    public ClusterServer ClusterServer = new();

    public void Dispose()
    {
        StandaloneServer.Dispose();
        ClusterServer.Dispose();
    }
}

public class UpdateConnectionPasswordTests(UpdateConnectionPasswordFixture fixture)
    : IClassFixture<UpdateConnectionPasswordFixture>
{
    private static readonly string Password = "PASSWORD";
    private static readonly string InvalidPassword = "INVALID";

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task UpdateConnectionPassword_DelayAuth(bool clusterMode)
    {
        // Build clients from fixture servers.
        Server server = clusterMode ? fixture.ClusterServer : fixture.StandaloneServer;
        await using BaseClient client = await server.CreateClient();
        await using BaseClient adminClient = await server.CreateClient();

        // Update client password and verify connection.
        await client.UpdateConnectionPasswordAsync(Password, immediateAuth: false);
        await AssertConnected(client);

        // Update server password, kill all clients, and verify reconnection.
        await SetServerPassword(adminClient, Password);
        await KillClients(adminClient);
        await AssertReconnected(client);

        // Clear client connection password and verify connection.
        await client.ClearConnectionPasswordAsync(immediateAuth: false);
        await AssertConnected(client);

        // Clear server password, kill all clients, and verify reconnection.
        await ClearServerPassword(adminClient);
        await KillClients(adminClient);
        await AssertReconnected(client);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task UpdateConnectionPassword_ImmediateAuth(bool clusterMode)
    {
        // Build client from fixture server.
        Server server = clusterMode ? fixture.ClusterServer : fixture.StandaloneServer;
        await using BaseClient client = await server.CreateClient();

        // Update server and client passwords and verify connection.
        await SetServerPassword(client, Password);
        await client.UpdateConnectionPasswordAsync(Password, immediateAuth: true);
        await AssertConnected(client);

        // Clear client and server passwords and verify connection.
        await client.ClearConnectionPasswordAsync(immediateAuth: false);
        await ClearServerPassword(client);
        await AssertConnected(client);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task UpdateConnectionPassword_InvalidPassword(bool clusterMode)
    {
        Server server = clusterMode ? fixture.ClusterServer : fixture.StandaloneServer;
        await using BaseClient client = await server.CreateClient();

        _ = await Assert.ThrowsAsync<ArgumentException>(() => client.UpdateConnectionPasswordAsync(null!, immediateAuth: true));
        _ = await Assert.ThrowsAsync<ArgumentException>(() => client.UpdateConnectionPasswordAsync("", immediateAuth: true));
        _ = await Assert.ThrowsAsync<RequestException>(() => client.UpdateConnectionPasswordAsync(InvalidPassword, immediateAuth: true));
    }

    /// <summary>
    /// Sets the server password to the specified value.
    /// </summary>
    private static async Task SetServerPassword(BaseClient client, string password)
    {
        if (client is GlideClient standaloneClient)
        {
            await standaloneClient.ConfigSetAsync("requirepass", password);
        }
        else if (client is GlideClusterClient clusterClient)
        {
            await clusterClient.ConfigSetAsync("requirepass", password, Route.AllNodes);
        }
        else
        {
            Assert.Fail("Unknown client type.");
        }

        // Wait for password update to propagate.
        await Task.Delay(TimeSpan.FromSeconds(1));
    }

    /// <summary>
    /// Clears the server password.
    /// </summary>
    private static async Task ClearServerPassword(BaseClient client)
        => await SetServerPassword(client, "");

    /// <summary>
    /// Kills all normal clients.
    /// </summary>
    private static async Task KillClients(BaseClient client)
    {
        gs[] killClientCommandArgs = ["CLIENT", "KILL", "TYPE", "NORMAL"];
        if (client is GlideClient standaloneClient)
        {
            _ = await standaloneClient.CustomCommand(killClientCommandArgs);
        }
        else if (client is GlideClusterClient clusterClient)
        {
            _ = await clusterClient.CustomCommand(killClientCommandArgs);
        }
        else
        {
            Assert.Fail("Unknown client type.");
        }
    }
}
