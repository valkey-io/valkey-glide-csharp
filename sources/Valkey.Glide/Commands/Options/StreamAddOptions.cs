// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.Commands.Options;

/// <summary>
/// Optional arguments for the XADD command.
/// </summary>
/// <seealso href="https://valkey.io/commands/xadd/"/>
public record StreamAddOptions
{
    /// <summary>
    /// The entry ID to use. If <c>null</c>, the server auto-generates an ID ("*").
    /// Use "&lt;ms&gt;-*" to auto-generate the sequence number for a specific timestamp.
    /// </summary>
    public ValkeyValue? Id { get; init; }

    /// <summary>
    /// If <c>false</c>, the stream will not be created if it doesn't exist (NOMKSTREAM).
    /// Defaults to <c>true</c>.
    /// </summary>
    public bool MakeStream { get; init; } = true;

    /// <summary>
    /// If set, the add operation will also trim older entries in the stream.
    /// </summary>
    public StreamTrimOptions? Trim { get; init; }

    internal GlideString[] ToArgs()
    {
        List<GlideString> args = [];

        if (!MakeStream)
            args.Add(ValkeyLiterals.NOMKSTREAM.ToGlideString());

        if (Trim is not null)
            args.AddRange(Trim.ToArgs());

        args.Add(Id.HasValue && !Id.Value.IsNull ? Id.Value.ToGlideString() : ValkeyLiterals.Wildcard.ToGlideString());

        return [.. args];
    }
}
