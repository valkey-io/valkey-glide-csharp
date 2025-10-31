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
    /// <param name="offset">The bit offset value.</param>
    public class BitOffset(long offset) : IBitOffset
    {
        public string GetOffset() => offset.ToString();
    }

    /// <summary>
    /// Offset multiplied by encoding width (prefixed with #).
    /// </summary>
    /// <param name="multiplier">The multiplier value.</param>
    public class BitOffsetMultiplier(long multiplier) : IBitOffset
    {
        public string GetOffset() => $"#{multiplier}";
    }

    /// <summary>
    /// GET subcommand for reading bits from the string.
    /// </summary>
    /// <param name="encoding">The bit field encoding.</param>
    /// <param name="offset">The bit field offset.</param>
    public class BitFieldGet(string encoding, IBitOffset offset) : IBitFieldReadOnlySubCommand
    {
        public string[] ToArgs() => ["GET", encoding, offset.GetOffset()];
    }

    /// <summary>
    /// SET subcommand for setting bits in the string.
    /// </summary>
    /// <param name="encoding">The bit field encoding.</param>
    /// <param name="offset">The bit field offset.</param>
    /// <param name="value">The value to set.</param>
    public class BitFieldSet(string encoding, IBitOffset offset, long value) : IBitFieldSubCommand
    {
        public string[] ToArgs() => ["SET", encoding, offset.GetOffset(), value.ToString()];
    }

    /// <summary>
    /// INCRBY subcommand for incrementing bits in the string.
    /// </summary>
    /// <param name="encoding">The bit field encoding.</param>
    /// <param name="offset">The bit field offset.</param>
    /// <param name="increment">The increment value.</param>
    public class BitFieldIncrBy(string encoding, IBitOffset offset, long increment) : IBitFieldSubCommand
    {
        public string[] ToArgs() => ["INCRBY", encoding, offset.GetOffset(), increment.ToString()];
    }

    /// <summary>
    /// OVERFLOW subcommand for controlling overflow behavior.
    /// </summary>
    /// <param name="overflowType">The overflow behavior type.</param>
    public class BitFieldOverflow(OverflowType overflowType) : IBitFieldSubCommand
    {
        public string[] ToArgs() => ["OVERFLOW", overflowType.ToString().ToUpper()];
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
