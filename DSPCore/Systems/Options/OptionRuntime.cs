using System.Collections.Generic;
using BepInEx.Configuration;
using HarmonyLib;

namespace DSPCore;

internal static class OptionRuntime
{
    private static readonly Dictionary<string, ConfigEntry<string>> Entries = new();
    private static readonly System.Reflection.MethodInfo? OpenMethod = AccessTools.Method(typeof(ManualBehaviour), "_Open");
    private static ConfigFile? configFile;

    public static void Initialize(ConfigFile config)
    {
        configFile = config;
        foreach (var descriptor in DspCore.Options.GetAll())
        {
            BindIfReady(descriptor);
        }
    }

    public static void BindIfReady(OptionDescriptor descriptor)
    {
        if (configFile == null)
        {
            return;
        }

        var key = OptionRegistry.KeyOf(descriptor.Section, descriptor.Key);
        Entries[key] = configFile.Bind(descriptor.Section, descriptor.Key, descriptor.DefaultValue, descriptor.Description);
    }

    public static string GetString(string section, string key)
    {
        return Entries.TryGetValue(OptionRegistry.KeyOf(section, key), out var entry) ? entry.Value : string.Empty;
    }

    public static bool TryGetString(string section, string key, out string value)
    {
        if (Entries.TryGetValue(OptionRegistry.KeyOf(section, key), out var entry))
        {
            value = entry.Value;
            return true;
        }

        value = string.Empty;
        return false;
    }

    public static bool SetString(string section, string key, string value)
    {
        if (Entries.TryGetValue(OptionRegistry.KeyOf(section, key), out var entry))
        {
            entry.Value = value;
            return true;
        }

        return false;
    }

    public static void OpenWindow()
    {
        if (!(UIRoot.instance?.optionWindow is UIOptionWindow optionWindow) || !optionWindow)
        {
            DspCore.Logger?.LogWarning("DSPCore option page cannot open before the vanilla option window is initialized.");
            return;
        }

        AccessOpen(optionWindow);
        OptionPageRuntime.SetTabIndex(optionWindow);
    }

    public static void OpenGlobalSavesWindow()
    {
        DspCore.Logger?.LogInfo("DSPCore GlobalSaves has no player-facing window; global data remains available through the Saves API.");
    }

    private static void AccessOpen(UIOptionWindow optionWindow)
    {
        if (OpenMethod == null)
        {
            DspCore.Logger?.LogWarning("DSPCore could not open the vanilla option window through ManualBehaviour._Open.");
            return;
        }

        OpenMethod.Invoke(optionWindow, null);
    }
}
