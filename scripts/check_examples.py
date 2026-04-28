#!/usr/bin/env python3

"""Check Valkey GLIDE C# code examples by extracting and compiling them.

Usage:
    python scripts/check_examples.py
"""

import os
import subprocess
import sys
import tempfile

_SCRIPTS_DIR = os.path.dirname(os.path.abspath(__file__))
_PROJECT_ROOT = os.path.dirname(_SCRIPTS_DIR)


def _find_glide_dll() -> str | None:
    """Locate the built Valkey.Glide.dll, preferring Release over Debug."""
    for config in ("Release", "Debug"):
        path = os.path.join(
            _PROJECT_ROOT,
            "sources",
            "Valkey.Glide",
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

    tmp_path = os.path.join(tempfile.gettempdir(), f"examples_{os.getpid()}.json")

    try:
        # Step 1: Extract examples
        result = subprocess.run(
            [
                sys.executable,
                os.path.join(_SCRIPTS_DIR, "extract_examples.py"),
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
                os.path.join(_SCRIPTS_DIR, "validate_examples.py"),
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
