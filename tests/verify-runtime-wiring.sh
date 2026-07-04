#!/usr/bin/env bash
set -euo pipefail

repo_root="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"

require() {
    local pattern="$1"
    local file="$2"
    local message="$3"

    if ! grep -qF "$pattern" "$repo_root/$file"; then
        echo "$message" >&2
        echo "Missing pattern: $pattern" >&2
        echo "File: $file" >&2
        exit 1
    fi
}

require_regex() {
    local pattern="$1"
    local file="$2"
    local message="$3"

    if ! grep -Eq "$pattern" "$repo_root/$file"; then
        echo "$message" >&2
        echo "Missing regex: $pattern" >&2
        echo "File: $file" >&2
        exit 1
    fi
}

plugin="DSPCore/Systems/Lifecycle/DSPCorePlugin.cs"
require "OptionRuntime.Initialize(Config);" "$plugin" "DSPCorePlugin must bind options during Awake."
require "MultiplayerRuntime.Initialize();" "$plugin" "DSPCorePlugin must initialize multiplayer soft detection."
require "ErrorRuntime.Initialize();" "$plugin" "DSPCorePlugin must subscribe error logging."
require "BuildBarRuntime.Initialize();" "$plugin" "DSPCorePlugin must initialize build bar save handling."
require "EntityLifecycleRuntime.Initialize();" "$plugin" "DSPCorePlugin must initialize entity lifecycle saves."
require "PlanetLifecycleRuntime.Initialize();" "$plugin" "DSPCorePlugin must initialize planet lifecycle saves."
require "GalaxyLifecycleRuntime.Initialize();" "$plugin" "DSPCorePlugin must initialize galaxy lifecycle saves."
require "GlobalSaveRuntime.Initialize();" "$plugin" "DSPCorePlugin must initialize global saves."
require "LocalizationRuntime.Initialize();" "$plugin" "DSPCorePlugin must load locale overrides."
require "harmony.PatchAll(typeof(ProtoRegistrationRuntimePatches));" "$plugin" "Proto registration patches must be wired."
require "harmony.PatchAll(typeof(BuildBarRuntimePatches));" "$plugin" "Build bar runtime patches must be wired."
require "harmony.PatchAll(typeof(PickerRuntimePatches));" "$plugin" "Picker runtime patches must be wired."
require "harmony.PatchAll(typeof(OptionPageRuntimePatches));" "$plugin" "Options page patches must be wired."
require "harmony.PatchAll(typeof(KeyBindRuntimePatches));" "$plugin" "Key bind runtime patches must be wired."
require "harmony.PatchAll(typeof(EntityLifecycleRuntimePatches));" "$plugin" "Entity lifecycle patches must be wired."
require "harmony.PatchAll(typeof(PlanetLifecycleRuntimePatches));" "$plugin" "Planet lifecycle patches must be wired."
require "harmony.PatchAll(typeof(BuildingParameterRuntimePatches));" "$plugin" "Blueprint parameter patches must be wired."
require "harmony.PatchAll(typeof(GalaxyLifecycleRuntimePatches));" "$plugin" "Galaxy lifecycle patches must be wired."
require "PatchRuntime.ApplyRegisteredPatches(harmony);" "$plugin" "Author patch platform must be applied after Harmony is ready."

