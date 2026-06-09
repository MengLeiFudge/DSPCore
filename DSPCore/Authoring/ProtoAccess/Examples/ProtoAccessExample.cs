using DSPCore;

namespace ExampleMod;

// 本文件是文档示例，不参与 DSPCore 编译。
// This file is a documentation example and is excluded from DSPCore compilation.
//
// 用途：
// - 在 DataUpdates / DataFinalFixes 中读取当前阶段可见的 Proto。
// - 修改前面阶段或其他模组已经声明的物品、配方、科技或教程。
// - 避免每个模组自己猜 LDB 写入时机和缓存重建时机。
//
// Usage:
// - Register the phase callback during plugin startup.
// - Use DataUpdates for normal cross-mod adjustment.
// - Use DataFinalFixes only for final cleanup before derived caches are rebuilt.
public static class ProtoAccessExample
{
    public static void Register()
    {
        ProtoRegistration.DataUpdates("com.example.my-mod", data =>
        {
            ItemProto item = data.FindItem(1001);
            if (item != null)
            {
                item.GridIndex = GridIndexes.From(tab: 3, row: 1, index: 5);
            }

            RecipeProto recipe = data.FindRecipe(1001);
            if (recipe != null)
            {
                recipe.GridIndex = GridIndexes.From(tab: 3, row: 1, index: 6);
            }
        });

        ProtoRegistration.DataFinalFixes("com.example.my-mod", data =>
        {
            if (!data.Access.CanMutate)
            {
                return;
            }

            foreach (ItemProto item in data.Access.Items())
            {
                // Keep enumeration adjustments narrow. This example only demonstrates
                // that the view can inspect registered and current LDB items together.
                if (item.ID == 1001)
                {
                    item.Description = "Adjusted by com.example.my-mod.";
                }
            }
        });
    }
}
