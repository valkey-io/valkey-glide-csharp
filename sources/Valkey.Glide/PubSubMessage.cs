// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using System.Text.Json;

namespace Valkey.Glide;

/// <summary>
/// Pub/sub channel subscription mode.
/// </summary>
public enum PubSubChannelMode
{
    /// <summary>Exact channel name subscription (SUBSCRIBE).</summary>
    Exact = 0,
    /// <summary>Pattern-based subscription (PSUBSCRIBE).</summary>
    Pattern = 1,
    /// <summary>Shard channel subscription (SSUBSCRIBE).</summary>
    Sharded = 2
}

/// <summary>
/// Represents a message received through PubSub subscription.
/// </summary>
public sealed class PubSubMessage
{
    /// <summary>
    /// The channel subscription mode for this message.
    /// </summary>
    public PubSubChannelMode ChannelMode { get; }

    /// <summary>
    /// The message content.
    /// </summary>
    public string Message { get; }

    /// <summary>
    /// The channel on which the message was received.
    /// </summary>
    public string Channel { get; }

    /// <summary>
    /// The pattern that matched the channel (null for exact and shard channel subscriptions).
    /// </summary>
    public string? Pattern { get; }

    /// <summary>
    /// Creates a new <see cref="PubSubMessage"/> for an exact channel subscription.
    /// </summary>
    /// <param name="message">The message content.</param>
    /// <param name="channel">The channel on which the message was received.</param>
    /// <returns>A new <see cref="PubSubMessage"/> instance.</returns>
    public static PubSubMessage FromChannel(string message, string channel)
    {
        return new(PubSubChannelMode.Exact, message, channel, null);
    }

    /// <summary>
    /// Creates a new <see cref="PubSubMessage"/> for a pattern-based subscription.
    /// </summary>
    /// <param name="message">The message content.</param>
    /// <param name="channel">The channel on which the message was received.</param>
    /// <param name="pattern">The pattern that matched the channel.</param>
    /// <returns>A new <see cref="PubSubMessage"/> instance.</returns>
    public static PubSubMessage FromPattern(string message, string channel, string pattern)
    {
        ArgumentNullException.ThrowIfNullOrWhiteSpace(pattern, nameof(pattern));
        return new(PubSubChannelMode.Pattern, message, channel, pattern);
    }

    /// <summary>
    /// Creates a new <see cref="PubSubMessage"/> for a shard channel subscription.
    /// </summary>
    /// <param name="message">The message content.</param>
    /// <param name="channel">The channel on which the message was received.</param>
    /// <returns>A new <see cref="PubSubMessage"/> instance.</returns>
    public static PubSubMessage FromShardChannel(string message, string channel)
    {
        return new(PubSubChannelMode.Sharded, message, channel, null);
    }

    /// <summary>
    /// Returns a JSON string representation of the PubSub message for debugging purposes.
    /// </summary>
    /// <returns>A JSON representation of the message.</returns>
    public override string ToString()
    {
        var messageObject = new
        {
            ChannelMode = ChannelMode.ToString(),
            Message,
            Channel,
            Pattern
        };

        return JsonSerializer.Serialize(messageObject, new JsonSerializerOptions
        {
            WriteIndented = false
        });
    }

    /// <summary>
    /// Determines whether the specified object is equal to the current PubSubMessage.
    /// </summary>
    /// <param name="obj">The object to compare with the current PubSubMessage.</param>
    /// <returns>True if the specified object is equal to the current PubSubMessage; otherwise, false.</returns>
    public override bool Equals(object? obj) =>
        obj is PubSubMessage other &&
        ChannelMode == other.ChannelMode &&
        Message == other.Message &&
        Channel == other.Channel &&
        Pattern == other.Pattern;

    /// <summary>
    /// Returns the hash code for this PubSubMessage.
    /// </summary>
    /// <returns>A hash code for the current PubSubMessage.</returns>
    public override int GetHashCode() => HashCode.Combine(ChannelMode, Message, Channel, Pattern);

    /// <summary>
    /// Initializes a new instance of the <see cref="PubSubMessage"/> class.
    /// </summary>
    private PubSubMessage(PubSubChannelMode channelMode, string message, string channel, string? pattern)
    {
        ArgumentNullException.ThrowIfNullOrWhiteSpace(message, nameof(message));
        ArgumentNullException.ThrowIfNullOrWhiteSpace(channel, nameof(channel));

        ChannelMode = channelMode;
        Message = message;
        Channel = channel;
        Pattern = pattern;
    }
}