options_page="DSPCore/Systems/Options/OptionPageRuntime.cs"
require "private const string TabObjectName = \"dspcore-option-tab\";" "$options_page" "DSPCore options must add a tab inside the vanilla option window."
require "private const string PageObjectName = \"dspcore-option-page\";" "$options_page" "DSPCore options must render inside the vanilla option window."
require "[HarmonyPatch(typeof(UIOptionWindow), \"_OnCreate\")]" "$options_page" "DSPCore option page must be prepared with the vanilla option window."
require "[HarmonyPatch(typeof(UIOptionWindow), \"_OnOpen\")]" "$options_page" "DSPCore option page must be restored when the vanilla option window opens."
require "[HarmonyPatch(typeof(UIOptionWindow), \"OnTabButtonClick\")]" "$options_page" "Vanilla tab clicks must hide the DSPCore option page."
require "OptionPageRuntime.SaveOptions();" "$options_page" "DSPCore option page must apply pending values with the vanilla apply button."
require "OptionPageRuntime.DiscardOptions();" "$options_page" "DSPCore option page must discard pending values with the vanilla cancel button."
require "option.Kind != OptionValueKind.KeyBinding" "$options_page" "Key binds must be excluded from DSPCore option controls and handled by the vanilla key page."
require "UiComboBox.CreateComboBox" "$options_page" "Option page must render enum controls."
require "UiSlider.CreateSlider" "$options_page" "Option page must render range controls."
require "UiCheckBox.CreateCheckBox" "$options_page" "Option page must render bool controls."

keybind="DSPCore/Systems/KeyBindProjection/KeyBindRuntime.cs"
require "DSPGame.key.builtinKeys = keys.ToArray();" "$keybind" "Key binds must inject custom BuiltinKey entries for the vanilla key page."
require "private const int VanillaKeyLimit = 256;" "$keybind" "Key bind runtime must keep the vanilla key id boundary explicit."
require "private const int OverrideKeyCapacity = 512;" "$keybind" "Key bind runtime must extend override key storage."
require "SaveCustomKeys();" "$keybind" "Custom key overrides must be saved outside vanilla options.xml."
require "EnsureOptionWindowKeyEntries(UIOptionWindow window)" "$keybind" "Late key registrations must add missing vanilla key-page entries."
require "window.CreateKeyEntry(i, builtinKeys[i])" "$keybind" "Missing vanilla key-page entries must reuse the vanilla UIKeyEntry factory."
require "[HarmonyPatch(typeof(GameOption), nameof(GameOption.InitKeys))]" "$keybind" "Key bind runtime must extend GameOption key initialization."
require "[HarmonyPatch(typeof(GameOption), nameof(GameOption.ExportXML))]" "$keybind" "Key bind runtime must avoid exporting custom key ids into vanilla XML."

if [[ -e "$repo_root/DSPCore/Systems/Options/OptionsWindow.cs" ]]; then
    echo "DSPCore must not keep the old self-owned OptionsWindow." >&2
    exit 1
fi

if [[ -e "$repo_root/DSPCore/Systems/Options/GlobalSavesWindow.cs" ]]; then
    echo "GlobalSaves must not expose a player-facing window." >&2
    exit 1
fi

global_save="DSPCore/Systems/SaveBridge/GlobalSaveRuntime.cs"
require "public static GlobalSaveOverview CreateOverview()" "$global_save" "GlobalSaveRuntime must expose a read-only overview for author APIs and diagnostics."
require "GlobalSaveBlockSnapshot" "$global_save" "GlobalSaveRuntime overview must use block metadata snapshots."
require "Blocks.Clear();" "$global_save" "GlobalSaveRuntime must keep in-memory block metadata synchronized after save/load."

