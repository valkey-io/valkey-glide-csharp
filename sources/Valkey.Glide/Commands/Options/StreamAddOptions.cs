// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.Commands.Options;

/// <summary>
/// Optional arguments for the <c>XADD</c> command.
/// </summary>
/// <seealso href="https://valkey.io/commands/xadd/"/>
public sealed class StreamAddOptions
{
    #region Constants

    /// <summary>
    /// A sentinel value ("*") that instructs the server to auto-generate a unique entry ID.
    /// </summary>
    public static readonly ValkeyValue AutoGenerateId = ValkeyLiterals.StreamAutoGenerateId;

    #endregion
    #region Public Properties

    /// <summary>
    /// The stream entry ID. If set to <see cref="AutoGenerateId"/>, the server auto-generates a unique ID.
    /// </summary>
    public ValkeyValue Id { get; init; } = AutoGenerateId;

    /// <summary>
    /// If <see langword="true"/>, the stream will not be created when it does not already exist (NOMKSTREAM).
    /// </summary>
    public bool NoMakeStream { get; init; } = false;

    /// <summary>
    /// If set, the add operation will also trim older entries in the stream.
    /// </summary>
    public StreamTrimOptions? Trim { get; init; } = null;

    #endregion
    #region Internal Methods

    /// <summary>
    /// Builds the command arguments for these options.
    /// </summary>
    internal GlideString[] ToArgs()
    {
        List<GlideString> args = [];

        if (NoMakeStream)
        {
            args.Add(ValkeyLiterals.NOMKSTREAM.ToGlideString());
        }

        if (Trim is not null)
        {
            args.AddRange(Trim.ToArgs());
        }

        args.Add(Id.ToGlideString());

        return [.. args];
    }

    #endregion
}
