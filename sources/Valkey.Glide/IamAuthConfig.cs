// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide;

/// <summary>
/// Configuration for IAM authentication with AWS services.
/// </summary>
/// <param name="clusterName">The name of the cluster.</param>
/// <param name="serviceType">The AWS service type.</param>
/// <param name="region">The AWS region where the cluster is located.</param>
/// <param name="refreshIntervalSeconds">Optional refresh interval in seconds.
/// Must be between <see cref="MinRefreshIntervalSeconds"/> and <see cref="MaxRefreshIntervalSeconds"/> if specified.
/// </param>
public class IamAuthConfig(
    string clusterName,
    ServiceType serviceType,
    string region,
    uint? refreshIntervalSeconds = null)
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
    public string ClusterName { get; set; } = clusterName ?? throw new ArgumentNullException(nameof(clusterName));

    /// <summary>
    /// The AWS service type.
    /// </summary>
    public ServiceType ServiceType { get; set; } = serviceType;

    /// <summary>
    /// The AWS region where the cluster is located.
    /// </summary>
    public string Region { get; set; } = region ?? throw new ArgumentNullException(nameof(region));

    /// <summary>
    /// Optional refresh interval in seconds.
    /// Must be between <see cref="MinRefreshIntervalSeconds"/> and <see cref="MaxRefreshIntervalSeconds"/> inclusive if specified.
    /// </summary>
    public uint? RefreshIntervalSeconds
    {
        get;
        set => field = ValidateRefreshInterval(value);
    } = ValidateRefreshInterval(refreshIntervalSeconds);

    #endregion
    #region Public Methods

    public override string ToString() =>
        // Override default implementation to hide sensitive information.
        $"IamAuthConfig {{ ServiceType = {ServiceType} }}";

    #endregion
    #region Private Methods

    /// <summary>
    /// Validates the specified refresh interval.
    /// </summary>
    /// <param name="refreshIntervalSeconds">The refresh interval to validate.</param>
    /// <returns>The validated refresh interval</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="refreshIntervalSeconds"/> is
    /// not between <see cref="MinRefreshIntervalSeconds"/> and <see cref="MaxRefreshIntervalSeconds"/> inclusive.
    /// </exception>
    private static uint? ValidateRefreshInterval(uint? refreshIntervalSeconds)
    {
        if (refreshIntervalSeconds.HasValue && (refreshIntervalSeconds.Value < MinRefreshIntervalSeconds || refreshIntervalSeconds.Value > MaxRefreshIntervalSeconds))
        {
            var msg = $"Refresh interval must be between {MinRefreshIntervalSeconds} and {MaxRefreshIntervalSeconds} seconds.";
            throw new ArgumentOutOfRangeException(nameof(refreshIntervalSeconds), refreshIntervalSeconds, msg);
        }

        return refreshIntervalSeconds;
    }

    #endregion
}
