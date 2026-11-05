# Changelog

All notable changes to the Valkey GLIDE C# client will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html)

## Unreleased

### Added

- Valkey Search 1.3 typed API, covering valkey-search 1.3's full parity surface against
  RediSearch (wave 1 and wave 2):
  - `Ft.HybridAsync`/`Ft.HybridWithCursorAsync` (FT.HYBRID): combines a SEARCH arm with a VSIM arm
    and fuses their results, via `HybridSearchArm`, `HybridVsimArm`, `HybridVsimFilter` (with
    `HybridFilterPolicy.Adhoc`/`Batches`), and `HybridCombine` (`HybridRrf`, `HybridLinear`,
    `HybridFunction`); the vector may be given as raw bytes or as an explicit `$name` parameter
    reference; `HybridOptions.LocalOnly` and `HybridOptions.Combine`'s `FUNCTION` kind are
    valkey-search extensions beyond RediSearch's own FT.HYBRID
  - `Ft.AggregateWithCursorAsync`/`Ft.CursorReadAsync`/`Ft.CursorDelAsync` (FT.AGGREGATE
    WITHCURSOR, FT.CURSOR READ/DEL): paged aggregate replies via `Ft.WithCursor`; cursors (and
    FT.HYBRID's own `WITHCURSOR`) are pinned to the PRIMARY of the index's slot in cluster mode,
    regardless of the client's read-from configuration, since a cursor is a per-process,
    node-local resource on the server
  - `Ft.SearchOptions.WithScores`/`Scorer`, `Ft.AggregateOptions.AddScores`/`Scorer`
    (WITHSCORES/SCORER/ADDSCORES): per-document relevance scores, exposed as `SearchDocument.Score`
    or the `__score` pipeline field; `"BM25STD"` is the only registered scorer
  - `Ft.SearchOptions.InKeys` (INKEYS): restricts FT.SEARCH to a given key set
  - `Ft.SearchOptions.Count` (`LIMIT 0 0`): returns only `SearchResult.TotalResults`, no document
    rows
  - `AggregateReducer.ToList`/`FirstValue`/`RandomSample` (TOLIST/FIRST_VALUE/RANDOM_SAMPLE
    reducers)
  - `Ft.CreateVectorField.Type` `Float16`/`Bfloat16` (FLOAT16/BFLOAT16 vector element types)
  - `Ft.CreateOptions.Filter` (FILTER): excludes non-matching documents from the index even if
    their key matches a configured prefix
  - `Ft.CreateField.Unf` (SORTABLE UNF): preserves a sortable field's original, un-normalized value
  - `Ft.LoadField`/`Ft.AggregateOptions.LoadFieldEntries` (`LOAD ... AS alias`): renames a loaded
    field's output column; requires `search.emulate-release >= 1.3.0` to take effect (see
    Limitations)
  - `Ft.CreateOptions.ScoreField` and `Ft.InfoLocalResult.ScoreField`/`DefaultScore`
    (SCORE_FIELD, `index_definition.score_field`/`default_score`): a document field whose value
    multiplies into the final relevance score; `InfoLocalResult` only reports `ScoreField` at
    `search.emulate-release >= 1.3.0` (see Limitations)
  - `Ft.CreateOptions.NoHighlight` (NOHL): wire-compatible no-op, since valkey-search does not
    implement highlighting
  - Vector Range queries in FT.SEARCH/FT.AGGREGATE: `@field:[VECTOR_RANGE radius $blob]`, with
    `$YIELD_DISTANCE_AS`
  - An adjacent `LIMIT` widening a `SORTBY`'s own default/`MAX` row cap, at
    `search.emulate-release >= 1.3.0` (see Limitations)
  - A bare `*` query as match-all in FT.SEARCH/FT.AGGREGATE (previously required the
    `@field:[-inf +inf]` workaround)
