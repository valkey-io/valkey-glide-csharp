### Summary

Further cleanup of stream commands. Disable some GLIDE-only methods pending further work.

### Issue Link

This pull request is linked to issue: #270

### Features and Behaviour Changes

Further cleanup of stream commands. Disable some GLIDE-only methods pending further work.

#### Interface

##### ✅ XADD

GLIDE offers four variants: single/multi stream without/with additional options.

| Interface | Method |
| --- | --- |
| `IBaseClient` | `Task<ValkeyValue> StreamAddAsync(ValkeyKey key, ValkeyValue streamField, ValkeyValue streamValue)` |
| `IBaseClient` | `Task<ValkeyValue> StreamAddAsync(ValkeyKey key, IEnumerable<NameValueEntry> streamPairs)` |
| `IBaseClient` | `Task<ValkeyValue> StreamAddAsync(ValkeyKey key, ValkeyValue streamField, ValkeyValue streamValue, StreamAddOptions options)` |
| `IBaseClient` | `Task<ValkeyValue> StreamAddAsync(ValkeyKey key, IEnumerable<NameValueEntry> streamPairs, StreamAddOptions options)` |
| `IBatchStreamCommands` | `IBatch StreamAdd(ValkeyKey key, ValkeyValue streamField, ValkeyValue streamValue)` |
| `IBatchStreamCommands` | `IBatch StreamAdd(ValkeyKey key, IEnumerable<NameValueEntry> streamPairs)` |
| `IBatchStreamCommands` | `IBatch StreamAdd(ValkeyKey key, ValkeyValue streamField, ValkeyValue streamValue, StreamAddOptions options)` |
| `IBatchStreamCommands` | `IBatch StreamAdd(ValkeyKey key, IEnumerable<NameValueEntry> streamPairs, StreamAddOptions options)` |
| `IDatabaseAsync` | `Task<ValkeyValue> StreamAddAsync(ValkeyKey key, ValkeyValue streamField, ValkeyValue streamValue, ValkeyValue? messageId = null, int? maxLength = null, bool useApproximateMaxLength = false, CommandFlags flags = CommandFlags.None)` |
| `IDatabaseAsync` | `Task<ValkeyValue> StreamAddAsync(ValkeyKey key, IEnumerable<NameValueEntry> streamPairs, ValkeyValue? messageId = null, int? maxLength = null, bool useApproximateMaxLength = false, CommandFlags flags = CommandFlags.None)` |

##### ✅ XREAD

GLIDE offers four variants: single/multi stream with/without additional options.

| Interface | Method |
| --- | --- |
| `IBaseClient` | `Task<StreamEntry[]> StreamReadAsync(StreamPosition position)` |
| `IBaseClient` | `Task<ValkeyStream[]> StreamReadAsync(IEnumerable<StreamPosition> streamPositions)` |
| `IBaseClient` | `Task<StreamEntry[]> StreamReadAsync(StreamPosition position, StreamReadOptions options)` |
| `IBaseClient` | `Task<ValkeyStream[]> StreamReadAsync(IEnumerable<StreamPosition> streamPositions, StreamReadOptions options)` |
| `IBatchStreamCommands` | `IBatch StreamRead(StreamPosition position)` |
| `IBatchStreamCommands` | `IBatch StreamRead(IEnumerable<StreamPosition> streamPositions)` |
| `IBatchStreamCommands` | `IBatch StreamRead(StreamPosition position, StreamReadOptions options)` |
| `IBatchStreamCommands` | `IBatch StreamRead(IEnumerable<StreamPosition> streamPositions, StreamReadOptions options)` |
| `IDatabaseAsync` | `Task<StreamEntry[]> StreamReadAsync(ValkeyKey key, ValkeyValue position, int? countPerStream = null, CommandFlags flags = CommandFlags.None)` |
| `IDatabaseAsync` | `Task<ValkeyStream[]> StreamReadAsync(IEnumerable<StreamPosition> streamPositions, int? countPerStream = null, CommandFlags flags = CommandFlags.None)` |

##### ✅ XRANGE / XREVRANGE

