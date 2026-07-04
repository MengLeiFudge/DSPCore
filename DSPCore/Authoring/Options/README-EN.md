# Options

The Options module lets mods declare simple options that DSPCore binds to the BepInEx `ConfigFile`, and provides a DSPCore-owned unified settings window. The public surface includes the `Options.Page(...).Section(...)` page context entry, `String`, `Bool`, `Int`, `Float`, `Enum`, `IntRange`, and `FloatRange` short entries, lower-level string registration, option page descriptors, settings version descriptors, option import/export, and `Options.OpenWindow()`.

## What This Module Provides

- Mods can declare options from authoring code without holding the DSPCore plugin instance.
- Simple options can use short entries such as `Options.Bool(...)`, `Options.IntRange(...)`, and `Options.Enum(...)` to register and read in one call. When several rows belong to one settings page, use `Options.Page(...).Section(...)` first to fix the page and section, then call the same short entries on `OptionSection`.
- When display names, order, or reset buttons are needed, pass `OptionUi` to the same method name. Page contexts carry `PageId` automatically.
- DSPCore binds options registered before startup, and also binds options registered after startup.
- If the DSPCore runtime has not bound the config file yet, short entries return the descriptor default instead of an empty string.
- The DSPCore unified settings window groups options by `OptionPageDescriptor` and `OptionDescriptor.PageId`, and includes DSPCore's own settings page.
- Players can use DSPCore's `Key Bind Display` option to show rebindable keys on mod settings pages, on the unified key-bind page, or in both places. Multiple visible rows still write the same config entry.
- Multiplayer or save compatibility checks can read `OptionVersionDescriptor` values.
- When settings need to be copied, stored, or passed across systems, export a structured snapshot or text snapshot and import it back into currently registered options.

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

When several rows belong to the same settings page, prefer a page context so each row does not repeat `pageId` and `section`:

```csharp
OptionSection settings = Options
    .Page("com.example.settings", "com.example.my-mod", "Example Settings")
    .Section("Example");

bool enabled = settings.Bool("Enabled", true, "Enable example feature.");
int maxRows = settings.IntRange("MaxRows", 3, "Maximum rows.", minimum: 1, maximum: 6);
```

If one page occasionally needs rows from different config sections, call the same methods directly on `OptionPage` and pass `section` explicitly.

When the row needs presentation metadata, keep the same short entry and pass `OptionUi`:

```csharp
bool enabled = Options.Bool(
    "Example",
    "Enabled",
    true,
    "Enable example feature.",
    new OptionUi(PageId: "com.example.settings", DisplayName: "Enable Example")
    {
        Order = 10,
        CanReset = true
    });
```

`OptionUi.PageId` controls grouping in the unified settings window. When `OptionPage` or `OptionSection` is used, the context overrides the page id. `OptionUi.DisplayName` controls the row title shown to players. `OptionUi.Order` controls order within the page. `OptionUi.CanReset` controls whether a Reset button is shown. Use the shortest overload when those are not needed.

## Capability: Import And Export Options

```csharp
string text = Options.ExportText();
OptionImportReport report = Options.ImportText(text);
```

`ExportValues()` returns `OptionValueSnapshot` entries for direct system consumption. `ExportText()` creates copyable and storable text. `ImportText(...)` only writes to options that are currently registered and bound to the BepInEx config. Unknown keys, invalid-format lines, and values that cannot be written are reported through `OptionImportReport.SkippedKeys`.

## Capability: Open The Unified Settings Window

```csharp
Options.OpenWindow();
Options.OpenGlobalSavesWindow();
```

The unified settings window displays all registered options. `Bool` uses a checkbox, `Enum` uses a dropdown, `IntRange` / `FloatRange` use sliders, `String`, `Int`, and `Float` use input fields, and key bindings use an input field plus a Capture button. Edits, captured keys, and Reset clicks write back to the DSPCore BepInEx config. Key binding rows report same-key conflicts inside the same `ConflictGroup`, and the player's `Key Bind Display` strategy controls which pages show key rows. The window must be opened after `UIRoot` is initialized; players can open it from the Mod Settings button at the bottom of the vanilla option window or the DSPCore Key Bindings button at the end of the vanilla key-binding page, and author code can still call `Options.OpenWindow()` from a custom button, key bind, or UI callback. `OpenGlobalSavesWindow()` opens the read-only metadata page for cross-save global data; it does not provide editing or raw binary viewing.

## Boundaries

- The underlying BepInEx values are still strings. Boolean, integer, floating-point, and enum read helpers exist.
- The vanilla option window and vanilla key-binding page only provide entry buttons. Option rows, key capture, Reset, and conflict hints remain inside the DSPCore-owned unified settings window.
- `Key Bind Display` only affects DSPCore's own unified settings window. It does not mean DSPCore has injected key rows into the vanilla `BuiltinKey` / `overrideKeys` data model.
- Invalid `Int` / `Float` / key binding input is reverted to the current config value; range sliders write back according to their minimum, maximum, and step values.
- `Options.Page(...)` registers the page and returns a context. `Options.RegisterPage(...)` remains available for old calls that only want to register the page descriptor.
- `OptionUi.Order` only changes in-window ordering inside the same page; it does not change config load order.
- The Reset button only writes the descriptor default value; it does not run migrations, restart logic, or custom side effects.
- The text export format is for DSPCore's own `ImportText(...)` round trip, not a promised hand-editing format.
- Imports only overwrite registered options; they do not create new option descriptors.
- Config keys should stay stable so player configuration remains valid.

## Examples

- `Examples/OptionRegistration.md`
- `Examples/OptionRegistrationExample.cs`
