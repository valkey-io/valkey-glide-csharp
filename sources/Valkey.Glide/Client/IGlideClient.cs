// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands;

namespace Valkey.Glide;

// ATTENTION: Methods should only be added to this interface if they are implemented by Valkey GLIDE clients
// but NOT by StackExchange.Redis databases. Methods implemented by both should be added to the corresponding
// Commands interface instead.

/// <summary>
/// Interface for Valkey GLIDE standalone client.
/// </summary>
public partial interface IGlideClient :
    IBaseClient,
    IGenericCommands,
    IServerManagementCommands
{
    /// <summary>
    /// Moves key from the currently selected database to the specified destination database.
    /// When key already exists in the destination database, or it does not exist in the source database, it does nothing.
    /// It is possible to use MOVE as a locking primitive because of this.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/move"/>
    /// <param name="key">The key to move.</param>
    /// <param name="database">The database to move the key to.</param>
    /// <returns><see langword="true"/> if key was moved. <see langword="false"/> if key was not moved.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// bool result = await client.MoveAsync(key, 2);
    /// </code>
    /// </example>
    /// </remarks>
    Task<bool> MoveAsync(ValkeyKey key, int database);

    /// <summary>
    /// Copies the value stored at the source to the destination key in the specified database. When
    /// replace is true, removes the destination key first if it already
    /// exists, otherwise performs no action.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/copy"/>
    /// <note>Since Valkey 6.2.0 and above.</note>
    /// <param name="source">The key to the source value.</param>
    /// <param name="destination">The key where the value should be copied to.</param>
    /// <param name="destinationDatabase">The database ID to store destination in.</param>
    /// <param name="replace">Whether to overwrite an existing values at destination.</param>
    /// <returns><see langword="true"/> if source was copied. <see langword="false"/> if source was not copied.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// bool result = await client.CopyAsync(source, dest, 1, replace: true);
    /// </code>
    /// </example>
    /// </remarks>
    Task<bool> CopyAsync(ValkeyKey source, ValkeyKey destination, int destinationDatabase, bool replace = false);
}
