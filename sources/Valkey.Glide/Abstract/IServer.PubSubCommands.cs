// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide;

public partial interface IServer
{
    /// <inheritdoc cref="IBaseClient.PubSubChannelsAsync()"/>
    /// <param name="pattern">A glob-style pattern to filter channels. If not specified, all active channels are returned.</param>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <returns>An array of active channel names matching <paramref name="pattern"/>.</returns>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<ValkeyChannel[]> SubscriptionChannelsAsync(ValkeyChannel pattern = default, CommandFlags flags = CommandFlags.None);

    /// <inheritdoc cref="IBaseClient.PubSubNumPatAsync()"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<long> SubscriptionPatternCountAsync(CommandFlags flags = CommandFlags.None);

    /// <inheritdoc cref="IBaseClient.PubSubNumSubAsync(ValkeyKey)"/>
    /// <param name="channel">The channel to check.</param>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<long> SubscriptionSubscriberCountAsync(ValkeyChannel channel, CommandFlags flags = CommandFlags.None);
}
