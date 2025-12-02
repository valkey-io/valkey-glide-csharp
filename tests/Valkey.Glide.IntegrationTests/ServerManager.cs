// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using System.Diagnostics;

using static Valkey.Glide.ConnectionConfiguration;

namespace Valkey.Glide.IntegrationTests;

/// <summary>
/// Manages Valkey server instances during testing
/// </summary>
public static class ServerManager
{
    public static readonly string GlideUtilsDirectory;
    public static readonly string CaCertificatePath;

    static ServerManager()
    {
        string? directory = Directory.GetCurrentDirectory();
        while (!(directory == null || Directory.EnumerateDirectories(directory).Any(d => Path.GetFileName(d) == "valkey-glide")))
        {
            directory = Path.GetDirectoryName(directory);
        }

        if (directory == null)
        {
            throw new FileNotFoundException("Can't detect the project dir");
        }

        GlideUtilsDirectory = Path.Combine(directory, "valkey-glide", "utils");
        CaCertificatePath = Path.Combine(GlideUtilsDirectory, "tls_crts", "ca.crt");
    }

    /// <summary>
    /// Starts a server and returns the addresses for it.
    /// </summary>
    /// <param name="name">The server name.</param>
    /// <param name="useClusterMode">Whether to start the server in cluster mode.</param>
    /// <param name="useTls">Whether to enable TLS on the server.</param>
    /// <returns>The addresses of the started server.</returns>
    public static List<(string host, ushort port)> GetServerAddresses(string name, bool useClusterMode = false, bool useTls = false)
    {
        // Build command arguments.
        List<string> args = [];

        if (useTls)
        {
            args.Add("--tls");
        }

        args.Add("start");
        args.AddRange(["--prefix", name]);
        args.AddRange(["-r", "3"]);

        if (useClusterMode)
        {
            args.Add("--cluster-mode");
        }

        // Run cluster manager script to start server.
        string cmd = string.Join(" ", args);
        string output = RunClusterManager(cmd, false);

        // Parse and return server addresses.
        List<(string host, ushort port)> addresses = [];

        foreach (string line in output.Split("\n"))
        {
            if (!line.StartsWith("CLUSTER_NODES="))
                continue;

            foreach (string address in line.Split("=")[1].Split(","))
            {
                string[] parts = address.Split(":");
                addresses.Add((parts[0], ushort.Parse(parts[1])));
            }
        }

        return addresses;
    }

    /// <summary>
    /// Starts a standalone server and returns a configuration builder for it.
    /// </summary>
    /// <param name="name">The server name.</param>
    /// <param name="useTls">Whether to enable TLS on the server.</param>
    /// <returns>A configuration builder for the started server.</returns>
    public static StandaloneClientConfigurationBuilder GetStandaloneServerConfig(string name, bool useTls = false)
    {
        var configBuilder = new StandaloneClientConfigurationBuilder();
        configBuilder.WithTls(useTls);

        var addresses = GetServerAddresses(name, useClusterMode: false, useTls: useTls);
        addresses.ForEach(address => configBuilder.WithAddress(address.host, address.port));

        return configBuilder;
    }

    /// <summary>
    /// Starts a cluster server and returns a configuration builder for it.
    /// </summary>
    /// <param name="name">The server name.</param>
    /// <param name="useTls">Whether to enable TLS on the server.</param>
    /// <returns>A configuration builder for the started server.</returns>
    public static ClusterClientConfigurationBuilder GetClusterServerConfig(string name, bool useTls = false)
    {
        var configBuilder = new ClusterClientConfigurationBuilder();
        configBuilder.WithTls(useTls);

        var addresses = GetServerAddresses(name, useClusterMode: true, useTls: useTls);
        addresses.ForEach(address => configBuilder.WithAddress(address.host, address.port));

        return configBuilder;
    }

    public static void StopServer(string name, bool keepLogs = false)
    {
        string cmd = $"stop --prefix {name} {(keepLogs ? "--keep-folder" : "")}";
        RunClusterManager(cmd, true);
    }

    /// <summary>
    /// Asserts that a client is connected by sending a PING command.
    /// </summary>
    /// <param name="client">The client to test.</param>
    public static async Task AssertConnected(BaseClient client)
    {
        if (client is GlideClient standaloneClient)
            Assert.True(await standaloneClient.PingAsync() > TimeSpan.Zero);

        else if (client is GlideClusterClient clusterClient)
            Assert.True(await clusterClient.PingAsync() > TimeSpan.Zero);

    }

    private static string RunClusterManager(string cmd, bool ignoreExitCode)
    {
        ProcessStartInfo info = new()
        {
            WorkingDirectory = GlideUtilsDirectory,
            FileName = "python3",
            Arguments = "cluster_manager.py " + cmd,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
        };

        using Process? script = Process.Start(info);
        script?.WaitForExit();
        string? error = script?.StandardError.ReadToEnd();
        string? output = script?.StandardOutput.ReadToEnd();
        int? exitCode = script?.ExitCode;

        return !ignoreExitCode && exitCode != 0
            ? throw new ApplicationException($"cluster_manager.py script failed: exit code {exitCode}.")
                : output ?? "";
    }
}
