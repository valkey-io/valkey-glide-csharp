// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands.Options;

namespace Valkey.Glide.Pipeline;

/// <summary>
/// Interface for standalone-specific batch operations that are not available in cluster mode.
/// </summary>
internal interface IBatchStandalone
{
    /// <inheritdoc cref="IGlideClient.CopyAsync(ValkeyKey, ValkeyKey, int, bool)" />
    /// <returns>Command Response - <see langword="true"/> if the key was copied.</returns>
    IBatch Copy(ValkeyKey source, ValkeyKey destination, int destinationDatabase, bool replace = false);

    /// <inheritdoc cref="IGlideClient.MigrateAsync(IEnumerable{ValkeyKey}, MigrateOptions)" />
    /// <returns>Command Response - <see langword="true"/> if the keys were migrated successfully, <see langword="false"/> if no keys were found.</returns>
    IBatch Migrate(IEnumerable<ValkeyKey> keys, MigrateOptions options);

    /// <inheritdoc cref="IGlideClient.MoveAsync(ValkeyKey, int)" />
    /// <returns>Command Response - <see langword="true"/> if the key was moved.</returns>
    IBatch Move(ValkeyKey key, int database);
}
