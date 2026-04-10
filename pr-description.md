### Summary

Close the remaining pub/sub API gaps between Valkey GLIDE C# and StackExchange.Redis. Adds missing `IDatabaseAsync.PublishAsync`, `IServer` pub/sub introspection commands, `ValkeyChannel` keyspace/keyevent notification channel factories, and the `KeyNotificationType` enum. Also refactors GLIDE-only pub/sub methods out of shared interfaces into client-specific interfaces for cleaner SER compatibility.

### Issue Link

Closes #291

### Features and Behaviour Changes

- Add `IDatabaseAsync.PublishAsync(channel, message, flags)` convenience wrapper that delegates to `ISubscriber.PublishAsync`.
- Add `IServer.SubscriptionChannelsAsync(pattern, flags)` wrapping `PUBSUB CHANNELS [pattern]`.
- Add `IServer.SubscriptionPatternCountAsync(flags)` wrapping `PUBSUB NUMPAT`.
- Add `IServer.SubscriptionSubscriberCountAsync(channel, flags)` wrapping `PUBSUB NUMSUB channel`.
- Add `ValkeyChannel.KeySpaceSingleKey(key, database)` for keyspace notification channel names.
- Add `ValkeyChannel.KeySpacePattern(pattern, database)` for keyspace notification patterns.
- Add `ValkeyChannel.KeySpacePrefix(prefix, database)` (2 overloads: `ValkeyKey` and `ReadOnlySpan<byte>`).
- Add `ValkeyChannel.KeyEvent(type, database)` (2 overloads: `KeyNotificationType` and `ReadOnlySpan<byte>`).
- Add `KeyNotificationType` enum for strongly-typed key notification event types.

### Implementation

- GLIDE-only pub/sub methods moved from shared `IPubSubBaseCommands` and `IPubSubClusterCommands` to new client-specific partials (`IBaseClient.PubSubCommands`, `IGlideClusterClient.PubSubCommands`). This keeps the shared interfaces focused on the SER-compatible API surface.
- New `IDatabaseAsync.PubSubCommands` and `Database.PubSubCommands` partial files house the database-level publish method.
- `ValkeyServer` implements the three introspection commands by delegating to the underlying GLIDE client.
- Keyspace/keyevent channel construction uses efficient `Span<byte>` assembly to avoid unnecessary allocations.
- `IDatabaseAsync` partial file doc comments cleaned up for consistency across all command categories.

### Limitations

:white_circle: None

### Testing

- Added `ValkeyChannelTests` unit tests covering all keyspace/keyevent channel factory methods (correct channel format, pattern flags, edge cases, error handling).
- Consolidated pub/sub integration tests: `ISubscriberCompatibilityTests` renamed to `PubSubCommandTests`, merged `PublishCommandTests` and `PubSubIntrospectionCommandTests` into it.
- Added integration tests for `SubscriptionChannelsAsync`, `SubscriptionPatternCountAsync`, and `SubscriptionSubscriberCountAsync`.
- Added `TestServers` property to `TestConfiguration` for server-level test parameterization.

### Related Issues

- #270 — Parent issue: command API cleanup for SER compatibility and cross-client consistency

### Checklist

- [x] This Pull Request is related to one issue.
- [x] Commit message has a detailed description of what changed and why.
- [x] Tests are added or updated and all checks pass.
- [x] ~~`CHANGELOG.md`, `README.md`, `DEVELOPER.md`, and other documentation files are updated.~~
- [x] Destination branch is correct - `main` or release
- [x] Create merge commit if merging release branch into `main`, squash otherwise.
