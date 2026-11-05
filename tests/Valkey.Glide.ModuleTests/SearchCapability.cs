// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.ServerModules;

namespace Valkey.Glide.ModuleTests;

/// <summary>
/// Detects whether the connected valkey-search module actually has 1.3 features live, and reads
/// its active <c>search.emulate-release</c> compatibility level -- mirroring Python's
/// <c>requires_search_1_3</c> fixture, which this port and the other wave-3 ports copy.
/// </summary>
/// <remarks>
/// Probes with a real command (<c>FT.SEARCH ... INKEYS 0</c>) rather than a version-string
/// compare: a version string alone conflates the module *binary's* capability with its *active*
/// compatibility level, which are different questions. A 1.3 module left at its own default
/// <c>search.emulate-release</c> (1.0.0, per COMPATIBILITY.md) still runs several legacy code
/// paths -- <see cref="EmulateReleaseLevelAsync"/> answers that second question, and the level fixture
/// built on top of it is how tests branch their assertions on the actual behaviour rather than
/// assuming "1.3-capable" implies "1.3 corrected behaviour everywhere".
/// </remarks>
public static class SearchCapability
{
    private const string ProbeIndexPrefix = "__search_capability_probe_";

    /// <summary>
    /// Probes whether valkey-search's 1.3 surface is live on the connected server, using a real
    /// <c>FT.SEARCH ... INKEYS 0</c> call (an option not compat-gated, so it is live as soon as
    /// the binary is 1.3+, regardless of the active <c>search.emulate-release</c> level) against a
    /// throwaway index it creates and cleans up itself.
    /// </summary>
    /// <param name="client">The client to probe with.</param>
    /// <returns><see langword="true"/> if the 1.3 surface is live.</returns>
    public static async Task<bool> Probe13CapabilityAsync(BaseClient client)
    {
        string index = ProbeIndexPrefix + Guid.NewGuid().ToString("N");
        try
        {
            await Ft.CreateAsync(client, index, new Ft.CreateTextField("title"));
        }
        catch (Errors.RequestException ex) when (
            ex.Message.Contains("unknown command", StringComparison.OrdinalIgnoreCase) ||
                ex.Message.Contains("not loaded", StringComparison.OrdinalIgnoreCase))
        {
            // M2: fail loudly, matching Python's requires_search_1_3 fixture ("if the search
            // module is missing entirely, the probe fails instead of skipping"). This harness
            // (unlike the main suite's ModuleUtils.SkipIfSearchModuleNotAvailableAsync gate, which
            // covers legitimately modules-free dev servers) never starts its own server --
            // ModuleTestConfiguration requires MODULES_STANDALONE_ENDPOINT and the start script's
            // own check_modules step is supposed to guarantee a module is loaded. A missing
            // module here means the harness itself is misconfigured, not a legitimate pre-1.3
            // server; silently returning false would make that failure indistinguishable from the
            // intended, loud skip reason ("search module is pre-1.3") and mask it.
            throw new InvalidOperationException(
                "The search module is not loaded on the module-test harness's server at all. " +
                    "This harness (ModuleTestConfiguration) never starts its own server, so a missing " +
                    "module here means the Docker-based deployment utils/start-valkey-docker-modules.sh " +
                    "started is misconfigured -- not a legitimate pre-1.3 server to skip against. Check " +
                    "the harness's MODULE LIST output and SEARCH_MODULE_PATH.",
                ex);
        }

        try
        {
            try
            {
                _ = await Ft.SearchAsync(client, index, "*", new Ft.SearchOptions { InKeys = [] });
                return true;
            }
            catch (Errors.RequestException ex) when (
                ex.Message.Contains("INKEYS", StringComparison.OrdinalIgnoreCase) ||
                    ex.Message.Contains("Unexpected argument", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }
        }
        finally
        {
            await DropProbeIndexAsync(client, index);
        }
    }

    /// <summary>
    /// Reads the server's active <c>search.emulate-release</c> compatibility level via
    /// <c>CONFIG GET</c>. Only meaningful when <see cref="Probe13CapabilityAsync"/> already
    /// returned <see langword="true"/> -- a pre-1.3 module has no such config key at all.
    /// </summary>
    /// <param name="client">The client to read the config from.</param>
    /// <returns>The level string (e.g. <c>"1.0.0"</c> or <c>"1.3.0"</c>), or <see langword="null"/> if the key is absent.</returns>
    public static async Task<string?> EmulateReleaseLevelAsync(BaseClient client)
    {
        GlideString[] args = ["CONFIG", "GET", "search.emulate-release"];
        object? raw = client switch
        {
            GlideClient standalone => await standalone.CustomCommand(args),
            // L5 (fixed): an unrouted CONFIG GET on a cluster client fans out to every node by
            // default and returns a multi-node value, which .SingleValue rejects outright
            // ("No single value stored") -- confirmed live while adding this cluster-branch test
            // coverage. search.emulate-release is a server-wide module config, identical on every
            // node, so routing to any single one (Random) is representative.
            GlideClusterClient cluster => (await cluster.CustomCommand(args, Route.Random)).SingleValue,
            _ => throw new ArgumentException($"Unsupported client type: {client.GetType()}", nameof(client)),
        };

        return raw switch
        {
            Dictionary<GlideString, object> map when map.TryGetValue("search.emulate-release", out var value) =>
                value?.ToString(),
            object[] pairs when pairs.Length >= 2 => pairs[1]?.ToString(),
            _ => null,
        };
    }

    /// <summary>
    /// <see langword="true"/> if <paramref name="level"/> is parseable and <c>&gt;= 1.3.0</c>, the
    /// threshold at which GLIDE's compat-gated typed options (<c>LoadField(alias:)</c>, SORTBY
    /// widening by an adjacent LIMIT, FT.INFO <c>score_field</c>) report the corrected RediSearch-
    /// compatible shape. Below this, the module runs the legacy branch of every
    /// <c>VALKEY_SEARCH_COMPATIBILITY_FIX</c> gate even though the binary is 1.3-capable.
    /// </summary>
    /// <param name="level">The level string from <see cref="EmulateReleaseLevelAsync"/>.</param>
    public static bool IsAtLeast130(string? level)
        => level is not null && System.Version.TryParse(level, out var parsed) && parsed >= new Version(1, 3, 0);

    /// <summary>
    /// Cleans up loudly per the port rules: a failed DROPINDEX of the probe index retries once,
    /// then rethrows rather than swallowing the error.
    /// </summary>
    private static async Task DropProbeIndexAsync(BaseClient client, string index)
    {
        try
        {
            await Ft.DropIndexAsync(client, index);
        }
        catch (Errors.RequestException)
        {
            // One retry, matching the port rules' "never swallows the error" requirement.
            await Ft.DropIndexAsync(client, index);
        }
    }
}
