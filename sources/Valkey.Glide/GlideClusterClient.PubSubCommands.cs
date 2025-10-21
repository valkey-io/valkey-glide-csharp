// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands;
using Valkey.Glide.Internals;

namespace Valkey.Glide;

public partial class GlideClusterClient : IPubSubClusterCommands
{
    /// <inheritdoc/>
    public async Task<long> PublishAsync(string channel, string message, CommandFlags flags = CommandFlags.None)
    {
        Utils.Requires<NotImplementedException>(flags == CommandFlags.None, "Command flags are not supported by GLIDE");
        return await Command(Request.Publish(channel, message), Route.Random);
    }

    /// <inheritdoc/>
    public async Task<long> PublishAsync(string channel, string message, bool sharded, CommandFlags flags = CommandFlags.None)
    {
        Utils.Requires<NotImplementedException>(flags == CommandFlags.None, "Command flags are not supported by GLIDE");

        return sharded
            ? await Command(Request.SPublish(channel, message), Route.Random)
            : await Command(Request.Publish(channel, message), Route.Random);
    }

    /// <inheritdoc/>
    public async Task<string[]> PubSubChannelsAsync(CommandFlags flags = CommandFlags.None)
    {
        Utils.Requires<NotImplementedException>(flags == CommandFlags.None, "Command flags are not supported by GLIDE");
        // In cluster mode, route to all primaries to get complete channel list
        return await Command(Request.PubSubChannels(), Route.AllPrimaries);
    }

    /// <inheritdoc/>
    public async Task<string[]> PubSubChannelsAsync(string pattern, CommandFlags flags = CommandFlags.None)
    {
        Utils.Requires<NotImplementedException>(flags == CommandFlags.None, "Command flags are not supported by GLIDE");
        // In cluster mode, route to all primaries to get complete channel list
        return await Command(Request.PubSubChannels(pattern), Route.AllPrimaries);
    }

    /// <inheritdoc/>
    public async Task<Dictionary<string, long>> PubSubNumSubAsync(string[] channels, CommandFlags flags = CommandFlags.None)
    {
        Utils.Requires<NotImplementedException>(flags == CommandFlags.None, "Command flags are not supported by GLIDE");
        GlideString[] channelArgs = channels.Select(c => (GlideString)c).ToArray();
        // In cluster mode, route to all primaries to aggregate subscriber counts
        return await Command(Request.PubSubNumSub(channelArgs), Route.AllPrimaries);
    }

    /// <inheritdoc/>
    public async Task<long> PubSubNumPatAsync(CommandFlags flags = CommandFlags.None)
    {
        Utils.Requires<NotImplementedException>(flags == CommandFlags.None, "Command flags are not supported by GLIDE");
        // In cluster mode, route to all primaries to aggregate pattern counts
        return await Command(Request.PubSubNumPat(), Route.AllPrimaries);
    }

    /// <inheritdoc/>
    public async Task<string[]> PubSubShardChannelsAsync(CommandFlags flags = CommandFlags.None)
    {
        Utils.Requires<NotImplementedException>(flags == CommandFlags.None, "Command flags are not supported by GLIDE");
        // In cluster mode, route to all primaries to get complete shard channel list
        return await Command(Request.PubSubShardChannels(), Route.AllPrimaries);
    }

    /// <inheritdoc/>
    public async Task<string[]> PubSubShardChannelsAsync(string pattern, CommandFlags flags = CommandFlags.None)
    {
        Utils.Requires<NotImplementedException>(flags == CommandFlags.None, "Command flags are not supported by GLIDE");
        // In cluster mode, route to all primaries to get complete shard channel list
        return await Command(Request.PubSubShardChannels(pattern), Route.AllPrimaries);
    }

    /// <inheritdoc/>
    public async Task<Dictionary<string, long>> PubSubShardNumSubAsync(string[] channels, CommandFlags flags = CommandFlags.None)
    {
        Utils.Requires<NotImplementedException>(flags == CommandFlags.None, "Command flags are not supported by GLIDE");
        GlideString[] channelArgs = channels.Select(c => (GlideString)c).ToArray();
        // In cluster mode, route to all primaries to aggregate shard subscriber counts
        return await Command(Request.PubSubShardNumSub(channelArgs), Route.AllPrimaries);
    }
}
