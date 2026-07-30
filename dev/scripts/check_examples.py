#!/usr/bin/env python3

"""Check code examples by extracting and compiling them.

Extracts C# code examples from XML doc comments in the source tree, then
compiles each against the built Valkey.Glide DLL to verify correctness.

Usage:
    python dev/scripts/check_examples.py
"""

import os
import subprocess
import sys
import tempfile

from _constants import LIBRARY_DIR, SCRIPTS_DIR


def _find_glide_dll() -> str | None:
    """Locate the built Valkey.Glide.dll, preferring Release over Debug."""
    for config in ("Release", "Debug"):
        path = os.path.join(
            LIBRARY_DIR,
            "bin",
            config,
            "net8.0",
            "Valkey.Glide.dll",
        )
        if os.path.exists(path):
            return path
    return None


def main():
    # Locate the built DLL before running extraction/validation.
    glide_dll = _find_glide_dll()
    if glide_dll is None:
        print(
            "Error: Built DLL not found. Run 'dotnet build' first.",
            file=sys.stderr,
        )
        sys.exit(1)

    with tempfile.NamedTemporaryFile(
        suffix=".json", prefix="examples_", delete=False
    ) as tmp_file:
        tmp_path = tmp_file.name

    try:
        # Step 1: Extract examples
        result = subprocess.run(
            [
                sys.executable,
                os.path.join(SCRIPTS_DIR, "extract_examples.py"),
                "--examples",
                tmp_path,
            ],
            check=False,
        )
        if result.returncode != 0:
            sys.exit(result.returncode)

        # Step 2: Validate examples
        result = subprocess.run(
            [
                sys.executable,
                os.path.join(SCRIPTS_DIR, "validate_examples.py"),
                "--examples",
                tmp_path,
                "--glide-dll",
                glide_dll,
                "--add-imports",
                "--add-clients",
            ],
            check=False,
        )
        sys.exit(result.returncode)

    finally:
        if os.path.exists(tmp_path):
            os.unlink(tmp_path)


if __name__ == "__main__":
    main()
