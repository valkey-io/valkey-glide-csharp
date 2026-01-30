// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using System.Net;

namespace Valkey.Glide;

/// <summary>
/// A connection used as the subscriber in a pub/sub scenario.
/// Compatible with StackExchange.Redis <c>ISubscriber</c>.
/// </summary>
public interface ISubscriber
{
    /// <summary>
    /// Indicates whether the instance can communicate with the server.
    /// </summary>
    bool IsConnected(ValkeyChannel channel = default);

    /// <summary>
    /// Posts a message to the given channel.
    /// </summary>
    /// <returns>
    /// The number of clients that received the message.
    /// </returns>
    long Publish(ValkeyChannel channel, ValkeyValue message, CommandFlags flags = CommandFlags.None);

    /// <inheritdoc cref="Publish(ValkeyChannel, ValkeyValue, CommandFlags)"/>
    Task<long> PublishAsync(ValkeyChannel channel, ValkeyValue message, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Subscribe to perform some operation when a message is broadcast, without any guarantee of ordered handling.
    /// </summary>
    void Subscribe(ValkeyChannel channel, Action<ValkeyChannel, ValkeyValue> handler, CommandFlags flags = CommandFlags.None);

    /// <inheritdoc cref="Subscribe(ValkeyChannel, Action{ValkeyChannel, ValkeyValue}, CommandFlags)"/>
    Task SubscribeAsync(ValkeyChannel channel, Action<ValkeyChannel, ValkeyValue> handler, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Subscribe to perform some operation when a message is broadcast, as a queue that guarantees ordered handling.
    /// </summary>
    ChannelMessageQueue Subscribe(ValkeyChannel channel, CommandFlags flags = CommandFlags.None);

    /// <inheritdoc cref="Subscribe(ValkeyChannel, CommandFlags)"/>
    Task<ChannelMessageQueue> SubscribeAsync(ValkeyChannel channel, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Unsubscribe from a specified message channel.
    /// </summary>
    void Unsubscribe(ValkeyChannel channel, Action<ValkeyChannel, ValkeyValue>? handler = null, CommandFlags flags = CommandFlags.None);

    /// <inheritdoc cref="Unsubscribe(ValkeyChannel, Action{ValkeyChannel, ValkeyValue}?, CommandFlags)"/>
    Task UnsubscribeAsync(ValkeyChannel channel, Action<ValkeyChannel, ValkeyValue>? handler = null, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Unsubscribe all subscriptions on this instance.
    /// </summary>
    void UnsubscribeAll(CommandFlags flags = CommandFlags.None);

    /// <inheritdoc cref="UnsubscribeAll(CommandFlags)"/>
    Task UnsubscribeAllAsync(CommandFlags flags = CommandFlags.None);

    // Not supported by Valkey GLIDE
    // -----------------------------

    [Obsolete("This method is not supported by Valkey GLIDE.", error: true)]
    EndPoint? IdentifyEndpoint(ValkeyChannel channel, CommandFlags flags = CommandFlags.None);

    [Obsolete("This method is not supported by Valkey GLIDE.", error: true)]
    Task<EndPoint?> IdentifyEndpointAsync(ValkeyChannel channel, CommandFlags flags = CommandFlags.None);

    [Obsolete("This method is not supported by Valkey GLIDE.", error: true)]
    EndPoint? SubscribedEndpoint(ValkeyChannel channel);
}
