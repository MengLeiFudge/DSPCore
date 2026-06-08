using System;

namespace DSPCore;

/// <summary>
/// 可选联机桥能力的短入口。
/// Short entry point for optional multiplayer bridge capabilities.
/// </summary>
public static class Multiplayer
{
    /// <summary>
    /// 当前运行环境是否检测到 Nebula。
    /// Indicates whether Nebula is detected in the current runtime.
    /// </summary>
    public static bool IsNebulaAvailable => MultiplayerRuntime.IsNebulaAvailable;

    /// <summary>
    /// 注册一个软 packet 处理器。
    /// Registers a soft packet handler.
    /// </summary>
    /// <param name="packetId">packet ID。Packet ID.</param>
    /// <param name="ownerModGuid">所属模组 GUID。Owner mod GUID.</param>
    /// <param name="handler">处理回调。Handler callback.</param>
    public static void RegisterPacket(string packetId, string ownerModGuid, Action<byte[]> handler)
    {
        DspCore.Multiplayer.RegisterPacket(new MultiplayerPacketDescriptor(packetId, ownerModGuid, handler));
    }

    /// <summary>
    /// 注册一个主机转发 packet 边界。
    /// Registers a host relay packet boundary.
    /// </summary>
    /// <param name="packetId">packet ID。Packet ID.</param>
    /// <param name="ownerModGuid">所属模组 GUID。Owner mod GUID.</param>
    /// <param name="handleOnHost">主机处理回调。Host handler callback.</param>
    public static void RegisterHostRelay(string packetId, string ownerModGuid, Action<byte[]> handleOnHost)
    {
        DspCore.Multiplayer.RegisterRelay(new MultiplayerRelayDescriptor(packetId, ownerModGuid, handleOnHost));
    }

    /// <summary>
    /// 注册一个星球数据请求边界。
    /// Registers a planet data request boundary.
    /// </summary>
    /// <param name="requestId">请求 ID。Request ID.</param>
    /// <param name="ownerModGuid">所属模组 GUID。Owner mod GUID.</param>
    /// <param name="exportPlanetData">主机导出星球数据。Host-side planet data export.</param>
    /// <param name="importPlanetData">客户端导入星球数据。Client-side planet data import.</param>
    public static void RegisterPlanetData(
        string requestId,
        string ownerModGuid,
        Func<int, byte[]> exportPlanetData,
        Action<int, byte[]> importPlanetData)
    {
        DspCore.Multiplayer.RegisterPlanetData(new MultiplayerPlanetDataDescriptor(requestId, ownerModGuid, exportPlanetData, importPlanetData));
    }

    /// <summary>
    /// 注册客户端缺失联机存档数据时的初始化回调。
    /// Registers a client initialization callback used when multiplayer save data is absent.
    /// </summary>
    /// <param name="ownerModGuid">所属模组 GUID。Owner mod GUID.</param>
    /// <param name="intoOtherSave">初始化回调。Initialization callback.</param>
    public static void RegisterClientIntoOtherSave(string ownerModGuid, Action intoOtherSave)
    {
        DspCore.Multiplayer.RegisterClientIntoOtherSave(new MultiplayerClientSaveDescriptor(ownerModGuid, intoOtherSave));
    }
}
