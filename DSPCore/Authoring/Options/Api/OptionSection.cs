namespace DSPCore;

/// <summary>
/// 表示一个设置页面内的配置分区上下文。
/// Represents a config section context within an option page.
/// </summary>
public sealed class OptionSection
{
    internal OptionSection(OptionPage page, string section)
    {
        Page = page;
        SectionName = section;
    }

    /// <summary>
    /// 所属设置页面上下文。
    /// Owning option page context.
    /// </summary>
    public OptionPage Page { get; }

    /// <summary>
    /// 配置分区名称。
    /// Config section name.
    /// </summary>
    public string SectionName { get; }

    /// <summary>
    /// 注册并读取一个字符串配置项。
    /// Registers and reads a string option.
    /// </summary>
    /// <param name="key">配置键。Config key.</param>
    /// <param name="defaultValue">默认值。Default value.</param>
    /// <param name="description">配置说明。Config description.</param>
    /// <param name="ui">展示元数据。Presentation metadata.</param>
    /// <returns>当前字符串值。Current string value.</returns>
    public string String(string key, string defaultValue, string description, OptionUi? ui = null)
    {
        return Options.String(SectionName, key, defaultValue, description, Page.WithPage(ui));
    }

    /// <summary>
    /// 注册并读取一个布尔配置项。
    /// Registers and reads a boolean option.
    /// </summary>
    /// <param name="key">配置键。Config key.</param>
    /// <param name="defaultValue">默认值。Default value.</param>
    /// <param name="description">配置说明。Config description.</param>
    /// <param name="ui">展示元数据。Presentation metadata.</param>
    /// <returns>当前布尔值。Current boolean value.</returns>
    public bool Bool(string key, bool defaultValue, string description, OptionUi? ui = null)
    {
        return Options.Bool(SectionName, key, defaultValue, description, Page.WithPage(ui));
    }

    /// <summary>
    /// 注册并读取一个整数配置项。
    /// Registers and reads an integer option.
    /// </summary>
    /// <param name="key">配置键。Config key.</param>
    /// <param name="defaultValue">默认值。Default value.</param>
    /// <param name="description">配置说明。Config description.</param>
    /// <param name="ui">展示元数据。Presentation metadata.</param>
    /// <returns>当前整数值。Current integer value.</returns>
    public int Int(string key, int defaultValue, string description, OptionUi? ui = null)
    {
        return Options.Int(SectionName, key, defaultValue, description, Page.WithPage(ui));
    }

    /// <summary>
    /// 注册并读取一个浮点配置项。
    /// Registers and reads a floating-point option.
    /// </summary>
    /// <param name="key">配置键。Config key.</param>
    /// <param name="defaultValue">默认值。Default value.</param>
    /// <param name="description">配置说明。Config description.</param>
    /// <param name="ui">展示元数据。Presentation metadata.</param>
    /// <returns>当前浮点值。Current floating-point value.</returns>
    public float Float(string key, float defaultValue, string description, OptionUi? ui = null)
    {
        return Options.Float(SectionName, key, defaultValue, description, Page.WithPage(ui));
    }

    /// <summary>
    /// 注册并读取一个枚举配置项。
    /// Registers and reads an enumeration option.
    /// </summary>
    /// <typeparam name="TEnum">枚举类型。Enumeration type.</typeparam>
    /// <param name="key">配置键。Config key.</param>
    /// <param name="defaultValue">默认值。Default value.</param>
    /// <param name="description">配置说明。Config description.</param>
    /// <param name="ui">展示元数据。Presentation metadata.</param>
    /// <returns>当前枚举值。Current enumeration value.</returns>
    public TEnum Enum<TEnum>(string key, TEnum defaultValue, string description, OptionUi? ui = null)
        where TEnum : struct, System.Enum
    {
        return Options.Enum(SectionName, key, defaultValue, description, Page.WithPage(ui));
    }

    /// <summary>
    /// 注册并读取一个整数范围配置项。
    /// Registers and reads an integer range option.
    /// </summary>
    /// <param name="key">配置键。Config key.</param>
    /// <param name="defaultValue">默认值。Default value.</param>
    /// <param name="description">配置说明。Config description.</param>
    /// <param name="minimum">最小值。Minimum value.</param>
    /// <param name="maximum">最大值。Maximum value.</param>
    /// <param name="step">步进值。Step value.</param>
    /// <returns>当前整数值。Current integer value.</returns>
    public int IntRange(string key, int defaultValue, string description, int minimum, int maximum, int step = 1)
    {
        return Options.IntRange(SectionName, key, defaultValue, description, minimum, maximum, step, Page.PageId);
    }

    /// <summary>
    /// 注册并读取一个带展示元数据的整数范围配置项。
    /// Registers and reads an integer range option with presentation metadata.
    /// </summary>
    /// <param name="key">配置键。Config key.</param>
    /// <param name="defaultValue">默认值。Default value.</param>
    /// <param name="description">配置说明。Config description.</param>
    /// <param name="minimum">最小值。Minimum value.</param>
    /// <param name="maximum">最大值。Maximum value.</param>
    /// <param name="ui">展示元数据。Presentation metadata.</param>
    /// <param name="step">步进值。Step value.</param>
    /// <returns>当前整数值。Current integer value.</returns>
    public int IntRange(string key, int defaultValue, string description, int minimum, int maximum, OptionUi? ui, int step = 1)
    {
        return Options.IntRange(SectionName, key, defaultValue, description, minimum, maximum, Page.WithPage(ui), step);
    }

    /// <summary>
    /// 注册并读取一个浮点范围配置项。
    /// Registers and reads a floating-point range option.
    /// </summary>
    /// <param name="key">配置键。Config key.</param>
    /// <param name="defaultValue">默认值。Default value.</param>
    /// <param name="description">配置说明。Config description.</param>
    /// <param name="minimum">最小值。Minimum value.</param>
    /// <param name="maximum">最大值。Maximum value.</param>
    /// <param name="step">步进值。Step value.</param>
    /// <returns>当前浮点值。Current floating-point value.</returns>
    public float FloatRange(string key, float defaultValue, string description, float minimum, float maximum, float step = 0f)
    {
        return Options.FloatRange(SectionName, key, defaultValue, description, minimum, maximum, step, Page.PageId);
    }

    /// <summary>
    /// 注册并读取一个带展示元数据的浮点范围配置项。
    /// Registers and reads a floating-point range option with presentation metadata.
    /// </summary>
    /// <param name="key">配置键。Config key.</param>
    /// <param name="defaultValue">默认值。Default value.</param>
    /// <param name="description">配置说明。Config description.</param>
    /// <param name="minimum">最小值。Minimum value.</param>
    /// <param name="maximum">最大值。Maximum value.</param>
    /// <param name="ui">展示元数据。Presentation metadata.</param>
    /// <param name="step">步进值。Step value.</param>
    /// <returns>当前浮点值。Current floating-point value.</returns>
    public float FloatRange(string key, float defaultValue, string description, float minimum, float maximum, OptionUi? ui, float step = 0f)
    {
        return Options.FloatRange(SectionName, key, defaultValue, description, minimum, maximum, Page.WithPage(ui), step);
    }
}
