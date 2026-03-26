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

    #region List Commands with CommandFlags (SER Compatibility)

    /// <inheritdoc cref="IListCommands.ListLeftPopAsync(ValkeyKey)" />
    /// <param name="key">The key of the list.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<ValkeyValue> ListLeftPopAsync(ValkeyKey key, CommandFlags flags);

    /// <inheritdoc cref="IListCommands.ListLeftPopAsync(ValkeyKey, long)" />
    /// <param name="key">The key of the list.</param>
    /// <param name="count">The count of the elements to pop from the list.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<ValkeyValue[]?> ListLeftPopAsync(ValkeyKey key, long count, CommandFlags flags);

    /// <inheritdoc cref="IListCommands.ListLeftPopAsync(IEnumerable{ValkeyKey}, long)" />
    /// <param name="keys">A collection of keys to lists.</param>
    /// <param name="count">The maximum number of elements to pop.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<ListPopResult> ListLeftPopAsync(IEnumerable<ValkeyKey> keys, long count, CommandFlags flags);

    /// <inheritdoc cref="IListCommands.ListLeftPushAsync(ValkeyKey, ValkeyValue, When)" />
    /// <param name="key">The key of the list.</param>
    /// <param name="value">The value to add to the head of the list.</param>
    /// <param name="when">Use When.Exists for LPUSHX behavior (only push if key exists).</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<long> ListLeftPushAsync(ValkeyKey key, ValkeyValue value, When when, CommandFlags flags);

    /// <inheritdoc cref="IListCommands.ListLeftPushAsync(ValkeyKey, IEnumerable{ValkeyValue}, When)" />
    /// <param name="key">The key of the list.</param>
    /// <param name="values">The elements to insert at the head of the list stored at key.</param>
    /// <param name="when">Use When.Exists for LPUSHX behavior (only push if key exists).</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<long> ListLeftPushAsync(ValkeyKey key, IEnumerable<ValkeyValue> values, When when, CommandFlags flags);

    /// <inheritdoc cref="IListCommands.ListLeftPushAsync(ValkeyKey, IEnumerable{ValkeyValue}, When)" />
    /// <param name="key">The key of the list.</param>
    /// <param name="values">The elements to insert at the head of the list stored at key.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<long> ListLeftPushAsync(ValkeyKey key, IEnumerable<ValkeyValue> values, CommandFlags flags);

    /// <inheritdoc cref="IListCommands.ListRightPopAsync(ValkeyKey)" />
    /// <param name="key">The key of the list.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<ValkeyValue> ListRightPopAsync(ValkeyKey key, CommandFlags flags);

    /// <inheritdoc cref="IListCommands.ListRightPopAsync(ValkeyKey, long)" />
    /// <param name="key">The key of the list.</param>
    /// <param name="count">The count of the elements to pop from the list.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<ValkeyValue[]?> ListRightPopAsync(ValkeyKey key, long count, CommandFlags flags);

    /// <inheritdoc cref="IListCommands.ListRightPopAsync(IEnumerable{ValkeyKey}, long)" />
    /// <param name="keys">A collection of keys to lists.</param>
    /// <param name="count">The maximum number of elements to pop.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<ListPopResult> ListRightPopAsync(IEnumerable<ValkeyKey> keys, long count, CommandFlags flags);

    /// <inheritdoc cref="IListCommands.ListRightPushAsync(ValkeyKey, ValkeyValue, When)" />
    /// <param name="key">The key of the list.</param>
    /// <param name="value">The value to add to the tail of the list.</param>
    /// <param name="when">Use When.Exists for RPUSHX behavior (only push if key exists).</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<long> ListRightPushAsync(ValkeyKey key, ValkeyValue value, When when, CommandFlags flags);

    /// <inheritdoc cref="IListCommands.ListRightPushAsync(ValkeyKey, IEnumerable{ValkeyValue}, When)" />
    /// <param name="key">The key of the list.</param>
    /// <param name="values">The elements to insert at the tail of the list stored at key.</param>
    /// <param name="when">Use When.Exists for RPUSHX behavior (only push if key exists).</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<long> ListRightPushAsync(ValkeyKey key, IEnumerable<ValkeyValue> values, When when, CommandFlags flags);

    /// <inheritdoc cref="IListCommands.ListRightPushAsync(ValkeyKey, IEnumerable{ValkeyValue}, When)" />
    /// <param name="key">The key of the list.</param>
    /// <param name="values">The elements to insert at the tail of the list stored at key.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<long> ListRightPushAsync(ValkeyKey key, IEnumerable<ValkeyValue> values, CommandFlags flags);

    /// <inheritdoc cref="IListCommands.ListLengthAsync(ValkeyKey)" />
    /// <param name="key">The key of the list.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<long> ListLengthAsync(ValkeyKey key, CommandFlags flags);

    /// <inheritdoc cref="IListCommands.ListRemoveAsync(ValkeyKey, ValkeyValue, long)" />
    /// <param name="key">The key of the list.</param>
    /// <param name="value">The value to remove from the list.</param>
    /// <param name="count">The count of the occurrences of elements equal to value to remove.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<long> ListRemoveAsync(ValkeyKey key, ValkeyValue value, long count, CommandFlags flags);

    /// <inheritdoc cref="IListCommands.ListTrimAsync(ValkeyKey, long, long)" />
    /// <param name="key">The key of the list.</param>
    /// <param name="start">The starting point of the range.</param>
    /// <param name="stop">The end of the range.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task ListTrimAsync(ValkeyKey key, long start, long stop, CommandFlags flags);

    /// <inheritdoc cref="IListCommands.ListRangeAsync(ValkeyKey, long, long)" />
    /// <param name="key">The key of the list.</param>
    /// <param name="start">The starting point of the range.</param>
    /// <param name="stop">The end of the range.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<ValkeyValue[]> ListRangeAsync(ValkeyKey key, long start, long stop, CommandFlags flags);

    /// <inheritdoc cref="IListCommands.ListGetByIndexAsync(ValkeyKey, long)" />
    /// <param name="key">The key of the list.</param>
    /// <param name="index">The index of the element in the list to retrieve.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<ValkeyValue> ListGetByIndexAsync(ValkeyKey key, long index, CommandFlags flags);

    /// <inheritdoc cref="IListCommands.ListInsertAfterAsync(ValkeyKey, ValkeyValue, ValkeyValue)" />
    /// <param name="key">The key of the list.</param>
    /// <param name="pivot">The reference point in the list.</param>
    /// <param name="value">The new element to insert.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<long> ListInsertAfterAsync(ValkeyKey key, ValkeyValue pivot, ValkeyValue value, CommandFlags flags);

    /// <inheritdoc cref="IListCommands.ListInsertBeforeAsync(ValkeyKey, ValkeyValue, ValkeyValue)" />
    /// <param name="key">The key of the list.</param>
    /// <param name="pivot">The reference point in the list.</param>
    /// <param name="value">The new element to insert.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<long> ListInsertBeforeAsync(ValkeyKey key, ValkeyValue pivot, ValkeyValue value, CommandFlags flags);

    /// <inheritdoc cref="IListCommands.ListMoveAsync(ValkeyKey, ValkeyKey, ListSide, ListSide)" />
    /// <param name="sourceKey">The key of the source list.</param>
    /// <param name="destinationKey">The key of the destination list.</param>
    /// <param name="sourceSide">The side of the source list to pop from (Left = head, Right = tail).</param>
    /// <param name="destinationSide">The side of the destination list to push to (Left = head, Right = tail).</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<ValkeyValue> ListMoveAsync(ValkeyKey sourceKey, ValkeyKey destinationKey, ListSide sourceSide, ListSide destinationSide, CommandFlags flags);

    /// <inheritdoc cref="IListCommands.ListPositionAsync(ValkeyKey, ValkeyValue, long, long)" />
    /// <param name="key">The key of the list.</param>
    /// <param name="element">The element to search for.</param>
    /// <param name="rank">The rank of the match to return (1-based). Negative values indicate searching from the end.</param>
    /// <param name="maxLength">Limit the search to this many elements. 0 means no limit.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<long> ListPositionAsync(ValkeyKey key, ValkeyValue element, long rank, long maxLength, CommandFlags flags);

    /// <inheritdoc cref="IListCommands.ListPositionsAsync(ValkeyKey, ValkeyValue, long, long, long)" />
    /// <param name="key">The key of the list.</param>
    /// <param name="element">The element to search for.</param>
    /// <param name="count">The maximum number of matches to return.</param>
    /// <param name="rank">The rank of the first match to return (1-based). Negative values indicate searching from the end.</param>
    /// <param name="maxLength">Limit the search to this many elements. 0 means no limit.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<long[]> ListPositionsAsync(ValkeyKey key, ValkeyValue element, long count, long rank, long maxLength, CommandFlags flags);

    /// <inheritdoc cref="IListCommands.ListSetByIndexAsync(ValkeyKey, long, ValkeyValue)" />
    /// <param name="key">The key of the list.</param>
    /// <param name="index">The index of the element in the list to set.</param>
    /// <param name="value">The new value.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task ListSetByIndexAsync(ValkeyKey key, long index, ValkeyValue value, CommandFlags flags);

    /// <inheritdoc cref="IListCommands.ListBlockingLeftPopAsync(IEnumerable{ValkeyKey}, TimeSpan)" />
    /// <param name="keys">The keys of the lists to pop from.</param>
    /// <param name="timeout">The maximum time to wait for a blocking operation to complete.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<ValkeyValue[]?> ListBlockingLeftPopAsync(IEnumerable<ValkeyKey> keys, TimeSpan timeout, CommandFlags flags);

    /// <inheritdoc cref="IListCommands.ListBlockingRightPopAsync(IEnumerable{ValkeyKey}, TimeSpan)" />
    /// <param name="keys">The keys of the lists to pop from.</param>
    /// <param name="timeout">The maximum time to wait for a blocking operation to complete.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<ValkeyValue[]?> ListBlockingRightPopAsync(IEnumerable<ValkeyKey> keys, TimeSpan timeout, CommandFlags flags);

    /// <inheritdoc cref="IListCommands.ListBlockingMoveAsync(ValkeyKey, ValkeyKey, ListSide, ListSide, TimeSpan)" />
    /// <param name="source">The key of the source list.</param>
    /// <param name="destination">The key of the destination list.</param>
    /// <param name="sourceSide">The side of the source list to pop from (Left = head, Right = tail).</param>
    /// <param name="destinationSide">The side of the destination list to push to (Left = head, Right = tail).</param>
    /// <param name="timeout">The maximum time to wait for a blocking operation to complete.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<ValkeyValue> ListBlockingMoveAsync(ValkeyKey source, ValkeyKey destination, ListSide sourceSide, ListSide destinationSide, TimeSpan timeout, CommandFlags flags);

    /// <inheritdoc cref="IListCommands.ListBlockingPopAsync(IEnumerable{ValkeyKey}, ListSide, TimeSpan)" />
    /// <param name="keys">A collection of keys to lists.</param>
    /// <param name="side">The side of the list to pop from (Left = head, Right = tail).</param>
    /// <param name="timeout">The maximum time to wait for a blocking operation to complete.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<ListPopResult> ListBlockingPopAsync(IEnumerable<ValkeyKey> keys, ListSide side, TimeSpan timeout, CommandFlags flags);

    /// <inheritdoc cref="IListCommands.ListBlockingPopAsync(IEnumerable{ValkeyKey}, ListSide, long, TimeSpan)" />
    /// <param name="keys">A collection of keys to lists.</param>
    /// <param name="side">The side of the list to pop from (Left = head, Right = tail).</param>
    /// <param name="count">The maximum number of elements to pop.</param>
    /// <param name="timeout">The maximum time to wait for a blocking operation to complete.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<ListPopResult> ListBlockingPopAsync(IEnumerable<ValkeyKey> keys, ListSide side, long count, TimeSpan timeout, CommandFlags flags);

    #endregion

    #region Set Commands with CommandFlags (SER Compatibility)

    /// <inheritdoc cref="ISetCommands.SetAddAsync(ValkeyKey, ValkeyValue)" />
    /// <param name="key">The key where members will be added to its set.</param>
    /// <param name="value">The value to add to the set.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<bool> SetAddAsync(ValkeyKey key, ValkeyValue value, CommandFlags flags);

    /// <inheritdoc cref="ISetCommands.SetAddAsync(ValkeyKey, IEnumerable{ValkeyValue})" />
    /// <param name="key">The key where members will be added to its set.</param>
    /// <param name="values">The values to add to the set.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<long> SetAddAsync(ValkeyKey key, IEnumerable<ValkeyValue> values, CommandFlags flags);

    /// <inheritdoc cref="ISetCommands.SetRemoveAsync(ValkeyKey, ValkeyValue)" />
    /// <param name="key">The key from which members will be removed.</param>
    /// <param name="value">The value to remove.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<bool> SetRemoveAsync(ValkeyKey key, ValkeyValue value, CommandFlags flags);

    /// <inheritdoc cref="ISetCommands.SetRemoveAsync(ValkeyKey, IEnumerable{ValkeyValue})" />
    /// <param name="key">The key from which members will be removed.</param>
    /// <param name="values">The values to remove.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<long> SetRemoveAsync(ValkeyKey key, IEnumerable<ValkeyValue> values, CommandFlags flags);

    /// <inheritdoc cref="ISetCommands.SetMembersAsync(ValkeyKey)" />
    /// <param name="key">The key from which to retrieve the set members.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<ValkeyValue[]> SetMembersAsync(ValkeyKey key, CommandFlags flags);

    /// <inheritdoc cref="ISetCommands.SetLengthAsync(ValkeyKey)" />
    /// <param name="key">The key from which to retrieve the number of set members.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<long> SetLengthAsync(ValkeyKey key, CommandFlags flags);

    /// <inheritdoc cref="ISetCommands.SetIntersectionLengthAsync(IEnumerable{ValkeyKey}, long)" />
    /// <param name="keys">The keys of the sets to intersect.</param>
    /// <param name="limit">The limit for the intersection cardinality value.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<long> SetIntersectionLengthAsync(IEnumerable<ValkeyKey> keys, long limit, CommandFlags flags);

    /// <inheritdoc cref="ISetCommands.SetPopAsync(ValkeyKey)" />
    /// <param name="key">The key of the set.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<ValkeyValue> SetPopAsync(ValkeyKey key, CommandFlags flags);

    /// <inheritdoc cref="ISetCommands.SetPopAsync(ValkeyKey, long)" />
    /// <param name="key">The key of the set.</param>
    /// <param name="count">The number of members to return.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<ValkeyValue[]> SetPopAsync(ValkeyKey key, long count, CommandFlags flags);

    /// <inheritdoc cref="ISetCommands.SetUnionAsync(ValkeyKey, ValkeyKey)" />
    /// <param name="first">The key of the first set.</param>
    /// <param name="second">The key of the second set.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<ValkeyValue[]> SetUnionAsync(ValkeyKey first, ValkeyKey second, CommandFlags flags);

    /// <inheritdoc cref="ISetCommands.SetUnionAsync(IEnumerable{ValkeyKey})" />
    /// <param name="keys">The keys of the sets to operate on.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<ValkeyValue[]> SetUnionAsync(IEnumerable<ValkeyKey> keys, CommandFlags flags);

    /// <inheritdoc cref="ISetCommands.SetIntersectAsync(ValkeyKey, ValkeyKey)" />
    /// <param name="first">The key of the first set.</param>
    /// <param name="second">The key of the second set.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<ValkeyValue[]> SetIntersectAsync(ValkeyKey first, ValkeyKey second, CommandFlags flags);

    /// <inheritdoc cref="ISetCommands.SetIntersectAsync(IEnumerable{ValkeyKey})" />
    /// <param name="keys">The keys of the sets to operate on.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<ValkeyValue[]> SetIntersectAsync(IEnumerable<ValkeyKey> keys, CommandFlags flags);

    /// <inheritdoc cref="ISetCommands.SetDifferenceAsync(ValkeyKey, ValkeyKey)" />
    /// <param name="first">The key of the first set.</param>
    /// <param name="second">The key of the second set.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<ValkeyValue[]> SetDifferenceAsync(ValkeyKey first, ValkeyKey second, CommandFlags flags);

    /// <inheritdoc cref="ISetCommands.SetDifferenceAsync(IEnumerable{ValkeyKey})" />
    /// <param name="keys">The keys of the sets to operate on.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<ValkeyValue[]> SetDifferenceAsync(IEnumerable<ValkeyKey> keys, CommandFlags flags);

    /// <inheritdoc cref="ISetCommands.SetUnionStoreAsync(ValkeyKey, ValkeyKey, ValkeyKey)" />
    /// <param name="destination">The key of the destination set.</param>
    /// <param name="first">The key of the first set.</param>
    /// <param name="second">The key of the second set.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<long> SetUnionStoreAsync(ValkeyKey destination, ValkeyKey first, ValkeyKey second, CommandFlags flags);

    /// <inheritdoc cref="ISetCommands.SetUnionStoreAsync(ValkeyKey, IEnumerable{ValkeyKey})" />
    /// <param name="destination">The key of the destination set.</param>
    /// <param name="keys">The keys of the sets to operate on.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<long> SetUnionStoreAsync(ValkeyKey destination, IEnumerable<ValkeyKey> keys, CommandFlags flags);

    /// <inheritdoc cref="ISetCommands.SetIntersectStoreAsync(ValkeyKey, ValkeyKey, ValkeyKey)" />
    /// <param name="destination">The key of the destination set.</param>
    /// <param name="first">The key of the first set.</param>
    /// <param name="second">The key of the second set.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<long> SetIntersectStoreAsync(ValkeyKey destination, ValkeyKey first, ValkeyKey second, CommandFlags flags);

    /// <inheritdoc cref="ISetCommands.SetIntersectStoreAsync(ValkeyKey, IEnumerable{ValkeyKey})" />
    /// <param name="destination">The key of the destination set.</param>
    /// <param name="keys">The keys of the sets to operate on.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<long> SetIntersectStoreAsync(ValkeyKey destination, IEnumerable<ValkeyKey> keys, CommandFlags flags);

    /// <inheritdoc cref="ISetCommands.SetDifferenceStoreAsync(ValkeyKey, ValkeyKey, ValkeyKey)" />
    /// <param name="destination">The key of the destination set.</param>
    /// <param name="first">The key of the first set.</param>
    /// <param name="second">The key of the second set.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<long> SetDifferenceStoreAsync(ValkeyKey destination, ValkeyKey first, ValkeyKey second, CommandFlags flags);

    /// <inheritdoc cref="ISetCommands.SetDifferenceStoreAsync(ValkeyKey, IEnumerable{ValkeyKey})" />
    /// <param name="destination">The key of the destination set.</param>
    /// <param name="keys">The keys of the sets to operate on.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<long> SetDifferenceStoreAsync(ValkeyKey destination, IEnumerable<ValkeyKey> keys, CommandFlags flags);

    /// <inheritdoc cref="ISetCommands.SetContainsAsync(ValkeyKey, ValkeyValue)" />
    /// <param name="key">The key of the set.</param>
    /// <param name="value">The member to check for existence in the set.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<bool> SetContainsAsync(ValkeyKey key, ValkeyValue value, CommandFlags flags);

    /// <inheritdoc cref="ISetCommands.SetContainsAsync(ValkeyKey, IEnumerable{ValkeyValue})" />
    /// <param name="key">The key of the set.</param>
    /// <param name="values">The members to check.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<bool[]> SetContainsAsync(ValkeyKey key, IEnumerable<ValkeyValue> values, CommandFlags flags);

    /// <inheritdoc cref="ISetCommands.SetRandomMemberAsync(ValkeyKey)" />
    /// <param name="key">The key from which to retrieve the set member.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<ValkeyValue> SetRandomMemberAsync(ValkeyKey key, CommandFlags flags);

    /// <inheritdoc cref="ISetCommands.SetRandomMembersAsync(ValkeyKey, long)" />
    /// <param name="key">The key from which to retrieve the set members.</param>
    /// <param name="count">The number of members to return.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<ValkeyValue[]> SetRandomMembersAsync(ValkeyKey key, long count, CommandFlags flags);

    /// <inheritdoc cref="ISetCommands.SetMoveAsync(ValkeyKey, ValkeyKey, ValkeyValue)" />
    /// <param name="source">The key of the set to remove the element from.</param>
    /// <param name="destination">The key of the set to add the element to.</param>
    /// <param name="value">The set element to move.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<bool> SetMoveAsync(ValkeyKey source, ValkeyKey destination, ValkeyValue value, CommandFlags flags);

    /// <inheritdoc cref="ISetCommands.SetScanAsync(ValkeyKey, ValkeyValue, int, long, int)" />
    /// <param name="key">The key of the set.</param>
    /// <param name="pattern">The pattern to match.</param>
    /// <param name="pageSize">The page size to iterate by.</param>
    /// <param name="cursor">The cursor position to start at.</param>
    /// <param name="pageOffset">The page offset to start at.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    IAsyncEnumerable<ValkeyValue> SetScanAsync(ValkeyKey key, ValkeyValue pattern, int pageSize, long cursor, int pageOffset, CommandFlags flags);

    #endregion

    #region Sorted Set Commands with CommandFlags (SER Compatibility)

    /// <inheritdoc cref="ISortedSetCommands.SortedSetAddAsync(ValkeyKey, ValkeyValue, double)" />
    /// <param name="key">The key of the sorted set.</param>
    /// <param name="member">The member to add to the sorted set.</param>
    /// <param name="score">The score of the member.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<bool> SortedSetAddAsync(ValkeyKey key, ValkeyValue member, double score, CommandFlags flags);

    /// <inheritdoc cref="ISortedSetCommands.SortedSetAddAsync(ValkeyKey, ValkeyValue, double, When)" />
    /// <param name="key">The key of the sorted set.</param>
    /// <param name="member">The member to add to the sorted set.</param>
    /// <param name="score">The score of the member.</param>
    /// <param name="when">Indicates when this operation should be performed.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<bool> SortedSetAddAsync(ValkeyKey key, ValkeyValue member, double score, When when, CommandFlags flags);

    /// <inheritdoc cref="ISortedSetCommands.SortedSetAddAsync(ValkeyKey, ValkeyValue, double, SortedSetWhen)" />
    /// <param name="key">The key of the sorted set.</param>
    /// <param name="member">The member to add to the sorted set.</param>
    /// <param name="score">The score of the member.</param>
    /// <param name="when">Indicates when this operation should be performed.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<bool> SortedSetAddAsync(ValkeyKey key, ValkeyValue member, double score, SortedSetWhen when, CommandFlags flags);

    /// <inheritdoc cref="ISortedSetCommands.SortedSetAddAsync(ValkeyKey, IEnumerable{SortedSetEntry})" />
    /// <param name="key">The key of the sorted set.</param>
    /// <param name="values">A collection of members and their scores to add.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<long> SortedSetAddAsync(ValkeyKey key, IEnumerable<SortedSetEntry> values, CommandFlags flags);

    /// <inheritdoc cref="ISortedSetCommands.SortedSetAddAsync(ValkeyKey, IEnumerable{SortedSetEntry}, When)" />
    /// <param name="key">The key of the sorted set.</param>
    /// <param name="values">A collection of members and their scores to add.</param>
    /// <param name="when">Indicates when this operation should be performed.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<long> SortedSetAddAsync(ValkeyKey key, IEnumerable<SortedSetEntry> values, When when, CommandFlags flags);

    /// <inheritdoc cref="ISortedSetCommands.SortedSetAddAsync(ValkeyKey, IEnumerable{SortedSetEntry}, SortedSetWhen)" />
    /// <param name="key">The key of the sorted set.</param>
    /// <param name="values">A collection of members and their scores to add.</param>
    /// <param name="when">Indicates when this operation should be performed.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<long> SortedSetAddAsync(ValkeyKey key, IEnumerable<SortedSetEntry> values, SortedSetWhen when, CommandFlags flags);

    /// <inheritdoc cref="ISortedSetCommands.SortedSetRemoveAsync(ValkeyKey, ValkeyValue)" />
    /// <param name="key">The key of the sorted set.</param>
    /// <param name="member">The member to remove from the sorted set.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<bool> SortedSetRemoveAsync(ValkeyKey key, ValkeyValue member, CommandFlags flags);

    /// <inheritdoc cref="ISortedSetCommands.SortedSetRemoveAsync(ValkeyKey, IEnumerable{ValkeyValue})" />
    /// <param name="key">The key of the sorted set.</param>
    /// <param name="members">A collection of members to remove from the sorted set.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<long> SortedSetRemoveAsync(ValkeyKey key, IEnumerable<ValkeyValue> members, CommandFlags flags);

    /// <inheritdoc cref="ISortedSetCommands.SortedSetLengthAsync(ValkeyKey, double, double, Exclude)" />
    /// <param name="key">The key of the sorted set.</param>
    /// <param name="min">The min score to filter by.</param>
    /// <param name="max">The max score to filter by.</param>
    /// <param name="exclude">Whether to exclude min and max from the range check.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<long> SortedSetLengthAsync(ValkeyKey key, double min, double max, Exclude exclude, CommandFlags flags);

    /// <inheritdoc cref="ISortedSetCommands.SortedSetCardAsync(ValkeyKey)" />
    /// <param name="key">The key of the sorted set.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<long> SortedSetCardAsync(ValkeyKey key, CommandFlags flags);

    /// <inheritdoc cref="ISortedSetCommands.SortedSetCountAsync(ValkeyKey, double, double, Exclude)" />
    /// <param name="key">The key of the sorted set.</param>
    /// <param name="min">The minimum score to count from.</param>
    /// <param name="max">The maximum score to count up to.</param>
    /// <param name="exclude">Whether to exclude min and max from the range check.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<long> SortedSetCountAsync(ValkeyKey key, double min, double max, Exclude exclude, CommandFlags flags);

    /// <inheritdoc cref="ISortedSetCommands.SortedSetRangeByRankAsync(ValkeyKey, long, long, Order)" />
    /// <param name="key">The key of the sorted set.</param>
    /// <param name="start">The start index to get.</param>
    /// <param name="stop">The stop index to get.</param>
    /// <param name="order">The order to sort by.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<ValkeyValue[]> SortedSetRangeByRankAsync(ValkeyKey key, long start, long stop, Order order, CommandFlags flags);

    /// <inheritdoc cref="ISortedSetCommands.SortedSetRangeByRankWithScoresAsync(ValkeyKey, long, long, Order)" />
    /// <param name="key">The key of the sorted set.</param>
    /// <param name="start">The start index to get.</param>
    /// <param name="stop">The stop index to get.</param>
    /// <param name="order">The order to sort by.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<SortedSetEntry[]> SortedSetRangeByRankWithScoresAsync(ValkeyKey key, long start, long stop, Order order, CommandFlags flags);

    /// <inheritdoc cref="ISortedSetCommands.SortedSetRangeByScoreAsync(ValkeyKey, double, double, Exclude, Order, long, long)" />
    /// <param name="key">The key of the sorted set.</param>
    /// <param name="start">The minimum score to filter by.</param>
    /// <param name="stop">The maximum score to filter by.</param>
    /// <param name="exclude">Which of start and stop to exclude.</param>
    /// <param name="order">The order to sort by.</param>
    /// <param name="skip">How many items to skip.</param>
    /// <param name="take">How many items to take.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<ValkeyValue[]> SortedSetRangeByScoreAsync(ValkeyKey key, double start, double stop, Exclude exclude, Order order, long skip, long take, CommandFlags flags);

    /// <inheritdoc cref="ISortedSetCommands.SortedSetRangeByScoreWithScoresAsync(ValkeyKey, double, double, Exclude, Order, long, long)" />
    /// <param name="key">The key of the sorted set.</param>
    /// <param name="start">The minimum score to filter by.</param>
    /// <param name="stop">The maximum score to filter by.</param>
    /// <param name="exclude">Which of start and stop to exclude.</param>
    /// <param name="order">The order to sort by.</param>
    /// <param name="skip">How many items to skip.</param>
    /// <param name="take">How many items to take.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<SortedSetEntry[]> SortedSetRangeByScoreWithScoresAsync(ValkeyKey key, double start, double stop, Exclude exclude, Order order, long skip, long take, CommandFlags flags);

    /// <inheritdoc cref="ISortedSetCommands.SortedSetRangeByValueAsync(ValkeyKey, ValkeyValue, ValkeyValue, Exclude, long, long)" />
    /// <param name="key">The key of the sorted set.</param>
    /// <param name="min">The min value to filter by.</param>
    /// <param name="max">The max value to filter by.</param>
    /// <param name="exclude">Which of min and max to exclude.</param>
    /// <param name="skip">How many items to skip.</param>
    /// <param name="take">How many items to take.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<ValkeyValue[]> SortedSetRangeByValueAsync(ValkeyKey key, ValkeyValue min, ValkeyValue max, Exclude exclude, long skip, long take, CommandFlags flags);

    /// <inheritdoc cref="ISortedSetCommands.SortedSetRangeByValueAsync(ValkeyKey, ValkeyValue, ValkeyValue, Exclude, Order, long, long)" />
    /// <param name="key">The key of the sorted set.</param>
    /// <param name="min">The min value to filter by.</param>
    /// <param name="max">The max value to filter by.</param>
    /// <param name="exclude">Which of min and max to exclude.</param>
    /// <param name="order">The order to sort by.</param>
    /// <param name="skip">How many items to skip.</param>
    /// <param name="take">How many items to take.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<ValkeyValue[]> SortedSetRangeByValueAsync(ValkeyKey key, ValkeyValue min, ValkeyValue max, Exclude exclude, Order order, long skip, long take, CommandFlags flags);

    /// <inheritdoc cref="ISortedSetCommands.SortedSetBlockingPopAsync(ValkeyKey, Order, double)" />
    /// <param name="key">The key of the sorted set.</param>
    /// <param name="order">The order to sort by when popping items out of the set.</param>
    /// <param name="timeout">The timeout in seconds.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<SortedSetEntry?> SortedSetBlockingPopAsync(ValkeyKey key, Order order, double timeout, CommandFlags flags);

    /// <inheritdoc cref="ISortedSetCommands.SortedSetBlockingPopAsync(ValkeyKey, long, Order, double)" />
    /// <param name="key">The key of the sorted set.</param>
    /// <param name="count">The number of elements to return.</param>
    /// <param name="order">The order to sort by when popping items out of the set.</param>
    /// <param name="timeout">The timeout in seconds.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<SortedSetEntry[]> SortedSetBlockingPopAsync(ValkeyKey key, long count, Order order, double timeout, CommandFlags flags);

    /// <inheritdoc cref="ISortedSetCommands.SortedSetBlockingPopAsync(IEnumerable{ValkeyKey}, long, Order, double)" />
    /// <param name="keys">The keys of the sorted sets.</param>
    /// <param name="count">The maximum number of records to pop out of the sorted set.</param>
    /// <param name="order">The order to sort by when popping items out of the set.</param>
    /// <param name="timeout">The timeout in seconds.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<SortedSetPopResult> SortedSetBlockingPopAsync(IEnumerable<ValkeyKey> keys, long count, Order order, double timeout, CommandFlags flags);

    /// <inheritdoc cref="ISortedSetCommands.SortedSetCombineAsync(SetOperation, IEnumerable{ValkeyKey}, IEnumerable{double}?, Aggregate)" />
    /// <param name="operation">The operation to perform.</param>
    /// <param name="keys">The keys of the sorted sets.</param>
    /// <param name="weights">The optional weights per set that correspond to keys.</param>
    /// <param name="aggregate">The aggregation method.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<ValkeyValue[]> SortedSetCombineAsync(SetOperation operation, IEnumerable<ValkeyKey> keys, IEnumerable<double>? weights, Aggregate aggregate, CommandFlags flags);

    /// <inheritdoc cref="ISortedSetCommands.SortedSetCombineWithScoresAsync(SetOperation, IEnumerable{ValkeyKey}, IEnumerable{double}?, Aggregate)" />
    /// <param name="operation">The operation to perform.</param>
    /// <param name="keys">The keys of the sorted sets.</param>
    /// <param name="weights">The optional weights per set that correspond to keys.</param>
    /// <param name="aggregate">The aggregation method.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<SortedSetEntry[]> SortedSetCombineWithScoresAsync(SetOperation operation, IEnumerable<ValkeyKey> keys, IEnumerable<double>? weights, Aggregate aggregate, CommandFlags flags);

    /// <inheritdoc cref="ISortedSetCommands.SortedSetCombineAndStoreAsync(SetOperation, ValkeyKey, ValkeyKey, ValkeyKey, Aggregate)" />
    /// <param name="operation">The operation to perform.</param>
    /// <param name="destination">The key to store the results in.</param>
    /// <param name="first">The key of the first sorted set.</param>
    /// <param name="second">The key of the second sorted set.</param>
    /// <param name="aggregate">The aggregation method.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<long> SortedSetCombineAndStoreAsync(SetOperation operation, ValkeyKey destination, ValkeyKey first, ValkeyKey second, Aggregate aggregate, CommandFlags flags);

    /// <inheritdoc cref="ISortedSetCommands.SortedSetCombineAndStoreAsync(SetOperation, ValkeyKey, IEnumerable{ValkeyKey}, IEnumerable{double}?, Aggregate)" />
    /// <param name="operation">The operation to perform.</param>
    /// <param name="destination">The key to store the results in.</param>
    /// <param name="keys">The keys of the sorted sets.</param>
    /// <param name="weights">The optional weights per set that correspond to keys.</param>
    /// <param name="aggregate">The aggregation method.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<long> SortedSetCombineAndStoreAsync(SetOperation operation, ValkeyKey destination, IEnumerable<ValkeyKey> keys, IEnumerable<double>? weights, Aggregate aggregate, CommandFlags flags);

    /// <inheritdoc cref="ISortedSetCommands.SortedSetIncrementAsync(ValkeyKey, ValkeyValue, double)" />
    /// <param name="key">The key of the sorted set.</param>
    /// <param name="member">A member of the sorted set.</param>
    /// <param name="value">The score increment.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<double> SortedSetIncrementAsync(ValkeyKey key, ValkeyValue member, double value, CommandFlags flags);

    /// <inheritdoc cref="ISortedSetCommands.SortedSetIntersectionLengthAsync(IEnumerable{ValkeyKey}, long)" />
    /// <param name="keys">The keys of the sorted sets.</param>
    /// <param name="limit">If the intersection cardinality reaches limit partway through the computation, the algorithm will exit and yield limit as the cardinality.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<long> SortedSetIntersectionLengthAsync(IEnumerable<ValkeyKey> keys, long limit, CommandFlags flags);

    /// <inheritdoc cref="ISortedSetCommands.SortedSetLengthByValueAsync(ValkeyKey, ValkeyValue, ValkeyValue, Exclude)" />
    /// <param name="key">The key of the sorted set.</param>
    /// <param name="min">The min value to filter by.</param>
    /// <param name="max">The max value to filter by.</param>
    /// <param name="exclude">Whether to exclude min and max from the range check.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<long> SortedSetLengthByValueAsync(ValkeyKey key, ValkeyValue min, ValkeyValue max, Exclude exclude, CommandFlags flags);

    /// <inheritdoc cref="ISortedSetCommands.SortedSetPopAsync(ValkeyKey, Order)" />
    /// <param name="key">The key of the sorted set.</param>
    /// <param name="order">The order to sort by when popping items out of the set.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<SortedSetEntry?> SortedSetPopAsync(ValkeyKey key, Order order, CommandFlags flags);

    /// <inheritdoc cref="ISortedSetCommands.SortedSetPopAsync(ValkeyKey, long, Order)" />
    /// <param name="key">The key of the sorted set.</param>
    /// <param name="count">The number of members to remove.</param>
    /// <param name="order">The order to sort by when popping items out of the set.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<SortedSetEntry[]> SortedSetPopAsync(ValkeyKey key, long count, Order order, CommandFlags flags);

    /// <inheritdoc cref="ISortedSetCommands.SortedSetPopAsync(IEnumerable{ValkeyKey}, long, Order)" />
    /// <param name="keys">The keys to check.</param>
    /// <param name="count">The maximum number of records to pop out of the sorted set.</param>
    /// <param name="order">The order to sort by when popping items out of the set.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<SortedSetPopResult> SortedSetPopAsync(IEnumerable<ValkeyKey> keys, long count, Order order, CommandFlags flags);

    /// <inheritdoc cref="ISortedSetCommands.SortedSetRandomMemberAsync(ValkeyKey)" />
    /// <param name="key">The key of the sorted set.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<ValkeyValue> SortedSetRandomMemberAsync(ValkeyKey key, CommandFlags flags);

    /// <inheritdoc cref="ISortedSetCommands.SortedSetRandomMembersAsync(ValkeyKey, long)" />
    /// <param name="key">The key of the sorted set.</param>
    /// <param name="count">The number of members to return.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<ValkeyValue[]> SortedSetRandomMembersAsync(ValkeyKey key, long count, CommandFlags flags);

    /// <inheritdoc cref="ISortedSetCommands.SortedSetRandomMembersWithScoresAsync(ValkeyKey, long)" />
    /// <param name="key">The key of the sorted set.</param>
    /// <param name="count">The number of members to return.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<SortedSetEntry[]> SortedSetRandomMembersWithScoresAsync(ValkeyKey key, long count, CommandFlags flags);

    /// <inheritdoc cref="ISortedSetCommands.SortedSetRangeAndStoreAsync(ValkeyKey, ValkeyKey, ValkeyValue, ValkeyValue, SortedSetOrder, Exclude, Order, long, long?)" />
    /// <param name="sourceKey">The sorted set to take the range from.</param>
    /// <param name="destinationKey">Where the resulting set will be stored.</param>
    /// <param name="start">The starting point in the sorted set.</param>
    /// <param name="stop">The stopping point in the range of the sorted set.</param>
    /// <param name="sortedSetOrder">The ordering criteria to use for the range.</param>
    /// <param name="exclude">Whether to exclude start and stop from the range check.</param>
    /// <param name="order">The direction to consider the start and stop in.</param>
    /// <param name="skip">The number of elements into the sorted set to skip.</param>
    /// <param name="take">The maximum number of elements to pull into the new set.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<long> SortedSetRangeAndStoreAsync(ValkeyKey sourceKey, ValkeyKey destinationKey, ValkeyValue start, ValkeyValue stop, SortedSetOrder sortedSetOrder, Exclude exclude, Order order, long skip, long? take, CommandFlags flags);

    /// <inheritdoc cref="ISortedSetCommands.SortedSetRankAsync(ValkeyKey, ValkeyValue, Order)" />
    /// <param name="key">The key of the sorted set.</param>
    /// <param name="member">The member to get the rank of.</param>
    /// <param name="order">The order to sort by.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<long?> SortedSetRankAsync(ValkeyKey key, ValkeyValue member, Order order, CommandFlags flags);

    /// <inheritdoc cref="ISortedSetCommands.SortedSetRemoveRangeByValueAsync(ValkeyKey, ValkeyValue, ValkeyValue, Exclude)" />
    /// <param name="key">The key of the sorted set.</param>
    /// <param name="min">The minimum lexicographical value.</param>
    /// <param name="max">The maximum lexicographical value.</param>
    /// <param name="exclude">Which of min and max to exclude.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<long> SortedSetRemoveRangeByValueAsync(ValkeyKey key, ValkeyValue min, ValkeyValue max, Exclude exclude, CommandFlags flags);

    /// <inheritdoc cref="ISortedSetCommands.SortedSetRemoveRangeByRankAsync(ValkeyKey, long, long)" />
    /// <param name="key">The key of the sorted set.</param>
    /// <param name="start">The start rank.</param>
    /// <param name="stop">The stop rank.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<long> SortedSetRemoveRangeByRankAsync(ValkeyKey key, long start, long stop, CommandFlags flags);

    /// <inheritdoc cref="ISortedSetCommands.SortedSetRemoveRangeByScoreAsync(ValkeyKey, double, double, Exclude)" />
    /// <param name="key">The key of the sorted set.</param>
    /// <param name="start">The minimum score to remove.</param>
    /// <param name="stop">The maximum score to remove.</param>
    /// <param name="exclude">Which of min and max to exclude.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<long> SortedSetRemoveRangeByScoreAsync(ValkeyKey key, double start, double stop, Exclude exclude, CommandFlags flags);

    /// <inheritdoc cref="ISortedSetCommands.SortedSetScanAsync(ValkeyKey, ValkeyValue, int, long, int)" />
    /// <param name="key">The key of the sorted set.</param>
    /// <param name="pattern">The pattern to match.</param>
    /// <param name="pageSize">The page size to iterate by.</param>
    /// <param name="cursor">The cursor position to start at.</param>
    /// <param name="pageOffset">The page offset to start at.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    IAsyncEnumerable<SortedSetEntry> SortedSetScanAsync(ValkeyKey key, ValkeyValue pattern, int pageSize, long cursor, int pageOffset, CommandFlags flags);

    /// <inheritdoc cref="ISortedSetCommands.SortedSetScoreAsync(ValkeyKey, ValkeyValue)" />
    /// <param name="key">The key of the sorted set.</param>
    /// <param name="member">The member whose score is to be retrieved.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<double?> SortedSetScoreAsync(ValkeyKey key, ValkeyValue member, CommandFlags flags);

    /// <inheritdoc cref="ISortedSetCommands.SortedSetScoresAsync(ValkeyKey, IEnumerable{ValkeyValue})" />
    /// <param name="key">The key of the sorted set.</param>
    /// <param name="members">The members to get the scores for.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<double?[]> SortedSetScoresAsync(ValkeyKey key, IEnumerable<ValkeyValue> members, CommandFlags flags);

    #endregion

    #region Generic Commands with CommandFlags (SER Compatibility)

    /// <inheritdoc cref="IGenericBaseCommands.KeyDeleteAsync(ValkeyKey)" />
    /// <param name="key">The key to delete.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<bool> KeyDeleteAsync(ValkeyKey key, CommandFlags flags);

    /// <inheritdoc cref="IGenericBaseCommands.KeyDeleteAsync(IEnumerable{ValkeyKey})" />
    /// <param name="keys">The keys to delete.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<long> KeyDeleteAsync(IEnumerable<ValkeyKey> keys, CommandFlags flags);

    /// <inheritdoc cref="IGenericBaseCommands.KeyUnlinkAsync(ValkeyKey)" />
    /// <param name="key">The key to unlink.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<bool> KeyUnlinkAsync(ValkeyKey key, CommandFlags flags);

    /// <inheritdoc cref="IGenericBaseCommands.KeyUnlinkAsync(IEnumerable{ValkeyKey})" />
    /// <param name="keys">The keys to unlink.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<long> KeyUnlinkAsync(IEnumerable<ValkeyKey> keys, CommandFlags flags);

    /// <inheritdoc cref="IGenericBaseCommands.KeyExistsAsync(ValkeyKey)" />
    /// <param name="key">The key to check.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<bool> KeyExistsAsync(ValkeyKey key, CommandFlags flags);

    /// <inheritdoc cref="IGenericBaseCommands.KeyExistsAsync(IEnumerable{ValkeyKey})" />
    /// <param name="keys">The keys to check.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<long> KeyExistsAsync(IEnumerable<ValkeyKey> keys, CommandFlags flags);

    /// <inheritdoc cref="IGenericBaseCommands.KeyExpireAsync(ValkeyKey, TimeSpan?)" />
    /// <param name="key">The key to expire.</param>
    /// <param name="expiry">Duration for the key to expire.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<bool> KeyExpireAsync(ValkeyKey key, TimeSpan? expiry, CommandFlags flags);

    /// <inheritdoc cref="IGenericBaseCommands.KeyExpireAsync(ValkeyKey, TimeSpan?, ExpireWhen)" />
    /// <param name="key">The key to expire.</param>
    /// <param name="expiry">Duration for the key to expire.</param>
    /// <param name="when">The option to set expiry.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<bool> KeyExpireAsync(ValkeyKey key, TimeSpan? expiry, ExpireWhen when, CommandFlags flags);

    /// <inheritdoc cref="IGenericBaseCommands.KeyExpireAsync(ValkeyKey, DateTime?)" />
    /// <param name="key">The key to expire.</param>
    /// <param name="expiry">The timestamp for expiry.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<bool> KeyExpireAsync(ValkeyKey key, DateTime? expiry, CommandFlags flags);

    /// <inheritdoc cref="IGenericBaseCommands.KeyExpireAsync(ValkeyKey, DateTime?, ExpireWhen)" />
    /// <param name="key">The key to expire.</param>
    /// <param name="expiry">The timestamp for expiry.</param>
    /// <param name="when">In Valkey 7+, the option to set expiry.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<bool> KeyExpireAsync(ValkeyKey key, DateTime? expiry, ExpireWhen when, CommandFlags flags);

    /// <inheritdoc cref="IGenericBaseCommands.KeyTimeToLiveAsync(ValkeyKey)" />
    /// <param name="key">The key to return its timeout.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<TimeSpan?> KeyTimeToLiveAsync(ValkeyKey key, CommandFlags flags);

    /// <inheritdoc cref="IGenericBaseCommands.KeyTypeAsync(ValkeyKey)" />
    /// <param name="key">The key to check its data type.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<ValkeyType> KeyTypeAsync(ValkeyKey key, CommandFlags flags);

    /// <inheritdoc cref="IGenericBaseCommands.KeyRenameAsync(ValkeyKey, ValkeyKey)" />
    /// <param name="key">The key to rename.</param>
    /// <param name="newKey">The new name of the key.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<bool> KeyRenameAsync(ValkeyKey key, ValkeyKey newKey, CommandFlags flags);

    /// <inheritdoc cref="IGenericBaseCommands.KeyRenameNXAsync(ValkeyKey, ValkeyKey)" />
    /// <param name="key">The key to rename.</param>
    /// <param name="newKey">The new name of the key.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<bool> KeyRenameNXAsync(ValkeyKey key, ValkeyKey newKey, CommandFlags flags);

    /// <inheritdoc cref="IGenericBaseCommands.KeyPersistAsync(ValkeyKey)" />
    /// <param name="key">The key to remove the existing timeout on.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<bool> KeyPersistAsync(ValkeyKey key, CommandFlags flags);

    /// <inheritdoc cref="IGenericBaseCommands.KeyDumpAsync(ValkeyKey)" />
    /// <param name="key">The key to serialize.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<byte[]?> KeyDumpAsync(ValkeyKey key, CommandFlags flags);

    /// <inheritdoc cref="IGenericBaseCommands.KeyRestoreAsync(ValkeyKey, byte[], TimeSpan?, RestoreOptions?)" />
    /// <param name="key">The key to create.</param>
    /// <param name="value">The serialized value to deserialize and assign to key.</param>
    /// <param name="expiry">The expiry to set as a duration.</param>
    /// <param name="restoreOptions">Set restore options with replace and absolute TTL modifiers, object idletime and frequency.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task KeyRestoreAsync(ValkeyKey key, byte[] value, TimeSpan? expiry, RestoreOptions? restoreOptions, CommandFlags flags);

    /// <inheritdoc cref="IGenericBaseCommands.KeyRestoreDateTimeAsync(ValkeyKey, byte[], DateTime?, RestoreOptions?)" />
    /// <param name="key">The key to create.</param>
    /// <param name="value">The serialized value to deserialize and assign to key.</param>
    /// <param name="expiry">The expiry to set as a date and time.</param>
    /// <param name="restoreOptions">Set restore options with replace and absolute TTL modifiers, object idletime and frequency.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task KeyRestoreDateTimeAsync(ValkeyKey key, byte[] value, DateTime? expiry, RestoreOptions? restoreOptions, CommandFlags flags);

    /// <inheritdoc cref="IGenericBaseCommands.KeyTouchAsync(ValkeyKey)" />
    /// <param name="key">The key to update last access time.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<bool> KeyTouchAsync(ValkeyKey key, CommandFlags flags);

    /// <inheritdoc cref="IGenericBaseCommands.KeyTouchAsync(IEnumerable{ValkeyKey})" />
    /// <param name="keys">The keys to update last access time.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<long> KeyTouchAsync(IEnumerable<ValkeyKey> keys, CommandFlags flags);

    /// <inheritdoc cref="IGenericBaseCommands.KeyExpireTimeAsync(ValkeyKey)" />
    /// <param name="key">The key to determine the expiration value of.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<DateTime?> KeyExpireTimeAsync(ValkeyKey key, CommandFlags flags);

    /// <inheritdoc cref="IGenericBaseCommands.KeyEncodingAsync(ValkeyKey)" />
    /// <param name="key">The key to determine the encoding of.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<string?> KeyEncodingAsync(ValkeyKey key, CommandFlags flags);

    /// <inheritdoc cref="IGenericBaseCommands.KeyFrequencyAsync(ValkeyKey)" />
    /// <param name="key">The key to determine the frequency of.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<long?> KeyFrequencyAsync(ValkeyKey key, CommandFlags flags);

    /// <inheritdoc cref="IGenericBaseCommands.KeyIdleTimeAsync(ValkeyKey)" />
    /// <param name="key">The key to determine the idle time of.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<long?> KeyIdleTimeAsync(ValkeyKey key, CommandFlags flags);

    /// <inheritdoc cref="IGenericBaseCommands.KeyRefCountAsync(ValkeyKey)" />
    /// <param name="key">The key to determine the reference count of.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<long?> KeyRefCountAsync(ValkeyKey key, CommandFlags flags);

    /// <inheritdoc cref="IGenericBaseCommands.KeyCopyAsync(ValkeyKey, ValkeyKey, bool)" />
    /// <param name="sourceKey">The key to the source value.</param>
    /// <param name="destinationKey">The key where the value should be copied to.</param>
    /// <param name="replace">Whether to overwrite an existing values at destinationKey.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<bool> KeyCopyAsync(ValkeyKey sourceKey, ValkeyKey destinationKey, bool replace, CommandFlags flags);

    /// <inheritdoc cref="IGenericBaseCommands.KeyMoveAsync(ValkeyKey, int)" />
    /// <param name="key">The key to move.</param>
    /// <param name="database">The database to move the key to.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<bool> KeyMoveAsync(ValkeyKey key, int database, CommandFlags flags);

    /// <inheritdoc cref="IGenericBaseCommands.KeyCopyAsync(ValkeyKey, ValkeyKey, int, bool)" />
    /// <param name="sourceKey">The key to the source value.</param>
    /// <param name="destinationKey">The key where the value should be copied to.</param>
    /// <param name="destinationDatabase">The database ID to store destinationKey in.</param>
    /// <param name="replace">Whether to overwrite an existing values at destinationKey.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<bool> KeyCopyAsync(ValkeyKey sourceKey, ValkeyKey destinationKey, int destinationDatabase, bool replace, CommandFlags flags);

    /// <inheritdoc cref="IGenericBaseCommands.KeyRandomAsync()" />
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<string?> KeyRandomAsync(CommandFlags flags);

    /// <inheritdoc cref="IGenericBaseCommands.SortAsync(ValkeyKey, long, long, Order, SortType, ValkeyValue, IEnumerable{ValkeyValue}?)" />
    /// <param name="key">The key of the list, set, or sorted set to be sorted.</param>
    /// <param name="skip">The number of elements to skip.</param>
    /// <param name="take">The number of elements to take. -1 means take all.</param>
    /// <param name="order">The sort order.</param>
    /// <param name="sortType">The sort type.</param>
    /// <param name="by">The pattern to sort by external keys.</param>
    /// <param name="get">The patterns to retrieve external keys' values.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<ValkeyValue[]> SortAsync(ValkeyKey key, long skip, long take, Order order, SortType sortType, ValkeyValue by, IEnumerable<ValkeyValue>? get, CommandFlags flags);

    /// <inheritdoc cref="IGenericBaseCommands.WaitAsync(long, long)" />
    /// <param name="numreplicas">The number of replicas to wait for.</param>
    /// <param name="timeout">The timeout in milliseconds.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<long> WaitAsync(long numreplicas, long timeout, CommandFlags flags);

    #endregion

    #region Bitmap Commands with CommandFlags (SER Compatibility)

    /// <inheritdoc cref="IBitmapCommands.StringGetBitAsync(ValkeyKey, long)" />
    /// <param name="key">The key of the string.</param>
    /// <param name="offset">The offset in the string to get the bit at.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<bool> StringGetBitAsync(ValkeyKey key, long offset, CommandFlags flags);

    /// <inheritdoc cref="IBitmapCommands.StringSetBitAsync(ValkeyKey, long, bool)" />
    /// <param name="key">The key of the string.</param>
    /// <param name="offset">The offset in the string to set the bit at.</param>
    /// <param name="value">The bit value to set (true for 1, false for 0).</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<bool> StringSetBitAsync(ValkeyKey key, long offset, bool value, CommandFlags flags);

    /// <inheritdoc cref="IBitmapCommands.StringBitCountAsync(ValkeyKey, long, long, StringIndexType)" />
    /// <param name="key">The key of the string.</param>
    /// <param name="start">The start offset.</param>
    /// <param name="end">The end offset.</param>
    /// <param name="indexType">The index type (bit or byte).</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<long> StringBitCountAsync(ValkeyKey key, long start, long end, StringIndexType indexType, CommandFlags flags);

    /// <inheritdoc cref="IBitmapCommands.StringBitPositionAsync(ValkeyKey, bool, long, long, StringIndexType)" />
    /// <param name="key">The key of the string.</param>
    /// <param name="bit">The bit value to search for (true for 1, false for 0).</param>
    /// <param name="start">The start offset.</param>
    /// <param name="end">The end offset.</param>
    /// <param name="indexType">The index type (bit or byte).</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<long> StringBitPositionAsync(ValkeyKey key, bool bit, long start, long end, StringIndexType indexType, CommandFlags flags);

    /// <inheritdoc cref="IBitmapCommands.StringBitOperationAsync(Bitwise, ValkeyKey, ValkeyKey, ValkeyKey)" />
    /// <param name="operation">The bitwise operation to perform.</param>
    /// <param name="destination">The key to store the result.</param>
    /// <param name="first">The first source key.</param>
    /// <param name="second">The second source key.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<long> StringBitOperationAsync(Bitwise operation, ValkeyKey destination, ValkeyKey first, ValkeyKey second, CommandFlags flags);

    /// <inheritdoc cref="IBitmapCommands.StringBitOperationAsync(Bitwise, ValkeyKey, IEnumerable{ValkeyKey})" />
    /// <param name="operation">The bitwise operation to perform.</param>
    /// <param name="destination">The key to store the result.</param>
    /// <param name="keys">The source keys.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<long> StringBitOperationAsync(Bitwise operation, ValkeyKey destination, IEnumerable<ValkeyKey> keys, CommandFlags flags);

    /// <inheritdoc cref="IBitmapCommands.StringBitFieldAsync(ValkeyKey, IEnumerable{Commands.Options.BitFieldOptions.IBitFieldSubCommand})" />
    /// <param name="key">The key of the string.</param>
    /// <param name="subCommands">The subcommands to execute (GET, SET, INCRBY).</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<long[]> StringBitFieldAsync(ValkeyKey key, IEnumerable<Commands.Options.BitFieldOptions.IBitFieldSubCommand> subCommands, CommandFlags flags);

    /// <inheritdoc cref="IBitmapCommands.StringBitFieldReadOnlyAsync(ValkeyKey, IEnumerable{Commands.Options.BitFieldOptions.IBitFieldReadOnlySubCommand})" />
    /// <param name="key">The key of the string.</param>
    /// <param name="subCommands">The GET subcommands to execute.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<long[]> StringBitFieldReadOnlyAsync(ValkeyKey key, IEnumerable<Commands.Options.BitFieldOptions.IBitFieldReadOnlySubCommand> subCommands, CommandFlags flags);

    #endregion

    #region HyperLogLog Commands with CommandFlags (SER Compatibility)

    /// <inheritdoc cref="IHyperLogLogCommands.HyperLogLogAddAsync(ValkeyKey, ValkeyValue)" />
    /// <param name="key">The key of the HyperLogLog.</param>
    /// <param name="element">The element to add to the HyperLogLog.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<bool> HyperLogLogAddAsync(ValkeyKey key, ValkeyValue element, CommandFlags flags);

    /// <inheritdoc cref="IHyperLogLogCommands.HyperLogLogAddAsync(ValkeyKey, IEnumerable{ValkeyValue})" />
    /// <param name="key">The key of the HyperLogLog.</param>
    /// <param name="elements">The elements to add to the HyperLogLog.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<bool> HyperLogLogAddAsync(ValkeyKey key, IEnumerable<ValkeyValue> elements, CommandFlags flags);

    /// <inheritdoc cref="IHyperLogLogCommands.HyperLogLogLengthAsync(ValkeyKey)" />
    /// <param name="key">The key of the HyperLogLog.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<long> HyperLogLogLengthAsync(ValkeyKey key, CommandFlags flags);

    /// <inheritdoc cref="IHyperLogLogCommands.HyperLogLogLengthAsync(IEnumerable{ValkeyKey})" />
    /// <param name="keys">The keys of the HyperLogLogs.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<long> HyperLogLogLengthAsync(IEnumerable<ValkeyKey> keys, CommandFlags flags);

    /// <inheritdoc cref="IHyperLogLogCommands.HyperLogLogMergeAsync(ValkeyKey, ValkeyKey, ValkeyKey)" />
    /// <param name="destination">The key of the destination HyperLogLog.</param>
    /// <param name="first">The key of the first source HyperLogLog.</param>
    /// <param name="second">The key of the second source HyperLogLog.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task HyperLogLogMergeAsync(ValkeyKey destination, ValkeyKey first, ValkeyKey second, CommandFlags flags);

    /// <inheritdoc cref="IHyperLogLogCommands.HyperLogLogMergeAsync(ValkeyKey, IEnumerable{ValkeyKey})" />
    /// <param name="destination">The key of the destination HyperLogLog.</param>
    /// <param name="sourceKeys">The keys of the source HyperLogLogs.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task HyperLogLogMergeAsync(ValkeyKey destination, IEnumerable<ValkeyKey> sourceKeys, CommandFlags flags);

    #endregion

    #region Stream Commands with CommandFlags (SER Compatibility)

    /// <inheritdoc cref="IStreamCommands.StreamAddAsync(ValkeyKey, ValkeyValue, ValkeyValue, ValkeyValue?, int?, bool)" />
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<ValkeyValue> StreamAddAsync(ValkeyKey key, ValkeyValue streamField, ValkeyValue streamValue, ValkeyValue? messageId, int? maxLength, bool useApproximateMaxLength, CommandFlags flags);

    /// <inheritdoc cref="IStreamCommands.StreamAddAsync(ValkeyKey, IEnumerable{NameValueEntry}, ValkeyValue?, int?, bool)" />
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<ValkeyValue> StreamAddAsync(ValkeyKey key, IEnumerable<NameValueEntry> streamPairs, ValkeyValue? messageId, int? maxLength, bool useApproximateMaxLength, CommandFlags flags);

    /// <inheritdoc cref="IStreamCommands.StreamAddAsync(ValkeyKey, ValkeyValue, ValkeyValue, ValkeyValue?, long?, bool, long?, bool)" />
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<ValkeyValue> StreamAddAsync(ValkeyKey key, ValkeyValue streamField, ValkeyValue streamValue, ValkeyValue? messageId, long? maxLength, bool useApproximateMaxLength, long? limit, bool noMakeStream, CommandFlags flags);

    /// <inheritdoc cref="IStreamCommands.StreamAddAsync(ValkeyKey, IEnumerable{NameValueEntry}, ValkeyValue?, long?, bool, long?, bool)" />
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<ValkeyValue> StreamAddAsync(ValkeyKey key, IEnumerable<NameValueEntry> streamPairs, ValkeyValue? messageId, long? maxLength, bool useApproximateMaxLength, long? limit, bool noMakeStream, CommandFlags flags);

    /// <inheritdoc cref="IStreamCommands.StreamReadAsync(ValkeyKey, ValkeyValue, int?)" />
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<StreamEntry[]> StreamReadAsync(ValkeyKey key, ValkeyValue position, int? countPerStream, CommandFlags flags);

    /// <inheritdoc cref="IStreamCommands.StreamReadAsync(IEnumerable{StreamPosition}, int?)" />
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<ValkeyStream[]> StreamReadAsync(IEnumerable<StreamPosition> streamPositions, int? countPerStream, CommandFlags flags);

    /// <inheritdoc cref="IStreamCommands.StreamRangeAsync(ValkeyKey, ValkeyValue?, ValkeyValue?, int?, Order)" />
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<StreamEntry[]> StreamRangeAsync(ValkeyKey key, ValkeyValue? start, ValkeyValue? end, int? count, Order order, CommandFlags flags);

    /// <inheritdoc cref="IStreamCommands.StreamCreateConsumerGroupAsync(ValkeyKey, ValkeyValue, ValkeyValue?)" />
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<bool> StreamCreateConsumerGroupAsync(ValkeyKey key, ValkeyValue groupName, ValkeyValue? position, CommandFlags flags);

    /// <inheritdoc cref="IStreamCommands.StreamCreateConsumerGroupAsync(ValkeyKey, ValkeyValue, ValkeyValue?, bool, long?)" />
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<bool> StreamCreateConsumerGroupAsync(ValkeyKey key, ValkeyValue groupName, ValkeyValue? position, bool createStream, long? entriesRead, CommandFlags flags);

    /// <inheritdoc cref="IStreamCommands.StreamDeleteConsumerGroupAsync(ValkeyKey, ValkeyValue)" />
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<bool> StreamDeleteConsumerGroupAsync(ValkeyKey key, ValkeyValue groupName, CommandFlags flags);

    /// <inheritdoc cref="IStreamCommands.StreamCreateConsumerAsync(ValkeyKey, ValkeyValue, ValkeyValue)" />
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<bool> StreamCreateConsumerAsync(ValkeyKey key, ValkeyValue groupName, ValkeyValue consumerName, CommandFlags flags);

    /// <inheritdoc cref="IStreamCommands.StreamDeleteConsumerAsync(ValkeyKey, ValkeyValue, ValkeyValue)" />
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<long> StreamDeleteConsumerAsync(ValkeyKey key, ValkeyValue groupName, ValkeyValue consumerName, CommandFlags flags);

    /// <inheritdoc cref="IStreamCommands.StreamConsumerGroupSetPositionAsync(ValkeyKey, ValkeyValue, ValkeyValue, long?)" />
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<bool> StreamConsumerGroupSetPositionAsync(ValkeyKey key, ValkeyValue groupName, ValkeyValue position, long? entriesRead, CommandFlags flags);

    /// <inheritdoc cref="IStreamCommands.StreamReadGroupAsync(ValkeyKey, ValkeyValue, ValkeyValue, ValkeyValue?, int?)" />
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<StreamEntry[]> StreamReadGroupAsync(ValkeyKey key, ValkeyValue groupName, ValkeyValue consumerName, ValkeyValue? position, int? count, CommandFlags flags);

    /// <inheritdoc cref="IStreamCommands.StreamReadGroupAsync(ValkeyKey, ValkeyValue, ValkeyValue, ValkeyValue?, int?, bool)" />
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<StreamEntry[]> StreamReadGroupAsync(ValkeyKey key, ValkeyValue groupName, ValkeyValue consumerName, ValkeyValue? position, int? count, bool noAck, CommandFlags flags);

    /// <inheritdoc cref="IStreamCommands.StreamReadGroupAsync(IEnumerable{StreamPosition}, ValkeyValue, ValkeyValue, int?)" />
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<ValkeyStream[]> StreamReadGroupAsync(IEnumerable<StreamPosition> streamPositions, ValkeyValue groupName, ValkeyValue consumerName, int? countPerStream, CommandFlags flags);

    /// <inheritdoc cref="IStreamCommands.StreamReadGroupAsync(IEnumerable{StreamPosition}, ValkeyValue, ValkeyValue, int?, bool)" />
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<ValkeyStream[]> StreamReadGroupAsync(IEnumerable<StreamPosition> streamPositions, ValkeyValue groupName, ValkeyValue consumerName, int? countPerStream, bool noAck, CommandFlags flags);

    /// <inheritdoc cref="IStreamCommands.StreamAcknowledgeAsync(ValkeyKey, ValkeyValue, ValkeyValue)" />
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<long> StreamAcknowledgeAsync(ValkeyKey key, ValkeyValue groupName, ValkeyValue messageId, CommandFlags flags);

    /// <inheritdoc cref="IStreamCommands.StreamAcknowledgeAsync(ValkeyKey, ValkeyValue, IEnumerable{ValkeyValue})" />
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<long> StreamAcknowledgeAsync(ValkeyKey key, ValkeyValue groupName, IEnumerable<ValkeyValue> messageIds, CommandFlags flags);

    /// <inheritdoc cref="IStreamCommands.StreamPendingAsync(ValkeyKey, ValkeyValue)" />
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<StreamPendingInfo> StreamPendingAsync(ValkeyKey key, ValkeyValue groupName, CommandFlags flags);

    /// <inheritdoc cref="IStreamCommands.StreamPendingMessagesAsync(ValkeyKey, ValkeyValue, int, ValkeyValue, ValkeyValue?, ValkeyValue?)" />
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<StreamPendingMessageInfo[]> StreamPendingMessagesAsync(ValkeyKey key, ValkeyValue groupName, int count, ValkeyValue consumerName, ValkeyValue? minId, ValkeyValue? maxId, CommandFlags flags);

    /// <inheritdoc cref="IStreamCommands.StreamPendingMessagesAsync(ValkeyKey, ValkeyValue, int, ValkeyValue, ValkeyValue?, ValkeyValue?, long?)" />
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<StreamPendingMessageInfo[]> StreamPendingMessagesAsync(ValkeyKey key, ValkeyValue groupName, int count, ValkeyValue consumerName, ValkeyValue? minId, ValkeyValue? maxId, long? minIdleTimeInMs, CommandFlags flags);

    /// <inheritdoc cref="IStreamCommands.StreamClaimAsync(ValkeyKey, ValkeyValue, ValkeyValue, long, IEnumerable{ValkeyValue})" />
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<StreamEntry[]> StreamClaimAsync(ValkeyKey key, ValkeyValue consumerGroup, ValkeyValue claimingConsumer, long minIdleTimeInMs, IEnumerable<ValkeyValue> messageIds, CommandFlags flags);

    /// <inheritdoc cref="IStreamCommands.StreamClaimAsync(ValkeyKey, ValkeyValue, ValkeyValue, long, IEnumerable{ValkeyValue}, long?, long?, int?, bool)" />
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<StreamEntry[]> StreamClaimAsync(ValkeyKey key, ValkeyValue consumerGroup, ValkeyValue claimingConsumer, long minIdleTimeInMs, IEnumerable<ValkeyValue> messageIds, long? idleTimeInMs, long? timeUnixMs, int? retryCount, bool force, CommandFlags flags);

    /// <inheritdoc cref="IStreamCommands.StreamClaimIdsOnlyAsync(ValkeyKey, ValkeyValue, ValkeyValue, long, IEnumerable{ValkeyValue})" />
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<ValkeyValue[]> StreamClaimIdsOnlyAsync(ValkeyKey key, ValkeyValue consumerGroup, ValkeyValue claimingConsumer, long minIdleTimeInMs, IEnumerable<ValkeyValue> messageIds, CommandFlags flags);

    /// <inheritdoc cref="IStreamCommands.StreamClaimIdsOnlyAsync(ValkeyKey, ValkeyValue, ValkeyValue, long, IEnumerable{ValkeyValue}, long?, long?, int?, bool)" />
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<ValkeyValue[]> StreamClaimIdsOnlyAsync(ValkeyKey key, ValkeyValue consumerGroup, ValkeyValue claimingConsumer, long minIdleTimeInMs, IEnumerable<ValkeyValue> messageIds, long? idleTimeInMs, long? timeUnixMs, int? retryCount, bool force, CommandFlags flags);

    /// <inheritdoc cref="IStreamCommands.StreamAutoClaimAsync(ValkeyKey, ValkeyValue, ValkeyValue, long, ValkeyValue, int?)" />
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<StreamAutoClaimResult> StreamAutoClaimAsync(ValkeyKey key, ValkeyValue consumerGroup, ValkeyValue claimingConsumer, long minIdleTimeInMs, ValkeyValue startAtId, int? count, CommandFlags flags);

    /// <inheritdoc cref="IStreamCommands.StreamAutoClaimIdsOnlyAsync(ValkeyKey, ValkeyValue, ValkeyValue, long, ValkeyValue, int?)" />
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<StreamAutoClaimIdsOnlyResult> StreamAutoClaimIdsOnlyAsync(ValkeyKey key, ValkeyValue consumerGroup, ValkeyValue claimingConsumer, long minIdleTimeInMs, ValkeyValue startAtId, int? count, CommandFlags flags);

    /// <inheritdoc cref="IStreamCommands.StreamLengthAsync(ValkeyKey)" />
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<long> StreamLengthAsync(ValkeyKey key, CommandFlags flags);

    /// <inheritdoc cref="IStreamCommands.StreamDeleteAsync(ValkeyKey, IEnumerable{ValkeyValue})" />
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<long> StreamDeleteAsync(ValkeyKey key, IEnumerable<ValkeyValue> messageIds, CommandFlags flags);

    /// <inheritdoc cref="IStreamCommands.StreamTrimAsync(ValkeyKey, int, bool)" />
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<long> StreamTrimAsync(ValkeyKey key, int maxLength, bool useApproximateMaxLength, CommandFlags flags);

    /// <inheritdoc cref="IStreamCommands.StreamTrimAsync(ValkeyKey, long?, bool, long?)" />
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<long> StreamTrimAsync(ValkeyKey key, long? maxLength, bool useApproximateMaxLength, long? limit, CommandFlags flags);

    /// <inheritdoc cref="IStreamCommands.StreamTrimByMinIdAsync(ValkeyKey, ValkeyValue, bool, long?)" />
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<long> StreamTrimByMinIdAsync(ValkeyKey key, ValkeyValue minId, bool useApproximateMaxLength, long? limit, CommandFlags flags);

    /// <inheritdoc cref="IStreamCommands.StreamInfoAsync(ValkeyKey)" />
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<StreamInfo> StreamInfoAsync(ValkeyKey key, CommandFlags flags);

    /// <inheritdoc cref="IStreamCommands.StreamGroupInfoAsync(ValkeyKey)" />
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<StreamGroupInfo[]> StreamGroupInfoAsync(ValkeyKey key, CommandFlags flags);

    /// <inheritdoc cref="IStreamCommands.StreamConsumerInfoAsync(ValkeyKey, ValkeyValue)" />
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<StreamConsumerInfo[]> StreamConsumerInfoAsync(ValkeyKey key, ValkeyValue groupName, CommandFlags flags);

    #endregion

    #region Geospatial Commands with CommandFlags (SER Compatibility)

    /// <inheritdoc cref="IGeospatialCommands.GeoAddAsync(ValkeyKey, double, double, ValkeyValue)" />
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<bool> GeoAddAsync(ValkeyKey key, double longitude, double latitude, ValkeyValue member, CommandFlags flags);

    /// <inheritdoc cref="IGeospatialCommands.GeoAddAsync(ValkeyKey, GeoEntry)" />
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<bool> GeoAddAsync(ValkeyKey key, GeoEntry value, CommandFlags flags);

    /// <inheritdoc cref="IGeospatialCommands.GeoAddAsync(ValkeyKey, IEnumerable{GeoEntry})" />
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<long> GeoAddAsync(ValkeyKey key, IEnumerable<GeoEntry> values, CommandFlags flags);

    /// <inheritdoc cref="IGeospatialCommands.GeoAddAsync(ValkeyKey, GeoEntry, GeoAddOptions)" />
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<bool> GeoAddAsync(ValkeyKey key, GeoEntry value, GeoAddOptions options, CommandFlags flags);

    /// <inheritdoc cref="IGeospatialCommands.GeoAddAsync(ValkeyKey, IEnumerable{GeoEntry}, GeoAddOptions)" />
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<long> GeoAddAsync(ValkeyKey key, IEnumerable<GeoEntry> values, GeoAddOptions options, CommandFlags flags);

    /// <inheritdoc cref="IGeospatialCommands.GeoDistanceAsync(ValkeyKey, ValkeyValue, ValkeyValue, GeoUnit)" />
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<double?> GeoDistanceAsync(ValkeyKey key, ValkeyValue member1, ValkeyValue member2, GeoUnit unit, CommandFlags flags);

    /// <inheritdoc cref="IGeospatialCommands.GeoHashAsync(ValkeyKey, ValkeyValue)" />
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<string?> GeoHashAsync(ValkeyKey key, ValkeyValue member, CommandFlags flags);

    /// <inheritdoc cref="IGeospatialCommands.GeoHashAsync(ValkeyKey, IEnumerable{ValkeyValue})" />
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<string?[]> GeoHashAsync(ValkeyKey key, IEnumerable<ValkeyValue> members, CommandFlags flags);

    /// <inheritdoc cref="IGeospatialCommands.GeoPositionAsync(ValkeyKey, ValkeyValue)" />
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<GeoPosition?> GeoPositionAsync(ValkeyKey key, ValkeyValue member, CommandFlags flags);

    /// <inheritdoc cref="IGeospatialCommands.GeoPositionAsync(ValkeyKey, IEnumerable{ValkeyValue})" />
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<GeoPosition?[]> GeoPositionAsync(ValkeyKey key, IEnumerable<ValkeyValue> members, CommandFlags flags);

    /// <inheritdoc cref="IGeospatialCommands.GeoSearchAsync(ValkeyKey, ValkeyValue, GeoSearchShape, long, bool, Order?, GeoRadiusOptions)" />
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<GeoRadiusResult[]> GeoSearchAsync(ValkeyKey key, ValkeyValue fromMember, GeoSearchShape shape, long count, bool demandClosest, Order? order, GeoRadiusOptions options, CommandFlags flags);

    /// <inheritdoc cref="IGeospatialCommands.GeoSearchAsync(ValkeyKey, GeoPosition, GeoSearchShape, long, bool, Order?, GeoRadiusOptions)" />
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<GeoRadiusResult[]> GeoSearchAsync(ValkeyKey key, GeoPosition fromPosition, GeoSearchShape shape, long count, bool demandClosest, Order? order, GeoRadiusOptions options, CommandFlags flags);

    /// <inheritdoc cref="IGeospatialCommands.GeoSearchAndStoreAsync(ValkeyKey, ValkeyKey, ValkeyValue, GeoSearchShape, long, bool, Order?, bool)" />
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<long> GeoSearchAndStoreAsync(ValkeyKey sourceKey, ValkeyKey destinationKey, ValkeyValue fromMember, GeoSearchShape shape, long count, bool demandClosest, Order? order, bool storeDistances, CommandFlags flags);

    /// <inheritdoc cref="IGeospatialCommands.GeoSearchAndStoreAsync(ValkeyKey, ValkeyKey, GeoPosition, GeoSearchShape, long, bool, Order?, bool)" />
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<long> GeoSearchAndStoreAsync(ValkeyKey sourceKey, ValkeyKey destinationKey, GeoPosition fromPosition, GeoSearchShape shape, long count, bool demandClosest, Order? order, bool storeDistances, CommandFlags flags);

    #endregion

    #region Connection Management Commands with CommandFlags (SER Compatibility)

    /// <inheritdoc cref="IConnectionManagementCommands.ClientGetNameAsync()" />
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<ValkeyValue> ClientGetNameAsync(CommandFlags flags);

    /// <inheritdoc cref="IConnectionManagementClusterCommands.ClientGetNameAsync(Route)" />
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<ClusterValue<ValkeyValue>> ClientGetNameAsync(Route route, CommandFlags flags);

    /// <inheritdoc cref="IConnectionManagementCommands.ClientIdAsync()" />
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<long> ClientIdAsync(CommandFlags flags);

    /// <inheritdoc cref="IConnectionManagementClusterCommands.ClientIdAsync(Route)" />
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<ClusterValue<long>> ClientIdAsync(Route route, CommandFlags flags);

    #endregion

    #region Server Management Commands with CommandFlags (SER Compatibility)

    /// <inheritdoc cref="IServerManagementCommands.EchoAsync(ValkeyValue)" />
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<ValkeyValue> EchoAsync(ValkeyValue message, CommandFlags flags);

    /// <inheritdoc cref="IServerManagementClusterCommands.EchoAsync(ValkeyValue, Route)" />
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<ClusterValue<ValkeyValue>> EchoAsync(ValkeyValue message, Route route, CommandFlags flags);

    /// <inheritdoc cref="IServerManagementCommands.ConfigGetAsync(ValkeyValue)" />
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<KeyValuePair<string, string>[]> ConfigGetAsync(ValkeyValue pattern, CommandFlags flags);

    /// <inheritdoc cref="IServerManagementClusterCommands.ConfigGetAsync(ValkeyValue)" />
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<ClusterValue<KeyValuePair<string, string>[]>> ConfigGetAsync(ValkeyValue pattern, Route route, CommandFlags flags);

    /// <inheritdoc cref="IServerManagementCommands.ConfigResetStatisticsAsync()" />
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task ConfigResetStatisticsAsync(CommandFlags flags);

    /// <inheritdoc cref="IServerManagementClusterCommands.ConfigResetStatisticsAsync(Route)" />
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task ConfigResetStatisticsAsync(Route route, CommandFlags flags);

    /// <inheritdoc cref="IServerManagementCommands.ConfigRewriteAsync()" />
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task ConfigRewriteAsync(CommandFlags flags);

    /// <inheritdoc cref="IServerManagementClusterCommands.ConfigRewriteAsync(Route)" />
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task ConfigRewriteAsync(Route route, CommandFlags flags);

    /// <inheritdoc cref="IServerManagementCommands.ConfigSetAsync(ValkeyValue, ValkeyValue)" />
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task ConfigSetAsync(ValkeyValue setting, ValkeyValue value, CommandFlags flags);

    /// <inheritdoc cref="IServerManagementClusterCommands.ConfigSetAsync(ValkeyValue, ValkeyValue, Route)" />
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task ConfigSetAsync(ValkeyValue setting, ValkeyValue value, Route route, CommandFlags flags);

    /// <inheritdoc cref="IServerManagementCommands.DatabaseSizeAsync(int)" />
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<long> DatabaseSizeAsync(int database, CommandFlags flags);

    /// <inheritdoc cref="IServerManagementClusterCommands.DatabaseSizeAsync(Route, int)" />
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<ClusterValue<long>> DatabaseSizeAsync(Route route, int database, CommandFlags flags);

    /// <inheritdoc cref="IServerManagementCommands.FlushAllDatabasesAsync()" />
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task FlushAllDatabasesAsync(CommandFlags flags);

    /// <inheritdoc cref="IServerManagementClusterCommands.FlushAllDatabasesAsync(Route)" />
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task FlushAllDatabasesAsync(Route route, CommandFlags flags);

    /// <inheritdoc cref="IServerManagementCommands.FlushDatabaseAsync(int)" />
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task FlushDatabaseAsync(int database, CommandFlags flags);

    /// <inheritdoc cref="IServerManagementClusterCommands.FlushDatabaseAsync(Route, int)" />
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task FlushDatabaseAsync(Route route, int database, CommandFlags flags);

    /// <inheritdoc cref="IServerManagementCommands.LastSaveAsync()" />
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<DateTime> LastSaveAsync(CommandFlags flags);

    /// <inheritdoc cref="IServerManagementClusterCommands.LastSaveAsync(Route)" />
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<ClusterValue<DateTime>> LastSaveAsync(Route route, CommandFlags flags);

    /// <inheritdoc cref="IServerManagementCommands.TimeAsync()" />
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<DateTime> TimeAsync(CommandFlags flags);

    /// <inheritdoc cref="IServerManagementClusterCommands.TimeAsync(Route)" />
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<ClusterValue<DateTime>> TimeAsync(Route route, CommandFlags flags);

    /// <inheritdoc cref="IServerManagementCommands.LolwutAsync()" />
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<string> LolwutAsync(CommandFlags flags);

    /// <inheritdoc cref="IServerManagementClusterCommands.LolwutAsync(Route)" />
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<ClusterValue<string>> LolwutAsync(Route route, CommandFlags flags);

    /// <inheritdoc cref="IServerManagementCommands.SelectAsync(long)" />
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task SelectAsync(long index, CommandFlags flags);

    #endregion

    #region Scripting Commands with CommandFlags (SER Compatibility)

    /// <inheritdoc cref="IScriptingAndFunctionBaseCommands.ScriptEvaluateAsync(string, IEnumerable{ValkeyKey}?, IEnumerable{ValkeyValue}?)" />
    /// <param name="script">The Lua script to evaluate.</param>
    /// <param name="keys">The keys to pass to the script (KEYS array).</param>
    /// <param name="values">The values to pass to the script (ARGV array).</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<ValkeyResult> ScriptEvaluateAsync(
        string script,
        IEnumerable<ValkeyKey>? keys,
        IEnumerable<ValkeyValue>? values,
        CommandFlags flags);

    /// <inheritdoc cref="IScriptingAndFunctionBaseCommands.ScriptEvaluateAsync(byte[], IEnumerable{ValkeyKey}?, IEnumerable{ValkeyValue}?)" />
    /// <param name="hash">The SHA1 hash of the script to evaluate.</param>
    /// <param name="keys">The keys to pass to the script (KEYS array).</param>
    /// <param name="values">The values to pass to the script (ARGV array).</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<ValkeyResult> ScriptEvaluateAsync(
        byte[] hash,
        IEnumerable<ValkeyKey>? keys,
        IEnumerable<ValkeyValue>? values,
        CommandFlags flags);

    /// <inheritdoc cref="IScriptingAndFunctionBaseCommands.ScriptEvaluateAsync(LuaScript, object?)" />
    /// <param name="script">The LuaScript to evaluate.</param>
    /// <param name="parameters">An object containing parameter values.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<ValkeyResult> ScriptEvaluateAsync(LuaScript script, object? parameters, CommandFlags flags);

    /// <inheritdoc cref="IScriptingAndFunctionBaseCommands.ScriptEvaluateAsync(LoadedLuaScript, object?)" />
    /// <param name="script">The LoadedLuaScript to evaluate.</param>
    /// <param name="parameters">An object containing parameter values.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task<ValkeyResult> ScriptEvaluateAsync(LoadedLuaScript script, object? parameters, CommandFlags flags);

    #endregion
}
