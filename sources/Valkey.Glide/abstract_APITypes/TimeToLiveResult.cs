namespace Valkey.Glide;

/// <summary>
/// The result of an operation to get the time to live for a key or hash field.
/// </summary>
/// <seealso href="https://valkey.io/commands/ttl/"/>
/// <seealso href="https://valkey.io/commands/pttl/"/>
/// <seealso href="https://valkey.io/commands/httl/"/>
/// <seealso href="https://valkey.io/commands/hpttl/"/>
public readonly struct TimeToLiveResult
{
    internal readonly long TimeToLiveMs;

    // Special time to live values.
    internal const long DoesNotExist = -2;
    internal const long NoExpiry = -1;

    /// <summary>
    /// Whether the key or hash field exists.
    /// </summary>
    public bool Exists => TimeToLiveMs != DoesNotExist;

    /// <summary>
    /// Whether the key or hash field has a time to live.
    /// </summary>
    public bool HasTimeToLive => TimeToLiveMs >= 0;

    /// <summary>
    /// The remaining time to live, or <see langword="null"/> if the key
    /// or field does not exist or does not have an expiry set.
    /// </summary>
    public TimeSpan? TimeToLive
        => HasTimeToLive ? TimeSpan.FromMilliseconds(TimeToLiveMs) : null;

    /// <summary>
    /// Creates a <see cref="TimeToLiveResult"/> from given value.
    /// </summary>
    internal TimeToLiveResult(long timeToLiveMs)
    {
        TimeToLiveMs = timeToLiveMs;
    }
}
