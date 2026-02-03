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
    private readonly BaseClient _client;

    internal Subscriber(ConnectionMultiplexer multiplexer, BaseClient client)
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
        ThrowIfValkeyChannelNullOrEmpty(channel, nameof(channel));
        GuardClauses.ThrowIfCommandFlags(flags);

        return await SendPublishCommand(channel, message);
    }

    public async Task SubscribeAsync(ValkeyChannel channel, Action<ValkeyChannel, ValkeyValue> handler, CommandFlags flags = CommandFlags.None)
    {
        // Validate arguments.
        ThrowIfValkeyChannelNullOrEmpty(channel, nameof(channel));
        ArgumentNullException.ThrowIfNull(handler, nameof(handler));
        GuardClauses.ThrowIfCommandFlags(flags);

        // Add handler to multiplexer.
        var subscription = _multiplexer.GetSubscription(channel);
        subscription.AddHandler(handler);

        await SendSubscribeCommand(channel);
    }

    public async Task<ChannelMessageQueue> SubscribeAsync(ValkeyChannel channel, CommandFlags flags = CommandFlags.None)
    {
        // Validate arguments.
        ThrowIfValkeyChannelNullOrEmpty(channel, nameof(channel));
        GuardClauses.ThrowIfCommandFlags(flags);

        // Add new queue to multiplexer.
        var queue = new ChannelMessageQueue(channel, this);
        var subscription = _multiplexer.GetSubscription(channel);
        subscription.AddQueue(queue);

        await SendSubscribeCommand(channel);

        return queue;
    }

    public async Task UnsubscribeAsync(ValkeyChannel channel, Action<ValkeyChannel, ValkeyValue>? handler = null, CommandFlags flags = CommandFlags.None)
    {
        // Validate arguments.
        ThrowIfValkeyChannelNullOrEmpty(channel, nameof(channel));
        GuardClauses.ThrowIfCommandFlags(flags);

        // Remove handler or subscription from multiplexer.
        if (handler != null)
        {
            _multiplexer.RemoveHandler(channel, handler);
        }
        else
        {
            _multiplexer.RemoveSubscription(channel);
        }

        await SendUnsubscribeCommand(channel);
    }

    public async Task UnsubscribeAllAsync(CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);

        // Remove all subscriptions from multiplexer.
        _multiplexer.RemoveAllSubscriptions();

        await _client.UnsubscribeAsync();
        await _client.PUnsubscribeAsync();

        if (_client is GlideClusterClient clusterClient)
        {
            await clusterClient.SUnsubscribeAsync();
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
    /// Sends the PUBLISH command to the server with the specified channel and message.
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
            if (_client is not GlideClusterClient clusterClient)
                throw new ArgumentException("Can only publish to shard channels in cluster mode.");

            return await clusterClient.SPublishAsync(channelStr, messageStr);
        }
        else
        {
            return await _client.PublishAsync(channelStr, messageStr);
        }
    }

    /// <summary>
    /// Sends the SUBSCRIBE command to the server for the specified channel.
    /// </summary>
    /// <param name="channel">The channel to subscribe to.</param>
    private async Task SendSubscribeCommand(ValkeyChannel channel)
    {
        var channelStr = channel.ToString();

        if (channel.IsSharded)
        {
            if (_client is not GlideClusterClient clusterClient)
                throw new ArgumentException("Can only subscribe to shard channels in cluster mode.");

            await clusterClient.SSubscribeAsync(channelStr);
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
    /// Sends the UNSUBSCRIBE command to the server for the specified channel.
    /// </summary>
    /// <param name="channel">The channel to unsubscribe from.</param>
    private async Task SendUnsubscribeCommand(ValkeyChannel channel)
    {
        var channelStr = channel.ToString();

        if (channel.IsSharded)
        {
            if (_client is not GlideClusterClient clusterClient)
                throw new ArgumentException("Can only subscribe to shard channels in cluster mode.");

            await clusterClient.SUnsubscribeAsync(channelStr);
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
    /// Throws if the specified channel is null or empty.
    /// </summary>
    /// <param name="channel">The channel to check.</param>
    /// <param name="param">The name of the parameter to include in the exception message.</param>
    /// <exception cref="ArgumentException"></exception>
    private static void ThrowIfValkeyChannelNullOrEmpty(ValkeyChannel channel, string param)
    {
        if (channel.IsNullOrEmpty)
            throw new ArgumentException("Channel cannot be null or empty", param);
    }

    #endregion
}
