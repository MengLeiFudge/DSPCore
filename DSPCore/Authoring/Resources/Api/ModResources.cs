using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace DSPCore;

/// <summary>
/// 作者侧模组资源和本地化短入口。
/// Author-facing short entry point for mod resources and localization.
/// </summary>
public static class ModResources
{
    /// <summary>
    /// 创建一个复用模组 GUID、资源根和程序集的资源包短入口。
    /// Creates a resource-pack short entry that reuses the mod GUID, root path, and assembly.
    /// </summary>
    /// <param name="ownerModGuid">所属模组 GUID。Owner mod GUID.</param>
    /// <param name="rootPath">资源根路径。Resource root path.</param>
    /// <param name="assembly">默认嵌入资源程序集。Default embedded-resource assembly.</param>
    /// <returns>资源包短入口。Resource-pack short entry.</returns>
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static ModResourcePack Pack(string ownerModGuid, string rootPath = "", Assembly? assembly = null)
    {
        return new ModResourcePack(ownerModGuid, rootPath, assembly ?? Assembly.GetCallingAssembly());
    }

    /// <summary>
    /// 注册一个模组资源根。
    /// Registers a mod resource root.
    /// </summary>
    /// <param name="id">资源 ID。Resource id.</param>
    /// <param name="ownerModGuid">所属模组 GUID。Owner mod GUID.</param>
    /// <param name="keyword">资源关键字。Resource keyword.</param>
    /// <param name="rootPath">资源根路径。Resource root path.</param>
    /// <param name="bundleName">可选资源包名称。Optional asset bundle name.</param>
    public static void Root(string id, string ownerModGuid, string keyword, string rootPath, string? bundleName = null)
    {
        Register(new ResourceDescriptor(id, ownerModGuid, keyword, rootPath, bundleName));
    }

    /// <summary>
    /// 注册一个本地化文本。
    /// Registers a localized text entry.
    /// </summary>
    /// <param name="key">本地化键。Localization key.</param>
    /// <param name="language">语言标识。Language id or abbreviation.</param>
    /// <param name="value">翻译文本。Translated text.</param>
    /// <param name="ownerModGuid">所属模组 GUID。Owner mod GUID.</param>
    public static void Text(string key, string language, string value, string ownerModGuid)
    {
        Localization(new LocalizationEntry(key, language, value, ownerModGuid));
    }

    /// <summary>
    /// 注册一个资源描述。
    /// Registers a resource descriptor.
    /// </summary>
    /// <param name="descriptor">资源描述。Resource descriptor.</param>
    public static void Register(ResourceDescriptor descriptor)
    {
        DspCore.Resources.RegisterResource(descriptor);
    }

    /// <summary>
    /// 注册一个本地化文本描述。
    /// Registers a localization descriptor.
    /// </summary>
    /// <param name="entry">本地化文本条目。Localization entry.</param>
    public static void Localization(LocalizationEntry entry)
    {
        DspCore.Resources.RegisterLocalization(entry);
    }

    /// <summary>
    /// 获取所有资源根。
    /// Gets all resource roots.
    /// </summary>
    /// <returns>资源描述集合。Resource descriptor collection.</returns>
    public static IReadOnlyCollection<ResourceDescriptor> GetResources()
    {
        return DspCore.Resources.GetResources();
    }

    /// <summary>
    /// 获取所有本地化文本。
    /// Gets all localization entries.
    /// </summary>
    /// <returns>本地化文本集合。Localization entry collection.</returns>
    public static IReadOnlyList<LocalizationEntry> GetLocalizations()
    {
        return DspCore.Resources.GetLocalizations();
    }
}
