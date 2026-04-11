// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands;

namespace Valkey.Glide.Pipeline;

/// <summary>
/// Interface for standalone-specific batch operations that are not available in cluster mode.
/// </summary>
internal interface IBatchStandalone
{
    /// <inheritdoc cref="IGenericCommands.MoveAsync(ValkeyKey, int)" />
    /// <returns>Command Response - <see langword="true"/> if the key was moved.</returns>
    IBatch Move(ValkeyKey key, int database);

    /// <inheritdoc cref="IGenericCommands.CopyAsync(ValkeyKey, ValkeyKey, int, bool)" />
    /// <returns>Command Response - <see langword="true"/> if the key was copied.</returns>
    IBatch Copy(ValkeyKey source, ValkeyKey destination, int destinationDatabase, bool replace = false);

    /// <inheritdoc cref="Commands.IConnectionManagementBaseCommands.SelectAsync(long)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.IConnectionManagementBaseCommands.SelectAsync(long)" /></returns>
    IBatch SelectAsync(long index);
}
