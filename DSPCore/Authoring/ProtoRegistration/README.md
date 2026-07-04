# 原型注册

ProtoRegistration 模块让模组把 `ItemProto`、`RecipeProto`、`TechProto`、`TutorialProto` 等 DSP 原型登记到 DSPCore 的数据阶段，由 DSPCore 在统一时机写入 LDB 并重建派生缓存。

## 这个模块带来什么便利

- 你不需要每个模组各自寻找 LDB 写入时机、手动插入 `ProtoSet` 或漏掉缓存重建。
- 你可以用 `Data`、`DataUpdates`、`DataFinalFixes` 回调表达“先声明、再跨模组调整、最后修正”的顺序，接近 Factorio 的 data.lua / data-updates.lua / data-final-fixes.lua。
- DSPCore 会在应用 Proto 后重建物品、模型、配方、信号、图标等关键派生缓存，减少“数据写进去了但 UI/索引没更新”的问题。
- 新内容可以用 `ProtoStableId` 声明稳定身份。作者给稳定 key 和可选 preferred int；DSPCore 会把它解析为实际运行时 int ID，并持久化映射，避免跨模组 ID 冲突和未来重命名迁移断裂。
- 旧 LDBTool 的 `PreAddProto` / `PostAddProto` 会桥接到 ProtoRegistration，便于已有代码迁移。
- `Protos` 仍保留为兼容/短别名；新文档和示例主推 `ProtoRegistration`。

## 功能：注册数据阶段回调

推荐写法是在启动阶段注册三个数据阶段回调。DSPCore 到对应阶段时会传入 `ProtoPhaseContext`，你在回调里注册物品、配方、科技或最终修正。

```csharp
ProtoRegistration.Data("com.example.my-mod", data =>
{
    data.RegisterItem(itemProto, "Declare base item");
});

ProtoRegistration.DataUpdates("com.example.my-mod", data =>
{
    data.RegisterRecipe(recipeProto, "Attach recipe after item declarations");
});

ProtoRegistration.DataFinalFixes("com.example.my-mod", data =>
{
    data.RegisterTutorial(tutorialProto, "Final tutorial chain fix");
});
```

同一阶段内可以用 `priority` 调整执行顺序，数值越小越早；相同 priority 保持注册顺序。

## 功能：直接注册新 Proto

直接类型化入口仍然保留，适合兼容旧代码或简单声明。新代码需要表达阶段逻辑时优先使用上面的回调写法。

```csharp
ProtoRegistration.RegisterItem(itemProto, "com.example.my-mod");
ProtoRegistration.RegisterRecipe(recipeProto, "com.example.my-mod", CoreDataPhase.DataUpdates);
ProtoRegistration.RegisterTech(techProto, "com.example.my-mod");
ProtoRegistration.RegisterTutorial(tutorialProto, "com.example.my-mod");
```

需要指定类型或记录用途时，可以使用通用直接入口：

```csharp
ProtoRegistration.Register(typeof(ItemProto), itemProto, "com.example.my-mod", CoreDataPhase.Data, ProtoKind.Item, "new building item");
```

## 功能：使用稳定 Proto 身份

新内容推荐使用稳定身份注册。`key` 是模组内稳定键；`preferredId` 只是优先候选，可以是任意正 int ID。如果该 int ID 已被原版或其他模组占用，DSPCore 会自动分配 `12000` 以上的可用 ID。最终映射会写入 BepInEx config 下的 `DSPCore/StableProtoIds.tsv`，后续启动继续复用同一个运行时 ID。

```csharp
ProtoRegistration.Data("com.example.my-mod", data =>
{
    data.RegisterItem(
            itemProto,
            ProtoStableId.Of("example-machine", preferredId: 12001),
            "Declare stable machine item")
        .SetBuildBar(category: 3, row: 1, index: 5);
});
```

如果旧版本的稳定 key 已经改名，把旧 key 放进 alias：

```csharp
ProtoStableId.Of("example-machine-v2", preferredId: 12001, "example-machine");
```

直接入口也支持稳定身份：

```csharp
ProtoRegistration.RegisterRecipe(
    recipeProto,
    "com.example.my-mod",
    ProtoStableId.Of("example-machine-recipe", preferredId: 12002));
```

没有稳定身份的注册仍按传入的 `Proto.ID` 工作；如果在 `Data` 阶段与已有 LDB ID 或同阶段注册重复，DSPCore 会把它当成注册错误，而不是静默覆盖已有内容。跨模组修改或显式替换应放在 `DataUpdates` / `DataFinalFixes`。

