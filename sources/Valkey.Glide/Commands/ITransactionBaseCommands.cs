// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.Commands;

/// <summary>
/// Supports commands for the "Transaction Commands" group for standalone and cluster clients.
/// <br />
/// See more on <see href="https://valkey.io/commands/?group=transactions">valkey.io</see>.
/// </summary>
public interface ITransactionBaseCommands
{
    /// <summary>
    /// Marks the given keys to be watched for conditional execution of a transaction. Transactions
    /// will only execute commands if the watched keys are not modified before execution of the
    /// transaction. Keys that do not exist are watched as if they were empty.
    /// </summary>
    /// <param name="keys">The keys to watch.</param>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    /// <exception cref="RequestException">Thrown if the command fails to execute on the server.</exception>
    /// <remarks>
    /// <para>
    /// In cluster mode, if keys in <paramref name="keys"/> map to different hash slots, the command
    /// will be split across these slots and executed separately for each. This means the command
    /// is atomic only at the slot level. If one or more slot-specific requests fail, the entire
    /// call will return the first encountered error, even though some requests may have succeeded
    /// while others did not. If this behavior impacts your application logic, consider splitting
    /// the request into sub-requests per slot to ensure atomicity.
    /// </para>
    /// <example>
    /// <code>
    /// await client.WatchAsync(["sampleKey"]);
    /// 
    /// // Execute transaction
    /// var batch = new Batch(true)
    ///     .StringSetAsync("sampleKey", "foobar");
    /// object[] transactionResult = await client.Exec(batch, false);
    /// // transactionResult is not null if transaction executed successfully
    /// 
    /// // Watch key again
    /// await client.WatchAsync(["sampleKey"]);
    /// var batch2 = new Batch(true)
    ///     .StringSetAsync("sampleKey", "foobar");
    /// // Modify the watched key from another client/connection
    /// await client.StringSetAsync("sampleKey", "hello world");
    /// object[] transactionResult2 = await client.Exec(batch2, true);
    /// // transactionResult2 is null because the watched key was modified
    /// </code>
    /// </example>
    /// </remarks>
    /// <seealso href="https://valkey.io/commands/watch/"/>
    Task WatchAsync(ValkeyKey[] keys, CommandFlags flags = CommandFlags.None);
}
