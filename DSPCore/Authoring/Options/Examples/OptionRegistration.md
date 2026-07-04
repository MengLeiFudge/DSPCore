# Option Registration Example

Use this for small mod settings that should live in the DSPCore BepInEx config file.

Prefer `Options.Page(...).Section(...)` when several settings belong to one DSPCore settings page. The returned `OptionSection` keeps the page id and config section, so each row only names its key, default value, and description.

Use `Options.Bool(...)`, `Options.Int(...)`, `Options.Float(...)`, `Options.String(...)`, `Options.Enum(...)`, `Options.IntRange(...)`, and `Options.FloatRange(...)` directly for one-off settings or settings that do not need a page. They register the descriptor and return the current value in one call.

Use the same method name with `OptionUi` when the row needs a display name, in-page order, or Reset button. Page contexts carry the page id automatically. This keeps simple settings short while avoiding a fall back to low-level `Register(...)` for ordinary presentation and row behavior metadata.

`OptionUi.Order` only changes row order inside the same page. `OptionUi.CanReset` only shows a Reset button that writes the descriptor default value back to the DSPCore BepInEx config file.

Use `Options.ExportValues()` / `Options.ImportValues(...)` when another system wants structured snapshots. Use `Options.ExportText()` / `Options.ImportText(...)` when the snapshot needs to be copied, stored, or sent as text. The text format is machine-readable and should be round-tripped through DSPCore instead of parsed by mods.

Call `Options.OpenWindow()` only from a button, key bind, or custom UI callback after `UIRoot` is initialized. Early plugin startup is too soon for Unity UI creation. DSPCore also adds a Mod Settings entry button to the vanilla option window; the actual option rows are still rendered by DSPCore's own unified settings window.
