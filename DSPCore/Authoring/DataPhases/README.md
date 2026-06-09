# 数据阶段

DataPhases 负责作者侧三阶段生命周期：`Data`、`DataUpdates`、`DataFinalFixes`。

- `Data`：初始声明，通常注册自己的物品、配方、科技、指引等 proto。
- `DataUpdates`：跨模组调整，可以通过 `ProtoPhaseContext.FindItem(...)` / `FindRecipe(...)` 或 `data.Access` 读取和修改前面阶段已注册的数据。
- `DataFinalFixes`：最终修正，在系统写入和缓存重建前做收口。

具体 proto 类型注册应优先看 Items、Recipes、Techs、Tutorials；跨模组读取和修改看 ProtoAccess；底层兼容和聚合入口仍保留在 ProtoRegistration。
