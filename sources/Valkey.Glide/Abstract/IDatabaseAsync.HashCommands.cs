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
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    Task<ValkeyValue> HashGetAsync(ValkeyKey key, ValkeyValue hashField, CommandFlags flags);

    /// <inheritdoc cref="IHashCommands.HashGetAsync(ValkeyKey, IEnumerable{ValkeyValue})"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    Task<ValkeyValue[]> HashGetAsync(ValkeyKey key, IEnumerable<ValkeyValue> hashFields, CommandFlags flags);

    /// <inheritdoc cref="IHashCommands.HashGetAllAsync(ValkeyKey)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    Task<HashEntry[]> HashGetAllAsync(ValkeyKey key, CommandFlags flags);

    /// <inheritdoc cref="IHashCommands.HashSetAsync(ValkeyKey, IEnumerable{HashEntry})"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    Task HashSetAsync(ValkeyKey key, IEnumerable<HashEntry> hashFields, CommandFlags flags);

    /// <inheritdoc cref="IHashCommands.HashSetAsync(ValkeyKey, ValkeyValue, ValkeyValue, When)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    Task<bool> HashSetAsync(ValkeyKey key, ValkeyValue hashField, ValkeyValue value, When when, CommandFlags flags);

    /// <inheritdoc cref="IHashCommands.HashDeleteAsync(ValkeyKey, ValkeyValue)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    Task<bool> HashDeleteAsync(ValkeyKey key, ValkeyValue hashField, CommandFlags flags);

    /// <inheritdoc cref="IHashCommands.HashDeleteAsync(ValkeyKey, IEnumerable{ValkeyValue})"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    Task<long> HashDeleteAsync(ValkeyKey key, IEnumerable<ValkeyValue> hashFields, CommandFlags flags);

    /// <inheritdoc cref="IHashCommands.HashExistsAsync(ValkeyKey, ValkeyValue)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    Task<bool> HashExistsAsync(ValkeyKey key, ValkeyValue hashField, CommandFlags flags);

    /// <inheritdoc cref="IHashCommands.HashIncrementAsync(ValkeyKey, ValkeyValue, long)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    Task<long> HashIncrementAsync(ValkeyKey key, ValkeyValue hashField, long value, CommandFlags flags);

    /// <inheritdoc cref="IHashCommands.HashIncrementAsync(ValkeyKey, ValkeyValue, double)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    Task<double> HashIncrementAsync(ValkeyKey key, ValkeyValue hashField, double value, CommandFlags flags);

    /// <inheritdoc cref="IHashCommands.HashKeysAsync(ValkeyKey)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    Task<ValkeyValue[]> HashKeysAsync(ValkeyKey key, CommandFlags flags);

    /// <inheritdoc cref="IHashCommands.HashLengthAsync(ValkeyKey)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    Task<long> HashLengthAsync(ValkeyKey key, CommandFlags flags);

    /// <inheritdoc cref="IHashCommands.HashScanAsync(ValkeyKey, ValkeyValue, int, long, int)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    IAsyncEnumerable<HashEntry> HashScanAsync(ValkeyKey key, ValkeyValue pattern, int pageSize, long cursor, int pageOffset, CommandFlags flags);

    /// <inheritdoc cref="IHashCommands.HashScanNoValuesAsync(ValkeyKey, ValkeyValue, int, long, int)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    IAsyncEnumerable<ValkeyValue> HashScanNoValuesAsync(ValkeyKey key, ValkeyValue pattern, int pageSize, long cursor, int pageOffset, CommandFlags flags);

    /// <inheritdoc cref="IHashCommands.HashStringLengthAsync(ValkeyKey, ValkeyValue)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    Task<long> HashStringLengthAsync(ValkeyKey key, ValkeyValue hashField, CommandFlags flags);

    /// <inheritdoc cref="IHashCommands.HashValuesAsync(ValkeyKey)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    Task<ValkeyValue[]> HashValuesAsync(ValkeyKey key, CommandFlags flags);

    /// <inheritdoc cref="IHashCommands.HashRandomFieldAsync(ValkeyKey)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    Task<ValkeyValue> HashRandomFieldAsync(ValkeyKey key, CommandFlags flags);

    /// <inheritdoc cref="IHashCommands.HashRandomFieldsAsync(ValkeyKey, long)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    Task<ValkeyValue[]> HashRandomFieldsAsync(ValkeyKey key, long count, CommandFlags flags);

    /// <inheritdoc cref="IHashCommands.HashRandomFieldsWithValuesAsync(ValkeyKey, long)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    Task<HashEntry[]> HashRandomFieldsWithValuesAsync(ValkeyKey key, long count, CommandFlags flags);

    /// <inheritdoc cref="IHashCommands.HashGetExAsync(ValkeyKey, IEnumerable{ValkeyValue}, HashGetExOptions)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    Task<ValkeyValue[]?> HashGetExAsync(ValkeyKey key, IEnumerable<ValkeyValue> fields, HashGetExOptions options, CommandFlags flags);

    /// <inheritdoc cref="IHashCommands.HashSetExAsync(ValkeyKey, IDictionary{ValkeyValue, ValkeyValue}, HashSetExOptions)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    Task<long> HashSetExAsync(ValkeyKey key, IDictionary<ValkeyValue, ValkeyValue> fieldValueMap, HashSetExOptions options, CommandFlags flags);

    /// <inheritdoc cref="IHashCommands.HashPersistAsync(ValkeyKey, IEnumerable{ValkeyValue})"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    Task<long[]> HashPersistAsync(ValkeyKey key, IEnumerable<ValkeyValue> fields, CommandFlags flags);

    /// <inheritdoc cref="IHashCommands.HashExpireAsync(ValkeyKey, long, IEnumerable{ValkeyValue}, HashFieldExpirationConditionOptions)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    Task<long[]> HashExpireAsync(ValkeyKey key, long seconds, IEnumerable<ValkeyValue> fields, HashFieldExpirationConditionOptions options, CommandFlags flags);

    /// <inheritdoc cref="IHashCommands.HashPExpireAsync(ValkeyKey, long, IEnumerable{ValkeyValue}, HashFieldExpirationConditionOptions)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    Task<long[]> HashPExpireAsync(ValkeyKey key, long milliseconds, IEnumerable<ValkeyValue> fields, HashFieldExpirationConditionOptions options, CommandFlags flags);

    /// <inheritdoc cref="IHashCommands.HashExpireAtAsync(ValkeyKey, long, IEnumerable{ValkeyValue}, HashFieldExpirationConditionOptions)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    Task<long[]> HashExpireAtAsync(ValkeyKey key, long unixSeconds, IEnumerable<ValkeyValue> fields, HashFieldExpirationConditionOptions options, CommandFlags flags);

    /// <inheritdoc cref="IHashCommands.HashPExpireAtAsync(ValkeyKey, long, IEnumerable{ValkeyValue}, HashFieldExpirationConditionOptions)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    Task<long[]> HashPExpireAtAsync(ValkeyKey key, long unixMilliseconds, IEnumerable<ValkeyValue> fields, HashFieldExpirationConditionOptions options, CommandFlags flags);

    /// <inheritdoc cref="IHashCommands.HashExpireTimeAsync(ValkeyKey, IEnumerable{ValkeyValue})"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    Task<long[]> HashExpireTimeAsync(ValkeyKey key, IEnumerable<ValkeyValue> fields, CommandFlags flags);

    /// <inheritdoc cref="IHashCommands.HashPExpireTimeAsync(ValkeyKey, IEnumerable{ValkeyValue})"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    Task<long[]> HashPExpireTimeAsync(ValkeyKey key, IEnumerable<ValkeyValue> fields, CommandFlags flags);

    /// <inheritdoc cref="IHashCommands.HashTtlAsync(ValkeyKey, IEnumerable{ValkeyValue})"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    Task<long[]> HashTtlAsync(ValkeyKey key, IEnumerable<ValkeyValue> fields, CommandFlags flags);

    /// <inheritdoc cref="IHashCommands.HashPTtlAsync(ValkeyKey, IEnumerable{ValkeyValue})"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    Task<long[]> HashPTtlAsync(ValkeyKey key, IEnumerable<ValkeyValue> fields, CommandFlags flags);
}
