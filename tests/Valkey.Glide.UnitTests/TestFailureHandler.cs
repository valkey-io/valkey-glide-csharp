// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace Valkey.Glide.UnitTests;

public static class TestFailureHandler
{
    private static readonly object Lock = new();
    private static bool s_initialized = false;
    private static bool s_firstFailure = true;

    [ModuleInitializer]
    public static void Initialize()
    {
        lock (Lock)
        {
            if (s_initialized)
            {
                return;
            }
            s_initialized = true;
            string? output = Environment.GetEnvironmentVariable("GITHUB_STEP_SUMMARY");
            if (output is null)
            {
                return;
            }
            AppDomain.CurrentDomain.FirstChanceException += (sender, e) =>
            {
                Exception? ex = e.Exception.InnerException;
                if (ex is not null && e.Exception.StackTrace?.Contains("Xunit") == true)
                {
                    if (s_firstFailure)
                    {
                        s_firstFailure = false;
                        File.AppendAllText(output, $"## Failed tests in CI pipeline:\n");
                    }
                    string testName = ExtractTestName(ex.StackTrace ?? "");
                    string permalink = BuildPermalink(testName, ex.StackTrace ?? "");
                    File.AppendAllText(output, $"### {permalink}\n```\n{ex.Message}\n```\n\n");
                }
            };

        }
    }

    private static string ExtractTestName(string stackTrace)
    {
        if (string.IsNullOrEmpty(stackTrace))
        {
            return "Unknown";
        }

        Match match = Regex.Match(stackTrace, @"at Valkey\.Glide\.(.+)\(");
        return match.Success ? match.Groups[1].Value : "Unknown";
    }

    private static string BuildPermalink(string testName, string stackTrace)
    {
        string? repo = Environment.GetEnvironmentVariable("GITHUB_REPOSITORY");
        string? sha = Environment.GetEnvironmentVariable("GITHUB_SHA");

        if (repo is null || sha is null)
        {
            return testName;
        }

        Match fileMatch = Regex.Match(stackTrace, @"in (.+):line (\d+)");
        if (!fileMatch.Success)
        {
            return testName;
        }

        string filePath = fileMatch.Groups[1].Value;
        string lineNumber = fileMatch.Groups[2].Value;

        // Convert absolute path to relative path
        string? workspace = Environment.GetEnvironmentVariable("GITHUB_WORKSPACE");
        if (workspace is not null && filePath.StartsWith(workspace))
        {
            filePath = filePath.Substring(workspace.Length).TrimStart('/');
        }

        return $"[testName](https://github.com/{repo}/blob/{sha}/{filePath}#L{lineNumber})";
    }
}

public class FailFailTests
{
    [Fact]
    public void FailFail()
    {
        Assert.Fail("This test always fails");
    }
}
