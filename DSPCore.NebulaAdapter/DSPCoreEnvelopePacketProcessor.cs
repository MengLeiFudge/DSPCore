using BepInEx.Logging;
using NebulaAPI.Networking;
using NebulaAPI.Packets;

namespace DSPCore.NebulaAdapter;

/// <summary>
/// 处理 Nebula 收到的 DSPCore 信封 packet。
/// Processes DSPCore envelope packets received from Nebula.
/// </summary>
[RegisterPacketProcessor]
public sealed class DSPCoreEnvelopePacketProcessor : BasePacketProcessor<DSPCoreEnvelopePacket>
{
    private static readonly ManualLogSource Log = Logger.CreateLogSource("DSPCore.NebulaAdapter.Packet");

    /// <summary>
    /// 将收到的信封 packet 派发回 DSPCore。
    /// Dispatches the received envelope packet back into DSPCore.
    /// </summary>
    public override void ProcessPacket(DSPCoreEnvelopePacket packet, INebulaConnection conn)
    {
        switch (packet.Kind)
        {
            case DSPCoreEnvelopeKind.Packet:
                Multiplayer.DispatchPacket(packet.Id, packet.Payload);
                break;
            case DSPCoreEnvelopeKind.HostRelay:
                if (IsHost)
                {
                    Multiplayer.DispatchHostRelay(packet.Id, packet.Payload);
                }

                break;
            case DSPCoreEnvelopeKind.PlanetDataRequest:
                if (IsHost)
                {
                    HandlePlanetDataRequest(packet, conn);
                }

                break;
            case DSPCoreEnvelopeKind.PlanetDataResponse:
                Multiplayer.ImportPlanetData(packet.Id, packet.PlanetId, packet.Payload);
                break;
            default:
                Log.LogWarning($"Unknown DSPCore multiplayer envelope kind: {packet.Kind}");
                break;
        }
    }

    private static void HandlePlanetDataRequest(DSPCoreEnvelopePacket packet, INebulaConnection conn)
    {
        if (!Multiplayer.TryExportPlanetData(packet.Id, packet.PlanetId, out var payload))
        {
            return;
        }

        conn.SendPacket(new DSPCoreEnvelopePacket(DSPCoreEnvelopeKind.PlanetDataResponse, packet.Id, payload, packet.PlanetId));
    }
}
