// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.TestUtils;

/// <summary>
/// Common test data.
/// </summary>
public static class Data
{
    /// <summary>
    /// Cluster modes for testing.
    /// </summary>
    public static TheoryData<bool> ClusterMode => [true, false];

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
            "http://localhost:4321",        // HTTP endpoint
            "https://example.com:4318",     // HTTPS endpoint
            "file:///tmp/output.txt",       // Unix-style file URI
            @"file://C:\Users\output.txt",  // Windows-style file URI
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
