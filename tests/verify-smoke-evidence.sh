#!/usr/bin/env bash
set -euo pipefail

default_log="/mnt/c/Users/MLJ/AppData/Roaming/r2modmanPlus-local/DysonSphereProgram/profiles/Default/BepInEx/LogOutput.log"
log_file="${1:-${SmokeLog:-$default_log}}"
shift || true

if [[ ! -f "$log_file" ]]; then
    echo "Smoke log does not exist: $log_file" >&2
    exit 1
fi

if [[ "$#" -eq 0 ]]; then
    set -- startup ui error offline-multiplayer
fi

require() {
    local pattern="$1"
    local message="$2"

    if ! grep -qF "$pattern" "$log_file"; then
        echo "$message" >&2
        echo "Missing pattern: $pattern" >&2
        echo "Log: $log_file" >&2
        exit 1
    fi
}

require_accepted_false() {
    local prefix="$1"
    local message="$2"

    if ! grep -Eq "$prefix accepted=False, hasTransport=(True|False)" "$log_file"; then
        echo "$message" >&2
        echo "Missing pattern: $prefix accepted=False, hasTransport=(True|False)" >&2
        echo "Log: $log_file" >&2
        exit 1
    fi
}

for check in "$@"; do
    case "$check" in
        startup)
            require "DSPCore runtime bridges are initialized." "DSPCore must initialize runtime bridges."
            require "DSPCore smoke declarations registered." "Smoke mod must register declarations."
            ;;
        ui)
            require "DSPCore smoke opened settings from key bind." "Ctrl+F8 must open the DSPCore settings window."
            require "DSPCore smoke window state settings: exists=True, activeSelf=True, activeInHierarchy=True" "DSPCore settings window must exist and be active after Ctrl+F8."
            ;;
        ui-pages)
            require "DSPCore smoke opened global saves from key bind." "Ctrl+F6 must open the DSPCore GlobalSaves window."
            require "DSPCore smoke window state global saves: exists=True, activeSelf=True, activeInHierarchy=True" "DSPCore GlobalSaves window must exist and be active after Ctrl+F6."
            require "DSPCore smoke opened build bar editor from key bind." "Ctrl+F7 must open the DSPCore BuildBar editor."
            require "DSPCore smoke window state build bar editor: exists=True, activeSelf=True, activeInHierarchy=True" "DSPCore BuildBar editor must exist and be active after Ctrl+F7."
            require "DSPCore smoke opened settings from key bind." "Ctrl+F8 must open the DSPCore settings window."
            require "DSPCore smoke window state settings: exists=True, activeSelf=True, activeInHierarchy=True" "DSPCore settings window must exist and be active after Ctrl+F8."
            ;;
        content)
            require "DSPCore smoke content state: iconApplied=True" "Smoke icon binding must apply a sprite to the target item."
            require "recipeMapped=True" "Smoke recipe type must map the target vanilla recipe."
            require "recipeType=com.menglei.dsp.core.smoke.recipe-type" "Smoke recipe type id must be visible in the runtime mapping."
            require "recipeTypeValue=20" "Smoke recipe must be marked as the DSPCore custom recipe type."
            ;;
        error)
            require "DSPCore smoke reported a manual diagnostic entry." "Ctrl+F9 must report a smoke diagnostic entry."
            ;;
        offline-multiplayer)
            require_accepted_false "DSPCore smoke packet send" "Offline packet send must fail cleanly outside a multiplayer session."
            require_accepted_false "DSPCore smoke host relay send" "Offline host relay send must fail cleanly outside a multiplayer session."
            require_accepted_false "DSPCore smoke planet data request" "Offline planet data request must fail cleanly outside a multiplayer session."
            ;;
        nebula)
            require "DSPCore Nebula adapter is initialized." "Nebula adapter must initialize."
            require "DSPCore smoke received packet" "Nebula packet route must deliver a smoke packet."
            require "DSPCore smoke host handled relay" "Nebula host relay must reach the host handler."
            require "DSPCore smoke exported planet data" "Nebula planet data request must export host data."
            require "DSPCore smoke imported planet data" "Nebula planet data response must import client data."
            ;;
        auto-nebula-host)
            require "DSPCore smoke auto Nebula host mode is ready." "Auto Nebula host profile must reach the host-ready smoke path."
            require "DSPCore Nebula adapter is initialized." "Nebula adapter must initialize on the host."
            ;;
        auto-nebula-client)
            require "DSPCore smoke auto Nebula client joining" "Auto Nebula client mode must attempt to join the configured host."
            require "DSPCore smoke scheduled auto Nebula client sends after load." "Auto Nebula client mode must schedule smoke sends after game load."
            require "DSPCore smoke auto Nebula client sends completed." "Auto Nebula client mode must trigger packet, host relay, and planet data sends."
            ;;
        *)
            echo "Unknown smoke evidence check: $check" >&2
            echo "Known checks: startup ui ui-pages content error offline-multiplayer nebula auto-nebula-host auto-nebula-client" >&2
            exit 1
            ;;
    esac
done
