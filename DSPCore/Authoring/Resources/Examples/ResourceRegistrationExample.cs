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
        ModResources.Root(
            id: "example-icons",
            ownerModGuid: "com.example.my-mod",
            keyword: "icons",
            rootPath: "assets/icons");

        ModResources.Text(
            key: "ExampleMachines",
            language: "zhCN",
            value: "示例机器",
            ownerModGuid: "com.example.my-mod");

        ModResources.Text(
            key: "ExampleMachines",
            language: "enUS",
            value: "Example Machines",
            ownerModGuid: "com.example.my-mod");
    }
}
