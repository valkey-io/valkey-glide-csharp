// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands;

namespace Valkey.Glide;

/// <summary>
/// Pub/sub commands for Valkey GLIDE clients.
/// </summary>
/// <remarks>
/// Methods should only be added to this interface if they are implemented by
/// <see cref="IBaseClient"/> but NOT by <see cref="IDatabaseAsync"/>. Methods implemented
/// by both should be added to <see cref="IPubSubBaseCommands"/> instead.
/// </remarks>
/// <seealso href="https://valkey.io/commands/#pubsub">Valkey – Pub/Sub Commands</seealso>
/// <seealso href="https://glide.valkey.io/how-to/publish-and-subscribe-messages/">Valkey GLIDE – Pub/Sub Messaging</seealso>
public partial interface IBaseClient : IPubSubBaseCommands
{
    #region PublishCommands

    /// <summary>
    /// Publishes a message to the specified channel.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/publish/">Valkey commands – PUBLISH</seealso>
    /// <param name="channel">The channel to publish the message to.</param>
    /// <param name="message">The message to publish.</param>
    /// <returns>The number of clients that received the message.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var subscriberCount = await client.PublishAsync("news", "Breaking news!");
    /// Console.WriteLine($"Delivered message to {subscriberCount} subscriber(s)");
    /// </code>
    /// </example>
    /// </remarks>
    abstract Task<long> PublishAsync(ValkeyKey channel, ValkeyValue message);

    #endregion
    #region SubscribeCommands

    /// <summary>
    /// Subscribes the client to the specified channel and waits for server confirmation.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/subscribe/">Valkey commands – SUBSCRIBE</seealso>
    /// <param name="channel">The channel to subscribe to.</param>
    /// <param name="timeout">Maximum time to wait for server confirmation. Waits indefinitely if not specified or <see cref="TimeSpan.Zero"/>.</param>
    /// <exception cref="ArgumentException">Thrown if <paramref name="timeout"/> is negative.</exception>
    /// <exception cref="Errors.TimeoutException">Thrown if server confirmation is not received within the specified <paramref name="timeout"/>.</exception>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.SubscribeAsync("news", TimeSpan.FromSeconds(5));
    /// Console.WriteLine("Subscribed to 'news' channel");
    /// </code>
    /// </example>
    /// </remarks>
    abstract Task SubscribeAsync(ValkeyKey channel, TimeSpan timeout);

    /// <summary>
    /// Subscribes the client to the specified channels and waits for server confirmation.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/subscribe/">Valkey commands – SUBSCRIBE</seealso>
    /// <param name="channels">A collection of channels to subscribe to.</param>
    /// <param name="timeout">Maximum time to wait for server confirmation. Waits indefinitely if not specified or <see cref="TimeSpan.Zero"/>.</param>
    /// <exception cref="ArgumentException">Thrown if <paramref name="timeout"/> is negative.</exception>
    /// <exception cref="Errors.TimeoutException">Thrown if server confirmation is not received within the specified <paramref name="timeout"/>.</exception>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.SubscribeAsync(["news", "updates"], TimeSpan.FromSeconds(5));
    /// Console.WriteLine("Subscribed to 'news' and 'updates' channels");
    /// </code>
    /// </example>
    /// </remarks>
    abstract Task SubscribeAsync(IEnumerable<ValkeyKey> channels, TimeSpan timeout);

    /// <summary>
    /// Subscribes the client to the specified channel and returns without waiting for server confirmation.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/subscribe/">Valkey commands – SUBSCRIBE</seealso>
    /// <param name="channel">The channel to subscribe to.</param>
    /// <remarks>
    /// The client subscribes asynchronously in the background.
    /// Use <see cref="GetSubscriptionsAsync"/> to verify the actual server subscription state.
    /// See <see cref="SubscribeAsync(ValkeyKey, TimeSpan)"/> for the blocking version.
    /// <example>
    /// <code>
    /// await client.SubscribeLazyAsync("news");
    /// Console.WriteLine("Subscribed to 'news' channel");
    /// </code>
    /// </example>
    /// </remarks>
    abstract Task SubscribeLazyAsync(ValkeyKey channel);

