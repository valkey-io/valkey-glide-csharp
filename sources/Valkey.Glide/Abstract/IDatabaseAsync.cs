// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands;
using Valkey.Glide.Commands.Options;

namespace Valkey.Glide;

/// <summary>
/// Describes functionality that is common to both standalone and cluster servers.<br />
/// See also <see cref="GlideClient" /> and <see cref="GlideClusterClient" />.
/// </summary>
public interface IDatabaseAsync : IConnectionManagementCommands, IGenericCommands, IGenericBaseCommands, IHashCommands, IHyperLogLogCommands, IListCommands, IScriptingAndFunctionBaseCommands, IServerManagementCommands, ISetCommands, ISortedSetCommands, IStringCommands
{
    /// <summary>
    /// Execute an arbitrary command against the server; this is primarily intended for executing modules,
    /// but may also be used to provide access to new features that lack a direct API.
    /// </summary>
    /// <param name="command">The command to run.</param>
    /// <param name="args">The arguments to pass for the command.</param>
    /// <returns>A dynamic representation of the command's result.</returns>
    /// <remarks>This API should be considered an advanced feature; inappropriate use can be harmful.</remarks>
    Task<ValkeyResult> ExecuteAsync(string command, params object[] args);

    /// <summary>
    /// Execute an arbitrary command against the server; this is primarily intended for executing modules,
    /// but may also be used to provide access to new features that lack a direct API.
    /// </summary>
    /// <param name="command">The command to run.</param>
    /// <param name="args">The arguments to pass for the command.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    /// <returns>A dynamic representation of the command's result.</returns>
    /// <remarks>This API should be considered an advanced feature; inappropriate use can be harmful.</remarks>
    Task<ValkeyResult> ExecuteAsync(string command, ICollection<object>? args, CommandFlags flags = CommandFlags.None);

    #region String Commands with CommandFlags (SER Compatibility)

    /// <inheritdoc cref="IStringCommands.StringSetAsync(ValkeyKey, ValkeyValue)" />
    /// <param name="key">The key to store.</param>
    /// <param name="value">The value to store with the given key.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<bool> StringSetAsync(ValkeyKey key, ValkeyValue value, CommandFlags flags);

    /// <inheritdoc cref="IStringCommands.StringSetAsync(IEnumerable{KeyValuePair{ValkeyKey, ValkeyValue}}, When)" />
    /// <param name="values">A collection of key-value pairs to set.</param>
    /// <param name="when">The condition to specify.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<bool> StringSetAsync(IEnumerable<KeyValuePair<ValkeyKey, ValkeyValue>> values, When when, CommandFlags flags);

    /// <inheritdoc cref="IStringCommands.StringGetAsync(ValkeyKey)" />
    /// <param name="key">The key to retrieve from the database.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<ValkeyValue> StringGetAsync(ValkeyKey key, CommandFlags flags);

    /// <inheritdoc cref="IStringCommands.StringGetAsync(IEnumerable{ValkeyKey})" />
    /// <param name="keys">A list of keys to retrieve values for.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<ValkeyValue[]> StringGetAsync(IEnumerable<ValkeyKey> keys, CommandFlags flags);

    /// <inheritdoc cref="IStringCommands.StringGetRangeAsync(ValkeyKey, long, long)" />
    /// <param name="key">The key of the string.</param>
    /// <param name="start">The starting offset.</param>
    /// <param name="end">The ending offset.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<ValkeyValue> StringGetRangeAsync(ValkeyKey key, long start, long end, CommandFlags flags);

    /// <inheritdoc cref="IStringCommands.StringSetRangeAsync(ValkeyKey, long, ValkeyValue)" />
    /// <param name="key">The key of the string to update.</param>
    /// <param name="offset">The position in the string where value should be written.</param>
    /// <param name="value">The string written with offset.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<ValkeyValue> StringSetRangeAsync(ValkeyKey key, long offset, ValkeyValue value, CommandFlags flags);

    /// <inheritdoc cref="IStringCommands.StringLengthAsync(ValkeyKey)" />
    /// <param name="key">The key to check its length.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<long> StringLengthAsync(ValkeyKey key, CommandFlags flags);

