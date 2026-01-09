// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using System.Diagnostics;
using System.Runtime.InteropServices;

using static Valkey.Glide.ConnectionConfiguration;

namespace Valkey.Glide.TestUtils;

/// <summary>
/// Manages Valkey server instances.
/// </summary>
public static class ServerManager
{
    // TODO #184: Verify whether this is necessary on Windows.
    // Number of replicas to use. Don't use replicas on Windows to avoid synchronization issues.
    private static readonly int NUM_REPLICAS = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? 0 : 3;

    /// <summary>
    /// Starts a Valkey server with the specified name, mode and TLS configuration.
    /// </summary>
    /// <returns>A list of server addresses started.</returns>
    public static IList<Address> StartServer(string name, bool useClusterMode = false, bool useTls = false)
    {
        // Build command arguments.
        List<string> args = [];

        if (useTls)
            args.Add("--tls");

        args.Add("start");
        args.Add($"--prefix {name}");
        args.Add($"--replicas {NUM_REPLICAS}");

        if (useClusterMode)
            args.Add("--cluster-mode");

        // Run cluster manager script to start server.
        string cmd = string.Join(" ", args);
        string output = Scripts.RunClusterManager(cmd, ignoreExitCode: false);

        // Parse and return server addresses from output.
        const string hostsPattern = @"^CLUSTER_NODES=(.+)$";
        var match = System.Text.RegularExpressions.Regex.Match(output, hostsPattern, System.Text.RegularExpressions.RegexOptions.Multiline);
        var hosts = match.Groups[1].Value;

        return Address.FromHosts(hosts);
    }

    /// <summary>
    /// Stops the Valkey server with the specified name.
    /// </summary>
    public static void StopServer(string name, bool keepLogs = false)
    {
        string stopCmd = $"stop --prefix {name}";
        if (keepLogs) stopCmd += " --keep-folder";
        Scripts.RunClusterManager(stopCmd, ignoreExitCode: true);
    }
}
