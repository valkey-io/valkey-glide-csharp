#!/usr/bin/env python3

"""Check Valkey GLIDE C# code examples by extracting and compiling them.

Extracts example code blocks from Valkey GLIDE source files or loads them
from a JSON file, wraps each in a compilable C# harness, runs `dotnet build`,
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
    # Flexible with whitespace/newlines between tags.
    _EXAMPLE_PATTERN = re.compile(
        r"<example>\s*(?:///\s*)?<code>\s*\n"
        r"(.*?)"
        r"^\s*///\s*</code>\s*(?:///\s*)?</example>",
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

    # Namespace imports to include by default.
    _USING_NAMESPACES = [
        "Valkey.Glide",
        "Valkey.Glide.Pipeline",
        "Valkey.Glide.Commands",
        "Valkey.Glide.Commands.Options",
    ]

    # Type imports (using static) to include by default.
    _USING_TYPES = [
        "Valkey.Glide.Pipeline.Batch",
        "Valkey.Glide.Pipeline.ClusterBatch",
        "Valkey.Glide.Commands.Options.InfoOptions",
        "Valkey.Glide.Commands.Options.BitFieldOptions",
        "Valkey.Glide.Commands.Options.BitFieldOptions.Encoding",
    ]

    # Matches a C# using directive (e.g. "using Foo.Bar;" or "using static Foo.Bar;")
    # but not a using declaration (e.g. "using var x = ...").
    _USING_DIRECTIVE = re.compile(r"^\s*using\s+(static\s+)?[A-Z][\w.]*\s*;")

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
    <Reference Include="Valkey.Glide">
      <HintPath>{dll_path}</HintPath>
    </Reference>
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

    def __init__(self, examples: dict[str, str], glide_root: str):
        """Initialize the checker.

        Use as a context manager to ensure cleanup of temporary files:

            with ExampleChecker(examples, "path/to/valkey-glide-csharp") as checker:
                errors = checker.check()

        Args:
            examples: Dict mapping source references (e.g. "File.cs:42")
                to code example strings.
            glide_root: Path to the Valkey GLIDE project
        """
        self._examples = examples
        self._temp_dir = tempfile.TemporaryDirectory()
        self._glide_root = os.path.abspath(glide_root)

        # Verify that Valkey GLIDE root exists.
        if not os.path.exists(self._glide_root):
            print(
                f"Error: Project not found at '{self._glide_root}'.",
                file=sys.stderr,
            )
            sys.exit(1)

        # Verify that Valkey GLIDE DLL exists.
        self._glide_dll_path = None

        for config in ("Release", "Debug"):
            path = os.path.join(
                self._glide_root,
                "sources",
                "Valkey.Glide",
                "bin",
                config,
                "net8.0",
                "Valkey.Glide.dll",
            )
            if os.path.exists(path):
                self._glide_dll_path = path
                break

        if self._glide_dll_path is None:
            print(
                f"Error: Built DLL. "
                "Run 'dotnet build' first.",
                file=sys.stderr,
            )
            sys.exit(1)

        # Verify that dotnet is installed.
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
            print("Error: 'dotnet' is not installed or not on PATH.", file=sys.stderr)
            sys.exit(1)

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

        # Create and populate the temp project,
        # then build it and identify any example failures.
        file_to_source = self._create_project()
        return self._build_project(file_to_source)

    def _create_project(self) -> dict[str, str]:
        """Generate the .csproj and wrapper files in the temp directory.

        Returns:
            A dict mapping generated filenames to source references.
        """
        csproj_content = self._CSPROJ_TEMPLATE.format(dll_path=self._glide_dll_path)

        with open(os.path.join(self._temp_dir.name, "ExampleValidation.csproj"), "w") as f:
            f.write(csproj_content)

        file_to_source: dict[str, str] = {}
        for source, content in self._examples.items():
            filename = self._generate_wrapper(source, content)
            file_to_source[filename] = source

        return file_to_source

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

        # Extract using directives from the example and merge with defaults.
        example_usings = []
        code_lines = []
        for line in content.splitlines():
            if self._USING_DIRECTIVE.match(line):
                example_usings.append(line.rstrip().rstrip(";") + ";")
            else:
                code_lines.append(line)
        content = "\n".join(code_lines).strip()

        default_usings = [f"using {ns};" for ns in self._USING_NAMESPACES] + [
            f"using static {t};" for t in self._USING_TYPES
        ]
        all_usings = default_usings + example_usings
        usings = "\n".join(dict.fromkeys(all_usings))  # deduplicate, preserve order

        class_name = f"Example_{uuid.uuid4().hex}"
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

    def _build_project(self, file_to_source: dict[str, str]) -> dict[str, list[str]]:
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

    with ExampleChecker(examples, args.glide_root) as checker:
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
