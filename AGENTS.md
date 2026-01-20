# AGENTS: Unified Context for Agentic Tools

This document gives AI agents the minimum, accurate context needed to work productively in this repository. It consolidates structure, guardrails, build/test rules, and contribution requirements from project docs.

## Repository Overview

- Primary language: `C#` with a Rust core via FFI
- Solution: `Valkey.Glide.sln`

## Architecture Quick Facts

- Single-target library (net8.0).
- Public APIs are async; cancellation is supported.
- Rust core (glide-core) accessed via P/Invoke; be careful with marshaling.
- Commands organized via partials in `BaseClient.*.cs`; cluster features use routing (`Route`, `ClusterValue<T>`).

## Build and Test Rules (Agents)

- Always target `net8.0` when building or testing.
- Prefer `Task` runner commands when available; otherwise use `dotnet` directly with `--framework net8.0`.
- Never pass individual `.cs` files to `dotnet test`; use project folders and filters.

Common commands:

- Build (library or solution):
  - `task build` (preferred)
  - `dotnet build --framework net8.0`
  - `dotnet build sources/Valkey.Glide/ --framework net8.0`

- Test (all or by project):
  - `task test` (preferred)
  - `dotnet test --framework net8.0`
  - `dotnet test tests/Valkey.Glide.UnitTests/ --framework net8.0`
  - `dotnet test tests/Valkey.Glide.IntegrationTests/ --framework net8.0`

- Filter tests:
  - By class: `--filter "FullyQualifiedName~ClassName"`
  - By method: `--filter "FullyQualifiedName~MethodName"`
  - By display name pattern: `--filter "DisplayName~Pattern"`

- Coverage and reports (preferred via Task):
  - `task coverage`, `task coverage:unit`, `task coverage:integration`
  - Reports go to `reports/`; test artifacts to `testresults/`

- Benchmarking:
  - Preferred: `task benchmark FRAMEWORK=net8.0`
  - Raw: `cd valkey-glide/benchmarks && ./install_and_test.sh -no-tls -minimal -only-glide -data 1 -tasks 10 -csharp -dotnet-framework net8.0`

## Contribution Requirements

### Developer Certificate of Origin (DCO) Signoff

All commits to this repository MUST include a DCO signoff to certify authorship and license compliance.

- Required signoff line (must appear at the end of every commit message):

  ```text
  Signed-off-by: Your Name <your.email@example.com>
  ```

- How to add signoffs:
  - Automatic (recommended): `git commit -s -m "<message>"`
  - Configure Git to always sign off: `git config --global format.signOff true`
  - Amend last commit to add signoff: `git commit --amend --signoff --no-edit`
  - Multiple commits missing signoffs: `git rebase -i HEAD~n --signoff`

- What the signoff means (summary):
  - You have the right to submit the work under the project license
  - The work is either your own, appropriately licensed, or provided by someone who certified the same
  - You understand contributions are public and recorded indefinitely

- Enforcement:
  - Pull requests are checked for proper signoffs
  - Commits without signoffs will be rejected (applies to all contributors)

### Conventional Commit Format

Use Conventional Commits to make history and automation clearer.

Note: Conventional Commits apply to commit messages only. Do not enforce this format for pull request titles or issue titles; use clear, descriptive English for those.

- Message format:

  ```text
  <type>(<scope>): <description>

  [optional body]

  [optional footer(s)]

  Signed-off-by: Your Name <your.email@example.com>
  ```

- Types:
  - `feat`: New feature
  - `fix`: Bug fix
  - `docs`: Documentation changes
  - `style`: Code style changes (formatting, whitespace, etc.)
  - `refactor`: Code refactoring (no functional changes)
  - `test`: Adding or updating tests
  - `chore`: Maintenance tasks (build, tooling, deps)

- Examples:
  - `feat(config): add ReadFrom parsing support`
  - `fix(client): resolve connection timeout issue`
  - `docs(readme): update installation instructions`
  - `test(config): add comprehensive ReadFrom tests`

- Example full commit message:

  ```text
  feat: Add ReadFrom parsing support to ConfigurationOptions

  - Implement ParseReadFromStrategy method
  - Add validation for ReadFrom strategy and AZ parameter combinations
  - Extend DoParse method to handle readFrom and az parameters

  Addresses GitHub issue #26

  Signed-off-by: Your Name <your.email@example.com>
  ```

## Guardrails & Policies

- Submodule:
  - `valkey-glide/` is a read-only submodule.
  - Only `valkey-glide/glide-core/` is relevant to this repo; ignore other language folders.
  - Do not edit submodule code from this repository.
- Generated outputs:
  - Treat `reports/`, `testresults/`, `bin/`, `obj/` as generated; do not commit.
- API compatibility:
  - Maintain StackExchange.Redis API compatibility (target version 2.8.58) in the public API surface whenever possible.
- Analyzer quirks:
  - `<ImplicitUsings>enable</ImplicitUsings>` can flag required `using` directives as unnecessary. Verify before removing (common false positives: `System.Net`, `System.ComponentModel`, `System.Text`, `Valkey.Glide.Internals`).

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

<!-- Analyzer guidance and common commands consolidated under Guardrails & Policies and Build/Test above to reduce redundancy. -->

## Quality Gates (Agent Checklist)

- Build passes on `net8.0`.
- Lint/format ok when applicable: `dotnet build --configuration Lint`, `dotnet format --verify-no-changes`.
- Tests pass; targeted via filters instead of per-file execution.
- Generated outputs not committed.
- Public API changes respect StackExchange.Redis compatibility.
- Commits include DCO signoff.

## Quick Facts for Reasoners

- Engines supported (per README): Valkey 7.2, 8.0, 8.1; Redis 6.2–7.2.
- Features include AZ Affinity, PubSub auto-reconnect, sharded PubSub, cluster MGET/MSET/DEL/FLUSHALL, cluster scan, batching, OpenTelemetry.
- Error handling via typed exceptions; async-first API.

## If You Need More

- General overview: `README.md`
- Developer setup and Task usage: `DEVELOPER.md`
- Commit/DCO rules: `.kiro/steering/commits.md`, `CONTRIBUTING.md`
- Test invocation rules: `.kiro/steering/dotnet-test-usage.md`
- Tech stack and CI expectations: `.kiro/steering/tech.md`
- Framework targeting rules: `.kiro/steering/framework.md`
- Structure details: `.kiro/steering/structure.md`

This file is optimized for autonomous agents. Keep commands scoped to `net8.0`; avoid modifying submodules; preserve API compatibility and DCO signoffs.
