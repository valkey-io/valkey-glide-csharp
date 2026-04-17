// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.Commands.Options;

/// <summary>
/// Options for the scan commands.
/// </summary>
public class ScanOptions
{
    /// <summary>
    /// Pattern to filter keys against.
    /// </summary>
    public ValkeyValue MatchPattern { get; set; } = ValkeyValue.Null;

    /// <summary>
    /// Hint for the number of keys to return per iteration.
    /// </summary>
    public long? Count { get; set; }

    /// <summary>
    /// Type to filter keys against.
    /// </summary>
    public ValkeyType? Type { get; set; }
}
