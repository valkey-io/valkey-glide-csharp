// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.Commands;

// ATTENTION: Methods should only be added to this interface if they are implemented
// by both Valkey GLIDE clients and StackExchange.Redis databases.

/// <summary>
/// Bitmap commands shared between Valkey GLIDE clients and StackExchange.Redis-compatible interfaces.
/// </summary>
/// <seealso href="https://valkey.io/commands/#bitmap">Valkey – Bitmap Commands</seealso>
public interface IBitmapBaseCommands
{
    // Intentionally empty - no shared methods between GLIDE and SER naming for bitmap commands.
    // GLIDE-style methods (GetBitAsync, SetBitAsync, etc.) are in IBaseClient.
    // SER-style methods (StringGetBitAsync, StringSetBitAsync, etc.) are in IDatabaseAsync.
}
