// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using System.Text.Json;

namespace Valkey.Glide;

/// <summary>
/// Represents a message received through PubSub subscription.
/// </summary>
public sealed class PubSubMessage
{
    /// <summary>
    /// The message content.
    /// </summary>
    public string Message { get; }

    /// <summary>
    /// The channel on which the message was received.
    /// </summary>
    public string Channel { get; }

    /// <summary>
    /// The pattern that matched the channel (null for exact channel subscriptions).
    /// </summary>
    public string? Pattern { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="PubSubMessage"/> class for exact channel subscriptions.
    /// </summary>
    /// <param name="message">The message content.</param>
    /// <param name="channel">The channel on which the message was received.</param>
    /// <exception cref="ArgumentNullException">Thrown when message or channel is null.</exception>
    /// <exception cref="ArgumentException">Thrown when message or channel is empty.</exception>
    public PubSubMessage(string message, string channel)
    {
        if (message == null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        if (channel == null)
        {
            throw new ArgumentNullException(nameof(channel));
        }

        if (string.IsNullOrEmpty(message))
        {
            throw new ArgumentException("Message cannot be empty", nameof(message));
        }

        if (string.IsNullOrEmpty(channel))
        {
            throw new ArgumentException("Channel cannot be empty", nameof(channel));
        }

        Message = message;
        Channel = channel;
        Pattern = null;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PubSubMessage"/> class for pattern-based subscriptions.
    /// </summary>
    /// <param name="message">The message content.</param>
    /// <param name="channel">The channel on which the message was received.</param>
    /// <param name="pattern">The pattern that matched the channel.</param>
    /// <exception cref="ArgumentNullException">Thrown when message, channel, or pattern is null.</exception>
    /// <exception cref="ArgumentException">Thrown when message, channel, or pattern is empty.</exception>
    public PubSubMessage(string message, string channel, string pattern)
    {
        if (message == null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        if (channel == null)
        {
            throw new ArgumentNullException(nameof(channel));
        }

        if (pattern == null)
        {
            throw new ArgumentNullException(nameof(pattern));
        }

        if (string.IsNullOrEmpty(message))
        {
            throw new ArgumentException("Message cannot be empty", nameof(message));
        }

        if (string.IsNullOrEmpty(channel))
        {
            throw new ArgumentException("Channel cannot be empty", nameof(channel));
        }

        if (string.IsNullOrEmpty(pattern))
        {
            throw new ArgumentException("Pattern cannot be empty", nameof(pattern));
        }

        Message = message;
        Channel = channel;
        Pattern = pattern;
    }

    /// <summary>
    /// Returns a JSON string representation of the PubSub message for debugging purposes.
    /// </summary>
    /// <returns>A JSON representation of the message.</returns>
    public override string ToString()
    {
        var messageObject = new
        {
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
    /// <returns>true if the specified object is equal to the current PubSubMessage; otherwise, false.</returns>
    public override bool Equals(object? obj) =>
        obj is PubSubMessage other &&
        Message == other.Message &&
        Channel == other.Channel &&
        Pattern == other.Pattern;

    /// <summary>
    /// Returns the hash code for this PubSubMessage.
    /// </summary>
    /// <returns>A hash code for the current PubSubMessage.</returns>
    public override int GetHashCode() => HashCode.Combine(Message, Channel, Pattern);
}
