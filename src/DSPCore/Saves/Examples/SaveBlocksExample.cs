using System.IO;
using DSPCore;

namespace ExampleMod;

public static class SaveBlocksExample
{
    private static bool enabled;
    private static int counter;

    public static void Export(BinaryWriter writer)
    {
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
