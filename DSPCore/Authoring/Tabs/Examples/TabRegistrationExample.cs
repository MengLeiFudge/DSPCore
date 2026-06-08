using DSPCore;

namespace ExampleMod;

// 本文件是文档示例，不参与 DSPCore 编译。
// This file is a documentation example and is excluded from DSPCore compilation.
//
// 用途：
// - Tabs 声明一个作者可见页面，并返回该页面的 TabSlot。
// - GridIndex 是 ItemProto / RecipeProto 自己的格子字段。
// - DSPCore 运行时负责把 TabSlot 和 GridIndex 投射到支持的界面，例如物品选择器、配方选择器、制造器页面、信号选择器和标签图标选择器。
// - DSPCore 不按第三方插件 GUID 跳过原版 signal/tag picker 注入；
//   真正自建且不复用原版 picker 的第三方界面需要专门适配。
//
// Usage:
// - Register once during startup.
// - Use a stable Id so saved UI state and compatibility reports can identify this tab.
// - Use the returned TabSlot when assigning ItemProto.GridIndex or RecipeProto.GridIndex.
public static class TabRegistrationExample
{
    public static void Register(ItemProto itemProto, RecipeProto recipeProto)
    {
        TabSlot machinesTab = Tabs.AddTab(new CoreTabDescriptor(
            // Id 建议包含 mod 前缀，避免不同模组冲突。
            // Prefix the id with your mod id to avoid cross-mod conflicts.
            Id: "example-machines",
            OwnerModGuid: "com.example.my-mod",
            Title: "Example Machines",

            // IconId 应指向 Icons.Register 中注册过的图标。
            // IconId should point to an icon registered through Icons.Register.
            IconId: "example-tab-icon",

            // Order 用于多个自定义分页之间排序。
            // Order sorts this tab button among other custom tab buttons.
            Order: 100));

        // GridIndex 是物品/配方自己的游戏原生格子字段。
        // GridIndex is the native game cell field on items and recipes.
        itemProto.GridIndex = ProtoRegistration.GetGridIndex(machinesTab, row: 1, index: 5);
        recipeProto.GridIndex = ProtoRegistration.GetGridIndex(machinesTab, row: 1, index: 5);
    }
}
