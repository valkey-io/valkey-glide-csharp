# CI/CD Test Matrices

This directory contains JSON configuration files that define **what** gets tested and a Python script that filters them by profile.

## Files

| File | Purpose |
|------|---------|
| `os-matrix.json` | OS/runner platforms (VMs and containers) |
| `server-matrix.json` | Valkey and Redis server versions |
| `version-matrix.json` | .NET SDK versions |
| `profiles.json` | Per-profile settings (e.g., test filters) |
| `load_matrices.py` | Filters all matrices by profile, outputs to `$GITHUB_OUTPUT` |

## Profiles

Each entry in the JSON files has a `profiles` array that declares which test profiles include it:


| Profile    | When used                                                                  |
| ---------- | -------------------------------------------------------------------------- |
| `smoke`    | For quick validation. Runs on wide platforms but only against CommandTests |
| `standard` | Standard tests run on common platform                                      |
| `full`     | Nightly / manual run on all platforms and all tests                        |

Per-profile settings like test filters are defined in `profiles.json`.

### Example

```json
{
    "OS": "ubuntu",
    "RUNNER": "ubuntu-24.04",
    "profiles": ["smoke", "standard", "full"]
}
```

This host runs in all three profiles. A host with `"profiles": ["full"]` only runs in the full suite.

## Adding a new platform or server

1. Add an entry to the appropriate JSON file.
2. Set `profiles` to the profiles it should run in.
3. For OS entries, set `tags` for CD pipeline inclusion (see below).
4. No code changes needed — the Python script filters generically.

## How it works

The reusable workflow `_reusable-test.yml` calls the `load_matrices.py` script for a specified profile, which generates JSON arrays representing the corresponding host, server, .NET version, and test filter.s

### Local testing

```bash
cd .github/json_matrices
python3 load_matrices.py smoke
python3 load_matrices.py standard
python3 load_matrices.py full
```

Without `$GITHUB_OUTPUT` set, the script prints results to stdout.

## Tags (CD pipeline)
TODO: To be removed as part of https://github.com/valkey-io/valkey-glide-csharp/issues/345

The `tags` field in `os-matrix.json` is separate from profiles. It controls which platforms the CD pipeline (`cd.yml`) builds binaries for:

- `"ci"` — included in CI test runs
- `"cd"` — included in CD binary builds
- `"container"` — container-based host

A host can have tags but empty profiles (e.g., Windows ARM is tagged `"cd"` for binary builds but has no test profiles since it can't run the server).
