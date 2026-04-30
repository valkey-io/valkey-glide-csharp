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

VALID_PROFILES = ("smoke", "standard", "full")


def load_json(filename: str) -> list | dict:
    return json.loads(Path(filename).read_text())


def filter_by_profile(entries: list, profile: str) -> list:
    return [e for e in entries if profile in e.get("profiles", [])]


def main() -> None:
    if len(sys.argv) != 2 or sys.argv[1] not in VALID_PROFILES:
        print(f"Usage: {sys.argv[0]} <{'|'.join(VALID_PROFILES)}>", file=sys.stderr)
        sys.exit(1)

    profile = sys.argv[1]

    profile_settings = load_json("profiles.json")[profile]

    os_matrix = filter_by_profile(load_json("os-matrix.json"), profile)
    host = [h for h in os_matrix if "IMAGE" not in h]
    container_host = [h for h in os_matrix if "IMAGE" in h]

    server = filter_by_profile(load_json("server-matrix.json"), profile)
    
    dotnet_entries = filter_by_profile(load_json("version-matrix.json"), profile)
    dotnet = [e["version"] for e in dotnet_entries]

    test_filter = profile_settings.get("test-filter", "")

    # Validate
    if profile == "smoke" and len(server) != 1:
        print(f"ERROR: Expected exactly 1 smoke server, found {len(server)}", file=sys.stderr)
        sys.exit(1)
    for name, value in [("HOST_MATRIX", host), ("SERVER_MATRIX", server), ("DOTNET_MATRIX", dotnet)]:
        if not value:
            print(f"ERROR: {name} is empty for profile={profile}", file=sys.stderr)
            sys.exit(1)

    # Summary
    print(f"Profile: {profile}")
    print(f"VM hosts: {len(host)}")
    print(f"Container hosts: {len(container_host)}")
    print(f"Servers: {len(server)}")
    print(f"Dotnet versions: {len(dotnet)}")
    print(f"Test filter: {test_filter or '(none)'}")

    # Write to GITHUB_OUTPUT
    github_output = os.environ.get("GITHUB_OUTPUT")
    if github_output:
        with open(github_output, "a") as f:
            f.write(f"host-matrix={json.dumps(host, separators=(',', ':'))}\n")
            f.write(f"container-host-matrix={json.dumps(container_host, separators=(',', ':'))}\n")
            f.write(f"server-matrix={json.dumps(server, separators=(',', ':'))}\n")
            f.write(f"dotnet-matrix={json.dumps(dotnet, separators=(',', ':'))}\n")
            f.write(f"test-filter={test_filter}\n")
    else:
        print(f"host-matrix={json.dumps(host, indent=2)}")
        print(f"container-host-matrix={json.dumps(container_host, indent=2)}")
        print(f"server-matrix={json.dumps(server, indent=2)}")
        print(f"dotnet-matrix={json.dumps(dotnet, indent=2)}")
        print(f"test-filter={test_filter}")


if __name__ == "__main__":
    main()
