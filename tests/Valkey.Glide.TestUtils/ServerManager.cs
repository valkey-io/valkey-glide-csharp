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
    // TODO #184
    private static readonly int replicaCount = OperatingSystem.IsWindows() ? 0 : 3;

    private static readonly string wslFileName = "wsl";
    private static readonly string pythonFileName = "python3";
    private static readonly string scriptName = "cluster_manager.py";

    /// <summary>
    /// Returns the path for the server certificate file.
    /// See <valkey-glide/utils/cluster_manager.py> for details.
    /// </summary>
    public static string GetServerCertificatePath()
    {
        return Path.Combine(GetScriptsDirectory(), "tls_crts", "ca.crt");
    }

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

        // TODO #184
        // args.AddRange(["--folder-path", GetServerDirectory()]);

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

        // TODO #184
        // args.AddRange(["--folder-path", GetServerDirectory()]);

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
        string fileName;
        string arguments;

        // On Windows, run the script via WSL with full WSL paths.
        // > wsl python3 "/mnt/c/.../valkey-glide/utils/cluster_manager.py" <scriptCmd>
        if (OperatingSystem.IsWindows())
        {
            // #184: Must use full WSL paths for script.
            string scriptPath = ToWslPath(Path.Combine(GetScriptsDirectory(), scriptName));

            fileName = wslFileName;
            arguments = $"{pythonFileName} {scriptPath} {scriptCmd}";
        }

        // Non-Windows systems can run the script directly.
        // > python3 "cluster_manager.py" <scriptCmd>
        else
        {
            fileName = pythonFileName;
            arguments = $"{scriptName} {scriptCmd}";
        }

        ProcessStartInfo info = new()
        {
            FileName = fileName,
            Arguments = arguments,
        };

        return RunProcess(info);
    }

    /// <summary>
    /// Converts a Windows path to a WSL path and returns the result.
    /// See <https://github.com/laurent22/wslpath> for more details.
    /// </summary>
    private static string ToWslPath(string scriptCmd)
    {
        ProcessStartInfo info = new()
        {
            FileName = wslFileName,
            Arguments = $"wslpath '{scriptCmd}'",
        };

        // Trim output to remove trailing newline.
        return RunProcess(info).Trim();
    }

    /// <summary>
    /// Returns the path for the server working directory.
    /// See <valkey-glide/utils/cluster_manager.py> for details.
    /// </summary>
    private static string GetServerDirectory()
    {
        const string directoryName = "clusters";

        // TODO #184: On Windows, servers cannot syncronize when created on a Windows filesystem
        // mounted in WSL (e.g. /mnt/c/). Use a directory inside WSL filesystem instead.
        if (OperatingSystem.IsWindows()) return $"~/{directoryName}";

        // For non-Windows, use a directory inside the scripts directory.
        return Path.Combine(GetScriptsDirectory(), "clusters");
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

        using Process? script = Process.Start(info);
        script?.WaitForExit();
        string? error = script?.StandardError.ReadToEnd();
        string? output = script?.StandardOutput.ReadToEnd();
        int? exitCode = script?.ExitCode;

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
