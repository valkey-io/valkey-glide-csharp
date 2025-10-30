// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Internals;

using static Valkey.Glide.ConnectionConfiguration;

namespace Valkey.Glide.UnitTests;

public class ConnectionConfigurationTests
{
    [Fact]
    public void RefreshTopologyFromInitialNodes()
    {
        ClusterClientConfigurationBuilder builder;
        ClusterClientConfiguration config;

        // Default (false).
        builder = new ClusterClientConfigurationBuilder();
        config = builder.Build();
        Assert.False(config.Request.RefreshTopologyFromInitialNodes);

        // Set to true.
        builder = new ClusterClientConfigurationBuilder();
        builder.WithRefreshTopologyFromInitialNodes(true);
        config = builder.Build();
        Assert.True(config.Request.RefreshTopologyFromInitialNodes);

        // Set to false.
        builder = new ClusterClientConfigurationBuilder();
        builder.WithRefreshTopologyFromInitialNodes(false);
        config = builder.Build();
        Assert.False(config.Request.RefreshTopologyFromInitialNodes);
    }
}
