// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.Commands;

/// <summary>
/// Pub/sub commands shared between <see cref="IBaseClient"/> and <see cref="IDatabaseAsync"/>.
/// </summary>
/// <remarks>
/// Intentionally empty — no shared methods for pub/sub commands.
/// GLIDE-style methods are on <see cref="IBaseClient"/>.
/// StackExchange.Redis-style methods are on <see cref="ISubscriber"/> and <see cref="IDatabaseAsync"/>.
/// </remarks>
/// <seealso href="https://valkey.io/commands/#pubsub">Valkey – Pub/Sub Commands</seealso>
/// <seealso href="https://glide.valkey.io/how-to/publish-and-subscribe-messages/">Valkey GLIDE – Pub/Sub Messaging</seealso>
public interface IPubSubBaseCommands { }