    /// <inheritdoc cref="IStringCommands.StringAppendAsync(ValkeyKey, ValkeyValue)" />
    /// <param name="key">The key of the string to append to.</param>
    /// <param name="value">The value to append to the string.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<long> StringAppendAsync(ValkeyKey key, ValkeyValue value, CommandFlags flags);

    /// <inheritdoc cref="IStringCommands.StringDecrementAsync(ValkeyKey)" />
    /// <param name="key">The key of the string to decrement.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<long> StringDecrementAsync(ValkeyKey key, CommandFlags flags);

    /// <inheritdoc cref="IStringCommands.StringDecrementAsync(ValkeyKey, long)" />
    /// <param name="key">The key of the string to decrement.</param>
    /// <param name="decrement">The amount to decrement by.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<long> StringDecrementAsync(ValkeyKey key, long decrement, CommandFlags flags);

    /// <inheritdoc cref="IStringCommands.StringIncrementAsync(ValkeyKey)" />
    /// <param name="key">The key of the string to increment.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<long> StringIncrementAsync(ValkeyKey key, CommandFlags flags);

    /// <inheritdoc cref="IStringCommands.StringIncrementAsync(ValkeyKey, long)" />
    /// <param name="key">The key of the string to increment.</param>
    /// <param name="increment">The amount to increment by.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<long> StringIncrementAsync(ValkeyKey key, long increment, CommandFlags flags);

    /// <inheritdoc cref="IStringCommands.StringIncrementAsync(ValkeyKey, double)" />
    /// <param name="key">The key of the string to increment.</param>
    /// <param name="increment">The amount to increment by.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<double> StringIncrementAsync(ValkeyKey key, double increment, CommandFlags flags);

    /// <inheritdoc cref="IStringCommands.StringGetDeleteAsync(ValkeyKey)" />
    /// <param name="key">The key to get and delete.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<ValkeyValue> StringGetDeleteAsync(ValkeyKey key, CommandFlags flags);

    /// <inheritdoc cref="IStringCommands.StringGetSetExpiryAsync(ValkeyKey, TimeSpan?)" />
    /// <param name="key">The key to be retrieved from the database.</param>
    /// <param name="expiry">The expiry to set.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<ValkeyValue> StringGetSetExpiryAsync(ValkeyKey key, TimeSpan? expiry, CommandFlags flags);

    /// <inheritdoc cref="IStringCommands.StringGetSetExpiryAsync(ValkeyKey, DateTime)" />
    /// <param name="key">The key to be retrieved from the database.</param>
    /// <param name="expiry">The exact date and time to expire at.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<ValkeyValue> StringGetSetExpiryAsync(ValkeyKey key, DateTime expiry, CommandFlags flags);

    /// <inheritdoc cref="IStringCommands.StringLongestCommonSubsequenceAsync(ValkeyKey, ValkeyKey)" />
    /// <param name="first">The key that stores the first string.</param>
    /// <param name="second">The key that stores the second string.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<string?> StringLongestCommonSubsequenceAsync(ValkeyKey first, ValkeyKey second, CommandFlags flags);

    /// <inheritdoc cref="IStringCommands.StringLongestCommonSubsequenceLengthAsync(ValkeyKey, ValkeyKey)" />
    /// <param name="first">The key that stores the first string.</param>
    /// <param name="second">The key that stores the second string.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<long> StringLongestCommonSubsequenceLengthAsync(ValkeyKey first, ValkeyKey second, CommandFlags flags);

    /// <inheritdoc cref="IStringCommands.StringLongestCommonSubsequenceWithMatchesAsync(ValkeyKey, ValkeyKey, long)" />
    /// <param name="first">The key that stores the first string.</param>
    /// <param name="second">The key that stores the second string.</param>
    /// <param name="minLength">Can be used to restrict the list of matches to the ones of a given minimum length.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<LCSMatchResult> StringLongestCommonSubsequenceWithMatchesAsync(ValkeyKey first, ValkeyKey second, long minLength, CommandFlags flags);

    #endregion

    #region Hash Commands with CommandFlags (SER Compatibility)

    /// <inheritdoc cref="IHashCommands.HashGetAsync(ValkeyKey, ValkeyValue)" />
    /// <param name="key">The key of the hash.</param>
    /// <param name="hashField">The field in the hash to get.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<ValkeyValue> HashGetAsync(ValkeyKey key, ValkeyValue hashField, CommandFlags flags);

