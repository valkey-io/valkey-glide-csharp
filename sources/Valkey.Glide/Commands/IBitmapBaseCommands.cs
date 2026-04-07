// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.Commands;

/// <summary>
/// Bitmap commands shared between Valkey GLIDE clients and StackExchange.Redis-compatible interfaces.
/// </summary>
/// <seealso href="https://valkey.io/commands/#bitmap">Valkey – Bitmap Commands</seealso>
/// <remarks>
/// <para>
/// This interface is intentionally empty because bitmap commands have different naming conventions
/// between Valkey GLIDE (e.g., <c>GetBitAsync</c>) and StackExchange.Redis (e.g., <c>StringGetBitAsync</c>).
/// </para>
/// <para>
/// For Valkey GLIDE-native methods, use <see cref="IBaseClient"/>.
/// For StackExchange.Redis-compatible methods, use <see cref="IDatabaseAsync"/>.
/// </para>
/// </remarks>
public interface IBitmapBaseCommands
{
    // Intentionally empty - no shared methods between GLIDE and SER naming for bitmap commands.
    // GLIDE-style methods (GetBitAsync, SetBitAsync, etc.) are in IBaseClient.
    // SER-style methods (StringGetBitAsync, StringSetBitAsync, etc.) are in IDatabaseAsync.
}
