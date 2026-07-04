using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace DSPCore;

/// <summary>
/// 配置项能力的短入口。
/// Short entry point for option capabilities.
/// </summary>
public static class Options
{
    private const string ExportHeader = "# DSPCore options export v1";

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
    /// 注册并读取一个带展示元数据的字符串配置项。
    /// Registers and reads a string option with presentation metadata.
    /// </summary>
    /// <param name="section">配置分区。Config section.</param>
    /// <param name="key">配置键。Config key.</param>
    /// <param name="defaultValue">默认值。Default value.</param>
    /// <param name="description">配置说明。Config description.</param>
    /// <param name="ui">展示元数据。Presentation metadata.</param>
    /// <returns>当前字符串值。Current string value.</returns>
    public static string String(string section, string key, string defaultValue, string description, OptionUi? ui)
    {
        var metadata = ui ?? OptionUi.Empty;
        Register(section, key, defaultValue, description, metadata.PageId, displayName: metadata.DisplayName, order: metadata.Order, canReset: metadata.CanReset);
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
    /// 注册并读取一个带展示元数据的布尔配置项。
    /// Registers and reads a boolean option with presentation metadata.
    /// </summary>
    /// <param name="section">配置分区。Config section.</param>
    /// <param name="key">配置键。Config key.</param>
    /// <param name="defaultValue">默认值。Default value.</param>
    /// <param name="description">配置说明。Config description.</param>
    /// <param name="ui">展示元数据。Presentation metadata.</param>
    /// <returns>当前布尔值。Current boolean value.</returns>
    public static bool Bool(string section, string key, bool defaultValue, string description, OptionUi? ui)
    {
        var metadata = ui ?? OptionUi.Empty;
        Register(section, key, defaultValue.ToString(), description, metadata.PageId, OptionValueKind.Bool, metadata.DisplayName, order: metadata.Order, canReset: metadata.CanReset);
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
    /// 注册并读取一个带展示元数据的整数配置项。
    /// Registers and reads an integer option with presentation metadata.
    /// </summary>
    /// <param name="section">配置分区。Config section.</param>
    /// <param name="key">配置键。Config key.</param>
    /// <param name="defaultValue">默认值。Default value.</param>
    /// <param name="description">配置说明。Config description.</param>
    /// <param name="ui">展示元数据。Presentation metadata.</param>
    /// <returns>当前整数值。Current integer value.</returns>
    public static int Int(string section, string key, int defaultValue, string description, OptionUi? ui)
    {
        var metadata = ui ?? OptionUi.Empty;
        Register(section, key, defaultValue.ToString(CultureInfo.InvariantCulture), description, metadata.PageId, OptionValueKind.Int, metadata.DisplayName, order: metadata.Order, canReset: metadata.CanReset);
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
    /// 注册并读取一个带展示元数据的浮点配置项。
    /// Registers and reads a floating-point option with presentation metadata.
    /// </summary>
    /// <param name="section">配置分区。Config section.</param>
    /// <param name="key">配置键。Config key.</param>
    /// <param name="defaultValue">默认值。Default value.</param>
    /// <param name="description">配置说明。Config description.</param>
    /// <param name="ui">展示元数据。Presentation metadata.</param>
    /// <returns>当前浮点值。Current floating-point value.</returns>
    public static float Float(string section, string key, float defaultValue, string description, OptionUi? ui)
    {
        var metadata = ui ?? OptionUi.Empty;
        Register(section, key, defaultValue.ToString(CultureInfo.InvariantCulture), description, metadata.PageId, OptionValueKind.Float, metadata.DisplayName, order: metadata.Order, canReset: metadata.CanReset);
        return GetFloat(section, key, defaultValue);
    }

    /// <summary>
    /// 注册并读取一个枚举配置项。
    /// Registers and reads an enumeration option.
    /// </summary>
    /// <typeparam name="TEnum">枚举类型。Enumeration type.</typeparam>
    /// <param name="section">配置分区。Config section.</param>
    /// <param name="key">配置键。Config key.</param>
    /// <param name="defaultValue">默认值。Default value.</param>
    /// <param name="description">配置说明。Config description.</param>
    /// <param name="pageId">可选设置页面 ID。Optional settings page ID.</param>
    /// <returns>当前枚举值。Current enumeration value.</returns>
    public static TEnum Enum<TEnum>(string section, string key, TEnum defaultValue, string description, string? pageId = null)
        where TEnum : struct, System.Enum
    {
        Register(
            section,
            key,
            defaultValue.ToString(),
            description,
            pageId,
            OptionValueKind.Enum,
            choices: System.Enum.GetNames(typeof(TEnum)));
        return GetEnum(section, key, defaultValue);
    }

    /// <summary>
    /// 注册并读取一个带展示元数据的枚举配置项。
    /// Registers and reads an enumeration option with presentation metadata.
    /// </summary>
    /// <typeparam name="TEnum">枚举类型。Enumeration type.</typeparam>
    /// <param name="section">配置分区。Config section.</param>
    /// <param name="key">配置键。Config key.</param>
    /// <param name="defaultValue">默认值。Default value.</param>
    /// <param name="description">配置说明。Config description.</param>
    /// <param name="ui">展示元数据。Presentation metadata.</param>
    /// <returns>当前枚举值。Current enumeration value.</returns>
    public static TEnum Enum<TEnum>(string section, string key, TEnum defaultValue, string description, OptionUi? ui)
        where TEnum : struct, System.Enum
    {
        var metadata = ui ?? OptionUi.Empty;
        Register(
            section,
            key,
            defaultValue.ToString(),
            description,
            metadata.PageId,
            OptionValueKind.Enum,
            metadata.DisplayName,
            System.Enum.GetNames(typeof(TEnum)),
            order: metadata.Order,
            canReset: metadata.CanReset);
        return GetEnum(section, key, defaultValue);
    }

    /// <summary>
    /// 注册并读取一个整数范围配置项。
    /// Registers and reads an integer range option.
    /// </summary>
    /// <param name="section">配置分区。Config section.</param>
    /// <param name="key">配置键。Config key.</param>
    /// <param name="defaultValue">默认值。Default value.</param>
    /// <param name="description">配置说明。Config description.</param>
    /// <param name="minimum">最小值。Minimum value.</param>
    /// <param name="maximum">最大值。Maximum value.</param>
    /// <param name="step">步进值。Step value.</param>
    /// <param name="pageId">可选设置页面 ID。Optional settings page ID.</param>
    /// <returns>当前整数值。Current integer value.</returns>
    public static int IntRange(
        string section,
        string key,
        int defaultValue,
        string description,
        int minimum,
        int maximum,
        int step = 1,
        string? pageId = null)
    {
        Register(
            section,
            key,
            defaultValue.ToString(CultureInfo.InvariantCulture),
            description,
            pageId,
            OptionValueKind.IntRange,
            minimum: minimum,
            maximum: maximum,
            step: Math.Max(1, step));
        return Math.Min(maximum, Math.Max(minimum, GetInt(section, key, defaultValue)));
    }

    /// <summary>
    /// 注册并读取一个带展示元数据的整数范围配置项。
    /// Registers and reads an integer range option with presentation metadata.
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
    public static int IntRange(
        string section,
        string key,
        int defaultValue,
        string description,
        int minimum,
        int maximum,
        OptionUi? ui,
        int step = 1)
    {
        var metadata = ui ?? OptionUi.Empty;
        Register(
            section,
            key,
            defaultValue.ToString(CultureInfo.InvariantCulture),
            description,
            metadata.PageId,
            OptionValueKind.IntRange,
            metadata.DisplayName,
            minimum: minimum,
            maximum: maximum,
            step: Math.Max(1, step),
            order: metadata.Order,
            canReset: metadata.CanReset);
        return Math.Min(maximum, Math.Max(minimum, GetInt(section, key, defaultValue)));
    }

    /// <summary>
    /// 注册并读取一个浮点范围配置项。
    /// Registers and reads a floating-point range option.
    /// </summary>
    /// <param name="section">配置分区。Config section.</param>
    /// <param name="key">配置键。Config key.</param>
    /// <param name="defaultValue">默认值。Default value.</param>
    /// <param name="description">配置说明。Config description.</param>
    /// <param name="minimum">最小值。Minimum value.</param>
    /// <param name="maximum">最大值。Maximum value.</param>
    /// <param name="step">步进值。Step value.</param>
    /// <param name="pageId">可选设置页面 ID。Optional settings page ID.</param>
    /// <returns>当前浮点值。Current floating-point value.</returns>
    public static float FloatRange(
        string section,
        string key,
        float defaultValue,
        string description,
        float minimum,
        float maximum,
        float step = 0f,
        string? pageId = null)
    {
        Register(
            section,
            key,
            defaultValue.ToString(CultureInfo.InvariantCulture),
            description,
            pageId,
            OptionValueKind.FloatRange,
            minimum: minimum,
            maximum: maximum,
            step: step);
        return Math.Min(maximum, Math.Max(minimum, GetFloat(section, key, defaultValue)));
    }

    /// <summary>
    /// 注册并读取一个带展示元数据的浮点范围配置项。
    /// Registers and reads a floating-point range option with presentation metadata.
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
    public static float FloatRange(
        string section,
        string key,
        float defaultValue,
        string description,
        float minimum,
        float maximum,
        OptionUi? ui,
        float step = 0f)
    {
        var metadata = ui ?? OptionUi.Empty;
        Register(
            section,
            key,
            defaultValue.ToString(CultureInfo.InvariantCulture),
            description,
            metadata.PageId,
            OptionValueKind.FloatRange,
            metadata.DisplayName,
            minimum: minimum,
            maximum: maximum,
            step: step,
            order: metadata.Order,
            canReset: metadata.CanReset);
        return Math.Min(maximum, Math.Max(minimum, GetFloat(section, key, defaultValue)));
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
    /// <param name="displayName">可选显示名称。Optional display name.</param>
    /// <param name="choices">可选候选值。Optional selectable choices.</param>
    /// <param name="minimum">可选最小值。Optional minimum value.</param>
    /// <param name="maximum">可选最大值。Optional maximum value.</param>
    /// <param name="step">可选步进值。Optional step value.</param>
    /// <param name="order">同页内排序值。Sort order within the page.</param>
    /// <param name="canReset">是否显示重置按钮。Whether to show a reset button.</param>
    public static void Register(
        string section,
        string key,
        string defaultValue,
        string description,
        string? pageId = null,
        OptionValueKind kind = OptionValueKind.String,
        string? displayName = null,
        string[]? choices = null,
        float? minimum = null,
        float? maximum = null,
        float? step = null,
        int order = 0,
        bool canReset = false)
    {
        DspCore.Options.Register(new OptionDescriptor(section, key, defaultValue, description, pageId, kind, displayName, choices, minimum, maximum, step)
        {
            Order = order,
            CanReset = canReset
        });
    }

    /// <summary>
    /// 打开原版设置窗口并切到 DSPCore 设置分页。
    /// Opens the vanilla option window and selects the DSPCore settings tab.
    /// </summary>
    public static void OpenWindow()
    {
        OptionRuntime.OpenWindow();
    }

    /// <summary>
    /// 兼容旧入口；GlobalSaves 不再提供玩家窗口。
    /// Compatibility entry; GlobalSaves no longer exposes a player-facing window.
    /// </summary>
    [Obsolete("GlobalSaves no longer has a player-facing window.")]
    public static void OpenGlobalSavesWindow()
    {
        OptionRuntime.OpenGlobalSavesWindow();
    }

    /// <summary>
    /// 导出所有已注册配置项的当前值快照。
    /// Exports current values of all registered options.
    /// </summary>
    /// <returns>配置值快照。Option value snapshots.</returns>
    public static IReadOnlyList<OptionValueSnapshot> ExportValues()
    {
        return DspCore.Options.GetAll()
            .OrderBy(option => option.Section, StringComparer.Ordinal)
            .ThenBy(option => option.Key, StringComparer.Ordinal)
            .Select(option => new OptionValueSnapshot(option.Section, option.Key, GetString(option.Section, option.Key)))
            .ToArray();
    }

    /// <summary>
    /// 导出所有已注册配置项的文本快照。
    /// Exports a text snapshot of all registered options.
    /// </summary>
    /// <returns>可传给 ImportText 的文本。Text that can be passed to ImportText.</returns>
    public static string ExportText()
    {
        var builder = new StringBuilder();
        builder.AppendLine(ExportHeader);
        foreach (var value in ExportValues())
        {
            builder.Append(Encode(value.Section));
            builder.Append('\t');
            builder.Append(Encode(value.Key));
            builder.Append('\t');
            builder.Append(Encode(value.Value));
            builder.AppendLine();
        }

        return builder.ToString();
    }

    /// <summary>
    /// 导入配置值快照。
    /// Imports option value snapshots.
    /// </summary>
    /// <param name="values">配置值快照。Option value snapshots.</param>
    /// <returns>导入结果。Import result.</returns>
    public static OptionImportReport ImportValues(IEnumerable<OptionValueSnapshot> values)
    {
        var knownKeys = new HashSet<string>(
            DspCore.Options.GetAll().Select(option => OptionRegistry.KeyOf(option.Section, option.Key)),
            StringComparer.Ordinal);
        var skipped = new List<string>();
        int applied = 0;

        foreach (var value in values)
        {
            var key = OptionRegistry.KeyOf(value.Section, value.Key);
            if (!knownKeys.Contains(key))
            {
                skipped.Add(value.Section + "/" + value.Key + " (not registered)");
                continue;
            }

            if (!OptionRuntime.SetString(value.Section, value.Key, value.Value))
            {
                skipped.Add(value.Section + "/" + value.Key + " (not bound)");
                continue;
            }

            applied++;
        }

        return new OptionImportReport(applied, skipped.Count, skipped);
    }

    /// <summary>
    /// 导入由 ExportText 生成的配置文本。
    /// Imports option text generated by ExportText.
    /// </summary>
    /// <param name="text">配置文本。Option text.</param>
    /// <returns>导入结果。Import result.</returns>
    public static OptionImportReport ImportText(string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return new OptionImportReport(0, 0, Array.Empty<string>());
        }

        var values = new List<OptionValueSnapshot>();
        var skipped = new List<string>();
        var lines = text.Replace("\r\n", "\n").Replace('\r', '\n').Split('\n');
        for (int i = 0; i < lines.Length; i++)
        {
            var line = lines[i].Trim();
            if (line.Length == 0 || line.StartsWith("#", StringComparison.Ordinal))
            {
                continue;
            }

            var parts = line.Split('\t');
            if (parts.Length != 3 ||
                !TryDecode(parts[0], out var section) ||
                !TryDecode(parts[1], out var key) ||
                !TryDecode(parts[2], out var value))
            {
                skipped.Add("line " + (i + 1).ToString(CultureInfo.InvariantCulture) + " (invalid format)");
                continue;
            }

            values.Add(new OptionValueSnapshot(section, key, value));
        }

        var report = ImportValues(values);
        if (skipped.Count == 0)
        {
            return report;
        }

        skipped.AddRange(report.SkippedKeys);
        return new OptionImportReport(report.AppliedCount, skipped.Count, skipped);
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
    /// 注册一个设置页面并返回页面上下文。
    /// Registers an option page and returns a page context.
    /// </summary>
    /// <param name="pageId">页面 ID。Page ID.</param>
    /// <param name="ownerModGuid">所属模组 GUID。Owner mod GUID.</param>
    /// <param name="title">页面标题。Page title.</param>
    /// <param name="order">排序值。Sort order.</param>
    /// <returns>设置页面上下文。Option page context.</returns>
    public static OptionPage Page(string pageId, string ownerModGuid, string title, int order = 0)
    {
        var descriptor = new OptionPageDescriptor(pageId, ownerModGuid, title, order);
        DspCore.Options.RegisterPage(descriptor);
        return new OptionPage(descriptor);
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

    private static string Encode(string value)
    {
        return Convert.ToBase64String(Encoding.UTF8.GetBytes(value ?? string.Empty));
    }

    private static bool TryDecode(string value, out string decoded)
    {
        try
        {
            decoded = Encoding.UTF8.GetString(Convert.FromBase64String(value));
            return true;
        }
        catch (FormatException)
        {
            decoded = string.Empty;
            return false;
        }
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

    /// <summary>
    /// 获取枚举配置值。
    /// Gets an enumeration option value.
    /// </summary>
    /// <typeparam name="TEnum">枚举类型。Enumeration type.</typeparam>
    /// <param name="section">配置分区。Config section.</param>
    /// <param name="key">配置键。Config key.</param>
    /// <param name="fallback">解析失败时的默认值。Fallback used when parsing fails.</param>
    /// <returns>当前枚举值。Current enumeration value.</returns>
    public static TEnum GetEnum<TEnum>(string section, string key, TEnum fallback)
        where TEnum : struct, System.Enum
    {
        var text = DspCore.Options.GetString(section, key);
        if (System.Enum.TryParse<TEnum>(text, true, out var value))
        {
            var names = System.Enum.GetNames(typeof(TEnum));
            if (names.Any(name => name.Equals(value.ToString(), StringComparison.Ordinal)))
            {
                return value;
            }
        }

        return fallback;
    }
}
