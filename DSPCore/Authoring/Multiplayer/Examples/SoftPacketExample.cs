using DSPCore;

internal static class SoftPacketExample
{
    public static void Register()
    {
        Multiplayer.RegisterPacket(
            packetId: "com.example.sync-mode",
            ownerModGuid: "com.example.my-mod",
            handler: static payload =>
            {
                int length = payload.Length;
            });

        Multiplayer.RegisterHostRelay(
            packetId: "com.example.sync-mode",
            ownerModGuid: "com.example.my-mod",
            handleOnHost: static payload =>
            {
                int length = payload.Length;
            });

        Multiplayer.RegisterPlanetData(
            requestId: "com.example.planet-state",
            ownerModGuid: "com.example.my-mod",
            exportPlanetData: static planetId => new byte[] { (byte)(planetId & 255) },
            importPlanetData: static (planetId, payload) =>
            {
                int length = payload.Length;
            });

        Multiplayer.RegisterClientIntoOtherSave(
            ownerModGuid: "com.example.my-mod",
            intoOtherSave: static () =>
            {
            });
    }

    public static void AdapterRead()
    {
        MultiplayerBridgeSnapshot snapshot = Multiplayer.GetAdapterSnapshot();
        foreach (MultiplayerPacketDescriptor packet in snapshot.Packets)
        {
            string packetId = packet.PacketId;
        }

        if (Multiplayer.TryGetPlanetDataRequest("com.example.planet-state", out var request))
        {
            byte[] payload = request.ExportPlanetData(1);
        }
    }
}
