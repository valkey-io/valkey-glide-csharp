// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Internals;

namespace Valkey.Glide;

/// <summary>
/// Configuration for the client-wide circuit breaker.
/// </summary>
/// <seealso href="https://glide.valkey.io/how-to/connections/circuit-breaker/">Valkey GLIDE – Configure a Circuit Breaker</seealso>
public sealed class CircuitBreakerConfig
{
    #region Constants

    /// <summary>
    /// Default sliding window duration for error rate calculation (10 seconds).
    /// </summary>
    public static readonly TimeSpan DefaultWindowSize = TimeSpan.FromSeconds(10);

    /// <summary>
    /// Default failure rate threshold within the window to trip the breaker (50%).
    /// </summary>
    public const float DefaultFailureRateThreshold = 0.5f;

    /// <summary>
    /// Default minimum number of errors within the window before the rate is evaluated (50).
    /// </summary>
    public const uint DefaultMinErrors = 50;

    /// <summary>
    /// Default time in Open state before allowing a probe request (5 seconds).
    /// </summary>
    public static readonly TimeSpan DefaultOpenTimeout = TimeSpan.FromSeconds(5);

    /// <summary>
    /// Default number of consecutive successful probe requests needed before closing the breaker (3).
    /// </summary>
    public const uint DefaultConsecutiveSuccesses = 3;

    #endregion
    #region Public Properties

    /// <summary>
    /// Sliding window duration for error rate calculation.
    /// </summary>
    public TimeSpan WindowSize { get; private set; } = DefaultWindowSize;

    /// <summary>
    /// Failure rate threshold (0.0, 1.0] within the window to trip the breaker.
    /// </summary>
    public float FailureRateThreshold { get; private set; } = DefaultFailureRateThreshold;

    /// <summary>
    /// Minimum number of errors within the window before the rate is evaluated.
    /// Prevents tripping on small sample sizes.
    /// </summary>
    public uint MinErrors { get; private set; } = DefaultMinErrors;

    /// <summary>
    /// Time in Open state before allowing a probe request.
    /// </summary>
    public TimeSpan OpenTimeout { get; private set; } = DefaultOpenTimeout;

    /// <summary>
    /// Whether command timeouts count toward tripping the breaker. Set to true only if
    /// timeouts reliably indicate server-side issues rather than client-side thread pool starvation.
    /// </summary>
    public bool CountTimeouts { get; private set; } = false;

    /// <summary>
    /// Number of consecutive successful probe requests needed before closing the breaker.
    /// Provides a grace period to prevent flapping.
    /// </summary>
    public uint ConsecutiveSuccesses { get; private set; } = DefaultConsecutiveSuccesses;

    #endregion
    #region Public Methods

    /// <summary>
    /// Sets the sliding window duration for error rate calculation.
    /// </summary>
    /// <param name="windowSize">The window size.</param>
    /// <returns>This instance for method chaining.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown if <paramref name="windowSize"/> is not positive or is too large.
    /// </exception>
    public CircuitBreakerConfig WithWindowSize(TimeSpan windowSize)
    {
        GuardClauses.ThrowIfNotPositiveUintMilliseconds(windowSize, nameof(windowSize));
        WindowSize = windowSize;
        return this;
    }

    /// <summary>
    /// Sets the error rate threshold (0.0, 1.0] within the window to trip the breaker.
    /// </summary>
    /// <param name="threshold">The threshold value.</param>
    /// <returns>This instance for method chaining.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when <paramref name="threshold"/> is zero or negative.
    /// </exception>
    public CircuitBreakerConfig WithFailureRateThreshold(float threshold)
    {
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(threshold, 0.0f, nameof(threshold));
        FailureRateThreshold = threshold;
        return this;
    }

    /// <summary>
    /// Sets the minimum number of errors within the window before the rate is evaluated.
    /// </summary>
    /// <param name="minErrors">The minimum error count.</param>
    /// <returns>This instance for method chaining.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when <paramref name="minErrors"/> is zero.
    /// </exception>
    public CircuitBreakerConfig WithMinErrors(uint minErrors)
    {
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(minErrors, 0u, nameof(minErrors));
        MinErrors = minErrors;
        return this;
    }

    /// <summary>
    /// Sets the time in Open state before allowing a probe request.
    /// </summary>
    /// <param name="openTimeout">The open timeout duration.</param>
    /// <returns>This instance for method chaining.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown if <paramref name="openTimeout"/> is not positive or is too large.
    /// </exception>
    public CircuitBreakerConfig WithOpenTimeout(TimeSpan openTimeout)
    {
        GuardClauses.ThrowIfNotPositiveUintMilliseconds(openTimeout, nameof(openTimeout));
        OpenTimeout = openTimeout;
        return this;
    }

    /// <summary>
    /// Sets whether command timeouts count toward tripping the breaker.
    /// </summary>
    /// <param name="countTimeouts">Whether to count timeouts.</param>
    /// <returns>This instance for method chaining.</returns>
    public CircuitBreakerConfig WithCountTimeouts(bool countTimeouts = true)
    {
        CountTimeouts = countTimeouts;
        return this;
    }

    /// <summary>
    /// Sets the number of consecutive successful probe requests needed before closing the breaker.
    /// </summary>
    /// <param name="consecutiveSuccesses">The number of successes.</param>
    /// <returns>This instance for method chaining.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when <paramref name="consecutiveSuccesses"/> is zero.
    /// </exception>
    public CircuitBreakerConfig WithConsecutiveSuccesses(uint consecutiveSuccesses)
    {
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(consecutiveSuccesses, 0u, nameof(consecutiveSuccesses));
        ConsecutiveSuccesses = consecutiveSuccesses;
        return this;
    }

    #endregion
    #region Internal Methods

    /// <summary>
    /// Converts to the FFI representation for marshalling to Rust core.
    /// </summary>
    internal FFI.CircuitBreakerConfig ToFfi()
    {
        // Casts to uint are safe: validated by WithWindowSize and WithOpenTimeout.
        var windowSize = (uint)WindowSize.TotalMilliseconds;
        var openTimeout = (uint)OpenTimeout.TotalMilliseconds;

        return new(
            windowSize,
            FailureRateThreshold,
            MinErrors,
            openTimeout,
            CountTimeouts,
            ConsecutiveSuccesses);
    }

    #endregion
}
