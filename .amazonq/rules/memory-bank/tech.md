# Technology Stack

## Programming Languages

### C# / .NET
- **Target Frameworks**: .NET 6.0 and .NET 8.0
- **Language Version**: Preview (latest C# features)
- **Features Used**:
  - Async/await for asynchronous operations
  - Nullable reference types enabled
  - Implicit usings enabled
  - Unsafe blocks for FFI interop

### Rust
- **Purpose**: FFI layer and core library integration
- **Build Tool**: Cargo
- **Key Features**:
  - Memory safety guarantees
  - Multi-threaded async runtime
  - Foreign Function Interface (FFI) to C#

## Build System

### .NET CLI
- **Primary Build Tool**: `dotnet build`
- **Solution File**: `Valkey.Glide.sln`
- **Project File**: `Valkey.Glide.csproj`

### Cargo (Rust)
- **Build Command**: `cargo build` (debug) or `cargo build --release` (release)
- **Working Directory**: `rust/`
- **Integration**: Pre-build event in MSBuild triggers Cargo build
- **Output**: Native libraries (`.dll`, `.so`, `.dylib`)

### Build Configurations
- **Debug**: Development builds with debugging symbols
- **Release**: Optimized production builds
- **Lint**: Code analysis with warnings as errors

## Dependencies

### NuGet Packages
- **xUnit v3**: Testing framework
- **xUnit Analyzers**: Code analysis for test quality

### Rust Dependencies
- Defined in `rust/Cargo.toml`
- Links to `valkey-glide/glide-core` for core functionality

### Git Submodules
- **valkey-glide**: Core library shared across all language clients
- Contains Rust core, FFI definitions, and protocol implementations

## Development Commands

### Build
```bash
# Build all projects
dotnet build

# Build specific configuration
dotnet build --configuration Debug
dotnet build --configuration Release
dotnet build --configuration Lint

# Skip Cargo build (use existing native libraries)
dotnet build /p:SkipCargo=true
```

### Test
```bash
# Run all tests sequentially
dotnet test -m:1

# Run tests in parallel
dotnet test

# Run tests for specific framework
dotnet test --framework net8.0
dotnet test --framework net6.0

# Run with logging
dotnet test --logger "console;verbosity=detailed"
dotnet test --logger "html;LogFileName=TestReport.html"

# Filter tests
dotnet test --filter "FullyQualifiedName~TestName"

# Test against existing endpoints
cluster-endpoints=localhost:7000 standalone-endpoints=localhost:6379 dotnet test
```

### Lint
```bash
# C# formatting check
dotnet format --verify-no-changes --verbosity diagnostic

# C# code analysis
dotnet build --configuration Lint

# Rust linting
cd rust
cargo clippy --all-features --all-targets -- -D warnings
cargo fmt --all -- --check
```

### Benchmark
```bash
cd benchmarks
dotnet run --framework net8.0 --dataSize 1024 --resultsFile test.json --concurrentTasks 4 --clients all --host localhost --clientCount 4
```

## Project Configuration

### MSBuild Properties
- **TargetFrameworks**: `net8.0;net6.0`
- **LangVersion**: `preview`
- **Nullable**: `enable`
- **ImplicitUsings**: `enable`
- **AllowUnsafeBlocks**: `true` (for FFI)
- **GenerateDocumentationFile**: `true`

### Code Analysis (Lint Configuration)
- **EnforceCodeStyleInBuild**: `true`
- **RunAnalyzersDuringBuild**: `true`
- **EnableNETAnalyzers**: `true`
- **TreatWarningsAsErrors**: `true`
- **AnalysisLevel**: `latest`

### Suppressed Warnings (Non-Lint)
- CS1591: Missing XML documentation
- CS1573: Parameter has no matching param tag
- CS1587: XML comment is not placed on a valid element

## Platform Support

### Operating Systems
- **Windows**: Native `.dll` support
- **Linux**: Native `.so` support
- **macOS**: Native `.dylib` support

### Platform Detection
- MSBuild conditions: `$([MSBuild]::IsOSPlatform('Windows'))`, `IsOSPlatform('Linux')`, `IsOSPlatform('OSX')`
- Native libraries automatically selected based on platform

## Testing Framework

### xUnit v3
- **Test Discovery**: Automatic via xUnit runner
- **Test Execution**: Parallel by default (can be disabled with `-m:1`)
- **Configuration**: `xunit.runner.json` in test projects
- **Analyzers**: xUnit analyzers enforce test quality standards

### Test Configuration
- **Style Rules**: Defined in `.editorconfig` (`dotnet_diagnostic.xUnit..` rules)
- **Failure Handling**: Custom `TestFailureHandler.cs` in test projects
- **Console Capture**: `ConsoleCapturingTestBase.cs` and `ConsoleOutputInterceptor.cs` for output testing

## Package Management

### NuGet
- **Package ID**: `Valkey.Glide`
- **Nuspec File**: `Valkey.Glide.nuspec`
- **Version**: Configured in project file (default: 255.255.255)
- **Symbol Package**: `.snupkg` format with portable PDB

### Distribution
- **Include Build Output**: `true`
- **Include Symbols**: `true`
- **Debug Type**: `portable`
- **Deterministic**: `true`

## Development Tools

### Required Software
- **.NET SDK**: Version 8 and 9
- **Rust**: rustup, cargo
- **Git**: For submodule management
- **Valkey/Redis**: For testing (valkey-server, valkey-cli)
- **Protocol Buffers**: protoc compiler (v25.1+)

### Optional Tools
- **Visual Studio**: Full IDE support with solution file
- **VS Code**: Lightweight editor with C# extension
- **ziglang & cargo-zigbuild**: For GNU Linux cross-compilation

### Platform-Specific Dependencies

**Ubuntu/Debian**:
- openssl, openssl-dev
- gcc, cmake, pkg-config

**macOS**:
- openssl (via Homebrew)
- gcc, cmake, pkgconfig (via Homebrew)

## CI/CD

### GitHub Actions
- **Workflows**: `.github/workflows/`
- **Test Matrices**: JSON-based OS and version matrices
- **Code Quality**: CodeQL, Semgrep, YAML linting
- **Dependency Management**: Dependabot configuration
