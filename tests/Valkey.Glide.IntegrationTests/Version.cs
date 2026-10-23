// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.IntegrationTests;

/// <summary>
/// Integration test utilities for checking the Valkey server version.
/// </summary>
internal static class Version
{
    public static readonly System.Version Valkey7_0 = new("7.0.0");
    public static readonly System.Version Valkey7_2 = new("7.2.0");
    public static readonly System.Version Valkey8_0 = new("8.0.0");
    public static readonly System.Version Valkey8_1 = new("8.1.0");
    public static readonly System.Version Valkey9_0 = new("9.0.0");
    public static readonly System.Version Valkey9_1 = new("9.1.0");
    public static readonly System.Version Valkey9_2 = new("9.2.0");

    /// <summary>
    /// Returns whether the server version is at least the given version.
    /// </summary>
    /// <param name="version">The version to compare against.</param>
    public static bool IsAtLeast(System.Version version)
        => TestConfiguration.SERVER_VERSION >= version;

    /// <summary>
    /// Skips the test if the server version is less than the given version.
    /// </summary>
    /// <param name="version">The minimum server version required by the test.</param>
    public static void SkipIfLessThan(System.Version version)
        => Assert.SkipUnless(IsAtLeast(version), $"Test requires Valkey {version} or later");
}
