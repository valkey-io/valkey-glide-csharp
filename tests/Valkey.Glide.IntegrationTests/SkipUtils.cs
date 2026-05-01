// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.ServerModules;

namespace Valkey.Glide.IntegrationTests;

/// <summary>
/// Integration test utilities for skipping test cases based on Valkey server version.
/// </summary>
internal static class SkipUtils
{
    #region Version Checks

    private static readonly Version Valkey7_0 = new("7.0.0");
    private static readonly Version Valkey9_0 = new("9.0.0");

    /// <summary>
    /// Skips the test if hash field expiry commands are not supported.
    /// </summary>
    public static void IfHashExpireNotSupported()
        => Assert.SkipWhen(
            TestConfiguration.SERVER_VERSION < Valkey9_0,
            "Hash expire commands require Valkey 9.0+");

    /// <summary>
    /// Skips the test if bit index type commands are not supported.
    /// </summary>
    public static void IfBitIndexTypeNotSupported()
        => Assert.SkipWhen(
            TestConfiguration.SERVER_VERSION < Valkey7_0,
            "Bit index type commands require Valkey 7.0+");

    /// <summary>
    /// Skips the test if set intersection cardinality commands are not supported.
    /// </summary>
    public static void IfSetInterCardNotSupported()
        => Assert.SkipWhen(
            TestConfiguration.SERVER_VERSION < Valkey7_0,
            "Set intersection cardinality commands require Valkey 7.0+");

    #endregion
    #region Module Checks

    /// <summary>
    /// Skips the test if the Valkey Search module is not loaded on the server.
    /// </summary>
    public static async Task IfSearchModuleNotLoaded(BaseClient client)
    {
        // TODO #361: Use client.ModuleListAsync() once implemented.
        try
        {
            _ = await Ft.ListAsync(client);
        }
        catch (Exception ex) when (
            ex.Message.Contains("unknown command", StringComparison.OrdinalIgnoreCase) ||
            ex.Message.Contains("ERR unknown", StringComparison.OrdinalIgnoreCase) ||
            ex.Message.Contains("module", StringComparison.OrdinalIgnoreCase))
        {
            Assert.Skip("Valkey Search module is not loaded on the server");
        }
    }

    #endregion
}
