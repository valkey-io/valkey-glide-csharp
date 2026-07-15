# Changelog

All notable changes to the Valkey GLIDE C# client will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html)

## 1.2.0

### Added

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
  - `REPLICAOF` (#446)
  - `RESET` (#435)
  - `SAVE` (#440)
- Custom socket address resolution support via callback (#392)

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