    /// <inheritdoc cref="IHashCommands.HashGetAsync(ValkeyKey, IEnumerable{ValkeyValue})" />
    /// <param name="key">The key of the hash.</param>
    /// <param name="hashFields">The fields in the hash to get.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<ValkeyValue[]> HashGetAsync(ValkeyKey key, IEnumerable<ValkeyValue> hashFields, CommandFlags flags);

    /// <inheritdoc cref="IHashCommands.HashGetAllAsync(ValkeyKey)" />
    /// <param name="key">The key of the hash to get all entries from.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<HashEntry[]> HashGetAllAsync(ValkeyKey key, CommandFlags flags);

    /// <inheritdoc cref="IHashCommands.HashSetAsync(ValkeyKey, IEnumerable{HashEntry})" />
    /// <param name="key">The key of the hash.</param>
    /// <param name="hashFields">The entries to set in the hash.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task HashSetAsync(ValkeyKey key, IEnumerable<HashEntry> hashFields, CommandFlags flags);

    /// <inheritdoc cref="IHashCommands.HashSetAsync(ValkeyKey, ValkeyValue, ValkeyValue, When)" />
    /// <param name="key">The key of the hash.</param>
    /// <param name="hashField">The field to set in the hash.</param>
    /// <param name="value">The value to set.</param>
    /// <param name="when">Which conditions under which to set the field value (defaults to always).</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<bool> HashSetAsync(ValkeyKey key, ValkeyValue hashField, ValkeyValue value, When when, CommandFlags flags);

    /// <inheritdoc cref="IHashCommands.HashDeleteAsync(ValkeyKey, ValkeyValue)" />
    /// <param name="key">The key of the hash.</param>
    /// <param name="hashField">The field to remove from the hash.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<bool> HashDeleteAsync(ValkeyKey key, ValkeyValue hashField, CommandFlags flags);

    /// <inheritdoc cref="IHashCommands.HashDeleteAsync(ValkeyKey, IEnumerable{ValkeyValue})" />
    /// <param name="key">The key of the hash.</param>
    /// <param name="hashFields">The fields to remove from the hash.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<long> HashDeleteAsync(ValkeyKey key, IEnumerable<ValkeyValue> hashFields, CommandFlags flags);

    /// <inheritdoc cref="IHashCommands.HashExistsAsync(ValkeyKey, ValkeyValue)" />
    /// <param name="key">The key of the hash.</param>
    /// <param name="hashField">The field to check in the hash.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<bool> HashExistsAsync(ValkeyKey key, ValkeyValue hashField, CommandFlags flags);

    /// <inheritdoc cref="IHashCommands.HashIncrementAsync(ValkeyKey, ValkeyValue, long)" />
    /// <param name="key">The key of the hash.</param>
    /// <param name="hashField">The field in the hash stored at key to increment its value.</param>
    /// <param name="value">The amount to increment.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<long> HashIncrementAsync(ValkeyKey key, ValkeyValue hashField, long value, CommandFlags flags);

    /// <inheritdoc cref="IHashCommands.HashIncrementAsync(ValkeyKey, ValkeyValue, double)" />
    /// <param name="key">The key of the hash.</param>
    /// <param name="hashField">The field in the hash stored at key to increment its value.</param>
    /// <param name="value">The amount to increment.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<double> HashIncrementAsync(ValkeyKey key, ValkeyValue hashField, double value, CommandFlags flags);

    /// <inheritdoc cref="IHashCommands.HashKeysAsync(ValkeyKey)" />
    /// <param name="key">The key of the hash.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<ValkeyValue[]> HashKeysAsync(ValkeyKey key, CommandFlags flags);

    /// <inheritdoc cref="IHashCommands.HashLengthAsync(ValkeyKey)" />
    /// <param name="key">The key of the hash.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<long> HashLengthAsync(ValkeyKey key, CommandFlags flags);

