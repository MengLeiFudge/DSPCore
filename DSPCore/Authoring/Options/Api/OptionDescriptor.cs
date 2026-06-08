namespace DSPCore;

/// <summary>
/// 描述一个由 DSPCore 绑定的配置项。
/// Describes a configuration option bound by DSPCore.
/// </summary>
/// <param name="Section">配置分区。Config section.</param>
/// <param name="Key">配置键。Config key.</param>
/// <param name="DefaultValue">默认值。Default value.</param>
/// <param name="Description">配置说明。Config description.</param>
/// <param name="PageId">可选设置页面 ID。Optional settings page ID.</param>
public sealed record OptionDescriptor(string Section, string Key, string DefaultValue, string Description, string? PageId = null);
