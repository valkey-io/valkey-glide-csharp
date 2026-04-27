# Developer Guide

This document describes how to set up your development environment to build and test the Valkey GLIDE C# wrapper.

## Development Overview

We're excited to share that the GLIDE C# client is currently in development! However, it's important to note that this client is a work in progress and is not yet complete or fully tested. Your contributions and feedback are highly encouraged as we work towards refining and improving this implementation. Thank you for your interest and understanding as we continue to develop this C# wrapper.

The C# client contains the following parts:

1. Rust part of the C# client located in `rust/src`; it communicates with [GLIDE core rust library](./valkey-glide/glide-core/README.md).
2. C# part of the client located in `sources`; it translates Rust async API into .NET async API.
3. Tests for the C# client located in `tests` directory.

## Build from Source

Software Dependencies:

- [Task](#task-installation)
- [.NET](#net-installation)
- [Git](#git-installation)
- [WSL](#wsl-installation) (for Windows only)
- [Valkey](#valkey-installation) (for testing)

Install the following packages to build [GLIDE core rust library](./valkey-glide/glide-core/README.md):

- [ziglang and zigbuild](#ziglang-and-zigbuild-installation) (for GNU Linux only)
- [Protoc](#protoc-installation)
- rustup
- GCC
- pkg-config
- cmake
- openssl
- openssl-dev

### Task Installation

[Task](https://taskfile.dev/) is fast, cross-platform build tool.

It's highly recommended to use `task` commands instead of raw `dotnet` commands for development.

```bash
# Using Homebrew (macOS/Linux)
brew install go-task/tap/go-task

# Using Chocolatey (Windows)
choco install go-task

# Direct download from GitHub releases:
# <https://github.com/go-task/task/releases>
```

### .NET Installation

[.NET](https://dotnet.microsoft.com/en-us/) is an open-source platform for building desktop, web, and mobile applications

The Valkey GLIDE C# client requires .NET 8 and 9.

```bash
# Using Homebrew (macOS/Linux)
brew install dotnet@8 dotnet@9

# Using Chocolatey (Windows)
choco install dotnet-8.0-sdk dotnet-9.0-sdk

# Direct download from Microsoft:
# <https://dotnet.microsoft.com/en-us/download>
```

### Git Installation

[Git](https://git-scm.com/) is a free and open source distributed version control system.

```bash
# Using Homebrew (macOS/Linux)
brew install git

# Using Chocolatey (Windows)
choco install git

# Direct install from Git:
# <https://git-scm.com/install>
```

### WSL Installation

Windows Subsystem for Linux ([WSL](https://learn.microsoft.com/en-us/windows/wsl/install)) lets developers install a Linux distribution and use Linux applications, utilities, and Bash command-line tools directly on Windows.

See [How to install Linux on Windows with WSL](https://learn.microsoft.com/en-us/windows/wsl/install) for installation instructions.

### Valkey Installation

[Valkey](https://valkey.io/) is required to to run tests. On Windows, it is recommended to install Valkey using [WSL](#wsl-installation).

```bash
# Using Homebrew (macOS/Linux)
# Install within WSL for Windows.
brew install valkey

# Direct installation from Valkey:
# <https://valkey.io/topics/installation>
```

### `ziglang` and `zigbuild` Installation

```bash
pip3 install ziglang
cargo install --locked cargo-zigbuild
```

### Protoc Installation

Download a binary matching your system from the [official release page](https://github.com/protocolbuffers/protobuf/releases/tag/v25.1) and make it accessible in your $PATH by moving it or creating a symlink. For example, on Linux you can copy it to `/usr/bin`:

```bash
sudo cp protoc /usr/bin/
```

### Additional Dependencies Installation for Ubuntu

```bash
# Install dependecies using apt:
sudo apt-get update -y
sudo apt install -y gcc pkg-config openssl libssl-dev

# Install Rust using curl:
curl --proto '=https' --tlsv1.2 -sSf https://sh.rustup.rs | sh
source "$HOME/.cargo/env"

# Install dependencies using cargo:
cargo install --locked cargo-deny lychee
```

### Additional Dependencies Installation for MacOS

```bash
# Install dependencies using Homebrew:
brew update
brew install openssl coreutils lychee

# Install Rust using curl:
curl --proto '=https' --tlsv1.2 -sSf https://sh.rustup.rs | sh
source "$HOME/.cargo/env"

# Install dependencies using cargo:
cargo install --locked cargo-deny
```

### Additional Dependencies Installation for Windows

```bash
# Install dependencies using choco:
choco install mingw pkgconfiglite openssl

# Install Rust directly:
# <https://rust-lang.org/tools/install/>

# Install dependencies using cargo:
cargo install --locked cargo-deny lychee
```

### Building and Installation

Before starting this step, make sure you've installed all software requirements.

In Windows, run from following commands from the appropriate Visual Studio Command Prompt shell.

1. Clone the repository

    ```bash
    # Clone repository
    git clone https://github.com/valkey-io/valkey-glide-csharp.git
    cd valkey-glide-csharp
    ```

2. Build the C# wrapper

    ```bash
    # Using Task (preferred):
    task build

    # Using dotnet:
    dotnet build
    ```

3. Run tests

    Using Task (preferred):

    ```bash
    # Run all tests with coverage (recommended)
    task test

    # Run specific test suites
    task test:unit         # Unit tests only
    task test:integration  # Integration tests only

    # Run tests with coverage and generate reports
    task coverage              # All tests with coverage
    task coverage:unit         # Unit tests with coverage
    task coverage:integration  # Integration tests with coverage

    # Run tests for specific framework
    task test framework=net8.0

    # Clean test results and reports
    task clean
    ```

    Using `dotnet`:

    ```bash
    # Run tests on supported dotnet versions sequentially
    dotnet test -m:1

    # Run tests on supported dotnet versions in parallel (may conflict and fail)
    dotnet test

    # Run tests with a specific dotnet version
    dotnet test --framework net8.0
    ```

## Task Commands Overview

The project uses Task for standardized development workflows. Here are the key commands:

```bash
# View all available tasks
task --list

# Default workflow (build + coverage)
task default

# Install required tools (coverage reporting)
task install-tools

# Build and test workflows
task build                  # Build the solution
task build target=lib       # Build only Valkey.Glide
task test                   # Build and run all tests with coverage
task coverage               # Run tests with coverage and generate HTML reports

# Specific test suites
task test:unit              # Unit tests only
task test:integration       # Integration tests only
task coverage:unit          # Unit tests with coverage
task coverage:integration   # Integration tests with coverage

# Coverage reporting
task coverage:report        # Generate HTML coverage report
task coverage:summary       # Display coverage summary
task clean                  # Clean test results and reports

# Linting and formatting
task lint                   # Run all linters
task format                 # Run all formatters
task check-links            # Check for broken links

```

## Advanced Testing Options

By default, `dotnet test` produces no reporting and does not display the test results.  To log the test results to the console and/or produce a test report, you can use the `--logger` attribute with the test command.  For example:

- `dotnet test --logger "html;LogFileName=TestReport.html"` (HTML reporting) or
- `dotnet test --logger "console;verbosity=detailed"` (console reporting)

To filter tests by class name or method name add the following expression to the command line: `--filter "FullyQualifiedName~<test or class name>"` (see the [.NET testing documentation](https://learn.microsoft.com/en-us/dotnet/core/testing/selective-unit-tests?pivots=xunit) for more details).

A command line may contain all listed above parameters, for example:

```bash
dotnet test --framework net8.0 --logger "html;LogFileName=TestReport.html" --logger "console;verbosity=detailed" --filter "FullyQualifiedName~GetReturnsNull" --results-directory .
```

To run IT tests against an existing cluster and/or standalone endpoint, set `cluster-endpoints` and/or `standalone-endpoints` environment variables.
In bash:

```bash
cluster-endpoints=localhost:7000 standalone-endpoints=localhost:6379 dotnet test
```

In Windows CMD:

```cmd
set cluster-endpoints=localhost:7000 && set standalone-endpoints=localhost:6379 && dotnet test
```

In Powershell:

```powershell
[Environment]::SetEnvironmentVariable('cluster-endpoints', 'localhost:7000')
[Environment]::SetEnvironmentVariable('standalone-endpoints', 'localhost:6379')
dotnet test
```

If those endpoints use TLS, add `tls` variable to `true` (applied to both endpoints):

```bash
cluster-endpoints=localhost:7000 standalone-endpoints=localhost:6379 tls=true dotnet test
```

You can combine this with test filter as well:

```bash
cluster-endpoints=localhost:7000 standalone-endpoints=localhost:6379 tls=true dotnet test --logger "console;verbosity=detailed" --filter "FullyQualifiedName~GetReturnsNull"
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

### DNS Tests

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

## Linting and Formatting

Before making a contribution, ensure that all new user APIs and non-obvious code is well documented, and run the code linters and analyzers.

```bash
# Run all linters:
task lint

# Run linters for specific languages:
task lint:rust     # Run Rust linting
task lint:csharp   # Run C# linting
task lint:yaml     # Run YAML linting

# Run all formatters:
task format

# Run formatters for specific languages:
task format:rust
task format:csharp
task format:yaml

# Check for broken links
task check-links
```

## Test framework and Style

The CSharp Valkey-Glide client uses xUnit v3 for testing code. The test code styles are defined in `.editorconfing` (see `dotnet_diagnostic.xUnit..` rules). The xUnit rules are enforced by the [xUnit analyzers](https://github.com/xunit/xunit.analyzers) referenced in the main xunit.v3 NuGet package. If you choose to use xunit.v3.core instead, you can reference xunit.analyzers explicitly. For additional info, please, refer to <https://xunit.NET> and <https://github.com/xunit/xunit>

## Documentation

For user-facing documentation including quick start guides, tutorials, and how-to guides, visit the official [Valkey GLIDE documentation site](https://glide.valkey.io/getting-started/quickstart/?lang=c%23).

## Benchmarking

Performance benchmarking for the C# client can be performed using [resp-bench](https://github.com/ikolomi/resp-bench), a multi-language benchmark suite for RESP protocol compatible databases. It supports Valkey GLIDE C# and StackExchange.Redis out of the box.

Refer to the [resp-bench README](https://github.com/ikolomi/resp-bench/blob/main/README.md) and [C# benchmark docs](https://github.com/ikolomi/resp-bench/blob/main/docs/BENCHMARKS_CSHARP.md) for setup and usage instructions.

## Community and Feedback

We encourage you to join our community to support, share feedback, and ask questions. You can approach us for anything on our Valkey Slack: [Join Valkey Slack](https://valkey.io/slack/).
