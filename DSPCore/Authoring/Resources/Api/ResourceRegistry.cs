using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DSPCore;

/// <summary>
/// 管理资源、图标和本地化声明。
/// Manages resource, icon, and localization declarations.
/// </summary>
public sealed class ResourceRegistry
{
    private readonly Dictionary<string, ResourceDescriptor> resources = new(StringComparer.Ordinal);
    private readonly List<LocalizationEntry> localizations = new();
    private readonly Dictionary<string, LocalizationEntry> localizationIndex = new(StringComparer.OrdinalIgnoreCase);
    private readonly List<LocalizationEntry> localizationOverrides = new();
    private readonly Dictionary<string, LocalizationEntry> localizationOverrideIndex = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// 注册一个资源根。
    /// Registers a resource root.
    /// </summary>
    /// <param name="descriptor">资源描述。Resource descriptor.</param>
    public void RegisterResource(ResourceDescriptor descriptor)
    {
        if (descriptor == null)
        {
            throw new ArgumentNullException(nameof(descriptor));
        }

        resources[descriptor.Id] = descriptor;
    }

    /// <summary>
    /// 尝试按资源根 ID 获取资源描述。
    /// Tries to get a resource descriptor by root id.
    /// </summary>
    /// <param name="id">资源根 ID。Resource root id.</param>
    /// <param name="descriptor">资源描述。Resource descriptor.</param>
    /// <returns>找到资源根时返回 true。Returns true when the resource root exists.</returns>
    public bool TryGetResource(string id, out ResourceDescriptor descriptor)
    {
        return resources.TryGetValue(id, out descriptor!);
    }

    /// <summary>
    /// 按资源根 ID 解析相对路径。
    /// Resolves a relative path by resource root id.
    /// </summary>
    /// <param name="resourceId">资源根 ID。Resource root id.</param>
    /// <param name="relativePath">相对路径或绝对路径。Relative or rooted path.</param>
    /// <param name="path">解析后的路径。Resolved path.</param>
    /// <returns>找到资源根时返回 true。Returns true when the resource root exists.</returns>
    public bool TryResolvePath(string resourceId, string relativePath, out string path)
    {
        if (!resources.TryGetValue(resourceId, out var resource))
        {
            path = ResourcePaths.Normalize(relativePath);
            return false;
        }

        path = resource.ResolvePath(relativePath);
        return true;
    }

    /// <summary>
    /// 按模组 GUID 和资源关键字解析相对路径。
    /// Resolves a relative path by owner mod GUID and resource keyword.
    /// </summary>
    /// <param name="ownerModGuid">所属模组 GUID。Owner mod GUID.</param>
    /// <param name="keyword">资源关键字。Resource keyword.</param>
    /// <param name="relativePath">相对路径或绝对路径。Relative or rooted path.</param>
    /// <param name="path">解析后的路径。Resolved path.</param>
    /// <returns>找到匹配资源根时返回 true。Returns true when a matching resource root exists.</returns>
    public bool TryResolvePath(string ownerModGuid, string keyword, string relativePath, out string path)
    {
        var resource = FindResource(ownerModGuid, keyword);
        if (resource == null)
        {
            path = ResourcePaths.Normalize(relativePath);
            return false;
        }

        path = resource.ResolvePath(relativePath);
        return true;
    }

    /// <summary>
    /// 解析路径；没有匹配资源根时返回规范化后的输入路径。
    /// Resolves a path and returns the normalized input when no resource root matches.
    /// </summary>
    /// <param name="ownerModGuid">所属模组 GUID。Owner mod GUID.</param>
    /// <param name="relativePath">相对路径或绝对路径。Relative or rooted path.</param>
    /// <param name="keyword">资源关键字；为空时使用该模组第一个资源根。Resource keyword; first root for the owner is used when omitted.</param>
    /// <returns>解析后的路径。Resolved path.</returns>
    public string ResolvePath(string ownerModGuid, string relativePath, string? keyword = null)
    {
        var resource = FindResource(ownerModGuid, keyword);
        return resource?.ResolvePath(relativePath) ?? ResourcePaths.Normalize(relativePath);
    }

    /// <summary>
    /// 解析资源根的 AssetBundle 路径。
    /// Resolves the asset bundle path for a resource root.
    /// </summary>
    /// <param name="resourceId">资源根 ID。Resource root id.</param>
    /// <param name="bundlePath">解析后的 AssetBundle 路径。Resolved AssetBundle path.</param>
    /// <returns>资源根存在且声明了 bundleName 时返回 true。Returns true when the root exists and declares bundleName.</returns>
    public bool TryResolveBundlePath(string resourceId, out string bundlePath)
    {
        if (!resources.TryGetValue(resourceId, out var resource) || string.IsNullOrWhiteSpace(resource.BundleName))
        {
            bundlePath = string.Empty;
            return false;
        }

        bundlePath = resource.ResolvePath(resource.BundleName!);
        return true;
    }