- Local module-test harness (`tests/Valkey.Glide.ModuleTests`, `utils/start-valkey-docker-modules.sh`,
  `utils/run-modules-ci-locally.sh`): runs the valkey-search 1.3 surface against real Docker-based
  valkey-search modules, gated by a capability probe and a `search.emulate-release` level fixture.
  The suite's feature-coverage tests run against the standalone client; two tests
  (`LevelFixture_ClusterBranch_ReadsSameLevelAsStandalone`, `Cursor_RoutesToIndexSlotPrimaryInClusterMode`)
  and the multi-shard placement test (`Search_MultiShardNoHashtag_AggregatesAcrossShards`) run
  against the cluster deployment directly when cluster endpoints are provided -- see Limitations
  for why generic feature tests are not duplicated across both topologies
- 40 module tests covering FT.CREATE (FLOAT16/BFLOAT16 vector types with live KNN queries,
  FILTER exclusion, SORTABLE UNF case preservation, SCORE_FIELD ranking and FT.INFO reporting,
  NOHL), FT.SEARCH (WITHSCORES ranking, SCORER rejection of unregistered scorers, INKEYS, COUNT,
  LIMIT pagination, bare `*`, Vector Range, the combined WITHSCORES+WITHSORTKEYS reply shape),
  FT.AGGREGATE (TOLIST/RANDOM_SAMPLE array-valued reducers, ADDSCORES/SCORER ranking, `LoadField`
  alias branched on the `search.emulate-release` level, SORTBY widened by an adjacent LIMIT,
  WITHCURSOR paged to exhaustion), FT.CURSOR READ/DEL error paths, FT.HYBRID (SEARCH+VSIM arms,
  RRF/LINEAR/FUNCTION combine verified to actually change ranking, the FILTER-immediately-after-
  VSIM-arm-with-no-LOAD regression case, LOCALONLY, VSIM FILTER with POLICY, the `$name` parameter
  reference path, omitted `K` defaulting to 10, VSIM RANGE and in-transaction rejection), and
  cluster-mode cursor routing to the index slot's primary

### Fixed

- `Ft.InfoLocalAsync`: the nested `index_definition` and `attributes` (and each vector attribute's
  own `index`/`algorithm`) sub-structures in a real valkey-search 1.3 `FT.INFO ... LOCAL` reply
  arrive from glide-core as flat key/value arrays rather than maps, since glide-core's generic
  FT.INFO reshape was written against the older RediSearch-style flat shape and only recurses into
  that shape's own `fields`/`vector_params` keys. `Ft.InfoLocalAsync` now normalizes these
  sub-structures itself instead of assuming glide-core already reshaped them, fixing a cast
  exception that only reproduces against a real module (not the mocked unit tests)
- `Ft.AggregateAsync`/`Ft.HybridAsync`/cursor reads threw `RequestException` on any row containing
  an array-valued reducer result (TOLIST, RANDOM_SAMPLE), since the response parser only handled
  scalar cell values. Array-valued cells are now reported as a JSON-array-shaped string (e.g.
  `["10","30"]`)
- The main integration-test suite's `ServerModules` FT tests (680 test cases, across standalone
  and cluster and both RESP2/RESP3 protocol parametrizations, in
  `FtCreateTests`/`FtSearchTests`/`FtAggregateTests`/`FtInfoTests`/`FtDropIndexTests`/`FtListTests`)
  never ran against a real search module in any CI or local configuration: `TestConfiguration`
  only read `standalone-endpoints`/`cluster-endpoints`, not the module-test harness's
  `MODULES_STANDALONE_ENDPOINT`/`MODULES_CLUSTER_ENDPOINTS`, so they always skipped via
  `ModuleUtils.SkipIfSearchModuleNotAvailableAsync`. `TestConfiguration` now falls back to the
  module-test harness's variable names, and `utils/run-modules-ci-locally.sh` now runs this suite
  (filtered to `ServerModules`) alongside `Valkey.Glide.ModuleTests`. Confirmed live in all three
  required configurations: all 680 pass against the 1.3-shaped `FT.INFO`/`FT.SEARCH` replies

### Limitations

