# Stream Command API Cleanup — PR Summary

## What Changed

This PR audits and cleans up all stream commands for StackExchange.Redis compatibility (`Abstract/` layer) and cross-client consistency (`Commands/` layer, `Client/` layer, and `BaseClient` partials).

### Key Changes

1. **New option types**: `StreamAddOptions`, `StreamClaimOptions`, `StreamRangeOptions`, `StreamReadOptions`/`StreamReadGroupOptions`, `StreamTrimOptions` (with `MaxLen`/`MinId` subclasses).
2. **New result types**: `StreamInfoFull` (with `StreamGroupFullInfo`, `StreamConsumerFullInfo`, `StreamPendingEntryInfo`), `StreamAutoClaimJustIdResult`.
3. **New SER enum**: `StreamTrimMode` (for SER compatibility layer).
4. **Renamed methods**: `StreamClaimIdsOnlyAsync` → `StreamClaimJustIdAsync`, `StreamAutoClaimIdsOnlyAsync` → `StreamAutoClaimJustIdAsync` for cross-client consistency.
5. **Moved methods from `IStreamBaseCommands` to `IBaseClient.StreamCommands`**: XADD (options-based), XREAD (with block), XREADGROUP (with block), XRANGE (options-based), XCLAIM (with options), XAUTOCLAIM JUSTID, XINFO STREAM FULL.
6. **Parameter renames**: `start`/`end` → `minId`/`maxId` on `StreamRangeAsync`, `order` → `messageOrder`.
7. **Made `consumerName` optional** on `StreamPendingMessagesAsync`.
8. **Replaced inline XCLAIM parameters** with `StreamClaimOptions` class.
9. **Replaced inline XADD parameters** with `StreamAddOptions` + `StreamTrimOptions`.
10. **Added `StreamTrimMode` parameter** to SER-compatible `StreamAddAsync` and `StreamTrimAsync` overloads.
11. **Added `XINFO STREAM FULL`** command support (`StreamInfoFullAsync`).
12. **Removed unsupported `StreamReadGroup` overload** and trim modes.
13. **Changed return types**: `StreamCreateConsumerGroupAsync` and `StreamConsumerGroupSetPositionAsync` now return `Task<bool>` instead of `Task`.
