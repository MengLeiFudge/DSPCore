using DSPCore;

namespace ExampleMod;

// 本文件是文档示例，不参与 DSPCore 编译。
// This file is a documentation example and is excluded from DSPCore compilation.
//
// 用途：
// - Tabs 只声明一个作者可见分页。
// - DSPCore 运行时负责把该分页投射到支持的界面，例如物品选择器、配方选择器和制造器页面。
// - 全息信标、蓝图、信号选择器等界面需要更完整的分页内容模型后再支持。
//
// Usage:
// - Register once during startup.
// - Use a stable Id so saved UI state and compatibility reports can identify this tab.
public static class TabsExample
{
    public static void Register()
    {
        Tabs.AddTab(new CoreTabDescriptor(
            // Id 建议包含 mod 前缀，避免不同模组冲突。
            // Prefix the id with your mod id to avoid cross-mod conflicts.
            Id: "example-machines",
            OwnerModGuid: "com.example.my-mod",
            Title: "Example Machines",

            // IconId 应指向 Icons.Register 中注册过的图标。
            // IconId should point to an icon registered through Icons.Register.
            IconId: "example-tab-icon",

            // Order 用于多个自定义分页之间排序。
            // Order sorts this tab among other custom tabs.
            Order: 100));
    }
}
