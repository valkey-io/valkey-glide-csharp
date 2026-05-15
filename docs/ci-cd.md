# CI/CD Test Matrices

The `.github/json_matrices/` directory contains our CI/CD matrix configurations. Our test runs are based on profiles,
where each profile defines a set of platforms, versions, and tests to be run. The JSON files list our available
configurations and options. `load_matrices.py` is used to process the profile and load the appropriate configs onto
GitHub workflow environments.

## Files

| File                  | Purpose                                                      |
| --------------------- | ------------------------------------------------------------ |
| `os-matrix.json`      | OS/runner platforms (VMs and containers)                     |
| `server-matrix.json`  | Valkey and Redis server versions                             |
| `version-matrix.json` | .NET SDK versions                                            |
| `profiles.json`       | Profiles and their settings (e.g., test filters)             |
| `load_matrices.py`    | Filters all matrices by profile, outputs to `$GITHUB_OUTPUT` |

## Profiles

Each entry in the JSON files has a `profiles` array that declares which test profiles include it:

| Profile    | When used                                           |
| ---------- | --------------------------------------------------- |
| `standard` | Standard tests run on common platform               |
| `full`     | Nightly / manual run on all platforms and all tests |

### Example

```json
{
    "os": "ubuntu",
    "runner": "ubuntu-24.04",
    "target": "x86_64-unknown-linux-gnu",
    "profiles": [ "standard", "full"]
}
```

This host runs in both profiles. A host with `"profiles": ["full"]` only runs in the full suite.
