// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands;
using Valkey.Glide.Commands.Options;

namespace Valkey.Glide;

/// <summary>
/// Hash commands with <see cref="CommandFlags"/> for StackExchange.Redis compatibility.
/// </summary>
/// <seealso cref="IHashCommands" />
public partial interface IDatabaseAsync
{
    /// <inheritdoc cref="IHashCommands.HashGetAsync(ValkeyKey, ValkeyValue)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<ValkeyValue> HashGetAsync(ValkeyKey key, ValkeyValue hashField, CommandFlags flags);

    /// <inheritdoc cref="IHashCommands.HashGetAsync(ValkeyKey, IEnumerable{ValkeyValue})"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<ValkeyValue[]> HashGetAsync(ValkeyKey key, IEnumerable<ValkeyValue> hashFields, CommandFlags flags);

    /// <inheritdoc cref="IHashCommands.HashGetAllAsync(ValkeyKey)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<HashEntry[]> HashGetAllAsync(ValkeyKey key, CommandFlags flags);

    /// <inheritdoc cref="IHashCommands.HashSetAsync(ValkeyKey, IEnumerable{HashEntry})"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task HashSetAsync(ValkeyKey key, IEnumerable<HashEntry> hashFields, CommandFlags flags);

    /// <inheritdoc cref="IHashCommands.HashSetAsync(ValkeyKey, ValkeyValue, ValkeyValue, When)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<bool> HashSetAsync(ValkeyKey key, ValkeyValue hashField, ValkeyValue value, When when = When.Always, CommandFlags flags = CommandFlags.None);

    /// <inheritdoc cref="IHashCommands.HashDeleteAsync(ValkeyKey, ValkeyValue)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<bool> HashDeleteAsync(ValkeyKey key, ValkeyValue hashField, CommandFlags flags);

    /// <inheritdoc cref="IHashCommands.HashDeleteAsync(ValkeyKey, IEnumerable{ValkeyValue})"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<long> HashDeleteAsync(ValkeyKey key, IEnumerable<ValkeyValue> hashFields, CommandFlags flags);

    /// <inheritdoc cref="IHashCommands.HashExistsAsync(ValkeyKey, ValkeyValue)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<bool> HashExistsAsync(ValkeyKey key, ValkeyValue hashField, CommandFlags flags);

    /// <inheritdoc cref="IHashCommands.HashIncrementAsync(ValkeyKey, ValkeyValue, long)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<long> HashIncrementAsync(ValkeyKey key, ValkeyValue hashField, long value = 1, CommandFlags flags = CommandFlags.None);

    /// <inheritdoc cref="IHashCommands.HashIncrementAsync(ValkeyKey, ValkeyValue, double)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<double> HashIncrementAsync(ValkeyKey key, ValkeyValue hashField, double value, CommandFlags flags);

    /// <inheritdoc cref="IHashCommands.HashKeysAsync(ValkeyKey)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<ValkeyValue[]> HashKeysAsync(ValkeyKey key, CommandFlags flags);

    /// <inheritdoc cref="IHashCommands.HashLengthAsync(ValkeyKey)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<long> HashLengthAsync(ValkeyKey key, CommandFlags flags);

    /// <inheritdoc cref="IHashCommands.HashScanAsync(ValkeyKey, ValkeyValue, int, long, int)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    IAsyncEnumerable<HashEntry> HashScanAsync(ValkeyKey key, ValkeyValue pattern = default, int pageSize = 250, long cursor = 0, int pageOffset = 0, CommandFlags flags = CommandFlags.None);

    /// <inheritdoc cref="IHashCommands.HashScanNoValuesAsync(ValkeyKey, ValkeyValue, int, long, int)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    IAsyncEnumerable<ValkeyValue> HashScanNoValuesAsync(ValkeyKey key, ValkeyValue pattern = default, int pageSize = 250, long cursor = 0, int pageOffset = 0, CommandFlags flags = CommandFlags.None);

    /// <inheritdoc cref="IHashCommands.HashStringLengthAsync(ValkeyKey, ValkeyValue)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<long> HashStringLengthAsync(ValkeyKey key, ValkeyValue hashField, CommandFlags flags);

    /// <inheritdoc cref="IHashCommands.HashValuesAsync(ValkeyKey)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<ValkeyValue[]> HashValuesAsync(ValkeyKey key, CommandFlags flags);

    /// <inheritdoc cref="IHashCommands.HashRandomFieldAsync(ValkeyKey)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<ValkeyValue> HashRandomFieldAsync(ValkeyKey key, CommandFlags flags);

    /// <inheritdoc cref="IHashCommands.HashRandomFieldsAsync(ValkeyKey, long)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<ValkeyValue[]> HashRandomFieldsAsync(ValkeyKey key, long count, CommandFlags flags);

    /// <inheritdoc cref="IHashCommands.HashRandomFieldsWithValuesAsync(ValkeyKey, long)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<HashEntry[]> HashRandomFieldsWithValuesAsync(ValkeyKey key, long count, CommandFlags flags);

    /// <inheritdoc cref="IHashCommands.HashGetExAsync(ValkeyKey, IEnumerable{ValkeyValue}, HashGetExOptions)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<ValkeyValue[]?> HashGetExAsync(ValkeyKey key, IEnumerable<ValkeyValue> fields, HashGetExOptions options, CommandFlags flags);

    /// <inheritdoc cref="IHashCommands.HashSetExAsync(ValkeyKey, IDictionary{ValkeyValue, ValkeyValue}, HashSetExOptions)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<long> HashSetExAsync(ValkeyKey key, IDictionary<ValkeyValue, ValkeyValue> fieldValueMap, HashSetExOptions options, CommandFlags flags);

}
