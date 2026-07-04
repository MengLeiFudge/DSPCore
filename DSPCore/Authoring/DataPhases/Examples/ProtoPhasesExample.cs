using DSPCore;

namespace ExampleMod;

// 本文件是文档示例，不参与 DSPCore 编译。
// This file is a documentation example and is excluded from DSPCore compilation.
//
// 用途：
// - ProtoRegistration 负责注册三段数据回调：Data、DataUpdates、DataFinalFixes。
// - DSPCore 执行阶段时会传入 ProtoPhaseContext。
// - 作者在回调里注册 ItemProto、RecipeProto、TechProto、TutorialProto 等，让它们进入当前阶段。
//
// 阶段建议：
// - Data：声明基础 Proto，例如新物品和基础模型。
// - DataUpdates：依赖基础 Proto 的更新，例如配方引用刚注册的物品。
// - DataFinalFixes：最终修正，例如 tutorial 链路、跨模组补丁或最终缓存前的调整。
//
// Usage:
// - Register callbacks during startup before DSPCore applies proto phases.
// - Use ownerModGuid to make conflicts and reports traceable.
public static class ProtoPhasesExample
{
    public static void Register(ItemProto itemProto, RecipeProto recipeProto, TutorialProto tutorialProto)
    {
        // 基础物品通常放在 Data 阶段。
        // Base item declarations usually belong in Data.
        ProtoRegistration.Data("com.example.my-mod", data =>
        {
            data.RegisterItem(
                    itemProto.SetGridIndex(tab: 3, row: 1, index: 5),
                    ProtoStableId.Of("example-item", preferredId: itemProto.ID),
                    "Declare the base item")
                .SetBuildBar(category: 3, row: 1, index: 5);
        });

        // 配方依赖物品 ID，因此放在 DataUpdates 更直观。
        // Recipe proto objects depend on item ids, so DataUpdates is clearer.
        ProtoRegistration.DataUpdates("com.example.my-mod", data =>
        {
            data.RegisterRecipe(
                recipeProto.SetGridIndex(tab: 3, row: 1, index: 6),
                ProtoStableId.Of("example-recipe", preferredId: recipeProto.ID),
                "Attach recipe after item declarations");
        });

        // Tutorial 或跨模组最终链路修正适合 DataFinalFixes。
        // Tutorial and final cross-mod chain fixes fit DataFinalFixes.
        ProtoRegistration.DataFinalFixes("com.example.my-mod", data =>
        {
            data.RegisterTutorial(tutorialProto, "Final tutorial chain fix");
        });
    }
}
