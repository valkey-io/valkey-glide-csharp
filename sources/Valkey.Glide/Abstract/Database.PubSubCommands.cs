// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Internals;

namespace Valkey.Glide;

internal partial class Database
{
    /// <inheritdoc cref="IDatabaseAsync.PublishAsync(ValkeyChannel, ValkeyValue, CommandFlags)"/>
    public async Task<long> PublishAsync(
        ValkeyChannel channel,
        ValkeyValue message,
        CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);

        // Validate arguments.
        if (channel.IsNullOrEmpty)
        {
            throw new ArgumentException("Channel cannot be null or empty");
        }

        var channelStr = channel.ToString();
        var messageStr = message.ToString();

        if (channel.IsSharded)
        {
            if (!IsCluster)
            {
                throw new InvalidOperationException("Sharded pub/sub is only supported in cluster mode.");
            }

            // TODO #205: Refactor to use GlideClusterClient instead of custom command.
            var result = await Command(Request.CustomCommand(["SPUBLISH", channelStr, messageStr]), Route.Random);
            return Convert.ToInt64(result);
        }

        return await PublishAsync(channelStr, messageStr);
    }
}
