// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands.Options;
using Valkey.Glide.Pipeline;

using static Valkey.Glide.Errors;
using static Valkey.Glide.Pipeline.Options;

namespace Valkey.Glide.Commands;

// ATTENTION: Methods should only be added to this interface if they are implemented
// by both Valkey GLIDE clients and StackExchange.Redis databases.

/// <summary>
/// Generic commands for standalone clients.
/// </summary>
/// <seealso href="https://valkey.io/commands/#generic">Valkey – Generic Commands</seealso>
public interface IGenericCommands
{
    /// <summary>
    /// Executes a single command, without checking inputs. Every part of the command, including subcommands,
    /// should be added as a separate value in <paramref name="args" />.
    /// The command will be routed automatically based on the command's default request policy.
    /// <para />
    /// This function should only be used for single-response commands. Commands that don't return complete response and awaits
    /// (such as SUBSCRIBE); that return potentially more than a single response (such as XREAD); or that change the client's
    /// behavior (such as entering pub/sub mode on RESP2 connections) shouldn't be called using this function.
    /// </summary>
    /// <example>
    /// <code>
    /// // Query all pub/sub clients
    /// object result = await client.CustomCommand(["CLIENT", "LIST", "TYPE", "PUBSUB"]);
    /// </code>
    /// </example>
    /// <remarks>
    /// This API returns all <see langword="string" /> data as <see cref="GlideString" />.
    /// </remarks>
    /// <param name="args">A list includes the command name and arguments for the custom command.</param>
    /// <returns>The returning value depends on the executed command.</returns>
    Task<object?> CustomCommand(IEnumerable<GlideString> args);

    /// <summary>
    /// Executes a batch by processing the queued commands.
    /// <para />
    /// See the <see href="https://valkey.io/topics/transactions/">Valkey Transactions (Atomic Batches)</see>.<br />
    /// See the <see href="https://valkey.io/topics/pipelining/">Valkey Pipelines (Non-Atomic Batches)</see>.
    /// </summary>
    /// <remarks>
    /// <b>Behavior notes:</b><br />
    /// <b>Atomic Batches (Transactions):</b> If a transaction fails due to a <c>WATCH</c> command,
    /// <c>Exec</c> will return <see langword="null" />.
    /// <example>
    /// <code>
    /// // Example 1: Atomic Batch (Transaction)
    /// Batch batch = new Batch(true) // Atomic (Transaction)
    ///     .Set("key", "1")
    ///     .Incr("key")
    ///     .Get("key");
    ///
    /// var result = await client.Exec(batch, true);
    /// // Expected result: ["OK", 2, 2]
    /// </code>
    /// </example>
    /// <example>
    /// <code>
    /// // Example 2: Non-Atomic Batch (Pipeline)
    /// Batch batch = new Batch(false) // Non-Atomic (Pipeline)
    ///     .Set("key1", "value1")
    ///     .Set("key2", "value2")
    ///     .Get("key1")
    ///     .Get("key2");
    ///
    /// var result = await client.Exec(batch, true);
    /// // Expected result: ["OK", "OK", "value1", "value2"]
    /// </code>
    /// </example>
    /// </remarks>
    /// <param name="batch">A <see cref="Batch" /> object containing a list of commands to be executed.</param>
    /// <param name="raiseOnError">
    /// Determines how errors are handled within the batch response.
    /// <para />
    /// When set to <see langword="true" />, the first encountered error in the batch will be raised as an
    /// exception of type <see cref="RequestException" /> after all retries and reconnections have been
    /// executed.
    /// <para />
    /// When set to <see langword="false" />, errors will be included as part of the batch response, allowing
    /// the caller to process both successful and failed commands together. In this case, error details
    /// will be provided as instances of <see cref="RequestException" />.
    /// </param>
    /// <returns>An array of results, where each entry corresponds to a command’s execution result.</returns>
    Task<object?[]?> Exec(Batch batch, bool raiseOnError);

