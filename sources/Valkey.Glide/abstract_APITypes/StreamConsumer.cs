// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide;

/// <summary>
/// Represents a stream consumer response from <c>XAUTOCLAIM</c> command.
/// </summary>
/// <seealso href="https://valkey.io/commands/xpending/"/>
public readonly struct StreamConsumer
{
    #region Public Properties

    /// <summary>
    /// The name of the consumer.
    /// </summary>
    public ValkeyValue Name { get; }

    /// <summary>
    /// The number of messages that have been delivered by not yet acknowledged by the consumer.
    /// </summary>
    public int PendingMessageCount { get; }

    #endregion
    #region Constructors

    internal StreamConsumer(ValkeyValue name, int pendingMessageCount)
    {
        Name = name;
        PendingMessageCount = pendingMessageCount;
    }

    #endregion
}
