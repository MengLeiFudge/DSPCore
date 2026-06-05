# Legacy

## Responsibility

This block keeps obsolete legacy namespaces and maps covered calls to DSPCore's new feature blocks.

## Public API

- `xiaoye97.LDBTool`
- `crecheng.DSPModSave`
- `CommonAPI`
- `BuildBarTool`

## Runtime

Legacy calls delegate to new registries where possible. Runtime behavior is still owned by the new feature blocks.

## Boundaries

Legacy namespaces must not become DSPCore's internal design language.
