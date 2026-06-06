# Tab Slots

The Tabs block lets a mod declare custom pages and receive a `TabSlot` that can be used when assigning item or recipe `GridIndex` values. DSPCore projects those pages to currently supported vanilla UI surfaces.

## What This Block Gives You

- You do not need to allocate custom tab values yourself or coordinate page-number conflicts across mods.
- You do not need to clone vanilla category buttons, calculate button positions, or hook each window creation flow yourself.
- Multiple mods can send tab declarations to one `TabRegistry`; DSPCore keeps one stable `TabSlot` for each stable `Id`.
- Tab icons can reference Icons by icon id, and tab titles can use DSP localization keys.
- DSPCore routes tab clicks back through the vanilla `OnTypeButtonClick` flow so button state and vanilla category behavior stay aligned.

## Capability: Declare Custom Pages

```csharp
TabSlot machinesTab = Tabs.AddTab(new CoreTabDescriptor(
    Id: "example-machines",
    OwnerModGuid: "com.example.my-mod",
    Title: "ExampleMachines",
    IconId: "example-machines-icon",
    Order: 100));
```

Keep `Id` stable. `OwnerModGuid` identifies ownership. `Title` is normally a localization key. `IconId` points to an icon registered in Icons. `Order` expresses button display ordering. The returned `TabSlot` is a page slot, not an item or recipe `GridIndex`.

## Capability: Put Items Or Recipe Proto Objects Into Page Cells

`GridIndex` is the native game cell field on `ItemProto` and `RecipeProto`. When creating an item or recipe, generate the `GridIndex` from a `TabSlot`, row, and cell index:

```csharp
itemProto.GridIndex = ProtoRegistration.GetGridIndex(machinesTab, row: 1, index: 5);
recipeProto.GridIndex = ProtoRegistration.GetGridIndex(machinesTab, row: 1, index: 5);
```

If you do not register a new page, you can still generate a `GridIndex` from one of the game's vanilla tab category values:

```csharp
itemProto.GridIndex = ProtoRegistration.GetGridIndex(tab: 1, row: 2, index: 3);
```

## What DSPCore Does After The Call

- Registration stores descriptors by `Id`; if the same `Id` is registered more than once, the later declaration replaces the earlier one but keeps the previously assigned `TabSlot`.
- New ids receive the next DSPCore custom `TabSlot`.
- When creating buttons, DSPCore orders tabs by `Order` and then `Id`; display order does not change the assigned `TabSlot` value.
- When `UIItemPicker`, `UIRecipePicker`, `UIReplicatorWindow`, `UISignalPicker`, or `UISignalTagPicker` is created, DSPCore clones the vanilla type button and creates one extra button for each declaration.
- The button title uses `.Translate()`. If `IconId` resolves through Icons, DSPCore applies the resolved sprite to the button image.
- When a custom tab is clicked, DSPCore calls the window's vanilla `OnTypeButtonClick` and refreshes button interactability when selection changes.

## What This Block Does Not Own

- It does not register items or recipes directly. Items and recipes still go through ProtoRegistration and use their own `GridIndex` to point at a cell inside a page.
- It does not support every DSP UI surface. Current coverage is vanilla item picker, recipe picker, replicator window, signal picker, and tag-icon picker.
- Blueprint icons, description icons, smart-input icons, and other vanilla surfaces that reuse signal/tag pickers benefit from this. When GenesisBook, OrbitalRing, or FE takes over signal/tag pickers, DSPCore skips its own button injection to avoid duplicate tabs. Truly rebuilt third-party picker surfaces that do not reuse vanilla pickers still need dedicated adapters.
- It does not create icons or localization; use Icons and Resources for those.

## Examples

- `Examples/TabRegistration.md`
- `Examples/TabRegistrationExample.cs`
