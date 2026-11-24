#!/usr/bin/env bash
# Usage: utils/check-modules-harness-sync.sh
# Exit 0: no drift in the tracked shared sections, or an expected/documented difference.
# Exit 1: something in a shared section differs and needs a human to look at both files.

set -euo pipefail

REPO_ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
FORK="${REPO_ROOT}/utils/start-valkey-docker-modules.sh"
UPSTREAM="${REPO_ROOT}/valkey-glide/utils/start-valkey-docker-modules.sh"

if [ ! -f "$UPSTREAM" ]; then
    echo "ERROR: submodule shared script not found at ${UPSTREAM} -- is the valkey-glide submodule checked out?" >&2
    exit 1
fi

strip_known_diffs() {
    grep -v '^\s*#' "$1" | grep -v '^\s*$' | awk '
        /if \[ -n "\$SEARCH_MODULE_PATH" \]/ { skip = 1 }
        skip && /^fi$/ { skip = 0; next }
        skip { next }
        { print }
    ' | grep -v \
        -e 'CLUSTER_PORT_START:-8[012]01' \
        -e 'STANDALONE_PORT:-6[349]99\|STANDALONE_PORT:-6389' \
        -e 'SEARCH_MODULE_PATH' \
        -e 'SEARCH_MODULE_CONTAINER_PATH' \
        -e 'SEARCH_MODULE_MOUNT_ARGS' \
        -e 'valkey-csh-modules-\|valkey-modules-' \
        -e 'loadmodule.*SEARCH_MODULE_CONTAINER_PATH\|loadmodule /usr/lib/valkey/libsearch.so'
}

DIFF_OUTPUT="$(diff <(strip_known_diffs "$FORK") <(strip_known_diffs "$UPSTREAM") || true)"

if [ -n "$DIFF_OUTPUT" ]; then
    echo "ERROR: utils/start-valkey-docker-modules.sh has drifted from the submodule's shared" >&2
    echo "utils/start-valkey-docker-modules.sh outside the known, documented differences" >&2
    echo "(ports, container names, SEARCH_MODULE_PATH). Review the diff below and either port the" >&2
    echo "upstream change across by hand, or update strip_known_diffs() in this guard if the" >&2
    echo "difference is newly intentional:" >&2
    echo "$DIFF_OUTPUT" >&2
    exit 1
fi

echo "OK: no undocumented drift between this fork and the submodule's shared start-valkey-docker-modules.sh"
