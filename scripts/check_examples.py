#!/usr/bin/env python3

"""Check Valkey GLIDE C# code examples by extracting and compiling them.

Extracts example code blocks from Valkey GLIDE source files (or loads them
from a JSON file), wraps each in a compilable C# harness, runs `dotnet build`,
and reports failures mapped back to their source.

If provided, the examples JSON file should map from a source key to the
associated code example:

```json
{
    "sources/File.cs:123": "var client = new ValkeyClient(...);",
    "sources/File.cs:456": "await client.SetAsync(\"key\", \"value\");"
}
```

Usage:
    python scripts/check_examples.py
      --glide_root path/to/valkey-glide-csharp
      [--examples path/to/examples.json]
"""

import argparse
import html
import json
import os
import re
import subprocess
import sys
import tempfile
import textwrap
import uuid


class ExampleExtractor:
    """Extracts C# code examples from XML doc comments in .cs files."""

    # Matches an <example><code>...</code></example> block in XML doc comments.
    # Captures the code content between <code> and </code> tags.
    _EXAMPLE_PATTERN = re.compile(
        r"^\s*///\s*<example>\s*\n"
        r"\s*///\s*<code>\s*\n"
        r"(.*?)"
        r"^\s*///\s*</code>\s*\n"
        r"\s*///\s*</example>",
        re.MULTILINE | re.DOTALL,
    )

    # Matches the /// prefix on a doc comment line (strips at most one space, not newlines).
    _DOC_PREFIX = re.compile(r"^\s*/// ?", re.MULTILINE)

    def __init__(self, glide_root: str):
        """Initialize the extractor.

        Args:
            glide_root: Root directory of the valkey-glide-csharp repository.
        """
        self._source_dir = os.path.join(glide_root, "sources")

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
            # Line number of the <example> tag (1-indexed).
            line_number = content[: match.start()].count("\n") + 1

            # Strip /// prefix from each captured code line.
            code = self._DOC_PREFIX.sub("", match.group(1)).strip()

            source = f"{filepath}:{line_number}"
            results[source] = code

        return results


