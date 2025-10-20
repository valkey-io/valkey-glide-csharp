// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide;

/// <summary>
/// Options for the GEOADD command.
/// </summary>
public class GeoAddOptions
{
    /// <summary>
    /// Gets the conditional change option.
    /// </summary>
    public ConditionalChange? ConditionalChange { get; }

    /// <summary>
    /// Gets whether to return the count of changed elements instead of added elements.
    /// </summary>
    public bool Changed { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="GeoAddOptions"/> class with conditional change option.
    /// </summary>
    /// <param name="conditionalChange">The conditional change option.</param>
    public GeoAddOptions(ConditionalChange conditionalChange)
    {
        ConditionalChange = conditionalChange;
        Changed = false;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GeoAddOptions"/> class with changed option.
    /// </summary>
    /// <param name="changed">Whether to return the count of changed elements.</param>
    public GeoAddOptions(bool changed)
    {
        ConditionalChange = null;
        Changed = changed;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GeoAddOptions"/> class with both options.
    /// </summary>
    /// <param name="conditionalChange">The conditional change option.</param>
    /// <param name="changed">Whether to return the count of changed elements.</param>
    public GeoAddOptions(ConditionalChange conditionalChange, bool changed)
    {
        ConditionalChange = conditionalChange;
        Changed = changed;
    }
}
