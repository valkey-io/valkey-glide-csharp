// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Valkey.Glide.TestUtils;

/// <summary>
/// Test utilities for Valkey GLIDE scripts.
/// </summary>
public static class Scripts
{
    private readonly static string ValkeyGlideDirectoryName = "valkey-glide";
    private readonly static string ClusterManagerScriptName = "cluster_manager.py";

    private readonly static bool IsWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

    /// <summary>
    /// Runs the cluster manager script with the specified command.
    /// See 'valkey-glide/utils/cluster_manager.py' for more details.
    /// </summary>
    public static string RunClusterManager(string cmd, bool ignoreExitCode)
    {
        string fileName = "python3";
        List<string> arguments = new List<string> { ClusterManagerScriptName, cmd };

        // If on Windows, run the script through WSL and pass a Unix path to the script.
        if (IsWindows)
        {
            arguments.Insert(0, fileName);
            fileName = "wsl";
        }

        ProcessStartInfo info = new()
        {
            FileName = fileName,
            Arguments = string.Join(" ", arguments),
            WorkingDirectory = GetScriptsDirectory(),
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

    /// <summary>
    /// Returns the path to the 'valkey-glide/utils' directory.
    /// Throws FileNotFoundException if the project directory cannot be detected.
    /// </summary>
    public static string GetScriptsDirectory()
    {
        // Returns true if the directory is the project root directory.
        Func<string, bool> IsProjectRoot = (directory) =>
            Directory.EnumerateDirectories(directory).Any(d => Path.GetFileName(d) == ValkeyGlideDirectoryName);

        string directory = Directory.GetCurrentDirectory();
        while (!IsProjectRoot(directory))
        {
            directory = Path.GetDirectoryName(directory);
            if (directory == null)
                throw new FileNotFoundException("Can't detect the project directory");
        }

        return Path.Combine(directory, ValkeyGlideDirectoryName, "utils");
    }
}
