using DSPCore;

namespace ExampleMod;

// 本文件是文档示例，不参与 DSPCore 编译。
// This file is a documentation example and is excluded from DSPCore compilation.
//
// 用途：
// - RegisterPacket / RegisterHostRelay / RegisterPlanetData 声明联机软边界。
// - RegisterClientIntoOtherSave 声明客户端缺失联机存档数据时的初始化回调。
// - 独立联机适配器通过 snapshot 或 TryGet... 读取声明，再用 Dispatch... 消费数据。
//
// Usage:
// - Register soft multiplayer declarations once during startup.
// - Keep packet and request ids stable across versions.
// - Use descriptor overloads only for batch or configuration-driven declarations.
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
            string requestId = request.RequestId;
        }

        if (Multiplayer.TryGetClientSaveInitializer("com.example.my-mod", out var initializer))
        {
            string ownerModGuid = initializer.OwnerModGuid;
        }
    }

    public static void AdapterConsume(byte[] payload)
    {
        Multiplayer.DispatchPacket("com.example.sync-mode", payload);
        Multiplayer.DispatchHostRelay("com.example.sync-mode", payload);

        if (Multiplayer.TryExportPlanetData("com.example.planet-state", planetId: 1, out var planetPayload))
        {
            Multiplayer.ImportPlanetData("com.example.planet-state", planetId: 1, planetPayload);
        }

        Multiplayer.ApplyClientIntoOtherSaveInitializers();
    }
}
