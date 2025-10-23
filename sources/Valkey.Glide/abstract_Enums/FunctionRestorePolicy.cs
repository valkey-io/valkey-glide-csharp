// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide;

/// <summary>
/// Policy for restoring function libraries.
/// </summary>
public enum FunctionRestorePolicy
{
    /// <summary>
    /// Append functions without replacing existing ones.
    /// Throws error if library already exists.
    /// </summary>
    Append,

    /// <summary>
    /// Delete all existing functions before restoring.
    /// </summary>
    Flush,

    /// <summary>
    /// Overwrite conflicting functions.
    /// </summary>
    Replace
}
