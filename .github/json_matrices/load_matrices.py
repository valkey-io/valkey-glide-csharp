#!/usr/bin/env python3
"""
Load and filter CI test matrices based on a profile (smoke, standard, full).

Outputs JSON arrays to $GITHUB_OUTPUT for use in GitHub Actions matrix strategies.

To add a new or update a run configuration, change json_matrices with the appropriate profiles.
"""

import json
import os
import sys
from pathlib import Path

def load_json(filename: str) -> list | dict:
    return json.loads(Path(filename).read_text())


PROFILES = load_json("profiles.json")


def filter_by_profile(entries: list, profile: str) -> list:
    return [e for e in entries if profile in e.get("profiles", [])]


def main() -> None:
    if len(sys.argv) != 2 or sys.argv[1] not in PROFILES:
        print(f"Usage: {sys.argv[0]} <{'|'.join(PROFILES)}>", file=sys.stderr)
        sys.exit(1)

    profile = sys.argv[1]

    profile_settings = PROFILES[profile]

    os_matrix = filter_by_profile(load_json("os-matrix.json"), profile)
    host = [h for h in os_matrix if "IMAGE" not in h]
    container_host = [h for h in os_matrix if "IMAGE" in h]

    server = filter_by_profile(load_json("server-matrix.json"), profile)
    assert server, "Given profile resulted in empty server matrix."
    
    dotnet_entries = filter_by_profile(load_json("version-matrix.json"), profile)
    dotnet = [e["version"] for e in dotnet_entries]
    assert dotnet, "Given profile resulted in empty dotnet version matrix."

    test_filter = "|".join(profile_settings.get("test-filter", []))

    configs = {
        "host-matrix": json.dumps(host),
        "container-host-matrix": json.dumps(container_host),
        "server-matrix": json.dumps(server),
        "dotnet-matrix": json.dumps(dotnet),
        "test-filter": test_filter,
    }

    # Write to GITHUB_OUTPUT
    print(f"Loaded matrices for profile '{profile}':")
    github_output = os.environ.get("GITHUB_OUTPUT")
    if github_output:
        with open(github_output, "a") as f:
            for key, value in configs.items():
               print(f"{key}={value}", file=f)
    else:
        for key, value in configs.items():
            print(f"{key}={value}")


if __name__ == "__main__":
    main()
