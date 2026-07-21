// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.TestUtils;

/// <summary>
/// Fixture with standalone and cluster servers and clients for integration tests.
/// </summary>
public class ClientFixture : ServerFixture
{
    #region Public Properties

    public GlideClient StandaloneClient { get; private set; } = null!;
    public GlideClusterClient ClusterClient { get; private set; } = null!;

    #endregion
    #region Public Methods

    public BaseClient GetClient(bool clusterMode)
        => clusterMode ? ClusterClient : StandaloneClient;

    /// <inheritdoc/>
    public override async ValueTask InitializeAsync()
    {
        await base.InitializeAsync();

        StandaloneClient = await StandaloneServer.CreateStandaloneClientAsync();
        ClusterClient = await ClusterServer.CreateClusterClientAsync();
    }

    /// <inheritdoc/>
    public override async ValueTask DisposeAsync()
    {
        StandaloneClient?.Dispose();
        ClusterClient?.Dispose();

        await base.DisposeAsync();
    }

    #endregion
}
