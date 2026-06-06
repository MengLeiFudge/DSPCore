# Protos

## Responsibility

This block declares proto registrations and the DSPCore data phases.

## Public API

- `Api/Protos.cs`: author-facing short entry point.
- `Api/ProtoRegistryFacade.cs`
- `Api/ProtoRegistration.cs`
- `Api/CoreDataPhase.cs`
- `Api/ProtoKind.cs`
- `Api/VanillaDataView.cs`

## Example

- `Examples/ProtoPhasesExample.cs`
- `Examples/ProtoPhases.md`

## Runtime

`Runtime/ProtoRuntime.cs` applies proto registrations around `VFPreload.InvokeOnLoadWorkEnded` and rebuilds key derived caches.

## Boundaries

This block owns proto insertion and phase ordering. Build bar placement, icon binding, localization, and custom recipe type behavior are separate feature blocks that can be called from proto workflows.
