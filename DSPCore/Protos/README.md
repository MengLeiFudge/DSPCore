# 原型阶段

Protos 模块让模组把 `ItemProto`、`RecipeProto`、`TechProto`、`TutorialProto` 等 DSP 原型登记到 DSPCore 的数据阶段，由 DSPCore 在统一时机写入 LDB 并重建派生缓存。

## 这个模块带来什么便利

- 你不需要每个模组各自寻找 LDB 写入时机、手动插入 `ProtoSet` 或漏掉缓存重建。
- 你可以用 `Data`、`DataUpdates`、`DataFinalFixes` 表达“先声明、再跨模组调整、最后修正”的顺序。
- DSPCore 会在应用 Proto 后重建物品、模型、配方、信号、图标等关键派生缓存，减少“数据写进去了但 UI/索引没更新”的问题。
- 旧 LDBTool 的 `PreAddProto` / `PostAddProto` 会桥接到 Protos，便于已有代码迁移。

## 功能：注册新 Proto

常用类型化入口：

```csharp
Protos.RegisterItem(itemProto, "com.example.my-mod");
Protos.RegisterRecipe(recipeProto, "com.example.my-mod", CoreDataPhase.DataUpdates);
Protos.RegisterTech(techProto, "com.example.my-mod");
Protos.RegisterTutorial(tutorialProto, "com.example.my-mod");
```

需要指定类型或记录用途时，可以使用通用入口：

```csharp
Protos.Register(typeof(ItemProto), itemProto, "com.example.my-mod", CoreDataPhase.Data, ProtoKind.Item, "new building item");
```

## 功能：设置物品或配方格子

`GridIndex` 是游戏原生字段，属于 `ItemProto` 和 `RecipeProto` 自身。它决定物品或配方在对应页面里的格子位置。

如果物品或配方要放到 DSPCore 自定义页面，先用 Tabs 注册页面并取得 `TabSlot`，再生成 `GridIndex`：

```csharp
TabSlot machinesTab = Tabs.AddTab(new CoreTabDescriptor(
    Id: "example-machines",
    OwnerModGuid: "com.example.my-mod",
    Title: "ExampleMachines",
    IconId: "example-machines-icon"));

itemProto.GridIndex = Protos.GetGridIndex(machinesTab, row: 1, index: 5);
recipeProto.GridIndex = Protos.GetGridIndex(machinesTab, row: 1, index: 5);
```

如果仍放在游戏原本页面，可以直接使用原版 tab 分类编号：

```csharp
itemProto.GridIndex = Protos.GetGridIndex(tab: 1, row: 2, index: 3);
```

`TabSlot` 是页面槽位；`GridIndex` 是物品/配方格子字段。不要把两者当成同一个概念。

## 功能：选择数据阶段

- `CoreDataPhase.Data`：初始数据声明，适合新增 Proto 和基础字段。
- `CoreDataPhase.DataUpdates`：跨模组数据调整，适合依赖其他模组声明后的修改。
- `CoreDataPhase.DataFinalFixes`：最终修正，适合在派生缓存重建前收口。

不要把所有逻辑都塞进 `DataFinalFixes`。只有确实依赖其他声明完成后的收口逻辑才放到最终修正阶段。

## 调用后 DSPCore 会怎么处理

- 注册阶段只记录 `ProtoType`、`Proto`、`ownerModGuid`、阶段、类型和用途。
- 运行时按阶段取出注册项，并按具体 Proto 类型分组写入对应 LDB `ProtoSet`。
- 如果找不到对应 LDB `ProtoSet`，该组会被跳过并写日志。
- 每个阶段只应用一次，重复触发不会重复插入同一阶段的数据。
- 最终修正后会重建 LDB 索引、物品派生缓存、模型派生缓存、配方派生缓存、自定义配方类型、信号索引和图标缓存。

## 功能：读取原版数据描述

`DspCore.Vanilla.GetItem(...)`、`GetRecipe(...)`、`GetTech(...)` 当前返回的是读取描述，用于记录“谁要读什么原版数据”。它不是直接返回 LDB 对象的查询 API。

## 这个模块不负责什么

- 不负责建造栏位置；创建物品后需要调用 BuildBar。
- 不负责图标资源加载；需要图标时调用 Icons。
- 不负责本地化字符串；需要文本时调用 Resources。
- 不负责分配页面槽位；需要新页面时调用 Tabs，并用返回的 `TabSlot` 生成物品/配方 `GridIndex`。
- 不负责自定义配方类型限制；需要机器可用性保护时调用 RecipeTypes。
- 当前 Proto 阶段挂点是保守的第一版桥接，不是最终 VFPreload 中段生命周期。

## 示例

- `Examples/ProtoPhases.md`
- `Examples/ProtoPhasesExample.cs`
