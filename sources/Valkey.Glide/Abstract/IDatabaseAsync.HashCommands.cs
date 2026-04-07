// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands;
using Valkey.Glide.Commands.Options;

namespace Valkey.Glide;

/// <summary>
/// Hash commands with <see cref="CommandFlags"/> for StackExchange.Redis compatibility.
/// </summary>
/// <seealso cref="IHashBaseCommands" />
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

    /// <inheritdoc cref="IHashBaseCommands.HashSetAsync(ValkeyKey, IEnumerable{KeyValuePair{ValkeyValue, ValkeyValue}})" path="/*[not(self::param[@name='hashFieldsAndValues']) and not(self::returns)]"/>
    /// <param name="hashFields">The entries to set in the hash.</param>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task HashSetAsync(ValkeyKey key, IEnumerable<HashEntry> hashFields, CommandFlags flags);

    /// <inheritdoc cref="IHashBaseCommands.HashSetAsync(ValkeyKey, ValkeyValue, ValkeyValue)"/>
    /// <param name="when">The conditions under which to set the field value.</param>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="ArgumentException">Thrown if <paramref name="when"/> is <see cref="When.Exists"/>.</exception>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<bool> HashSetAsync(ValkeyKey key, ValkeyValue hashField, ValkeyValue value, When when, CommandFlags flags = CommandFlags.None);

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

    /// <inheritdoc cref="IHashBaseCommands.HashIncrementByAsync(ValkeyKey, ValkeyValue, long)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<long> HashIncrementAsync(ValkeyKey key, ValkeyValue hashField, long value = 1, CommandFlags flags = CommandFlags.None);

    /// <inheritdoc cref="IHashBaseCommands.HashIncrementByAsync(ValkeyKey, ValkeyValue, double)"/>
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
