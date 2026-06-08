using System.Collections.Generic;
using BepInEx.Configuration;

namespace DSPCore;

internal static class OptionRuntime
{
    private static readonly Dictionary<string, ConfigEntry<string>> Entries = new();
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
}
