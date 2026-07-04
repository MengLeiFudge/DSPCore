# KeyBinds

The KeyBinds block lets a mod declare a default key binding, trigger timing, and callback. DSPCore polls the binding at runtime and invokes your logic. Ordinary bindings use `KeyBinds.Register(id, ownerModGuid, displayName, defaultKey, callback, ...)`; use `KeyBindDescriptor` when code needs to pass full descriptor objects around.

## What This Block Gives You

- You do not need every mod to write its own `Input.GetKeyDown` / `GetKey` / `GetKeyUp` polling.
- Default key parsing and simple `Ctrl` / `Alt` / `Shift` modifiers are centralized in DSPCore.
- Key bindings with `CanOverride=true` are injected into the vanilla `BuiltinKey` / `overrideKeys` data model and appear directly on the vanilla key-binding page.
- Player rebind, restore-default, and conflict hints reuse the vanilla `UIKeyEntry` behavior.
- Callback exceptions are reported through the ErrorWindow system instead of being silently swallowed or breaking the whole update flow.
- One descriptor model covers press, hold, and release triggers.

## Capability: Register Key Bindings

```csharp
KeyBinds.Register(
    id: "example-toggle",
    ownerModGuid: "com.example.my-mod",
    displayName: "ExampleToggle",
    defaultKey: "Ctrl+K",
    callback: TogglePanel,
    action: CoreKeyAction.Press,
    conflictGroup: 100,
    displayPageId: "com.example.settings");
```

`DefaultKey` uses Unity `KeyCode` names and may include simple `Ctrl`, `Alt`, or `Shift` modifiers. `Action` can be `Press`, `Hold`, or `Release`. `ConflictGroup` is written to the vanilla `BuiltinKey.conflictGroup`, so conflict checking belongs to the vanilla key-binding page. `displayPageId` remains as an old source-compatibility parameter and no longer controls player UI placement. Use `KeyBinds.Register(new KeyBindDescriptor(...))` when code needs to construct or cache the full declaration first.

## What DSPCore Does After The Call

- Registration stores the descriptor; if the same `Id` is registered more than once, the later registration replaces the earlier one.
- When `CanOverride=true`, DSPCore appends a `BuiltinKey` to the vanilla key list and expands `GameOption.overrideKeys` / `VFInput.override_keys`.
- The vanilla key-binding page creates the matching `UIKeyEntry`. Player overrides are saved to `BepInEx/config/DSPCore/keybinds.dat`, outside the vanilla options.xml 256-key export area.
- Each update, DSPCore prefers the player override from the vanilla override array; missing overrides fall back to `DefaultKey`.
- If required modifiers are not pressed, the callback does not trigger.
- Depending on `Action`, DSPCore calls `Input.GetKeyDown`, `Input.GetKey`, or `Input.GetKeyUp`.
- If the callback throws, DSPCore reports the error under `OwnerModGuid`.

## What This Block Does Not Own

- `ConflictGroup` is handled by the vanilla UI; DSPCore does not automatically rebind keys, disable callbacks, or choose mod priority.
- `displayPageId` no longer affects player UI; key bindings are displayed on the vanilla key-binding page.
- Keep callbacks small. Expensive scans or complex state machines should be handled by your mod's later update flow.
- Invalid player config logs one warning and falls back to the default key; if the default key is invalid too, the binding is skipped.

## Examples

- `Examples/KeyBindRegistration.md`
- `Examples/KeyBindRegistrationExample.cs`