buildbar="DSPCore/Systems/QuickBarProjection/BuildBarRuntime.cs"
require "DspCore.Saves.Register(\"DSPCore.BuildBar\", new BuildBarSaveHandler(), CoreLoadOrder.Postload);" "$buildbar" "BuildBar player overrides must be saved through DSPCore saves."
require "ReassignBuildBar" "$buildbar" "BuildBar player rebinding must use the vanilla build bar interaction key."
require "ClearBuildBar" "$buildbar" "BuildBar player clearing must use the vanilla build bar interaction key."
require "Pickers.Open(new PickerRequest(" "$buildbar" "BuildBar hotkey rebinding must select items through the picker surface."
require "SetPlayerOverrideAndRefresh(slot, item.ID)" "$buildbar" "BuildBar hotkey selection must write player overrides."
require "SetPlayerOverrideAndRefresh(slot, 0)" "$buildbar" "BuildBar hotkey interaction must support explicit empty slots."
require "DspCore.BuildBar.ClearPlayerOverride(slot)" "$buildbar" "BuildBar runtime must support returning to author defaults."
require "DspCore.BuildBar.GetPlayerOverrides()" "$buildbar" "BuildBar runtime must apply player overrides."
require "DspCore.BuildBar.GetEffectiveBindings()" "$buildbar" "BuildBar runtime must use author defaults overlaid with player overrides."
require "EnsureExtendedRows(__instance);" "$buildbar" "BuildBar row > 1 UI buttons must be created when the build menu opens."
require "RefreshExtendedRows(__instance);" "$buildbar" "BuildBar row > 1 UI buttons must refresh on open/update."
require "[HarmonyPatch(typeof(UIBuildMenu), nameof(UIBuildMenu.StaticLoad))]" "$buildbar" "BuildBar must patch vanilla static loading."
require "[HarmonyPatch(typeof(UIBuildMenu), \"_OnOpen\")]" "$buildbar" "BuildBar must patch menu open."
require "[HarmonyPatch(typeof(UIBuildMenu), \"_OnUpdate\")]" "$buildbar" "BuildBar must patch menu update."
require "ImportRebindBuildBarConfig();" "$buildbar" "BuildBar must keep the one-time RebindBuildBar import path."
if [[ -e "$repo_root/DSPCore/Systems/QuickBarProjection/BuildBarOverrideWindow.cs" ]]; then
    echo "BuildBar must not keep the old self-owned override editor window." >&2
    exit 1
fi

picker="DSPCore/Systems/PickerSurfaces/PickerRuntime.cs"
for surface in UIItemPicker UIRecipePicker UISignalPicker UISignalTagPicker; do
    require "[HarmonyPatch(typeof($surface), \"RefreshIcons\")]" "$picker" "$surface must patch RefreshIcons."
    require "[HarmonyPatch(typeof($surface), \"SetMaterialProps\")]" "$picker" "$surface must patch SetMaterialProps."
    require "[HarmonyPatch(typeof($surface), \"TestMouseIndex\")]" "$picker" "$surface must patch TestMouseIndex."
    require "[HarmonyPatch(typeof($surface), \"_OnUpdate\")]" "$picker" "$surface must patch _OnUpdate."
done
require "[HarmonyPatch(typeof(UIItemPicker), \"_OnClose\")]" "$picker" "Item picker must clear active requests on close."
require "[HarmonyPatch(typeof(UIRecipePicker), \"_OnClose\")]" "$picker" "Recipe picker must clear active requests on close."
require "[HarmonyPatch(typeof(UISignalPicker), \"_OnClose\")]" "$picker" "Signal picker must clear active requests on close."
require "EnsureItemPickerCapacity(picker, layout.Metrics.Capacity" "$picker" "Item picker must expand backing arrays for dynamic layout."
require "EnsureRecipePickerCapacity(picker, layout.Metrics.Capacity" "$picker" "Recipe picker must expand backing arrays for dynamic layout."
require "EnsureSignalPickerCapacity(picker, layout.Metrics.Capacity" "$picker" "Signal picker must expand backing arrays for dynamic layout."
require "EnsureSignalTagPickerCapacity(picker, layout.Metrics.Capacity" "$picker" "Signal tag picker must expand backing arrays for dynamic layout."
require "PickerRuntime.ReplaceUpdateColumnConstants(instructions)" "$picker" "Picker update loops must use runtime column counts."
require "PickerRuntime.ReplaceSignalTagUpdateGridConstants(instructions)" "$picker" "Signal tag picker update must use runtime row and column counts."

