// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands;
using Valkey.Glide.Commands.Options;

namespace Valkey.Glide;

/// ATTENTION: Methods should only be added to this interface if they are implemented
/// by StackExchange.Redis databases but NOT by Valkey GLIDE clients. Methods implemented
/// by both should be added to <see cref="IHashBaseCommands"/> instead.

public partial interface IDatabaseAsync
{
    /// <inheritdoc cref="IHashBaseCommands.HashGetAsync(ValkeyKey, ValkeyValue)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<ValkeyValue> HashGetAsync(ValkeyKey key, ValkeyValue hashField, CommandFlags flags);

    /// <inheritdoc cref="IHashBaseCommands.HashGetAsync(ValkeyKey, IEnumerable{ValkeyValue})"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<ValkeyValue[]> HashGetAsync(ValkeyKey key, IEnumerable<ValkeyValue> hashFields, CommandFlags flags);

    /// <inheritdoc cref="IHashBaseCommands.HashGetAllAsync(ValkeyKey)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<HashEntry[]> HashGetAllAsync(ValkeyKey key, CommandFlags flags);

    /// <inheritdoc cref="IHashBaseCommands.HashSetAsync(ValkeyKey, IEnumerable{HashEntry})"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task HashSetAsync(ValkeyKey key, IEnumerable<HashEntry> hashFields, CommandFlags flags);

    /// <inheritdoc cref="IHashBaseCommands.HashSetAsync(ValkeyKey, ValkeyValue, ValkeyValue, When)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<bool> HashSetAsync(ValkeyKey key, ValkeyValue hashField, ValkeyValue value, When when = When.Always, CommandFlags flags = CommandFlags.None);

    /// <inheritdoc cref="IHashBaseCommands.HashDeleteAsync(ValkeyKey, ValkeyValue)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<bool> HashDeleteAsync(ValkeyKey key, ValkeyValue hashField, CommandFlags flags);

    /// <inheritdoc cref="IHashBaseCommands.HashDeleteAsync(ValkeyKey, IEnumerable{ValkeyValue})"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<long> HashDeleteAsync(ValkeyKey key, IEnumerable<ValkeyValue> hashFields, CommandFlags flags);

    /// <inheritdoc cref="IHashBaseCommands.HashExistsAsync(ValkeyKey, ValkeyValue)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<bool> HashExistsAsync(ValkeyKey key, ValkeyValue hashField, CommandFlags flags);

    /// <inheritdoc cref="IHashBaseCommands.HashIncrementAsync(ValkeyKey, ValkeyValue, long)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<long> HashIncrementAsync(ValkeyKey key, ValkeyValue hashField, long value = 1, CommandFlags flags = CommandFlags.None);

    /// <inheritdoc cref="IHashBaseCommands.HashIncrementAsync(ValkeyKey, ValkeyValue, double)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<double> HashIncrementAsync(ValkeyKey key, ValkeyValue hashField, double value, CommandFlags flags);

    /// <inheritdoc cref="IHashBaseCommands.HashKeysAsync(ValkeyKey)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<ValkeyValue[]> HashKeysAsync(ValkeyKey key, CommandFlags flags);

    /// <inheritdoc cref="IHashBaseCommands.HashLengthAsync(ValkeyKey)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<long> HashLengthAsync(ValkeyKey key, CommandFlags flags);

    /// <inheritdoc cref="IHashBaseCommands.HashScanAsync(ValkeyKey, ValkeyValue, int, long, int)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    IAsyncEnumerable<HashEntry> HashScanAsync(ValkeyKey key, ValkeyValue pattern = default, int pageSize = 250, long cursor = 0, int pageOffset = 0, CommandFlags flags = CommandFlags.None);

    /// <inheritdoc cref="IHashBaseCommands.HashScanNoValuesAsync(ValkeyKey, ValkeyValue, int, long, int)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    IAsyncEnumerable<ValkeyValue> HashScanNoValuesAsync(ValkeyKey key, ValkeyValue pattern = default, int pageSize = 250, long cursor = 0, int pageOffset = 0, CommandFlags flags = CommandFlags.None);

    /// <inheritdoc cref="IHashBaseCommands.HashStringLengthAsync(ValkeyKey, ValkeyValue)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<long> HashStringLengthAsync(ValkeyKey key, ValkeyValue hashField, CommandFlags flags);

    /// <inheritdoc cref="IHashBaseCommands.HashValuesAsync(ValkeyKey)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<ValkeyValue[]> HashValuesAsync(ValkeyKey key, CommandFlags flags);

    /// <inheritdoc cref="IHashBaseCommands.HashRandomFieldAsync(ValkeyKey)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<ValkeyValue> HashRandomFieldAsync(ValkeyKey key, CommandFlags flags);

    /// <inheritdoc cref="IHashBaseCommands.HashRandomFieldsAsync(ValkeyKey, long)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<ValkeyValue[]> HashRandomFieldsAsync(ValkeyKey key, long count, CommandFlags flags);

    /// <inheritdoc cref="IHashBaseCommands.HashRandomFieldsWithValuesAsync(ValkeyKey, long)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<HashEntry[]> HashRandomFieldsWithValuesAsync(ValkeyKey key, long count, CommandFlags flags);

    /// <inheritdoc cref="IHashBaseCommands.HashGetExAsync(ValkeyKey, IEnumerable{ValkeyValue}, HashGetExOptions)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<ValkeyValue[]?> HashGetExAsync(ValkeyKey key, IEnumerable<ValkeyValue> fields, HashGetExOptions options, CommandFlags flags);

    /// <inheritdoc cref="IHashBaseCommands.HashSetExAsync(ValkeyKey, IDictionary{ValkeyValue, ValkeyValue}, HashSetExOptions)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<long> HashSetExAsync(ValkeyKey key, IDictionary<ValkeyValue, ValkeyValue> fieldValueMap, HashSetExOptions options, CommandFlags flags);
}
