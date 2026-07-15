// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.TestUtils;

/// <summary>
/// Fixture with standalone and cluster servers for integration tests.
/// </summary>
public class ServerFixture : IAsyncLifetime
{
    #region Public Properties

    public StandaloneServer StandaloneServer { get; private set; } = null!;
    public ClusterServer ClusterServer { get; private set; } = null!;

    #endregion
    #region Public Methods

    public Server GetServer(bool clusterMode)
        => clusterMode ? ClusterServer : StandaloneServer;

    /// <inheritdoc/>
    public virtual ValueTask InitializeAsync()
    {
        StandaloneServer = CreateStandaloneServer();
        ClusterServer = CreateClusterServer();

        return ValueTask.CompletedTask;
    }

    /// <inheritdoc/>
    public virtual ValueTask DisposeAsync()
    {
        ClusterServer.Dispose();
        StandaloneServer.Dispose();

        return ValueTask.CompletedTask;
    }

    #endregion
    #region Protected Methods

    protected virtual StandaloneServer CreateStandaloneServer() => new();
    protected virtual ClusterServer CreateClusterServer() => new();

    #endregion
}
