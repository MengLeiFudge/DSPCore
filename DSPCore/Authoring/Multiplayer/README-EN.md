# Optional Multiplayer Bridge

The Multiplayer module provides soft multiplayer declarations without making the DSPCore main project depend on Nebula. Runtime detects Nebula through the loaded BepInEx plugin list and lets mods register packets, host relay boundaries, planet data requests, and client missing-save initialization boundaries.

## What This Module Provides

- Single-player mods do not receive a hard Nebula compile-time or runtime dependency through DSPCore.
- Multiplayer adapters can declare packet IDs and handling boundaries first, then a dedicated adapter can connect them to Nebula APIs.
- `Multiplayer.IsNebulaAvailable` is available for runtime branching.
- `RegisterHostRelay(...)` describes packets that should be handled and relayed by the host.
- `RegisterPlanetData(...)` describes export/import boundaries for client planet data requests.
- `RegisterClientIntoOtherSave(...)` describes initialization when a client has no multiplayer save data.

## Boundaries

- DSPCore does not directly send Nebula packets in the current soft bridge.
- Real synchronization should be implemented by a dedicated Nebula adapter that reads the `DspCore.Multiplayer` registry.
- Packet IDs must stay stable to avoid protocol incompatibility.

## Examples

- `Examples/SoftPacket.md`
- `Examples/SoftPacketExample.cs`
