#!/usr/bin/env bash
set -euo pipefail

repo_root="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
profile_dir="${1:-${ProfileDir:-/mnt/c/Users/MLJ/AppData/Roaming/r2modmanPlus-local/DysonSphereProgram/profiles/Default}}"

copy_file() {
    local source="$1"
    local target="$2"

    if [[ ! -f "$source" ]]; then
        echo "Missing build output: $source" >&2
        echo "Run dotnet build DSPCore.sln with ProfileDir and DSPGameDir first." >&2
        exit 1
    fi

    mkdir -p "$(dirname "$target")"
    cp "$source" "$target"
    echo "$target"
}

if [[ ! -d "$profile_dir/BepInEx" ]]; then
    echo "ProfileDir does not contain BepInEx: $profile_dir" >&2
    exit 1
fi

copy_file "$repo_root/DSPCore/bin/Debug/DSPCore.dll" "$profile_dir/BepInEx/plugins/DSPCore/DSPCore.dll"
copy_file "$repo_root/DSPCore.Preloader/bin/Debug/DSPCore.Preloader.dll" "$profile_dir/BepInEx/patchers/DSPCore.Preloader.dll"
copy_file "$repo_root/tests/DSPCore.SmokeMod/bin/Debug/DSPCore.SmokeMod.dll" "$profile_dir/BepInEx/plugins/DSPCore.SmokeMod/DSPCore.SmokeMod.dll"

if [[ -f "$repo_root/DSPCore.NebulaAdapter/bin/Debug/DSPCore.NebulaAdapter.dll" ]]; then
    copy_file "$repo_root/DSPCore.NebulaAdapter/bin/Debug/DSPCore.NebulaAdapter.dll" "$profile_dir/BepInEx/plugins/DSPCore/DSPCore.NebulaAdapter.dll"
fi
