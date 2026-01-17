// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using System.Diagnostics;

namespace Valkey.Glide.TestUtils;

/// <summary>
/// Path utilities for Valkey GLIDE tests.
/// </summary>
public static class Paths
{
    private const string ValkeyGlideDirectoryName = "valkey-glide";

    /// <summary>
    /// Returns the path to the 'valkey-glide/utils' directory.
    /// </summary>
    /// <returns>The absolute path to the scripts directory.</returns>
    /// <exception cref="FileNotFoundException">Thrown if the project directory cannot be detected.</exception>
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

    /// <summary>
    /// Converts the given Windows path to the corresponding
    /// Windows Subsystem for Linux (WSL) path.
    /// </summary>
    /// <param name="path">The Windows path to convert.</param>
    /// <returns>The corresponding WSL Unix path.</returns>
    /// <exception cref="ApplicationException">Thrown if the conversion fails.</exception>
    public static string ToWslPath(string windowsPath)
    {
        ProcessStartInfo info = new()
        {
            FileName = "wsl",
            Arguments = $"wslpath -a '{windowsPath}'",
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
        };

        using Process? script = Process.Start(info);
        script?.WaitForExit();
        string? error = script?.StandardError.ReadToEnd();
        string? output = script?.StandardOutput.ReadToEnd();
        int? exitCode = script?.ExitCode;

        if (exitCode != 0)
        {
            throw new ApplicationException(
                $"wslpath failed to convert Windows path to Unix path: exit code {exitCode}.\n" +
                $"Path: {windowsPath}\n" +
                $"Error: {error}\n" +
                $"Output: {output}");
        }

        return output.Trim();
    }
}
