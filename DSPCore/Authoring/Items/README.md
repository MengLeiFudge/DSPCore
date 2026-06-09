# 物品注册

Items 是物品原型注册入口。它只表达“注册 ItemProto”这一个作者能力；三阶段执行时机由 DataPhases 提供，底层写入和缓存重建由 Systems/ProtoPipeline 处理。

## 这个模块带来什么便利

- 已经拿到 `ItemProto` 时，可以直接在对象上设置 `GridIndex`、绑定图标并注册，避免反复跳到 registry 风格入口。
- 图标绑定可以直接接 `ModResourcePack`，复用 owner 和资源根；普通路径、嵌入 PNG 和 AssetBundle 图标都有对象入口。
- `Items.Register(...)` 仍保留为低层入口，用于批量注册或不想使用扩展方法的代码。

## 功能：对象链式注册

```csharp
var pack = ModResources.Pack("com.example.my-mod", "assets/icons", typeof(MyPlugin).Assembly);

itemProto
    .SetGridIndex(tab, row: 1, index: 5)
    .BindIcon(pack, "example-machine", "example-machine.png")
    .RegisterItem("com.example.my-mod", purpose: "Declare example machine");

itemProto.SetBuildBar(tab: 3, row: 2, index: 5);
```

`SetGridIndex(...)` 写入原版 `ItemProto.GridIndex`。`BindIcon(...)`、`BindEmbeddedIcon(...)` 和 `BindAssetBundleIcon(...)` 只注册图标 descriptor，图标仍由 Icons runtime 在缓存重建时应用。`RegisterItem(...)` 会把当前物品登记到 DSPCore 的 ProtoPipeline。

## 功能：低层注册

```csharp
Items.Register(itemProto, "com.example.my-mod", CoreDataPhase.Data, "Declare example machine");
```

当前 `Items.Register(...)` 仍是 `ProtoRegistration.RegisterItem(...)` 的薄入口，用于把物品注册从大的 ProtoRegistration 概念中拆出来。已有 `ProtoRegistration.RegisterItem(...)` 继续保留。

## 这个模块不负责什么

- 不自动创建 `ItemProto`，物品字段仍由作者按 DSP 语义准备。
- 不负责配方、科技或解锁链路；对应能力分别放在 Recipes、Techs 或业务模组。
- 不替代 BuildBar；建造栏绑定仍使用 `ItemProto.SetBuildBar(...)` 或 `BuildBar.BindQuickBar(...)`。

## 示例

- `Examples/ItemAuthoringChain.md`
- `Examples/ItemAuthoringChainExample.cs`
