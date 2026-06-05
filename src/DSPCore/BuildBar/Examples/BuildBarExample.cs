using DSPCore;

namespace ExampleMod;

// 本文件是文档示例，不参与 DSPCore 编译。
// This file is a documentation example and is excluded from DSPCore compilation.
//
// 用途：
// - BuildBar 只负责把某个物品绑定到建造栏槽位。
// - 它不创建 ItemProto，也不决定物品属于哪个分页。
// - 物品创建、图标、配方、本地化等功能应先由对应功能块处理，然后再调用 BuildBar。
//
// 槽位模型：
// - tab：建造栏分类，从 1 开始。
// - row：建造栏行，从 1 开始；row = 1 是原版行，row > 1 是 DSPCore 扩展行。
// - index：按钮位置，从 1 开始。
//
// Usage:
// - Call BuildBar.BindItem after the item id is known.
// - Prefer the tab/row/index API over legacy BuildBarTool semantics.
public static class BuildBarExample
{
    public static void Register(ItemProto myItemProto)
    {
        // 直接用物品 ID 绑定。
        // Bind by item id.
        BuildBar.BindItem(tab: 3, row: 2, index: 4, itemId: 9554);

        // 如果你刚创建了 ItemProto，也可以直接传 ItemProto。
        // Bind by ItemProto when your feature just created the proto.
        BuildBar.BindItem(tab: 3, row: 2, index: 5, item: myItemProto);
    }

    public static void RegisterLegacy()
    {
        // 旧 BuildBarTool 调用仍可兼容，但新模组不要继续使用。
        // Legacy BuildBarTool calls are accepted for compatibility only.
#pragma warning disable CS0618
        BuildBarTool.BuildBarTool.SetBuildBar(3, 4, 9554, true);
#pragma warning restore CS0618
    }
}
