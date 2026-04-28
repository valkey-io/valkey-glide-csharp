#!/usr/bin/env python3

"""Extract C# code examples from Valkey GLIDE source files.

Walks the sources/ directory, finds <example><code>...</code></example> blocks
in XML doc comments, and writes them to a JSON file mapping source references
to code snippets.

Usage:
    python scripts/extract_examples.py --examples path/to/output.json
"""

import argparse
import json
import os
import re
import sys


# Project root is one level up from this script's directory (scripts/).
_PROJECT_ROOT = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))


class ExamplesExtractor:
    """Extracts C# code examples from XML doc comments in .cs files."""

    # Matches an <example><code>...</code></example> block in XML doc comments.
    _EXAMPLE_PATTERN = re.compile(
        r"<example>\s*(?:///\s*)?<code>\s*\n"
        r"(.*?)"
        r"^\s*///\s*</code>\s*(?:///\s*)?</example>",
        re.MULTILINE | re.DOTALL,
    )

    # Matches the /// prefix on a doc comment line.
    _DOC_PREFIX = re.compile(r"^\s*/// ?", re.MULTILINE)

    def __init__(self):
        """Initialize the extractor using the project root."""
        self._source_dir = os.path.join(_PROJECT_ROOT, "sources")

        if not os.path.isdir(self._source_dir):
            print(
                f"Error: '{self._source_dir}' is not a directory", file=sys.stderr
            )
            sys.exit(1)

    def extract(self) -> dict[str, str]:
        """Extract all <example> code blocks from .cs files.

        Returns:
            A dict mapping source references (e.g. "path/to/File.cs:42")
            to the raw code lines from the example block.
        """
        results: dict[str, str] = {}

        for dirpath, _, filenames in os.walk(self._source_dir):
            for filename in sorted(filenames):
                if not filename.endswith(".cs"):
                    continue

                filepath = os.path.join(dirpath, filename)
                results.update(self._extract_from_file(filepath))

        return results

    def _extract_from_file(self, filepath: str) -> dict[str, str]:
        """Extract example blocks from a single .cs file."""
        results: dict[str, str] = {}

        with open(filepath, "r", encoding="utf-8-sig") as f:
            content = f.read()

        for match in self._EXAMPLE_PATTERN.finditer(content):
            line_number = content[: match.start()].count("\n") + 1
            code = self._DOC_PREFIX.sub("", match.group(1)).strip()
            source = f"{filepath}:{line_number}"
            results[source] = code

        return results


def main():
    parser = argparse.ArgumentParser(
        description="Extract C# code examples from Valkey GLIDE source files."
    )
    parser.add_argument(
        "--examples",
        required=True,
        help="Path to write the extracted examples JSON file.",
    )
    args = parser.parse_args()

    extractor = ExamplesExtractor()
    examples = extractor.extract()

    print(f"Extracted {len(examples)} examples from sources/")

    os.makedirs(os.path.dirname(os.path.abspath(args.examples)), exist_ok=True)

    with open(args.examples, "w") as f:
        json.dump(examples, f, indent=2)

    print(f"Written to {args.examples}")


if __name__ == "__main__":
    main()
