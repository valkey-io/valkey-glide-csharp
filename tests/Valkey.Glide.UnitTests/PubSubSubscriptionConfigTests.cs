// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.UnitTests;

public class PubSubSubscriptionConfigTests
{
    // Test constants
    private static readonly string TestChannel = "test-channel";
    private static readonly string TestPattern = "test-*";
    private static readonly string TestShardedChannel = "sharded-channel";

    private static readonly MessageCallback Callback = (message, ctx) => { };
    private static readonly object Context = new { TestData = "test" };

    #region StandalonePubSubSubscriptionConfig Tests

    [Fact]
    public void StandaloneConfig_WithChannel_AddsExactChannelSubscription()
    {
        // Arrange
        var config = new StandalonePubSubSubscriptionConfig();

        // Act
        var result = config.WithChannel(TestChannel);

        // Assert
        Assert.Same(config, result); // Should return same instance for chaining
        Assert.True(config.Subscriptions.ContainsKey(PubSubChannelMode.Exact));
        Assert.Contains(TestChannel, config.Subscriptions[PubSubChannelMode.Exact]);
    }

    [Fact]
    public void StandaloneConfig_WithPattern_AddsPatternSubscription()
    {
        // Arrange
        var config = new StandalonePubSubSubscriptionConfig();

        // Act
        var result = config.WithPattern(TestPattern);

        // Assert
        Assert.Same(config, result); // Should return same instance for chaining
        Assert.True(config.Subscriptions.ContainsKey(PubSubChannelMode.Pattern));
        Assert.Contains(TestPattern, config.Subscriptions[PubSubChannelMode.Pattern]);
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

        // Act
        var result = config.WithCallback(Callback, Context);

        // Assert
        Assert.Same(config, result);
        Assert.Same(Callback, config.Callback);
        Assert.Same(Context, config.Context);
    }

    [Fact]
    public void StandaloneConfig_WithCallback_NullCallback_ThrowsArgumentNullException()
    {
        // Arrange
        var config = new StandalonePubSubSubscriptionConfig();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => config.WithCallback(null!));
    }

    [Fact]
    public void StandaloneConfig_BuilderPattern_SupportsMethodChaining()
    {
        // Arrange & Act
        var config = new StandalonePubSubSubscriptionConfig()
            .WithChannel(TestChannel)
            .WithPattern(TestPattern)
            .WithCallback(Callback, Context);

        // Assert
        Assert.Contains(TestChannel, config.Subscriptions[PubSubChannelMode.Exact]);
        Assert.Contains(TestPattern, config.Subscriptions[PubSubChannelMode.Pattern]);
        Assert.Equal(Callback, config.Callback);
        Assert.Equal(Context, config.Context);
    }

    #endregion

    #region ClusterPubSubSubscriptionConfig Tests

    [Fact]
    public void ClusterConfig_WithChannel_AddsExactChannelSubscription()
    {
        // Arrange
        var config = new ClusterPubSubSubscriptionConfig();

        // Act
        var result = config.WithChannel(TestChannel);

        // Assert
        Assert.Same(config, result);
        Assert.Contains(TestChannel, config.Subscriptions[PubSubChannelMode.Exact]);
    }

    [Fact]
    public void ClusterConfig_WithPattern_AddsPatternSubscription()
    {
        // Arrange
        var config = new ClusterPubSubSubscriptionConfig();

        // Act
        var result = config.WithPattern(TestPattern);

        // Assert
        Assert.Same(config, result);
        Assert.Contains(TestPattern, config.Subscriptions[PubSubChannelMode.Pattern]);
    }

    [Fact]
    public void ClusterConfig_WithShardedChannel_AddsShardedSubscription()
    {
        // Arrange
        var config = new ClusterPubSubSubscriptionConfig();

        // Act
        var result = config.WithShardedChannel(TestShardedChannel);

        // Assert
        Assert.Same(config, result);
        Assert.Contains(TestShardedChannel, config.Subscriptions[PubSubChannelMode.Sharded]);
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
        config.WithShardedChannel(TestShardedChannel);
        config.WithShardedChannel(TestShardedChannel); // Add same channel again

        // Assert
        Assert.Single(config.Subscriptions[PubSubChannelMode.Sharded]);
        Assert.Contains(TestShardedChannel, config.Subscriptions[PubSubChannelMode.Sharded]);
    }

    [Fact]
    public void ClusterConfig_WithCallback_SetsCallbackAndContext()
    {
        // Arrange
        var config = new ClusterPubSubSubscriptionConfig();

        // Act
        var result = config.WithCallback(Callback, Context);

        // Assert
        Assert.Same(config, result);
        Assert.Same(Callback, config.Callback);
        Assert.Same(Context, config.Context);
    }

    [Fact]
    public void ClusterConfig_WithCallback_NullCallback_ThrowsArgumentNullException()
    {
        // Arrange
        var config = new ClusterPubSubSubscriptionConfig();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => config.WithCallback(null!));
    }

    [Fact]
    public void ClusterConfig_BuilderPattern_SupportsMethodChaining()
    {
        // Arrange & Act
        var config = new ClusterPubSubSubscriptionConfig()
            .WithChannel("channel1")
            .WithPattern("pattern*")
            .WithShardedChannel("sharded1")
            .WithCallback(Callback, Context);

        // Assert
        Assert.True(config.Subscriptions.ContainsKey(PubSubChannelMode.Exact));
        Assert.True(config.Subscriptions.ContainsKey(PubSubChannelMode.Pattern));
        Assert.True(config.Subscriptions.ContainsKey(PubSubChannelMode.Sharded));
        Assert.Equal(Callback, config.Callback);
        Assert.Equal(Context, config.Context);
    }

    #endregion
}