- `search.emulate-release` defaults to the module's current major version (`1.0.0` on a stock 1.3
  module) rather than `1.3.0`; GLIDE stays a pass-through and does not set it. Four observed
  behaviours change below `search.emulate-release 1.3.0` even on a 1.3-capable binary:
  `Ft.LoadField.Alias` (the legacy branch treats `AS` as a literal field name, so the command is
  rejected outright rather than renaming the column), an adjacent `LIMIT` widening a `SORTBY`'s
  row cap (stays capped at `SORTBY`'s own default/`MAX`), `Ft.InfoLocalResult.ScoreField`
  (absent entirely below 1.3.0, with `DefaultScore` always reporting the legacy literal `1.0`),
  and the wire-level prefix byte on a `WITHSORTKEYS` sort key (`SearchDocument.SortKey`): `'$'` at
  >= 1.3.0, `'#'` below it, confirmed live against the same rc2 binary at both levels. GLIDE
  reports the sort key verbatim either way; nothing strips or reinterprets the prefix
- FT.HYBRID and FT.CURSOR READ/DEL dispatch via `RequestType.CustomCommand`, not a dedicated
  `RequestType`, since glide-core's reply reshaping keys on the literal command text rather than
  the `RequestType` tag -- no behavioural difference, but a future glide-core release adding
  dedicated `RequestType`s for these commands may be adopted in a follow-up
- Cursor affinity (FT.AGGREGATE/FT.HYBRID `WITHCURSOR`, and every subsequent FT.CURSOR READ/DEL)
  pins to the PRIMARY of the index's slot in cluster mode, regardless of the client's read-from
  configuration, matching RediSearch clients' behaviour via a different mechanism (a cursor is a
  per-process, node-local resource; redis-py instead remembers the node of the last FT.AGGREGATE
  call)
- `FT.CREATE` is not broadcast cluster-wide on valkey-search 1.3.0-rc1: an index lands on exactly
  the one primary GLIDE's cluster client happens to route the command to (by the index name's key
  hash), and `FT.SEARCH`/`FT.AGGREGATE` only see documents that are both present and indexed on
  that same primary -- confirmed live against a real 6-node cluster deployment. This is a
  server/engine behaviour, not a GLIDE routing bug; a caller running this port's cluster client
  against a multi-primary deployment should either co-locate the index and its documents with a
  shared hashtag, or confirm on their own valkey-search version whether this still holds

## 1.2.0

### Added

