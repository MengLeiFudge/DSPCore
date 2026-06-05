namespace DSPCore;

/// <summary>
/// 描述一个图标资源。
/// Describes an icon resource.
/// </summary>
/// <param name="Id">图标 ID。Icon id.</param>
/// <param name="OwnerModGuid">声明方模组 GUID。Declaring mod GUID.</param>
/// <param name="AssetPath">资源路径。Asset path.</param>
/// <param name="FallbackIconId">fallback 图标 ID。Fallback icon id.</param>
public sealed record IconDescriptor(string Id, string OwnerModGuid, string AssetPath, string? FallbackIconId = null);
