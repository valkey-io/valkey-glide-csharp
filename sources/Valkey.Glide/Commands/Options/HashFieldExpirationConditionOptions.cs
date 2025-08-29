// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.Commands.Options;

/// <summary>
/// Optional arguments for hash field expiration commands (HEXPIRE, HPEXPIRE, HEXPIREAT, HPEXPIREAT).
/// </summary>
/// <remarks>
/// See <a href="https://valkey.io/commands/hexpire/">valkey.io</a>
/// </remarks>
public class HashFieldExpirationConditionOptions
{
    /// <summary>
    /// The expiration condition.
    /// </summary>
    public ExpireOptions? Condition { get; set; }

    /// <summary>
    /// Sets the expiration condition.
    /// </summary>
    /// <param name="condition">The expiration condition</param>
    /// <returns>This HashFieldExpirationConditionOptions instance for method chaining</returns>
    public HashFieldExpirationConditionOptions SetCondition(ExpireOptions condition)
    {
        Condition = condition;
        return this;
    }
}

/// <summary>
/// Expiration condition options for hash field expiration commands.
/// </summary>
public enum ExpireOptions
{
    /// <summary>
    /// Set expiration only if the field has no expiration.
    /// </summary>
    HAS_NO_EXPIRY,

    /// <summary>
    /// Set expiration only if the field has an existing expiration.
    /// </summary>
    HAS_EXISTING_EXPIRY,

    /// <summary>
    /// Set expiration only if the new expiration is greater than the current one.
    /// </summary>
    NEW_EXPIRY_GREATER_THAN_CURRENT,

    /// <summary>
    /// Set expiration only if the new expiration is less than the current one.
    /// </summary>
    NEW_EXPIRY_LESS_THAN_CURRENT
}
