using DSPCore;

namespace ExampleMod;

// 本文件是文档示例，不参与 DSPCore 编译。
// This file is a documentation example and is excluded from DSPCore compilation.
//
// 用途：
// - GameEnums 当前用于声明一组配方属于自定义配方类型。
// - 它不创建 RecipeProto；配方创建应先由 ProtoRegistration 完成。
// - 它负责把已有配方和允许使用这些配方的机器关联起来。
//
// Usage:
// - Register after recipe ids and machine item ids are known.
// - Keep RecipeIds and AssemblerItemIds stable across versions when possible.
public static class RecipeTypeRegistrationExample
{
    public static void Register()
    {
        RecipeTypes.Register(new RecipeTypeDescriptor(
            Id: "example.special-smelting",
            OwnerModGuid: "com.example.my-mod",
            DisplayName: "Special Smelting",

            // 这些配方会被标记为自定义类型。
            // These recipes will be treated as a custom recipe type.
            RecipeIds: new[] { 955401, 955402 },

            // 只有这些制作器/机器能使用上面的配方。
            // Only these assembler or machine item ids can use those recipes.
            AssemblerItemIds: new[] { 9554 }));
    }
}
