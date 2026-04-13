// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide;

/// <summary>
/// Determines how stream trimming should be performed.
/// </summary>
public enum StreamTrimMode
{
    /// <summary>
    /// Trims the stream according to the specified policy (MAXLEN or MINID) regardless of whether
    /// entries are referenced by any consumer groups, but preserves existing references to these
    /// entries in all consumer groups' PEL.
    /// </summary>
    KeepReferences = 0,
}
