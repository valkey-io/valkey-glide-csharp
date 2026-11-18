// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide;

/// <summary>
/// The type of save operation to perform.
/// </summary>
public enum SaveType
{
    /// <summary>
    /// Rewrite the append-only file in the background.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/bgrewriteaof/"/>
    BackgroundRewriteAppendOnlyFile,

    /// <summary>
    /// Save the database in the background.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/bgsave/"/>
    BackgroundSave,

    /// <summary>
    /// Save the database in the foreground, blocking the server until complete.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/save/"/>
    ForegroundSave,
}
