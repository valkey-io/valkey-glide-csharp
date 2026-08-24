// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands.Options;

using static Valkey.Glide.Commands.Options.InfoOptions;

namespace Valkey.Glide;

// ATTENTION: Methods should only be added to this interface if they are implemented by Valkey GLIDE clients
// but NOT by StackExchange.Redis databases. Methods implemented by both should be added to the corresponding
// Commands interface instead.

/// <summary>
/// Server management commands for Valkey GLIDE standalone client.
/// </summary>
/// <seealso href="https://valkey.io/commands/#server">Valkey – Server Management Commands</seealso>
public partial interface IGlideClient
{
    /// <summary>
    /// Asynchronously saves the dataset to disk in the background.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/bgsave/">Valkey commands – BGSAVE</seealso>
    /// <returns>A status string.</returns>
    /// <example>
    /// <code>
    /// var response = await client.BackgroundSaveAsync();
    /// Console.WriteLine(response); // "Background saving started"
    /// </code>
    /// </example>
    Task<string> BackgroundSaveAsync();

    /// <summary>
    /// Aborts all in-progress and scheduled background saves.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/bgsave/">Valkey commands – BGSAVE</seealso>
    /// <returns>A status string.</returns>
    /// <exception cref="Errors.RequestException">Thrown if no background save is currently in progress or scheduled.</exception>
    /// <example>
    /// <code>
    /// var response = await client.BackgroundSaveCancelAsync();
    /// Console.WriteLine(response); // "Background saving cancelled"
    /// </code>
    /// </example>
    /// <remarks>
    /// <para>Since Valkey 8.1.</para>
    /// </remarks>
    Task<string> BackgroundSaveCancelAsync();

    /// <summary>
    /// Schedules a background save of the database.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/bgsave/">Valkey commands – BGSAVE</seealso>
    /// <returns>A status string.</returns>
    /// <example>
    /// <code>
    /// var response = await client.BackgroundSaveScheduleAsync();
    /// Console.WriteLine(response); // "Background saving scheduled"
    /// </code>
    /// </example>
    Task<string> BackgroundSaveScheduleAsync();

    /// <summary>
    /// Initiates a background rewrite of the append-only file (AOF).
    /// </summary>
    /// <seealso href="https://valkey.io/commands/bgrewriteaof/">Valkey commands – BGREWRITEAOF</seealso>
    /// <returns>A status string.</returns>
    /// <example>
    /// <code>
    /// var response = await client.BgRewriteAofAsync();
    /// Console.WriteLine(response); // "Background append only file rewriting started"
    /// </code>
    /// </example>
    Task<string> BgRewriteAofAsync();

    /// <summary>
    /// Gets the values of configuration parameters.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/config-get/">Valkey commands – CONFIG GET</seealso>
    /// <param name="pattern">The pattern of config values to get.</param>
    /// <returns>All matching configuration parameters.</returns>
    /// <example>
    /// <code>
    /// KeyValuePair&lt;string, string&gt;[] config = await client.ConfigGetAsync("max*");
    /// </code>
    /// </example>
    Task<KeyValuePair<string, string>[]> ConfigGetAsync(ValkeyValue pattern = default);

    /// <summary>
    /// Resets the statistics reported by the server using the INFO and LATENCY HISTOGRAM.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/config-resetstat/">Valkey commands – CONFIG RESETSTAT</seealso>
    /// <example>
    /// <code>
    /// await client.ConfigResetStatisticsAsync();
    /// </code>
    /// </example>
    Task ConfigResetStatisticsAsync();

    /// <summary>
    /// Rewrites the <c>valkey.conf</c> file the server was started with, applying the minimal changes needed
    /// to make it reflect the configuration currently used by the server, which may differ from the original
    /// because of the use of the <c>CONFIG SET</c> command.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/config-rewrite/">Valkey commands – CONFIG REWRITE</seealso>
    /// <example>
    /// <code>
    /// await client.ConfigRewriteAsync();
    /// </code>
    /// </example>
    Task ConfigRewriteAsync();

    /// <summary>
    /// Reconfigures the server at runtime without the need to restart Valkey. Callers can change trivial
    /// parameters or switch from one persistence option to another using this command.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/config-set/">Valkey commands – CONFIG SET</seealso>
    /// <param name="setting">The setting name.</param>
    /// <param name="value">The new setting value.</param>
    /// <example>
    /// <code>
    /// await client.ConfigSetAsync("maxmemory", "100mb");
    /// </code>
    /// </example>
    Task ConfigSetAsync(ValkeyValue setting, ValkeyValue value);

