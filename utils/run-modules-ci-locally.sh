#!/usr/bin/env bash
# Runs this repo's module tests (tests/Valkey.Glide.ModuleTests/) against a Docker-based
# standalone + cluster Valkey deployment with real valkey-search modules loaded -- not the
# default dev server the main integration-test suite's ServerModules fixtures exercise (which
# loads no module by default, and has no capability gating). Adapted from valkey-glide's
# utils/run-modules-ci-locally.sh for this repo.
#
# Usage: three image/release combinations, matching the port's three required local
# configurations:
#   utils/run-modules-ci-locally.sh                                                    # 1: rc2, emulate-release 1.3.0
#   EMULATE_RELEASE="" utils/run-modules-ci-locally.sh                                  # 2: rc2, module's own default (1.0.0)
#   IMAGE=valkey/valkey-bundle:9.1.3 SEARCH_MODULE_PATH="" EMULATE_RELEASE="" \
#     utils/run-modules-ci-locally.sh                                                   # 3: 9.1.3 (search 1.2.1), 1.3 tests skip
#
# SEARCH_MODULE_PATH defaults to a pre-built rc2 .so compiled from valkey-search source
# (tools/module-src/valkey-search/.build-release/libsearch.so in the parent Glide/ directory) so
# configs 1 and 2 use the real rc2 binary, not the 9.1.3 bundle image's bundled 1.2.1 module. Set
# it to an empty string to use the image's own module unmodified (config 3).
#
# Requires: docker, valkey-cli (Homebrew: `brew install valkey`), dotnet, and shlock (around any
# local module-CI run -- the container names here are fixed, so parallel runs across languages
# collide; take /tmp/glide-modules-ci.lock before invoking this script).

set -euo pipefail

REPO_ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
SCRATCH_DIR="${REPO_ROOT}/.module-ci-local-scratch"
mkdir -p "$SCRATCH_DIR"

IMAGE="${IMAGE:-valkey/valkey-bundle:9.2.0-rc2}"
ENGINE_VERSION="${ENGINE_VERSION:-9.0}"
CLUSTER_PORT_START="${CLUSTER_PORT_START:-8201}"
STANDALONE_PORT="${STANDALONE_PORT:-6499}"
DEFAULT_RC2_SO="$(cd "${REPO_ROOT}/.." && pwd)/tools/module-src/valkey-search/.build-release/libsearch.so"
SEARCH_MODULE_PATH="${SEARCH_MODULE_PATH-${DEFAULT_RC2_SO}}"
EMULATE_RELEASE="${EMULATE_RELEASE-1.3.0}"

# M6: DEFAULT_RC2_SO points outside this repo, at a layout this repo doesn't itself provide or
# document how to produce (a sibling checkout's build output). Rather than silently failing with
# a confusing Docker bind-mount error later if that layout isn't present, fail loudly and early
# with the actual problem and the two ways to recover: build it, or point SEARCH_MODULE_PATH
# somewhere else (including empty, to use the image's own bundled module instead).
if [ -n "$SEARCH_MODULE_PATH" ] && [ ! -f "$SEARCH_MODULE_PATH" ]; then
    echo "ERROR: SEARCH_MODULE_PATH (${SEARCH_MODULE_PATH}) does not exist." >&2
    if [ "$SEARCH_MODULE_PATH" = "$DEFAULT_RC2_SO" ]; then
        echo "This is the default path, which expects a valkey-search 1.3 .so built from source at" >&2
        echo "../tools/module-src/valkey-search/.build-release/libsearch.so relative to this repo --" >&2
        echo "a sibling checkout's build output this repo does not itself produce. Either build" >&2
        echo "valkey-search there, or set SEARCH_MODULE_PATH to an existing .so (or to an empty" >&2
        echo "string, to fall back to the chosen IMAGE's own bundled search module)." >&2
    fi
    exit 1
fi

