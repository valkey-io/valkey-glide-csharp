// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Internals;

namespace Valkey.Glide.ServerModules;

public static partial class Ft
{
    #region Public Methods

    /// <summary>
    /// Runs an aggregate command against an index, requesting a paged reply via <c>WITHCURSOR</c>.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/ft.aggregate/">Valkey commands – FT.AGGREGATE</seealso>
    /// <param name="client">The client to execute the command.</param>
    /// <param name="index">The name of the index to aggregate.</param>
    /// <param name="query">The search query expression.</param>
    /// <param name="options">
    /// Options for the aggregation command. <see cref="AggregateOptions.WithCursorOptions"/> must be set.
    /// </param>
    /// <returns>
    /// A <see cref="CursorResult"/> with the first page of rows and the cursor id to pass to
    /// <see cref="CursorReadAsync"/> for subsequent pages (<c>0</c> once exhausted).
    /// </returns>
    /// <remarks>
    /// Routed to the PRIMARY of <paramref name="index"/>'s slot in cluster mode, regardless of the
    /// client's read-from configuration -- a cursor is a per-process, node-local resource.
    /// </remarks>
    public static Task<CursorResult> AggregateWithCursorAsync(BaseClient client, ValkeyKey index, ValkeyValue query, AggregateOptions options)
        => client.Command(Request.FtAggregateWithCursor(index, query, options), PrimaryRoute(index));

    /// <summary>
    /// Reads the next page of results from a cursor opened by <see cref="AggregateWithCursorAsync"/>
    /// or <see cref="HybridWithCursorAsync"/>.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/ft.cursor/">Valkey commands – FT.CURSOR</seealso>
    /// <param name="client">The client to execute the command.</param>
    /// <param name="index">The index name the cursor was opened against.</param>
    /// <param name="cursorId">
    /// The cursor id, from the opening command's reply or a previous <see cref="CursorReadAsync"/>
    /// call. <c>0</c> means the cursor is already exhausted; calling this with <c>0</c> is a caller
    /// error, not something this method special-cases.
    /// </param>
    /// <param name="count">
    /// Overrides how many rows this page returns, instead of the count the cursor was opened with.
    /// Pass <see langword="null"/> to leave it unset.
    /// </param>
    /// <returns>
    /// A <see cref="CursorResult"/> with the next page of rows and the cursor id to pass to the
    /// next <see cref="CursorReadAsync"/> call (<c>0</c> once exhausted).
    /// </returns>
    /// <remarks>
    /// Routed to the PRIMARY of <paramref name="index"/>'s slot in cluster mode, regardless of the
    /// client's read-from configuration -- a read that lands on a different node (a replica a
    /// PREFER_REPLICA/AZ-affinity client would otherwise have used) fails with "Cursor not found".
    /// </remarks>
    public static Task<CursorResult> CursorReadAsync(BaseClient client, ValkeyKey index, long cursorId, long? count = null)
        => client.Command(Request.FtCursorRead(index, cursorId, count), PrimaryRoute(index));

    /// <summary>
    /// Deletes an open cursor, discarding it before it would otherwise be exhausted or time out.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/ft.cursor/">Valkey commands – FT.CURSOR</seealso>
    /// <param name="client">The client to execute the command.</param>
    /// <param name="index">The index name the cursor was opened against.</param>
    /// <param name="cursorId">The cursor id to delete.</param>
    /// <remarks>
    /// Routed to the PRIMARY of <paramref name="index"/>'s slot in cluster mode, regardless of the
    /// client's read-from configuration. See <see cref="CursorReadAsync"/>'s remarks.
    /// </remarks>
    public static Task CursorDelAsync(BaseClient client, ValkeyKey index, long cursorId)
        => client.Command(Request.FtCursorDel(index, cursorId), PrimaryRoute(index));

    #endregion
    #region Private Methods

    /// <summary>
    /// A route pinning a command to the PRIMARY of <paramref name="index"/>'s slot, used for every
    /// cursor-bearing command (FT.AGGREGATE/FT.HYBRID WITHCURSOR, FT.CURSOR READ/DEL) since a
    /// cursor is a per-process, node-local resource. No-op (ignored) against a standalone client.
    /// </summary>
    /// <param name="index">The index whose slot the command should be routed to.</param>
    /// <remarks>
    /// L9 (deferred, see QA disposition): hashes <paramref name="index"/> via
    /// <see cref="ValkeyKey.ToString"/> (UTF-8 text), not its raw bytes, so a binary (non-UTF-8)
    /// index name would compute a different CRC16 slot than the one glide-core's own routing
    /// layer computes from the raw bytes for the index's actual key traffic (FT.CREATE,
    /// HSET/HSETNX documents under it, etc.) -- misrouting the cursor request to the wrong node's
    /// primary. Not fixed in this PR: fixing it correctly requires either a byte-based
    /// <c>Route.SlotKeyRoute</c> overload (a public API addition to <c>Route</c>, affecting every
    /// command that takes a route, not just FT.*) or reimplementing glide-core's CRC16 hashtag
    /// algorithm independently in C# -- the latter is a real correctness risk to introduce without
    /// a reference implementation on hand to test it against, and the former is bigger than this
    /// PR's scope. A binary index name is already an unusual choice (FT.CREATE's own <c>index</c>
    /// argument is conventionally a readable name, unlike a document key), so the practical
    /// exposure is narrow.
    /// </remarks>
    private static Route PrimaryRoute(ValkeyKey index)
        => new Route.SlotKeyRoute(index.ToString() ?? string.Empty, Route.SlotType.Primary);

    #endregion
    #region Nested Types

    /// <summary>
    /// A paged reply from FT.AGGREGATE/FT.HYBRID with <c>WITHCURSOR</c>, or from FT.CURSOR READ.
    /// </summary>
    public sealed class CursorResult
    {
        /// <summary>
        /// The result rows for this page, in the same shape FT.AGGREGATE/FT.HYBRID return without
        /// a cursor.
        /// </summary>
        public required IDictionary<ValkeyValue, ValkeyValue>[] Rows { get; init; }

        /// <summary>
        /// The cursor id to pass to the next <see cref="CursorReadAsync"/> call,
        /// or <c>0</c> once the cursor is exhausted.
        /// </summary>
        public required long CursorId { get; init; }
    }

    #endregion
}
