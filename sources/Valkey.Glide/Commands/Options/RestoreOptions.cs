// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.Commands.Options;

/// <summary>
/// Optional arguments for the RESTORE command.
/// </summary>
/// <remarks>
/// <para>Expiry options (<see cref="Ttl"/> and <see cref="ExpireAt"/>) are mutually exclusive.</para>
/// <para>IDLETIME and FREQ modifiers cannot be set at the same time.</para>
/// <para>See <a href="https://valkey.io/commands/restore/">valkey.io</a></para>
/// </remarks>
public class RestoreOptions
{
    /// <summary>
    /// The time-to-live duration for the key. If not set, the key will persist indefinitely.
    /// </summary>
    /// <remarks>Mutually exclusive with <see cref="ExpireAt"/>.</remarks>
    public TimeSpan? Ttl { get; set; }

    /// <summary>
    /// The absolute expiration time for the key. If not set, the key will persist indefinitely.
    /// </summary>
    /// <remarks>Mutually exclusive with <see cref="Ttl"/>.</remarks>
    public DateTimeOffset? ExpireAt { get; set; }

    /// <summary>
    /// When true, replaces the key if it already exists.
    /// </summary>
    public bool Replace { get; set; }

    /// <summary>
    /// The idle time of the object in seconds (for eviction purposes).
    /// </summary>
    /// <remarks>Mutually exclusive with <see cref="Frequency"/>.</remarks>
    public long? IdleTime { get; set; }

    /// <summary>
    /// The frequency counter of the object (for LFU eviction).
    /// </summary>
    /// <remarks>Mutually exclusive with <see cref="IdleTime"/>.</remarks>
    public long? Frequency { get; set; }

    /// <summary>
    /// Creates the TTL value and flags for the RESTORE command.
    /// </summary>
    /// <returns>A tuple containing (ttlMilliseconds, useAbsttl).</returns>
    /// <exception cref="ArgumentException">Thrown when both Ttl and ExpireAt are set.</exception>
    internal (long ttlMs, bool useAbsttl) GetTtlArgs()
    {
        if (Ttl.HasValue && ExpireAt.HasValue)
        {
            throw new ArgumentException("Ttl and ExpireAt cannot be set at the same time.");
        }

        if (Ttl.HasValue)
        {
            return ((long)Ttl.Value.TotalMilliseconds, false);
        }

        if (ExpireAt.HasValue)
        {
            return (ExpireAt.Value.ToUnixTimeMilliseconds(), true);
        }

        return (0, false);
    }

    /// <summary>
    /// Creates the argument array for the RESTORE command (excluding TTL).
    /// </summary>
    /// <returns>A string array that holds the subcommands and their arguments.</returns>
    /// <exception cref="ArgumentException">Thrown when both IdleTime and Frequency are set.</exception>
    internal GlideString[] ToArgs()
    {
        List<GlideString> resultList = [];

        if (ExpireAt.HasValue)
        {
            resultList.Add((GlideString)ValkeyLiterals.ABSTTL);
        }

        if (Replace)
        {
            resultList.Add((GlideString)ValkeyLiterals.REPLACE);
        }

        if (IdleTime.HasValue && Frequency.HasValue)
        {
            throw new ArgumentException("IdleTime and Frequency cannot be set at the same time.");
        }

        if (IdleTime.HasValue)
        {
            resultList.Add((GlideString)ValkeyLiterals.IDLETIME);
            resultList.Add(IdleTime.Value.ToGlideString());
        }

        if (Frequency.HasValue)
        {
            resultList.Add((GlideString)ValkeyLiterals.FREQ);
            resultList.Add(Frequency.Value.ToGlideString());
        }

        return [.. resultList];
    }
}
