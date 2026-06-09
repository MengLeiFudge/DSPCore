namespace DSPCore;

/// <summary>
/// 表示一个已注册的 DSPCore 设置页面上下文。
/// Represents a registered DSPCore option page context.
/// </summary>
public sealed class OptionPage
{
    internal OptionPage(OptionPageDescriptor descriptor)
    {
        Descriptor = descriptor;
    }

    /// <summary>
    /// 页面描述。
    /// Page descriptor.
    /// </summary>
    public OptionPageDescriptor Descriptor { get; }

    /// <summary>
    /// 页面稳定 ID。
    /// Stable page ID.
    /// </summary>
    public string PageId => Descriptor.PageId;

    /// <summary>
    /// 所属模组 GUID。
    /// Owner mod GUID.
    /// </summary>
    public string OwnerModGuid => Descriptor.OwnerModGuid;

    /// <summary>
    /// 页面标题。
    /// Page title.
    /// </summary>
    public string Title => Descriptor.Title;

    /// <summary>
    /// 获取页面内的配置分区上下文。
    /// Gets an option section context within this page.
    /// </summary>
    /// <param name="section">配置分区。Config section.</param>
    /// <returns>配置分区上下文。Option section context.</returns>
    public OptionSection Section(string section)
    {
        return new OptionSection(this, section);
    }

    /// <summary>
    /// 在当前页面注册并读取一个字符串配置项。
    /// Registers and reads a string option on this page.
    /// </summary>
    /// <param name="section">配置分区。Config section.</param>
    /// <param name="key">配置键。Config key.</param>
    /// <param name="defaultValue">默认值。Default value.</param>
    /// <param name="description">配置说明。Config description.</param>
    /// <param name="ui">展示元数据。Presentation metadata.</param>
    /// <returns>当前字符串值。Current string value.</returns>
    public string String(string section, string key, string defaultValue, string description, OptionUi? ui = null)
    {
        return Options.String(section, key, defaultValue, description, WithPage(ui));
    }

    /// <summary>
    /// 在当前页面注册并读取一个布尔配置项。
    /// Registers and reads a boolean option on this page.
    /// </summary>
    /// <param name="section">配置分区。Config section.</param>
    /// <param name="key">配置键。Config key.</param>
    /// <param name="defaultValue">默认值。Default value.</param>
    /// <param name="description">配置说明。Config description.</param>
    /// <param name="ui">展示元数据。Presentation metadata.</param>
    /// <returns>当前布尔值。Current boolean value.</returns>
    public bool Bool(string section, string key, bool defaultValue, string description, OptionUi? ui = null)
    {
        return Options.Bool(section, key, defaultValue, description, WithPage(ui));
    }

    /// <summary>
    /// 在当前页面注册并读取一个整数配置项。
    /// Registers and reads an integer option on this page.
    /// </summary>
    /// <param name="section">配置分区。Config section.</param>
    /// <param name="key">配置键。Config key.</param>
    /// <param name="defaultValue">默认值。Default value.</param>
    /// <param name="description">配置说明。Config description.</param>
    /// <param name="ui">展示元数据。Presentation metadata.</param>
    /// <returns>当前整数值。Current integer value.</returns>
    public int Int(string section, string key, int defaultValue, string description, OptionUi? ui = null)
    {
        return Options.Int(section, key, defaultValue, description, WithPage(ui));
    }

    /// <summary>
    /// 在当前页面注册并读取一个浮点配置项。
    /// Registers and reads a floating-point option on this page.
    /// </summary>
    /// <param name="section">配置分区。Config section.</param>
    /// <param name="key">配置键。Config key.</param>
    /// <param name="defaultValue">默认值。Default value.</param>
    /// <param name="description">配置说明。Config description.</param>
    /// <param name="ui">展示元数据。Presentation metadata.</param>
    /// <returns>当前浮点值。Current floating-point value.</returns>
    public float Float(string section, string key, float defaultValue, string description, OptionUi? ui = null)
    {
        return Options.Float(section, key, defaultValue, description, WithPage(ui));
    }

