using DSPCore;

namespace ExampleMod;

// 本文件是文档示例，不参与 DSPCore 编译。
// This file is a documentation example and is excluded from DSPCore compilation.
//
// 用途：
// - ModResources.Root 只登记资源归属和根路径。
// - ModResources.Text 会把本地化 key/value 交给 DSPCore 的本地化桥接。
//
// Usage:
// - Register resource metadata before other capabilities try to consume it.
// - Keep localization keys stable because UI, Tabs, and Proto names may reference them.
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
    }
}
