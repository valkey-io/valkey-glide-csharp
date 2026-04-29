// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

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
    /// Executes a single command without checking inputs. Every part of the command, including subcommands,
    /// should be added as a separate value in <paramref name="args" />.
    /// The command is routed automatically based on the command's default request policy.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/">Valkey commands</seealso>
    /// <param name="args">A list includes the command name and arguments for the custom command.</param>
    /// <returns>The returning value depends on the executed command.</returns>
    /// <remarks>
    /// This API returns all <see langword="string" /> data as <see cref="GlideString" />.
    /// <para />
    /// This function should only be used for single-response commands. Commands that don't return complete response and awaits
    /// (such as SUBSCRIBE); that return potentially more than a single response (such as XREAD); or that change the client's
    /// behavior (such as entering pub/sub mode on RESP2 connections) shouldn't be called using this function.
    /// <example>
    /// <code>
    /// var result = await client.CustomCommand(["CLIENT", "LIST", "TYPE", "PUBSUB"]);
    /// </code>
    /// </example>
    /// </remarks>
    Task<object?> CustomCommand(IEnumerable<GlideString> args);

    /// <summary>
    /// Executes a batch by processing the queued commands.
    /// </summary>
    /// <seealso href="https://valkey.io/topics/transactions/">Valkey Transactions (Atomic Batches)</seealso>
    /// <seealso href="https://valkey.io/topics/pipelining/">Valkey Pipelines (Non-Atomic Batches)</seealso>
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
    /// <returns>An array of results, where each entry corresponds to a command's execution result.</returns>
    /// <remarks>
    /// <b>Atomic Batches (Transactions):</b> If a transaction fails due to a <c>WATCH</c> command,
    /// <c>Exec</c> will return <see langword="null" />.
    /// <example>
    /// <code>
    /// // Example 1: Atomic Batch (Transaction)
    /// var batch = new Batch(true) // Atomic (Transaction)
    ///     .SetAsync("key", "1")
    ///     .IncrementAsync("key")
    ///     .GetAsync("key");
    ///
    /// var result = await client.Exec(batch, true);
    /// // Expected result: ["OK", 2, 2]
    /// </code>
    /// </example>
    /// <example>
    /// <code>
    /// // Example 2: Non-Atomic Batch (Pipeline)
    /// var batch = new Batch(false) // Non-Atomic (Pipeline)
    ///     .SetAsync("key1", "value1")
    ///     .SetAsync("key2", "value2")
    ///     .GetAsync("key1")
    ///     .GetAsync("key2");
    ///
    /// var result = await client.Exec(batch, true);
    /// // Expected result: ["OK", "OK", "value1", "value2"]
    /// </code>
    /// </example>
    /// </remarks>
    Task<object?[]?> Exec(Batch batch, bool raiseOnError);

    /// <summary>
    /// Executes a batch by processing the queued commands with additional options.
    /// </summary>
    /// <seealso href="https://valkey.io/topics/transactions/">Valkey Transactions (Atomic Batches)</seealso>
    /// <seealso href="https://valkey.io/topics/pipelining/">Valkey Pipelines (Non-Atomic Batches)</seealso>
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
    /// An array of results, where each entry corresponds to a command's execution result
    /// or <see langword="null" /> if a transaction failed due to a <c>WATCH</c> command.
    /// </returns>
    /// <remarks>
    /// <b>Atomic Batches (Transactions):</b> If a transaction fails due to a <c>WATCH</c> command,
    /// <c>Exec</c> will return <see langword="null" />.
    /// <example>
    /// <code>
    /// // Example 1: Atomic Batch (Transaction)
    /// Pipeline.Options.BatchOptions options = new(
    ///     timeout: 1000); // Set a timeout of 1000 milliseconds
    ///
    /// var batch = new Batch(true) // Atomic (Transaction)
    ///     .SetAsync("key", "1")
    ///     .IncrementAsync("key")
    ///     .GetAsync("key");
    ///
    /// var result = await client.Exec(batch, false, options);
    /// // Expected result: ["OK", 2, 2]
    /// </code>
    /// </example>
    /// <example>
    /// <code>
    /// // Example 2: Non-Atomic Batch (Pipeline)
    /// Pipeline.Options.BatchOptions options = new(
    ///     timeout: 1000); // Set a timeout of 1000 milliseconds
    ///
    /// var batch = new Batch(false) // Non-Atomic (Pipeline) keys may span different hash slots
    ///     .SetAsync("key1", "value1")
    ///     .SetAsync("key2", "value2")
    ///     .GetAsync("key1")
    ///     .GetAsync("key2");
    ///
    /// var result = await client.Exec(batch, false, options);
    /// // Expected result: ["OK", "OK", "value1", "value2"]
    /// </code>
    /// </example>
    /// </remarks>
    Task<object?[]?> Exec(Batch batch, bool raiseOnError, BatchOptions options);
}
