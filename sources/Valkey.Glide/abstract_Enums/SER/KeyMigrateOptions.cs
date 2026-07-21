using System;

namespace Valkey.Glide;

/// <summary>
/// Additional options for the <see cref="IDatabaseAsync.KeyMigrateAsync"/> command.
/// </summary>
[Flags]
public enum KeyMigrateOptions
{
    /// <summary>
    /// No options specified.
    /// </summary>
    None = 0,

    /// <summary>
    /// Do not remove the key from the local instance.
    /// </summary>
    Copy = 1,

    /// <summary>
    /// Replace existing key on the remote instance.
    /// </summary>
    Replace = 2,
}
