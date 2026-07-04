using System.Collections.Generic;

namespace DSPCore;

internal static class LocalizationLanguages
{
    private static readonly Dictionary<string, int> LanguageIds = new(System.StringComparer.OrdinalIgnoreCase)
    {
        ["enUS"] = Localization.LCID_ENUS,
        ["zhCN"] = Localization.LCID_ZHCN,
        ["frFR"] = Localization.LCID_FRFR,
        ["deDE"] = Localization.LCID_DEDE,
        ["esES"] = Localization.LCID_ESES,
        ["jaJA"] = Localization.LCID_JAJA,
        ["koKO"] = Localization.LCID_KOKO
    };

    public static int ResolveId(string language)
    {
        if (int.TryParse(language, out var id))
        {
            return id;
        }

        return LanguageIds.TryGetValue(language, out id) ? id : 0;
    }

    public static string Normalize(string language)
    {
        int id = ResolveId(language);
        return id > 0 ? id.ToString() : (language ?? string.Empty).Trim();
    }
}