    /// <summary>
    /// 尝试打开资源文件。
    /// Tries to open a resource file.
    /// </summary>
    /// <param name="ownerModGuid">所属模组 GUID。Owner mod GUID.</param>
    /// <param name="relativePath">相对路径或绝对路径。Relative or rooted path.</param>
    /// <param name="stream">打开的只读文件流。Opened read-only file stream.</param>
    /// <param name="resolvedPath">实际检查的路径。Actual resolved path.</param>
    /// <param name="keyword">资源关键字；为空时先查规范化路径，再查该模组各资源根。Resource keyword; when omitted, normalized path is checked before owner roots.</param>
    /// <returns>文件存在并成功打开时返回 true。Returns true when the file exists and opens successfully.</returns>
    public bool TryOpenRead(string ownerModGuid, string relativePath, out Stream stream, out string resolvedPath, string? keyword = null)
    {
        foreach (var candidate in EnumerateCandidatePaths(ownerModGuid, relativePath, keyword))
        {
            if (!TryResolveExistingFile(candidate, out resolvedPath))
            {
                continue;
            }

            try
            {
                stream = File.Open(resolvedPath, FileMode.Open, FileAccess.Read, FileShare.Read);
                return true;
            }
            catch (IOException)
            {
            }
            catch (UnauthorizedAccessException)
            {
            }
        }

        resolvedPath = ResourcePaths.Normalize(relativePath);
        stream = Stream.Null;
        return false;
    }

    /// <summary>
    /// 尝试读取资源文件 bytes。
    /// Tries to read resource file bytes.
    /// </summary>
    /// <param name="ownerModGuid">所属模组 GUID。Owner mod GUID.</param>
    /// <param name="relativePath">相对路径或绝对路径。Relative or rooted path.</param>
    /// <param name="data">读取到的 bytes。Read bytes.</param>
    /// <param name="resolvedPath">实际读取路径。Actual resolved path.</param>
    /// <param name="keyword">资源关键字；为空时先查规范化路径，再查该模组各资源根。Resource keyword; when omitted, normalized path is checked before owner roots.</param>
    /// <returns>文件存在并成功读取时返回 true。Returns true when the file exists and is read successfully.</returns>
    public bool TryReadBytes(string ownerModGuid, string relativePath, out byte[] data, out string resolvedPath, string? keyword = null)
    {
        if (TryOpenRead(ownerModGuid, relativePath, out var stream, out resolvedPath, keyword))
        {
            using (stream)
            using (var buffer = new MemoryStream())
            {
                stream.CopyTo(buffer);
                data = buffer.ToArray();
                return true;
            }
        }

        data = Array.Empty<byte>();
        return false;
    }

    /// <summary>
    /// 尝试按资源根解析一个已经存在的文件路径。
    /// Tries to resolve an existing file path through resource roots.
    /// </summary>
    /// <param name="ownerModGuid">所属模组 GUID。Owner mod GUID.</param>
    /// <param name="relativePath">相对路径或绝对路径。Relative or rooted path.</param>
    /// <param name="resolvedPath">存在的文件路径。Existing file path.</param>
    /// <param name="keyword">资源关键字；为空时先查规范化路径，再查该模组各资源根。Resource keyword; when omitted, normalized path is checked before owner roots.</param>
    /// <returns>找到存在的文件时返回 true。Returns true when an existing file is found.</returns>
    public bool TryResolveExistingFile(string ownerModGuid, string relativePath, out string resolvedPath, string? keyword = null)
    {
        foreach (var candidate in EnumerateCandidatePaths(ownerModGuid, relativePath, keyword))
        {
            if (TryResolveExistingFile(candidate, out resolvedPath))
            {
                return true;
            }
        }

        resolvedPath = ResourcePaths.Normalize(relativePath);
        return false;
    }

    /// <summary>
    /// 注册一个本地化文本。
    /// Registers a localized text entry.
    /// </summary>
    /// <param name="entry">本地化文本条目。Localization entry.</param>
    public void RegisterLocalization(LocalizationEntry entry)
    {
        if (entry == null)
        {
            throw new ArgumentNullException(nameof(entry));
        }

        localizations.Add(entry);
        localizationIndex[GetLocalizationIndexKey(entry.Key, entry.Language)] = entry;
    }

    internal void RegisterLocalizationOverride(LocalizationEntry entry)
    {
        if (entry == null)
        {
            throw new ArgumentNullException(nameof(entry));
        }

        localizationOverrides.Add(entry);
        localizationOverrideIndex[GetLocalizationIndexKey(entry.Key, entry.Language)] = entry;
    }

