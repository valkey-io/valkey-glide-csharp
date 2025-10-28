// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide;

/// <summary>
/// Policy for restoring functions from a backup.
/// </summary>
public enum FunctionRestorePolicy
{
    /// <summary>
    /// Append functions without replacing existing ones. Fails if a library already exists.
    /// </summary>
    Append,

    /// <summary>
    /// Delete all existing functions before restoring.
    /// </summary>
    Flush,

    /// <summary>
    /// Overwrite conflicting functions with the restored versions.
    /// </summary>
    Replace
}
