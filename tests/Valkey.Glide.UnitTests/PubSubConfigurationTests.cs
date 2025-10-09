// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using System;
using System.Collections.Generic;

using Xunit;

using static Valkey.Glide.ConnectionConfiguration;

namespace Valkey.Glide.UnitTests;

/// <summary>
/// Unit tests for PubSub configuration extensions to ConnectionConfiguration builders.
/// These tests verify that PubSub subscription configuration flows correctly through
/// the configuration builders and validation works as expected.
/// </summary>
public class PubSubConfigurationTests
{
    #region StandaloneClientConfigurationBuilder Tests

    [Fact]
    public void StandaloneClientConfigurationBuilder_WithPubSubSubscriptions_ValidConfig_SetsConfiguration()
    {
        // Arrange
        var pubSubConfig = new StandalonePubSubSubscriptionConfig()
            .WithChannel("test-channel")
            .WithPattern("test-*");

        // Act
        var builder = new StandaloneClientConfigurationBuilder()
            .WithAddress("localhost", 6379)
            .WithPubSubSubscriptions(pubSubConfig);

        var config = builder.Build();

        // Assert
        Assert.NotNull(config.Request.PubSubSubscriptions);
        Assert.Same(pubSubConfig, config.Request.PubSubSubscriptions);
    }