    /// <summary>
    /// Returns the number of keys in the currently-selected database.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/dbsize/">Valkey commands – DBSIZE</seealso>
    /// <returns>The number of keys in the currently-selected database.</returns>
    /// <example>
    /// <code>
    /// long keyCount = await client.DatabaseSizeAsync();
    /// </code>
    /// </example>
    Task<long> DatabaseSizeAsync();

    /// <summary>
    /// Starts a coordinated failover from the connected primary to one of its replicas.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/failover/">Valkey commands – FAILOVER</seealso>
    /// <example>
    /// <code>
    /// await client.FailoverAsync();
    /// </code>
    /// </example>
    Task FailoverAsync();

    /// <summary>
    /// Starts a coordinated failover from the connected primary to one of its replicas.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/failover/">Valkey commands – FAILOVER</seealso>
    /// <param name="options">The failover options.</param>
    /// <example>
    /// <code>
    /// await client.FailoverAsync(FailoverOptions.Abort());
    /// </code>
    /// </example>
    /// <example>
    /// <code>
    /// await client.FailoverAsync(FailoverOptions.To("localhost", 6380, TimeSpan.FromSeconds(1)));
    /// </code>
    /// </example>
    Task FailoverAsync(FailoverOptions options);

    /// <summary>
    /// Deletes all the keys of all the existing databases.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/flushall/">Valkey commands – FLUSHALL</seealso>
    /// <example>
    /// <code>
    /// await client.FlushAllDatabasesAsync();
    /// </code>
    /// </example>
    Task FlushAllDatabasesAsync();

    /// <summary>
    /// Deletes all the keys of the currently selected database.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/flushdb/">Valkey commands – FLUSHDB</seealso>
    /// <example>
    /// <code>
    /// await client.FlushDatabaseAsync();
    /// </code>
    /// </example>
    Task FlushDatabaseAsync();

    /// <summary>
    /// Get information and statistics about the server using <see cref="Section.DEFAULT" /> option.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/info/">Valkey commands – INFO</seealso>
    /// <returns>A <see langword="string" /> containing the information for the sections requested.</returns>
    /// <example>
    /// <code>
    /// string info = await client.InfoAsync();
    /// </code>
    /// </example>
    Task<string> InfoAsync();

    /// <summary>
    /// Get information and statistics about the server.<br />
    /// Starting from server version 7, command supports multiple <see cref="Section" /> arguments.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/info/">Valkey commands – INFO</seealso>
    /// <param name="sections">A list of <see cref="Section" /> values specifying which sections of information to
    /// retrieve. When no parameter is provided, the <see cref="Section.DEFAULT" /> option is assumed.</param>
    /// <returns>
    /// <inheritdoc cref="InfoAsync()" path="/returns" />
    /// </returns>
    /// <example>
    /// <code>
    /// Section[] sections = [Section.SERVER, Section.MEMORY];
    /// string info = await client.InfoAsync(sections);
    /// </code>
    /// </example>
    Task<string> InfoAsync(IEnumerable<Section> sections);

    /// <summary>
    /// Returns the time of the last DB save executed with success.
    /// A client may check if a <c>BGSAVE</c> command succeeded by reading the <c>LASTSAVE</c> value, then
    /// issuing a <c>BGSAVE</c> command and checking at regular intervals whether <c>LASTSAVE</c> changed.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/lastsave/">Valkey commands – LASTSAVE</seealso>
    /// <returns>UNIX time of the last DB save executed with success.</returns>
    /// <example>
    /// <code>
    /// var lastSave = await client.LastSaveAsync();
    /// </code>
    /// </example>
    Task<DateTimeOffset> LastSaveAsync();

    /// <summary>
    /// Returns latency spike time series for the specified event.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/latency-history/">Valkey commands – LATENCY HISTORY</seealso>
    /// <param name="event">The name of the event to get latency history for.</param>
    /// <returns>An array of <see cref="LatencyEntry"/> representing the latency spike time series for the event.
    /// Returns an empty array if the event does not exist.</returns>
    /// <example>
    /// <code>
    /// var history = await client.LatencyHistoryAsync("command");
    /// foreach (var entry in history)
    /// {
    ///     Console.WriteLine($"Time: {entry.Time}, Duration: {entry.Duration.TotalMilliseconds}ms");
    /// }
    /// </code>
    /// </example>
    Task<LatencyEntry[]> LatencyHistoryAsync(ValkeyValue @event);

