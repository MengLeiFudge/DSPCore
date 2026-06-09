# 工厂网络

Networks 模块提供工厂网络查询适配层。不同模组可以用 `Networks.Register(networkId, ownerModGuid, tryGetCommonNetwork, ...)` 注册自己的网络判断逻辑，调用方通过 `Networks.TryGetCommonNetwork(...)` 询问两个实体是否共享某个网络，也可以用 `Networks.IsConnectedToNetwork(...)` 查询实体是否连接到指定网络。

## 这个模块带来什么便利

- 调用方不需要知道目标网络来自电网、物流网、流体网还是自定义网络。
- 多个网络适配器可以并存，先返回结果的适配器胜出。
- 网络能力留在作者侧 API；具体扫描和缓存由注册适配器负责。
- `networkId` 是适配器定义的稳定整数；同一个适配器内应保持一致。

## 功能：短入口注册网络适配器

```csharp
Networks.Register(
    networkId: "com.example.power-group",
    ownerModGuid: "com.example.my-mod",
    tryGetCommonNetwork: static (factory, entityA, entityB) => entityA > 0 && entityB > 0 ? 1 : null,
    isConnectedToNetwork: static (factory, entityId, networkId) => entityId > 0 && networkId == 1);
```

`tryGetCommonNetwork` 返回 `null` 表示这个适配器没有结果，DSPCore 会继续询问后续适配器。需要直接构造完整描述对象时，仍可使用 `Networks.Register(new NetworkDescriptor(...))`。

## 边界

- DSPCore 当前不内置所有网络类型扫描。
- `networkId` 的含义由返回它的适配器定义。
- 如果需要跨星球网络，应在适配器 ID 和返回值中明确编码。

## 示例

- `Examples/CommonNetwork.md`
- `Examples/CommonNetworkExample.cs`
