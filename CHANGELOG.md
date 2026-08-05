# Changelog

All notable changes to the Valkey GLIDE C# client will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html)

## 1.2.0

### Fixed

- `FailoverOptions` throws `ArgumentOutOfRangeException` for zero timeout (#488)
- `XREADGROUP` returning only the first field-value pair per stream entry (#430)
- Incorrect default routes:
  - `SELECT` routed to Random instead of AllNodes in cluster mode (#491)
  - `CONFIG SET` routed to AllPrimaries instead of AllNodes in cluster mode (#492)
  - `CONFIG REWRITE` routed to Random instead of AllNodes in cluster mode (#493)
  - `CONFIG RESETSTAT` routed to AllPrimaries instead of AllNodes in cluster mode (#493)
  - `FUNCTION KILL` routed to AllPrimaries instead of AllNodes in cluster mode (#494)

### Removed

- `CircuitBreakerConfig.MaxTimeSpan` — validation consolidated into internal `TimeUtils` (#488)

### Added

- Mutual TLS (mTLS) support (#488)
- Circuit breaker configuration (#474)
- Support additional commands (#435):
  - `BGREWRITEAOF` (#444)
  - `BGSAVE CANCEL` (#436)
  - `BGSAVE SCHEDULE` (#436)
  - `BGSAVE` (#436)
  - `CLIENT CACHING` (#451)
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
