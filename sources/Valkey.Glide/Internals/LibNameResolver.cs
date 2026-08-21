// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.Internals;

/// <summary>
/// Shared logic for composing the value reported via <c>CLIENT SETINFO LIB-NAME</c> from an
/// optional library-name override and an optional client-info tag. Used by every connection type
/// (standard/cluster clients and the MONITOR client) so the composition — including the
/// whitespace-only-tag handling — stays identical across them.
/// </summary>
internal static class LibNameResolver
{
    /// <summary>
    /// The default library name reported when no override is supplied.
    /// </summary>
    internal const string Default = "GlideC#";

    /// <summary>
    /// Composes the resolved library name.
    /// </summary>
    /// <param name="libraryName">Optional full override for the library name. Defaults to <see cref="Default"/>.</param>
    /// <param name="clientInfoTag">
    /// Optional tag appended in parentheses, e.g. <c>GlideC#(tag)</c>. A <see langword="null"/>, empty,
    /// or whitespace-only tag is treated as absent: <c>CLIENT SETINFO LIB-NAME</c> rejects values
    /// containing whitespace (and the core ignores that failure, dropping the whole lib-name), so a
    /// whitespace-only tag degrades to the base name rather than composing a value the server refuses.
    /// </param>
    /// <returns>The non-null resolved library name to send to the server.</returns>
    internal static string Resolve(string? libraryName, string? clientInfoTag)
    {
        string baseName = libraryName ?? Default;
        return string.IsNullOrWhiteSpace(clientInfoTag)
            ? baseName
            : $"{baseName}({clientInfoTag})";
    }
}
