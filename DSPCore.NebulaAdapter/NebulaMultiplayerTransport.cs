using BepInEx.Logging;
using NebulaAPI;

namespace DSPCore.NebulaAdapter;

internal sealed class NebulaMultiplayerTransport : IMultiplayerTransport
{
    private readonly ManualLogSource logger;

    public NebulaMultiplayerTransport(ManualLogSource logger)
    {
        this.logger = logger;
    }

    public bool SendPacket(string packetId, byte[] payload, MultiplayerSendTarget target, int targetId)
    {
        return Send(new DSPCoreEnvelopePacket(DSPCoreEnvelopeKind.Packet, packetId, payload), target, targetId);
    }

    public bool SendHostRelay(string packetId, byte[] payload)
    {
        if (NebulaModAPI.MultiplayerSession?.IsServer == true)
        {
            Multiplayer.DispatchHostRelay(packetId, payload ?? System.Array.Empty<byte>());
            return true;
        }

        return Send(new DSPCoreEnvelopePacket(DSPCoreEnvelopeKind.HostRelay, packetId, payload), MultiplayerSendTarget.Host, 0);
    }

    public bool RequestPlanetData(string requestId, int planetId)
    {
        if (NebulaModAPI.MultiplayerSession?.IsServer == true)
        {
            if (!Multiplayer.TryExportPlanetData(requestId, planetId, out var payload))
            {
                return false;
            }

            Multiplayer.ImportPlanetData(requestId, planetId, payload);
            return true;
        }

        return Send(
            new DSPCoreEnvelopePacket(DSPCoreEnvelopeKind.PlanetDataRequest, requestId, System.Array.Empty<byte>(), planetId),
            MultiplayerSendTarget.Host,
            0);
    }

    public bool SendPlanetDataResponse(string requestId, int planetId, byte[] payload)
    {
        return Send(
            new DSPCoreEnvelopePacket(DSPCoreEnvelopeKind.PlanetDataResponse, requestId, payload, planetId),
            MultiplayerSendTarget.All,
            0);
    }

    private bool Send(DSPCoreEnvelopePacket packet, MultiplayerSendTarget target, int targetId)
    {
        var session = NebulaModAPI.MultiplayerSession;
        var network = session?.Network;
        if (network == null || !NebulaModAPI.IsMultiplayerActive)
        {
            logger.LogWarning($"Cannot send DSPCore multiplayer packet {packet.Id}: Nebula multiplayer is not active.");
            return false;
        }

        switch (target)
        {
            case MultiplayerSendTarget.Host:
                network.SendPacket(packet);
                return true;
            case MultiplayerSendTarget.LocalPlanet:
                network.SendPacketToLocalPlanet(packet);
                return true;
            case MultiplayerSendTarget.LocalStar:
                network.SendPacketToLocalStar(packet);
                return true;
            case MultiplayerSendTarget.Planet:
                network.SendPacketToPlanet(packet, targetId);
                return true;
            case MultiplayerSendTarget.Star:
                network.SendPacketToStar(packet, targetId);
                return true;
            default:
                network.SendPacket(packet);
                return true;
        }
    }
}
