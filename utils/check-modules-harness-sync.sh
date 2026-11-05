#!/usr/bin/env bash
# CI-sync guard (M6, port rules: "Python's CI-sync guard pins that file"): fails loudly if
# utils/start-valkey-docker-modules.sh (THIS REPO's fork) and the shared
# valkey-glide/utils/start-valkey-docker-modules.sh (submodule) have drifted in a way this guard
# cannot already account for.
#
# This repo's copy is NOT additive to the shared file -- it is a fork with real, intentional
# differences (default ports/container names to avoid colliding with the submodule's own CI use
# of the shared script; the SEARCH_MODULE_PATH bind-mount feature, which the shared script has no
# equivalent of). That is a deviation from the port rule's "shared harness files are additive"
# requirement, which assumes a single upstream file every port can add its own arm to. The
# submodule's start-valkey-docker-modules.sh has no such extension point today (it hard-codes
# /usr/lib/valkey/libsearch.so with no override), so adding SEARCH_MODULE_PATH without forking
# would require an upstream change to valkey-glide itself, outside this fork's scope.
#
# Since a true additive merge isn't possible without an upstream change, this script instead
# diffs the two files' SHARED machinery (the parts this fork did not intentionally change) and
# fails if that shared machinery has drifted -- catching the case where the submodule pin moves
# forward and the shared script changes underneath this fork's copy (e.g. a new safety fix, a new
# CodeRabbit-flagged bug fix, a new required module) without this fork being revisited to port it
# across by hand.
#
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

# Known, intentional divergences (documented at the top of the fork): default ports, container
# names, the SEARCH_MODULE_PATH feature, and comment wording. Strip comment-only lines (this
# fork's comments are deliberately rewritten in its own voice) and the specific, known functional
# lines before comparing everything else -- this is deliberately narrow (exact strings, not
# regexes) so an unrelated FUNCTIONAL change anywhere else in either file still trips the guard.
strip_known_diffs() {
    # Drop comments/blanks, then the whole SEARCH_MODULE_PATH if-block (identified by its
    # distinctive `if [ -n "$SEARCH_MODULE_PATH" ]` condition through its matching `fi`) as a
    # unit -- safer than stripping every bare `fi` line, which would also hide an unrelated `fi`
    # elsewhere in the file.
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
