---
name: Stability Testing
about: Track the stability testing for a Valkey GLIDE C# release (maintainers only)
title: "task(release): Valkey GLIDE C# {X.Y.Z} – Stability Testing"
type: "Task"
labels: ["performance", "dev"]
assignees: []
---

<!--
Stability testing sub-issue of a release tracking issue (for example, issue #660 under #619 for 1.2). Replace every
{placeholder}, set the milestone to {X.Y}, add it as a sub-issue of the tracking issue, and delete the HTML comments.
For the process, see docs/releasing/stability-testing-process.md.

The issue is public. Do not include AWS account IDs, S3 bucket or job names, or the names of internal systems.
-->

## Description

Stability testing for the Valkey GLIDE C# {X.Y.Z} release (#{tracking issue}).
The tests use [resp-bench](https://github.com/Bit-Quill/resp-bench) to run a 12-hour soak test, and compare it with a StackExchange.Redis {SE.Redis version} control run.

## Summary

<!--
Start with whether there are errors or signs of a memory leak.
A leak shows as RSS that continues to increase after start-up while the StackExchange.Redis control stays flat.
Give the change from hour 1 to the last full hour, and the fitted trend.
Both runs use all of the shared test host. Thus, describe throughput as sustained load, not as a comparison of the
clients. Give a caveat with each latency comparison.
-->

Valkey GLIDE C# {X.Y.Z} ran for 12 hours at full load.
It had {no errors / N errors}, and {no sign of a memory leak / signs of a memory leak}.

- Memory (RSS) {stayed flat after start-up / continued to increase}. It {grew/shrank} by {N} MiB ({N}%) from hour 1 to hour {last full hour}, {the same as / unlike} the StackExchange.Redis control.
- GLIDE sustained about {N} requests per second ({N} billion requests) for 12 hours, with {zero/N} errors.
  Both runs used all of the shared test host (about {N}% CPU). Thus, throughput shows the limit of the test setup, not of the client.
- {Latency comparison with StackExchange.Redis: p99, p99.9, max, and p50, with exceptions.}
  Both runs used all of the host. Thus, these figures show mostly the queues on the host, not the clients.

## Key Details

|                   | Valkey GLIDE C# {X.Y.Z}                | StackExchange.Redis (control)          |
| ----------------- | -------------------------------------- | -------------------------------------- |
| Test date         | {YYYY-MM-DD to YYYY-MM-DD}             | {YYYY-MM-DD to YYYY-MM-DD}             |
| Client            | `{commit}` ({X.Y.Z-rcN})               | {SE.Redis version}                     |
| resp-bench commit | `{commit}`                             | `{commit}`                             |
| Machine           | AWS EC2 `m5.2xlarge` (8 vCPU, 32 GiB)  | AWS EC2 `m5.2xlarge` (8 vCPU, 32 GiB)  |
| OS                | Amazon Linux 2023, x86_64              | Amazon Linux 2023, x86_64              |
| Server            | Valkey {version}, same host (loopback) | Valkey {version}, same host (loopback) |
| .NET              | .NET {version} SDK ({SDK version})     | .NET {version} SDK ({SDK version})     |

<!--
Get the client and resp-bench commits, instance type, and Valkey version from the run-metadata.json file of each run.
Get the .NET SDK version from the logs/cloud-init-output.log file of each run.
-->

- **Workload:** the resp-bench `soak-csharp` (GLIDE) and `soakctl-csharp` (StackExchange.Redis) matrices, with the `soak-standalone-12h` workload.
  - Warm-up: 100K SETs.
  - Steady state: 12 hours of 80% GET and 20% SET at full load, over 100K keys. Keys are 16 bytes and values are 512 bytes.
  - 64 connections, with the high-throughput client configurations.
- **Control run:** the StackExchange.Redis run used the same workload on a different instance.
- **Release candidate:** the results are from {X.Y.Z-rcN} (`{commit}`). {List the changes between that RC and the final release. State whether any of them touch the GET/SET path.}
- **resp-bench commits:** {If the two runs used different commits, give the differences, and confirm that the workload is the same.}
  - The control run set `DOTNET_ThreadPool_ForceMinWorkerThreads=80` (128 minimum worker threads).
  - The resp-bench StackExchange.Redis client makes blocking calls from concurrent tasks. Without this setting, the client times out on its first request at 64 connections.
- **Memory:** the RSS of the whole benchmark process group, sampled every 5 seconds. The group includes the `make` and `dotnet run` host processes.
  - Read RSS as a trend, not as the memory of the client alone.
  - The trend does not include the first hour. RSS increases during start-up, by about {N} MiB for GLIDE and {N} MiB for StackExchange.Redis.
- **Throughput and latency:** one aggregate over the 12-hour steady-state phase. The resp-bench C# engine does not report values for each interval.
  - The workload has no rate limit, and the client and server share the 8 vCPUs of the host.
  - Thus, at 64 connections, the host limits both runs (about {N}% CPU), not the client.

## Results

<!-- Attach the RSS chart (both runs on one plot, RSS against elapsed hours) and the results zip to the issue. -->

### Memory

| Metric                        |   GLIDE {X.Y} | SE.Redis {version} |
| ----------------------------- | ------------: | -----------------: |
| RSS at start                  |               |                    |
| RSS, hour 1 median            |               |                    |
| RSS, hour {last} median       |               |                    |
| Change, hour 1 to hour {last} |               |                    |
| RSS range after hour 1        |               |                    |
| Fitted trend after hour 1     |               |                    |

### Throughput and Errors

Steady state, 12 hours:

| Metric                  | GLIDE {X.Y} | SE.Redis {version} |
| ----------------------- | ----------: | -----------------: |
| Requests                |             |                    |
| Errors                  |             |                    |
| Throughput (requests/s) |             |                    |
| Median CPU (whole host) |             |                    |

### Latency

Microseconds, steady state, 12 hours:

| Metric    | GLIDE {X.Y} | SE.Redis {version} | GLIDE vs SE.Redis (%) |
| --------- | ----------: | -----------------: | --------------------: |
| GET p50   |             |                    |                       |
| GET p99   |             |                    |                       |
| GET p99.9 |             |                    |                       |
| GET max   |             |                    |                       |
| SET p50   |             |                    |                       |
| SET p99   |             |                    |                       |
| SET p99.9 |             |                    |                       |
| SET max   |             |                    |                       |
