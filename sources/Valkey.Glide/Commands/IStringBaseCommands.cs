// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.Commands;

/// <summary>
/// String commands shared between <see cref="IBaseClient"/> and <see cref="IDatabaseAsync"/>.
/// </summary>
/// <remarks>
/// Intentionally empty — no shared methods for string commands.
/// GLIDE-style methods are on <see cref="IBaseClient"/>.
/// StackExchange.Redis-style methods are on <see cref="IDatabaseAsync"/>.
/// </remarks>
/// <seealso href="https://valkey.io/commands/#string">Valkey – String Commands</seealso>
public interface IStringBaseCommands { }