GLIDE offers two variants: with and without additional options.

| Interface | Method |
| --- | --- |
| `IBaseClient` | `Task<StreamEntry[]> StreamRangeAsync(ValkeyKey key)` |
| `IBaseClient` | `Task<StreamEntry[]> StreamRangeAsync(ValkeyKey key, StreamRangeOptions options)` |
| `IBatchStreamCommands` | `IBatch StreamRange(ValkeyKey key)` |
| `IBatchStreamCommands` | `IBatch StreamRange(ValkeyKey key, StreamRangeOptions options)` |
| `IDatabaseAsync` | `Task<StreamEntry[]> StreamRangeAsync(ValkeyKey key, ValkeyValue? minId = null, ValkeyValue? maxId = null, int? count = null, Order messageOrder = Order.Ascending, CommandFlags flags = CommandFlags.None)` |

##### ✅ XREADGROUP

GLIDE offers four variants: single/multi stream with/without additional options.

| Interface | Method |
| --- | --- |
| `IBaseClient` | `Task<StreamEntry[]> StreamReadGroupAsync(StreamPosition position, ValkeyValue groupName, ValkeyValue consumerName)` |
| `IBaseClient` | `Task<ValkeyStream[]> StreamReadGroupAsync(IEnumerable<StreamPosition> positions, ValkeyValue groupName, ValkeyValue consumerName)` |
| `IBaseClient` | `Task<StreamEntry[]> StreamReadGroupAsync(StreamPosition position, ValkeyValue groupName, ValkeyValue consumerName, StreamReadGroupOptions options)` |
| `IBaseClient` | `Task<ValkeyStream[]> StreamReadGroupAsync(IEnumerable<StreamPosition> positions, ValkeyValue groupName, ValkeyValue consumerName, StreamReadGroupOptions options)` |
| `IBatchStreamCommands` | `IBatch StreamReadGroup(StreamPosition position, ValkeyValue groupName, ValkeyValue consumerName)` |
| `IBatchStreamCommands` | `IBatch StreamReadGroup(IEnumerable<StreamPosition> positions, ValkeyValue groupName, ValkeyValue consumerName)` |
| `IBatchStreamCommands` | `IBatch StreamReadGroup(StreamPosition position, ValkeyValue groupName, ValkeyValue consumerName, StreamReadGroupOptions options)` |
| `IBatchStreamCommands` | `IBatch StreamReadGroup(IEnumerable<StreamPosition> positions, ValkeyValue groupName, ValkeyValue consumerName, StreamReadGroupOptions options)` |
| `IDatabaseAsync` | `Task<StreamEntry[]> StreamReadGroupAsync(ValkeyKey key, ValkeyValue groupName, ValkeyValue consumerName, ValkeyValue? position = null, int? count = null, bool noAck = false, CommandFlags flags = CommandFlags.None)` |
| `IDatabaseAsync` | `Task<StreamEntry[]> StreamReadGroupAsync(ValkeyKey key, ValkeyValue groupName, ValkeyValue consumerName, ValkeyValue? position, int? count, bool noAck, TimeSpan? claimMinIdleTime, CommandFlags flags)` |
| `IDatabaseAsync` | `Task<ValkeyStream[]> StreamReadGroupAsync(IEnumerable<StreamPosition> streamPositions, ValkeyValue groupName, ValkeyValue consumerName, int? countPerStream = null, bool noAck = false, CommandFlags flags = CommandFlags.None)` |

Notes:

