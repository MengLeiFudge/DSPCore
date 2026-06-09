# ItemAuthoringChain

本场景用于把已经准备好的 `ItemProto` 接入 DSPCore 的作者侧短链路。

## 适用时机

- 物品字段已经由模组按 DSP 语义准备好。
- 需要给物品设置分页位置、绑定图标并注册到 ProtoPipeline。
- 同一个模组资源根会被多个图标复用。

## 关键参数

- `ItemProto`：要注册的物品原型。
- `TabSlot` / `tab`、`row`、`index`：用于生成原版 `GridIndex`。
- `ModResourcePack`：复用 owner、资源根和默认 assembly 的资源包短入口。
- `CoreDataPhase`：物品声明通常放在 `Data`。

## 运行时前提

在 DSPCore 应用 proto 阶段前完成注册。`BindIcon(...)` 只登记图标 descriptor，目标物品必须在图标 runtime 应用时已经进入 LDB。

## 常见误用

- 不要把 Items 当成自动构造器；它不填充 `ItemProto` 的业务字段。
- 不要把建造栏位置塞进 `GridIndex`；建造栏仍使用 `ItemProto.SetBuildBar(...)`。

代码示例见 `ItemAuthoringChainExample.cs`。
