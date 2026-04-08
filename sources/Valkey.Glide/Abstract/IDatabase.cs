// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide;

/// <summary>
/// Describes functionality that is common to both standalone and cluster servers.<br />
/// See also <see cref="GlideClient" /> and <see cref="GlideClusterClient" />.
/// </summary>
public interface IDatabase : IDatabaseAsync
{
    /// <summary>
    /// Allows creation of a group of operations that will be sent to the server as a single unit,
    /// but which may or may not be processed on the server contiguously.
    /// </summary>
    /// <returns>The created batch.</returns>
    /// <remarks>Unlike StackExchange.Redis, GLIDE does not support <c>asyncState</c>.</remarks>
    IBatch CreateBatch();

    /// <summary>
    /// Allows creation of a group of operations that will be sent to the server as a single unit,
    /// and processed on the server as a single unit.
    /// </summary>
    /// <returns>The created transaction.</returns>
    /// <remarks>Unlike StackExchange.Redis, GLIDE does not support <c>asyncState</c>.</remarks>
    ITransaction CreateTransaction();
}
