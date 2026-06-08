using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DSPCore;

/// <summary>
/// 提供 tagged block 存档读写工具。
/// Provides tagged block save read/write helpers.
/// </summary>
public static class SaveBlockFormat
{
    /// <summary>
    /// 写入多个 tagged block。
    /// Writes multiple tagged blocks.
    /// </summary>
    /// <param name="writer">目标写入器。Target writer.</param>
    /// <param name="blocks">存档块。Save blocks.</param>
    public static void WriteBlocks(BinaryWriter writer, IEnumerable<SaveBlock> blocks)
    {
        var blockList = blocks.ToArray();
        writer.Write(blockList.Length);
        foreach (var block in blockList)
        {
            using var stream = new MemoryStream();
            using (var blockWriter = new BinaryWriter(stream, Encoding.UTF8, true))
            {
                block.Write(blockWriter);
            }

            writer.Write(block.Tag);
            writer.Write((int)stream.Length);
            writer.Write(stream.GetBuffer(), 0, (int)stream.Length);
        }
    }

    /// <summary>
    /// 读取多个 tagged block，未知 tag 会被跳过。
    /// Reads multiple tagged blocks; unknown tags are skipped.
    /// </summary>
    /// <param name="reader">源读取器。Source reader.</param>
    /// <param name="blocks">存档块。Save blocks.</param>
    /// <param name="onBlockError">单块错误回调。Single block error callback.</param>
    public static void ReadBlocks(BinaryReader reader, IEnumerable<SaveBlock> blocks, Action<string, Exception>? onBlockError = null)
    {
        var handlers = blocks.ToDictionary(item => item.Tag, item => item.Read, StringComparer.Ordinal);
        int count = reader.ReadInt32();
        for (int i = 0; i < count; i++)
        {
            string tag = reader.ReadString();
            int length = reader.ReadInt32();
            byte[] data = reader.ReadBytes(length);

            if (!handlers.TryGetValue(tag, out var read))
            {
                continue;
            }

            try
            {
                using var stream = new MemoryStream(data);
                using var blockReader = new BinaryReader(stream, Encoding.UTF8);
                read(blockReader);
            }
            catch (Exception ex)
            {
                onBlockError?.Invoke(tag, ex);
            }
        }
    }
}
