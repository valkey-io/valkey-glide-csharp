# GitHub Labels

GitHub label scheme for `valkey-io/valkey-glide-csharp`. Labels are grouped into categories for consistency and filtering.

## Issue Types

Use GitHub's built-in issue types instead of type labels. The default types are **Bug**, **Feature**, and **Task**.

## Area

Which part of the codebase or project does this affect? An issue can have multiple labels.

| Label           | Color                   | Description                                      |
| --------------- | ----------------------- | ------------------------------------------------ |
| `ci`            | `#3b1f8e` – dark purple | CI/CD pipelines and GitHub Actions               |
| `core`          | `#3b1f8e` – dark purple | Core library (`sources/Valkey.Glide/`)           |
| `docs`          | `#3b1f8e` – dark purple | Documentation and developer guides               |
| `rust`          | `#3b1f8e` – dark purple | Rust FFI layer (`rust/`)                         |
| `tests`         | `#3b1f8e` – dark purple | Unit tests, integration tests, or test utilities |

## Status

Workflow state. Applied and updated as the issue progresses. New issues are 'triage' by default.

| Label        | Color            | Description                        |
| ------------ | ---------------- | ---------------------------------- |
| `backlog`    | `#0052cc` – blue | Reviewed but not yet scheduled     |
| `release-*`  | `#0052cc` – blue | Targeted for the specified release |
| `triage`     | `#0052cc` – blue | Needs triage — not yet reviewed    |

## Special

GitHub-recognized labels and other cross-cutting concerns.

| Label              | Color              | Description                                     |
| ------------------ | ------------------ | ----------------------------------------------- |
| `breaking`         | `#0e8a16` – green  | Introduces a breaking change                    |
| `compatibility`    | `#0e8a16` – green  | StackExchange.Redis API compatibility           |
| `dependencies`     | `#0e8a16` – green  | Dependency updates (auto-applied by Dependabot) |
| `flaky-test`       | `#0e8a16` – green  | Flaky test in CI                                |
| `good first issue` | `#0e8a16` – green  | Good for new contributors                       |
| `help wanted`      | `#0e8a16` – green  | Extra attention is needed                       |
| `inquiry`          | `#0e8a16` – green  | Question about how something works              |

## Mapping to Issue Templates

| Template                                                           | Issue Type | Default Labels                          |
| ------------------------------------------------------------------ | ---------- | --------------------------------------- |
| [Bug Report](../.github/ISSUE_TEMPLATE/bug-report.yml)             | Bug        | `triage`                                |
| [Feature Request](../.github/ISSUE_TEMPLATE/feature-request.yml)   | Feature    | `triage`                                |
| [Flaky CI Test](../.github/ISSUE_TEMPLATE/flaky-ci-test-issue.yml) | Bug        | `ci`, `tests`, `triage`, `flaky-test`   |
| [Inquiry](../.github/ISSUE_TEMPLATE/inquiry.yml)                   | Task       | `triage`, `inquiry`                     |
| [Task](../.github/ISSUE_TEMPLATE/task.yml)                         | Task       | `triage`                                |
