using System;
using BepInEx.Bootstrap;

namespace DSPCore;

/// <summary>
/// 提供常用补丁启用条件。
/// Provides common patch enable conditions.
/// </summary>
public static class PatchConditions
{
    /// <summary>
    /// 判断指定插件是否已加载并满足最低版本。
    /// Checks whether a plugin is loaded and satisfies the minimum version.
    /// </summary>
    /// <param name="pluginGuid">插件 GUID。Plugin GUID.</param>
    /// <param name="minimumVersion">可选最低版本。Optional minimum version.</param>
    /// <returns>满足条件时返回 true。Returns true when the condition is satisfied.</returns>
    public static bool PluginLoaded(string pluginGuid, string? minimumVersion = null)
    {
        if (!Chainloader.PluginInfos.TryGetValue(pluginGuid, out var plugin))
        {
            return false;
        }

        var requiredText = minimumVersion;
        if (string.IsNullOrWhiteSpace(requiredText))
        {
            return true;
        }

        return TryParseVersion(plugin.Metadata.Version.ToString(), out var actual) &&
            TryParseVersion(requiredText!, out var required) &&
            actual.CompareTo(required) >= 0;
    }

    /// <summary>
    /// 获取插件缺失或版本不足的说明。
    /// Gets a reason for a missing or outdated plugin.
    /// </summary>
    /// <param name="pluginGuid">插件 GUID。Plugin GUID.</param>
    /// <param name="minimumVersion">可选最低版本。Optional minimum version.</param>
    /// <returns>禁用原因。Disabled reason.</returns>
    public static string GetPluginRequirementReason(string pluginGuid, string? minimumVersion = null)
    {
        if (!Chainloader.PluginInfos.TryGetValue(pluginGuid, out var plugin))
        {
            return $"required plugin {pluginGuid} is not loaded";
        }

        var requiredText = minimumVersion;
        if (!string.IsNullOrWhiteSpace(requiredText) &&
            (!TryParseVersion(plugin.Metadata.Version.ToString(), out var actual) ||
             !TryParseVersion(requiredText!, out var required) ||
             actual.CompareTo(required) < 0))
        {
            return $"required plugin {pluginGuid} version >= {minimumVersion} is not satisfied";
        }

        return string.Empty;
    }

    private static bool TryParseVersion(string text, out System.Version version)
    {
        try
        {
            version = new System.Version(text);
            return true;
        }
        catch (ArgumentException)
        {
            version = new System.Version(0, 0);
            return false;
        }
        catch (FormatException)
        {
            version = new System.Version(0, 0);
            return false;
        }
        catch (OverflowException)
        {
            version = new System.Version(0, 0);
            return false;
        }
    }
}
