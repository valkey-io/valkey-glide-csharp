// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using System;
using System.Text;

namespace Valkey.Glide;

/// <summary>
/// Represents a pub/sub channel name.
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
    /// Indicates whether this channel represents a shard channel (see <c>SSUBSCRIBE</c>).
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
    /// Creates a new <see cref="ValkeyChannel"/> that acts as a wildcard subscription.
    /// </summary>
    public static ValkeyChannel Pattern(string value) => new(value, ValkeyChannelOptions.Pattern);

    /// <summary>
    /// Creates a new <see cref="ValkeyChannel"/> that acts as a wildcard subscription.
    /// </summary>
    public static ValkeyChannel Pattern(byte[] value) => new(value, ValkeyChannelOptions.Pattern);

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
    public override int GetHashCode() => ValkeyValue.GetHashCode(Value) ^ (int)(Options);

    /// <summary>
    /// Obtains a string representation of the channel name.
    /// </summary>
    public override string ToString() => ((string?)this) ?? "(null)";

    internal void AssertNotNull()
    {
        if (IsNull) throw new ArgumentException("A null channel is not valid in this context");
    }

    internal ValkeyChannel Clone()
    {
        if (Value is null || Value.Length == 0)
        {
            return this;
        }
        var copy = (byte[])Value.Clone();
        return new ValkeyChannel(copy, Options);
    }

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

    // Not supported by Valkey GLIDE
    // -----------------------------

    [Obsolete("This method is not supported by Valkey GLIDE.", error: true)]
    public ValkeyChannel WithKeyRouting()
        => throw new NotSupportedException("This method is not supported by Valkey GLIDE.");

    [Obsolete("This method is not supported by Valkey GLIDE.", error: true)]
    public static implicit operator ValkeyChannel(string key)
        => throw new NotSupportedException("This method is not supported by Valkey GLIDE.");

    [Obsolete("This method is not supported by Valkey GLIDE.", error: true)]
    public static implicit operator ValkeyChannel(byte[]? key)
        => key is null ? default : new ValkeyChannel(key, s_DefaultPatternMode);
}
