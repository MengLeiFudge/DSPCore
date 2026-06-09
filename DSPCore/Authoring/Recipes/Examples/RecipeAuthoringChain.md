# RecipeAuthoringChain

本场景用于把已经准备好的 `RecipeProto` 接入 DSPCore 的作者侧短链路。

## 适用时机

- 配方字段已经由模组按 DSP 语义准备好。
- 需要给配方设置分页位置、绑定图标并注册到 ProtoPipeline。
- 配方依赖的物品先在 `Data` 阶段声明，配方本身适合放在 `DataUpdates`。

## 关键参数

- `RecipeProto`：要注册的配方原型。
- `TabSlot` / `tab`、`row`、`index`：用于生成原版 `GridIndex`。
- `ModResourcePack`：复用 owner、资源根和默认 assembly 的资源包短入口。
- `CoreDataPhase.DataUpdates`：常用于依赖已声明物品的配方。

## 运行时前提

在 DSPCore 应用 proto 阶段前完成注册。`BindIcon(...)` 只登记图标 descriptor，目标配方必须在图标 runtime 应用时已经进入 LDB。

## 常见误用

- 不要把 Recipes 当成自动构造器；它不填充 `RecipeProto` 的业务字段。
- 不要在 Recipes 中声明机器可用性限制；自定义配方类型和机器过滤属于 GameEnums。

代码示例见 `RecipeAuthoringChainExample.cs`。
