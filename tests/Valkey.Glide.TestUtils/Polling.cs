// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Xunit;

namespace Valkey.Glide.TestUtils;

/// <summary>
/// Polling utilities for integration tests that need to wait for asynchronous conditions.
/// </summary>
public static class Polling
{
    #region Constants

    /// <summary>
    /// Default maximum duration to poll before failing.
    /// </summary>
    private static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(10);

    /// <summary>
    /// Default interval between polling attempts.
    /// </summary>
    private static readonly TimeSpan DefaultInterval = TimeSpan.FromSeconds(0.5);

    #endregion
    #region Public Methods

    /// <summary>
    /// Polls the given <paramref name="condition"/> until it returns <see langword="true"/>, or fails with
    /// <paramref name="message"/> if the <paramref name="timeout"/> expires.
    /// </summary>
    /// <param name="condition">A function that returns <see langword="true"/> when the expected state is reached.</param>
    /// <param name="message">The message to report if the condition is not met within the timeout.</param>
    /// <param name="timeout">Maximum time to poll. Defaults to <see cref="DefaultTimeout"/>.</param>
    /// <param name="interval">Delay between attempts. Defaults to <see cref="DefaultInterval"/>.</param>
    public static Task WaitFor(
        Func<bool> condition,
        string message,
        TimeSpan? timeout = null,
        TimeSpan? interval = null)
        => WaitForAsync(()
            => Task.FromResult(condition()), message, timeout, interval);

    /// <summary>
    /// Polls the given <paramref name="condition"/> until it returns <see langword="true"/>, or fails with
    /// <paramref name="message"/> if the <paramref name="timeout"/> expires.
    /// </summary>
    /// <param name="condition">An async function that returns <see langword="true"/> when the expected state is reached.</param>
    /// <param name="message">The message to report if the condition is not met within the timeout.</param>
    /// <param name="timeout">Maximum time to poll. Defaults to <see cref="DefaultTimeout"/>.</param>
    /// <param name="interval">Delay between attempts. Defaults to <see cref="DefaultInterval"/>.</param>
    public static async Task WaitForAsync(
        Func<Task<bool>> condition,
        string message,
        TimeSpan? timeout = null,
        TimeSpan? interval = null)
    {
        timeout ??= DefaultTimeout;
        interval ??= DefaultInterval;

        using CancellationTokenSource cts = new(timeout.Value);
        while (!cts.Token.IsCancellationRequested)
        {
            if (await condition())
            {
                return;
            }

            await Task.Delay(interval.Value);
        }

        Assert.Fail(message);
    }

    #endregion
}
