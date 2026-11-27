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
    /// Options for the aggregation command. <see cref="AggregateOptions.WithCursorOptions"/> is optional;
    /// when it is null the cursor opens with server defaults.
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
        => client.Command(Request.FtAggregateWithCursor(index, query, options));

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
        => client.Command(Request.FtCursorRead(index, cursorId, count));

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
        => client.Command(Request.FtCursorDel(index, cursorId));

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
