# Tabs

The Tabs block lets a mod declare custom tab buttons and lets DSPCore project them to currently supported vanilla UI surfaces.

## What This Block Gives You

- You do not need to clone vanilla category buttons, calculate button positions, or hook each window creation flow yourself.
- Multiple mods can send tab declarations to one `TabRegistry`, reducing UI overwrite conflicts between separate patches.
- Tab icons can reference Icons by icon id, and tab titles can use DSP localization keys.
- DSPCore routes tab clicks back through the vanilla `OnTypeButtonClick` flow so button state and vanilla category behavior stay aligned.

## Capability: Declare Custom Tabs

```csharp
Tabs.Add(new CoreTabDescriptor(
    Id: "example-machines",
    OwnerModGuid: "com.example.my-mod",
    Title: "ExampleMachines",
    IconId: "example-machines-icon",
    Order: 100));
```

Keep `Id` stable. `OwnerModGuid` identifies ownership. `Title` is normally a localization key. `IconId` points to an icon registered in Icons. `Order` expresses intended ordering.

## What DSPCore Does After The Call

- Registration stores descriptors by `Id`; if the same `Id` is registered more than once, the later declaration replaces the earlier one.
- When creating buttons, DSPCore orders tabs by `Order` and then `Id`.
- When `UIItemPicker`, `UIRecipePicker`, or `UIReplicatorWindow` is created, DSPCore clones the vanilla type button and creates one extra button for each declaration.
- The button title uses `.Translate()`. If `IconId` resolves through Icons, DSPCore applies the resolved sprite to the button image.
- When a custom tab is clicked, DSPCore calls the window's vanilla `OnTypeButtonClick` and refreshes button interactability when selection changes.

## What This Block Does Not Own

- It does not define the tab content set; current runtime only projects tab buttons into the vanilla category flow.
- It does not support every DSP UI surface. Current coverage is item picker, recipe picker, and replicator window.
- Signal picker, beacon, blueprint, and other surfaces need a richer tab-content model before they can be supported.
- It does not create icons or localization; use Icons and Resources for those.

## Examples

- `Examples/TabRegistration.md`
- `Examples/TabRegistrationExample.cs`
