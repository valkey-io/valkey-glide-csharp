// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.Commands;

/// <summary>
/// Pub/sub commands shared between Valkey GLIDE client and StackExchange.Redis-compatible database interfaces.
/// </summary>
/// <seealso href="https://valkey.io/commands/#pubsub">Valkey – Pub/Sub Commands</seealso>
/// <seealso href="https://glide.valkey.io/how-to/publish-and-subscribe-messages/">Valkey GLIDE – Pub/Sub Messaging</seealso>
public interface IPubSubBaseCommands
{
    /// Intentionally empty - no shared methods for pub/sub commands.
    /// GLIDE-style methods are in <see cref="IBaseClient.PubSubCommands"/>.
    /// StackExchange.Redis-style methods are in <see cref="ISubscriber"/> and <see cref="IDatabaseAsync.PubSubCommands"/>.
}
