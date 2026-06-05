namespace DSPCore;

/// <summary>
/// 描述一个本地化文本条目。
/// Describes a localization text entry.
/// </summary>
/// <param name="Key">本地化键。Localization key.</param>
/// <param name="Language">语言标识。Language id or abbreviation.</param>
/// <param name="Value">翻译文本。Translated text.</param>
/// <param name="OwnerModGuid">声明方模组 GUID。Declaring mod GUID.</param>
public sealed record LocalizationEntry(string Key, string Language, string Value, string OwnerModGuid);
