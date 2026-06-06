namespace DSPCore;

/// <summary>
/// 描述一个模组资源根。
/// Describes a mod resource root.
/// </summary>
/// <param name="Id">资源 ID。Resource id.</param>
/// <param name="OwnerModGuid">声明方模组 GUID。Declaring mod GUID.</param>
/// <param name="Keyword">资源关键字。Resource keyword.</param>
/// <param name="RootPath">资源根路径。Resource root path.</param>
/// <param name="BundleName">资源包名称。Asset bundle name.</param>
public sealed record ResourceDescriptor(string Id, string OwnerModGuid, string Keyword, string RootPath, string? BundleName = null);
