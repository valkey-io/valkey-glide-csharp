// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.IntegrationTests;

/// <summary>
/// Utility methods for checking Valkey module availability in integration tests.
/// </summary>
internal static class ModuleUtils
{
    /// <summary>
    /// Checks if the JSON module is available on the server.
    /// </summary>
    /// <param name="client">The client to use for the check.</param>
    /// <returns>True if the JSON module is available, false otherwise.</returns>
    public static async Task<bool> IsJsonModuleAvailableAsync(BaseClient client)
    {
        try
        {
            // Try a simple JSON.SET command to check if the module is loaded
            string testKey = $"__json_module_check_{Guid.NewGuid()}";
            GlideString[] args = ["JSON.SET", testKey, "$", "{}"];

            if (client is GlideClient standaloneClient)
            {
                _ = await standaloneClient.CustomCommand(args);
            }
            else if (client is GlideClusterClient clusterClient)
            {
                _ = await clusterClient.CustomCommand(args);
            }
            else
            {
                return false;
            }

            // Clean up the test key
            _ = await client.DeleteAsync(testKey);
            return true;
        }
        catch (Errors.RequestException)
        {
            // JSON module is not available
            return false;
        }
    }

    /// <summary>
    /// Skips the test if the JSON module is not available on the server.
    /// </summary>
    /// <param name="client">The client to use for the check.</param>
    public static async Task SkipIfJsonModuleNotAvailableAsync(BaseClient client)
    {
        bool isAvailable = await IsJsonModuleAvailableAsync(client);
        Assert.SkipWhen(!isAvailable, "JSON module is not available on the server");
    }
}