    [Fact]
    public void StandaloneClientConfigurationBuilder_WithPubSubSubscriptions_NullConfig_ThrowsArgumentNullException()
    {
        // Arrange
        var builder = new StandaloneClientConfigurationBuilder()
            .WithAddress("localhost", 6379);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => builder.WithPubSubSubscriptions(null!));
    }

    [Fact]
    public void StandaloneClientConfigurationBuilder_WithPubSubSubscriptions_InvalidConfig_ThrowsArgumentException()
    {
        // Arrange
        var pubSubConfig = new StandalonePubSubSubscriptionConfig(); // Empty config - invalid

        var builder = new StandaloneClientConfigurationBuilder()
            .WithAddress("localhost", 6379);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => builder.WithPubSubSubscriptions(pubSubConfig));
    }

    [Fact]
    public void StandaloneClientConfigurationBuilder_WithPubSubSubscriptions_WithCallback_SetsCallbackAndContext()
    {
        // Arrange
        var context = new { TestData = "test" };
        MessageCallback callback = (message, ctx) => { /* test callback */ };

        var pubSubConfig = new StandalonePubSubSubscriptionConfig()
            .WithChannel("test-channel")
            .WithCallback<StandalonePubSubSubscriptionConfig>(callback, context);

        // Act
        var builder = new StandaloneClientConfigurationBuilder()
            .WithAddress("localhost", 6379)
            .WithPubSubSubscriptions(pubSubConfig);

        var config = builder.Build();

        // Assert
        Assert.NotNull(config.Request.PubSubSubscriptions);
        var storedConfig = config.Request.PubSubSubscriptions as StandalonePubSubSubscriptionConfig;
        Assert.NotNull(storedConfig);
        Assert.Same(callback, storedConfig.Callback);
        Assert.Same(context, storedConfig.Context);
    }

    [Fact]
    public void StandaloneClientConfigurationBuilder_WithPubSubSubscriptions_MultipleChannelsAndPatterns_SetsAllSubscriptions()
    {
        // Arrange
        var pubSubConfig = new StandalonePubSubSubscriptionConfig()
            .WithChannel("channel1")
            .WithChannel("channel2")
            .WithPattern("pattern1*")
            .WithPattern("pattern2*");

        // Act
        var builder = new StandaloneClientConfigurationBuilder()
            .WithAddress("localhost", 6379)
            .WithPubSubSubscriptions(pubSubConfig);

        var config = builder.Build();

        // Assert
        Assert.NotNull(config.Request.PubSubSubscriptions);
        var storedConfig = config.Request.PubSubSubscriptions as StandalonePubSubSubscriptionConfig;
        Assert.NotNull(storedConfig);

        // Check exact channels (mode 0)
        Assert.True(storedConfig.Subscriptions.ContainsKey(0));
        Assert.Contains("channel1", storedConfig.Subscriptions[0]);
        Assert.Contains("channel2", storedConfig.Subscriptions[0]);

        // Check patterns (mode 1)
        Assert.True(storedConfig.Subscriptions.ContainsKey(1));
        Assert.Contains("pattern1*", storedConfig.Subscriptions[1]);
        Assert.Contains("pattern2*", storedConfig.Subscriptions[1]);
    }

    #endregion

    #region ClusterClientConfigurationBuilder Tests

    [Fact]
    public void ClusterClientConfigurationBuilder_WithPubSubSubscriptions_ValidConfig_SetsConfiguration()
    {
        // Arrange
        var pubSubConfig = new ClusterPubSubSubscriptionConfig()
            .WithChannel("test-channel")
            .WithPattern("test-*")
            .WithShardedChannel("shard-channel");

        // Act
        var builder = new ClusterClientConfigurationBuilder()
            .WithAddress("localhost", 6379)
            .WithPubSubSubscriptions(pubSubConfig);

        var config = builder.Build();

        // Assert
        Assert.NotNull(config.Request.PubSubSubscriptions);
        Assert.Same(pubSubConfig, config.Request.PubSubSubscriptions);
    }

    [Fact]
    public void ClusterClientConfigurationBuilder_WithPubSubSubscriptions_NullConfig_ThrowsArgumentNullException()
    {
        // Arrange
        var builder = new ClusterClientConfigurationBuilder()
            .WithAddress("localhost", 6379);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => builder.WithPubSubSubscriptions(null!));
    }

    [Fact]
    public void ClusterClientConfigurationBuilder_WithPubSubSubscriptions_InvalidConfig_ThrowsArgumentException()
    {
        // Arrange
        var pubSubConfig = new ClusterPubSubSubscriptionConfig(); // Empty config - invalid

        var builder = new ClusterClientConfigurationBuilder()
            .WithAddress("localhost", 6379);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => builder.WithPubSubSubscriptions(pubSubConfig));
    }

    [Fact]
    public void ClusterClientConfigurationBuilder_WithPubSubSubscriptions_WithCallback_SetsCallbackAndContext()
    {
        // Arrange
        var context = new { TestData = "test" };
        MessageCallback callback = (message, ctx) => { /* test callback */ };

        var pubSubConfig = new ClusterPubSubSubscriptionConfig()
            .WithChannel("test-channel")
            .WithCallback<ClusterPubSubSubscriptionConfig>(callback, context);

        // Act
        var builder = new ClusterClientConfigurationBuilder()
            .WithAddress("localhost", 6379)
            .WithPubSubSubscriptions(pubSubConfig);

        var config = builder.Build();

        // Assert
        Assert.NotNull(config.Request.PubSubSubscriptions);
        var storedConfig = config.Request.PubSubSubscriptions as ClusterPubSubSubscriptionConfig;
        Assert.NotNull(storedConfig);
        Assert.Same(callback, storedConfig.Callback);
        Assert.Same(context, storedConfig.Context);
    }

    [Fact]
    public void ClusterClientConfigurationBuilder_WithPubSubSubscriptions_AllSubscriptionTypes_SetsAllSubscriptions()
    {
        // Arrange
        var pubSubConfig = new ClusterPubSubSubscriptionConfig()
            .WithChannel("channel1")
            .WithChannel("channel2")
            .WithPattern("pattern1*")
            .WithPattern("pattern2*")
            .WithShardedChannel("shard1")
            .WithShardedChannel("shard2");

        // Act
        var builder = new ClusterClientConfigurationBuilder()
            .WithAddress("localhost", 6379)
            .WithPubSubSubscriptions(pubSubConfig);

        var config = builder.Build();

        // Assert
        Assert.NotNull(config.Request.PubSubSubscriptions);
        var storedConfig = config.Request.PubSubSubscriptions as ClusterPubSubSubscriptionConfig;
        Assert.NotNull(storedConfig);

        // Check exact channels (mode 0)
        Assert.True(storedConfig.Subscriptions.ContainsKey(0));
        Assert.Contains("channel1", storedConfig.Subscriptions[0]);
        Assert.Contains("channel2", storedConfig.Subscriptions[0]);

        // Check patterns (mode 1)
        Assert.True(storedConfig.Subscriptions.ContainsKey(1));
        Assert.Contains("pattern1*", storedConfig.Subscriptions[1]);
        Assert.Contains("pattern2*", storedConfig.Subscriptions[1]);

        // Check sharded channels (mode 2)
        Assert.True(storedConfig.Subscriptions.ContainsKey(2));
        Assert.Contains("shard1", storedConfig.Subscriptions[2]);
        Assert.Contains("shard2", storedConfig.Subscriptions[2]);
    }

    #endregion

    #region FFI Marshaling Tests

    [Fact]
    public void ConnectionConfig_ToFfi_WithPubSubConfig_MarshalsProperly()
    {
        // Arrange
        var pubSubConfig = new StandalonePubSubSubscriptionConfig()
            .WithChannel("test-channel")
            .WithPattern("test-*");

        // Act - Test through the builder which creates the internal ConnectionConfig
        var builder = new StandaloneClientConfigurationBuilder()
            .WithAddress("localhost", 6379)
            .WithPubSubSubscriptions(pubSubConfig);

        var config = builder.Build();
        var ffiConfig = config.Request.ToFfi();

        // Assert
        Assert.NotNull(ffiConfig);
        // Note: We can't directly test the FFI marshaling without access to the internal structures,
        // but we can verify that the ToFfi() method doesn't throw and the config is passed through
    }

    [Fact]
    public void ConnectionConfig_ToFfi_WithoutPubSubConfig_MarshalsProperly()
    {
        // Arrange & Act - Test through the builder without PubSub config
        var builder = new StandaloneClientConfigurationBuilder()
            .WithAddress("localhost", 6379);

        var config = builder.Build();
        var ffiConfig = config.Request.ToFfi();

        // Assert
        Assert.NotNull(ffiConfig);
        // Note: We can't directly test the FFI marshaling without access to the internal structures,
        // but we can verify that the ToFfi() method doesn't throw when PubSubSubscriptions is null
    }

    #endregion

    #region Validation Tests

    [Fact]
    public void StandaloneClientConfigurationBuilder_WithPubSubSubscriptions_ValidatesConfigurationDuringBuild()
    {
        // Arrange
        var pubSubConfig = new StandalonePubSubSubscriptionConfig()
            .WithChannel("test-channel");

        // Act & Assert - Should not throw
        var builder = new StandaloneClientConfigurationBuilder()
            .WithAddress("localhost", 6379)
            .WithPubSubSubscriptions(pubSubConfig);

        var config = builder.Build(); // This should succeed
        Assert.NotNull(config);
    }

    [Fact]
    public void ClusterClientConfigurationBuilder_WithPubSubSubscriptions_ValidatesConfigurationDuringBuild()
    {
        // Arrange
        var pubSubConfig = new ClusterPubSubSubscriptionConfig()
            .WithShardedChannel("shard-channel");

        // Act & Assert - Should not throw
        var builder = new ClusterClientConfigurationBuilder()
            .WithAddress("localhost", 6379)
            .WithPubSubSubscriptions(pubSubConfig);

        var config = builder.Build(); // This should succeed
        Assert.NotNull(config);
    }

    [Fact]
    public void StandaloneClientConfigurationBuilder_WithPubSubSubscriptions_EmptyChannelName_ThrowsArgumentException()
    {
        // Arrange
        var builder = new StandaloneClientConfigurationBuilder()
            .WithAddress("localhost", 6379);

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
        {
            var pubSubConfig = new StandalonePubSubSubscriptionConfig()
                .WithChannel(""); // Empty channel name should be invalid
            builder.WithPubSubSubscriptions(pubSubConfig);
        });
    }

    [Fact]
    public void ClusterClientConfigurationBuilder_WithPubSubSubscriptions_EmptyShardedChannelName_ThrowsArgumentException()
    {
        // Arrange
        var builder = new ClusterClientConfigurationBuilder()
            .WithAddress("localhost", 6379);

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
        {
            var pubSubConfig = new ClusterPubSubSubscriptionConfig()
                .WithShardedChannel(""); // Empty sharded channel name should be invalid
            builder.WithPubSubSubscriptions(pubSubConfig);
        });
    }

    #endregion
}
