// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide;

/// <summary>
/// Configuration for IAM authentication with AWS services.
/// </summary>
public class IamAuthConfig
{
    #region Constants

    /// <summary>
    /// Minimum refresh interval in seconds.
    /// </summary>
    public const uint MinRefreshIntervalSeconds = 15 * 60; // 15 minutes

    /// <summary>
    /// Maximum refresh interval in seconds.
    /// </summary>
    public const uint MaxRefreshIntervalSeconds = 12 * 60 * 60; // 12 hours

    #endregion
    #region Public Properties

    /// <summary>
    /// The name of the cluster.
    /// </summary>
    public string ClusterName { get; }

    /// <summary>
    /// The AWS service type.
    /// </summary>
    public ServiceType ServiceType { get; }

    /// <summary>
    /// The AWS region where the cluster is located.
    /// </summary>
    public string Region { get; }

    /// <summary>
    /// Optional refresh interval in seconds.
    /// Must be between <see cref="MinRefreshIntervalSeconds"/> and <see cref="MaxRefreshIntervalSeconds"/> inclusive if specified.
    /// </summary>
    public uint? RefreshIntervalSeconds
    {
        get;
        private init
        {
            if (value.HasValue && (value.Value < MinRefreshIntervalSeconds || value.Value > MaxRefreshIntervalSeconds))
            {
                var msg = $"Refresh interval must be between {MinRefreshIntervalSeconds} and {MaxRefreshIntervalSeconds} seconds.";
                throw new ArgumentOutOfRangeException(nameof(RefreshIntervalSeconds), value, msg);
            }

            field = value;
        }
    }

    #endregion
    #region Constructors

    /// <summary>
    /// Creates a new <see cref="IamAuthConfig"/> instance.
    /// </summary>
    /// <param name="clusterName"><inheritdoc cref="ClusterName" path="/summary" /></param>
    /// <param name="serviceType"><inheritdoc cref="ServiceType" path="/summary" /></param>
    /// <param name="region"><inheritdoc cref="Region" path="/summary" /></param>
    /// <param name="refreshIntervalSeconds"><inheritdoc cref="RefreshIntervalSeconds" path="/summary" /></param>
    public IamAuthConfig(
        string clusterName,
        ServiceType serviceType,
        string region,
        uint? refreshIntervalSeconds = null)
    {
        ArgumentNullException.ThrowIfNull(clusterName, nameof(clusterName));
        ArgumentNullException.ThrowIfNull(region, nameof(region));

        ClusterName = clusterName;
        ServiceType = serviceType;
        Region = region;
        RefreshIntervalSeconds = refreshIntervalSeconds;
    }

    #endregion
    #region Public Methods

    public override string ToString() =>
        // Override default implementation to hide sensitive information.
        $"IamAuthConfig {{ ServiceType = {ServiceType} }}";

    #endregion
}
