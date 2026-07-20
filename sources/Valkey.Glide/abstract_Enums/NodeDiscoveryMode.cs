// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide;

/// <summary>
/// Controls how the client discovers node roles and topology in standalone mode.
/// <para />
/// This option is only relevant for standalone clients; it is ignored in cluster mode, which
/// has its own topology discovery mechanism.
/// </summary>
/// <seealso href="https://glide.valkey.io/reference/connection-options/">Valkey GLIDE – Connection Options</seealso>
public enum NodeDiscoveryMode : uint
{
    /// <summary>
    /// Default: verify node roles via <c>INFO REPLICATION</c>, use only the provided addresses.
    /// This is the fully backward-compatible behavior.
    /// </summary>
    Standard = 0,

    /// <summary>
    /// Skip role detection entirely. Trust the provided addresses as-is; the first address is
    /// treated as the primary. Use when connecting through a proxy (e.g., Envoy) or when the
    /// topology is known and static.
    /// <para />
    /// <b>Note</b>: Do not set a client name when using this mode with a proxy, as the proxy may
    /// not support the <c>CLIENT SETNAME</c> command sent during connection setup.
    /// </summary>
    Static = 1,

    /// <summary>
    /// Discover the full topology (primary + all replicas) from any starting node. Provide any
    /// single node address (primary or replica) and the client will find and connect to all
    /// other nodes.
    /// <para />
    /// Discovery happens only at client creation time; topology changes after creation are not
    /// re-discovered until the client is recreated. This mode is mutually exclusive with
    /// read-only mode, which skips the <c>INFO REPLICATION</c> call that discovery requires.
    /// </summary>
    DiscoverAll = 2,
}
