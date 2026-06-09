# Options

The Options module lets mods declare simple options that DSPCore binds to the BepInEx `ConfigFile`. The public surface includes `String`, `Bool`, `Int`, and `Float` short entries, lower-level string registration, option page descriptors, and settings version descriptors.

## What This Module Provides

- Mods can declare options from authoring code without holding the DSPCore plugin instance.
- Simple options can use short entries such as `Options.Bool(...)` and `Options.Int(...)` to register and read in one call.
- DSPCore binds options registered before startup, and also binds options registered after startup.
- If the DSPCore runtime has not bound the config file yet, short entries return the descriptor default instead of an empty string.
- A settings UI can be generated from `OptionPageDescriptor` and `OptionDescriptor` values with `PageId`.
- Multiplayer or save compatibility checks can read `OptionVersionDescriptor` values.

## Capability: Short Register-And-Read Entries

```csharp
bool enabled = Options.Bool("Example", "Enabled", true, "Enable example feature.");
int rowCount = Options.Int("Example", "Rows", 2, "Build bar row count.");
float scale = Options.Float("Example", "Scale", 1.0f, "UI scale.");
string mode = Options.String("Example", "Mode", "Normal", "Example mode.");
```

These methods register the option first, then return the current value. Use them for ordinary toggles, numbers, and text settings.

## Boundaries

- The underlying BepInEx values are still strings. Boolean, integer, and floating-point read helpers exist, while enums are parsed by authors.
- This module does not create concrete settings pages directly.
- Config keys should stay stable so player configuration remains valid.

## Examples

- `Examples/OptionRegistration.md`
- `Examples/OptionRegistrationExample.cs`
