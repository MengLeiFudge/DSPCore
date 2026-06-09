using System.Globalization;

namespace DSPCore;

/// <summary>
/// 配置项能力的短入口。
/// Short entry point for option capabilities.
/// </summary>
public static class Options
{
    /// <summary>
    /// 注册并读取一个字符串配置项。
    /// Registers and reads a string option.
    /// </summary>
    /// <param name="section">配置分区。Config section.</param>
    /// <param name="key">配置键。Config key.</param>
    /// <param name="defaultValue">默认值。Default value.</param>
    /// <param name="description">配置说明。Config description.</param>
    /// <param name="pageId">可选设置页面 ID。Optional settings page ID.</param>
    /// <returns>当前字符串值。Current string value.</returns>
    public static string String(string section, string key, string defaultValue, string description, string? pageId = null)
    {
        Register(section, key, defaultValue, description, pageId);
        return GetString(section, key);
    }

    /// <summary>
    /// 注册并读取一个布尔配置项。
    /// Registers and reads a boolean option.
    /// </summary>
    /// <param name="section">配置分区。Config section.</param>
    /// <param name="key">配置键。Config key.</param>
    /// <param name="defaultValue">默认值。Default value.</param>
    /// <param name="description">配置说明。Config description.</param>
    /// <param name="pageId">可选设置页面 ID。Optional settings page ID.</param>
    /// <returns>当前布尔值。Current boolean value.</returns>
    public static bool Bool(string section, string key, bool defaultValue, string description, string? pageId = null)
    {
        Register(section, key, defaultValue.ToString(), description, pageId, OptionValueKind.Bool);
        return GetBool(section, key, defaultValue);
    }

    /// <summary>
    /// 注册并读取一个整数配置项。
    /// Registers and reads an integer option.
    /// </summary>
    /// <param name="section">配置分区。Config section.</param>
    /// <param name="key">配置键。Config key.</param>
    /// <param name="defaultValue">默认值。Default value.</param>
    /// <param name="description">配置说明。Config description.</param>
    /// <param name="pageId">可选设置页面 ID。Optional settings page ID.</param>
    /// <returns>当前整数值。Current integer value.</returns>
    public static int Int(string section, string key, int defaultValue, string description, string? pageId = null)
    {
        Register(section, key, defaultValue.ToString(CultureInfo.InvariantCulture), description, pageId, OptionValueKind.Int);
        return GetInt(section, key, defaultValue);
    }

    /// <summary>
    /// 注册并读取一个浮点配置项。
    /// Registers and reads a floating-point option.
    /// </summary>
    /// <param name="section">配置分区。Config section.</param>
    /// <param name="key">配置键。Config key.</param>
    /// <param name="defaultValue">默认值。Default value.</param>
    /// <param name="description">配置说明。Config description.</param>
    /// <param name="pageId">可选设置页面 ID。Optional settings page ID.</param>
    /// <returns>当前浮点值。Current floating-point value.</returns>
    public static float Float(string section, string key, float defaultValue, string description, string? pageId = null)
    {
        Register(section, key, defaultValue.ToString(CultureInfo.InvariantCulture), description, pageId, OptionValueKind.Float);
        return GetFloat(section, key, defaultValue);
    }

    /// <summary>
    /// 注册一个字符串配置项。
    /// Registers a string option.
    /// </summary>
    /// <param name="section">配置分区。Config section.</param>
    /// <param name="key">配置键。Config key.</param>
    /// <param name="defaultValue">默认值。Default value.</param>
    /// <param name="description">配置说明。Config description.</param>
    /// <param name="pageId">可选设置页面 ID。Optional settings page ID.</param>
    /// <param name="kind">基础控件类型。Basic control type.</param>
    public static void Register(
        string section,
        string key,
        string defaultValue,
        string description,
        string? pageId = null,
        OptionValueKind kind = OptionValueKind.String)
    {
        DspCore.Options.Register(new OptionDescriptor(section, key, defaultValue, description, pageId, kind));
    }

    /// <summary>
    /// 打开 DSPCore 统一设置窗口。
    /// Opens the DSPCore unified settings window.
    /// </summary>
    public static void OpenWindow()
    {
        OptionRuntime.OpenWindow();
    }

    /// <summary>
    /// 注册一个设置页面。
    /// Registers an option page.
    /// </summary>
    /// <param name="pageId">页面 ID。Page ID.</param>
    /// <param name="ownerModGuid">所属模组 GUID。Owner mod GUID.</param>
    /// <param name="title">页面标题。Page title.</param>
    /// <param name="order">排序值。Sort order.</param>
    public static void RegisterPage(string pageId, string ownerModGuid, string title, int order = 0)
    {
        DspCore.Options.RegisterPage(new OptionPageDescriptor(pageId, ownerModGuid, title, order));
    }

    /// <summary>
    /// 注册一个设置版本。
    /// Registers a settings version.
    /// </summary>
    /// <param name="ownerModGuid">所属模组 GUID。Owner mod GUID.</param>
    /// <param name="version">设置版本。Settings version.</param>
    public static void RegisterVersion(string ownerModGuid, string version)
    {
        DspCore.Options.RegisterVersion(new OptionVersionDescriptor(ownerModGuid, version));
    }

    /// <summary>
    /// 获取字符串配置值。
    /// Gets a string option value.
    /// </summary>
    /// <param name="section">配置分区。Config section.</param>
    /// <param name="key">配置键。Config key.</param>
    /// <returns>当前值。Current value.</returns>
    public static string GetString(string section, string key)
    {
        return DspCore.Options.GetString(section, key);
    }

    /// <summary>
    /// 获取布尔配置值。
    /// Gets a boolean option value.
    /// </summary>
    /// <param name="section">配置分区。Config section.</param>
    /// <param name="key">配置键。Config key.</param>
    /// <param name="fallback">解析失败时的默认值。Fallback used when parsing fails.</param>
    /// <returns>当前布尔值。Current boolean value.</returns>
    public static bool GetBool(string section, string key, bool fallback = false)
    {
        return DspCore.Options.GetBool(section, key, fallback);
    }

    /// <summary>
    /// 获取整数配置值。
    /// Gets an integer option value.
    /// </summary>
    /// <param name="section">配置分区。Config section.</param>
    /// <param name="key">配置键。Config key.</param>
    /// <param name="fallback">解析失败时的默认值。Fallback used when parsing fails.</param>
    /// <returns>当前整数值。Current integer value.</returns>
    public static int GetInt(string section, string key, int fallback = 0)
    {
        return DspCore.Options.GetInt(section, key, fallback);
    }

    /// <summary>
    /// 获取浮点配置值。
    /// Gets a floating-point option value.
    /// </summary>
    /// <param name="section">配置分区。Config section.</param>
    /// <param name="key">配置键。Config key.</param>
    /// <param name="fallback">解析失败时的默认值。Fallback used when parsing fails.</param>
    /// <returns>当前浮点值。Current floating-point value.</returns>
    public static float GetFloat(string section, string key, float fallback = 0f)
    {
        return DspCore.Options.GetFloat(section, key, fallback);
    }
}
