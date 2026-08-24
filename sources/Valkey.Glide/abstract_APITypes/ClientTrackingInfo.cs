// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide;

/// <summary>
/// Represents a <see href="https://valkey.io/commands/client-trackinginfo/">CLIENT TRACKINGINFO</see> response.
/// </summary>
/// <seealso href="https://valkey.io/commands/client-trackinginfo/"/>
public sealed record ClientTrackingInfo
{
    #region Public Properties

    /// <summary>
    /// The set of tracking flags.
    /// </summary>
    public required IReadOnlySet<string> Flags { get; init; }

    /// <summary>
    /// The client ID receiving invalidation messages, or <c>-1</c> if not redirecting.
    /// </summary>
    public required long Redirect { get; init; }

    /// <summary>
    /// The set of key prefixes monitored for invalidation.
    /// </summary>
    public required IReadOnlySet<string> Prefixes { get; init; }

    #endregion
    #region Constructors & Builders

    internal ClientTrackingInfo() { }

    #endregion
}
