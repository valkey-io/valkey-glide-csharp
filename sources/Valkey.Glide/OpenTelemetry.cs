// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide;

/// <summary>
/// OpenTelemetry integration for Valkey GLIDE.
/// OpenTelemetry can only be initialized once per process.
/// </summary>
public static class OpenTelemetry
{
    private static readonly object Lock = new();
    private static OpenTelemetryConfig? s_config;
    private static readonly Random Random = new();

    /// <summary>
    /// Initialize OpenTelemetry with the provided configuration.
    /// Can only be called once per process. Subsequent calls will be ignored.
    /// </summary>
    /// <param name="config">The OpenTelemetry configuration.</param>
    /// <exception cref="ArgumentNullException">Thrown if config is null.</exception>
    public static void Init(OpenTelemetryConfig config)
    {
        if (config == null)
        {
            throw new ArgumentNullException(nameof(config));
        }

        lock (Lock)
        {
            if (s_config != null)
            {
                Logger.Log(Level.Warn, "GlideOpenTelemetry", "OpenTelemetry already initialized - ignoring new configuration");
                return;
            }

            // TODO: Call FFI to initialize Rust core
            s_config = config;
            Logger.Log(Level.Info, "GlideOpenTelemetry", "OpenTelemetry initialized");
        }
    }

    /// <summary>
    /// Check if OpenTelemetry is initialized.
    /// </summary>
    /// <returns>True if initialized, false otherwise.</returns>
    public static bool IsInitialized()
    {
        lock (Lock)
        {
            return s_config != null;
        }
    }

    /// <summary>
    /// Get the current sample percentage for traces.
    /// </summary>
    /// <returns>The sample percentage if traces are configured, null otherwise.</returns>
    public static uint? GetSamplePercentage()
    {
        lock (Lock)
        {
            return s_config?.Traces?.SamplePercentage;
        }
    }

    /// <summary>
    /// Set the sample percentage for traces at runtime.
    /// </summary>
    /// <param name="percentage">The sample percentage (0-100).</param>
    /// <exception cref="InvalidOperationException">Thrown if OpenTelemetry is not initialized or traces are not configured.</exception>
    /// <exception cref="ArgumentException">Thrown if percentage is greater than 100.</exception>
    public static void SetSamplePercentage(uint percentage)
    {
        if (!IsInitialized())
        {
            throw new InvalidOperationException("OpenTelemetry not initialized");
        }

        lock (Lock)
        {
            if (s_config?.Traces == null)
            {
                throw new InvalidOperationException("OpenTelemetry traces not initialized");
            }

            s_config.Traces.SetSamplePercentage(percentage);
        }
    }

    /// <summary>
    /// Determine if the current request should be sampled for tracing.
    /// </summary>
    /// <returns>True if the request should be sampled, false otherwise.</returns>
    internal static bool ShouldSample()
    {
        if (!IsInitialized())
        {
            return false;
        }

        var percentage = GetSamplePercentage();
        if (percentage == null || percentage == 0)
        {
            return false;
        }

        return (Random.NextDouble() * 100) < percentage;
    }

    /// <summary>
    /// Clear the OpenTelemetry configuration.
    /// Used for testing only.
    /// </summary>
    internal static void Clear()
    {
        lock (Lock)
        {
            s_config = null;
        }
    }
}
