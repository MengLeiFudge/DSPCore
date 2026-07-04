# 可选联机桥

Multiplayer 模块提供软联机声明和发送抽象，不让 DSPCore 主项目硬依赖 Nebula。运行时会通过 BepInEx 已加载插件列表检测 Nebula，并允许模组注册 packet、本地主机转发、星球数据请求和客户端缺失存档初始化边界。真实 Nebula 传输由独立 `DSPCore.NebulaAdapter` 项目接入。

## 这个模块带来什么便利

- 单机模组不会因为 DSPCore 引入 Nebula 编译或运行依赖。
- 联机适配可以先声明 packet ID 和处理边界，再由专门适配器接入 Nebula API。
- `Multiplayer.IsNebulaAvailable` 可用于运行时分支。
- `Multiplayer.HasTransport` 可判断是否已有真实联机 transport 接入；没有 transport 时发送方法返回 `false`。
- `RegisterHostRelay(...)` 描述需要主机处理并转发的 packet。
- `RegisterPlanetData(...)` 描述客户端向主机请求某个星球数据的导出/导入边界。
- `RegisterClientIntoOtherSave(...)` 描述客户端缺失联机存档数据时的初始化回调。
- `GetAdapterSnapshot()`、`TryGetPacket(...)`、`TryGetHostRelay(...)`、`TryGetPlanetDataRequest(...)` 和 `TryGetClientSaveInitializer(...)` 让独立联机适配器不用直接读取内部 registry 细节。
- `DispatchPacket(...)`、`DispatchHostRelay(...)`、`TryExportPlanetData(...)` 和 `ImportPlanetData(...)` 让适配器把收到的数据交回 DSPCore 统一调用作者处理器，异常会进入 Errors。
- `ApplyClientIntoOtherSaveInitializers()` 可由适配器在客户端缺失联机存档数据时调用。
- `SendPacket(...)`、`SendHostRelay(...)` 和 `RequestPlanetData(...)` 通过当前 transport 发出请求；主项目不直接引用 Nebula 类型。

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

if (Multiplayer.HasTransport)
{
    Multiplayer.SendPacket("com.example.sync-mode", payload, MultiplayerSendTarget.LocalPlanet);
    Multiplayer.SendHostRelay("com.example.sync-mode", payload);
    Multiplayer.RequestPlanetData("com.example.planet-state", planetId);
}
```

普通模组应优先使用参数短入口。`MultiplayerPacketDescriptor`、`MultiplayerRelayDescriptor`、`MultiplayerPlanetDataDescriptor` 和 `MultiplayerClientSaveDescriptor` 也可以直接传给对应 `Register...(...)`，适合配置驱动或批量构造。

## 功能：适配器读取并消费声明

```csharp
MultiplayerBridgeSnapshot snapshot = Multiplayer.GetAdapterSnapshot();
foreach (MultiplayerPacketDescriptor packet in snapshot.Packets)
{
    // Adapter maps packet.PacketId to its own network transport.
}

// 收到 transport packet 后，把稳定 ID 和 payload 交回 DSPCore。
Multiplayer.DispatchPacket("com.example.sync-mode", payload);

// 主机收到需要主机处理的 packet 后调用。
Multiplayer.DispatchHostRelay("com.example.sync-mode", payload);

// 主机导出星球数据，客户端收到后导入。
if (Multiplayer.TryExportPlanetData("com.example.planet-state", planetId, out byte[] data))
{
    Multiplayer.ImportPlanetData("com.example.planet-state", planetId, data);
}
```

适配器也可以用 `TryGetPacket(...)`、`TryGetHostRelay(...)`、`TryGetPlanetDataRequest(...)` 按稳定 ID 查询单个声明，或用 `TryGetClientSaveInitializer(...)` 按 owner GUID 查询客户端缺失存档初始化声明。

## 边界

- `DSPCore` 主项目不直接引用 Nebula；真实 Nebula packet 发送和接收由 `DSPCore.NebulaAdapter` 完成。
- 未安装或未启用 transport 时，发送方法返回 `false`，调用方应按单机逻辑降级。
- 适配器入口只暴露声明、dispatch 和初始化边界，不把 Nebula 类型带入 DSPCore 主项目。
- `Dispatch...` 和星球数据导入导出只负责调用作者处理器，不负责网络传输、权限判断、主客机判断或可靠性策略。
- `MultiplayerSendTarget.Host` 交给当前 transport 的主机发送语义；不同联机实现可有不同底层路由。
- packet ID 必须稳定，避免联机协议不兼容。

## 示例

- `Examples/SoftPacket.md`
- `Examples/SoftPacketExample.cs`
