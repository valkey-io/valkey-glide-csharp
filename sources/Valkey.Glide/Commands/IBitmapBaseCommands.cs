// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.Commands;

/// <summary>
/// Bitmap commands shared between <see cref="IBaseClient"/> and <see cref="IDatabaseAsync"/>.
/// </summary>
/// <seealso href="https://valkey.io/commands/#bitmap">Valkey – Bitmap Commands</seealso>
public interface IBitmapBaseCommands
{
    /// Intentionally empty - no shared methods for bitmap commands.
    /// GLIDE-style methods are in <see cref="IBaseClient.BitmapCommands"/>.
    /// StackExchange.Redis-style methods are in <see cref="IDatabaseAsync.BitmapCommands"/>.
}
