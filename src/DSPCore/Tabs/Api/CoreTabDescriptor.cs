namespace DSPCore;

/// <summary>
/// 描述一个 DSPCore 分页。
/// Describes a DSPCore tab.
/// </summary>
/// <param name="Id">分页 ID。Tab id.</param>
/// <param name="OwnerModGuid">声明方模组 GUID。Declaring mod GUID.</param>
/// <param name="Title">分页标题本地化键或文本。Tab title localization key or text.</param>
/// <param name="IconId">分页图标 ID。Tab icon id.</param>
/// <param name="Order">排序值。Sort order.</param>
public sealed record CoreTabDescriptor(string Id, string OwnerModGuid, string Title, string IconId, int Order = 0);
