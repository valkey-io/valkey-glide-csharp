// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using System.Net;

using Valkey.Glide.Internals;

namespace Valkey.Glide;

/// <summary>
/// Subscriber implementation for pub/sub operations.
/// </summary>
internal sealed class Subscriber : ISubscriber
{
    private readonly ConnectionMultiplexer _multiplexer;
    private readonly Database _client;

    internal Subscriber(ConnectionMultiplexer multiplexer, Database client)
    {
        _multiplexer = multiplexer;
        _client = client;
    }

    #region SyncMethods

    public long Publish(ValkeyChannel channel, ValkeyValue message, CommandFlags flags = CommandFlags.None)
        => PublishAsync(channel, message, flags).GetAwaiter().GetResult();

    public void Subscribe(ValkeyChannel channel, Action<ValkeyChannel, ValkeyValue> handler, CommandFlags flags = CommandFlags.None)
        => SubscribeAsync(channel, handler, flags).GetAwaiter().GetResult();

    public ChannelMessageQueue Subscribe(ValkeyChannel channel, CommandFlags flags = CommandFlags.None)
        => SubscribeAsync(channel, flags).GetAwaiter().GetResult();

    public void Unsubscribe(ValkeyChannel channel, Action<ValkeyChannel, ValkeyValue>? handler = null, CommandFlags flags = CommandFlags.None)
        => UnsubscribeAsync(channel, handler, flags).GetAwaiter().GetResult();

    public void UnsubscribeAll(CommandFlags flags = CommandFlags.None)
        => UnsubscribeAllAsync(flags).GetAwaiter().GetResult();

    #endregion
    #region AsyncMethods

    public async Task<long> PublishAsync(ValkeyChannel channel, ValkeyValue message, CommandFlags flags = CommandFlags.None)
    {
        // Validate arguments.
        ThrowIfChannelNullOrEmpty(channel);
        GuardClauses.ThrowIfCommandFlags(flags);

        return await SendPublishCommand(channel, message);
    }

    public async Task SubscribeAsync(ValkeyChannel channel, Action<ValkeyChannel, ValkeyValue> handler, CommandFlags flags = CommandFlags.None)
    {
        // Validate arguments.
        ThrowIfChannelNullOrEmpty(channel);
        ArgumentNullException.ThrowIfNull(handler);
        GuardClauses.ThrowIfCommandFlags(flags);

        // Add handler to multiplexer.
        bool added = _multiplexer.AddSubscriptionHandler(channel, handler);

        // Send subscribe command if subscription was added.
        if (added)
        {
            await SendSubscribeCommand(channel);
        }
    }

    public async Task<ChannelMessageQueue> SubscribeAsync(ValkeyChannel channel, CommandFlags flags = CommandFlags.None)
    {
        // Validate arguments.
        ThrowIfChannelNullOrEmpty(channel);
        GuardClauses.ThrowIfCommandFlags(flags);

        // Add queue to multiplexer.
        var queue = new ChannelMessageQueue(channel, this);
        bool added = _multiplexer.AddSubscriptionQueue(channel, queue);

        // Send subscribe command if subscription was added.
        if (added)
        {
            await SendSubscribeCommand(channel);
        }

        return queue;
    }

    public async Task UnsubscribeAsync(ValkeyChannel channel, Action<ValkeyChannel, ValkeyValue>? handler = null, CommandFlags flags = CommandFlags.None)
    {
        // Validate arguments.
        ThrowIfChannelNullOrEmpty(channel);
        GuardClauses.ThrowIfCommandFlags(flags);

        // Remove handler from subscription or remove entire subscription.
        bool removed;
        if (handler != null)
        {
            removed = _multiplexer.RemoveSubscriptionHandler(channel, handler);
        }
        else
        {
            _multiplexer.RemoveSubscription(channel);
            removed = true;
        }

        // Send unsubscribe command if subscription was removed.
        if (removed)
        {
            await SendUnsubscribeCommand(channel);
        }
    }

