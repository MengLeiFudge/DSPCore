using DSPCore;

namespace ExampleMod;

// 本文件是文档示例，不参与 DSPCore 编译。
// This file is a documentation example and is excluded from DSPCore compilation.
//
// 用途：
// - ProtoStableId 负责把“模组内稳定 key”解析成游戏运行时 int ID。
// - preferredId 只是优先候选；冲突时 DSPCore 会分配其他可用 ID。
// - aliases 用于旧 key 迁移，避免版本升级后丢失原有映射。
//
// Usage:
// - Register stable identities before DSPCore applies proto phases.
// - Keep stable keys machine-readable and independent from localization text.
public static class StableProtoIdentityExample
{
    public static void Register(ItemProto itemProto, RecipeProto recipeProto, TabSlot tab)
    {
        ProtoRegistration.Data("com.example.my-mod", data =>
        {
            data.RegisterItem(
                    itemProto.SetGridIndex(tab, row: 1, index: 5),
                    ProtoStableId.Of("example-machine", 12001, "example-machine-old"),
                    purpose: "Declare stable example machine")
                .SetBuildBar(category: 3, row: 1, index: 5);
        });

        ProtoRegistration.DataUpdates("com.example.my-mod", data =>
        {
            data.RegisterRecipe(
                recipeProto.SetGridIndex(tab, row: 1, index: 6),
                ProtoStableId.Of("example-machine-recipe", preferredId: 12002),
                purpose: "Declare stable example machine recipe");
        });
    }
}
