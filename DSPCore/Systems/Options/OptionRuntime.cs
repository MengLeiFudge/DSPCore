using System.Collections.Generic;
using BepInEx.Configuration;

namespace DSPCore;

internal static class OptionRuntime
{
    private static readonly Dictionary<string, ConfigEntry<string>> Entries = new();
    private static ConfigFile? configFile;
    private static OptionsWindow? window;

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
        if (!UiWindowManager.Initialized || !UIRoot.instance)
        {
            DspCore.Logger?.LogWarning("DSPCore options window cannot open before UIRoot is initialized.");
            return;
        }

        if (window != null)
        {
            UiWindowManager.DestroyWindow(window);
            window = null;
        }

        window = UiWindowManager.CreateWindow<OptionsWindow>("dspcore-options-window", "DSPCore Settings");
        window.Open();
    }
}
