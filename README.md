# Valkey GLIDE for CSharp

Valkey General Language Independent Driver for the Enterprise (GLIDE) is the official open-source Valkey client library for C#. Built on a robust Rust core, it provides high-performance, reliable connectivity to Valkey and Redis OSS servers with comprehensive async/await support.

## Why Choose Valkey GLIDE for C#?

- **High Performance**: Built with a Rust core for optimal performance and low latency
- **Async/Await Support**: Full support for modern C# asynchronous programming patterns
- **Cross-Platform**: Supports .NET 6.0+ on Windows, Linux, and macOS
- **Type Safety**: Strongly-typed API with comprehensive IntelliSense support
- **Enterprise Ready**: Designed for production workloads with robust error handling
- **Community Driven**: Open source with active community support
- **API Compatibility**: Compatible with StackExchange.Redis APIs to ease migration

## Key Features

- **[AZ Affinity](https://valkey.io/blog/az-affinity-strategy/)** – Ensures low-latency connections and minimal cross-zone costs by routing read traffic to replicas in the client's availability zone. **(Requires Valkey server version 8.0+ or AWS ElastiCache for Valkey 7.2+)**
- **[PubSub Auto-Reconnection](https://github.com/valkey-io/valkey-glide/wiki/General-Concepts#pubsub-support)** – Seamless background resubscription on topology updates or disconnection
- **[Sharded PubSub](https://github.com/valkey-io/valkey-glide/wiki/General-Concepts#pubsub-support)** – Native support for sharded PubSub across cluster slots
- **[Cluster-Aware MGET/MSET/DEL/FLUSHALL](https://github.com/valkey-io/valkey-glide/wiki/General-Concepts#multi-slot-command-handling)** – Execute multi-key commands across cluster slots without manual key grouping
- **[Cluster Scan](https://github.com/valkey-io/valkey-glide/wiki/General-Concepts#cluster-scan)** – Unified key iteration across shards using a consistent, high-level API
- **[Batching (Pipeline and Transaction)](https://github.com/valkey-io/valkey-glide/wiki/General-Concepts#batching-pipeline-and-transaction)** – Execute multiple commands efficiently in a single network roundtrip
- **[OpenTelemetry](https://github.com/valkey-io/valkey-glide/wiki/General-Concepts#opentelemetry)** – Integrated tracing support for enhanced observability

> [!IMPORTANT]
> Valkey.Glide C# wrapper is in a preview state and still has many features that remain to be implemented before GA.

## Supported Engine Versions

Valkey GLIDE for C# is API-compatible with the following engine versions:

| Engine Type           |  6.2  |  7.0  |   7.1  |  7.2  |  8.0  |  8.1  |  9.0  |
|-----------------------|-------|-------|--------|-------|-------|-------|-------|
| Valkey                |   -   |   -   |   -    |   V   |   V   |   V   |   V   |
| Redis                 |   V   |   V   |   V    |   V   |   -   |   -   |   -   |

## Installation

### NuGet Package

Install the Valkey GLIDE package from NuGet:

```bash
dotnet add package Valkey.Glide
```

Or via Package Manager Console in Visual Studio:

```powershell
Install-Package Valkey.Glide
```

### Requirements

- **.NET 6.0** or higher
- **Supported Platforms**: Windows, Linux, macOS
- **Valkey/Redis Server**: Version 6.2+ (see compatibility table above)

## Quick Start

### Standalone Client

```csharp
using Valkey.Glide;

// Create a standalone client
var connection = await ConnectionMultiplexer.ConnectAsync("localhost:6379");
var db = connection.Database;

// Basic string operations
await db.StringSetAsync("key", "value");
var result = await db.StringGetAsync("key");
Console.WriteLine($"Retrieved: {result}");
```

### Cluster Client

```csharp
using Valkey.Glide;
using static Valkey.Glide.ConnectionConfiguration;

// Create a cluster client
var config = new ClusterClientConfigurationBuilder()
    .WithAddress("cluster-node1.example.com", 6379)
    .WithAddress("cluster-node2.example.com", 6379)
    .WithAddress("cluster-node3.example.com", 6379)
    .Build();

using var client = await GlideClusterClient.CreateClient(config);

// Cluster operations work seamlessly
await client.StringSetAsync("user:1000", "John Doe");
var user = await client.StringGetAsync("user:1000");
Console.WriteLine($"User: {user}");
```

### With Authentication and TLS

```csharp
var config = new StandaloneClientConfigurationBuilder()
    .WithAddress("secure-server.example.com", 6380)
    .WithAuthentication("username", "password")
    .WithTls()
    .Build();

using var client = await GlideClient.CreateClient(config);
```

## Core API Examples

### String Operations

```csharp
// Set and get strings
await client.StringSetAsync("greeting", "Hello, World!");
var greeting = await client.StringGetAsync("greeting");

// Multiple keys
var keyValuePairs = new KeyValuePair<ValkeyKey, ValkeyValue>[]
{
    new("key1", "value1"),
    new("key2", "value2")
};
await client.StringSetAsync(keyValuePairs);

var values = await client.StringGetAsync(new[] { "key1", "key2" });
```

### Hash Operations

```csharp
// Hash operations
await client.HashSetAsync("user:1000", "name", "John Doe");
await client.HashSetAsync("user:1000", "email", "john@example.com");

var name = await client.HashGetAsync("user:1000", "name");
var allFields = await client.HashGetAllAsync("user:1000");
```

### List Operations

```csharp
// List operations
await client.ListPushAsync("tasks", "task1", ListDirection.Left);
await client.ListPushAsync("tasks", "task2", ListDirection.Left);

var task = await client.ListPopAsync("tasks", ListDirection.Right);
var allTasks = await client.ListRangeAsync("tasks", 0, -1);
```

### Set Operations

```csharp
// Set operations
await client.SetAddAsync("tags", "csharp");
await client.SetAddAsync("tags", "dotnet");
await client.SetAddAsync("tags", "valkey");

var isMember = await client.SetIsMemberAsync("tags", "csharp");
var allTags = await client.SetMembersAsync("tags");
```

### Building & Testing

Development instructions for local building & testing the package are in the [DEVELOPER.md](DEVELOPER.md) file.

## Documentation

- **[API Documentation](https://valkey.io/valkey-glide/)** - Complete API reference
- **[General Concepts](https://github.com/valkey-io/valkey-glide/wiki/General-Concepts)** - Core concepts and patterns

## Performance

Valkey GLIDE for C# is built for high performance:

- **Rust Core**: Leverages Rust's memory safety and performance included multi-threaded support
- **Async/Await**: Non-blocking operations for better throughput
- **Connection Pooling**: Efficient connection management
- **Pipeline Support**: Batch operations for reduced latency

## Error Handling

```csharp
try
{
    await client.StringSetAsync("key", "value");
    var result = await client.StringGetAsync("key");
}
catch (ConnectionException ex)
{
    Console.WriteLine($"Connection error: {ex.Message}");
}
catch (TimeoutException ex)
{
    Console.WriteLine($"Operation timed out: {ex.Message}");
}
catch (ValkeyException ex)
{
    Console.WriteLine($"Valkey error: {ex.Message}");
}
```

## Contributing

We welcome contributions! Please see our [Contributing Guidelines](./CONTRIBUTING.md) for details on:

- Setting up the development environment
- Running tests
- Submitting pull requests
- Code style guidelines

## Getting Help

- **GitHub Issues**: [Report bugs or request features](https://github.com/valkey-io/valkey-glide-csharp/issues)
- **Valkey Slack**: [Join our community](https://join.slack.com/t/valkey-oss-developer/shared_invite/zt-2nxs51chx-EB9hu9Qdch3GMfRcztTSkQ)
- **Documentation**: [Official docs](https://valkey.io/valkey-glide/)

When reporting issues, please include:

1. Valkey GLIDE version
2. .NET version and runtime
3. Operating system
4. Server version and configuration
5. Minimal reproducible code
6. Error messages and stack traces

## License

This project is licensed under the [Apache License 2.0](./LICENSE).

## Ecosystem

Valkey GLIDE for C# integrates well with the .NET ecosystem:

- **ASP.NET Core**: Use as a caching layer or session store
- **Entity Framework**: Complement your ORM with high-performance caching
- **Minimal APIs**: Perfect for microservices and API backends
- **Background Services**: Ideal for queue processing and background tasks

---

**Ready to get started?** Install the NuGet package and check out our [Quick Start](#quick-start) guide!