    /// <summary>
    /// Subscribes the client to the specified channels and returns without waiting for server confirmation.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/subscribe/">Valkey commands – SUBSCRIBE</seealso>
    /// <param name="channels">A collection of channels to subscribe to.</param>
    /// <remarks>
    /// The client subscribes asynchronously in the background.
    /// Use <see cref="GetSubscriptionsAsync"/> to verify the actual server subscription state.
    /// See <see cref="SubscribeAsync(IEnumerable{ValkeyKey}, TimeSpan)"/> for the blocking version.
    /// <example>
    /// <code>
    /// await client.SubscribeLazyAsync(["news", "updates"]);
    /// Console.WriteLine("Subscribed to 'news' and 'updates' channels");
    /// </code>
    /// </example>
    /// </remarks>
    abstract Task SubscribeLazyAsync(IEnumerable<ValkeyKey> channels);

    /// <summary>
    /// Subscribes the client to the specified pattern and waits for server confirmation.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/psubscribe/">Valkey commands – PSUBSCRIBE</seealso>
    /// <param name="pattern">The pattern to subscribe to.</param>
    /// <param name="timeout">Maximum time to wait for server confirmation. Waits indefinitely if not specified or <see cref="TimeSpan.Zero"/>.</param>
    /// <exception cref="ArgumentException">Thrown if <paramref name="timeout"/> is negative.</exception>
    /// <exception cref="Errors.TimeoutException">Thrown if server confirmation is not received within the specified <paramref name="timeout"/>.</exception>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.PSubscribeAsync("news.*", TimeSpan.FromSeconds(5));
    /// Console.WriteLine("Subscribed to 'news.*' pattern");
    /// </code>
    /// </example>
    /// </remarks>
    abstract Task PSubscribeAsync(ValkeyKey pattern, TimeSpan timeout);

    /// <summary>
    /// Subscribes the client to the specified patterns and waits for server confirmation.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/psubscribe/">Valkey commands – PSUBSCRIBE</seealso>
    /// <param name="patterns">A collection of patterns to subscribe to.</param>
    /// <param name="timeout">Maximum time to wait for server confirmation. Waits indefinitely if not specified or <see cref="TimeSpan.Zero"/>.</param>
    /// <exception cref="ArgumentException">Thrown if <paramref name="timeout"/> is negative.</exception>
    /// <exception cref="Errors.TimeoutException">Thrown if server confirmation is not received within the specified <paramref name="timeout"/>.</exception>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.PSubscribeAsync(["news.*", "updates.*"], TimeSpan.FromSeconds(5));
    /// Console.WriteLine("Subscribed to 'news.*' and 'updates.*' patterns");
    /// </code>
    /// </example>
    /// </remarks>
    abstract Task PSubscribeAsync(IEnumerable<ValkeyKey> patterns, TimeSpan timeout);

    /// <summary>
    /// Subscribes the client to the specified pattern and returns without waiting for server confirmation.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/psubscribe/">Valkey commands – PSUBSCRIBE</seealso>
    /// <param name="pattern">The pattern to subscribe to.</param>
    /// <remarks>
    /// The client subscribes asynchronously in the background.
    /// Use <see cref="GetSubscriptionsAsync"/> to verify the actual server subscription state.
    /// See <see cref="PSubscribeAsync(ValkeyKey, TimeSpan)"/> for the blocking version.
    /// <example>
    /// <code>
    /// await client.PSubscribeLazyAsync("news.*");
    /// Console.WriteLine("Subscribed to 'news.*' pattern");
    /// </code>
    /// </example>
    /// </remarks>
    abstract Task PSubscribeLazyAsync(ValkeyKey pattern);

