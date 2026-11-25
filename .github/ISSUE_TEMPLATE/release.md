---
name: Release
about: Track a Valkey GLIDE C# release (maintainers only)
title: "task(release): Valkey GLIDE C# {X.Y.Z} – Release"
type: "Task"
labels: ["dev"]
assignees: []
---

<!--
Release tracking issue (for example, issue #619 for 1.2). Replace every {placeholder}, set the milestone to {X.Y}, and
delete the HTML comments. For the process, see docs/releasing/release-process.md.

The issue is public. Do not include AWS account IDs, internal test project names, or the names of internal systems.
-->

## Description

Publish Valkey GLIDE C# version {X.Y.Z} on [NuGet](https://www.nuget.org/packages/Valkey.Glide) and [GitHub](https://github.com/valkey-io/valkey-glide-csharp/releases).
This release follows the [Valkey GLIDE {a.b.c} release](https://github.com/valkey-io/valkey-glide/releases).

## Checklist

### Pre-release candidate

<!-- Add an item for each change needed before the first RC, with its pull request. -->

1. [ ] Create release branch – [release-{X.Y}](https://github.com/valkey-io/valkey-glide-csharp/tree/release-{X.Y}).
1. [ ] Update submodule to the `valkey-glide` release branch (#{PR}).
1. [ ] Verify that the changelog includes all changes (#{PR}).
1. [ ] Full matrix tests pass on the release branch.

### Post-release candidate

<!-- Do these checks on the published RC. For the internal tests, record only the run ID. -->

| Status | Item                   | Artifact          | Notes                   |
| ------ | ---------------------- | ----------------- | ----------------------- |
|        | Full matrix tests      |                   |                         |
|        | Internal tests         |                   |                         |
|        | Internal bug bash      | [Issues](#issues) |                         |
|        | Internal AppSec review | [Issues](#issues) |                         |
|        | Performance            | #{issue}          | Tested on `{X.Y.Z-rcN}` |
|        | Stability test         | #{issue}          | Tested on `{X.Y.Z-rcN}` |

### Release

| Status | Item                            | Artifact |
| ------ | ------------------------------- | -------- |
|        | Create `v{X.Y.Z}` tag on GitHub |          |
|        | Publish `{X.Y.Z}` to NuGet      |          |

### Post-release

| Status | Item                                                                                          | Artifact |
| ------ | --------------------------------------------------------------------------------------------- | -------- |
|        | Documentation verified on [glide.valkey.io](https://glide.valkey.io/)                         |          |
|        | Release notes published on GitHub                                                             |          |
|        | [Supported Features](https://glide.valkey.io/reference/supported-features/) docs page updated |          |
|        | [Supported Commands](https://glide.valkey.io/commands/supported-commands/) docs page updated  |          |
|        | Cherry-pick to `main`                                                                         |          |

## Builds

<!-- Add a row for every tag, including RCs that were never published. -->

| ID          | GitHub tag                                                                         | NuGet release |
| ----------- | ---------------------------------------------------------------------------------- | ------------- |
| {X.Y.Z-rc1} | [v{X.Y.Z-rc1}](https://github.com/valkey-io/valkey-glide-csharp/tree/v{X.Y.Z-rc1}) |               |

## Pull Requests

Pull requests to release branch, in order:

1. #{PR}

Pull requests to cherry-pick release branch changes to `main`, in order:

1. #{PR}

## Issues

New issues raised during the review process:

1. #{issue}

## Symbols

- ✅ – completed or passed
- ❌ – not completed or failed
- 🏳️ – not applicable
