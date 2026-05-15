#!/usr/bin/env python3

"""Manages code coverage.

Provides commands for generating reports, checking coverage against the
baseline, updating the baseline, and cleaning artifacts.

Usage:
    python dev/scripts/coverage.py report  [--unit] [--integration]
    python dev/scripts/coverage.py check
    python dev/scripts/coverage.py update
    python dev/scripts/coverage.py clean   [--unit] [--integration]

Commands:
    report          Generate HTML and JSON coverage reports from coverage data.
    check           Compare measured coverage against the baseline.
    update          Updates the coverage baseline.
    clean           Remove coverage results and reports.

Options:
    --unit          Target unit test coverage only.
    --integration   Target integration test coverage only.
"""

import argparse
import json
import os
import shutil
import subprocess
import sys

from _constants import (
    ALL_TEST_SUITES,
    COVERAGE_BASELINE_PATH,
    COVERAGE_REPORT_INDEX_COMBINED,
    COVERAGE_REPORT_INDEX_FOR_TEST_SUITE,
    COVERAGE_REPORTS_DIR_COMBINED,
    COVERAGE_REPORTS_DIR_FOR_TEST_SUITE,
    COVERAGE_RESULTS_DIR,
    COVERAGE_RESULTS_DIR_FOR_TEST_SUITE,
    COVERAGE_SUMMARY_COMBINED,
    COVERAGE_SUMMARY_FOR_TEST_SUITE,
    PROJECT_ROOT,
    TestSuite,
)

# ---------------------------------------------------------------------------
# Constants
# ---------------------------------------------------------------------------

_REPORTGENERATOR_ASSEMBLY_FILTERS = "+Valkey.Glide*"
_REPORTGENERATOR_CLASS_FILTERS = (
    "-Valkey.Glide.UnitTests*;"
    "-Valkey.Glide.IntegrationTests*;"
    "-Valkey.Glide.TestUtils*"
)
_REPORTGENERATOR_TYPES = "Html;JsonSummary"


# ---------------------------------------------------------------------------
# Commands
# ---------------------------------------------------------------------------


def _cmd_report(test_suites: list[TestSuite]) -> int:
    """Generate HTML and JSON coverage reports from coverage data."""

    # [A] Generate per-suite reports
    # ------------------------------

    for test_suite in test_suites:
        coverage_dir = COVERAGE_RESULTS_DIR_FOR_TEST_SUITE[test_suite]

        # Check if this test suite has data.
        has_data = any(
            f == "coverage.cobertura.xml"
            for _, _, files in os.walk(coverage_dir)
            for f in files
        )

        if not has_data:
            raise FileNotFoundError(
                f"No coverage data found for {test_suite.value} in {coverage_dir}. "
                "Run tests with --coverage first."
            )

        print(f"Generating {test_suite.value} coverage report...")

        subprocess.run(
            [
                "reportgenerator",
                f"-reports:{os.path.join(coverage_dir, '**', 'coverage.cobertura.xml')}",
                f"-targetdir:{COVERAGE_REPORTS_DIR_FOR_TEST_SUITE[test_suite]}",
                f"-reporttypes:{_REPORTGENERATOR_TYPES}",
                f"-assemblyfilters:{_REPORTGENERATOR_ASSEMBLY_FILTERS}",
                f"-classfilters:{_REPORTGENERATOR_CLASS_FILTERS}",
            ],
            cwd=PROJECT_ROOT,
            check=True,
        )

    # [B] Generate combined report
    # ----------------------------

    print("Generating combined coverage report...")

    subprocess.run(
        [
            "reportgenerator",
            f"-reports:{os.path.join(COVERAGE_RESULTS_DIR, '**', 'coverage.cobertura.xml')}",
            f"-targetdir:{COVERAGE_REPORTS_DIR_COMBINED}",
            f"-reporttypes:{_REPORTGENERATOR_TYPES}",
            f"-assemblyfilters:{_REPORTGENERATOR_ASSEMBLY_FILTERS}",
            f"-classfilters:{_REPORTGENERATOR_CLASS_FILTERS}",
        ],
        cwd=PROJECT_ROOT,
        check=True,
    )

    # Print clickable file:// links to generated reports.
    print("\nCoverage reports generated:")
    for test_suite in test_suites:
        print(f"  {test_suite.value}:")
        print(f"    HTML:    file://{COVERAGE_REPORT_INDEX_FOR_TEST_SUITE[test_suite]}")
        print(f"    JSON:    file://{COVERAGE_SUMMARY_FOR_TEST_SUITE[test_suite]}")

    print("  combined:")
    print(f"    HTML:    file://{COVERAGE_REPORT_INDEX_COMBINED}")
    print(f"    JSON:    file://{COVERAGE_SUMMARY_COMBINED}")

    return 0


def _cmd_check(test_suites: list[TestSuite]) -> int:
    """Compare measured coverage against the baseline."""
    # TODO: Implement in Task 2.
    print("coverage check not yet implemented")
    return 0


def _cmd_update(test_suites: list[TestSuite]) -> int:
    """Updates the coverage baseline."""
    # TODO: Implement in Task 2.
    print("coverage update not yet implemented")
    return 0


def _cmd_clean(test_suites: list[TestSuite]) -> int:
    """Remove coverage results and reports."""
    for test_suite in test_suites:
        results_dir = COVERAGE_RESULTS_DIR_FOR_TEST_SUITE[test_suite]
        if os.path.exists(results_dir):
            shutil.rmtree(results_dir)

        reports_dir = COVERAGE_REPORTS_DIR_FOR_TEST_SUITE[test_suite]
        if os.path.exists(reports_dir):
            shutil.rmtree(reports_dir)

    # Clean combined reports only when all test suites are targeted.
    if set(test_suites) == set(ALL_TEST_SUITES):
        if os.path.exists(COVERAGE_REPORTS_DIR_COMBINED):
            shutil.rmtree(COVERAGE_REPORTS_DIR_COMBINED)

    print("Coverage results and reports cleaned.")
    return 0


# ---------------------------------------------------------------------------
# Main
# ---------------------------------------------------------------------------

_COMMANDS = {
    "report": _cmd_report,
    "check": _cmd_check,
    "update": _cmd_update,
    "clean": _cmd_clean,
}


def _parse_args() -> argparse.Namespace:
    parser = argparse.ArgumentParser(
        description="Manages code coverage.",
    )

    parser.add_argument(
        "command",
        choices=_COMMANDS.keys(),
        help="Command to run",
    )
    parser.add_argument(
        "--unit",
        action="store_true",
        help="Target unit test coverage",
    )
    parser.add_argument(
        "--integration",
        action="store_true",
        help="Target integration test coverage",
    )

    return parser.parse_args()


def main() -> int:
    args = _parse_args()

    # Determine target test suites (default to all).
    if not args.unit and not args.integration:
        test_suites = ALL_TEST_SUITES
    else:
        test_suites = []
        if args.unit:
            test_suites.append(TestSuite.UNIT)
        if args.integration:
            test_suites.append(TestSuite.INTEGRATION)

    return _COMMANDS[args.command](test_suites)


if __name__ == "__main__":
    sys.exit(main())
