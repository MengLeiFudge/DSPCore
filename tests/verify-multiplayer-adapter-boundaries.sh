#!/usr/bin/env bash
set -euo pipefail

repo_root="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"

if grep -R --include='*.csproj' -n 'NebulaAPI' "$repo_root/DSPCore" >/dev/null; then
    echo "Main DSPCore project must not reference NebulaAPI." >&2
    exit 1
fi

grep -q '<Reference Include="NebulaAPI">' "$repo_root/DSPCore.NebulaAdapter/DSPCore.NebulaAdapter.csproj"
grep -q '<ProjectReference Include="..\\DSPCore\\DSPCore.csproj" Private="false" />' "$repo_root/DSPCore.NebulaAdapter/DSPCore.NebulaAdapter.csproj"

grep -q 'NebulaModAPI.RegisterPackets' "$repo_root/DSPCore.NebulaAdapter/DSPCoreNebulaAdapterPlugin.cs"
grep -q 'Multiplayer.RegisterTransport' "$repo_root/DSPCore.NebulaAdapter/DSPCoreNebulaAdapterPlugin.cs"
grep -q 'Multiplayer.UnregisterTransport' "$repo_root/DSPCore.NebulaAdapter/DSPCoreNebulaAdapterPlugin.cs"
grep -q 'BasePacketProcessor<DSPCoreEnvelopePacket>' "$repo_root/DSPCore.NebulaAdapter/DSPCoreEnvelopePacketProcessor.cs"

grep -q 'Multiplayer.DispatchPacket(packet.Id, packet.Payload)' "$repo_root/DSPCore.NebulaAdapter/DSPCoreEnvelopePacketProcessor.cs"
grep -q 'Multiplayer.DispatchHostRelay(packet.Id, packet.Payload)' "$repo_root/DSPCore.NebulaAdapter/DSPCoreEnvelopePacketProcessor.cs"
grep -q 'Multiplayer.TryExportPlanetData(packet.Id, packet.PlanetId, out var payload)' "$repo_root/DSPCore.NebulaAdapter/DSPCoreEnvelopePacketProcessor.cs"
grep -q 'Multiplayer.ImportPlanetData(packet.Id, packet.PlanetId, packet.Payload)' "$repo_root/DSPCore.NebulaAdapter/DSPCoreEnvelopePacketProcessor.cs"
