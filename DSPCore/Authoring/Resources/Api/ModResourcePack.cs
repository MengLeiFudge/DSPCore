using System;
using System.Linq;
using System.Reflection;

namespace DSPCore;

/// <summary>
/// 封装同一模组的资源根、程序集和归属信息。
/// Wraps one mod's resource root, assembly, and ownership metadata.
/// </summary>
public sealed class ModResourcePack
{
    private readonly Assembly defaultAssembly;

    internal ModResourcePack(string ownerModGuid, string rootPath, Assembly assembly)
    {
        OwnerModGuid = ownerModGuid;
        RootPath = Normalize(rootPath);
        defaultAssembly = assembly;
    }

    /// <summary>
    /// 所属模组 GUID。
    /// Owner mod GUID.
    /// </summary>
    public string OwnerModGuid { get; }

    /// <summary>
    /// 资源根路径。
    /// Resource root path.
    /// </summary>
    public string RootPath { get; }

    /// <summary>
    /// 默认嵌入资源程序集。
    /// Default assembly for embedded resources.
    /// </summary>
    public Assembly Assembly => defaultAssembly;

    /// <summary>
    /// 登记这个资源包的根路径。
    /// Registers this pack's root path.
    /// </summary>
    /// <param name="id">资源根 ID。Resource root id.</param>
    /// <param name="keyword">资源关键字。Resource keyword.</param>
    /// <param name="bundleName">可选资源包名称。Optional asset bundle name.</param>
    public void Root(string id, string keyword, string? bundleName = null)
    {
        ModResources.Root(id, OwnerModGuid, keyword, RootPath, bundleName);
    }

    /// <summary>
    /// 注册一个属于这个资源包的本地化文本。
    /// Registers a localized text entry owned by this pack.
    /// </summary>
    /// <param name="key">本地化键。Localization key.</param>
    /// <param name="language">语言标识。Language id or abbreviation.</param>
    /// <param name="value">翻译文本。Translated text.</param>
    public void Text(string key, string language, string value)
    {
        ModResources.Text(key, language, value, OwnerModGuid);
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
        return ModResources.TryTranslate(key, out value, language);
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
        return ModResources.Translate(key, language, fallback);
    }

    /// <summary>
    /// 从这个资源包下的 Unity Resources 路径注册图标。
    /// Registers an icon from a Unity Resources path under this pack.
    /// </summary>
    public void IconFromResources(string id, string resourcesPath, string? fallbackIconId = null)
    {
        Icons.FromResources(id, OwnerModGuid, ResolvePath(resourcesPath), fallbackIconId);
    }

    /// <summary>
    /// 从这个资源包下的本地 PNG 文件注册图标。
    /// Registers an icon from a local PNG file under this pack.
    /// </summary>
    public void IconFromFile(string id, string filePath, string? fallbackIconId = null)
    {
        Icons.FromFile(id, OwnerModGuid, ResolvePath(filePath), fallbackIconId);
    }

    /// <summary>
    /// 从这个资源包的默认程序集注册嵌入 PNG 图标。
    /// Registers an embedded PNG icon from this pack's default assembly.
    /// </summary>
    public void IconFromEmbedded(string id, string resourceName, string? fallbackIconId = null)
    {
        Icons.FromEmbedded(id, OwnerModGuid, defaultAssembly, resourceName, fallbackIconId);
    }

    /// <summary>
    /// 从指定程序集注册嵌入 PNG 图标。
    /// Registers an embedded PNG icon from a specific assembly.
    /// </summary>
    public void IconFromEmbedded(string id, Assembly assembly, string resourceName, string? fallbackIconId = null)
    {
        Icons.FromEmbedded(id, OwnerModGuid, assembly, resourceName, fallbackIconId);
    }

    /// <summary>
    /// 从这个资源包下的 AssetBundle 注册图标。
    /// Registers an icon from an AssetBundle under this pack.
    /// </summary>
    public void IconFromAssetBundle(string id, string bundlePath, string assetName, string? fallbackIconId = null)
    {
        Icons.FromAssetBundle(id, OwnerModGuid, ResolvePath(bundlePath), assetName, fallbackIconId);
    }

    /// <summary>
    /// 注册这个资源包下的图标并绑定到目标 Proto。
    /// Registers an icon under this pack and binds it to a target proto.
    /// </summary>
    public void BindIconToProto(
        string id,
        string assetPath,
        ProtoKind targetKind,
        int targetProtoId,
        string? fallbackIconId = null)
    {
        Icons.BindToProto(id, OwnerModGuid, ResolvePath(assetPath), targetKind, targetProtoId, fallbackIconId);
    }

    /// <summary>
    /// 注册这个资源包下的图标并绑定到物品 Proto。
    /// Registers an icon under this pack and binds it to an item proto.
    /// </summary>
    /// <param name="id">图标 ID。Icon id.</param>
    /// <param name="assetPath">资源路径。Asset path.</param>
    /// <param name="itemId">物品 ID。Item id.</param>
    /// <param name="fallbackIconId">可选 fallback 图标 ID。Optional fallback icon id.</param>
    public void ItemIcon(string id, string assetPath, int itemId, string? fallbackIconId = null)
    {
        BindIconToProto(id, assetPath, ProtoKind.Item, itemId, fallbackIconId);
    }

