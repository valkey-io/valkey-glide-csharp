// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using System.Net;

using Valkey.Glide.Internals;

namespace Valkey.Glide;

/// <summary>
/// Subscriber implementation for pub/sub operations.
/// Comp
/// </summary>
internal sealed class Subscriber : ISubscriber
{
    private readonly BaseClient _client;

    internal Subscriber(BaseClient client)
    {
        _client = client;
    }

    #region SyncMethods

    /// <inheritdoc/>
    public long Publish(ValkeyChannel channel, ValkeyValue message, CommandFlags flags = CommandFlags.None)
        => PublishAsync(channel, message, flags).GetAwaiter().GetResult();

    /// <inheritdoc/>
    public void Subscribe(ValkeyChannel channel, Action<ValkeyChannel, ValkeyValue> handler, CommandFlags flags = CommandFlags.None)
        => SubscribeAsync(channel, handler, flags).GetAwaiter().GetResult();

    /// <inheritdoc/>
    public ChannelMessageQueue Subscribe(ValkeyChannel channel, CommandFlags flags = CommandFlags.None)
        => SubscribeAsync(channel, flags).GetAwaiter().GetResult();

    /// <inheritdoc/>
    public void Unsubscribe(ValkeyChannel channel, Action<ValkeyChannel, ValkeyValue>? handler = null, CommandFlags flags = CommandFlags.None)
        => UnsubscribeAsync(channel, handler, flags).GetAwaiter().GetResult();

    /// <inheritdoc/>
    public void UnsubscribeAll(CommandFlags flags = CommandFlags.None)
        => UnsubscribeAllAsync(flags).GetAwaiter().GetResult();

    #endregion
    #region AsyncMethods

    /// <inheritdoc/>
    public Task<long> PublishAsync(ValkeyChannel channel, ValkeyValue message, CommandFlags flags = CommandFlags.None)
    {
        string channelStr = channel.ToString();
        string messageStr = message.ToString();

        if (channel.IsSharded)
        {
            if (_client is GlideClusterClient clusterClient)
                return clusterClient.SPublishAsync(channelStr, messageStr, flags);

            throw new ArgumentException("Can only publish to sharded channels in cluster mode.");
        }

        return _client.PublishAsync(channelStr, messageStr, flags);
    }

    /// <inheritdoc/>
    public Task SubscribeAsync(ValkeyChannel channel, Action<ValkeyChannel, ValkeyValue> handler, CommandFlags flags = CommandFlags.None)
    {
        // TODO #193: Implement SubscribeAsync
        GuardClauses.ThrowIfCommandFlags(flags);
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public Task<ChannelMessageQueue> SubscribeAsync(ValkeyChannel channel, CommandFlags flags = CommandFlags.None)
    {
        // TODO #193: Implement SubscribeAsync
        GuardClauses.ThrowIfCommandFlags(flags);
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public Task UnsubscribeAsync(ValkeyChannel channel, Action<ValkeyChannel, ValkeyValue>? handler = null, CommandFlags flags = CommandFlags.None)
    {
        // TODO #193: Implement UnsubscribeAsync
        GuardClauses.ThrowIfCommandFlags(flags);
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public Task UnsubscribeAllAsync(CommandFlags flags = CommandFlags.None)
    {
        // TODO #193: Implement UnsubscribeAllAsync
        GuardClauses.ThrowIfCommandFlags(flags);
        throw new NotImplementedException();
    }

    #endregion
    #region NotSupportedMethods

    /// <inheritdoc/>
    public bool IsConnected(ValkeyChannel channel = default)
    => throw new NotImplementedException("This method is not supported by Valkey GLIDE.");

    /// <inheritdoc/>
    public EndPoint? IdentifyEndpoint(ValkeyChannel channel, CommandFlags flags = CommandFlags.None)
        => throw new NotImplementedException("This method is not supported by Valkey GLIDE.");

    /// <inheritdoc/>
    public Task<EndPoint?> IdentifyEndpointAsync(ValkeyChannel channel, CommandFlags flags = CommandFlags.None)
        => throw new NotImplementedException("This method is not supported by Valkey GLIDE.");

    /// <inheritdoc/>
    public EndPoint? SubscribedEndpoint(ValkeyChannel channel)
        => throw new NotImplementedException("This method is not supported by Valkey GLIDE.");

    #endregion
    #region HelperMethods

    // TODO #193

    #endregion
}
