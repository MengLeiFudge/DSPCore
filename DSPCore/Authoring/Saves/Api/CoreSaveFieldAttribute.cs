using System;

namespace DSPCore;

/// <summary>
/// 标记可由 DSPCore 自动 schema 存档读写的字段或属性。
/// Marks a field or property for DSPCore automatic schema save I/O.
/// </summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public sealed class CoreSaveFieldAttribute : Attribute
{
    /// <summary>
    /// 创建一个自动存档字段标记。
    /// Creates an automatic save field marker.
    /// </summary>
    /// <param name="tag">稳定字段 tag。Stable field tag.</param>
    /// <param name="order">写出顺序。Write order.</param>
    public CoreSaveFieldAttribute(string tag, int order = 0)
    {
        if (string.IsNullOrWhiteSpace(tag))
        {
            throw new ArgumentException("Save field tag cannot be empty.", nameof(tag));
        }

        Tag = tag;
        Order = order;
    }

    /// <summary>
    /// 稳定字段 tag。
    /// Stable field tag.
    /// </summary>
    public string Tag { get; }

    /// <summary>
    /// 写出顺序。
    /// Write order.
    /// </summary>
    public int Order { get; }
}