    /// <summary>
    /// 在当前页面注册并读取一个枚举配置项。
    /// Registers and reads an enumeration option on this page.
    /// </summary>
    /// <typeparam name="TEnum">枚举类型。Enumeration type.</typeparam>
    /// <param name="section">配置分区。Config section.</param>
    /// <param name="key">配置键。Config key.</param>
    /// <param name="defaultValue">默认值。Default value.</param>
    /// <param name="description">配置说明。Config description.</param>
    /// <param name="ui">展示元数据。Presentation metadata.</param>
    /// <returns>当前枚举值。Current enumeration value.</returns>
    public TEnum Enum<TEnum>(string section, string key, TEnum defaultValue, string description, OptionUi? ui = null)
        where TEnum : struct, System.Enum
    {
        return Options.Enum(section, key, defaultValue, description, WithPage(ui));
    }

    /// <summary>
    /// 在当前页面注册并读取一个整数范围配置项。
    /// Registers and reads an integer range option on this page.
    /// </summary>
    /// <param name="section">配置分区。Config section.</param>
    /// <param name="key">配置键。Config key.</param>
    /// <param name="defaultValue">默认值。Default value.</param>
    /// <param name="description">配置说明。Config description.</param>
    /// <param name="minimum">最小值。Minimum value.</param>
    /// <param name="maximum">最大值。Maximum value.</param>
    /// <param name="step">步进值。Step value.</param>
    /// <returns>当前整数值。Current integer value.</returns>
    public int IntRange(string section, string key, int defaultValue, string description, int minimum, int maximum, int step = 1)
    {
        return Options.IntRange(section, key, defaultValue, description, minimum, maximum, step, PageId);
    }

    /// <summary>
    /// 在当前页面注册并读取一个带展示元数据的整数范围配置项。
    /// Registers and reads an integer range option with presentation metadata on this page.
    /// </summary>
    /// <param name="section">配置分区。Config section.</param>
    /// <param name="key">配置键。Config key.</param>
    /// <param name="defaultValue">默认值。Default value.</param>
    /// <param name="description">配置说明。Config description.</param>
    /// <param name="minimum">最小值。Minimum value.</param>
    /// <param name="maximum">最大值。Maximum value.</param>
    /// <param name="ui">展示元数据。Presentation metadata.</param>
    /// <param name="step">步进值。Step value.</param>
    /// <returns>当前整数值。Current integer value.</returns>
    public int IntRange(string section, string key, int defaultValue, string description, int minimum, int maximum, OptionUi? ui, int step = 1)
    {
        return Options.IntRange(section, key, defaultValue, description, minimum, maximum, WithPage(ui), step);
    }

    /// <summary>
    /// 在当前页面注册并读取一个浮点范围配置项。
    /// Registers and reads a floating-point range option on this page.
    /// </summary>
    /// <param name="section">配置分区。Config section.</param>
    /// <param name="key">配置键。Config key.</param>
    /// <param name="defaultValue">默认值。Default value.</param>
    /// <param name="description">配置说明。Config description.</param>
    /// <param name="minimum">最小值。Minimum value.</param>
    /// <param name="maximum">最大值。Maximum value.</param>
    /// <param name="step">步进值。Step value.</param>
    /// <returns>当前浮点值。Current floating-point value.</returns>
    public float FloatRange(string section, string key, float defaultValue, string description, float minimum, float maximum, float step = 0f)
    {
        return Options.FloatRange(section, key, defaultValue, description, minimum, maximum, step, PageId);
    }

    /// <summary>
    /// 在当前页面注册并读取一个带展示元数据的浮点范围配置项。
    /// Registers and reads a floating-point range option with presentation metadata on this page.
    /// </summary>
    /// <param name="section">配置分区。Config section.</param>
    /// <param name="key">配置键。Config key.</param>
    /// <param name="defaultValue">默认值。Default value.</param>
    /// <param name="description">配置说明。Config description.</param>
    /// <param name="minimum">最小值。Minimum value.</param>
    /// <param name="maximum">最大值。Maximum value.</param>
    /// <param name="ui">展示元数据。Presentation metadata.</param>
    /// <param name="step">步进值。Step value.</param>
    /// <returns>当前浮点值。Current floating-point value.</returns>
    public float FloatRange(string section, string key, float defaultValue, string description, float minimum, float maximum, OptionUi? ui, float step = 0f)
    {
        return Options.FloatRange(section, key, defaultValue, description, minimum, maximum, WithPage(ui), step);
    }

    internal OptionUi WithPage(OptionUi? ui)
    {
        var metadata = ui ?? OptionUi.Empty;
        return new OptionUi(PageId, metadata.DisplayName)
        {
            Order = metadata.Order,
            CanReset = metadata.CanReset
        };
    }
}