    /// <inheritdoc cref="IHashCommands.HashScanAsync(ValkeyKey, ValkeyValue, int, long, int)" />
    /// <param name="key">The key of the hash.</param>
    /// <param name="pattern">The pattern to match fields against (defaults to all fields).</param>
    /// <param name="pageSize">The number of elements to return in each page (defaults to 250).</param>
    /// <param name="cursor">The cursor that points to the next iteration of results.</param>
    /// <param name="pageOffset">The page offset to start at (defaults to 0).</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    IAsyncEnumerable<HashEntry> HashScanAsync(ValkeyKey key, ValkeyValue pattern, int pageSize, long cursor, int pageOffset, CommandFlags flags);

    /// <inheritdoc cref="IHashCommands.HashScanNoValuesAsync(ValkeyKey, ValkeyValue, int, long, int)" />
    /// <param name="key">The key of the hash.</param>
    /// <param name="pattern">The pattern to match fields against (defaults to all fields).</param>
    /// <param name="pageSize">The number of elements to return in each page (defaults to 250).</param>
    /// <param name="cursor">The cursor that points to the next iteration of results.</param>
    /// <param name="pageOffset">The page offset to start at (defaults to 0).</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    IAsyncEnumerable<ValkeyValue> HashScanNoValuesAsync(ValkeyKey key, ValkeyValue pattern, int pageSize, long cursor, int pageOffset, CommandFlags flags);

    /// <inheritdoc cref="IHashCommands.HashStringLengthAsync(ValkeyKey, ValkeyValue)" />
    /// <param name="key">The key of the hash.</param>
    /// <param name="hashField">The field to get the string length of its value.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<long> HashStringLengthAsync(ValkeyKey key, ValkeyValue hashField, CommandFlags flags);

    /// <inheritdoc cref="IHashCommands.HashValuesAsync(ValkeyKey)" />
    /// <param name="key">The key of the hash.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<ValkeyValue[]> HashValuesAsync(ValkeyKey key, CommandFlags flags);

    /// <inheritdoc cref="IHashCommands.HashRandomFieldAsync(ValkeyKey)" />
    /// <param name="key">The key of the hash.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<ValkeyValue> HashRandomFieldAsync(ValkeyKey key, CommandFlags flags);

    /// <inheritdoc cref="IHashCommands.HashRandomFieldsAsync(ValkeyKey, long)" />
    /// <param name="key">The key of the hash.</param>
    /// <param name="count">The number of fields to return.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<ValkeyValue[]> HashRandomFieldsAsync(ValkeyKey key, long count, CommandFlags flags);

    /// <inheritdoc cref="IHashCommands.HashRandomFieldsWithValuesAsync(ValkeyKey, long)" />
    /// <param name="key">The key of the hash.</param>
    /// <param name="count">The number of fields to return.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<HashEntry[]> HashRandomFieldsWithValuesAsync(ValkeyKey key, long count, CommandFlags flags);

    /// <inheritdoc cref="IHashCommands.HashGetExAsync(ValkeyKey, IEnumerable{ValkeyValue}, HashGetExOptions)" />
    /// <param name="key">The key of the hash.</param>
    /// <param name="fields">The fields in the hash stored at key to retrieve from the database.</param>
    /// <param name="options">Optional parameters for the command including expiry settings or persist option.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<ValkeyValue[]?> HashGetExAsync(ValkeyKey key, IEnumerable<ValkeyValue> fields, HashGetExOptions options, CommandFlags flags);

    /// <inheritdoc cref="IHashCommands.HashSetExAsync(ValkeyKey, IDictionary{ValkeyValue, ValkeyValue}, HashSetExOptions)" />
    /// <param name="key">The key of the hash.</param>
    /// <param name="fieldValueMap">A field-value map consisting of fields and their corresponding values to be set in the hash stored at the specified key.</param>
    /// <param name="options">Optional parameters for the command including conditional changes and expiry settings.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<long> HashSetExAsync(ValkeyKey key, IDictionary<ValkeyValue, ValkeyValue> fieldValueMap, HashSetExOptions options, CommandFlags flags);

    /// <inheritdoc cref="IHashCommands.HashPersistAsync(ValkeyKey, IEnumerable{ValkeyValue})" />
    /// <param name="key">The key of the hash.</param>
    /// <param name="fields">The fields to remove expiration from.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<long[]> HashPersistAsync(ValkeyKey key, IEnumerable<ValkeyValue> fields, CommandFlags flags);