error_runtime="DSPCore/Systems/ErrorWindow/ErrorRuntime.cs"
require "Application.logMessageReceivedThreaded += OnLogMessageReceivedThreaded;" "$error_runtime" "Error runtime must subscribe Unity log events."
require "EnsureButton(tip, \"DSPCoreCopyButton\", \"Copy\"" "$error_runtime" "Fatal window must expose a copy button."
require "EnsureButton(tip, \"DSPCoreCloseButton\", \"Close\"" "$error_runtime" "Fatal window must expose a close button."
require "GUIUtility.systemCopyBuffer = BuildFatalWindowDiagnosticText(tip)" "$error_runtime" "Fatal copy button must write diagnostic text to clipboard."
require "[HarmonyPatch(typeof(UIFatalErrorTip), nameof(UIFatalErrorTip.ShowError))]" "$error_runtime" "Fatal error window must be patched on ShowError."
require "[HarmonyPatch(typeof(UIFatalErrorTip), nameof(UIFatalErrorTip.ShowAssertionFailed))]" "$error_runtime" "Fatal error window must be patched on assertions."
require "[HarmonyPatch(typeof(UIFatalErrorTip), \"_OnOpen\")]" "$error_runtime" "Fatal error window must restore buttons when opened."

entity="DSPCore/Systems/EntityLifecycle/EntityLifecycleRuntime.cs"
require "DspCore.Saves.Register(SaveKey, new SaveHandler(), CoreLoadOrder.Postload);" "$entity" "Entity lifecycle state must be saved through DSPCore saves."
require_regex '\[HarmonyPatch\(typeof\(PlanetFactory\), nameof\(PlanetFactory\.CreateEntityLogicComponents\)\)\]' "$entity" "Entity components must attach after vanilla logic components are created."
require_regex '\[HarmonyPatch\(typeof\(PlanetFactory\), nameof\(PlanetFactory\.RemoveEntityWithComponents\)\)\]' "$entity" "Entity components must be removed before vanilla entity removal."
require_regex '\[HarmonyPatch\(typeof\(GameData\), nameof\(GameData\.GetOrCreateFactory\)\)\]' "$entity" "Entity pending state must apply when factories load."
require_regex '\[HarmonyPatch\(typeof\(PowerSystem\), nameof\(PowerSystem\.GameTick\)\)\]' "$entity" "Entity components must receive power ticks."
require_regex '\[HarmonyPatch\(typeof\(FactorySystem\), nameof\(FactorySystem\.GameTick\)\)\]' "$entity" "Entity components must receive factory ticks."
require_regex '\[HarmonyPatch\(typeof\(FactorySystem\), nameof\(FactorySystem\.GameTickLabOutputToNext\)\)\]' "$entity" "Entity components must receive post factory ticks."

planet="DSPCore/Systems/PlanetLifecycle/PlanetLifecycleRuntime.cs"
require "DspCore.Saves.Register(SaveKey, new SaveHandler(), CoreLoadOrder.Postload);" "$planet" "Planet systems must be saved through DSPCore saves."
require_regex '\[HarmonyPatch\(typeof\(GameData\), nameof\(GameData\.GetOrCreateFactory\)\)\]' "$planet" "Planet systems must initialize when factories load."
require "[HarmonyPatch(typeof(FactoryModel), \"OnCameraPostRender\")]" "$planet" "Planet systems must receive draw updates."
require_regex '\[HarmonyPatch\(typeof\(PowerSystem\), nameof\(PowerSystem\.GameTick\)\)\]' "$planet" "Planet systems must receive power ticks."
require_regex '\[HarmonyPatch\(typeof\(FactorySystem\), nameof\(FactorySystem\.GameTick\)\)\]' "$planet" "Planet systems must receive factory ticks."
require_regex '\[HarmonyPatch\(typeof\(FactorySystem\), nameof\(FactorySystem\.GameTickLabOutputToNext\)\)\]' "$planet" "Planet systems must receive post factory ticks."

