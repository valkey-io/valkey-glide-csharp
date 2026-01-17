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
    private static readonly bool IsWindows = OperatingSystem.IsWindows();
    private static readonly int replicaCount = 3; // TODO #184: Verify usage

    private static readonly string wslFileName = "wsl";
    private static readonly string pythonFileName = "python3";
    private static readonly string scriptName = "cluster_manager.py";

    /// <summary>
    /// Returns the path for the server certificate file.
    /// See <valkey-glide/utils/cluster_manager.py> for details.
    /// </summary>
    public static string GetServerCertificatePath()
    {
        // On Windows, convert to WSL path.
        string path = Path.Combine(GetScriptsDirectory(), "tls_crts", "ca.crt");
        return IsWindows ? RunWslPath(path) : path;
    }

    /// <summary>
    /// Starts a Valkey server with the specified name, mode and TLS configuration.
    /// </summary>
    /// <returns>A list of server addresses started.</returns>
    public static IList<Address> StartServer(string name, bool useClusterMode = false, bool useTls = false)
    {
        // Build command arguments
        // -----------------------

        List<string> args = new();

        if (useTls)
            args.Add("--tls");

        args.Add("start");
        args.AddRange(["--prefix", name]);

        // TODO #184: Use 3 replicas by default.
        args.AddRange(["--replica-count", replicaCount.ToString()]);

        if (useClusterMode)
            args.Add("--cluster-mode");

        // TODO #184: Specify the clusters directory to prevent path-related
        // issues when running on Windows with WSL.
        // args.AddRange(["--folder-path", GetServerDirectory()]);

        // Run cluster manager script
        // --------------------------

        string startCommand = string.Join(" ", args);
        string output = RunClusterManager(startCommand);

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

        if (keepLogs)
            args.Add("--keep-folder");

        // TODO #184: Specify the clusters directory to prevent path-related
        // issues when running on Windows with WSL.
        // args.AddRange(["--folder-path", GetServerDirectory()]);

        string stopCommand = string.Join(" ", args);
        RunClusterManager(stopCommand);
    }

    /// <summary>
    /// Runs the cluster manager script with the specified command.
    /// See 'valkey-glide/utils/cluster_manager.py' for more details.
    /// </summary>
    private static string RunClusterManager(string scriptCmd)
    {
        string fileName;
        string arguments;

        // On Windows, run the script via WSL.
        if (OperatingSystem.IsWindows())
        {
            fileName = wslFileName;
            arguments = $"{pythonFileName} {scriptName} {scriptCmd}";
        }
        else
        {
            fileName = pythonFileName;
            arguments = $"{scriptName} {scriptCmd}";
        }

        ProcessStartInfo info = new()
        {
            FileName = fileName,
            Arguments = arguments,
            WorkingDirectory = GetScriptsDirectory(),
        };

        return RunProcess(info);
    }

    /// <summary>
    /// Runs wslpath with the specified command.
    /// See <https://github.com/laurent22/wslpath> for more details.
    /// </summary>
    private static string RunWslPath(string scriptCmd)
    {

        ProcessStartInfo info = new()
        {
            FileName = wslFileName,
            Arguments = $"wslpath '{scriptCmd}'",
            WorkingDirectory = GetScriptsDirectory(),
        };

        // Trim output to remove trailing newline.
        return RunProcess(info).Trim();
    }

    // TODO #184
    /// <summary>
    /// Returns the path for the server working directory.
    /// See <valkey-glide/utils/cluster_manager.py> for details.
    /// </summary>
    public static string GetServerDirectory()
    {
        // On Windows, convert to WSL path.
        string path = Path.Combine(GetScriptsDirectory(), "clusters");
        return IsWindows ? RunWslPath(path) : path;
    }

    /// <summary>
    /// Returns the path to the 'valkey-glide/utils' directory.
    /// </summary>
    /// <returns>The absolute path to the scripts directory.</returns>
    /// <exception cref="FileNotFoundException">Thrown if the project directory cannot be detected.</exception>
    private static string GetScriptsDirectory()
    {
        // Returns true if the directory is the project root directory.
        const string ValkeyGlideDirectory = "valkey-glide";
        Func<string, bool> IsProjectRoot = (directory) =>
                Directory.EnumerateDirectories(directory).Any(d => Path.GetFileName(d) == ValkeyGlideDirectory);

        string directory = Directory.GetCurrentDirectory();
        while (!IsProjectRoot(directory))
        {
            directory = Path.GetDirectoryName(directory);
            if (directory == null)
                throw new FileNotFoundException("Can't detect the project directory");
        }

        return Path.Combine(directory, ValkeyGlideDirectory, "utils");
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

        using Process process = Process.Start(info);
        if (process == null)
        {
            throw new InvalidOperationException($"{info.FileName} is not installed or not accessible.");
        }

        string output = process.StandardOutput.ReadToEnd();
        string error = process.StandardError.ReadToEnd();

        process.WaitForExit();

        int exitCode = process.ExitCode;
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
