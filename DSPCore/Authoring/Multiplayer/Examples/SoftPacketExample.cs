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
    }
}