    /// <summary>
    /// 立即查询已注册的本地化文本。
    /// Immediately queries a registered localization text.
    /// </summary>
    /// <param name="key">本地化键。Localization key.</param>
    /// <param name="value">翻译文本。Translated text.</param>
    /// <param name="language">语言标识；为空时使用当前游戏语言。Language id; current game language is used when omitted.</param>
    /// <returns>找到文本时返回 true。Returns true when a text is found.</returns>
    public bool TryTranslate(string key, out string value, string? language = null)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            value = string.Empty;
            return false;
        }

        if (TryTranslateCore(key, language, out value))
        {
            return true;
        }

        value = string.Empty;
        return false;
    }

    /// <summary>
    /// 立即查询已注册的本地化文本，找不到时返回 fallback 或 key。
    /// Immediately queries a registered localization text and returns fallback or key when missing.
    /// </summary>
    /// <param name="key">本地化键。Localization key.</param>
    /// <param name="language">语言标识；为空时使用当前游戏语言。Language id; current game language is used when omitted.</param>
    /// <param name="fallback">可选 fallback。Optional fallback.</param>
    /// <returns>翻译文本、fallback 或 key。Translated text, fallback, or key.</returns>
    public string Translate(string key, string? language = null, string? fallback = null)
    {
        return TryTranslate(key, out var value, language) ? value : fallback ?? key;
    }

    /// <summary>
    /// 获取所有资源根。
    /// Gets all resource roots.
    /// </summary>
    /// <returns>资源描述快照。Snapshot of resource descriptors.</returns>
    public IReadOnlyCollection<ResourceDescriptor> GetResources()
    {
        return resources.Values;
    }

    /// <summary>
    /// 获取所有本地化文本。
    /// Gets all localization entries.
    /// </summary>
    /// <returns>本地化文本快照。Snapshot of localization entries.</returns>
    public IReadOnlyList<LocalizationEntry> GetLocalizations()
    {
        return localizations.ToArray();
    }

    internal IReadOnlyList<LocalizationEntry> GetLocalizationOverrides()
    {
        return localizationOverrides.ToArray();
    }

    private bool TryTranslateCore(string key, string? language, out string value)
    {
        string[] languages = ResolveLanguages(language);
        foreach (var candidate in languages)
        {
            if (localizationOverrideIndex.TryGetValue(GetLocalizationIndexKey(key, candidate), out var overrideEntry))
            {
                value = overrideEntry.Value;
                return true;
            }

            if (localizationIndex.TryGetValue(GetLocalizationIndexKey(key, candidate), out var entry))
            {
                value = entry.Value;
                return true;
            }
        }

        value = string.Empty;
        return false;
    }

    private static string[] ResolveLanguages(string? language)
    {
        if (!string.IsNullOrWhiteSpace(language))
        {
            return new[] { language! };
        }

        var current = ResolveCurrentLanguage();
        return string.IsNullOrWhiteSpace(current) ? Array.Empty<string>() : new[] { current };
    }

    private static string ResolveCurrentLanguage()
    {
        if (Localization.CurrentLanguage != null)
        {
            return Localization.CurrentLanguage.lcId.ToString();
        }

        return Localization.CurrentLanguageLCID > 0 ? Localization.CurrentLanguageLCID.ToString() : string.Empty;
    }

    private static string GetLocalizationIndexKey(string key, string language)
    {
        return key + "\u001f" + NormalizeLanguage(language);
    }

    private static string NormalizeLanguage(string language)
    {
        return LocalizationLanguages.Normalize(language);
    }

    private ResourceDescriptor? FindResource(string ownerModGuid, string? keyword)
    {
        return resources.Values.FirstOrDefault(item =>
            string.Equals(item.OwnerModGuid, ownerModGuid, StringComparison.Ordinal)
            && (string.IsNullOrWhiteSpace(keyword) || string.Equals(item.Keyword, keyword, StringComparison.Ordinal)));
    }

    private IEnumerable<string> EnumerateCandidatePaths(string ownerModGuid, string relativePath, string? keyword)
    {
        var normalized = ResourcePaths.Normalize(relativePath);
        yield return normalized;

        IEnumerable<ResourceDescriptor> candidates = resources.Values.Where(item =>
            string.Equals(item.OwnerModGuid, ownerModGuid, StringComparison.Ordinal)
            && (string.IsNullOrWhiteSpace(keyword) || string.Equals(item.Keyword, keyword, StringComparison.Ordinal)));

        foreach (var resource in candidates)
        {
            var path = resource.ResolvePath(relativePath);
            if (!string.Equals(path, normalized, StringComparison.Ordinal))
            {
                yield return path;
            }
        }
    }

    private static bool TryResolveExistingFile(string path, out string resolvedPath)
    {
        resolvedPath = ResourcePaths.Normalize(path);
        try
        {
            return File.Exists(resolvedPath);
        }
        catch (IOException)
        {
            return false;
        }
        catch (UnauthorizedAccessException)
        {
            return false;
        }
    }
}
