#!/usr/bin/env bash
set -euo pipefail

repo_root="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
source_profile="${1:-}"
host_profile="${2:-}"
client_profile="${3:-}"

if [[ -z "$source_profile" || -z "$host_profile" || -z "$client_profile" ]]; then
    echo "Usage: tests/prepare-nebula-smoke-profiles.sh <source-profile> <host-profile> <client-profile>" >&2
    exit 2
fi

if [[ ! -d "$source_profile/BepInEx" ]]; then
    echo "Source profile does not contain BepInEx: $source_profile" >&2
    exit 1
fi

copy_profile() {
    local target="$1"

    if [[ -e "$target" ]]; then
        echo "Target profile already exists; refusing to overwrite: $target" >&2
        exit 1
    fi

    mkdir -p "$(dirname "$target")"
    cp -a "$source_profile" "$target"
}

enable_old_files() {
    local directory="$1"

    if [[ ! -d "$directory" ]]; then
        return
    fi

    while IFS= read -r -d '' old_file; do
        local enabled_file="${old_file%.old}"
        if [[ -e "$enabled_file" ]]; then
            echo "Cannot enable $old_file because $enabled_file already exists." >&2
            exit 1
        fi

        mv "$old_file" "$enabled_file"
    done < <(find "$directory" -maxdepth 1 -type f -name '*.old' -print0)
}

set_config_value() {
    local file="$1"
    local key="$2"
    local value="$3"

    if [[ -f "$file" ]]; then
        if grep -qE "^${key}[[:space:]]*=" "$file"; then
            sed -i -E "s|^(${key}[[:space:]]*=[[:space:]]*).*|\\1${value}|" "$file"
        else
            printf '\n%s = %s\n' "$key" "$value" >> "$file"
        fi
    fi
}

set_section_config_value() {
    local file="$1"
    local section="$2"
    local key="$3"
    local value="$4"
    local next_file="$file.dspcore-next"

    touch "$file"
    awk -v section="$section" -v key="$key" -v value="$value" '
        BEGIN {
            in_section = 0
            section_seen = 0
            key_written = 0
        }
        $0 == "[" section "]" {
            in_section = 1
            section_seen = 1
            print
            next
        }
        /^\[/ {
            if (in_section && key_written == 0) {
                print key " = " value
                key_written = 1
            }
            in_section = 0
            print
            next
        }
        in_section && $0 ~ ("^" key "[[:space:]]*=") {
            if (key_written == 0) {
                print key " = " value
                key_written = 1
            }
            next
        }
        {
            print
        }
        END {
            if (section_seen == 0) {
                print ""
                print "[" section "]"
                print key " = " value
            } else if (in_section && key_written == 0) {
                print key " = " value
            }
        }
    ' "$file" > "$next_file"
    mv "$next_file" "$file"
}

prepare_profile() {
    local profile="$1"
    local nickname="$2"
    local auto_mode="$3"

    enable_old_files "$profile/BepInEx/plugins/nebula-NebulaMultiplayerMod"
    enable_old_files "$profile/BepInEx/plugins/PhantomGamers-IlLine"
    enable_old_files "$profile/BepInEx/plugins/starfi5h-NebulaCompatibilityAssist"

    set_config_value "$profile/BepInEx/config/nebula.cfg" "Nickname" "$nickname"
    set_config_value "$profile/BepInEx/config/nebula.cfg" "LastIP" "127.0.0.1:8469"
    set_config_value "$profile/BepInEx/config/nebula.cfg" "HostPort" "8469"
    set_section_config_value "$profile/BepInEx/config/com.menglei.dsp.core.cfg" "DSPCore.Smoke" "EnableContentProjection" "False"
    set_section_config_value "$profile/BepInEx/config/com.menglei.dsp.core.cfg" "DSPCore.Smoke" "AutoNebulaMode" "$auto_mode"
    set_section_config_value "$profile/BepInEx/config/com.menglei.dsp.core.cfg" "DSPCore.Smoke" "AutoNebulaAddress" "127.0.0.1:8469"
    set_section_config_value "$profile/BepInEx/config/com.menglei.dsp.core.cfg" "DSPCore.Smoke" "AutoNebulaActionDelaySeconds" "20"

    "$repo_root/tests/install-smoke-mod.sh" "$profile" >/dev/null
    "$repo_root/tests/verify-nebula-profile-ready.sh" "$profile"
}

copy_profile "$host_profile"
copy_profile "$client_profile"

prepare_profile "$host_profile" "DSPCoreSmokeHost" "Host"
prepare_profile "$client_profile" "DSPCoreSmokeClient" "Client"

echo "Nebula smoke host profile: $host_profile"
echo "Nebula smoke client profile: $client_profile"
