// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.Commands;

/// <summary>
/// Pub/sub cluster commands shared between <see cref="IBaseClient"/> and <see cref="IDatabaseAsync"/>.
/// </summary>
/// <seealso href="https://valkey.io/commands/#pubsub">Valkey – Pub/Sub Commands</seealso>
public interface IPubSubClusterCommands
{
    /// Intentionally empty - no shared methods for pub/sub cluster commands.
    /// GLIDE-style methods are in <see cref="IBaseClient.PubSubClusterCommands"/>.
    /// StackExchange.Redis-style methods are in <see cref="ISubscriber"/> and <see cref="IDatabaseAsync.PubSubCommands"/>.
    /// (StackExchange.Redis does not have any cluster-specific pub/sub methods).
}
