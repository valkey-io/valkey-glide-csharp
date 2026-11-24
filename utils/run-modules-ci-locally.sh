#!/usr/bin/env bash
# Runs the module tests in tests/Valkey.Glide.ModuleTests against Docker standalone and cluster
# deployments with valkey-search modules loaded.
#
# Usage:
#   SEARCH_MODULE_PATH=/path/to/libsearch.so utils/run-modules-ci-locally.sh
#                                                                   # that module, emulate-release 1.3.0
#   SEARCH_MODULE_PATH=/path/to/libsearch.so EMULATE_RELEASE="" utils/run-modules-ci-locally.sh
#                                                                   # that module, its own default level
#   IMAGE=valkey/valkey-bundle:9.1.3 SEARCH_MODULE_PATH="" EMULATE_RELEASE="" \
#     utils/run-modules-ci-locally.sh                               # bundled 1.2.1 module, 1.3 tests skip
#
# SEARCH_MODULE_PATH is required. It selects the search module .so bind-mounted into the
# containers. An empty value uses the image's own module.
# Requires docker, valkey-cli, and dotnet. Hold /tmp/glide-modules-ci.lock around any run,
# because the container names are fixed.

set -euo pipefail

REPO_ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
SCRATCH_DIR="${REPO_ROOT}/.module-ci-local-scratch"
mkdir -p "$SCRATCH_DIR"

IMAGE="${IMAGE:-valkey/valkey-bundle:9.2.0-rc2}"
ENGINE_VERSION="${ENGINE_VERSION:-9.0}"
CLUSTER_PORT_START="${CLUSTER_PORT_START:-8201}"
STANDALONE_PORT="${STANDALONE_PORT:-6499}"
SEARCH_MODULE_PATH="${SEARCH_MODULE_PATH?set SEARCH_MODULE_PATH to a libsearch.so path, or to an empty string to use the image module}"
EMULATE_RELEASE="${EMULATE_RELEASE-1.3.0}"

if [ -n "$SEARCH_MODULE_PATH" ] && [ ! -f "$SEARCH_MODULE_PATH" ]; then
    echo "ERROR: SEARCH_MODULE_PATH (${SEARCH_MODULE_PATH}) does not exist." >&2
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

echo "=== Checking utils/start-valkey-docker-modules.sh hasn't drifted from the submodule's shared copy ==="
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

echo "=== Running module tests (task test:modules) ==="
cd "$REPO_ROOT"
MODULES_STANDALONE_ENDPOINT="$MODULES_STANDALONE_ENDPOINT" \
    MODULES_CLUSTER_ENDPOINTS="${MODULES_CLUSTER_ENDPOINTS:-}" \
    task test:modules configuration=Debug

echo "=== Running the main suite's ServerModules FT integration tests against the module servers ==="
env "standalone-endpoints=$MODULES_STANDALONE_ENDPOINT" \
    "cluster-endpoints=${MODULES_CLUSTER_ENDPOINTS:-}" \
    dotnet test tests/Valkey.Glide.IntegrationTests/Valkey.Glide.IntegrationTests.csproj -c Debug \
    --filter "FullyQualifiedName~ServerModules"
