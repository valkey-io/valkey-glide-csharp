# AGENTS: Context for Agentic Tools

This document contains information and instructions that are specific for agentic tools.
Generic information for developers and contributors can be found in the corresponding guides:

- [`DEVELOPER.md`](DEVELOPER.md) provides instructions for development work,
including setup, building, testing, formatting, linting, and checks.
- [`CONTRIBUTING.md`](CONTRIBUTING.md) provides instruction for contributing to this project,
including commit requirements like DCO signoff and conventional commits.

## Repository Overview

- Primary language: `C#` with a Rust core (glide-core) accessed via P/Invoke.
- Solution: `Valkey.Glide.sln` – single-target library (`net8.0`) with an async public API.
- Commands organized via partials in `BaseClient.*.cs`; cluster features use routing (`Route`, `ClusterValue<T>`).

## Working Effectively (Agents)

- ALWAYS use `task` runner commands — do NOT invoke `dotnet`, `dotnet format`, `cargo`,
  `cargo clippy`, or `cargo fmt` directly. `task` applies required flags (framework
  scoping, analyzer/warning-as-error rules) that raw commands miss. Fall back to a raw
  command only if no `task` target exists, and scope .NET to `--framework net8.0`.
- `task test:integration` auto-starts throwaway servers; it just needs `python3` and a
  `valkey-server`/`redis-server` binary on `PATH`. A `*-server not found on PATH` error
  means the binary is missing — fix `PATH`, don't treat it as a test failure.
- Never pass individual `.cs` files to `dotnet test`; use project folders and `filter=`.
- Commit with `git commit -s` (DCO signoff is enforced) using Conventional Commit messages.

## Guardrails & Policies

- Submodule: `valkey-glide/` is a read-only submodule — do not edit it. Only
  `valkey-glide/glide-core/` is relevant here; ignore other language folders.
- API compatibility: maintain StackExchange.Redis API compatibility (target version 2.8.58)
  in the public API surface whenever possible.
- Documentation: all public and protected members need XML doc comments (`CS1591` should
  produce zero warnings). Follow [`docs/documentation.md`](docs/documentation.md).

## Project Structure (Essential)

- `sources/Valkey.Glide/`
  - `BaseClient.cs` and partials: `BaseClient.*.cs` for commands (String, Hash, List, Set, SortedSet, Generic)
  - `GlideClient.cs`, `GlideClusterClient.cs`
  - `ConnectionConfiguration.cs` (builders), `GlideString.cs` (encoding), `ClusterValue.cs`, `Route.cs`, `Logger.cs`, `Errors.cs`
  - Folders:
    - `Abstract/` – base abstractions for clients, pipelines, and shared contracts
    - `abstract_APITypes/` – public API types (value objects/DTOs) exposed to consumers
    - `abstract_Enums/` – public enums used across the API surface and routing
    - `Commands/` – shared command helpers (argument builders, key routing helpers, common utilities)
    - `Internals/` – interop and low-level glue (P/Invoke to Rust core, marshaling, buffers, utilities)
    - `Pipeline/` – batching/pipelining primitives and request/response grouping
- `tests/`
  - `Valkey.Glide.UnitTests/` – unit-level validation, parsing, API construction
  - `Valkey.Glide.IntegrationTests/` – end-to-end standalone and cluster tests, batching, AZ Affinity, error handling

## Quality Gates (Agent Checklist)

Run each via `task` (see [DEVELOPER.md](DEVELOPER.md) for the commands):

- [ ] [Build](DEVELOPER.md#build)
- [ ] [Format](DEVELOPER.md#format)
- [ ] [Lint](DEVELOPER.md#lint)
- [ ] [Checks](DEVELOPER.md#checks) pass.
- [ ] [Unit and integration tests](DEVELOPER.md#tests) pass. Use filters to scope to relevant tests only.
- [ ] Public API changes respect StackExchange.Redis compatibility.
- [ ] All commits include DCO signoff (see [CONTRIBUTING.md](CONTRIBUTING.md)).

## Quick Facts for Reasoners

- Engines supported (per README): Valkey 7.2, 8.0, 8.1; Redis 6.2–7.2.
- Features include AZ Affinity, PubSub auto-reconnect, sharded PubSub, cluster MGET/MSET/DEL/FLUSHALL, cluster scan, batching, OpenTelemetry.
- Error handling via typed exceptions; async-first API.
