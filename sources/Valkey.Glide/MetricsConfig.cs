// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide;

/// <summary>
/// Configuration for OpenTelemetry metrics.
/// </summary>
public sealed class MetricsConfig
{
    #region Public Properties

    /// <summary>
    /// The endpoint for metrics export.
    /// </summary>
    public string Endpoint { get; }

    #endregion
    #region Constructors

    private MetricsConfig(string endpoint)
    {
        Endpoint = endpoint;
    }

    #endregion
    #region Public Methods

    /// <summary>
    /// Creates a new MetricsConfig builder.
    /// </summary>
    public static Builder CreateBuilder() => new();

    #endregion

    /// <summary>
    /// Builder for MetricsConfig.
    /// </summary>
    public sealed class Builder
    {
        private string? _endpoint;

        /// <summary>
        /// Sets the endpoint for metrics export.
        /// </summary>
        /// <exception cref="ArgumentException">Thrown if endpoint is null, empty, or not a valid absolute URI.</exception>
        public Builder WithEndpoint(string endpoint)
        {
            if (string.IsNullOrWhiteSpace(endpoint))
                throw new ArgumentException("Endpoint cannot be null, empty, or whitespace only", nameof(endpoint));

            if (!Uri.IsWellFormedUriString(endpoint, UriKind.Absolute))
                throw new ArgumentException("Endpoint must be a valid absolute URI", nameof(endpoint));

            _endpoint = endpoint;
            return this;
        }

        /// <summary>
        /// Builds the MetricsConfig.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if endpoint is not specified.</exception>
        public MetricsConfig Build()
        {
            if (_endpoint == null)
                throw new InvalidOperationException("Endpoint must be specified");

            return new MetricsConfig(_endpoint);
        }
    }
}