    /// <summary>
    /// Unsubscribes from the specified channel queue.
    /// </summary>
    /// <param name="queue">The channel message queue to unsubscribe.</param>
    /// <returns></returns>
    internal async Task UnsubscribeAsync(ChannelMessageQueue queue)
    {
        // Validate arguments.
        var channel = queue.Channel;
        ThrowIfChannelNullOrEmpty(channel);

        // Remove queue from multiplexer.
        bool removed = _multiplexer.RemoveSubscriptionQueue(queue);

        // Send unsubscribe command if subscription was removed.
        if (removed)
        {
            await SendUnsubscribeCommand(channel);
        }
    }

    public async Task UnsubscribeAllAsync(CommandFlags flags = CommandFlags.None)
    {
        // Validate arguments.
        GuardClauses.ThrowIfCommandFlags(flags);

        // Remove all subscriptions from multiplexer.
        _multiplexer.RemoveAllSubscriptions();

        // Send unsubscribe commands for all channel modes.
        await _client.UnsubscribeAsync();
        await _client.PUnsubscribeAsync();

        if (_client.IsCluster)
        {
            // TODO #205: Refactor to use GlideClusterClient instead of custom command.
            await _client.Command(Request.CustomCommand(["SUNSUBSCRIBE"]), Route.Random);
        }
    }

    #endregion
    #region SyncMethods

    public long Publish(ValkeyChannel channel, ValkeyValue message, CommandFlags flags = CommandFlags.None)
        => PublishAsync(channel, message, flags).GetAwaiter().GetResult();

    public void Subscribe(ValkeyChannel channel, Action<ValkeyChannel, ValkeyValue> handler, CommandFlags flags = CommandFlags.None)
        => SubscribeAsync(channel, handler, flags).GetAwaiter().GetResult();

    public ChannelMessageQueue Subscribe(ValkeyChannel channel, CommandFlags flags = CommandFlags.None)
        => SubscribeAsync(channel, flags).GetAwaiter().GetResult();

    public void Unsubscribe(ValkeyChannel channel, Action<ValkeyChannel, ValkeyValue>? handler = null, CommandFlags flags = CommandFlags.None)
        => UnsubscribeAsync(channel, handler, flags).GetAwaiter().GetResult();

    public void UnsubscribeAll(CommandFlags flags = CommandFlags.None)
        => UnsubscribeAllAsync(flags).GetAwaiter().GetResult();

    #endregion
    #region AsyncMethods

    public async Task<long> PublishAsync(ValkeyChannel channel, ValkeyValue message, CommandFlags flags = CommandFlags.None)
    {
        // Validate arguments.
        ThrowIfValkeyChannelNullOrEmpty(channel, nameof(channel));
        GuardClauses.ThrowIfCommandFlags(flags);

        return await SendPublishCommand(channel, message);
    }

    public async Task SubscribeAsync(ValkeyChannel channel, Action<ValkeyChannel, ValkeyValue> handler, CommandFlags flags = CommandFlags.None)
    {
        // Validate arguments.
        ThrowIfChannelNullOrEmpty(channel);
        ArgumentNullException.ThrowIfNull(handler);
        GuardClauses.ThrowIfCommandFlags(flags);

        // Add handler to multiplexer.
        bool added = _multiplexer.AddSubscriptionHandler(channel, handler);

        // Send subscribe command if subscription was added.
        if (added)
        {
            await SendSubscribeCommand(channel);
        }
    }

    public async Task<ChannelMessageQueue> SubscribeAsync(ValkeyChannel channel, CommandFlags flags = CommandFlags.None)
    {
        // Validate arguments.
        ThrowIfChannelNullOrEmpty(channel);
        GuardClauses.ThrowIfCommandFlags(flags);

        // Add queue to multiplexer.
        var queue = new ChannelMessageQueue(channel, this);
        bool added = _multiplexer.AddSubscriptionQueue(channel, queue);

        // Send subscribe command if subscription was added.
        if (added)
        {
            await SendSubscribeCommand(channel);
        }

        return queue;
    }

