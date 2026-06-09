using DSPCore;

namespace ExampleMod;

// 本文件是文档示例，不参与 DSPCore 编译。
// This file is a documentation example and is excluded from DSPCore compilation.
//
// 用途：
// - 已经拿到 TutorialProto 时，用对象扩展方法完成注册。
// - 指引依赖其他 Proto 全部声明完成时，通常放在 DataFinalFixes。
//
// Usage:
// - Prepare the TutorialProto fields before calling this chain.
// - Register before DSPCore applies proto phases.
public static class TutorialAuthoringExample
{
    public static void Register(TutorialProto tutorialProto)
    {
        tutorialProto.RegisterTutorial(
            ownerModGuid: "com.example.my-mod",
            phase: CoreDataPhase.DataFinalFixes,
            purpose: "Show example machine guide");
    }
}
