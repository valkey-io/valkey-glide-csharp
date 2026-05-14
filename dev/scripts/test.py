#!/usr/bin/env python3

"""Run .NET tests for the Valkey GLIDE C# client.

Assembles and executes `dotnet test` commands for unit tests, integration tests,
or both. When coverage is enabled, cleans stale results first and configures
Coverlet to produce Cobertura XML output.

Usage:
    python dev/scripts/test.py                    # Run all tests, no coverage
    python dev/scripts/test.py --unit             # Unit tests only
    python dev/scripts/test.py --integration      # Integration tests only
    python dev/scripts/test.py --coverage         # All tests with coverage
    python dev/scripts/test.py --unit --coverage  # Unit tests with coverage
    python dev/scripts/test.py --filter MyClass   # Filter by class/method name
"""

import argparse
import os
import shutil
import subprocess
import sys

# ---------------------------------------------------------------------------
# Constants
# ---------------------------------------------------------------------------

_SCRIPTS_DIR = os.path.dirname(os.path.abspath(__file__))
_PROJECT_ROOT = os.path.dirname(os.path.dirname(_SCRIPTS_DIR))

_UNIT_TEST_PROJECT = os.path.join(
    _PROJECT_ROOT,
    "tests",
    "Valkey.Glide.UnitTests",
    "Valkey.Glide.UnitTests.csproj",
)
_INTEGRATION_TEST_PROJECT = os.path.join(
    _PROJECT_ROOT,
    "tests",
    "Valkey.Glide.IntegrationTests",
    "Valkey.Glide.IntegrationTests.csproj",
)

_PROJECT_MAP = {
    "unit": _UNIT_TEST_PROJECT,
    "integration": _INTEGRATION_TEST_PROJECT
}

_COVERAGE_DIR = os.path.join(_PROJECT_ROOT, "dev", "coverage")
_COVERAGE_RESULTS_DIR = os.path.join(_COVERAGE_DIR, "results")
_RUNSETTINGS_PATH = os.path.join(_COVERAGE_DIR, ".runsettings")


# ---------------------------------------------------------------------------
# Helpers
# ---------------------------------------------------------------------------


def _build_command(suite: str, *, coverage: bool, filter: str | None) -> list[str]:
    """Assemble the dotnet test command."""
    cmd = [
        "dotnet",
        "test",
        _PROJECT_MAP[suite],
        "--configuration",
        "Release",
        "--verbosity",
        "normal",
    ]

    if coverage:
        cmd.extend([
            "--collect:XPlat Code Coverage",
            "--results-directory",
            os.path.join(_COVERAGE_RESULTS_DIR, suite),
            "--settings",
            _RUNSETTINGS_PATH,
        ])

    if filter:
        cmd.extend(["--filter", f"FullyQualifiedName~{filter}"])

    return cmd


def _run_suite(suite: str, *, coverage: bool, filter: str | None) -> int:
    """Run a single test suite. Returns the process exit code."""
    if coverage:
        results_dir = os.path.join(_COVERAGE_RESULTS_DIR, suite)
        if os.path.exists(results_dir):
            shutil.rmtree(results_dir)
        os.makedirs(results_dir, exist_ok=True)

    cmd = _build_command(suite, coverage=coverage, filter=filter)
    result = subprocess.run(cmd, cwd=_PROJECT_ROOT, check=False)

    return result.returncode


# ---------------------------------------------------------------------------
# Main
# ---------------------------------------------------------------------------


def _parse_args() -> argparse.Namespace:
    parser = argparse.ArgumentParser(
        description="Run .NET tests with optional coverage collection.",
    )

    parser.add_argument(
        "--unit",
        action="store_true",
        help="Run unit tests",
    )
    parser.add_argument(
        "--integration",
        action="store_true",
        help="Run integration tests",
    )
    parser.add_argument(
        "--all",
        action="store_true",
        help="Run all tests",
    )

    parser.add_argument(
        "--coverage",
        action="store_true",
        default=False,
        help="Enable code coverage collection",
    )

    parser.add_argument(
        "--filter",
        type=str,
        default=None,
        help="Filter tests by class or method name (FullyQualifiedName~ match)",
    )

    return parser.parse_args()


def main() -> int:
    args = _parse_args()
    coverage = args.coverage

    # Determine which suites to run (default to all).
    if args.all:
        suites = ["unit", "integration"]
    elif not args.unit and not args.integration:
        suites = ["unit", "integration"]
    else:
        suites = []
        if args.unit:
            suites.append("unit")
        if args.integration:
            suites.append("integration")

    # Run all suites and report overall result.
    failed_suites = []
    for suite in suites:
        exit_code = _run_suite(suite, coverage=coverage, filter=args.filter)
        if exit_code != 0:
            failed_suites.append(suite)

    if failed_suites:
        print(f"\nFailed suites: {', '.join(failed_suites)}")
        return 1

    return 0


if __name__ == "__main__":
    sys.exit(main())
