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
    // File and directory paths.
    private static readonly string scriptsDirectoryPath;
    private static readonly string scriptFilePath;
    private static readonly string certificateFilePath;
    private static readonly string serverDirectoryPath;

    private static readonly string wslFileName = "wsl";
    private static readonly string pythonFileName = "python3";

    private static readonly int replicaCount = 3;

    static ServerManager()
    {
        // [A] Find project root
        // ---------------------

        // Work up from the current directory until we find the solution file.
        string directory = Directory.GetCurrentDirectory();
        while (!Directory.EnumerateFiles(directory, "Valkey.Glide.sln").Any())
        {
            directory = Path.GetDirectoryName(directory);
            if (directory == null)
            {
                throw new FileNotFoundException("Can't detect the project directory");
            }
        }

        // [B] Determine script and certificates paths
        // -------------------------------------------

        scriptsDirectoryPath = Path.Combine(directory, "valkey-glide", "utils");
        scriptFilePath = Path.Combine(scriptsDirectoryPath, "cluster_manager.py");
        certificateFilePath = Path.Combine(scriptsDirectoryPath, "tls_crts", "ca.crt");

        // [C] Determine server directory path
        // -----------------------------------

        // #184: On Windows, servers cannot synchronize when the server directory exists on a Windows
        // filesystem mounted in WSL (e.g. /mnt/c/), so use a directory inside WSL filesystem instead.
        // Use a unique directory to avoid conflicts with other test runs.
        if (OperatingSystem.IsWindows())
        {
            serverDirectoryPath = $"~/valkey_glide_test_{Guid.NewGuid():N}";
        }
        else
        {
            serverDirectoryPath = Path.Combine(scriptsDirectoryPath, "clusters");
        }
    }

    /// <summary>
    /// Gets the path for the server certificate file.
    /// See <valkey-glide/utils/cluster_manager.py> for details.
    /// </summary>
    public static string ServerCertificatePath => certificateFilePath;

    /// <summary>
    /// Starts a Valkey server with the specified name, mode and TLS configuration.
    /// </summary>
    /// <returns>A list of server addresses started.</returns>
    public static IList<Address> StartServer(string name, bool useClusterMode = false, bool useTls = false)
    {
        // Build command arguments.
        List<string> args = new();

        if (useTls)
            args.Add("--tls");

        args.Add("start");
        args.AddRange(["--prefix", name]);
        args.AddRange(["-r", replicaCount.ToString()]);
        args.AddRange(["--folder-path", serverDirectoryPath]);

        if (useClusterMode)
            args.Add("--cluster-mode");

        // Run cluster manager script.
        string startCommand = string.Join(" ", args);
        string output = RunClusterManager(startCommand);

        // Parse output to get node addresses.
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
        List<string> args = [];

        args.Add("stop");
        args.AddRange(["--prefix", name]);
        args.AddRange(["--folder-path", serverDirectoryPath]);

        if (keepLogs)
            args.Add("--keep-folder");

        string stopCommand = string.Join(" ", args);
        RunClusterManager(stopCommand);
    }

    /// <summary>
    /// Runs the cluster manager script with the specified command.
    /// See 'valkey-glide/utils/cluster_manager.py' for more details.
    /// </summary>
    private static string RunClusterManager(string scriptCmd)
    {
        ProcessStartInfo info = new();

        // On Windows, run the script via WSL with full WSL paths.
        // > wsl python3 "/mnt/c/.../valkey-glide/utils/cluster_manager.py" <scriptCmd>
        if (OperatingSystem.IsWindows())
        {
            // #184: Must use full WSL paths for script.
            // Otherwise, paths created by the script will not resolve correctly.
            info.FileName = wslFileName;
            info.Arguments = $"{pythonFileName} {ToWslPath(scriptFilePath)} {scriptCmd}";
        }

        // Non-Windows systems can run the script directly from the scripts directory.
        // > python3 "cluster_manager.py" <scriptCmd>
        else
        {
            info.FileName = pythonFileName;
            info.Arguments = $"{scriptFilePath} {scriptCmd}";
            info.WorkingDirectory = scriptsDirectoryPath;
        }

        return RunProcess(info);
    }

    /// <summary>
    /// Converts a Windows path to a WSL path and returns the result.
    /// See <https://github.com/laurent22/wslpath> for more details.
    /// </summary>
    private static string ToWslPath(string windowsPath)
    {
        ProcessStartInfo info = new()
        {
            FileName = wslFileName,
            Arguments = $"wslpath '{windowsPath}'",
        };

        // Trim output to remove trailing newline.
        return RunProcess(info).Trim();
    }

    /// <summary>
    /// Runs a process with the specified start info and returns its output.
    /// </summary>
    private static string RunProcess(ProcessStartInfo info)
    {
        // Redirect output and error streams.
        info.UseShellExecute = false;
        info.RedirectStandardOutput = true;
        info.RedirectStandardError = true;

        using Process script = Process.Start(info);
        if (script == null)
        {
            throw new ApplicationException($"Failed to start process: {info.FileName} {info.Arguments}");
        }

        script.WaitForExit();
        string error = script.StandardError.ReadToEnd();
        string output = script.StandardOutput.ReadToEnd();

        int exitCode = script.ExitCode;
        if (exitCode != 0)
        {
            throw new ApplicationException(
                $"Process failed: exit code {exitCode}.\n" +
                $"Command: {info.FileName} {info.Arguments}\n" +
                $"Error: {error}\n" +
                $"Output: {output}");
        }

        return output ?? "";
    }
}