    /// <summary>
    /// Subscribes the client to the specified patterns and returns without waiting for server confirmation.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/psubscribe/">Valkey commands – PSUBSCRIBE</seealso>
    /// <param name="patterns">A collection of patterns to subscribe to.</param>
    /// <remarks>
    /// The client subscribes asynchronously in the background.
    /// Use <see cref="GetSubscriptionsAsync"/> to verify the actual server subscription state.
    /// See <see cref="PSubscribeAsync(IEnumerable{ValkeyKey}, TimeSpan)"/> for the blocking version.
    /// <example>
    /// <code>
    /// await client.PSubscribeLazyAsync(["news.*", "updates.*"]);
    /// Console.WriteLine("Subscribed to 'news.*' and 'updates.*' patterns");
    /// </code>
    /// </example>
    /// </remarks>
    abstract Task PSubscribeLazyAsync(IEnumerable<ValkeyKey> patterns);

    #endregion
    #region UnsubscribeCommands

    /// <summary>
    /// Unsubscribes the client from all channels and waits for server confirmation.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/unsubscribe/">Valkey commands – UNSUBSCRIBE</seealso>
    /// <param name="timeout">Maximum time to wait for server confirmation. Waits indefinitely if not specified or <see cref="TimeSpan.Zero"/>.</param>
    /// <exception cref="ArgumentException">Thrown if <paramref name="timeout"/> is negative.</exception>
    /// <exception cref="Errors.TimeoutException">Thrown if server confirmation is not received within the specified <paramref name="timeout"/>.</exception>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.UnsubscribeAsync(TimeSpan.FromSeconds(5));
    /// Console.WriteLine("Unsubscribed from all channels");
    /// </code>
    /// </example>
    /// </remarks>
    abstract Task UnsubscribeAsync(TimeSpan timeout);

    /// <summary>
    /// Unsubscribes the client from the specified channel and waits for server confirmation.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/unsubscribe/">Valkey commands – UNSUBSCRIBE</seealso>
    /// <param name="channel">The channel to unsubscribe from.</param>
    /// <param name="timeout">Maximum time to wait for server confirmation. Waits indefinitely if not specified or <see cref="TimeSpan.Zero"/>.</param>
    /// <exception cref="ArgumentException">Thrown if <paramref name="timeout"/> is negative.</exception>
    /// <exception cref="Errors.TimeoutException">Thrown if server confirmation is not received within the specified <paramref name="timeout"/>.</exception>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.UnsubscribeAsync("news", TimeSpan.FromSeconds(5));
    /// Console.WriteLine("Unsubscribed from 'news' channel");
    /// </code>
    /// </example>
    /// </remarks>
    abstract Task UnsubscribeAsync(ValkeyKey channel, TimeSpan timeout);

    /// <summary>
    /// Unsubscribes the client from the specified channels and waits for server confirmation.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/unsubscribe/">Valkey commands – UNSUBSCRIBE</seealso>
    /// <param name="channels">A collection of channels to unsubscribe from. If empty or <see cref="PubSub.AllChannels"/>, unsubscribes from all channels.</param>
    /// <param name="timeout">Maximum time to wait for server confirmation. Waits indefinitely if not specified or <see cref="TimeSpan.Zero"/>.</param>
    /// <exception cref="ArgumentException">Thrown if <paramref name="timeout"/> is negative.</exception>
    /// <exception cref="Errors.TimeoutException">Thrown if server confirmation is not received within the specified <paramref name="timeout"/>.</exception>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.UnsubscribeAsync(["news", "updates"], TimeSpan.FromSeconds(5));
    /// Console.WriteLine("Unsubscribed from 'news' and 'updates' channels");
    /// </code>
    /// </example>
    /// </remarks>
    abstract Task UnsubscribeAsync(IEnumerable<ValkeyKey> channels, TimeSpan timeout);

