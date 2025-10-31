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
        string cmd = $"start {(useClusterMode ? "--cluster-mode" : "")} {(useTls ? "--tls" : "")} --prefix {name} -r 3";
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
}