## 功能：设置物品或配方格子

`GridIndex` 是游戏原生字段，属于 `ItemProto` 和 `RecipeProto` 自身。它决定物品或配方在对应页面里的格子位置。

如果物品或配方要放到 DSPCore 自定义页面，先用 Tabs 注册页面并取得 `TabSlot`，再生成 `GridIndex`：

```csharp
TabSlot machinesTab = Tabs.AddTab(
    id: "example-machines",
    ownerModGuid: "com.example.my-mod",
    title: "ExampleMachines",
    iconId: "example-machines-icon");

itemProto.GridIndex = ProtoRegistration.GetGridIndex(machinesTab, row: 1, index: 5);
recipeProto.GridIndex = ProtoRegistration.GetGridIndex(machinesTab, row: 1, index: 5);
```

如果仍放在游戏原本页面，可以直接使用原版 tab 分类编号：

```csharp
itemProto.GridIndex = ProtoRegistration.GetGridIndex(tab: 1, row: 2, index: 3);
```

`TabSlot` 是页面槽位；`GridIndex` 是物品/配方格子字段。不要把两者当成同一个概念。

## 功能：选择数据阶段

- `CoreDataPhase.Data` / `ProtoRegistration.Data(...)`：初始数据声明，适合新增 Proto 和基础字段。
- `CoreDataPhase.DataUpdates` / `ProtoRegistration.DataUpdates(...)`：跨模组数据调整，适合依赖其他模组声明后的修改。
- `CoreDataPhase.DataFinalFixes` / `ProtoRegistration.DataFinalFixes(...)`：最终修正，适合在派生缓存重建前收口。

不要把所有逻辑都塞进 `DataFinalFixes`。只有确实依赖其他声明完成后的收口逻辑才放到最终修正阶段。

## 调用后 DSPCore 会怎么处理

- 启动阶段只记录数据阶段回调和直接注册项，不立即写 LDB。
- 运行时进入某个数据阶段后，先按 `priority` 和注册顺序执行该阶段回调。
- 回调通过 `ProtoPhaseContext` 注册的 Proto 会归入当前阶段。
- 带 `ProtoStableId` 的注册会先解析最终运行时 int ID；已有映射优先，其次尝试 alias，其次尝试 preferred int，最后从自定义 ID 区间分配。
- 运行时按阶段取出注册项，并按具体 Proto 类型分组写入对应 LDB `ProtoSet`。
- `Data` 阶段不再允许无稳定身份的重复 int ID 静默覆盖已有内容；这类冲突会作为结构化异常记录到 Errors 和日志。
- 如果找不到对应 LDB `ProtoSet`，该组会被跳过并写日志。
- 每个阶段只应用一次，重复触发不会重复插入同一阶段的数据。
- 最终修正后会重建 LDB 索引、物品派生缓存、模型派生缓存、配方派生缓存、自定义配方类型、信号索引和图标缓存。

## 功能：读取原版数据描述

`DspCore.Vanilla.GetItem(...)`、`GetRecipe(...)`、`GetTech(...)` 返回的是旧的读取描述。新的阶段内直接查询和修改应使用 `ProtoPhaseContext.FindItem(...)` / `FindRecipe(...)` 或 `data.Access`，相关说明见 `Authoring/ProtoAccess`。

## 这个模块不负责什么

- 不负责建造栏位置；创建物品后需要调用 BuildBar。
- 不负责图标资源加载；需要图标时调用 Icons。
- 不负责本地化字符串；需要文本时调用 Resources。
- 不负责分配页面槽位；需要新页面时调用 Tabs，并用返回的 `TabSlot` 生成物品/配方 `GridIndex`。
- 不负责自定义配方类型限制；需要机器可用性保护时调用 GameEnums。
- 不把 stable key 暴露给游戏原生存档；游戏运行时、LDB、UI 和数组索引仍使用最终 int ID。
- 当前 Proto 阶段挂点是保守的第一版桥接，不是最终 VFPreload 中段生命周期。

## 示例

- `Examples/StableProtoIdentity.md`
- `Examples/StableProtoIdentityExample.cs`
- 三段数据阶段完整写法见 `../DataPhases/Examples/ProtoPhases.md` 和 `../DataPhases/Examples/ProtoPhasesExample.cs`。
