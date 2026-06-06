using DSPCore;

namespace ExampleMod;

// 本文件是文档示例，不参与 DSPCore 编译。
// This file is a documentation example and is excluded from DSPCore compilation.
//
// 用途：
// - BuildBar 只负责把某个物品绑定到建造栏槽位。
// - 它不创建 ItemProto，也不决定物品属于哪个分页。
// - 物品创建、图标、配方、本地化等功能应先由对应功能块处理，然后把 ItemProto 绑定到快捷栏。
//
// 槽位模型：
// - tab：建造栏分类，从 1 开始。
// - row：建造栏行，从 1 开始；row = 1 是原版行，row > 1 是 DSPCore 扩展行。
// - index：按钮位置，从 1 开始。
//
// Usage:
// - Call ItemProto.SetBuildBar after the item proto is created.
// - Use BuildBar.SetBuildBar only when you only have an item id.
public static class QuickBarBindingExample
{
    public static void Register(ItemProto myItemProto)
    {
        // 首选写法：刚创建 ItemProto 后，直接把它绑定到快捷建造栏。
        // Preferred style: bind the item proto directly after creating it.
        myItemProto.SetBuildBar(tab: 3, row: 2, index: 5);

        // 只有手上只有 item id 时，才使用静态入口。
        // Use the static entry only when you only have an item id.
        BuildBar.BindQuickBar(tab: 3, row: 2, index: 4, itemId: 9554);

        // 需要沿用 BuildIndex 风格时，可从 BuildIndex 拆出 category/index。
        // Use the BuildIndex-style overload when migrating BuildIndex-based code.
        myItemProto.SetBuildBar(buildIndex: myItemProto.BuildIndex, row: 2);
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
