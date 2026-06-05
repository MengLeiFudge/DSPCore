# Compatibility

## Responsibility

This block records compatibility patch declarations as feature-level metadata.

## Public API

- `CompatibilityPatchDescriptor`
- `CompatibilityPatchRegistry`

## Example

- `Examples/CompatibilityPatchExample.cs`

## Runtime

Runtime application is owned by the concrete feature that needs the patch. This registry is the declaration and reporting surface.

## Boundaries

Do not hide unrelated fixes under a generic fixer name. Split fixes by feature, such as tutorial, build bar, save, or UI.
