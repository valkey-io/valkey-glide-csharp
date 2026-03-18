// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.TestUtils;

/// <summary>
/// Common test data.
/// </summary>
public static class Data
{
    /// <summary>
    /// Cluster modes to test.
    /// </summary>
    public static TheoryData<bool> ClusterMode => [true, false];

    /// <summary>
    /// Server IP addresses to test.
    /// </summary>
    public static TheoryData<string> IpAddresses => [
        Constants.Ipv4Address,
        Constants.Ipv6Address];
}
