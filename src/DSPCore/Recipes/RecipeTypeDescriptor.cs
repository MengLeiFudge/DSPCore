namespace DSPCore;

/// <summary>
/// 描述一个自定义配方类型。
/// Describes a custom recipe type.
/// </summary>
/// <param name="Id">配方类型 ID。Recipe type id.</param>
/// <param name="OwnerModGuid">声明方模组 GUID。Declaring mod GUID.</param>
/// <param name="DisplayName">显示名称。Display name.</param>
public sealed record RecipeTypeDescriptor(string Id, string OwnerModGuid, string DisplayName);