    /// <summary>
    /// Executes a batch by processing the queued commands.
    /// <para />
    /// <b>Routing Behavior:</b>
    /// <list type="bullet">
    ///   <item>
    ///     If a <see cref="Route" /> is specified in <see cref="ClusterBatchOptions" />, the entire batch is sent
    ///     to the specified node.
    ///   </item>
    ///   <item>
    ///     If no <see cref="Route" /> is specified:
    ///     <list type="bullet">
    ///       <item>
    ///         <b>Atomic batches (Transactions):</b> Routed to the slot owner of the
    ///         first key in the batch. If no key is found, the request is sent to a random node.
    ///       </item>
    ///       <item>
    ///         <b>Non-atomic batches (Pipelines):</b> Each command is routed to the node
    ///         owning the corresponding key's slot. If no key is present, routing follows the
    ///         command's request policy. Multi-node commands are automatically split and
    ///         dispatched to the appropriate nodes.
    ///       </item>
    ///     </list>
    ///   </item>
    /// </list>
    /// See the <see href="https://valkey.io/topics/transactions/">Valkey Transactions (Atomic Batches)</see>.<br />
    /// See the <see href="https://valkey.io/topics/pipelining/">Valkey Pipelines (Non-Atomic Batches)</see>.
    /// </summary>
    /// <remarks>
    /// <b>Behavior notes:</b><br />
    /// <b>Atomic Batches (Transactions):</b> If a transaction fails due to a <c>WATCH</c> command,
    /// <c>Exec</c> will return <see langword="null" />.
    /// <example>
    /// <code>
    /// // Example 1: Atomic Batch (Transaction) all keys must share the same hash slot
    /// BatchOptions options = new(
    ///     timeout: 1000, // Set a timeout of 1000 milliseconds
    ///     raiseOnError: false); // Do not raise an error on failure
    ///
    /// Batch batch = new Batch(true) // Atomic (Transaction)
    ///     .Set("key", "1")
    ///     .Incr("key")
    ///     .Get("key");
    ///
    /// var result = await client.Exec(batch, false, options);
    /// // Expected result: ["OK", 2, 2]
    /// </code>
    /// </example>
    /// <example>
    /// <code>
    /// // Example 2: Non-Atomic Batch (Pipeline)
    /// BatchOptions options = new(
    ///     timeout: 1000, // Set a timeout of 1000 milliseconds
    ///     raiseOnError: false); // Do not raise an error on failure
    ///
    /// Batch batch = new Batch(false) // Non-Atomic (Pipeline) keys may span different hash slots
    ///     .Set("key1", "value1")
    ///     .Set("key2", "value2")
    ///     .Get("key1")
    ///     .Get("key2");
    ///
    /// var result = await client.Exec(batch, false, options);
    /// // Expected result: ["OK", "OK", "value1", "value2"]
    /// </code>
    /// </example>
    /// </remarks>
    /// <param name="batch">A <see cref="Batch" /> object containing a list of commands to be executed.</param>
    /// <param name="raiseOnError">
    /// Determines how errors are handled within the batch response.
    /// <para />
    /// When set to <see langword="true" />, the first encountered error in the batch will be raised as an
    /// exception of type <see cref="RequestException" /> after all retries and reconnections have been
    /// executed.
    /// <para />
    /// When set to <see langword="false" />, errors will be included as part of the batch response, allowing
    /// the caller to process both successful and failed commands together. In this case, error details
    /// will be provided as instances of <see cref="RequestException" />.
    /// </param>
    /// <param name="options">A <see cref="BatchOptions" /> object containing execution options.</param>
    /// <returns>
    /// An array of results, where each entry corresponds to a command’s execution result
    /// or <see langword="null" /> if a transaction failed due to a <c>WATCH</c> command.
    /// </returns>
    Task<object?[]?> Exec(Batch batch, bool raiseOnError, BatchOptions options);

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
    /// <param name="sourceKey">The key to the source value.</param>
    /// <param name="destinationKey">The key where the value should be copied to.</param>
    /// <param name="destinationDatabase">The database ID to store destinationKey in.</param>
    /// <param name="replace">Whether to overwrite an existing values at destinationKey.</param>
    /// <returns><see langword="true"/> if sourceKey was copied. <see langword="false"/> if sourceKey was not copied.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// bool result = await client.CopyAsync(sourceKey, destKey, 1, replace: true);
    /// </code>
    /// </example>
    /// </remarks>
    Task<bool> CopyAsync(ValkeyKey sourceKey, ValkeyKey destinationKey, int destinationDatabase, bool replace = false);

    /// <summary>
    /// Incrementally iterates over the matching keys in the database.
    /// <para>
    /// The SCAN command is a cursor-based iterator. An iteration starts when the cursor
    /// is set to <c>"0"</c>. At every call of the command, the
    /// server returns an updated cursor that the user needs to use as the cursor argument in the next
    /// call. The iteration terminates when the cursor is <c>"0"</c>.
    /// </para>
    /// </summary>
    /// <param name="cursor">The cursor for iteration.</param>
    /// <param name="options">Optional scan options for filtering results.</param>
    /// <returns>The next cursor and an array of matching keys.</returns>
    /// <example>
    /// <code>
    /// var allKeys = new List&lt;ValkeyKey&gt;();
    /// string cursor = "0";
    ///
    /// do
    /// {
    ///     (cursor, var keys) = await client.ScanAsync(cursor);
    ///     allKeys.AddRange(keys);
    /// } while (cursor != "0");
    /// </code>
    /// </example>
    /// <seealso href="https://valkey.io/commands/scan/">SCAN command</seealso>
    Task<(string cursor, ValkeyKey[] keys)> ScanAsync(string cursor, ScanOptions? options = null);
}
