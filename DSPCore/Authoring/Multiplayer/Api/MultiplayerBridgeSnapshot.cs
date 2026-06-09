using System.Collections.Generic;

namespace DSPCore;

/// <summary>
/// 描述可供联机适配器读取的软联机声明快照。
/// Describes a soft multiplayer declaration snapshot for multiplayer adapters.
/// </summary>
/// <param name="IsNebulaAvailable">当前运行环境是否检测到 Nebula。Whether Nebula is detected in the current runtime.</param>
/// <param name="Packets">packet 声明。Packet declarations.</param>
/// <param name="Relays">主机转发声明。Host relay declarations.</param>
/// <param name="PlanetDataRequests">星球数据请求声明。Planet data request declarations.</param>
/// <param name="ClientSaveInitializers">客户端缺失存档初始化声明。Client missing-save initializer declarations.</param>
public sealed record MultiplayerBridgeSnapshot(
    bool IsNebulaAvailable,
    IReadOnlyCollection<MultiplayerPacketDescriptor> Packets,
    IReadOnlyCollection<MultiplayerRelayDescriptor> Relays,
    IReadOnlyCollection<MultiplayerPlanetDataDescriptor> PlanetDataRequests,
    IReadOnlyCollection<MultiplayerClientSaveDescriptor> ClientSaveInitializers);
