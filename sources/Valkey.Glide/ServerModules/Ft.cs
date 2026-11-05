// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Internals;

namespace Valkey.Glide.ServerModules;

/// <summary>
/// Valkey search commands for clients.
/// </summary>
/// <seealso href="https://valkey.io/docs/topics/search/">Valkey Search – Overview</seealso>
public static partial class Ft
{
    #region Private Fields

    private static readonly HashSet<string> SupportedLanguages =
    [
        with(StringComparer.OrdinalIgnoreCase),
        "arabic", "dutch", "english", "french", "german", "indonesian",
        "italian", "portuguese", "russian", "spanish", "swedish", "turkish",
    ];

    #endregion
    #region Public Methods

    /// <summary>
    /// Drops a search index.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/ft.dropindex/">Valkey commands – FT.DROPINDEX</seealso>
    /// <param name="client">The client to execute the command.</param>
    /// <param name="index">The index to drop.</param>
    /// <example>
    /// <code>
    /// await Ft.CreateAsync(client, "my-index", new Ft.CreateTextField("title"));
    /// await Ft.DropIndexAsync(client, "my-index");
    /// </code>
    /// </example>
    public static Task DropIndexAsync(BaseClient client, ValkeyKey index)
        => client.Command(Request.FtDropIndex(index));

    /// <summary>
    /// Returns all index names.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/ft._list/">Valkey commands – FT._LIST</seealso>
    /// <param name="client">The client to execute the command.</param>
    /// <returns>The set of index names.</returns>
    /// <example>
    /// <code>
    /// await Ft.CreateAsync(client, "my-index", new Ft.CreateTextField("title"));
    /// var indexes = await Ft.ListAsync(client);  // {"my-index"}
    /// </code>
    /// </example>
    public static Task<ISet<ValkeyValue>> ListAsync(BaseClient client)
        => client.Command(Request.FtList());

    #endregion
    #region Internal Methods

    /// <summary>
    /// Guards a SCORER value client-side (M3: "the server rejects X" docstrings require a
    /// matching client-side guard). <c>BM25STD</c> is the only scorer valkey-search 1.3.0
    /// currently registers; anything else is rejected by the server outright.
    /// </summary>
    /// <param name="scorer">The scorer value to check.</param>
    /// <exception cref="ArgumentException">Thrown if <paramref name="scorer"/> is set and is not <c>BM25STD</c>.</exception>
    internal static void EnsureSupportedScorer(ValkeyValue scorer)
    {
        if (!scorer.IsNull && !string.Equals(scorer.ToString(), "BM25STD", StringComparison.Ordinal))
        {
            throw new ArgumentException(
                $"Unsupported scorer '{scorer}' -- valkey-search 1.3.0 only registers 'BM25STD'; the server rejects any other value.");
        }
    }

    /// <summary>
    /// Guards a DIALECT value client-side (M3). FT.HYBRID is fixed at dialect 2 server-side;
    /// passing any other value is rejected by the server.
    /// </summary>
    /// <param name="dialect">The dialect value to check.</param>
    /// <exception cref="ArgumentException">Thrown if <paramref name="dialect"/> is set and is not <c>2</c>.</exception>
    internal static void EnsureSupportedHybridDialect(int? dialect)
    {
        if (dialect.HasValue && dialect.Value != 2)
        {
            throw new ArgumentException(
                $"Unsupported DIALECT {dialect} for FT.HYBRID -- it is fixed at dialect 2 server-side; the server rejects any other value.");
        }
    }

    /// <summary>
    /// Guards a LANGUAGE value client-side (M3). valkey-search 1.3.0-rc1 registers exactly these
    /// 12 stemmer languages (<c>src/indexes/text/language_registry.cc</c>); the server rejects
    /// any other value.
    /// </summary>
    /// <param name="language">The language value to check.</param>
    /// <exception cref="ArgumentException">Thrown if <paramref name="language"/> is set and is not one of the server's registered languages.</exception>
    internal static void EnsureSupportedLanguage(ValkeyValue language)
    {
        if (language.IsNull)
        {
            return;
        }

        string value = language.ToString();
        if (!SupportedLanguages.Contains(value))
        {
            throw new ArgumentException(
                $"Unsupported LANGUAGE '{value}' -- valkey-search 1.3.0-rc1 only registers: " +
                    $"{string.Join(", ", SupportedLanguages)}; the server rejects any other value.");
        }
    }

    #endregion
}
