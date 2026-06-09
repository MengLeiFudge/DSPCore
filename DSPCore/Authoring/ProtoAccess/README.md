# 原型访问

ProtoAccess 承接“在第二层/第三层获取和修改他人注册数据”的作者能力。

## 这个模块带来什么便利

- `ProtoPhaseContext` 会暴露当前阶段可见的 Proto 查询入口。
- 常见查询可以直接用 `data.FindItem(...)`、`data.FindRecipe(...)`、`data.FindTech(...)` 和 `data.FindTutorial(...)`。
- 需要枚举或判断阶段边界时使用 `data.Access`，例如 `data.Access.Items()` 和 `data.Access.CanMutate`。
- 返回的是当前阶段使用的 Proto 对象引用；在 `DataUpdates` 和 `DataFinalFixes` 里修改字段，会在 DSPCore 写入 LDB 或重建派生缓存前生效。

## 功能：在 DataUpdates / DataFinalFixes 中调整数据

```csharp
ProtoRegistration.DataUpdates("com.example.my-mod", data =>
{
    ItemProto item = data.FindItem(1001);
    if (item != null)
    {
        item.GridIndex = GridIndexes.From(tab: 3, row: 1, index: 5);
    }
});
```

`Find*` 会优先返回 DSPCore 注册表里当前阶段可见的对象，再回退到当前 LDB 对象。它适合跨模组调整前面阶段已经声明的物品、配方、科技或教程。

## 边界

- `Data` 阶段仍以声明新 Proto 为主；跨模组修改应放在 `DataUpdates` 或 `DataFinalFixes`。
- `CanMutate` 只表达阶段语义，不会冻结返回对象；作者仍需要按阶段约定使用。
- `Items()` / `Recipes()` / `Techs()` / `Tutorials()` 会把当前 LDB 和可见注册项按 ID 合并；同 ID 的后注册对象覆盖前者。
- `VanillaDataView` 仍保留为描述式读取请求，不作为直接 LDB 查询 API 主推。

## 示例

- `Examples/ProtoAccess.md`
- `Examples/ProtoAccessExample.cs`
