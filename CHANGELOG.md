# Changelog

All notable changes to the Valkey GLIDE C# client will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html)

## 1.2.0

### Added

- Support additional configuration options:
  - All nodes read-from strategy (#207)
  - Circuit breaker (#474)
  - Inflight requests limit (#484)
  - Mutual TLS (#488)
  - Periodic topology checks (#485)
- Support missing stream commands for GLIDE client (#326):
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
- Support additional commands (#435):
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
  - `FAILOVER` (#446)
  - `MEMORY DOCTOR` (#443)
  - `MEMORY MALLOC-STATS` (#443)
  - `MEMORY PURGE` (#443)
  - `MEMORY STATS` (#443)
  - `MIGRATE` (#447)
  - `MONITOR` (#456)
  - `REPLICAOF` (#446)
  - `RESET` (#435)
  - `SAVE` (#440)
- Custom socket address resolution support via callback (#392)
- `NodeDiscoveryMode` configuration option for standalone clients (#131)

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
