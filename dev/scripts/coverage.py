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
    report          Generate coverage reports.
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
    COVERAGE_BASELINE_PATH,
    COVERAGE_REPORT_COMBINED_INDEX_PATH,
    COVERAGE_REPORT_INDEX_FOR_TEST_SUITE,
    COVERAGE_REPORTS_COMBINED_DIR,
    COVERAGE_REPORTS_DIR_FOR_TEST_SUITE,
    COVERAGE_RESULTS_DIR,
    COVERAGE_RESULTS_DIR_FOR_TEST_SUITE,
    COVERAGE_REPORT_COMBINED_SUMMARY_PATH,
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
# Helpers
# ---------------------------------------------------------------------------


def _get_coverage() -> dict:
    """Read measured coverage and baseline, return comparison data.

    Returns:
        {
            "line": {"measured": float, "baseline": float, "comparison": int},
            "branch": {"measured": float, "baseline": float, "comparison": int},
        }

    comparison is -1 (regressed), 0 (unchanged), or 1 (improved).
    """
    if not os.path.exists(COVERAGE_REPORT_COMBINED_SUMMARY_PATH):
        raise FileNotFoundError(
            f"Coverage summary not found at {COVERAGE_REPORT_COMBINED_SUMMARY_PATH}. Run 'coverage.py report' first."
        )

    with open(COVERAGE_REPORT_COMBINED_SUMMARY_PATH) as f:
        summary = json.load(f)["summary"]

    measured_line = summary["linecoverage"]
    measured_branch = summary["branchcoverage"]

    with open(COVERAGE_BASELINE_PATH) as f:
        baseline = json.load(f)

    baseline_line = baseline["line_coverage"]
    baseline_branch = baseline["branch_coverage"]

    return {
        "line": {
            "measured": measured_line,
            "baseline": baseline_line,
            "comparison": (measured_line > baseline_line) - (measured_line < baseline_line),
        },
        "branch": {
            "measured": measured_branch,
            "baseline": baseline_branch,
            "comparison": (measured_branch > baseline_branch) - (measured_branch < baseline_branch),
        },
    }


def _print_coverage_comparison(coverage: dict):
    """Print coverage status for each metric."""

    for key, entry in coverage.items():
        if entry["comparison"] < 0:
            print(f"FAILED: {key} coverage decreased ({entry['baseline']}% -> {entry['measured']}%)")
        elif entry["comparison"] == 0:
            print(f"PASSED: {key} coverage unchanged ({entry['baseline']}%)")
        else:
            print(f"FAILED: {key} coverage increased ({entry['baseline']}% -> {entry['measured']}%)")


# ---------------------------------------------------------------------------
# Commands
# ---------------------------------------------------------------------------


def _cmd_report(test_suites: list[TestSuite]) -> bool:
    """Generate HTML and JSON coverage reports from coverage data."""
    for test_suite in test_suites:
        coverage_dir = COVERAGE_RESULTS_DIR_FOR_TEST_SUITE[test_suite]

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
                "dotnet",
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

    print("Generating combined coverage report...")

    subprocess.run(
        [
            "dotnet",
            "reportgenerator",
            f"-reports:{os.path.join(COVERAGE_RESULTS_DIR, '**', 'coverage.cobertura.xml')}",
            f"-targetdir:{COVERAGE_REPORTS_COMBINED_DIR}",
            f"-reporttypes:{_REPORTGENERATOR_TYPES}",
            f"-assemblyfilters:{_REPORTGENERATOR_ASSEMBLY_FILTERS}",
            f"-classfilters:{_REPORTGENERATOR_CLASS_FILTERS}",
        ],
        cwd=PROJECT_ROOT,
        check=True,
    )

    print("\nCoverage reports generated:")

    for test_suite in test_suites:
        print(f"  {test_suite.value}:")
        print(f"    HTML:    file://{COVERAGE_REPORT_INDEX_FOR_TEST_SUITE[test_suite]}")
        print(f"    JSON:    file://{COVERAGE_SUMMARY_FOR_TEST_SUITE[test_suite]}")

    print("  combined:")
    print(f"    HTML:    file://{COVERAGE_REPORT_COMBINED_INDEX_PATH}")
    print(f"    JSON:    file://{COVERAGE_REPORT_COMBINED_SUMMARY_PATH}")

    return True


def _cmd_check() -> bool:
    """Compare measured coverage against the baseline."""
    coverage = _get_coverage()
    _print_coverage_comparison(coverage)

    return all(entry["comparison"] == 0 for entry in coverage.values())


def _cmd_update() -> bool:
    """Updates the coverage baseline."""
    coverage = _get_coverage()
    _print_coverage_comparison(coverage)

    if any(entry["comparison"] < 0 for entry in coverage.values()):
        return False

    if all(entry["comparison"] == 0 for entry in coverage.values()):
        return True

    with open(COVERAGE_BASELINE_PATH) as f:
        baseline_data = json.load(f)

    for key, entry in coverage.items():
        json_key = "line_coverage" if key == "line" else "branch_coverage"
        if entry["comparison"] > 0:
            baseline_data[json_key] = entry["measured"]

    with open(COVERAGE_BASELINE_PATH, "w") as f:
        json.dump(baseline_data, f, indent=2)
        f.write("\n")

    return True


def _cmd_clean(test_suites: list[TestSuite]) -> bool:
    """Remove coverage results and reports."""
    for test_suite in test_suites:
        results_dir = COVERAGE_RESULTS_DIR_FOR_TEST_SUITE[test_suite]
        if os.path.exists(results_dir):
            shutil.rmtree(results_dir)

        reports_dir = COVERAGE_REPORTS_DIR_FOR_TEST_SUITE[test_suite]
        if os.path.exists(reports_dir):
            shutil.rmtree(reports_dir)

    if set(test_suites) == set(TestSuite):
        if os.path.exists(COVERAGE_REPORTS_COMBINED_DIR):
            shutil.rmtree(COVERAGE_REPORTS_COMBINED_DIR)

    print("Coverage results and reports cleaned.")

    return True


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

    # Check and update commands
    if args.command in ("check", "update"):
        if args.unit:
            print(f"Error: --unit not supported for '{args.command}'")
            return 1
        if args.integration:
            print(f"Error: --integration not supported for '{args.command}'")
            return 1
        return 0 if _COMMANDS[args.command]() else 1

    # Report and clean commands
    if not args.unit and not args.integration:
        test_suites = list(TestSuite)
    else:
        test_suites = []
        if args.unit:
            test_suites.append(TestSuite.UNIT)
        if args.integration:
            test_suites.append(TestSuite.INTEGRATION)

    return 0 if _COMMANDS[args.command](test_suites) else 1

if __name__ == "__main__":
    sys.exit(main())
