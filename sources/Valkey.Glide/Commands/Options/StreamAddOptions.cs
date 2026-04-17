// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.Commands.Options;

/// <summary>
/// Optional arguments for the XADD command.
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
    /// The stream entry ID. If set to <see cref="AutoGenerateId"/> the server auto-generates a unique ID.
    /// A well-formed ID consists of two 64-bit numbers separated by "-" (e.g., "1526919030474-55").
    /// An incomplete ID with only the milliseconds part (e.g., "{ms}-*") will have its sequence number auto-generated.
    /// </summary>
    public ValkeyValue Id { get; init; } = AutoGenerateId;

    /// <summary>
    /// If <see langword="false"/>, a new stream will not be created if no stream matches the given key.
    /// </summary>
    public bool MakeStream { get; init; } = true;

    /// <summary>
    /// If set, the add operation will also trim older entries in the stream.
    /// </summary>
    public StreamTrimOptions? Trim { get; init; } = null;

    #endregion
    #region Internal Methods

    /// <inheritdoc/>
    internal GlideString[] ToArgs()
    {
        List<GlideString> args = [];

        if (!MakeStream)
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
