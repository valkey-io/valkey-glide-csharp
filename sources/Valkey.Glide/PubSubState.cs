// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide;

/// <summary>
/// The pub/sub subscription state for a client.
/// <para />
/// The desired state represent the subscriptions that the client has requested using
/// subscribe and unsubscribe commands, while the actual state represents the subscriptions
/// that are currently active on the server.
/// <para />
/// A background reconciliation task continuously aligns actual subscriptions with
/// desired subscriptions, handling any discrepancies that may arise due to network
/// issues, server restarts, connection drops, or topology changes.
/// </summary>
public class PubSubState
{
    /// <summary>The desired subscriptions, indexed by channel mode.</summary>
    public IReadOnlyDictionary<PubSubChannelMode, IReadOnlySet<string>> Desired { get; }

    /// <summary>The actual subscriptions, indexed by channel mode.</summary>
    public IReadOnlyDictionary<PubSubChannelMode, IReadOnlySet<string>> Actual { get; }

    internal PubSubState(
        IReadOnlyDictionary<PubSubChannelMode, IReadOnlySet<string>> desired,
        IReadOnlyDictionary<PubSubChannelMode, IReadOnlySet<string>> actual)
        => (Desired, Actual) = (desired, actual);
}
