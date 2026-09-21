// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.TestUtils;

/// <summary>
/// Common test data.
/// </summary>
public static class Data
{
    /// <summary>
    /// A sample Availability Zone (AZ) identifier for AZ-affinity tests.
    /// </summary>
    public const string AvailabilityZone = "us-east-1a";

    /// <summary>
    /// A second sample Availability Zone (AZ) identifier, distinct from <see cref="AvailabilityZone"/>.
    /// </summary>
    public const string OtherAvailabilityZone = "us-east-1b";

    /// <summary>
    /// An Availability Zone (AZ) identifier that no node belongs to, used to exercise fallback behaviour.
    /// </summary>
    public const string NonExistingAvailabilityZone = "non-existing-az";

    /// <summary>
    /// Cluster modes for testing.
    /// </summary>
    public static TheoryData<bool> ClusterMode => [true, false];

    /// <summary>
    /// Batch atomicity modes for testing.
    /// </summary>
    public static TheoryData<bool> IsAtomic => [true, false];

    /// <summary>
    /// All node discovery modes for testing.
    /// </summary>
    public static TheoryData<NodeDiscoveryMode> NodeDiscoveryModes
        => [.. Enum.GetValues<NodeDiscoveryMode>()];

    /// <summary>
    /// Server IP addresses for testing.
    /// </summary>
    public static TheoryData<string> IpAddresses => [
        Constants.Ipv4Address,
        Constants.Ipv6Address];

    /// <summary>
    /// Valid endpoints for testing.
    /// </summary>
    public static TheoryData<string> ValidEndpoints =>
        [
            "http://localhost:4321",         // HTTP endpoint
            "https://example.com:4318",      // HTTPS endpoint
            "file:///tmp/example.txt",       // Unix-style file URI
            "file:///C:/Users/example.txt",   // Windows-style file URI
        ];

    /// <summary>
    /// Invalid endpoints for testing.
    /// </summary>
    public static TheoryData<string> InvalidEndpoints =>
        [
            (string)null!,        // null
            "",                   // empty
            "\t",                 // whitespace only
            "not-a-url",          // no scheme
            "://missing-scheme",  // malformed scheme
            "just some text",     // plain text
        ];
}
