# Product Overview

## Project Purpose

Valkey GLIDE for C# is the official open-source Valkey client library for .NET applications. It provides high-performance, reliable connectivity to Valkey and Redis OSS servers with comprehensive async/await support. Built on a robust Rust core, it delivers enterprise-grade performance and reliability for production workloads.

## Value Proposition

- **High Performance**: Rust core provides optimal performance, low latency, and multi-threaded support
- **Modern Async/Await**: Full support for C# asynchronous programming patterns with non-blocking operations
- **Cross-Platform**: Supports .NET 6.0+ on Windows, Linux, and macOS
- **Type Safety**: Strongly-typed API with comprehensive IntelliSense support
- **Enterprise Ready**: Designed for production workloads with robust error handling and connection pooling
- **API Compatibility**: Compatible with StackExchange.Redis APIs to ease migration from existing codebases
- **Community Driven**: Open source with active community support

## Key Features

### Advanced Connectivity
- **AZ Affinity**: Routes read traffic to replicas in the client's availability zone for low-latency connections and minimal cross-zone costs (Requires Valkey 8.0+ or AWS ElastiCache for Valkey 7.2+)
- **PubSub Auto-Reconnection**: Seamless background resubscription on topology updates or disconnection
- **Sharded PubSub**: Native support for sharded PubSub across cluster slots

### Cluster Operations
- **Cluster-Aware Multi-Key Commands**: Execute MGET/MSET/DEL/FLUSHALL across cluster slots without manual key grouping
- **Cluster Scan**: Unified key iteration across shards using a consistent, high-level API
- **Seamless Cluster Support**: Automatic slot mapping and node discovery

### Performance Optimization
- **Batching**: Pipeline and Transaction support for executing multiple commands in a single network roundtrip
- **Connection Pooling**: Efficient connection management for better throughput
- **Async/Await**: Non-blocking operations throughout the API

### Observability
- **OpenTelemetry**: Integrated tracing support for enhanced observability and monitoring

## Target Users

- **.NET Developers**: Building applications with .NET 6.0+ requiring high-performance caching or data storage
- **Enterprise Teams**: Organizations needing production-ready Valkey/Redis connectivity with robust error handling
- **Microservices Architects**: Teams building distributed systems with ASP.NET Core, Minimal APIs, or background services
- **Migration Teams**: Developers migrating from StackExchange.Redis seeking improved performance and modern features

## Use Cases

### Web Applications
- **ASP.NET Core**: Caching layer, session store, distributed cache
- **Minimal APIs**: High-performance data access for microservices and API backends
- **Entity Framework**: Complement ORM with high-performance caching

### Background Processing
- **Background Services**: Queue processing, job scheduling, task coordination
- **Worker Services**: Distributed task processing with reliable message queuing

### Real-Time Applications
- **PubSub Messaging**: Real-time notifications, chat applications, event streaming
- **Sharded PubSub**: Scalable messaging across cluster deployments

### Data Management
- **Distributed Caching**: Multi-region caching with AZ affinity for cost optimization
- **Session Management**: Distributed session storage for web applications
- **Rate Limiting**: High-performance rate limiting and throttling

## Supported Engine Versions

| Engine Type | 6.2 | 7.0 | 7.1 | 7.2 | 8.0 | 8.1 |
|-------------|-----|-----|-----|-----|-----|-----|
| Valkey      | -   | -   | -   | ✓   | ✓   | ✓   |
| Redis       | ✓   | ✓   | ✓   | ✓   | -   | -   |

## Current Status

> **Preview State**: The C# wrapper is currently in preview and has many features that remain to be implemented before GA. The project is actively developed with contributions and feedback highly encouraged.
