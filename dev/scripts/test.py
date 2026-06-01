#!/usr/bin/env python3

"""Runs tests.

Assembles and executes `dotnet test` commands for unit tests, integration tests,
or both. When coverage is enabled, cleans stale results first and configures
Coverlet to produce Cobertura XML output.

Usage:
    python dev/scripts/test.py
        [--unit]
        [--integration]
        [--coverage]
        [--filter MyClass]
        [--verbosity LEVEL]
        [--configuration Debug]

Options:
    --unit                  Run unit tests.
    --integration           Run integration tests.
    --coverage              Enable Coverlet code coverage collection.
    --filter NAME           Filter tests by class or method name (FullyQualifiedName~ match).
    --verbosity LEVEL       Set dotnet test verbosity (quiet|minimal|normal|detailed|diagnostic).
    --configuration CFG     Build configuration (e.g. Debug, Release). Default: Release.
"""

import argparse
import json
import os
import shutil
import subprocess
import sys

from _constants import (
    COVERAGE_BASELINE_PATH,
    COVERAGE_RESULTS_DIR_FOR_TEST_SUITE,
    COVERAGE_RUNSETTINGS_PATH,
    PROJECT_ROOT,
    TEST_PROJECT_FOR_TEST_SUITE,
    TestSuite,
)


# ---------------------------------------------------------------------------
# Helpers
# ---------------------------------------------------------------------------


def _check_server_version() -> None:
    """Verify the installed server matches the type and version specified in the coverage baseline."""

    # [1] Get required server info from coverage baseline
    with open(COVERAGE_BASELINE_PATH) as f:
        baseline = json.load(f)

    server_type = baseline["server_type"]
    server_version = baseline["server_version"]

    # [2] Check server version
    cmd_args = [f"{server_type}-server", "--version"]
    result = subprocess.run(cmd_args, capture_output=True, text=True)

    if result.returncode != 0:
        raise FileNotFoundError(f"{server_type}-server not found on PATH.")

    if server_type.lower() not in result.stdout.lower():
        raise RuntimeError(f"Coverage requires {server_type} but found: {result.stdout.strip()}")

    if f"v={server_version}." not in result.stdout:
        raise RuntimeError(f"Coverage requires version {server_version} but found: {result.stdout.strip()}")


def _build_command(
    test_suite: TestSuite,
    *,
    coverage: bool,
    filter: str | None,
    verbosity: str,
    configuration: str,
) -> list[str]:
    """Assemble the dotnet test command."""
    cmd = [
        "dotnet",
        "test",
        TEST_PROJECT_FOR_TEST_SUITE[test_suite],
        "--configuration",
        configuration,
        "--verbosity",
        verbosity,
    ]

    if coverage:
        cmd.extend(
            [
                "--collect:XPlat Code Coverage",
                "--results-directory",
                COVERAGE_RESULTS_DIR_FOR_TEST_SUITE[test_suite],
                "--settings",
                COVERAGE_RUNSETTINGS_PATH,
            ]
        )

    if filter:
        cmd.extend(["--filter", f"FullyQualifiedName~{filter}"])

    return cmd


def _run_test_suite(
    test_suite: TestSuite,
    *,
    coverage: bool,
    filter: str | None,
    verbosity: str,
    configuration: str,
) -> int:
    """Run a single test suite. Returns the process exit code."""
    if coverage:
        results_dir = COVERAGE_RESULTS_DIR_FOR_TEST_SUITE[test_suite]
        if os.path.exists(results_dir):
            shutil.rmtree(results_dir)
        os.makedirs(results_dir, exist_ok=True)

    cmd = _build_command(
        test_suite,
        coverage=coverage,
        filter=filter,
        verbosity=verbosity,
        configuration=configuration,
    )
    result = subprocess.run(cmd, cwd=PROJECT_ROOT, check=False)

    return result.returncode


# ---------------------------------------------------------------------------
# Main
# ---------------------------------------------------------------------------


def _parse_args() -> argparse.Namespace:
    parser = argparse.ArgumentParser(
        description="Runs tests.",
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
    parser.add_argument(
        "--verbosity",
        type=str,
        default="normal",
        choices=["quiet", "minimal", "normal", "detailed", "diagnostic"],
        help="Set dotnet test verbosity level (default: normal)",
    )
    parser.add_argument(
        "--configuration",
        type=str,
        default="Release",
        help="Build configuration (default: Release)",
    )

    return parser.parse_args()


def main() -> int:
    args = _parse_args()

    # Verify server version when collecting coverage.
    if args.coverage:
        _check_server_version()

    # Determine which test suites to run (default to all).
    if not args.unit and not args.integration:
        test_suites = list(TestSuite)
    else:
        test_suites = []
        if args.unit:
            test_suites.append(TestSuite.UNIT)
        if args.integration:
            test_suites.append(TestSuite.INTEGRATION)

    # Run all test suites and report overall result.
    failed = []
    for test_suite in test_suites:
        exit_code = _run_test_suite(
            test_suite,
            coverage=args.coverage,
            filter=args.filter,
            verbosity=args.verbosity,
            configuration=args.configuration,
        )

        if exit_code != 0:
            failed.append(test_suite.value)

    if failed:
        print(f"\nTest suites failed: {', '.join(failed)}")
        return 1

    return 0


if __name__ == "__main__":
    sys.exit(main())
