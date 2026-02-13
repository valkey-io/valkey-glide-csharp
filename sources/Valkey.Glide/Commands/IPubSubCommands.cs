// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.Commands;

/// <summary>
/// Pub/sub commands available in both standalone and cluster modes.
/// <br />
/// See more on <see href="https://valkey.io/commands/#pubsub">valkey.io</see>.
/// </summary>
public interface IPubSubCommands
{
    #region PublishCommands

    /// <summary>
    /// Publishes a message to the specified channel.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/publish/">valkey.io</seealso>
    /// <param name="channel">The channel to publish the message to.</param>
    /// <param name="message">The message to publish.</param>
    /// <returns>The number of clients that received the message.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// long subscriberCount = await clusterClient.PublishAsync("news", "Breaking news!");
    /// Console.WriteLine($"Message delivered to {subscriberCount} subscribers");
    /// </code>
    /// </example>
    /// </remarks>
    abstract Task<long> PublishAsync(string channel, string message);

    #endregion
    #region SubscribeCommands

    /// <summary>
    /// Subscribes the client to the specified channel and waits for server confirmation.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/subscribe/">valkey.io</seealso>
    /// <param name="channel">The channel to subscribe to.</param>
    /// <param name="timeout">Maximum time to wait for server confirmation. Waits indefinitely if not specified or <see cref="TimeSpan.Zero"/>.</param>
    /// <exception cref="ArgumentException">Thrown if timeout is negative.</exception>
    /// <exception cref="TimeoutException">Thrown if server confirmation is not received within the specified timeout.</exception>
    abstract Task SubscribeAsync(string channel, TimeSpan timeout = default);

    /// <summary>
    /// Subscribes the client to the specified channels and waits for server confirmation.
    /// <para />
    /// This command updates the client's desired subscription state and waits for server confirmation.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/subscribe/">valkey.io</seealso>
    /// <param name="channels">A collection of channels to subscribe to.</param>
    /// <param name="timeout">Maximum time to wait for server confirmation. Waits indefinitely if not specified or <see cref="TimeSpan.Zero"/>.</param>
    /// <exception cref="ArgumentException">Thrown if timeout is negative.</exception>
    /// <exception cref="TimeoutException">Thrown if server confirmation is not received within the specified timeout.</exception>
    abstract Task SubscribeAsync(IEnumerable<string> channels, TimeSpan timeout = default);

    /// <summary>
    /// Subscribes the client to the specified channel and returns without waiting for server confirmation.
    /// <para />
    /// See <see cref="SubscribeAsync(string, TimeSpan)"/> for the blocking version.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/subscribe/">valkey.io</seealso>
    /// <param name="channel">The channel to subscribe to.</param>
    /// <remarks>
    /// The client will attempt to subscribe asynchronously in the background.
    /// Use <see cref="GetSubscriptionsAsync"/> to verify the actual server subscription state.
    /// <example>
    /// <code>
    /// await client.SubscribeLazyAsync("news");
    /// Console.WriteLine("Subscribed to 'news' channel");
    /// </code>
    /// </example>
    /// </remarks>
    abstract Task SubscribeLazyAsync(string channel);

    /// <summary>
    /// Subscribes the client to the specified channels and returns without waiting for server confirmation.
    /// <para />
    /// See <see cref="SubscribeAsync(IEnumerable{string}, TimeSpan)"/> for the blocking version.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/subscribe/">valkey.io</seealso>
    /// <param name="channels">A collection of channels to subscribe to.</param>
    /// <remarks>
    /// The client will attempt to subscribe asynchronously in the background.
    /// Use <see cref="GetSubscriptionsAsync"/> to verify the actual server subscription state.
    /// <example>
    /// <code>
    /// var channels = new string[] { "news", "updates" };
    /// await client.SubscribeLazyAsync(channels);
    /// Console.WriteLine("Subscribed to 'news' and 'updates' channels");
    /// </code>
    /// </example>
    /// </remarks>
    abstract Task SubscribeLazyAsync(IEnumerable<string> channels);

