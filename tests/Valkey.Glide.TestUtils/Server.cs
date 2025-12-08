// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using System.Diagnostics;

using static Valkey.Glide.ConnectionConfiguration;

namespace Valkey.Glide.TestUtils;

/// <summary>
/// Base class for server that is stopped when disposed.
/// </summary>
public class Server : IDisposable
{
    private bool _disposed = false;

    protected bool _useTls = false;
    protected string _name = $"Server_{Guid.NewGuid():N}";
    protected List<(string host, ushort port)> _addresses = [];

    // Utils directory for the cluster manager script and file path for the server certificate that it generates.
    // See 'valkey-glide/utils/cluster_manager.py' for more details.
    public static readonly string GlideUtilsDirectory = Path.Combine("..", "..", "valkey-glide", "utils");
    public static readonly string ServerCertificatePath = Path.Combine(GlideUtilsDirectory, "tls_crts", "ca.crt");

    ~Server()
    {
        Dispose();
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;

        StopServer(_name);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Starts a server with the specified name, mode and TLS configuration.
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
    /// Stops the server with the specified name.
    /// </summary>
    public static void StopServer(string name, bool keepLogs = false)
    {
        string stopCmd = $"stop --prefix {name}";
        if (keepLogs) stopCmd += " --keep-folder";
        RunClusterManager(stopCmd, true);
    }

    /// <summary>
    /// Runs the cluster manager script with the specified command.
    /// </summary>
    /// <returns>The script output.</returns>
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

/// <summary>
/// Cluster server that is stopped when disposed.
/// </summary>
public sealed class ClusterServer : Server
{
    public ClusterServer(bool useTls = false)
    {
        _useTls = useTls;
        _addresses = StartServer(_name, useClusterMode: true, useTls: _useTls);
    }

    /// <summary>
    /// Builds and returns a cluster client configuration builder for this server.
    /// </summary>
    public ClusterClientConfigurationBuilder CreateConfigBuilder()
    {
        var configBuilder = new ClusterClientConfigurationBuilder();
        configBuilder.WithTls(useTls: _useTls);

        foreach (var (host, port) in _addresses)
            configBuilder.WithAddress(host, port);

        return configBuilder;
    }
}

/// <summary>
/// Standalone server that is stopped when disposed.
/// </summary>
public sealed class StandaloneServer : Server
{
    public StandaloneServer(bool useTls = false)
    {
        _useTls = useTls;
        _addresses = StartServer(_name, useClusterMode: false, useTls: _useTls);
    }

    /// <summary>
    /// Builds and returns a standalone client configuration builder for this server.
    /// </summary>
    public StandaloneClientConfigurationBuilder CreateConfigBuilder()
    {
        var configBuilder = new StandaloneClientConfigurationBuilder();
        configBuilder.WithTls(useTls: _useTls);

        foreach (var (host, port) in _addresses)
            configBuilder.WithAddress(host, port);

        return configBuilder;
    }
}
