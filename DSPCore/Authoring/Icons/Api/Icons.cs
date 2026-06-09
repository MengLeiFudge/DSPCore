using System.Collections.Generic;
using System.Reflection;

namespace DSPCore;

/// <summary>
/// 作者侧图标注册入口。
/// Author-facing icon registration entry point.
/// </summary>
public static class Icons
{
    /// <summary>
    /// 从 Unity Resources 路径注册一个图标。
    /// Registers an icon from a Unity Resources path.
    /// </summary>
    /// <param name="id">图标 ID。Icon id.</param>
    /// <param name="ownerModGuid">所属模组 GUID。Owner mod GUID.</param>
    /// <param name="resourcesPath">Resources 下的 sprite 路径。Sprite path under Resources.</param>
    /// <param name="fallbackIconId">可选 fallback 图标 ID。Optional fallback icon id.</param>
    public static void FromResources(string id, string ownerModGuid, string resourcesPath, string? fallbackIconId = null)
    {
        Register(new IconDescriptor(id, ownerModGuid, resourcesPath, fallbackIconId));
    }

    /// <summary>
    /// 从本地 PNG 文件注册一个图标。
    /// Registers an icon from a local PNG file.
    /// </summary>
    /// <param name="id">图标 ID。Icon id.</param>
    /// <param name="ownerModGuid">所属模组 GUID。Owner mod GUID.</param>
    /// <param name="filePath">PNG 文件路径。PNG file path.</param>
    /// <param name="fallbackIconId">可选 fallback 图标 ID。Optional fallback icon id.</param>
    public static void FromFile(string id, string ownerModGuid, string filePath, string? fallbackIconId = null)
    {
        Register(new IconDescriptor(id, ownerModGuid, filePath, fallbackIconId));
    }

    /// <summary>
    /// 从程序集嵌入 PNG 资源注册一个图标。
    /// Registers an icon from an embedded PNG resource in an assembly.
    /// </summary>
    /// <param name="id">图标 ID。Icon id.</param>
    /// <param name="ownerModGuid">所属模组 GUID。Owner mod GUID.</param>
    /// <param name="assembly">包含资源的程序集。Assembly containing the resource.</param>
    /// <param name="resourceName">manifest resource name。Manifest resource name.</param>
    /// <param name="fallbackIconId">可选 fallback 图标 ID。Optional fallback icon id.</param>
    public static void FromEmbedded(
        string id,
        string ownerModGuid,
        Assembly assembly,
        string resourceName,
        string? fallbackIconId = null)
    {
        Register(new IconDescriptor(id, ownerModGuid, IconAssetPaths.Embedded(assembly, resourceName), fallbackIconId));
    }

    /// <summary>
    /// 注册一个图标并绑定到目标 Proto。
    /// Registers an icon and binds it to a target proto.
    /// </summary>
    /// <param name="id">图标 ID。Icon id.</param>
    /// <param name="ownerModGuid">所属模组 GUID。Owner mod GUID.</param>
    /// <param name="assetPath">Resources 路径或本地文件路径。Resources path or local file path.</param>
    /// <param name="targetKind">目标 Proto 类型。Target proto kind.</param>
    /// <param name="targetProtoId">目标 Proto ID。Target proto id.</param>
    /// <param name="fallbackIconId">可选 fallback 图标 ID。Optional fallback icon id.</param>
    public static void BindToProto(
        string id,
        string ownerModGuid,
        string assetPath,
        ProtoKind targetKind,
        int targetProtoId,
        string? fallbackIconId = null)
    {
        Register(new IconDescriptor(id, ownerModGuid, assetPath, fallbackIconId, targetKind, targetProtoId));
    }

    /// <summary>
    /// 注册一个嵌入 PNG 图标并绑定到目标 Proto。
    /// Registers an embedded PNG icon and binds it to a target proto.
    /// </summary>
    /// <param name="id">图标 ID。Icon id.</param>
    /// <param name="ownerModGuid">所属模组 GUID。Owner mod GUID.</param>
    /// <param name="assembly">包含资源的程序集。Assembly containing the resource.</param>
    /// <param name="resourceName">manifest resource name。Manifest resource name.</param>
    /// <param name="targetKind">目标 Proto 类型。Target proto kind.</param>
    /// <param name="targetProtoId">目标 Proto ID。Target proto id.</param>
    /// <param name="fallbackIconId">可选 fallback 图标 ID。Optional fallback icon id.</param>
    public static void BindEmbeddedToProto(
        string id,
        string ownerModGuid,
        Assembly assembly,
        string resourceName,
        ProtoKind targetKind,
        int targetProtoId,
        string? fallbackIconId = null)
    {
        Register(new IconDescriptor(
            id,
            ownerModGuid,
            IconAssetPaths.Embedded(assembly, resourceName),
            fallbackIconId,
            targetKind,
            targetProtoId));
    }

    /// <summary>
    /// 注册一个图标描述。
    /// Registers an icon descriptor.
    /// </summary>
    public static void Register(IconDescriptor descriptor)
    {
        DspCore.Icons.Register(descriptor);
    }

    /// <summary>
    /// 尝试按图标 ID 获取图标描述。
    /// Tries to get an icon descriptor by icon id.
    /// </summary>
    public static bool TryGet(string id, out IconDescriptor descriptor)
    {
        return DspCore.Icons.TryGet(id, out descriptor);
    }

    /// <summary>
    /// 获取所有图标描述。
    /// Gets all icon descriptors.
    /// </summary>
    public static IReadOnlyCollection<IconDescriptor> GetAll()
    {
        return DspCore.Icons.GetAll();
    }
}