    /// <summary>
    /// Reports the latest latency events logged by the server.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/latency-latest/">Valkey commands – LATENCY LATEST</seealso>
    /// <returns>An array of <see cref="LatencyEventInfo"/> for the latest latency events.</returns>
    /// <example>
    /// <code>
    /// var latest = await client.LatencyLatestAsync();
    /// foreach (var info in latest)
    /// {
    ///     Console.WriteLine($"Event: {info.EventName}, Latest: {info.LatestDuration.TotalMilliseconds}ms");
    /// }
    /// </code>
    /// </example>
    Task<LatencyEventInfo[]> LatencyLatestAsync();

    /// <summary>
    /// Displays a piece of generative computer art of the specific Valkey version and its optional arguments.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/lolwut/">Valkey commands – LOLWUT</seealso>
    /// <returns>A <see langword="string" /> containing the Valkey version and generative art.</returns>
    /// <example>
    /// <code>
    /// string art = await client.LolwutAsync();
    /// </code>
    /// </example>
    // TODO #475: Move to IBaseClient.
    Task<string> LolwutAsync();

    /// <summary>
    /// Returns a report about memory problems detected by the server.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/memory-doctor/">Valkey commands – MEMORY DOCTOR</seealso>
    /// <returns>The memory diagnostic report.</returns>
    /// <example>
    /// <code>
    /// var report = await client.MemoryDoctorAsync();
    /// Console.WriteLine("Memory report: " + report);
    /// </code>
    /// </example>
    Task<string> MemoryDoctorAsync();

    /// <summary>
    /// Returns the internal statistics of the memory allocator.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/memory-malloc-stats/">Valkey commands – MEMORY MALLOC-STATS</seealso>
    /// <returns>The memory allocator statistics.</returns>
    /// <example>
    /// <code>
    /// var report = await client.MemoryMallocStatsAsync();
    /// Console.WriteLine("Allocator stats: " + report);
    /// </code>
    /// </example>
    Task<string> MemoryMallocStatsAsync();

    /// <summary>
    /// Asks the server to reclaim memory from the allocator back to the operating system.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/memory-purge/">Valkey commands – MEMORY PURGE</seealso>
    /// <example>
    /// <code>
    /// await client.MemoryPurgeAsync();
    /// </code>
    /// </example>
    Task MemoryPurgeAsync();

    /// <summary>
    /// Returns detailed memory consumption statistics of the server.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/memory-stats/">Valkey commands – MEMORY STATS</seealso>
    /// <returns>A <see cref="MemoryStats" /> containing detailed memory usage statistics.</returns>
    /// <example>
    /// <code>
    /// var stats = await client.MemoryStatsAsync();
    /// Console.WriteLine($"Peak allocated: {stats.PeakAllocated}");
    /// </code>
    /// </example>
    Task<MemoryStats> MemoryStatsAsync();

    /// <summary>
    /// Makes the server a replica of the specified primary.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/replicaof/">Valkey commands – REPLICAOF</seealso>
    /// <param name="host">The host of the primary to replicate.</param>
    /// <param name="port">The port of the primary to replicate.</param>
    /// <example>
    /// <code>
    /// await client.ReplicaOfAsync("localhost", 6379);
    /// </code>
    /// </example>
    Task ReplicaOfAsync(string host, int port);

    /// <summary>
    /// Promotes the current server to a primary by stopping replication.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/replicaof/">Valkey commands – REPLICAOF</seealso>
    /// <example>
    /// <code>
    /// await client.ReplicaOfNoOneAsync();
    /// </code>
    /// </example>
    Task ReplicaOfNoOneAsync();

    /// <summary>
    /// Returns the current server time in UTC format.
    /// Use the <see cref="DateTimeOffset.ToLocalTime"/> method to get local time.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/time/">Valkey commands – TIME</seealso>
    /// <returns>The server's current time.</returns>
    /// <example>
    /// <code>
    /// var serverTime = await client.TimeAsync();
    /// </code>
    /// </example>
    Task<DateTimeOffset> TimeAsync();
}
