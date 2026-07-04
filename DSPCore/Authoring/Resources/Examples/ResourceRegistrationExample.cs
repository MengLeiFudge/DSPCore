using DSPCore;

namespace ExampleMod;

// 本文件是文档示例，不参与 DSPCore 编译。
// This file is a documentation example and is excluded from DSPCore compilation.
//
// 用途：
// - ModResources.Root 只登记资源归属和根路径。
// - ModResources.Text 会把本地化 key/value 交给 DSPCore 的本地化桥接。
// - pack.Translate 可以在注册后立即读取 DSPCore 自己的翻译索引。
// - ModResourcePack 的 ItemIcon/RecipeIcon 等 typed helpers 会复用 owner 和 rootPath。
//
// Usage:
// - Register resource metadata before other capabilities try to consume it.
// - Keep localization keys stable because UI, Tabs, and Proto names may reference them.
// - Use ModResources.Translate or pack.Translate when author code needs text before DSP Localization reloads.
// - Use typed icon helpers when the target proto kind is already known from the method name.
public static class ResourceRegistrationExample
{
    public static void Register()
    {
        var pack = ModResources.Pack(
            ownerModGuid: "com.example.my-mod",
            rootPath: "assets",
            assembly: typeof(ResourceRegistrationExample).Assembly);

        pack.Root(id: "example-assets", keyword: "assets");

        pack.Text("ExampleMachines", "zhCN", "示例机器");
        pack.Text("ExampleMachines", "enUS", "Example Machines");
        string zhTitle = pack.Translate("ExampleMachines", language: "zhCN");

        pack.ItemIcon("example-machine-icon", "icons/example-machine.png", itemId: 9554);
    }
}
