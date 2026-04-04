// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands;

namespace Valkey.Glide.Pipeline;

/// <summary>
/// Supports commands for the "HyperLogLog" group for batch operations.
/// </summary>
internal interface IBatchHyperLogLogCommands
{
    /// <inheritdoc cref="IHyperLogLogCommands.HyperLogLogAddAsync(ValkeyKey, ValkeyValue)" path="/summary" />
    /// <inheritdoc cref="IHyperLogLogCommands.HyperLogLogAddAsync(ValkeyKey, ValkeyValue)" path="/param" />
    /// <returns>Command Response - <inheritdoc cref="IHyperLogLogCommands.HyperLogLogAddAsync(ValkeyKey, ValkeyValue)" /></returns>
    IBatch HyperLogLogAdd(ValkeyKey key, ValkeyValue element);

    /// <inheritdoc cref="IHyperLogLogCommands.HyperLogLogAddAsync(ValkeyKey, IEnumerable{ValkeyValue})" path="/summary" />
    /// <inheritdoc cref="IHyperLogLogCommands.HyperLogLogAddAsync(ValkeyKey, IEnumerable{ValkeyValue})" path="/param" />
    /// <returns>Command Response - <inheritdoc cref="IHyperLogLogCommands.HyperLogLogAddAsync(ValkeyKey, IEnumerable{ValkeyValue})" /></returns>
    IBatch HyperLogLogAdd(ValkeyKey key, IEnumerable<ValkeyValue> elements);

    /// <inheritdoc cref="IHyperLogLogCommands.HyperLogLogLengthAsync(ValkeyKey)" path="/summary" />
    /// <inheritdoc cref="IHyperLogLogCommands.HyperLogLogLengthAsync(ValkeyKey)" path="/param" />
    /// <returns>Command Response - <inheritdoc cref="IHyperLogLogCommands.HyperLogLogLengthAsync(ValkeyKey)" /></returns>
    IBatch HyperLogLogLength(ValkeyKey key);

    /// <inheritdoc cref="IHyperLogLogCommands.HyperLogLogLengthAsync(IEnumerable{ValkeyKey})" path="/summary" />
    /// <inheritdoc cref="IHyperLogLogCommands.HyperLogLogLengthAsync(IEnumerable{ValkeyKey})" path="/param" />
    /// <returns>Command Response - <inheritdoc cref="IHyperLogLogCommands.HyperLogLogLengthAsync(IEnumerable{ValkeyKey})" /></returns>
    IBatch HyperLogLogLength(IEnumerable<ValkeyKey> keys);

    /// <inheritdoc cref="IHyperLogLogCommands.HyperLogLogMergeAsync(ValkeyKey, ValkeyKey, ValkeyKey)" path="/summary" />
    /// <inheritdoc cref="IHyperLogLogCommands.HyperLogLogMergeAsync(ValkeyKey, ValkeyKey, ValkeyKey)" path="/param" />
    /// <returns>Command Response - <inheritdoc cref="IHyperLogLogCommands.HyperLogLogMergeAsync(ValkeyKey, ValkeyKey, ValkeyKey)" /></returns>
    IBatch HyperLogLogMerge(ValkeyKey destination, ValkeyKey first, ValkeyKey second);

    /// <inheritdoc cref="IHyperLogLogCommands.HyperLogLogMergeAsync(ValkeyKey, IEnumerable{ValkeyKey})" path="/summary" />
    /// <inheritdoc cref="IHyperLogLogCommands.HyperLogLogMergeAsync(ValkeyKey, IEnumerable{ValkeyKey})" path="/param" />
    /// <returns>Command Response - <inheritdoc cref="IHyperLogLogCommands.HyperLogLogMergeAsync(ValkeyKey, IEnumerable{ValkeyKey})" /></returns>
    IBatch HyperLogLogMerge(ValkeyKey destination, IEnumerable<ValkeyKey> sourceKeys);
}
