// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.TestUtils;

using static Valkey.Glide.Errors;
using static Valkey.Glide.TestUtils.Client;

namespace Valkey.Glide.IntegrationTests;

public class UpdateConnectionPasswordTests
{
    private static readonly string Password = "PASSWORD";
    private static readonly string InvalidPassword = "INVALID";

    [Theory]
    [InlineData(true)]
    [InlineData(false)]

    public async Task UpdateConnectionPassword_DelayAuth(bool clusterMode)
    {
        // Start server and build clients.
        using Server server = clusterMode ? new ClusterServer() : new StandaloneServer();

        await using BaseClient client = await server.CreateClient();
        await AssertConnected(client);

        await using BaseClient adminClient = await server.CreateClient();
        await AssertConnected(adminClient);

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
        // Start server and build client.
        using Server server = clusterMode ? new ClusterServer() : new StandaloneServer();

        await using BaseClient client = await server.CreateClient();
        await AssertConnected(client);

        // Update server and client passwords and verify connection.
        await SetServerPassword(client, Password);
        await client.UpdateConnectionPasswordAsync(Password, immediateAuth: true);
        await AssertConnected(client);

        // Clear client and server passwords and verify connection.
        await client.ClearConnectionPasswordAsync(immediateAuth: false);
        await ClearServerPassword(client);
        await AssertConnected(client);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task UpdateConnectionPassword_Standalone_InvalidPassword(BaseClient client)
    {
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
