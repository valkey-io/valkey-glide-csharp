namespace Valkey.Glide;

/// <summary>
/// Represents the result of a time-to-live operation.
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
    /// Whether the key or field has an expiry set.
    /// </summary>
    public bool HasExpiry => TimeToLiveMs >= 0;

    /// <summary>
    /// The remaining time to live, or <see langword="null"/> if the key
    /// or field does not exist or does not have an expiry set.
    /// </summary>
    public TimeSpan? TimeToLive => HasExpiry ? TimeSpan.FromMilliseconds(TimeToLiveMs) : null;

    /// <summary>
    /// Creates a <see cref="TimeToLiveResult"/> from given value.
    /// </summary>
    internal TimeToLiveResult(long timeToLiveMs)
    {
        TimeToLiveMs = timeToLiveMs;
    }
}
