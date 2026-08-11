# Developer Guide

This document describes how to set up your development environment to build and test the Valkey GLIDE C# wrapper.

## Development Overview

The Valkey GLIDE C# client is released and available for use. Contributions and feedback are welcome as we continue to improve it.

The C# client contains the following parts:

1. Rust part of the C# client located in `rust/src`; it communicates with [GLIDE core rust library](./valkey-glide/glide-core/README.md).
2. C# part of the client located in `sources`; it translates Rust async API into .NET async API.
3. Tests for the C# client located in `tests` directory.

## Setup

Install the following dependencies using the instructions below.

Valkey GLIDE C# dependencies:

- [.NET](https://dotnet.microsoft.com/en-us/)
  - .NET 8 runtime
  — .NET 10 SDK to build
- [Git](https://git-scm.com/)
- [Task](https://taskfile.dev/)
- [Valkey](https://valkey.io/)
- [WSL](https://learn.microsoft.com/en-us/windows/wsl/install) (Windows only)

Dependencies for building the [GLIDE core](./valkey-glide/glide-core/README.md):

- [cargo-zigbuild](https://github.com/rust-cross/cargo-zigbuild) (GNU Linux only)
- [cmake](https://cmake.org/)
- [gcc](https://gcc.gnu.org/)
- [openssl](https://www.openssl.org/)
- [pkg-config](https://www.freedesktop.org/wiki/Software/pkg-config/)
- [protobuf](https://github.com/protocolbuffers/protobuf)
- [rustup](https://rustup.rs/)
- [ziglang](https://ziglang.org/) (GNU Linux only)

Developer tools:

- [actionlint](https://github.com/rhysd/actionlint)
- [cargo-deny](https://github.com/EmbarkStudios/cargo-deny)
- [lychee](https://github.com/lycheeverse/lychee)
- [Node.js](https://nodejs.org/)
- [Python 3](https://www.python.org/)
- [uv](https://github.com/astral-sh/uv)

### macOS

```bash
# 1. Install Homebrew packages:
brew update
brew install actionlint cmake coreutils dotnet@8 dotnet@10 git go-task/tap/go-task lychee node openssl python uv valkey

# 2. Install Rust:
curl --proto '=https' --tlsv1.2 -sSf https://sh.rustup.rs | sh
source "$HOME/.cargo/env"
cargo install --locked cargo-deny

# 3. Install protoc:
# Download a binary from https://github.com/protocolbuffers/protobuf/releases/tag/v25.1 and put it on your PATH.
```

### Linux

```bash
# 1. Install apt packages:
sudo apt-get update -y
sudo apt install -y cmake dotnet-sdk-8.0 dotnet-sdk-10.0 gcc git libssl-dev nodejs npm openssl pkg-config python3 valkey

# 2. Install Rust:
curl --proto '=https' --tlsv1.2 -sSf https://sh.rustup.rs | sh
source "$HOME/.cargo/env"
cargo install --locked cargo-deny lychee

# 3. Install uv:
# See https://github.com/astral-sh/uv for other install methods.
curl -LsSf https://astral.sh/uv/install.sh | sh

# 4. Install actionlint (x64):
# See https://github.com/rhysd/actionlint/releases for other architectures.
curl -fsSL https://github.com/rhysd/actionlint/releases/download/v1.7.12/actionlint_1.7.12_linux_amd64.tar.gz | sudo tar xz -C /usr/local/bin actionlint

# 5. Install protoc:
# Download a binary from https://github.com/protocolbuffers/protobuf/releases/tag/v25.1, then: sudo cp protoc /usr/bin/

# 6. Install Task:
# See https://taskfile.dev/installation/ for other methods.
curl -1sLf 'https://dl.cloudsmith.io/public/task/task/setup.deb.sh' | sudo -E bash
sudo apt install task
```

#### GNU Linux

To build the Rust core on GNU Linux, additionally install [ziglang](https://ziglang.org/)
(via pip) and [cargo-zigbuild](https://github.com/rust-cross/cargo-zigbuild):

```bash
# 1. Install ziglang (via pip):
sudo apt install -y python3-pip
pip3 install ziglang

# 2. Install cargo-zigbuild:
cargo install --locked cargo-zigbuild
```

### Windows

```bash
# 1. Install Chocolatey packages:
choco install actionlint cmake dotnet-8.0-sdk dotnet-10.0-sdk git go-task mingw nodejs openssl pkgconfiglite python uv

# 2. Install Rust:
# Install from https://rust-lang.org/tools/install/, then:
cargo install --locked cargo-deny lychee

# 3. Install protoc:
# Download a binary from https://github.com/protocolbuffers/protobuf/releases/tag/v25.1 and put it on your PATH.
```

On Windows, integration tests additionally require [Windows Subsystem for Linux](https://learn.microsoft.com/en-us/windows/wsl/about) (WSL). Start by [installing WSL](https://learn.microsoft.com/en-us/windows/wsl/install), then install Python 3 within it:

```bash
sudo apt-get update -y
sudo apt install -y python3
```

## Commands

The project uses [Task](https://taskfile.dev/) for standardized development workflows.
Run `task --list` to view all available tasks.

### Build

```bash
task build             # Build the solution
task build target=lib  # Build only Valkey.Glide
```

### Checks

Run checks to validate examples, links, and TODOs.

```bash
task check           # Run all checks
task check:examples  # Check C# examples in comments
task check:links     # Check for broken links
task check:todos     # Check that TODOs reference an open GitHub issue
```

For additional details:

- Check examples script: `dev/scripts/check_examples.py`
- Check links configuration: `dev/conf/lychee.toml`
- Check TODOs script: `dev/scripts/check_todos.py`

### Format

Run automated formatters to ensure consistent code style.

```bash
task format           # Run all formatters
task format:csharp    # Run C# formatter
task format:markdown  # Run Markdown formatter
task format:python    # Run Python formatter
task format:rust      # Run Rust formatter
task format:yaml      # Run YAML formatter
```

### Lint

Run linters to catch style issues and static analysis warnings.

```bash
task lint           # Run all linters
task lint:actions   # Run GitHub Actions linter
task lint:csharp    # Run C# linter
task lint:markdown  # Run Markdown linter
task lint:python    # Run Python linter
task lint:rust      # Run Rust linter
task lint:yaml      # Run YAML linter
```

C# style and analysis rules are defined in the project `.editorconfig` files:

- [`.editorconfig`](.editorconfig) — repository-wide defaults.
- [`sources/Valkey.Glide/abstract_Enums/.editorconfig`](sources/Valkey.Glide/abstract_Enums/.editorconfig)
- [`sources/Valkey.Glide/abstract_APITypes/.editorconfig`](sources/Valkey.Glide/abstract_APITypes/.editorconfig)

### Tests

Run unit and integration tests for verify expected behaviour.

```bash
# Run tests
task test
task test:unit
task test:integration

# Run specific tests
task test:unit filter=MyTestClass          # Filter by test class
task test:integration filter=MyMethodName  # Filter by test method
```

By default, integration tests starts Valkey servers automatically. To run against
existing servers instead, set the endpoint environment variables:

- `standalone-endpoints` — standalone server(s)
- `cluster-endpoints` — cluster server(s).
- `tls=true` — connect over TLS.

Each endpoint variable takes one or more comma-separated `host:port` values. If only
standalone or cluster endpoint are specified, the other suite is skipped.

```bash
# Standalone integration tests only.
env standalone-endpoints=localhost:6379 task test:integration

# Standalone and cluster integration tests with TLS, filtered to one class:
env standalone-endpoints=localhost:6379 cluster-endpoints=localhost:7000,localhost:7001,localhost:7002 tls=true task test:integration filter=ReadFromTests
```

### Test Coverage

This project includes support for measuring line and branch coverage,
including a coverage baseline and checks to ensure coverage does not decrease.
See [docs/coverage.md](docs/coverage.md) for more details.

```bash
# Run tests with coverage.
task test coverage=true              # Run all tests with coverage
task test:unit coverage=true         # Unit tests with coverage
task test:integration coverage=true  # Integration tests with coverage

# Coverage commands
task coverage:install # Install coverage reporting tools
task coverage:report  # Generate HTML + JSON coverage reports
task coverage:check   # Compare measured coverage against baseline
task coverage:update  # Update the coverage baseline
task coverage:clean   # Remove coverage results and reports
```

#### IAM Authentication Tests

To run [IAM authentication tests](tests/Valkey.Glide.IntegrationTests/IamAuthTests.cs) locally, set the following environment variables:

```bash
export AWS_ACCESS_KEY_ID=test_access_key
export AWS_SECRET_ACCESS_KEY=test_secret_key
export AWS_SESSION_TOKEN=test_session_token
```

If any of these environment variables are not set, IAM authentication tests will be skipped.

**Note:** The credential values shown above (`test_access_key`, etc.) are arbitrary placeholder strings. The AWS SDK uses them to generate an authentication token, but the local test server doesn't validate the token. These tests verify that the IAM authentication flow works correctly (token generation, connection establishment, and token refresh), not that the credentials are valid.

#### DNS Tests

To run [DNS tests](tests/Valkey.Glide.IntegrationTests/DnsTests.cs) locally:

1. Add the following entries to your hosts file:
   - Linux/macOS: `/etc/hosts`
   - Windows: `C:\Windows\System32\drivers\etc\hosts`

   ```text
   127.0.0.1 valkey.glide.test.tls.com
   127.0.0.1 valkey.glide.test.no_tls.com
   ::1 valkey.glide.test.tls.com
   ::1 valkey.glide.test.no_tls.com
   ```

2. Set the environment variable:

   ```bash
   export VALKEY_GLIDE_DNS_TESTS_ENABLED=1
   ```

If the environment variable is not set, DNS tests will be skipped.

## Documentation

- [Valkey GLIDE](<https://glide.valkey.io/getting-started/quickstart/?lang=c%23>) – Official Valkey GLIDE documentation for users, including quick start guides, tutorials, and how-to guides.
- [Documentation Guidelines](docs/documentation.md) – Project documentation guidelines for developers.

## Benchmarking

Performance benchmarking for the C# client can be performed using [resp-bench](https://github.com/ikolomi/resp-bench), a multi-language benchmark suite for RESP protocol compatible databases. It supports Valkey GLIDE C# and StackExchange.Redis out of the box.

Refer to the [resp-bench README](https://github.com/ikolomi/resp-bench/blob/main/README.md) and [C# benchmark docs](https://github.com/ikolomi/resp-bench/blob/main/docs/BENCHMARKS_CSHARP.md) for setup and usage instructions.

## Community and Feedback

We encourage you to join our community to support, share feedback, and ask questions. You can approach us for anything on our Valkey Slack: [Join Valkey Slack](https://valkey.io/slack/).

## References

Quick links to important project documentation:

- [Reviewing Pull Requests](docs/reviewing.md) – How pull requests are reviewed and merged.
- [Documentation Guidelines](docs/documentation.md) – Guidelines for documentation.
- [Coverage](docs/coverage.md) – How to collect, report, and update test coverage baselines.
- [Contributing Guidelines](CONTRIBUTING.md) – Introduction to the contribution process.
