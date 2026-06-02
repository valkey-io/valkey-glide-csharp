// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.TestUtils;

/// <summary>
/// Fixture with standalone and cluster servers for integration tests.
/// </summary>
public class ServerFixture : IAsyncLifetime
{
    #region Public Properties

    // Standalone and cluster servers.
    public StandaloneServer StandaloneServer { get; private set; } = null!;
    public ClusterServer ClusterServer { get; private set; } = null!;

    #endregion
    #region Public Methods

    public Server GetServer(bool clusterMode)
        => clusterMode ? ClusterServer : StandaloneServer;

    public ValueTask InitializeAsync()
    {
        StandaloneServer = CreateStandaloneServer();
        ClusterServer = CreateClusterServer();

        return ValueTask.CompletedTask;
    }

    public ValueTask DisposeAsync()
    {
        ClusterServer.Dispose();
        StandaloneServer.Dispose();

        return ValueTask.CompletedTask;
    }

    #endregion
    #region Protected Methods

    // Creates standalone or cluster server.
    // Desccendant can override to customize server creation.
    protected virtual StandaloneServer CreateStandaloneServer() => new();
    protected virtual ClusterServer CreateClusterServer() => new();

    #endregion
}
