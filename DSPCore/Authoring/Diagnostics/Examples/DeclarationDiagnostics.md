# Declaration Diagnostics Example

Use this when a mod has author-side invariants that DSPCore cannot infer from shared registries.

DSPCore runs built-in checks during startup for registered proto IDs, GridIndex reuse, missing custom tabs, tab icons, option pages, and basic localization language pairs. Use `Diagnostics.Warn(...)` or `Diagnostics.Error(...)` for mod-specific rules such as unreachable recipes, missing machine unlocks, unsupported config combinations, or required third-party adapter state.

Use stable `code` values. Logs and copied diagnostic text should be searchable by both the code and the concrete subject. `subject` should name the object that needs attention, such as `item=9554`, `recipe=9554`, `tab=com.example.machines`, or `option=Example/Enabled`.

Diagnostics do not stop loading. They are intended to make support reports and author testing faster. Use a runtime guard or a conditional patch declaration when a feature must actually be disabled.
