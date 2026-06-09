# Optional Multiplayer Bridge

The Multiplayer module provides soft multiplayer declarations without making the DSPCore main project depend on Nebula. Runtime detects Nebula through the loaded BepInEx plugin list and lets mods register packets, host relay boundaries, planet data requests, and client missing-save initialization boundaries.

## What This Module Provides

- Single-player mods do not receive a hard Nebula compile-time or runtime dependency through DSPCore.
- Multiplayer adapters can declare packet IDs and handling boundaries first, then a dedicated adapter can connect them to Nebula APIs.
- `Multiplayer.IsNebulaAvailable` is available for runtime branching.
- `RegisterHostRelay(...)` describes packets that should be handled and relayed by the host.
- `RegisterPlanetData(...)` describes export/import boundaries for client planet data requests.
- `RegisterClientIntoOtherSave(...)` describes initialization when a client has no multiplayer save data.
- `GetAdapterSnapshot()`, `TryGetPacket(...)`, `TryGetHostRelay(...)`, `TryGetPlanetDataRequest(...)`, and `TryGetClientSaveInitializer(...)` let standalone multiplayer adapters read declarations without depending on registry internals.
- `ApplyClientIntoOtherSaveInitializers()` can be called by adapters when a client has no multiplayer save data.

## Capability: Declare Multiplayer Boundaries

```csharp
Multiplayer.RegisterPacket(
    packetId: "com.example.sync-mode",
    ownerModGuid: "com.example.my-mod",
    handler: HandlePacket);

Multiplayer.RegisterHostRelay(
    packetId: "com.example.sync-mode",
    ownerModGuid: "com.example.my-mod",
    handleOnHost: HandleOnHost);
```

Mods should prefer the parameter short entries for ordinary declarations. `MultiplayerPacketDescriptor`, `MultiplayerRelayDescriptor`, `MultiplayerPlanetDataDescriptor`, and `MultiplayerClientSaveDescriptor` can also be passed directly to the corresponding `Register...(...)` method for configuration-driven or batch construction.

## Capability: Adapter Declaration Reads

```csharp
MultiplayerBridgeSnapshot snapshot = Multiplayer.GetAdapterSnapshot();
foreach (MultiplayerPacketDescriptor packet in snapshot.Packets)
{
    // Adapter maps packet.PacketId to its own network transport.
}
```

Adapters can also use `TryGetPacket(...)`, `TryGetHostRelay(...)`, and `TryGetPlanetDataRequest(...)` to query one declaration by stable id, or `TryGetClientSaveInitializer(...)` to query a client missing-save initializer by owner GUID.

## Boundaries

- DSPCore does not directly send Nebula packets in the current soft bridge.
- Real synchronization should be implemented by a dedicated Nebula adapter that reads the `DspCore.Multiplayer` registry.
- Adapter entries expose declarations and initialization boundaries only; Nebula types stay out of the main DSPCore project.
- Packet IDs must stay stable to avoid protocol incompatibility.

## Examples

- `Examples/SoftPacket.md`
- `Examples/SoftPacketExample.cs`
