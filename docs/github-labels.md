# GitHub Labels

GitHub label scheme for `valkey-io/valkey-glide-csharp`. Labels are grouped into categories for consistency and filtering.

## Type

What kind of work is this? Every issue should have exactly one type label.

| Label     | Color                | Description                                          |
| --------- | -------------------- | ---------------------------------------------------- |
| `epic`    | `#3b1f8e` – purple   | Large body of work spanning multiple issues          |
| `bug`     | `#7057ff` – purple   | Something isn't working                              |
| `feature` | `#7057ff` – purple   | New feature or enhancement                           |
| `task`    | `#7057ff` – purple   | Implementation work, cleanup, refactoring, or chores |
| `inquiry` | `#7057ff` – purple   | Question about how something works                   |

## Area

Which part of the codebase or project does this affect? An issue can have multiple labels.

| Label           | Color                     | Description                                      |
| --------------- | ------------------------- | ------------------------------------------------ |
| `core`          | `#d4c5f9` – light purple  | Core library (`sources/Valkey.Glide/`)           |
| `tests`         | `#d4c5f9` – light purple  | Unit tests, integration tests, or test utilities |
| `ci`            | `#d4c5f9` – light purple  | CI/CD pipelines and GitHub Actions               |
| `docs`          | `#d4c5f9` – light purple  | Documentation and developer guides               |
| `rust`          | `#d4c5f9` – light purple  | Rust FFI layer (`rust/`)                         |
| `compatibility` | `#d4c5f9` – light purple  | StackExchange.Redis API compatibility            |

## Status

Workflow state. Applied and updated as the issue progresses. New issues are 'triage' by default.

| Label              | Color            | Description                         |
| ------------------ | ---------------- | ----------------------------------- |
| `status/triage`    | `#0052cc` – blue | Needs triage — not yet reviewed     |
| `status/backlog`   | `#0052cc` – blue | Reviewed but not yet scheduled      |
| `status/release-*` | `#0052cc` – blue | Targeted for the specified release  |

## Special

GitHub-recognized labels and other cross-cutting concerns.

| Label              | Color              | Description                                     |
| ------------------ | ------------------ | ----------------------------------------------- |
| `good first issue` | `#ededed` – grey   | Good for new contributors                       |
| `help wanted`      | `#ededed` – grey   | Extra attention is needed                       |
| `breaking`         | `#ededed` – grey   | Introduces a breaking change                    |
| `compatibility`    | `#ededed` – grey   | Related to StackExchange.Redis compatibility    |
| `dependencies`     | `#ededed` – grey   | Dependency updates (auto-applied by Dependabot) |
| `flaky-test`       | `#ededed` – grey   | Flaky test in CI                                |

## Mapping to Issue Templates

| Template                                                           | Default Labels                                                     |
| ------------------------------------------------------------------ | ------------------------------------------------------------------ |
| [Bug Report](../.github/ISSUE_TEMPLATE/bug-report.yml)             | `type/bug`, `status/triage`                                        |
| [Feature Request](../.github/ISSUE_TEMPLATE/feature-request.yml)   | `type/feature`, `status/triage`                                    |
| [Task](../.github/ISSUE_TEMPLATE/task.yml)                         | `type/task`, `status/triage`                                       |
| [Inquiry](../.github/ISSUE_TEMPLATE/inquiry.yml)                   | `type/inquiry`, `status/triage`                                    |
| [Flaky CI Test](../.github/ISSUE_TEMPLATE/flaky-ci-test-issue.yml) | `type/bug`, `area/ci`, `area/tests`, `status/triage`, `flaky-test` |
