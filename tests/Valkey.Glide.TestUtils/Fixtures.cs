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

    public virtual ValueTask InitializeAsync()
    {
        StandaloneServer = CreateStandaloneServer();
        ClusterServer = CreateClusterServer();

        return ValueTask.CompletedTask;
    }

    public virtual ValueTask DisposeAsync()
    {
        ClusterServer.Dispose();
        StandaloneServer.Dispose();

        return ValueTask.CompletedTask;
    }

    #endregion
    #region Protected Methods

    // Creates standalone or cluster server.
    // Descendants can override to customize server creation.
    protected virtual StandaloneServer CreateStandaloneServer() => new();
    protected virtual ClusterServer CreateClusterServer() => new();

    #endregion
}

/// <summary>
/// Fixture with standalone and cluster servers and clients for integration tests.
/// </summary>
public class ClientFixture : ServerFixture
{
    #region Public Properties

    /// Standalone and cluster clients.
    public GlideClient StandaloneClient { get; private set; } = null!;
    public GlideClusterClient ClusterClient { get; private set; } = null!;

    #endregion
    #region Public Methods

    public BaseClient GetClient(bool clusterMode)
        => clusterMode ? ClusterClient : StandaloneClient;

    public override async ValueTask InitializeAsync()
    {
        await base.InitializeAsync();

        StandaloneClient = await StandaloneServer.CreateStandaloneClientAsync();
        ClusterClient = await ClusterServer.CreateClusterClientAsync();
    }

    public override async ValueTask DisposeAsync()
    {
        StandaloneClient?.Dispose();
        ClusterClient?.Dispose();

        await base.DisposeAsync();
    }

    #endregion
    #region Protected Methods

    // Creates standalone or cluster clients.
    // Descendants can override to customize clients creation.
    protected virtual Task<GlideClient> CreateStandaloneClientAsync() => StandaloneServer.CreateStandaloneClientAsync();
    protected virtual Task<GlideClusterClient> CreateClusterServerAsync() => ClusterServer.CreateClusterClientAsync();

    #endregion
}
