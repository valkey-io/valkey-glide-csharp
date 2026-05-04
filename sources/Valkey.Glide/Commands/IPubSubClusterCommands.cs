// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.Commands;

/// <summary>
/// Pub/sub cluster commands shared between <see cref="IBaseClient"/> and <see cref="IDatabaseAsync"/>.
/// </summary>
/// <remarks>
/// Intentionally empty — no shared methods for pub/sub cluster commands.
/// GLIDE-style methods are on <see cref="IBaseClient"/>.
/// StackExchange.Redis does not have cluster-specific pub/sub methods.
/// </remarks>
/// <seealso href="https://valkey.io/commands/#pubsub">Valkey – Pub/Sub Commands</seealso>
public interface IPubSubClusterCommands { }
