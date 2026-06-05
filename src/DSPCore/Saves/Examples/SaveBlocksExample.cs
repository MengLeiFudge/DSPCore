using System.IO;
using DSPCore;

namespace ExampleMod;

// 本文件是文档示例，不参与 DSPCore 编译。
// This file is a documentation example and is excluded from DSPCore compilation.
//
// 用途：
// - SaveBlockFormat 用于“标签化分块保存”。
// - 每个字段块都有 Tag，读取时未知 Tag 可以跳过，方便以后增删字段。
// - 这比手写固定顺序 BinaryReader/BinaryWriter 更适合长期维护。
//
// Usage:
// - 在 ICoreSaveHandler.Export 中调用 Export(writer)。
// - 在 ICoreSaveHandler.Import 中调用 Import(reader)。
// - 当你新增字段时，新增一个 SaveBlock；旧存档不会因为没有该 Tag 而崩溃。
public static class SaveBlocksExample
{
    private static bool enabled;
    private static int counter;

    public static void Export(BinaryWriter writer)
    {
        // 写入时每个 block 会带 tag 和长度。
        // Each written block carries its tag and byte length.
        SaveBlock[] blocks =
        [
            new SaveBlock(
                Tag: "Settings",
                Write: blockWriter => blockWriter.Write(enabled),
                Read: blockReader => enabled = blockReader.ReadBoolean()),
            new SaveBlock(
                Tag: "Counter",
                Write: blockWriter => blockWriter.Write(counter),
                Read: blockReader => counter = blockReader.ReadInt32())
        ];

        SaveBlockFormat.WriteBlocks(writer, blocks);
    }

    public static void Import(BinaryReader reader)
    {
        // 读取时只处理这里声明过的 tag；未知 tag 会被跳过。
        // Only declared tags are read; unknown tags are skipped.
        SaveBlock[] blocks =
        [
            new SaveBlock(
                Tag: "Settings",
                Write: blockWriter => blockWriter.Write(enabled),
                Read: blockReader => enabled = blockReader.ReadBoolean()),
            new SaveBlock(
                Tag: "Counter",
                Write: blockWriter => blockWriter.Write(counter),
                Read: blockReader => counter = blockReader.ReadInt32())
        ];

        SaveBlockFormat.ReadBlocks(reader, blocks);
    }
}
