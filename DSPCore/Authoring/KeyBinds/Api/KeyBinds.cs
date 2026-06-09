using System;
using System.Collections.Generic;
using System.Linq;

namespace DSPCore;

/// <summary>
/// 作者侧按键绑定入口。
/// Author-facing key binding entry point.
/// </summary>
public static class KeyBinds
{
    internal const string OptionsPageId = "dspcore.keybinds";

    /// <summary>
    /// 注册一个可重绑定按键。
    /// Registers a rebindable key binding.
    /// </summary>
    /// <param name="id">按键 ID。Key binding id.</param>
    /// <param name="ownerModGuid">声明方模组 GUID。Declaring mod GUID.</param>
    /// <param name="displayName">显示名称本地化键或文本。Display name localization key or text.</param>
    /// <param name="defaultKey">默认按键文本。Default key text.</param>
    /// <param name="callback">触发回调。Trigger callback.</param>
    /// <param name="action">触发方式。Trigger action.</param>
    /// <param name="conflictGroup">冲突组。Conflict group.</param>
    /// <param name="canOverride">玩家是否可以重绑定。Whether players can rebind it.</param>
    public static void Register(
        string id,
        string ownerModGuid,
        string displayName,
        string defaultKey,
        Action callback,
        CoreKeyAction action = CoreKeyAction.Press,
        int conflictGroup = 0,
        bool canOverride = true)
    {
        Register(new KeyBindDescriptor(
            id,
            ownerModGuid,
            displayName,
            defaultKey,
            action,
            conflictGroup,
            canOverride,
            callback));
    }

    /// <summary>
    /// 注册一个可重绑定按键。
    /// Registers a rebindable key binding.
    /// </summary>
    public static void Register(KeyBindDescriptor descriptor)
    {
        DspCore.KeyBinds.Register(descriptor);
        if (!descriptor.CanOverride)
        {
            return;
        }

        DspCore.Options.RegisterPage(new OptionPageDescriptor(OptionsPageId, descriptor.OwnerModGuid, "Key Bindings", 9000));
        DspCore.Options.Register(new OptionDescriptor(
            GetOptionSection(descriptor),
            GetOptionKey(descriptor),
            descriptor.DefaultKey,
            BuildDescription(descriptor),
            OptionsPageId,
            OptionValueKind.KeyBinding,
            descriptor.DisplayName));
    }

    /// <summary>
    /// 获取所有按键声明。
    /// Gets all key binding declarations.
    /// </summary>
    public static IReadOnlyCollection<KeyBindDescriptor> GetAll()
    {
        return DspCore.KeyBinds.GetAll();
    }

    internal static string GetConfiguredKeyText(KeyBindDescriptor descriptor)
    {
        var value = DspCore.Options.GetString(GetOptionSection(descriptor), GetOptionKey(descriptor));
        return string.IsNullOrWhiteSpace(value) ? descriptor.DefaultKey : value;
    }

    internal static string BuildOptionDescription(OptionDescriptor option)
    {
        if (!TryGetDescriptor(option.Section, option.Key, out var descriptor))
        {
            return option.Description;
        }

        var description = BuildDescription(descriptor);
        var conflictText = BuildConflictText(descriptor);
        return string.IsNullOrEmpty(conflictText)
            ? description
            : description + "; <color=#ffb347>" + conflictText + "</color>";
    }

    private static string GetOptionSection(KeyBindDescriptor descriptor)
    {
        return "KeyBinds." + descriptor.OwnerModGuid;
    }

    private static string GetOptionKey(KeyBindDescriptor descriptor)
    {
        return descriptor.Id;
    }

    private static string BuildDescription(KeyBindDescriptor descriptor)
    {
        return descriptor.ConflictGroup == 0
            ? $"Default: {descriptor.DefaultKey}"
            : $"Default: {descriptor.DefaultKey}; conflict group: {descriptor.ConflictGroup}";
    }

    private static bool TryGetDescriptor(string section, string key, out KeyBindDescriptor descriptor)
    {
        foreach (var candidate in DspCore.KeyBinds.GetAll())
        {
            if (candidate.CanOverride &&
                GetOptionSection(candidate).Equals(section, System.StringComparison.Ordinal) &&
                GetOptionKey(candidate).Equals(key, System.StringComparison.Ordinal))
            {
                descriptor = candidate;
                return true;
            }
        }

        descriptor = default!;
        return false;
    }

    private static string BuildConflictText(KeyBindDescriptor descriptor)
    {
        if (descriptor.ConflictGroup == 0 || !TryGetEffectiveKeyText(descriptor, out var keyText))
        {
            return string.Empty;
        }

        var conflicts = DspCore.KeyBinds.GetAll()
            .Where(candidate => !candidate.Id.Equals(descriptor.Id, System.StringComparison.Ordinal))
            .Where(candidate => candidate.ConflictGroup == descriptor.ConflictGroup)
            .Where(candidate => TryGetEffectiveKeyText(candidate, out var candidateKeyText) && candidateKeyText.Equals(keyText, System.StringComparison.Ordinal))
            .Select(candidate => $"{candidate.DisplayName} ({candidate.OwnerModGuid})")
            .ToArray();

        return conflicts.Length == 0
            ? string.Empty
            : "conflicts with " + string.Join(", ", conflicts);
    }

    private static bool TryGetEffectiveKeyText(KeyBindDescriptor descriptor, out string keyText)
    {
        var configured = descriptor.CanOverride ? GetConfiguredKeyText(descriptor) : descriptor.DefaultKey;
        if (KeyBindRuntime.TryNormalizeKeyText(configured, out keyText))
        {
            return true;
        }

        return !configured.Equals(descriptor.DefaultKey, System.StringComparison.OrdinalIgnoreCase) &&
            KeyBindRuntime.TryNormalizeKeyText(descriptor.DefaultKey, out keyText);
    }
}
