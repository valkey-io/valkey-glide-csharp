// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.Commands;

/// <summary>
/// String commands shared between <see cref="IBaseClient"/> and <see cref="IDatabaseAsync"/>.
/// </summary>
/// <seealso href="https://valkey.io/commands/#string">Valkey – String Commands</seealso>
public interface IStringBaseCommands
{
    /// Intentionally empty - no shared methods for string commands.
    /// GLIDE-style methods are in <see cref="IBaseClient.StringCommands"/>.
    /// StackExchange.Redis-style methods are in <see cref="IDatabaseAsync.StringCommands"/>.
}