    /// <inheritdoc cref="IHashCommands.HashExpireAsync(ValkeyKey, long, IEnumerable{ValkeyValue}, HashFieldExpirationConditionOptions)" />
    /// <param name="key">The key of the hash.</param>
    /// <param name="seconds">The expiration time in seconds.</param>
    /// <param name="fields">The fields in the hash stored at key to set expiration for.</param>
    /// <param name="options">The expiration condition options.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<long[]> HashExpireAsync(ValkeyKey key, long seconds, IEnumerable<ValkeyValue> fields, HashFieldExpirationConditionOptions options, CommandFlags flags);

    /// <inheritdoc cref="IHashCommands.HashPExpireAsync(ValkeyKey, long, IEnumerable{ValkeyValue}, HashFieldExpirationConditionOptions)" />
    /// <param name="key">The key of the hash.</param>
    /// <param name="milliseconds">The expiration time to set for the fields, in milliseconds.</param>
    /// <param name="fields">The fields to set expiration for.</param>
    /// <param name="options">The expiration options.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<long[]> HashPExpireAsync(ValkeyKey key, long milliseconds, IEnumerable<ValkeyValue> fields, HashFieldExpirationConditionOptions options, CommandFlags flags);

    /// <inheritdoc cref="IHashCommands.HashExpireAtAsync(ValkeyKey, long, IEnumerable{ValkeyValue}, HashFieldExpirationConditionOptions)" />
    /// <param name="key">The key of the hash.</param>
    /// <param name="unixSeconds">The expiration time to set for the fields, as a Unix timestamp in seconds.</param>
    /// <param name="fields">The fields to set expiration for.</param>
    /// <param name="options">The expiration options.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<long[]> HashExpireAtAsync(ValkeyKey key, long unixSeconds, IEnumerable<ValkeyValue> fields, HashFieldExpirationConditionOptions options, CommandFlags flags);

    /// <inheritdoc cref="IHashCommands.HashPExpireAtAsync(ValkeyKey, long, IEnumerable{ValkeyValue}, HashFieldExpirationConditionOptions)" />
    /// <param name="key">The key of the hash.</param>
    /// <param name="unixMilliseconds">The expiration time to set for the fields, as a Unix timestamp in milliseconds.</param>
    /// <param name="fields">A collection of hash field names for which to set the expiration.</param>
    /// <param name="options">Optional conditions and configurations for the expiration.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<long[]> HashPExpireAtAsync(ValkeyKey key, long unixMilliseconds, IEnumerable<ValkeyValue> fields, HashFieldExpirationConditionOptions options, CommandFlags flags);

    /// <inheritdoc cref="IHashCommands.HashExpireTimeAsync(ValkeyKey, IEnumerable{ValkeyValue})" />
    /// <param name="key">The key of the hash.</param>
    /// <param name="fields">The fields to get the expiration timestamp for.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<long[]> HashExpireTimeAsync(ValkeyKey key, IEnumerable<ValkeyValue> fields, CommandFlags flags);

    /// <inheritdoc cref="IHashCommands.HashPExpireTimeAsync(ValkeyKey, IEnumerable{ValkeyValue})" />
    /// <param name="key">The key of the hash.</param>
    /// <param name="fields">The fields to get the expiration timestamp for.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<long[]> HashPExpireTimeAsync(ValkeyKey key, IEnumerable<ValkeyValue> fields, CommandFlags flags);

    /// <inheritdoc cref="IHashCommands.HashTtlAsync(ValkeyKey, IEnumerable{ValkeyValue})" />
    /// <param name="key">The key of the hash.</param>
    /// <param name="fields">The fields to get the TTL for.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<long[]> HashTtlAsync(ValkeyKey key, IEnumerable<ValkeyValue> fields, CommandFlags flags);

    /// <inheritdoc cref="IHashCommands.HashPTtlAsync(ValkeyKey, IEnumerable{ValkeyValue})" />
    /// <param name="key">The key of the hash.</param>
    /// <param name="fields">The fields to get the TTL for.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<long[]> HashPTtlAsync(ValkeyKey key, IEnumerable<ValkeyValue> fields, CommandFlags flags);

    #endregion
}
