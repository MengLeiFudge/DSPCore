#!/usr/bin/env bash
set -euo pipefail

profile="${1:-}"

if [[ -z "$profile" ]]; then
    echo "Usage: tests/start-dsp-profile.sh <BepInEx profile directory> [DSP game directory] [-- extra DSPGAME args]" >&2
    exit 2
fi

shift
game_dir="${DSPGameDir:-/mnt/d/Steam/steamapps/common/Dyson Sphere Program}"
if [[ "$#" -gt 0 && "${1:-}" != "--" ]]; then
    game_dir="$1"
    shift
fi
if [[ "${1:-}" == "--" ]]; then
    shift
fi
extra_args=("$@")

if [[ ! -f "$profile/BepInEx/core/BepInEx.Preloader.dll" ]]; then
    echo "Profile does not contain BepInEx.Preloader.dll: $profile" >&2
    exit 1
fi

if [[ ! -f "$game_dir/DSPGAME.exe" ]]; then
    echo "DSPGAME.exe not found in game directory: $game_dir" >&2
    exit 1
fi

doorstop_config="$game_dir/doorstop_config.ini"
backup_config="$game_dir/doorstop_config.ini.dspcore-smoke-backup"
next_config="$game_dir/doorstop_config.ini.dspcore-smoke-next"

if [[ ! -f "$doorstop_config" ]]; then
    echo "Doorstop config not found: $doorstop_config" >&2
    exit 1
fi

if [[ -e "$backup_config" ]]; then
    echo "Backup config already exists; refusing to continue: $backup_config" >&2
    exit 1
fi

if [[ -e "$next_config" ]]; then
    echo "Temporary config already exists; refusing to continue: $next_config" >&2
    exit 1
fi

restore_config() {
    if [[ -f "$next_config" ]]; then
        rm -f "$next_config"
    fi

    if [[ -f "$backup_config" ]]; then
        mv "$backup_config" "$doorstop_config"
    fi
}

trap restore_config EXIT

cp "$doorstop_config" "$backup_config"

profile_windows="$(wslpath -w "$profile/BepInEx/core/BepInEx.Preloader.dll" | tr '\\' '/')"
awk -v target="$profile_windows" '
    BEGIN { updated = 0 }
    /^targetAssembly=/ {
        print "targetAssembly=" target
        updated = 1
        next
    }
    { print }
    END {
        if (updated == 0) {
            print "targetAssembly=" target
        }
    }
' "$doorstop_config" > "$next_config"
mv "$next_config" "$doorstop_config"

if [[ "${#extra_args[@]}" -gt 0 ]]; then
    arguments_windows="${extra_args[*]}"
    powershell.exe -NoProfile -Command "Start-Process -FilePath '$(wslpath -w "$game_dir/DSPGAME.exe")' -ArgumentList '$arguments_windows'"
else
    powershell.exe -NoProfile -Command "Start-Process -FilePath '$(wslpath -w "$game_dir/DSPGAME.exe")'"
fi
sleep "${DSPCORE_START_PROFILE_RESTORE_DELAY:-5}"
