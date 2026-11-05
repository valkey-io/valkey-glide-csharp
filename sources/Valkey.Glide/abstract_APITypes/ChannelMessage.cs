// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide;

/// <summary>
/// Represents a message that is broadcast via publish/subscribe.
/// Compatible with StackExchange.Redis <c>ChannelMessage</c>.
/// </summary>
public readonly struct ChannelMessage
{
    #region Private Fields

    private readonly ChannelMessageQueue _queue;

    #endregion
    #region Constructors

    internal ChannelMessage(ChannelMessageQueue queue, in ValkeyChannel channel, in ValkeyValue value)
    {
        _queue = queue;
        Channel = channel;
        Message = value;
    }

    #endregion
    #region Public Properties

    /// <summary>
    /// The channel that the subscription was created from.
    /// </summary>
    public ValkeyChannel SubscriptionChannel => _queue.Channel;

    /// <summary>
    /// The channel that the message was broadcast to.
    /// </summary>
    public ValkeyChannel Channel { get; }

    /// <summary>
    /// The value that was broadcast.
    /// </summary>
    public ValkeyValue Message { get; }

    #endregion
    #region Operators

    /// <summary>
    /// Returns whether the given messages are equal.
    /// </summary>
    /// <param name="left">The first message to compare.</param>
    /// <param name="right">The second message to compare.</param>
    public static bool operator ==(ChannelMessage left, ChannelMessage right) => left.Equals(right);

    /// <summary>
    /// Returns whether the given messages are not equal.
    /// </summary>
    /// <param name="left">The first message to compare.</param>
    /// <param name="right">The second message to compare.</param>
    public static bool operator !=(ChannelMessage left, ChannelMessage right) => !left.Equals(right);

    #endregion
    #region Public Methods

    /// <inheritdoc/>
    public override string ToString() => ((string?)Channel) + ":" + ((string?)Message);

    /// <inheritdoc/>
    public override int GetHashCode() => Channel.GetHashCode() ^ Message.GetHashCode();

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is ChannelMessage cm
        && cm.Channel == Channel
        && cm.Message == Message;

    #endregion
}
