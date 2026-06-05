# Protos

## Responsibility

This block declares proto registrations and the DSPCore data phases.

## Public API

- `Protos`: author-facing short entry point.
- `ProtoRegistryFacade`
- `ProtoRegistration`
- `CoreDataPhase`
- `ProtoKind`
- `VanillaDataView`

## Example

- `Examples/ProtoPhasesExample.cs`

## Runtime

`Runtime/ProtoRuntime.cs` applies proto registrations around `VFPreload.InvokeOnLoadWorkEnded` and rebuilds key derived caches.

## Boundaries

This block owns proto insertion and phase ordering. Build bar placement, icon binding, localization, and custom recipe type behavior are separate feature blocks that can be called from proto workflows.
