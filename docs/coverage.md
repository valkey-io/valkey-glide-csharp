# Code Coverage

This project uses a coverage ratchet mechanism that prevents coverage from regressing. Coverage can only stay the same or go up.

## How It Works

The coverage pipeline has three stages:

1. **Collect** — Run tests with coverage enabled: `task test coverage=true`
2. **Report** — Generate HTML and JSON reports: `task coverage:report`
3. **Enforce** — Validate against the baseline: `task coverage:check`

## Local Usage

### Prerequisites

- Python 3.10+
- [ReportGenerator](https://github.com/danielpalme/ReportGenerator) (`task install-tools`)

### Running Coverage Locally

```bash
# Run all tests with coverage collection
task test coverage=true

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

The coverage check passes when measured coverage is **at or above** the baseline value. Any decrease fails the check.

The baseline is stored in `dev/coverage/coverage-baseline.json` and is automatically updated on the main branch when coverage improves.

## CI Behavior

- **Pull requests**: `coverage:check` runs in validation-only mode. The build fails if coverage regresses.
- **Main branch pushes**: `coverage:update` runs, which validates and then ratchets the baseline upward if coverage improved.

## File Layout

```
dev/coverage/
├── coverage-baseline.json   # Committed baseline (ratchet floor)
├── .runsettings             # Coverlet configuration
├── results/                 # Raw .cobertura.xml (gitignored)
│   ├── unit/
│   └── integration/
└── reports/                 # Generated HTML/JSON (gitignored)
    ├── unit/
    ├── integration/
    └── combined/
```

## Troubleshooting

### Coverage check is failing

1. Run `task test coverage=true` to collect fresh coverage data
2. Run `task coverage:report` to generate reports
3. Run `task coverage:check` to see which metric regressed
4. Add tests to cover the missing lines/branches

### Verifying the current baseline

```bash
cat dev/coverage/coverage-baseline.json
```

### Reports not generating

Ensure `reportgenerator` is installed: `task install-tools`