    /// <summary>
    /// Subscribes the client to the specified pattern and waits for server confirmation.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/psubscribe/">valkey.io</seealso>
    /// <param name="pattern">The pattern to subscribe to.</param>
    /// <param name="timeout">Maximum time to wait for server confirmation. Waits indefinitely if not specified or <see cref="TimeSpan.Zero"/>.</param>
    /// <exception cref="ArgumentException">Thrown if timeout is negative.</exception>
    /// <exception cref="TimeoutException">Thrown if server confirmation is not received within the specified timeout.</exception>
    abstract Task PSubscribeAsync(string pattern, TimeSpan timeout = default);

    /// <summary>
    /// Subscribes the client to the specified patterns and waits for server confirmation.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/psubscribe/">valkey.io</seealso>
    /// <param name="patterns">A collection of patterns to subscribe to.</param>
    /// <param name="timeout">Maximum time to wait for server confirmation. Waits indefinitely if not specified or <see cref="TimeSpan.Zero"/>.</param>
    /// <exception cref="ArgumentException">Thrown if timeout is negative.</exception>
    /// <exception cref="TimeoutException">Thrown if server confirmation is not received within the specified timeout.</exception>
    abstract Task PSubscribeAsync(IEnumerable<string> patterns, TimeSpan timeout = default);

    /// <summary>
    /// Subscribes the client to the specified pattern and returns without waiting for server confirmation.
    /// <para />
    /// The client will attempt to subscribe asynchronously in the background.
    /// Use <see cref="IPubSubCommands.GetSubscriptionsAsync"/> to verify the
    /// actual server subscription state.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/psubscribe/">valkey.io</seealso>
    /// <param name="pattern">The pattern to subscribe to.</param>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.PSubscribeLazyAsync("news.*");
    /// Console.WriteLine("Subscribed to 'news.*' pattern");
    /// </code>
    /// </example>
    /// </remarks>
    abstract Task PSubscribeLazyAsync(string pattern);

    /// <summary>
    /// Subscribes the client to the specified patterns and returns without waiting for server confirmation.
    /// <para />
    /// The client will attempt to subscribe asynchronously in the background.
    /// Use <see cref="IPubSubCommands.GetSubscriptionsAsync"/> to verify the
    /// actual server subscription state.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/psubscribe/">valkey.io</seealso>
    /// <param name="patterns">A collection of patterns to subscribe to.</param>
    /// <remarks>
    /// <example>
    /// <code>
    /// var patterns = new string[] { "news.*", "updates.*" };
    /// await client.PSubscribeLazyAsync(patterns);
    /// Console.WriteLine("Subscribed to 'news.*' and 'updates.*' patterns");
    /// </code>
    /// </example>
    /// </remarks>
    abstract Task PSubscribeLazyAsync(IEnumerable<string> patterns);

    #endregion
    #region UnsubscribeCommands

    /// <summary>
    /// Unsubscribes the client from all channels and waits for server confirmation.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/unsubscribe/">valkey.io</seealso>
    /// <param name="timeout">Maximum time to wait for server confirmation. Waits indefinitely if not specified or <see cref="TimeSpan.Zero"/>.</param>
    /// <exception cref="ArgumentException">Thrown if timeout is negative.</exception>
    /// <exception cref="TimeoutException">Thrown if server confirmation is not received within the specified timeout.</exception>
    abstract Task UnsubscribeAsync(TimeSpan timeout = default);

    /// <summary>
    /// Unsubscribes the client from the specified channel and waits for server confirmation.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/unsubscribe/">valkey.io</seealso>
    /// <param name="channel">The channel to unsubscribe from.</param>
    /// <param name="timeout">Maximum time to wait for server confirmation. Waits indefinitely if not specified or <see cref="TimeSpan.Zero"/>.</param>
    /// <exception cref="ArgumentException">Thrown if timeout is negative.</exception>
    /// <exception cref="TimeoutException">Thrown if server confirmation is not received within the specified timeout.</exception>
    abstract Task UnsubscribeAsync(string channel, TimeSpan timeout = default);

    /// <summary>
    /// Unsubscribes the client from the specified channels and waits for server confirmation.
    /// <para />
    /// If no channels or <see cref="PubSub.AllChannels"/> is specified, unsubscribes the client from all channels.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/unsubscribe/">valkey.io</seealso>
    /// <param name="channels">A collection of channels to unsubscribe from.</param>
    /// <param name="timeout">Maximum time to wait for server confirmation. Waits indefinitely if not specified or <see cref="TimeSpan.Zero"/>.</param>
    /// <exception cref="ArgumentException">Thrown if timeout is negative.</exception>
    /// <exception cref="TimeoutException">Thrown if server confirmation is not received within the specified timeout.</exception>
    abstract Task UnsubscribeAsync(IEnumerable<string> channels, TimeSpan timeout = default);

