// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands;

namespace Valkey.Glide;

/// <summary>
/// List commands with <see cref="CommandFlags"/> for StackExchange.Redis compatibility.
/// </summary>
/// <seealso cref="IListCommands" />
public partial interface IDatabaseAsync
{
    /// <inheritdoc cref="IListCommands.ListLeftPopAsync(ValkeyKey)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<ValkeyValue> ListLeftPopAsync(ValkeyKey key, CommandFlags flags);

    /// <inheritdoc cref="IListCommands.ListLeftPopAsync(ValkeyKey, long)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<ValkeyValue[]?> ListLeftPopAsync(ValkeyKey key, long count, CommandFlags flags);

    /// <inheritdoc cref="IListCommands.ListLeftPopAsync(IEnumerable{ValkeyKey}, long)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<ListPopResult> ListLeftPopAsync(IEnumerable<ValkeyKey> keys, long count, CommandFlags flags);

    /// <inheritdoc cref="IListCommands.ListLeftPushAsync(ValkeyKey, ValkeyValue, When)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<long> ListLeftPushAsync(ValkeyKey key, ValkeyValue value, When when = When.Always, CommandFlags flags = CommandFlags.None);

    /// <inheritdoc cref="IListCommands.ListLeftPushAsync(ValkeyKey, IEnumerable{ValkeyValue}, When)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<long> ListLeftPushAsync(ValkeyKey key, IEnumerable<ValkeyValue> values, When when = When.Always, CommandFlags flags = CommandFlags.None);

    /// <inheritdoc cref="IListCommands.ListLeftPushAsync(ValkeyKey, IEnumerable{ValkeyValue}, When)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<long> ListLeftPushAsync(ValkeyKey key, IEnumerable<ValkeyValue> values, CommandFlags flags);

    /// <inheritdoc cref="IListCommands.ListRightPopAsync(ValkeyKey)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<ValkeyValue> ListRightPopAsync(ValkeyKey key, CommandFlags flags);

    /// <inheritdoc cref="IListCommands.ListRightPopAsync(ValkeyKey, long)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<ValkeyValue[]?> ListRightPopAsync(ValkeyKey key, long count, CommandFlags flags);

    /// <inheritdoc cref="IListCommands.ListRightPopAsync(IEnumerable{ValkeyKey}, long)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<ListPopResult> ListRightPopAsync(IEnumerable<ValkeyKey> keys, long count, CommandFlags flags);

    /// <inheritdoc cref="IListCommands.ListRightPushAsync(ValkeyKey, ValkeyValue, When)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<long> ListRightPushAsync(ValkeyKey key, ValkeyValue value, When when = When.Always, CommandFlags flags = CommandFlags.None);

    /// <inheritdoc cref="IListCommands.ListRightPushAsync(ValkeyKey, IEnumerable{ValkeyValue}, When)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<long> ListRightPushAsync(ValkeyKey key, IEnumerable<ValkeyValue> values, When when = When.Always, CommandFlags flags = CommandFlags.None);

    /// <inheritdoc cref="IListCommands.ListRightPushAsync(ValkeyKey, IEnumerable{ValkeyValue}, When)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<long> ListRightPushAsync(ValkeyKey key, IEnumerable<ValkeyValue> values, CommandFlags flags);

    /// <inheritdoc cref="IListCommands.ListLengthAsync(ValkeyKey)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<long> ListLengthAsync(ValkeyKey key, CommandFlags flags);

    /// <inheritdoc cref="IListCommands.ListRemoveAsync(ValkeyKey, ValkeyValue, long)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<long> ListRemoveAsync(ValkeyKey key, ValkeyValue value, long count = 0, CommandFlags flags = CommandFlags.None);

    /// <inheritdoc cref="IListCommands.ListTrimAsync(ValkeyKey, long, long)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task ListTrimAsync(ValkeyKey key, long start, long stop, CommandFlags flags);

    /// <inheritdoc cref="IListCommands.ListRangeAsync(ValkeyKey, long, long)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<ValkeyValue[]> ListRangeAsync(ValkeyKey key, long start, long stop, CommandFlags flags);

    /// <inheritdoc cref="IListCommands.ListGetByIndexAsync(ValkeyKey, long)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<ValkeyValue> ListGetByIndexAsync(ValkeyKey key, long index, CommandFlags flags);

    /// <inheritdoc cref="IListCommands.ListInsertAfterAsync(ValkeyKey, ValkeyValue, ValkeyValue)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<long> ListInsertAfterAsync(ValkeyKey key, ValkeyValue pivot, ValkeyValue value, CommandFlags flags);

    /// <inheritdoc cref="IListCommands.ListInsertBeforeAsync(ValkeyKey, ValkeyValue, ValkeyValue)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<long> ListInsertBeforeAsync(ValkeyKey key, ValkeyValue pivot, ValkeyValue value, CommandFlags flags);

    /// <inheritdoc cref="IListCommands.ListMoveAsync(ValkeyKey, ValkeyKey, ListSide, ListSide)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<ValkeyValue> ListMoveAsync(ValkeyKey sourceKey, ValkeyKey destinationKey, ListSide sourceSide, ListSide destinationSide, CommandFlags flags);

    /// <inheritdoc cref="IListCommands.ListPositionAsync(ValkeyKey, ValkeyValue, long, long)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<long> ListPositionAsync(ValkeyKey key, ValkeyValue element, long rank = 1, long maxLength = 0, CommandFlags flags = CommandFlags.None);

    /// <inheritdoc cref="IListCommands.ListPositionsAsync(ValkeyKey, ValkeyValue, long, long, long)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<long[]> ListPositionsAsync(ValkeyKey key, ValkeyValue element, long count, long rank = 1, long maxLength = 0, CommandFlags flags = CommandFlags.None);

    /// <inheritdoc cref="IListCommands.ListSetByIndexAsync(ValkeyKey, long, ValkeyValue)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task ListSetByIndexAsync(ValkeyKey key, long index, ValkeyValue value, CommandFlags flags);

    /// <inheritdoc cref="IListCommands.ListBlockingLeftPopAsync(IEnumerable{ValkeyKey}, TimeSpan)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<ValkeyValue[]?> ListBlockingLeftPopAsync(IEnumerable<ValkeyKey> keys, TimeSpan timeout, CommandFlags flags);

    /// <inheritdoc cref="IListCommands.ListBlockingRightPopAsync(IEnumerable{ValkeyKey}, TimeSpan)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<ValkeyValue[]?> ListBlockingRightPopAsync(IEnumerable<ValkeyKey> keys, TimeSpan timeout, CommandFlags flags);

    /// <inheritdoc cref="IListCommands.ListBlockingMoveAsync(ValkeyKey, ValkeyKey, ListSide, ListSide, TimeSpan)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<ValkeyValue> ListBlockingMoveAsync(ValkeyKey source, ValkeyKey destination, ListSide sourceSide, ListSide destinationSide, TimeSpan timeout, CommandFlags flags);

    /// <inheritdoc cref="IListCommands.ListBlockingPopAsync(IEnumerable{ValkeyKey}, ListSide, TimeSpan)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<ListPopResult> ListBlockingPopAsync(IEnumerable<ValkeyKey> keys, ListSide side, TimeSpan timeout, CommandFlags flags);

    /// <inheritdoc cref="IListCommands.ListBlockingPopAsync(IEnumerable{ValkeyKey}, ListSide, long, TimeSpan)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<ListPopResult> ListBlockingPopAsync(IEnumerable<ValkeyKey> keys, ListSide side, long count, TimeSpan timeout, CommandFlags flags);
}
