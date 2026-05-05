#!/usr/bin/env python3

"""Validate C# code examples by compiling them in a temporary .NET project.

Loads examples from a JSON file, wraps each in a C# class with a .NET project,
runs `dotnet build`, and reports failures mapped back to their source.

The examples JSON file must map from a source key to the associated code example:

```json
{
    "sources/File.cs:123": "var client = new ValkeyClient(...);",
    "sources/File.cs:456": "await client.SetAsync(\"key\", \"value\");"
}
```

Usage:
    python scripts/validate_examples.py
        --examples path/to/examples.json
        --glide-dll path/to/Valkey.Glide.dll
        [--add-imports]
        [--add-clients]

Options:
    --examples      Path to an existing JSON file containing examples to validate.
    --glide-dll     Path to the built Valkey.Glide.dll to reference during compilation.
    --add-imports   Include default using directives (Valkey.Glide namespaces) in
                    the generated classes. Disabled by default.
    --add-clients   Include static client fields (GlideClient, GlideClusterClient,
                    IDatabase, IServer) in the generated classes. Disabled by default.
"""

import argparse
import json
import os
import re
import subprocess
import sys
import tempfile
import textwrap
import uuid


class ExamplesValidator:
    """Validates C# code examples by compiling them in a temporary .NET project."""

    # Namespace imports to include when --add-imports is specified.
    _USING_NAMESPACES = [
        "Valkey.Glide",
        "Valkey.Glide.Pipeline",
        "Valkey.Glide.Commands",
        "Valkey.Glide.Commands.Options",
        "Valkey.Glide.ServerModules",
    ]

    # Type imports (using static) to include when --add-imports is specified.
    _USING_TYPES = [
        "Valkey.Glide.Commands.Options.BitFieldOptions.Encoding",
        "Valkey.Glide.Commands.Options.BitFieldOptions",
        "Valkey.Glide.Commands.Options.InfoOptions",
        "Valkey.Glide.Pipeline.Batch",
        "Valkey.Glide.Pipeline.ClusterBatch",
        "Valkey.Glide.Pipeline.Options",
    ]

    # Client fields to include when --add-clients is specified.
    _CLIENT_FIELDS = [
        "static Valkey.Glide.GlideClient client = null!;",
        "static Valkey.Glide.GlideClusterClient clusterClient = null!;",
        "static Valkey.Glide.IDatabase db = null!;",
        "static Valkey.Glide.IServer server = null!;",
    ]

    # Matches a C# using directive (e.g. "using Foo.Bar;" or "using static Foo.Bar;")
    # but not a using declaration (e.g. "using var x = ...").
    _USING_DIRECTIVE = re.compile(r"^\s*using\s+(static\s+)?[A-Z][\w.]*\s*;")

    # Template for the .csproj that references the main library.
    _PROJECT_TEMPLATE = """\
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>
  <ItemGroup>
    <Reference Include="Valkey.Glide">
      <HintPath>{dll_path}</HintPath>
    </Reference>
  </ItemGroup>
</Project>
"""

    # Template for each generated C# class.
    _CLASS_TEMPLATE = """\
// Auto-generated from {source}

{usings}

namespace ExampleValidation;

public class {class_name}
{{
{fields}

    static async Task Run()
    {{
{indented_code}
    }}
}}
"""

    # Regex for matching dotnet build error lines.
    _ERROR_PATTERN = re.compile(
        r"(?P<file>[^(]+)\(\d+,\d+\):\s*error\s+(?P<message>CS\d+:\s*[^[]*)"
    )

    def __init__(
        self,
        *,
        examples: dict[str, str],
        glide_dll: str,
        add_imports: bool = False,
        add_clients: bool = False,
    ):
        """Initialize the validator.

        Use as a context manager to ensure cleanup of temporary files:

            with ExamplesValidator(examples=examples, glide_dll=path) as validator:
                errors = validator.validate()

        Args:
            examples: Dict mapping source references to code example strings.
            glide_dll: Path to the built Valkey.Glide.dll to reference during compilation.
            add_imports: If True, include default using directives in the generated class.
            add_clients: If True, include static client fields in the generated class.
        """
        self._examples: dict[str, str] = examples
        self._add_imports: bool = add_imports
        self._add_clients: bool = add_clients

        self._temp_dir = tempfile.TemporaryDirectory()
        self._glide_dll_path = os.path.abspath(glide_dll)

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

    def validate(self) -> dict[str, list[str]]:
        """Validate the examples by compiling them.

        Returns:
            A dict mapping source references to lists of error messages.
            An empty dict means all examples were successfully validated.
        """
        if not self._examples:
            return {}

        self._file_to_source: dict[str, str] = {}
        self._source_to_errors: dict[str, list[str]] = {}

        self._create_project()
        self._build_project()

        return self._source_to_errors

    def _create_project(self) -> None:
        """Generate the .csproj and wrapper files in the temp directory."""
        csproj_content = self._PROJECT_TEMPLATE.format(dll_path=self._glide_dll_path)

        with open(os.path.join(self._temp_dir.name, "ExampleValidation.csproj"), "w") as f:
            f.write(csproj_content)

        for source, content in self._examples.items():
            self._generate_class(source, content)

    def _generate_class(self, source: str, content: str) -> None:
        """Generate and write a C# class for a single example."""

        using_directives = []

        # Extract 'using' directives from code.
        code_lines = []
        for line in content.splitlines():
            if self._USING_DIRECTIVE.match(line):
                using_directives.append(line)
            else:
                code_lines.append(line)

        # Add default 'using' directives.
        if self._add_imports:
            using_directives += [f"using {ns};" for ns in self._USING_NAMESPACES]
            using_directives += [f"using static {t};" for t in self._USING_TYPES]

        # Detect duplicate using directives.
        using_directives_set: set[str] = set()
        for u in using_directives:
            normalized = u.strip()
            if normalized in using_directives_set:
                self._source_to_errors.setdefault(source, []).append(
                    f"Duplicate import: {normalized}"
                )
            else:
                using_directives_set.add(normalized)

        # Get fields to include
        fields = []
        if self._add_clients:
            fields += self._CLIENT_FIELDS

        class_name = f"Example_{uuid.uuid4().hex}"
        usings_str = "\n".join(using_directives_set)
        fields_str = "\n".join(f"    {f}" for f in fields)
        indented_code = textwrap.indent("\n".join(code_lines).strip(), "        ")

        file_content = self._CLASS_TEMPLATE.format(
            class_name=class_name,
            source=source,
            usings=usings_str,
            fields=fields_str + "\n" if fields_str else "",
            indented_code=indented_code,
        )

        filename = f"{class_name}.cs"
        filepath = os.path.join(self._temp_dir.name, filename)
        with open(filepath, "w") as f:
            f.write(file_content)

        self._file_to_source[filename] = source

    def _build_project(self) -> None:
        """Run dotnet build and record compilation errors."""
        result = subprocess.run(
            ["dotnet", "build", "--framework", "net8.0"],
            cwd=self._temp_dir.name,
            capture_output=False,
            stdout=subprocess.PIPE,
            stderr=subprocess.STDOUT,
            text=True,
        )

        # Build succeeded:
        if result.returncode == 0:
            return

        # Build failed with matching errors:
        found_errors = False
        for line in result.stdout.splitlines():
            match = self._ERROR_PATTERN.search(line)
            if match:
                found_errors = True
                basename = os.path.basename(match.group("file"))
                source = self._file_to_source.get(basename, basename)
                self._source_to_errors.setdefault(source, []).append(
                    match.group("message").strip()
                )

        if found_errors:
            return

        # Build failed without any matching errors:
        fallback = [f"dotnet build failed with exit code {result.returncode}."]
        raw = result.stdout.strip()
        if raw:
            fallback.append("Raw build output:")
            fallback.extend(raw.splitlines())
        self._source_to_errors.setdefault("<build>", []).extend(fallback)