    /// <summary>
    /// Unsubscribes the client from all channels and returns without waiting for server confirmation.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/unsubscribe/">Valkey commands – UNSUBSCRIBE</seealso>
    /// <remarks>
    /// The client unsubscribes asynchronously in the background.
    /// Use <see cref="GetSubscriptionsAsync"/> to verify the actual server subscription state.
    /// See <see cref="UnsubscribeAsync(TimeSpan)"/> for the blocking version.
    /// <example>
    /// <code>
    /// await client.UnsubscribeLazyAsync();
    /// Console.WriteLine("Unsubscribed from all channels");
    /// </code>
    /// </example>
    /// </remarks>
    abstract Task UnsubscribeLazyAsync();

    /// <summary>
    /// Unsubscribes the client from the specified channel and returns without waiting for server confirmation.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/unsubscribe/">Valkey commands – UNSUBSCRIBE</seealso>
    /// <param name="channel">The channel to unsubscribe from.</param>
    /// <remarks>
    /// The client unsubscribes asynchronously in the background.
    /// Use <see cref="GetSubscriptionsAsync"/> to verify the actual server subscription state.
    /// See <see cref="UnsubscribeAsync(ValkeyKey, TimeSpan)"/> for the blocking version.
    /// <example>
    /// <code>
    /// await client.UnsubscribeLazyAsync("news");
    /// Console.WriteLine("Unsubscribed from 'news' channel");
    /// </code>
    /// </example>
    /// </remarks>
    abstract Task UnsubscribeLazyAsync(ValkeyKey channel);

    /// <summary>
    /// Unsubscribes the client from the specified channels and returns without waiting for server confirmation.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/unsubscribe/">Valkey commands – UNSUBSCRIBE</seealso>
    /// <param name="channels">A collection of channels to unsubscribe from. If empty or <see cref="PubSub.AllChannels"/>, unsubscribes from all channels.</param>
    /// <remarks>
    /// The client unsubscribes asynchronously in the background.
    /// Use <see cref="GetSubscriptionsAsync"/> to verify the actual server subscription state.
    /// See <see cref="UnsubscribeAsync(IEnumerable{ValkeyKey}, TimeSpan)"/> for the blocking version.
    /// <example>
    /// <code>
    /// await client.UnsubscribeLazyAsync(["news", "updates"]);
    /// Console.WriteLine("Unsubscribed from 'news' and 'updates' channels");
    /// </code>
    /// </example>
    /// <example>
    /// <code>
    /// await client.UnsubscribeLazyAsync(PubSub.AllChannels);
    /// Console.WriteLine("Unsubscribed from all channels");
    /// </code>
    /// </example>
    /// </remarks>
    abstract Task UnsubscribeLazyAsync(IEnumerable<ValkeyKey> channels);

    /// <summary>
    /// Unsubscribes the client from all patterns and waits for server confirmation.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/punsubscribe/">Valkey commands – PUNSUBSCRIBE</seealso>
    /// <param name="timeout">Maximum time to wait for server confirmation. Waits indefinitely if not specified or <see cref="TimeSpan.Zero"/>.</param>
    /// <exception cref="ArgumentException">Thrown if <paramref name="timeout"/> is negative.</exception>
    /// <exception cref="Errors.TimeoutException">Thrown if server confirmation is not received within the specified <paramref name="timeout"/>.</exception>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.PUnsubscribeAsync(TimeSpan.FromSeconds(5));
    /// Console.WriteLine("Unsubscribed from all patterns");
    /// </code>
    /// </example>
    /// </remarks>
    abstract Task PUnsubscribeAsync(TimeSpan timeout);

    /// <summary>
    /// Unsubscribes the client from the specified pattern and waits for server confirmation.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/punsubscribe/">Valkey commands – PUNSUBSCRIBE</seealso>
    /// <param name="pattern">The pattern to unsubscribe from.</param>
    /// <param name="timeout">Maximum time to wait for server confirmation. Waits indefinitely if not specified or <see cref="TimeSpan.Zero"/>.</param>
    /// <exception cref="ArgumentException">Thrown if <paramref name="timeout"/> is negative.</exception>
    /// <exception cref="Errors.TimeoutException">Thrown if server confirmation is not received within the specified <paramref name="timeout"/>.</exception>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.PUnsubscribeAsync("news.*", TimeSpan.FromSeconds(5));
    /// Console.WriteLine("Unsubscribed from 'news.*' pattern");
    /// </code>
    /// </example>
    /// </remarks>
    abstract Task PUnsubscribeAsync(ValkeyKey pattern, TimeSpan timeout);

