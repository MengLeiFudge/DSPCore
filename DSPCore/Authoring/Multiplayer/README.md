# 可选联机桥

Multiplayer 模块提供软联机声明，不让 DSPCore 主项目硬依赖 Nebula。运行时会通过 BepInEx 已加载插件列表检测 Nebula，并允许模组注册 packet、本地主机转发、星球数据请求和客户端缺失存档初始化边界。

## 这个模块带来什么便利

- 单机模组不会因为 DSPCore 引入 Nebula 编译或运行依赖。
- 联机适配可以先声明 packet ID 和处理边界，再由专门适配器接入 Nebula API。
- `Multiplayer.IsNebulaAvailable` 可用于运行时分支。
- `RegisterHostRelay(...)` 描述需要主机处理并转发的 packet。
- `RegisterPlanetData(...)` 描述客户端向主机请求某个星球数据的导出/导入边界。
- `RegisterClientIntoOtherSave(...)` 描述客户端缺失联机存档数据时的初始化回调。
- `GetAdapterSnapshot()`、`TryGetPacket(...)`、`TryGetHostRelay(...)`、`TryGetPlanetDataRequest(...)` 和 `TryGetClientSaveInitializer(...)` 让独立联机适配器不用直接读取内部 registry 细节。
- `ApplyClientIntoOtherSaveInitializers()` 可由适配器在客户端缺失联机存档数据时调用。

## 功能：声明联机边界

```csharp
Multiplayer.RegisterPacket(
    packetId: "com.example.sync-mode",
    ownerModGuid: "com.example.my-mod",
    handler: HandlePacket);

Multiplayer.RegisterHostRelay(
    packetId: "com.example.sync-mode",
    ownerModGuid: "com.example.my-mod",
    handleOnHost: HandleOnHost);
```

普通模组应优先使用参数短入口。`MultiplayerPacketDescriptor`、`MultiplayerRelayDescriptor`、`MultiplayerPlanetDataDescriptor` 和 `MultiplayerClientSaveDescriptor` 也可以直接传给对应 `Register...(...)`，适合配置驱动或批量构造。

## 功能：适配器读取声明

```csharp
MultiplayerBridgeSnapshot snapshot = Multiplayer.GetAdapterSnapshot();
foreach (MultiplayerPacketDescriptor packet in snapshot.Packets)
{
    // Adapter maps packet.PacketId to its own network transport.
}
```

适配器也可以用 `TryGetPacket(...)`、`TryGetHostRelay(...)`、`TryGetPlanetDataRequest(...)` 按稳定 ID 查询单个声明，或用 `TryGetClientSaveInitializer(...)` 按 owner GUID 查询客户端缺失存档初始化声明。

## 边界

- 当前不直接发送 Nebula packet；这是软桥声明层。
- 需要真实同步时，应由单独 Nebula 适配器读取 `DspCore.Multiplayer` 注册表。
- 适配器入口只暴露声明和初始化边界，不把 Nebula 类型带入 DSPCore 主项目。
- packet ID 必须稳定，避免联机协议不兼容。

## 示例

- `Examples/SoftPacket.md`
- `Examples/SoftPacketExample.cs`
