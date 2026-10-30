// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Internals;

namespace Valkey.Glide;

/// <summary>
/// Subscriber implementation for pub/sub operations.
/// </summary>
internal sealed class Subscriber : ISubscriber
{
    #region Constants

    // Default async timeout for StackExchange.Redis compabitility.
    private const int DefaultTimeoutMs = 5000;

    #endregion
    #region Private Fields

    private readonly ConnectionMultiplexer _multiplexer;
    private readonly Database _client;

    #endregion
    #region Constructors

    internal Subscriber(ConnectionMultiplexer multiplexer, Database client)
    {
        _multiplexer = multiplexer;
        _client = client;
    }

    #endregion
    #region Public Methods

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

    public async Task UnsubscribeAllAsync(CommandFlags flags = CommandFlags.None)
    {
        // Validate arguments.
        GuardClauses.ThrowIfCommandFlags(flags);

        // Remove all subscriptions from multiplexer.
        _multiplexer.RemoveAllSubscriptions();

        // Send unsubscribe commands for all channel modes.
        var timeout = GetTimeout();
        await _client.UnsubscribeAsync(timeout);
        await _client.PUnsubscribeAsync(timeout);

        if (_client.IsCluster)
        {
            // TODO #205: Refactor to use GlideClusterClient instead of custom command.
            _ = await _client.Command(Request.CustomCommand(["SUNSUBSCRIBE_BLOCKING", GetTimeoutMs().ToString()]), Route.Random);
        }
    }

    #endregion
    #region Internal Methods

    /// <summary>
    /// Unsubscribes from the specified channel queue.
    /// </summary>
    /// <param name="queue">The channel message queue to unsubscribe.</param>
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

    #endregion
    #region Private Methods

    /// <summary>
    /// Throws if the specified channel is null or empty.
    /// </summary>
    /// <param name="channel">The channel to check.</param>
    /// <exception cref="ArgumentException"></exception>
    private static void ThrowIfChannelNullOrEmpty(ValkeyChannel channel)
    {
        if (channel.IsNullOrEmpty)
        {
            throw new ArgumentException("Channel cannot be null or empty");
        }
    }

    /// <summary>
    /// Sends a PUBLISH command to the server with the specified channel and message.
    /// </summary>
    /// <param name="channel">The channel to publish to.</param>
    /// <param name="message">The message to publish.</param>
    /// <returns>The number of clients that received the message.</returns>
    private async Task<long> SendPublishCommand(ValkeyChannel channel, ValkeyValue message)
    {
        if (channel.IsSharded)
        {
            ThrowIfNotClusterMode();

            // TODO #205: Refactor to use GlideClusterClient instead of custom command.
            var result = await _client.Command(Request.CustomCommand(["SPUBLISH", channel.ToValkeyKey(), message.ToGlideString()]), Route.Random);
            return Convert.ToInt64(result);
        }

        return await _client.PublishAsync(channel.ToValkeyKey(), message);
    }

    /// <summary>
    /// Sends a SUBSCRIBE command to the server for the specified channel.
    /// </summary>
    /// <param name="channel">The channel to subscribe to.</param>
    private async Task SendSubscribeCommand(ValkeyChannel channel)
    {
        if (channel.IsSharded)
        {
            ThrowIfNotClusterMode();

            // TODO #205: Refactor to use GlideClusterClient instead of custom command.
            _ = await _client.Command(Request.CustomCommand(["SSUBSCRIBE_BLOCKING", channel.ToValkeyKey(), GetTimeoutMs().ToString()]), Route.Random);
        }
        else if (channel.IsPattern)
        {
            await _client.PSubscribeAsync(channel.ToValkeyKey(), GetTimeout());
        }
        else
        {
            await _client.SubscribeAsync(channel.ToValkeyKey(), GetTimeout());
        }
    }

    /// <summary>
    /// Sends an UNSUBSCRIBE command to the server for the specified channel.
    /// </summary>
    /// <param name="channel">The channel to unsubscribe from.</param>
    private async Task SendUnsubscribeCommand(ValkeyChannel channel)
    {
        if (channel.IsSharded)
        {
            ThrowIfNotClusterMode();

            // TODO #205: Refactor to use GlideClusterClient instead of custom command.
            _ = await _client.Command(Request.CustomCommand(["SUNSUBSCRIBE_BLOCKING", channel.ToValkeyKey(), GetTimeoutMs().ToString()]), Route.Random);
        }
        else if (channel.IsPattern)
        {
            await _client.PUnsubscribeAsync(channel.ToValkeyKey(), GetTimeout());
        }
        else
        {
            await _client.UnsubscribeAsync(channel.ToValkeyKey(), GetTimeout());
        }
    }

    /// <summary>
    /// Returns the timeout for subscribe/unsubscribe operations.
    /// </summary>
    private TimeSpan GetTimeout() => TimeSpan.FromMilliseconds(GetTimeoutMs());

    /// <summary>
    /// Returns the timeout in milliseconds for subscribe/unsubscribe operations.
    /// </summary>
    private int GetTimeoutMs() => _multiplexer.RawConfig.AsyncTimeout ?? DefaultTimeoutMs;

    /// <summary>
    /// Throws if the client is not in cluster mode.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if sharded pub/sub is used in standalone mode.</exception>
    private void ThrowIfNotClusterMode()
    {
        if (!_client.IsCluster)
        {
            throw new InvalidOperationException("Sharded pub/sub is only supported in cluster mode.");
        }
    }

    #endregion
}
