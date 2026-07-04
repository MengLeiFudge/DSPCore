using DSPCore;

namespace ExampleMod;

// 本文件是文档示例，不参与 DSPCore 编译。
// This file is a documentation example and is excluded from DSPCore compilation.
//
// 用途：
// - 已经拿到 RecipeProto 时，用对象扩展方法完成 GridIndex、图标绑定和注册。
// - 配方通常依赖已声明物品，因此常放在 DataUpdates。
//
// Usage:
// - Prepare the RecipeProto fields before calling this chain.
// - Register before DSPCore applies proto phases.
public static class RecipeAuthoringChainExample
{
    public static void Register(RecipeProto recipeProto, TabSlot tab)
    {
        var pack = ModResources.Pack(
            ownerModGuid: "com.example.my-mod",
            rootPath: "assets/icons",
            assembly: typeof(RecipeAuthoringChainExample).Assembly);

        recipeProto
            .SetIconTag("example-recipe")
            .SetNonProductive()
            .SetGridIndex(tab, row: 1, index: 6)
            .BindIcon(pack, "example-recipe", "example-recipe.png")
            .RegisterRecipe("com.example.my-mod", CoreDataPhase.DataUpdates, "Attach example recipe");
    }
}
