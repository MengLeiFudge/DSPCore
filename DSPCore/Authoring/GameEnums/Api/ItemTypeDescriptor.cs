namespace DSPCore;

using System.Collections.Generic;

/// <summary>
/// 描述一个自定义物品类型。
/// Describes a custom item type.
/// </summary>
/// <param name="Id">物品类型 ID。Item type id.</param>
/// <param name="OwnerModGuid">声明方模组 GUID。Declaring mod GUID.</param>
/// <param name="DisplayName">显示名称。Display name.</param>
/// <param name="ItemIds">归属此类型的物品 ID。Item ids that belong to this type.</param>
public sealed record ItemTypeDescriptor(
    string Id,
    string OwnerModGuid,
    string DisplayName,
    IReadOnlyCollection<int>? ItemIds = null);
