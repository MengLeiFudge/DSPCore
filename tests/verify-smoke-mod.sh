#!/usr/bin/env bash
set -euo pipefail

repo_root="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"

require() {
    local pattern="$1"
    local file="$2"
    local message="$3"

    if ! grep -qF -- "$pattern" "$repo_root/$file"; then
        echo "$message" >&2
        echo "Missing pattern: $pattern" >&2
        echo "File: $file" >&2
        exit 1
    fi
}

smoke_project="tests/DSPCore.SmokeMod/DSPCore.SmokeMod.csproj"
smoke_plugin="tests/DSPCore.SmokeMod/SmokePlugin.cs"
smoke_readme="tests/DSPCore.SmokeMod/README.md"
install_script="tests/install-smoke-mod.sh"
evidence_script="tests/verify-smoke-evidence.sh"
nebula_profile_script="tests/verify-nebula-profile-ready.sh"
nebula_evidence_script="tests/verify-nebula-smoke-evidence.sh"
nebula_prepare_script="tests/prepare-nebula-smoke-profiles.sh"
nebula_update_script="tests/update-nebula-smoke-profile.sh"
start_profile_script="tests/start-dsp-profile.sh"
packaging="DSPCore.Packaging/Program.cs"

require "<ProjectReference Include=\"..\\..\\DSPCore\\DSPCore.csproj\" Private=\"false\" />" "$smoke_project" "Smoke mod must compile against the local DSPCore project."
require "[BepInDependency(DSPCorePlugin.PluginGuid)]" "$smoke_plugin" "Smoke mod must declare a DSPCore dependency."
require "Options.RegisterPage(PageId, PluginGuid" "$smoke_plugin" "Smoke mod must register a visible options page."
require "Options.Bool(" "$smoke_plugin" "Smoke mod must cover bool option rendering."
require "Options.Enum(" "$smoke_plugin" "Smoke mod must cover enum option rendering."
require "Options.IntRange(" "$smoke_plugin" "Smoke mod must cover range option rendering."
require "KeyBinds.Register(" "$smoke_plugin" "Smoke mod must cover key binding registration."
require "Options.OpenWindow();" "$smoke_plugin" "Smoke mod keybind must open the DSPCore settings window."
require "Options.OpenGlobalSavesWindow();" "$smoke_plugin" "Smoke mod keybind must open the GlobalSaves viewer."
require "Saves.GlobalAuto<SmokeGlobalState>" "$smoke_plugin" "Smoke mod must create GlobalSaves data for the read-only viewer."
require "Errors.Report(" "$smoke_plugin" "Smoke mod must create a manual diagnostic report."
require "BuildBar.BindQuickBarWithResult(" "$smoke_plugin" "Smoke mod must cover build bar default binding."
require "BuildBar.OpenEditor();" "$smoke_plugin" "Smoke mod keybind must open the BuildBar editor."
require "Icons.BindToProto(" "$smoke_plugin" "Smoke mod must cover icon binding projection."
require "GameEnums.RegisterRecipeType(" "$smoke_plugin" "Smoke mod must cover custom recipe type projection."
require "DSPCore smoke content state" "$smoke_plugin" "Smoke mod must log content projection state after load."
require "AutoNebulaMode" "$smoke_plugin" "Smoke mod must expose opt-in Nebula room automation."
require "Nebula dedicated arguments" "$smoke_plugin" "Smoke mod must leave host creation to Nebula's dedicated startup path."
require "NebulaNetwork.Client" "$smoke_plugin" "Smoke mod must join Nebula hosts through the direct network client path."
require "JoinGame" "$smoke_plugin" "Smoke mod must be able to join a Nebula host through reflection."
require "DSPCore smoke auto Nebula client sends completed." "$smoke_plugin" "Smoke mod must trigger multiplayer smoke sends after auto join."
require "Multiplayer.RegisterPacket(" "$smoke_plugin" "Smoke mod must cover soft multiplayer packet handlers."
require "Multiplayer.RegisterHostRelay(" "$smoke_plugin" "Smoke mod must cover host relay declarations."
require "Multiplayer.RegisterPlanetData(" "$smoke_plugin" "Smoke mod must cover planet data request declarations."
require "Multiplayer.SendPacket(" "$smoke_plugin" "Smoke mod must expose a packet send trigger."
require "Multiplayer.SendHostRelay(" "$smoke_plugin" "Smoke mod must expose a host relay send trigger."
require "Multiplayer.RequestPlanetData(" "$smoke_plugin" "Smoke mod must expose a planet data request trigger."
require "Nebula 双端检查项" "$smoke_readme" "Smoke README must document the Nebula two-client verification path."
require "tests/install-smoke-mod.sh" "$smoke_readme" "Smoke README must document the install helper."
require "tests/prepare-nebula-smoke-profiles.sh" "$smoke_readme" "Smoke README must document the Nebula profile preparation helper."
require "tests/update-nebula-smoke-profile.sh" "$smoke_readme" "Smoke README must document the Nebula profile update helper."
require "tests/start-dsp-profile.sh" "$smoke_readme" "Smoke README must document the profile launch helper."
require "tests/verify-smoke-evidence.sh" "$smoke_readme" "Smoke README must document the log evidence checker."
require "DSPCore.SmokeMod.dll" "$smoke_readme" "Smoke README must document the plugin DLL."
require "DSPCore.SmokeMod.dll" "$install_script" "Install helper must copy the smoke plugin."
require "DSPCore.NebulaAdapter.dll" "$install_script" "Install helper must copy the optional Nebula adapter when built."
require "NebulaMultiplayerMod" "$nebula_profile_script" "Nebula profile checker must require the real Nebula mod."
require "require_no_disabled_dlls" "$nebula_profile_script" "Nebula profile checker must reject disabled dependency DLLs."
require "accepted=True, hasTransport=True" "$nebula_evidence_script" "Nebula evidence checker must require accepted sends on the client."
require "DSPCore smoke host handled relay" "$nebula_evidence_script" "Nebula evidence checker must validate host relay on host log."
require "refusing to overwrite" "$nebula_prepare_script" "Nebula profile preparer must avoid overwriting existing user profiles."
require "verify-nebula-profile-ready.sh" "$nebula_prepare_script" "Nebula profile preparer must run the readiness check after preparing profiles."
require "set_section_config_value" "$nebula_prepare_script" "Nebula profile preparer must write DSPCore smoke options inside the DSPCore.Smoke config section."
require "AutoNebulaMode" "$nebula_prepare_script" "Nebula profile preparer must configure automatic host/client smoke modes."
require "AutoNebulaAddress" "$nebula_prepare_script" "Nebula profile preparer must configure the automatic client join address."
require "set_smoke_config_values" "$nebula_update_script" "Nebula profile updater must normalize DSPCore smoke options inside the DSPCore.Smoke config section."
require "AutoNebulaMode" "$nebula_update_script" "Nebula profile updater must configure automatic smoke mode."
require "verify-nebula-profile-ready.sh" "$nebula_update_script" "Nebula profile updater must verify profile readiness after updating."
require "doorstop_config.ini.dspcore-smoke-backup" "$start_profile_script" "Profile launch helper must back up the game Doorstop config."
require "trap restore_config EXIT" "$start_profile_script" "Profile launch helper must restore the game Doorstop config."
require "-ArgumentList" "$start_profile_script" "Profile launch helper must support Nebula dedicated-server arguments."
require "ui-pages" "$evidence_script" "Evidence checker must validate deeper UI page open logs."
require "content" "$evidence_script" "Evidence checker must validate icon and recipe type smoke logs."
require "startup ui error offline-multiplayer" "$evidence_script" "Evidence checker must default to local smoke checks."
require "DSPCore smoke received packet" "$evidence_script" "Evidence checker must validate Nebula packet delivery."
require "DSPCore smoke imported planet data" "$evidence_script" "Evidence checker must validate Nebula planet data import."
require "auto-nebula-host" "$evidence_script" "Evidence checker must validate auto Nebula host logs."
require "auto-nebula-client" "$evidence_script" "Evidence checker must validate auto Nebula client logs."

if grep -qF -- "DSPCore.SmokeMod" "$repo_root/$packaging"; then
    echo "Smoke mod must not be included in Thunderstore packaging." >&2
    exit 1
fi
