# Project Structure

## Directory Organization

```
valkey-glide-csharp/
├── sources/Valkey.Glide/          # Main client library
│   ├── Abstract/                   # Abstract base classes and interfaces
│   ├── abstract_APITypes/          # API type definitions
│   ├── abstract_Enums/             # Enumeration types
│   ├── Commands/                   # Command implementations
│   ├── Internals/                  # Internal implementation details
│   ├── Pipeline/                   # Pipeline and batching support
│   ├── BaseClient.*.cs             # Command category implementations
│   ├── GlideClient.cs              # Standalone client
│   ├── GlideClusterClient.cs       # Cluster client
│   ├── ConnectionConfiguration.cs  # Configuration builders
│   └── Valkey.Glide.csproj         # Project file
├── tests/
│   ├── Valkey.Glide.UnitTests/     # Unit tests
│   └── Valkey.Glide.IntegrationTests/ # Integration tests
├── benchmarks/                     # Performance benchmarking tool
├── rust/                           # Rust FFI layer
│   ├── src/                        # Rust source code
│   └── Cargo.toml                  # Rust dependencies
├── valkey-glide/                   # Git submodule (core library)
└── Valkey.Glide.sln                # Visual Studio solution
```

## Core Components

### Client Classes

**BaseClient** (`BaseClient.cs`)
- Abstract base class for all client types
- Provides common functionality for standalone and cluster clients
- Command implementations organized by category in partial classes:
  - `BaseClient.GenericCommands.cs` - Generic key operations (DEL, EXISTS, EXPIRE, etc.)
  - `BaseClient.StringCommands.cs` - String operations (GET, SET, MGET, MSET, etc.)
  - `BaseClient.HashCommands.cs` - Hash operations (HGET, HSET, HGETALL, etc.)
  - `BaseClient.ListCommands.cs` - List operations (LPUSH, RPUSH, LRANGE, etc.)
  - `BaseClient.SetCommands.cs` - Set operations (SADD, SMEMBERS, SISMEMBER, etc.)
  - `BaseClient.SortedSetCommands.cs` - Sorted set operations (ZADD, ZRANGE, ZSCORE, etc.)
  - `BaseClient.HyperLogLogCommands.cs` - HyperLogLog operations (PFADD, PFCOUNT, etc.)

**GlideClient** (`GlideClient.cs`)
- Standalone (non-cluster) client implementation
- Inherits from BaseClient
- Supports single-node and master-replica configurations
- Factory method: `CreateClient(StandaloneClientConfiguration)`

**GlideClusterClient** (`GlideClusterClient.cs`)
- Cluster client implementation
- Inherits from BaseClient
- Handles cluster slot mapping and multi-node operations
- Factory method: `CreateClient(ClusterClientConfiguration)`

### Configuration

**ConnectionConfiguration** (`ConnectionConfiguration.cs`)
- Configuration builders for client setup
- `StandaloneClientConfigurationBuilder` - Standalone client configuration
- `ClusterClientConfigurationBuilder` - Cluster client configuration
- Fluent API for setting addresses, authentication, TLS, timeouts, etc.

### Type System

**GlideString** (`GlideString.cs`)
- Binary-safe string type for handling raw byte data
- Supports both string and byte[] representations

**ClusterValue** (`ClusterValue.cs`)
- Represents values returned from cluster operations
- Handles single values and multi-node responses

**Route** (`Route.cs`)
- Defines routing strategies for cluster commands
- Supports routing to specific nodes, slots, or all nodes

### Error Handling

**Errors** (`Errors.cs`)
- Custom exception types:
  - `ValkeyException` - Base exception for all Valkey errors
  - `ConnectionException` - Connection-related errors
  - `TimeoutException` - Operation timeout errors
  - `RequestException` - Request processing errors

### Pipeline Support

**Pipeline/** directory
- Pipeline and transaction batching support
- Enables executing multiple commands in a single network roundtrip
- Reduces latency for bulk operations

## Architectural Patterns

### Layered Architecture

1. **C# API Layer** (`sources/Valkey.Glide/`)
   - Public API exposed to .NET applications
   - Async/await wrappers around Rust FFI
   - Type conversions and marshalling

2. **Rust FFI Layer** (`rust/src/`)
   - Foreign Function Interface between C# and Rust
   - Bridges C# async API to Rust async runtime
   - Memory management and safety guarantees

3. **Core Library** (`valkey-glide/glide-core/`)
   - Shared Rust core library (git submodule)
   - Protocol implementation and connection management
   - Multi-language support (C#, Python, Java, Go, Node.js)

### Client Hierarchy

```
BaseClient (abstract)
├── GlideClient (standalone)
└── GlideClusterClient (cluster)
```

### Command Organization

Commands are organized by data structure type in partial classes:
- Generic commands (keys, expiration, etc.)
- String commands
- Hash commands
- List commands
- Set commands
- Sorted set commands
- HyperLogLog commands

This organization improves maintainability and follows the Valkey/Redis command categorization.

## Build System

### Multi-Targeting
- Targets both .NET 6.0 and .NET 8.0
- Configured in `Valkey.Glide.csproj` via `<TargetFrameworks>net8.0;net6.0</TargetFrameworks>`

### Rust Integration
- Pre-build event triggers Cargo build
- Native libraries copied to output directory:
  - Windows: `glide_rs.dll`
  - Linux: `libglide_rs.so`
  - macOS: `libglide_rs.dylib`

### Build Configurations
- **Debug**: Development builds with symbols
- **Release**: Optimized production builds
- **Lint**: Code analysis and style enforcement

## Test Organization

### Unit Tests (`tests/Valkey.Glide.UnitTests/`)
- Fast, isolated tests without external dependencies
- Test internal logic, type conversions, and edge cases
- Examples: `BoundaryTests.cs`, `CommandTests.cs`, `GildeStringTests.cs`

### Integration Tests (`tests/Valkey.Glide.IntegrationTests/`)
- Tests against real Valkey/Redis servers
- Organized by command category and client type
- Examples: `StringCommandTests.cs`, `ClusterClientTests.cs`, `StandaloneClientTests.cs`
- Shared test utilities: `SharedClientTests.cs`, `SharedCommandTests.cs`, `SharedBatchTests.cs`

### Test Framework
- Uses xUnit v3 for all tests
- Test styles enforced via `.editorconfig` and xUnit analyzers
- Supports filtering by test name, framework version, and logging options
