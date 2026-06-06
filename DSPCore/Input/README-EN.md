# Input

The Input block lets a mod declare a default key binding, trigger timing, and callback. DSPCore polls the binding at runtime and invokes your logic.

## What This Block Gives You

- You do not need every mod to write its own `Input.GetKeyDown` / `GetKey` / `GetKeyUp` polling.
- Default key parsing and simple `Ctrl` / `Alt` / `Shift` modifiers are centralized in DSPCore.
- Callback exceptions are reported to Errors instead of being silently swallowed or breaking the whole update flow.
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

`DefaultKey` uses Unity `KeyCode` names and may include simple `Ctrl`, `Alt`, or `Shift` modifiers. `Action` can be `Press`, `Hold`, or `Release`. `ConflictGroup` is currently a numeric descriptor field.

## What DSPCore Does After The Call

- Registration stores the descriptor; if the same `Id` is registered more than once, the later registration replaces the earlier one.
- Each update, DSPCore parses and caches `DefaultKey`.
- If required modifiers are not pressed, the callback does not trigger.
- Depending on `Action`, DSPCore calls `Input.GetKeyDown`, `Input.GetKey`, or `Input.GetKeyUp`.
- If the callback throws, DSPCore reports the error under `OwnerModGuid`.

## What This Block Does Not Own

- Full player rebinding UI is not implemented yet.
- `ConflictGroup` is currently descriptor data only; it does not provide conflict UI or automatic conflict resolution yet.
- Keep callbacks small. Expensive scans or complex state machines should be handled by your mod's later update flow.
- Invalid `DefaultKey` values log a warning and skip that binding.

## Examples

- `Examples/KeyBindRegistration.md`
- `Examples/KeyBindRegistrationExample.cs`
