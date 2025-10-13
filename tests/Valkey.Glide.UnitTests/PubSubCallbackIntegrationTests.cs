// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using System;

using Valkey.Glide.Internals;

using Xunit;

using static Valkey.Glide.ConnectionConfiguration;

namespace Valkey.Glide.UnitTests;

/// <summary>
/// Integration tests for PubSub callback registration with FFI client creation.
/// Note: Tests involving MockBaseClient have been moved to integration tests (task 7.6)
/// to avoid test infrastructure issues while ensuring proper test coverage.
/// </summary>
public class PubSubCallbackIntegrationTests
{
    [Fact]
    public void PubSubCallbackManager_GetNativeCallbackPtr_ReturnsValidPointer()
    {
        // Arrange & Act
        IntPtr callbackPtr = PubSubCallbackManager.GetNativeCallbackPtr();

        // Assert
        Assert.NotEqual(IntPtr.Zero, callbackPtr);
    }

    [Fact]
    public void StandaloneClientConfiguration_WithPubSubSubscriptions_IncludesPubSubConfig()
    {
        // Arrange
        StandalonePubSubSubscriptionConfig pubsubConfig = new StandalonePubSubSubscriptionConfig()
            .WithChannel("test-channel")
            .WithPattern("test-*")
            .WithCallback<StandalonePubSubSubscriptionConfig>((message, context) => { /* test callback */ });

        // Act
        StandaloneClientConfiguration clientConfig = new StandaloneClientConfigurationBuilder()
            .WithAddress("localhost", 6379)
            .WithPubSubSubscriptions(pubsubConfig)
            .Build();

        // Assert
        Assert.NotNull(clientConfig.Request.PubSubSubscriptions);
        Assert.Same(pubsubConfig, clientConfig.Request.PubSubSubscriptions);
    }

    [Fact]
    public void ClusterClientConfiguration_WithPubSubSubscriptions_IncludesPubSubConfig()
    {
        // Arrange
        ClusterPubSubSubscriptionConfig pubsubConfig = new ClusterPubSubSubscriptionConfig()
            .WithChannel("test-channel")
            .WithPattern("test-*")
            .WithShardedChannel("shard-channel")
            .WithCallback<ClusterPubSubSubscriptionConfig>((message, context) => { /* test callback */ });

        // Act
        ClusterClientConfiguration clientConfig = new ClusterClientConfigurationBuilder()
            .WithAddress("localhost", 6379)
            .WithPubSubSubscriptions(pubsubConfig)
            .Build();

        // Assert
        Assert.NotNull(clientConfig.Request.PubSubSubscriptions);
        Assert.Same(pubsubConfig, clientConfig.Request.PubSubSubscriptions);
    }
}
