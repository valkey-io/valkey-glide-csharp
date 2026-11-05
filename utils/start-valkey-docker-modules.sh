#!/usr/bin/env bash
# Starts standalone + cluster Valkey servers with modules (Search, JSON, Bloom, LDAP) via Docker,
# on dedicated ports, and verifies MODULE LIST on every node. Adapted from valkey-glide's
# utils/start-valkey-docker-modules.sh (the main repo's shared CI/local-reproduction script) for
# this repo's module-test harness.
#
# Every container runs with --network host, which Docker Desktop on macOS requires enabling
# separately (Settings > Resources > Network > "Enable host networking") -- without it,
# containers start but 127.0.0.1:<port> is never reachable from the host, which looks identical
# to the server itself failing.
#
# Required env vars:
#   IMAGE               Docker image tag, e.g. valkey/valkey-bundle:9.1.3
#   ENGINE_VERSION      Valkey engine version, e.g. 9.0 (controls EXTRA_ARGS)
# Optional env vars:
#   CLUSTER_PORT_START  First of 6 consecutive cluster ports (default 8201)
#   STANDALONE_PORT     Standalone port (default 6499)
#   SEARCH_MODULE_PATH  Path to a search module .so to bind-mount in place of the image's own
#                       /usr/lib/valkey/libsearch.so (e.g. a search 1.3 .so built from source).
#                       Empty uses the image's own module unmodified.
#   SEARCH_MODULE_ARGS  Extra args appended to the search module's loadmodule line, e.g.
#                       "--emulate-release 1.3.0". The arg name must be the BARE config name
#                       ("emulate-release", not "search.emulate-release" -- that longer form only
#                       exists at the CONFIG GET/SET surface, where Valkey core namespaces every
#                       module config by module name; a .conf file's loadmodule directive does not
#                       do that stripping).
#   SCRATCH_DIR         Directory for generated .conf files. MUST be under $HOME on macOS Docker
#                       Desktop (shares /Users by default, not /tmp -- a bind-mounted file staged
#                       under /tmp silently becomes an empty directory in the container).
#
# On success, exports (via GITHUB_ENV if set, else writes export lines to
# $SCRATCH_DIR/endpoints.env for the caller to source) these variables:
#   MODULES_STANDALONE_ENDPOINT
#   MODULES_CLUSTER_ENDPOINTS

set -euo pipefail

: "${IMAGE:?IMAGE must be set, e.g. valkey/valkey-bundle:9.1.3}"
: "${ENGINE_VERSION:?ENGINE_VERSION must be set, e.g. 9.0}"
CLUSTER_PORT_START="${CLUSTER_PORT_START:-8201}"
STANDALONE_PORT="${STANDALONE_PORT:-6499}"
SEARCH_MODULE_PATH="${SEARCH_MODULE_PATH:-}"
SEARCH_MODULE_ARGS="${SEARCH_MODULE_ARGS:-}"
REPO_ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
SCRATCH_DIR="${SCRATCH_DIR:-${REPO_ROOT}/.module-ci-local-scratch}"
mkdir -p "$SCRATCH_DIR"
FALLBACK_ENV_FILE="${SCRATCH_DIR}/endpoints.env"
if [ -z "${GITHUB_ENV:-}" ]; then
    : > "$FALLBACK_ENV_FILE"
fi

export_var() {
    local key="$1" value="$2"
    if [ -n "${GITHUB_ENV:-}" ]; then
        echo "${key}=${value}" >> "$GITHUB_ENV"
    else
        echo "export ${key}=${value}" >> "$FALLBACK_ENV_FILE"
    fi
}

echo "Pulling ${IMAGE}..."
docker pull "$IMAGE"

macos_host_network_hint() {
    if [[ "$(uname -s)" == "Darwin" ]]; then
        echo "HINT: this script uses --network host, which Docker Desktop on" >&2
        echo "macOS requires enabling separately (Settings > Resources >" >&2
        echo "Network > 'Enable host networking') -- if that setting is off," >&2
        echo "the container starts but the port above is never reachable from" >&2
        echo "this host, which looks exactly like the readiness timeout above." >&2
    fi
}

# Container-internal path for the search module .so: the image's own path, always --
# SEARCH_MODULE_PATH (when set) bind-mounts a replacement file AT that same path rather than a
# second path, since the bundle image's entrypoint auto-discovers and loads every .so under
# /usr/lib/valkey/ not already named in the conf. Naming the override at a different path
# (confirmed empirically) leaves the original /usr/lib/valkey/libsearch.so in place for
# auto-discovery to find too, and the second load of the same module name aborts the server
# ("Module name is busy").
SEARCH_MODULE_CONTAINER_PATH="/usr/lib/valkey/libsearch.so"
SEARCH_MODULE_MOUNT_ARGS=()
if [ -n "$SEARCH_MODULE_PATH" ]; then
    SEARCH_MODULE_MOUNT_ARGS=(-v "${SEARCH_MODULE_PATH}:${SEARCH_MODULE_CONTAINER_PATH}:ro")
fi

gen_conf() {
    local port="$1" extra_lines="$2" conf_path="$3"
    {
        echo "port ${port}"
        echo "bind 127.0.0.1 -::1"
        echo "protected-mode no"
        echo "save \"\""
        echo "enable-debug-command yes"
        if [ -n "$extra_lines" ]; then
            echo "$extra_lines"
        fi
        echo "loadmodule ${SEARCH_MODULE_CONTAINER_PATH} ${SEARCH_MODULE_ARGS}"
    } > "$conf_path"
}

