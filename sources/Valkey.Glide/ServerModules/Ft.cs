// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Internals;

namespace Valkey.Glide.ServerModules;

/// <summary>
/// Valkey search commands for clients.
/// </summary>
/// <seealso href="https://valkey.io/docs/topics/search/">Valkey Search – Overview</seealso>
public static partial class Ft
{
    #region Public Methods

    /// <summary>
    /// Drops a search index.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/ft.dropindex/">Valkey commands – FT.DROPINDEX</seealso>
    /// <param name="client">The client to execute the command.</param>
    /// <param name="index">The index to drop.</param>
    /// <remarks>
    /// <example>
    /// <code>
    /// await Ft.CreateAsync(client, "my-index", new Ft.CreateTextField("title"));
    /// await Ft.DropIndexAsync(client, "my-index");
    /// </code>
    /// </example>
    /// </remarks>
    public static Task DropIndexAsync(BaseClient client, ValkeyKey index)
        => client.Command(Request.FtDropIndex(index));

    /// <summary>
    /// Returns all index names.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/ft._list/">Valkey commands – FT._LIST</seealso>
    /// <param name="client">The client to execute the command.</param>
    /// <returns>The set of index names.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await Ft.CreateAsync(client, "my-index", new Ft.CreateTextField("title"));
    /// var indexes = await Ft.ListAsync(client);  // {"my-index"}
    /// </code>
    /// </example>
    /// </remarks>
    public static Task<ISet<ValkeyValue>> ListAsync(BaseClient client)
        => client.Command(Request.FtList());

    #endregion
}
