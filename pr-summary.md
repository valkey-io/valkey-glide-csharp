# PR Summary

## What Changed

This PR closes the remaining pub/sub API gaps between Valkey GLIDE C# and StackExchange.Redis (issue #291).

## Key Changes

### New APIs

- **`IDatabaseAsync.PublishAsync`**: Convenience wrapper for publishing from a database context (delegates to `ISubscriber.PublishAsync`).
- **`IServer` pub/sub introspection**: `SubscriptionChannelsAsync`, `SubscriptionPatternCountAsync`, `SubscriptionSubscriberCountAsync` — wrapping `PUBSUB CHANNELS`, `PUBSUB NUMPAT`, `PUBSUB NUMSUB`.
- **`ValkeyChannel` keyspace/keyevent factories**: `KeySpaceSingleKey`, `KeySpacePattern`, `KeySpacePrefix` (2 overloads), `KeyEvent` (2 overloads) — for building keyspace/keyevent notification channel names.
- **`KeyNotificationType` enum**: Strongly-typed enum for all Valkey key notification event types.

### Refactoring

- Moved GLIDE-only pub/sub methods from shared interfaces (`IPubSubBaseCommands`, `IPubSubClusterCommands`) to client-specific interfaces (`IBaseClient.PubSubCommands`, `IGlideClusterClient.PubSubCommands`), keeping shared interfaces focused on SER-compatible surface.
- Added `IDatabaseAsync.PubSubCommands` and `Database.PubSubCommands` partial files for the new database-level publish method.
- Cleaned up `IDatabaseAsync` partial file doc comments for consistency across all command categories.

### Tests

- Added `ValkeyChannelTests` (unit tests) for all keyspace/keyevent channel factory methods.
- Consolidated integration tests: renamed `ISubscriberCompatibilityTests` → `PubSubCommandTests`, merged `PublishCommandTests` and `PubSubIntrospectionCommandTests` into it.
- Added integration tests for `SubscriptionChannelsAsync`, `SubscriptionPatternCountAsync`, `SubscriptionSubscriberCountAsync`.
- Added `TestServers` to `TestConfiguration` for server-level test parameterization.

### Documentation

- Added `docs/stackexchange-redis-compatibility.md` and `docs/stackexchange-redis-pubsub-signatures.md` reference docs.
