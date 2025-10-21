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
    public void StandaloneConfig_WithSubscription_AddsCorrectSubscription()
    {
        // Arrange
        var config = new StandalonePubSubSubscriptionConfig();

        // Act
        var result = config.WithSubscription(PubSubChannelMode.Exact, "exact-channel");

        // Assert
        Assert.Same(config, result);
        Assert.True(config.Subscriptions.ContainsKey((uint)PubSubChannelMode.Exact));
        Assert.Contains("exact-channel", config.Subscriptions[(uint)PubSubChannelMode.Exact]);
    }

    [Fact]
    public void StandaloneConfig_WithSubscription_NullOrEmptyChannel_ThrowsArgumentException()
    {
        // Arrange
        var config = new StandalonePubSubSubscriptionConfig();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => config.WithSubscription(PubSubChannelMode.Exact, null!));
        Assert.Throws<ArgumentException>(() => config.WithSubscription(PubSubChannelMode.Exact, ""));
        Assert.Throws<ArgumentException>(() => config.WithSubscription(PubSubChannelMode.Exact, "   "));
    }

    [Fact]
    public void StandaloneConfig_WithSubscription_InvalidMode_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var config = new StandalonePubSubSubscriptionConfig();

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => config.WithSubscription((PubSubChannelMode)999, "test"));
    }

    [Fact]
    public void StandaloneConfig_WithSubscription_DuplicateChannel_DoesNotAddDuplicate()
    {
        // Arrange
        var config = new StandalonePubSubSubscriptionConfig();

        // Act
        config.WithChannel("test-channel");
        config.WithChannel("test-channel"); // Add same channel again

        // Assert
        Assert.Single(config.Subscriptions[(uint)PubSubChannelMode.Exact]);
        Assert.Contains("test-channel", config.Subscriptions[(uint)PubSubChannelMode.Exact]);
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
    public void StandaloneConfig_Validate_NoSubscriptions_ThrowsArgumentException()
    {
        // Arrange
        var config = new StandalonePubSubSubscriptionConfig();

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => config.Validate());
        Assert.Contains("At least one subscription must be configured", exception.Message);
    }

    [Fact]
    public void StandaloneConfig_Validate_ValidConfiguration_DoesNotThrow()
    {
        // Arrange
        var config = new StandalonePubSubSubscriptionConfig()
            .WithChannel("test-channel");

        // Act & Assert
        config.Validate(); // Should not throw
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
        Assert.True(config.Subscriptions.ContainsKey((uint)PubSubClusterChannelMode.Exact));
        Assert.Contains("test-channel", config.Subscriptions[(uint)PubSubClusterChannelMode.Exact]);
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
        Assert.True(config.Subscriptions.ContainsKey((uint)PubSubClusterChannelMode.Pattern));
        Assert.Contains("test-*", config.Subscriptions[(uint)PubSubClusterChannelMode.Pattern]);
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
        Assert.True(config.Subscriptions.ContainsKey((uint)PubSubClusterChannelMode.Sharded));
        Assert.Contains("sharded-channel", config.Subscriptions[(uint)PubSubClusterChannelMode.Sharded]);
    }

    [Fact]
    public void ClusterConfig_WithSubscription_AddsCorrectSubscription()
    {
        // Arrange
        var config = new ClusterPubSubSubscriptionConfig();

        // Act
        var result = config.WithSubscription(PubSubClusterChannelMode.Sharded, "sharded-channel");

        // Assert
        Assert.Same(config, result);
        Assert.True(config.Subscriptions.ContainsKey((uint)PubSubClusterChannelMode.Sharded));
        Assert.Contains("sharded-channel", config.Subscriptions[(uint)PubSubClusterChannelMode.Sharded]);
    }

    [Fact]
    public void ClusterConfig_WithSubscription_NullOrEmptyChannel_ThrowsArgumentException()
    {
        // Arrange
        var config = new ClusterPubSubSubscriptionConfig();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => config.WithSubscription(PubSubClusterChannelMode.Exact, null!));
        Assert.Throws<ArgumentException>(() => config.WithSubscription(PubSubClusterChannelMode.Exact, ""));
        Assert.Throws<ArgumentException>(() => config.WithSubscription(PubSubClusterChannelMode.Exact, "   "));
    }

    [Fact]
    public void ClusterConfig_WithSubscription_InvalidMode_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var config = new ClusterPubSubSubscriptionConfig();

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => config.WithSubscription((PubSubClusterChannelMode)999, "test"));
    }

    [Fact]
    public void ClusterConfig_WithSubscription_DuplicateChannel_DoesNotAddDuplicate()
    {
        // Arrange
        var config = new ClusterPubSubSubscriptionConfig();

        // Act
        config.WithShardedChannel("sharded-channel");
        config.WithShardedChannel("sharded-channel"); // Add same channel again

        // Assert
        Assert.Single(config.Subscriptions[(uint)PubSubClusterChannelMode.Sharded]);
        Assert.Contains("sharded-channel", config.Subscriptions[(uint)PubSubClusterChannelMode.Sharded]);
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
    public void ClusterConfig_Validate_NoSubscriptions_ThrowsArgumentException()
    {
        // Arrange
        var config = new ClusterPubSubSubscriptionConfig();

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => config.Validate());
        Assert.Contains("At least one subscription must be configured", exception.Message);
    }

    [Fact]
    public void ClusterConfig_Validate_ValidConfiguration_DoesNotThrow()
    {
        // Arrange
        var config = new ClusterPubSubSubscriptionConfig()
            .WithShardedChannel("test-channel");

        // Act & Assert
        config.Validate(); // Should not throw
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
        Assert.True(config.Subscriptions.ContainsKey((uint)PubSubClusterChannelMode.Exact));
        Assert.True(config.Subscriptions.ContainsKey((uint)PubSubClusterChannelMode.Pattern));
        Assert.True(config.Subscriptions.ContainsKey((uint)PubSubClusterChannelMode.Sharded));
        Assert.NotNull(config.Callback);
        Assert.Equal("context", config.Context);
    }

    #endregion

    #region MessageCallback Tests

    [Fact]
    public void MessageCallback_CanBeInvoked()
    {
        // Arrange
        bool messageReceived = false;
        PubSubMessage? receivedMessage = null;
        object? receivedContext = null;

        MessageCallback callback = (message, context) =>
        {
            messageReceived = true;
            receivedMessage = message;
            receivedContext = context;
        };

        var testMessage = new PubSubMessage("test-message", "test-channel");
        string testContext = "test-context";

        // Act
        callback(testMessage, testContext);

        // Assert
        Assert.True(messageReceived);
        Assert.Same(testMessage, receivedMessage);
        Assert.Same(testContext, receivedContext);
    }

    #endregion

    #region Validation Tests

    [Fact]
    public void BasePubSubSubscriptionConfig_Validate_EmptyChannelList_ThrowsArgumentException()
    {
        // Arrange
        var config = new StandalonePubSubSubscriptionConfig();
        config.Subscriptions[(uint)PubSubChannelMode.Exact] = new List<string>();

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => config.Validate());
        Assert.Contains("has no channels or patterns configured", exception.Message);
    }

    [Fact]
    public void BasePubSubSubscriptionConfig_Validate_NullChannelInList_ThrowsArgumentException()
    {
        // Arrange
        var config = new StandalonePubSubSubscriptionConfig();
        config.Subscriptions[(uint)PubSubChannelMode.Exact] = new List<string> { null! };

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => config.Validate());
        Assert.Contains("Channel name or pattern cannot be null, empty, or whitespace", exception.Message);
    }

    [Fact]
    public void BasePubSubSubscriptionConfig_Validate_EmptyChannelInList_ThrowsArgumentException()
    {
        // Arrange
        var config = new StandalonePubSubSubscriptionConfig();
        config.Subscriptions[(uint)PubSubChannelMode.Exact] = new List<string> { "" };

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => config.Validate());
        Assert.Contains("Channel name or pattern cannot be null, empty, or whitespace", exception.Message);
    }

    [Fact]
    public void BasePubSubSubscriptionConfig_Validate_WhitespaceChannelInList_ThrowsArgumentException()
    {
        // Arrange
        var config = new StandalonePubSubSubscriptionConfig();
        config.Subscriptions[(uint)PubSubChannelMode.Exact] = new List<string> { "   " };

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => config.Validate());
        Assert.Contains("Channel name or pattern cannot be null, empty, or whitespace", exception.Message);
    }

    #endregion

    #region Enum Tests

    [Fact]
    public void PubSubChannelMode_HasCorrectValues()
    {
        // Assert
        Assert.Equal(0, (int)PubSubChannelMode.Exact);
        Assert.Equal(1, (int)PubSubChannelMode.Pattern);
    }

    [Fact]
    public void PubSubClusterChannelMode_HasCorrectValues()
    {
        // Assert
        Assert.Equal(0, (int)PubSubClusterChannelMode.Exact);
        Assert.Equal(1, (int)PubSubClusterChannelMode.Pattern);
        Assert.Equal(2, (int)PubSubClusterChannelMode.Sharded);
    }

    #endregion
}
