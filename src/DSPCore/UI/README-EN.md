# UI

## Responsibility

This block stores lightweight UI descriptors and creation helpers for feature blocks.

## Public API

- `Api/UiButtonDescriptor.cs`
- `Api/UiNodeFactory.cs`

## Runtime

Current runtime uses direct adapters for specific surfaces. This block is the common abstraction point for future shared UI helpers.

## Boundaries

Do not put feature-specific UI behavior here. Keep concrete behavior in the owning feature block.
