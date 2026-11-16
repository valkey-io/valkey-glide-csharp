# StackExchange.Redis Compatibility

Valkey GLIDE C# aims to be API-compatible with [StackExchange.Redis](https://github.com/StackExchange/StackExchange.Redis) v2.8.58.
This document lists known differences that affect migration from StackExchange.Redis.

## API Differences

| StackExchange.Redis | Valkey GLIDE         | Notes                                                                             |
| ------------------- | -------------------- | --------------------------------------------------------------------------------- |
| `ClientType.Slave`  | `ClientType.Replica` | Obsolete alias not supported.                                                     |
| `MigrateOptions`    | `KeyMigrateOptions`  | Renamed to avoid conflicting with `Valkey.Glide.Commands.Options.MigrateOptions`. |
