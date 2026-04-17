// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.Pipeline;

/// <summary>
/// Batch implementation for standalone <see cref="GlideClient" />. Batches allow the execution of a group
/// of commands in a single step.
/// <para />
/// Batch Response: An <c>array</c> of command responses is returned by the client <see cref="GlideClient.Exec(Batch, bool)" />
/// and <see cref="GlideClient.Exec(Batch, bool, Options.BatchOptions)" /> API, in the order they were given. Each element
/// in the array represents a command given to the <c>Batch</c>. The response for each command depends on the executed
/// Valkey command. Specific response types are documented alongside each method.
/// <para />
/// See the <see href="https://valkey.io/topics/transactions/">Valkey Transactions (Atomic Batches)</see>.<br />
/// See the <see href="https://valkey.io/topics/pipelining/">Valkey Pipelines (Non-Atomic Batches)</see>.
/// </summary>
/// <remarks>
/// Standalone Batches are executed on the primary node.
/// <inheritdoc cref="GlideClient.Exec(Batch, bool)" path="/remarks/example" />
/// </remarks>
/// <param name="isAtomic">
/// <inheritdoc cref="BaseBatch{T}.BaseBatch(bool)" />
/// </param>
/// <seealso href="https://glide.valkey.io/how-to/send-batch-commands/">Valkey GLIDE – Send Batch Commands</seealso>
public sealed class Batch(bool isAtomic) : BaseBatch<Batch>(isAtomic), IBatch, IBatchStandalone
{
    // Explicit interface implementations for IBatchStandalone
    IBatch IBatchStandalone.Move(ValkeyKey key, int database) => Move(key, database);
    IBatch IBatchStandalone.Copy(ValkeyKey source, ValkeyKey destination, int destinationDatabase, bool replace) => Copy(source, destination, destinationDatabase, replace);
    IBatch IBatchStandalone.SelectAsync(long index) => SelectAsync(index);

    // All other command implementations are inherited from BaseBatch<T> partials.
}
