namespace DSPCore;

/// <summary>
/// 描述一个可重绑定按键。
/// Describes a rebindable key binding.
/// </summary>
/// <param name="Id">按键 ID。Key binding id.</param>
/// <param name="OwnerModGuid">声明方模组 GUID。Declaring mod GUID.</param>
/// <param name="DisplayName">显示名称本地化键或文本。Display name localization key or text.</param>
/// <param name="DefaultKey">默认按键文本。Default key text.</param>
/// <param name="Action">触发方式。Trigger action.</param>
/// <param name="ConflictGroup">冲突组。Conflict group.</param>
/// <param name="CanOverride">玩家是否可以重绑定。Whether players can rebind it.</param>
/// <param name="Callback">触发回调。Trigger callback.</param>
/// <param name="DisplayPageId">可选模组设置页 ID。Optional mod settings page ID.</param>
public sealed record KeyBindDescriptor(
    string Id,
    string OwnerModGuid,
    string DisplayName,
    string DefaultKey,
    CoreKeyAction Action = CoreKeyAction.Press,
    int ConflictGroup = 0,
    bool CanOverride = true,
    System.Action? Callback = null,
    string? DisplayPageId = null);
