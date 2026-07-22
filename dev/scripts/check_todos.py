#!/usr/bin/env python3

"""Check that all TODO comments reference an open GitHub issue.

Validation rules:
  1. Every TODO must include a GitHub issue reference with format `TODO #<number>`.
  2. The referenced number must correspond to an existing Valkey GLIDE C# GitHub issue.
  3. The referenced issue must be open.

Usage:
    python dev/scripts/check_todos.py
"""

import re
import subprocess
import sys
from typing import NamedTuple

from _constants import GITHUB_REPO, PROJECT_ROOT


class _Todo(NamedTuple):
    """A TODO comment found in the codebase."""

    file: str
    line: int
    text: str


# Matches a TODO inside a comment, optionally capturing the issue number.
# A match with no `github_id` capture means the TODO is missing an issue reference.
_TODO_PATTERN = re.compile(
    r"(//|#|/\*|\*|<!--).*TODO\b(\s+#(?P<github_id>\d+))?", re.IGNORECASE
)


def _find_todos() -> list[_Todo]:
    """Find all TODO comments in tracked files using git grep."""
    result = subprocess.run(
        ["git", "grep", "-n", "-i", "-P", _TODO_PATTERN.pattern],
        cwd=PROJECT_ROOT,
        capture_output=True,
        text=True,
    )

    if result.returncode == 1:
        return []
    if result.returncode != 0:
        print(f"Error: git grep failed: {result.stderr.strip()}", file=sys.stderr)
        sys.exit(1)

    todos = []
    for line in result.stdout.splitlines():
        parts = line.split(":", 2)
        if len(parts) == 3:
            filepath, line_no, text = parts
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


def _validate_todos(todos: list[_Todo]) -> dict[_Todo, str]:
    """Validate TODO format and issue state. Returns a map from failed TODO to the corresponding reason."""
    failures: dict[_Todo, str] = {}
    checked_issues: dict[int, str | None] = {}

    for todo in todos:
        match = _TODO_PATTERN.search(todo.text)
        github_id = match.group("github_id")

        if not github_id:
            failures[todo] = "missing GitHub issue reference"
        else:
            issue_num = int(github_id)
            if issue_num not in checked_issues:
                checked_issues[issue_num] = _check_issue(issue_num)
            if checked_issues[issue_num]:
                failures[todo] = checked_issues[issue_num]

    return failures


def main():
    print("Checking TODO comments...\n")

    todos = _find_todos()
    failures = _validate_todos(todos)

    for todo, reason in failures.items():
        print(f"  FAIL  {todo.file}:{todo.line}")
        print(f"        {reason}\n")

    passed = len(todos) - len(failures)
    print(f"Checked {len(todos)} TODOs: {passed} passed, {len(failures)} failed.")

    sys.exit(1 if failures else 0)


if __name__ == "__main__":
    main()
