---
name: Performance Testing
about: Track the performance testing for a Valkey GLIDE C# release (maintainers only)
title: "task(release): Valkey GLIDE C# {X.Y.Z} – Performance Testing"
type: "Task"
labels: ["performance", "dev"]
assignees: []
---

<!--
Performance testing sub-issue of a release tracking issue (for example, issue #659 under #619 for 1.2). Replace every
{placeholder}, set the milestone to {X.Y}, add it as a sub-issue of the tracking issue, and delete the HTML comments.
For the process, see docs/releasing/performance-testing-process.md.

The issue is public. Do not include AWS account IDs, S3 bucket or job names, or the names of internal systems.
-->

## Description

Performance testing for the Valkey GLIDE C# {X.Y.Z} release (#{tracking issue}).
The tests use [resp-bench](https://github.com/Bit-Quill/resp-bench) to compare {X.Y.Z} with two baselines: Valkey GLIDE {baseline version} and StackExchange.Redis {SE.Redis version}.

## Summary

<!--
Start with whether there are regressions.
Give ranges across all tested connection counts. Round them from the tables, for example +13.9% to +58.8% becomes
"14–59%".
Name each connection count or percentile where GLIDE is worse than the baseline or StackExchange.Redis. Give the size
of the gap.
If performance changed noticeably, name the probable cause: PRs in this repository and in valkey-io/valkey-glide.
Write "likely" unless you isolated the cause.
-->

Valkey GLIDE C# {X.Y.Z} {has no regressions / is noticeably faster / has regressions} relative to Valkey GLIDE {baseline version}.
Compared to StackExchange.Redis {SE.Redis version}, it {how it compares, for example "has higher throughput and lower latency at all connection counts"}.

Across all tested connection counts:

- Throughput is {range}% {higher/lower} than Valkey GLIDE {baseline version}, and {range}% {higher/lower} than StackExchange.Redis {SE.Redis version}.
- Relative to Valkey GLIDE {baseline version}, median (p50) latency is {range}% {lower/higher}, and p99 latency is {range}% {lower/higher}.
- Relative to StackExchange.Redis {SE.Redis version}, latency is {summary of latency comparison, with exceptions}.

{Likely cause of any notable change, or "No notable changes."}

## Key Details

|                     | {baseline version} (Baseline)          | {X.Y.Z}                                |
| ------------------- | -------------------------------------- | -------------------------------------- |
| Test date           | {YYYY-MM-DD}                           | {YYYY-MM-DD (HH:MM–HH:MM UTC)}         |
| Valkey GLIDE C#     | `{commit}` ({version or tag})          | `{commit}` ({X.Y.Z-rcN})               |
| StackExchange.Redis | {SE.Redis version}                     | {SE.Redis version}                     |
| resp-bench commit   | `{commit}`                             | `{commit}`                             |
| Machine             | AWS EC2 `m5.2xlarge` (8 vCPU, 32 GiB)  | AWS EC2 `m5.2xlarge` (8 vCPU, 32 GiB)  |
| OS                  | Amazon Linux 2023, x86_64              | Amazon Linux 2023, x86_64              |
| Server              | Valkey {version}, same host (loopback) | Valkey {version}, same host (loopback) |
| .NET                | .NET {version} SDK                     | .NET {version} SDK ({SDK version})     |

<!--
Get the RC commit, resp-bench commit, instance type, and Valkey version from run-metadata.json.
Get the .NET SDK version from logs/cloud-init-output.log.
-->

- **Workload:** the resp-bench `perf-csharp` matrix, with the `basic-standalone-single-client-1M-reqs` workload.
  - Warm-up: 1M SETs.
  - Steady state: 1M requests, 80% GET and 20% SET, over 1M keys. Keys are 16 bytes and values are 512 bytes.
  - Each connection count (1, 2, 4, 8, 16) ran {N} times for each client, with the default client configuration.
  - {State whether the baseline used the same workload and matrix settings.}
- **Baseline:** {the source of the baseline: the published NuGet version, or a commit. For a commit, give the
  differences from the tag, with any changes on the GET/SET path.}
- **Release candidate:** the {X.Y.Z} results are from {X.Y.Z-rcN} (`{commit}`). {List the changes between that RC and
  the final release. State whether any of them touch the GET/SET path.}
- **Reported numbers:** medians of the {N} iterations, from the steady-state (80/20) phase. The standard deviation of
  throughput across iterations was {N}% of the mean or less.
- **StackExchange.Redis:** the StackExchange.Redis cells set `DOTNET_ThreadPool_ForceMinWorkerThreads=80` (128
  minimum worker threads).
  - The resp-bench StackExchange.Redis client makes blocking calls from concurrent tasks.
  - Without this setting, the client times out at 16 or more connections on an 8-vCPU host.
  - {State whether the baseline run used the same setting.}

## Results

### Throughput

Requests per second, steady state:

| Connections | GLIDE {baseline} | GLIDE {X.Y} | SE.Redis {version} | GLIDE {baseline} to GLIDE {X.Y} (%) | SE.Redis {version} to GLIDE {X.Y} (%) |
| ----------: | ---------------: | ----------: | -----------------: | ----------------------------------: | ------------------------------------: |
|           1 |                  |             |                    |                                     |                                       |
|           2 |                  |             |                    |                                     |                                       |
|           4 |                  |             |                    |                                     |                                       |
|           8 |                  |             |                    |                                     |                                       |
|          16 |                  |             |                    |                                     |                                       |

### Latency

Microseconds, steady state:

<!-- Use the same columns as the throughput table for each of the four tables below. -->

#### GET p50

| Connections | GLIDE {baseline} | GLIDE {X.Y} | SE.Redis {version} | GLIDE {baseline} to GLIDE {X.Y} (%) | SE.Redis {version} to GLIDE {X.Y} (%) |
| ----------: | ---------------: | ----------: | -----------------: | ----------------------------------: | ------------------------------------: |
|           1 |                  |             |                    |                                     |                                       |
|           2 |                  |             |                    |                                     |                                       |
|           4 |                  |             |                    |                                     |                                       |
|           8 |                  |             |                    |                                     |                                       |
|          16 |                  |             |                    |                                     |                                       |

#### GET p99

| Connections | GLIDE {baseline} | GLIDE {X.Y} | SE.Redis {version} | GLIDE {baseline} to GLIDE {X.Y} (%) | SE.Redis {version} to GLIDE {X.Y} (%) |
| ----------: | ---------------: | ----------: | -----------------: | ----------------------------------: | ------------------------------------: |
|           1 |                  |             |                    |                                     |                                       |
|           2 |                  |             |                    |                                     |                                       |
|           4 |                  |             |                    |                                     |                                       |
|           8 |                  |             |                    |                                     |                                       |
|          16 |                  |             |                    |                                     |                                       |

#### SET p50

| Connections | GLIDE {baseline} | GLIDE {X.Y} | SE.Redis {version} | GLIDE {baseline} to GLIDE {X.Y} (%) | SE.Redis {version} to GLIDE {X.Y} (%) |
| ----------: | ---------------: | ----------: | -----------------: | ----------------------------------: | ------------------------------------: |
|           1 |                  |             |                    |                                     |                                       |
|           2 |                  |             |                    |                                     |                                       |
|           4 |                  |             |                    |                                     |                                       |
|           8 |                  |             |                    |                                     |                                       |
|          16 |                  |             |                    |                                     |                                       |

#### SET p99

| Connections | GLIDE {baseline} | GLIDE {X.Y} | SE.Redis {version} | GLIDE {baseline} to GLIDE {X.Y} (%) | SE.Redis {version} to GLIDE {X.Y} (%) |
| ----------: | ---------------: | ----------: | -----------------: | ----------------------------------: | ------------------------------------: |
|           1 |                  |             |                    |                                     |                                       |
|           2 |                  |             |                    |                                     |                                       |
|           4 |                  |             |                    |                                     |                                       |
|           8 |                  |             |                    |                                     |                                       |
|          16 |                  |             |                    |                                     |                                       |
