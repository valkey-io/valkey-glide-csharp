// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.UnitTests;

/// <summary>
/// Grep-based verification tests for IDE0001 "Name can be simplified" violations.
/// These tests verify that fully-qualified names have been simplified to their shortest
/// unambiguous form, and that cross-interface references are preserved.
/// </summary>
public class Ide0001SimplifyNamesTests
{
    private static readonly string RepoRoot = GetRepoRoot();

    private static string GetRepoRoot()
    {
        string dir = AppContext.BaseDirectory;
        while (dir is not null && !File.Exists(Path.Combine(dir, "Valkey.Glide.sln")))
            dir = Path.GetDirectoryName(dir)!;
        return dir ?? throw new InvalidOperationException("Could not find repository root (Valkey.Glide.sln).");
    }

    private static int CountOccurrences(string filePath, string pattern)
    {
        string fullPath = Path.Combine(RepoRoot, filePath);
        if (!File.Exists(fullPath))
            throw new FileNotFoundException($"File not found: {fullPath}");
        return File.ReadAllLines(fullPath).Count(line => line.Contains(pattern, StringComparison.Ordinal));
    }

    // ── Bug Condition Exploration (Property 1) ──────────────────────────
    // These tests encode the EXPECTED behavior: zero fully-qualified patterns.
    // On unfixed code they FAIL (proving the bug exists).
    // After the fix they PASS (proving the violations are resolved).

    [Fact]
    public void IPubSubCommands_NoFullyQualifiedTimeoutException()
    {
        int count = CountOccurrences(
            "sources/Valkey.Glide/Commands/IPubSubCommands.cs",
            "cref=\"Valkey.Glide.Errors.TimeoutException\"");
        Assert.Equal(0, count);
    }

    [Fact]
    public void IPubSubClusterCommands_NoFullyQualifiedTimeoutException()
    {
        int count = CountOccurrences(
            "sources/Valkey.Glide/Commands/IPubSubClusterCommands.cs",
            "cref=\"Valkey.Glide.Errors.TimeoutException\"");
        Assert.Equal(0, count);
    }

    [Fact]
    public void IPubSubCommands_NoSelfQualifiedGetSubscriptionsAsync()
    {
        int count = CountOccurrences(
            "sources/Valkey.Glide/Commands/IPubSubCommands.cs",
            "cref=\"IPubSubCommands.GetSubscriptionsAsync\"");
        Assert.Equal(0, count);
    }

    [Fact]
    public void ScanTests_NoFullyQualifiedRequestException()
    {
        int count = CountOccurrences(
            "tests/Valkey.Glide.IntegrationTests/ScanTests.cs",
            "Valkey.Glide.Errors.RequestException");
        Assert.Equal(0, count);
    }

    [Fact]
    public void ISubscriberCompatibilityTests_NoFullyQualifiedValkeyTypes()
    {
        // The using alias directive at file scope requires fully-qualified names
        // because it is processed before global usings take effect.
        // This is NOT an IDE0001 violation that can be fixed — the compiler requires it.
        // Verify the alias line still exists (preservation).
        int channelCount = CountOccurrences(
            "tests/Valkey.Glide.IntegrationTests/ISubscriberCompatibilityTests.cs",
            "Valkey.Glide.ValkeyChannel");
        int valueCount = CountOccurrences(
            "tests/Valkey.Glide.IntegrationTests/ISubscriberCompatibilityTests.cs",
            "Valkey.Glide.ValkeyValue");
        Assert.Equal(1, channelCount);
        Assert.Equal(1, valueCount);
    }

    // ── Preservation (Property 2) ───────────────────────────────────────
    // These tests verify cross-interface references are NOT modified.
    // They PASS on both unfixed and fixed code.

    [Fact]
    public void IPubSubClusterCommands_PreservesCrossInterfaceReferences()
    {
        int count = CountOccurrences(
            "sources/Valkey.Glide/Commands/IPubSubClusterCommands.cs",
            "cref=\"IPubSubCommands.GetSubscriptionsAsync\"");
        Assert.True(count > 0,
            "Cross-interface cref=\"IPubSubCommands.GetSubscriptionsAsync\" references " +
            "in IPubSubClusterCommands.cs must be preserved.");
    }
}
