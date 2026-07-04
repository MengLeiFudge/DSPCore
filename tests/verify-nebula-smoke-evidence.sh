#!/usr/bin/env bash
set -euo pipefail

host_log="${1:-}"
client_log="${2:-}"

if [[ -z "$host_log" || -z "$client_log" ]]; then
    echo "Usage: tests/verify-nebula-smoke-evidence.sh <host LogOutput.log> <client LogOutput.log>" >&2
    exit 2
fi

require() {
    local log_file="$1"
    local pattern="$2"
    local message="$3"

    if [[ ! -f "$log_file" ]]; then
        echo "Log does not exist: $log_file" >&2
        exit 1
    fi

    if ! grep -qF "$pattern" "$log_file"; then
        echo "$message" >&2
        echo "Missing pattern: $pattern" >&2
        echo "Log: $log_file" >&2
        exit 1
    fi
}

require_accepted_true() {
    local log_file="$1"
    local prefix="$2"
    local message="$3"

    if [[ ! -f "$log_file" ]]; then
        echo "Log does not exist: $log_file" >&2
        exit 1
    fi

    if ! grep -Eq "$prefix accepted=True, hasTransport=True" "$log_file"; then
        echo "$message" >&2
        echo "Missing pattern: $prefix accepted=True, hasTransport=True" >&2
        echo "Log: $log_file" >&2
        exit 1
    fi
}

require "$host_log" "DSPCore Nebula adapter is initialized." "Host must initialize the DSPCore Nebula adapter."
require "$client_log" "DSPCore Nebula adapter is initialized." "Client must initialize the DSPCore Nebula adapter."

require_accepted_true "$client_log" "DSPCore smoke packet send" "Client packet send must be accepted by Nebula transport."
require "$host_log" "DSPCore smoke received packet" "Host must receive the client smoke packet."

require_accepted_true "$client_log" "DSPCore smoke host relay send" "Client host relay send must be accepted by Nebula transport."
require "$host_log" "DSPCore smoke host handled relay" "Host must handle the smoke host relay."

require_accepted_true "$client_log" "DSPCore smoke planet data request" "Client planet data request must be accepted by Nebula transport."
require "$host_log" "DSPCore smoke exported planet data" "Host must export smoke planet data."
require "$client_log" "DSPCore smoke imported planet data" "Client must import smoke planet data response."
