// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using System.Diagnostics;

using static Valkey.Glide.ConnectionConfiguration;

namespace Valkey.Glide.IntegrationTests;

/// <summary>
/// Manages Valkey server instances during testing
/// </summary>
public static class ServerManager
{
    // Utils directory for the cluster manager script and file path
    // for the CA certificate that it generates when starting TLS servers.
    // See 'valkey-glide/utils/cluster_manager.py' for more details.
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
    /// Starts a standalone server with the given name and TLS configuration.
    /// Returns a configuration builder corresponding to that server.
    /// </summary>
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
    /// Returns a configuration builder corresponding to that server.
    /// </summary>
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
    /// Returns the server addresses.
    /// </summary>
    internal static List<(string host, ushort port)> StartServer(string name, bool useClusterMode = false, bool useTls = false)
    {
        // Build command arguments.
        List<string> args = [];

        args.Add("start");
        args.Add($"--prefix {name}");
        args.Add("-r 3");

        if (useClusterMode)
            args.Add("--cluster-mode");

        if (useTls)
            args.Add("--tls");

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
