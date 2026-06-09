using DSPCore;

namespace ExampleMod;

// 本文件是文档示例，不参与 DSPCore 编译。
// This file is a documentation example and is excluded from DSPCore compilation.
//
// 用途：
// - Features.Register 声明功能块级能力。
// - Modules.Register 声明模组内部或跨模组可查询模块。
// - descriptor 路径仍保留给批量注册或配置驱动注册。
//
// Usage:
// - Register once during startup.
// - Use stable ids so other mods can query the declaration.
// - Keep strict initialization ordering inside your own initialization logic when needed.
public static class ModuleDeclarationExample
{
    private const string Owner = "com.example.my-mod";

    public static void Register()
    {
        Features.Register(
            id: Owner + ".machines",
            displayName: "Example Machines",
            initialize: InitializeMachines,
            priority: 100);

        Modules.Register(
            id: Owner + ".compat.target-plugin",
            displayName: "Target Plugin Compatibility",
            initialize: InitializeCompatibility,
            dependencies: new[] { Owner + ".machines" });
    }

    private static void InitializeMachines()
    {
    }

    private static void InitializeCompatibility()
    {
    }
}
