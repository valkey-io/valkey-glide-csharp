### Summary

Add `Lease<byte>?` hash and string get methods to the `IDatabaseAsync` for StackExchange.Redis compatibility.

### Description

StackExchange.Redis provides `Lease<byte>?`-returning variants of `HashGetAsync` and `StringGetAsync` for zero-copy byte access. These are performance-oriented overloads that return a pooled buffer (`Lease<byte>`) instead of copying bytes into a managed `ValkeyValue`. GLIDE C# does not currently provide these overloads.

| Valkey Command | StackExchange.Redis Method |
| ------- | --- |
| HGET    | `Task<Lease<byte>?> HashGetLeaseAsync(RedisKey key, RedisValue hashField, CommandFlags flags = CommandFlags.None)` |
| HGETDEL | `Task<Lease<byte>?> HashFieldGetLeaseAndDeleteAsync(RedisKey key, RedisValue hashField, CommandFlags flags = CommandFlags.None)` |
| HGETEX  | `Task<Lease<byte>?> HashFieldGetLeaseAndSetExpiryAsync(RedisKey key, RedisValue hashField, TimeSpan? expiry = null, bool persist = false, CommandFlags flags = CommandFlags.None)` |
| HGETEX  | `Task<Lease<byte>?> HashFieldGetLeaseAndSetExpiryAsync(RedisKey key, RedisValue hashField, DateTime expiry, CommandFlags flags = CommandFlags.None)` |
| GET     | `Task<Lease<byte>?> StringGetLeaseAsync(RedisKey key, CommandFlags flags = CommandFlags.None)` |

### Proposed Solution

1. Add a NuGet dependency on `Pipelines.Sockets.Unofficial` to use `Lease<byte>` directly (MIT licensed, same author as SER).
2. Add the methods above to `IDatabaseAsync` and implement in `Database`, delegating to the existing GLIDE methods.

### Effort & Severity Estimates

- Effort: small
- Severity: low

### Related

- #280 — Hash command API cleanup (deferred this work from HGET section).
- #270 — Parent issue: command API cleanup.
