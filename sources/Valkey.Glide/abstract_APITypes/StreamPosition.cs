// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide;

/// <summary>
/// Describes a pair consisting of the Stream Key and the <see cref="Position"/> from which to begin reading a stream.
/// </summary>
public struct StreamPosition
{
    #region Constants

    /// <summary>
    /// The beginning of the stream ("0-0"). Valid for XREAD, XREADGROUP, and XAUTOCLAIM.
    /// </summary>
    public static readonly ValkeyValue Beginning = ValkeyLiterals.StreamMinimumId;

    /// <summary>
    /// Only messages not yet delivered to any consumer in the group (">"). Valid for XREADGROUP.
    /// </summary>
    public static readonly ValkeyValue UndeliveredMessages = ValkeyLiterals.StreamUndeliveredMessages;

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
}
