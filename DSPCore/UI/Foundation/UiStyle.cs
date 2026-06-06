#nullable disable
using System;
using UnityEngine;

namespace DSPCore;

/// <summary>
/// DSPCore UI 框架的通用颜色和格式化工具。
/// Common colors and formatting helpers for the DSPCore UI framework.
/// </summary>
public static class UiStyle
{
    /// <summary>
    /// 页面强调色，适合标题、强调条和选中态。
    /// Accent color for titles, accent strips, and selected states.
    /// </summary>
    public static readonly Color Orange = new(1f, 0.65f, 0.18f, 0.95f);

    /// <summary>
    /// 主要文本颜色。
    /// Primary text color.
    /// </summary>
    public static readonly Color White = new(1f, 1f, 1f, 0.9f);

    /// <summary>
    /// 数值或辅助强调色。
    /// Secondary accent color for counters and helper highlights.
    /// </summary>
    public static readonly Color Blue = new(0.45f, 0.78f, 1f, 0.95f);

    /// <summary>
    /// 将大数字格式化为 UI 计数短文本。
    /// Formats a large number into compact UI counter text.
    /// </summary>
    public static string FormatCompact(double value)
    {
        double abs = Math.Abs(value);
        return abs switch
        {
            >= 1_000_000_000 => $"{value / 1_000_000_000:0.##}G",
            >= 1_000_000 => $"{value / 1_000_000:0.##}M",
            >= 1_000 => $"{value / 1_000:0.##}K",
            _ => value.ToString("0.##"),
        };
    }

    /// <summary>
    /// 按倍率调整颜色 RGB 分量并保留透明度。
    /// Multiplies RGB channels while preserving alpha.
    /// </summary>
    public static Color MultiplyRgb(Color color, float multiplier)
    {
        return new Color(color.r * multiplier, color.g * multiplier, color.b * multiplier, color.a);
    }
}
