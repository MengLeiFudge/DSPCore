# Option Registration Example

Use this for small mod settings that should live in the DSPCore BepInEx config file.

Prefer `Options.Bool(...)`, `Options.Int(...)`, `Options.Float(...)`, `Options.String(...)`, `Options.Enum(...)`, `Options.IntRange(...)`, and `Options.FloatRange(...)` for normal settings. They register the descriptor and return the current value in one call.

Use the same method name with `OptionUi` when the row needs a display name, page id, in-page order, or Reset button. This keeps simple settings short while avoiding a fall back to low-level `Register(...)` for ordinary presentation and row behavior metadata.

`OptionUi.Order` only changes row order inside the same page. `OptionUi.CanReset` only shows a Reset button that writes the descriptor default value back to the DSPCore BepInEx config file.

Use `Options.ExportValues()` / `Options.ImportValues(...)` when another system wants structured snapshots. Use `Options.ExportText()` / `Options.ImportText(...)` when the snapshot needs to be copied, stored, or sent as text. The text format is machine-readable and should be round-tripped through DSPCore instead of parsed by mods.

Call `Options.OpenWindow()` only from a button, key bind, or custom UI callback after `UIRoot` is initialized. Early plugin startup is too soon for Unity UI creation. The current window is DSPCore-owned and does not inject into the vanilla option page.