    /// <summary>
    /// Unsubscribes the client from the specified patterns and waits for server confirmation.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/punsubscribe/">Valkey commands – PUNSUBSCRIBE</seealso>
    /// <param name="patterns">A collection of patterns to unsubscribe from.</param>
    /// <param name="timeout">Maximum time to wait for server confirmation. Waits indefinitely if not specified or <see cref="TimeSpan.Zero"/>.</param>
    /// <exception cref="ArgumentException">Thrown if <paramref name="timeout"/> is negative.</exception>
    /// <exception cref="Errors.TimeoutException">Thrown if server confirmation is not received within the specified <paramref name="timeout"/>.</exception>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.PUnsubscribeAsync(["news.*", "updates.*"], TimeSpan.FromSeconds(5));
    /// Console.WriteLine("Unsubscribed from 'news.*' and 'updates.*' patterns");
    /// </code>
    /// </example>
    /// </remarks>
    abstract Task PUnsubscribeAsync(IEnumerable<ValkeyKey> patterns, TimeSpan timeout);

    /// <summary>
    /// Unsubscribes the client from all patterns and returns without waiting for server confirmation.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/punsubscribe/">Valkey commands – PUNSUBSCRIBE</seealso>
    /// <remarks>
    /// The client unsubscribes asynchronously in the background.
    /// Use <see cref="GetSubscriptionsAsync"/> to verify the actual server subscription state.
    /// See <see cref="PUnsubscribeAsync(TimeSpan)"/> for the blocking version.
    /// <example>
    /// <code>
    /// await client.PUnsubscribeLazyAsync();
    /// Console.WriteLine("Unsubscribed from all patterns");
    /// </code>
    /// </example>
    /// </remarks>
    abstract Task PUnsubscribeLazyAsync();

    /// <summary>
    /// Unsubscribes the client from the specified pattern and returns without waiting for server confirmation.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/punsubscribe/">Valkey commands – PUNSUBSCRIBE</seealso>
    /// <param name="pattern">The pattern to unsubscribe from.</param>
    /// <remarks>
    /// The client unsubscribes asynchronously in the background.
    /// Use <see cref="GetSubscriptionsAsync"/> to verify the actual server subscription state.
    /// See <see cref="PUnsubscribeAsync(ValkeyKey, TimeSpan)"/> for the blocking version.
    /// <example>
    /// <code>
    /// await client.PUnsubscribeLazyAsync("news.*");
    /// Console.WriteLine("Unsubscribed from 'news.*' pattern");
    /// </code>
    /// </example>
    /// </remarks>
    abstract Task PUnsubscribeLazyAsync(ValkeyKey pattern);

    /// <summary>
    /// Unsubscribes the client from the specified patterns and returns without waiting for server confirmation.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/punsubscribe/">Valkey commands – PUNSUBSCRIBE</seealso>
    /// <param name="patterns">A collection of patterns to unsubscribe from. If empty or <see cref="PubSub.AllPatterns"/>, unsubscribes from all patterns.</param>
    /// <remarks>
    /// The client unsubscribes asynchronously in the background.
    /// Use <see cref="GetSubscriptionsAsync"/> to verify the actual server subscription state.
    /// See <see cref="PUnsubscribeAsync(IEnumerable{ValkeyKey}, TimeSpan)"/> for the blocking version.
    /// <example>
    /// <code>
    /// await client.PUnsubscribeLazyAsync(["news.*", "updates.*"]);
    /// Console.WriteLine("Unsubscribed from 'news.*' and 'updates.*' patterns");
    /// </code>
    /// </example>
    /// <example>
    /// <code>
    /// await client.PUnsubscribeLazyAsync(PubSub.AllPatterns);
    /// Console.WriteLine("Unsubscribed from all patterns");
    /// </code>
    /// </example>
    /// </remarks>
    abstract Task PUnsubscribeLazyAsync(IEnumerable<ValkeyKey> patterns);

