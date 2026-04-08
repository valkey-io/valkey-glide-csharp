// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.Commands.Options;

/// <summary>
/// The condition for a command to the set the expiry for a key or field.
/// </summary>
/// <seealso href="https://valkey.io/commands/expire/"/>
/// <seealso href="https://valkey.io/commands/expireat/"/>
/// <seealso href="https://valkey.io/commands/pexpire/"/>
/// <seealso href="https://valkey.io/commands/pexpireat/"/>
/// <seealso href="https://valkey.io/commands/hexpire/"/>
/// <seealso href="https://valkey.io/commands/hexpireat/"/>
/// <seealso href="https://valkey.io/commands/hpexpire/"/>
/// <seealso href="https://valkey.io/commands/hpexpireat/"/>
public enum ExpireCondition
{
    /// <summary>
    /// Always set the expiry.
    /// </summary>
    Always,

    /// <summary>
    /// Set expiry only if the key or field has no existing expiry (NX).
    /// </summary>
    OnlyIfNotExists,

    /// <summary>
    /// Set expiry only if the key or field has an existing expiry (XX).
    /// </summary>
    OnlyIfExists,

    /// <summary>
    /// Set expiry only if the new expiry is greater than the current expiry (GT).
    /// </summary>
    OnlyIfGreaterThan,

    /// <summary>
    /// Set expiry only if the new expiry is less than the current expiry (LT).
    /// </summary>
    OnlyIfLessThan,
}
