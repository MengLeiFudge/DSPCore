# KeyBinds

The KeyBinds block lets a mod declare a default key binding, trigger timing, and callback. DSPCore polls the binding at runtime and invokes your logic.

## What This Block Gives You

- You do not need every mod to write its own `Input.GetKeyDown` / `GetKey` / `GetKeyUp` polling.
- Default key parsing and simple `Ctrl` / `Alt` / `Shift` modifiers are centralized in DSPCore.
- Key bindings with `CanOverride=true` are automatically added to the DSPCore unified settings window, where players can edit the key text.
- When bindings in the same `ConflictGroup` resolve to the same key text, the unified settings window shows the conflicting bindings.
- Callback exceptions are reported through the ErrorWindow system instead of being silently swallowed or breaking the whole update flow.
- One descriptor model covers press, hold, and release triggers.

## Capability: Register Key Bindings

```csharp
KeyBinds.Register(new KeyBindDescriptor(
    Id: "example-toggle",
    OwnerModGuid: "com.example.my-mod",
    DisplayName: "ExampleToggle",
    DefaultKey: "Ctrl+K",
    Action: CoreKeyAction.Press,
    ConflictGroup: 100,
    Callback: TogglePanel));
```

`DefaultKey` uses Unity `KeyCode` names and may include simple `Ctrl`, `Alt`, or `Shift` modifiers. `Action` can be `Press`, `Hold`, or `Release`. `ConflictGroup` value 0 opts out of conflict checks; non-zero groups are checked in the unified settings window after key text normalization.

## What DSPCore Does After The Call

- Registration stores the descriptor; if the same `Id` is registered more than once, the later registration replaces the earlier one.
- When `CanOverride=true`, DSPCore registers the key binding as an option row in the unified settings window.
- Each update, DSPCore prefers the player-configured key text; empty or invalid config falls back to `DefaultKey`.
- When the unified settings window opens, DSPCore scans valid key texts in the same `ConflictGroup`; if the current binding matches another declaration, its option row shows the conflict target.
- If required modifiers are not pressed, the callback does not trigger.
- Depending on `Action`, DSPCore calls `Input.GetKeyDown`, `Input.GetKey`, or `Input.GetKeyUp`.
- If the callback throws, DSPCore reports the error under `OwnerModGuid`.

## What This Block Does Not Own

- The current rebinding entry is text input; it does not capture "press any key" interactions yet.
- `ConflictGroup` only reports same-key conflicts in the same group; it does not automatically rebind keys, disable callbacks, or choose mod priority.
- Keep callbacks small. Expensive scans or complex state machines should be handled by your mod's later update flow.
- Invalid player config logs one warning and falls back to the default key; if the default key is invalid too, the binding is skipped.

## Examples

- `Examples/KeyBindRegistration.md`
- `Examples/KeyBindRegistrationExample.cs`
