// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide;

/// <summary>
/// Represents a <c>CLIENT TRACKINGINFO</c> response containing the client's tracking state.
/// </summary>
/// <seealso href="https://valkey.io/commands/client-trackinginfo/"/>
public sealed class ClientTrackingInfo
{
    #region Public Properties

    /// <summary>
    /// The set of tracking flags.
    /// </summary>
    public ISet<string> Flags { get; }

    /// <summary>
    /// The client ID receiving invalidation messages, or <c>-1</c> if not redirecting.
    /// </summary>
    public long Redirect { get; }

    /// <summary>
    /// The set of key prefixes monitored for invalidation.
    /// </summary>
    public ISet<string> Prefixes { get; }

    #endregion
    #region Constructors

    internal ClientTrackingInfo(ISet<string> flags, long redirect, ISet<string> prefixes)
    {
        Flags = flags;
        Redirect = redirect;
        Prefixes = prefixes;
    }

    #endregion
}
