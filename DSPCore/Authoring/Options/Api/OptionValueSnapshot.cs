namespace DSPCore;

/// <summary>
/// 表示一个可导出或导入的配置值快照。
/// Represents an exportable or importable option value snapshot.
/// </summary>
/// <param name="Section">配置分区。Config section.</param>
/// <param name="Key">配置键。Config key.</param>
/// <param name="Value">配置值。Config value.</param>
public sealed record OptionValueSnapshot(string Section, string Key, string Value);
