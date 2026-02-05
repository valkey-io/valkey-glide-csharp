// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using System.Net;

namespace Valkey.Glide;

/// <summary>
/// A connection used as the subscriber in a pub/sub scenario.
/// Compatible with StackExchange.Valkey <c>ISubscriber</c>.
/// </summary>
public interface ISubscriber
{
    #region SyncMethods

    /// <summary>
    /// Posts a message to the given channel.
    /// </summary>
    /// <param name="channel">The channel to publish to.</param>
    /// <param name="message">The message to publish.</param>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    /// <returns>
    /// The number of clients that received the message *on the destination server*,
    /// note that this doesn't mean much in a cluster as clients can get the message through other nodes.
    /// </returns>
    /// <remarks><seealso href="https://valkey.io/commands/publish"/></remarks>
    long Publish(ValkeyChannel channel, ValkeyValue message, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Subscribe to perform some operation when a message to the preferred/active node is broadcast, without any guarantee of ordered handling.
    /// </summary>
    /// <param name="channel">The channel to subscribe to.</param>
    /// <param name="handler">The handler to invoke when a message is received on <paramref name="channel"/>.</param>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    /// <remarks>
    /// See
    /// <seealso href="https://valkey.io/commands/subscribe"/>,
    /// <seealso href="https://valkey.io/commands/psubscribe"/>.
    /// <seealso href="https://valkey.io/commands/ssubscribe"/>.
    /// </remarks>
    void Subscribe(ValkeyChannel channel, Action<ValkeyChannel, ValkeyValue> handler, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Subscribe to perform some operation when a message to the preferred/active node is broadcast, as a queue that guarantees ordered handling.
    /// </summary>
    /// <param name="channel">The Valkey channel to subscribe to.</param>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    /// <returns>A channel that represents this source.</returns>
    /// <remarks>
    /// See
    /// <seealso href="https://valkey.io/commands/subscribe"/>,
    /// <seealso href="https://valkey.io/commands/psubscribe"/>.
    /// <seealso href="https://valkey.io/commands/ssubscribe"/>.
    /// </remarks>
    ChannelMessageQueue Subscribe(ValkeyChannel channel, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Unsubscribe from a specified message channel.
    /// Note: if no handler is specified, the subscription is canceled regardless of the subscribers.
    /// If a handler is specified, the subscription is only canceled if this handler is the last handler remaining against the channel.
    /// </summary>
    /// <param name="channel">The channel that was subscribed to.</param>
    /// <param name="handler">The handler to no longer invoke when a message is received on <paramref name="channel"/>.</param>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    /// <remarks>
    /// See
    /// <seealso href="https://valkey.io/commands/unsubscribe"/>,
    /// <seealso href="https://valkey.io/commands/punsubscribe"/>.
    /// <seealso href="https://valkey.io/commands/sunsubscribe"/>.
    /// </remarks>
    void Unsubscribe(ValkeyChannel channel, Action<ValkeyChannel, ValkeyValue>? handler = null, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Unsubscribe all subscriptions on this instance.
    /// </summary>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    /// <remarks>
    /// See
    /// <seealso href="https://valkey.io/commands/unsubscribe"/>,
    /// <seealso href="https://valkey.io/commands/punsubscribe"/>.
    /// <seealso href="https://valkey.io/commands/sunsubscribe"/>.
    /// </remarks>
    void UnsubscribeAll(CommandFlags flags = CommandFlags.None);

    #endregion
    #region AsyncMethods

    /// <inheritdoc cref="Publish(ValkeyChannel, ValkeyValue, CommandFlags)"/>
    Task<long> PublishAsync(ValkeyChannel channel, ValkeyValue message, CommandFlags flags = CommandFlags.None);

    /// <inheritdoc cref="Subscribe(ValkeyChannel, Action{ValkeyChannel, ValkeyValue}, CommandFlags)"/>
    Task SubscribeAsync(ValkeyChannel channel, Action<ValkeyChannel, ValkeyValue> handler, CommandFlags flags = CommandFlags.None);

    /// <inheritdoc cref="Subscribe(ValkeyChannel, CommandFlags)"/>
    Task<ChannelMessageQueue> SubscribeAsync(ValkeyChannel channel, CommandFlags flags = CommandFlags.None);

    /// <inheritdoc cref="Unsubscribe(ValkeyChannel, Action{ValkeyChannel, ValkeyValue}?, CommandFlags)"/>
    Task UnsubscribeAsync(ValkeyChannel channel, Action<ValkeyChannel, ValkeyValue>? handler = null, CommandFlags flags = CommandFlags.None);

    /// <inheritdoc cref="UnsubscribeAll(CommandFlags)"/>
    Task UnsubscribeAllAsync(CommandFlags flags = CommandFlags.None);

    #endregion
    #region NotSupportedMethods

    [Obsolete("This method is not supported by Valkey GLIDE.", error: true)]
    bool IsConnected(ValkeyChannel channel = default);

    [Obsolete("This method is not supported by Valkey GLIDE.", error: true)]
    EndPoint? IdentifyEndpoint(ValkeyChannel channel, CommandFlags flags = CommandFlags.None);

    [Obsolete("This method is not supported by Valkey GLIDE.", error: true)]
    Task<EndPoint?> IdentifyEndpointAsync(ValkeyChannel channel, CommandFlags flags = CommandFlags.None);

    [Obsolete("This method is not supported by Valkey GLIDE.", error: true)]
    EndPoint? SubscribedEndpoint(ValkeyChannel channel);

    #endregion
}