    /// <summary>
    /// Unsubscribes the client from all channels and returns without waiting for server confirmation.
    /// <para />
    /// The client will attempt to unsubscribe asynchronously in the background.
    /// Use <see cref="IPubSubCommands.GetSubscriptionsAsync"/> to verify the
    /// actual server subscription state.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/unsubscribe/">valkey.io</seealso>
    /// <remarks>
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
    /// <para />
    /// The client will attempt to unsubscribe asynchronously in the background.
    /// Use <see cref="IPubSubCommands.GetSubscriptionsAsync"/> to verify the
    /// actual server subscription state.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/unsubscribe/">valkey.io</seealso>
    /// <param name="channel">The channel to unsubscribe from.</param>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.UnsubscribeLazyAsync("news");
    /// Console.WriteLine("Unsubscribed from 'news' channel");
    /// </code>
    /// </example>
    /// </remarks>
    abstract Task UnsubscribeLazyAsync(string channel);

    /// <summary>
    /// Unsubscribes the client from the specified channels and returns without waiting for server confirmation.
    /// <para />
    /// If no channels or <see cref="PubSub.AllChannels"/> is specified, unsubscribes the client from all channels.
    /// <para />
    /// The client will attempt to unsubscribe asynchronously in the background.
    /// Use <see cref="IPubSubCommands.GetSubscriptionsAsync"/> to verify the
    /// actual server subscription state.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/unsubscribe/">valkey.io</seealso>
    /// <param name="channels">A collection of channels to unsubscribe from.</param>
    /// <remarks>
    /// <example>
    /// <code>
    /// var channels = new string[] { "news", "updates" };
    /// await client.UnsubscribeLazyAsync(channels);
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
    abstract Task UnsubscribeLazyAsync(IEnumerable<string> channels);

    /// <summary>
    /// Unsubscribes the client from all patterns and waits for server confirmation.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/punsubscribe/">valkey.io</seealso>
    /// <param name="timeout">Maximum time to wait for server confirmation. Waits indefinitely if not specified or <see cref="TimeSpan.Zero"/>.</param>
    /// <exception cref="ArgumentException">Thrown if timeout is negative.</exception>
    /// <exception cref="TimeoutException">Thrown if server confirmation is not received within the specified timeout.</exception>
    abstract Task PUnsubscribeAsync(TimeSpan timeout = default);

    /// <summary>
    /// Unsubscribes the client from the specified pattern and waits for server confirmation.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/punsubscribe/">valkey.io</seealso>
    /// <param name="pattern">The pattern to unsubscribe from.</param>
    /// <param name="timeout">Maximum time to wait for server confirmation. Waits indefinitely if not specified or <see cref="TimeSpan.Zero"/>.</param>
    /// <exception cref="ArgumentException">Thrown if timeout is negative.</exception>
    /// <exception cref="TimeoutException">Thrown if server confirmation is not received within the specified timeout.</exception>
    abstract Task PUnsubscribeAsync(string pattern, TimeSpan timeout = default);

    /// <summary>
    /// Unsubscribes the client from the specified patterns and waits for server confirmation.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/punsubscribe/">valkey.io</seealso>
    /// <param name="patterns">A collection of patterns to unsubscribe from.</param>
    /// <param name="timeout">Maximum time to wait for server confirmation. Waits indefinitely if not specified or <see cref="TimeSpan.Zero"/>.</param>
    /// <exception cref="ArgumentException">Thrown if timeout is negative.</exception>
    /// <exception cref="TimeoutException">Thrown if server confirmation is not received within the specified timeout.</exception>
    abstract Task PUnsubscribeAsync(IEnumerable<string> patterns, TimeSpan timeout = default);

