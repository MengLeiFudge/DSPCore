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
require "harmony.PatchAll(typeof(OptionWindowEntryRuntimePatches));" "$plugin" "Options entry patches must be wired."
require "harmony.PatchAll(typeof(EntityLifecycleRuntimePatches));" "$plugin" "Entity lifecycle patches must be wired."
require "harmony.PatchAll(typeof(PlanetLifecycleRuntimePatches));" "$plugin" "Planet lifecycle patches must be wired."
require "harmony.PatchAll(typeof(BuildingParameterRuntimePatches));" "$plugin" "Blueprint parameter patches must be wired."
require "harmony.PatchAll(typeof(GalaxyLifecycleRuntimePatches));" "$plugin" "Galaxy lifecycle patches must be wired."
require "PatchRuntime.ApplyRegisteredPatches(harmony);" "$plugin" "Author patch platform must be applied after Harmony is ready."

options_entry="DSPCore/Systems/Options/OptionWindowEntryRuntime.cs"
require "private const string ButtonName = \"dspcore-options-entry-button\";" "$options_entry" "Vanilla option window entry button name must stay stable."
require "private const string KeyButtonName = \"dspcore-key-options-entry-button\";" "$options_entry" "Vanilla key-binding page entry button name must stay stable."
require "OptionRuntime.OpenWindow();" "$options_entry" "Option entry button must open the DSPCore options window."
require "[HarmonyPatch(typeof(UIOptionWindow), \"_OnCreate\")]" "$options_entry" "DSPCore options entry must be created with the vanilla option window."
require "[HarmonyPatch(typeof(UIOptionWindow), \"_OnOpen\")]" "$options_entry" "DSPCore options entry must be restored when the vanilla option window opens."
require "SetButtonText(button, OptionText.EntryButton);" "$options_entry" "Main options entry must use localized text."
require "SetButtonText(button, OptionText.KeyEntryButton);" "$options_entry" "Key page entry must use localized text."

options_window="DSPCore/Systems/Options/OptionsWindow.cs"
require "GridDsl.Header(OptionText.Title, OptionText.Summary" "$options_window" "Options window must render its localized header."
require "GridDsl.ButtonNode(OptionText.GlobalSavesButton, OptionRuntime.OpenGlobalSavesWindow" "$options_window" "Options window must expose the read-only global saves viewer."
require "GridDsl.ButtonNode(BuildBarText.OpenEditor, BuildBarRuntime.OpenOverrideWindow" "$options_window" "Options window must expose the BuildBar editor entry."
require "GridDsl.ButtonNode(OptionText.Close, Close" "$options_window" "Options window must expose a close button."
require "AddResetButton(window, root, option" "$options_window" "Options window must render reset controls for resettable options."
require "StartKeyCapture(option, input, descriptionText)" "$options_window" "Options window must expose key capture controls."
require "KeyBindRuntime.TryCaptureCurrentKey(out var keyText)" "$options_window" "Key capture must read real key input through KeyBindRuntime."
require "KeyBinds.BuildOptionDescription(option)" "$options_window" "Key binding rows must show conflict-aware descriptions."

global_saves_window="DSPCore/Systems/Options/GlobalSavesWindow.cs"
require "GlobalSaveRuntime.CreateOverview();" "$global_saves_window" "Global save viewer must read the runtime snapshot."
require "GridDsl.Header(OptionText.GlobalSavesTitle, OptionText.GlobalSavesSummary" "$global_saves_window" "Global save viewer must render a localized header."
require "OptionText.GlobalSavesFooter" "$global_saves_window" "Global save viewer must state that data is read-only."
require "GlobalSaveBlockSnapshot" "$global_saves_window" "Global save viewer must render block snapshots rather than raw save bytes."
require "GetStatusText(GlobalSaveBlockSnapshot block)" "$global_saves_window" "Global save viewer must distinguish registered, initialized, and file-only blocks."

global_save="DSPCore/Systems/SaveBridge/GlobalSaveRuntime.cs"
require "public static GlobalSaveOverview CreateOverview()" "$global_save" "GlobalSaveRuntime must expose a read-only overview for the UI."
require "GlobalSaveBlockSnapshot" "$global_save" "GlobalSaveRuntime overview must use block metadata snapshots."
require "Blocks.Clear();" "$global_save" "GlobalSaveRuntime must keep in-memory block metadata synchronized after save/load."

buildbar="DSPCore/Systems/QuickBarProjection/BuildBarRuntime.cs"
require "DspCore.Saves.Register(\"DSPCore.BuildBar\", new BuildBarSaveHandler(), CoreLoadOrder.Postload);" "$buildbar" "BuildBar player overrides must be saved through DSPCore saves."
require "BuildBarOverrideWindow? overrideWindow" "$buildbar" "BuildBar runtime must retain the player override editor window."
require "public static void OpenOverrideWindow()" "$buildbar" "BuildBar runtime must expose an editor open entry."
require "Pickers.Open(new PickerRequest(" "$buildbar" "BuildBar editor must select items through the picker surface."
require "SetPlayerOverrideAndRefresh(slot, item.ID)" "$buildbar" "BuildBar editor selection must write player overrides."
require "SetPlayerOverrideAndRefresh(slot, 0)" "$buildbar" "BuildBar editor must support explicit empty slots."
require "DspCore.BuildBar.ClearPlayerOverride(slot)" "$buildbar" "BuildBar editor must support returning to author defaults."
require "DspCore.BuildBar.GetPlayerOverrides()" "$buildbar" "BuildBar runtime must apply player overrides."
require "DspCore.BuildBar.GetEffectiveBindings()" "$buildbar" "BuildBar runtime must use author defaults overlaid with player overrides."
require "EnsureExtendedRows(__instance);" "$buildbar" "BuildBar row > 1 UI buttons must be created when the build menu opens."
require "RefreshExtendedRows(__instance);" "$buildbar" "BuildBar row > 1 UI buttons must refresh on open/update."
require "[HarmonyPatch(typeof(UIBuildMenu), nameof(UIBuildMenu.StaticLoad))]" "$buildbar" "BuildBar must patch vanilla static loading."
require "[HarmonyPatch(typeof(UIBuildMenu), \"_OnOpen\")]" "$buildbar" "BuildBar must patch menu open."
require "[HarmonyPatch(typeof(UIBuildMenu), \"_OnUpdate\")]" "$buildbar" "BuildBar must patch menu update."
require "ImportRebindBuildBarConfig();" "$buildbar" "BuildBar must keep the one-time RebindBuildBar import path."

buildbar_editor="DSPCore/Systems/QuickBarProjection/BuildBarOverrideWindow.cs"
require "GridDsl.Header(BuildBarText.Title, BuildBarText.Summary" "$buildbar_editor" "BuildBar editor must render a localized header."
require "BuildBarRuntime.OpenSlotEditor(selectedSlot)" "$buildbar_editor" "BuildBar editor must expose item selection for the selected slot."
require "BuildBarRuntime.ClearSlot(selectedSlot)" "$buildbar_editor" "BuildBar editor must expose explicit empty overrides."
require "BuildBarRuntime.UseDefault(selectedSlot)" "$buildbar_editor" "BuildBar editor must expose default restoration."
require "DspCore.BuildBar.GetPlayerOverrides()" "$buildbar_editor" "BuildBar editor must show player override state."
require "DspCore.BuildBar.GetAll()" "$buildbar_editor" "BuildBar editor must show author default bindings."
require "GetEditableRowCount()" "$buildbar_editor" "BuildBar editor must expose every row used by author defaults or player overrides."

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
