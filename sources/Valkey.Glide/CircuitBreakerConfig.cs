// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Internals;

namespace Valkey.Glide;

/// <summary>
/// Configuration for the client-wide circuit breaker.
/// </summary>
/// <seealso href="https://glide.valkey.io/how-to/connections/circuit-breaker/">Valkey GLIDE – Configure a Circuit Breaker</seealso>
public sealed class CircuitBreakerConfig
{
    #region Public Properties

    /// <summary>
    /// Sliding window duration for error rate calculation.
    /// </summary>
    public TimeSpan? WindowSize { get; private set; }

    /// <summary>
    /// Failure rate threshold (0.0, 1.0] within the window to trip the breaker.
    /// </summary>
    public double? FailureRateThreshold { get; private set; }

    /// <summary>
    /// Minimum number of errors within the window before the rate is evaluated.
    /// Prevents tripping on small sample sizes.
    /// </summary>
    public uint? MinErrors { get; private set; }

    /// <summary>
    /// Time in Open state before allowing a probe request.
    /// </summary>
    public TimeSpan? OpenTimeout { get; private set; }

    /// <summary>
    /// Whether command timeouts count toward tripping the breaker. Set to true only if
    /// timeouts reliably indicate server-side issues rather than client-side thread pool starvation.
    /// </summary>
    public bool CountTimeouts { get; private set; }

    /// <summary>
    /// Number of consecutive successful probe requests needed before closing the breaker.
    /// Provides a grace period to prevent flapping.
    /// </summary>
    public uint? ConsecutiveSuccesses { get; private set; }

    #endregion
    #region Public Methods

    /// <summary>
    /// Sets the sliding window duration for error rate calculation.
    /// </summary>
    /// <param name="windowSize">The window size. Must be positive.</param>
    /// <returns>This instance for method chaining.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="windowSize"/> is zero or negative.</exception>
    public CircuitBreakerConfig WithWindowSize(TimeSpan windowSize)
    {
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(windowSize, TimeSpan.Zero);
        WindowSize = windowSize;
        return this;
    }

    /// <summary>
    /// Sets the error rate threshold (0.0, 1.0] within the window to trip the breaker.
    /// </summary>
    /// <param name="threshold">The threshold value, must be in the range (0.0, 1.0].</param>
    /// <returns>This instance for method chaining.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="threshold"/> is not in (0.0, 1.0].</exception>
    public CircuitBreakerConfig WithFailureRateThreshold(double threshold)
    {
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(threshold, 0.0);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(threshold, 1.0);
        FailureRateThreshold = threshold;
        return this;
    }

    /// <summary>
    /// Sets the minimum number of errors within the window before the rate is evaluated.
    /// </summary>
    /// <param name="minErrors">The minimum error count. Must be greater than zero.</param>
    /// <returns>This instance for method chaining.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="minErrors"/> is zero.</exception>
    public CircuitBreakerConfig WithMinErrors(uint minErrors)
    {
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(minErrors, 0u);
        MinErrors = minErrors;
        return this;
    }

    /// <summary>
    /// Sets the time in Open state before allowing a probe request.
    /// </summary>
    /// <param name="openTimeout">The open timeout duration. Must be positive.</param>
    /// <returns>This instance for method chaining.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="openTimeout"/> is zero or negative.</exception>
    public CircuitBreakerConfig WithOpenTimeout(TimeSpan openTimeout)
    {
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(openTimeout, TimeSpan.Zero);
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
    /// <param name="consecutiveSuccesses">The number of successes. Must be greater than zero.</param>
    /// <returns>This instance for method chaining.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="consecutiveSuccesses"/> is zero.</exception>
    public CircuitBreakerConfig WithConsecutiveSuccesses(uint consecutiveSuccesses)
    {
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(consecutiveSuccesses, 0u);
        ConsecutiveSuccesses = consecutiveSuccesses;
        return this;
    }

    #endregion
    #region Internal Methods

    /// <summary>
    /// Converts to the FFI representation for marshalling to Rust core.
    /// </summary>
    internal FFI.CircuitBreakerConfig ToFfi() => new(
        (uint)(WindowSize?.TotalMilliseconds ?? 0),
        (float)(FailureRateThreshold ?? 0),
        MinErrors ?? 0,
        (uint)(OpenTimeout?.TotalMilliseconds ?? 0),
        CountTimeouts,
        ConsecutiveSuccesses ?? 0
    );

    #endregion
}