    /// <summary>
    /// Unsubscribes the client from all patterns and returns without waiting for server confirmation.
    /// <para />
    /// The client will attempt to unsubscribe asynchronously in the background.
    /// Use <see cref="IPubSubCommands.GetSubscriptionsAsync"/> to verify the
    /// actual server subscription state.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/punsubscribe/">valkey.io</seealso>
    /// <remarks>
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
    /// <para />
    /// The client will attempt to unsubscribe asynchronously in the background.
    /// Use <see cref="IPubSubCommands.GetSubscriptionsAsync"/> to verify the
    /// actual server subscription state.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/punsubscribe/">valkey.io</seealso>
    /// <param name="pattern">The pattern to unsubscribe from.</param>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.PUnsubscribeLazyAsync("news.*");
    /// Console.WriteLine("Unsubscribed from 'news.*' pattern");
    /// </code>
    /// </example>
    /// </remarks>
    abstract Task PUnsubscribeLazyAsync(string pattern);

    /// <summary>
    /// Unsubscribes the client from the specified patterns and returns without waiting for server confirmation.
    /// <para />
    /// If no patterns or <see cref="PubSub.AllPatterns"/> is specified, unsubscribes the client from all patterns.
    /// <para />
    /// The client will attempt to unsubscribe asynchronously in the background.
    /// Use <see cref="IPubSubCommands.GetSubscriptionsAsync"/> to verify the
    /// actual server subscription state.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/punsubscribe/">valkey.io</seealso>
    /// <param name="patterns">A collection of patterns to unsubscribe from.</param>
    /// <remarks>
    /// <example>
    /// <code>
    /// var patterns = new string[] { "news.*", "updates.*" };
    /// await client.PUnsubscribeLazyAsync(patterns);
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
    abstract Task PUnsubscribeLazyAsync(IEnumerable<string> patterns);

    #endregion
    #region IntrospectionCommands

    /// <summary>
    /// Lists the currently active channels.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/pubsub-channels/">valkey.io</seealso>
    /// <returns>A set of active channel names.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var channels = await client.PubSubChannelsAsync();
    /// Console.WriteLine($"Active channels: {string.Join("', '", channels)}");
    /// </code>
    /// </example>
    /// </remarks>
    abstract Task<ISet<string>> PubSubChannelsAsync();

    /// <summary>
    /// Lists the currently active channels matching the specified pattern.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/pubsub-channels/">valkey.io</seealso>
    /// <param name="pattern">A glob-style pattern to filter channel names.</param>
    /// <returns>A set of active channel names matching the pattern.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var channels = await client.PubSubChannelsAsync("news.*");
    /// Console.WriteLine($"News channels: {string.Join("', '", channels)}");
    /// </code>
    /// </example>
    /// </remarks>
    abstract Task<ISet<string>> PubSubChannelsAsync(string pattern);

    /// <summary>
    /// Returns the number of subscribers for the specified channels.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/pubsub-numsub/">valkey.io</seealso>
    /// <param name="channels">A set of channel names to query.</param>
    /// <returns>A dictionary mapping channel names to their subscriber counts.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var channels = new string[] { "news", "updates" };
    /// Dictionary&lt;string, long&gt; counts = await client.PubSubNumSubAsync(channels);
    /// foreach (var kvp in counts)
    /// {
    ///     Console.WriteLine($"{kvp.Key}: {kvp.Value} subscribers");
    /// }
    /// </code>
    /// </example>
    /// </remarks>
    abstract Task<Dictionary<string, long>> PubSubNumSubAsync(IEnumerable<string> channels);

    /// <summary>
    /// Returns the number of active pattern subscriptions.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/pubsub-numpat/">valkey.io</seealso>
    /// <returns>The number of patterns all clients are subscribed to.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// long patternCount = await client.PubSubNumPatAsync();
    /// Console.WriteLine($"Active pattern subscriptions: {patternCount}");
    /// </code>
    /// </example>
    /// </remarks>
    abstract Task<long> PubSubNumPatAsync();

    /// <summary>
    /// Returns the current pub/sub subscription state, which includes both the desired and
    /// actual subscriptions for the client. See <see cref="PubSubState"/> for more details.
    /// </summary>
    /// <returns>The pub/sub subscription state.</returns>
    /// <example>
    /// <code>
    /// var state = await client.GetSubscriptionsAsync();
    /// var desiredChannels = state.Desired[PubSubChannelMode.Exact];
    /// var actualPatterns = state.Actual[PubSubChannelMode.Pattern];
    /// </code>
    /// </example>
    Task<PubSubState> GetSubscriptionsAsync();

    #endregion
}