- Support additional configuration options:
  - All nodes read-from strategy (#207)
  - AZ affinity across all nodes read-from strategy (#533)
  - Circuit breaker (#474)
  - Client library name and info tag (#514)
  - Inflight requests limit (#484)
  - Mutual TLS (#488)
  - Periodic topology checks (#485)
  - Custom socket address resolution support via callback (#392)
  - Node discovery mode option for standalone clients (#131)
- Support missing stream commands (#326):
  - `XAUTOCLAIM`
  - `XCLAIM`
  - `XGROUP CREATE`
  - `XGROUP CREATECONSUMER`
  - `XGROUP DELCONSUMER`
  - `XGROUP DESTROY`
  - `XGROUP SETID`
  - `XINFO CONSUMERS`
  - `XINFO GROUPS`
  - `XINFO STREAM FULL`
  - `XPENDING`
- Support missing commands (#435):
  - `BGREWRITEAOF` (#444)
  - `BGSAVE CANCEL` (#436)
  - `BGSAVE SCHEDULE` (#436)
  - `BGSAVE` (#436)
  - `CLIENT CACHING` (#451)
  - `CLIENT KILL` (#276)
  - `CLIENT PAUSE` (#437)
  - `CLIENT TRACKING` (#451)
  - `CLIENT TRACKINGINFO` (#451)
  - `CLIENT UNPAUSE` (#437)
  - `DELIFEQ` (#575)
  - `FAILOVER` (#446)
  - `HGETDEL` (#285)
  - `MEMORY DOCTOR` (#443)
  - `MEMORY MALLOC-STATS` (#443)
  - `MEMORY PURGE` (#443)
  - `MEMORY STATS` (#443)
  - `MIGRATE` (#447)
  - `MONITOR` (#456)
  - `REPLICAOF` (#446)
  - `RESET` (#435)
  - `SAVE` (#440)
- Support new Valkey 9.1 commands:
  - `HSETEX` with `NX` and `XX` options (#582)
- Support new Valkey 9.2 commands:
  - `INFO` with `cluster_info` section (#590)
  - `SET` with `IFNE` option (#579)
  - `SISMEMBER` with `XX` option (#581)
- Single-field batch overloads for hash field expiry commands (#285):
  - `HGETEX`
  - `HPERSIST`
  - `HPEXPIRE`
  - `HPEXPIREAT`
  - `HPEXPIRETIME`
  - `HPTTL`

### Changed

- For compatibility with StackExchange.Redis:
  - `IDatabaseAsync.StreamReadAsync` parameter renamed to `count` (#326)
  - `IDatabaseAsync.StreamAutoClaimIdsOnlyAsync` returns `StreamAutoClaimIdsOnlyResult` (#326)
- Updated released but previously unused stream types (#326):
  - Rename `StreamGroupFullInfo` to `StreamGroupInfoFull`
  - Rename `StreamConsumerFullInfo` to `StreamConsumerInfoFull`
  - Rename `StreamPendingEntryInfo` to `StreamPendingEntry`
  - Update `StreamClaimOptions` to use factory and fluent methods instead of an object initializer
- `Logger`, `InfoOptions`, and `Options` are now `static` classes.
- The Availability Zone (`ReadFrom.Az`) is trimmed of surrounding whitespace before it is used, so an AZ-affinity strategy configured with a padded value now engages AZ affinity instead of silently spreading reads across all nodes (#533)
- `SET` with `IFEQ` or `IFNE` now throws a `RequestException` when compression is enabled (#579)

### Fixed

- Marshalling of non-ASCII characters on Windows (#501)
- `FailoverOptions` throws `ArgumentOutOfRangeException` for zero timeout (#488)
- `StreamReadGroupAsync` returning only the first field-value pair per stream entry (#430)
- Incorrect default routes:
  - `SELECT` routed to Random instead of AllNodes in cluster mode (#491)
  - `CONFIG SET` routed to AllPrimaries instead of AllNodes in cluster mode (#492)
  - `CONFIG REWRITE` routed to Random instead of AllNodes in cluster mode (#493)
  - `CONFIG RESETSTAT` routed to AllPrimaries instead of AllNodes in cluster mode (#493)
  - `FUNCTION KILL` routed to AllPrimaries instead of AllNodes in cluster mode (#494)
- `GlideString(byte[])` no longer builds the hex-dump representation on construction (#522)
- `CompareTo(null)` returns a positive value for `ScoreBound` and `GlideString` (#550)
- `ClusterValue<T>.HasSingleData` returns expected result for value-type `T` (#547)
- Culture-invariant parsing of numeric string responses (#557)
- `ISubscriber` corrupting binary (non-UTF-8) channel names and messages (#594)
- `GETSET` custom command compresses its value when compression is enabled (#579)

### Security

- IAM authentication now requires TLS: configuring IAM auth without TLS is rejected at client creation instead of sending the IAM token over a plaintext connection (#611)

## 1.1.0

### Added

- Valkey JSON (JSON.*) command support for clients and batches (#358)
- Valkey Search (FT.*) command support for clients (#225)
- Client-side caching with TTL-based expiration, LRU/LFU eviction policies, and cache metrics API (#330)
- Compression support for CustomCommand with incompatible command detection and improved error messages (#348)

### Security

- Remove credential leakage vectors from FFI debug output (#371)

## 1.0.0

### Added

- StackExchange.Redis compatible pub/sub API (#202)
- Transparent compression support with Zstd and LZ4 backends (#213)
- Windows CI and testing with WSL (#184)
