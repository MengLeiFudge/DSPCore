# Factory Networks

The Networks module provides a factory network query adapter layer. Mods can use `Networks.Register(networkId, ownerModGuid, tryGetCommonNetwork, ...)` to register their own network logic, and callers use `Networks.TryGetCommonNetwork(...)` to ask whether two entities share a network, or `Networks.IsConnectedToNetwork(...)` to ask whether an entity is connected to a specific network.

## What This Module Provides

- Callers do not need to know whether the target network is power, logistics, fluid, or custom.
- Multiple adapters can coexist; the first adapter that returns a result wins.
- The author API owns the query surface, while concrete scanning and caching stay in adapters.
- `networkId` is a stable integer defined by the adapter and should remain consistent inside that adapter.

## Capability: Short Network Adapter Registration

```csharp
Networks.Register(
    networkId: "com.example.power-group",
    ownerModGuid: "com.example.my-mod",
    tryGetCommonNetwork: static (factory, entityA, entityB) => entityA > 0 && entityB > 0 ? 1 : null,
    isConnectedToNetwork: static (factory, entityId, networkId) => entityId > 0 && networkId == 1);
```

`tryGetCommonNetwork` returns `null` when this adapter has no result, and DSPCore continues asking later adapters. Use `Networks.Register(new NetworkDescriptor(...))` when code needs to construct the full descriptor object directly.

## Boundaries

- DSPCore does not currently scan every network type by itself.
- The meaning of `networkId` is defined by the adapter that returns it.
- Cross-planet networks should encode that boundary in the adapter ID and return value.

## Examples

- `Examples/CommonNetwork.md`
- `Examples/CommonNetworkExample.cs`
