# Code Coverage

This project includes support for measuring line and branch coverage, including a coverage baseline and checks to ensure coverage does not decrease.

## How It Works

The coverage pipeline has three stages:

1. **Collect** — Run tests with coverage enabled: `task test coverage=true`
2. **Report** — Generate HTML and JSON reports: `task coverage:report`
3. **Enforce** — Validate against the baseline: `task coverage:check`

## Local Usage

### Prerequisites

- Python 3.10+

### Running Coverage Locally

```bash
# Run all tests with coverage collection
task test coverage=true

# Install coverage reporting tools
task coverage:install

# Generate reports (HTML + JSON)
task coverage:report

# Check coverage against baseline
task coverage:check

# Update baseline (if coverage improved)
task coverage:update

# Clean coverage artifacts
task coverage:clean
```

## Threshold

The coverage check passes only when measured coverage **exactly matches** the baseline. Any change (increase or decrease) fails the check — run `task coverage:update` to increase the baseline.

## CI Behavior

- `coverage:check` runs on pull requests after tests. The build fails if coverage differs from the baseline.
- **Baseline updates**: Run `task coverage:update` locally and commit the updated `coverage-baseline.json`.

## File Layout

```text
dev/coverage/
├── coverage-baseline.json   # Coverage baseline
├── .runsettings             # Coverlet configuration
├── results/                 # Raw .cobertura.xml coverage results
│   ├── unit/
│   └── integration/
└── reports/                 # Generated HTML/JSON reports
    ├── unit/
    ├── integration/
    └── combined/
```
