// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using static Valkey.Glide.Commands.Constants.Constants;

namespace Valkey.Glide.Commands.Options;

/// <summary>
/// The options for an operation to set hash field value(s).
/// </summary>
/// <seealso href="https://valkey.io/commands/hsetex/"/>
public sealed class HashSetOptions : Options
{
    #region Public Properties

    /// <summary>
    /// The condition under which to set the field(s).
    /// </summary>
    public HashSetCondition Condition { get; init; } = HashSetCondition.Always;

    /// <summary>
    /// The expiry configuration for the field(s). If <see langword="null"/>, no expiry is set.
    /// </summary>
    public SetExpiryOptions? Expiry { get; init; }

    #endregion
    #region Internal Methods

    /// <inheritdoc/>
    internal override GlideString[] ToArgs()
    {
        List<GlideString> args = [];

        if (Condition == HashSetCondition.OnlyIfNoneExist)
        {
            args.Add(FnxKeyword);
        }
        else if (Condition == HashSetCondition.OnlyIfAllExist)
        {
            args.Add(FxxKeyword);
        }

        if (Expiry is not null)
        {
            args.AddRange(Expiry.ToArgs());
        }

        return [.. args];
    }

    #endregion
}
