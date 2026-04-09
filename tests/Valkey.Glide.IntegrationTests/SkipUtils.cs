// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.IntegrationTests;

/// <summary>
/// Integration test utilities for skipping test cases based on Valkey server version.
/// </summary>
internal static class SkipUtils
{
    private static readonly Version Valkey9_0 = new Version("9.0.0");

    /// <summary>
    /// Skips the test if hash field expiry commands are not supported.
    /// </summary>
    public static void IfHashExpireNotSupported()
        => Assert.SkipWhen(TestConfiguration.SERVER_VERSION < Valkey9_0, "Hash expire commands require Valkey 9.0+");
}
