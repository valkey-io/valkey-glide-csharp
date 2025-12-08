// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using System.Diagnostics;

using static Valkey.Glide.ConnectionConfiguration;

namespace Valkey.Glide.TestUtils;

/// <summary>
/// Test utilities for Valkey GLIDE servers.
/// </summary>
public static class Server
{
    // Utils directory for the cluster manager script and file path for the server certificate that it generates.
    // See 'valkey-glide/utils/cluster_manager.py' for more details.
    public static readonly string GlideUtilsDirectory = Path.Combine("..", "..", "valkey-glide", "utils");
    public static readonly string ServerCertificatePath = Path.Combine(GlideUtilsDirectory, "tls_crts", "ca.crt");

    /// <summary>
    /// Starts a standalone server with the given name and TLS configuration.
    /// </summary>
    /// <returns>A configuration builder corresponding to that server.</returns>
    public static StandaloneClientConfigurationBuilder StartStandaloneServer(string name, bool useTls = false)
    {
        var configBuilder = new StandaloneClientConfigurationBuilder();
        configBuilder.WithTls(useTls);

        var addresses = StartServer(name, useClusterMode: false, useTls: useTls);
        addresses.ForEach(address => configBuilder.WithAddress(address.host, address.port));

        return configBuilder;
    }

    /// <summary>
    /// Starts a cluster server with the given name and TLS configuration.
    /// </summary>
    /// <returns>A configuration builder corresponding to that cluster.</returns>
    public static ClusterClientConfigurationBuilder StartClusterServer(string name, bool useTls = false)
    {
        var configBuilder = new ClusterClientConfigurationBuilder();
        configBuilder.WithTls(useTls);

        var addresses = StartServer(name, useClusterMode: true, useTls: useTls);
        addresses.ForEach(address => configBuilder.WithAddress(address.host, address.port));

        return configBuilder;
    }

    /// <summary>
    /// Stops the server with the specified name.
    /// </summary>
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

    /// <summary>
    /// Starts a server with the specified name, mode, and TLS configuration.
    /// </summary>
    /// <returns>A list of server addresses (host, port) started.</returns>
    public static List<(string host, ushort port)> StartServer(string name, bool useClusterMode = false, bool useTls = false)
    {
        // Build command arguments.
        List<string> args = [];

        if (useTls)
            args.Add("--tls");

        args.Add("start");
        args.Add($"--prefix {name}");
        args.Add("-r 3");

        if (useClusterMode)
            args.Add("--cluster-mode");

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
    /// Runs the cluster manager script with the specified command.
    /// Returns the script output.
    /// </summary>
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

        if (!ignoreExitCode && exitCode != 0)
        {
            throw new ApplicationException(
                $"cluster_manager.py script failed: exit code {exitCode}.\n" +
                $"Command: {cmd}\n" +
                $"Error: {error}\n" +
                $"Output: {output}");
        }

        return output ?? "";
    }
}