class ExampleChecker:
    """Checks C# code examples by compiling them in a temporary .NET project."""

    # Imports to include in every generated wrapper file.
    _USINGS = [
        "System",
        "System.Collections.Generic",
        "System.Linq",
        "System.Threading.Tasks",
        "Valkey.Glide",
    ]

    # Template for the .csproj that references the main library.
    _CSPROJ_TEMPLATE = """\
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
    <NoWarn>
        CS0219  // Variable assigned but never used
    </NoWarn>
  </PropertyGroup>
  <ItemGroup>
    <ProjectReference Include="{project_ref}" />
  </ItemGroup>
</Project>
"""

    # Template for each generated .cs wrapper file:
    #   - Declare static client, database, and server instances.
    #   - Async method to allow 'await' in examples.
    _WRAPPER_TEMPLATE = """\
// Auto-generated from {source}

{usings}

namespace Valkey.Glide.ExampleValidation;

public class {class_name}
{{
    // Valkey GLIDE
    static GlideClient client = null!;
    static GlideClusterClient clusterClient = null!;

    // StackExchange.Redis
    static IDatabase db = null!;
    static IServer server = null!;

    static async Task Run()
    {{
{indented_code}
    }}
}}
"""

    # Regex for matching dotnet build error lines.
    # See: https://learn.microsoft.com/en-us/cpp/build/formatting-the-output-of-a-custom-build-step-or-build-event
    # Format: {filename}({line},{column}): error {code}: {message} [{project}]
    _ERROR_PATTERN = re.compile(
        r"(?P<file>[^(]+)\(\d+,\d+\):\s*error\s+(?P<message>CS\d+:\s*[^[]*)"
    )

    def __init__(self, examples: dict[str, str], project_path: str):
        """Initialize the checker.

        Use as a context manager to ensure cleanup of temporary files:

            with ExampleChecker(examples, "path/to/Project.csproj") as checker:
                errors = checker.check()

        Args:
            examples: Dict mapping source references (e.g. "File.cs:42")
                to code example strings.
            project_path: Path to the .csproj to compile examples against.
        """
        self._examples = examples
        self._csproj_path = os.path.abspath(project_path)
        self._temp_dir = tempfile.TemporaryDirectory()

        self._check_prerequisites()

    def __enter__(self):
        return self

    def __exit__(self, *_):
        self._temp_dir.cleanup()

    def check(self) -> dict[str, list[str]]:
        """Check the examples by compiling them.

        Returns:
            A dict mapping source references to lists of error messages for
            examples that failed compilation. An empty dict means all examples
            compiled successfully.
        """
        if not self._examples:
            return {}

        self._write_csproj()

        # Generate wrapper files and track filename -> source.
        file_to_source: dict[str, str] = {}
        for source, content in self._examples.items():
            filename = self._generate_wrapper(source, content)
            file_to_source[filename] = source

        return self._build(file_to_source)

    def _check_prerequisites(self):
        """Verify that dotnet is installed and the project is available.

        Raises:
            SystemExit: If prerequisites are not met.
        """
        try:
            result = subprocess.run(
                ["dotnet", "--version"],
                capture_output=True,
                text=True,
            )
            if result.returncode != 0:
                print(
                    "Error: 'dotnet' is installed but returned an error.",
                    file=sys.stderr,
                )
                sys.exit(1)
        except FileNotFoundError:
            print(
                "Error: 'dotnet' is not installed or not on PATH.", file=sys.stderr
            )
            sys.exit(1)

        if not os.path.exists(self._csproj_path):
            print(
                f"Error: Project not found at '{self._csproj_path}'.",
                file=sys.stderr,
            )
            sys.exit(1)

    def _write_csproj(self):
        """Generate the .csproj file in the working directory."""
        project_ref = os.path.relpath(self._csproj_path, self._temp_dir.name)
        content = self._CSPROJ_TEMPLATE.format(project_ref=project_ref)

        with open(os.path.join(self._temp_dir.name, "ExampleValidation.csproj"), "w") as f:
            f.write(content)

    def _generate_wrapper(self, source: str, content: str) -> str:
        """Generate and write a .cs wrapper file for a single example.

        Args:
            source: Reference to the example source (e.g. "File.cs:42").
            content: The raw code example to wrap.

        Returns:
            The filename (basename) of the generated file.
        """
        # Docstrings may contain character references like &gt;
        # that need to be mapped to the corresponding character.
        content = html.unescape(content)

        class_name = f"Example_{uuid.uuid4().hex}"
        usings = "\n".join(f"using {ns};" for ns in self._USINGS)
        indented_code = textwrap.indent(content, "        ")

        file_content = self._WRAPPER_TEMPLATE.format(
            source=source,
            usings=usings,
            class_name=class_name,
            indented_code=indented_code,
        )

        filename = f"{class_name}.cs"
        filepath = os.path.join(self._temp_dir.name, filename)
        with open(filepath, "w") as f:
            f.write(file_content)

        return filename

    def _build(self, file_to_source: dict[str, str]) -> dict[str, list[str]]:
        """Run dotnet build and return errors mapped by source.

        Args:
            file_to_source: Mapping from generated filenames to source references.

        Returns:
            A dict mapping source references to lists of error messages.
        """
        result = subprocess.run(
            ["dotnet", "build", "--framework", "net8.0"],
            cwd=self._temp_dir.name,
            capture_output=True,
            text=True,
        )

        if result.returncode == 0:
            return {}

        errors: dict[str, list[str]] = {}
        for line in result.stdout.splitlines():
            match = self._ERROR_PATTERN.search(line)
            if match:
                basename = os.path.basename(match.group("file"))
                source = file_to_source.get(basename, basename)
                errors.setdefault(source, []).append(match.group("message").strip())

        return errors


def main():
    parser = argparse.ArgumentParser(
        description="Check C# code examples by extracting and compiling them."
    )
    parser.add_argument(
        "--glide_root",
        required=True,
        help="Root directory of the valkey-glide-csharp repository.",
    )
    parser.add_argument(
        "--examples",
        default=None,
        help="Path to a JSON file containing examples. "
        "If not provided, examples are extracted from source files.",
    )
    args = parser.parse_args()

    # Load or extract examples.
    if args.examples:
        if not os.path.isfile(args.examples):
            print(f"Error: '{args.examples}' not found", file=sys.stderr)
            sys.exit(1)

        with open(args.examples, "r") as f:
            examples = json.load(f)
    else:
        extractor = ExampleExtractor(args.glide_root)
        examples = extractor.extract()

    project = os.path.join(
        args.glide_root, "sources", "Valkey.Glide", "Valkey.Glide.csproj"
    )

    with ExampleChecker(examples, project) as checker:
        print(f"Checking {len(examples)} examples...")
        errors = checker.check()

    if errors:
        print(f"\n{'='*60}")
        print(f"FAILURES ({len(errors)} of {len(examples)} examples)")
        print(f"{'='*60}\n")

        for source, messages in errors.items():
            print(f"  FAIL: {source}")
            for message in messages:
                print(f"        {message}")
            print()

        print(f"{len(examples) - len(errors)} passed, {len(errors)} failed")
        sys.exit(1)
    else:
        print(f"\nAll {len(examples)} examples compiled successfully.")
        sys.exit(0)


if __name__ == "__main__":
    main()
