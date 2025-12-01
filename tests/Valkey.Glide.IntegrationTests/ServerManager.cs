// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using System.Diagnostics;

namespace Valkey.Glide.IntegrationTests;

/// <summary>
/// Manages Valkey server instances during testing
/// </summary>
public static class ServerManager
{
    public static List<(string host, ushort port)> StartStandaloneServer(string name, bool useTls = false)
    {
        return StartServer(name, useTls: useTls, useClusterMode: false);
    }

    public static List<(string host, ushort port)> StartClusterServer(string name, bool useTls = false)
    {
        return StartServer(name, useTls: useTls, useClusterMode: true);
    }

    public static void StopServer(string name, bool keepLogs = false)
    {
        string cmd = $"stop --prefix {name} {(keepLogs ? "--keep-folder" : "")}";
        RunClusterManager(cmd, true);
    }

    private static List<(string host, ushort port)> StartServer(string name, bool useTls, bool useClusterMode)
    {
        List<string> args = new();

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

        string cmd = string.Join(" ", args);
        return ParseHostsFromOutput(RunClusterManager(cmd, false));
    }

    private static List<(string host, ushort port)> ParseHostsFromOutput(string output)
    {
        List<(string host, ushort port)> hosts = [];
        foreach (string line in output.Split("\n"))
        {
            if (!line.StartsWith("CLUSTER_NODES="))
            {
                continue;
            }

            string[] addresses = line.Split("=")[1].Split(",");
            foreach (string address in addresses)
            {
                string[] parts = address.Split(":");
                hosts.Add((parts[0], ushort.Parse(parts[1])));
            }
        }
        return hosts;
    }

    private static string RunClusterManager(string cmd, bool ignoreExitCode)
    {
        string? projectDir = Directory.GetCurrentDirectory();
        while (!(projectDir == null || Directory.EnumerateDirectories(projectDir).Any(d => Path.GetFileName(d) == "valkey-glide")))
        {
            projectDir = Path.GetDirectoryName(projectDir);
        }

        if (projectDir == null)
        {
            throw new FileNotFoundException("Can't detect the project dir");
        }

        string scriptDir = Path.Combine(projectDir, "valkey-glide", "utils");

        ProcessStartInfo info = new()
        {
            WorkingDirectory = scriptDir,
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
}
