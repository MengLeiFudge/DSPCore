using System;
using System.Collections.Generic;

namespace DSPCore;

/// <summary>
/// 管理作者声明的配置项。
/// Manages author-declared options.
/// </summary>
public sealed class OptionRegistry
{
    private readonly Dictionary<string, OptionDescriptor> descriptors = new(StringComparer.Ordinal);
    private readonly Dictionary<string, OptionPageDescriptor> pages = new(StringComparer.Ordinal);
    private readonly Dictionary<string, OptionVersionDescriptor> versions = new(StringComparer.Ordinal);

    /// <summary>
    /// 注册或替换一个配置项。
    /// Registers or replaces an option.
    /// </summary>
    /// <param name="descriptor">配置项描述。Option descriptor.</param>
    public void Register(OptionDescriptor descriptor)
    {
        descriptors[KeyOf(descriptor.Section, descriptor.Key)] = descriptor;
        OptionRuntime.BindIfReady(descriptor);
    }

    /// <summary>
    /// 注册或替换一个设置页面描述。
    /// Registers or replaces an option page descriptor.
    /// </summary>
    /// <param name="descriptor">页面描述。Page descriptor.</param>
    public void RegisterPage(OptionPageDescriptor descriptor)
    {
        pages[descriptor.PageId] = descriptor;
    }

    /// <summary>
    /// 注册或替换一个设置版本描述。
    /// Registers or replaces an option version descriptor.
    /// </summary>
    /// <param name="descriptor">版本描述。Version descriptor.</param>
    public void RegisterVersion(OptionVersionDescriptor descriptor)
    {
        versions[descriptor.OwnerModGuid] = descriptor;
    }

    /// <summary>
    /// 获取配置项字符串值。
    /// Gets an option string value.
    /// </summary>
    /// <param name="section">配置分区。Config section.</param>
    /// <param name="key">配置键。Config key.</param>
    /// <returns>当前值，未绑定时返回空字符串。Current value, or empty string when unbound.</returns>
    public string GetString(string section, string key)
    {
        return OptionRuntime.GetString(section, key);
    }

    /// <summary>
    /// 获取布尔配置值。
    /// Gets a boolean option value.
    /// </summary>
    /// <param name="section">配置分区。Config section.</param>
    /// <param name="key">配置键。Config key.</param>
    /// <param name="fallback">解析失败时的默认值。Fallback used when parsing fails.</param>
    /// <returns>当前布尔值。Current boolean value.</returns>
    public bool GetBool(string section, string key, bool fallback = false)
    {
        return bool.TryParse(GetString(section, key), out var value) ? value : fallback;
    }

    /// <summary>
    /// 获取整数配置值。
    /// Gets an integer option value.
    /// </summary>
    /// <param name="section">配置分区。Config section.</param>
    /// <param name="key">配置键。Config key.</param>
    /// <param name="fallback">解析失败时的默认值。Fallback used when parsing fails.</param>
    /// <returns>当前整数值。Current integer value.</returns>
    public int GetInt(string section, string key, int fallback = 0)
    {
        return int.TryParse(GetString(section, key), out var value) ? value : fallback;
    }

    /// <summary>
    /// 获取所有配置项描述快照。
    /// Gets a snapshot of all option descriptors.
    /// </summary>
    /// <returns>配置项描述集合。Option descriptor collection.</returns>
    public IReadOnlyCollection<OptionDescriptor> GetAll()
    {
        return descriptors.Values;
    }

    /// <summary>
    /// 获取所有设置页面描述快照。
    /// Gets a snapshot of all option page descriptors.
    /// </summary>
    /// <returns>页面描述集合。Page descriptor collection.</returns>
    public IReadOnlyCollection<OptionPageDescriptor> GetPages()
    {
        return pages.Values;
    }

    /// <summary>
    /// 获取所有设置版本描述快照。
    /// Gets a snapshot of all option version descriptors.
    /// </summary>
    /// <returns>版本描述集合。Version descriptor collection.</returns>
    public IReadOnlyCollection<OptionVersionDescriptor> GetVersions()
    {
        return versions.Values;
    }

    internal static string KeyOf(string section, string key)
    {
        return section + "\u001f" + key;
    }
}
