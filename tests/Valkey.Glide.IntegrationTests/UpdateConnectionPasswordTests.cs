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

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task UpdateConnectionPassword_DelayAuth(bool clusterMode)
    {
        using Server server = clusterMode ? fixture.ClusterServer : fixture.StandaloneServer;
        await using BaseClient client = await server.CreateClient();

        // Update password and verify connection.
        await client.UpdateConnectionPasswordAsync(Password, immediateAuth: false);
        await AssertConnected(client);

        // Update password, kill clients, and verify reconnection.
        await server.SetPassword(Password);
        await server.KillClients();
        await AssertReconnected(client);

        // Clear password and verify connection.
        await client.ClearConnectionPasswordAsync(immediateAuth: false);
        await AssertConnected(client);

        // Clear password, kill clients, and verify reconnection.
        await server.ClearPassword();
        await server.KillClients();
        await AssertReconnected(client);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task UpdateConnectionPassword_ImmediateAuth(bool clusterMode)
    {
        using Server server = clusterMode ? fixture.ClusterServer : fixture.StandaloneServer;
        await using BaseClient client = await server.CreateClient();

        // Update password and verify connection.
        await server.SetPassword(Password);
        await client.UpdateConnectionPasswordAsync(Password, immediateAuth: true);
        await AssertConnected(client);

        // Clear passwords, kill clients, and verify reconnection.
        await server.ClearPassword();
        await server.KillClients();
        await AssertConnected(client);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task UpdateConnectionPassword_InvalidPassword(bool clusterMode)
    {
        using Server server = clusterMode ? fixture.ClusterServer : fixture.StandaloneServer;
        await using BaseClient client = await server.CreateClient();

        _ = await Assert.ThrowsAsync<ArgumentException>(() => client.UpdateConnectionPasswordAsync(null!, immediateAuth: true));
        _ = await Assert.ThrowsAsync<ArgumentException>(() => client.UpdateConnectionPasswordAsync("", immediateAuth: true));
        _ = await Assert.ThrowsAsync<RequestException>(() => client.UpdateConnectionPasswordAsync("INVALID", immediateAuth: true));
    }
}