    public async Task UnsubscribeAsync(ValkeyChannel channel, Action<ValkeyChannel, ValkeyValue>? handler = null, CommandFlags flags = CommandFlags.None)
    {
        // Validate arguments.
        ThrowIfChannelNullOrEmpty(channel);
        GuardClauses.ThrowIfCommandFlags(flags);

        // Remove handler from subscription or remove entire subscription.
        bool removed;
        if (handler != null)
        {
            removed = _multiplexer.RemoveSubscriptionHandler(channel, handler);
        }
        else
        {
            _multiplexer.RemoveSubscription(channel);
            removed = true;
        }

        // Send unsubscribe command if subscription was removed.
        if (removed)
        {
            await SendUnsubscribeCommand(channel);
        }
    }

    /// <summary>
    /// Unsubscribes from the specified channel queue.
    /// </summary>
    /// <param name="queue">The channel message queue to unsubscribe.</param>
    /// <returns></returns>
    internal async Task UnsubscribeAsync(ChannelMessageQueue queue)
    {
        // Validate arguments.
        var channel = queue.Channel;
        ThrowIfChannelNullOrEmpty(channel);

        // Remove queue from multiplexer.
        bool removed = _multiplexer.RemoveSubscriptionQueue(queue);

        // Send unsubscribe command if subscription was removed.
        if (removed)
        {
            await SendUnsubscribeCommand(channel);
        }
    }

    public async Task UnsubscribeAllAsync(CommandFlags flags = CommandFlags.None)
    {
        // Validate arguments.
        GuardClauses.ThrowIfCommandFlags(flags);

        // Remove all subscriptions from multiplexer.
        _multiplexer.RemoveAllSubscriptions();

        // Send unsubscribe commands for all channel modes.
        await _client.UnsubscribeAsync();
        await _client.PUnsubscribeAsync();

        if (_client is GlideClusterClient clusterClient)
        {
            var route = Route.Random;
            await _client.Command(Request.CustomCommand(["SUNSUBSCRIBE"]), route);
        }
    }

    #endregion
    #region NotSupportedMethods

    public bool IsConnected(ValkeyChannel channel = default)
    => throw new NotImplementedException("This method is not supported by Valkey GLIDE.");

    public EndPoint? IdentifyEndpoint(ValkeyChannel channel, CommandFlags flags = CommandFlags.None)
        => throw new NotImplementedException("This method is not supported by Valkey GLIDE.");

    public Task<EndPoint?> IdentifyEndpointAsync(ValkeyChannel channel, CommandFlags flags = CommandFlags.None)
        => throw new NotImplementedException("This method is not supported by Valkey GLIDE.");

    public EndPoint? SubscribedEndpoint(ValkeyChannel channel)
        => throw new NotImplementedException("This method is not supported by Valkey GLIDE.");

    #endregion
    #region HelperMethods

    /// <summary>
    /// Sends a PUBLISH command to the server with the specified channel and message.
    /// </summary>
    /// <param name="channel">The channel to publish to.</param>
    /// <param name="message">The message to publish.</param>
    /// <returns>The number of clients that received the message.</returns>
    private async Task<long> SendPublishCommand(ValkeyChannel channel, ValkeyValue message)
    {
        var channelStr = channel.ToString();
        var messageStr = message.ToString();

        if (channel.IsSharded)
        {
            ThrowIfNotClusterMode();

            // TODO #205: Refactor to use GlideClusterClient instead of custom command.
            var result = await _client.Command(Request.CustomCommand(["SPUBLISH", channelStr, messageStr]), Route.Random);
            return Convert.ToInt64(result);
        }

        return await _client.PublishAsync(channelStr, messageStr);
    }

    /// <summary>
    /// Sends a SUBSCRIBE command to the server for the specified channel.
    /// </summary>
    /// <param name="channel">The channel to subscribe to.</param>
    private async Task SendSubscribeCommand(ValkeyChannel channel)
    {
        var channelStr = channel.ToString();

        if (channel.IsSharded)
        {
            ThrowIfNotClusterMode();
            await _client.Command(Request.CustomCommand(["SSUBSCRIBE", channelStr]), Route.Random);
        }
        else if (channel.IsPattern)
        {
            await _client.PSubscribeAsync(channelStr);
        }
        else
        {
            await _client.SubscribeAsync(channelStr);
        }
    }