    /// <summary>
    /// 注册这个资源包下的图标并绑定到配方 Proto。
    /// Registers an icon under this pack and binds it to a recipe proto.
    /// </summary>
    /// <param name="id">图标 ID。Icon id.</param>
    /// <param name="assetPath">资源路径。Asset path.</param>
    /// <param name="recipeId">配方 ID。Recipe id.</param>
    /// <param name="fallbackIconId">可选 fallback 图标 ID。Optional fallback icon id.</param>
    public void RecipeIcon(string id, string assetPath, int recipeId, string? fallbackIconId = null)
    {
        BindIconToProto(id, assetPath, ProtoKind.Recipe, recipeId, fallbackIconId);
    }

    /// <summary>
    /// 注册这个资源包下的图标并绑定到科技 Proto。
    /// Registers an icon under this pack and binds it to a tech proto.
    /// </summary>
    /// <param name="id">图标 ID。Icon id.</param>
    /// <param name="assetPath">资源路径。Asset path.</param>
    /// <param name="techId">科技 ID。Tech id.</param>
    /// <param name="fallbackIconId">可选 fallback 图标 ID。Optional fallback icon id.</param>
    public void TechIcon(string id, string assetPath, int techId, string? fallbackIconId = null)
    {
        BindIconToProto(id, assetPath, ProtoKind.Tech, techId, fallbackIconId);
    }

    /// <summary>
    /// 注册这个资源包下的图标并绑定到指引或教程 Proto。
    /// Registers an icon under this pack and binds it to a tutorial or guide proto.
    /// </summary>
    /// <param name="id">图标 ID。Icon id.</param>
    /// <param name="assetPath">资源路径。Asset path.</param>
    /// <param name="tutorialId">指引或教程 ID。Tutorial or guide id.</param>
    /// <param name="fallbackIconId">可选 fallback 图标 ID。Optional fallback icon id.</param>
    public void TutorialIcon(string id, string assetPath, int tutorialId, string? fallbackIconId = null)
    {
        BindIconToProto(id, assetPath, ProtoKind.Tutorial, tutorialId, fallbackIconId);
    }

    /// <summary>
    /// 注册这个资源包下的图标并绑定到信号 Proto。
    /// Registers an icon under this pack and binds it to a signal proto.
    /// </summary>
    /// <param name="id">图标 ID。Icon id.</param>
    /// <param name="assetPath">资源路径。Asset path.</param>
    /// <param name="signalId">信号 ID。Signal id.</param>
    /// <param name="fallbackIconId">可选 fallback 图标 ID。Optional fallback icon id.</param>
    public void SignalIcon(string id, string assetPath, int signalId, string? fallbackIconId = null)
    {
        BindIconToProto(id, assetPath, ProtoKind.Signal, signalId, fallbackIconId);
    }

    /// <summary>
    /// 注册这个资源包默认程序集中的嵌入 PNG 图标并绑定到目标 Proto。
    /// Registers an embedded PNG icon from this pack's default assembly and binds it to a target proto.
    /// </summary>
    public void BindEmbeddedIconToProto(
        string id,
        string resourceName,
        ProtoKind targetKind,
        int targetProtoId,
        string? fallbackIconId = null)
    {
        Icons.BindEmbeddedToProto(id, OwnerModGuid, defaultAssembly, resourceName, targetKind, targetProtoId, fallbackIconId);
    }

    /// <summary>
    /// 注册指定程序集中的嵌入 PNG 图标并绑定到目标 Proto。
    /// Registers an embedded PNG icon from a specific assembly and binds it to a target proto.
    /// </summary>
    public void BindEmbeddedIconToProto(
        string id,
        Assembly assembly,
        string resourceName,
        ProtoKind targetKind,
        int targetProtoId,
        string? fallbackIconId = null)
    {
        Icons.BindEmbeddedToProto(id, OwnerModGuid, assembly, resourceName, targetKind, targetProtoId, fallbackIconId);
    }

    /// <summary>
    /// 注册这个资源包下的 AssetBundle 图标并绑定到目标 Proto。
    /// Registers an AssetBundle icon under this pack and binds it to a target proto.
    /// </summary>
    public void BindAssetBundleIconToProto(
        string id,
        string bundlePath,
        string assetName,
        ProtoKind targetKind,
        int targetProtoId,
        string? fallbackIconId = null)
    {
        Icons.BindAssetBundleToProto(
            id,
            OwnerModGuid,
            ResolvePath(bundlePath),
            assetName,
            targetKind,
            targetProtoId,
            fallbackIconId);
    }

    /// <summary>
    /// 把相对路径解析到资源包根路径下。
    /// Resolves a relative path under this pack's root path.
    /// </summary>
    public string ResolvePath(string relativePath)
    {
        var registered = DspCore.Resources.GetResources()
            .FirstOrDefault(item =>
                string.Equals(item.OwnerModGuid, OwnerModGuid, StringComparison.Ordinal)
                && string.Equals(Normalize(item.RootPath), RootPath, StringComparison.Ordinal));
        return registered?.ResolvePath(relativePath) ?? ResourcePaths.Combine(RootPath, relativePath);
    }

    private static string Normalize(string path)
    {
        return ResourcePaths.Normalize(path);
    }
}
