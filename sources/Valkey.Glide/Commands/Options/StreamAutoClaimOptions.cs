// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.Commands.Options;

/// <summary>
/// Arguments for the XAUTOCLAIM command.
/// </summary>
/// <seealso href="https://valkey.io/commands/xautoclaim/"/>
public sealed class StreamAutoClaimOptions
{
    #region Public Properties

    /// <summary>
    /// The minimum idle time an entry must have to be claimed.
    /// </summary>
    public TimeSpan MinIdleTime { get; }

    /// <summary>
    /// The stream ID at which to start scanning pending entries.
    /// </summary>
    public ValkeyValue StartAtId { get; }

    /// <summary>
    /// The maximum number of entries to scan (COUNT),
    /// or <see langword="null"/> to use the server default.
    /// </summary>
    public int? Count { get; }

    #endregion
    #region Constructors

    private StreamAutoClaimOptions(TimeSpan minIdleTime, ValkeyValue startAtId, int? count)
    {
        MinIdleTime = minIdleTime;
        StartAtId = startAtId;
        Count = count;
    }

    #endregion
    #region Builders

    /// <summary>
    /// Creates options that scan pending entries from the beginning of the pending entries list.
    /// </summary>
    /// <param name="minIdleTime">The minimum idle time an entry must have to be claimed.</param>
    /// <returns>Options that scan pending entries from the beginning of the pending entries list.</returns>
    public static StreamAutoClaimOptions FromStart(TimeSpan minIdleTime)
        => new(minIdleTime, StreamPosition.Beginning, null);

    /// <inheritdoc cref="FromStart(TimeSpan)"/>
    /// <param name="count">The maximum number of entries to scan (COUNT).</param>
    public static StreamAutoClaimOptions FromStart(TimeSpan minIdleTime, int count)
        => new(minIdleTime, StreamPosition.Beginning, count);

    /// <summary>
    /// Creates options that scan pending entries starting from the given stream ID.
    /// </summary>
    /// <param name="minIdleTime">The minimum idle time an entry must have to be claimed.</param>
    /// <param name="startAtId">The stream ID at which to start scanning pending entries.</param>
    /// <returns>Options that scan pending entries starting from the given stream ID.</returns>
    public static StreamAutoClaimOptions FromId(TimeSpan minIdleTime, ValkeyValue startAtId)
        => new(minIdleTime, startAtId, null);

    /// <inheritdoc cref="FromId(TimeSpan, ValkeyValue)"/>
    /// <param name="count">The maximum number of entries to scan (COUNT).</param>
    public static StreamAutoClaimOptions FromId(TimeSpan minIdleTime, ValkeyValue startAtId, int count)
        => new(minIdleTime, startAtId, count);

    #endregion
}
