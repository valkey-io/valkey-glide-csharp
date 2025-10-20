// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.Commands.Options;

/// <summary>
/// Options for BITFIELD command operations.
/// </summary>
public static class BitFieldOptions
{
    /// <summary>
    /// Base interface for BitField subcommands.
    /// </summary>
    public interface IBitFieldSubCommand
    {
        /// <summary>
        /// Converts the subcommand to its string arguments.
        /// </summary>
        /// <returns>Array of string arguments for the subcommand.</returns>
        string[] ToArgs();
    }

    /// <summary>
    /// Interface for read-only BitField subcommands.
    /// </summary>
    public interface IBitFieldReadOnlySubCommand : IBitFieldSubCommand
    {
    }

    /// <summary>
    /// Interface for bit field offsets.
    /// </summary>
    public interface IBitOffset
    {
        string GetOffset();
    }

    /// <summary>
    /// Regular bit offset.
    /// </summary>
    public class BitOffset : IBitOffset
    {
        private readonly long _offset;

        public BitOffset(long offset) => _offset = offset;
        public string GetOffset() => _offset.ToString();
    }

    /// <summary>
    /// Offset multiplied by encoding width (prefixed with #).
    /// </summary>
    public class BitOffsetMultiplier : IBitOffset
    {
        private readonly long _multiplier;

        public BitOffsetMultiplier(long multiplier) => _multiplier = multiplier;
        public string GetOffset() => $"#{_multiplier}";
    }

    /// <summary>
    /// GET subcommand for reading bits from the string.
    /// </summary>
    public class BitFieldGet : IBitFieldReadOnlySubCommand
    {
        private readonly string _encoding;
        private readonly IBitOffset _offset;

        public BitFieldGet(string encoding, IBitOffset offset)
        {
            _encoding = encoding;
            _offset = offset;
        }

        public string[] ToArgs() => ["GET", _encoding, _offset.GetOffset()];
    }

    /// <summary>
    /// SET subcommand for setting bits in the string.
    /// </summary>
    public class BitFieldSet : IBitFieldSubCommand
    {
        private readonly string _encoding;
        private readonly IBitOffset _offset;
        private readonly long _value;

        public BitFieldSet(string encoding, IBitOffset offset, long value)
        {
            _encoding = encoding;
            _offset = offset;
            _value = value;
        }

        public string[] ToArgs() => ["SET", _encoding, _offset.GetOffset(), _value.ToString()];
    }

    /// <summary>
    /// INCRBY subcommand for incrementing bits in the string.
    /// </summary>
    public class BitFieldIncrBy : IBitFieldSubCommand
    {
        private readonly string _encoding;
        private readonly IBitOffset _offset;
        private readonly long _increment;

        public BitFieldIncrBy(string encoding, IBitOffset offset, long increment)
        {
            _encoding = encoding;
            _offset = offset;
            _increment = increment;
        }

        public string[] ToArgs() => ["INCRBY", _encoding, _offset.GetOffset(), _increment.ToString()];
    }

    /// <summary>
    /// OVERFLOW subcommand for controlling overflow behavior.
    /// </summary>
    public class BitFieldOverflow : IBitFieldSubCommand
    {
        private readonly OverflowType _overflowType;

        /// <summary>
        /// Creates an OVERFLOW subcommand.
        /// </summary>
        /// <param name="overflowType">The overflow behavior type.</param>
        public BitFieldOverflow(OverflowType overflowType)
        {
            _overflowType = overflowType;
        }

        public string[] ToArgs() => ["OVERFLOW", _overflowType.ToString().ToUpper()];
    }

    /// <summary>
    /// Overflow behavior types for BitField operations.
    /// </summary>
    public enum OverflowType
    {
        /// <summary>
        /// Wrap around on overflow (modulo arithmetic).
        /// </summary>
        Wrap,
        /// <summary>
        /// Saturate at min/max values on overflow.
        /// </summary>
        Sat,
        /// <summary>
        /// Return null on overflow.
        /// </summary>
        Fail
    }

    /// <summary>
    /// Helper methods for creating common encodings.
    /// </summary>
    public static class Encoding
    {
        /// <summary>
        /// Creates an unsigned encoding string.
        /// </summary>
        /// <param name="bits">Number of bits (1-63).</param>
        /// <returns>Unsigned encoding string (e.g., "u8").</returns>
        public static string Unsigned(int bits) => $"u{bits}";

        /// <summary>
        /// Creates a signed encoding string.
        /// </summary>
        /// <param name="bits">Number of bits (1-64).</param>
        /// <returns>Signed encoding string (e.g., "i8").</returns>
        public static string Signed(int bits) => $"i{bits}";
    }


}