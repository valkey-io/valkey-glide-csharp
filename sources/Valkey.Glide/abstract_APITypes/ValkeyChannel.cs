// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using System;
using System.Text;

namespace Valkey.Glide;

/// <summary>
/// Represents a pub/sub channel name.
/// Compatible with StackExchange.Redis <c>RedisChannel</c>.
/// </summary>
public readonly struct ValkeyChannel : IEquatable<ValkeyChannel>
{
    internal readonly byte[]? Value;
    internal readonly ValkeyChannelOptions Options;

    [Flags]
    internal enum ValkeyChannelOptions
    {
        None = 0,
        Pattern = 1 << 0,
        Sharded = 1 << 1,
    }

    /// <summary>
    /// Indicates whether the channel-name is either null or a zero-length value.
    /// </summary>
    public bool IsNullOrEmpty => Value == null || Value.Length == 0;

    /// <summary>
    /// Indicates whether this channel represents a wildcard pattern (see <c>PSUBSCRIBE</c>).
    /// </summary>
    public bool IsPattern => (Options & ValkeyChannelOptions.Pattern) != 0;

    /// <summary>
    /// Indicates whether this channel represents a sharded channel (see <c>SSUBSCRIBE</c>).
    /// </summary>
    public bool IsSharded => (Options & ValkeyChannelOptions.Sharded) != 0;

    internal bool IsNull => Value == null;

    /// <summary>
    /// Indicates whether channels should use <see cref="PatternMode.Auto"/> when no <see cref="PatternMode"/>
    /// is specified; this is enabled by default, but can be disabled to avoid unexpected wildcard scenarios.
    /// </summary>
    public static bool UseImplicitAutoPattern
    {
        get => s_DefaultPatternMode == PatternMode.Auto;
        set => s_DefaultPatternMode = value ? PatternMode.Auto : PatternMode.Literal;
    }
    private static PatternMode s_DefaultPatternMode = PatternMode.Auto;

    /// <summary>
    /// Creates a new <see cref="ValkeyChannel"/> that does not act as a wildcard subscription.
    /// </summary>
    public static ValkeyChannel Literal(string value) => new(value, ValkeyChannelOptions.None);

    /// <summary>
    /// Creates a new <see cref="ValkeyChannel"/> that does not act as a wildcard subscription.
    /// </summary>
    public static ValkeyChannel Literal(byte[] value) => new(value, ValkeyChannelOptions.None);

    /// <summary>
    /// Creates a new <see cref="ValkeyChannel"/> that does not act as a wildcard subscription.
    /// </summary>
    internal static ValkeyChannel Literal(ValkeyKey value) => new((byte[]?)value, ValkeyChannelOptions.None);

    /// <summary>
    /// Creates a new <see cref="ValkeyChannel"/> that acts as a wildcard subscription.
    /// </summary>
    public static ValkeyChannel Pattern(string value) => new(value, ValkeyChannelOptions.Pattern);

    /// <summary>
    /// Creates a new <see cref="ValkeyChannel"/> that acts as a wildcard subscription.
    /// </summary>
    public static ValkeyChannel Pattern(byte[] value) => new(value, ValkeyChannelOptions.Pattern);

    /// <summary>
    /// Creates a new <see cref="ValkeyChannel"/> that acts as a wildcard subscription.
    /// </summary>
    internal static ValkeyChannel Pattern(ValkeyKey value) => new((byte[]?)value, ValkeyChannelOptions.Pattern);

    /// <summary>
    /// Create a new channel from a buffer, explicitly controlling the pattern mode.
    /// </summary>
    public ValkeyChannel(byte[]? value, PatternMode mode) : this(value, DeterminePatternBased(value, mode) ? ValkeyChannelOptions.Pattern : ValkeyChannelOptions.None)
    {
    }

    /// <summary>
    /// Create a new channel from a string, explicitly controlling the pattern mode.
    /// </summary>
    public ValkeyChannel(string value, PatternMode mode) : this(value is null ? null : Encoding.UTF8.GetBytes(value), mode)
    {
    }

    /// <summary>
    /// Create a new channel from a buffer, representing a sharded channel.
    /// </summary>
    public static ValkeyChannel Sharded(byte[]? value) => new(value, ValkeyChannelOptions.Sharded);

    /// <summary>
    /// Create a new channel from a string, representing a sharded channel.
    /// </summary>
    public static ValkeyChannel Sharded(string value) => new(value, ValkeyChannelOptions.Sharded);

    /// <summary>
    /// Create a new channel from a <see cref="ValkeyKey"/>, representing a sharded channel.
    /// </summary>
    internal static ValkeyChannel Sharded(ValkeyKey value) => new((byte[]?)value, ValkeyChannelOptions.Sharded);

    internal ValkeyChannel(byte[]? value, ValkeyChannelOptions options)
    {
        Value = value;
        Options = options;
    }

    internal ValkeyChannel(string? value, ValkeyChannelOptions options)
    {
        Value = value is null ? null : Encoding.UTF8.GetBytes(value);
        Options = options;
    }

    private static bool DeterminePatternBased(byte[]? value, PatternMode mode) => mode switch
    {
        PatternMode.Auto => value != null && Array.IndexOf(value, (byte)'*') >= 0,
        PatternMode.Literal => false,
        PatternMode.Pattern => true,
        _ => throw new ArgumentOutOfRangeException(nameof(mode)),
    };

    /// <summary>
    /// Indicate whether two channel names are not equal.
    /// </summary>
    public static bool operator !=(ValkeyChannel x, ValkeyChannel y) => !(x == y);

    /// <summary>
    /// Indicate whether two channel names are not equal.
    /// </summary>
    public static bool operator !=(string x, ValkeyChannel y) => !(x == y);

    /// <summary>
    /// Indicate whether two channel names are not equal.
    /// </summary>
    public static bool operator !=(byte[] x, ValkeyChannel y) => !(x == y);

    /// <summary>
    /// Indicate whether two channel names are not equal.
    /// </summary>
    public static bool operator !=(ValkeyChannel x, string y) => !(x == y);

    /// <summary>
    /// Indicate whether two channel names are not equal.
    /// </summary>
    public static bool operator !=(ValkeyChannel x, byte[] y) => !(x == y);

    /// <summary>
    /// Indicate whether two channel names are equal.
    /// </summary>
    public static bool operator ==(ValkeyChannel x, ValkeyChannel y) =>
        (x.Options == y.Options)
        && ValkeyValue.Equals(x.Value, y.Value);

    /// <summary>
    /// Indicate whether two channel names are equal.
    /// </summary>
    public static bool operator ==(string x, ValkeyChannel y) =>
        ValkeyValue.Equals(x is null ? null : Encoding.UTF8.GetBytes(x), y.Value);

    /// <summary>
    /// Indicate whether two channel names are equal.
    /// </summary>
    public static bool operator ==(byte[] x, ValkeyChannel y) => ValkeyValue.Equals(x, y.Value);

    /// <summary>
    /// Indicate whether two channel names are equal.
    /// </summary>
    public static bool operator ==(ValkeyChannel x, string y) =>
        ValkeyValue.Equals(x.Value, y is null ? null : Encoding.UTF8.GetBytes(y));

    /// <summary>
    /// Indicate whether two channel names are equal.
    /// </summary>
    public static bool operator ==(ValkeyChannel x, byte[] y) => ValkeyValue.Equals(x.Value, y);

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj switch
    {
        ValkeyChannel rcObj => ValkeyValue.Equals(Value, rcObj.Value),
        string sObj => ValkeyValue.Equals(Value, Encoding.UTF8.GetBytes(sObj)),
        byte[] bObj => ValkeyValue.Equals(Value, bObj),
        _ => false,
    };

    /// <summary>
    /// Indicate whether two channel names are equal.
    /// </summary>
    public bool Equals(ValkeyChannel other) =>
        (Options == other.Options)
        && ValkeyValue.Equals(Value, other.Value);

    /// <inheritdoc/>
    public override int GetHashCode() => ValkeyValue.GetHashCode(Value) ^ (int)Options;

    /// <summary>
    /// Obtains a string representation of the channel name.
    /// </summary>
    public override string ToString() => ((string?)this) ?? "(null)";

    internal void AssertNotNull()
    {
        if (IsNull) throw new ArgumentException("A null channel is not valid in this context");
    }

    /// <summary>
    /// Converts this channel to a <see cref="ValkeyKey"/>.
    /// </summary>
    internal ValkeyKey ToValkeyKey()
        => Value is null ? ValkeyKey.Null : (ValkeyKey)Value;

    internal ValkeyChannel Clone()
    {
        if (Value is null || Value.Length == 0)
        {
            return this;
        }
        var copy = (byte[])Value.Clone();
        return new ValkeyChannel(copy, Options);
    }

    #region Keyspace

    /// <summary>
    /// Create a key-notification channel for a single key in a single database.
    /// </summary>
    /// <param name="key">The key to monitor.</param>
    /// <param name="database">The database index.</param>
    /// <returns>A channel representing <c>__keyspace@{database}__:{key}</c>.</returns>
    public static ValkeyChannel KeySpaceSingleKey(in ValkeyKey key, int database)
    {
        if (key.IsNull || key.IsEmpty)
        {
            throw new ArgumentNullException(nameof(key));
        }

        return BuildKeySpaceChannel(in key, database, isPattern: false, appendStar: false);
    }

    /// <summary>
    /// Create a key-notification channel for a pattern, optionally in a specified database.
    /// </summary>
    /// <param name="pattern">The key pattern to monitor.</param>
    /// <param name="database">The database index. If null, matches all databases.</param>
    /// <returns>A channel representing <c>__keyspace@{database|*}__:{pattern}[*]</c>.</returns>
    public static ValkeyChannel KeySpacePattern(in ValkeyKey pattern, int? database = null)
    {
        if (pattern.IsNull || pattern.IsEmpty)
        {
            return BuildKeySpaceChannel(ValkeyKey.Null, database, isPattern: true, appendStar: true);
        }

        return BuildKeySpaceChannel(in pattern, database, isPattern: true, appendStar: false);
    }

    /// <summary>
    /// Create a key-notification channel using a raw prefix, optionally in a specified database.
    /// </summary>
    /// <param name="prefix">The key prefix to monitor. Must not be empty.</param>
    /// <param name="database">The database index. If null, matches all databases.</param>
    /// <returns>A channel representing <c>__keyspace@{database|*}__:{prefix}*</c>.</returns>
    public static ValkeyChannel KeySpacePrefix(in ValkeyKey prefix, int? database = null)
    {
        if (prefix.IsNull || prefix.IsEmpty)
        {
            throw new ArgumentNullException(nameof(prefix));
        }

        return BuildKeySpaceChannel(in prefix, database, isPattern: true, appendStar: true);
    }

    /// <summary>
    /// Create a key-notification channel using a raw byte prefix, optionally in a specified database.
    /// </summary>
    /// <param name="prefix">The key prefix bytes to monitor. Must not be empty.</param>
    /// <param name="database">The database index. If null, matches all databases.</param>
    /// <returns>A channel representing <c>__keyspace@{database|*}__:{prefix}*</c>.</returns>
    public static ValkeyChannel KeySpacePrefix(ReadOnlySpan<byte> prefix, int? database = null)
    {
        if (prefix.IsEmpty)
        {
            throw new ArgumentNullException(nameof(prefix));
        }

        return BuildChannelFromSpan("__keyspace@"u8, prefix, database, appendStar: true);
    }

    /// <summary>
    /// Create an event-notification channel for a given event type, optionally in a specified database.
    /// </summary>
    /// <param name="type">The notification type.</param>
    /// <param name="database">The database index. If null, matches all databases.</param>
    /// <returns>A channel representing <c>__keyevent@{database|*}__:{type}</c>.</returns>
    public static ValkeyChannel KeyEvent(KeyNotificationType type, int? database = null)
        => KeyEvent(GetKeyNotificationWireName(type), database);

    /// <summary>
    /// Create an event-notification channel for a given event type using raw bytes.
    /// </summary>
    /// <param name="type">The event type bytes. Must not be empty.</param>
    /// <param name="database">The database index. If null, matches all databases.</param>
    /// <returns>A channel representing <c>__keyevent@{database|*}__:{type}</c>.</returns>
    public static ValkeyChannel KeyEvent(ReadOnlySpan<byte> type, int? database)
    {
        if (type.IsEmpty)
        {
            throw new ArgumentNullException(nameof(type));
        }

        return BuildKeyEventChannel(type, database);
    }

    private static ValkeyChannel BuildKeySpaceChannel(in ValkeyKey key, int? database, bool isPattern, bool appendStar)
    {
        byte[] dbBytes = Encoding.UTF8.GetBytes(database?.ToString() ?? "*");
        int keyLen = key.IsNull ? 0 : key.TotalLength();
        byte[] arr = new byte[11 + dbBytes.Length + 3 + keyLen + (appendStar ? 1 : 0)];
        var span = arr.AsSpan();

        "__keyspace@"u8.CopyTo(span); span = span.Slice(11);
        dbBytes.CopyTo(span); span = span.Slice(dbBytes.Length);
        "__:"u8.CopyTo(span); span = span.Slice(3);
        if (keyLen > 0) { key.CopyTo(span); span = span.Slice(keyLen); }
        if (appendStar) span[0] = (byte)'*';

        return new ValkeyChannel(arr, isPattern ? ValkeyChannelOptions.Pattern : ValkeyChannelOptions.None);
    }

    private static ValkeyChannel BuildChannelFromSpan(
        ReadOnlySpan<byte> header,
        ReadOnlySpan<byte> payload,
        int? database,
        bool appendStar)
    {
        byte[] dbBytes = Encoding.UTF8.GetBytes(database?.ToString() ?? "*");
        byte[] arr = new byte[header.Length + dbBytes.Length + 3 + payload.Length + (appendStar ? 1 : 0)];
        var span = arr.AsSpan();

        header.CopyTo(span); span = span.Slice(header.Length);
        dbBytes.CopyTo(span); span = span.Slice(dbBytes.Length);
        "__:"u8.CopyTo(span); span = span.Slice(3);
        payload.CopyTo(span); span = span.Slice(payload.Length);
        if (appendStar) span[0] = (byte)'*';

        return new ValkeyChannel(arr, ValkeyChannelOptions.Pattern);
    }

    private static ValkeyChannel BuildKeyEventChannel(ReadOnlySpan<byte> type, int? database)
    {
        byte[] dbBytes = Encoding.UTF8.GetBytes(database?.ToString() ?? "*");
        byte[] arr = new byte[11 + dbBytes.Length + 3 + type.Length];
        var span = arr.AsSpan();

        "__keyevent@"u8.CopyTo(span); span = span.Slice(11);
        dbBytes.CopyTo(span); span = span.Slice(dbBytes.Length);
        "__:"u8.CopyTo(span); span = span.Slice(3);
        type.CopyTo(span);

        return new ValkeyChannel(arr, database is null ? ValkeyChannelOptions.Pattern : ValkeyChannelOptions.None);
    }

    private static ReadOnlySpan<byte> GetKeyNotificationWireName(KeyNotificationType type) => type switch
    {
        KeyNotificationType.Unknown => ""u8,
        KeyNotificationType.Append => "append"u8,
        KeyNotificationType.Copy => "copy"u8,
        KeyNotificationType.Del => "del"u8,
        KeyNotificationType.Expire => "expire"u8,
        KeyNotificationType.HDel => "hdel"u8,
        KeyNotificationType.HExpired => "hexpired"u8,
        KeyNotificationType.HIncrByFloat => "hincrbyfloat"u8,
        KeyNotificationType.HIncrBy => "hincrby"u8,
        KeyNotificationType.HPersist => "hpersist"u8,
        KeyNotificationType.HSet => "hset"u8,
        KeyNotificationType.IncrByFloat => "incrbyfloat"u8,
        KeyNotificationType.IncrBy => "incrby"u8,
        KeyNotificationType.LInsert => "linsert"u8,
        KeyNotificationType.LPop => "lpop"u8,
        KeyNotificationType.LPush => "lpush"u8,
        KeyNotificationType.LRem => "lrem"u8,
        KeyNotificationType.LSet => "lset"u8,
        KeyNotificationType.LTrim => "ltrim"u8,
        KeyNotificationType.MoveFrom => "move_from"u8,
        KeyNotificationType.MoveTo => "move_to"u8,
        KeyNotificationType.Persist => "persist"u8,
        KeyNotificationType.RenameFrom => "rename_from"u8,
        KeyNotificationType.RenameTo => "rename_to"u8,
        KeyNotificationType.Restore => "restore"u8,
        KeyNotificationType.RPop => "rpop"u8,
        KeyNotificationType.RPush => "rpush"u8,
        KeyNotificationType.SAdd => "sadd"u8,
        KeyNotificationType.Set => "set"u8,
        KeyNotificationType.SetRange => "setrange"u8,
        KeyNotificationType.SortStore => "sortstore"u8,
        KeyNotificationType.SRem => "srem"u8,
        KeyNotificationType.SPop => "spop"u8,
        KeyNotificationType.XAdd => "xadd"u8,
        KeyNotificationType.XDel => "xdel"u8,
        KeyNotificationType.XGroupCreateConsumer => "xgroup-createconsumer"u8,
        KeyNotificationType.XGroupCreate => "xgroup-create"u8,
        KeyNotificationType.XGroupDelConsumer => "xgroup-delconsumer"u8,
        KeyNotificationType.XGroupDestroy => "xgroup-destroy"u8,
        KeyNotificationType.XGroupSetId => "xgroup-setid"u8,
        KeyNotificationType.XSetId => "xsetid"u8,
        KeyNotificationType.XTrim => "xtrim"u8,
        KeyNotificationType.ZAdd => "zadd"u8,
        KeyNotificationType.ZDiffStore => "zdiffstore"u8,
        KeyNotificationType.ZInterStore => "zinterstore"u8,
        KeyNotificationType.ZUnionStore => "zunionstore"u8,
        KeyNotificationType.ZIncr => "zincr"u8,
        KeyNotificationType.ZRemByRank => "zrembyrank"u8,
        KeyNotificationType.ZRemByScore => "zrembyscore"u8,
        KeyNotificationType.ZRem => "zrem"u8,
        KeyNotificationType.Expired => "expired"u8,
        KeyNotificationType.Evicted => "evicted"u8,
        KeyNotificationType.New => "new"u8,
        KeyNotificationType.Overwritten => "overwritten"u8,
        KeyNotificationType.TypeChanged => "type_changed"u8,
        _ => throw new ArgumentOutOfRangeException(nameof(type)),
    };

    #endregion

    /// <summary>
    /// The matching pattern for this channel.
    /// </summary>
    public enum PatternMode
    {
        /// <summary>
        /// Will be treated as a pattern if it includes *.
        /// </summary>
        Auto = 0,

        /// <summary>
        /// Never a pattern.
        /// </summary>
        Literal = 1,

        /// <summary>
        /// Always a pattern.
        /// </summary>
        Pattern = 2,
    }

    /// <summary>
    /// Obtain the channel name as a <c>byte[]</c>.
    /// </summary>
    public static implicit operator byte[]?(ValkeyChannel key) => key.Value;

    /// <summary>
    /// Obtain the channel name as a <see cref="string"/>.
    /// </summary>
    public static implicit operator string?(ValkeyChannel key)
    {
        var arr = key.Value;
        if (arr is null)
        {
            return null;
        }
        try
        {
            return Encoding.UTF8.GetString(arr);
        }
        catch (Exception e) when (e is DecoderFallbackException or ArgumentException or ArgumentNullException)
        {
            return BitConverter.ToString(arr);
        }
    }

}