if [ -n "$EMULATE_RELEASE" ] && ! [[ "$EMULATE_RELEASE" =~ ^[0-9]+\.[0-9]+\.[0-9]+$ ]]; then
    echo "ERROR: EMULATE_RELEASE must be X.Y.Z (e.g. 1.3.0) or empty, got: ${EMULATE_RELEASE}" >&2
    exit 1
fi

cleanup() {
    echo "=== Tearing down module containers ==="
    docker rm -f valkey-csh-modules-standalone >/dev/null 2>&1 || true
    for port in $(seq "$CLUSTER_PORT_START" "$((CLUSTER_PORT_START + 5))"); do
        docker rm -f "valkey-csh-modules-cluster-${port}" >/dev/null 2>&1 || true
    done
}
trap cleanup EXIT

echo "=== Checking utils/start-valkey-docker-modules.sh hasn't drifted from the submodule's shared copy (M6) ==="
"${REPO_ROOT}/utils/check-modules-harness-sync.sh"

echo "=== Starting module servers (IMAGE=${IMAGE}, SEARCH_MODULE_PATH=${SEARCH_MODULE_PATH:-<image default>}, EMULATE_RELEASE=${EMULATE_RELEASE:-<module default>}) ==="
export IMAGE ENGINE_VERSION CLUSTER_PORT_START STANDALONE_PORT SCRATCH_DIR SEARCH_MODULE_PATH
if [ -n "$EMULATE_RELEASE" ]; then
    export SEARCH_MODULE_ARGS="--emulate-release ${EMULATE_RELEASE}"
else
    export SEARCH_MODULE_ARGS=""
fi
ENV_CAPTURE="${SCRATCH_DIR}/env-capture"
GITHUB_ENV="$ENV_CAPTURE" "${REPO_ROOT}/utils/start-valkey-docker-modules.sh"
# The capture file uses the GITHUB_ENV "KEY=value" format (no `export`), so a plain `source`
# only sets local shell variables, not exported ones -- `dotnet test` below is a child process
# and would see neither var without the explicit `set -a`/`set +a` bracket.
set -a
# shellcheck disable=SC1090
source "$ENV_CAPTURE"
set +a
rm -f "$ENV_CAPTURE"

echo "=== Liveness: valkey_version, MODULE LIST, search.emulate-release ==="
for port in "$STANDALONE_PORT" $(seq "$CLUSTER_PORT_START" "$((CLUSTER_PORT_START + 5))"); do
    echo "--- port $port ---"
    valkey-cli -p "$port" INFO server | grep -i valkey_version
    valkey-cli -p "$port" MODULE LIST
    valkey-cli -p "$port" CONFIG GET search.emulate-release
done

echo "=== Running module tests (dotnet test) ==="
cd "$REPO_ROOT"
MODULES_STANDALONE_ENDPOINT="$MODULES_STANDALONE_ENDPOINT" \
    MODULES_CLUSTER_ENDPOINTS="${MODULES_CLUSTER_ENDPOINTS:-}" \
    dotnet test tests/Valkey.Glide.ModuleTests/Valkey.Glide.ModuleTests.csproj -c Debug

echo "=== Running the main suite's ServerModules FT integration tests against the module servers (H4) ==="
# TestConfiguration (tests/Valkey.Glide.IntegrationTests) reads standalone-endpoints/cluster-endpoints,
# not the MODULES_* names above -- map them here so the ~40 existing FT*Tests.cs tests under
# ServerModules/ actually exercise a real module-loaded server instead of silently skipping via
# ModuleUtils.SkipIfSearchModuleNotAvailableAsync against this project's own modules-free dev server.
env "standalone-endpoints=$MODULES_STANDALONE_ENDPOINT" \
    "cluster-endpoints=${MODULES_CLUSTER_ENDPOINTS:-}" \
    dotnet test tests/Valkey.Glide.IntegrationTests/Valkey.Glide.IntegrationTests.csproj -c Debug \
    --filter "FullyQualifiedName~ServerModules"
