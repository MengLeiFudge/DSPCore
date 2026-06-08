using System;
using System.Collections.Generic;

namespace DSPCore;

/// <summary>
/// 管理可选联机桥声明。
/// Manages optional multiplayer bridge declarations.
/// </summary>
public sealed class MultiplayerRegistry
{
    private readonly Dictionary<string, MultiplayerPacketDescriptor> packets = new(StringComparer.Ordinal);
    private readonly Dictionary<string, MultiplayerRelayDescriptor> relays = new(StringComparer.Ordinal);
    private readonly Dictionary<string, MultiplayerPlanetDataDescriptor> planetData = new(StringComparer.Ordinal);
    private readonly Dictionary<string, MultiplayerClientSaveDescriptor> clientSaves = new(StringComparer.Ordinal);

    /// <summary>
    /// 注册一个 packet 处理器。
    /// Registers a packet handler.
    /// </summary>
    /// <param name="descriptor">packet 描述。Packet descriptor.</param>
    public void RegisterPacket(MultiplayerPacketDescriptor descriptor)
    {
        packets[descriptor.PacketId] = descriptor;
    }

    /// <summary>
    /// 注册一个主机转发边界。
    /// Registers a host relay boundary.
    /// </summary>
    /// <param name="descriptor">转发描述。Relay descriptor.</param>
    public void RegisterRelay(MultiplayerRelayDescriptor descriptor)
    {
        relays[descriptor.PacketId] = descriptor;
    }

    /// <summary>
    /// 注册一个星球数据请求边界。
    /// Registers a planet data request boundary.
    /// </summary>
    /// <param name="descriptor">星球数据描述。Planet data descriptor.</param>
    public void RegisterPlanetData(MultiplayerPlanetDataDescriptor descriptor)
    {
        planetData[descriptor.RequestId] = descriptor;
    }

    /// <summary>
    /// 注册客户端缺失联机存档数据时的初始化回调。
    /// Registers a client initialization callback used when multiplayer save data is absent.
    /// </summary>
    /// <param name="descriptor">客户端存档描述。Client save descriptor.</param>
    public void RegisterClientIntoOtherSave(MultiplayerClientSaveDescriptor descriptor)
    {
        clientSaves[descriptor.OwnerModGuid] = descriptor;
    }

    /// <summary>
    /// 获取所有 packet 描述快照。
    /// Gets a snapshot of all packet descriptors.
    /// </summary>
    /// <returns>packet 描述集合。Packet descriptor collection.</returns>
    public IReadOnlyCollection<MultiplayerPacketDescriptor> GetAll()
    {
        return packets.Values;
    }

    /// <summary>
    /// 获取所有主机转发描述快照。
    /// Gets a snapshot of all host relay descriptors.
    /// </summary>
    /// <returns>转发描述集合。Relay descriptor collection.</returns>
    public IReadOnlyCollection<MultiplayerRelayDescriptor> GetRelays()
    {
        return relays.Values;
    }

    /// <summary>
    /// 获取所有星球数据描述快照。
    /// Gets a snapshot of all planet data descriptors.
    /// </summary>
    /// <returns>星球数据描述集合。Planet data descriptor collection.</returns>
    public IReadOnlyCollection<MultiplayerPlanetDataDescriptor> GetPlanetDataRequests()
    {
        return planetData.Values;
    }

    /// <summary>
    /// 获取所有客户端存档初始化描述快照。
    /// Gets a snapshot of all client save initialization descriptors.
    /// </summary>
    /// <returns>客户端存档描述集合。Client save descriptor collection.</returns>
    public IReadOnlyCollection<MultiplayerClientSaveDescriptor> GetClientSaveInitializers()
    {
        return clientSaves.Values;
    }
}
