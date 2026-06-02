# Reviewing and Merging Pull Requests

> This guide covers what happens after a pull request is submitted — how maintainers review pull requests,
> what contributors should expect during the review process, and how pull requests get merged or closed.

## What Maintainers Look For

When a maintainer picks up your pull request for review, they evaluate it against several criteria:

| Area                        | What's checked                                                                                             | Reference                            |
| --------------------------- | ---------------------------------------------------------------------------------------------------------- | ------------------------------------ |
| **Code quality**            | Readability, structure, naming conventions, and adherence to the project's coding style                    |                                      |
| **Test coverage**           | New or changed functionality has corresponding tests that pass                                             | [coverage.md](coverage.md)           |
| **Documentation**           | All public and protected members have XML doc comments (`CS1591` produces zero warnings)                   | [documentation.md](documentation.md) |
| **API compatibility**       | Public API surface maintains compatibility with StackExchange.Redis (target version 2.8.58) where possible |                                      |
| **Contribution guidelines** | DCO signoff is present, conventional commit format is used, pull request template is filled out            | [DEVELOPER.md](../DEVELOPER.md)      |
| **CI status**               | All automated checks (build, lint, tests) are passing                                                      |                                      |

Maintainers may request changes, ask clarifying questions, or suggest alternative approaches. This is a normal part of the process — it's collaborative, not adversarial.

## Staying Involved

Once you open a pull request, stay engaged:

- **Respond to feedback promptly.** Maintainers set aside time to review, and a timely response keeps the momentum going.
- **Address all review comments.** If you disagree with a suggestion, explain your reasoning — discussion is welcome.
- **Push follow-up commits** to address requested changes rather than force-pushing over the review history, unless asked otherwise.
- **Monitor CI checks.** If a check fails after you push updates, investigate and fix it before pinging reviewers again.

Letting a pull request go quiet for too long can result in it being marked stale (see below).

## Keep Pull Requests Focused

Each pull request should address a single change or issue. Focused pull requests are:

- Easier for maintainers to review
- Less likely to introduce unrelated regressions
- Faster to get approved and merged

If your work touches multiple areas, consider splitting it into separate pull requests that can be reviewed independently.

## Pull Request Checklist

Before requesting a review, make sure you've worked through the [pull request template](../.github/pull_request_template.md) checklist.

## Merge Strategies

The repository uses two merge strategies depending on the branch context:

| Scenario                | Strategy               | Why                                                       |
| ----------------------- | ---------------------- | --------------------------------------------------------- |
| Feature branch → `main` | **Squash and merge**   | Combines all commits into one clean commit on `main`      |
| Release branch → `main` | **Merge commit**       | Preserves the full commit history from the release branch |

Most contributor pull requests target `main` from a feature branch, so squash and merge is the typical path. The maintainer merging the pull request will select the appropriate strategy.

## Inactive Pull Requests

Pull requests that go quiet for an extended period may be closed by a maintainer at their discretion. If you're still working on a PR that has gone inactive, push an update or leave a comment to signal it's still in progress. If a closed pull request is still relevant, you can reopen it or open a new one.

## Maintainer Takeover of Pull Requests

Maintainers may sometimes take over a pull request to get it across the finish line. This most often happens when the original contributor has become unresponsive or the pull request has gone stale for too long.

Before taking over, a maintainer will leave a comment on the pull request indicating their intent and allow the original contributor a reasonable period to respond. If there is no response, maintainers reserve the right to complete or close the pull request.

## Related Resources

- [Pull Request Template](../.github/pull_request_template.md)
- [Contributing Guide](../CONTRIBUTING.md)
- [Developer Guide](../DEVELOPER.md)
