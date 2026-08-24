// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide;

/// <summary>
/// A stream key and starting position for the <c>XREAD</c> command.
/// </summary>
/// <seealso href="https://valkey.io/commands/xread/"/>
public readonly struct StreamPosition
{
    #region Constants

    /// <summary>
    /// The beginning of the stream ("0-0").
    /// Valid for XREAD, XREADGROUP, and XAUTOCLAIM.
    /// </summary>
    public static readonly ValkeyValue Beginning = ValkeyLiterals.StreamMinimumId;

    /// <summary>
    /// Only messages not yet delivered to any consumer in the group (">").
    /// Valid for XREADGROUP.
    /// </summary>
    public static readonly ValkeyValue UndeliveredMessages = ValkeyLiterals.StreamUndeliveredMessages;

    /// <summary>
    /// Only new messages arriving after this point ("$").
    /// Valid for XREAD, XGROUP CREATE, and XGROUP SETID.
    /// </summary>
    public static readonly ValkeyValue NewMessages = ValkeyLiterals.StreamNewMessages;

    #endregion
    #region Public Properties

    /// <summary>
    /// The stream key.
    /// </summary>
    public ValkeyKey Key { get; }

    /// <summary>
    /// The offset at which to begin reading the stream.
    /// </summary>
    public ValkeyValue Position { get; }

    #endregion
    #region Constructors

    /// <summary>
    /// Initializes a <see cref="StreamPosition"/> value.
    /// </summary>
    /// <param name="key">The key for the stream.</param>
    /// <param name="position">The position from which to begin reading the stream.</param>
    public StreamPosition(ValkeyKey key, ValkeyValue position)
    {
        Key = key;
        Position = position;
    }

    #endregion
}
