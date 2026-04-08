namespace Valkey.Glide;

/// <summary>
/// Represents the result of an operation to retrieve the time to live for a key or field.
/// </summary>
/// <seealso href="https://valkey.io/commands/ttl/"/>
/// <seealso href="https://valkey.io/commands/pttl/"/>
/// <seealso href="https://valkey.io/commands/httl/"/>
/// <seealso href="https://valkey.io/commands/hpttl/"/>
public readonly struct TimeToLiveResult
{
    internal readonly long TimeToLiveMs;

    /// <summary>
    /// Whether the key or field exists.
    /// </summary>
    public bool Exists => TimeToLiveMs != -2L;

    /// <summary>
    /// Whether the key or field has a time to live.
    /// </summary>
    public bool HasTimeToLive => TimeToLiveMs >= 0;

    /// <summary>
    /// The remaining time to live, or <see langword="null"/> if the key
    /// or field does not exist or does not have an expiry set.
    /// </summary>
    public TimeSpan? TimeToLive => HasTimeToLive ? TimeSpan.FromMilliseconds(TimeToLiveMs) : null;

    /// <summary>
    /// Creates a <see cref="TimeToLiveResult"/> from given value.
    /// </summary>
    internal TimeToLiveResult(long timeToLiveMs)
    {
        TimeToLiveMs = timeToLiveMs;
    }
}
