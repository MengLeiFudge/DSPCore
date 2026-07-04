#!/usr/bin/env bash
set -euo pipefail

repo_root="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
profile="${1:-}"
mode="${2:-}"
address="${3:-127.0.0.1:8469}"

if [[ -z "$profile" || -z "$mode" ]]; then
    echo "Usage: tests/update-nebula-smoke-profile.sh <profile> <Host|Client|Off> [address]" >&2
    exit 2
fi

case "$mode" in
    Host|Client|Off) ;;
    *)
        echo "Mode must be Host, Client, or Off: $mode" >&2
        exit 2
        ;;
esac

if [[ ! -d "$profile/BepInEx" ]]; then
    echo "Profile does not contain BepInEx: $profile" >&2
    exit 1
fi

set_smoke_config_values() {
    local file="$1"
    local mode="$2"
    local address="$3"
    local next_file="$file.dspcore-next"

    touch "$file"
    awk -v mode="$mode" -v address="$address" '
        function print_values() {
            print "EnableContentProjection = False"
            print "AutoNebulaMode = " mode
            print "AutoNebulaAddress = " address
            print "AutoNebulaActionDelaySeconds = 20"
            values_written = 1
        }
        BEGIN {
            target_section = "DSPCore.Smoke"
            in_target = 0
            duplicate_target = 0
            target_seen = 0
            values_written = 0
        }
        {
            line = $0
            sub(/\r$/, "", line)
        }
        line ~ /^\[/ {
            if (in_target && duplicate_target == 0 && values_written == 0) {
                print_values()
            }

            if (line == "[" target_section "]") {
                in_target = 1
                if (target_seen == 0) {
                    target_seen = 1
                    duplicate_target = 0
                    values_written = 0
                    print line
                } else {
                    duplicate_target = 1
                }
                next
            }

            in_target = 0
            duplicate_target = 0
            print line
            next
        }
        line ~ /^(EnableContentProjection|AutoNebulaMode|AutoNebulaAddress|AutoNebulaActionDelaySeconds)[[:space:]]*=/ {
            next
        }
        in_target && duplicate_target {
            next
        }
        {
            print line
        }
        END {
            if (target_seen == 0) {
                print ""
                print "[" target_section "]"
                print_values()
            } else if (in_target && duplicate_target == 0 && values_written == 0) {
                print_values()
            }
        }
    ' "$file" > "$next_file"
    mv "$next_file" "$file"
}

config="$profile/BepInEx/config/com.menglei.dsp.core.cfg"

"$repo_root/tests/install-smoke-mod.sh" "$profile" >/dev/null
set_smoke_config_values "$config" "$mode" "$address"
"$repo_root/tests/verify-nebula-profile-ready.sh" "$profile"

echo "Updated Nebula smoke profile: $profile"
echo "AutoNebulaMode = $mode"
echo "AutoNebulaAddress = $address"