galaxy="DSPCore/Systems/GalaxyLifecycle/GalaxyLifecycleRuntime.cs"
require "DspCore.Saves.Register(SaveKey, new SaveHandler(), CoreLoadOrder.Postload);" "$galaxy" "Galaxy systems must be saved through DSPCore saves."
require_regex '\[HarmonyPatch\(typeof\(GameData\), nameof\(GameData\.SetForNewGame\)\)\]' "$galaxy" "Galaxy systems must initialize for new games."
require_regex '\[HarmonyPatch\(typeof\(SpaceSector\), nameof\(SpaceSector\.GameTick\)\)\]' "$galaxy" "Galaxy systems must tick with the space sector."

blueprints="DSPCore/Systems/BlueprintParameters/BuildingParameterRuntime.cs"
require_regex '\[HarmonyPatch\(typeof\(BuildingParameters\), nameof\(BuildingParameters\.CopyFromFactoryObject\)\)\]' "$blueprints" "Blueprint blocks must be copied from factory objects."
require_regex '\[HarmonyPatch\(typeof\(BuildingParameters\), nameof\(BuildingParameters\.FromParamsArray\)\)\]' "$blueprints" "Blueprint blocks must survive parameter array import."
require_regex '\[HarmonyPatch\(typeof\(BuildingParameters\), nameof\(BuildingParameters\.ToParamsArray\)\)\]' "$blueprints" "Blueprint blocks must be appended to parameter array export."
require_regex '\[HarmonyPatch\(typeof\(BuildingParameters\), nameof\(BuildingParameters\.CanPasteToFactoryObject\)\)\]' "$blueprints" "Blueprint blocks must participate in paste validation."
require_regex '\[HarmonyPatch\(typeof\(BuildingParameters\), nameof\(BuildingParameters\.PasteToFactoryObject\)\)\]' "$blueprints" "Blueprint blocks must paste into factory objects."
require_regex '\[HarmonyPatch\(typeof\(BuildingParameters\), nameof\(BuildingParameters\.ApplyPrebuildParametersToEntity\)\)\]' "$blueprints" "Blueprint blocks must apply to prebuild-created entities."

model="DSPCore/Systems/ModelProjection/ModelRuntime.cs"
proto="DSPCore/Systems/ProtoPipeline/ProtoRegistrationRuntime.cs"
require 'RunCacheStep("Model descriptors", ModelRuntime.Apply);' "$proto" "Proto cache rebuild must apply model clone declarations inside an isolated cache step."
require 'RunCacheStep("PrefabDesc array", ModelRuntime.RebuildPrefabDescArray);' "$proto" "Proto cache rebuild must refresh prefab desc array after model clones inside an isolated cache step."
require "LDB.models.dataArray = list.ToArray();" "$model" "ModelRuntime must write cloned models into LDB.models."
require "PlanetFactory.InitPrefabDescArray();" "$model" "ModelRuntime must rebuild factory prefab descriptors."

save="DSPCore/Systems/SaveBridge/SaveRuntime.cs"
require_regex '\[HarmonyPatch\(typeof\(GameData\), nameof\(GameData\.NewGame\)\)\]' "$save" "Save bridge must handle new games."
require_regex '\[HarmonyPatch\(typeof\(GameSave\), nameof\(GameSave\.SaveCurrentGame\)\)\]' "$save" "Save bridge must export on manual save."
require_regex '\[HarmonyPatch\(typeof\(GameSave\), nameof\(GameSave\.AutoSave\)\)\]' "$save" "Save bridge must export on autosave."
require_regex '\[HarmonyPatch\(typeof\(GameSave\), nameof\(GameSave\.LoadCurrentGame\)\)\]' "$save" "Save bridge must import around load."
require "[HarmonyPatch(typeof(UIGameSaveEntry), \"DeleteSaveFile\")]" "$save" "Save bridge must delete sidecar saves with vanilla saves."