- GLIDE methods use `StreamPosition` to pair key+position (matching Java/Python `Map<key, position>` pattern). SER methods retain separate parameters.
- The `claimMinIdleTime` overload throws `NotImplementedException` — see [#322](https://github.com/valkey-io/valkey-glide/issues/322).

##### ✅ XLEN

| Interface | Method |
| --- | --- |
| `IStreamBaseCommands` | `Task<long> StreamLengthAsync(ValkeyKey key)` |
| `IBatchStreamCommands` | `IBatch StreamLength(ValkeyKey key)` |
| `IDatabaseAsync` | `Task<long> StreamLengthAsync(ValkeyKey key, CommandFlags flags)` |

##### ✅ XDEL

GLIDE offers single and multi-ID variants.

| Interface | Method |
| --- | --- |
| `IStreamBaseCommands` | `Task<long> StreamDeleteAsync(ValkeyKey key, IEnumerable<ValkeyValue> messageIds)` |
| `IBaseClient` | `Task<bool> StreamDeleteAsync(ValkeyKey key, ValkeyValue messageId)` |
| `IBatchStreamCommands` | `IBatch StreamDelete(ValkeyKey key, IEnumerable<ValkeyValue> messageIds)` |
| `IBatchStreamCommands` | `IBatch StreamDelete(ValkeyKey key, ValkeyValue messageId)` |
| `IDatabaseAsync` | `Task<long> StreamDeleteAsync(ValkeyKey key, IEnumerable<ValkeyValue> messageIds, CommandFlags flags)` |

#### Disabled GLIDE Methods (SER-only for now)

The following commands have SER-compatible implementations on `IDatabaseAsync` (routed through `Request` directly) but their GLIDE-native methods on `IBaseClient`, `IStreamBaseCommands`, and `IBatchStreamCommands` are temporarily commented out pending individual cleanup. The internal `Request` layer is intact for all of them.

- XGROUP CREATE — `IDatabaseAsync.StreamCreateConsumerGroupAsync`
- XGROUP SETID — `IDatabaseAsync.StreamConsumerGroupSetPositionAsync`
- XGROUP DESTROY / CREATECONSUMER / DELCONSUMER — `IDatabaseAsync.StreamDeleteConsumerGroupAsync`, `StreamCreateConsumerAsync`, `StreamDeleteConsumerAsync`
- XCLAIM — `IDatabaseAsync.StreamClaimAsync`, `StreamClaimIdsOnlyAsync`
- XAUTOCLAIM — `IDatabaseAsync.StreamAutoClaimAsync`, `StreamAutoClaimIdsOnlyAsync`
- XTRIM — `IDatabaseAsync.StreamTrimAsync`, `StreamTrimByMinIdAsync`
- XACK — `IDatabaseAsync.StreamAcknowledgeAsync`
- XPENDING — `IDatabaseAsync.StreamPendingAsync`, `StreamPendingMessagesAsync`
- XINFO STREAM — `IDatabaseAsync.StreamInfoAsync`
- XINFO STREAM FULL — disabled (GLIDE-only, no SER equivalent)
- XINFO GROUPS / CONSUMERS — `IDatabaseAsync.StreamGroupInfoAsync`, `StreamConsumerInfoAsync`

### Limitations

- `claimMinIdleTime` on SER `StreamReadGroupAsync` throws `NotImplementedException`. See [#322](https://github.com/valkey-io/valkey-glide/issues/322).
- GLIDE methods for non-core stream commands (XGROUP, XCLAIM, XAUTOCLAIM, XTRIM, XACK, XPENDING, XINFO) are temporarily disabled pending individual cleanup.

### Testing

- SER API integration tests for all stream commands (`StackExchange/StreamCommandTests.cs`).
- GLIDE integration tests for enabled commands (XADD, XREAD, XRANGE, XREADGROUP, XLEN, XDEL).
- GLIDE integration tests for disabled commands are commented out.
- Batch tests updated to match enabled commands.
- 486 unit tests pass, 172 stream integration tests pass.

### Related Issues

- #270 — Parent issue: command API cleanup.
- #282 — Stream command cleanup parent issue.
- #322 — Support `claimMinIdleTime` on SER `StreamReadGroupAsync` (sub-issue of #282).

### Checklist

- [x] This Pull Request is related to one issue.
- [x] Commit message has a detailed description of what changed and why.
- [x] Tests are added or updated and all checks pass.
- [x] ~`CHANGELOG.md`, `README.md`, `DEVELOPER.md`, and other documentation files are updated.~
- [x] Destination branch is correct - `main` or release
- [x] ~~Create merge commit if merging release branch into `main`~~, squash otherwise.
