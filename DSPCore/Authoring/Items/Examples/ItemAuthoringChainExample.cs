using DSPCore;

namespace ExampleMod;

// 本文件是文档示例，不参与 DSPCore 编译。
// This file is a documentation example and is excluded from DSPCore compilation.
//
// 用途：
// - 已经拿到 ItemProto 时，用对象扩展方法完成 GridIndex、图标绑定和注册。
// - BuildBar 仍是独立能力，使用 ItemProto.SetBuildBar(...)。
//
// Usage:
// - Prepare the ItemProto fields before calling this chain.
// - Register before DSPCore applies proto phases.
public static class ItemAuthoringChainExample
{
    public static void Register(ItemProto itemProto, TabSlot tab)
    {
        var pack = ModResources.Pack(
            ownerModGuid: "com.example.my-mod",
            rootPath: "assets/icons",
            assembly: typeof(ItemAuthoringChainExample).Assembly);

        itemProto
            .SetGridIndex(tab, row: 1, index: 5)
            .BindIcon(pack, "example-machine", "example-machine.png")
            .RegisterItem("com.example.my-mod", purpose: "Declare example machine");

        itemProto.SetBuildBar(tab: 3, row: 2, index: 5);
    }
}
