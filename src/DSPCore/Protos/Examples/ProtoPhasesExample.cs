using DSPCore;

namespace ExampleMod;

// 本文件是文档示例，不参与 DSPCore 编译。
// This file is a documentation example and is excluded from DSPCore compilation.
//
// 用途：
// - Protos 负责把 ItemProto、RecipeProto、TechProto、TutorialProto 等注册到 DSPCore 数据阶段。
// - DSPCore 当前使用三段：Data、DataUpdates、DataFinalFixes。
// - 物品/配方/科技/指引各自仍可以拆成更细功能，但都通过 Protos 进入游戏 LDB。
//
// 阶段建议：
// - Data：声明基础 Proto，例如新物品和基础模型。
// - DataUpdates：依赖基础 Proto 的更新，例如配方引用刚注册的物品。
// - DataFinalFixes：最终修正，例如 tutorial 链路、跨模组补丁或最终缓存前的调整。
//
// Usage:
// - Register during startup before DSPCore applies proto phases.
// - Use ownerModGuid to make conflicts and reports traceable.
public static class ProtoPhasesExample
{
    public static void Register(ItemProto itemProto, RecipeProto recipeProto, TutorialProto tutorialProto)
    {
        // 基础物品通常放在 Data 阶段。
        // Base item declarations usually belong in Data.
        Protos.RegisterItem(
            proto: itemProto,
            ownerModGuid: "com.example.my-mod",
            phase: CoreDataPhase.Data,
            purpose: "Declare the base item");

        // 配方依赖物品 ID，因此放在 DataUpdates 更直观。
        // Recipes depend on item ids, so DataUpdates is clearer.
        Protos.RegisterRecipe(
            proto: recipeProto,
            ownerModGuid: "com.example.my-mod",
            phase: CoreDataPhase.DataUpdates,
            purpose: "Attach recipe after item declarations");

        // Tutorial 或跨模组最终链路修正适合 DataFinalFixes。
        // Tutorial and final cross-mod chain fixes fit DataFinalFixes.
        Protos.RegisterTutorial(
            proto: tutorialProto,
            ownerModGuid: "com.example.my-mod",
            phase: CoreDataPhase.DataFinalFixes,
            purpose: "Final tutorial chain fix");
    }
}
