using DSPCore;

namespace ExampleMod;

// 本文件是文档示例，不参与 DSPCore 编译。
// This file is a documentation example and is excluded from DSPCore compilation.
//
// 用途：
// - 已经拿到 TechProto 时，用对象扩展方法完成注册。
// - 科技依赖物品或配方时，通常放在 DataUpdates。
//
// Usage:
// - Prepare the TechProto fields before calling this chain.
// - Register before DSPCore applies proto phases.
public static class TechAuthoringExample
{
    public static void Register(TechProto techProto)
    {
        techProto.RegisterTech(
            ownerModGuid: "com.example.my-mod",
            phase: CoreDataPhase.DataUpdates,
            purpose: "Unlock example machine");
    }
}
