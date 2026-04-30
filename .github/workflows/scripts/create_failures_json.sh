#!/usr/bin/env bash
# Derive failures.json from a TRX test results file.
#
# Required environment variables:
#   TRX_FILE, JOB_NAME, DOTNET, SERVER_TYPE, SERVER_VERSION,
#   HOST_OS, HOST_ARCH, HOST_RUNNER, RUN_ID, RUN_URL
set -euo pipefail

SUMMARY_FILE=failures.json

if ! command -v xmllint >/dev/null 2>&1 || ! command -v jq >/dev/null 2>&1; then
    echo '{"workflow":"C# tests","summary":{"note":"Install xmllint+jq for rich failures"}}' > "$SUMMARY_FILE"
    exit 0
fi

FAILED_COUNT=$(xmllint --xpath "string(//Counters/@failed)" "$TRX_FILE" 2>/dev/null || echo 0)
PASSED_COUNT=$(xmllint --xpath "string(//Counters/@passed)" "$TRX_FILE" 2>/dev/null || echo 0)
TOTAL_COUNT=$(xmllint --xpath "string(//Counters/@total)" "$TRX_FILE" 2>/dev/null || echo 0)
SKIPPED_COUNT=$(xmllint --xpath "string(//Counters/@skipped)" "$TRX_FILE" 2>/dev/null || echo 0)

jq -n \
    --arg workflow "C# tests" \
    --argjson runId "$RUN_ID" \
    --arg jobName "$JOB_NAME" \
    --arg dotnet "$DOTNET" \
    --arg serverType "$SERVER_TYPE" \
    --arg serverVersion "$SERVER_VERSION" \
    --arg os "$HOST_OS" \
    --arg arch "$HOST_ARCH" \
    --arg runner "$HOST_RUNNER" \
    --argjson total "$TOTAL_COUNT" \
    --argjson passed "$PASSED_COUNT" \
    --argjson failed "$FAILED_COUNT" \
    --argjson skipped "$SKIPPED_COUNT" \
    --arg runUrl "$RUN_URL" \
    '{
        workflow: $workflow,
        runId: $runId,
        jobName: $jobName,
        matrix: {
            dotnet: $dotnet,
            server: { type: $serverType, version: $serverVersion },
            host: { OS: $os, ARCH: $arch, RUNNER: $runner }
        },
        summary: { total: $total, passed: $passed, failed: $failed, skipped: $skipped },
        failed: [],
        links: { runUrl: $runUrl }
    }' > "$SUMMARY_FILE"
