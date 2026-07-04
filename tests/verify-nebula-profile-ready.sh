#!/usr/bin/env bash
set -euo pipefail

profile="${1:-}"

if [[ -z "$profile" ]]; then
    echo "Usage: tests/verify-nebula-profile-ready.sh <BepInEx profile directory>" >&2
    exit 2
fi

if [[ ! -d "$profile/BepInEx" ]]; then
    echo "Profile does not contain BepInEx: $profile" >&2
    exit 1
fi

require_file() {
    local path="$1"
    local message="$2"

    if [[ ! -f "$path" ]]; then
        echo "$message" >&2
        echo "Missing file: $path" >&2
        exit 1
    fi
}

require_dir() {
    local path="$1"
    local message="$2"

    if [[ ! -d "$path" ]]; then
        echo "$message" >&2
        echo "Missing directory: $path" >&2
        exit 1
    fi
}

require_glob() {
    local pattern="$1"
    local message="$2"

    if ! compgen -G "$pattern" >/dev/null; then
        echo "$message" >&2
        echo "Missing pattern: $pattern" >&2
        exit 1
    fi
}

require_no_disabled_dlls() {
    local directory="$1"
    local message="$2"

    mapfile -t disabled_dlls < <(find "$directory" -maxdepth 1 -type f -name '*.dll.old' -printf '%f\n' | sort)
    if [[ "${#disabled_dlls[@]}" -ne 0 ]]; then
        echo "$message" >&2
        printf 'Disabled DLL: %s\n' "${disabled_dlls[@]}" >&2
        exit 1
    fi
}

nebula_mod_dir="$profile/BepInEx/plugins/nebula-NebulaMultiplayerMod"

require_file "$profile/BepInEx/plugins/DSPCore/DSPCore.dll" "DSPCore main plugin must be installed."
require_file "$profile/BepInEx/plugins/DSPCore/DSPCore.NebulaAdapter.dll" "DSPCore Nebula adapter must be installed."
require_file "$profile/BepInEx/plugins/DSPCore.SmokeMod/DSPCore.SmokeMod.dll" "DSPCore smoke plugin must be installed."
require_file "$profile/BepInEx/plugins/nebula-NebulaMultiplayerModApi/NebulaAPI.dll" "Nebula Multiplayer Mod API must be enabled."
require_dir "$nebula_mod_dir" "Nebula Multiplayer Mod plugin directory must exist."
require_no_disabled_dlls "$nebula_mod_dir" "Nebula Multiplayer Mod still has disabled DLLs; enable the mod before running room smoke tests."
require_glob "$nebula_mod_dir/Nebula*.dll" "Nebula Multiplayer Mod must be enabled, not only present as .old files."
