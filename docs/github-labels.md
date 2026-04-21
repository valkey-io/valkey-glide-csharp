# GitHub Labels

GitHub label scheme for `valkey-io/valkey-glide-csharp`. Labels are grouped into categories for consistency and filtering.

## Issue Types

Use GitHub's built-in issue types instead of type labels. The default types are **Bug**, **Feature**, and **Task**.
Epics use the Task type with the `epic` special label.

## Area

Which part of the codebase or project does this affect? An issue can have multiple labels.

| Label           | Color                   | Description                                      |
| --------------- | ----------------------- | ------------------------------------------------ |
| `core`          | `#3b1f8e` – dark purple | Core library (`sources/Valkey.Glide/`)           |
| `tests`         | `#3b1f8e` – dark purple | Unit tests, integration tests, or test utilities |
| `ci`            | `#3b1f8e` – dark purple | CI/CD pipelines and GitHub Actions               |
| `docs`          | `#3b1f8e` – dark purple | Documentation and developer guides               |
| `rust`          | `#3b1f8e` – dark purple | Rust FFI layer (`rust/`)                         |
| `compatibility` | `#3b1f8e` – dark purple | StackExchange.Redis API compatibility            |

## Status

Workflow state. Applied and updated as the issue progresses. New issues are 'triage' by default.

| Label        | Color            | Description                        |
| ------------ | ---------------- | ---------------------------------- |
| `triage`     | `#0052cc` – blue | Needs triage — not yet reviewed    |
| `backlog`    | `#0052cc` – blue | Reviewed but not yet scheduled     |
| `release-*`  | `#0052cc` – blue | Targeted for the specified release |

## Special

GitHub-recognized labels and other cross-cutting concerns.

| Label              | Color            | Description                                     |
| ------------------ | ---------------- | ----------------------------------------------- |
| `epic`             | `#ededed` – grey | Large body of work spanning multiple issues     |
| `good first issue` | `#ededed` – grey | Good for new contributors                       |
| `help wanted`      | `#ededed` – grey | Extra attention is needed                       |
| `breaking`         | `#ededed` – grey | Introduces a breaking change                    |
| `dependencies`     | `#ededed` – grey | Dependency updates (auto-applied by Dependabot) |
| `flaky-test`       | `#ededed` – grey | Flaky test in CI                                |

## Mapping to Issue Templates

| Template                                                           | Issue Type | Default Labels                               |
| ------------------------------------------------------------------ | ---------- | -------------------------------------------- |
| [Bug Report](../.github/ISSUE_TEMPLATE/bug-report.yml)             | Bug        | `status/triage`                              |
| [Feature Request](../.github/ISSUE_TEMPLATE/feature-request.yml)   | Feature    | `status/triage`                              |
| [Task](../.github/ISSUE_TEMPLATE/task.yml)                         | Task       | `status/triage`                              |
| [Inquiry](../.github/ISSUE_TEMPLATE/inquiry.yml)                   | Task       | `status/triage`                              |
| [Flaky CI Test](../.github/ISSUE_TEMPLATE/flaky-ci-test-issue.yml) | Bug        | `ci`, `tests`, `status/triage`, `flaky-test` |
