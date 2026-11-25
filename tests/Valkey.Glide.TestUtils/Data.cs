// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using System.Text;

namespace Valkey.Glide.TestUtils;

/// <summary>
/// Common test data.
/// </summary>
public static class Data
{
    #region Private Fields

    /// <summary>
    /// Bytes that are not valid UTF-8.
    /// Used to verify binary safety.
    /// </summary>
    private static readonly byte[] InvalidUtf8 = [0xFF, 0x00, 0x80, 0xC3, 0x28, 0xFE];

    #endregion
    #region Theory Data

    /// <summary>
    /// RESP protocol versions for testing.
    /// </summary>
    public static TheoryData<ConnectionConfiguration.Protocol> Protocols =>
        [ConnectionConfiguration.Protocol.RESP2, ConnectionConfiguration.Protocol.RESP3];

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

    #endregion
    #region Data Builders

    /// <summary>
    /// Returns a unique key for testing.
    /// The key includes bytes that are not valid UTF-8 to verify binary safety.
    /// </summary>
    public static ValkeyKey BuildKey()
        => (byte[])[.. Encoding.UTF8.GetBytes($"{Guid.NewGuid()}:"), .. InvalidUtf8];

    /// <summary>
    /// Returns a unique key for testing with the given prefix.
    /// The key includes bytes that are not valid UTF-8 to verify binary safety.
    /// </summary>
    public static ValkeyKey BuildKey(string prefix)
        => (byte[])[.. Encoding.UTF8.GetBytes($"{prefix}{Guid.NewGuid()}:"), .. InvalidUtf8];

    /// <summary>
    /// Returns a unique value for testing.
    /// The value includes bytes that are not valid UTF-8 to verify binary safety.
    /// </summary>
    public static ValkeyValue BuildValue()
        => (byte[])[.. Encoding.UTF8.GetBytes($"{Guid.NewGuid()}:"), .. InvalidUtf8];

    #endregion
}
