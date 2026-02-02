// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.UnitTests;

public class PubSubSubscriptionConfigTests
{
    #region StandalonePubSubSubscriptionConfig Tests

    [Fact]
    public void StandaloneConfig_WithChannel_AddsExactChannelSubscription()
    {
        // Arrange
        var config = new StandalonePubSubSubscriptionConfig();

        // Act
        var result = config.WithChannel("test-channel");

        // Assert
        Assert.Same(config, result); // Should return same instance for chaining
        Assert.True(config.Subscriptions.ContainsKey((uint)PubSubChannelMode.Exact));
        Assert.Contains("test-channel", config.Subscriptions[(uint)PubSubChannelMode.Exact]);
    }

    [Fact]
    public void StandaloneConfig_WithPattern_AddsPatternSubscription()
    {
        // Arrange
        var config = new StandalonePubSubSubscriptionConfig();

        // Act
        var result = config.WithPattern("test-*");

        // Assert
        Assert.Same(config, result); // Should return same instance for chaining
        Assert.True(config.Subscriptions.ContainsKey((uint)PubSubChannelMode.Pattern));
        Assert.Contains("test-*", config.Subscriptions[(uint)PubSubChannelMode.Pattern]);
    }

    [Fact]
    public void StandaloneConfig_WithChannel_NullOrEmptyChannel_ThrowsArgumentException()
    {
        // Arrange
        var config = new StandalonePubSubSubscriptionConfig();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => config.WithChannel(null!));
        Assert.Throws<ArgumentException>(() => config.WithChannel(""));
        Assert.Throws<ArgumentException>(() => config.WithChannel("   "));
    }

    [Fact]
    public void StandaloneConfig_WithPattern_NullOrEmptyPattern_ThrowsArgumentException()
    {
        // Arrange
        var config = new StandalonePubSubSubscriptionConfig();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => config.WithPattern(null!));
        Assert.Throws<ArgumentException>(() => config.WithPattern(""));
        Assert.Throws<ArgumentException>(() => config.WithPattern("   "));
    }

    [Fact]
    public void StandaloneConfig_WithCallback_SetsCallbackAndContext()
    {
        // Arrange
        var config = new StandalonePubSubSubscriptionConfig();
        var context = new { TestData = "test" };
        MessageCallback callback = (message, ctx) => { };

        // Act
        var result = config.WithCallback<StandalonePubSubSubscriptionConfig>(callback, context);

        // Assert
        Assert.Same(config, result);
        Assert.Same(callback, config.Callback);
        Assert.Same(context, config.Context);
    }

    [Fact]
    public void StandaloneConfig_WithCallback_NullCallback_ThrowsArgumentNullException()
    {
        // Arrange
        var config = new StandalonePubSubSubscriptionConfig();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => config.WithCallback<StandalonePubSubSubscriptionConfig>(null!));
    }

    [Fact]
    public void StandaloneConfig_BuilderPattern_SupportsMethodChaining()
    {
        // Arrange & Act
        var config = new StandalonePubSubSubscriptionConfig()
            .WithChannel("channel1")
            .WithPattern("pattern*")
            .WithCallback<StandalonePubSubSubscriptionConfig>((msg, ctx) => { }, "context");

        // Assert
        Assert.True(config.Subscriptions.ContainsKey((uint)PubSubChannelMode.Exact));
        Assert.True(config.Subscriptions.ContainsKey((uint)PubSubChannelMode.Pattern));
        Assert.NotNull(config.Callback);
        Assert.Equal("context", config.Context);
    }

    #endregion

    #region ClusterPubSubSubscriptionConfig Tests

    [Fact]
    public void ClusterConfig_WithChannel_AddsExactChannelSubscription()
    {
        // Arrange
        var config = new ClusterPubSubSubscriptionConfig();

        // Act
        var result = config.WithChannel("test-channel");

        // Assert
        Assert.Same(config, result);
        Assert.True(config.Subscriptions.ContainsKey((uint)PubSubChannelMode.Exact));
        Assert.Contains("test-channel", config.Subscriptions[(uint)PubSubChannelMode.Exact]);
    }

    [Fact]
    public void ClusterConfig_WithPattern_AddsPatternSubscription()
    {
        // Arrange
        var config = new ClusterPubSubSubscriptionConfig();

        // Act
        var result = config.WithPattern("test-*");

        // Assert
        Assert.Same(config, result);
        Assert.True(config.Subscriptions.ContainsKey((uint)PubSubChannelMode.Pattern));
        Assert.Contains("test-*", config.Subscriptions[(uint)PubSubChannelMode.Pattern]);
    }

    [Fact]
    public void ClusterConfig_WithShardedChannel_AddsShardedSubscription()
    {
        // Arrange
        var config = new ClusterPubSubSubscriptionConfig();

        // Act
        var result = config.WithShardedChannel("sharded-channel");

        // Assert
        Assert.Same(config, result);
        Assert.True(config.Subscriptions.ContainsKey((uint)PubSubChannelMode.Sharded));
        Assert.Contains("sharded-channel", config.Subscriptions[(uint)PubSubChannelMode.Sharded]);
    }

    [Fact]
    public void ClusterConfig_WithChannel_NullOrEmptyChannel_ThrowsArgumentException()
    {
        // Arrange
        var config = new ClusterPubSubSubscriptionConfig();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => config.WithChannel(null!));
        Assert.Throws<ArgumentException>(() => config.WithChannel(""));
        Assert.Throws<ArgumentException>(() => config.WithChannel("   "));
    }

    [Fact]
    public void ClusterConfig_WithPattern_NullOrEmptyPattern_ThrowsArgumentException()
    {
        // Arrange
        var config = new ClusterPubSubSubscriptionConfig();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => config.WithPattern(null!));
        Assert.Throws<ArgumentException>(() => config.WithPattern(""));
        Assert.Throws<ArgumentException>(() => config.WithPattern("   "));
    }

    [Fact]
    public void ClusterConfig_WithShardedChannel_NullOrEmptyChannel_ThrowsArgumentException()
    {
        // Arrange
        var config = new ClusterPubSubSubscriptionConfig();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => config.WithShardedChannel(null!));
        Assert.Throws<ArgumentException>(() => config.WithShardedChannel(""));
        Assert.Throws<ArgumentException>(() => config.WithShardedChannel("   "));
    }

    [Fact]
    public void ClusterConfig_DuplicateChannel_DoesNotAddDuplicate()
    {
        // Arrange
        var config = new ClusterPubSubSubscriptionConfig();

        // Act
        config.WithShardedChannel("sharded-channel");
        config.WithShardedChannel("sharded-channel"); // Add same channel again

        // Assert
        Assert.Single(config.Subscriptions[(uint)PubSubChannelMode.Sharded]);
        Assert.Contains("sharded-channel", config.Subscriptions[(uint)PubSubChannelMode.Sharded]);
    }

    [Fact]
    public void ClusterConfig_WithCallback_SetsCallbackAndContext()
    {
        // Arrange
        var config = new ClusterPubSubSubscriptionConfig();
        var context = new { TestData = "test" };
        MessageCallback callback = (message, ctx) => { };

        // Act
        var result = config.WithCallback<ClusterPubSubSubscriptionConfig>(callback, context);

        // Assert
        Assert.Same(config, result);
        Assert.Same(callback, config.Callback);
        Assert.Same(context, config.Context);
    }

    [Fact]
    public void ClusterConfig_WithCallback_NullCallback_ThrowsArgumentNullException()
    {
        // Arrange
        var config = new ClusterPubSubSubscriptionConfig();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => config.WithCallback<ClusterPubSubSubscriptionConfig>(null!));
    }

    [Fact]
    public void ClusterConfig_BuilderPattern_SupportsMethodChaining()
    {
        // Arrange & Act
        var config = new ClusterPubSubSubscriptionConfig()
            .WithChannel("channel1")
            .WithPattern("pattern*")
            .WithShardedChannel("sharded1")
            .WithCallback<ClusterPubSubSubscriptionConfig>((msg, ctx) => { }, "context");

        // Assert
        Assert.True(config.Subscriptions.ContainsKey((uint)PubSubChannelMode.Exact));
        Assert.True(config.Subscriptions.ContainsKey((uint)PubSubChannelMode.Pattern));
        Assert.True(config.Subscriptions.ContainsKey((uint)PubSubChannelMode.Sharded));
        Assert.NotNull(config.Callback);
        Assert.Equal("context", config.Context);
    }

    #endregion

}
