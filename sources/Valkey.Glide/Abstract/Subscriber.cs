// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using System;
using System.Net;
using System.Threading.Tasks;

using Valkey.Glide.Internals;

namespace Valkey.Glide;

/// <summary>
/// Subscriber implementation for pub/sub operations.
/// </summary>
internal sealed class Subscriber : ISubscriber
{
    private readonly BaseClient _client;

    internal Subscriber(BaseClient client)
    {
        _client = client;
    }

    public long Publish(ValkeyChannel channel, ValkeyValue message, CommandFlags flags = CommandFlags.None)
    {
        // TODO #193: Implement Publish
        GuardClauses.ThrowIfCommandFlags(flags);
        throw new NotImplementedException();
    }

    public Task<long> PublishAsync(ValkeyChannel channel, ValkeyValue message, CommandFlags flags = CommandFlags.None)
    {
        // TODO #193: Implement PublishAsync
        GuardClauses.ThrowIfCommandFlags(flags);
        throw new NotImplementedException();
    }

    public void Subscribe(ValkeyChannel channel, Action<ValkeyChannel, ValkeyValue> handler, CommandFlags flags = CommandFlags.None)
    {
        // TODO #193: Implement Subscribe
        GuardClauses.ThrowIfCommandFlags(flags);
        throw new NotImplementedException();
    }

    public Task SubscribeAsync(ValkeyChannel channel, Action<ValkeyChannel, ValkeyValue> handler, CommandFlags flags = CommandFlags.None)
    {
        // TODO #193: Implement SubscribeAsync
        GuardClauses.ThrowIfCommandFlags(flags);
        throw new NotImplementedException();
    }

    public ChannelMessageQueue Subscribe(ValkeyChannel channel, CommandFlags flags = CommandFlags.None)
    {
        // TODO #193: Implement Subscribe
        GuardClauses.ThrowIfCommandFlags(flags);
        throw new NotImplementedException();
    }

    public Task<ChannelMessageQueue> SubscribeAsync(ValkeyChannel channel, CommandFlags flags = CommandFlags.None)
    {
        // TODO #193: Implement SubscribeAsync
        GuardClauses.ThrowIfCommandFlags(flags);
        throw new NotImplementedException();
    }

    public void Unsubscribe(ValkeyChannel channel, Action<ValkeyChannel, ValkeyValue>? handler = null, CommandFlags flags = CommandFlags.None)
    {
        // TODO #193: Implement Unsubscribe
        GuardClauses.ThrowIfCommandFlags(flags);
        throw new NotImplementedException();
    }

    public Task UnsubscribeAsync(ValkeyChannel channel, Action<ValkeyChannel, ValkeyValue>? handler = null, CommandFlags flags = CommandFlags.None)
    {
        // TODO #193: Implement UnsubscribeAsync
        GuardClauses.ThrowIfCommandFlags(flags);
        throw new NotImplementedException();
    }

    public void UnsubscribeAll(CommandFlags flags = CommandFlags.None)
    {
        // TODO #193: Implement UnsubscribeAll
        GuardClauses.ThrowIfCommandFlags(flags);
        throw new NotImplementedException();
    }

    public Task UnsubscribeAllAsync(CommandFlags flags = CommandFlags.None)
    {
        // TODO #193: Implement UnsubscribeAllAsync
        GuardClauses.ThrowIfCommandFlags(flags);
        throw new NotImplementedException();
    }

    // Not supported by Valkey GLIDE
    // -----------------------------

    public bool IsConnected(ValkeyChannel channel = default)
        => throw new NotImplementedException("This method is not supported by Valkey GLIDE.");

    public EndPoint? IdentifyEndpoint(ValkeyChannel channel, CommandFlags flags = CommandFlags.None)
        => throw new NotImplementedException("This method is not supported by Valkey GLIDE.");

    public Task<EndPoint?> IdentifyEndpointAsync(ValkeyChannel channel, CommandFlags flags = CommandFlags.None)
        => throw new NotImplementedException("This method is not supported by Valkey GLIDE.");

    public EndPoint? SubscribedEndpoint(ValkeyChannel channel)
        => throw new NotImplementedException("This method is not supported by Valkey GLIDE.");
}