def main():
    parser = argparse.ArgumentParser(
        description="Validate C# code examples by compiling them."
    )
    parser.add_argument(
        "--examples",
        required=True,
        help="Path to an existing JSON file containing examples to validate.",
    )
    parser.add_argument(
        "--glide-dll",
        required=True,
        help="Path to the built Valkey.Glide.dll to reference during compilation.",
    )
    parser.add_argument(
        "--add-imports",
        action="store_true",
        default=False,
        help="Include default using directives in the generated classes.",
    )
    parser.add_argument(
        "--add-clients",
        action="store_true",
        default=False,
        help="Include static client fields in the generated classes.",
    )
    args = parser.parse_args()

    if not os.path.isfile(args.examples):
        print(f"Error: '{args.examples}' not found", file=sys.stderr)
        sys.exit(1)

    if not os.path.isfile(args.glide_dll):
        print(f"Error: '{args.glide_dll}' not found", file=sys.stderr)
        sys.exit(1)

    with open(args.examples, "r") as f:
        examples = json.load(f)

    if not examples:
        print("No examples found in the provided file.")
        sys.exit(0)

    with ExamplesValidator(
        examples=examples,
        glide_dll=args.glide_dll,
        add_imports=args.add_imports,
        add_clients=args.add_clients,
    ) as validator:
        print(f"Validating {len(examples)} examples...")
        errors = validator.validate()

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
