// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide;

/// <summary>
/// The pub/sub subscription state for a client.
/// <para />
/// The desired state represents the subscriptions that the client has requested using
/// subscribe and unsubscribe commands, while the actual state represents the subscriptions
/// that are currently active on the server.
/// <para />
/// A background reconciliation task continuously aligns actual subscriptions with
/// desired subscriptions, handling any discrepancies that may arise due to network
/// issues, server restarts, connection drops, or topology changes.
/// </summary>
/// <seealso cref="ConnectionConfiguration.ClientConfigurationBuilder{T}.PubSubReconciliationInterval"/>
public class PubSubState
{
    /// <summary>The desired subscriptions, indexed by channel mode.</summary>
    public IReadOnlyDictionary<PubSubChannelMode, IReadOnlySet<ValkeyKey>> Desired { get; }

    /// <summary>The actual subscriptions, indexed by channel mode.</summary>
    public IReadOnlyDictionary<PubSubChannelMode, IReadOnlySet<ValkeyKey>> Actual { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="PubSubState"/> class
    /// with the specified desired and actual subscription states.
    /// </summary>
    internal PubSubState(
        IReadOnlyDictionary<PubSubChannelMode, IReadOnlySet<ValkeyKey>> desired,
        IReadOnlyDictionary<PubSubChannelMode, IReadOnlySet<ValkeyKey>> actual)
    {
        (Desired, Actual) = (desired, actual);
    }
}
