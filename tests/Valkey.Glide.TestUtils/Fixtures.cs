// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.TestUtils;

/// <summary>
/// A test fixture that provides isolated standalone and cluster servers and clusters.
/// </summary>
public class ClientFixture : IAsyncLifetime
{
    private readonly StandaloneServer _standaloneServer = new();
    private readonly ClusterServer _clusterServer = new();

    private BaseClient? _standaloneClient;
    private BaseClient? _clusterClient;

    /// <summary>
    /// Returns the client for the specified cluster mode.
    /// </summary>
    public BaseClient GetClient(bool clusterMode)
        => clusterMode ? _clusterClient! : _standaloneClient!;

    /// <inheritdoc/>
    public async ValueTask InitializeAsync()
    {
        _standaloneClient = await _standaloneServer.CreateClientAsync();
        _clusterClient = await _clusterServer.CreateClientAsync();
    }

    /// <inheritdoc/>
    public ValueTask DisposeAsync()
    {
        _standaloneClient?.Dispose();
        _clusterClient?.Dispose();
        _standaloneServer.Dispose();
        _clusterServer.Dispose();
        return ValueTask.CompletedTask;
    }
}
