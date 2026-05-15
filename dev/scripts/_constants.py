"""Shared constants for dev scripts."""

import os
from enum import Enum

# ---------------------------------------------------------------------------
# Paths
# ---------------------------------------------------------------------------

SCRIPTS_DIR = os.path.dirname(os.path.abspath(__file__))
PROJECT_ROOT = os.path.dirname(os.path.dirname(SCRIPTS_DIR))

COVERAGE_DIR = os.path.join(PROJECT_ROOT, "dev", "coverage")
COVERAGE_RESULTS_DIR = os.path.join(COVERAGE_DIR, "results")
COVERAGE_REPORTS_DIR = os.path.join(COVERAGE_DIR, "reports")
COVERAGE_BASELINE_PATH = os.path.join(COVERAGE_DIR, "coverage-baseline.json")
COVERAGE_RUNSETTINGS_PATH = os.path.join(COVERAGE_DIR, ".runsettings")

COVERAGE_REPORTS_DIR_COMBINED = os.path.join(COVERAGE_REPORTS_DIR, "combined")
COVERAGE_REPORT_INDEX_COMBINED = os.path.join(
    COVERAGE_REPORTS_DIR_COMBINED, "index.html"
)
COVERAGE_SUMMARY_COMBINED = os.path.join(COVERAGE_REPORTS_DIR_COMBINED, "Summary.json")

# ---------------------------------------------------------------------------
# Enums
# ---------------------------------------------------------------------------


class TestSuite(str, Enum):
    """Test suite identifiers."""

    UNIT = "unit"
    INTEGRATION = "integration"


ALL_TEST_SUITES = list(TestSuite)

# ---------------------------------------------------------------------------
# Maps
# ---------------------------------------------------------------------------

# Maps test suite → test project (.csproj) path.
TEST_PROJECT_FOR_TEST_SUITE = {
    TestSuite.UNIT: os.path.join(
        PROJECT_ROOT,
        "tests",
        "Valkey.Glide.UnitTests",
        "Valkey.Glide.UnitTests.csproj",
    ),
    TestSuite.INTEGRATION: os.path.join(
        PROJECT_ROOT,
        "tests",
        "Valkey.Glide.IntegrationTests",
        "Valkey.Glide.IntegrationTests.csproj",
    ),
}

# Maps test suite → coverage results directory.
COVERAGE_RESULTS_DIR_FOR_TEST_SUITE = {
    TestSuite.UNIT: os.path.join(COVERAGE_RESULTS_DIR, "unit"),
    TestSuite.INTEGRATION: os.path.join(COVERAGE_RESULTS_DIR, "integration"),
}

# Maps test suite → coverage reports directory.
COVERAGE_REPORTS_DIR_FOR_TEST_SUITE = {
    TestSuite.UNIT: os.path.join(COVERAGE_REPORTS_DIR, "unit"),
    TestSuite.INTEGRATION: os.path.join(COVERAGE_REPORTS_DIR, "integration"),
}

# Maps test suite → coverage report index.html path.
COVERAGE_REPORT_INDEX_FOR_TEST_SUITE = {
    TestSuite.UNIT: os.path.join(COVERAGE_REPORTS_DIR, "unit", "index.html"),
    TestSuite.INTEGRATION: os.path.join(
        COVERAGE_REPORTS_DIR, "integration", "index.html"
    ),
}

# Maps test suite → coverage Summary.json path.
COVERAGE_SUMMARY_FOR_TEST_SUITE = {
    TestSuite.UNIT: os.path.join(COVERAGE_REPORTS_DIR, "unit", "Summary.json"),
    TestSuite.INTEGRATION: os.path.join(
        COVERAGE_REPORTS_DIR, "integration", "Summary.json"
    ),
}
