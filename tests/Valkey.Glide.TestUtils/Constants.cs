// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.TestUtils;

/// <summary>
/// Shared test constants.
/// </summary>
public static class Constants
{
    // Host names and addresses for tests.
    // See 'cluster_manager.py' for details.
    public static readonly string HostnameTls = "valkey.glide.test.tls.com";
    public static readonly string HostnameNoTls = "valkey.glide.test.no_tls.com";
    public static readonly string Ipv4Address = "127.0.0.1";
    public static readonly string Ipv6Address = "::1";
}
