#!/usr/bin/env python3
# TODO #490: Temporary test TODO to verify CI check (remove after verification)

"""Check that all TODOs follow the required format.

Validation rules:
  1. Every TODO must follow the format `TODO #<github_id>: <description>`.
  2. The description should provide a summary of the proposed changes, and must be at least 10 characters long.
  3. The referenced number must correspond to an open Valkey GLIDE C# GitHub issue.

Options:
  --fail-issues ID [ID ...]
      When provided, the script fails if any TODOs reference the specified GitHub issues.

Usage:
    python dev/scripts/check_todos.py
    python dev/scripts/check_todos.py --fail-issues 123 456
"""

import argparse
import os
import re
import subprocess
import sys
from fnmatch import fnmatch
from pathlib import Path
from typing import NamedTuple

from _constants import GITHUB_REPO, PROJECT_ROOT


class _Todo(NamedTuple):
    """A TODO found in the codebase."""

    file: str
    line: int
    text: str


# Used by git grep to discover TODO occurrences in tracked files.
_TODO_GREP_PATTERN = r"\bTODO\b"

# Used to validate format and extract GitHub issue ID and description.
_TODO_VALIDATION_PATTERN = re.compile(
    r"TODO #(?P<github_id>\d+): (?P<description>.+)",
)

# Minimum length for the description.
_MIN_DESCRIPTION_LENGTH = 10

# File path patterns to ignore.
_IGNORE_FILE = os.path.join(PROJECT_ROOT, "dev", "conf", "check-todos-ignore")


def _load_ignore_patterns() -> list[str]:
    """Load ignore patterns from the check-todos-ignore file."""
    if not os.path.isfile(_IGNORE_FILE):
        print(f"Error: ignore file not found: {_IGNORE_FILE}", file=sys.stderr)
        sys.exit(1)

    return [
        ignore for line in Path(_IGNORE_FILE).read_text().splitlines()
        if (ignore := line.strip()) and not ignore.startswith("#")
    ]


def _is_ignored(filepath: str, patterns: list[str]) -> bool:
    """Check if a filepath matches any exclusion pattern."""
    return any(fnmatch(filepath, p) for p in patterns)


def _find_todos() -> list[_Todo]:
    """Find all TODOs in tracked files using git grep."""
    result = subprocess.run(
        ["git", "grep", "-n", "-i", "-P", _TODO_GREP_PATTERN],
        cwd=PROJECT_ROOT,
        capture_output=True,
        text=True,
    )

    if result.returncode == 1:
        return []
    if result.returncode != 0:
        print(f"Error: git grep failed: {result.stderr.strip()}", file=sys.stderr)
        sys.exit(1)

    ignored_patterns = _load_ignore_patterns()

    todos = []
    for line in result.stdout.splitlines():
        parts = line.split(":", 2)
        if len(parts) == 3:
            filepath, line_no, text = parts
            if not _is_ignored(filepath, ignored_patterns):
                todos.append(_Todo(filepath, int(line_no), text.strip()))

    return todos


def _check_issue(github_id: int) -> str | None:
    """Check issue state. Returns an error message, or None if the issue is open."""
    result = subprocess.run(
        [
            "gh", "issue", "view", str(github_id),
            "--repo", GITHUB_REPO,
            "--json", "state",
            "--jq", ".state",
        ],
        capture_output=True,
        text=True,
    )

    if result.returncode != 0:
        return f"#{github_id} is not a valid GitHub issue"

    state = result.stdout.strip()
    if state == "OPEN":
        return None

    return f"#{github_id} is not open (state: {state})"


def _validate_todos(
    todos: list[_Todo], fail_issues: set[int]
) -> dict[_Todo, str]:
    """
    Validate TODO format and issue state.
    Returns a map from failed TODO to the corresponding reason.
    """
    failures: dict[_Todo, str] = {}
    checked_issues: dict[int, str | None] = {}

    for todo in todos:

        # Validate format.
        match = _TODO_VALIDATION_PATTERN.search(todo.text)
        if not match:
            failures[todo] = "invalid format (expected: TODO #<github_id>: <description>)"
            continue

        # Check GitHub issue
        github_id = int(match.group("github_id"))

        if fail_issues and github_id in fail_issues:
            failures[todo] = f"TODO cannot reference #{github_id}"
            continue

        if github_id not in checked_issues:
            checked_issues[github_id] = _check_issue(github_id)
        if checked_issues[github_id]:
            failures[todo] = checked_issues[github_id]
            continue

        # Check description length
        description = match.group("description")
        if len(description.strip()) < _MIN_DESCRIPTION_LENGTH:
            failures[todo] = f"description too short (must be at least {_MIN_DESCRIPTION_LENGTH} characters)"

    return failures


def main():
    # Build arguments
    parser = argparse.ArgumentParser(description="Check TODO format and issue state.")
    parser.add_argument(
        "--fail-issues",
        type=int,
        nargs="*",
        default=[],
        help="Issue IDs whose TODOs should fail validation.",
    )
    args = parser.parse_args()

    # Check TODOs
    print("Checking TODOs...\n")

    todos = _find_todos()
    failures = _validate_todos(todos, set(args.fail_issues))

    # Print results
    for todo, reason in failures.items():
        print(f"  FAIL  {todo.file}:{todo.line}")
        print(f"        {reason}\n")

    passed = len(todos) - len(failures)
    print(f"Checked {len(todos)} TODOs: {passed} passed, {len(failures)} failed.")

    sys.exit(1 if failures else 0)


if __name__ == "__main__":
    main()
