// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide;

/// <summary>
/// A pending message from a <c>XPENDING</c> response.
/// </summary>
/// <seealso href="https://valkey.io/commands/xpending/"/>
public readonly struct StreamPendingMessageInfo
{
    #region Public Properties

    /// <summary>
    /// The ID of the pending message.
    /// </summary>
    public ValkeyValue MessageId { get; }

    /// <summary>
    /// The consumer that received the pending message.
    /// </summary>
    public ValkeyValue ConsumerName { get; }

    /// <summary>
    /// The time that has passed since the message was last delivered to a consumer.
    /// </summary>
    /// <remarks>Valkey GLIDE only.</remarks>
    public TimeSpan Idle { get; }

    /// <summary>
    /// The number of times the message has been delivered to a consumer.
    /// </summary>
    public int DeliveryCount { get; }

    /// <summary>
    /// The number of milliseconds that has passed since the message was last delivered to a consumer.
    /// </summary>
    public long IdleTimeInMilliseconds => (long)Idle.TotalMilliseconds;

    #endregion
    #region Constructors

    internal StreamPendingMessageInfo(ValkeyValue messageId, ValkeyValue consumerName, TimeSpan idle, int deliveryCount)
    {
        MessageId = messageId;
        ConsumerName = consumerName;
        Idle = idle;
        DeliveryCount = deliveryCount;
    }

    #endregion
}
