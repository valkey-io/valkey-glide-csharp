// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.TestUtils;

/// <summary>
/// Fixture with standalone and cluster servers for integration tests.
/// </summary>
public class ServerFixture : IDisposable
{
    /// <summary>
    /// Standalone server instance.
    /// </summary>
    public StandaloneServer StandaloneServer { get; } = new();

    /// <summary>
    /// Cluster server instance.
    /// </summary>
    public ClusterServer ClusterServer { get; } = new();

    /// <summary>
    /// Returns the server for the given mode.
    /// </summary>
    public Server GetServer(bool clusterMode)
        => clusterMode ? ClusterServer : StandaloneServer;

    public void Dispose()
    {
        ClusterServer.Dispose();
        StandaloneServer.Dispose();
    }
}
