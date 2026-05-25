// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.TestUtils;

/// <summary>
/// Fixture with standalone and cluster servers for integration tests.
/// </summary>
public class ServerFixture : IDisposable
{
    #region Public Properties

    // Standalone and cluster servers.
    public StandaloneServer StandaloneServer { get; }
    public ClusterServer ClusterServer { get; }

    #endregion
    #region Constructors

    public ServerFixture()
    {
        StandaloneServer = CreateStandaloneServer();
        ClusterServer = CreateClusterServer();
    }

    #endregion
    #region Public Methods

    public Server GetServer(bool clusterMode)
        => clusterMode ? ClusterServer : StandaloneServer;

    public void Dispose()
    {
        ClusterServer.Dispose();
        StandaloneServer.Dispose();
    }

    #endregion
    #region Protected Methods

    // Creates standalone or cluster server.
    protected virtual StandaloneServer CreateStandaloneServer() => new();
    protected virtual ClusterServer CreateClusterServer() => new();

    #endregion
}
