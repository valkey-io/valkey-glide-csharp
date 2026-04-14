// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands;

namespace Valkey.Glide;

/// <summary>
/// Set commands with <see cref="CommandFlags"/> for StackExchange.Redis compatibility.
/// </summary>
/// <seealso cref="ISetBaseCommands" />
public partial interface IDatabaseAsync
{
    /// <inheritdoc cref="ISetBaseCommands.SetAddAsync(ValkeyKey, ValkeyValue)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<bool> SetAddAsync(ValkeyKey key, ValkeyValue value, CommandFlags flags);

    /// <inheritdoc cref="ISetBaseCommands.SetAddAsync(ValkeyKey, IEnumerable{ValkeyValue})"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<long> SetAddAsync(ValkeyKey key, IEnumerable<ValkeyValue> values, CommandFlags flags);

    /// <inheritdoc cref="ISetBaseCommands.SetRemoveAsync(ValkeyKey, ValkeyValue)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<bool> SetRemoveAsync(ValkeyKey key, ValkeyValue value, CommandFlags flags);

    /// <inheritdoc cref="ISetBaseCommands.SetRemoveAsync(ValkeyKey, IEnumerable{ValkeyValue})"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<long> SetRemoveAsync(ValkeyKey key, IEnumerable<ValkeyValue> values, CommandFlags flags);

    /// <inheritdoc cref="IBaseClient.SetMembersAsync(ValkeyKey)" path="/*[not(self::returns)]"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <returns>An array containing all members of the set.</returns>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<ValkeyValue[]> SetMembersAsync(ValkeyKey key, CommandFlags flags = CommandFlags.None);

    /// <inheritdoc cref="IBaseClient.SetCardAsync(ValkeyKey)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<long> SetLengthAsync(ValkeyKey key, CommandFlags flags = CommandFlags.None);

    /// <inheritdoc cref="IBaseClient.SetInterCardAsync(IEnumerable{ValkeyKey}, long)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<long> SetIntersectionLengthAsync(IEnumerable<ValkeyKey> keys, long limit = 0, CommandFlags flags = CommandFlags.None);

    /// <inheritdoc cref="ISetBaseCommands.SetPopAsync(ValkeyKey)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<ValkeyValue> SetPopAsync(ValkeyKey key, CommandFlags flags);

    /// <inheritdoc cref="ISetBaseCommands.SetPopAsync(ValkeyKey, long)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<ValkeyValue[]> SetPopAsync(ValkeyKey key, long count, CommandFlags flags);

    /// <summary>
    /// Returns the members of the set resulting from the specified <paramref name="operation"/> of the sets stored at <paramref name="first"/> and <paramref name="second"/>.
    /// </summary>
    /// <param name="operation">The set operation to perform (union, intersect, or difference).</param>
    /// <param name="first">The key of the first set.</param>
    /// <param name="second">The key of the second set.</param>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <returns>An array of set members resulting from the operation.</returns>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<ValkeyValue[]> SetCombineAsync(SetOperation operation, ValkeyKey first, ValkeyKey second, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Returns the members of the set resulting from the specified <paramref name="operation"/> of the sets stored at the given <paramref name="keys"/>.
    /// </summary>
    /// <param name="operation">The set operation to perform (union, intersect, or difference).</param>
    /// <param name="keys">The keys of the sets to combine.</param>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <returns>An array of set members resulting from the operation.</returns>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<ValkeyValue[]> SetCombineAsync(SetOperation operation, IEnumerable<ValkeyKey> keys, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Performs the specified <paramref name="operation"/> on the sets stored at <paramref name="first"/> and <paramref name="second"/>,
    /// and stores the result in the <paramref name="destination"/> key.
    /// </summary>
    /// <param name="operation">The set operation to perform (union, intersect, or difference).</param>
    /// <param name="destination">The key to store the resulting set.</param>
    /// <param name="first">The key of the first set.</param>
    /// <param name="second">The key of the second set.</param>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <returns>The number of elements in the resulting set.</returns>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<long> SetCombineAndStoreAsync(SetOperation operation, ValkeyKey destination, ValkeyKey first, ValkeyKey second, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Performs the specified <paramref name="operation"/> on the sets stored at the given <paramref name="keys"/>,
    /// and stores the result in the <paramref name="destination"/> key.
    /// </summary>
    /// <param name="operation">The set operation to perform (union, intersect, or difference).</param>
    /// <param name="destination">The key to store the resulting set.</param>
    /// <param name="keys">The keys of the sets to combine.</param>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <returns>The number of elements in the resulting set.</returns>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<long> SetCombineAndStoreAsync(SetOperation operation, ValkeyKey destination, IEnumerable<ValkeyKey> keys, CommandFlags flags = CommandFlags.None);

    /// <inheritdoc cref="IBaseClient.SetIsMemberAsync(ValkeyKey, ValkeyValue)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<bool> SetContainsAsync(ValkeyKey key, ValkeyValue value, CommandFlags flags = CommandFlags.None);

    /// <inheritdoc cref="IBaseClient.SetIsMemberAsync(ValkeyKey, IEnumerable{ValkeyValue})"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<bool[]> SetContainsAsync(ValkeyKey key, IEnumerable<ValkeyValue> values, CommandFlags flags = CommandFlags.None);

    /// <inheritdoc cref="ISetBaseCommands.SetRandomMemberAsync(ValkeyKey)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<ValkeyValue> SetRandomMemberAsync(ValkeyKey key, CommandFlags flags);

    /// <inheritdoc cref="ISetBaseCommands.SetRandomMembersAsync(ValkeyKey, long)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<ValkeyValue[]> SetRandomMembersAsync(ValkeyKey key, long count, CommandFlags flags);

    /// <inheritdoc cref="ISetBaseCommands.SetMoveAsync(ValkeyKey, ValkeyKey, ValkeyValue)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<bool> SetMoveAsync(ValkeyKey source, ValkeyKey destination, ValkeyValue value, CommandFlags flags);

    // TODO #287
    /// <inheritdoc cref="ISetBaseCommands.SetScanAsync(ValkeyKey, ValkeyValue, int, long, int)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    IAsyncEnumerable<ValkeyValue> SetScanAsync(ValkeyKey key, ValkeyValue pattern = default, int pageSize = 250, long cursor = 0, int pageOffset = 0, CommandFlags flags = CommandFlags.None);
}