echo "=== Starting standalone Valkey server with modules on port ${STANDALONE_PORT} ==="
docker rm -f valkey-csh-modules-standalone >/dev/null 2>&1 || true
STANDALONE_CONF="${SCRATCH_DIR}/standalone.conf"
gen_conf "$STANDALONE_PORT" "" "$STANDALONE_CONF"
docker run -d --name valkey-csh-modules-standalone --network host \
    -v "${STANDALONE_CONF}:/tmp/valkey.conf" \
    "${SEARCH_MODULE_MOUNT_ARGS[@]+"${SEARCH_MODULE_MOUNT_ARGS[@]}"}" \
    "$IMAGE" valkey-server /tmp/valkey.conf

READY=false
for _ in $(seq 1 30); do
    if valkey-cli -p "$STANDALONE_PORT" PING 2>/dev/null | grep -q PONG; then
        echo "Standalone server with modules ready on port ${STANDALONE_PORT}"
        READY=true
        break
    fi
    sleep 0.5
done
if [ "$READY" != "true" ]; then
    echo "ERROR: Standalone module server on port ${STANDALONE_PORT} failed to start"
    docker logs valkey-csh-modules-standalone 2>&1 || true
    macos_host_network_hint
    exit 1
fi
export_var MODULES_STANDALONE_ENDPOINT "127.0.0.1:${STANDALONE_PORT}"

PORT_START=$CLUSTER_PORT_START
PORT_END=$((PORT_START + 5))
echo "=== Starting Valkey cluster with modules on ports ${PORT_START}-${PORT_END} ==="

MAJOR=$(echo "$ENGINE_VERSION" | cut -d. -f1)
EXTRA_ARGS=""
if [ "$MAJOR" -ge 9 ]; then
    EXTRA_ARGS="cluster-databases 16"
fi

for port in $(seq "$PORT_START" "$PORT_END"); do
    docker rm -f "valkey-csh-modules-cluster-${port}" >/dev/null 2>&1 || true
    NODE_CONF="${SCRATCH_DIR}/cluster-${port}.conf"
    gen_conf "$port" "cluster-enabled yes
cluster-config-file nodes-${port}.conf
cluster-node-timeout 5000
appendonly no
${EXTRA_ARGS}" "$NODE_CONF"
    docker run -d --name "valkey-csh-modules-cluster-${port}" --network host \
        -v "${NODE_CONF}:/tmp/valkey.conf" \
        "${SEARCH_MODULE_MOUNT_ARGS[@]+"${SEARCH_MODULE_MOUNT_ARGS[@]}"}" \
        "$IMAGE" valkey-server /tmp/valkey.conf
done

for port in $(seq "$PORT_START" "$PORT_END"); do
    READY=false
    for _ in $(seq 1 30); do
        if valkey-cli -p "$port" PING 2>/dev/null | grep -q PONG; then
            echo "  Cluster node $port: ready"
            READY=true
            break
        fi
        sleep 0.5
    done
    if [ "$READY" != "true" ]; then
        echo "ERROR: Cluster module node on port $port failed to start"
        docker logs "valkey-csh-modules-cluster-${port}" 2>&1 || true
        macos_host_network_hint
        exit 1
    fi
done

ENDPOINTS=""
for port in $(seq "$PORT_START" "$PORT_END"); do
    if [ -n "$ENDPOINTS" ]; then
        ENDPOINTS="${ENDPOINTS},"
    fi
    ENDPOINTS="${ENDPOINTS}127.0.0.1:${port}"
done

P1=$PORT_START
P2=$((PORT_START + 1))
P3=$((PORT_START + 2))
P4=$((PORT_START + 3))
P5=$((PORT_START + 4))
P6=$((PORT_START + 5))
valkey-cli --cluster create \
    127.0.0.1:$P1 127.0.0.1:$P2 127.0.0.1:$P3 \
    127.0.0.1:$P4 127.0.0.1:$P5 127.0.0.1:$P6 \
    --cluster-replicas 1 \
    --cluster-yes

export_var MODULES_CLUSTER_ENDPOINTS "$ENDPOINTS"
echo "Cluster with modules ready on ports ${PORT_START}-${PORT_END}"

echo "=== Module server verification ==="
# M6: previously checked only for 'search', unlike the submodule's shared script, which checks
# the full set the bundle image ships (search, json, bf, ldap) -- a real functional drift from
# the shared file, not just documentation wording. A conf-generation regression that silently
# dropped json/bf/ldap (the exact failure mode the shared script's own comment warns about) would
# have passed unnoticed here.
check_modules() {
    local label="$1"
    local port="$2"
    local versions
    versions=$(valkey-cli -p "$port" MODULE LIST | awk '
        $0=="name" { getline name }
        $0=="ver"  { getline ver; print name, ver }
    ')
    echo "${label} (port ${port}) loaded modules (name version):"
    echo "${versions}"

    for mod in search json bf ldap; do
        if ! echo "${versions}" | grep -q "^${mod} "; then
            echo "ERROR: ${label} (port ${port}) is missing the '${mod}' module"
            exit 1
        fi
    done
}

echo "Standalone with modules (${STANDALONE_PORT}): $(valkey-cli -p "$STANDALONE_PORT" PING)"
check_modules "Standalone" "$STANDALONE_PORT"

for port in $(seq "$PORT_START" "$PORT_END"); do
    echo "Cluster with modules node $port: $(valkey-cli -p "$port" PING)"
    check_modules "Cluster node" "$port"
done
