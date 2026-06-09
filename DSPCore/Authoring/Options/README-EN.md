# Options

The Options module lets mods declare simple options that DSPCore binds to the BepInEx `ConfigFile`, and provides a DSPCore-owned unified settings window. The public surface includes `String`, `Bool`, `Int`, `Float`, `Enum`, `IntRange`, and `FloatRange` short entries, lower-level string registration, option page descriptors, settings version descriptors, and `Options.OpenWindow()`.

## What This Module Provides

- Mods can declare options from authoring code without holding the DSPCore plugin instance.
- Simple options can use short entries such as `Options.Bool(...)`, `Options.IntRange(...)`, and `Options.Enum(...)` to register and read in one call.
- DSPCore binds options registered before startup, and also binds options registered after startup.
- If the DSPCore runtime has not bound the config file yet, short entries return the descriptor default instead of an empty string.
- The DSPCore unified settings window groups options by `OptionPageDescriptor` and `OptionDescriptor.PageId`.
- Multiplayer or save compatibility checks can read `OptionVersionDescriptor` values.

## Capability: Short Register-And-Read Entries

```csharp
bool enabled = Options.Bool("Example", "Enabled", true, "Enable example feature.");
int rowCount = Options.Int("Example", "Rows", 2, "Build bar row count.");
float scale = Options.Float("Example", "Scale", 1.0f, "UI scale.");
string mode = Options.String("Example", "Mode", "Normal", "Example mode.");
ExampleMode displayMode = Options.Enum("Example", "DisplayMode", ExampleMode.Normal, "Example display mode.");
int maxRows = Options.IntRange("Example", "MaxRows", 3, "Maximum rows.", minimum: 1, maximum: 6);
float opacity = Options.FloatRange("Example", "Opacity", 0.8f, "Panel opacity.", minimum: 0.2f, maximum: 1.0f, step: 0.05f);
```

These methods register the option first, then return the current value. Use them for ordinary toggles, numbers, text settings, enum dropdowns, and range sliders.

## Capability: Open The Unified Settings Window

```csharp
Options.OpenWindow();
```

The unified settings window displays all registered options. `Bool` uses a checkbox, `Enum` uses a dropdown, `IntRange` / `FloatRange` use sliders, and `String`, `Int`, `Float`, and key bindings use input fields that write back to the DSPCore BepInEx config when editing ends. The window must be opened after `UIRoot` is initialized, usually from a mod button, key bind, or custom UI callback.

## Boundaries

- The underlying BepInEx values are still strings. Boolean, integer, floating-point, and enum read helpers exist.
- The current window is DSPCore-owned and does not inject into the vanilla option page.
- Invalid `Int` / `Float` / key binding input is reverted to the current config value; range sliders write back according to their minimum, maximum, and step values.
- Config keys should stay stable so player configuration remains valid.

## Examples

- `Examples/OptionRegistration.md`
- `Examples/OptionRegistrationExample.cs`