    /// <summary>
    /// Sends an UNSUBSCRIBE command to the server for the specified channel.
    /// </summary>
    /// <param name="channel">The channel to unsubscribe from.</param>
    private async Task SendUnsubscribeCommand(ValkeyChannel channel)
    {
        var channelStr = channel.ToString();

        if (channel.IsSharded)
        {
            ThrowIfNotClusterMode();

            // TODO #205: Refactor to use GlideClusterClient instead of custom command.
            await _client.Command(Request.CustomCommand(["SUNSUBSCRIBE", channelStr]), Route.Random);
        }
        else if (channel.IsPattern)
        {
            await _client.PUnsubscribeAsync(channelStr);
        }
        else
        {
            await _client.UnsubscribeAsync(channelStr);
        }
    }

    /// <summary>
    /// Throws if the client is not in cluster mode.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when sharded pub/sub is used in standalone mode.</exception>
    private void ThrowIfNotClusterMode()
    {
        if (!_client.IsCluster)
        {
            throw new InvalidOperationException("Sharded pub/sub is only supported in cluster mode.");
        }
    }

    /// <summary>
    /// Throws if the specified channel is null or empty.
    /// </summary>
    /// <param name="channel">The channel to check.</param>
    /// <exception cref="ArgumentException"></exception>
    private static void ThrowIfChannelNullOrEmpty(ValkeyChannel channel)
    {
        var channelStr = channel.ToString();
        var messageStr = message.ToString();

        if (channel.IsSharded)
        {
            if (_client is not GlideClusterClient clusterClient)
            {
                // TODO Implement support for sharded pub/sub.
                throw new NotImplementedException("Sharded PubSub is not yet supported in Valkey GLIDE.");
            }

            return await clusterClient.SPublishAsync(channelStr, messageStr);
        }

        return await _client.PublishAsync(channelStr, messageStr);
    }

    /// <summary>
    /// Sends a SUBSCRIBE command to the server for the specified channel.
    /// </summary>
    /// <param name="channel">The channel to subscribe to.</param>
    private async Task SendSubscribeCommand(ValkeyChannel channel)
    {
        var channelStr = channel.ToString();

        if (channel.IsSharded)
        {
            ThrowIfNotClusterMode();
            await _client.Command(Request.CustomCommand(["SSUBSCRIBE", channelStr]), Route.Random);
        }
        else if (channel.IsPattern)
        {
            await _client.PSubscribeAsync(channelStr);
        }
        else
        {
            await _client.SubscribeAsync(channelStr);
        }
    }

    /// <summary>
    /// Sends an UNSUBSCRIBE command to the server for the specified channel.
    /// </summary>
    /// <param name="channel">The channel to unsubscribe from.</param>
    private async Task SendUnsubscribeCommand(ValkeyChannel channel)
    {
        var channelStr = channel.ToString();

        if (channel.IsSharded)
        {
            ThrowIfNotClusterMode();
            await _client.Command(Request.CustomCommand(["SUNSUBSCRIBE", channelStr]), Route.Random);
        }
        else if (channel.IsPattern)
        {
            await _client.PUnsubscribeAsync(channelStr);
        }
        else
        {
            await _client.UnsubscribeAsync(channelStr);
        }
    }

    /// <summary>
    /// Throws if the client is not in cluster mode.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when sharded pub/sub is used in standalone mode.</exception>
    private void ThrowIfNotClusterMode()
    {
        if (!_client.IsCluster)
        {
            throw new InvalidOperationException("Sharded pub/sub is only supported in cluster mode.");
        }
    }

    /// <summary>
    /// Throws if the specified channel is null or empty.
    /// </summary>
    /// <param name="channel">The channel to check.</param>
    /// <exception cref="ArgumentException"></exception>
    private static void ThrowIfChannelNullOrEmpty(ValkeyChannel channel)
    {
        if (channel.IsNullOrEmpty)
            throw new ArgumentException("Channel cannot be null or empty");
    }

    #endregion
}
