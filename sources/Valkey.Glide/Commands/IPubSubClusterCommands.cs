// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.Commands;

/// <summary>
/// Pub/sub cluster commands shared between Valkey GLIDE client and StackExchange.Redis-compatible database interfaces.
/// </summary>
/// <seealso href="https://valkey.io/commands/#pubsub">Valkey – Pub/Sub Commands</seealso>
public interface IPubSubClusterCommands
{
    /// Intentionally empty - no shared methods for pub/sub cluster commands.
    /// GLIDE-style methods are in <see cref="IBaseClient.PubSubClusterCommands"/>.
    /// StackExchange.Redis-style does not supported sharded pub/sub.
}
