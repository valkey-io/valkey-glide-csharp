# Valkey GLIDE for C\#

Valkey General Language Independent Driver for the Enterprise (GLIDE) is the official open-source Valkey client library for C#. Built on a robust Rust core, it provides high-performance, reliable connectivity to Valkey and Redis OSS servers with comprehensive async/await support.

## Why Choose Valkey GLIDE for C#?

- **High Performance**: Built with a Rust core for optimal performance and low latency
- **Async/Await Support**: Full support for modern C# asynchronous programming patterns
- **Cross-Platform**: Supports .NET 8.0+ on Windows, Linux, and macOS
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
- **[Transparent Compression](#compression-operations)** – Automatic value compression with Zstd and LZ4 backends to reduce bandwidth and storage

> [!IMPORTANT]
> Valkey.Glide C# wrapper is in a preview state and still has many features that remain to be implemented before GA.

## Why Choose Valkey GLIDE?

- **Community and Open Source** – Join our vibrant community and contribute to the project. We are always here to respond, and the client is for the community.
- **High Performance** – Created from the ground up for high performance and low latency, powered by a shared Rust core.
- **Reliability and High Availability** – Designed to ensure your applications are always up and running.
- **Stability and Fault Tolerance** – Built on years of experience to create a bulletproof client.
- **StackExchange.Redis Compatibility** – API-compatible with [StackExchange.Redis](https://github.com/StackExchange/StackExchange.Redis) to ease migration.

## Documentation

Visit the official Valkey GLIDE [documentation site](https://glide.valkey.io).

- [Key Features](https://glide.valkey.io/overview/#key-features)
- [Quick Start](https://glide.valkey.io/getting-started/quickstart/?lang=csharp)
- [Supported Engine Versions](https://glide.valkey.io/overview/#supported-engine-versions)
- [Basic Operations](https://glide.valkey.io/getting-started/basic-operations/)
- [Client Initialization](https://glide.valkey.io/how-to/client-initialization/)
- [Installation](https://glide.valkey.io/how-to/installation/)
- [Troubleshooting](https://glide.valkey.io/troubleshooting/)

## Getting Help

- **GitHub Issues**: [Report bugs or request features](https://github.com/valkey-io/valkey-glide-csharp/issues)
- **Valkey Slack**: [Join our community](https://join.slack.com/t/valkey-oss-developer/shared_invite/zt-2nxs51chx-EB9hu9Qdch3GMfRcztTSkQ)
- **Documentation**: [Official documentation](https://glide.valkey.io)

## Contributing

We welcome contributions! Please see our [Contributing Guidelines](./CONTRIBUTING.md) for details. Development instructions for local building and testing are in [DEVELOPER.md](DEVELOPER.md).

## License

This project is licensed under the [Apache License 2.0](./LICENSE).
