// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands.Options;

namespace Valkey.Glide;

public partial interface IBaseClient
{
    /// <summary>
    /// Sets the expiry duration for the specified hash field.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/hpexpire"/>
    /// <param name="key">The key of the hash.</param>
    /// <param name="hashField">The field to set the expiry for.</param>
    /// <param name="expiry">The expiry duration.</param>
    /// <param name="condition">The condition under which to set the expiry.</param>
    /// <returns>An <see cref="ExpireResult"/> array with one entry per field.</returns>
    Task<ExpireResult[]> HashExpireAsync(
        ValkeyKey key,
        ValkeyValue hashField,
        TimeSpan expiry,
        ExpireCondition condition = ExpireCondition.Always);

    /// <inheritdoc cref="HashExpireAsync(ValkeyKey, ValkeyValue, TimeSpan, ExpireCondition)"/>
    /// <param name="hashFields">The fields to set the expiry for.</param>
    Task<ExpireResult[]> HashExpireAsync(
        ValkeyKey key,
        IEnumerable<ValkeyValue> hashFields,
        TimeSpan expiry,
        ExpireCondition condition = ExpireCondition.Always);

    /// <summary>
    /// Sets the expiry timestamp for the specified hash field.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/hpexpireat"/>
    /// <param name="key">The key of the hash.</param>
    /// <param name="hashField">The field to set the expiry for.</param>
    /// <param name="expiry">The expiry timestamp.</param>
    /// <param name="condition">The condition under which to set the expiry.</param>
    /// <returns>An <see cref="ExpireResult"/> array with one entry per field.</returns>
    Task<ExpireResult[]> HashExpireAtAsync(
        ValkeyKey key,
        ValkeyValue hashField,
        DateTimeOffset expiry,
        ExpireCondition condition = ExpireCondition.Always);

    /// <inheritdoc cref="HashExpireAtAsync(ValkeyKey, ValkeyValue, DateTimeOffset, ExpireCondition)"/>
    /// <param name="hashFields">The fields to set the expiry for.</param>
    Task<ExpireResult[]> HashExpireAtAsync(
        ValkeyKey key,
        IEnumerable<ValkeyValue> hashFields,
        DateTimeOffset expiry,
        ExpireCondition condition = ExpireCondition.Always);

    /// <summary>
    /// Gets the expiry time for the specified hash field.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/hpexpiretime"/>
    /// <param name="key">The key of the hash.</param>
    /// <param name="hashField">The field to get the expiry time for.</param>
    /// <returns>An <see cref="ExpireTimeResult"/> for the hash field.</returns>
    Task<ExpireTimeResult> HashExpireTimeAsync(ValkeyKey key, ValkeyValue hashField);

    /// <inheritdoc cref="HashExpireTimeAsync(ValkeyKey, ValkeyValue)"/>
    /// <param name="hashFields">The fields to get the expiry time for.</param>
    /// <returns>An <see cref="ExpireTimeResult"/> array with one entry for each hash field.</returns>
    Task<ExpireTimeResult[]> HashExpireTimeAsync(ValkeyKey key, IEnumerable<ValkeyValue> hashFields);
}
