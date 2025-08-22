// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Linq;

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

            TestContext.Current.SendDiagnosticMessage($"GH OUTPUT = {Environment.GetEnvironmentVariable("GITHUB_OUTPUT")}");
            TestContext.Current.SendDiagnosticMessage($"GH OUTPUT = {Environment.GetEnvironmentVariable("GITHUB_STEP_SUMMARY")}");
            var envVars = Environment.GetEnvironmentVariables();
            foreach (var var in envVars)
            {
                TestContext.Current.SendDiagnosticMessage($"{var} = {envVars[var]}");
            }
            string? output = Environment.GetEnvironmentVariable("GITHUB_OUTPUT");
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
                    File.AppendAllText(output, $"### {testName}\n```\n{ex.Message}\n```\n\n");
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
}

public class FailFailTests
{
    [Fact]
    public void FailFail()
    {
        Assert.Fail("This test always fails");
    }
}
