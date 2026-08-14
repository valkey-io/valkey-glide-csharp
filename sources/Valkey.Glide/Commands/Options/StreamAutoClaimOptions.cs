// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.Commands.Options;

/// <summary>
/// Arguments for the <c>XAUTOCLAIM</c> command.
/// </summary>
/// <seealso href="https://valkey.io/commands/xautoclaim/"/>
public sealed class StreamAutoClaimOptions
{
    #region Public Methods

    /// <summary>
    /// Sets the maximum number of entries to scan (<c>COUNT</c>).
    /// </summary>
    /// <param name="count">The maximum number of entries to scan (COUNT).</param>
    /// <returns>The same <see cref="StreamAutoClaimOptions"/> instance, for chaining.</returns>
    public StreamAutoClaimOptions WithCount(int count)
    {
        Count = count;
        return this;
    }

    #endregion
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
    /// The maximum number of entries to scan (<c>COUNT</c>),
    /// or <see langword="null"/> to use the server default.
    /// </summary>
    public int? Count { get; private set; }

    #endregion
    #region Constructors

    private StreamAutoClaimOptions(TimeSpan minIdleTime, ValkeyValue startAtId)
    {
        MinIdleTime = minIdleTime;
        StartAtId = startAtId;
    }

    #endregion
    #region Builders

    /// <summary>
    /// Creates options that scan pending entries from the beginning of the pending entries list.
    /// </summary>
    /// <param name="minIdleTime">The minimum idle time an entry must have to be claimed.</param>
    /// <returns>Options that scan pending entries from the beginning of the pending entries list.</returns>
    public static StreamAutoClaimOptions FromStart(TimeSpan minIdleTime)
        => new(minIdleTime, StreamPosition.Beginning);

    /// <summary>
    /// Creates options that scan pending entries starting from the given stream ID.
    /// </summary>
    /// <param name="minIdleTime">The minimum idle time an entry must have to be claimed.</param>
    /// <param name="startAtId">The stream ID at which to start scanning pending entries.</param>
    /// <returns>Options that scan pending entries starting from the given stream ID.</returns>
    public static StreamAutoClaimOptions FromId(TimeSpan minIdleTime, ValkeyValue startAtId)
        => new(minIdleTime, startAtId);

    #endregion
}
