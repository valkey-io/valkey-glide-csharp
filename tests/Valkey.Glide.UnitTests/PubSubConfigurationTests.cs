// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using static Valkey.Glide.ConnectionConfiguration;

namespace Valkey.Glide.UnitTests;

/// <summary>
/// Unit tests for PubSub configuration extensions to ConnectionConfiguration builders.
/// These tests verify that PubSub subscription configuration flows correctly through
/// the configuration builders and validation works as expected.
/// </summary>
public class PubSubConfigurationTests
{
    // Test constants.
    private static readonly string Channel1 = "channel1";
    private static readonly string Channel2 = "channel2";
    private static readonly string Pattern1 = "pattern1*";
    private static readonly string Pattern2 = "pattern2*";
    private static readonly string ShardChannel1 = "shard1";
    private static readonly string ShardChannel2 = "shard2";
    private static readonly Object Context = new { TestData = "test" };
    private static readonly MessageCallback Callback = (message, ctx) => { /* test callback */ };

    #region StandaloneClientConfigurationBuilder Tests

    [Fact]
    public void StandaloneClientConfigurationBuilder_WithPubSubSubscriptions_ValidConfig_SetsConfiguration()
    {
        // Arrange
        var pubSubConfig = new StandalonePubSubSubscriptionConfig()
            .WithChannel(Channel1)
            .WithPattern(Pattern1);

        // Act
        var builder = new StandaloneClientConfigurationBuilder()
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
        var builder = new StandaloneClientConfigurationBuilder();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => builder.WithPubSubSubscriptions(null!));
    }

    [Fact]
    public void StandaloneClientConfigurationBuilder_WithPubSubSubscriptions_WithCallback_SetsCallbackAndContext()
    {
        // Arrange
        var pubSubConfig = new StandalonePubSubSubscriptionConfig()
            .WithChannel(Channel1)
            .WithCallback(Callback, Context);

        // Act
        var builder = new StandaloneClientConfigurationBuilder()
            .WithPubSubSubscriptions(pubSubConfig);

        var config = builder.Build();

        // Assert
        Assert.NotNull(config.Request.PubSubSubscriptions);
        var storedConfig = config.Request.PubSubSubscriptions as StandalonePubSubSubscriptionConfig;
        Assert.NotNull(storedConfig);
        Assert.Same(Callback, storedConfig.Callback);
        Assert.Same(Context, storedConfig.Context);
    }

    [Fact]
    public void StandaloneClientConfigurationBuilder_WithPubSubSubscriptions_MultipleChannelsAndPatterns_SetsAllSubscriptions()
    {
        // Arrange
        var pubSubConfig = new StandalonePubSubSubscriptionConfig()
            .WithChannel(Channel1)
            .WithChannel(Channel2)
            .WithPattern(Pattern1)
            .WithPattern(Pattern2);

        // Act
        var builder = new StandaloneClientConfigurationBuilder()
            .WithPubSubSubscriptions(pubSubConfig);

        var config = builder.Build();

        // Assert
        Assert.NotNull(config.Request.PubSubSubscriptions);
        var storedConfig = config.Request.PubSubSubscriptions as StandalonePubSubSubscriptionConfig;
        Assert.NotNull(storedConfig);

        // Check exact channels and patterns.
        Assert.Equivalent(new HashSet<string> { Channel1, Channel2 }, storedConfig.Subscriptions[PubSubChannelMode.Exact]);
        Assert.Equivalent(new HashSet<string> { Pattern1, Pattern2 }, storedConfig.Subscriptions[PubSubChannelMode.Pattern]);
    }

    #endregion

    #region ClusterClientConfigurationBuilder Tests

    [Fact]
    public void ClusterClientConfigurationBuilder_WithPubSubSubscriptions_ValidConfig_SetsConfiguration()
    {
        // Arrange
        var pubSubConfig = new ClusterPubSubSubscriptionConfig()
            .WithChannel(Channel1)
            .WithPattern(Pattern1)
            .WithShardChannel(ShardChannel1);

        // Act
        var builder = new ClusterClientConfigurationBuilder()
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
        var builder = new ClusterClientConfigurationBuilder();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => builder.WithPubSubSubscriptions(null!));
    }

    [Fact]
    public void ClusterClientConfigurationBuilder_WithPubSubSubscriptions_WithCallback_SetsCallbackAndContext()
    {
        // Arrange
        var pubSubConfig = new ClusterPubSubSubscriptionConfig()
            .WithChannel(Channel1)
            .WithCallback(Callback, Context);

        // Act
        var builder = new ClusterClientConfigurationBuilder()
            .WithPubSubSubscriptions(pubSubConfig);

        var config = builder.Build();

        // Assert
        Assert.NotNull(config.Request.PubSubSubscriptions);
        var storedConfig = config.Request.PubSubSubscriptions as ClusterPubSubSubscriptionConfig;
        Assert.NotNull(storedConfig);
        Assert.Same(Callback, storedConfig.Callback);
        Assert.Same(Context, storedConfig.Context);
    }

    [Fact]
    public void ClusterClientConfigurationBuilder_WithPubSubSubscriptions_AllSubscriptionTypes_SetsAllSubscriptions()
    {
        // Arrange
        var pubSubConfig = new ClusterPubSubSubscriptionConfig()
            .WithChannel(Channel1)
            .WithChannel(Channel2)
            .WithPattern(Pattern1)
            .WithPattern(Pattern2)
            .WithShardChannel(ShardChannel1)
            .WithShardChannel(ShardChannel2);

        // Act
        var builder = new ClusterClientConfigurationBuilder()
            .WithPubSubSubscriptions(pubSubConfig);

        var config = builder.Build();

        // Assert
        Assert.NotNull(config.Request.PubSubSubscriptions);
        var storedConfig = config.Request.PubSubSubscriptions as ClusterPubSubSubscriptionConfig;
        Assert.NotNull(storedConfig);

        // Check exact channels, patterns, and shard channels.
        Assert.Equivalent(new HashSet<string> { Channel1, Channel2 }, storedConfig.Subscriptions[PubSubChannelMode.Exact]);
        Assert.Equivalent(new HashSet<string> { Pattern1, Pattern2 }, storedConfig.Subscriptions[PubSubChannelMode.Pattern]);
        Assert.Equivalent(new HashSet<string> { ShardChannel1, ShardChannel2 }, storedConfig.Subscriptions[PubSubChannelMode.Sharded]);
    }

    #endregion

    #region Validation Tests

    [Fact]
    public void StandaloneClientConfigurationBuilder_WithPubSubSubscriptions_ValidatesConfigurationDuringBuild()
    {
        // Arrange
        var pubSubConfig = new StandalonePubSubSubscriptionConfig()
            .WithChannel(Channel1);

        // Act & Assert - Should not throw
        var builder = new StandaloneClientConfigurationBuilder()
            .WithPubSubSubscriptions(pubSubConfig);

        var config = builder.Build(); // This should succeed
        Assert.NotNull(config);
    }

    [Fact]
    public void ClusterClientConfigurationBuilder_WithPubSubSubscriptions_ValidatesConfigurationDuringBuild()
    {
        // Arrange
        var pubSubConfig = new ClusterPubSubSubscriptionConfig()
            .WithShardChannel(ShardChannel1);

        // Act & Assert - Should not throw
        var builder = new ClusterClientConfigurationBuilder()
            .WithPubSubSubscriptions(pubSubConfig);

        var config = builder.Build(); // This should succeed
        Assert.NotNull(config);
    }

    [Fact]
    public void StandaloneClientConfigurationBuilder_WithPubSubSubscriptions_EmptyChannelName_ThrowsArgumentException()
    {
        // Arrange
        var builder = new StandaloneClientConfigurationBuilder();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => { new StandalonePubSubSubscriptionConfig().WithChannel(""); });
    }

    [Fact]
    public void ClusterClientConfigurationBuilder_WithPubSubSubscriptions_EmptyShardedChannelName_ThrowsArgumentException()
    {
        // Arrange
        var builder = new ClusterClientConfigurationBuilder();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => { new ClusterPubSubSubscriptionConfig().WithShardChannel(""); });
    }

    #endregion
}
