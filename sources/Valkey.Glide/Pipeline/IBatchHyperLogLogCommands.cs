// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands;

namespace Valkey.Glide.Pipeline;

/// <summary>
/// Supports commands for the "HyperLogLog" group for batch operations.
/// </summary>
internal interface IBatchHyperLogLogCommands
{
    /// <inheritdoc cref="IHyperLogLogCommands.HyperLogLogAddAsync(ValkeyKey, ValkeyValue, CommandFlags)" path="/summary" />
    /// <inheritdoc cref="IHyperLogLogCommands.HyperLogLogAddAsync(ValkeyKey, ValkeyValue, CommandFlags)" path="/param" />
    /// <returns>Command Response - <inheritdoc cref="IHyperLogLogCommands.HyperLogLogAddAsync(ValkeyKey, ValkeyValue, CommandFlags)" /></returns>
    IBatch HyperLogLogAdd(ValkeyKey key, ValkeyValue element);

    /// <inheritdoc cref="IHyperLogLogCommands.HyperLogLogAddAsync(ValkeyKey, ValkeyValue[], CommandFlags)" path="/summary" />
    /// <inheritdoc cref="IHyperLogLogCommands.HyperLogLogAddAsync(ValkeyKey, ValkeyValue[], CommandFlags)" path="/param" />
    /// <returns>Command Response - <inheritdoc cref="IHyperLogLogCommands.HyperLogLogAddAsync(ValkeyKey, ValkeyValue[], CommandFlags)" /></returns>
    IBatch HyperLogLogAdd(ValkeyKey key, ValkeyValue[] elements);

    /// <inheritdoc cref="IHyperLogLogCommands.HyperLogLogLengthAsync(ValkeyKey, CommandFlags)" path="/summary" />
    /// <inheritdoc cref="IHyperLogLogCommands.HyperLogLogLengthAsync(ValkeyKey, CommandFlags)" path="/param" />
    /// <returns>Command Response - <inheritdoc cref="IHyperLogLogCommands.HyperLogLogLengthAsync(ValkeyKey, CommandFlags)" /></returns>
    IBatch HyperLogLogLength(ValkeyKey key);

    /// <inheritdoc cref="IHyperLogLogCommands.HyperLogLogLengthAsync(ValkeyKey[], CommandFlags)" path="/summary" />
    /// <inheritdoc cref="IHyperLogLogCommands.HyperLogLogLengthAsync(ValkeyKey[], CommandFlags)" path="/param" />
    /// <returns>Command Response - <inheritdoc cref="IHyperLogLogCommands.HyperLogLogLengthAsync(ValkeyKey[], CommandFlags)" /></returns>
    IBatch HyperLogLogLength(ValkeyKey[] keys);

    /// <inheritdoc cref="IHyperLogLogCommands.HyperLogLogMergeAsync(ValkeyKey, ValkeyKey, ValkeyKey, CommandFlags)" path="/summary" />
    /// <inheritdoc cref="IHyperLogLogCommands.HyperLogLogMergeAsync(ValkeyKey, ValkeyKey, ValkeyKey, CommandFlags)" path="/param" />
    /// <returns>Command Response - <inheritdoc cref="IHyperLogLogCommands.HyperLogLogMergeAsync(ValkeyKey, ValkeyKey, ValkeyKey, CommandFlags)" /></returns>
    IBatch HyperLogLogMerge(ValkeyKey destination, ValkeyKey first, ValkeyKey second);

    /// <inheritdoc cref="IHyperLogLogCommands.HyperLogLogMergeAsync(ValkeyKey, ValkeyKey[], CommandFlags)" path="/summary" />
    /// <inheritdoc cref="IHyperLogLogCommands.HyperLogLogMergeAsync(ValkeyKey, ValkeyKey[], CommandFlags)" path="/param" />
    /// <returns>Command Response - <inheritdoc cref="IHyperLogLogCommands.HyperLogLogMergeAsync(ValkeyKey, ValkeyKey[], CommandFlags)" /></returns>
    IBatch HyperLogLogMerge(ValkeyKey destination, ValkeyKey[] sourceKeys);
}
