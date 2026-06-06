using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;

namespace DSPCore;

internal static class LocalizationRuntime
{
    private static readonly System.Reflection.FieldInfo? NamesIndexerField = AccessTools.Field(typeof(Localization), "namesIndexer");
    private static readonly System.Reflection.FieldInfo? StringsField = AccessTools.Field(typeof(Localization), "strings");
    private static readonly System.Reflection.FieldInfo? FloatsField = AccessTools.Field(typeof(Localization), "floats");

    private static readonly Dictionary<string, int> LanguageIds = new(StringComparer.OrdinalIgnoreCase)
    {
        ["enUS"] = Localization.LCID_ENUS,
        ["zhCN"] = Localization.LCID_ZHCN,
        ["frFR"] = Localization.LCID_FRFR,
        ["deDE"] = Localization.LCID_DEDE,
        ["esES"] = Localization.LCID_ESES,
        ["jaJA"] = Localization.LCID_JAJA,
        ["koKO"] = Localization.LCID_KOKO
    };

    public static void AddKeys()
    {
        var namesIndexer = GetNamesIndexer();
        if (namesIndexer == null)
        {
            return;
        }

        foreach (var key in DspCore.Resources.GetLocalizations().Select(item => item.Key).Distinct(StringComparer.Ordinal))
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
        var entries = DspCore.Resources.GetLocalizations()
            .Where(item => ResolveLanguageId(item.Language) == languageId)
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

    private static int ResolveLanguageId(string language)
    {
        if (int.TryParse(language, out var id))
        {
            return id;
        }

        return LanguageIds.TryGetValue(language, out id) ? id : 0;
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
