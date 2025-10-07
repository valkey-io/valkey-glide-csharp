// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.Pipeline;

/// <summary>
/// Interface for standalone-specific batch operations that are not available in cluster mode.
/// </summary>
internal interface IBatchStandalone
{
    /// <inheritdoc cref="Commands.IServerManagementCommands.SelectAsync(long, CommandFlags)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.IServerManagementCommands.SelectAsync(long, CommandFlags)" /></returns>
    IBatch SelectAsync(long index);
}
