# Developer Guide

This document describes how to set up your development environment to build and test the Valkey GLIDE C# wrapper.

## Development Overview

We're excited to share that the GLIDE C# client is currently in development! However, it's important to note that this client is a work in progress and is not yet complete or fully tested. Your contributions and feedback are highly encouraged as we work towards refining and improving this implementation. Thank you for your interest and understanding as we continue to develop this C# wrapper.

The C# client contains the following parts:

1. Rust part of the C# client located in `rust/src`; it communicates with [GLIDE core rust library](/valkey-glide/glide-core/README.md).
2. C# part of the client located in `sources`; it translates Rust async API into .NET async API.
3. Tests for the C# client located in `tests` directory.
4. A dedicated benchmarking tool designed to evaluate and compare the performance of Valkey GLIDE and other .NET clients. It is located in `/benchmarks`.

TODO: examples, UT, design docs

## Build from Source

Software Dependencies:

- [Task](#task-installation)
- [.NET](#net-installation)
- [Git](#git-installation)
- [WSL](#wsl-installation) (for Windows only)
- [Valkey](#valkey-installation) (for testing)

Install the following packages to build [GLIDE core rust library](/valkey-glide/glide-core/README.md):

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
sudo apt-get update -y
sudo apt install -y gcc pkg-config openssl libssl-dev
curl --proto '=https' --tlsv1.2 -sSf https://sh.rustup.rs | sh
source "$HOME/.cargo/env"
```

### Additional Dependencies Installation for MacOS

```bash
brew update
brew install openssl coreutils
curl --proto '=https' --tlsv1.2 -sSf https://sh.rustup.rs | sh
source "$HOME/.cargo/env"
```

### Additional Dependencies Installation for Windows

```bash
choco install mingw pkgconfiglite openssl

# Install Rust directly:
# <https://rust-lang.org/tools/install/>
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
    task test FRAMEWORK=net8.0

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

# Benchmarking
task benchmark FRAMEWORK=net8.0  # Run performance benchmarks
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

## Benchmark

1. Ensure that you have installed `valkey-server` and `valkey-cli` on your host. You can find the valkey installation guide above.

2. Execute benchmarks using Task (preferred):

    ```bash
    cd csharp
    # Run benchmarks with standardized configuration
    task benchmark FRAMEWORK=net8.0
    ```

3. Alternative using raw commands:

    ```bash
    cd <repo root>/benchmarks/csharp
    dotnet run --framework net8.0 --dataSize 1024 --resultsFile test.json --concurrentTasks 4 --clients all --host localhost --clientCount 4
    ```

4. Use a [helper script](../benchmarks/README.md) which runs end-to-end benchmarking workflow:

    ```bash
    cd <repo root>/benchmarks
    ./install_and_test.sh -csharp
    ```

Run benchmarking script with `-h` flag to get list and help about all command line parameters.

## Linting

Before making a contribution, ensure that all new user APIs and non-obvious code is well documented, and run the code linters and analyzers.

C# linter:

```bash
dotnet format --verify-no-changes --verbosity diagnostic
```

C# code analyzer:

```bash
dotnet build --configuration Lint
```

Rust linter:

```bash
cargo clippy --all-features --all-targets -- -D warnings
cargo fmt --all -- --check
```

**Note**: Task commands automatically use standardized configurations for build and test operations, ensuring consistency across development environments.

## Test framework and Style

The CSharp Valkey-Glide client uses xUnit v3 for testing code. The test code styles are defined in `.editorcofing` (see `dotnet_diagnostic.xUnit..` rules). The xUnit rules are enforced by the [xUnit analyzers](https://github.com/xunit/xunit.analyzers) referenced in the main xunit.v3 NuGet package. If you choose to use xunit.v3.core instead, you can reference xunit.analyzers explicitly. For additional info, please, refer to <https://xunit.NET> and <https://github.com/xunit/xunit>

## Community and Feedback

We encourage you to join our community to support, share feedback, and ask questions. You can approach us for anything on our Valkey Slack: [Join Valkey Slack](https://join.slack.com/t/valkey-oss-developer/shared_invite/zt-2nxs51chx-EB9hu9Qdch3GMfRcztTSkQ).
