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
        RegisterPacket(new MultiplayerPacketDescriptor(packetId, ownerModGuid, handler));
    }

    /// <summary>
    /// 注册一个软 packet 描述。
    /// Registers a soft packet descriptor.
    /// </summary>
    /// <param name="descriptor">packet 描述。Packet descriptor.</param>
    public static void RegisterPacket(MultiplayerPacketDescriptor descriptor)
    {
        DspCore.Multiplayer.RegisterPacket(descriptor);
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
        RegisterHostRelay(new MultiplayerRelayDescriptor(packetId, ownerModGuid, handleOnHost));
    }

    /// <summary>
    /// 注册一个主机转发 packet 描述。
    /// Registers a host relay packet descriptor.
    /// </summary>
    /// <param name="descriptor">转发描述。Relay descriptor.</param>
    public static void RegisterHostRelay(MultiplayerRelayDescriptor descriptor)
    {
        DspCore.Multiplayer.RegisterRelay(descriptor);
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
        RegisterPlanetData(new MultiplayerPlanetDataDescriptor(requestId, ownerModGuid, exportPlanetData, importPlanetData));
    }

    /// <summary>
    /// 注册一个星球数据请求描述。
    /// Registers a planet data request descriptor.
    /// </summary>
    /// <param name="descriptor">星球数据描述。Planet data descriptor.</param>
    public static void RegisterPlanetData(MultiplayerPlanetDataDescriptor descriptor)
    {
        DspCore.Multiplayer.RegisterPlanetData(descriptor);
    }

    /// <summary>
    /// 注册客户端缺失联机存档数据时的初始化回调。
    /// Registers a client initialization callback used when multiplayer save data is absent.
    /// </summary>
    /// <param name="ownerModGuid">所属模组 GUID。Owner mod GUID.</param>
    /// <param name="intoOtherSave">初始化回调。Initialization callback.</param>
    public static void RegisterClientIntoOtherSave(string ownerModGuid, Action intoOtherSave)
    {
        RegisterClientIntoOtherSave(new MultiplayerClientSaveDescriptor(ownerModGuid, intoOtherSave));
    }

    /// <summary>
    /// 注册客户端缺失联机存档数据时的初始化描述。
    /// Registers a client missing-save initialization descriptor.
    /// </summary>
    /// <param name="descriptor">客户端存档描述。Client save descriptor.</param>
    public static void RegisterClientIntoOtherSave(MultiplayerClientSaveDescriptor descriptor)
    {
        DspCore.Multiplayer.RegisterClientIntoOtherSave(descriptor);
    }

    /// <summary>
    /// 获取可供联机适配器读取的声明快照。
    /// Gets a declaration snapshot for multiplayer adapters.
    /// </summary>
    /// <returns>联机桥快照。Multiplayer bridge snapshot.</returns>
    public static MultiplayerBridgeSnapshot GetAdapterSnapshot()
    {
        return new MultiplayerBridgeSnapshot(
            IsNebulaAvailable,
            DspCore.Multiplayer.GetAll(),
            DspCore.Multiplayer.GetRelays(),
            DspCore.Multiplayer.GetPlanetDataRequests(),
            DspCore.Multiplayer.GetClientSaveInitializers());
    }

    /// <summary>
    /// 尝试获取指定 packet 描述。
    /// Tries to get a packet descriptor by id.
    /// </summary>
    public static bool TryGetPacket(string packetId, out MultiplayerPacketDescriptor descriptor)
    {
        return DspCore.Multiplayer.TryGetPacket(packetId, out descriptor);
    }

    /// <summary>
    /// 尝试获取指定主机转发描述。
    /// Tries to get a host relay descriptor by packet id.
    /// </summary>
    public static bool TryGetHostRelay(string packetId, out MultiplayerRelayDescriptor descriptor)
    {
        return DspCore.Multiplayer.TryGetRelay(packetId, out descriptor);
    }

    /// <summary>
    /// 尝试获取指定星球数据请求描述。
    /// Tries to get a planet data request descriptor by id.
    /// </summary>
    public static bool TryGetPlanetDataRequest(string requestId, out MultiplayerPlanetDataDescriptor descriptor)
    {
        return DspCore.Multiplayer.TryGetPlanetDataRequest(requestId, out descriptor);
    }

    /// <summary>
    /// 尝试获取指定客户端存档初始化描述。
    /// Tries to get a client save initializer descriptor by owner mod GUID.
    /// </summary>
    public static bool TryGetClientSaveInitializer(string ownerModGuid, out MultiplayerClientSaveDescriptor descriptor)
    {
        return DspCore.Multiplayer.TryGetClientSaveInitializer(ownerModGuid, out descriptor);
    }

    /// <summary>
    /// 执行所有客户端缺失联机存档数据时的初始化回调。
    /// Runs all client missing-save initialization callbacks.
    /// </summary>
    public static void ApplyClientIntoOtherSaveInitializers()
    {
        MultiplayerRuntime.ApplyClientIntoOtherSave();
    }
}
