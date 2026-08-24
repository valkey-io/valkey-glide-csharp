// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using static Valkey.Glide.Internals.TimeUtils;

namespace Valkey.Glide.Commands.Options;

/// <summary>
/// Arguments for the <c>XCLAIM</c> command.
/// </summary>
/// <seealso href="https://valkey.io/commands/xclaim/"/>
public sealed class StreamClaimOptions
{
    #region Public Methods

    /// <summary>
    /// Sets the idle time of the claimed message to the specified interval (IDLE).
    /// Clears any idle time previously set by <see cref="WithIdle"/> or <see cref="WithIdleUnix"/>.
    /// </summary>
    /// <param name="idle">The idle time to set.</param>
    /// <returns>The same <see cref="StreamClaimOptions"/> instance, for chaining.</returns>
    public StreamClaimOptions WithIdle(TimeSpan idle)
    {
        Idle = idle;
        IdleUnix = null;
        return this;
    }

    /// <summary>
    /// Sets the last delivery time of the claimed message to the specified timestamp (TIME).
    /// Clears any idle time previously set by <see cref="WithIdle"/> or <see cref="WithIdleUnix"/>.
    /// </summary>
    /// <param name="idleUnix">The Unix timestamp to set as the last delivery time.</param>
    /// <returns>The same <see cref="StreamClaimOptions"/> instance, for chaining.</returns>
    public StreamClaimOptions WithIdleUnix(DateTimeOffset idleUnix)
    {
        IdleUnix = idleUnix;
        Idle = null;
        return this;
    }

    /// <summary>
    /// Sets the retry counter to the specified value (RETRYCOUNT).
    /// </summary>
    /// <param name="retryCount">The retry counter value.</param>
    /// <returns>The same <see cref="StreamClaimOptions"/> instance, for chaining.</returns>
    public StreamClaimOptions WithRetryCount(int retryCount)
    {
        RetryCount = retryCount;
        return this;
    }

    /// <summary>
    /// Creates a PEL entry even if the message is not already assigned to a consumer (FORCE).
    /// </summary>
    /// <returns>The same <see cref="StreamClaimOptions"/> instance, for chaining.</returns>
    public StreamClaimOptions WithForce()
    {
        Force = true;
        return this;
    }

    #endregion
    #region Public Properties

    /// <summary>
    /// The minimum idle time an entry must have to be claimed.
    /// </summary>
    public TimeSpan MinIdleTime { get; }

    /// <summary>
    /// The idle time for the message (IDLE).
    /// </summary>
    public TimeSpan? Idle { get; private set; }

    /// <summary>
    /// The idle time for the message as a timestamp (TIME).
    /// </summary>
    public DateTimeOffset? IdleUnix { get; private set; }

    /// <summary>
    /// The retry counter value (RETRYCOUNT).
    /// </summary>
    public int? RetryCount { get; private set; }

    /// <summary>
    /// Whether to create a PEL entry even if the message is not already assigned to a consumer (FORCE).
    /// </summary>
    public bool Force { get; private set; }

    #endregion
    #region Constructors

    private StreamClaimOptions(TimeSpan minIdleTime)
    {
        MinIdleTime = minIdleTime;
    }

    #endregion
    #region Builders

    /// <summary>
    /// Creates options that claim entries idle for at least the given time.
    /// </summary>
    /// <param name="minIdleTime">The minimum idle time.</param>
    /// <returns>Options that claim entries idle for at least the given time.</returns>
    public static StreamClaimOptions From(TimeSpan minIdleTime)
        => new(minIdleTime);

    #endregion
    #region Internal Methods

    /// <summary>
    /// Builds the command arguments for these options and the specified message IDs.
    /// </summary>
    /// <param name="messageIds">The message IDs to claim.</param>
    internal GlideString[] ToArgs(IEnumerable<ValkeyValue> messageIds)
    {
        List<GlideString> args = [ToULongMs(MinIdleTime, nameof(MinIdleTime)).ToGlideString(), .. messageIds];

        if (Idle.HasValue)
        {
            args.Add(ValkeyLiterals.IDLE);
            args.Add(ToULongMs(Idle.Value, nameof(Idle)).ToGlideString());
        }

        if (IdleUnix.HasValue)
        {
            args.Add(ValkeyLiterals.TIME);
            args.Add(IdleUnix.Value.ToUnixTimeMilliseconds().ToGlideString());
        }

        if (RetryCount.HasValue)
        {
            args.Add(ValkeyLiterals.RETRYCOUNT);
            args.Add(RetryCount.Value.ToGlideString());
        }

        if (Force)
        {
            args.Add(ValkeyLiterals.FORCE);
        }

        return [.. args];
    }

    #endregion
}
