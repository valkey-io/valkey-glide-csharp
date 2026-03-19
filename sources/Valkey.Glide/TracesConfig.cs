// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide;

/// <summary>
/// Configuration for OpenTelemetry traces.
/// </summary>
public sealed class TracesConfig
{
    #region Constants

    internal const uint DefaultSamplePercentage = 1u; // Exposed internally for testing.
    private const uint MaxSamplePercentage = 100u;

    #endregion
    #region Public Properties

    /// <summary>
    /// The endpoint for traces export.
    /// </summary>
    public string Endpoint { get; }

    /// <summary>
    /// The percentage of requests to sample.
    /// If not specified, defaults to 1 percent.
    /// </summary>
    public uint SamplePercentage
    {
        get;

        // Sample percentage should only be
        // modified by <see cref="OpenTelemetry"/>
        internal set
        {
            ValidateSamplePercentage(value);
            field = value;
        }
    }

    #endregion
    #region Constructors & Builders

    private TracesConfig(string endpoint, uint samplePercentage)
    {
        Endpoint = endpoint;
        SamplePercentage = samplePercentage;
    }

    /// <summary>
    /// Creates a new TracesConfig builder.
    /// </summary>
    public static Builder CreateBuilder() => new();

    /// <summary>
    /// Builder for TracesConfig.
    /// </summary>
    public sealed class Builder
    {
        private string? _endpoint;
        private uint _samplePercentage = DefaultSamplePercentage;

        /// <summary>
        /// Sets the endpoint for traces export.
        /// </summary>
        /// <exception cref="ArgumentException">Thrown if <paramref name="endpoint"/> is <c>null</c>, empty, or not a well-formed URI.</exception>
        public Builder WithEndpoint(string endpoint)
        {
            if (string.IsNullOrWhiteSpace(endpoint))
            {
                throw new ArgumentException("Endpoint cannot be null, empty, or whitespace only", nameof(endpoint));
            }

            if (!Uri.TryCreate(endpoint, UriKind.Absolute, out _))
            {
                throw new ArgumentException("Endpoint must be a valid absolute URI", nameof(endpoint));
            }

            _endpoint = endpoint;
            return this;
        }

        /// <summary>
        /// Sets the sample percentage.
        /// </summary>
        /// <exception cref="ArgumentException">Thrown if <paramref name="samplePercentage"/> is greater than 100.</exception>
        public Builder WithSamplePercentage(uint samplePercentage)
        {
            ValidateSamplePercentage(samplePercentage);
            _samplePercentage = samplePercentage;

            return this;
        }

        /// <summary>
        /// Builds the TracesConfig.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if endpoint is not specified.</exception>
        public TracesConfig Build()
            => _endpoint == null
                ? throw new InvalidOperationException("Endpoint must be specified")
                : new TracesConfig(_endpoint, _samplePercentage);
    }

    #endregion
    #region Private Methods

    /// <summary>
    /// Validates the specified sample percentage.
    /// </summary>
    /// <param name="samplePercentage">The sample percentage to validate.</param>
    /// <inheritdoc cref="Builder.WithSamplePercentage" path="/exception" />
    private static void ValidateSamplePercentage(uint samplePercentage)
    {
        if (samplePercentage > MaxSamplePercentage)
        {
            throw new ArgumentException($"Sample percentage cannot be greater than {MaxSamplePercentage}", nameof(samplePercentage));
        }
    }

    #endregion
}
