using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using BepInEx;
using HarmonyLib;

namespace DSPCore;

internal static class LocalizationRuntime
{
    private const string LocaleOverrideOwner = "DSPCore.LocaleOverride";
    private const string LocaleDirectory = "DSPCore/Locales";
    private static readonly System.Reflection.FieldInfo? NamesIndexerField = AccessTools.Field(typeof(Localization), "namesIndexer");
    private static readonly System.Reflection.FieldInfo? StringsField = AccessTools.Field(typeof(Localization), "strings");
    private static readonly System.Reflection.FieldInfo? FloatsField = AccessTools.Field(typeof(Localization), "floats");
    private static bool localeOverridesLoaded;

    public static void Initialize()
    {
        LoadLocaleOverrides();
    }

    public static void AddKeys()
    {
        var namesIndexer = GetNamesIndexer();
        if (namesIndexer == null)
        {
            return;
        }

        foreach (var key in GetAllLocalizationEntries().Select(item => item.Key).Distinct(StringComparer.Ordinal))
        {
            if (!namesIndexer.ContainsKey(key))
            {
                namesIndexer.Add(key, namesIndexer.Count);
            }
        }
    }

    public static void ApplyLanguage(int index)
    {
        var stringsArray = GetStringsArray();
        var namesIndexer = GetNamesIndexer();
        if (Localization.Languages == null || stringsArray == null || namesIndexer == null || index < 0 || index >= Localization.Languages.Length)
        {
            return;
        }

        AddKeys();
        var language = Localization.Languages[index];
        var languageId = language.lcId;
        var entries = GetEffectiveLocalizations(languageId)
            .Where(item => LocalizationLanguages.ResolveId(item.Language) == languageId)
            .ToArray();
        if (entries.Length == 0)
        {
            return;
        }

        var strings = stringsArray[index];
        if (strings == null)
        {
            return;
        }

        EnsureLanguageArraySize(index, namesIndexer.Count);
        stringsArray = GetStringsArray();
        strings = stringsArray![index];

        foreach (var entry in entries)
        {
            if (!namesIndexer.TryGetValue(entry.Key, out var keyIndex))
            {
                continue;
            }

            strings[keyIndex] = entry.Value;
        }
    }

    private static void LoadLocaleOverrides()
    {
        if (localeOverridesLoaded)
        {
            return;
        }

        localeOverridesLoaded = true;
        var directory = GetLocaleDirectory();
        if (!Directory.Exists(directory))
        {
            return;
        }

        foreach (var path in Directory.GetFiles(directory, "locale-*.tsv", SearchOption.TopDirectoryOnly).OrderBy(item => item, StringComparer.OrdinalIgnoreCase))
        {
            LoadLocaleFile(path);
        }
    }

    private static void LoadLocaleFile(string path)
    {
        var fileName = Path.GetFileNameWithoutExtension(path);
        var language = fileName.StartsWith("locale-", StringComparison.OrdinalIgnoreCase)
            ? fileName.Substring("locale-".Length)
            : string.Empty;
        if (LocalizationLanguages.ResolveId(language) <= 0)
        {
            DspCore.Logger?.LogWarning($"DSPCore locale override file has unknown language and was skipped: {path}");
            return;
        }

        try
        {
            var lineNumber = 0;
            foreach (var line in File.ReadLines(path, Encoding.UTF8))
            {
                lineNumber++;
                if (string.IsNullOrWhiteSpace(line) || line.TrimStart().StartsWith("#", StringComparison.Ordinal))
                {
                    continue;
                }

                var separator = line.IndexOf('\t');
                if (separator <= 0)
                {
                    DspCore.Logger?.LogWarning($"DSPCore locale override skipped malformed line {lineNumber} in {path}.");
                    continue;
                }

                var key = line.Substring(0, separator).Trim();
                if (key.Length == 0)
                {
                    DspCore.Logger?.LogWarning($"DSPCore locale override skipped empty key on line {lineNumber} in {path}.");
                    continue;
                }

                var value = line.Substring(separator + 1).Replace("\\n", "\n");
                DspCore.Resources.RegisterLocalizationOverride(new LocalizationEntry(key, language, value, LocaleOverrideOwner));
            }
        }
        catch (Exception ex)
        {
            DspCore.Errors.ReportException(LocaleOverrideOwner, ex);
            DspCore.Logger?.LogError($"DSPCore locale override load failed for {path}: {ex}");
        }
    }

    private static IEnumerable<LocalizationEntry> GetAllLocalizationEntries()
    {
        return DspCore.Resources.GetLocalizations().Concat(DspCore.Resources.GetLocalizationOverrides());
    }

    private static IEnumerable<LocalizationEntry> GetEffectiveLocalizations(int languageId)
    {
        var result = new Dictionary<string, LocalizationEntry>(StringComparer.Ordinal);
        foreach (var entry in DspCore.Resources.GetLocalizations().Where(item => LocalizationLanguages.ResolveId(item.Language) == languageId))
        {
            result[entry.Key] = entry;
        }

        foreach (var entry in DspCore.Resources.GetLocalizationOverrides().Where(item => LocalizationLanguages.ResolveId(item.Language) == languageId))
        {
            result[entry.Key] = entry;
        }

        return result.Values;
    }

    private static string GetLocaleDirectory()
    {
        return Path.Combine(Paths.ConfigPath, LocaleDirectory);
    }

    private static void EnsureLanguageArraySize(int index, int size)
    {
        var stringsArray = GetStringsArray();
        if (stringsArray != null && stringsArray[index].Length < size)
        {
            var strings = stringsArray[index];
            Array.Resize(ref strings, size);
            stringsArray[index] = strings;
        }

        var floatsArray = GetFloatsArray();
        if (floatsArray != null && floatsArray[index] != null && floatsArray[index].Length < size)
        {
            var floats = floatsArray[index];
            Array.Resize(ref floats, size);
            floatsArray[index] = floats;
        }
    }

    private static Dictionary<string, int>? GetNamesIndexer()
    {
        return NamesIndexerField?.GetValue(null) as Dictionary<string, int>;
    }

    private static string[][]? GetStringsArray()
    {
        return StringsField?.GetValue(null) as string[][];
    }

    private static float[][]? GetFloatsArray()
    {
        return FloatsField?.GetValue(null) as float[][];
    }
}

internal static class LocalizationRuntimePatches
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(Localization), "LoadSettings")]
    private static void LoadSettings()
    {
        LocalizationRuntime.AddKeys();
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(Localization), nameof(Localization.LoadLanguage))]
    private static void LoadLanguage(int index)
    {
        LocalizationRuntime.ApplyLanguage(index);
    }
}
