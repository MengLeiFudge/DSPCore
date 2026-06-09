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
