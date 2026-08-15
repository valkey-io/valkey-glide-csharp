// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide;

/// <summary>
/// Consumer information from a <c>XINFO CONSUMERS</c> response.
/// </summary>
/// <seealso href="https://valkey.io/commands/xinfo-consumers/"/>
public readonly struct StreamConsumerInfo
{
    #region Public Properties

    /// <summary>
    /// The name of the consumer (<c>name</c>).
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// The number of pending messages for the consumer (<c>pending</c>).
    /// </summary>
    public int PendingMessageCount { get; }

    /// <summary>
    /// The time that has passed since the consumer's last interaction (<c>idle</c>).
    /// </summary>
    /// <remarks>Valkey GLIDE only.</remarks>
    public TimeSpan Idle { get; }

    /// <summary>
    /// The time that has passed since the consumer's last successful
    /// interaction (<c>inactive</c>), or <see langword="null"/> if not specified.
    /// </summary>
    /// <remarks>
    /// <para>Since Valkey 7.2.0.</para>
    /// <para>Valkey GLIDE only.</para>
    /// </remarks>
    public TimeSpan? Inactive { get; }

    /// <summary>
    /// The number of milliseconds that has passed since the consumer's last interaction (<c>idle</c>).
    /// </summary>
    public long IdleTimeInMilliseconds => (long)Idle.TotalMilliseconds;

    #endregion
    #region Constructors

    internal StreamConsumerInfo(string name, int pendingMessageCount, TimeSpan idle, TimeSpan? inactive)
    {
        Name = name;
        PendingMessageCount = pendingMessageCount;
        Idle = idle;
        Inactive = inactive;
    }

    #endregion
}
