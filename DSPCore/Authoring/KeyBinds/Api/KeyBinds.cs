using System.Collections.Generic;

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
}
