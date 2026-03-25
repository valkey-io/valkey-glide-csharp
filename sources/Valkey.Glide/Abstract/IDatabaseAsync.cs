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
}
