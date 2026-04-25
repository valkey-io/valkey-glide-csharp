# Add Compression Support for CustomCommand and Fix Panic Guard Handling

### Features and Behaviour Changes

This PR adds proper compression support for the C# FFI layer, enabling:

- **CustomCommand compression**: Commands sent via `CustomCommand()` now properly compress/decompress values when compression is enabled. Previously, CustomCommand calls bypassed compression processing.
- **Batch/pipeline compression**: Full compression support for batch and pipeline operations, including CustomCommand within batches.
- **Incompatible command detection**: Commands that are incompatible with compression (APPEND, GETRANGE, SETRANGE, STRLEN, INCR, INCRBY, INCRBYFLOAT, DECR, DECRBY, GETBIT, SETBIT, BITCOUNT, BITPOS, BITFIELD, BITFIELD_RO, BITOP, LCS, SUBSTR) now return proper error messages explaining why they cannot be used with compression enabled.
- **Improved error messages**: Fixed an issue where handled errors were incorrectly reported as "Native function panicked" instead of the actual error message.

### Implementation

Key changes in `rust/src/lib.rs`:

1. **`extract_cmd_args` helper function**: Extracts simple arguments from a redis command as byte vectors, reducing code duplication.

2. **`resolve_custom_command_type` function**: Resolves the actual `RequestType` from the first argument of a CustomCommand. Uses the centralized `RequestType::from_command_name()` method from glide-core to avoid code duplication.

3. **Updated `command` function**:
   - Passes compression manager to `create_cmd` for compression processing
   - Resolves the request type for CustomCommand for decompression
   - Fixed panic guard handling to set `panicked = false` before early returns

4. **Updated `batch` function**:
   - Passes compression manager to `create_pipeline` for compression processing
   - Added batch response decompression using `glide_core::compression::decompress_batch_response()`

5. **Panic guard fix**: The `PanicGuard` is designed to catch actual Rust panics. Previously, when returning early due to handled errors (compression failure, command creation failure), the guard would incorrectly report "Native function panicked". Now we properly signal that these are controlled exits.

Key changes in `rust/src/ffi.rs`:

6. **Updated `create_cmd` function**: Now accepts an optional compression manager and handles compression for all commands including CustomCommand.

7. **Updated `create_pipeline` function**: Passes compression manager to `create_cmd` for each command in the pipeline.

8. **Added `resolve_custom_command_type` function**: Resolves the actual RequestType for CustomCommand in the FFI layer.

9. **Added `client_side_cache` and `node_discovery_mode` fields**: Updated `ConnectionRequest` creation to include new fields from the latest glide-core.

### Limitations

- Client-side caching configuration is stubbed out (`client_side_cache: None`) - full support would require additional C# configuration options.
- Node discovery mode is set to default - full support would require additional C# configuration options.

### Testing

All 41 compression integration tests pass:

- Basic compression/decompression with Zstd and Lz4
- Cluster and standalone client compression
- Minimum size threshold handling
- Custom compression levels
- Backward compatibility with uncompressed data
- Multiple operations data integrity
- Binary data handling
- Compression statistics
- Multi-key operations (MGET, MSET, MSETNX)
- CustomCommand compression (SETEX, PSETEX, SETNX via CustomCommand)
- Batch/pipeline compression (standalone and cluster)
- Transaction compression
- Batch with CustomCommand compression
- Incompatible command error handling (18 commands tested via parameterized tests)

### Related Issues

- Depends on glide-core compression support merged in valkey-io/valkey-glide#5771
- Depends on glide-core batch decompression support (compression-batch-fix branch)
- Uses centralized `RequestType::from_command_name()` added in glide-core to reduce code duplication across language bindings
