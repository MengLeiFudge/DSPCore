# 配方注册

Recipes 是配方原型注册入口。它只表达“注册 RecipeProto”这一个作者能力；三阶段执行时机由 DataPhases 提供，底层写入和缓存重建由 Systems/ProtoPipeline 处理。

## 这个模块带来什么便利

- 已经拿到 `RecipeProto` 时，可以直接在对象上设置 `GridIndex`、绑定图标并注册；普通路径、嵌入 PNG 和 AssetBundle 图标都有对象入口。
- 配方依赖物品 ID 时，仍可通过 `CoreDataPhase.DataUpdates` 表达“物品先声明，配方后挂接”的时机。
- `Recipes.Register(...)` 仍保留为低层入口。

## 功能：对象链式注册

```csharp
var pack = ModResources.Pack("com.example.my-mod", "assets/icons", typeof(MyPlugin).Assembly);

recipeProto
    .SetGridIndex(tab, row: 1, index: 6)
    .BindIcon(pack, "example-recipe", "example-recipe.png")
    .RegisterRecipe("com.example.my-mod", CoreDataPhase.DataUpdates, "Attach example recipe");
```

`SetGridIndex(...)` 写入原版 `RecipeProto.GridIndex`。`BindIcon(...)`、`BindEmbeddedIcon(...)` 和 `BindAssetBundleIcon(...)` 只注册图标 descriptor，图标仍由 Icons runtime 在缓存重建时应用。`RegisterRecipe(...)` 会把当前配方登记到 DSPCore 的 ProtoPipeline。

## 功能：低层注册

```csharp
Recipes.Register(recipeProto, "com.example.my-mod", CoreDataPhase.DataUpdates, "Attach example recipe");
```

当前 `Recipes.Register(...)` 仍是 `ProtoRegistration.RegisterRecipe(...)` 的薄入口。配方类型或机器可用性限制不属于这里，放在 GameEnums。

## 这个模块不负责什么

- 不自动创建 `RecipeProto`，配方字段仍由作者按 DSP 语义准备。
- 不负责自定义配方类型或机器可用性限制；这些属于 GameEnums。
- 不负责科技解锁、教程链路或玩家便利页面。

## 示例

- `Examples/RecipeAuthoringChain.md`
- `Examples/RecipeAuthoringChainExample.cs`
