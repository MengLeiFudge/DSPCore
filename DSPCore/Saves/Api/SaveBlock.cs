using System;
using System.IO;

namespace DSPCore;

/// <summary>
/// 描述一个 tagged block 存档块。
/// Describes a tagged save block.
/// </summary>
/// <param name="Tag">块标识。Block tag.</param>
/// <param name="Write">写入回调。Write callback.</param>
/// <param name="Read">读取回调。Read callback.</param>
public sealed record SaveBlock(string Tag, Action<BinaryWriter> Write, Action<BinaryReader> Read);
