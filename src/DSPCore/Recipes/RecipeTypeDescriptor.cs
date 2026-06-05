namespace DSPCore;

using System.Collections.Generic;

/// <summary>
/// 描述一个自定义配方类型。
/// Describes a custom recipe type.
/// </summary>
/// <param name="Id">配方类型 ID。Recipe type id.</param>
/// <param name="OwnerModGuid">声明方模组 GUID。Declaring mod GUID.</param>
/// <param name="DisplayName">显示名称。Display name.</param>
/// <param name="RecipeIds">归属此类型的配方 ID。Recipe ids that belong to this type.</param>
/// <param name="AssemblerItemIds">支持此类型的制作器物品 ID。Assembler item ids that support this type.</param>
public sealed record RecipeTypeDescriptor(
    string Id,
    string OwnerModGuid,
    string DisplayName,
    IReadOnlyCollection<int>? RecipeIds = null,
    IReadOnlyCollection<int>? AssemblerItemIds = null);
