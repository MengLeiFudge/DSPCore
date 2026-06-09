# Clone Model Example

Use this when a new building can reuse an existing model but needs different prefab metadata.

Prefer `sourceModel.CloneAsModel(...)` when the source `ModelProto` is already available. Use `Models.CloneModel(...)` when the code only has a source model index. Use `ModelDescriptor` only for configuration-driven or batch declarations.
