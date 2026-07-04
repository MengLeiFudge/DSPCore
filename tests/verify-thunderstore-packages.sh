#!/usr/bin/env bash
set -euo pipefail

repo_root="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
version="$(grep -oP '"version_number"\s*:\s*"\K[^"]+' "$repo_root/manifest.json")"
core_zip="$repo_root/artifacts/thunderstore/MengLei-DSPCore-$version.zip"
adapter_zip="$repo_root/artifacts/thunderstore/MengLei-DSPCore_NebulaAdapter-$version.zip"
adapter_manifest="$repo_root/artifacts/thunderstore/DSPCore_NebulaAdapter/manifest.json"

if [[ ! -f "$core_zip" ]]; then
    echo "Missing core package zip: $core_zip" >&2
    exit 1
fi

if [[ ! -f "$adapter_zip" ]]; then
    echo "Missing Nebula adapter package zip: $adapter_zip" >&2
    exit 1
fi

core_listing="$(unzip -Z1 "$core_zip")"
adapter_listing="$(unzip -Z1 "$adapter_zip")"

grep -qx "plugins/DSPCore/DSPCore.dll" <<<"$core_listing"
grep -qx "patchers/DSPCore.Preloader.dll" <<<"$core_listing"
if grep -qx "plugins/DSPCore/DSPCore.NebulaAdapter.dll" <<<"$core_listing"; then
    echo "Core package must not include DSPCore.NebulaAdapter.dll." >&2
    exit 1
fi

grep -qx "plugins/DSPCore/DSPCore.NebulaAdapter.dll" <<<"$adapter_listing"
grep -q "\"MengLei-DSPCore-$version\"" "$adapter_manifest"
grep -q "\"nebula-NebulaMultiplayerModApi-2.1.0\"" "$adapter_manifest"
