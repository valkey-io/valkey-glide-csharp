# CI/CD Test Matrices

This directory contains our CI/CD configurations. Our test runs are based on profiles, where each profile define
a set of platforms, versions, and tests to be run. The json files list our available configurations and options.
`load_matrices.py` is used to process the profile and load the appropriate configs onto Github workflow environments.

## Files

| File | Purpose |
|------|---------|
| `os-matrix.json` | OS/runner platforms (VMs and containers) |
| `server-matrix.json` | Valkey and Redis server versions |
| `version-matrix.json` | .NET SDK versions |
| `profiles.json` | Profiles and their settings (e.g., test filters) |
| `load_matrices.py` | Filters all matrices by profile, outputs to `$GITHUB_OUTPUT` |

## Profiles

Each entry in the JSON files has a `profiles` array that declares which test profiles include it:

| Profile    | When used                                                                  |
| ---------- | -------------------------------------------------------------------------- |
| `smoke`    | For quick validation. Runs on wide platforms but only against CommandTests |
| `standard` | Standard tests run on common platform                                      |
| `full`     | Nightly / manual run on all platforms and all tests                        |

### Example

```json
{
    "OS": "ubuntu",
    "RUNNER": "ubuntu-24.04",
    "profiles": ["smoke", "standard", "full"]
}
```

This host runs in all three profiles. A host with `"profiles": ["full"]` only runs in the full suite.