    #endregion
    #region IntrospectionCommands

    /// <summary>
    /// Lists the currently active channels.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/pubsub-channels/">Valkey commands – PUBSUB CHANNELS</seealso>
    /// <returns>A set of active channel names.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var activeChannels = await client.PubSubChannelsAsync();
    /// Console.WriteLine($"Active channels: {string.Join(", ", activeChannels)}");
    /// </code>
    /// </example>
    /// </remarks>
    abstract Task<ISet<ValkeyKey>> PubSubChannelsAsync();

    /// <summary>
    /// Lists the currently active channels matching the specified pattern.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/pubsub-channels/">Valkey commands – PUBSUB CHANNELS</seealso>
    /// <param name="pattern">A glob-style pattern to filter channel names.</param>
    /// <returns>A set of active channel names matching <paramref name="pattern"/>.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var newsChannels = await client.PubSubChannelsAsync("news.*");
    /// Console.WriteLine($"News channels: {string.Join(", ", newsChannels)}");
    /// </code>
    /// </example>
    /// </remarks>
    abstract Task<ISet<ValkeyKey>> PubSubChannelsAsync(ValkeyKey pattern);

    /// <summary>
    /// Returns the number of subscribers for the specified channel.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/pubsub-numsub/">Valkey commands – PUBSUB NUMSUB</seealso>
    /// <param name="channel">The channel name to query.</param>
    /// <returns>The number of subscribers for <paramref name="channel"/>.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var subscriberCount = await client.PubSubNumSubAsync("news");
    /// Console.WriteLine($"'news' has {subscriberCount} subscriber(s)");
    /// </code>
    /// </example>
    /// </remarks>
    abstract Task<long> PubSubNumSubAsync(ValkeyKey channel);

    /// <summary>
    /// Returns the number of subscribers for the specified channels.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/pubsub-numsub/">Valkey commands – PUBSUB NUMSUB</seealso>
    /// <param name="channels">A collection of channel names to query.</param>
    /// <returns>A dictionary mapping channel names to their subscriber counts.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var subscriberCounts = await client.PubSubNumSubAsync(["news", "updates"]);
    /// foreach (var kvp in subscriberCounts)
    /// {
    ///     Console.WriteLine($"{kvp.Key}: {kvp.Value} subscribers");
    /// }
    /// </code>
    /// </example>
    /// </remarks>
    abstract Task<Dictionary<ValkeyKey, long>> PubSubNumSubAsync(IEnumerable<ValkeyKey> channels);

    /// <summary>
    /// Returns the number of active pattern subscriptions.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/pubsub-numpat/">Valkey commands – PUBSUB NUMPAT</seealso>
    /// <returns>The number of patterns all clients are subscribed to.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var patternCount = await client.PubSubNumPatAsync();
    /// Console.WriteLine($"{patternCount} active pattern subscription(s)");
    /// </code>
    /// </example>
    /// </remarks>
    abstract Task<long> PubSubNumPatAsync();

    /// <summary>
    /// Returns the current pub/sub subscription state, including both the desired and
    /// actual subscriptions for the client.
    /// </summary>
    /// <returns>The <see cref="PubSubState"/> for this client.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var state = await client.GetSubscriptionsAsync();
    /// var desiredChannels = state.Desired[PubSubChannelMode.Exact];
    /// var actualPatterns = state.Actual[PubSubChannelMode.Pattern];
    /// </code>
    /// </example>
    /// </remarks>
    Task<PubSubState> GetSubscriptionsAsync();

    #endregion
}
