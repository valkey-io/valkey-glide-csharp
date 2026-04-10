// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands;

namespace Valkey.Glide;

/// ATTENTION: Methods should only be added to this interface if they are implemented
/// by StackExchange.Redis databases but NOT by Valkey GLIDE clients. Methods implemented
/// by both should be added to <see cref="IHyperLogLogBaseCommands"/> instead.

public partial interface IDatabaseAsync
{
    /// <inheritdoc cref="IHyperLogLogBaseCommands.HyperLogLogAddAsync(ValkeyKey, ValkeyValue)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<bool> HyperLogLogAddAsync(ValkeyKey key, ValkeyValue element, CommandFlags flags);

    /// <inheritdoc cref="IHyperLogLogBaseCommands.HyperLogLogAddAsync(ValkeyKey, IEnumerable{ValkeyValue})"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<bool> HyperLogLogAddAsync(ValkeyKey key, IEnumerable<ValkeyValue> elements, CommandFlags flags);

    /// <inheritdoc cref="IHyperLogLogBaseCommands.HyperLogLogLengthAsync(ValkeyKey)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<long> HyperLogLogLengthAsync(ValkeyKey key, CommandFlags flags);

    /// <inheritdoc cref="IHyperLogLogBaseCommands.HyperLogLogLengthAsync(IEnumerable{ValkeyKey})"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<long> HyperLogLogLengthAsync(IEnumerable<ValkeyKey> keys, CommandFlags flags);

    /// <inheritdoc cref="IHyperLogLogBaseCommands.HyperLogLogMergeAsync(ValkeyKey, ValkeyKey, ValkeyKey)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task HyperLogLogMergeAsync(ValkeyKey destination, ValkeyKey first, ValkeyKey second, CommandFlags flags);

    /// <inheritdoc cref="IHyperLogLogBaseCommands.HyperLogLogMergeAsync(ValkeyKey, IEnumerable{ValkeyKey})"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task HyperLogLogMergeAsync(ValkeyKey destination, IEnumerable<ValkeyKey> sourceKeys, CommandFlags flags);
}
