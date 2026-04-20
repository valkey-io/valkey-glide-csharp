# GitHub Labels

GitHub label scheme for `valkey-io/valkey-glide-csharp`. Labels use a `category/value` prefix convention for consistency and filtering.

## Type

What kind of work is this? Every issue should have exactly one `type/` label.

| Label          | Color                | Description                                          |
| -------------- | -------------------- | ---------------------------------------------------- |
| `type/bug`     | `#7057ff` – purple   | Something isn't working                              |
| `type/feature` | `#7057ff` – purple   | New feature or enhancement                           |
| `type/task`    | `#7057ff` – purple   | Implementation work, cleanup, refactoring, or chores |

## Area

Which part of the codebase or project does this affect? An issue can have multiple `area/` labels.

| Label                | Color                     | Description                                      |
| -------------------- | ------------------------- | ------------------------------------------------ |
| `area/core`          | `#d4c5f9` – light purple  | Core library (`sources/Valkey.Glide/`)           |
| `area/tests`         | `#d4c5f9` – light purple  | Unit tests, integration tests, or test utilities |
| `area/ci`            | `#d4c5f9` – light purple  | CI/CD pipelines and GitHub Actions               |
| `area/docs`          | `#d4c5f9` – light purple  | Documentation and developer guides               |
| `area/rust`          | `#d4c5f9` – light purple  | Rust FFI layer (`rust/`)                         |
| `area/compatibility` | `#d4c5f9` – light purple  | StackExchange.Redis API compatibility            |

## Status

Workflow state. Applied and updated as the issue progresses. New issues are 'triage' by default.

| Label              | Color            | Description                                     |
| ------------------ | ---------------- | ----------------------------------------------- |
| `status/triage`    | `#0052cc` – blue | Needs triage — not yet reviewed or prioritized  |
| `status/backlog`   | `#0052cc` – blue | Reviewed and accepted, but not yet scheduled    |
| `status/release-*` | `#0052cc` – blue | Targeted for the specified release              |

## Priority

How urgent is this? Applied during triage.

| Label               | Color                | Description                  |
| ------------------- | -------------------- | ---------------------------- |
| `priority/critical` | `#b60205` – red      | Must be resolved immediately |
| `priority/high`     | `#d93f0b` – orange   | Should be resolved soon      |
| `priority/medium`   | `#fbca04` – yellow   | Normal priority              |
| `priority/low`      | `#0e8a16` – green    | Nice to have, no urgency     |

## Special

GitHub-recognized labels and other cross-cutting concerns.

| Label              | Color              | Description                                     |
| ------------------ | ------------------ | ----------------------------------------------- |
| `good first issue` | `#ededed` – grey   | Good for new contributors                       |
| `help wanted`      | `#ededed` – grey   | Extra attention is needed                       |
| `breaking`         | `#ededed` – grey   | Introduces a breaking change                    |
| `dependencies`     | `#ededed` – grey   | Dependency updates (auto-applied by Dependabot) |
| `flaky-test`       | `#ededed` – grey   | Flaky test in CI                                |

## Mapping to Issue Templates

| Template                                                           | Default Labels                                                     |
| ------------------------------------------------------------------ | ------------------------------------------------------------------ |
| [Bug Report](../.github/ISSUE_TEMPLATE/bug-report.yml)             | `type/bug`, `status/triage`                                        |
| [Feature Request](../.github/ISSUE_TEMPLATE/feature-request.yml)   | `type/feature`, `status/triage`                                    |
| [Task](../.github/ISSUE_TEMPLATE/task.yml)                         | `type/task`, `status/triage`                                       |
| [Flaky CI Test](../.github/ISSUE_TEMPLATE/flaky-ci-test-issue.yml) | `type/bug`, `area/ci`, `area/tests`, `status/triage`, `flaky-test` |
