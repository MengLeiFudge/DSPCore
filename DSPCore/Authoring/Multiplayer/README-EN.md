# Optional Multiplayer Bridge

The Multiplayer module provides soft multiplayer declarations and send abstractions without making the DSPCore main project depend on Nebula. Runtime detects Nebula through the loaded BepInEx plugin list and lets mods register packets, host relay boundaries, planet data requests, and client missing-save initialization boundaries. Real Nebula transport is provided by the standalone `DSPCore.NebulaAdapter` project.

## What This Module Provides

- Single-player mods do not receive a hard Nebula compile-time or runtime dependency through DSPCore.
- Multiplayer adapters can declare packet IDs and handling boundaries first, then a dedicated adapter can connect them to Nebula APIs.
- `Multiplayer.IsNebulaAvailable` is available for runtime branching.
- `Multiplayer.HasTransport` indicates whether a real multiplayer transport is attached; send methods return `false` without one.
- `RegisterHostRelay(...)` describes packets that should be handled and relayed by the host.
- `RegisterPlanetData(...)` describes export/import boundaries for client planet data requests.
- `RegisterClientIntoOtherSave(...)` describes initialization when a client has no multiplayer save data.
- `GetAdapterSnapshot()`, `TryGetPacket(...)`, `TryGetHostRelay(...)`, `TryGetPlanetDataRequest(...)`, and `TryGetClientSaveInitializer(...)` let standalone multiplayer adapters read declarations without depending on registry internals.
- `DispatchPacket(...)`, `DispatchHostRelay(...)`, `TryExportPlanetData(...)`, and `ImportPlanetData(...)` let adapters hand received data back to DSPCore so author handlers are invoked through one error-reporting path.
- `ApplyClientIntoOtherSaveInitializers()` can be called by adapters when a client has no multiplayer save data.
- `SendPacket(...)`, `SendHostRelay(...)`, and `RequestPlanetData(...)` send through the current transport without exposing Nebula types from the main project.

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

if (Multiplayer.HasTransport)
{
    Multiplayer.SendPacket("com.example.sync-mode", payload, MultiplayerSendTarget.LocalPlanet);
    Multiplayer.SendHostRelay("com.example.sync-mode", payload);
    Multiplayer.RequestPlanetData("com.example.planet-state", planetId);
}
```

Mods should prefer the parameter short entries for ordinary declarations. `MultiplayerPacketDescriptor`, `MultiplayerRelayDescriptor`, `MultiplayerPlanetDataDescriptor`, and `MultiplayerClientSaveDescriptor` can also be passed directly to the corresponding `Register...(...)` method for configuration-driven or batch construction.

## Capability: Adapter Declaration Reads And Dispatch

```csharp
MultiplayerBridgeSnapshot snapshot = Multiplayer.GetAdapterSnapshot();
foreach (MultiplayerPacketDescriptor packet in snapshot.Packets)
{
    // Adapter maps packet.PacketId to its own network transport.
}

// After the transport receives a packet, hand the stable id and payload back to DSPCore.
Multiplayer.DispatchPacket("com.example.sync-mode", payload);

// Call this when the host receives a packet that should be handled by the host.
Multiplayer.DispatchHostRelay("com.example.sync-mode", payload);

// Host exports planet data; the client imports it after receiving it.
if (Multiplayer.TryExportPlanetData("com.example.planet-state", planetId, out byte[] data))
{
    Multiplayer.ImportPlanetData("com.example.planet-state", planetId, data);
}
```

Adapters can also use `TryGetPacket(...)`, `TryGetHostRelay(...)`, and `TryGetPlanetDataRequest(...)` to query one declaration by stable id, or `TryGetClientSaveInitializer(...)` to query a client missing-save initializer by owner GUID.

## Boundaries

- The main `DSPCore` project does not reference Nebula directly. Real Nebula packet send/receive behavior is implemented by `DSPCore.NebulaAdapter`.
- Send methods return `false` when no transport is installed or active; callers should fall back to single-player behavior.
- Adapter entries expose declarations, dispatch, and initialization boundaries only; Nebula types stay out of the main DSPCore project.
- `Dispatch...` and planet-data import/export only invoke author handlers. They do not own network transport, permission checks, host/client role checks, or reliability policy.
- `MultiplayerSendTarget.Host` is routed through the current transport's host-send semantics; different multiplayer implementations may map it differently.
- Packet IDs must stay stable to avoid protocol incompatibility.

## Examples

- `Examples/SoftPacket.md`
- `Examples/SoftPacketExample.cs`
